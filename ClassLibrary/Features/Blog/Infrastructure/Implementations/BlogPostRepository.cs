using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClassLibrary.Features.Blog.Core.Models; // Opdateret
using ClassLibrary.Features.Blog.Infrastructure.Abstractions; // Opdateret
using ClassLibrary.SharedKernel.Persistence.Implementations; // For Repository<T>
using ClassLibrary.Infrastructure.DataInitialization; // Tilføjet for JsonDataInitializer
// using ClassLibrary.SharedKernel.Persistence.Abstractions; // Ikke længere nødvendig direkte her

namespace ClassLibrary.Features.Blog.Infrastructure.Implementations
{
    /// <summary>
    /// Repository til håndtering af blogindlæg
    /// </summary>
    public class BlogPostRepository : Repository<BlogPost>, IBlogPostRepository
    {
        public BlogPostRepository() : base(Path.Combine(JsonDataInitializer.CalculatedWorkspaceRoot, "Data", "Json", "blogposts.json"))
        {
        }

        public override async Task<BlogPost> AddAsync(BlogPost entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            ValidateEntity(entity);
            // ID-generering håndteres nu af base.AddAsync, hvis entity.Id er 0
            return await base.AddAsync(entity);
        }

        public override async Task<BlogPost> UpdateAsync(BlogPost entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            ValidateEntity(entity);
            return await base.UpdateAsync(entity);
        }

        /// <summary>
        /// Finder alle offentliggjorte blogindlæg
        /// </summary>
        public async Task<IEnumerable<BlogPost>> GetPublishedPostsAsync()
        {
            return await base.FindAsync(b => b.IsPublished);
        }

        /// <summary>
        /// Finder blogindlæg for en bestemt forfatter
        /// </summary>
        public async Task<IEnumerable<BlogPost>> GetByAuthorIdAsync(int authorId)
        {
            if (authorId <= 0)
                throw new ArgumentException("AuthorId skal være større end 0", nameof(authorId));
            return await base.FindAsync(b => b.AuthorId == authorId);
        }

        /// <summary>
        /// Finder blogindlæg baseret på tags
        /// </summary>
        public async Task<IEnumerable<BlogPost>> GetByTagsAsync(string tags)
        {
            if (string.IsNullOrWhiteSpace(tags))
            {
                return Enumerable.Empty<BlogPost>(); 
            }

            var tagList = tags.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(t => t.ToLowerInvariant()) 
                .ToList();
            
            if (!tagList.Any())
            {
                return Enumerable.Empty<BlogPost>();
            }
            
            return await base.FindAsync(b => b.Tags != null && b.Tags.Any(bt => tagList.Contains(bt.ToLowerInvariant())));
        }

        /// <summary>
        /// Finder blogindlæg baseret på datointerval
        /// </summary>
        public async Task<IEnumerable<BlogPost>> GetByPublishDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
                throw new ArgumentException("Startdato skal være før slutdato");
            
            return await base.FindAsync(b => b.PublishDate.Date >= startDate.Date && b.PublishDate.Date <= endDate.Date);
        }

        /// <summary>
        /// Finder blogindlæg baseret på søgeord
        /// </summary>
        public async Task<IEnumerable<BlogPost>> SearchAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return Enumerable.Empty<BlogPost>();
            }
            var term = searchTerm.ToLowerInvariant();

            return await base.FindAsync(b => 
                (b.Title != null && b.Title.ToLowerInvariant().Contains(term)) ||
                (b.Content != null && b.Content.ToLowerInvariant().Contains(term)) ||
                (b.Summary != null && b.Summary.ToLowerInvariant().Contains(term)));
        }

        /// <summary>
        /// Finder de seneste blogindlæg
        /// </summary>
        public async Task<IEnumerable<BlogPost>> GetLatestPostsAsync(int count)
        {
            if (count <= 0)
                throw new ArgumentException("Antal indlæg skal være større end 0", nameof(count));

            var allActivePosts = await base.GetAllAsync();
            return allActivePosts
                .Where(p => p.IsPublished)
                .OrderByDescending(b => b.PublishDate)
                .Take(count);
        }

        /// <summary>
        /// Validerer et blogindlæg
        /// </summary>
        protected override void ValidateEntity(BlogPost entity)
        {
            base.ValidateEntity(entity);

            if (string.IsNullOrWhiteSpace(entity.Title))
                throw new ArgumentException("Titel kan ikke være tom", nameof(entity.Title));

            if (string.IsNullOrWhiteSpace(entity.Content))
                throw new ArgumentException("Indhold kan ikke være tomt", nameof(entity.Content));

            if (string.IsNullOrWhiteSpace(entity.Summary))
                throw new ArgumentException("Resumé kan ikke være tomt", nameof(entity.Summary));

            if (entity.AuthorId <= 0)
                throw new ArgumentException("Forfatter ID skal være større end 0", nameof(entity.AuthorId));

            if (entity.IsPublished && entity.PublishDate > DateTime.UtcNow.AddMinutes(1))
                throw new ArgumentException("Publiceringsdato for et publiceret indlæg kan ikke være i fremtiden.", nameof(entity.PublishDate));
            
            if (entity.IsPublished && entity.PublishDate == DateTime.MinValue)
                entity.PublishDate = DateTime.UtcNow;

            if (entity.Tags == null)
                entity.Tags = new List<string>();

            if (entity.Tags.Any(string.IsNullOrWhiteSpace))
                throw new ArgumentException("Tags kan ikke indeholde tomme eller whitespace værdier.", nameof(entity.Tags));
        }
    }
} 