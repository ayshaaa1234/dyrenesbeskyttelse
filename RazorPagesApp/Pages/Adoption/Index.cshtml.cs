using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using ClassLibrary.Features.AnimalManagement.Application.Abstractions;
using ClassLibrary.Features.AnimalManagement.Core.Models;
using ClassLibrary.Features.AnimalManagement.Core.Enums;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace RazorPagesApp.Pages.Adoption
{
    public class IndexModel : PageModel
    {
        private readonly IAnimalManagementService _animalService;
        public const int DefaultPageSize = 9; // 3x3 grid

        public IndexModel(IAnimalManagementService animalService)
        {
            _animalService = animalService;
        }

        public IList<Animal> AnimalsToAdopt { get; set; } = new List<Animal>();

        [BindProperty(SupportsGet = true)]
        public Species? SelectedSpecies { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? SelectedGender { get; set; }

        [BindProperty(SupportsGet = true)]
        public int CurrentPage { get; set; } = 1;

        public int PageSize { get; set; } = DefaultPageSize;
        public int TotalAnimals { get; set; }
        public int TotalPages => (int)Math.Ceiling(TotalAnimals / (double)PageSize);
        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;


        public List<SelectListItem> SpeciesOptions { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> GenderOptions { get; set; } = new List<SelectListItem>();

        public async Task OnGetAsync()
        {
            // Populate filter options (kun én gang, eller hvis de er dynamiske)
            if (!SpeciesOptions.Any())
            {
                SpeciesOptions = Enum.GetValues(typeof(Species))
                                    .Cast<Species>()
                                    .Select(s => new SelectListItem { Value = s.ToString(), Text = s.ToString() })
                                    .ToList();
                SpeciesOptions.Insert(0, new SelectListItem { Value = "", Text = "Alle Arter" });
            }
            if (!GenderOptions.Any())
            {
                GenderOptions = new List<SelectListItem>
                {
                    new SelectListItem { Value = "", Text = "Alle Køn" },
                    new SelectListItem { Value = "Han", Text = "Han" },
                    new SelectListItem { Value = "Hun", Text = "Hun" },
                };
            }

            var allAvailableAnimalsQuery = (await _animalService.GetAvailableAnimalsAsync()).AsQueryable();

            if (SelectedSpecies.HasValue)
            {
                allAvailableAnimalsQuery = allAvailableAnimalsQuery.Where(a => a.Species == SelectedSpecies.Value);
            }

            if (!string.IsNullOrEmpty(SelectedGender))
            {
                allAvailableAnimalsQuery = allAvailableAnimalsQuery.Where(a => a.Gender != null && a.Gender.Equals(SelectedGender, StringComparison.OrdinalIgnoreCase));
            }

            TotalAnimals = allAvailableAnimalsQuery.Count();

            AnimalsToAdopt = allAvailableAnimalsQuery
                                .Skip((CurrentPage - 1) * PageSize)
                                .Take(PageSize)
                                .ToList();
        }
    }
} 