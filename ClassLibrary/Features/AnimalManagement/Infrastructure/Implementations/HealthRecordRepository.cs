using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClassLibrary.Features.AnimalManagement.Core.Models;
using ClassLibrary.Features.AnimalManagement.Infrastructure.Abstractions;
using ClassLibrary.SharedKernel.Persistence.Implementations;
using ClassLibrary.SharedKernel.Exceptions; // For RepositoryException, hvis nødvendigt
using ClassLibrary.Infrastructure.DataInitialization; // Tilføjet for JsonDataInitializer

namespace ClassLibrary.Features.AnimalManagement.Infrastructure.Implementations
{
    public class HealthRecordRepository : Repository<HealthRecord>, IHealthRecordRepository
    {
        public HealthRecordRepository() : base(Path.Combine(JsonDataInitializer.CalculatedWorkspaceRoot, "Data", "Json", "healthrecords.json")) { }

        public override async Task<HealthRecord> AddAsync(HealthRecord entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            ValidateEntity(entity);
            return await base.AddAsync(entity);
        }
        
        public override async Task<HealthRecord> UpdateAsync(HealthRecord entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            ValidateEntity(entity);

            var existingRecord = await base.GetByIdAsync(entity.Id); 
            if (existingRecord == null) 
                 throw new RepositoryException($"HealthRecord med ID {entity.Id} blev ikke fundet for opdatering.");
            
            return await base.UpdateAsync(entity); 
        }
        
        protected override void ValidateEntity(HealthRecord entity)
        {
            base.ValidateEntity(entity);
            if (entity.AnimalId <= 0) 
                throw new ArgumentException("AnimalId skal være større end 0.", nameof(entity.AnimalId));
            if (entity.RecordDate == default(DateTime))
                throw new ArgumentException("RecordDate skal være angivet.", nameof(entity.RecordDate));
            if (entity.RecordDate > DateTime.UtcNow.AddMinutes(1))
                throw new ArgumentOutOfRangeException(nameof(entity.RecordDate), "RecordDate kan ikke være i fremtiden.");
            if (string.IsNullOrWhiteSpace(entity.Diagnosis))
                throw new ArgumentException("Diagnose kan ikke være tom.", nameof(entity.Diagnosis));
             if (string.IsNullOrWhiteSpace(entity.VeterinarianName) && string.IsNullOrWhiteSpace(entity.VeterinarianId))
                throw new ArgumentException("Enten VeterinarianName eller VeterinarianId skal angives.", nameof(entity.VeterinarianName));

        }

        public async Task<IEnumerable<HealthRecord>> GetHealthRecordsByAnimalIdAsync(int animalId)
        {
            if (animalId <= 0) 
            {
                return Enumerable.Empty<HealthRecord>();
            }
            return await base.FindAsync(h => h.AnimalId == animalId);
        }

        public async Task<HealthRecord?> GetLatestHealthRecordForAnimalAsync(int animalId)
        {
            if (animalId <= 0) 
            {
                return null;
            }
            var records = await base.FindAsync(h => h.AnimalId == animalId);
            return records.OrderByDescending(h => h.RecordDate).FirstOrDefault();
        }

        public async Task<IEnumerable<HealthRecord>> GetHealthRecordsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate) throw new ArgumentException("Startdato kan ikke være efter slutdato.");
            return await base.FindAsync(h => h.RecordDate.Date >= startDate.Date && h.RecordDate.Date <= endDate.Date);
        }

        public async Task<IEnumerable<HealthRecord>> GetHealthRecordsByDiagnosisAsync(string diagnosis)
        {
            if (string.IsNullOrWhiteSpace(diagnosis)) return Enumerable.Empty<HealthRecord>();
            return await base.FindAsync(h => !string.IsNullOrWhiteSpace(h.Diagnosis) && h.Diagnosis.Contains(diagnosis, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<IEnumerable<HealthRecord>> GetHealthRecordsByTreatmentAsync(string treatment)
        {
            if (string.IsNullOrWhiteSpace(treatment)) return Enumerable.Empty<HealthRecord>();
            return await base.FindAsync(h => !string.IsNullOrWhiteSpace(h.Treatment) && h.Treatment.Contains(treatment, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<IEnumerable<HealthRecord>> GetHealthRecordsByVaccinationStatusAsync(bool isVaccinated)
        {
            return await base.FindAsync(h => h.IsVaccinated == isVaccinated);
        }

        public async Task<IEnumerable<HealthRecord>> GetHealthRecordsByMedicationAsync(string medication)
        {
            if (string.IsNullOrWhiteSpace(medication)) return Enumerable.Empty<HealthRecord>();
            return await base.FindAsync(h => !string.IsNullOrWhiteSpace(h.Medication) && h.Medication.Contains(medication, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<IEnumerable<HealthRecord>> GetHealthRecordsBySeverityAsync(string severity)
        {
            if (string.IsNullOrWhiteSpace(severity)) return Enumerable.Empty<HealthRecord>();
            return await base.FindAsync(h => !string.IsNullOrWhiteSpace(h.Severity) && h.Severity.Equals(severity, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<IEnumerable<HealthRecord>> GetHealthRecordsByVeterinarianAsync(string veterinarianIdentifier)
        {
            if (string.IsNullOrWhiteSpace(veterinarianIdentifier)) return Enumerable.Empty<HealthRecord>();
            
            return await base.FindAsync(h => 
                (!string.IsNullOrWhiteSpace(h.VeterinarianName) && h.VeterinarianName.Contains(veterinarianIdentifier, StringComparison.OrdinalIgnoreCase)) || 
                (!string.IsNullOrWhiteSpace(h.VeterinarianId) && h.VeterinarianId.Equals(veterinarianIdentifier, StringComparison.OrdinalIgnoreCase))
            );
        }
    }
} 