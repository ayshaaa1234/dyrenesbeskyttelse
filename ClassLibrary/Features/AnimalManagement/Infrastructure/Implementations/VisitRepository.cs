using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClassLibrary.Features.AnimalManagement.Core.Models;
using ClassLibrary.Features.AnimalManagement.Core.Enums;
using ClassLibrary.Features.AnimalManagement.Infrastructure.Abstractions;
using ClassLibrary.SharedKernel.Persistence.Implementations;
using ClassLibrary.SharedKernel.Exceptions;

namespace ClassLibrary.Features.AnimalManagement.Infrastructure.Implementations
{
    public class VisitRepository : Repository<Visit>, IVisitRepository
    {
        private const string FilePath = "Data/Json/visits.json";

        public VisitRepository() : base(FilePath) { }

        public override async Task<Visit> AddAsync(Visit entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            ValidateEntity(entity);
            return await base.AddAsync(entity);
        }
        
        public override async Task<Visit> UpdateAsync(Visit entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            ValidateEntity(entity);

            var existingVisit = await base.GetByIdAsync(entity.Id);
            if (existingVisit == null) 
                throw new RepositoryException($"Visit med ID {entity.Id} blev ikke fundet for opdatering.");
            
            return await base.UpdateAsync(entity);
        }
        
        protected override void ValidateEntity(Visit entity)
        {
            base.ValidateEntity(entity);
            if (entity.AnimalId <= 0) 
                throw new ArgumentException("AnimalId skal være større end 0.", nameof(entity.AnimalId));
            
            if (entity.PlannedDate == default(DateTime))
                throw new ArgumentException("PlannedDate skal være angivet.", nameof(entity.PlannedDate));

            if (entity.PlannedDuration < 0) 
                throw new ArgumentOutOfRangeException(nameof(entity.PlannedDuration), "Planlagt varighed kan ikke være negativ.");

            if (string.IsNullOrWhiteSpace(entity.Type))
                throw new ArgumentException("Type (besøgstype) kan ikke være tom.", nameof(entity.Type));

            if (!Enum.IsDefined(typeof(VisitStatus), entity.Status))
                throw new ArgumentException("Ugyldig VisitStatus.", nameof(entity.Status));

            if (entity.ActualDate.HasValue && entity.ActualDate.Value > DateTime.UtcNow.AddMinutes(1))
                throw new ArgumentOutOfRangeException(nameof(entity.ActualDate), "ActualDate kan ikke være i fremtiden.");

            if (entity.Status == VisitStatus.Completed)
            {
                if (!entity.ActualDate.HasValue)
                    throw new ArgumentException("ActualDate skal være sat for et afsluttet besøg.", nameof(entity.ActualDate));
                if (!entity.ActualDuration.HasValue || entity.ActualDuration.Value <= 0)
                    throw new ArgumentException("ActualDuration skal være sat og positiv for et afsluttet besøg.", nameof(entity.ActualDuration));
            }
        }

        public async Task<IEnumerable<Visit>> GetVisitsByAnimalAsync(int animalId)
        {
            if (animalId <= 0) 
            {
                return Enumerable.Empty<Visit>();
            }
            return await base.FindAsync(v => v.AnimalId == animalId);
        }

        public async Task<IEnumerable<Visit>> GetVisitsByCustomerAsync(int customerId)
        {
            if (customerId <= 0) 
            {
                return Enumerable.Empty<Visit>();
            }
            return await base.FindAsync(v => v.CustomerId == customerId);
        }

        public async Task<IEnumerable<Visit>> GetVisitsByEmployeeAsync(int employeeId)
        {
             if (employeeId <= 0) 
            {
                return Enumerable.Empty<Visit>();
            }
            return await base.FindAsync(v => v.EmployeeId == employeeId);
        }

        public async Task<IEnumerable<Visit>> GetVisitsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate) throw new ArgumentException("Startdato kan ikke være efter slutdato.");
            
            return await base.FindAsync(v => 
                (v.PlannedDate.Date >= startDate.Date && v.PlannedDate.Date <= endDate.Date) || 
                (v.ActualDate.HasValue && v.ActualDate.Value.Date >= startDate.Date && v.ActualDate.Value.Date <= endDate.Date)
            );
        }

        public async Task<IEnumerable<Visit>> GetVisitsByStatusAsync(VisitStatus status)
        {
            if (!Enum.IsDefined(typeof(VisitStatus), status))
            {
                return Enumerable.Empty<Visit>();
            }
            return await base.FindAsync(v => v.Status == status);
        }

        public async Task<IEnumerable<Visit>> GetVisitsByTypeAsync(string type)
        {
            if (string.IsNullOrWhiteSpace(type)) return Enumerable.Empty<Visit>();
            return await base.FindAsync(v => !string.IsNullOrWhiteSpace(v.Type) && v.Type.Equals(type, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<IEnumerable<Visit>> GetCancelledVisitsAsync()
        {
            return await base.FindAsync(v => v.Status == VisitStatus.Cancelled);
        }

        public async Task<IEnumerable<Visit>> GetCompletedVisitsAsync()
        {
            return await base.FindAsync(v => v.Status == VisitStatus.Completed);
        }

        public async Task<IEnumerable<Visit>> GetWaitlistedVisitsAsync()
        {
            return await base.FindAsync(v => v.Status == VisitStatus.Waitlisted);
        }

        public async Task<IEnumerable<Visit>> GetVeterinaryVisitsAsync()
        {
            return await base.FindAsync(v => v.IsVeterinaryVisit);
        }

        public async Task<IEnumerable<Visit>> GetAdoptionVisitsAsync()
        {
            return await base.FindAsync(v => v.ResultedInAdoption);
        }

        public async Task<Visit?> GetLatestVisitForAnimalAsync(int animalId)
        {
            if (animalId <= 0) 
            {
                return null;
            }
            var visits = await base.FindAsync(v => v.AnimalId == animalId);
            return visits.OrderByDescending(v => v.ActualDate ?? v.PlannedDate).FirstOrDefault();
        }

        public async Task<Visit?> GetLatestVisitForCustomerAsync(int customerId)
        {
            if (customerId <= 0) 
            {
                return null;
            }
            var visits = await base.FindAsync(v => v.CustomerId == customerId);
            return visits.OrderByDescending(v => v.ActualDate ?? v.PlannedDate).FirstOrDefault();
        }
    }
} 