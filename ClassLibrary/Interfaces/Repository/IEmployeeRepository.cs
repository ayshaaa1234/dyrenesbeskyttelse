using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClassLibrary.Models;

namespace ClassLibrary.Interfaces
{
    /// <summary>
    /// Interface for repository til håndtering af medarbejdere
    /// </summary>
    public interface IEmployeeRepository : IRepository<Employee>
    {
        /// <summary>
        /// Finder en medarbejder baseret på email
        /// </summary>
        Task<Employee> GetByEmailAsync(string email);

        /// <summary>
        /// Finder aktive medarbejdere
        /// </summary>
        Task<IEnumerable<Employee>> GetActiveEmployeesAsync();

        /// <summary>
        /// Finder medarbejdere baseret på stilling
        /// </summary>
        Task<IEnumerable<Employee>> GetByPositionAsync(string position);

        /// <summary>
        /// Finder medarbejdere baseret på specialisering
        /// </summary>
        Task<IEnumerable<Employee>> GetBySpecializationAsync(string specialization);

        /// <summary>
        /// Finder medarbejdere der er ansat i et bestemt datointerval
        /// </summary>
        Task<IEnumerable<Employee>> GetByHireDateRangeAsync(DateTime startDate, DateTime endDate);

        /// <summary>
        /// Finder medarbejdere baseret på navn (søger i både fornavn og efternavn)
        /// </summary>
        Task<IEnumerable<Employee>> SearchByNameAsync(string name);
    }
} 