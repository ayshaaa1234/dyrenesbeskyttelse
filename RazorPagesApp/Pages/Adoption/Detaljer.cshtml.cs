using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ClassLibrary.Features.AnimalManagement.Application.Abstractions;
using ClassLibrary.Features.AnimalManagement.Core.Models;
// using ClassLibrary.Features.Adoptions.Application.Abstractions; // Tilføjes når vi har IAdoptionService
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RazorPagesApp.Pages.Adoption
{
    public class DetaljerModel : PageModel
    {
        private readonly IAnimalManagementService _animalService;
        // private readonly IAdoptionService _adoptionService; // Tilføjes senere

        public DetaljerModel(IAnimalManagementService animalService /*, IAdoptionService adoptionService */)
        {
            _animalService = animalService;
            // _adoptionService = adoptionService; // Tilføjes senere
        }

        public Animal? Animal { get; set; }
        public IList<HealthRecord> HealthRecords { get; set; } = new List<HealthRecord>();
        public IList<Visit> Visits { get; set; } = new List<Visit>();

        // Evt. property til at styre om adoptionsknap skal vises
        // public bool CanAdopt { get; set; } = false;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Animal = await _animalService.GetAnimalByIdAsync(id);

            if (Animal == null)
            {
                return NotFound();
            }

            // Kun hent detaljer hvis dyret er tilgængeligt for adoption (eller anden logik baseret på Animal.Status)
            // Dette er en overvejelse - for nu henter vi altid detaljer, hvis dyret findes.
            // if (Animal.Status != ClassLibrary.Features.AnimalManagement.Core.Enums.AnimalStatus.Available)
            // {
            //     // Håndter visning for dyr der ikke er "Available" - måske en anden besked
            // }

            var healthRecordsTask = _animalService.GetHealthRecordsByAnimalIdAsync(id);
            var visitsTask = _animalService.GetVisitsByAnimalIdAsync(id);

            await Task.WhenAll(healthRecordsTask, visitsTask);

            HealthRecords = new List<HealthRecord>(await healthRecordsTask);
            Visits = new List<Visit>(await visitsTask);
            
            // Her kunne vi tjekke med _adoptionService om dyret kan adopteres
            // f.eks. _adoptionService.IsAnimalAvailableForAdoptionAsync(id)
            // CanAdopt = Animal.Status == ClassLibrary.Features.AnimalManagement.Core.Enums.AnimalStatus.Available; 

            return Page();
        }
    }
} 