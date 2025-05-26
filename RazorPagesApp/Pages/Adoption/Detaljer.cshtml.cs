using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ClassLibrary.Features.AnimalManagement.Application.Abstractions;
using ClassLibrary.Features.AnimalManagement.Core.Models;
// using ClassLibrary.Features.Adoptions.Application.Abstractions; // TODO: Tilføjes når IAdoptionService er implementeret og skal bruges.
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RazorPagesApp.Pages.Adoption
{
    // PageModel for siden, der viser detaljer om et specifikt dyr til adoption.
    public class DetaljerModel : PageModel
    {
        private readonly IAnimalManagementService _animalService; // Service til at hente dyredata.
        // private readonly IAdoptionService _adoptionService; // TODO: Injicer IAdoptionService, når den er relevant for denne side.

        // Dependency injection af IAnimalManagementService.
        public DetaljerModel(IAnimalManagementService animalService /*, IAdoptionService adoptionService */)
        {
            _animalService = animalService;
            // _adoptionService = adoptionService;
        }

        public Animal? Animal { get; set; } // Holder det dyr, der vises på siden.
        public IList<HealthRecord> HealthRecords { get; set; } = new List<HealthRecord>(); // Liste over dyrets sundhedsjournaler.
        public IList<Visit> Visits { get; set; } = new List<Visit>(); // Liste over dyrets besøg (f.eks. dyrlægebesøg).

        // TODO: Overvej om denne property er nødvendig, eller om logikken kan håndteres direkte i Razor-siden baseret på Animal.Status.
        // public bool CanAdopt { get; set; } = false;

        // Handler for GET-requests. Kaldes når siden indlæses med et dyre-ID (id).
        public async Task<IActionResult> OnGetAsync(int id)
        {
            Animal = await _animalService.GetAnimalByIdAsync(id); // Henter dyret fra databasen.

            if (Animal == null) // Hvis dyret ikke findes, returneres en 404 Not Found fejl.
            {
                return NotFound();
            }

            // TODO: Overvej om detaljer (sundhedsjournaler, besøg) kun skal hentes under visse betingelser (f.eks. Animal.Status).
            // For nu hentes de altid, hvis dyret findes.
            // if (Animal.Status != ClassLibrary.Features.AnimalManagement.Core.Enums.AnimalStatus.Available)
            // {
            //     // Håndter visning for dyr der ikke er "Available" - måske en anden besked eller færre detaljer.
            // }

            // Starter asynkrone opgaver for at hente sundhedsjournaler og besøg parallelt for bedre performance.
            var healthRecordsTask = _animalService.GetHealthRecordsByAnimalIdAsync(id);
            var visitsTask = _animalService.GetVisitsByAnimalIdAsync(id);

            // Venter på at begge asynkrone opgaver fuldføres.
            await Task.WhenAll(healthRecordsTask, visitsTask);

            // Tildeler resultaterne til properties.
            HealthRecords = new List<HealthRecord>(await healthRecordsTask);
            Visits = new List<Visit>(await visitsTask);
            
            // TODO: Implementer logik med _adoptionService for at tjekke, om dyret kan adopteres, hvis det er mere komplekst end blot Animal.Status.
            // f.eks. _adoptionService.IsAnimalAvailableForAdoptionAsync(id)
            // CanAdopt = Animal.Status == ClassLibrary.Features.AnimalManagement.Core.Enums.AnimalStatus.Available; 

            return Page(); // Returnerer Razor Page med de hentede data.
        }
    }
} 