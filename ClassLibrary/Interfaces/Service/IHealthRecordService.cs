using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClassLibrary.Models;

namespace ClassLibrary.Interfaces
{
    /// <summary>
    /// Interface for service til håndtering af sundhedsjournaler
    /// </summary>
    public interface IHealthRecordService
    {
        /// <summary>
        /// Henter alle sundhedsjournaler
        /// </summary>
        Task<IEnumerable<HealthRecord>> GetAllHealthRecordsAsync();

        /// <summary>
        /// Henter en sundhedsjournal baseret på ID
        /// </summary>
        Task<HealthRecord> GetHealthRecordByIdAsync(int id);

        /// <summary>
        /// Opretter en ny sundhedsjournal
        /// </summary>
        Task<HealthRecord> CreateHealthRecordAsync(HealthRecord healthRecord);

        /// <summary>
        /// Opdaterer en eksisterende sundhedsjournal
        /// </summary>
        Task<HealthRecord> UpdateHealthRecordAsync(HealthRecord healthRecord);

        /// <summary>
        /// Sletter en sundhedsjournal
        /// </summary>
        Task DeleteHealthRecordAsync(int id);

        /// <summary>
        /// Henter sundhedsjournaler for et bestemt dyr
        /// </summary>
        Task<IEnumerable<HealthRecord>> GetHealthRecordsByAnimalIdAsync(int animalId);

        /// <summary>
        /// Henter sundhedsjournaler baseret på datointerval
        /// </summary>
        Task<IEnumerable<HealthRecord>> GetHealthRecordsByDateRangeAsync(DateTime startDate, DateTime endDate);

        /// <summary>
        /// Henter sundhedsjournaler baseret på diagnose
        /// </summary>
        Task<IEnumerable<HealthRecord>> GetHealthRecordsByDiagnosisAsync(string diagnosis);

        /// <summary>
        /// Henter sundhedsjournaler baseret på behandling
        /// </summary>
        Task<IEnumerable<HealthRecord>> GetHealthRecordsByTreatmentAsync(string treatment);

        /// <summary>
        /// Henter sundhedsjournaler baseret på vaccinationsstatus
        /// </summary>
        Task<IEnumerable<HealthRecord>> GetHealthRecordsByVaccinationStatusAsync(bool isVaccinated);

        /// <summary>
        /// Henter sundhedsjournaler baseret på medicin
        /// </summary>
        Task<IEnumerable<HealthRecord>> GetHealthRecordsByMedicationAsync(string medication);

        /// <summary>
        /// Henter sundhedsjournaler baseret på alvorlighedsgrad
        /// </summary>
        Task<IEnumerable<HealthRecord>> GetHealthRecordsBySeverityAsync(string severity);

        /// <summary>
        /// Henter sundhedsjournaler baseret på dyrlæge
        /// </summary>
        Task<IEnumerable<HealthRecord>> GetHealthRecordsByVeterinarianAsync(string veterinarian);

        /// <summary>
        /// Henter sundhedsjournaler for dyr der mangler vaccination
        /// </summary>
        Task<IEnumerable<HealthRecord>> GetHealthRecordsNeedingVaccinationAsync();

        /// <summary>
        /// Henter den seneste sundhedsjournal for et dyr
        /// </summary>
        Task<HealthRecord> GetLatestHealthRecordForAnimalAsync(int animalId);

        /// <summary>
        /// Registrerer en vaccination for et dyr
        /// </summary>
        Task RegisterVaccinationAsync(int healthRecordId, DateTime nextVaccinationDate);

        /// <summary>
        /// Tilføjer medicin til en sundhedsjournal
        /// </summary>
        Task AddMedicationAsync(int healthRecordId, string medication);

        /// <summary>
        /// Opdaterer alvorlighedsgraden for en sundhedsjournal
        /// </summary>
        Task UpdateSeverityAsync(int healthRecordId, string severity);

        /// <summary>
        /// Planlægger en ny aftale
        /// </summary>
        Task ScheduleAppointmentAsync(int healthRecordId, DateTime appointmentDate);
    }
} 