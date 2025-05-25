using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClassLibrary.Models;
using ClassLibrary.Interfaces;

namespace ClassLibrary.Repositories
{
    /// <summary>
    /// Repository til håndtering af besøgslogge
    /// </summary>
    public class VisitLogRepository : Repository<VisitLog>, IVisitLogRepository
    {
        public VisitLogRepository() : base()
        {
        }

        /// <summary>
        /// Finder besøgslogge for et bestemt dyr
        /// </summary>
        public Task<IEnumerable<VisitLog>> GetByAnimalIdAsync(int animalId)
        {
            if (animalId <= 0)
                throw new ArgumentException("AnimalId skal være større end 0");

            return Task.FromResult(_items.Where(v => v.AnimalId == animalId));
        }

        /// <summary>
        /// Finder besøgslogge baseret på datointerval
        /// </summary>
        public Task<IEnumerable<VisitLog>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
                throw new ArgumentException("Startdato skal være før slutdato");

            if (startDate > DateTime.Now)
                throw new ArgumentException("Startdato kan ikke være i fremtiden");

            return Task.FromResult(_items.Where(v => 
                v.VisitDate >= startDate && v.VisitDate <= endDate));
        }

        /// <summary>
        /// Finder besøgslogge baseret på besøgstype
        /// </summary>
        public Task<IEnumerable<VisitLog>> GetByVisitTypeAsync(string visitType)
        {
            if (string.IsNullOrWhiteSpace(visitType))
                throw new ArgumentException("Besøgstype kan ikke være tom");

            return Task.FromResult(_items.Where(v => 
                v.VisitType.Contains(visitType, StringComparison.OrdinalIgnoreCase)));
        }

        /// <summary>
        /// Finder besøgslogge baseret på besøger
        /// </summary>
        public Task<IEnumerable<VisitLog>> GetByVisitorAsync(string visitor)
        {
            if (string.IsNullOrWhiteSpace(visitor))
                throw new ArgumentException("Besøger kan ikke være tom");

            return Task.FromResult(_items.Where(v => 
                v.Visitor.Contains(visitor, StringComparison.OrdinalIgnoreCase)));
        }

        /// <summary>
        /// Finder besøgslogge baseret på formål
        /// </summary>
        public Task<IEnumerable<VisitLog>> GetByPurposeAsync(string purpose)
        {
            if (string.IsNullOrWhiteSpace(purpose))
                throw new ArgumentException("Formål kan ikke være tomt");

            return Task.FromResult(_items.Where(v => 
                v.Purpose.Contains(purpose, StringComparison.OrdinalIgnoreCase)));
        }

        /// <summary>
        /// Finder besøgslogge baseret på varighed
        /// </summary>
        public Task<IEnumerable<VisitLog>> GetByDurationRangeAsync(int minDuration, int maxDuration)
        {
            if (minDuration < 0)
                throw new ArgumentException("Minimumsvarighed kan ikke være negativ");
            if (maxDuration < minDuration)
                throw new ArgumentException("Maksimumsvarighed skal være større end minimumsvarighed");

            return Task.FromResult(_items.Where(v => 
                v.Duration >= minDuration && v.Duration <= maxDuration));
        }

        /// <summary>
        /// Finder besøgslogge baseret på dyrlægebesøg
        /// </summary>
        public Task<IEnumerable<VisitLog>> GetByVeterinaryVisitAsync(bool isVeterinaryVisit)
        {
            return Task.FromResult(_items.Where(v => v.IsVeterinaryVisit == isVeterinaryVisit));
        }

        /// <summary>
        /// Finder besøgslogge baseret på adoptionsresultat
        /// </summary>
        public Task<IEnumerable<VisitLog>> GetByAdoptionResultAsync(bool resultedInAdoption)
        {
            return Task.FromResult(_items.Where(v => v.ResultedInAdoption == resultedInAdoption));
        }

        /// <summary>
        /// Finder alle besøg for en bestemt kunde
        /// </summary>
        public Task<IEnumerable<VisitLog>> GetByCustomerIdAsync(int customerId)
        {
            if (customerId <= 0)
                throw new ArgumentException("CustomerId skal være større end 0");

            return Task.FromResult(_items.Where(v => v.CustomerId == customerId));
        }

        /// <summary>
        /// Finder alle besøg for en bestemt medarbejder
        /// </summary>
        public Task<IEnumerable<VisitLog>> GetByEmployeeIdAsync(int employeeId)
        {
            if (employeeId <= 0)
                throw new ArgumentException("EmployeeId skal være større end 0");

            return Task.FromResult(_items.Where(v => v.EmployeeId == employeeId));
        }

        /// <summary>
        /// Finder alle lægebesøg
        /// </summary>
        public Task<IEnumerable<VisitLog>> GetVeterinaryVisitsAsync()
        {
            return Task.FromResult(_items.Where(v => v.IsVeterinaryVisit));
        }

        /// <summary>
        /// Finder besøg der resulterede i adoption
        /// </summary>
        public Task<IEnumerable<VisitLog>> GetVisitsResultingInAdoptionAsync()
        {
            return Task.FromResult(_items.Where(v => v.ResultedInAdoption));
        }

        /// <summary>
        /// Finder det seneste besøg for et dyr
        /// </summary>
        public Task<VisitLog> GetLatestVisitForAnimalAsync(int animalId)
        {
            if (animalId <= 0)
                throw new ArgumentException("AnimalId skal være større end 0");

            var visit = _items
                .Where(v => v.AnimalId == animalId)
                .OrderByDescending(v => v.VisitDate)
                .FirstOrDefault();

            if (visit == null)
                throw new KeyNotFoundException($"Ingen besøg fundet for dyr med ID: {animalId}");

            return Task.FromResult(visit);
        }
    }
} 