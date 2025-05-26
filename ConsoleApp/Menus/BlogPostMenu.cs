using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using ClassLibrary.Services;
using ClassLibrary.Models;

namespace ConsoleApp.Menus
{
    /// <summary>
    /// Menu til håndtering af blogindlæg
    /// </summary>
    public class BlogPostMenu : MenuBase
    {
        private readonly BlogPostService _blogPostService;

        public BlogPostMenu(BlogPostService blogPostService)
        {
            _blogPostService = blogPostService;
        }

        public override async Task ShowAsync()
        {
            while (true)
            {
                ShowHeader("Blogindlæg");
                Console.WriteLine("1. Vis alle blogindlæg");
                Console.WriteLine("2. Vis blogindlæg efter ID");
                Console.WriteLine("3. Vis publicerede indlæg");
                Console.WriteLine("4. Vis indlæg efter forfatter");
                Console.WriteLine("5. Vis indlæg efter dato");
                Console.WriteLine("6. Vis indlæg efter tags");
                Console.WriteLine("7. Søg i indlæg");
                Console.WriteLine("8. Vis seneste indlæg");
                Console.WriteLine("9. Opret nyt indlæg");
                Console.WriteLine("10. Opdater indlæg");
                Console.WriteLine("11. Slet indlæg");
                Console.WriteLine("12. Publicer indlæg");
                Console.WriteLine("13. Afpublicer indlæg");
                Console.WriteLine("14. Tilføj tag");
                Console.WriteLine("15. Fjern tag");
                Console.WriteLine("16. Like indlæg");
                Console.WriteLine("17. Unlike indlæg");
                Console.WriteLine("0. Tilbage til hovedmenu");
                Console.Write("\nVælg en mulighed: ");

                var choice = Console.ReadLine();

                try
                {
                    switch (choice)
                    {
                        case "1":
                            await ShowAllBlogPosts();
                            break;
                        case "2":
                            await ShowBlogPostById();
                            break;
                        case "3":
                            await ShowPublishedPosts();
                            break;
                        case "4":
                            await ShowPostsByAuthor();
                            break;
                        case "5":
                            await ShowPostsByDate();
                            break;
                        case "6":
                            await ShowPostsByTags();
                            break;
                        case "7":
                            await SearchPosts();
                            break;
                        case "8":
                            await ShowLatestPosts();
                            break;
                        case "9":
                            await CreateNewBlogPost();
                            break;
                        case "10":
                            await UpdateBlogPost();
                            break;
                        case "11":
                            await DeleteBlogPost();
                            break;
                        case "12":
                            await PublishBlogPost();
                            break;
                        case "13":
                            await UnpublishBlogPost();
                            break;
                        case "14":
                            await AddTag();
                            break;
                        case "15":
                            await RemoveTag();
                            break;
                        case "16":
                            await LikeBlogPost();
                            break;
                        case "17":
                            await UnlikeBlogPost();
                            break;
                        case "0":
                            return;
                        default:
                            ShowError("Ugyldigt valg");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    HandleException(ex);
                }
            }
        }

        private async Task ShowAllBlogPosts()
        {
            ShowHeader("Alle blogindlæg");
            var posts = await _blogPostService.GetAllBlogPostsAsync();
            DisplayBlogPosts(posts);
        }

        private async Task ShowBlogPostById()
        {
            ShowHeader("Blogindlæg efter ID");
            Console.Write("Indtast indlægs ID: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                ShowError("Ugyldigt ID");
                return;
            }

            var post = await _blogPostService.GetBlogPostByIdAsync(id);
            DisplayBlogPostInfo(post);
        }

        private async Task ShowPublishedPosts()
        {
            ShowHeader("Publicerede indlæg");
            var posts = await _blogPostService.GetPublishedPostsAsync();
            DisplayBlogPosts(posts);
        }

        private async Task ShowPostsByAuthor()
        {
            ShowHeader("Indlæg efter forfatter");
            Console.Write("Indtast forfatter ID: ");
            if (!int.TryParse(Console.ReadLine(), out int authorId))
            {
                ShowError("Ugyldigt ID");
                return;
            }

            var posts = await _blogPostService.GetBlogPostsByAuthorAsync(authorId);
            DisplayBlogPosts(posts);
        }

        private async Task ShowPostsByDate()
        {
            ShowHeader("Indlæg efter dato");
            Console.Write("Indtast startdato (dd/mm/yyyy): ");
            if (!DateTime.TryParse(Console.ReadLine(), out DateTime startDate))
            {
                ShowError("Ugyldig startdato");
                return;
            }

            Console.Write("Indtast slutdato (dd/mm/yyyy): ");
            if (!DateTime.TryParse(Console.ReadLine(), out DateTime endDate))
            {
                ShowError("Ugyldig slutdato");
                return;
            }

            var posts = await _blogPostService.GetBlogPostsByDateRangeAsync(startDate, endDate);
            DisplayBlogPosts(posts);
        }

        private async Task ShowPostsByTags()
        {
            ShowHeader("Indlæg efter tags");
            Console.Write("Indtast tags (kommasepareret): ");
            var tags = Console.ReadLine() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(tags))
            {
                ShowError("Tags kan ikke være tomme");
                return;
            }

            var posts = await _blogPostService.GetBlogPostsByTagsAsync(tags);
            DisplayBlogPosts(posts);
        }

