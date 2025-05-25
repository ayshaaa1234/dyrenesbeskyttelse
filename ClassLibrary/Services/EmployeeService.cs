using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using ClassLibrary.Models;
using ClassLibrary.Interfaces;

namespace ClassLibrary.Services
{
    /// <summary>
    /// Service til håndtering af medarbejdere
    /// </summary>
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private static readonly Regex EmailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);

        /// <summary>
        /// Konstruktør
        /// </summary>
        public EmployeeService(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository ?? throw new ArgumentNullException(nameof(employeeRepository));
        }

        /// <summary>
        /// Henter alle medarbejdere
        /// </summary>
        public async Task<IEnumerable<Employee>> GetAllEmployeesAsync()
        {
            return await _employeeRepository.GetAllAsync();
        }

        /// <summary>
        /// Henter en medarbejder baseret på ID
        /// </summary>
        public async Task<Employee> GetEmployeeByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("ID skal være større end 0");

            var employee = await _employeeRepository.GetByIdAsync(id);
            if (employee == null)
                throw new KeyNotFoundException($"Ingen medarbejder fundet med ID: {id}");

            return employee;
        }

        /// <summary>
        /// Opretter en ny medarbejder
        /// </summary>
        public async Task<Employee> CreateEmployeeAsync(Employee employee)
        {
            if (employee == null)
                throw new ArgumentNullException(nameof(employee));

            ValidateEmployee(employee);
            return await _employeeRepository.AddAsync(employee);
        }

        /// <summary>
        /// Opdaterer en eksisterende medarbejder
        /// </summary>
        public async Task<Employee> UpdateEmployeeAsync(Employee employee)
        {
            if (employee == null)
                throw new ArgumentNullException(nameof(employee));

            ValidateEmployee(employee);
            return await _employeeRepository.UpdateAsync(employee);
        }

        /// <summary>
        /// Sletter en medarbejder
        /// </summary>
        public async Task DeleteEmployeeAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("ID skal være større end 0");

            await _employeeRepository.DeleteAsync(id);
        }

        /// <summary>
        /// Henter en medarbejder baseret på email
        /// </summary>
        public async Task<Employee> GetEmployeeByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email kan ikke være tom");

            if (!EmailRegex.IsMatch(email))
                throw new ArgumentException("Ugyldigt email format");

            return await _employeeRepository.GetByEmailAsync(email);
        }

        /// <summary>
        /// Henter medarbejdere baseret på navn
        /// </summary>
        public async Task<IEnumerable<Employee>> GetEmployeesByNameAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Navn kan ikke være tomt");

            return await _employeeRepository.GetByNameAsync(name);
        }

        /// <summary>
        /// Henter medarbejdere baseret på stilling
        /// </summary>
        public async Task<IEnumerable<Employee>> GetEmployeesByPositionAsync(string position)
        {
            if (string.IsNullOrWhiteSpace(position))
                throw new ArgumentException("Stilling kan ikke være tom");

            return await _employeeRepository.GetByPositionAsync(position);
        }

        /// <summary>
        /// Henter medarbejdere baseret på afdeling
        /// </summary>
        public async Task<IEnumerable<Employee>> GetEmployeesByDepartmentAsync(string department)
        {
            if (string.IsNullOrWhiteSpace(department))
                throw new ArgumentException("Afdeling kan ikke være tom");

            return await _employeeRepository.GetByDepartmentAsync(department);
        }

        /// <summary>
        /// Henter medarbejdere baseret på ansættelsesdato
        /// </summary>
        public async Task<IEnumerable<Employee>> GetEmployeesByHireDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
                throw new ArgumentException("Startdato skal være før slutdato");

            if (startDate > DateTime.Now)
                throw new ArgumentException("Startdato kan ikke være i fremtiden");

            return await _employeeRepository.GetByHireDateRangeAsync(startDate, endDate);
        }

        /// <summary>
        /// Henter medarbejdere baseret på status
        /// </summary>
        public async Task<IEnumerable<Employee>> GetEmployeesByStatusAsync(bool isActive)
        {
            return await _employeeRepository.GetByStatusAsync(isActive);
        }

    

        /// <summary>
        /// Henter aktive medarbejdere
        /// </summary>
        public async Task<IEnumerable<Employee>> GetActiveEmployeesAsync()
        {
            return await _employeeRepository.GetActiveEmployeesAsync();
        }

        /// <summary>
        /// Henter medarbejdere baseret på specialisering
        /// </summary>
        public async Task<IEnumerable<Employee>> GetEmployeesBySpecializationAsync(string specialization)
        {
            if (string.IsNullOrWhiteSpace(specialization))
                throw new ArgumentException("Specialisering kan ikke være tom");

            return await _employeeRepository.GetBySpecializationAsync(specialization);
        }

        /// <summary>
        /// Søger efter medarbejdere baseret på navn
        /// </summary>
        public async Task<IEnumerable<Employee>> SearchEmployeesByNameAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Navn kan ikke være tomt");

            return await _employeeRepository.SearchByNameAsync(name);
        }

        /// <summary>
        /// Aktiverer en medarbejder
        /// </summary>
        public async Task ActivateEmployeeAsync(int employeeId)
        {
            if (employeeId <= 0)
                throw new ArgumentException("EmployeeId skal være større end 0");

            var employee = await GetEmployeeByIdAsync(employeeId);
            if (employee.IsActive)
                throw new InvalidOperationException("Medarbejderen er allerede aktiv");

            employee.IsActive = true;
            await _employeeRepository.UpdateAsync(employee);
        }

        /// <summary>
        /// Deaktiverer en medarbejder
        /// </summary>
        public async Task DeactivateEmployeeAsync(int employeeId)
        {
            if (employeeId <= 0)
                throw new ArgumentException("EmployeeId skal være større end 0");

            var employee = await GetEmployeeByIdAsync(employeeId);
            if (!employee.IsActive)
                throw new InvalidOperationException("Medarbejderen er allerede deaktiveret");

            employee.IsActive = false;
            await _employeeRepository.UpdateAsync(employee);
        }

        /// <summary>
        /// Opdaterer en medarbejders løn
        /// </summary>
        public async Task UpdateEmployeeSalaryAsync(int employeeId, decimal newSalary)
        {
            if (employeeId <= 0)
                throw new ArgumentException("EmployeeId skal være større end 0");

            if (newSalary < 0)
                throw new ArgumentException("Løn kan ikke være negativ");

            var employee = await GetEmployeeByIdAsync(employeeId);
            employee.Salary = newSalary;
            await _employeeRepository.UpdateAsync(employee);
        }

        /// <summary>
        /// Tilføjer en specialisering til en medarbejder
        /// </summary>
        public async Task AddSpecializationAsync(int employeeId, string specialization)
        {
            if (employeeId <= 0)
                throw new ArgumentException("EmployeeId skal være større end 0");

            if (string.IsNullOrWhiteSpace(specialization))
                throw new ArgumentException("Specialisering kan ikke være tom");

            var employee = await GetEmployeeByIdAsync(employeeId);
            if (employee.Specializations.Contains(specialization))
                throw new InvalidOperationException("Medarbejderen har allerede denne specialisering");

            employee.Specializations.Add(specialization);
            await _employeeRepository.UpdateAsync(employee);
        }

        /// <summary>
        /// Fjerner en specialisering fra en medarbejder
        /// </summary>
        public async Task RemoveSpecializationAsync(int employeeId, string specialization)
        {
            if (employeeId <= 0)
                throw new ArgumentException("EmployeeId skal være større end 0");

            if (string.IsNullOrWhiteSpace(specialization))
                throw new ArgumentException("Specialisering kan ikke være tom");

            var employee = await GetEmployeeByIdAsync(employeeId);
            if (!employee.Specializations.Contains(specialization))
                throw new InvalidOperationException("Medarbejderen har ikke denne specialisering");

            employee.Specializations.Remove(specialization);
            await _employeeRepository.UpdateAsync(employee);
        }

        /// <summary>
        /// Henter medarbejdere baseret på telefonnummer
        /// </summary>
        public async Task<IEnumerable<Employee>> GetEmployeesByPhoneAsync(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                throw new ArgumentException("Telefonnummer kan ikke være tomt");

            return await _employeeRepository.GetByPhoneAsync(phone);
        }

        /// <summary>
        /// Henter medarbejdere baseret på løninterval
        /// </summary>
        public async Task<IEnumerable<Employee>> GetEmployeesBySalaryRangeAsync(decimal minSalary, decimal maxSalary)
        {
            if (minSalary < 0)
                throw new ArgumentException("Minimumsløn kan ikke være negativ");
            if (maxSalary < minSalary)
                throw new ArgumentException("Maksimumsløn skal være større end minimumsløn");

            return await _employeeRepository.GetBySalaryRangeAsync(minSalary, maxSalary);
        }

        /// <summary>
        /// Validerer en medarbejder
        /// </summary>
        private void ValidateEmployee(Employee employee)
        {
            if (string.IsNullOrWhiteSpace(employee.FirstName))
                throw new ArgumentException("Fornavn kan ikke være tomt");

            if (string.IsNullOrWhiteSpace(employee.LastName))
                throw new ArgumentException("Efternavn kan ikke være tomt");

            if (string.IsNullOrWhiteSpace(employee.Email))
                throw new ArgumentException("Email kan ikke være tom");

            if (!EmailRegex.IsMatch(employee.Email))
                throw new ArgumentException("Ugyldigt email format");

            if (string.IsNullOrWhiteSpace(employee.Phone))
                throw new ArgumentException("Telefonnummer kan ikke være tomt");

            if (string.IsNullOrWhiteSpace(employee.Position))
                throw new ArgumentException("Stilling kan ikke være tom");

            if (string.IsNullOrWhiteSpace(employee.Department))
                throw new ArgumentException("Afdeling kan ikke være tom");

            if (employee.Salary < 0)
                throw new ArgumentException("Løn kan ikke være negativ");

            if (employee.HireDate > DateTime.Now)
                throw new ArgumentException("Ansættelsesdato kan ikke være i fremtiden");
        }
    }
} 