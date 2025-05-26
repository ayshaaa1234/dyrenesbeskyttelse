using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClassLibrary.Features.AnimalManagement.Application.Abstractions;
using ClassLibrary.Features.AnimalManagement.Application.Models;
using ClassLibrary.Features.AnimalManagement.Core.Models;
using ClassLibrary.Features.AnimalManagement.Core.Enums;
using ClassLibrary.Features.AnimalManagement.Infrastructure.Abstractions;
using ClassLibrary.SharedKernel.Exceptions; // For RepositoryException, hvis det bruges direkte

namespace ClassLibrary.Features.AnimalManagement.Application.Implementations
{
    /// <summary>
    /// Implementering af IAnimalManagementService
    /// </summary>
    public class AnimalManagementService : IAnimalManagementService
    {
        private readonly IAnimalRepository _animalRepository;
        private readonly IHealthRecordRepository _healthRecordRepository;
        private readonly IVisitRepository _visitRepository;

        public AnimalManagementService(
            IAnimalRepository animalRepository,
            IHealthRecordRepository healthRecordRepository,
            IVisitRepository visitRepository)
        {
            _animalRepository = animalRepository ?? throw new ArgumentNullException(nameof(animalRepository));
            _healthRecordRepository = healthRecordRepository ?? throw new ArgumentNullException(nameof(healthRecordRepository));
            _visitRepository = visitRepository ?? throw new ArgumentNullException(nameof(visitRepository));
        }

        #region Animal Operations
        public async Task<IEnumerable<Animal>> GetAllAnimalsAsync()
        {
            return await _animalRepository.GetAllAsync();
        }

        public async Task<Animal?> GetAnimalByIdAsync(int id)
        {
            if (id <= 0) throw new ArgumentException("ID skal være større end 0.", nameof(id));
            return await _animalRepository.GetByIdAsync(id);
            // KeyNotFoundException håndteres typisk af Controller/Presenter laget, hvis null returneres.
        }

        public async Task<Animal> CreateAnimalAsync(Animal animal)
        {
            if (animal == null) throw new ArgumentNullException(nameof(animal));
            // Yderligere validering kan ske her før kald til repository
            return await _animalRepository.AddAsync(animal);
        }

        public async Task<Animal> UpdateAnimalAsync(Animal animal)
        {
            if (animal == null) throw new ArgumentNullException(nameof(animal));
            var existingAnimal = await GetAnimalByIdAsync(animal.Id);
            if (existingAnimal == null)
                throw new KeyNotFoundException($"Dyr med ID {animal.Id} blev ikke fundet.");
            // Yderligere validering kan ske her
            return await _animalRepository.UpdateAsync(animal);
        }

