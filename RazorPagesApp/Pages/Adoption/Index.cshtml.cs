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
    // PageModel for Index-siden under Adoption, der viser en liste over dyr til adoption.
    public class IndexModel : PageModel
    {
        private readonly IAnimalManagementService _animalService; // Service til at hente dyredata.
        public const int DefaultPageSize = 9; // Standard antal dyr pr. side (passer til et 3x3 grid).

        // Dependency injection af IAnimalManagementService.
        public IndexModel(IAnimalManagementService animalService)
        {
            _animalService = animalService;
        }

        public IList<Animal> AnimalsToAdopt { get; set; } = new List<Animal>(); // Liste over dyr, der vises på den aktuelle side.

        // Properties til at binde filterværdier fra query-strenge (GET requests).
        [BindProperty(SupportsGet = true)]
        public Species? SelectedSpecies { get; set; } // Valgt art til filtrering.

        [BindProperty(SupportsGet = true)]
        public Gender? SelectedGender { get; set; } // Valgt køn til filtrering.

        [BindProperty(SupportsGet = true)]
        public int CurrentPage { get; set; } = 1; // Nuværende side nummer, default er 1.

        public int PageSize { get; set; } = DefaultPageSize; // Antal dyr pr. side.
        public int TotalAnimals { get; set; } // Samlet antal dyr, der matcher filtrene.
        public int TotalPages => (int)Math.Ceiling(TotalAnimals / (double)PageSize); // Beregner totalt antal sider.
        public bool HasPreviousPage => CurrentPage > 1; // Indikerer om der er en forrige side.
        public bool HasNextPage => CurrentPage < TotalPages; // Indikerer om der er en næste side.


        // Lister af SelectListItem til at populere filter dropdowns i Razor-siden.
        public List<SelectListItem> SpeciesOptions { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> GenderOptions { get; set; } = new List<SelectListItem>();

        // Handler for GET-requests. Kaldes når siden indlæses eller når filtre/paginering ændres.
        public async Task OnGetAsync()
        {
            // Populerer filter dropdowns, hvis de ikke allerede er fyldt.
            // Dette gøres typisk kun én gang, medmindre de skal opdateres dynamisk.
            if (!SpeciesOptions.Any())
            {
                SpeciesOptions = Enum.GetValues(typeof(Species))
                                    .Cast<Species>()
                                    .Select(s => new SelectListItem { Value = s.ToString(), Text = GetSpeciesDisplayName(s) })
                                    .ToList();
                SpeciesOptions.Insert(0, new SelectListItem { Value = "", Text = "Alle Arter" }); // Tilføjer "Alle Arter" som default valg.
            }
            if (!GenderOptions.Any())
            {
                GenderOptions = Enum.GetValues(typeof(Gender))
                                    .Cast<Gender>()
                                    .Select(g => new SelectListItem { Value = g.ToString(), Text = GetGenderDisplayName(g) })
                                    .ToList();
                GenderOptions.Insert(0, new SelectListItem { Value = "", Text = "Alle Køn" }); // Tilføjer "Alle Køn" som default valg.
            }

            // Henter alle tilgængelige dyr fra servicen og konverterer til IQueryable for yderligere filtrering.
            var allAvailableAnimalsQuery = (await _animalService.GetAvailableAnimalsAsync()).AsQueryable();

            // Anvender art-filter, hvis en art er valgt.
            if (SelectedSpecies.HasValue)
            {
                allAvailableAnimalsQuery = allAvailableAnimalsQuery.Where(a => a.Species == SelectedSpecies.Value);
            }

            // Anvender køn-filter, hvis et køn er valgt.
            if (SelectedGender.HasValue)
            {
                // Håndterer 'Ukendt' køn specifikt. Hvis 'Ukendt' er valgt, inkluderes kun dyr med 'Ukendt' køn.
                // Ellers (hvis 'Han' eller 'Hun' er valgt), inkluderes kun dyr med det specifikke køn og ekskluderes 'Ukendt'.
                if (SelectedGender.Value == Gender.Unknown)
                {
                     allAvailableAnimalsQuery = allAvailableAnimalsQuery.Where(a => a.Gender == SelectedGender.Value);
                }
                else
                {
                    allAvailableAnimalsQuery = allAvailableAnimalsQuery.Where(a => a.Gender == SelectedGender.Value && a.Gender != Gender.Unknown);
                }
            }

            TotalAnimals = allAvailableAnimalsQuery.Count(); // Tæller totalt antal dyr efter filtrering.

            // Anvender paginering: skipper de relevante dyr og tager kun dem for den aktuelle side.
            AnimalsToAdopt = allAvailableAnimalsQuery
                                .OrderBy(a => a.Name) // Sorterer dyrene alfabetisk efter navn (kan justeres).
                                .Skip((CurrentPage - 1) * PageSize)
                                .Take(PageSize)
                                .ToList();
        }

        // Hjælpemetode til at få et brugervenligt navn for en art (Species enum).
        // TODO: Overvej at flytte til en centraliseret helper-klasse eller bruge [Display] attributter på enums.
        private string GetSpeciesDisplayName(Species species)
        {
            // Simpel implementering - kan udvides med [Display(Name="...")] attributter på enum for bedre lokaliserbarhed.
            return species.ToString(); 
        }

        // Hjælpemetode til at få et brugervenligt navn for et køn (Gender enum).
        private string GetGenderDisplayName(Gender gender)
        {
            return gender switch
            {
                Gender.Male => "Han",
                Gender.Female => "Hun",
                Gender.Unknown => "Ukendt",
                _ => gender.ToString() // Fallback hvis en kønsværdi mangler oversættelse.
            };
        }
    }
} 