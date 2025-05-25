using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using ClassLibrary.Models;
using ClassLibrary.Interfaces;

namespace ClassLibrary.Repositories
{
    /// <summary>
    /// Repository til håndtering af medarbejdere
    /// </summary>
    public class EmployeeRepository : Repository<Employee>, IEmployeeRepository
    {
        private static readonly Regex EmailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);

        public EmployeeRepository() : base()
        {
        }

        /// <summary>
        /// Finder medarbejdere baseret på navn
        /// </summary>
        public Task<IEnumerable<Employee>> GetByNameAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Navn kan ikke være tomt");

            return Task.FromResult(_items.Where(e => 
                e.Name.Equals(name, StringComparison.OrdinalIgnoreCase)));
        }

        /// <summary>
        /// Finder en medarbejder baseret på email
        /// </summary>
        public Task<Employee> GetByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email kan ikke være tom");

            if (!EmailRegex.IsMatch(email))
                throw new ArgumentException("Ugyldigt email format");

            var employee = _items.FirstOrDefault(e => 
                e.Email.Equals(email, StringComparison.OrdinalIgnoreCase));

            if (employee == null)
                throw new KeyNotFoundException($"Ingen medarbejder fundet med email: {email}");

            return Task.FromResult(employee);
        }

        /// <summary>
        /// Finder medarbejdere baseret på telefonnummer
        /// </summary>
        public Task<IEnumerable<Employee>> GetByPhoneAsync(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                throw new ArgumentException("Telefonnummer kan ikke være tomt");

            // Fjern alle ikke-numeriske tegn for at gøre søgningen mere fleksibel
            var normalizedPhone = new string(phone.Where(char.IsDigit).ToArray());
            
            return Task.FromResult(_items.Where(e => 
                new string(e.Phone.Where(char.IsDigit).ToArray()).Contains(normalizedPhone)));
        }

        /// <summary>
        /// Finder medarbejdere baseret på stilling
        /// </summary>
        public Task<IEnumerable<Employee>> GetByPositionAsync(string position)
        {
            if (string.IsNullOrWhiteSpace(position))
                throw new ArgumentException("Stilling kan ikke være tom");

            return Task.FromResult(_items.Where(e => 
                e.Position.Contains(position, StringComparison.OrdinalIgnoreCase)));
        }

        /// <summary>
        /// Finder medarbejdere baseret på afdeling
        /// </summary>
        public Task<IEnumerable<Employee>> GetByDepartmentAsync(string department)
        {
            if (string.IsNullOrWhiteSpace(department))
                throw new ArgumentException("Afdeling kan ikke være tom");

            return Task.FromResult(_items.Where(e => 
                e.Department.Contains(department, StringComparison.OrdinalIgnoreCase)));
        }

        /// <summary>
        /// Finder medarbejdere baseret på ansættelsesdato
        /// </summary>
        public Task<IEnumerable<Employee>> GetByHireDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
                throw new ArgumentException("Startdato skal være før slutdato");

            if (startDate > DateTime.Now)
                throw new ArgumentException("Startdato kan ikke være i fremtiden");

            return Task.FromResult(_items.Where(e => 
                e.HireDate >= startDate && e.HireDate <= endDate));
        }

        /// <summary>
        /// Finder medarbejdere baseret på status
        /// </summary>
        public Task<IEnumerable<Employee>> GetByStatusAsync(bool isActive)
        {
            return Task.FromResult(_items.Where(e => e.IsActive == isActive));
        }

        /// <summary>
        /// Finder medarbejdere baseret på løninterval
        /// </summary>
        public Task<IEnumerable<Employee>> GetBySalaryRangeAsync(decimal minSalary, decimal maxSalary)
        {
            if (minSalary < 0)
                throw new ArgumentException("Minimumsløn kan ikke være negativ");
            if (maxSalary < minSalary)
                throw new ArgumentException("Maksimumsløn skal være større end minimumsløn");

            return Task.FromResult(_items.Where(e => 
                e.Salary >= minSalary && e.Salary <= maxSalary));
        }

        /// <summary>
        /// Finder aktive medarbejdere
        /// </summary>
        public Task<IEnumerable<Employee>> GetActiveEmployeesAsync()
        {
            return Task.FromResult(_items.Where(e => e.IsActive));
        }

        /// <summary>
        /// Finder medarbejdere baseret på specialisering
        /// </summary>
        public Task<IEnumerable<Employee>> GetBySpecializationAsync(string specialization)
        {
            if (string.IsNullOrWhiteSpace(specialization))
                throw new ArgumentException("Specialisering kan ikke være tom");

            return Task.FromResult(_items.Where(e => 
                e.Specializations.Any(s => s.Contains(specialization, StringComparison.OrdinalIgnoreCase))));
        }

        /// <summary>
        /// Finder medarbejdere baseret på navn (søger i både fornavn og efternavn)
        /// </summary>
        public Task<IEnumerable<Employee>> SearchByNameAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Navn kan ikke være tomt");

            return Task.FromResult(_items.Where(e => 
                e.Name.Contains(name, StringComparison.OrdinalIgnoreCase) ||
                e.FirstName.Contains(name, StringComparison.OrdinalIgnoreCase) ||
                e.LastName.Contains(name, StringComparison.OrdinalIgnoreCase)));
        }

        
    }

    
}