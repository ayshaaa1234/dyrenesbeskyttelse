using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using ClassLibrary.Features.Customers.Core.Models; // Opdateret for Customer
using ClassLibrary.Features.Adoptions.Core.Models; // Opdateret for Adoption
using ClassLibrary.Features.Adoptions.Core.Enums; // For AdoptionStatus
using ClassLibrary.Features.Customers.Infrastructure.Abstractions; // Opdateret for ICustomerRepository
using ClassLibrary.SharedKernel.Persistence.Implementations; // For Repository<T>
using ClassLibrary.SharedKernel.Exceptions; // For RepositoryException, hvis nødvendigt
using ClassLibrary.Infrastructure.DataInitialization; // Tilføjet for JsonDataInitializer

namespace ClassLibrary.Features.Customers.Infrastructure.Implementations // Opdateret namespace
{
    /// <summary>
    /// Repository til håndtering af kunder
    /// </summary>
    public class CustomerRepository : Repository<Customer>, ICustomerRepository
    {
        private static readonly Regex EmailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);
        // Simpelt regex for dansk telefonnummer (kan være mere avanceret)
        private static readonly Regex PhoneRegex = new Regex(@"^(\+45\s?)?\d{8}$", RegexOptions.Compiled); 
        private static readonly Regex PostalCodeRegex = new Regex(@"^\d{4}$", RegexOptions.Compiled);

        public CustomerRepository() : base(Path.Combine(JsonDataInitializer.CalculatedWorkspaceRoot, "Data", "Json", "customers.json"))
        {
        }

        protected override void ValidateEntity(Customer entity)
        {
            base.ValidateEntity(entity); // Kalder basisklassens null-check

            if (string.IsNullOrWhiteSpace(entity.FirstName))
                throw new ArgumentException("Fornavn kan ikke være tomt", nameof(entity.FirstName));
            if (string.IsNullOrWhiteSpace(entity.LastName))
                throw new ArgumentException("Efternavn kan ikke være tomt", nameof(entity.LastName));
            if (string.IsNullOrWhiteSpace(entity.Email))
                throw new ArgumentException("Email kan ikke være tom", nameof(entity.Email));
            if (!EmailRegex.IsMatch(entity.Email))
                throw new ArgumentException("Ugyldigt email format", nameof(entity.Email));
            if (string.IsNullOrWhiteSpace(entity.Phone))
                throw new ArgumentException("Telefonnummer kan ikke være tomt", nameof(entity.Phone));
            if (!PhoneRegex.IsMatch(entity.Phone.Replace(" ", "")))
                throw new ArgumentException("Ugyldigt telefonnummer format. Forventer 8 cifre, evt. med +45.", nameof(entity.Phone));
            if (string.IsNullOrWhiteSpace(entity.Address))
                throw new ArgumentException("Adresse kan ikke være tom", nameof(entity.Address));
            if (string.IsNullOrWhiteSpace(entity.PostalCode))
                throw new ArgumentException("Postnummer kan ikke være tomt", nameof(entity.PostalCode));
            if (!PostalCodeRegex.IsMatch(entity.PostalCode))
                throw new ArgumentException("Ugyldigt postnummer format. Forventer 4 cifre.", nameof(entity.PostalCode));
            if (string.IsNullOrWhiteSpace(entity.City))
                throw new ArgumentException("By kan ikke være tom", nameof(entity.City));
        }

        public override async Task<Customer> AddAsync(Customer customer)
        {   
            ValidateEntity(customer); // Valider først
            var items = await LoadDataAsync(); // Load data for at tjekke unik email

            if (items.Any(c => !c.IsDeleted && c.Email.Equals(customer.Email, StringComparison.OrdinalIgnoreCase)))
                throw new RepositoryException($"En aktiv kunde med email {customer.Email} eksisterer allerede");

            return await base.AddAsync(customer); // base.AddAsync håndterer IsDeleted = false etc.
        }

        public override async Task<Customer> UpdateAsync(Customer customer)
        {
            ValidateEntity(customer); // Valider først
            var items = await LoadDataAsync(); 

            var existingCustomer = items.FirstOrDefault(c => c.Id == customer.Id && !c.IsDeleted);
            if (existingCustomer == null)
                throw new KeyNotFoundException($"Ingen aktiv kunde fundet med ID: {customer.Id} for opdatering.");

            if (!existingCustomer.Email.Equals(customer.Email, StringComparison.OrdinalIgnoreCase) &&
                items.Any(c => c.Id != customer.Id && !c.IsDeleted && c.Email.Equals(customer.Email, StringComparison.OrdinalIgnoreCase)))
                throw new RepositoryException($"En anden aktiv kunde med email {customer.Email} eksisterer allerede");

            // Bevar oprindelig registreringsdato. Andre properties (IsDeleted, DeletedAt) håndteres af base.UpdateAsync via GetByIdAsync
            customer.RegistrationDate = existingCustomer.RegistrationDate; 
            
            return await base.UpdateAsync(customer);
        }

