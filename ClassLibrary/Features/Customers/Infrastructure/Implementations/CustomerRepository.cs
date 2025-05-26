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
    /// Repository til håndtering af kundedata.
    /// </summary>
    public class CustomerRepository : Repository<Customer>, ICustomerRepository
    {
        private static readonly Regex EmailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);
        // Simpelt regex for dansk telefonnummer (kan være mere avanceret)
        private static readonly Regex PhoneRegex = new Regex(@"^(\+45\s?)?\d{8}$", RegexOptions.Compiled); 
        private static readonly Regex PostalCodeRegex = new Regex(@"^\d{4}$", RegexOptions.Compiled);

        /// <summary>
        /// Initialiserer en ny instans af <see cref="CustomerRepository"/> klassen.
        /// </summary>
        public CustomerRepository() : base(Path.Combine(JsonDataInitializer.CalculatedWorkspaceRoot, "Data", "Json", "customers.json"))
        {
        }

        /// <summary>
        /// Validerer en kundeentitet.
        /// </summary>
        /// <param name="entity">Kunden der skal valideres.</param>
        /// <exception cref="ArgumentException">Kastes hvis påkrævede felter (Fornavn, Efternavn, Email, Telefon, Adresse, Postnummer, By) er tomme eller har ugyldigt format.</exception>
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

        /// <summary>
        /// Tilføjer en ny kunde asynkront efter validering og tjek for unik email.
        /// </summary>
        /// <param name="customer">Kunden der skal tilføjes.</param>
        /// <returns>Den tilføjede kunde.</returns>
        /// <exception cref="ArgumentNullException">Kastes hvis customer er null.</exception>
        /// <exception cref="RepositoryException">Kastes hvis en aktiv kunde med samme email allerede eksisterer.</exception>
        public override async Task<Customer> AddAsync(Customer customer)
        {   
            ValidateEntity(customer); // Valider først
            var items = await LoadDataAsync(); // Load data for at tjekke unik email

            if (items.Any(c => !c.IsDeleted && c.Email.Equals(customer.Email, StringComparison.OrdinalIgnoreCase)))
                throw new RepositoryException($"En aktiv kunde med email {customer.Email} eksisterer allerede");

            return await base.AddAsync(customer); // base.AddAsync håndterer IsDeleted = false etc.
        }

        /// <summary>
        /// Opdaterer en eksisterende kunde asynkront efter validering og tjek for unik email (hvis ændret).
        /// </summary>
        /// <param name="customer">Kunden med de opdaterede værdier.</param>
        /// <returns>Den opdaterede kunde.</returns>
        /// <exception cref="ArgumentNullException">Kastes hvis customer er null.</exception>
        /// <exception cref="KeyNotFoundException">Kastes hvis ingen aktiv kunde findes med det angivne ID.</exception>
        /// <exception cref="RepositoryException">Kastes hvis en anden aktiv kunde med den nye email allerede eksisterer.</exception>
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

        /// <summary>
        /// Sletter en kunde (soft delete) asynkront. 
        /// Bemærk: Tjek for aktive adoptioner bør foretages i et service-lag før kald til denne metode.
        /// </summary>
        /// <param name="id">ID på kunden der skal slettes.</param>
        /// <returns>En opgave der repræsenterer den asynkrone sletteoperation.</returns>
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

        /// <summary>
        /// Henter kunder baseret på navn (fornavn, efternavn eller fulde navn).
        /// </summary>
        /// <param name="name">Navnet eller en del af navnet der søges efter (case-insensitive).</param>
        /// <returns>En samling af kunder, der matcher navnet. Returnerer en tom samling, hvis navnet er tomt eller null.</returns>
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

        /// <summary>
        /// Henter en kunde baseret på email.
        /// </summary>
        /// <param name="email">Email der søges efter (case-insensitive, eksakt matchning efter validering).</param>
        /// <returns>Kunden der matcher emailen, eller null hvis ingen findes eller emailen er ugyldig.</returns>
        public async Task<Customer?> GetByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email) || !EmailRegex.IsMatch(email))
            {
                return null;
            }
            var customers = await base.FindAsync(c => !string.IsNullOrWhiteSpace(c.Email) && c.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
            return customers.FirstOrDefault();
        }

        /// <summary>
        /// Henter kunder baseret på telefonnummer (kun cifre sammenlignes).
        /// </summary>
        /// <param name="phone">Telefonnummeret der søges efter.</param>
        /// <returns>En samling af kunder, der matcher telefonnummeret (delvis matchning af cifre). Returnerer en tom samling, hvis telefonnummeret er tomt eller kun indeholder ikke-cifre.</returns>
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

        /// <summary>
        /// Henter kunder baseret på adresse.
        /// </summary>
        /// <param name="address">Adressen eller en del af adressen der søges efter (case-insensitive).</param>
        /// <returns>En samling af kunder, der matcher adressen. Returnerer en tom samling, hvis adressen er tom eller null.</returns>
        public async Task<IEnumerable<Customer>> GetByAddressAsync(string address)
        {
            if (string.IsNullOrWhiteSpace(address))
            {
                return Enumerable.Empty<Customer>();
            }
            return await base.FindAsync(c => !string.IsNullOrWhiteSpace(c.Address) && c.Address.Contains(address, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Henter kunder baseret på postnummer.
        /// </summary>
        /// <param name="postalCode">Postnummeret der søges efter (eksakt matchning efter validering).</param>
        /// <returns>En samling af kunder, der matcher postnummeret. Returnerer en tom samling, hvis postnummeret er ugyldigt, tomt eller null.</returns>
        public async Task<IEnumerable<Customer>> GetByPostalCodeAsync(string postalCode)
        {
            if (string.IsNullOrWhiteSpace(postalCode) || !PostalCodeRegex.IsMatch(postalCode))
            {
                return Enumerable.Empty<Customer>();
            }
            return await base.FindAsync(c => !string.IsNullOrWhiteSpace(c.PostalCode) && c.PostalCode.Equals(postalCode));
        }

        /// <summary>
        /// Henter kunder baseret på by.
        /// </summary>
        /// <param name="city">Byen eller en del af byen der søges efter (case-insensitive).</param>
        /// <returns>En samling af kunder, der matcher byen. Returnerer en tom samling, hvis byen er tom eller null.</returns>
        public async Task<IEnumerable<Customer>> GetByCityAsync(string city)
        {
            if (string.IsNullOrWhiteSpace(city))
            {
                return Enumerable.Empty<Customer>();
            }
            return await base.FindAsync(c => !string.IsNullOrWhiteSpace(c.City) && c.City.Contains(city, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Henter kunder registreret inden for et specificeret datointerval.
        /// </summary>
        /// <param name="startDate">Startdato for intervallet.</param>
        /// <param name="endDate">Slutdato for intervallet.</param>
        /// <returns>En samling af kunder registreret inden for det angivne datointerval.</returns>
        /// <exception cref="ArgumentException">Kastes hvis startdato er efter slutdato.</exception>
        public async Task<IEnumerable<Customer>> GetByRegistrationDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
                throw new ArgumentException("Startdato skal være før slutdato");
            return await base.FindAsync(c => c.RegistrationDate.Date >= startDate.Date && c.RegistrationDate.Date <= endDate.Date);
        }

        /// <summary>
        /// Henter kunder med aktive adoptioner. 
        /// Bemærk: Denne metode er pt. en stub og returnerer en tom liste, da adoptionsdata ikke er direkte tilgængelig i Customer JSON-filen.
        /// </summary>
        /// <returns>En tom samling af kunder (stub implementering).</returns>
        public async Task<IEnumerable<Customer>> GetCustomersWithActiveAdoptionsAsync()
        {
            Console.WriteLine("Advarsel: GetCustomersWithActiveAdoptionsAsync i CustomerRepository er en stub pga. manglende adoptionsdata i Customer JSON.");
            return await Task.FromResult(Enumerable.Empty<Customer>());
        }
    }
} 