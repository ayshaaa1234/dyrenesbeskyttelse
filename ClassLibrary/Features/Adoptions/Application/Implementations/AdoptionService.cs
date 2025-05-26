using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq; // Tilføjet for LINQ-metoder som Select, Contains
using ClassLibrary.Features.Adoptions.Application.Abstractions;
using ClassLibrary.Features.Adoptions.Core.Models;
using ClassLibrary.Features.Adoptions.Core.Enums;
using ClassLibrary.Features.Adoptions.Infrastructure.Abstractions;
using ClassLibrary.Features.AnimalManagement.Core.Models; 
using ClassLibrary.Features.AnimalManagement.Core.Enums; 
using ClassLibrary.Features.AnimalManagement.Infrastructure.Abstractions; 
using ClassLibrary.Features.Customers.Infrastructure.Abstractions; // Korrekt namespace for ICustomerRepository
using ClassLibrary.Features.Employees.Infrastructure.Abstractions; // Korrekt namespace for IEmployeeRepository
// Forventede fremtidige namespaces:
// using ClassLibrary.Features.Customers.Core.Models;
// using ClassLibrary.Features.Employees.Core.Models;

namespace ClassLibrary.Features.Adoptions.Application.Implementations
{
    /// <summary>
    /// Service til håndtering af adoptioner
    /// </summary>
    public class AdoptionService : IAdoptionService
    {
        private readonly IAdoptionRepository _adoptionRepository;
        private readonly IAnimalRepository _animalRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IEmployeeRepository _employeeRepository;

        /// <summary>
        /// Konstruktør
        /// </summary>
        public AdoptionService(
            IAdoptionRepository adoptionRepository,
            IAnimalRepository animalRepository, 
            ICustomerRepository customerRepository, 
            IEmployeeRepository employeeRepository) 
        {
            _adoptionRepository = adoptionRepository ?? throw new ArgumentNullException(nameof(adoptionRepository));
            _animalRepository = animalRepository ?? throw new ArgumentNullException(nameof(animalRepository));
            _customerRepository = customerRepository ?? throw new ArgumentNullException(nameof(customerRepository));
            _employeeRepository = employeeRepository ?? throw new ArgumentNullException(nameof(employeeRepository));
        }

        /// <summary>
        /// Henter alle adoptioner
        /// </summary>
        public async Task<IEnumerable<Adoption>> GetAllAdoptionsAsync()
        {
            return await _adoptionRepository.GetAllAsync();
        }

