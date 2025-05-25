using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClassLibrary.Models;
using ClassLibrary.Interfaces;

namespace ClassLibrary.Services
{
    /// <summary>
    /// Service til håndtering af dyr
    /// </summary>
    public class AnimalService : IAnimalService
    {
        private readonly IAnimalRepository _animalRepository;

        /// <summary>
        /// Konstruktør
        /// </summary>
        public AnimalService(IAnimalRepository animalRepository)
        {
            _animalRepository = animalRepository ?? throw new ArgumentNullException(nameof(animalRepository));
        }

        /// <summary>
        /// Henter alle dyr
        /// </summary>
        public async Task<IEnumerable<Animal>> GetAllAnimalsAsync()
        {
            return await _animalRepository.GetAllAsync();
        }

        /// <summary>
        /// Henter et dyr baseret på ID
        /// </summary>
        public async Task<Animal> GetAnimalByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("ID skal være større end 0");

            var animal = await _animalRepository.GetByIdAsync(id);
            if (animal == null)
                throw new KeyNotFoundException($"Intet dyr fundet med ID: {id}");

            return animal;
        }

        /// <summary>
        /// Opretter et nyt dyr
        /// </summary>
        public async Task<Animal> CreateAnimalAsync(Animal animal)
        {
            if (animal == null)
                throw new ArgumentNullException(nameof(animal));

            ValidateAnimal(animal);
            return await _animalRepository.AddAsync(animal);
        }

        /// <summary>
        /// Opdaterer et eksisterende dyr
        /// </summary>
        public async Task<Animal> UpdateAnimalAsync(Animal animal)
        {
            if (animal == null)
                throw new ArgumentNullException(nameof(animal));

            ValidateAnimal(animal);
            return await _animalRepository.UpdateAsync(animal);
        }

        /// <summary>
        /// Sletter et dyr
        /// </summary>
        public async Task DeleteAnimalAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("ID skal være større end 0");

