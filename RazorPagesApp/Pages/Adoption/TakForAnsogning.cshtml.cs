using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RazorPagesApp.Pages.Adoption
{
    public class TakForAnsogningModel : PageModel
    {
        public int? AdoptionId { get; set; }
        public string? AnimalName { get; set; }

        public IActionResult OnGet()
        {
            if (TempData["AdoptionId"] is int adoptionId && TempData["AnimalName"] is string animalName)
            {
                AdoptionId = adoptionId;
                AnimalName = animalName;
                return Page();
            }
            // Hvis TempData mangler, er det sandsynligvis en direkte navigation, send til adoptionsforsiden.
            return RedirectToPage("./Index");
        }
    }
} 