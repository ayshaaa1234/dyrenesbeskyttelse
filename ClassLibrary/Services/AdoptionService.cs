using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClassLibrary.Models;
using ClassLibrary.Interfaces;

namespace ClassLibrary.Services
{
    /// <summary>
    /// Service til håndtering af adoptioner
    /// </summary>
    public class AdoptionService : IAdoptionService
    {
        private readonly IAdoptionRepository _adoptionRepository;

        /// <summary>
        /// Konstruktør
        /// </summary>
        public AdoptionService(IAdoptionRepository adoptionRepository)
        {
            _adoptionRepository = adoptionRepository ?? throw new ArgumentNullException(nameof(adoptionRepository));
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
                throw new ArgumentException("ID skal være større end 0");

            var adoption = await _adoptionRepository.GetByIdAsync(id);
            if (adoption == null)
                throw new KeyNotFoundException($"Ingen adoption fundet med ID: {id}");

            return adoption;
        }

        /// <summary>
        /// Opretter en ny adoption
        /// </summary>
        public async Task<Adoption> CreateAdoptionAsync(Adoption adoption)
        {
            if (adoption == null)
                throw new ArgumentNullException(nameof(adoption));

            ValidateAdoption(adoption);
            return await _adoptionRepository.AddAsync(adoption);
        }

        /// <summary>
        /// Opdaterer en eksisterende adoption
        /// </summary>
        public async Task<Adoption> UpdateAdoptionAsync(Adoption adoption)
        {
            if (adoption == null)
                throw new ArgumentNullException(nameof(adoption));

            ValidateAdoption(adoption);
            return await _adoptionRepository.UpdateAsync(adoption);
        }

        /// <summary>
        /// Sletter en adoption
        /// </summary>
        public async Task DeleteAdoptionAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("ID skal være større end 0");

            await _adoptionRepository.DeleteAsync(id);
        }

        /// <summary>
        /// Henter adoptioner for en bestemt kunde
        /// </summary>
        public async Task<IEnumerable<Adoption>> GetAdoptionsByCustomerAsync(int customerId)
        {
            if (customerId <= 0)
                throw new ArgumentException("CustomerId skal være større end 0");

            return await _adoptionRepository.GetByCustomerIdAsync(customerId);
        }

        /// <summary>
        /// Henter adoptioner for et bestemt dyr
        /// </summary>
        public async Task<IEnumerable<Adoption>> GetAdoptionsByAnimalAsync(int animalId)
        {
            if (animalId <= 0)
                throw new ArgumentException("AnimalId skal være større end 0");

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
            return await _adoptionRepository.GetByStatusAsync(status);
        }

        /// <summary>
        /// Henter adoptioner baseret på dyreart
        /// </summary>
        public async Task<IEnumerable<Adoption>> GetAdoptionsBySpeciesAsync(Species species)
        {
            return await _adoptionRepository.GetBySpeciesAsync(species);
        }

        /// <summary>
        /// Henter adoptioner baseret på alder i år
        /// </summary>
        public async Task<IEnumerable<Adoption>> GetAdoptionsByAgeInYearsAsync(int years)
        {
            if (years < 0)
                throw new ArgumentException("Alder kan ikke være negativ");

            return await _adoptionRepository.GetByAgeInYearsAsync(years);
        }

        /// <summary>
        /// Henter adoptioner baseret på alder i måneder
        /// </summary>
        public async Task<IEnumerable<Adoption>> GetAdoptionsByAgeInMonthsAsync(int months)
        {
            if (months < 0)
                throw new ArgumentException("Alder kan ikke være negativ");

            return await _adoptionRepository.GetByAgeInMonthsAsync(months);
        }

        /// <summary>
        /// Henter adoptioner baseret på alder i uger
        /// </summary>
        public async Task<IEnumerable<Adoption>> GetAdoptionsByAgeInWeeksAsync(int weeks)
        {
            if (weeks < 0)
                throw new ArgumentException("Alder kan ikke være negativ");

            return await _adoptionRepository.GetByAgeInWeeksAsync(weeks);
        }

