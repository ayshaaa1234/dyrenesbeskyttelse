using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ClassLibrary.Features.Employees.Core.Models; // For Employee
using ClassLibrary.Features.Employees.Infrastructure.Abstractions; // For IEmployeeRepository
using ClassLibrary.SharedKernel.Persistence.Implementations; // For Repository<T>
using ClassLibrary.SharedKernel.Exceptions; // For RepositoryException

namespace ClassLibrary.Features.Employees.Infrastructure.Implementations // Opdateret namespace
{
    public class EmployeeRepository : Repository<Employee>, IEmployeeRepository
    {
        private const string FilePath = "Data/Json/employees.json";

        // Regex fra BaseUser kan bruges hvis de er protected static, ellers definer dem her.
        // For nu antager vi, at de er tilgængelige eller gen-defineres efter behov.
        private static readonly Regex EmailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);
        private static readonly Regex PhoneRegex = new Regex(@"^(\+45\s?)?\d{8}$", RegexOptions.Compiled);

        public EmployeeRepository() : base(FilePath) { }

        protected override void ValidateEntity(Employee entity)
        {
            base.ValidateEntity(entity); // Generel null-tjek etc.

            if (string.IsNullOrWhiteSpace(entity.FirstName))
                throw new ArgumentException("Fornavn kan ikke være tomt.", nameof(entity.FirstName));
            if (string.IsNullOrWhiteSpace(entity.LastName))
                throw new ArgumentException("Efternavn kan ikke være tomt.", nameof(entity.LastName));
            if (string.IsNullOrWhiteSpace(entity.Email))
                throw new ArgumentException("Email kan ikke være tom.", nameof(entity.Email));
            if (!EmailRegex.IsMatch(entity.Email))
                throw new ArgumentException("Ugyldigt email format.", nameof(entity.Email));
            if (string.IsNullOrWhiteSpace(entity.Phone))
                throw new ArgumentException("Telefonnummer kan ikke være tomt.", nameof(entity.Phone));
            if (!PhoneRegex.IsMatch(entity.Phone.Replace(" ", "")))
                throw new ArgumentException("Ugyldigt telefonnummer format. Forventer 8 cifre, evt. med +45.", nameof(entity.Phone));
            if (string.IsNullOrWhiteSpace(entity.Position))
                throw new ArgumentException("Stilling kan ikke være tom.", nameof(entity.Position));
            
            if (entity.HireDate == default(DateTime))
                 throw new ArgumentOutOfRangeException(nameof(entity.HireDate), "Ansættelsesdato skal være angivet.");
            if (entity.HireDate > DateTime.UtcNow.AddMinutes(1)) 
                throw new ArgumentOutOfRangeException(nameof(entity.HireDate), "Ansættelsesdato kan ikke være i fremtiden.");

            if (entity.Salary < 0)
                throw new ArgumentOutOfRangeException(nameof(entity.Salary), "Løn kan ikke være negativ.");
        }

        public override async Task<Employee> AddAsync(Employee employee)
        {
            ValidateEntity(employee);
            if (await EmailExistsForAnotherActiveEmployeeAsync(employee.Email, 0))
                throw new RepositoryException($"En aktiv medarbejder med email {employee.Email} eksisterer allerede.");

            // Id og RegistrationDate sættes af hhv. Repository<T> og BaseUser
            return await base.AddAsync(employee);
        }

        public override async Task<Employee> UpdateAsync(Employee employee)
        {
            ValidateEntity(employee);
            var items = await LoadDataAsync();
            var existingEmployee = items.FirstOrDefault(e => e.Id == employee.Id && !e.IsDeleted);

            if (existingEmployee == null) 
                throw new KeyNotFoundException($"Ingen aktiv medarbejder fundet med ID: {employee.Id} for opdatering.");

            if (!existingEmployee.Email.Equals(employee.Email, StringComparison.OrdinalIgnoreCase) &&
                await EmailExistsForAnotherActiveEmployeeAsync(employee.Email, employee.Id))
                throw new RepositoryException($"En anden aktiv medarbejder med email {employee.Email} eksisterer allerede.");

            // Bevar oprindelig HireDate og RegistrationDate
            employee.HireDate = existingEmployee.HireDate;
            employee.RegistrationDate = existingEmployee.RegistrationDate;
            
            return await base.UpdateAsync(employee);
        }

