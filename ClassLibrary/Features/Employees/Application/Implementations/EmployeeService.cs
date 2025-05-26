using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ClassLibrary.Features.Employees.Core.Models;
using ClassLibrary.Features.Employees.Application.Abstractions;
using ClassLibrary.Features.Employees.Infrastructure.Abstractions;
using ClassLibrary.SharedKernel.Application.Implementations; // For BaseUserService
using ClassLibrary.SharedKernel.Exceptions; // For RepositoryException

namespace ClassLibrary.Features.Employees.Application.Implementations
{
    public class EmployeeService : BaseUserService<Employee>, IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;

        public EmployeeService(IEmployeeRepository employeeRepository) : base(employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        // Implementering af abstrakte metoder fra BaseUserService
        public override async Task<IEnumerable<Employee>> GetByNameAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Navn kan ikke være tomt.", nameof(name));
            return await _employeeRepository.GetByNameAsync(name);
        }

        public override async Task<Employee?> GetByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email) || !EmailRegex.IsMatch(email))
                throw new ArgumentException("Ugyldigt eller tomt email format.", nameof(email));
            return await _employeeRepository.GetByEmailAsync(email);
        }

        public override async Task<IEnumerable<Employee>> GetByPhoneAsync(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                throw new ArgumentException("Telefonnummer kan ikke være tomt.", nameof(phone));
            return await _employeeRepository.GetByPhoneAsync(phone);
        }

        public override async Task<IEnumerable<Employee>> GetByRegistrationDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            // For Employees, RegistrationDate er typisk lig HireDate.
            // Det er op til implementeringen i repository at bruge det korrekte felt.
            // Her kalder vi bare repository-metoden, som forventes at søge på RegistrationDate/HireDate.
            if (startDate > endDate)
                throw new ArgumentException("Startdato skal være før slutdato.");
            return await _employeeRepository.GetByHireDateRangeAsync(startDate, endDate); // Bruger HireDate specifikt her
        }

        public override async Task<IEnumerable<Employee>> SearchAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return new List<Employee>();

            var results = new List<Employee>();
            results.AddRange(await _employeeRepository.GetByNameAsync(searchTerm));
            var byEmail = await _employeeRepository.GetByEmailAsync(searchTerm);
            if (byEmail != null) results.Add(byEmail);
            results.AddRange(await _employeeRepository.GetByPhoneAsync(searchTerm));
            results.AddRange(await _employeeRepository.GetByPositionAsync(searchTerm));
            results.AddRange(await _employeeRepository.GetByDepartmentAsync(searchTerm));
            
            return results.Distinct(new EmployeeComparer());
        }

        // Override CreateAsync & UpdateAsync for Employee specifik logik
        public override async Task<Employee> CreateAsync(Employee employee)
        {
            if (employee == null) throw new ArgumentNullException(nameof(employee));
            
            // Sæt RegistrationDate til HireDate før validering, hvis HireDate er sat.
            if(employee.HireDate != default)
            {
                employee.RegistrationDate = employee.HireDate;
            }
            else // Hvis HireDate ikke er sat, brug UtcNow (som BaseUser også ville gøre for RegistrationDate)
            {
                employee.HireDate = DateTime.UtcNow;
                employee.RegistrationDate = employee.HireDate;
            }
            
            ValidateUser(employee); // Kalder den overstyrede ValidateUser nedenfor

            var existingByEmail = await _employeeRepository.GetByEmailAsync(employee.Email);
            if (existingByEmail != null && !existingByEmail.IsDeleted)
            {
                throw new RepositoryException($"En aktiv medarbejder med emailen '{employee.Email}' eksisterer allerede.");
            }
            return await base.CreateAsync(employee); // Kalder BaseUserService.CreateAsync
        }

        public override async Task<Employee> UpdateAsync(Employee employee)
        {
            if (employee == null) throw new ArgumentNullException(nameof(employee));
            
            var existingEmployee = await _employeeRepository.GetByIdAsync(employee.Id);
            if (existingEmployee == null || existingEmployee.IsDeleted)
            {
                throw new KeyNotFoundException($"Ingen aktiv medarbejder fundet med ID: {employee.Id} til opdatering.");
            }

            // Bevar oprindelig HireDate og RegistrationDate fra den eksisterende medarbejder
            // Medmindre de specifikt ændres i 'employee' input objektet før ValidateUser kaldes.
            // Vi sætter dem her for at sikre, de ikke utilsigtet overskrives af BaseUser logik,
            // hvis 'employee' objektet ikke har dem sat korrekt.
            employee.HireDate = existingEmployee.HireDate; 
            employee.RegistrationDate = existingEmployee.RegistrationDate;

            ValidateUser(employee); // Kalder den overstyrede ValidateUser

            if (!existingEmployee.Email.Equals(employee.Email, StringComparison.OrdinalIgnoreCase))
            {
                var conflictingCustomer = await _employeeRepository.GetByEmailAsync(employee.Email);
                if (conflictingCustomer != null && conflictingCustomer.Id != employee.Id && !conflictingCustomer.IsDeleted)
                {
                    throw new RepositoryException($"En anden aktiv medarbejder med emailen '{employee.Email}' eksisterer allerede.");
                }
            }
            return await base.UpdateAsync(employee); // Kalder BaseUserService.UpdateAsync
        }

        protected override void ValidateUser(Employee employee) // Employee-specifik validering
        {
            base.ValidateUser(employee); // Generel BaseUser validering

            if (string.IsNullOrWhiteSpace(employee.Position))
                throw new ArgumentException("Stilling kan ikke være tom.", nameof(employee.Position));
            if (employee.Salary < 0)
                throw new ArgumentOutOfRangeException(nameof(employee.Salary), "Løn kan ikke være negativ.");
            if (employee.HireDate == default || employee.HireDate > DateTime.UtcNow.AddMinutes(1)) // Tillad lille udsving for "nu"
                throw new ArgumentOutOfRangeException(nameof(employee.HireDate), "Ugyldig ansættelsesdato.");
            
            // Sikrer at RegistrationDate er sat (normalt til HireDate for Employees)
            if (employee.RegistrationDate == default || employee.RegistrationDate > DateTime.UtcNow.AddMinutes(1))
                throw new ArgumentOutOfRangeException(nameof(employee.RegistrationDate), "Ugyldig registreringsdato.");
        }

        // Implementering af IEmployeeService specifikke metoder
        public async Task<IEnumerable<Employee>> GetEmployeesByPositionAsync(string position)
        {
            if (string.IsNullOrWhiteSpace(position))
                throw new ArgumentException("Stilling kan ikke være tom", nameof(position));
            return await _employeeRepository.GetByPositionAsync(position);
        }

        public async Task<IEnumerable<Employee>> GetEmployeesByDepartmentAsync(string department)
        {
            if (string.IsNullOrWhiteSpace(department))
                throw new ArgumentException("Afdeling kan ikke være tom", nameof(department));
            return await _employeeRepository.GetByDepartmentAsync(department);
        }

        public async Task<IEnumerable<Employee>> GetEmployeesByHireDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
                throw new ArgumentException("Startdato skal være før slutdato.");
            return await _employeeRepository.GetByHireDateRangeAsync(startDate, endDate);
        }

        public async Task<IEnumerable<Employee>> GetEmployeesBySalaryRangeAsync(decimal minSalary, decimal maxSalary)
        {
            if (minSalary < 0 || maxSalary < 0)
                throw new ArgumentOutOfRangeException(nameof(minSalary), "Løn kan ikke være negativ.");
            if (minSalary > maxSalary)
                 throw new ArgumentOutOfRangeException(nameof(minSalary), "Minimumsløn kan ikke være større end maksimumsløn.");
            return await _employeeRepository.GetBySalaryRangeAsync(minSalary, maxSalary);
        }

        public async Task<IEnumerable<Employee>> GetEmployeesBySpecializationAsync(string specialization)
        {
            if (string.IsNullOrWhiteSpace(specialization))
                throw new ArgumentException("Specialisering kan ikke være tom", nameof(specialization));
            return await _employeeRepository.GetBySpecializationAsync(specialization);
        }

        public async Task ReactivateEmployeeAsync(int employeeId)
        {
            var employee = await _employeeRepository.GetByIdIncludeDeletedAsync(employeeId); // Antager en metode der kan hente selvom IsDeleted=true
            if (employee == null)
                throw new KeyNotFoundException($"Ingen medarbejder fundet med ID: {employeeId}.");
            if (!employee.IsDeleted)
                throw new InvalidOperationException("Medarbejder er allerede aktiv.");

            employee.IsDeleted = false;
            employee.DeletedAt = null;
            await _employeeRepository.UpdateAsync(employee); // Repository.UpdateAsync bør håndtere dette korrekt.
        }

        public async Task UpdateEmployeeSalaryAsync(int employeeId, decimal newSalary)
        {
            var employee = await _employeeRepository.GetByIdAsync(employeeId);
            if (employee == null || employee.IsDeleted)
                throw new KeyNotFoundException($"Ingen aktiv medarbejder fundet med ID: {employeeId}.");
            if (newSalary < 0)
                throw new ArgumentOutOfRangeException(nameof(newSalary), "Løn kan ikke være negativ.");

            employee.Salary = newSalary;
            await _employeeRepository.UpdateAsync(employee);
        }

        public async Task AddSpecializationAsync(int employeeId, string specialization)
        {
            var employee = await _employeeRepository.GetByIdAsync(employeeId);
            if (employee == null || employee.IsDeleted)
                throw new KeyNotFoundException($"Ingen aktiv medarbejder fundet med ID: {employeeId}.");
            if (string.IsNullOrWhiteSpace(specialization))
                throw new ArgumentException("Specialisering kan ikke være tom.", nameof(specialization));

            if (employee.Specializations == null) employee.Specializations = new List<string>();
            if (employee.Specializations.Contains(specialization, StringComparer.OrdinalIgnoreCase))
                throw new InvalidOperationException("Medarbejder har allerede denne specialisering.");

            employee.Specializations.Add(specialization);
            await _employeeRepository.UpdateAsync(employee);
        }

        public async Task RemoveSpecializationAsync(int employeeId, string specialization)
        {
            var employee = await _employeeRepository.GetByIdAsync(employeeId);
            if (employee == null || employee.IsDeleted)
                throw new KeyNotFoundException($"Ingen aktiv medarbejder fundet med ID: {employeeId}.");
            if (string.IsNullOrWhiteSpace(specialization))
                throw new ArgumentException("Specialisering kan ikke være tom.", nameof(specialization));

            // Check if Specializations list is null or if the specialization was not found and thus not removed.
            // RemoveAll returns the number of elements removed. If 0, it means the specialization wasn't there.
            if (employee.Specializations == null || employee.Specializations.RemoveAll(s => s.Equals(specialization, StringComparison.OrdinalIgnoreCase)) == 0)
                throw new InvalidOperationException("Medarbejder har ikke denne specialisering eller den kunne ikke fjernes.");
            
            await _employeeRepository.UpdateAsync(employee);
        }

        private class EmployeeComparer : IEqualityComparer<Employee>
        {
            public bool Equals(Employee? x, Employee? y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (x is null || y is null) return false;
                return x.Id == y.Id;
            }
            public int GetHashCode(Employee obj) => obj.Id.GetHashCode();
        }
    }
} 