        private async Task SearchPosts()
        {
            ShowHeader("Søg i indlæg");
            Console.Write("Indtast søgeord: ");
            var searchTerm = Console.ReadLine() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                ShowError("Søgeord kan ikke være tomt");
                return;
            }

            var posts = await _blogPostService.SearchBlogPostsAsync(searchTerm);
            DisplayBlogPosts(posts);
        }

        private async Task ShowLatestPosts()
        {
            ShowHeader("Seneste indlæg");
            Console.Write("Indtast antal indlæg: ");
            if (!int.TryParse(Console.ReadLine(), out int count))
            {
                ShowError("Ugyldigt antal");
                return;
            }

            var posts = await _blogPostService.GetLatestBlogPostsAsync(count);
            DisplayBlogPosts(posts);
        }

        private async Task CreateNewBlogPost()
        {
            ShowHeader("Opret nyt indlæg");

            Console.Write("Indtast titel: ");
            var title = Console.ReadLine() ?? string.Empty;

            Console.Write("Indtast indhold: ");
            var content = Console.ReadLine() ?? string.Empty;

            Console.Write("Indtast billede URL (valgfrit): ");
            var imageUrl = Console.ReadLine();

            Console.Write("Indtast forfatter ID: ");
            if (!int.TryParse(Console.ReadLine(), out int authorId))
            {
                ShowError("Ugyldigt forfatter ID");
                return;
            }

            Console.Write("Indtast forfatter navn: ");
            var author = Console.ReadLine() ?? string.Empty;

            Console.Write("Indtast kategori: ");
            var category = Console.ReadLine() ?? string.Empty;

            Console.Write("Indtast tags (kommasepareret): ");
            var tags = Console.ReadLine() ?? string.Empty;

            Console.Write("Indtast resumé: ");
            var summary = Console.ReadLine() ?? string.Empty;

            var post = new BlogPost
            {
                Title = title,
                Content = content,
                ImageUrl = imageUrl,
                AuthorId = authorId,
                Author = author,
                Category = category,
                Tags = tags,
                Summary = summary,
                PublishDate = DateTime.Now,
                IsPublished = false
            };

            await _blogPostService.CreateBlogPostAsync(post);
            ShowSuccess("Blogindlæg oprettet succesfuldt!");
        }

        private async Task UpdateBlogPost()
        {
            ShowHeader("Opdater indlæg");

            Console.Write("Indtast indlægs ID: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                ShowError("Ugyldigt ID");
                return;
            }

            var post = await _blogPostService.GetBlogPostByIdAsync(id);
            if (post == null)
            {
                ShowError("Indlæg ikke fundet");
                return;
            }

            Console.WriteLine("\nNuværende information:");
            DisplayBlogPostInfo(post);
            Console.WriteLine("\nIndtast ny information (tryk Enter for at beholde nuværende værdi):");

            Console.Write($"Titel [{post.Title}]: ");
            var title = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(title))
                post.Title = title;

