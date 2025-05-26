using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClassLibrary.Features.Blog.Core.Models; // Opdateret

namespace ClassLibrary.Features.Blog.Application.Abstractions
{
    /// <summary>
    /// Interface for service til håndtering af blogindlæg
    /// </summary>
    public interface IBlogPostService
    {
        /// <summary>
        /// Henter alle blogindlæg
        /// </summary>
        Task<IEnumerable<BlogPost>> GetAllBlogPostsAsync();

        /// <summary>
        /// Henter et blogindlæg baseret på ID
        /// </summary>
        Task<BlogPost> GetBlogPostByIdAsync(int id);

        /// <summary>
        /// Opretter et nyt blogindlæg
        /// </summary>
        Task<BlogPost> CreateBlogPostAsync(BlogPost blogPost);

        /// <summary>
        /// Opdaterer et eksisterende blogindlæg
        /// </summary>
        Task<BlogPost> UpdateBlogPostAsync(BlogPost blogPost);

        /// <summary>
        /// Sletter et blogindlæg
        /// </summary>
        Task DeleteBlogPostAsync(int id);

        /// <summary>
        /// Henter alle publicerede blogindlæg
        /// </summary>
        Task<IEnumerable<BlogPost>> GetPublishedPostsAsync();

        /// <summary>
        /// Henter blogindlæg for en bestemt forfatter
        /// </summary>
        Task<IEnumerable<BlogPost>> GetBlogPostsByAuthorAsync(int authorId);

        /// <summary>
        /// Henter blogindlæg publiceret i et datointerval
        /// </summary>
        Task<IEnumerable<BlogPost>> GetBlogPostsByDateRangeAsync(DateTime startDate, DateTime endDate);

        /// <summary>
        /// Henter blogindlæg baseret på tags
        /// </summary>
        Task<IEnumerable<BlogPost>> GetBlogPostsByTagsAsync(string tags);

        /// <summary>
        /// Søger efter blogindlæg
        /// </summary>
        Task<IEnumerable<BlogPost>> SearchBlogPostsAsync(string searchTerm);

        /// <summary>
        /// Henter de seneste blogindlæg
        /// </summary>
        Task<IEnumerable<BlogPost>> GetLatestBlogPostsAsync(int count);

        /// <summary>
        /// Publicerer et blogindlæg
        /// </summary>
        Task PublishBlogPostAsync(int id);

        /// <summary>
        /// Afpublicerer et blogindlæg
        /// </summary>
        Task UnpublishBlogPostAsync(int id);

        /// <summary>
        /// Tilføjer et tag til et blogindlæg
        /// </summary>
        Task AddTagToBlogPostAsync(int id, string tag);

        /// <summary>
        /// Fjerner et tag fra et blogindlæg
        /// </summary>
        Task RemoveTagFromBlogPostAsync(int id, string tag);

        /// <summary>
        /// Tilføjer et like til et blogindlæg
        /// </summary>
        Task LikeBlogPostAsync(int id);

        /// <summary>
        /// Fjerner et like fra et blogindlæg
        /// </summary>
        Task UnlikeBlogPostAsync(int id);
    }
} 