using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClassLibrary.Models;

namespace ClassLibrary.Interfaces
{
    /// <summary>
    /// Interface for repository til håndtering af besøgslogge
    /// </summary>
    public interface IVisitLogRepository : IRepository<VisitLog>
    {
        /// <summary>
        /// Finder alle besøg for et bestemt dyr
        /// </summary>
        Task<IEnumerable<VisitLog>> GetByAnimalIdAsync(int animalId);

        /// <summary>
        /// Finder alle besøg for en bestemt kunde
        /// </summary>
        Task<IEnumerable<VisitLog>> GetByCustomerIdAsync(int customerId);

        /// <summary>
        /// Finder alle besøg for en bestemt medarbejder
        /// </summary>
        Task<IEnumerable<VisitLog>> GetByEmployeeIdAsync(int employeeId);

        /// <summary>
        /// Finder besøg i et bestemt datointerval
        /// </summary>
        Task<IEnumerable<VisitLog>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);

        /// <summary>
        /// Finder alle lægebesøg
        /// </summary>
        Task<IEnumerable<VisitLog>> GetVeterinaryVisitsAsync();

        /// <summary>
        /// Finder besøg der resulterede i adoption
        /// </summary>
        Task<IEnumerable<VisitLog>> GetVisitsResultingInAdoptionAsync();

        /// <summary>
        /// Finder det seneste besøg for et dyr
        /// </summary>
        Task<VisitLog> GetLatestVisitForAnimalAsync(int animalId);

        /// <summary>
        /// Finder besøgslogge baseret på besøgstype
        /// </summary>
        Task<IEnumerable<VisitLog>> GetByVisitTypeAsync(string visitType);

        /// <summary>
        /// Finder besøgslogge baseret på besøger
        /// </summary>
        Task<IEnumerable<VisitLog>> GetByVisitorAsync(string visitor);

        /// <summary>
        /// Finder besøgslogge baseret på formål
        /// </summary>
        Task<IEnumerable<VisitLog>> GetByPurposeAsync(string purpose);

        /// <summary>
        /// Finder besøgslogge baseret på varighedsinterval
        /// </summary>
        Task<IEnumerable<VisitLog>> GetByDurationRangeAsync(int minDuration, int maxDuration);

        /// <summary>
        /// Finder besøgslogge baseret på lægebesøg
        /// </summary>
        Task<IEnumerable<VisitLog>> GetByVeterinaryVisitAsync(bool isVeterinaryVisit);

        /// <summary>
        /// Finder besøgslogge baseret på adoptionsresultat
        /// </summary>
        Task<IEnumerable<VisitLog>> GetByAdoptionResultAsync(bool resultedInAdoption);
    }
} 