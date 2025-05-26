using ClassLibrary.Features.Blog.Application.Abstractions;
using ClassLibrary.Features.Blog.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace RazorPagesApp.Pages
{
    // PageModel for forsiden (Index).
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger; // Service til logging.
        private readonly IBlogPostService _blogPostService; // Service til at hente blogindlægsdata.

        // Dependency injection af ILogger og IBlogPostService.
        public IndexModel(ILogger<IndexModel> logger, IBlogPostService blogPostService)
        {
            _logger = logger;
            _blogPostService = blogPostService;
        }

        // Property til at holde listen af de seneste blogindlæg, der skal vises på forsiden.
        // Den har en privat setter, da den kun skal sættes internt i denne klasse.
        public IEnumerable<BlogPost> LatestBlogPosts { get; private set; } = new List<BlogPost>();

        // Handler for GET-requests. Kaldes når forsiden indlæses.
        public async Task OnGetAsync()
        {
            try
            {
                // Henter de 3 seneste publicerede blogindlæg fra servicen.
                LatestBlogPosts = await _blogPostService.GetLatestBlogPostsAsync(3);
            }
            catch (Exception ex) // Fanger eventuelle fejl under hentning af blogindlæg.
            {
                _logger.LogError(ex, "Fejl ved hentning af seneste blogindlæg til forsiden.");
                LatestBlogPosts = new List<BlogPost>(); // Sætter listen til tom ved fejl for at undgå null reference exceptions i Razor Page.
            }
        }
    }
}
