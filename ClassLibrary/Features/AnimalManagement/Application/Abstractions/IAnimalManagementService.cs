using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClassLibrary.Features.AnimalManagement.Core.Models; // Opdateret
using ClassLibrary.Features.AnimalManagement.Core.Enums; // Opdateret
using ClassLibrary.Features.AnimalManagement.Application.Models; // For AnimalHealthSummary

namespace ClassLibrary.Features.AnimalManagement.Application.Abstractions // Opdateret
{
    /// <summary>
    /// Service til håndtering af dyr, deres sundhedsjournaler og besøg
    /// </summary>
    public interface IAnimalManagementService
    {
        #region Animal Operations
        /// <summary>
        /// Henter alle dyr
        /// </summary>
        Task<IEnumerable<Animal>> GetAllAnimalsAsync();

        /// <summary>
        /// Henter et dyr baseret på ID
        /// </summary>
        Task<Animal?> GetAnimalByIdAsync(int id); // Returnerer nullable Animal

        /// <summary>
        /// Opretter et nyt dyr
        /// </summary>
        Task<Animal> CreateAnimalAsync(Animal animal);

        /// <summary>
        /// Opdaterer et eksisterende dyr
        /// </summary>
        Task<Animal> UpdateAnimalAsync(Animal animal);

        /// <summary>
        /// Sletter et dyr
        /// </summary>
        Task DeleteAnimalAsync(int id);

        /// <summary>
        /// Henter alle tilgængelige dyr
        /// </summary>
        Task<IEnumerable<Animal>> GetAvailableAnimalsAsync();

        /// <summary>
        /// Henter alle adopterede dyr
        /// </summary>
        Task<IEnumerable<Animal>> GetAdoptedAnimalsAsync();

        /// <summary>
        /// Henter dyr baseret på dyreart
        /// </summary>
        Task<IEnumerable<Animal>> GetAnimalsBySpeciesAsync(Species species);

        /// <summary>
        /// Henter dyr baseret på alder i år
        /// </summary>
        Task<IEnumerable<Animal>> GetAnimalsByAgeInYearsAsync(int age);

        /// <summary>
        /// Henter dyr baseret på alder i måneder
        /// </summary>
        Task<IEnumerable<Animal>> GetAnimalsByAgeInMonthsAsync(int ageInMonths); // Matcher service implementering

        /// <summary>
        /// Henter dyr baseret på alder i uger
        /// </summary>
        Task<IEnumerable<Animal>> GetAnimalsByAgeInWeeksAsync(int ageInWeeks); // Matcher service implementering

        /// <summary>
        /// Henter dyr baseret på køn
        /// </summary>
        Task<IEnumerable<Animal>> GetAnimalsByGenderAsync(Gender gender); // Opdateret til Gender enum

        /// <summary>
        /// Henter dyr baseret på vægtinterval
        /// </summary>
        Task<IEnumerable<Animal>> GetAnimalsByWeightRangeAsync(decimal minWeight, decimal maxWeight);

        /// <summary>
        /// Henter dyr baseret på aldersinterval i år
        /// </summary>
        Task<IEnumerable<Animal>> GetAnimalsByAgeRangeInYearsAsync(int minAge, int maxAge);

        /// <summary>
        /// Henter dyr baseret på aldersinterval i måneder
        /// </summary>
        Task<IEnumerable<Animal>> GetAnimalsByAgeRangeInMonthsAsync(int minAgeInMonths, int maxAgeInMonths);

        /// <summary>
        /// Henter dyr baseret på aldersinterval i uger
        /// </summary>
        Task<IEnumerable<Animal>> GetAnimalsByAgeRangeInWeeksAsync(int minAgeInWeeks, int maxAgeInWeeks);

        /// <summary>
        /// Henter dyr baseret på adoptionsstatus
        /// </summary>
        Task<IEnumerable<Animal>> GetAnimalsByAdoptionStatusAsync(AnimalStatus status);

        /// <summary>
        /// Henter dyr baseret på indkomstdato
        /// </summary>
        Task<IEnumerable<Animal>> GetAnimalsByIntakeDateRangeAsync(DateTime startDate, DateTime endDate);

        /// <summary>
        /// Henter dyr baseret på navn
        /// </summary>
        Task<IEnumerable<Animal>> GetAnimalsByNameAsync(string name);

        /// <summary>
        /// Henter dyr baseret på race
        /// </summary>
        Task<IEnumerable<Animal>> GetAnimalsByBreedAsync(string breed);
        #endregion

        #region Health Record Operations
        /// <summary>
        /// Tilføjer en sundhedsjournal til et dyr
        /// </summary>
        Task<HealthRecord> AddHealthRecordAsync(int animalId, HealthRecord record);

        /// <summary>
        /// Opdaterer en eksisterende sundhedsjournal
        /// </summary>
        Task<HealthRecord> UpdateHealthRecordAsync(HealthRecord record);

        /// <summary>
        /// Sletter en sundhedsjournal
        /// </summary>
        Task DeleteHealthRecordAsync(int healthRecordId);
        
        /// <summary>
        /// Henter en sundhedsjournal baseret på ID
        /// </summary>
        Task<HealthRecord?> GetHealthRecordByIdAsync(int healthRecordId);

