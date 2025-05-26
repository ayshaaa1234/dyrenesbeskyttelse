using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClassLibrary.Features.AnimalManagement.Core.Models;
using ClassLibrary.Features.AnimalManagement.Core.Enums;
using ClassLibrary.SharedKernel.Persistence.Abstractions;

namespace ClassLibrary.Features.AnimalManagement.Infrastructure.Abstractions
{
    /// <summary>
    /// Interface for repository til håndtering af besøgsdata.
    /// </summary>
    public interface IVisitRepository : IRepository<Visit>
    {
        // AddAsync, GetByIdAsync, UpdateAsync, DeleteAsync arves fra IRepository<Visit>
        // For nu antager vi standard sletning, så vi kan fjerne den specifikke DeleteVisitAsync.

        /// <summary>
        /// Henter alle besøg tilknyttet et specifikt dyr.
        /// </summary>
        /// <param name="animalId">ID på dyret.</param>
        Task<IEnumerable<Visit>> GetVisitsByAnimalAsync(int animalId);
        /// <summary>
        /// Henter alle besøg tilknyttet en specifik kunde.
        /// </summary>
        /// <param name="customerId">ID på kunden.</param>
        Task<IEnumerable<Visit>> GetVisitsByCustomerAsync(int customerId); // CustomerId vil komme fra Customer feature
        /// <summary>
        /// Henter alle besøg håndteret af en specifik medarbejder.
        /// </summary>
        /// <param name="employeeId">ID på medarbejderen.</param>
        Task<IEnumerable<Visit>> GetVisitsByEmployeeAsync(int employeeId); // EmployeeId vil komme fra Employee feature
        /// <summary>
        /// Henter besøg, der er planlagt eller har fundet sted inden for et specificeret datointerval.
        /// </summary>
        /// <param name="startDate">Startdato for intervallet.</param>
        /// <param name="endDate">Slutdato for intervallet.</param>
        Task<IEnumerable<Visit>> GetVisitsByDateRangeAsync(DateTime startDate, DateTime endDate);
        /// <summary>
        /// Henter besøg baseret på deres status.
        /// </summary>
        /// <param name="status">Besøgsstatus der søges efter.</param>
        Task<IEnumerable<Visit>> GetVisitsByStatusAsync(VisitStatus status);
        /// <summary>
        /// Henter besøg baseret på deres type (f.eks. "Adoptionssamtale", "Kontrolbesøg").
        /// </summary>
        /// <param name="type">Besøgstypen der søges efter.</param>
        Task<IEnumerable<Visit>> GetVisitsByTypeAsync(string type);
        /// <summary>
        /// Henter alle besøg, der er blevet annulleret.
        /// </summary>
        Task<IEnumerable<Visit>> GetCancelledVisitsAsync();
        /// <summary>
        /// Henter alle besøg, der er blevet gennemført.
        /// </summary>
        Task<IEnumerable<Visit>> GetCompletedVisitsAsync();
        /// <summary>
        /// Henter alle besøg, der er på venteliste.
        /// </summary>
        Task<IEnumerable<Visit>> GetWaitlistedVisitsAsync();
        /// <summary>
        /// Henter alle besøg, der er markeret som dyrlægebesøg.
        /// </summary>
        Task<IEnumerable<Visit>> GetVeterinaryVisitsAsync();
        /// <summary>
        /// Henter alle besøg, der har resulteret i en adoption.
        /// </summary>
        Task<IEnumerable<Visit>> GetAdoptionVisitsAsync(); // Indikerer et besøg der resulterede i adoption
        /// <summary>
        /// Henter det seneste registrerede besøg for et specifikt dyr.
        /// </summary>
        /// <param name="animalId">ID på dyret.</param>
        /// <returns>Det seneste besøg, eller null hvis ingen findes.</returns>
        Task<Visit?> GetLatestVisitForAnimalAsync(int animalId);
        /// <summary>
        /// Henter det seneste registrerede besøg for en specifik kunde.
        /// </summary>
        /// <param name="customerId">ID på kunden.</param>
        /// <returns>Det seneste besøg for kunden, eller null hvis ingen findes.</returns>
        Task<Visit?> GetLatestVisitForCustomerAsync(int customerId); // CustomerId vil komme fra Customer feature
    }
} 