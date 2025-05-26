using ClassLibrary.Features.Blog.Application.Abstractions;
using ClassLibrary.Features.Blog.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging; // Tilføjet for logging
using System.Threading.Tasks; // Tilføjet for Task

namespace RazorPagesApp.Pages.Nyheder
{
    public class IndlaegModel : PageModel
    {
        private readonly IBlogPostService _blogPostService;
        private readonly ILogger<IndlaegModel> _logger;

        public IndlaegModel(IBlogPostService blogPostService, ILogger<IndlaegModel> logger)
        {
            _blogPostService = blogPostService;
            _logger = logger;
        }

        public BlogPost? BlogPost { get; private set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                _logger.LogWarning("ID parameter mangler for at hente blogindlæg.");
                return NotFound();
            }

            try
            {
                BlogPost = await _blogPostService.GetBlogPostByIdAsync(id.Value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Fejl ved hentning af blogindlæg med ID: {id.Value}");
                // Overvej at vise en fejlside i stedet for NotFound, hvis det er mere passende
                return NotFound(); // Eller en specifik fejlside
            }
            
            if (BlogPost == null || !BlogPost.IsPublished)
            {
                _logger.LogWarning($"Blogindlæg med ID: {id.Value} blev ikke fundet eller er ikke publiceret.");
                return NotFound();
            }

            ViewData["Title"] = BlogPost.Title;
            return Page();
        }
    }
} 