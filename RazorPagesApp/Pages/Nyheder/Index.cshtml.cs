using ClassLibrary.Features.Blog.Application.Abstractions;
using ClassLibrary.Features.Blog.Core.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RazorPagesApp.Pages.Nyheder
{
    // PageModel for Nyheder Index-siden, der viser en liste over publicerede blogindlæg.
    public class IndexModel : PageModel
    {
        private readonly IBlogPostService _blogPostService; // Service til at hente blogindlægsdata.

        // Dependency injection af IBlogPostService.
        public IndexModel(IBlogPostService blogPostService)
        {
            _blogPostService = blogPostService;
        }

        // Property til at holde listen af blogindlæg, der skal vises på siden.
        // Den har en privat setter, da den kun skal sættes internt i denne klasse.
        public IEnumerable<BlogPost> BlogPosts { get; private set; } = new List<BlogPost>();

        // Handler for GET-requests. Kaldes når siden indlæses.
        public async Task OnGetAsync()
        {
            // Henter alle publicerede blogindlæg fra servicen.
            BlogPosts = await _blogPostService.GetPublishedPostsAsync();
        }
    }
} 