using System.Threading.Tasks;
using ClassLibrary.Features.AnimalManagement.Application.Abstractions;
using ClassLibrary.Features.AnimalManagement.Core.Models;
using ClassLibrary.Features.AnimalManagement.Core.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Linq;

namespace RazorPagesApp.Pages.Admin.Animals
{
    // TODO: Tilføj [Authorize(Roles = "Administrator")]
    public class CreateModel : PageModel
    {
        private readonly IAnimalManagementService _animalService;

        public CreateModel(IAnimalManagementService animalService)
        {
            _animalService = animalService;
        }

        [BindProperty]
        public Animal Animal { get; set; } = new Animal { IntakeDate = DateTime.Today }; // Initialiser med standardværdier

        // SelectLister til Enums
        public SelectList SpeciesList { get; set; } = default!;
        public SelectList GenderList { get; set; } = default!;
        public SelectList StatusList { get; set; } = default!;

        public void OnGet()
        {
            PopulateSelectLists();
            // Sæt standardværdier for nye dyr, hvis det ønskes
            Animal.Status = ClassLibrary.Features.AnimalManagement.Core.Enums.AnimalStatus.Available; 
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                PopulateSelectLists();
                return Page();
            }

            try
            {
                await _animalService.CreateAnimalAsync(Animal);
                TempData["Message"] = $"Dyret '{Animal.Name}' blev oprettet succesfuldt.";
                return RedirectToPage("./Index");
            }
            catch (ArgumentException ex) // Eller specifikke exceptions fra din service/repo
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                PopulateSelectLists();
                return Page();
            }
            catch (Exception)
            {
                // Log ex - overvej at logge her
                ModelState.AddModelError(string.Empty, "En fejl opstod under oprettelse af dyret.");
                PopulateSelectLists();
                return Page();
            }
        }

        private void PopulateSelectLists()
        {
            SpeciesList = new SelectList(Enum.GetValues(typeof(Species)).Cast<Species>().Select(e => new { Value = e, Text = e.ToString() }), "Value", "Text");
            GenderList = new SelectList(Enum.GetValues(typeof(Gender)).Cast<Gender>().Select(e => new { Value = e, Text = e.ToString() }), "Value", "Text");
            StatusList = new SelectList(Enum.GetValues(typeof(AnimalStatus)).Cast<AnimalStatus>().Select(e => new { Value = e, Text = e.ToString() }), "Value", "Text");
        }
    }
} 