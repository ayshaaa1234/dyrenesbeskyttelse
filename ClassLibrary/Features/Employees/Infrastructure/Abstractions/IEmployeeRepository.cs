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
        /// <param name="email">Emailen der søges efter.</param>
        /// <returns>En opgave, der repræsenterer den asynkrone operation. Opgavens resultat indeholder den fundne medarbejder eller null, hvis ingen medarbejder matcher.</returns>
        Task<Employee?> GetByEmailAsync(string email);

        /// <summary>
        /// Finder medarbejdere baseret på stilling (kan matche delvist)
        /// </summary>
        /// <param name="position">Stillingen der søges efter.</param>
        /// <returns>En opgave, der repræsenterer den asynkrone operation. Opgavens resultat indeholder en samling af medarbejdere, der matcher stillingen.</returns>
        Task<IEnumerable<Employee>> GetByPositionAsync(string position);

        /// <summary>
        /// Finder medarbejdere baseret på afdeling (kan matche delvist)
        /// </summary>
        /// <param name="department">Afdelingen der søges efter.</param>
        /// <returns>En opgave, der repræsenterer den asynkrone operation. Opgavens resultat indeholder en samling af medarbejdere, der matcher afdelingen.</returns>
        Task<IEnumerable<Employee>> GetByDepartmentAsync(string department);

        /// <summary>
        /// Finder medarbejdere der har en specifik specialisering
        /// </summary>
        /// <param name="specialization">Specialiseringen der søges efter.</param>
        /// <returns>En opgave, der repræsenterer den asynkrone operation. Opgavens resultat indeholder en samling af medarbejdere, der har den specificerede specialisering.</returns>
        Task<IEnumerable<Employee>> GetBySpecializationAsync(string specialization);

        /// <summary>
        /// Finder medarbejdere der er ansat i et bestemt datointerval
        /// </summary>
        /// <param name="startDate">Startdato for intervallet.</param>
        /// <param name="endDate">Slutdato for intervallet.</param>
        /// <returns>En opgave, der repræsenterer den asynkrone operation. Opgavens resultat indeholder en samling af medarbejdere ansat inden for det angivne datointerval.</returns>
        Task<IEnumerable<Employee>> GetByHireDateRangeAsync(DateTime startDate, DateTime endDate);

        /// <summary>
        /// Finder medarbejdere baseret på navn (kan matche fornavn eller efternavn delvist)
        /// </summary>
        /// <param name="name">Navnet eller en del af navnet der søges efter.</param>
        /// <returns>En opgave, der repræsenterer den asynkrone operation. Opgavens resultat indeholder en samling af medarbejdere, der matcher navnet.</returns>
        Task<IEnumerable<Employee>> GetByNameAsync(string name);

        /// <summary>
        /// Finder medarbejdere baseret på telefonnummer (kan matche delvist)
        /// </summary>
        /// <param name="phone">Telefonnummeret der søges efter.</param>
        /// <returns>En opgave, der repræsenterer den asynkrone operation. Opgavens resultat indeholder en samling af medarbejdere, der matcher telefonnummeret.</returns>
        Task<IEnumerable<Employee>> GetByPhoneAsync(string phone);

        /// <summary>
        /// Finder medarbejdere baseret på løninterval
        /// </summary>
        /// <param name="minSalary">Minimumslønnen i intervallet.</param>
        /// <param name="maxSalary">Maksimumslønnen i intervallet.</param>
        /// <returns>En opgave, der repræsenterer den asynkrone operation. Opgavens resultat indeholder en samling af medarbejdere inden for det specificerede løninterval.</returns>
        Task<IEnumerable<Employee>> GetBySalaryRangeAsync(decimal minSalary, decimal maxSalary);

        /// <summary>
        /// Henter en medarbejder baseret på ID, inklusiv soft-deleted.
        /// </summary>
        /// <param name="id">ID på medarbejderen der søges efter.</param>
        /// <returns>En opgave, der repræsenterer den asynkrone operation. Opgavens resultat indeholder den fundne medarbejder (aktiv eller soft-deleted) eller null, hvis ingen medarbejder matcher ID'et.</returns>
        Task<Employee?> GetByIdIncludeDeletedAsync(int id);
        
        // GetActiveEmployeesAsync() er allerede dækket af base.GetAllAsync() eller base.FindAsync(e => !e.IsDeleted)
        // Hvis der er behov for specifikt at hente KUN aktive, kan det implementeres, men ofte er standarden at hente aktive.
        // Jeg fjerner den for nu, da Repository<T> allerede filtrerer på !IsDeleted.

        // GetByStatusAsync(bool isActive) er erstattet af IsDeleted logik i Repository<T>
        // En evt. metode kunne være GetInactiveEmployeesAsync() hvis nødvendigt.
    }
} 