using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RazorPagesApp.Pages.Adoption
{
    // PageModel for bekræftelsessiden efter en adoptionsansøgning er sendt.
    public class TakForAnsogningModel : PageModel
    {
        // Properties til at holde data, der skal vises på siden (hentet fra TempData).
        public int? AdoptionId { get; set; }
        public string? AnimalName { get; set; }

        // Handler for GET-requests. Kaldes når siden indlæses.
        public IActionResult OnGet()
        {
            // Forsøger at hente AdoptionId og AnimalName fra TempData.
            // TempData bruges til at overføre data mellem redirects (fra AnsogningModel til denne side).
            if (TempData["AdoptionId"] is int adoptionId && TempData["AnimalName"] is string animalName)
            {
                // Hvis data findes i TempData, tildeles de til sidens properties.
                AdoptionId = adoptionId;
                AnimalName = animalName;
                return Page(); // Returnerer Razor Page med de hentede data.
            }
            // Hvis nødvendige data ikke findes i TempData (f.eks. hvis brugeren navigerer direkte til denne side), 
            // omdirigeres brugeren til adoptionsindexsiden for at undgå fejl eller forvirring.
            return RedirectToPage("./Index");
        }
    }
} 