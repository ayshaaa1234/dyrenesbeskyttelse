using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClassLibrary.Models;

namespace ClassLibrary.Interfaces
{
    /// <summary>
    /// Interface for service til håndtering af besøgslogge
    /// </summary>
    public interface IVisitLogService
    {
        /// <summary>
        /// Henter alle besøgslogge
        /// </summary>
        Task<IEnumerable<VisitLog>> GetAllVisitLogsAsync();

        /// <summary>
        /// Henter en besøgslog baseret på ID
        /// </summary>
        Task<VisitLog> GetVisitLogByIdAsync(int id);

        /// <summary>
        /// Opretter en ny besøgslog
        /// </summary>
        Task<VisitLog> CreateVisitLogAsync(VisitLog visitLog);

        /// <summary>
        /// Opdaterer en eksisterende besøgslog
        /// </summary>
        Task<VisitLog> UpdateVisitLogAsync(VisitLog visitLog);

        /// <summary>
        /// Sletter en besøgslog
        /// </summary>
        Task DeleteVisitLogAsync(int id);

        /// <summary>
        /// Henter besøgslogge for et bestemt dyr
        /// </summary>
        Task<IEnumerable<VisitLog>> GetVisitLogsByAnimalIdAsync(int animalId);

        /// <summary>
        /// Henter besøgslogge for en bestemt kunde
        /// </summary>
        Task<IEnumerable<VisitLog>> GetVisitLogsByCustomerIdAsync(int customerId);

        /// <summary>
        /// Henter besøgslogge for en bestemt medarbejder
        /// </summary>
        Task<IEnumerable<VisitLog>> GetVisitLogsByEmployeeIdAsync(int employeeId);

        /// <summary>
        /// Henter besøgslogge baseret på datointerval
        /// </summary>
        Task<IEnumerable<VisitLog>> GetVisitLogsByDateRangeAsync(DateTime startDate, DateTime endDate);

        /// <summary>
        /// Henter besøgslogge baseret på besøgstype
        /// </summary>
        Task<IEnumerable<VisitLog>> GetVisitLogsByVisitTypeAsync(string visitType);

        /// <summary>
        /// Henter besøgslogge baseret på besøger
        /// </summary>
        Task<IEnumerable<VisitLog>> GetVisitLogsByVisitorAsync(string visitor);

        /// <summary>
        /// Henter besøgslogge baseret på formål
        /// </summary>
        Task<IEnumerable<VisitLog>> GetVisitLogsByPurposeAsync(string purpose);

        /// <summary>
        /// Henter besøgslogge baseret på varighedsinterval
        /// </summary>
        Task<IEnumerable<VisitLog>> GetVisitLogsByDurationRangeAsync(int minDuration, int maxDuration);

        /// <summary>
        /// Henter besøgslogge baseret på lægebesøg
        /// </summary>
        Task<IEnumerable<VisitLog>> GetVisitLogsByVeterinaryVisitAsync(bool isVeterinaryVisit);

        /// <summary>
        /// Henter besøgslogge baseret på adoptionsresultat
        /// </summary>
        Task<IEnumerable<VisitLog>> GetVisitLogsByAdoptionResultAsync(bool resultedInAdoption);

        /// <summary>
        /// Henter alle lægebesøg
        /// </summary>
        Task<IEnumerable<VisitLog>> GetVeterinaryVisitsAsync();

        /// <summary>
        /// Henter besøg der resulterede i adoption
        /// </summary>
        Task<IEnumerable<VisitLog>> GetVisitsResultingInAdoptionAsync();

        /// <summary>
        /// Henter det seneste besøg for et dyr
        /// </summary>
        Task<VisitLog> GetLatestVisitForAnimalAsync(int animalId);

        /// <summary>
        /// Marker et besøg som lægebesøg
        /// </summary>
        Task MarkAsVeterinaryVisitAsync(int visitLogId);

        /// <summary>
        /// Marker et besøg som resulteret i adoption
        /// </summary>
        Task MarkAsResultedInAdoptionAsync(int visitLogId);

        /// <summary>
        /// Tilføjer noter til en besøgslog
        /// </summary>
        Task AddNotesAsync(int visitLogId, string notes);

        /// <summary>
        /// Opdaterer besøgsvarigheden
        /// </summary>
        Task UpdateDurationAsync(int visitLogId, int duration);
    }
} 