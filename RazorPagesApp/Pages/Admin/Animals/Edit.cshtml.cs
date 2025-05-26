using System.Threading.Tasks;
using ClassLibrary.Features.AnimalManagement.Application.Abstractions;
using ClassLibrary.Features.AnimalManagement.Core.Models;
using ClassLibrary.Features.AnimalManagement.Core.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Linq;
using System.Collections.Generic; // For KeyNotFoundException

namespace RazorPagesApp.Pages.Admin.Animals
{
    // TODO: Tilføj [Authorize(Roles = "Administrator")]
    public class EditModel : PageModel
    {
        private readonly IAnimalManagementService _animalService;

        public EditModel(IAnimalManagementService animalService)
        {
            _animalService = animalService;
        }

        [BindProperty]
        public Animal Animal { get; set; } = default!;

        public SelectList SpeciesList { get; set; } = default!;
        public SelectList GenderList { get; set; } = default!;
        public SelectList StatusList { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound(); // Eller BadRequest()
            }

            var animalFromDb = await _animalService.GetAnimalByIdAsync(id.Value);
            if (animalFromDb == null)
            {
                return NotFound();
            }
            Animal = animalFromDb;
            PopulateSelectLists();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Fjern IsAdopted fra ModelState, hvis det ikke skal kunne ændres direkte her
            // ModelState.Remove("Animal.IsAdopted"); 
            // ModelState.Remove("Animal.AdoptionDate");
            // ModelState.Remove("Animal.AdoptedByCustomerId");

            if (!ModelState.IsValid)
            {
                PopulateSelectLists();
                return Page();
            }

            try
            {
                await _animalService.UpdateAnimalAsync(Animal);
                TempData["Message"] = $"Dyret '{Animal.Name}' blev opdateret succesfuldt.";
                return RedirectToPage("./Index");
            }
            catch (KeyNotFoundException ex) 
            {
                ModelState.AddModelError(string.Empty, ex.Message); // Specifik fejl for ikke fundet
                PopulateSelectLists();
                return Page();
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                PopulateSelectLists();
                return Page();
            }
            catch (Exception)
            {
                // Log ex - overvej at logge her
                ModelState.AddModelError(string.Empty, "En fejl opstod under opdatering af dyret.");
                PopulateSelectLists();
                return Page();
            }
        }

        private void PopulateSelectLists()
        {
            SpeciesList = new SelectList(Enum.GetValues(typeof(Species)).Cast<Species>().Select(e => new { Value = e, Text = e.ToString() }), "Value", "Text", Animal?.Species);
            GenderList = new SelectList(Enum.GetValues(typeof(Gender)).Cast<Gender>().Select(e => new { Value = e, Text = e.ToString() }), "Value", "Text", Animal?.Gender);
            StatusList = new SelectList(Enum.GetValues(typeof(AnimalStatus)).Cast<AnimalStatus>().Select(e => new { Value = e, Text = e.ToString() }), "Value", "Text", Animal?.Status);
        }
    }
} 