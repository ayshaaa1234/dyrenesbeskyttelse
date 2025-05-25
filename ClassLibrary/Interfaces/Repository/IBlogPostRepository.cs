using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClassLibrary.Models;

namespace ClassLibrary.Interfaces
{
    /// <summary>
    /// Interface for repository til håndtering af blogindlæg
    /// </summary>
    public interface IBlogPostRepository : IRepository<BlogPost>
    {
        /// <summary>
        /// Finder alle publicerede blogindlæg
        /// </summary>
        Task<IEnumerable<BlogPost>> GetPublishedPostsAsync();

        /// <summary>
        /// Finder blogindlæg for en bestemt forfatter
        /// </summary>
        Task<IEnumerable<BlogPost>> GetByAuthorIdAsync(int authorId);

        /// <summary>
        /// Finder blogindlæg publiceret i et bestemt datointerval
        /// </summary>
        Task<IEnumerable<BlogPost>> GetByPublishDateRangeAsync(DateTime startDate, DateTime endDate);

        /// <summary>
        /// Finder blogindlæg baseret på tags
        /// </summary>
        Task<IEnumerable<BlogPost>> GetByTagsAsync(string tags);

        /// <summary>
        /// Finder blogindlæg baseret på søgeord i titel eller indhold
        /// </summary>
        Task<IEnumerable<BlogPost>> SearchAsync(string searchTerm);

        /// <summary>
        /// Finder de seneste blogindlæg
        /// </summary>
        Task<IEnumerable<BlogPost>> GetLatestPostsAsync(int count);
    }
} 