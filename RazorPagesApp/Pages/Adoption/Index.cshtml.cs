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
        public Gender? SelectedGender { get; set; }

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
            // Populate filter options
            if (!SpeciesOptions.Any())
            {
                SpeciesOptions = Enum.GetValues(typeof(Species))
                                    .Cast<Species>()
                                    .Select(s => new SelectListItem { Value = s.ToString(), Text = GetSpeciesDisplayName(s) })
                                    .ToList();
                SpeciesOptions.Insert(0, new SelectListItem { Value = "", Text = "Alle Arter" });
            }
            if (!GenderOptions.Any())
            {
                GenderOptions = Enum.GetValues(typeof(Gender))
                                    .Cast<Gender>()
                                    .Select(g => new SelectListItem { Value = g.ToString(), Text = GetGenderDisplayName(g) })
                                    .ToList();
                GenderOptions.Insert(0, new SelectListItem { Value = "", Text = "Alle Køn" });
            }

            var allAvailableAnimalsQuery = (await _animalService.GetAvailableAnimalsAsync()).AsQueryable();

            if (SelectedSpecies.HasValue)
            {
                allAvailableAnimalsQuery = allAvailableAnimalsQuery.Where(a => a.Species == SelectedSpecies.Value);
            }

            if (SelectedGender.HasValue)
            {
                // Antager at 'Ukendt' køn ikke skal med, medmindre det specifikt er valgt.
                // Hvis SelectedGender er 'Unknown', så inkluder dem.
                // Ellers, match på det specifikke køn.
                if (SelectedGender.Value == Gender.Unknown)
                {
                     allAvailableAnimalsQuery = allAvailableAnimalsQuery.Where(a => a.Gender == SelectedGender.Value);
                }
                else
                {
                    // For Male eller Female, inkluder dyr med det specifikke køn.
                    // Hvis man vil ekskludere Unknown helt når et specifikt køn er valgt, skal det ikke med her.
                    allAvailableAnimalsQuery = allAvailableAnimalsQuery.Where(a => a.Gender == SelectedGender.Value && a.Gender != Gender.Unknown);
                }
            }

            TotalAnimals = allAvailableAnimalsQuery.Count();

            AnimalsToAdopt = allAvailableAnimalsQuery
                                .Skip((CurrentPage - 1) * PageSize)
                                .Take(PageSize)
                                .ToList();
        }

        // Helper methods for display names (kan flyttes til et mere centralt sted senere)
        private string GetSpeciesDisplayName(Species species)
        {
            // Simpel implementering - kan udvides med [Display(Name="...")] attributter på enum
            return species.ToString(); 
        }

        private string GetGenderDisplayName(Gender gender)
        {
            return gender switch
            {
                Gender.Male => "Han",
                Gender.Female => "Hun",
                Gender.Unknown => "Ukendt",
                _ => gender.ToString()
            };
        }
    }
} 