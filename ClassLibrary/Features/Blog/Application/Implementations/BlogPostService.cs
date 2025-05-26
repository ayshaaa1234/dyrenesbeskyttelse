using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClassLibrary.Features.Blog.Core.Models; // Opdateret
using ClassLibrary.Features.Blog.Application.Abstractions; // Opdateret
using ClassLibrary.Features.Blog.Infrastructure.Abstractions; // Opdateret

namespace ClassLibrary.Features.Blog.Application.Implementations
{
    /// <summary>
    /// Service til håndtering af blogindlæg
    /// </summary>
    public class BlogPostService : IBlogPostService
    {
        private readonly IBlogPostRepository _blogPostRepository;

        /// <summary>
        /// Konstruktør
        /// </summary>
        public BlogPostService(IBlogPostRepository blogPostRepository)
        {
            _blogPostRepository = blogPostRepository ?? throw new ArgumentNullException(nameof(blogPostRepository));
        }

        /// <summary>
        /// Henter alle blogindlæg
        /// </summary>
        public async Task<IEnumerable<BlogPost>> GetAllBlogPostsAsync()
        {
            return await _blogPostRepository.GetAllAsync();
        }

        /// <summary>
        /// Henter et blogindlæg baseret på ID
        /// </summary>
        public async Task<BlogPost> GetBlogPostByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("ID skal være større end 0", nameof(id));

            var blogPost = await _blogPostRepository.GetByIdAsync(id);
            if (blogPost == null)
                throw new KeyNotFoundException($"Intet blogindlæg fundet med ID: {id}");

            return blogPost;
        }

        /// <summary>
        /// Opretter et nyt blogindlæg
        /// </summary>
        public async Task<BlogPost> CreateBlogPostAsync(BlogPost blogPost)
        {
            if (blogPost == null)
                throw new ArgumentNullException(nameof(blogPost));

            ValidateBlogPost(blogPost);
            blogPost.IsPublished = false;
            blogPost.Likes = 0;
            blogPost.Tags = blogPost.Tags ?? new List<string>();
            return await _blogPostRepository.AddAsync(blogPost);
        }

        /// <summary>
        /// Opdaterer et eksisterende blogindlæg
        /// </summary>
        public async Task<BlogPost> UpdateBlogPostAsync(BlogPost blogPost)
        {
            if (blogPost == null)
                throw new ArgumentNullException(nameof(blogPost));

            var existingPost = await GetBlogPostByIdAsync(blogPost.Id);
            ValidateBlogPost(blogPost);

            // Bevar eksisterende værdier
            blogPost.PublishDate = existingPost.PublishDate;
            blogPost.IsPublished = existingPost.IsPublished;
            blogPost.Likes = existingPost.Likes;
            blogPost.Tags = blogPost.Tags ?? existingPost.Tags ?? new List<string>();

            return await _blogPostRepository.UpdateAsync(blogPost);
        }

        /// <summary>
        /// Sletter et blogindlæg
        /// </summary>
        public async Task DeleteBlogPostAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("ID skal være større end 0", nameof(id));

            var blogPost = await GetBlogPostByIdAsync(id);
            if (blogPost.IsPublished)
                throw new InvalidOperationException("Kan ikke slette et publiceret blogindlæg. Afpublicer det først.");

