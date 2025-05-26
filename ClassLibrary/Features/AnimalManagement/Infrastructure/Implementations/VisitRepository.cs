using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClassLibrary.Features.AnimalManagement.Core.Models;
using ClassLibrary.Features.AnimalManagement.Core.Enums;
using ClassLibrary.Features.AnimalManagement.Infrastructure.Abstractions;
using ClassLibrary.SharedKernel.Persistence.Implementations;
using ClassLibrary.SharedKernel.Exceptions;
using ClassLibrary.Infrastructure.DataInitialization;

namespace ClassLibrary.Features.AnimalManagement.Infrastructure.Implementations
{
    /// <summary>
    /// Repository til håndtering af besøgsdata.
    /// </summary>
    public class VisitRepository : Repository<Visit>, IVisitRepository
    {
        /// <summary>
        /// Initialiserer en ny instans af <see cref="VisitRepository"/> klassen.
        /// </summary>
        public VisitRepository() : base(Path.Combine(JsonDataInitializer.CalculatedWorkspaceRoot, "Data", "Json", "visits.json")) { }

        /// <summary>
        /// Tilføjer et nyt besøg asynkront.
        /// </summary>
        /// <param name="entity">Besøget der skal tilføjes.</param>
        /// <returns>Det tilføjede besøg.</returns>
        /// <exception cref="ArgumentNullException">Kastes hvis entity er null.</exception>
        public override async Task<Visit> AddAsync(Visit entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            ValidateEntity(entity);
            return await base.AddAsync(entity);
        }
        
        /// <summary>
        /// Opdaterer et eksisterende besøg asynkront.
        /// </summary>
        /// <param name="entity">Besøget med de opdaterede værdier.</param>
        /// <returns>Det opdaterede besøg.</returns>
        /// <exception cref="ArgumentNullException">Kastes hvis entity er null.</exception>
        /// <exception cref="RepositoryException">Kastes hvis besøget med det angivne ID ikke findes.</exception>
        public override async Task<Visit> UpdateAsync(Visit entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            ValidateEntity(entity);

            var existingVisit = await base.GetByIdAsync(entity.Id);
            if (existingVisit == null) 
                throw new RepositoryException($"Visit med ID {entity.Id} blev ikke fundet for opdatering.");
            
            return await base.UpdateAsync(entity);
        }
        
        /// <summary>
        /// Validerer en besøgsentitet.
        /// </summary>
        /// <param name="entity">Besøget der skal valideres.</param>
        /// <exception cref="ArgumentException">Kastes ved diverse valideringsfejl relateret til AnimalId, PlannedDate, Type, Status eller ActualDate/ActualDuration for afsluttede besøg.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Kastes hvis PlannedDuration er negativ eller ActualDate er i fremtiden.</exception>
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

        /// <summary>
        /// Henter alle besøg tilknyttet et specifikt dyr.
        /// </summary>
        /// <param name="animalId">ID på dyret.</param>
        /// <returns>En samling af besøg for det angivne dyr. Returnerer en tom samling, hvis AnimalId er ugyldigt.</returns>
        public async Task<IEnumerable<Visit>> GetVisitsByAnimalAsync(int animalId)
        {
            if (animalId <= 0) 
            {
                return Enumerable.Empty<Visit>();
            }
            return await base.FindAsync(v => v.AnimalId == animalId);
        }

        /// <summary>
        /// Henter alle besøg tilknyttet en specifik kunde.
        /// </summary>
        /// <param name="customerId">ID på kunden.</param>
        /// <returns>En samling af besøg for den angivne kunde. Returnerer en tom samling, hvis CustomerId er ugyldigt.</returns>
        public async Task<IEnumerable<Visit>> GetVisitsByCustomerAsync(int customerId)
        {
            if (customerId <= 0) 
            {
                return Enumerable.Empty<Visit>();
            }
            return await base.FindAsync(v => v.CustomerId == customerId);
        }

        /// <summary>
        /// Henter alle besøg håndteret af en specifik medarbejder.
        /// </summary>
        /// <param name="employeeId">ID på medarbejderen.</param>
        /// <returns>En samling af besøg for den angivne medarbejder. Returnerer en tom samling, hvis EmployeeId er ugyldigt.</returns>
        public async Task<IEnumerable<Visit>> GetVisitsByEmployeeAsync(int employeeId)
        {
             if (employeeId <= 0) 
            {
                return Enumerable.Empty<Visit>();
            }
            return await base.FindAsync(v => v.EmployeeId == employeeId);
        }

