using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClassLibrary.Models;

namespace ClassLibrary.Interfaces
{
    /// <summary>
    /// Interface for service til håndtering af medarbejdere
    /// </summary>
    public interface IEmployeeService
    {
        /// <summary>
        /// Henter alle medarbejdere
        /// </summary>
        Task<IEnumerable<Employee>> GetAllEmployeesAsync();

        /// <summary>
        /// Henter en medarbejder baseret på ID
        /// </summary>
        Task<Employee> GetEmployeeByIdAsync(int id);

        /// <summary>
        /// Opretter en ny medarbejder
        /// </summary>
        Task<Employee> CreateEmployeeAsync(Employee employee);

        /// <summary>
        /// Opdaterer en eksisterende medarbejder
        /// </summary>
        Task<Employee> UpdateEmployeeAsync(Employee employee);

        /// <summary>
        /// Sletter en medarbejder
        /// </summary>
        Task DeleteEmployeeAsync(int id);

        /// <summary>
        /// Henter en medarbejder baseret på email
        /// </summary>
        Task<Employee> GetEmployeeByEmailAsync(string email);

        /// <summary>
        /// Henter medarbejdere baseret på navn
        /// </summary>
        Task<IEnumerable<Employee>> GetEmployeesByNameAsync(string name);

        /// <summary>
        /// Henter medarbejdere baseret på telefonnummer
        /// </summary>
        Task<IEnumerable<Employee>> GetEmployeesByPhoneAsync(string phone);

        /// <summary>
        /// Henter medarbejdere baseret på stilling
        /// </summary>
        Task<IEnumerable<Employee>> GetEmployeesByPositionAsync(string position);

        /// <summary>
        /// Henter medarbejdere baseret på afdeling
        /// </summary>
        Task<IEnumerable<Employee>> GetEmployeesByDepartmentAsync(string department);

        /// <summary>
        /// Henter medarbejdere baseret på ansættelsesdato
        /// </summary>
        Task<IEnumerable<Employee>> GetEmployeesByHireDateRangeAsync(DateTime startDate, DateTime endDate);

        /// <summary>
        /// Henter medarbejdere baseret på status
        /// </summary>
        Task<IEnumerable<Employee>> GetEmployeesByStatusAsync(bool isActive);

        /// <summary>
        /// Henter medarbejdere baseret på løninterval
        /// </summary>
        Task<IEnumerable<Employee>> GetEmployeesBySalaryRangeAsync(decimal minSalary, decimal maxSalary);

        /// <summary>
        /// Henter aktive medarbejdere
        /// </summary>
        Task<IEnumerable<Employee>> GetActiveEmployeesAsync();

        /// <summary>
        /// Henter medarbejdere baseret på specialisering
        /// </summary>
        Task<IEnumerable<Employee>> GetEmployeesBySpecializationAsync(string specialization);

        /// <summary>
        /// Søger efter medarbejdere baseret på navn
        /// </summary>
        Task<IEnumerable<Employee>> SearchEmployeesByNameAsync(string name);

        /// <summary>
        /// Aktiverer en medarbejder
        /// </summary>
        Task ActivateEmployeeAsync(int employeeId);

        /// <summary>
        /// Deaktiverer en medarbejder
        /// </summary>
        Task DeactivateEmployeeAsync(int employeeId);

        /// <summary>
        /// Opdaterer en medarbejders løn
        /// </summary>
        Task UpdateEmployeeSalaryAsync(int employeeId, decimal newSalary);

        /// <summary>
        /// Tilføjer en specialisering til en medarbejder
        /// </summary>
        Task AddSpecializationAsync(int employeeId, string specialization);

        /// <summary>
        /// Fjerner en specialisering fra en medarbejder
        /// </summary>
        Task RemoveSpecializationAsync(int employeeId, string specialization);
    }
} 