            await _blogPostRepository.DeleteAsync(id);
        }

        /// <summary>
        /// Henter alle publicerede blogindlæg
        /// </summary>
        public async Task<IEnumerable<BlogPost>> GetPublishedPostsAsync()
        {
            return await _blogPostRepository.GetPublishedPostsAsync();
        }

        /// <summary>
        /// Henter blogindlæg for en bestemt forfatter
        /// </summary>
        public async Task<IEnumerable<BlogPost>> GetBlogPostsByAuthorAsync(int authorId)
        {
            if (authorId <= 0)
                throw new ArgumentException("AuthorId skal være større end 0", nameof(authorId));

            return await _blogPostRepository.GetByAuthorIdAsync(authorId);
        }

        /// <summary>
        /// Henter blogindlæg publiceret i et datointerval
        /// </summary>
        public async Task<IEnumerable<BlogPost>> GetBlogPostsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
                throw new ArgumentException("Startdato skal være før slutdato");

            return await _blogPostRepository.GetByPublishDateRangeAsync(startDate, endDate);
        }

        /// <summary>
        /// Henter blogindlæg baseret på tags
        /// </summary>
        public async Task<IEnumerable<BlogPost>> GetBlogPostsByTagsAsync(string tags)
        {
            if (string.IsNullOrWhiteSpace(tags))
                throw new ArgumentException("Tags kan ikke være tomme", nameof(tags));

            return await _blogPostRepository.GetByTagsAsync(tags);
        }

        /// <summary>
        /// Søger efter blogindlæg
        /// </summary>
        public async Task<IEnumerable<BlogPost>> SearchBlogPostsAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                throw new ArgumentException("Søgeord kan ikke være tomt", nameof(searchTerm));

            return await _blogPostRepository.SearchAsync(searchTerm);
        }

        /// <summary>
        /// Henter de seneste blogindlæg
        /// </summary>
        public async Task<IEnumerable<BlogPost>> GetLatestBlogPostsAsync(int count)
        {
            if (count <= 0)
                throw new ArgumentException("Antal indlæg skal være større end 0", nameof(count));

            return await _blogPostRepository.GetLatestPostsAsync(count);
        }

        /// <summary>
        /// Publicerer et blogindlæg
        /// </summary>
        public async Task PublishBlogPostAsync(int id)
        {
            var blogPost = await GetBlogPostByIdAsync(id);
            if (blogPost.IsPublished)
                throw new InvalidOperationException("Blogindlægget er allerede publiceret");

            if (string.IsNullOrWhiteSpace(blogPost.Summary))
                throw new InvalidOperationException("Blogindlægget skal have et resumé før det kan publiceres");

            blogPost.IsPublished = true;
            blogPost.PublishDate = DateTime.Now;
            await _blogPostRepository.UpdateAsync(blogPost);
        }

        /// <summary>
        /// Afpublicerer et blogindlæg
        /// </summary>
        public async Task UnpublishBlogPostAsync(int id)
        {
            var blogPost = await GetBlogPostByIdAsync(id);
            if (!blogPost.IsPublished)
                throw new InvalidOperationException("Blogindlægget er allerede afpubliceret");

            blogPost.IsPublished = false;
            await _blogPostRepository.UpdateAsync(blogPost);
        }

        /// <summary>
        /// Tilføjer et tag til et blogindlæg
        /// </summary>
        public async Task AddTagToBlogPostAsync(int id, string tag)
        {
            if (string.IsNullOrWhiteSpace(tag))
                throw new ArgumentException("Tag kan ikke være tomt", nameof(tag));

            var blogPost = await GetBlogPostByIdAsync(id);
            if (blogPost.Tags == null)
                blogPost.Tags = new List<string>();

            if (blogPost.Tags.Contains(tag, StringComparer.OrdinalIgnoreCase))
                throw new InvalidOperationException("Tagget findes allerede på blogindlægget");

            blogPost.Tags.Add(tag.Trim());
            await _blogPostRepository.UpdateAsync(blogPost);
        }

        /// <summary>
        /// Fjerner et tag fra et blogindlæg
        /// </summary>
        public async Task RemoveTagFromBlogPostAsync(int id, string tag)
        {
            if (string.IsNullOrWhiteSpace(tag))
                throw new ArgumentException("Tag kan ikke være tomt", nameof(tag));

            var blogPost = await GetBlogPostByIdAsync(id);
            if (blogPost.Tags == null || !blogPost.Tags.Contains(tag, StringComparer.OrdinalIgnoreCase))
                throw new InvalidOperationException("Tagget findes ikke på blogindlægget");

            blogPost.Tags.RemoveAll(t => t.Equals(tag, StringComparison.OrdinalIgnoreCase));
            await _blogPostRepository.UpdateAsync(blogPost);
        }

        /// <summary>
        /// Tilføjer et like til et blogindlæg
        /// </summary>
        public async Task LikeBlogPostAsync(int id)
        {
            var blogPost = await GetBlogPostByIdAsync(id);
            if (!blogPost.IsPublished)
                throw new InvalidOperationException("Kan ikke like et ikke-publiceret blogindlæg");

            blogPost.Likes++;
            await _blogPostRepository.UpdateAsync(blogPost);
        }

        /// <summary>
        /// Fjerner et like fra et blogindlæg
        /// </summary>
        public async Task UnlikeBlogPostAsync(int id)
        {
            var blogPost = await GetBlogPostByIdAsync(id);
            if (!blogPost.IsPublished)
                throw new InvalidOperationException("Kan ikke unlike et ikke-publiceret blogindlæg");

            if (blogPost.Likes <= 0)
                throw new InvalidOperationException("Blogindlægget har ingen likes at fjerne");

            blogPost.Likes--;
            await _blogPostRepository.UpdateAsync(blogPost);
        }

        /// <summary>
        /// Validerer et blogindlæg
        /// </summary>
        private void ValidateBlogPost(BlogPost blogPost)
        {
            if (string.IsNullOrWhiteSpace(blogPost.Title))
                throw new ArgumentException("Titel kan ikke være tom");

            if (string.IsNullOrWhiteSpace(blogPost.Content))
                throw new ArgumentException("Indhold kan ikke være tomt");

            if (blogPost.AuthorId <= 0)
                throw new ArgumentException("Forfatter ID skal være større end 0");

            if (blogPost.PublishDate > DateTime.Now.AddDays(1) && blogPost.IsPublished) // Småjustering for at tillade publicering samme dag
                throw new ArgumentException("Publiceringsdato for et publiceret indlæg kan ikke være i fremtiden");
        }
    }
} 