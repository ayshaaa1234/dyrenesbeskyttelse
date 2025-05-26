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
    // TODO: Tilføj [Authorize(Roles = "Administrator")] eller lignende
    public class IndexModel : PageModel
    {
        private readonly IAnimalManagementService _animalService;

        public IndexModel(IAnimalManagementService animalService)
        {
            _animalService = animalService;
        }

        public IList<Animal> Animals { get; set; } = new List<Animal>();

        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        [BindProperty(SupportsGet = true)]
        public AnimalStatus? Status { get; set; }

        [BindProperty(SupportsGet = true)]
        public bool? NeedsVaccination { get; set; }

        [TempData]
        public string? Message { get; set; }

        public async Task OnGetAsync()
        {
            IEnumerable<Animal> animalsResult;

            if (Status.HasValue)
            {
                animalsResult = await _animalService.GetAnimalsByAdoptionStatusAsync(Status.Value);
            }
            else if (NeedsVaccination.HasValue && NeedsVaccination.Value)
            {
                animalsResult = await _animalService.GetAnimalsNeedingVaccinationAsync();
            }
            else if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                animalsResult = await _animalService.GetAnimalsByNameAsync(SearchTerm);
            }
            else
            {
                animalsResult = await _animalService.GetAllAnimalsAsync();
            }
            
            Animals = animalsResult?.ToList() ?? new List<Animal>();
        }
    }
} 