            await _animalRepository.DeleteAsync(id);
        }

        /// <summary>
        /// Henter dyr baseret på dyreart
        /// </summary>
        public async Task<IEnumerable<Animal>> GetAnimalsBySpeciesAsync(Species species)
        {
            return await _animalRepository.GetBySpeciesAsync(species);
        }

        /// <summary>
        /// Henter dyr baseret på alder i år
        /// </summary>
        public async Task<IEnumerable<Animal>> GetAnimalsByAgeInYearsAsync(int years)
        {
            if (years < 0)
                throw new ArgumentException("Alder kan ikke være negativ");

            return await _animalRepository.GetByAgeInYearsAsync(years);
        }

        /// <summary>
        /// Henter dyr baseret på alder i måneder
        /// </summary>
        public async Task<IEnumerable<Animal>> GetAnimalsByAgeInMonthsAsync(int months)
        {
            if (months < 0)
                throw new ArgumentException("Alder kan ikke være negativ");

            return await _animalRepository.GetByAgeInMonthsAsync(months);
        }

        /// <summary>
        /// Henter dyr baseret på alder i uger
        /// </summary>
        public async Task<IEnumerable<Animal>> GetAnimalsByAgeInWeeksAsync(int weeks)
        {
            if (weeks < 0)
                throw new ArgumentException("Alder kan ikke være negativ");

            return await _animalRepository.GetByAgeInWeeksAsync(weeks);
        }

        /// <summary>
        /// Henter dyr baseret på køn
        /// </summary>
        public async Task<IEnumerable<Animal>> GetAnimalsByGenderAsync(string gender)
        {
            if (string.IsNullOrWhiteSpace(gender))
                throw new ArgumentException("Køn kan ikke være tomt");

            return await _animalRepository.GetByGenderAsync(gender);
        }

        /// <summary>
        /// Henter dyr baseret på vægtinterval
        /// </summary>
        public async Task<IEnumerable<Animal>> GetAnimalsByWeightRangeAsync(decimal minWeight, decimal maxWeight)
        {
            if (minWeight < 0)
                throw new ArgumentException("Minimumsvægt kan ikke være negativ");
            if (maxWeight < minWeight)
                throw new ArgumentException("Maksimumsvægt skal være større end minimumsvægt");

            return await _animalRepository.GetByWeightRangeAsync(minWeight, maxWeight);
        }

        /// <summary>
        /// Henter dyr baseret på aldersinterval i år
        /// </summary>
        public async Task<IEnumerable<Animal>> GetAnimalsByAgeRangeInYearsAsync(int minYears, int maxYears)
        {
            if (minYears < 0)
                throw new ArgumentException("Minimumsalder kan ikke være negativ");
            if (maxYears < minYears)
                throw new ArgumentException("Maksimumsalder skal være større end minimumsalder");

            return await _animalRepository.GetByAgeRangeInYearsAsync(minYears, maxYears);
        }

        /// <summary>
        /// Henter dyr baseret på aldersinterval i måneder
        /// </summary>
        public async Task<IEnumerable<Animal>> GetAnimalsByAgeRangeInMonthsAsync(int minMonths, int maxMonths)
        {
            if (minMonths < 0)
                throw new ArgumentException("Minimumsalder kan ikke være negativ");
            if (maxMonths < minMonths)
                throw new ArgumentException("Maksimumsalder skal være større end minimumsalder");

            return await _animalRepository.GetByAgeRangeInMonthsAsync(minMonths, maxMonths);
        }

        /// <summary>
        /// Henter dyr baseret på aldersinterval i uger
        /// </summary>
        public async Task<IEnumerable<Animal>> GetAnimalsByAgeRangeInWeeksAsync(int minWeeks, int maxWeeks)
        {
            if (minWeeks < 0)
                throw new ArgumentException("Minimumsalder kan ikke være negativ");
            if (maxWeeks < minWeeks)
                throw new ArgumentException("Maksimumsalder skal være større end minimumsalder");

            return await _animalRepository.GetByAgeRangeInWeeksAsync(minWeeks, maxWeeks);
        }

        /// <summary>
        /// Henter dyr baseret på adoptionsstatus
        /// </summary>
        public async Task<IEnumerable<Animal>> GetAnimalsByAdoptionStatusAsync(bool isAdopted)
        {
            return await _animalRepository.GetByAdoptionStatusAsync(isAdopted);
        }

        /// <summary>
        /// Henter dyr baseret på indkomstdato
        /// </summary>
        public async Task<IEnumerable<Animal>> GetAnimalsByIntakeDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
                throw new ArgumentException("Startdato skal være før slutdato");

            return await _animalRepository.GetByIntakeDateRangeAsync(startDate, endDate);
        }

        /// <summary>
        /// Henter dyr baseret på navn
        /// </summary>
        public async Task<IEnumerable<Animal>> GetAnimalsByNameAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Navn kan ikke være tomt");

            return await _animalRepository.GetByNameAsync(name);
        }

        /// <summary>
        /// Henter dyr baseret på race
        /// </summary>
        public async Task<IEnumerable<Animal>> GetAnimalsByBreedAsync(string breed)
        {
            if (string.IsNullOrWhiteSpace(breed))
                throw new ArgumentException("Race kan ikke være tom");

            return await _animalRepository.GetByBreedAsync(breed);
        }

        /// <summary>
        /// Henter alle adopterede dyr
        /// </summary>
        public async Task<IEnumerable<Animal>> GetAdoptedAnimalsAsync()
        {
            return await _animalRepository.GetAdoptedAnimalsAsync();
        }

        /// <summary>
        /// Henter alle tilgængelige dyr
        /// </summary>
        public async Task<IEnumerable<Animal>> GetAvailableAnimalsAsync()
        {
            return await _animalRepository.GetAvailableAnimalsAsync();
        }

        /// <summary>
        /// Henter dyr der skal til lægen i en bestemt uge
        /// </summary>
        public async Task<IEnumerable<Animal>> GetAnimalsWithVetAppointmentsInWeekAsync(int weekNumber)
        {
            if (weekNumber < 1 || weekNumber > 53)
                throw new ArgumentException("Ugenummer skal være mellem 1 og 53");

            return await _animalRepository.GetAnimalsWithVetAppointmentsInWeekAsync(weekNumber);
        }

        /// <summary>
        /// Markerer et dyr som adopteret
        /// </summary>
        public async Task MarkAnimalAsAdoptedAsync(int animalId, int customerId)
        {
            if (animalId <= 0)
                throw new ArgumentException("AnimalId skal være større end 0");
            if (customerId <= 0)
                throw new ArgumentException("CustomerId skal være større end 0");

            var animal = await GetAnimalByIdAsync(animalId);
            if (animal.IsAdopted)
                throw new InvalidOperationException("Dyret er allerede adopteret");

            animal.IsAdopted = true;
            animal.AdoptionDate = DateTime.Now;
            animal.AdoptedByCustomerId = customerId;

            await _animalRepository.UpdateAsync(animal);
        }

        /// <summary>
        /// Markerer et dyr som ikke adopteret
        /// </summary>
        public async Task MarkAnimalAsAvailableAsync(int animalId)
        {
            if (animalId <= 0)
                throw new ArgumentException("AnimalId skal være større end 0");

            var animal = await GetAnimalByIdAsync(animalId);
            if (!animal.IsAdopted)
                throw new InvalidOperationException("Dyret er allerede tilgængeligt");

            animal.IsAdopted = false;
            animal.AdoptionDate = null;
            animal.AdoptedByCustomerId = null;

            await _animalRepository.UpdateAsync(animal);
        }

        /// <summary>
        /// Tilføjer en sundhedsjournal til et dyr
        /// </summary>
        public async Task AddHealthRecordAsync(int animalId, HealthRecord healthRecord)
        {
            if (animalId <= 0)
                throw new ArgumentException("AnimalId skal være større end 0");
            if (healthRecord == null)
                throw new ArgumentNullException(nameof(healthRecord));

            var animal = await GetAnimalByIdAsync(animalId);
            animal.HealthRecords.Add(healthRecord);
            await _animalRepository.UpdateAsync(animal);
        }

        /// <summary>
        /// Tilføjer et besøg til et dyr
        /// </summary>
        public async Task AddVisitLogAsync(int animalId, VisitLog visitLog)
        {
            if (animalId <= 0)
                throw new ArgumentException("AnimalId skal være større end 0");
            if (visitLog == null)
                throw new ArgumentNullException(nameof(visitLog));

            var animal = await GetAnimalByIdAsync(animalId);
            animal.VisitLogs.Add(visitLog);
            await _animalRepository.UpdateAsync(animal);
        }

        /// <summary>
        /// Validerer et dyr
        /// </summary>
        private void ValidateAnimal(Animal animal)
        {
            if (string.IsNullOrWhiteSpace(animal.Name))
                throw new ArgumentException("Navn kan ikke være tomt");

            if (string.IsNullOrWhiteSpace(animal.Gender))
                throw new ArgumentException("Køn kan ikke være tomt");

            if (string.IsNullOrWhiteSpace(animal.Breed))
                throw new ArgumentException("Race kan ikke være tom");

            if (animal.Weight < 0)
                throw new ArgumentException("Vægt kan ikke være negativ");

            if (string.IsNullOrWhiteSpace(animal.HealthStatus))
                throw new ArgumentException("Sundhedsstatus kan ikke være tom");

            if (animal.BirthDate.HasValue && animal.BirthDate.Value > DateTime.Now)
                throw new ArgumentException("Fødselsdato kan ikke være i fremtiden");

            if (animal.IntakeDate > DateTime.Now)
                throw new ArgumentException("Indkomstdato kan ikke være i fremtiden");
        }
    }
} 