using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClassLibrary.Models;
using ClassLibrary.Interfaces;

namespace ClassLibrary.Services
{
    /// <summary>
    /// Service til håndtering af sundhedsjournaler
    /// </summary>
    public class HealthRecordService : IHealthRecordService
    {
        private readonly IHealthRecordRepository _healthRecordRepository;

        /// <summary>
        /// Konstruktør
        /// </summary>
        public HealthRecordService(IHealthRecordRepository healthRecordRepository)
        {
            _healthRecordRepository = healthRecordRepository ?? throw new ArgumentNullException(nameof(healthRecordRepository));
        }

        /// <summary>
        /// Henter alle sundhedsjournaler
        /// </summary>
        public async Task<IEnumerable<HealthRecord>> GetAllHealthRecordsAsync()
        {
            return await _healthRecordRepository.GetAllAsync();
        }

        /// <summary>
        /// Henter en sundhedsjournal baseret på ID
        /// </summary>
        public async Task<HealthRecord> GetHealthRecordByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("ID skal være større end 0");

            var healthRecord = await _healthRecordRepository.GetByIdAsync(id);
            if (healthRecord == null)
                throw new KeyNotFoundException($"Ingen sundhedsjournal fundet med ID: {id}");

            return healthRecord;
        }

        /// <summary>
        /// Opretter en ny sundhedsjournal
        /// </summary>
        public async Task<HealthRecord> CreateHealthRecordAsync(HealthRecord healthRecord)
        {
            if (healthRecord == null)
                throw new ArgumentNullException(nameof(healthRecord));

            ValidateHealthRecord(healthRecord);
            return await _healthRecordRepository.AddAsync(healthRecord);
        }

        /// <summary>
        /// Opdaterer en eksisterende sundhedsjournal
        /// </summary>
        public async Task<HealthRecord> UpdateHealthRecordAsync(HealthRecord healthRecord)
        {
            if (healthRecord == null)
                throw new ArgumentNullException(nameof(healthRecord));

            ValidateHealthRecord(healthRecord);
            return await _healthRecordRepository.UpdateAsync(healthRecord);
        }

        /// <summary>
        /// Sletter en sundhedsjournal
        /// </summary>
        public async Task DeleteHealthRecordAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("ID skal være større end 0");

