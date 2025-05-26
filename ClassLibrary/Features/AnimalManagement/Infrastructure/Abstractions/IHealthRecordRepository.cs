using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClassLibrary.Features.AnimalManagement.Core.Models;
using ClassLibrary.SharedKernel.Persistence.Abstractions;

namespace ClassLibrary.Features.AnimalManagement.Infrastructure.Abstractions
{
    /// <summary>
    /// Interface for repository til håndtering af sundhedsjournaldata for dyr.
    /// </summary>
    public interface IHealthRecordRepository : IRepository<HealthRecord>
    {
        // AddAsync, GetByIdAsync, UpdateAsync, DeleteAsync arves fra IRepository<HealthRecord>
        // Men vi skal tilføje en specifik DeleteHealthRecordAsync, hvis den skal have anden logik end standard DeleteAsync(id)
        // For nu antager vi standard sletning, så vi kan fjerne den specifikke DeleteHealthRecordAsync.
        // Hvis der er specifik logik, skal den tilføjes her.

        /// <summary>
        /// Henter alle sundhedsjournaler tilknyttet et specifikt dyr.
        /// </summary>
        /// <param name="animalId">ID på dyret.</param>
        Task<IEnumerable<HealthRecord>> GetHealthRecordsByAnimalIdAsync(int animalId);
        /// <summary>
        /// Henter den seneste sundhedsjournal for et specifikt dyr.
        /// </summary>
        /// <param name="animalId">ID på dyret.</param>
        /// <returns>Den seneste sundhedsjournal, eller null hvis ingen findes.</returns>
        Task<HealthRecord?> GetLatestHealthRecordForAnimalAsync(int animalId);
        /// <summary>
        /// Henter sundhedsjournaler oprettet inden for et specificeret datointerval.
        /// </summary>
        /// <param name="startDate">Startdato for intervallet.</param>
        /// <param name="endDate">Slutdato for intervallet.</param>
        Task<IEnumerable<HealthRecord>> GetHealthRecordsByDateRangeAsync(DateTime startDate, DateTime endDate);
        /// <summary>
        /// Henter sundhedsjournaler baseret på en specifik diagnose (delvis matchning understøttes typisk).
        /// </summary>
        /// <param name="diagnosis">Diagnosen der søges efter.</param>
        Task<IEnumerable<HealthRecord>> GetHealthRecordsByDiagnosisAsync(string diagnosis);
        /// <summary>
        /// Henter sundhedsjournaler baseret på en specifik behandling (delvis matchning understøttes typisk).
        /// </summary>
        /// <param name="treatment">Behandlingen der søges efter.</param>
        Task<IEnumerable<HealthRecord>> GetHealthRecordsByTreatmentAsync(string treatment);
        /// <summary>
        /// Henter sundhedsjournaler baseret på om dyret er markeret som vaccineret i journalen.
        /// </summary>
        /// <param name="isVaccinated">True for vaccinerede, false for ikke-vaccinerede.</param>
        Task<IEnumerable<HealthRecord>> GetHealthRecordsByVaccinationStatusAsync(bool isVaccinated);
        /// <summary>
        /// Henter sundhedsjournaler baseret på en specifik medicin (delvis matchning understøttes typisk).
        /// </summary>
        /// <param name="medication">Medicinen der søges efter.</param>
        Task<IEnumerable<HealthRecord>> GetHealthRecordsByMedicationAsync(string medication);
        /// <summary>
        /// Henter sundhedsjournaler baseret på en specifik alvorlighedsgrad.
        /// </summary>
        /// <param name="severity">Alvorlighedsgraden der søges efter.</param>
        Task<IEnumerable<HealthRecord>> GetHealthRecordsBySeverityAsync(string severity);
        /// <summary>
        /// Henter sundhedsjournaler tilknyttet en specifik dyrlæge.
        /// </summary>
        /// <param name="veterinarianId">ID eller navn på dyrlægen.</param>
        Task<IEnumerable<HealthRecord>> GetHealthRecordsByVeterinarianAsync(string veterinarianId); 
        // Task<IEnumerable<Animal>> GetAnimalsNeedingVaccinationAsync(); // Flyttet til IAnimalRepository da det returnerer Animal
    }
} 