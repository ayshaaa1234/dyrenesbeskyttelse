using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClassLibrary.Features.Employees.Core.Models; // For Employee
using ClassLibrary.SharedKernel.Application.Abstractions; // For IBaseUserService

namespace ClassLibrary.Features.Employees.Application.Abstractions // Opdateret namespace
{
    /// <summary>
    /// Interface for service til håndtering af medarbejdere
    /// </summary>
    public interface IEmployeeService : IBaseUserService<Employee>
    {
        /// <summary>
        /// Henter medarbejdere baseret på stilling
        /// </summary>
        Task<IEnumerable<Employee>> GetEmployeesByPositionAsync(string position);

        /// <summary>
        /// Henter medarbejdere baseret på afdeling
        /// </summary>
        Task<IEnumerable<Employee>> GetEmployeesByDepartmentAsync(string department);

        /// <summary>
        /// Henter medarbejdere baseret på ansættelsesdato interval
        /// </summary>
        Task<IEnumerable<Employee>> GetEmployeesByHireDateRangeAsync(DateTime startDate, DateTime endDate);

        /// <summary>
        /// Henter medarbejdere baseret på løninterval
        /// </summary>
        Task<IEnumerable<Employee>> GetEmployeesBySalaryRangeAsync(decimal minSalary, decimal maxSalary);

        /// <summary>
        /// Henter medarbejdere baseret på specialisering
        /// </summary>
        Task<IEnumerable<Employee>> GetEmployeesBySpecializationAsync(string specialization);

        /// <summary>
        /// Genaktiverer en tidligere "slettet" (IsDeleted=true) medarbejder.
        /// </summary>
        Task ReactivateEmployeeAsync(int employeeId);

        // DeactivateEmployeeAsync er dækket af DeleteAsync fra IBaseUserService.
        // GetEmployeesByStatusAsync og GetActiveEmployeesAsync er dækket af standard filtrering i repository/BaseUserService.

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