        /// <summary>
        /// Henter besøg, der er planlagt eller har fundet sted inden for et specificeret datointerval.
        /// </summary>
        /// <param name="startDate">Startdato for intervallet.</param>
        /// <param name="endDate">Slutdato for intervallet.</param>
        /// <returns>En samling af besøg inden for det angivne datointerval.</returns>
        /// <exception cref="ArgumentException">Kastes hvis startdato er efter slutdato.</exception>
        public async Task<IEnumerable<Visit>> GetVisitsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate) throw new ArgumentException("Startdato kan ikke være efter slutdato.");
            
            return await base.FindAsync(v => 
                (v.PlannedDate.Date >= startDate.Date && v.PlannedDate.Date <= endDate.Date) || 
                (v.ActualDate.HasValue && v.ActualDate.Value.Date >= startDate.Date && v.ActualDate.Value.Date <= endDate.Date)
            );
        }

        /// <summary>
        /// Henter besøg baseret på deres status.
        /// </summary>
        /// <param name="status">Besøgsstatus der søges efter.</param>
        /// <returns>En samling af besøg med den angivne status. Returnerer en tom samling, hvis status er udefineret.</returns>
        public async Task<IEnumerable<Visit>> GetVisitsByStatusAsync(VisitStatus status)
        {
            if (!Enum.IsDefined(typeof(VisitStatus), status))
            {
                return Enumerable.Empty<Visit>();
            }
            return await base.FindAsync(v => v.Status == status);
        }

        /// <summary>
        /// Henter besøg baseret på deres type.
        /// </summary>
        /// <param name="type">Besøgstypen der søges efter (case-insensitive, eksakt matchning).</param>
        /// <returns>En samling af matchende besøg. Returnerer en tom samling, hvis typen er tom eller null.</returns>
        public async Task<IEnumerable<Visit>> GetVisitsByTypeAsync(string type)
        {
            if (string.IsNullOrWhiteSpace(type)) return Enumerable.Empty<Visit>();
            return await base.FindAsync(v => !string.IsNullOrWhiteSpace(v.Type) && v.Type.Equals(type, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Henter alle besøg, der er blevet annulleret.
        /// </summary>
        /// <returns>En samling af annullerede besøg.</returns>
        public async Task<IEnumerable<Visit>> GetCancelledVisitsAsync()
        {
            return await base.FindAsync(v => v.Status == VisitStatus.Cancelled);
        }

        /// <summary>
        /// Henter alle besøg, der er blevet gennemført.
        /// </summary>
        /// <returns>En samling af gennemførte besøg.</returns>
        public async Task<IEnumerable<Visit>> GetCompletedVisitsAsync()
        {
            return await base.FindAsync(v => v.Status == VisitStatus.Completed);
        }

        /// <summary>
        /// Henter alle besøg, der er på venteliste.
        /// </summary>
        /// <returns>En samling af besøg på venteliste.</returns>
        public async Task<IEnumerable<Visit>> GetWaitlistedVisitsAsync()
        {
            return await base.FindAsync(v => v.Status == VisitStatus.Waitlisted);
        }

        /// <summary>
        /// Henter alle besøg, der er markeret som dyrlægebesøg.
        /// </summary>
        /// <returns>En samling af dyrlægebesøg.</returns>
        public async Task<IEnumerable<Visit>> GetVeterinaryVisitsAsync()
        {
            return await base.FindAsync(v => v.IsVeterinaryVisit);
        }

        /// <summary>
        /// Henter alle besøg, der har resulteret i en adoption.
        /// </summary>
        /// <returns>En samling af besøg, der førte til adoption.</returns>
        public async Task<IEnumerable<Visit>> GetAdoptionVisitsAsync()
        {
            return await base.FindAsync(v => v.ResultedInAdoption);
        }

        /// <summary>
        /// Henter det seneste registrerede besøg for et specifikt dyr (baseret på ActualDate, dernæst PlannedDate).
        /// </summary>
        /// <param name="animalId">ID på dyret.</param>
        /// <returns>Det seneste besøg, eller null hvis ingen findes eller AnimalId er ugyldigt.</returns>
        public async Task<Visit?> GetLatestVisitForAnimalAsync(int animalId)
        {
            if (animalId <= 0) 
            {
                return null;
            }
            var visits = await base.FindAsync(v => v.AnimalId == animalId);
            return visits.OrderByDescending(v => v.ActualDate ?? v.PlannedDate).FirstOrDefault();
        }

        /// <summary>
        /// Henter det seneste registrerede besøg for en specifik kunde (baseret på ActualDate, dernæst PlannedDate).
        /// </summary>
        /// <param name="customerId">ID på kunden.</param>
        /// <returns>Det seneste besøg for kunden, eller null hvis ingen findes eller CustomerId er ugyldigt.</returns>
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