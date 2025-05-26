using System.Threading.Tasks;
using ClassLibrary.Features.AnimalManagement.Application.Abstractions;
using ClassLibrary.Features.AnimalManagement.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic; // Nødvendig for KeyNotFoundException
using System; // Nødvendig for Exception

namespace RazorPagesApp.Pages.Admin.Animals
{
    /// <summary>
    /// PageModel for sletning af dyr.
    /// </summary>
    // TODO: Tilføj [Authorize(Roles = "Administrator")]
    public class DeleteModel : PageModel
    {
        private readonly IAnimalManagementService _animalService;

        /// <summary>
        /// Initialiserer en ny instans af <see cref="DeleteModel"/> klassen.
        /// </summary>
        /// <param name="animalService">Servicen til håndtering af dyredata.</param>
        public DeleteModel(IAnimalManagementService animalService)
        {
            _animalService = animalService;
        }

        /// <summary>
        /// Henter eller sætter det dyreobjekt, der skal slettes. Bindes til visning af dyrets detaljer før sletning.
        /// </summary>
        [BindProperty]
        public Animal Animal { get; set; } = default!;
        
        /// <summary>
        /// Henter eller sætter en eventuel fejlmeddelelse, der skal vises på siden.
        /// Anvender TempData til at persistere meddelelsen over en redirect.
        /// </summary>
        [TempData]
        public string? ErrorMessage { get; set; }

        /// <summary>
        /// Håndterer HTTP GET-anmodningen for at vise bekræftelsessiden før sletning af et dyr.
        /// </summary>
        /// <param name="id">ID'et på det dyr, der overvejes slettet.</param>
        /// <returns>En <see cref="IActionResult"/> der repræsenterer resultatet af operationen.</returns>
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound(); // ID er påkrævet
            }

            var animalFromDb = await _animalService.GetAnimalByIdAsync(id.Value);
            if (animalFromDb == null)
            {
                return NotFound(); // Dyr ikke fundet
            }

            // Tjek om dyret allerede er markeret som slettet (hvis soft delete er implementeret)
            if (animalFromDb.IsDeleted)
            {
                TempData["Message"] = $"Dyret '{animalFromDb.Name}' (ID: {id.Value}) er allerede slettet.";
                return RedirectToPage("./Index");
            }
            Animal = animalFromDb;
            return Page();
        }

        /// <summary>
        /// Håndterer HTTP POST-anmodningen for at udføre sletningen af et dyr.
        /// </summary>
        /// <param name="id">ID'et på det dyr, der skal slettes.</param>
        /// <returns>En <see cref="IActionResult"/> der repræsenterer resultatet af operationen.</returns>
        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound(); // ID er påkrævet
            }

            var animalToDelete = await _animalService.GetAnimalByIdAsync(id.Value);
            if (animalToDelete == null)
            {
                // Dyr findes ikke (måske slettet af en anden bruger i mellemtiden)
                TempData["Message"] = $"Dyr med ID {id.Value} blev ikke fundet og kunne ikke slettes.";
                return RedirectToPage("./Index");
            }
            
            // Ekstra sikkerhedstjek: Hvis dyret er blevet markeret som slettet siden OnGetAsync blev kaldt
            if (animalToDelete.IsDeleted)
            {
                TempData["Message"] = $"Dyret '{animalToDelete.Name}' (ID: {id.Value}) er allerede slettet.";
                return RedirectToPage("./Index");
            }

            try
            {
                // Udfør sletning (forventeligt soft delete via servicen)
                await _animalService.DeleteAnimalAsync(id.Value); 
                TempData["Message"] = $"Dyret '{animalToDelete.Name}' (ID: {id.Value}) blev slettet succesfuldt.";
                return RedirectToPage("./Index");
            }
            catch (KeyNotFoundException ex) // Håndter hvis DeleteAnimalAsync specifikt kaster denne
            {
                // Denne fejl kan opstå, hvis dyret slettes af en anden proces mellem GetAnimalByIdAsync og DeleteAnimalAsync.
                TempData["ErrorMessage"] = ex.Message; 
                // Overvej at returnere til Delete-siden med fejlen, hvis det giver mere mening end Index.
                return RedirectToPage("./Index"); 
            }
            catch (Exception) // Generel fejlhåndtering
            {
                // Overvej at logge den fulde exception her
                TempData["Message"] = $"Fejl under sletning af {Animal?.Name}.";
                return RedirectToPage("./Index");
            }
        }
    }
} 