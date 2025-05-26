using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClassLibrary.Features.AnimalManagement.Core.Models;
using ClassLibrary.SharedKernel.Persistence.Abstractions;

namespace ClassLibrary.Features.AnimalManagement.Infrastructure.Abstractions
{
    public interface IHealthRecordRepository : IRepository<HealthRecord>
    {
        // AddAsync, GetByIdAsync, UpdateAsync, DeleteAsync arves fra IRepository<HealthRecord>
        // Men vi skal tilføje en specifik DeleteHealthRecordAsync, hvis den skal have anden logik end standard DeleteAsync(id)
        // For nu antager vi standard sletning, så vi kan fjerne den specifikke DeleteHealthRecordAsync.
        // Hvis der er specifik logik, skal den tilføjes her.

        Task<IEnumerable<HealthRecord>> GetHealthRecordsByAnimalIdAsync(int animalId);
        Task<HealthRecord?> GetLatestHealthRecordForAnimalAsync(int animalId);
        Task<IEnumerable<HealthRecord>> GetHealthRecordsByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<HealthRecord>> GetHealthRecordsByDiagnosisAsync(string diagnosis);
        Task<IEnumerable<HealthRecord>> GetHealthRecordsByTreatmentAsync(string treatment);
        Task<IEnumerable<HealthRecord>> GetHealthRecordsByVaccinationStatusAsync(bool isVaccinated);
        Task<IEnumerable<HealthRecord>> GetHealthRecordsByMedicationAsync(string medication);
        Task<IEnumerable<HealthRecord>> GetHealthRecordsBySeverityAsync(string severity);
        Task<IEnumerable<HealthRecord>> GetHealthRecordsByVeterinarianAsync(string veterinarianId); 
        // Task<IEnumerable<Animal>> GetAnimalsNeedingVaccinationAsync(); // Flyttet til IAnimalRepository da det returnerer Animal
    }
} 