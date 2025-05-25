using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClassLibrary.Models;

namespace ClassLibrary.Interfaces
{
    /// <summary>
    /// Interface for repository til håndtering af sundhedsjournaler
    /// </summary>
    public interface IHealthRecordRepository : IRepository<HealthRecord>
    {
        /// <summary>
        /// Finder alle sundhedsjournaler for et bestemt dyr
        /// </summary>
        Task<IEnumerable<HealthRecord>> GetByAnimalIdAsync(int animalId);

        /// <summary>
        /// Finder sundhedsjournaler i et bestemt datointerval
        /// </summary>
        Task<IEnumerable<HealthRecord>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);

        /// <summary>
        /// Finder sundhedsjournaler baseret på diagnose
        /// </summary>
        Task<IEnumerable<HealthRecord>> GetByDiagnosisAsync(string diagnosis);

        /// <summary>
        /// Finder sundhedsjournaler baseret på behandling
        /// </summary>
        Task<IEnumerable<HealthRecord>> GetByTreatmentAsync(string treatment);

        /// <summary>
        /// Finder sundhedsjournaler baseret på vaccinationsstatus
        /// </summary>
        Task<IEnumerable<HealthRecord>> GetByVaccinationStatusAsync(bool isVaccinated);

        /// <summary>
        /// Finder sundhedsjournaler baseret på medicin
        /// </summary>
        Task<IEnumerable<HealthRecord>> GetByMedicationAsync(string medication);

        /// <summary>
        /// Finder sundhedsjournaler baseret på alvorlighedsgrad
        /// </summary>
        Task<IEnumerable<HealthRecord>> GetBySeverityAsync(string severity);

        /// <summary>
        /// Finder sundhedsjournaler for en bestemt dyrlæge
        /// </summary>
        Task<IEnumerable<HealthRecord>> GetByVeterinarianAsync(string veterinarian);

        /// <summary>
        /// Finder dyr der mangler vaccination
        /// </summary>
        Task<IEnumerable<HealthRecord>> GetRecordsNeedingVaccinationAsync();

        /// <summary>
        /// Finder den seneste sundhedsjournal for et dyr
        /// </summary>
        Task<HealthRecord> GetLatestRecordForAnimalAsync(int animalId);
    }
} 