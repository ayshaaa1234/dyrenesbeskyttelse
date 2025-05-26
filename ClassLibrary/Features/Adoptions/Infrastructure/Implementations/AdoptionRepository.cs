using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClassLibrary.Features.Adoptions.Infrastructure.Abstractions;
using ClassLibrary.Features.Adoptions.Core.Models;
using ClassLibrary.Features.Adoptions.Core.Enums;
using ClassLibrary.SharedKernel.Persistence.Implementations; // For Repository<T>
using ClassLibrary.Features.AnimalManagement.Core.Models; // For Animal
using ClassLibrary.Features.AnimalManagement.Core.Enums; // For Species
using ClassLibrary.SharedKernel.Exceptions; // For KeyNotFoundException
// using ClassLibrary.Features.AnimalManagement.Core.Models; // For Animal, Species - tilføjes senere

namespace ClassLibrary.Features.Adoptions.Infrastructure.Implementations
{
    /// <summary>
    /// Repository til håndtering af adoptioner
    /// </summary>
    public class AdoptionRepository : Repository<Adoption>, IAdoptionRepository
    {
        private const string FilePath = "Data/Json/adoptions.json";

        public AdoptionRepository() : base(FilePath) { }

        // Override AddAsync for at håndtere specifik ID-generering
        public override async Task<Adoption> AddAsync(Adoption entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            ValidateEntity(entity);
            return await base.AddAsync(entity);
        }

        public override async Task<Adoption> UpdateAsync(Adoption entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            ValidateEntity(entity);
            return await base.UpdateAsync(entity);
        }

        protected override void ValidateEntity(Adoption entity)
        {
            base.ValidateEntity(entity); 
            
            if (entity.CustomerId <= 0)
                throw new ArgumentException("Kunde ID skal være større end 0", nameof(entity.CustomerId));

            if (entity.AnimalId <= 0)
                throw new ArgumentException("Dyr ID skal være større end 0", nameof(entity.AnimalId));

            if (entity.EmployeeId <= 0 && (entity.Status == AdoptionStatus.Approved || entity.Status == AdoptionStatus.Pending))
                throw new ArgumentException("Medarbejder ID skal være større end 0 for adoptioner der er godkendt eller afventer godkendelse.", nameof(entity.EmployeeId));

            if (string.IsNullOrWhiteSpace(entity.AdoptionType))
                throw new ArgumentException("Adoptionstype kan ikke være tom", nameof(entity.AdoptionType));

            if (entity.AdoptionDate > DateTime.UtcNow.AddMinutes(1))
                throw new ArgumentOutOfRangeException(nameof(entity.AdoptionDate), "Adoptionsdato kan ikke være i fremtiden.");
            
            if (entity.AdoptionDate == default(DateTime) && (entity.Status == AdoptionStatus.Approved || entity.Status == AdoptionStatus.Completed))
                throw new ArgumentException("Adoptionsdato skal være sat for godkendte/afsluttede adoptioner.", nameof(entity.AdoptionDate));

            if (!Enum.IsDefined(typeof(AdoptionStatus), entity.Status))
                throw new ArgumentException("Ugyldig adoptionsstatus", nameof(entity.Status));
        }

        public async Task<IEnumerable<Adoption>> GetByCustomerIdAsync(int customerId)
        {
            if (customerId <= 0)
            {
                return Enumerable.Empty<Adoption>();
            }
            return await base.FindAsync(a => a.CustomerId == customerId);
        }

        public async Task<IEnumerable<Adoption>> GetByAnimalIdAsync(int animalId)
        {
            if (animalId <= 0)
            {
                return Enumerable.Empty<Adoption>();
            }
            return await base.FindAsync(a => a.AnimalId == animalId);
        }

        public async Task<IEnumerable<Adoption>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
                throw new ArgumentException("Startdato skal være før slutdato");
            return await base.FindAsync(a => a.AdoptionDate.Date >= startDate.Date && a.AdoptionDate.Date <= endDate.Date);
        }

        public async Task<IEnumerable<Adoption>> GetByStatusAsync(AdoptionStatus status)
        {
            if (!Enum.IsDefined(typeof(AdoptionStatus), status))
            {
                return Enumerable.Empty<Adoption>();
            }
            return await base.FindAsync(a => a.Status == status);
        }

