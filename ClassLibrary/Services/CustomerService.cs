using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using ClassLibrary.Models;
using ClassLibrary.Interfaces;

namespace ClassLibrary.Services
{
    /// <summary>
    /// Service til håndtering af kunder
    /// </summary>
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        private static readonly Regex EmailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);

        /// <summary>
        /// Konstruktør
        /// </summary>
        public CustomerService(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository ?? throw new ArgumentNullException(nameof(customerRepository));
        }

        /// <summary>
        /// Henter alle kunder
        /// </summary>
        public async Task<IEnumerable<Customer>> GetAllCustomersAsync()
        {
            return await _customerRepository.GetAllAsync();
        }

        /// <summary>
        /// Henter en kunde baseret på ID
        /// </summary>
        public async Task<Customer> GetCustomerByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("ID skal være større end 0");

            var customer = await _customerRepository.GetByIdAsync(id);
            if (customer == null)
                throw new KeyNotFoundException($"Ingen kunde fundet med ID: {id}");

            return customer;
        }

        /// <summary>
        /// Opretter en ny kunde
        /// </summary>
        public async Task<Customer> CreateCustomerAsync(Customer customer)
        {
            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            ValidateCustomer(customer);
            return await _customerRepository.AddAsync(customer);
        }

        /// <summary>
        /// Opdaterer en eksisterende kunde
        /// </summary>
        public async Task<Customer> UpdateCustomerAsync(Customer customer)
        {
            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            ValidateCustomer(customer);
            return await _customerRepository.UpdateAsync(customer);
        }

        /// <summary>
        /// Sletter en kunde
        /// </summary>
        public async Task DeleteCustomerAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("ID skal være større end 0");

            await _customerRepository.DeleteAsync(id);
        }

        /// <summary>
        /// Henter kunder baseret på navn
        /// </summary>
        public async Task<IEnumerable<Customer>> GetCustomersByNameAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Navn kan ikke være tomt");

            return await _customerRepository.GetByNameAsync(name);
        }

        /// <summary>
        /// Henter en kunde baseret på email
        /// </summary>
        public async Task<Customer> GetCustomerByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email kan ikke være tom");

            if (!EmailRegex.IsMatch(email))
                throw new ArgumentException("Ugyldigt email format");

            return await _customerRepository.GetByEmailAsync(email);
        }

        /// <summary>
        /// Henter kunder baseret på telefonnummer
        /// </summary>
        public async Task<IEnumerable<Customer>> GetCustomersByPhoneAsync(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                throw new ArgumentException("Telefonnummer kan ikke være tomt");

            return await _customerRepository.GetByPhoneAsync(phone);
        }

        /// <summary>
        /// Henter kunder baseret på adresse
        /// </summary>
        public async Task<IEnumerable<Customer>> GetCustomersByAddressAsync(string address)
        {
            if (string.IsNullOrWhiteSpace(address))
                throw new ArgumentException("Adresse kan ikke være tom");

            return await _customerRepository.GetByAddressAsync(address);
        }

        /// <summary>
        /// Henter kunder baseret på postnummer
        /// </summary>
        public async Task<IEnumerable<Customer>> GetCustomersByPostalCodeAsync(string postalCode)
        {
            if (string.IsNullOrWhiteSpace(postalCode))
                throw new ArgumentException("Postnummer kan ikke være tomt");

            return await _customerRepository.GetByPostalCodeAsync(postalCode);
        }

        /// <summary>
        /// Henter kunder baseret på by
        /// </summary>
        public async Task<IEnumerable<Customer>> GetCustomersByCityAsync(string city)
        {
            if (string.IsNullOrWhiteSpace(city))
                throw new ArgumentException("By kan ikke være tom");

            return await _customerRepository.GetByCityAsync(city);
        }

        /// <summary>
        /// Henter kunder baseret på medlemskabsstatus
        /// </summary>
        public async Task<IEnumerable<Customer>> GetCustomersByMembershipStatusAsync(bool isMember)
        {
            return await _customerRepository.GetByMembershipStatusAsync(isMember);
        }

        /// <summary>
        /// Henter kunder baseret på medlemskabstype
        /// </summary>
        public async Task<IEnumerable<Customer>> GetCustomersByMembershipTypeAsync(string membershipType)
        {
            if (string.IsNullOrWhiteSpace(membershipType))
                throw new ArgumentException("Medlemskabstype kan ikke være tom");

            return await _customerRepository.GetByMembershipTypeAsync(membershipType);
        }

        /// <summary>
        /// Henter kunder der er registreret i et bestemt datointerval
        /// </summary>
        public async Task<IEnumerable<Customer>> GetCustomersByRegistrationDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
                throw new ArgumentException("Startdato skal være før slutdato");

            return await _customerRepository.GetByRegistrationDateRangeAsync(startDate, endDate);
        }

        /// <summary>
        /// Henter kunder der har adopteret dyr
        /// </summary>
        public async Task<IEnumerable<Customer>> GetCustomersWithAdoptionsAsync()
        {
            return await _customerRepository.GetCustomersWithAdoptionsAsync();
        }

        /// <summary>
        /// Søger efter kunder baseret på navn
        /// </summary>
        public async Task<IEnumerable<Customer>> SearchCustomersByNameAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Navn kan ikke være tomt");

            return await _customerRepository.SearchByNameAsync(name);
        }

        /// <summary>
        /// Tilføjer et medlemskab til en kunde
        /// </summary>
        public async Task AddMembershipAsync(int customerId, string membershipType)
        {
            if (customerId <= 0)
                throw new ArgumentException("CustomerId skal være større end 0");

            if (string.IsNullOrWhiteSpace(membershipType))
                throw new ArgumentException("Medlemskabstype kan ikke være tom");

            var customer = await GetCustomerByIdAsync(customerId);
            if (customer.IsMember)
                throw new InvalidOperationException("Kunden har allerede et medlemskab");

            customer.IsMember = true;
            customer.MembershipType = membershipType;
            await _customerRepository.UpdateAsync(customer);
        }

        /// <summary>
        /// Fjerner et medlemskab fra en kunde
        /// </summary>
        public async Task RemoveMembershipAsync(int customerId)
        {
            if (customerId <= 0)
                throw new ArgumentException("CustomerId skal være større end 0");

            var customer = await GetCustomerByIdAsync(customerId);
            if (!customer.IsMember)
                throw new InvalidOperationException("Kunden har ikke et medlemskab");

            customer.IsMember = false;
            customer.MembershipType = string.Empty;
            await _customerRepository.UpdateAsync(customer);
        }

        /// <summary>
        /// Opdaterer en kundes medlemskabstype
        /// </summary>
        public async Task UpdateMembershipTypeAsync(int customerId, string membershipType)
        {
            if (customerId <= 0)
                throw new ArgumentException("CustomerId skal være større end 0");

            if (string.IsNullOrWhiteSpace(membershipType))
                throw new ArgumentException("Medlemskabstype kan ikke være tom");

            var customer = await GetCustomerByIdAsync(customerId);
            if (!customer.IsMember)
                throw new InvalidOperationException("Kunden har ikke et medlemskab");

            customer.MembershipType = membershipType;
            await _customerRepository.UpdateAsync(customer);
        }

        /// <summary>
        /// Validerer en kunde
        /// </summary>
        private void ValidateCustomer(Customer customer)
        {
            if (string.IsNullOrWhiteSpace(customer.FirstName))
                throw new ArgumentException("Fornavn kan ikke være tomt");

            if (string.IsNullOrWhiteSpace(customer.LastName))
                throw new ArgumentException("Efternavn kan ikke være tomt");

            if (string.IsNullOrWhiteSpace(customer.Email))
                throw new ArgumentException("Email kan ikke være tom");

            if (!EmailRegex.IsMatch(customer.Email))
                throw new ArgumentException("Ugyldigt email format");

            if (string.IsNullOrWhiteSpace(customer.Phone))
                throw new ArgumentException("Telefonnummer kan ikke være tomt");

            if (string.IsNullOrWhiteSpace(customer.Address))
                throw new ArgumentException("Adresse kan ikke være tom");

            if (string.IsNullOrWhiteSpace(customer.PostalCode))
                throw new ArgumentException("Postnummer kan ikke være tomt");

            if (string.IsNullOrWhiteSpace(customer.City))
                throw new ArgumentException("By kan ikke være tom");

            if (customer.RegistrationDate > DateTime.Now)
                throw new ArgumentException("Registreringsdato kan ikke være i fremtiden");

            if (customer.IsMember && string.IsNullOrWhiteSpace(customer.MembershipType))
                throw new ArgumentException("Medlemskabstype skal angives når kunden er medlem");
        }
    }
} 