        /// <summary>
        /// Henter en adoption baseret på ID
        /// </summary>
        public async Task<Adoption> GetAdoptionByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("ID skal være større end 0", nameof(id));
            }
            var adoption = await _adoptionRepository.GetByIdAsync(id);
            if (adoption == null)
            {
                throw new KeyNotFoundException($"Adoption med ID {id} blev ikke fundet.");
            }
            return adoption;
        }

        /// <summary>
        /// Opretter en ny adoption
        /// </summary>
        public async Task<Adoption> CreateAdoptionAsync(Adoption adoption)
        {
            if (adoption == null)
                throw new ArgumentNullException(nameof(adoption));

            await ValidateAdoptionAsync(adoption, true); // true for isNew
            return await _adoptionRepository.AddAsync(adoption);
        }

        /// <summary>
        /// Opdaterer en eksisterende adoption
        /// </summary>
        public async Task<Adoption> UpdateAdoptionAsync(Adoption adoption)
        {
            if (adoption == null)
                throw new ArgumentNullException(nameof(adoption));
            
            // Sørg for at adoptionen findes før validering
            var existingAdoption = await _adoptionRepository.GetByIdAsync(adoption.Id);
            if (existingAdoption == null)
            {
                throw new KeyNotFoundException($"Adoption med ID {adoption.Id} blev ikke fundet for opdatering.");
            }
            // Bevar oprindelige datoer hvis de ikke må ændres via denne metode
            adoption.AdoptionDate = existingAdoption.AdoptionDate; 
            // adoption.ApprovalDate = existingAdoption.ApprovalDate; // Skal håndteres af Approve/Reject metoder
            // adoption.RejectionDate = existingAdoption.RejectionDate;
            // adoption.CompletionDate = existingAdoption.CompletionDate;

            await ValidateAdoptionAsync(adoption, false); // false for isNew
            return await _adoptionRepository.UpdateAsync(adoption);
        }

        /// <summary>
        /// Sletter en adoption
        /// </summary>
        public async Task DeleteAdoptionAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("ID skal være større end 0", nameof(id));
            
            var adoption = await _adoptionRepository.GetByIdAsync(id);
            if (adoption == null)
                throw new KeyNotFoundException($"Adoption med ID {id} blev ikke fundet for sletning.");
            
            // Yderligere forretningslogik for sletning (f.eks. kan man kun slette 'Pending' adoptioner?)
            // Hvis dyret var sat til 'Adopted' pga. denne adoption, skal det måske reserveres eller gøres ledigt.
            if (adoption.Status == AdoptionStatus.Approved || adoption.Status == AdoptionStatus.Completed)
            {
                 var animal = await _animalRepository.GetByIdAsync(adoption.AnimalId);
                 if(animal != null && animal.Status == AnimalManagement.Core.Enums.AnimalStatus.Adopted)
                 {
                    // Overvej at sætte status til Available eller Reserved baseret på forretningsregler
                    // animal.Status = AnimalManagement.Core.Enums.AnimalStatus.Available;
                    // await _animalRepository.UpdateAsync(animal);
                    Console.WriteLine($"INFO: Dyr ID {animal.Id} var adopteret via adoption ID {id}. Overvej at opdatere dyrets status manuelt efter sletning af adoption.");
                 }
            }

            await _adoptionRepository.DeleteAsync(id);
        }

        /// <summary>
        /// Henter adoptioner for en bestemt kunde
        /// </summary>
        public async Task<IEnumerable<Adoption>> GetAdoptionsByCustomerAsync(int customerId)
        {
            if (customerId <= 0)
                return Enumerable.Empty<Adoption>();
            return await _adoptionRepository.GetByCustomerIdAsync(customerId);
        }

        /// <summary>
        /// Henter adoptioner for et bestemt dyr
        /// </summary>
        public async Task<IEnumerable<Adoption>> GetAdoptionsByAnimalAsync(int animalId)
        {
            if (animalId <= 0)
                return Enumerable.Empty<Adoption>();
            return await _adoptionRepository.GetByAnimalIdAsync(animalId);
        }

        /// <summary>
        /// Henter adoptioner i et datointerval
        /// </summary>
        public async Task<IEnumerable<Adoption>> GetAdoptionsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
                throw new ArgumentException("Startdato skal være før slutdato");
            return await _adoptionRepository.GetByDateRangeAsync(startDate, endDate);
        }

        /// <summary>
        /// Henter adoptioner baseret på status
        /// </summary>
        public async Task<IEnumerable<Adoption>> GetAdoptionsByStatusAsync(AdoptionStatus status)
        {
            if (!Enum.IsDefined(typeof(AdoptionStatus), status))
                throw new ArgumentException("Ugyldig adoptionsstatus", nameof(status));
            return await _adoptionRepository.GetByStatusAsync(status);
        }

        /// <summary>
        /// Henter adoptioner baseret på dyreart
        /// </summary>
        public async Task<IEnumerable<Adoption>> GetAdoptionsBySpeciesAsync(AnimalManagement.Core.Enums.Species species)
        {
            var animalsOfSpecies = await _animalRepository.GetAnimalsBySpeciesAsync(species);
            if (animalsOfSpecies == null || !animalsOfSpecies.Any())
            {
                return Enumerable.Empty<Adoption>();
            }
            var animalIds = animalsOfSpecies.Select(a => a.Id).Distinct().ToList();
            if (!animalIds.Any()) return Enumerable.Empty<Adoption>();

            // Hent alle adoptioner og filtrer dem derefter baseret på dyrenes ID'er
            var allAdoptions = await _adoptionRepository.GetAllAsync(); 
            return allAdoptions.Where(ad => animalIds.Contains(ad.AnimalId));
        }

        /// <summary>
        /// Beregner alder i hele år baseret på fødselsdato og en given dato.
        /// </summary>
        /// <param name="birthDate">Fødselsdatoen.</param>
        /// <param name="onDate">Datoen alderen skal beregnes på.</param>
        /// <returns>Alder i hele år.</returns>
        private int CalculateAgeInYears(DateTime birthDate, DateTime onDate)
        {
            var age = onDate.Year - birthDate.Year;
            if (birthDate.Date > onDate.Date.AddYears(-age)) age--;
            return age;
        }
        
        /// <summary>
        /// Beregner alder i hele måneder baseret på fødselsdato og en given dato.
        /// </summary>
        /// <param name="birthDate">Fødselsdatoen.</param>
        /// <param name="onDate">Datoen alderen skal beregnes på.</param>
        /// <returns>Alder i hele måneder.</returns>
        private int CalculateAgeInMonths(DateTime birthDate, DateTime onDate)
        {
            var months = ((onDate.Year - birthDate.Year) * 12) + onDate.Month - birthDate.Month;
            if (onDate.Day < birthDate.Day)
            {
                months--;
            }
            return months;
        }

        /// <summary>
        /// Beregner alder i hele uger baseret på fødselsdato og en given dato.
        /// </summary>
        /// <param name="birthDate">Fødselsdatoen.</param>
        /// <param name="onDate">Datoen alderen skal beregnes på.</param>
        /// <returns>Alder i hele uger. Returnerer 0 hvis fødselsdato er efter den givne dato.</returns>
        private int CalculateAgeInWeeks(DateTime birthDate, DateTime onDate)
        {
            if (birthDate > onDate) return 0; // Eller kast exception
            return (int)(onDate.Date - birthDate.Date).TotalDays / 7;
        }

        public async Task<IEnumerable<Adoption>> GetAdoptionsByAgeInYearsAsync(int years)
        {
            if (years < 0) throw new ArgumentException("Alder kan ikke være negativ", nameof(years));

            var allAnimals = await _animalRepository.GetAllAsync();
            if(allAnimals == null || !allAnimals.Any()) return Enumerable.Empty<Adoption>();
            var today = DateTime.UtcNow;
            
            // Find dyr indenfor den specificerede alder i år
            var animalsInAgeRange = allAnimals.Where(a => a.BirthDate.HasValue && CalculateAgeInYears(a.BirthDate.Value, today) == years).ToList();
            if (!animalsInAgeRange.Any()) return Enumerable.Empty<Adoption>();

            var animalIds = animalsInAgeRange.Select(a => a.Id).Distinct().ToList();
            if (!animalIds.Any()) return Enumerable.Empty<Adoption>();

            // Hent alle adoptioner og filtrer dem derefter baseret på dyrenes ID'er
            var allAdoptions = await _adoptionRepository.GetAllAsync();
            return allAdoptions.Where(ad => animalIds.Contains(ad.AnimalId));
        }

        public async Task<IEnumerable<Adoption>> GetAdoptionsByAgeInMonthsAsync(int months)
        {
            if (months < 0) throw new ArgumentException("Alder i måneder kan ikke være negativ", nameof(months));

            var allAnimals = await _animalRepository.GetAllAsync();
            if(allAnimals == null || !allAnimals.Any()) return Enumerable.Empty<Adoption>();
            var today = DateTime.UtcNow;

            // Find dyr indenfor den specificerede alder i måneder
            var animalsInAgeRange = allAnimals.Where(a => a.BirthDate.HasValue && CalculateAgeInMonths(a.BirthDate.Value, today) == months).ToList();
            if (!animalsInAgeRange.Any()) return Enumerable.Empty<Adoption>();

            var animalIds = animalsInAgeRange.Select(a => a.Id).Distinct().ToList();
            if (!animalIds.Any()) return Enumerable.Empty<Adoption>();

            // Hent alle adoptioner og filtrer dem derefter baseret på dyrenes ID'er
            var allAdoptions = await _adoptionRepository.GetAllAsync();
            return allAdoptions.Where(ad => animalIds.Contains(ad.AnimalId));
        }

        public async Task<IEnumerable<Adoption>> GetAdoptionsByAgeInWeeksAsync(int weeks)
        {
            if (weeks < 0) throw new ArgumentException("Alder i uger kan ikke være negativ", nameof(weeks));
            
            var allAnimals = await _animalRepository.GetAllAsync();
            if(allAnimals == null || !allAnimals.Any()) return Enumerable.Empty<Adoption>();
            var today = DateTime.UtcNow;

            // Find dyr indenfor den specificerede alder i uger
            var animalsInAgeRange = allAnimals.Where(a => a.BirthDate.HasValue && CalculateAgeInWeeks(a.BirthDate.Value, today) == weeks).ToList();
            if (!animalsInAgeRange.Any()) return Enumerable.Empty<Adoption>();

            var animalIds = animalsInAgeRange.Select(a => a.Id).Distinct().ToList();
            if (!animalIds.Any()) return Enumerable.Empty<Adoption>();

            // Hent alle adoptioner og filtrer dem derefter baseret på dyrenes ID'er
            var allAdoptions = await _adoptionRepository.GetAllAsync();
            return allAdoptions.Where(ad => animalIds.Contains(ad.AnimalId));
        }

        public async Task<IEnumerable<Adoption>> GetAdoptionsByAgeRangeInYearsAsync(int minYears, int maxYears)
        {
            if (minYears < 0) throw new ArgumentException("Minimumsalder kan ikke være negativ", nameof(minYears));
            if (maxYears < minYears) throw new ArgumentException("Maksimumsalder skal være større end minimumsalder", nameof(maxYears));

            var allAnimals = await _animalRepository.GetAllAsync();
            if(allAnimals == null || !allAnimals.Any()) return Enumerable.Empty<Adoption>();
            var today = DateTime.UtcNow;
            
            // Find dyr indenfor det specificerede aldersinterval i år
            var animalsInAgeRange = allAnimals.Where(a => 
                a.BirthDate.HasValue && 
                CalculateAgeInYears(a.BirthDate.Value, today) >= minYears &&
                CalculateAgeInYears(a.BirthDate.Value, today) <= maxYears
            ).ToList();
            if (!animalsInAgeRange.Any()) return Enumerable.Empty<Adoption>();

            var animalIds = animalsInAgeRange.Select(a => a.Id).Distinct().ToList();
            if (!animalIds.Any()) return Enumerable.Empty<Adoption>();

            // Hent alle adoptioner og filtrer dem derefter baseret på dyrenes ID'er
            var allAdoptions = await _adoptionRepository.GetAllAsync();
            return allAdoptions.Where(ad => animalIds.Contains(ad.AnimalId));
        }

        public async Task<IEnumerable<Adoption>> GetAdoptionsByAgeRangeInMonthsAsync(int minMonths, int maxMonths)
        {
            if (minMonths < 0) throw new ArgumentException("Minimumsalder i måneder kan ikke være negativ", nameof(minMonths));
            if (maxMonths < minMonths) throw new ArgumentException("Maksimumsalder i måneder skal være større end minimumsalder", nameof(maxMonths));
            
            var allAnimals = await _animalRepository.GetAllAsync();
            if(allAnimals == null || !allAnimals.Any()) return Enumerable.Empty<Adoption>();
            var today = DateTime.UtcNow;

            // Find dyr indenfor det specificerede aldersinterval i måneder
            var animalsInAgeRange = allAnimals.Where(a => 
                a.BirthDate.HasValue &&
                CalculateAgeInMonths(a.BirthDate.Value, today) >= minMonths &&
                CalculateAgeInMonths(a.BirthDate.Value, today) <= maxMonths
            ).ToList();
            if (!animalsInAgeRange.Any()) return Enumerable.Empty<Adoption>();

            var animalIds = animalsInAgeRange.Select(a => a.Id).Distinct().ToList();
            if (!animalIds.Any()) return Enumerable.Empty<Adoption>();

            // Hent alle adoptioner og filtrer dem derefter baseret på dyrenes ID'er
            var allAdoptions = await _adoptionRepository.GetAllAsync();
            return allAdoptions.Where(ad => animalIds.Contains(ad.AnimalId));
        }

        public async Task<IEnumerable<Adoption>> GetAdoptionsByAgeRangeInWeeksAsync(int minWeeks, int maxWeeks)
        {
            if (minWeeks < 0) throw new ArgumentException("Minimumsalder i uger kan ikke være negativ", nameof(minWeeks));
            if (maxWeeks < minWeeks) throw new ArgumentException("Maksimumsalder i uger skal være større end minimumsalder", nameof(maxWeeks));

            var allAnimals = await _animalRepository.GetAllAsync();
            if(allAnimals == null || !allAnimals.Any()) return Enumerable.Empty<Adoption>();
            var today = DateTime.UtcNow;
            
            // Find dyr indenfor det specificerede aldersinterval i uger
            var animalsInAgeRange = allAnimals.Where(a => 
                a.BirthDate.HasValue &&
                CalculateAgeInWeeks(a.BirthDate.Value, today) >= minWeeks &&
                CalculateAgeInWeeks(a.BirthDate.Value, today) <= maxWeeks
            ).ToList();
            if (!animalsInAgeRange.Any()) return Enumerable.Empty<Adoption>();

            var animalIds = animalsInAgeRange.Select(a => a.Id).Distinct().ToList();
            if (!animalIds.Any()) return Enumerable.Empty<Adoption>();

            // Hent alle adoptioner og filtrer dem derefter baseret på dyrenes ID'er
            var allAdoptions = await _adoptionRepository.GetAllAsync();
            return allAdoptions.Where(ad => animalIds.Contains(ad.AnimalId));
        }
        
        public async Task<IEnumerable<Adoption>> GetAdoptionsByTypeAsync(string adoptionType)
        {
            if (string.IsNullOrWhiteSpace(adoptionType)) return Enumerable.Empty<Adoption>();
            return await _adoptionRepository.GetByAdoptionTypeAsync(adoptionType);
        }

        public async Task<IEnumerable<Adoption>> GetAdoptionsByEmployeeAsync(int employeeId)
        {
            if (employeeId <= 0) return Enumerable.Empty<Adoption>();
            return await _adoptionRepository.GetByEmployeeIdAsync(employeeId);
        }

        public async Task<IEnumerable<Adoption>> GetAdoptionsByAdopterEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return Enumerable.Empty<Adoption>();

            var customers = await _customerRepository.FindAsync(c => c.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
            if (customers == null || !customers.Any())
            {
                return Enumerable.Empty<Adoption>();
            }

            var allAdoptionsForCustomers = new List<Adoption>();
            foreach (var customer in customers)
            {
                var adoptions = await _adoptionRepository.GetByCustomerIdAsync(customer.Id);
                if (adoptions != null && adoptions.Any())
                {
                    allAdoptionsForCustomers.AddRange(adoptions);
                }
            }
            return allAdoptionsForCustomers.Distinct(); // Sikrer unikke adoptioner, hvis en eller anden grund skulle give dubletter
        }

        public async Task<Adoption> GetLatestAdoptionForAnimalAsync(int animalId)
        {
            if (animalId <= 0)
                 throw new ArgumentOutOfRangeException(nameof(animalId), "Animal ID skal være større end 0."); 
            return await _adoptionRepository.GetLatestAdoptionForAnimalAsync(animalId);
        }

        public async Task ApproveAdoptionAsync(int adoptionId, int employeeId)
        {
            var adoption = await GetAdoptionByIdAsync(adoptionId);
            if (adoption.Status != AdoptionStatus.Pending)
                throw new InvalidOperationException("Kun adoptioner med status 'Pending' kan godkendes.");
            
            var employee = await _employeeRepository.GetByIdAsync(employeeId);
            if (employee == null) throw new KeyNotFoundException($"Medarbejder med ID {employeeId} blev ikke fundet.");

            adoption.Status = AdoptionStatus.Approved;
            adoption.ApprovalDate = DateTime.UtcNow;
            adoption.EmployeeId = employeeId; // Sæt medarbejder ID for godkendelse
            // AdoptionDate bør sættes her eller når kunden bekræfter
            if(adoption.AdoptionDate == default(DateTime)) adoption.AdoptionDate = DateTime.UtcNow;

            await _adoptionRepository.UpdateAsync(adoption);
            
            var animal = await _animalRepository.GetByIdAsync(adoption.AnimalId); 
            if (animal != null) 
            {
                animal.Status = AnimalManagement.Core.Enums.AnimalStatus.Adopted;
                await _animalRepository.UpdateAsync(animal);
            }
        }

        public async Task RejectAdoptionAsync(int adoptionId, int employeeId)
        {
            var adoption = await GetAdoptionByIdAsync(adoptionId);
            if (adoption.Status != AdoptionStatus.Pending)
                throw new InvalidOperationException("Kun adoptioner med status 'Pending' kan afvises.");

            var employee = await _employeeRepository.GetByIdAsync(employeeId);
            if (employee == null) throw new KeyNotFoundException($"Medarbejder med ID {employeeId} blev ikke fundet.");

            adoption.Status = AdoptionStatus.Rejected;
            adoption.RejectionDate = DateTime.UtcNow;
            adoption.EmployeeId = employeeId; 
            await _adoptionRepository.UpdateAsync(adoption);
            
            // Hvis dyret var reserveret, skal det måske gøres ledigt igen
            var animal = await _animalRepository.GetByIdAsync(adoption.AnimalId);
            if (animal != null && animal.Status == AnimalManagement.Core.Enums.AnimalStatus.Reserved) 
            {
                animal.Status = AnimalManagement.Core.Enums.AnimalStatus.Available;
                await _animalRepository.UpdateAsync(animal);
            }
        }

        public async Task CompleteAdoptionAsync(int adoptionId)
        {
            var adoption = await _adoptionRepository.GetByIdAsync(adoptionId);
            if (adoption == null) throw new KeyNotFoundException($"Adoption med ID {adoptionId} blev ikke fundet.");
            if (adoption.Status != AdoptionStatus.Approved)
                throw new InvalidOperationException("Kun godkendte adoptioner kan afsluttes.");

            adoption.Status = AdoptionStatus.Completed;
            adoption.CompletionDate = DateTime.UtcNow;
            if(adoption.AdoptionDate == default(DateTime)) adoption.AdoptionDate = DateTime.UtcNow; // Sæt adoptionsdato hvis ikke allerede sat

            await _adoptionRepository.UpdateAsync(adoption);
            // Dyrets status er allerede 'Adopted' fra ApproveAdoptionAsync
        }
        
        /// <summary>
        /// Annullerer en godkendt eller afventende adoption.
        /// Opdaterer adoptionens status til Cancelled og tilføjer en note med årsagen.
        /// Hvis dyret var reserveret eller adopteret pga. denne adoption, sættes dyrets status tilbage til Available.
        /// </summary>
        /// <param name="adoptionId">ID på adoptionen der skal annulleres.</param>
        /// <param name="employeeId">ID på medarbejderen der foretager annulleringen.</param>
        /// <param name="reason">Årsag til annulleringen.</param>
        public async Task CancelAdoptionAsync(int adoptionId, int employeeId, string reason)
        {
            var adoption = await GetAdoptionByIdAsync(adoptionId); // Genbruger GetAdoptionByIdAsync for validering

            if (adoption.Status != AdoptionStatus.Pending && adoption.Status != AdoptionStatus.Approved)
            {
                throw new InvalidOperationException($"Kun adoptioner med status 'Pending' eller 'Approved' kan annulleres. Nuværende status: {adoption.Status}");
            }

            // Log hvem der annullerede
            adoption.EmployeeId = employeeId; 
            adoption.Status = AdoptionStatus.Cancelled;
            adoption.Notes = string.IsNullOrWhiteSpace(adoption.Notes) 
                ? $"Annulleret af medarbejder ID {employeeId}. Årsag: {reason}" 
                : adoption.Notes + $"\nAnnulleret af medarbejder ID {employeeId}. Årsag: {reason}";
            
            // Nulstil datoer der ikke længere er relevante
            adoption.ApprovalDate = null;
            adoption.RejectionDate = null; // Hvis den fejlagtigt var sat
            adoption.CompletionDate = null;
            // AdoptionDate skal muligvis også nulstilles, hvis den var sat ved Approved.
            // Hvis en adoption annulleres, er den ikke længere 'adopteret' på en specifik dato via denne proces.
            adoption.AdoptionDate = default(DateTime); 

            // Hvis dyret var reserveret eller adopteret pga. denne adoption, gør det ledigt igen.
            var animal = await _animalRepository.GetByIdAsync(adoption.AnimalId);
            if (animal != null && (animal.Status == AnimalManagement.Core.Enums.AnimalStatus.Reserved || animal.Status == AnimalManagement.Core.Enums.AnimalStatus.Adopted))
            {
                // Tjek om denne adoption var årsagen til dyrets nuværende status
                if (animal.Status == AnimalManagement.Core.Enums.AnimalStatus.Adopted && animal.AdoptionDate.HasValue && adoption.CompletionDate.HasValue && animal.AdoptedByCustomerId == adoption.CustomerId) {
                     // Hvis adoptionen var Gennemført, og vi annullerer, skal dyret blive Ledigt
                     animal.Status = AnimalManagement.Core.Enums.AnimalStatus.Available;
                     animal.IsAdopted = false;
                     animal.AdoptionDate = null;
                     animal.AdoptedByCustomerId = null;
                }
                else if (animal.Status == AnimalManagement.Core.Enums.AnimalStatus.Reserved) {
                    // Hvis dyret blot var reserveret, og adoptionen nu annulleres, skal dyret blive Ledigt
                    // (Antaget at 'Reserved' betyder reserveret til en specifik 'Pending' eller 'Approved' adoption)
                    animal.Status = AnimalManagement.Core.Enums.AnimalStatus.Available;
                }
                await _animalRepository.UpdateAsync(animal);
            }

            await _adoptionRepository.UpdateAsync(adoption);
        }

        /// <summary>
        /// Validerer en adoption før oprettelse eller opdatering.
        /// Kaster exceptions hvis validering fejler.
        /// </summary>
        /// <param name="adoption">Adoptionsobjektet der skal valideres.</param>
        /// <param name="isNew">Angiver om det er en ny adoption (true) eller en opdatering (false).</param>
        /// <exception cref="ArgumentException">Kastes hvis Kunde ID, Dyr ID, Medarbejder ID (hvis angivet), Adoptionstype, Adoptionsdato eller Status er ugyldig.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Kastes hvis Adoptionsdato er for langt ude i fremtiden.</exception>
        /// <exception cref="InvalidOperationException">Kastes hvis dyret ikke er ledigt for adoption (ved ny adoption),
        /// eller hvis dyret allerede har en aktiv adoption (ved ny adoption).</exception>
        private async Task ValidateAdoptionAsync(Adoption adoption, bool isNew)
        {
            if (adoption.CustomerId <= 0 || await _customerRepository.GetByIdAsync(adoption.CustomerId) == null)
                throw new ArgumentException("Ugyldigt eller ikke-eksisterende Kunde ID.", nameof(adoption.CustomerId));

            var animal = await _animalRepository.GetByIdAsync(adoption.AnimalId);
            if (adoption.AnimalId <= 0 || animal == null)
                throw new ArgumentException("Ugyldigt eller ikke-eksisterende Dyr ID.", nameof(adoption.AnimalId));

            // Ved oprettelse, tjek om dyret er ledigt
            if (isNew && animal.Status != AnimalManagement.Core.Enums.AnimalStatus.Available)
                throw new InvalidOperationException($"Dyret '{animal.Name}' (ID: {animal.Id}) er ikke ledigt for adoption. Status: {animal.Status}.");

            // EmployeeId er ikke altid påkrævet initielt, men ved visse statusændringer.
            if (adoption.EmployeeId.HasValue && adoption.EmployeeId.Value > 0) 
            {
                 if (await _employeeRepository.GetByIdAsync(adoption.EmployeeId.Value) == null)
                    throw new ArgumentException("Ugyldigt eller ikke-eksisterende Medarbejder ID.", nameof(adoption.EmployeeId));
            }
            else if (adoption.EmployeeId.HasValue && adoption.EmployeeId.Value <= 0)
            {
                 throw new ArgumentException("Medarbejder ID, hvis angivet, skal være et positivt tal.", nameof(adoption.EmployeeId));
            }

            if (string.IsNullOrWhiteSpace(adoption.AdoptionType))
                throw new ArgumentException("Adoptionstype kan ikke være tom.", nameof(adoption.AdoptionType));
            
            if (isNew && adoption.AdoptionDate != default(DateTime) && adoption.AdoptionDate.Date < DateTime.UtcNow.Date)
                throw new ArgumentException("Adoptionsdato kan ikke være i fortiden ved oprettelse.", nameof(adoption.AdoptionDate));
            
            if (adoption.AdoptionDate != default(DateTime) && adoption.AdoptionDate.Date > DateTime.UtcNow.Date.AddYears(1)) // Max 1 år frem
                throw new ArgumentOutOfRangeException(nameof(adoption.AdoptionDate), "Adoptionsdato kan ikke være mere end et år ude i fremtiden.");
            
            if (!Enum.IsDefined(typeof(AdoptionStatus), adoption.Status))
                throw new ArgumentException("Ugyldig adoptionsstatus.", nameof(adoption.Status));

            // Tjek om dyret allerede har en aktiv (Pending/Approved) adoption
            if (isNew)
            {
                var existingAdoptionsForAnimal = await _adoptionRepository.GetByAnimalIdAsync(adoption.AnimalId);
                if (existingAdoptionsForAnimal.Any(a => a.Status == AdoptionStatus.Pending || a.Status == AdoptionStatus.Approved))
                {
                    throw new InvalidOperationException($"Dyret ID {adoption.AnimalId} har allerede en afventende eller godkendt adoption.");
                }
            }
            
            // Hvis status er Approved eller Completed, skal AdoptionDate være sat
            if ((adoption.Status == AdoptionStatus.Approved || adoption.Status == AdoptionStatus.Completed) && adoption.AdoptionDate == default(DateTime))
            {
                // Sæt den til nu hvis den ikke er sat, eller kast exception
                // adoption.AdoptionDate = DateTime.UtcNow;
                throw new ArgumentException("Adoptionsdato skal være angivet for godkendte/afsluttede adoptioner.", nameof(adoption.AdoptionDate));
            }
        }
    }
} 