        public override async Task DeleteAsync(int id)
        { 
            // Dette tjek bør flyttes til et service-lag, da Customer.Adoptions ikke vil være populære her.
            // var customer = await GetByIdAsync(id); 
            // if (customer == null) 
            //     throw new KeyNotFoundException($"Ingen aktiv kunde fundet med ID: {id} for sletning.");
            // if (customer.Adoptions != null && customer.Adoptions.Any(a => !a.IsDeleted && a.Status != AdoptionStatus.Completed && a.Status != AdoptionStatus.Rejected))
            //     throw new RepositoryException("Kan ikke slette kunde med aktive (ikke-afsluttede/afviste) adoptioner. Dette tjek bør flyttes til et service-lag.");

            await base.DeleteAsync(id); 
        }

        public async Task<IEnumerable<Customer>> GetByNameAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return Enumerable.Empty<Customer>();
            }
            return await base.FindAsync(c => 
                (!string.IsNullOrWhiteSpace(c.FirstName) && c.FirstName.Contains(name, StringComparison.OrdinalIgnoreCase)) ||
                (!string.IsNullOrWhiteSpace(c.LastName) && c.LastName.Contains(name, StringComparison.OrdinalIgnoreCase)) ||
                (!string.IsNullOrWhiteSpace(c.FullName) && c.FullName.Contains(name, StringComparison.OrdinalIgnoreCase))
            );
        }

        public async Task<Customer?> GetByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email) || !EmailRegex.IsMatch(email))
            {
                return null;
            }
            var customers = await base.FindAsync(c => !string.IsNullOrWhiteSpace(c.Email) && c.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
            return customers.FirstOrDefault();
        }

        public async Task<IEnumerable<Customer>> GetByPhoneAsync(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
            {
                return Enumerable.Empty<Customer>();
            }
            
            var normalizedInputPhone = new string(phone.Where(char.IsDigit).ToArray());
            if (string.IsNullOrWhiteSpace(normalizedInputPhone))
            {
                 return Enumerable.Empty<Customer>();
            }

            return await base.FindAsync(c => 
                !string.IsNullOrWhiteSpace(c.Phone) && 
                new string(c.Phone.Where(char.IsDigit).ToArray()).Contains(normalizedInputPhone)
            );
        }

        public async Task<IEnumerable<Customer>> GetByAddressAsync(string address)
        {
            if (string.IsNullOrWhiteSpace(address))
            {
                return Enumerable.Empty<Customer>();
            }
            return await base.FindAsync(c => !string.IsNullOrWhiteSpace(c.Address) && c.Address.Contains(address, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<IEnumerable<Customer>> GetByPostalCodeAsync(string postalCode)
        {
            if (string.IsNullOrWhiteSpace(postalCode) || !PostalCodeRegex.IsMatch(postalCode))
            {
                return Enumerable.Empty<Customer>();
            }
            return await base.FindAsync(c => !string.IsNullOrWhiteSpace(c.PostalCode) && c.PostalCode.Equals(postalCode));
        }

        public async Task<IEnumerable<Customer>> GetByCityAsync(string city)
        {
            if (string.IsNullOrWhiteSpace(city))
            {
                return Enumerable.Empty<Customer>();
            }
            return await base.FindAsync(c => !string.IsNullOrWhiteSpace(c.City) && c.City.Contains(city, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<IEnumerable<Customer>> GetByRegistrationDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
                throw new ArgumentException("Startdato skal være før slutdato");
            return await base.FindAsync(c => c.RegistrationDate.Date >= startDate.Date && c.RegistrationDate.Date <= endDate.Date);
        }

        public async Task<IEnumerable<Customer>> GetCustomersWithActiveAdoptionsAsync()
        {
            Console.WriteLine("Advarsel: GetCustomersWithActiveAdoptionsAsync i CustomerRepository er en stub pga. manglende adoptionsdata i Customer JSON.");
            return await Task.FromResult(Enumerable.Empty<Customer>());
        }
    }
} 