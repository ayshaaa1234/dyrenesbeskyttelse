using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClassLibrary.Features.AnimalManagement.Application.Abstractions;
using ClassLibrary.Features.AnimalManagement.Core.Models;
using ClassLibrary.Features.AnimalManagement.Core.Enums;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RazorPagesApp.Pages.Admin
{
    // TODO: Tilføj [Authorize(Roles = "Administrator")]
    public class IndexModel : PageModel
    {
        private readonly IAnimalManagementService _animalService;

        public IndexModel(IAnimalManagementService animalService)
        {
            _animalService = animalService;
        }

        public int TotalAnimals { get; set; }
        public int AnimalsReadyForAdoption { get; set; }
        public List<Animal> RecentlyAddedAnimals { get; set; } = new List<Animal>();
        public List<Visit> UpcomingVisits { get; set; } = new List<Visit>();
        public int AnimalsNeedingVaccination { get; set; } // Tilføjet for vaccine-widget

        public async Task OnGetAsync()
        {
            var allAnimals = await _animalService.GetAllAnimalsAsync();
            TotalAnimals = allAnimals.Count();
            AnimalsReadyForAdoption = allAnimals.Count(a => a.Status == AnimalStatus.Available);
            RecentlyAddedAnimals = allAnimals.OrderByDescending(a => a.IntakeDate).Take(5).ToList();

            // Hent alle besøg og filtrer dem
            // Dette kan være ineffektivt for mange besøg - overvej en mere specifik service metode senere
            var allVisitsRaw = await _animalService.GetAllVisitsAsync(); 
            var upcomingVisitsRaw = allVisitsRaw
                .Where(v => v.PlannedDate.Date >= DateTime.Today && (v.Status == VisitStatus.Scheduled || v.Status == VisitStatus.Confirmed))
                .OrderBy(v => v.PlannedDate)
                .Take(5) 
                .ToList();

            var animalIdsForUpcomingVisits = upcomingVisitsRaw.Select(v => v.AnimalId).Distinct().ToList();
            IEnumerable<Animal> animalsForVisits = new List<Animal>();
            if (animalIdsForUpcomingVisits.Any())
            {
                animalsForVisits = await _animalService.GetAnimalsByIdsAsync(animalIdsForUpcomingVisits);
            }
            var animalsDict = animalsForVisits.ToDictionary(a => a.Id);

            UpcomingVisits = new List<Visit>();
            foreach (var visitRaw in upcomingVisitsRaw)
            {
                if (animalsDict.TryGetValue(visitRaw.AnimalId, out var animalForVisit))
                {
                    visitRaw.Animal = animalForVisit; 
                    UpcomingVisits.Add(visitRaw);
                }
            }

            // Hent dyr der skal vaccineres
            // Fejlhåndtering for NotImplementedException, hvis servicemetoden ikke er klar
            try
            {
                var animalsToVaccinate = await _animalService.GetAnimalsNeedingVaccinationAsync();
                AnimalsNeedingVaccination = animalsToVaccinate.Count();
            }
            catch (NotImplementedException)
            {
                AnimalsNeedingVaccination = -1; // Indikerer at funktionen ikke er implementeret
            }
            catch (Exception)
            {
                AnimalsNeedingVaccination = -2; // Indikerer en generel fejl
            }
        }
    }
} 