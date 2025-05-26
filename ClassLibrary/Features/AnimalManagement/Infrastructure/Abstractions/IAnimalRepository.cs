using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClassLibrary.Features.AnimalManagement.Core.Models;
using ClassLibrary.Features.AnimalManagement.Core.Enums;
using ClassLibrary.SharedKernel.Persistence.Abstractions;

namespace ClassLibrary.Features.AnimalManagement.Infrastructure.Abstractions
{
    public interface IAnimalRepository : IRepository<Animal>
    {
        Task<IEnumerable<Animal>> GetAvailableAnimalsAsync();
        Task<IEnumerable<Animal>> GetAdoptedAnimalsAsync();
        Task<IEnumerable<Animal>> GetAnimalsBySpeciesAsync(Species species);
        Task<IEnumerable<Animal>> GetAnimalsByAgeInYearsAsync(int age);
        Task<IEnumerable<Animal>> GetAnimalsByAgeInMonthsAsync(int age);
        Task<IEnumerable<Animal>> GetAnimalsByAgeInWeeksAsync(int age);
        Task<IEnumerable<Animal>> GetAnimalsByGenderAsync(string gender);
        Task<IEnumerable<Animal>> GetAnimalsByWeightRangeAsync(decimal minWeight, decimal maxWeight);
        Task<IEnumerable<Animal>> GetAnimalsByAgeRangeInYearsAsync(int minAge, int maxAge);
        Task<IEnumerable<Animal>> GetAnimalsByAgeRangeInMonthsAsync(int minAge, int maxAge);
        Task<IEnumerable<Animal>> GetAnimalsByAgeRangeInWeeksAsync(int minAge, int maxAge);
        Task<IEnumerable<Animal>> GetAnimalsByAdoptionStatusAsync(AnimalStatus status); // Bruger AnimalStatus fra Core.Enums
        Task<IEnumerable<Animal>> GetAnimalsByIntakeDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<Animal>> GetAnimalsByNameAsync(string name);
        Task<IEnumerable<Animal>> GetAnimalsByBreedAsync(string breed);
        Task<IEnumerable<Animal>> GetAnimalsNeedingVaccinationAsync(); // Flyttet fra Health Record Operations
    }
} 