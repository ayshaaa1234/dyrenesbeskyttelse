using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClassLibrary.Models;
using ClassLibrary.Interfaces;

namespace ClassLibrary.Services
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
                throw new ArgumentException("ID skal være større end 0");

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
            return await _blogPostRepository.AddAsync(blogPost);
        }

        /// <summary>
        /// Opdaterer et eksisterende blogindlæg
        /// </summary>
        public async Task<BlogPost> UpdateBlogPostAsync(BlogPost blogPost)
        {
            if (blogPost == null)
                throw new ArgumentNullException(nameof(blogPost));

            ValidateBlogPost(blogPost);
            return await _blogPostRepository.UpdateAsync(blogPost);
        }

        /// <summary>
        /// Sletter et blogindlæg
        /// </summary>
        public async Task DeleteBlogPostAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("ID skal være større end 0");

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
                throw new ArgumentException("AuthorId skal være større end 0");

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
                throw new ArgumentException("Tags kan ikke være tomme");

            return await _blogPostRepository.GetByTagsAsync(tags);
        }

        /// <summary>
        /// Søger efter blogindlæg
        /// </summary>
        public async Task<IEnumerable<BlogPost>> SearchBlogPostsAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                throw new ArgumentException("Søgeord kan ikke være tomt");

            return await _blogPostRepository.SearchAsync(searchTerm);
        }

        /// <summary>
        /// Henter de seneste blogindlæg
        /// </summary>
        public async Task<IEnumerable<BlogPost>> GetLatestBlogPostsAsync(int count)
        {
            if (count <= 0)
                throw new ArgumentException("Antal indlæg skal være større end 0");

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
                throw new ArgumentException("Tag kan ikke være tomt");

            var blogPost = await GetBlogPostByIdAsync(id);
            blogPost.AddTag(tag);
            await _blogPostRepository.UpdateAsync(blogPost);
        }

        /// <summary>
        /// Fjerner et tag fra et blogindlæg
        /// </summary>
        public async Task RemoveTagFromBlogPostAsync(int id, string tag)
        {
            if (string.IsNullOrWhiteSpace(tag))
                throw new ArgumentException("Tag kan ikke være tomt");

            var blogPost = await GetBlogPostByIdAsync(id);
            blogPost.RemoveTag(tag);
            await _blogPostRepository.UpdateAsync(blogPost);
        }

        /// <summary>
        /// Tilføjer et like til et blogindlæg
        /// </summary>
        public async Task LikeBlogPostAsync(int id)
        {
            var blogPost = await GetBlogPostByIdAsync(id);
            blogPost.Likes++;
            await _blogPostRepository.UpdateAsync(blogPost);
        }

        /// <summary>
        /// Fjerner et like fra et blogindlæg
        /// </summary>
        public async Task UnlikeBlogPostAsync(int id)
        {
            var blogPost = await GetBlogPostByIdAsync(id);
            if (blogPost.Likes > 0)
            {
                blogPost.Likes--;
                await _blogPostRepository.UpdateAsync(blogPost);
            }
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

            if (string.IsNullOrWhiteSpace(blogPost.Author))
                throw new ArgumentException("Forfatter kan ikke være tom");

            if (string.IsNullOrWhiteSpace(blogPost.Category))
                throw new ArgumentException("Kategori kan ikke være tom");

            if (string.IsNullOrWhiteSpace(blogPost.Summary))
                throw new ArgumentException("Resumé kan ikke være tomt");

            if (blogPost.AuthorId <= 0)
                throw new ArgumentException("AuthorId skal være større end 0");

            if (blogPost.PublishDate > DateTime.Now)
                throw new ArgumentException("Publiceringsdato kan ikke være i fremtiden");
        }
    }
} 