        /// <summary>
        /// Henter alle sundhedsjournaler for et dyr
        /// </summary>
        Task<IEnumerable<HealthRecord>> GetHealthRecordsByAnimalIdAsync(int animalId);

        /// <summary>
        /// Henter den seneste sundhedsjournal for et dyr
        /// </summary>
        Task<HealthRecord?> GetLatestHealthRecordForAnimalAsync(int animalId);

        /// <summary>
        /// Registrerer en vaccination for et dyr, opdaterer eksisterende eller opretter ny health record
        /// </summary>
        Task<HealthRecord> RegisterOrUpdateVaccinationAsync(int animalId, DateTime vaccinationDate, DateTime? nextVaccinationDate, string notes);
        
        /// <summary>
        /// Henter dyr der mangler vaccination
        /// </summary>
        Task<IEnumerable<Animal>> GetAnimalsNeedingVaccinationAsync();

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
        /// Henter sundhedsjournaler baseret på dyrlæge ID (eller navn)
        /// </summary>
        Task<IEnumerable<HealthRecord>> GetHealthRecordsByVeterinarianAsync(string veterinarianIdentifier);

        /// <summary>
        /// Tilføjer medicin til en sundhedsjournal
        /// </summary>
        Task<HealthRecord> AddMedicationToHealthRecordAsync(int healthRecordId, string medication);

        /// <summary>
        /// Opdaterer alvorlighedsgraden for en sundhedsjournal
        /// </summary>
        Task<HealthRecord> UpdateHealthRecordSeverityAsync(int healthRecordId, string severity);
        #endregion

        #region Visit Operations
        /// <summary>
        /// Planlægger et nyt besøg
        /// </summary>
        Task<Visit> CreateVisitAsync(Visit visit); // Omdøbt fra ScheduleVisitAsync for konsistens

        /// <summary>
        /// Opdaterer et eksisterende besøg
        /// </summary>
        Task<Visit> UpdateVisitAsync(Visit visit);
        
        /// <summary>
        /// Sletter et besøg
        /// </summary>
        Task DeleteVisitAsync(int visitId);

        /// <summary>
        /// Henter et besøg baseret på ID
        /// </summary>
        Task<Visit?> GetVisitByIdAsync(int visitId);

        /// <summary>
        /// Henter alle besøg for et dyr
        /// </summary>
        Task<IEnumerable<Visit>> GetVisitsByAnimalIdAsync(int animalId);

        /// <summary>
        /// Henter kommende besøg for et dyr (planlagt efter nuværende tidspunkt og ikke aflyst/gennemført)
        /// </summary>
        Task<IEnumerable<Visit>> GetUpcomingVisitsForAnimalAsync(int animalId);

        /// <summary>
        /// Aflyser et besøg
        /// </summary>
        Task<Visit> CancelVisitAsync(int visitId);

        /// <summary>
        /// Gennemfører et besøg
        /// </summary>
        Task<Visit> CompleteVisitAsync(int visitId, DateTime actualDate, int actualDuration, string notes);

        /// <summary>
        /// Henter besøg for en bestemt kunde (forventer CustomerId i Visit)
        /// </summary>
        Task<IEnumerable<Visit>> GetVisitsByCustomerIdAsync(int customerId);

        /// <summary>
        /// Henter besøg for en bestemt medarbejder (forventer EmployeeId i Visit)
        /// </summary>
        Task<IEnumerable<Visit>> GetVisitsByEmployeeIdAsync(int employeeId);

        /// <summary>
        /// Henter besøg baseret på datointerval (PlannedDate eller ActualDate)
        /// </summary>
        Task<IEnumerable<Visit>> GetVisitsByDateRangeAsync(DateTime startDate, DateTime endDate);

        /// <summary>
        /// Henter besøg baseret på status
        /// </summary>
        Task<IEnumerable<Visit>> GetVisitsByStatusAsync(VisitStatus status);

        /// <summary>
        /// Henter besøg baseret på type
        /// </summary>
        Task<IEnumerable<Visit>> GetVisitsByTypeAsync(string type);
        
        /// <summary>
        /// Henter det seneste besøg for et dyr (baseret på ActualDate eller PlannedDate)
        /// </summary>
        Task<Visit?> GetLatestVisitForAnimalAsync(int animalId);

        /// <summary>
        /// Henter det seneste besøg for en kunde
        /// </summary>
        Task<Visit?> GetLatestVisitForCustomerAsync(int customerId);

        /// <summary>
        /// Bekræfter et besøg
        /// </summary>
        Task<Visit> ConfirmVisitAsync(int visitId);

        /// <summary>
        /// Tilføjer et besøg til venteliste
        /// </summary>
        Task<Visit> WaitlistVisitAsync(int visitId);
        
        /// <summary>
        /// Opdaterer noter for et besøg
        /// </summary>
        Task<Visit> UpdateVisitNotesAsync(int visitId, string notes);
        #endregion

        #region Combined Operations
        /// <summary>
        /// Henter en samlet oversigt over et dyrs sundhed og besøg
        /// </summary>
        Task<AnimalHealthSummary?> GetAnimalHealthSummaryAsync(int animalId);

        /// <summary>
        /// Henter dyr der skal til lægen i en bestemt uge
        /// </summary>
        Task<IEnumerable<Animal>> GetAnimalsWithVetAppointmentsInDateRangeAsync(DateTime startDate, DateTime endDate);
        #endregion
    }
}