        /// <summary>
        /// Henter adoptioner baseret på aldersinterval i år
        /// </summary>
        public async Task<IEnumerable<Adoption>> GetAdoptionsByAgeRangeInYearsAsync(int minYears, int maxYears)
        {
            if (minYears < 0)
                throw new ArgumentException("Minimumsalder kan ikke være negativ");
            if (maxYears < minYears)
                throw new ArgumentException("Maksimumsalder skal være større end minimumsalder");

            return await _adoptionRepository.GetByAgeRangeInYearsAsync(minYears, maxYears);
        }

        /// <summary>
        /// Henter adoptioner baseret på aldersinterval i måneder
        /// </summary>
        public async Task<IEnumerable<Adoption>> GetAdoptionsByAgeRangeInMonthsAsync(int minMonths, int maxMonths)
        {
            if (minMonths < 0)
                throw new ArgumentException("Minimumsalder kan ikke være negativ");
            if (maxMonths < minMonths)
                throw new ArgumentException("Maksimumsalder skal være større end minimumsalder");

            return await _adoptionRepository.GetByAgeRangeInMonthsAsync(minMonths, maxMonths);
        }

        /// <summary>
        /// Henter adoptioner baseret på aldersinterval i uger
        /// </summary>
        public async Task<IEnumerable<Adoption>> GetAdoptionsByAgeRangeInWeeksAsync(int minWeeks, int maxWeeks)
        {
            if (minWeeks < 0)
                throw new ArgumentException("Minimumsalder kan ikke være negativ");
            if (maxWeeks < minWeeks)
                throw new ArgumentException("Maksimumsalder skal være større end minimumsalder");

            return await _adoptionRepository.GetByAgeRangeInWeeksAsync(minWeeks, maxWeeks);
        }

        /// <summary>
        /// Henter adoptioner baseret på adoptionstype
        /// </summary>
        public async Task<IEnumerable<Adoption>> GetAdoptionsByTypeAsync(string adoptionType)
        {
            if (string.IsNullOrWhiteSpace(adoptionType))
                throw new ArgumentException("Adoptionstype kan ikke være tom");

            return await _adoptionRepository.GetByAdoptionTypeAsync(adoptionType);
        }

        /// <summary>
        /// Henter adoptioner baseret på medarbejder
        /// </summary>
        public async Task<IEnumerable<Adoption>> GetAdoptionsByEmployeeAsync(int employeeId)
        {
            if (employeeId <= 0)
                throw new ArgumentException("EmployeeId skal være større end 0");

            return await _adoptionRepository.GetByEmployeeIdAsync(employeeId);
        }

        /// <summary>
        /// Henter adoptioner baseret på adopters email
        /// </summary>
        public async Task<IEnumerable<Adoption>> GetAdoptionsByAdopterEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email kan ikke være tom");

            return await _adoptionRepository.GetByAdopterEmailAsync(email);
        }

        /// <summary>
        /// Henter den seneste adoption for et dyr
        /// </summary>
        public async Task<Adoption> GetLatestAdoptionForAnimalAsync(int animalId)
        {
            if (animalId <= 0)
                throw new ArgumentException("AnimalId skal være større end 0");

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
            await _adoptionRepository.UpdateAsync(adoption);
        }

        /// <summary>
        /// Validerer en adoption
        /// </summary>
        private void ValidateAdoption(Adoption adoption)
        {
            if (adoption.AnimalId <= 0)
                throw new ArgumentException("AnimalId skal være større end 0");

            if (adoption.CustomerId <= 0)
                throw new ArgumentException("CustomerId skal være større end 0");

            if (adoption.EmployeeId <= 0)
                throw new ArgumentException("EmployeeId skal være større end 0");

            if (string.IsNullOrWhiteSpace(adoption.AdopterName))
                throw new ArgumentException("Adoptantens navn kan ikke være tomt");

            if (string.IsNullOrWhiteSpace(adoption.AdopterEmail))
                throw new ArgumentException("Adoptantens email kan ikke være tom");

            if (string.IsNullOrWhiteSpace(adoption.AdopterPhone))
                throw new ArgumentException("Adoptantens telefonnummer kan ikke være tomt");

            if (string.IsNullOrWhiteSpace(adoption.AdoptionType))
                throw new ArgumentException("Adoptionstype kan ikke være tom");
        }
    }
} 