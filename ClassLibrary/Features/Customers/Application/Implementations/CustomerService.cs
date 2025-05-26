using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ClassLibrary.Features.Customers.Core.Models;
using ClassLibrary.Features.Customers.Application.Abstractions;
using ClassLibrary.Features.Customers.Infrastructure.Abstractions;
using ClassLibrary.Features.Adoptions.Infrastructure.Abstractions;
using ClassLibrary.Features.Adoptions.Core.Enums;
using ClassLibrary.SharedKernel.Application.Implementations; // For BaseUserService
using ClassLibrary.SharedKernel.Exceptions; // For RepositoryException

namespace ClassLibrary.Features.Customers.Application.Implementations
{
    public class CustomerService : BaseUserService<Customer>, ICustomerService
    {
        // _repository er allerede defineret i BaseUserService som IRepository<Customer>.
        // Vi har brug for en castet version for at tilgå ICustomerRepository specifikke metoder.
        private readonly ICustomerRepository _customerRepository;
        private readonly IAdoptionRepository _adoptionRepository;

        private static readonly Regex PostalCodeRegex = new Regex(@"^[0-9]{4}$", RegexOptions.Compiled);

        public CustomerService(ICustomerRepository customerRepository, IAdoptionRepository adoptionRepository) : base(customerRepository)
        {
            _customerRepository = customerRepository;
            _adoptionRepository = adoptionRepository;
        }

        // Implementering af abstrakte metoder fra BaseUserService
        public override async Task<IEnumerable<Customer>> GetByNameAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Navn kan ikke være tomt.", nameof(name));
            return await _customerRepository.GetByNameAsync(name);
        }

