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
    /// <summary>
    /// PageModel for redigering af eksisterende dyr.
    /// </summary>
    // TODO: Tilføj [Authorize(Roles = "Administrator")]
    public class EditModel : PageModel
    {
        private readonly IAnimalManagementService _animalService;

        /// <summary>
        /// Initialiserer en ny instans af <see cref="EditModel"/> klassen.
        /// </summary>
        /// <param name="animalService">Servicen til håndtering af dyredata.</param>
        public EditModel(IAnimalManagementService animalService)
        {
            _animalService = animalService;
        }

        /// <summary>
        /// Henter eller sætter det dyreobjekt, der bindes til formularen for redigering.
        /// </summary>
        [BindProperty]
        public Animal Animal { get; set; } = default!;

        /// <summary>
        /// Håndterer HTTP GET-anmodningen for at hente et dyr til redigering.
        /// </summary>
        /// <param name="id">ID'et på det dyr, der skal redigeres.</param>
        /// <returns>En <see cref="IActionResult"/> der repræsenterer resultatet af operationen.</returns>
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound(); // ID er påkrævet for at finde et dyr
            }

            var animalFromDb = await _animalService.GetAnimalByIdAsync(id.Value);
            if (animalFromDb == null)
            {
                return NotFound(); // Dyr med det angivne ID blev ikke fundet
            }
            Animal = animalFromDb;
            // Udfyld SelectLister for dropdowns i formularen
            PopulateSelectListsInViewData();
            return Page();
        }

        /// <summary>
        /// Håndterer HTTP POST-anmodningen for at gemme ændringer til et dyr.
        /// </summary>
        /// <returns>En <see cref="IActionResult"/> der repræsenterer resultatet af operationen.</returns>
        public async Task<IActionResult> OnPostAsync()
        {
            // Overvej at fjerne properties fra ModelState, som ikke skal kunne ændres via denne formular,
            // f.eks. IsAdopted, AdoptionDate, etc., hvis de håndteres andetsteds.
            // Eksempel: ModelState.Remove("Animal.IsAdopted"); 

            if (!ModelState.IsValid)
            {
                // Hvis model-state ikke er valid, genindlæs siden med valideringsfejl.
                PopulateSelectListsInViewData();
                return Page();
            }

            try
            {
                // Forsøg at opdatere dyret via servicen
                await _animalService.UpdateAnimalAsync(Animal);
                // Sæt en succesmeddelelse i TempData
                TempData["Message"] = $"Dyret '{Animal.Name}' blev opdateret succesfuldt.";
                return RedirectToPage("./Index");
            }
            catch (KeyNotFoundException ex) // Håndter specifikt, hvis dyret ikke længere findes
            {
                ModelState.AddModelError(string.Empty, ex.Message); 
                PopulateSelectListsInViewData();
                return Page();
            }
            catch (ArgumentException ex) // Håndter andre forventede fejl fra servicen
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                PopulateSelectListsInViewData();
                return Page();
            }
            catch (Exception) // Generel fejlhåndtering
            {
                // Overvej at logge den fulde exception her
                ModelState.AddModelError(string.Empty, "En fejl opstod under opdatering af dyret.");
                PopulateSelectListsInViewData();
                return Page();
            }
        }

        /// <summary>
        /// Udfylder ViewData med SelectLister for Species, Gender og Status.
        /// Disse bruges til at populere dropdown-menuer i formularen.
        /// </summary>
        private void PopulateSelectListsInViewData()
        {
            ViewData["SpeciesList"] = new SelectList(Enum.GetValues(typeof(Species)).Cast<Species>().Select(e => new { Value = e, Text = e.ToString() }), "Value", "Text", Animal?.Species);
            ViewData["GenderList"] = new SelectList(Enum.GetValues(typeof(Gender)).Cast<Gender>().Select(e => new { Value = e, Text = e.ToString() }), "Value", "Text", Animal?.Gender);
            ViewData["StatusList"] = new SelectList(Enum.GetValues(typeof(AnimalStatus)).Cast<AnimalStatus>().Select(e => new { Value = e, Text = e.ToString() }), "Value", "Text", Animal?.Status);
        }
    }
} 