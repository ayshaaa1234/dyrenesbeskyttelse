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
    /// <summary>
    /// PageModel for oprettelse af nye dyr.
    /// </summary>
    // TODO: Tilføj [Authorize(Roles = "Administrator")]
    public class CreateModel : PageModel
    {
        private readonly IAnimalManagementService _animalService;

        /// <summary>
        /// Initialiserer en ny instans af <see cref="CreateModel"/> klassen.
        /// </summary>
        /// <param name="animalService">Servicen til håndtering af dyredata.</param>
        public CreateModel(IAnimalManagementService animalService)
        {
            _animalService = animalService;
        }

        /// <summary>
        /// Henter eller sætter det dyreobjekt, der bindes til formularen.
        /// Initialiseres med standardværdier for et nyt dyr.
        /// </summary>
        [BindProperty]
        public Animal Animal { get; set; } = new Animal { IntakeDate = DateTime.Today };

        // Fjernet: SelectLister som properties. Bruges nu via ViewData.
        // public SelectList SpeciesList { get; set; } = default!;
        // public SelectList GenderList { get; set; } = default!;
        // public SelectList StatusList { get; set; } = default!;

        /// <summary>
        /// Håndterer HTTP GET-anmodningen.
        /// Forbereder siden ved at udfylde nødvendige SelectLister i ViewData og sætter standardstatus for dyret.
        /// </summary>
        public void OnGet()
        {
            PopulateSelectListsInViewData();
            Animal.Status = ClassLibrary.Features.AnimalManagement.Core.Enums.AnimalStatus.Available; 
        }

        /// <summary>
        /// Håndterer HTTP POST-anmodningen for at oprette et nyt dyr.
        /// Validerer model-state og kalder dyreservice for at persistere det nye dyr.
        /// </summary>
        /// <returns>En <see cref="IActionResult"/> der repræsenterer resultatet af operationen.</returns>
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                PopulateSelectListsInViewData();
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
                PopulateSelectListsInViewData();
                return Page();
            }
            catch (Exception)
            {
                // Log ex - overvej at logge her
                ModelState.AddModelError(string.Empty, "En fejl opstod under oprettelse af dyret.");
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