        public async Task DeleteAnimalAsync(int id)
        {
            if (id <= 0) throw new ArgumentException("ID skal være større end 0.", nameof(id));
            var animalToDelete = await GetAnimalByIdAsync(id);
            if (animalToDelete == null)
                throw new KeyNotFoundException($"Dyr med ID {id} blev ikke fundet for sletning.");
            await _animalRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<Animal>> GetAvailableAnimalsAsync()
        {
            return await _animalRepository.GetAvailableAnimalsAsync();
        }

        public async Task<IEnumerable<Animal>> GetAdoptedAnimalsAsync()
        {
            return await _animalRepository.GetAdoptedAnimalsAsync();
        }

        public async Task<IEnumerable<Animal>> GetAnimalsBySpeciesAsync(Species species)
        {
            return await _animalRepository.GetAnimalsBySpeciesAsync(species);
        }

        public async Task<IEnumerable<Animal>> GetAnimalsByAgeInYearsAsync(int age)
        {
            if (age < 0) throw new ArgumentOutOfRangeException(nameof(age), "Alder kan ikke være negativ.");
            return await _animalRepository.GetAnimalsByAgeInYearsAsync(age);
        }

        public async Task<IEnumerable<Animal>> GetAnimalsByAgeInMonthsAsync(int ageInMonths) // Opdateret parameter
        {
            if (ageInMonths < 0) throw new ArgumentOutOfRangeException(nameof(ageInMonths), "Alder kan ikke være negativ.");
            return await _animalRepository.GetAnimalsByAgeInMonthsAsync(ageInMonths);
        }

        public async Task<IEnumerable<Animal>> GetAnimalsByAgeInWeeksAsync(int ageInWeeks) // Opdateret parameter
        {
            if (ageInWeeks < 0) throw new ArgumentOutOfRangeException(nameof(ageInWeeks), "Alder kan ikke være negativ.");
            return await _animalRepository.GetAnimalsByAgeInWeeksAsync(ageInWeeks);
        }

        public async Task<IEnumerable<Animal>> GetAnimalsByGenderAsync(Gender gender)
        {
            return await _animalRepository.GetAnimalsByGenderAsync(gender);
        }

        public async Task<IEnumerable<Animal>> GetAnimalsByWeightRangeAsync(decimal minWeight, decimal maxWeight)
        {
            if (minWeight < 0 || maxWeight < 0) throw new ArgumentOutOfRangeException("Vægt kan ikke være negativ.");
            if (minWeight > maxWeight) throw new ArgumentException("Minimumsvægt kan ikke være større end maksimumsvægt.");
            return await _animalRepository.GetAnimalsByWeightRangeAsync(minWeight, maxWeight);
        }

        public async Task<IEnumerable<Animal>> GetAnimalsByAgeRangeInYearsAsync(int minAge, int maxAge)
        {
            if (minAge < 0 || maxAge < 0) throw new ArgumentOutOfRangeException("Alder kan ikke være negativ.");
            if (minAge > maxAge) throw new ArgumentException("Minimumsalder kan ikke være større end maksimumsalder.");
            return await _animalRepository.GetAnimalsByAgeRangeInYearsAsync(minAge, maxAge);
        }

        public async Task<IEnumerable<Animal>> GetAnimalsByAgeRangeInMonthsAsync(int minAgeInMonths, int maxAgeInMonths)
        {
            if (minAgeInMonths < 0 || maxAgeInMonths < 0) throw new ArgumentOutOfRangeException("Alder kan ikke være negativ.");
            if (minAgeInMonths > maxAgeInMonths) throw new ArgumentException("Minimumsalder kan ikke være større end maksimumsalder.");
            return await _animalRepository.GetAnimalsByAgeRangeInMonthsAsync(minAgeInMonths, maxAgeInMonths);
        }

        public async Task<IEnumerable<Animal>> GetAnimalsByAgeRangeInWeeksAsync(int minAgeInWeeks, int maxAgeInWeeks)
        {
            if (minAgeInWeeks < 0 || maxAgeInWeeks < 0) throw new ArgumentOutOfRangeException("Alder kan ikke være negativ.");
            if (minAgeInWeeks > maxAgeInWeeks) throw new ArgumentException("Minimumsalder kan ikke være større end maksimumsalder.");
            return await _animalRepository.GetAnimalsByAgeRangeInWeeksAsync(minAgeInWeeks, maxAgeInWeeks);
        }

        public async Task<IEnumerable<Animal>> GetAnimalsByAdoptionStatusAsync(AnimalStatus status)
        {
            return await _animalRepository.GetAnimalsByAdoptionStatusAsync(status);
        }

        public async Task<IEnumerable<Animal>> GetAnimalsByIntakeDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate) throw new ArgumentException("Startdato kan ikke være efter slutdato.");
            return await _animalRepository.GetAnimalsByIntakeDateRangeAsync(startDate, endDate);
        }

