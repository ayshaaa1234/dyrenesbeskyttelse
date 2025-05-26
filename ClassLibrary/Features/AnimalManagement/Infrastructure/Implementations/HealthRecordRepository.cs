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
    /// <summary>
    /// Repository til håndtering af sundhedsjournaldata for dyr.
    /// </summary>
    public class HealthRecordRepository : Repository<HealthRecord>, IHealthRecordRepository
    {
        /// <summary>
        /// Initialiserer en ny instans af <see cref="HealthRecordRepository"/> klassen.
        /// </summary>
        public HealthRecordRepository() : base(Path.Combine(JsonDataInitializer.CalculatedWorkspaceRoot, "Data", "Json", "healthrecords.json")) { }

        /// <summary>
        /// Tilføjer en ny sundhedsjournal asynkront.
        /// </summary>
        /// <param name="entity">Sundhedsjournalen der skal tilføjes.</param>
        /// <returns>Den tilføjede sundhedsjournal.</returns>
        /// <exception cref="ArgumentNullException">Kastes hvis entity er null.</exception>
        public override async Task<HealthRecord> AddAsync(HealthRecord entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            ValidateEntity(entity);
            return await base.AddAsync(entity);
        }
        
        /// <summary>
        /// Opdaterer en eksisterende sundhedsjournal asynkront.
        /// </summary>
        /// <param name="entity">Sundhedsjournalen med de opdaterede værdier.</param>
        /// <returns>Den opdaterede sundhedsjournal.</returns>
        /// <exception cref="ArgumentNullException">Kastes hvis entity er null.</exception>
        /// <exception cref="RepositoryException">Kastes hvis sundhedsjournalen med det angivne ID ikke findes.</exception>
        public override async Task<HealthRecord> UpdateAsync(HealthRecord entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            ValidateEntity(entity);

            var existingRecord = await base.GetByIdAsync(entity.Id); 
            if (existingRecord == null) 
                 throw new RepositoryException($"HealthRecord med ID {entity.Id} blev ikke fundet for opdatering.");
            
            return await base.UpdateAsync(entity); 
        }
        
        /// <summary>
        /// Validerer en sundhedsjournal entitet.
        /// </summary>
        /// <param name="entity">Sundhedsjournalen der skal valideres.</param>
        /// <exception cref="ArgumentException">Kastes ved diverse valideringsfejl relateret til AnimalId, RecordDate, Diagnose eller Veterinær information.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Kastes hvis RecordDate er i fremtiden.</exception>
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

        /// <summary>
        /// Henter alle sundhedsjournaler tilknyttet et specifikt dyr.
        /// </summary>
        /// <param name="animalId">ID på dyret.</param>
        /// <returns>En samling af sundhedsjournaler for det angivne dyr. Returnerer en tom samling, hvis AnimalId er ugyldigt.</returns>
        public async Task<IEnumerable<HealthRecord>> GetHealthRecordsByAnimalIdAsync(int animalId)
        {
            if (animalId <= 0) 
            {
                return Enumerable.Empty<HealthRecord>();
            }
            return await base.FindAsync(h => h.AnimalId == animalId);
        }

        /// <summary>
        /// Henter den seneste sundhedsjournal for et specifikt dyr.
        /// </summary>
        /// <param name="animalId">ID på dyret.</param>
        /// <returns>Den seneste sundhedsjournal, eller null hvis ingen findes eller AnimalId er ugyldigt.</returns>
        public async Task<HealthRecord?> GetLatestHealthRecordForAnimalAsync(int animalId)
        {
            if (animalId <= 0) 
            {
                return null;
            }
            var records = await base.FindAsync(h => h.AnimalId == animalId);
            return records.OrderByDescending(h => h.RecordDate).FirstOrDefault();
        }

        /// <summary>
        /// Henter sundhedsjournaler oprettet inden for et specificeret datointerval.
        /// </summary>
        /// <param name="startDate">Startdato for intervallet.</param>
        /// <param name="endDate">Slutdato for intervallet.</param>
        /// <returns>En samling af sundhedsjournaler inden for det angivne datointerval.</returns>
        /// <exception cref="ArgumentException">Kastes hvis startdato er efter slutdato.</exception>
        public async Task<IEnumerable<HealthRecord>> GetHealthRecordsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate) throw new ArgumentException("Startdato kan ikke være efter slutdato.");
            return await base.FindAsync(h => h.RecordDate.Date >= startDate.Date && h.RecordDate.Date <= endDate.Date);
        }

        /// <summary>
        /// Henter sundhedsjournaler baseret på en specifik diagnose.
        /// </summary>
        /// <param name="diagnosis">Diagnosen der søges efter (case-insensitive, delvis matchning).</param>
        /// <returns>En samling af matchende sundhedsjournaler. Returnerer en tom samling, hvis diagnosen er tom eller null.</returns>
        public async Task<IEnumerable<HealthRecord>> GetHealthRecordsByDiagnosisAsync(string diagnosis)
        {
            if (string.IsNullOrWhiteSpace(diagnosis)) return Enumerable.Empty<HealthRecord>();
            return await base.FindAsync(h => !string.IsNullOrWhiteSpace(h.Diagnosis) && h.Diagnosis.Contains(diagnosis, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Henter sundhedsjournaler baseret på en specifik behandling.
        /// </summary>
        /// <param name="treatment">Behandlingen der søges efter (case-insensitive, delvis matchning).</param>
        /// <returns>En samling af matchende sundhedsjournaler. Returnerer en tom samling, hvis behandlingen er tom eller null.</returns>
        public async Task<IEnumerable<HealthRecord>> GetHealthRecordsByTreatmentAsync(string treatment)
        {
            if (string.IsNullOrWhiteSpace(treatment)) return Enumerable.Empty<HealthRecord>();
            return await base.FindAsync(h => !string.IsNullOrWhiteSpace(h.Treatment) && h.Treatment.Contains(treatment, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Henter sundhedsjournaler baseret på om dyret er markeret som vaccineret i journalen.
        /// </summary>
        /// <param name="isVaccinated">True for vaccinerede, false for ikke-vaccinerede.</param>
        /// <returns>En samling af sundhedsjournaler, der matcher den angivne vaccinationsstatus.</returns>
        public async Task<IEnumerable<HealthRecord>> GetHealthRecordsByVaccinationStatusAsync(bool isVaccinated)
        {
            return await base.FindAsync(h => h.IsVaccinated == isVaccinated);
        }

        /// <summary>
        /// Henter sundhedsjournaler baseret på en specifik medicin.
        /// </summary>
        /// <param name="medication">Medicinen der søges efter (case-insensitive, delvis matchning).</param>
        /// <returns>En samling af matchende sundhedsjournaler. Returnerer en tom samling, hvis medicinen er tom eller null.</returns>
        public async Task<IEnumerable<HealthRecord>> GetHealthRecordsByMedicationAsync(string medication)
        {
            if (string.IsNullOrWhiteSpace(medication)) return Enumerable.Empty<HealthRecord>();
            return await base.FindAsync(h => !string.IsNullOrWhiteSpace(h.Medication) && h.Medication.Contains(medication, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Henter sundhedsjournaler baseret på en specifik alvorlighedsgrad.
        /// </summary>
        /// <param name="severity">Alvorlighedsgraden der søges efter (case-insensitive, eksakt matchning).</param>
        /// <returns>En samling af matchende sundhedsjournaler. Returnerer en tom samling, hvis alvorlighedsgraden er tom eller null.</returns>
        public async Task<IEnumerable<HealthRecord>> GetHealthRecordsBySeverityAsync(string severity)
        {
            if (string.IsNullOrWhiteSpace(severity)) return Enumerable.Empty<HealthRecord>();
            return await base.FindAsync(h => !string.IsNullOrWhiteSpace(h.Severity) && h.Severity.Equals(severity, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Henter sundhedsjournaler tilknyttet en specifik dyrlæge (baseret på navn eller ID).
        /// </summary>
        /// <param name="veterinarianIdentifier">ID eller navn på dyrlægen (case-insensitive søgning).</param>
        /// <returns>En samling af matchende sundhedsjournaler. Returnerer en tom samling, hvis identifikatoren er tom eller null.</returns>
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