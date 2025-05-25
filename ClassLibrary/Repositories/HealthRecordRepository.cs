using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClassLibrary.Models;
using ClassLibrary.Interfaces;

namespace ClassLibrary.Repositories
{
    /// <summary>
    /// Repository til håndtering af sundhedsjournaler
    /// </summary>
    public class HealthRecordRepository : Repository<HealthRecord>, IHealthRecordRepository
    {
        public HealthRecordRepository() : base()
        {
        }

        /// <summary>
        /// Finder sundhedsjournaler for et bestemt dyr
        /// </summary>
        public Task<IEnumerable<HealthRecord>> GetByAnimalIdAsync(int animalId)
        {
            if (animalId <= 0)
                throw new ArgumentException("AnimalId skal være større end 0");

            return Task.FromResult(_items.Where(h => h.AnimalId == animalId));
        }

        /// <summary>
        /// Finder sundhedsjournaler baseret på datointerval
        /// </summary>
        public Task<IEnumerable<HealthRecord>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
                throw new ArgumentException("Startdato skal være før slutdato");

            if (startDate > DateTime.Now)
                throw new ArgumentException("Startdato kan ikke være i fremtiden");

            return Task.FromResult(_items.Where(h => 
                h.RecordDate >= startDate && h.RecordDate <= endDate));
        }

        /// <summary>
        /// Finder sundhedsjournaler baseret på diagnose
        /// </summary>
        public Task<IEnumerable<HealthRecord>> GetByDiagnosisAsync(string diagnosis)
        {
            if (string.IsNullOrWhiteSpace(diagnosis))
                throw new ArgumentException("Diagnose kan ikke være tom");

            return Task.FromResult(_items.Where(h => 
                h.Diagnosis.Contains(diagnosis, StringComparison.OrdinalIgnoreCase)));
        }

        /// <summary>
        /// Finder sundhedsjournaler baseret på behandling
        /// </summary>
        public Task<IEnumerable<HealthRecord>> GetByTreatmentAsync(string treatment)
        {
            if (string.IsNullOrWhiteSpace(treatment))
                throw new ArgumentException("Behandling kan ikke være tom");

            return Task.FromResult(_items.Where(h => 
                h.Treatment.Contains(treatment, StringComparison.OrdinalIgnoreCase)));
        }

        /// <summary>
        /// Finder sundhedsjournaler baseret på vaccinationsstatus
        /// </summary>
        public Task<IEnumerable<HealthRecord>> GetByVaccinationStatusAsync(bool isVaccinated)
        {
            return Task.FromResult(_items.Where(h => h.IsVaccinated == isVaccinated));
        }

        /// <summary>
        /// Finder sundhedsjournaler baseret på medicin
        /// </summary>
        public Task<IEnumerable<HealthRecord>> GetByMedicationAsync(string medication)
        {
            if (string.IsNullOrWhiteSpace(medication))
                throw new ArgumentException("Medicin kan ikke være tom");

            return Task.FromResult(_items.Where(h => 
                h.Medication.Contains(medication, StringComparison.OrdinalIgnoreCase)));
        }

        /// <summary>
        /// Finder sundhedsjournaler baseret på alvorlighedsgrad
        /// </summary>
        public Task<IEnumerable<HealthRecord>> GetBySeverityAsync(string severity)
        {
            if (string.IsNullOrWhiteSpace(severity))
                throw new ArgumentException("Alvorlighedsgrad kan ikke være tom");

            return Task.FromResult(_items.Where(h => 
                h.Severity.Contains(severity, StringComparison.OrdinalIgnoreCase)));
        }

        /// <summary>
        /// Finder sundhedsjournaler baseret på behandler
        /// </summary>
        public Task<IEnumerable<HealthRecord>> GetByVeterinarianAsync(string veterinarian)
        {
            if (string.IsNullOrWhiteSpace(veterinarian))
                throw new ArgumentException("Behandler kan ikke være tom");

            return Task.FromResult(_items.Where(h => 
                h.VeterinarianName.Contains(veterinarian, StringComparison.OrdinalIgnoreCase)));
        }

        /// <summary>
        /// Finder dyr der mangler vaccination
        /// </summary>
        public Task<IEnumerable<HealthRecord>> GetRecordsNeedingVaccinationAsync()
        {
            var today = DateTime.Today;
            return Task.FromResult(_items.Where(h => 
                h.IsVaccinated && 
                h.NextVaccinationDate.HasValue && 
                h.NextVaccinationDate.Value <= today));
        }

        /// <summary>
        /// Finder den seneste sundhedsjournal for et dyr
        /// </summary>
        public Task<HealthRecord> GetLatestRecordForAnimalAsync(int animalId)
        {
            if (animalId <= 0)
                throw new ArgumentException("AnimalId skal være større end 0");

            var record = _items
                .Where(h => h.AnimalId == animalId)
                .OrderByDescending(h => h.RecordDate)
                .FirstOrDefault();

            if (record == null)
                throw new KeyNotFoundException($"Ingen sundhedsjournal fundet for dyr med ID: {animalId}");

            return Task.FromResult(record);
        }
    }
} 