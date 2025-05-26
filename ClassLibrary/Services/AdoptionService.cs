using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClassLibrary.Interfaces;
using ClassLibrary.Models;

namespace ClassLibrary.Services
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
                throw new ArgumentException("ID skal være større end 0", nameof(id));

            var adoption = await _adoptionRepository.GetByIdAsync(id);
            if (adoption == null)
                throw new KeyNotFoundException($"Adoption med ID {id} blev ikke fundet");

            return adoption;
        }

        /// <summary>
        /// Opretter en ny adoption
        /// </summary>
        public async Task<Adoption> CreateAdoptionAsync(Adoption adoption)
        {
            if (adoption == null)
                throw new ArgumentNullException(nameof(adoption));

            await ValidateAdoptionAsync(adoption);
            return await _adoptionRepository.AddAsync(adoption);
        }

        /// <summary>
        /// Opdaterer en eksisterende adoption
        /// </summary>
        public async Task<Adoption> UpdateAdoptionAsync(Adoption adoption)
        {
            if (adoption == null)
                throw new ArgumentNullException(nameof(adoption));

            await ValidateAdoptionAsync(adoption);
            return await _adoptionRepository.UpdateAsync(adoption);
        }

        /// <summary>
        /// Sletter en adoption
        /// </summary>
        public async Task DeleteAdoptionAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("ID skal være større end 0", nameof(id));

            await _adoptionRepository.DeleteAsync(id);
        }

        /// <summary>
        /// Henter adoptioner for en bestemt kunde
        /// </summary>
        public async Task<IEnumerable<Adoption>> GetAdoptionsByCustomerAsync(int customerId)
        {
            if (customerId <= 0)
                throw new ArgumentException("Kunde ID skal være større end 0", nameof(customerId));

            return await _adoptionRepository.GetByCustomerIdAsync(customerId);
        }

        /// <summary>
        /// Henter adoptioner for et bestemt dyr
        /// </summary>
        public async Task<IEnumerable<Adoption>> GetAdoptionsByAnimalAsync(int animalId)
        {
            if (animalId <= 0)
                throw new ArgumentException("Dyr ID skal være større end 0", nameof(animalId));

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
        public async Task<IEnumerable<Adoption>> GetAdoptionsBySpeciesAsync(Species species)
        {
            if (!Enum.IsDefined(typeof(Species), species))
                throw new ArgumentException("Ugyldig dyreart", nameof(species));

            return await _adoptionRepository.GetBySpeciesAsync(species);
        }

        /// <summary>
        /// Henter adoptioner baseret på alder i år
        /// </summary>
        public async Task<IEnumerable<Adoption>> GetAdoptionsByAgeInYearsAsync(int years)
        {
            if (years < 0)
                throw new ArgumentException("Alder kan ikke være negativ", nameof(years));

            return await _adoptionRepository.GetByAgeInYearsAsync(years);
        }

        /// <summary>
        /// Henter adoptioner baseret på alder i måneder
        /// </summary>
        public async Task<IEnumerable<Adoption>> GetAdoptionsByAgeInMonthsAsync(int months)
        {
            if (months < 0)
                throw new ArgumentException("Alder kan ikke være negativ", nameof(months));

            return await _adoptionRepository.GetByAgeInMonthsAsync(months);
        }

        /// <summary>
        /// Henter adoptioner baseret på alder i uger
        /// </summary>
        public async Task<IEnumerable<Adoption>> GetAdoptionsByAgeInWeeksAsync(int weeks)
        {
            if (weeks < 0)
                throw new ArgumentException("Alder kan ikke være negativ", nameof(weeks));

            return await _adoptionRepository.GetByAgeInWeeksAsync(weeks);
        }

        /// <summary>
        /// Henter adoptioner baseret på aldersinterval i år
        /// </summary>
        public async Task<IEnumerable<Adoption>> GetAdoptionsByAgeRangeInYearsAsync(int minYears, int maxYears)
        {
            if (minYears < 0)
                throw new ArgumentException("Minimumsalder kan ikke være negativ", nameof(minYears));
            if (maxYears < minYears)
                throw new ArgumentException("Maksimumsalder skal være større end minimumsalder", nameof(maxYears));

            return await _adoptionRepository.GetByAgeRangeInYearsAsync(minYears, maxYears);
        }

        /// <summary>
        /// Henter adoptioner baseret på aldersinterval i måneder
        /// </summary>
        public async Task<IEnumerable<Adoption>> GetAdoptionsByAgeRangeInMonthsAsync(int minMonths, int maxMonths)
        {
            if (minMonths < 0)
                throw new ArgumentException("Minimumsalder kan ikke være negativ", nameof(minMonths));
            if (maxMonths < minMonths)
                throw new ArgumentException("Maksimumsalder skal være større end minimumsalder", nameof(maxMonths));

            return await _adoptionRepository.GetByAgeRangeInMonthsAsync(minMonths, maxMonths);
        }

        /// <summary>
        /// Henter adoptioner baseret på aldersinterval i uger
        /// </summary>
        public async Task<IEnumerable<Adoption>> GetAdoptionsByAgeRangeInWeeksAsync(int minWeeks, int maxWeeks)
        {
            if (minWeeks < 0)
                throw new ArgumentException("Minimumsalder kan ikke være negativ", nameof(minWeeks));
            if (maxWeeks < minWeeks)
                throw new ArgumentException("Maksimumsalder skal være større end minimumsalder", nameof(maxWeeks));

            return await _adoptionRepository.GetByAgeRangeInWeeksAsync(minWeeks, maxWeeks);
        }

        /// <summary>
        /// Henter adoptioner baseret på adoptionstype
        /// </summary>
        public async Task<IEnumerable<Adoption>> GetAdoptionsByTypeAsync(string adoptionType)
        {
            if (string.IsNullOrWhiteSpace(adoptionType))
                throw new ArgumentException("Adoptionstype kan ikke være tom", nameof(adoptionType));

            return await _adoptionRepository.GetByAdoptionTypeAsync(adoptionType);
        }

        /// <summary>
        /// Henter adoptioner baseret på medarbejder
        /// </summary>
        public async Task<IEnumerable<Adoption>> GetAdoptionsByEmployeeAsync(int employeeId)
        {
            if (employeeId <= 0)
                throw new ArgumentException("Medarbejder ID skal være større end 0", nameof(employeeId));

            return await _adoptionRepository.GetByEmployeeIdAsync(employeeId);
        }

        /// <summary>
        /// Henter adoptioner baseret på adopters email
        /// </summary>
        public async Task<IEnumerable<Adoption>> GetAdoptionsByAdopterEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email kan ikke være tom", nameof(email));

            return await _adoptionRepository.GetByAdopterEmailAsync(email);
        }

        /// <summary>
        /// Henter den seneste adoption for et dyr
        /// </summary>
        public async Task<Adoption> GetLatestAdoptionForAnimalAsync(int animalId)
        {
            if (animalId <= 0)
                throw new ArgumentException("Dyr ID skal være større end 0", nameof(animalId));

            return await _adoptionRepository.GetLatestAdoptionForAnimalAsync(animalId);
        }

        /// <summary>
        /// Godkender en adoption
        /// </summary>
        public async Task ApproveAdoptionAsync(int adoptionId)
        {
            var adoption = await GetAdoptionByIdAsync(adoptionId);
            
            if (adoption.Status != AdoptionStatus.Pending)
                throw new InvalidOperationException("Kun ventende adoptioner kan godkendes");

            adoption.Status = AdoptionStatus.Approved;
            adoption.ApprovalDate = DateTime.Now;
            
            await _adoptionRepository.UpdateAsync(adoption);
        }

        /// <summary>
        /// Afviser en adoption
        /// </summary>
        public async Task RejectAdoptionAsync(int adoptionId)
        {
            var adoption = await GetAdoptionByIdAsync(adoptionId);
            
            if (adoption.Status != AdoptionStatus.Pending)
                throw new InvalidOperationException("Kun ventende adoptioner kan afvises");

            adoption.Status = AdoptionStatus.Rejected;
            adoption.RejectionDate = DateTime.Now;
            
            await _adoptionRepository.UpdateAsync(adoption);
        }

        /// <summary>
        /// Gennemfører en adoption
        /// </summary>
        public async Task CompleteAdoptionAsync(int adoptionId)
        {
            var adoption = await GetAdoptionByIdAsync(adoptionId);
            
            if (adoption.Status != AdoptionStatus.Approved)
                throw new InvalidOperationException("Kun godkendte adoptioner kan gennemføres");

            adoption.Status = AdoptionStatus.Completed;
            adoption.CompletionDate = DateTime.Now;
            
            await _adoptionRepository.UpdateAsync(adoption);
        }

        /// <summary>
        /// Validerer en adoption
        /// </summary>
        private async Task ValidateAdoptionAsync(Adoption adoption)
        {
            if (adoption.CustomerId <= 0)
                throw new ArgumentException("Kunde ID skal være større end 0");

            if (adoption.AnimalId <= 0)
                throw new ArgumentException("Dyr ID skal være større end 0");

            if (adoption.EmployeeId <= 0)
                throw new ArgumentException("Medarbejder ID skal være større end 0");

            if (string.IsNullOrWhiteSpace(adoption.AdoptionType))
                throw new ArgumentException("Adoptionstype kan ikke være tom");

            if (string.IsNullOrWhiteSpace(adoption.AdopterName))
                throw new ArgumentException("Adoptantens navn kan ikke være tomt");

            if (string.IsNullOrWhiteSpace(adoption.AdopterEmail))
                throw new ArgumentException("Adoptantens email kan ikke være tom");

            if (string.IsNullOrWhiteSpace(adoption.AdopterPhone))
                throw new ArgumentException("Adoptantens telefonnummer kan ikke være tomt");

            if (adoption.AdoptionDate > DateTime.Now)
                throw new ArgumentException("Adoptionsdato kan ikke være i fremtiden");

            if (!Enum.IsDefined(typeof(AdoptionStatus), adoption.Status))
                throw new ArgumentException("Ugyldig adoptionsstatus");

            // Valider datoer baseret på status
            switch (adoption.Status)
            {
                case AdoptionStatus.Approved:
                    if (!adoption.ApprovalDate.HasValue)
                        throw new ArgumentException("Godkendelsesdato mangler for godkendt adoption");
                    break;
                case AdoptionStatus.Rejected:
                    if (!adoption.RejectionDate.HasValue)
                        throw new ArgumentException("Afvisningsdato mangler for afvist adoption");
                    break;
                case AdoptionStatus.Completed:
                    if (!adoption.CompletionDate.HasValue)
                        throw new ArgumentException("Gennemførselsdato mangler for gennemført adoption");
                    break;
            }

            // Tjek om kunden eksisterer
            var customer = await _customerRepository.GetByIdAsync(adoption.CustomerId);
            if (customer == null)
                throw new KeyNotFoundException($"Kunde med ID {adoption.CustomerId} blev ikke fundet");

            // Tjek om dyret eksisterer
            var animal = await _animalRepository.GetByIdAsync(adoption.AnimalId);
            if (animal == null)
                throw new KeyNotFoundException($"Dyr med ID {adoption.AnimalId} blev ikke fundet");

            // Tjek om medarbejderen eksisterer
            var employee = await _employeeRepository.GetByIdAsync(adoption.EmployeeId);
            if (employee == null)
                throw new KeyNotFoundException($"Medarbejder med ID {adoption.EmployeeId} blev ikke fundet");

            // Tjek om dyret allerede er adopteret
            var existingAdoption = await _adoptionRepository.GetLatestAdoptionForAnimalAsync(adoption.AnimalId);
            if (existingAdoption != null && existingAdoption.Status == AdoptionStatus.Completed)
                throw new InvalidOperationException("Dette dyr er allerede adopteret");
        }
    }
} 