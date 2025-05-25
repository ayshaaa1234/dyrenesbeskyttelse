using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClassLibrary.Models;
using ClassLibrary.Interfaces;

namespace ClassLibrary.Repositories
{
    /// <summary>
    /// Repository til håndtering af blogindlæg
    /// </summary>
    public class BlogPostRepository : Repository<BlogPost>, IBlogPostRepository
    {
        public BlogPostRepository() : base()
        {
        }

        /// <summary>
        /// Finder alle offentliggjorte blogindlæg
        /// </summary>
        public Task<IEnumerable<BlogPost>> GetPublishedPostsAsync()
        {
            return Task.FromResult(_items.Where(b => b.IsPublished));
        }

        /// <summary>
        /// Finder blogindlæg for en bestemt forfatter
        /// </summary>
        public Task<IEnumerable<BlogPost>> GetByAuthorIdAsync(int authorId)
        {
            if (authorId <= 0)
                throw new ArgumentException("AuthorId skal være større end 0");

            return Task.FromResult(_items.Where(b => b.AuthorId == authorId));
        }

        /// <summary>
        /// Finder blogindlæg baseret på tags
        /// </summary>
        public Task<IEnumerable<BlogPost>> GetByTagsAsync(string tags)
        {
            if (string.IsNullOrWhiteSpace(tags))
                throw new ArgumentException("Tags kan ikke være tomme");

            var tagList = tags.Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(t => t.Trim())
                .ToList();

            return Task.FromResult(_items.Where(b => 
                tagList.Any(t => b.HasTag(t))));
        }

        /// <summary>
        /// Finder blogindlæg baseret på datointerval
        /// </summary>
        public Task<IEnumerable<BlogPost>> GetByPublishDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
                throw new ArgumentException("Startdato skal være før slutdato");

            return Task.FromResult(_items.Where(b => 
                b.PublishDate >= startDate && b.PublishDate <= endDate));
        }

        /// <summary>
        /// Finder blogindlæg baseret på søgeord
        /// </summary>
        public Task<IEnumerable<BlogPost>> SearchAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                throw new ArgumentException("Søgeord kan ikke være tomt");

            return Task.FromResult(_items.Where(b => 
                b.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                b.Content.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                b.Summary.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)));
        }

        /// <summary>
        /// Finder de seneste blogindlæg
        /// </summary>
        public Task<IEnumerable<BlogPost>> GetLatestPostsAsync(int count)
        {
            if (count <= 0)
                throw new ArgumentException("Antal indlæg skal være større end 0");

            return Task.FromResult(_items
                .OrderByDescending(b => b.PublishDate)
                .Take(count));
        }
    }
} 