            Console.Write($"Indhold [{post.Content}]: ");
            var content = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(content))
                post.Content = content;

            Console.Write($"Billede URL [{post.ImageUrl}]: ");
            var imageUrl = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(imageUrl))
                post.ImageUrl = imageUrl;

            Console.Write($"Kategori [{post.Category}]: ");
            var category = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(category))
                post.Category = category;

            Console.Write($"Resumé [{post.Summary}]: ");
            var summary = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(summary))
                post.Summary = summary;

            await _blogPostService.UpdateBlogPostAsync(post);
            ShowSuccess("Blogindlæg opdateret succesfuldt!");
        }

        private async Task DeleteBlogPost()
        {
            ShowHeader("Slet indlæg");

            Console.Write("Indtast indlægs ID: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                ShowError("Ugyldigt ID");
                return;
            }

            var post = await _blogPostService.GetBlogPostByIdAsync(id);
            if (post == null)
            {
                ShowError("Indlæg ikke fundet");
                return;
            }

            Console.WriteLine("\nEr du sikker på, at du vil slette dette indlæg?");
            DisplayBlogPostInfo(post);
            Console.Write("\nSkriv 'JA' for at bekræfte: ");
            
            if (Console.ReadLine()?.ToUpper() != "JA")
            {
                Console.WriteLine("Sletning annulleret.");
                Console.WriteLine("\nTryk på en tast for at fortsætte...");
                Console.ReadKey();
                return;
            }

            await _blogPostService.DeleteBlogPostAsync(id);
            ShowSuccess("Blogindlæg slettet succesfuldt!");
        }

        private async Task PublishBlogPost()
        {
            ShowHeader("Publicer indlæg");

            Console.Write("Indtast indlægs ID: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                ShowError("Ugyldigt ID");
                return;
            }

            await _blogPostService.PublishBlogPostAsync(id);
            ShowSuccess("Blogindlæg publiceret succesfuldt!");
        }

        private async Task UnpublishBlogPost()
        {
            ShowHeader("Afpublicer indlæg");

            Console.Write("Indtast indlægs ID: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                ShowError("Ugyldigt ID");
                return;
            }

            await _blogPostService.UnpublishBlogPostAsync(id);
            ShowSuccess("Blogindlæg afpubliceret succesfuldt!");
        }

        private async Task AddTag()
        {
            ShowHeader("Tilføj tag");

            Console.Write("Indtast indlægs ID: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                ShowError("Ugyldigt ID");
                return;
            }

            Console.Write("Indtast tag: ");
            var tag = Console.ReadLine() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(tag))
            {
                ShowError("Tag kan ikke være tomt");
                return;
            }

            await _blogPostService.AddTagToBlogPostAsync(id, tag);
            ShowSuccess("Tag tilføjet succesfuldt!");
        }

        private async Task RemoveTag()
        {
            ShowHeader("Fjern tag");

            Console.Write("Indtast indlægs ID: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                ShowError("Ugyldigt ID");
                return;
            }

            Console.Write("Indtast tag: ");
            var tag = Console.ReadLine() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(tag))
            {
                ShowError("Tag kan ikke være tomt");
                return;
            }

            await _blogPostService.RemoveTagFromBlogPostAsync(id, tag);
            ShowSuccess("Tag fjernet succesfuldt!");
        }

        private async Task LikeBlogPost()
        {
            ShowHeader("Like indlæg");

            Console.Write("Indtast indlægs ID: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                ShowError("Ugyldigt ID");
                return;
            }

            await _blogPostService.LikeBlogPostAsync(id);
            ShowSuccess("Indlæg liket succesfuldt!");
        }

        private async Task UnlikeBlogPost()
        {
            ShowHeader("Unlike indlæg");

            Console.Write("Indtast indlægs ID: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                ShowError("Ugyldigt ID");
                return;
            }

            await _blogPostService.UnlikeBlogPostAsync(id);
            ShowSuccess("Like fjernet succesfuldt!");
        }

        private void DisplayBlogPosts(IEnumerable<BlogPost> posts)
        {
            if (!posts.Any())
            {
                Console.WriteLine("Ingen blogindlæg fundet.");
            }
            else
            {
                foreach (var post in posts)
                {
                    DisplayBlogPostInfo(post);
                    Console.WriteLine(new string('-', 50));
                }
            }

            Console.WriteLine("\nTryk på en tast for at fortsætte...");
            Console.ReadKey();
        }

        private void DisplayBlogPostInfo(BlogPost post)
        {
            Console.WriteLine($"ID: {post.Id}");
            Console.WriteLine($"Titel: {post.Title}");
            Console.WriteLine($"Forfatter: {post.Author}");
            Console.WriteLine($"Kategori: {post.Category}");
            Console.WriteLine($"Publiceret: {(post.IsPublished ? "Ja" : "Nej")}");
            if (post.IsPublished)
            {
                Console.WriteLine($"Publiceringsdato: {post.PublishDate:dd/MM/yyyy HH:mm}");
            }
            Console.WriteLine($"Likes: {post.Likes}");
            if (!string.IsNullOrWhiteSpace(post.Tags))
            {
                Console.WriteLine($"Tags: {post.Tags}");
            }
            Console.WriteLine($"Resumé: {post.Summary}");
            if (!string.IsNullOrWhiteSpace(post.ImageUrl))
            {
                Console.WriteLine($"Billede URL: {post.ImageUrl}");
            }
            Console.WriteLine("\nIndhold:");
            Console.WriteLine(post.Content);
        }
    }
}
