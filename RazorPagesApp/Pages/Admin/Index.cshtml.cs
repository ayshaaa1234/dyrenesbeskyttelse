using ClassLibrary.Features.AnimalManagement.Application.Abstractions;
using ClassLibrary.Features.AnimalManagement.Core.Models;
using ClassLibrary.Features.AnimalManagement.Core.Enums;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RazorPagesApp.Pages.Admin
{
    // TODO: Tilføj [Authorize(Roles = "Administrator")]
    /// <summary>
    /// PageModel for administratorens overbliksside.
    /// Henter og viser nøgleinformationer som antal dyr, dyr klar til adoption,
    /// nyligt tilføjede dyr, kommende besøg og dyr, der trænger til vaccination.
    /// </summary>
    public class IndexModel : PageModel
    {
        private readonly IAnimalManagementService _animalService;

        /// <summary>
        /// Initialiserer en ny instans af <see cref="IndexModel"/>.
        /// </summary>
        /// <param name="animalService">Tjeneste til håndtering af dyreinformation.</param>
        public IndexModel(IAnimalManagementService animalService)
        {
            _animalService = animalService;
        }

        /// <summary>
        /// Det samlede antal dyr i systemet.
        /// </summary>
        public int TotalAnimals { get; set; }

        /// <summary>
        /// Antallet af dyr, der er klar til adoption.
        /// </summary>
        public int AnimalsReadyForAdoption { get; set; }

        /// <summary>
        /// En liste over de senest tilføjede dyr.
        /// </summary>
        public List<Animal> RecentlyAddedAnimals { get; set; } = new List<Animal>();

        /// <summary>
        /// En liste over kommende planlagte besøg.
        /// </summary>
        public List<Visit> UpcomingVisits { get; set; } = new List<Visit>();
        
        /// <summary>
        /// Antallet af dyr, der trænger til vaccination.
        /// Værdien -1 indikerer, at funktionen ikke er implementeret.
        /// Værdien -2 indikerer en fejl under hentning af data.
        /// </summary>
        public int AnimalsNeedingVaccination { get; set; } // Tilføjet for vaccine-widget

        /// <summary>
        /// Asynkron metode der kaldes, når siden anmodes med HTTP GET.
        /// Henter data fra <see cref="IAnimalManagementService"/> og opdaterer sidens properties.
        /// </summary>
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

            // Hent dyreinformation for de kommende besøg for at kunne vise dyrenavne etc.
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
                    visitRaw.Animal = animalForVisit; // Tilføj det fulde Animal objekt til Visit objektet
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