            await _healthRecordRepository.DeleteAsync(id);
        }

        /// <summary>
        /// Henter sundhedsjournaler for et bestemt dyr
        /// </summary>
        public async Task<IEnumerable<HealthRecord>> GetHealthRecordsByAnimalIdAsync(int animalId)
        {
            if (animalId <= 0)
                throw new ArgumentException("AnimalId skal være større end 0");

            return await _healthRecordRepository.GetByAnimalIdAsync(animalId);
        }

        /// <summary>
        /// Henter sundhedsjournaler baseret på datointerval
        /// </summary>
        public async Task<IEnumerable<HealthRecord>> GetHealthRecordsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
                throw new ArgumentException("Startdato skal være før slutdato");

            if (startDate > DateTime.Now)
                throw new ArgumentException("Startdato kan ikke være i fremtiden");

            return await _healthRecordRepository.GetByDateRangeAsync(startDate, endDate);
        }

        /// <summary>
        /// Henter sundhedsjournaler baseret på diagnose
        /// </summary>
        public async Task<IEnumerable<HealthRecord>> GetHealthRecordsByDiagnosisAsync(string diagnosis)
        {
            if (string.IsNullOrWhiteSpace(diagnosis))
                throw new ArgumentException("Diagnose kan ikke være tom");

            return await _healthRecordRepository.GetByDiagnosisAsync(diagnosis);
        }

        /// <summary>
        /// Henter sundhedsjournaler baseret på behandling
        /// </summary>
        public async Task<IEnumerable<HealthRecord>> GetHealthRecordsByTreatmentAsync(string treatment)
        {
            if (string.IsNullOrWhiteSpace(treatment))
                throw new ArgumentException("Behandling kan ikke være tom");

            return await _healthRecordRepository.GetByTreatmentAsync(treatment);
        }

        /// <summary>
        /// Henter sundhedsjournaler baseret på vaccinationsstatus
        /// </summary>
        public async Task<IEnumerable<HealthRecord>> GetHealthRecordsByVaccinationStatusAsync(bool isVaccinated)
        {
            return await _healthRecordRepository.GetByVaccinationStatusAsync(isVaccinated);
        }

        /// <summary>
        /// Henter sundhedsjournaler baseret på medicin
        /// </summary>
        public async Task<IEnumerable<HealthRecord>> GetHealthRecordsByMedicationAsync(string medication)
        {
            if (string.IsNullOrWhiteSpace(medication))
                throw new ArgumentException("Medicin kan ikke være tom");

            return await _healthRecordRepository.GetByMedicationAsync(medication);
        }

        /// <summary>
        /// Henter sundhedsjournaler baseret på alvorlighedsgrad
        /// </summary>
        public async Task<IEnumerable<HealthRecord>> GetHealthRecordsBySeverityAsync(string severity)
        {
            if (string.IsNullOrWhiteSpace(severity))
                throw new ArgumentException("Alvorlighedsgrad kan ikke være tom");

            return await _healthRecordRepository.GetBySeverityAsync(severity);
        }

        /// <summary>
        /// Henter sundhedsjournaler baseret på dyrlæge
        /// </summary>
        public async Task<IEnumerable<HealthRecord>> GetHealthRecordsByVeterinarianAsync(string veterinarian)
        {
            if (string.IsNullOrWhiteSpace(veterinarian))
                throw new ArgumentException("Dyrlæge kan ikke være tom");

            return await _healthRecordRepository.GetByVeterinarianAsync(veterinarian);
        }

        /// <summary>
        /// Henter sundhedsjournaler for dyr der mangler vaccination
        /// </summary>
        public async Task<IEnumerable<HealthRecord>> GetHealthRecordsNeedingVaccinationAsync()
        {
            return await _healthRecordRepository.GetRecordsNeedingVaccinationAsync();
        }

        /// <summary>
        /// Henter den seneste sundhedsjournal for et dyr
        /// </summary>
        public async Task<HealthRecord> GetLatestHealthRecordForAnimalAsync(int animalId)
        {
            if (animalId <= 0)
                throw new ArgumentException("AnimalId skal være større end 0");

            return await _healthRecordRepository.GetLatestRecordForAnimalAsync(animalId);
        }

        /// <summary>
        /// Registrerer en vaccination for et dyr
        /// </summary>
        public async Task RegisterVaccinationAsync(int healthRecordId, DateTime nextVaccinationDate)
        {
            if (healthRecordId <= 0)
                throw new ArgumentException("HealthRecordId skal være større end 0");

            if (nextVaccinationDate <= DateTime.Now)
                throw new ArgumentException("Næste vaccinationsdato skal være i fremtiden");

            var healthRecord = await GetHealthRecordByIdAsync(healthRecordId);
            healthRecord.IsVaccinated = true;
            healthRecord.NextVaccinationDate = nextVaccinationDate;
            await _healthRecordRepository.UpdateAsync(healthRecord);
        }

        /// <summary>
        /// Tilføjer medicin til en sundhedsjournal
        /// </summary>
        public async Task AddMedicationAsync(int healthRecordId, string medication)
        {
            if (healthRecordId <= 0)
                throw new ArgumentException("HealthRecordId skal være større end 0");

            if (string.IsNullOrWhiteSpace(medication))
                throw new ArgumentException("Medicin kan ikke være tom");

            var healthRecord = await GetHealthRecordByIdAsync(healthRecordId);
            if (!string.IsNullOrEmpty(healthRecord.Medication))
                healthRecord.Medication += ", ";
            healthRecord.Medication += medication;
            await _healthRecordRepository.UpdateAsync(healthRecord);
        }

        /// <summary>
        /// Opdaterer alvorlighedsgraden for en sundhedsjournal
        /// </summary>
        public async Task UpdateSeverityAsync(int healthRecordId, string severity)
        {
            if (healthRecordId <= 0)
                throw new ArgumentException("HealthRecordId skal være større end 0");

            if (string.IsNullOrWhiteSpace(severity))
                throw new ArgumentException("Alvorlighedsgrad kan ikke være tom");

            var healthRecord = await GetHealthRecordByIdAsync(healthRecordId);
            healthRecord.Severity = severity;
            await _healthRecordRepository.UpdateAsync(healthRecord);
        }

        /// <summary>
        /// Planlægger en ny aftale
        /// </summary>
        public async Task ScheduleAppointmentAsync(int healthRecordId, DateTime appointmentDate)
        {
            if (healthRecordId <= 0)
                throw new ArgumentException("HealthRecordId skal være større end 0");

            if (appointmentDate <= DateTime.Now)
                throw new ArgumentException("Aftaledato skal være i fremtiden");

            var healthRecord = await GetHealthRecordByIdAsync(healthRecordId);
            healthRecord.AppointmentDate = appointmentDate;
            await _healthRecordRepository.UpdateAsync(healthRecord);
        }

        /// <summary>
        /// Validerer en sundhedsjournal
        /// </summary>
        private void ValidateHealthRecord(HealthRecord healthRecord)
        {
            if (healthRecord.AnimalId <= 0)
                throw new ArgumentException("AnimalId skal være større end 0");

            if (string.IsNullOrWhiteSpace(healthRecord.VeterinarianName))
                throw new ArgumentException("Dyrlægenavn kan ikke være tomt");

            if (string.IsNullOrWhiteSpace(healthRecord.Diagnosis))
                throw new ArgumentException("Diagnose kan ikke være tom");

            if (string.IsNullOrWhiteSpace(healthRecord.Treatment))
                throw new ArgumentException("Behandling kan ikke være tom");

            if (healthRecord.RecordDate > DateTime.Now)
                throw new ArgumentException("Journaldato kan ikke være i fremtiden");

            if (healthRecord.AppointmentDate.HasValue && healthRecord.AppointmentDate.Value <= DateTime.Now)
                throw new ArgumentException("Aftaledato skal være i fremtiden");

            if (healthRecord.IsVaccinated && !healthRecord.NextVaccinationDate.HasValue)
                throw new ArgumentException("Næste vaccinationsdato skal angives når dyret er vaccineret");

            if (healthRecord.NextVaccinationDate.HasValue && healthRecord.NextVaccinationDate.Value <= DateTime.Now)
                throw new ArgumentException("Næste vaccinationsdato skal være i fremtiden");
        }
    }
} 