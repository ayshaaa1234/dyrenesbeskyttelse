using System.Threading.Tasks;
using ClassLibrary.Features.AnimalManagement.Application.Abstractions;
using ClassLibrary.Features.AnimalManagement.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic; // For KeyNotFoundException

namespace RazorPagesApp.Pages.Admin.Animals
{
    // TODO: Tilføj [Authorize(Roles = "Administrator")]
    public class DeleteModel : PageModel
    {
        private readonly IAnimalManagementService _animalService;

        public DeleteModel(IAnimalManagementService animalService)
        {
            _animalService = animalService;
        }

        [BindProperty]
        public Animal Animal { get; set; } = default!;
        
        [TempData]
        public string? ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var animalFromDb = await _animalService.GetAnimalByIdAsync(id.Value);
            if (animalFromDb == null)
            {
                return NotFound();
            }
            // Tjek om dyret allerede er slettet (hvis soft delete er aktivt i GetByIdAsync)
            // Eller hvis din GetByIdAsync ikke returnerer soft-deleted, er dette tjek ok.
            if (animalFromDb.IsDeleted)
            {
                TempData["Message"] = $"Dyret '{animalFromDb.Name}' (ID: {id.Value}) er allerede slettet.";
                return RedirectToPage("./Index");
            }
            Animal = animalFromDb;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var animalToDelete = await _animalService.GetAnimalByIdAsync(id.Value);
            if (animalToDelete == null)
            {
                // Dyr findes ikke (måske slettet af en anden i mellemtiden)
                TempData["Message"] = $"Dyr med ID {id.Value} blev ikke fundet og kunne ikke slettes.";
                return RedirectToPage("./Index");
            }
            
            // Ekstra sikkerhedstjek, hvis dyret er blevet slettet siden OnGet
            if (animalToDelete.IsDeleted)
            {
                TempData["Message"] = $"Dyret '{animalToDelete.Name}' (ID: {id.Value}) er allerede slettet.";
                return RedirectToPage("./Index");
            }

            try
            {
                await _animalService.DeleteAnimalAsync(id.Value); // Dette forventes at lave en soft delete
                TempData["Message"] = $"Dyret '{animalToDelete.Name}' (ID: {id.Value}) blev slettet succesfuldt.";
                return RedirectToPage("./Index");
            }
            catch (KeyNotFoundException ex) // Hvis DeleteAnimalAsync kaster dette specifikt
            {
                TempData["ErrorMessage"] = ex.Message; 
                return RedirectToPage("./Index"); // Eller vis fejl på Delete siden
            }
            catch (Exception)
            {
                // Log ex
                TempData["Message"] = $"Fejl under sletning af {Animal?.Name}.";
                return RedirectToPage("./Index");
            }
        }
    }
} 