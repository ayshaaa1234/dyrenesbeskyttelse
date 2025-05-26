using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClassLibrary.Features.AnimalManagement.Application.Abstractions;
using ClassLibrary.Features.AnimalManagement.Core.Models;
using ClassLibrary.Features.AnimalManagement.Core.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RazorPagesApp.Pages.Admin.Animals
{
    /// <summary>
    /// PageModel for dyreoversigten (Index-siden).
    /// Håndterer visning af en liste af dyr med mulighed for søgning og filtrering.
    /// </summary>
    // TODO: Tilføj [Authorize(Roles = "Administrator")] eller lignende
    public class IndexModel : PageModel
    {
        private readonly IAnimalManagementService _animalService;

        /// <summary>
        /// Initialiserer en ny instans af <see cref="IndexModel"/> klassen.
        /// </summary>
        /// <param name="animalService">Servicen til håndtering af dyredata.</param>
        public IndexModel(IAnimalManagementService animalService)
        {
            _animalService = animalService;
            Animals = Enumerable.Empty<Animal>();
        }

        /// <summary>
        /// Henter eller sætter listen af dyr, der skal vises på siden.
        /// </summary>
        public IEnumerable<Animal> Animals { get; set; }
        
        /// <summary>
        /// Henter eller sætter søgestrengen anvendt til at filtrere dyr efter navn eller art.
        /// Bindes fra query-streng i URL'en.
        /// </summary>
        [BindProperty(SupportsGet = true)]
        public string? SearchString { get; set; }
        
        /// <summary>
        /// Henter eller sætter filteret for dyrets status.
        /// Bindes fra query-streng i URL'en.
        /// </summary>
        [BindProperty(SupportsGet = true)]
        public string? StatusFilter { get; set; }
        
        /// <summary>
        /// Henter eller sætter en værdi, der angiver, om kun vaccinerede dyr skal vises.
        /// Bindes fra query-streng i URL'en.
        /// </summary>
        [BindProperty(SupportsGet = true)]
        public bool VaccinatedOnly { get; set; }

        /// <summary>
        /// Henter eller sætter en meddelelse, der skal vises til brugeren (f.eks. succes eller fejl).
        /// Anvender TempData til at persistere meddelelsen over en redirect.
        /// </summary>
        [TempData]
        public string? Message { get; set; }

        /// <summary>
        /// Håndterer HTTP GET-anmodningen.
        /// Henter og filtrerer dyr baseret på de angivne søgeparametre.
        /// </summary>
        /// <returns>En <see cref="Task"/> der repræsenterer den asynkrone operation.</returns>
        public async Task OnGetAsync()
        {
            var allAnimals = await _animalService.GetAllAnimalsAsync();
            IEnumerable<Animal> query = allAnimals ?? Enumerable.Empty<Animal>();

            if (!string.IsNullOrEmpty(SearchString))
            {
                query = query.Where(a => 
                    (a.Name != null && a.Name.Contains(SearchString, System.StringComparison.OrdinalIgnoreCase)) ||
                    (a.Species.ToString().Contains(SearchString, System.StringComparison.OrdinalIgnoreCase)));
            }

            if (!string.IsNullOrEmpty(StatusFilter))
            {
                if (System.Enum.TryParse<AnimalStatus>(StatusFilter, out var statusEnum))
                {
                    query = query.Where(a => a.Status == statusEnum);
                }
            }

            if (VaccinatedOnly)
            {
                query = query.Where(a => a.HealthRecords != null && a.HealthRecords.Any(hr => hr.IsVaccinated));
            }

            Animals = query.OrderBy(a => a.Name).ToList();
        }
    }
} 