        public override async Task<Customer?> GetByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email kan ikke være tom.", nameof(email));
            if (!EmailRegex.IsMatch(email)) // EmailRegex er protected static i BaseUserService
                throw new ArgumentException("Ugyldigt email format.", nameof(email));
            return await _customerRepository.GetByEmailAsync(email);
        }

        public override async Task<IEnumerable<Customer>> GetByPhoneAsync(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                throw new ArgumentException("Telefonnummer kan ikke være tomt.", nameof(phone));
            // Yderligere validering af phone format kan ske her eller i repository
            return await _customerRepository.GetByPhoneAsync(phone);
        }

        public override async Task<IEnumerable<Customer>> GetByRegistrationDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
                throw new ArgumentException("Startdato skal være før slutdato.");
            return await _customerRepository.GetByRegistrationDateRangeAsync(startDate, endDate);
        }

        public override async Task<IEnumerable<Customer>> SearchAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return new List<Customer>(); // Eller kast ArgumentException

            // Simpel søgning - kan udvides. Kombinerer søgning på flere felter.
            var byName = await _customerRepository.GetByNameAsync(searchTerm);
            var byEmailCustomer = await _customerRepository.GetByEmailAsync(searchTerm);
            var byPhone = await _customerRepository.GetByPhoneAsync(searchTerm);
            var byAddress = await _customerRepository.GetByAddressAsync(searchTerm);
            var byCity = await _customerRepository.GetByCityAsync(searchTerm);

            var results = new List<Customer>();
            results.AddRange(byName);
            if (byEmailCustomer != null) results.Add(byEmailCustomer);
            results.AddRange(byPhone);
            results.AddRange(byAddress);
            results.AddRange(byCity);

            return results.Distinct(new CustomerComparer()); // Undgå duplikater
        }

        // Override CreateAsync og UpdateAsync for at tilføje unik email logik
        public override async Task<Customer> CreateAsync(Customer customer)
        {
            if (customer == null) throw new ArgumentNullException(nameof(customer));
            ValidateUser(customer); // Inkluderer base.ValidateUser + customer specifik

            var existingByEmail = await _customerRepository.GetByEmailAsync(customer.Email);
            if (existingByEmail != null && !existingByEmail.IsDeleted) // Tjek kun mod aktive kunder
            {
                throw new RepositoryException($"En aktiv kunde med emailen '{customer.Email}' eksisterer allerede.");
            }
            // BaseUser constructor sætter RegistrationDate til UtcNow
            return await base.CreateAsync(customer); // Kalder BaseUserService.CreateAsync, som kalder _repository.AddAsync()
        }

        public override async Task<Customer> UpdateAsync(Customer customer)
        {
            if (customer == null) throw new ArgumentNullException(nameof(customer));
            ValidateUser(customer);

            var existingCustomer = await _customerRepository.GetByIdAsync(customer.Id);
            if (existingCustomer == null || existingCustomer.IsDeleted)
            {
                throw new KeyNotFoundException($"Ingen aktiv kunde fundet med ID: {customer.Id} til opdatering.");
            }

            // Hvis email ændres, tjek for unikhed for *andre* aktive kunder
            if (!existingCustomer.Email.Equals(customer.Email, StringComparison.OrdinalIgnoreCase))
            {
                var conflictingCustomer = await _customerRepository.GetByEmailAsync(customer.Email);
                if (conflictingCustomer != null && conflictingCustomer.Id != customer.Id && !conflictingCustomer.IsDeleted)
                {
                    throw new RepositoryException($"En anden aktiv kunde med emailen '{customer.Email}' eksisterer allerede.");
                }
            }
            // Bevar oprindelig registreringsdato. BaseUserService.UpdateAsync vil gøre dette.
            return await base.UpdateAsync(customer); // Kalder BaseUserService.UpdateAsync
        }

        public override async Task DeleteAsync(int id)
        {
            // Tjek om kunden eksisterer og ikke allerede er slettet
            var customer = await _customerRepository.GetByIdAsync(id);
            if (customer == null || customer.IsDeleted)
            {
                // Kunden findes ikke eller er allerede "slettet" (soft delete).
                // Da metoden er void-returnerende, gør vi intet yderligere.
                return; 
            }

            // Tjek for aktive adoptioner
            var customerAdoptions = await _adoptionRepository.GetByCustomerIdAsync(id);
            if (customerAdoptions != null && customerAdoptions.Any(a => a.Status == AdoptionStatus.Approved)) // Brug Approved her også
            {
                // Kunden har aktive (Approved) adoptioner, så sletning er ikke tilladt.
                // Overvej at kaste en specifik exception her, f.eks. CustomerHasActiveAdoptionsException
                // For nu kaster vi en generel InvalidOperationException
                throw new InvalidOperationException($"Kunde med ID {id} kan ikke slettes, da der er aktive adoptioner tilknyttet.");
            }

            // Ingen aktive adoptioner, fortsæt med sletning (soft delete via BaseUserService)
            await base.DeleteAsync(id); // Kald base-metoden
        }

        // Implementering af ICustomerService specifikke metoder
        public async Task<IEnumerable<Customer>> GetCustomersByAddressAsync(string address)
        {
            if (string.IsNullOrWhiteSpace(address))
                throw new ArgumentException("Adresse kan ikke være tom", nameof(address));
            return await _customerRepository.GetByAddressAsync(address);
        }

        public async Task<IEnumerable<Customer>> GetCustomersByPostalCodeAsync(string postalCode)
        {
            if (string.IsNullOrWhiteSpace(postalCode) || !PostalCodeRegex.IsMatch(postalCode))
                throw new ArgumentException("Ugyldigt eller tomt postnummer", nameof(postalCode));
            return await _customerRepository.GetByPostalCodeAsync(postalCode);
        }

        public async Task<IEnumerable<Customer>> GetCustomersByCityAsync(string city)
        {
            if (string.IsNullOrWhiteSpace(city))
                throw new ArgumentException("By kan ikke være tom", nameof(city));
            return await _customerRepository.GetByCityAsync(city);
        }

        public async Task<IEnumerable<Customer>> GetCustomersWithActiveAdoptionsAsync() // Omdøbt for konsistens
        {
            // Hent alle adoptioner med status 'Approved' (antager dette er "aktiv")
            var activeAdoptions = await _adoptionRepository.GetByStatusAsync(AdoptionStatus.Approved);

            if (activeAdoptions == null || !activeAdoptions.Any())
            {
                return Enumerable.Empty<Customer>();
            }

            // Find unikke kunde-ID'er fra de aktive adoptioner
            var customerIdsWithActiveAdoptions = activeAdoptions
                .Select(a => a.CustomerId)
                .Distinct()
                .ToList();

            if (!customerIdsWithActiveAdoptions.Any())
            {
                return Enumerable.Empty<Customer>();
            }

            // Hent kunder baseret på de fundne ID'er
            // Antager at _customerRepository har en metode GetByIdsAsync eller lignende.
            // Hvis ikke, skal vi hente dem en efter en, eller tilføje GetByIdsAsync.
            // For nu, lad os antage vi henter dem en efter en, selvom det ikke er optimalt for performance.
            var customersWithActiveAdoptions = new List<Customer>();
            foreach (var customerId in customerIdsWithActiveAdoptions)
            {
                var customer = await _customerRepository.GetByIdAsync(customerId);
                if (customer != null && !customer.IsDeleted) // Sørg for at kunden ikke er slettet
                {
                    customersWithActiveAdoptions.Add(customer);
                }
            }
            return customersWithActiveAdoptions;
        }

        protected override void ValidateUser(Customer customer) // Customer-specifik validering
        {
            base.ValidateUser(customer); // Kør først den generelle BaseUser validering

            if (string.IsNullOrWhiteSpace(customer.Address))
                throw new ArgumentException("Adresse kan ikke være tom", nameof(customer.Address));
            if (string.IsNullOrWhiteSpace(customer.PostalCode))
                throw new ArgumentException("Postnummer kan ikke være tom", nameof(customer.PostalCode));
            if (!PostalCodeRegex.IsMatch(customer.PostalCode))
                throw new ArgumentException("Ugyldigt postnummer format. Forventer 4 cifre.", nameof(customer.PostalCode));
            if (string.IsNullOrWhiteSpace(customer.City))
                throw new ArgumentException("By kan ikke være tom", nameof(customer.City));
        }

        // Simpel IEqualityComparer for Customer baseret på Id for Distinct()
        private class CustomerComparer : IEqualityComparer<Customer>
        {
            public bool Equals(Customer? x, Customer? y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (x is null || y is null) return false;
                return x.Id == y.Id;
            }

            public int GetHashCode(Customer obj)
            {
                return obj.Id.GetHashCode();
            }
        }
    }
} 