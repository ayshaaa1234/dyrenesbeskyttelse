using ClassLibrary.Features.Blog.Application.Abstractions;
using ClassLibrary.Features.Blog.Core.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RazorPagesApp.Pages.Nyheder
{
    public class IndexModel : PageModel
    {
        private readonly IBlogPostService _blogPostService;

        public IndexModel(IBlogPostService blogPostService)
        {
            _blogPostService = blogPostService;
        }

        public IEnumerable<BlogPost> BlogPosts { get; private set; } = new List<BlogPost>();

        public async Task OnGetAsync()
        {
            BlogPosts = await _blogPostService.GetPublishedPostsAsync();
        }
    }
} 