using ClassLibrary.Features.Blog.Application.Abstractions;
using ClassLibrary.Features.Blog.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RazorPagesApp.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IBlogPostService _blogPostService;

        public IndexModel(ILogger<IndexModel> logger, IBlogPostService blogPostService)
        {
            _logger = logger;
            _blogPostService = blogPostService;
        }

        public IEnumerable<BlogPost> LatestBlogPosts { get; private set; } = new List<BlogPost>();

        public async Task OnGetAsync()
        {
            try
            {
                // Hent de 3 seneste publicerede blogindlæg
                LatestBlogPosts = await _blogPostService.GetLatestBlogPostsAsync(3);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fejl ved hentning af seneste blogindlæg til forsiden.");
                LatestBlogPosts = new List<BlogPost>(); 
            }
        }
    }
}