        public async Task<IEnumerable<Animal>> GetAnimalsByNameAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return await GetAllAnimalsAsync(); // Eller anden logik
            return await _animalRepository.GetAnimalsByNameAsync(name);
        }

        public async Task<IEnumerable<Animal>> GetAnimalsByBreedAsync(string breed)
        {
            if (string.IsNullOrWhiteSpace(breed)) return await GetAllAnimalsAsync(); // Eller anden logik
            return await _animalRepository.GetAnimalsByBreedAsync(breed);
        }

        public async Task<IEnumerable<Animal>> GetAnimalsByIdsAsync(IEnumerable<int> ids)
        {
            if (ids == null || !ids.Any())
            {
                return Enumerable.Empty<Animal>();
            }
            return await _animalRepository.GetAnimalsByIdsAsync(ids); // Forudsætter denne metode findes på IAnimalRepository
        }
        #endregion

        #region Health Record Operations
        public async Task<HealthRecord> AddHealthRecordAsync(int animalId, HealthRecord record)
        {
            if (record == null) throw new ArgumentNullException(nameof(record));
            if (animalId <= 0) throw new ArgumentException("Animal ID skal være større end 0.", nameof(animalId));
            
            var animal = await GetAnimalByIdAsync(animalId);
            if (animal == null)
                throw new KeyNotFoundException($"Dyr med ID {animalId} blev ikke fundet for at tilføje sundhedsjournal.");
            
            record.AnimalId = animalId;
            // Yderligere validering af record her
            return await _healthRecordRepository.AddAsync(record);
        }

        public async Task<HealthRecord> UpdateHealthRecordAsync(HealthRecord record)
        {
            if (record == null) throw new ArgumentNullException(nameof(record));
            if (record.Id <= 0) throw new ArgumentException("HealthRecord ID skal være større end 0 for opdatering.", nameof(record.Id));

            var existingRecord = await _healthRecordRepository.GetByIdAsync(record.Id);
            if (existingRecord == null)
                throw new KeyNotFoundException($"Sundhedsjournal med ID {record.Id} blev ikke fundet.");

            // Sørg for at AnimalId ikke ændres utilsigtet, eller valider hvis det er tilladt
            if(existingRecord.AnimalId != record.AnimalId)
            {
                // Håndter dette scenarie - enten forbyd eller valider det nye AnimalId
                throw new InvalidOperationException($"Det er ikke tilladt at ændre AnimalId for en eksisterende sundhedsjournal. Eksisterende: {existingRecord.AnimalId}, Forsøgt: {record.AnimalId}");
            }
            
            // Yderligere validering af record her
            return await _healthRecordRepository.UpdateAsync(record);
        }
        
        public async Task DeleteHealthRecordAsync(int healthRecordId)
        {
            if (healthRecordId <= 0) throw new ArgumentException("HealthRecord ID skal være større end 0.", nameof(healthRecordId));
            var recordToDelete = await _healthRecordRepository.GetByIdAsync(healthRecordId);
            if (recordToDelete == null)
                throw new KeyNotFoundException($"Sundhedsjournal med ID {healthRecordId} blev ikke fundet for sletning.");
            await _healthRecordRepository.DeleteAsync(healthRecordId);
        }

        public async Task<HealthRecord?> GetHealthRecordByIdAsync(int healthRecordId)
        {
            if (healthRecordId <= 0) throw new ArgumentException("HealthRecord ID skal være større end 0.", nameof(healthRecordId));
            return await _healthRecordRepository.GetByIdAsync(healthRecordId);
        }

        public async Task<IEnumerable<HealthRecord>> GetHealthRecordsByAnimalIdAsync(int animalId)
        {
            if (animalId <= 0) throw new ArgumentException("Animal ID skal være større end 0.", nameof(animalId));
            // Valider om dyret eksisterer først
            var animal = await GetAnimalByIdAsync(animalId);
            if (animal == null) 
                 throw new KeyNotFoundException($"Dyr med ID {animalId} blev ikke fundet.");
            return await _healthRecordRepository.GetHealthRecordsByAnimalIdAsync(animalId);
        }

        public async Task<HealthRecord?> GetLatestHealthRecordForAnimalAsync(int animalId)
        {
            if (animalId <= 0) throw new ArgumentException("Animal ID skal være større end 0.", nameof(animalId));
            var animal = await GetAnimalByIdAsync(animalId);
            if (animal == null) 
                 throw new KeyNotFoundException($"Dyr med ID {animalId} blev ikke fundet.");
            return await _healthRecordRepository.GetLatestHealthRecordForAnimalAsync(animalId);
        }

        public async Task<HealthRecord> RegisterOrUpdateVaccinationAsync(int animalId, DateTime vaccinationDate, DateTime? nextVaccinationDate, string notes)
        {
            if (animalId <= 0) throw new ArgumentException("Animal ID skal være større end 0.", nameof(animalId));
            var animal = await GetAnimalByIdAsync(animalId);
            if (animal == null)
                throw new KeyNotFoundException($"Dyr med ID {animalId} blev ikke fundet.");

            // Prøv at finde en eksisterende vaccinations-record for dagen, eller seneste generelle.
            // Dette er en forsimpling. En mere robust løsning kunne se efter specifikke vaccinations-typer.
            var latestRecord = await GetLatestHealthRecordForAnimalAsync(animalId); 

            if (latestRecord != null && latestRecord.RecordDate.Date == vaccinationDate.Date && latestRecord.IsVaccinated)
            {
                // Opdater eksisterende record for samme dag hvis det er en vaccination
                latestRecord.NextVaccinationDate = nextVaccinationDate;
                latestRecord.Notes = string.IsNullOrWhiteSpace(notes) ? latestRecord.Notes : $"{latestRecord.Notes}\n{notes}".Trim();
                latestRecord.UpdatedAt = DateTime.Now;
                return await UpdateHealthRecordAsync(latestRecord);
            }
            else
            {
                // Opret ny record
                var newRecord = new HealthRecord
                {
                    AnimalId = animalId,
                    RecordDate = vaccinationDate,
                    IsVaccinated = true,
                    NextVaccinationDate = nextVaccinationDate,
                    Notes = notes ?? "Vaccination registreret.",
                    Diagnosis = "Vaccination", // Standard diagnose
                    Treatment = "Vaccine givet", // Standard behandling
                    CreatedAt = DateTime.Now
                };
                return await AddHealthRecordAsync(animalId, newRecord);
            }
        }

        public async Task<IEnumerable<Animal>> GetAnimalsNeedingVaccinationAsync()
        {
            // Denne implementering vil kalde den nye metode i IAnimalRepository
            return await _animalRepository.GetAnimalsNeedingVaccinationAsync();
        }

        public async Task<IEnumerable<HealthRecord>> GetHealthRecordsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate) throw new ArgumentException("Startdato kan ikke være efter slutdato.");
            return await _healthRecordRepository.GetHealthRecordsByDateRangeAsync(startDate, endDate);
        }

        public async Task<IEnumerable<HealthRecord>> GetHealthRecordsByDiagnosisAsync(string diagnosis)
        {
            if (string.IsNullOrWhiteSpace(diagnosis)) return await _healthRecordRepository.GetAllAsync(); // Eller exception
            return await _healthRecordRepository.GetHealthRecordsByDiagnosisAsync(diagnosis);
        }

        public async Task<IEnumerable<HealthRecord>> GetHealthRecordsByTreatmentAsync(string treatment)
        {
             if (string.IsNullOrWhiteSpace(treatment)) return await _healthRecordRepository.GetAllAsync(); // Eller exception
            return await _healthRecordRepository.GetHealthRecordsByTreatmentAsync(treatment);
        }

        public async Task<IEnumerable<HealthRecord>> GetHealthRecordsByVaccinationStatusAsync(bool isVaccinated)
        {
            return await _healthRecordRepository.GetHealthRecordsByVaccinationStatusAsync(isVaccinated);
        }

        public async Task<IEnumerable<HealthRecord>> GetHealthRecordsByMedicationAsync(string medication)
        {
            if (string.IsNullOrWhiteSpace(medication)) return await _healthRecordRepository.GetAllAsync(); // Eller exception
            return await _healthRecordRepository.GetHealthRecordsByMedicationAsync(medication);
        }

        public async Task<IEnumerable<HealthRecord>> GetHealthRecordsBySeverityAsync(string severity)
        {
            if (string.IsNullOrWhiteSpace(severity)) return await _healthRecordRepository.GetAllAsync(); // Eller exception
            return await _healthRecordRepository.GetHealthRecordsBySeverityAsync(severity);
        }

        public async Task<IEnumerable<HealthRecord>> GetHealthRecordsByVeterinarianAsync(string veterinarianIdentifier)
        {
            if (string.IsNullOrWhiteSpace(veterinarianIdentifier)) return await _healthRecordRepository.GetAllAsync(); // Eller exception
            return await _healthRecordRepository.GetHealthRecordsByVeterinarianAsync(veterinarianIdentifier);
        }

        public async Task<HealthRecord> AddMedicationToHealthRecordAsync(int healthRecordId, string medication)
        {
            if (healthRecordId <= 0) throw new ArgumentException("HealthRecord ID skal være større end 0.", nameof(healthRecordId));
            if (string.IsNullOrWhiteSpace(medication)) throw new ArgumentException("Medicin må ikke være tomt.", nameof(medication));

            var record = await GetHealthRecordByIdAsync(healthRecordId);
            if (record == null)
                throw new KeyNotFoundException($"Sundhedsjournal med ID {healthRecordId} blev ikke fundet.");

            record.Medication = string.IsNullOrWhiteSpace(record.Medication) ? medication : $"{record.Medication}, {medication}";
            record.UpdatedAt = DateTime.Now;
            return await UpdateHealthRecordAsync(record);
        }

        public async Task<HealthRecord> UpdateHealthRecordSeverityAsync(int healthRecordId, string severity)
        {
            if (healthRecordId <= 0) throw new ArgumentException("HealthRecord ID skal være større end 0.", nameof(healthRecordId));
            if (string.IsNullOrWhiteSpace(severity)) throw new ArgumentException("Alvorlighedsgrad må ikke være tomt.", nameof(severity));
            
            var record = await GetHealthRecordByIdAsync(healthRecordId);
            if (record == null)
                throw new KeyNotFoundException($"Sundhedsjournal med ID {healthRecordId} blev ikke fundet.");

            record.Severity = severity;
            record.UpdatedAt = DateTime.Now;
            return await UpdateHealthRecordAsync(record);
        }
        #endregion

        #region Visit Operations
        public async Task<Visit> CreateVisitAsync(Visit visit)
        {
            if (visit == null) throw new ArgumentNullException(nameof(visit));
            if (visit.AnimalId <= 0) throw new ArgumentException("AnimalId skal være specificeret for et besøg.", nameof(visit.AnimalId));

            var animal = await GetAnimalByIdAsync(visit.AnimalId);
            if (animal == null)
                throw new KeyNotFoundException($"Dyr med ID {visit.AnimalId} blev ikke fundet for at oprette besøg.");
            
            // Yderligere validering af visit (f.eks. CustomerId, EmployeeId hvis påkrævet)
            return await _visitRepository.AddAsync(visit);
        }

        public async Task<Visit> UpdateVisitAsync(Visit visit)
        {
            if (visit == null) throw new ArgumentNullException(nameof(visit));
            if (visit.Id <= 0) throw new ArgumentException("Visit ID skal være større end 0 for opdatering.", nameof(visit.Id));

            var existingVisit = await GetVisitByIdAsync(visit.Id);
            if (existingVisit == null)
                throw new KeyNotFoundException($"Besøg med ID {visit.Id} blev ikke fundet.");

            // Sørg for at AnimalId ikke ændres utilsigtet, medmindre det er en tilladt operation
             if(existingVisit.AnimalId != visit.AnimalId)
            {
                 throw new InvalidOperationException($"Det er ikke tilladt at ændre AnimalId for et eksisterende besøg. Eksisterende: {existingVisit.AnimalId}, Forsøgt: {visit.AnimalId}");
            }
            // Yderligere validering
            return await _visitRepository.UpdateAsync(visit);
        }

        public async Task DeleteVisitAsync(int visitId)
        {
            if (visitId <= 0) throw new ArgumentException("Visit ID skal være større end 0.", nameof(visitId));
            var visitToDelete = await GetVisitByIdAsync(visitId);
            if (visitToDelete == null)
                throw new KeyNotFoundException($"Besøg med ID {visitId} blev ikke fundet for sletning.");
            await _visitRepository.DeleteAsync(visitId);
        }

        public async Task<Visit?> GetVisitByIdAsync(int visitId)
        {
            if (visitId <= 0) throw new ArgumentException("Visit ID skal være større end 0.", nameof(visitId));
            return await _visitRepository.GetByIdAsync(visitId);
        }

        public async Task<IEnumerable<Visit>> GetVisitsByAnimalIdAsync(int animalId)
        {
            if (animalId <= 0) throw new ArgumentException("Animal ID skal være større end 0.", nameof(animalId));
             var animal = await GetAnimalByIdAsync(animalId);
            if (animal == null) 
                 throw new KeyNotFoundException($"Dyr med ID {animalId} blev ikke fundet.");
            return await _visitRepository.GetVisitsByAnimalAsync(animalId);
        }

        public async Task<IEnumerable<Visit>> GetUpcomingVisitsForAnimalAsync(int animalId)
        {
            if (animalId <= 0) throw new ArgumentException("Animal ID skal være større end 0.", nameof(animalId));
            var animal = await GetAnimalByIdAsync(animalId);
            if (animal == null) 
                 throw new KeyNotFoundException($"Dyr med ID {animalId} blev ikke fundet.");

            var allVisits = await _visitRepository.GetVisitsByAnimalAsync(animalId);
            return allVisits.Where(v => v.PlannedDate > DateTime.Now && 
                                        v.Status != VisitStatus.Cancelled && 
                                        v.Status != VisitStatus.Completed);
        }

        public async Task<Visit> CancelVisitAsync(int visitId)
        {
            if (visitId <= 0) throw new ArgumentException("Visit ID skal være større end 0.", nameof(visitId));
            var visit = await GetVisitByIdAsync(visitId);
            if (visit == null)
                throw new KeyNotFoundException($"Besøg med ID {visitId} blev ikke fundet.");

            if (visit.Status == VisitStatus.Completed || visit.Status == VisitStatus.Cancelled)
                throw new InvalidOperationException($"Besøg med status '{visit.Status}' kan ikke aflyses.");

            visit.Status = VisitStatus.Cancelled;
            // visit.ActualDate = null; // Nulstil evt. ActualDate hvis relevant
            // visit.ActualDuration = null;
            return await UpdateVisitAsync(visit);
        }

        public async Task<Visit> CompleteVisitAsync(int visitId, DateTime actualDate, int actualDuration, string notes)
        {
            if (visitId <= 0) throw new ArgumentException("Visit ID skal være større end 0.", nameof(visitId));
            if (actualDate == default(DateTime) || actualDate > DateTime.Now) throw new ArgumentException("Ugyldig gennemførselsdato.", nameof(actualDate));
            if (actualDuration <= 0) throw new ArgumentException("Faktisk varighed skal være positiv.", nameof(actualDuration));

            var visit = await GetVisitByIdAsync(visitId);
            if (visit == null)
                throw new KeyNotFoundException($"Besøg med ID {visitId} blev ikke fundet.");

            if (visit.Status == VisitStatus.Completed)
                throw new InvalidOperationException("Besøget er allerede gennemført.");
            if (visit.Status == VisitStatus.Cancelled)
                 throw new InvalidOperationException("Et aflyst besøg kan ikke markeres som gennemført.");


            visit.Status = VisitStatus.Completed;
            visit.ActualDate = actualDate;
            visit.ActualDuration = actualDuration;
            if (!string.IsNullOrWhiteSpace(notes))
            {
                visit.Notes = string.IsNullOrWhiteSpace(visit.Notes) ? notes : $"{visit.Notes}\n{notes}".Trim();
            }
            return await UpdateVisitAsync(visit);
        }

        public async Task<IEnumerable<Visit>> GetVisitsByCustomerIdAsync(int customerId)
        {
            // Her antages det, at Customer feature/repository findes og kan validere customerId
            // For nu, validerer vi kun > 0
            if (customerId <= 0) throw new ArgumentException("Customer ID skal være større end 0.", nameof(customerId));
            // Kald til CustomerRepository.GetByIdAsync(customerId) for at validere eksistens, hvis det implementeres
            return await _visitRepository.GetVisitsByCustomerAsync(customerId);
        }

        public async Task<IEnumerable<Visit>> GetVisitsByEmployeeIdAsync(int employeeId)
        {
            // Her antages det, at Employee feature/repository findes og kan validere employeeId
            if (employeeId <= 0) throw new ArgumentException("Employee ID skal være større end 0.", nameof(employeeId));
            // Kald til EmployeeRepository.GetByIdAsync(employeeId) for at validere eksistens, hvis det implementeres
            return await _visitRepository.GetVisitsByEmployeeAsync(employeeId);
        }

        public async Task<IEnumerable<Visit>> GetVisitsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate) throw new ArgumentException("Startdato kan ikke være efter slutdato.");
            return await _visitRepository.GetVisitsByDateRangeAsync(startDate, endDate);
        }

        public async Task<IEnumerable<Visit>> GetVisitsByStatusAsync(VisitStatus status)
        {
            return await _visitRepository.GetVisitsByStatusAsync(status);
        }

        public async Task<IEnumerable<Visit>> GetVisitsByTypeAsync(string type)
        {
            if (string.IsNullOrWhiteSpace(type)) return await _visitRepository.GetAllAsync(); // Eller exception
            return await _visitRepository.GetVisitsByTypeAsync(type);
        }
        
        public async Task<Visit?> GetLatestVisitForAnimalAsync(int animalId)
        {
            if (animalId <= 0) throw new ArgumentException("Animal ID skal være større end 0.", nameof(animalId));
            var animal = await GetAnimalByIdAsync(animalId);
            if (animal == null) 
                 throw new KeyNotFoundException($"Dyr med ID {animalId} blev ikke fundet.");
            return await _visitRepository.GetLatestVisitForAnimalAsync(animalId);
        }

        public async Task<Visit?> GetLatestVisitForCustomerAsync(int customerId)
        {
            if (customerId <= 0) throw new ArgumentException("Customer ID skal være større end 0.", nameof(customerId));
            // Valider kunde eksistens her om nødvendigt
            return await _visitRepository.GetLatestVisitForCustomerAsync(customerId);
        }

        public async Task<Visit> ConfirmVisitAsync(int visitId)
        {
            if (visitId <= 0) throw new ArgumentException("Visit ID skal være større end 0.", nameof(visitId));
            var visit = await GetVisitByIdAsync(visitId);
            if (visit == null)
                throw new KeyNotFoundException($"Besøg med ID {visitId} blev ikke fundet.");

            if (visit.Status != VisitStatus.Scheduled && visit.Status != VisitStatus.Waitlisted)
                throw new InvalidOperationException($"Kun besøg med status 'Scheduled' or 'Waitlisted' kan bekræftes. Nuværende status: '{visit.Status}'.");
            
            visit.Status = VisitStatus.Confirmed;
            return await UpdateVisitAsync(visit);
        }

        public async Task<Visit> WaitlistVisitAsync(int visitId)
        {
            if (visitId <= 0) throw new ArgumentException("Visit ID skal være større end 0.", nameof(visitId));
            var visit = await GetVisitByIdAsync(visitId);
            if (visit == null)
                throw new KeyNotFoundException($"Besøg med ID {visitId} blev ikke fundet.");
            
            // Yderligere logik for hvornår et besøg kan sættes på venteliste
            if (visit.Status != VisitStatus.Scheduled) // Eksempel: Kun planlagte kan komme på venteliste
                 throw new InvalidOperationException($"Kun besøg med status 'Scheduled' kan sættes på venteliste. Nuværende status: '{visit.Status}'.");

            visit.Status = VisitStatus.Waitlisted;
            return await UpdateVisitAsync(visit);
        }
        
        public async Task<Visit> UpdateVisitNotesAsync(int visitId, string notes)
        {
            if (visitId <= 0) throw new ArgumentException("Visit ID skal være større end 0.", nameof(visitId));
            var visit = await GetVisitByIdAsync(visitId);
            if (visit == null)
                throw new KeyNotFoundException($"Besøg med ID {visitId} blev ikke fundet.");

            visit.Notes = notes ?? string.Empty; // Tillad at nulstille noter med null
            return await UpdateVisitAsync(visit);
        }

        public async Task<IEnumerable<Visit>> GetAllVisitsAsync()
        {
            return await _visitRepository.GetAllAsync(); // Antager _visitRepository har GetAllAsync()
        }
        #endregion

        #region Combined Operations
        public async Task<AnimalHealthSummary?> GetAnimalHealthSummaryAsync(int animalId)
        {
            if (animalId <= 0) throw new ArgumentException("Animal ID skal være større end 0.", nameof(animalId));
            var animal = await GetAnimalByIdAsync(animalId);
            if (animal == null)
                return null; // Eller kast KeyNotFoundException hvis et summary altid forventes for et eksisterende dyr

            var latestRecord = await GetLatestHealthRecordForAnimalAsync(animalId);
            var visitsForAnimal = await GetVisitsByAnimalIdAsync(animalId);
            
            var upcomingVisits = visitsForAnimal
                .Where(v => v.PlannedDate > DateTime.Now && v.Status != VisitStatus.Cancelled && v.Status != VisitStatus.Completed)
                .OrderBy(v => v.PlannedDate)
                .ToList();
            
            var pastVisits = visitsForAnimal
                .Where(v => v.ActualDate.HasValue && v.ActualDate.Value <= DateTime.Now || v.Status == VisitStatus.Completed || v.Status == VisitStatus.Cancelled)
                .OrderByDescending(v => v.ActualDate ?? v.PlannedDate) // Sorter efter ActualDate hvis det findes, ellers PlannedDate
                .ToList();

            bool needsVaccination = false;
            if (latestRecord?.NextVaccinationDate.HasValue == true && latestRecord.NextVaccinationDate.Value.Date <= DateTime.Today)
            {
                needsVaccination = true;
            } else if (latestRecord == null || !latestRecord.IsVaccinated) {
                // Yderligere logik kan tilføjes her for at bestemme om dyret mangler vaccination
                // baseret på art, alder, eller manglende vaccinationshistorik.
                // For nu, hvis der ikke er en NextVaccinationDate i fremtiden, og den ikke er markeret vaccineret.
                // Eller hvis der slet ingen record er. Dette er en forsimpling.
                var speciesPolicy = animal.Species; // Eksempel på brug af art
                if (latestRecord == null) needsVaccination = true; // Antag at et dyr uden historik skal tjekkes for vaccination
            }


            return new AnimalHealthSummary
            {
                Animal = animal,
                LatestHealthRecord = latestRecord,
                NextVaccinationDate = latestRecord?.NextVaccinationDate,
                UpcomingVisits = upcomingVisits,
                PastVisits = pastVisits,
                NeedsVaccination = needsVaccination,
                HealthStatus = DetermineHealthStatus(animal, latestRecord), // Opdateret til at tage Animal
                HealthAlerts = GenerateHealthAlerts(animal, latestRecord, upcomingVisits) // Opdateret til at tage Animal
            };
        }
        
        // Hjælpemetoder fra gammelt repository, nu i service
        private string DetermineHealthStatus(Animal animal, HealthRecord? latestRecord)
        {
            if (animal.Status == AnimalStatus.Deceased) return "Afdød";
            if (animal.Status == AnimalStatus.Adopted) return "Adopteret";
            if (animal.Status == AnimalStatus.Reserved) return "Reserveret";

            if (latestRecord == null)
                return "Sundhedsstatus Ukendt (ingen journal)";

            if (latestRecord.NextVaccinationDate.HasValue && latestRecord.NextVaccinationDate.Value.Date <= DateTime.Today)
                return "Mangler vaccination";
            
            if (!string.IsNullOrWhiteSpace(latestRecord.Severity))
            {
                if (latestRecord.Severity.Contains("kritisk", StringComparison.OrdinalIgnoreCase)) return "Kritisk";
                if (latestRecord.Severity.Contains("alvorlig", StringComparison.OrdinalIgnoreCase)) return "Alvorlig";
            }

            if (!string.IsNullOrWhiteSpace(latestRecord.Diagnosis) && 
                (latestRecord.Diagnosis.Contains("syg", StringComparison.OrdinalIgnoreCase) || 
                 latestRecord.Diagnosis.Contains("skade", StringComparison.OrdinalIgnoreCase)))
                return "Under observation/behandling";

            if (animal.Status == AnimalStatus.InTreatment) return "Under behandling";
            
            return "Tilsyneladende Rask";
        }

        private List<string> GenerateHealthAlerts(Animal animal, HealthRecord? latestRecord, List<Visit> upcomingVisits)
        {
            var alerts = new List<string>();

            if (animal.Status == AnimalStatus.InTreatment)
                alerts.Add("Dyret er markeret som 'Under behandling'.");

            if (latestRecord?.NextVaccinationDate.HasValue == true && latestRecord.NextVaccinationDate.Value.Date <= DateTime.Today)
                alerts.Add($"Mangler vaccination (Næste: {latestRecord.NextVaccinationDate.Value:dd-MM-yyyy})");
            else if (latestRecord == null || !latestRecord.IsVaccinated)
                 alerts.Add("Vaccinationsstatus ukendt eller mangler primær vaccination.");


            if (!string.IsNullOrWhiteSpace(latestRecord?.Severity))
            {
                 if (latestRecord.Severity.Contains("kritisk", StringComparison.OrdinalIgnoreCase))
                    alerts.Add($"Kritisk sundhedstilstand noteret: {latestRecord.Diagnosis}");
                 else if (latestRecord.Severity.Contains("alvorlig", StringComparison.OrdinalIgnoreCase))
                    alerts.Add($"Alvorlig sundhedstilstand noteret: {latestRecord.Diagnosis}");
            }
            
            if (!string.IsNullOrWhiteSpace(latestRecord?.Diagnosis) && 
                !latestRecord.Diagnosis.Equals("Vaccination", StringComparison.OrdinalIgnoreCase) &&
                !latestRecord.Diagnosis.Equals("Rutinetjek", StringComparison.OrdinalIgnoreCase))
            {
                 alerts.Add($"Seneste diagnose: {latestRecord.Diagnosis}");
            }


            var upcomingVetVisits = upcomingVisits.Where(v => v.IsVeterinaryVisit || v.Type.Equals("Veterinær", StringComparison.OrdinalIgnoreCase)).ToList();
            if (upcomingVetVisits.Any())
                alerts.Add($"{upcomingVetVisits.Count} kommende dyrlægebesøg planlagt.");

            if (!alerts.Any())
                alerts.Add("Ingen umiddelbare sundhedsalarmer.");

            return alerts;
        }


        public async Task<IEnumerable<Animal>> GetAnimalsWithVetAppointmentsInDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate) throw new ArgumentException("Startdato kan ikke være efter slutdato.");

            var visitsInDateRange = await _visitRepository.GetVisitsByDateRangeAsync(startDate,endDate);
            var vetVisitAnimalIds = visitsInDateRange
                .Where(v => (v.IsVeterinaryVisit || v.Type.Equals("Veterinær", StringComparison.OrdinalIgnoreCase)) 
                            && v.Status != VisitStatus.Cancelled)
                .Select(v => v.AnimalId)
                .Distinct();

            var animals = new List<Animal>();
            foreach (var animalId in vetVisitAnimalIds)
            {
                var animal = await _animalRepository.GetByIdAsync(animalId);
                if (animal != null) // Dobbeltcheck at dyret stadig eksisterer og ikke er soft-deleted
                {
                    animals.Add(animal);
                }
            }
            return animals.AsEnumerable();
        }
        #endregion
    }
} 