        public Task<IEnumerable<Adoption>> GetBySpeciesAsync(AnimalManagement.Core.Enums.Species species)
        {
            Console.WriteLine("Advarsel: GetBySpeciesAsync i AdoptionRepository er en stub. Filtrering på dyreart kræver adgang til Animal data og bør ske i service-laget.");
            return Task.FromResult(Enumerable.Empty<Adoption>());
        }

        public Task<IEnumerable<Adoption>> GetByAgeInYearsAsync(int years)
        {
            Console.WriteLine("Advarsel: GetByAgeInYearsAsync i AdoptionRepository er en stub. Implementer i service-laget.");
            return Task.FromResult(Enumerable.Empty<Adoption>());
        }

        public Task<IEnumerable<Adoption>> GetByAgeInMonthsAsync(int months)
        {
            Console.WriteLine("Advarsel: GetByAgeInMonthsAsync i AdoptionRepository er en stub. Implementer i service-laget.");
            return Task.FromResult(Enumerable.Empty<Adoption>());
        }

        public Task<IEnumerable<Adoption>> GetByAgeInWeeksAsync(int weeks)
        {
            Console.WriteLine("Advarsel: GetByAgeInWeeksAsync i AdoptionRepository er en stub. Implementer i service-laget.");
            return Task.FromResult(Enumerable.Empty<Adoption>());
        }

        public Task<IEnumerable<Adoption>> GetByAgeRangeInYearsAsync(int minYears, int maxYears)
        {
            Console.WriteLine("Advarsel: GetByAgeRangeInYearsAsync i AdoptionRepository er en stub. Implementer i service-laget.");
            return Task.FromResult(Enumerable.Empty<Adoption>());
        }

        public Task<IEnumerable<Adoption>> GetByAgeRangeInMonthsAsync(int minMonths, int maxMonths)
        {
            if (minMonths < 0 || maxMonths < minMonths) { 
                Console.WriteLine("Advarsel: Ugyldigt aldersinterval i GetByAgeRangeInMonthsAsync. Implementer i service-laget.");
                return Task.FromResult(Enumerable.Empty<Adoption>());
            }
            Console.WriteLine("Advarsel: GetByAgeRangeInMonthsAsync i AdoptionRepository er en stub. Implementer i service-laget.");
            return Task.FromResult(Enumerable.Empty<Adoption>());
        }

        public Task<IEnumerable<Adoption>> GetByAgeRangeInWeeksAsync(int minWeeks, int maxWeeks)
        {
             if (minWeeks < 0 || maxWeeks < minWeeks) { 
                Console.WriteLine("Advarsel: Ugyldigt aldersinterval i GetByAgeRangeInWeeksAsync. Implementer i service-laget.");
                return Task.FromResult(Enumerable.Empty<Adoption>());
            }
            Console.WriteLine("Advarsel: GetByAgeRangeInWeeksAsync i AdoptionRepository er en stub. Implementer i service-laget.");
            return Task.FromResult(Enumerable.Empty<Adoption>());
        }

        public async Task<IEnumerable<Adoption>> GetByAdoptionTypeAsync(string adoptionType)
        {
            if (string.IsNullOrWhiteSpace(adoptionType))
            {
                return Enumerable.Empty<Adoption>();
            }
            return await base.FindAsync(a => !string.IsNullOrWhiteSpace(a.AdoptionType) && a.AdoptionType.Equals(adoptionType, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<IEnumerable<Adoption>> GetByEmployeeIdAsync(int employeeId)
        {
            if (employeeId <= 0)
            {
                 return Enumerable.Empty<Adoption>();
            }
            return await base.FindAsync(a => a.EmployeeId == employeeId);
        }

        public async Task<IEnumerable<Adoption>> GetByAdopterEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return Enumerable.Empty<Adoption>();
            }
            Console.WriteLine("Advarsel: GetByAdopterEmailAsync i AdoptionRepository er midlertidigt deaktiveret pga. fjernelse af AdopterEmail fra Adoption modellen. Skal implementeres i service-laget.");
            return await Task.FromResult(Enumerable.Empty<Adoption>());
        }

        public async Task<Adoption> GetLatestAdoptionForAnimalAsync(int animalId)
        {
            if (animalId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(animalId), "Animal ID skal være større end 0.");
            }

            var adoptions = await base.FindAsync(a => a.AnimalId == animalId);
            var latestAdoption = adoptions.OrderByDescending(a => a.AdoptionDate).FirstOrDefault();
            
            if (latestAdoption == null)
            {
                throw new KeyNotFoundException($"Ingen adoption fundet for dyr med ID: {animalId}");
            }
            return latestAdoption;
        }
    }
} 