        private async Task<bool> EmailExistsForAnotherActiveEmployeeAsync(string email, int currentEmployeeId)
        {
            if (string.IsNullOrWhiteSpace(email) || !EmailRegex.IsMatch(email)) return false;
            
            var employeesWithEmail = await base.FindAsync(e => 
                !string.IsNullOrWhiteSpace(e.Email) && 
                e.Email.Equals(email, StringComparison.OrdinalIgnoreCase)); 
            
            return employeesWithEmail.Any(e => e.Id != currentEmployeeId);
        }

        // Implementering af IEmployeeRepository metoder
        public async Task<Employee?> GetByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email) || !EmailRegex.IsMatch(email))
                throw new ArgumentException("Ugyldig eller tom email.");
            return (await base.FindAsync(e => e.Email.Equals(email, StringComparison.OrdinalIgnoreCase) && !e.IsDeleted)).FirstOrDefault();
        }

        public async Task<IEnumerable<Employee>> GetByPositionAsync(string position)
        {
            if (string.IsNullOrWhiteSpace(position))
                throw new ArgumentException("Stilling kan ikke være tom.", nameof(position));
            return await base.FindAsync(e => e.Position.Contains(position, StringComparison.OrdinalIgnoreCase) && !e.IsDeleted);
        }

        public async Task<IEnumerable<Employee>> GetByDepartmentAsync(string department)
        {
            if (string.IsNullOrWhiteSpace(department))
                throw new ArgumentException("Afdeling kan ikke være tom.", nameof(department));
            return await base.FindAsync(e => e.Department.Contains(department, StringComparison.OrdinalIgnoreCase) && !e.IsDeleted);
        }

        public async Task<IEnumerable<Employee>> GetBySpecializationAsync(string specialization)
        {
            if (string.IsNullOrWhiteSpace(specialization))
                throw new ArgumentException("Specialisering kan ikke være tom.", nameof(specialization));
            return await base.FindAsync(e => e.Specializations.Contains(specialization, StringComparer.OrdinalIgnoreCase) && !e.IsDeleted);
        }

        public async Task<IEnumerable<Employee>> GetByHireDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
                throw new ArgumentException("Startdato skal være før slutdato.");
            return await base.FindAsync(e => e.HireDate.Date >= startDate.Date && e.HireDate.Date <= endDate.Date && !e.IsDeleted);
        }

        public async Task<IEnumerable<Employee>> GetByNameAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Navn kan ikke være tomt.", nameof(name));
            return await base.FindAsync(e => 
                (e.FirstName.Contains(name, StringComparison.OrdinalIgnoreCase) || 
                 e.LastName.Contains(name, StringComparison.OrdinalIgnoreCase) || 
                 e.Name.Contains(name, StringComparison.OrdinalIgnoreCase)) && !e.IsDeleted);
        }

        public async Task<IEnumerable<Employee>> GetByPhoneAsync(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                throw new ArgumentException("Telefonnummer kan ikke være tomt.", nameof(phone));
            var normalizedPhone = new string(phone.Where(char.IsDigit).ToArray());
            if (string.IsNullOrWhiteSpace(normalizedPhone)) 
                throw new ArgumentException("Telefonnummer indeholder ingen cifre.");
            return await base.FindAsync(e => new string(e.Phone.Where(char.IsDigit).ToArray()).Contains(normalizedPhone) && !e.IsDeleted);
        }

        public async Task<IEnumerable<Employee>> GetBySalaryRangeAsync(decimal minSalary, decimal maxSalary)
        {
            if (minSalary < 0 || maxSalary < 0)
                throw new ArgumentOutOfRangeException(nameof(minSalary), "Løn kan ikke være negativ.");
            if (minSalary > maxSalary)
                throw new ArgumentOutOfRangeException(nameof(minSalary), "Minimumsløn kan ikke være større end maksimumsløn.");
            return await base.FindAsync(e => e.Salary >= minSalary && e.Salary <= maxSalary && !e.IsDeleted);
        }

        public async Task<Employee?> GetByIdIncludeDeletedAsync(int id)
        {
            var items = await LoadDataAsync(); 
            return items.FirstOrDefault(x => x.Id == id);
        }
    }
} 