using ClassLibrary.Features.Blog.Application.Abstractions;
using ClassLibrary.Features.Blog.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging; // Tilføjet for logging funktionalitet.
using System.Threading.Tasks; // Tilføjet for asynkron programmering med Task.
using System; // Tilføjet for Exception klassen.

namespace RazorPagesApp.Pages.Nyheder
{
    // PageModel for siden, der viser et enkelt blogindlæg.
    public class IndlaegModel : PageModel
    {
        private readonly IBlogPostService _blogPostService; // Service til at hente blogindlægsdata.
        private readonly ILogger<IndlaegModel> _logger; // Service til logging.

        // Dependency injection af IBlogPostService og ILogger.
        public IndlaegModel(IBlogPostService blogPostService, ILogger<IndlaegModel> logger)
        {
            _blogPostService = blogPostService;
            _logger = logger;
        }

        // Property til at holde det blogindlæg, der skal vises på siden.
        // Den har en privat setter, da den kun skal sættes internt i denne klasse.
        public BlogPost? BlogPost { get; private set; }

        // Handler for GET-requests. Kaldes når siden indlæses med et ID-parameter.
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null) // Tjekker om ID-parameteret er angivet.
            {
                _logger.LogWarning("ID parameter mangler for at hente blogindlæg.");
                return NotFound(); // Returnerer 404 Not Found, hvis ID mangler.
            }

            try
            {
                // Henter blogindlægget fra servicen baseret på det angivne ID.
                BlogPost = await _blogPostService.GetBlogPostByIdAsync(id.Value);
            }
            catch (Exception ex) // Fanger eventuelle fejl under hentning af blogindlægget.
            {
                _logger.LogError(ex, $"Fejl ved hentning af blogindlæg med ID: {id.Value}");
                // TODO: Overvej at vise en mere brugervenlig fejlside i stedet for NotFound, hvis det er mere passende.
                return NotFound(); // Returnerer 404 Not Found ved fejl.
            }
            
            // Tjekker om blogindlægget blev fundet og om det er publiceret.
            if (BlogPost == null || !BlogPost.IsPublished)
            {
                _logger.LogWarning($"Blogindlæg med ID: {id.Value} blev ikke fundet eller er ikke publiceret.");
                return NotFound(); // Returnerer 404 Not Found, hvis indlægget ikke findes eller ikke er publiceret.
            }

            ViewData["Title"] = BlogPost.Title; // Sætter sidens titel til blogindlæggets titel.
            return Page(); // Returnerer Razor Page med de hentede data.
        }
    }
} 