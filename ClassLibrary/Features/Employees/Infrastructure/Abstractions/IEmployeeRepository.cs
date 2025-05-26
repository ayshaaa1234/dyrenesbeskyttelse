using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClassLibrary.Features.Employees.Core.Models; // For Employee
using ClassLibrary.SharedKernel.Persistence.Abstractions; // For IRepository<T>

namespace ClassLibrary.Features.Employees.Infrastructure.Abstractions // Opdateret namespace
{
    /// <summary>
    /// Interface for repository til håndtering af medarbejdere
    /// </summary>
    public interface IEmployeeRepository : IRepository<Employee>
    {
        /// <summary>
        /// Finder en medarbejder baseret på email (forventer eksakt match, ignorerer case)
        /// </summary>
        Task<Employee?> GetByEmailAsync(string email);

        /// <summary>
        /// Finder medarbejdere baseret på stilling (kan matche delvist)
        /// </summary>
        Task<IEnumerable<Employee>> GetByPositionAsync(string position);

        /// <summary>
        /// Finder medarbejdere baseret på afdeling (kan matche delvist)
        /// </summary>
        Task<IEnumerable<Employee>> GetByDepartmentAsync(string department);

        /// <summary>
        /// Finder medarbejdere der har en specifik specialisering
        /// </summary>
        Task<IEnumerable<Employee>> GetBySpecializationAsync(string specialization);

        /// <summary>
        /// Finder medarbejdere der er ansat i et bestemt datointerval
        /// </summary>
        Task<IEnumerable<Employee>> GetByHireDateRangeAsync(DateTime startDate, DateTime endDate);

        /// <summary>
        /// Finder medarbejdere baseret på navn (kan matche fornavn eller efternavn delvist)
        /// </summary>
        Task<IEnumerable<Employee>> GetByNameAsync(string name);

        /// <summary>
        /// Finder medarbejdere baseret på telefonnummer (kan matche delvist)
        /// </summary>
        Task<IEnumerable<Employee>> GetByPhoneAsync(string phone);

        /// <summary>
        /// Finder medarbejdere baseret på løninterval
        /// </summary>
        Task<IEnumerable<Employee>> GetBySalaryRangeAsync(decimal minSalary, decimal maxSalary);

        /// <summary>
        /// Henter en medarbejder baseret på ID, inklusiv soft-deleted.
        /// </summary>
        Task<Employee?> GetByIdIncludeDeletedAsync(int id);
        
        // GetActiveEmployeesAsync() er allerede dækket af base.GetAllAsync() eller base.FindAsync(e => !e.IsDeleted)
        // Hvis der er behov for specifikt at hente KUN aktive, kan det implementeres, men ofte er standarden at hente aktive.
        // Jeg fjerner den for nu, da Repository<T> allerede filtrerer på !IsDeleted.

        // GetByStatusAsync(bool isActive) er erstattet af IsDeleted logik i Repository<T>
        // En evt. metode kunne være GetInactiveEmployeesAsync() hvis nødvendigt.
    }
} 