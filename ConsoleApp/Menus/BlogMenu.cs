using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using ClassLibrary.Features.Blog.Application.Abstractions;
using ClassLibrary.Features.Blog.Core.Models;

namespace ConsoleApp.Menus
{
    public class BlogMenu
    {
        private readonly IBlogPostService _blogPostService;
        // Overvej at injecte IEmployeeService hvis AuthorId skal valideres/forfatternavn hentes derfra
        // For nu antager vi at Author (navn) og AuthorId sættes manuelt.

        public BlogMenu(IBlogPostService blogPostService)
        {
            _blogPostService = blogPostService;
        }

        public async Task ShowAsync()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Blogadministration");
                Console.WriteLine("----------------------------------------");
                Console.WriteLine("1. Vis alle blogindlæg");
                Console.WriteLine("2. Opret nyt blogindlæg");
                Console.WriteLine("3. Opdater blogindlæg");
                Console.WriteLine("4. Slet blogindlæg");
                Console.WriteLine("5. Publicer blogindlæg");
                Console.WriteLine("6. Afpublicer blogindlæg");
                Console.WriteLine("7. Tilføj tag til blogindlæg");
                Console.WriteLine("8. Fjern tag fra blogindlæg");
                Console.WriteLine("9. Like blogindlæg");
                Console.WriteLine("10. Unlike blogindlæg");
                Console.WriteLine("11. Vis publicerede blogindlæg");
                Console.WriteLine("12. Søg efter blogindlæg");
                Console.WriteLine("13. Vis blogindlæg af forfatter (ID)");
                Console.WriteLine("14. Vis blogindlæg efter datointerval");
                Console.WriteLine("15. Vis blogindlæg efter tags");
                Console.WriteLine("16. Vis seneste blogindlæg");
                Console.WriteLine("0. Tilbage til hovedmenu");
                Console.WriteLine("----------------------------------------");
                Console.Write("Tag et valg: ");

                string? choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        await ListAllBlogPostsAsync();
                        break;
                    case "2":
                        await CreateBlogPostAsync();
                        break;
                    case "3":
                        await UpdateBlogPostAsync();
                        break;
                    case "4":
                        await DeleteBlogPostAsync();
                        break;
                    case "5":
                        await PublishBlogPostAsync();
                        break;
                    case "6":
                        await UnpublishBlogPostAsync();
                        break;
                    case "7":
                        await AddTagToBlogPostAsync();
                        break;
                    case "8":
                        await RemoveTagFromBlogPostAsync();
                        break;
                    case "9":
                        await LikeBlogPostAsync();
                        break;
                    case "10":
                        await UnlikeBlogPostAsync();
                        break;
                    case "11":
                        await ListPublishedBlogPostsAsync();
                        break;
                    case "12":
                        await SearchBlogPostsAsync();
                        break;
                    case "13":
                        await ListBlogPostsByAuthorAsync();
                        break;
                    case "14":
                        await ListBlogPostsByDateRangeAsync();
                        break;
                    case "15":
                        await ListBlogPostsByTagsAsync();
                        break;
                    case "16":
                        await ListLatestBlogPostsAsync();
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Ugyldigt valg. Prøv igen.");
                        Console.ReadKey();
                        break;
                }
            }
        }

        private async Task ListAllBlogPostsAsync(IEnumerable<BlogPost>? postsToDisplay = null)
        {
            Console.Clear();
            var blogPosts = postsToDisplay ?? await _blogPostService.GetAllBlogPostsAsync();

            if (!blogPosts.Any())
            {
                Console.WriteLine("Ingen blogindlæg fundet.");
            }
            else
            {
                Console.WriteLine("Alle blogindlæg:");
                foreach (var post in blogPosts)
                {
                    Console.WriteLine($"- ID: {post.Id}, Titel: \"{post.Title}\", Forfatter: {post.Author} (ID: {post.AuthorId})");
                    Console.WriteLine($"  Publiceret: {(post.IsPublished ? post.PublishDate.ToString("yyyy-MM-dd") : "Nej")}, Kategori: {post.Category}, Likes: {post.Likes}");
                    Console.WriteLine($"  Tags: {(post.Tags.Any() ? string.Join(", ", post.Tags) : "Ingen")}");
                    Console.WriteLine($"  Resumé: {post.Summary}");
                    Console.WriteLine($"  Oprettet: {post.CreatedAt:yyyy-MM-dd HH:mm}");
                    Console.WriteLine("---");
                }
            }
            Console.WriteLine("Tryk på en tast for at fortsætte...");
            Console.ReadKey();
        }

        private async Task CreateBlogPostAsync()
        {
            Console.Clear();
            Console.WriteLine("Opret nyt blogindlæg");

            Console.Write("Titel: ");
            string title = Console.ReadLine() ?? string.Empty;

            Console.Write("Indhold: ");
            string content = Console.ReadLine() ?? string.Empty;

            Console.Write("Billed-URL (valgfri): ");
            string? pictureUrl = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(pictureUrl)) pictureUrl = null;

            int authorId = GetIntFromUserInput("Forfatter ID (tal): ");

            Console.Write("Forfatternavn: ");
            string authorName = Console.ReadLine() ?? string.Empty;

            Console.Write("Kategori: ");
            string category = Console.ReadLine() ?? string.Empty;

            Console.Write("Resumé: ");
            string summary = Console.ReadLine() ?? string.Empty;
            
            Console.Write("Tags (kommasepareret, valgfri): ");
            string tagsInput = Console.ReadLine() ?? string.Empty;
            List<string> tags = tagsInput.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();

            var newBlogPost = new BlogPost
            {
                Title = title,
                Content = content,
                PictureUrl = pictureUrl,
                AuthorId = authorId,
                Author = authorName,
                Category = category,
                Summary = summary,
                Tags = tags,
                // IsPublished, PublishDate, CreatedAt, Likes sættes af modellen/servicen
            };

            try
            {
                var createdPost = await _blogPostService.CreateBlogPostAsync(newBlogPost);
                Console.WriteLine($"Blogindlæg \"{createdPost.Title}\" oprettet med ID: {createdPost.Id}.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fejl ved oprettelse af blogindlæg: {ex.Message}");
            }
            Console.WriteLine("Tryk på en tast for at fortsætte...");
            Console.ReadKey();
        }

        private async Task UpdateBlogPostAsync()
        {
            Console.Clear();
            int postId = GetIntFromUserInput("Indtast ID på blogindlægget der skal opdateres: ");
            var post = await _blogPostService.GetBlogPostByIdAsync(postId);

            if (post == null)
            {
                Console.WriteLine($"Blogindlæg med ID {postId} ikke fundet.");
                Console.ReadKey();
                return;
            }

            Console.WriteLine($"Opdaterer \"{post.Title}\" (ID: {post.Id})");

            Console.Write($"Ny titel (nuværende: \"{post.Title}\"): ");
            post.Title = Console.ReadLine() ?? post.Title;

            Console.Write($"Nyt indhold (tryk Enter for at beholde eksisterende): ");
            string? newContent = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(newContent)) post.Content = newContent;

            Console.Write($"Ny billed-URL (nuværende: \"{post.PictureUrl ?? "Ingen"}\"): ");
            string? newPictureUrl = Console.ReadLine();
            post.PictureUrl = string.IsNullOrWhiteSpace(newPictureUrl) ? null : newPictureUrl;
            
            Console.Write($"Nyt forfatter ID (nuværende: {post.AuthorId}): ");
            string? authorIdInput = Console.ReadLine();
            if (int.TryParse(authorIdInput, out int newAuthorId)) post.AuthorId = newAuthorId;

            Console.Write($"Nyt forfatternavn (nuværende: \"{post.Author}\"): ");
            post.Author = Console.ReadLine() ?? post.Author;

            Console.Write($"Ny kategori (nuværende: \"{post.Category}\"): ");
            post.Category = Console.ReadLine() ?? post.Category;
            
            Console.Write($"Nyt resumé (tryk Enter for at beholde eksisterende): ");
            string? newSummary = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(newSummary)) post.Summary = newSummary;

            Console.Write($"Nye tags (kommasepareret, nuværende: \"{string.Join(", ", post.Tags)}\"): ");
            string? tagsInput = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(tagsInput))
            {
                post.Tags = tagsInput.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();
            }


            try
            {
                await _blogPostService.UpdateBlogPostAsync(post);
                Console.WriteLine($"Blogindlæg \"{post.Title}\" opdateret.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fejl ved opdatering af blogindlæg: {ex.Message}");
            }
            Console.WriteLine("Tryk på en tast for at fortsætte...");
            Console.ReadKey();
        }

        private async Task DeleteBlogPostAsync()
        {
            Console.Clear();
            int postId = GetIntFromUserInput("Indtast ID på blogindlægget der skal slettes: ");
            var post = await _blogPostService.GetBlogPostByIdAsync(postId);

            if (post == null)
            {
                Console.WriteLine($"Blogindlæg med ID {postId} ikke fundet.");
            }
            else
            {
                Console.Write($"Er du sikker på du vil slette \"{post.Title}\" (ID: {postId})? (ja/nej): ");
                if (Console.ReadLine()?.ToLower() == "ja")
                {
                    try
                    {
                        await _blogPostService.DeleteBlogPostAsync(postId);
                        Console.WriteLine($"Blogindlæg \"{post.Title}\" (ID: {postId}) blev slettet.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Fejl ved sletning af blogindlæg: {ex.Message}");
                    }
                }
                else
                {
                    Console.WriteLine("Sletning annulleret.");
                }
            }
            Console.WriteLine("Tryk på en tast for at fortsætte...");
            Console.ReadKey();
        }

        private async Task PublishBlogPostAsync()
        {
            Console.Clear();
            int postId = GetIntFromUserInput("Indtast ID på blogindlægget der skal publiceres: ");
            var post = await _blogPostService.GetBlogPostByIdAsync(postId);

            if (post == null)
            {
                Console.WriteLine($"Blogindlæg med ID {postId} ikke fundet.");
            }
            else if (post.IsPublished)
            {
                Console.WriteLine($"Blogindlæg \"{post.Title}\" er allerede publiceret ({post.PublishDate:yyyy-MM-dd}).");
            }
            else
            {
                try
                {
                    await _blogPostService.PublishBlogPostAsync(postId);
                    // BlogPost.PublishDate sættes af servicen, så vi henter den opdaterede post for at vise korrekt dato.
                    var updatedPost = await _blogPostService.GetBlogPostByIdAsync(postId);
                    Console.WriteLine($"Blogindlæg \"{updatedPost?.Title}\" blev publiceret. Publiceringsdato: {updatedPost?.PublishDate:yyyy-MM-dd}.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Fejl ved publicering af blogindlæg: {ex.Message}");
                }
            }
            Console.WriteLine("Tryk på en tast for at fortsætte...");
            Console.ReadKey();
        }

        private async Task UnpublishBlogPostAsync()
        {
            Console.Clear();
            int postId = GetIntFromUserInput("Indtast ID på blogindlægget der skal afpubliceres: ");
            var post = await _blogPostService.GetBlogPostByIdAsync(postId);

            if (post == null)
            {
                Console.WriteLine($"Blogindlæg med ID {postId} ikke fundet.");
            }
            else if (!post.IsPublished)
            {
                Console.WriteLine($"Blogindlæg \"{post.Title}\" er ikke publiceret.");
            }
            else
            {
                try
                {
                    await _blogPostService.UnpublishBlogPostAsync(postId);
                    Console.WriteLine($"Blogindlæg \"{post.Title}\" blev afpubliceret.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Fejl ved afpublicering af blogindlæg: {ex.Message}");
                }
            }
            Console.WriteLine("Tryk på en tast for at fortsætte...");
            Console.ReadKey();
        }
        
        private async Task AddTagToBlogPostAsync()
        {
            Console.Clear();
            int postId = GetIntFromUserInput("Indtast ID på blogindlægget der skal have tilføjet et tag: ");
            var post = await _blogPostService.GetBlogPostByIdAsync(postId);

            if (post == null)
            {
                Console.WriteLine($"Blogindlæg med ID {postId} ikke fundet.");
            }
            else
            {
                Console.Write($"Indtast tag der skal tilføjes til \"{post.Title}\": ");
                string? tag = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(tag))
                {
                    Console.WriteLine("Tag må ikke være tomt.");
                }
                else
                {
                    try
                    {
                        await _blogPostService.AddTagToBlogPostAsync(postId, tag);
                        Console.WriteLine($"Tag '{tag}' tilføjet til blogindlæg \"{post.Title}\".");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Fejl ved tilføjelse af tag: {ex.Message}");
                    }
                }
            }
            Console.WriteLine("Tryk på en tast for at fortsætte...");
            Console.ReadKey();
        }

        private async Task RemoveTagFromBlogPostAsync()
        {
            Console.Clear();
            int postId = GetIntFromUserInput("Indtast ID på blogindlægget hvorfra et tag skal fjernes: ");
            var post = await _blogPostService.GetBlogPostByIdAsync(postId);

            if (post == null)
            {
                Console.WriteLine($"Blogindlæg med ID {postId} ikke fundet.");
            }
            else if (!post.Tags.Any())
            {
                 Console.WriteLine($"Blogindlæg \"{post.Title}\" har ingen tags.");
            }
            else
            {
                Console.WriteLine($"Nuværende tags for \"{post.Title}\": {string.Join(", ", post.Tags)}");
                Console.Write("Indtast tag der skal fjernes: ");
                string? tag = Console.ReadLine();
                 if (string.IsNullOrWhiteSpace(tag))
                {
                    Console.WriteLine("Tag må ikke være tomt.");
                }
                else
                {
                    try
                    {
                        await _blogPostService.RemoveTagFromBlogPostAsync(postId, tag);
                        Console.WriteLine($"Tag '{tag}' fjernet fra blogindlæg \"{post.Title}\".");
                    }
                    catch (Exception ex)
                    {
                        // Catch specifikt hvis tag ikke findes, eller lad service håndtere det.
                        Console.WriteLine($"Fejl ved fjernelse af tag: {ex.Message}");
                    }
                }
            }
            Console.WriteLine("Tryk på en tast for at fortsætte...");
            Console.ReadKey();
        }

        private async Task LikeBlogPostAsync()
        {
            Console.Clear();
            int postId = GetIntFromUserInput("Indtast ID på blogindlægget der skal likes: ");
            var post = await _blogPostService.GetBlogPostByIdAsync(postId);

            if (post == null)
            {
                Console.WriteLine($"Blogindlæg med ID {postId} ikke fundet.");
            }
            else
            {
                try
                {
                    await _blogPostService.LikeBlogPostAsync(postId);
                    var updatedPost = await _blogPostService.GetBlogPostByIdAsync(postId); // Hent for at få opdateret like-antal
                    Console.WriteLine($"Blogindlæg \"{updatedPost?.Title}\" har nu {updatedPost?.Likes} likes.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Fejl ved at like blogindlæg: {ex.Message}");
                }
            }
            Console.WriteLine("Tryk på en tast for at fortsætte...");
            Console.ReadKey();
        }
        
        private async Task UnlikeBlogPostAsync()
        {
            Console.Clear();
            int postId = GetIntFromUserInput("Indtast ID på blogindlægget der skal unlikes: ");
            var post = await _blogPostService.GetBlogPostByIdAsync(postId);

            if (post == null)
            {
                Console.WriteLine($"Blogindlæg med ID {postId} ikke fundet.");
            }
             else if (post.Likes == 0)
            {
                Console.WriteLine($"Blogindlæg \"{post.Title}\" har ingen likes at fjerne.");
            }
            else
            {
                try
                {
                    await _blogPostService.UnlikeBlogPostAsync(postId);
                     var updatedPost = await _blogPostService.GetBlogPostByIdAsync(postId); // Hent for at få opdateret like-antal
                    Console.WriteLine($"Like fjernet fra blogindlæg \"{updatedPost?.Title}\". Antal likes: {updatedPost?.Likes}.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Fejl ved at unlike blogindlæg: {ex.Message}");
                }
            }
            Console.WriteLine("Tryk på en tast for at fortsætte...");
            Console.ReadKey();
        }

        private async Task ListPublishedBlogPostsAsync()
        {
            Console.Clear();
            Console.WriteLine("Publicerede blogindlæg:");
            var publishedPosts = await _blogPostService.GetPublishedPostsAsync();
            await ListAllBlogPostsAsync(publishedPosts); // Genbrug visningslogik
        }

        private async Task SearchBlogPostsAsync()
        {
            Console.Clear();
            Console.Write("Indtast søgeterm (søger i titel, indhold, kategori, forfatter, tags): ");
            string? searchTerm = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                Console.WriteLine("Søgeterm må ikke være tom.");
                Console.ReadKey();
                return;
            }

            var searchResults = await _blogPostService.SearchBlogPostsAsync(searchTerm);
            Console.WriteLine($"Søgeresultater for '{searchTerm}':");
            await ListAllBlogPostsAsync(searchResults); // Genbrug visningslogik
        }

        private async Task ListBlogPostsByAuthorAsync()
        {
            Console.Clear();
            int authorId = GetIntFromUserInput("Indtast forfatter ID for at se deres blogindlæg: ");
            
            // Potentielt her: Kald IEmployeeService for at validere AuthorId / hente navn
            // For nu stoler vi på, at inputtet er et validt ID kendt af BlogService.
            
            var postsByAuthor = await _blogPostService.GetBlogPostsByAuthorAsync(authorId);
            Console.WriteLine($"Blogindlæg af forfatter med ID {authorId}:");
            // Kunne hente forfatternavn hvis vi havde EmployeeService, eller hvis BlogService returnerede det.
            // Lige nu viser ListAllBlogPostsAsync det Author-navn, der er gemt på selve BlogPost.
            await ListAllBlogPostsAsync(postsByAuthor);
        }

        private async Task ListBlogPostsByDateRangeAsync()
        {
            Console.Clear();
            Console.WriteLine("Vis blogindlæg efter datointerval");
            DateTime startDate = GetDateFromUserInput("Indtast startdato (YYYY-MM-DD): ");
            DateTime endDate = GetDateFromUserInput("Indtast slutdato (YYYY-MM-DD): ");

            if (startDate > endDate)
            {
                Console.WriteLine("Startdato kan ikke være efter slutdato.");
                Console.ReadKey();
                return;
            }

            var posts = await _blogPostService.GetBlogPostsByDateRangeAsync(startDate, endDate.AddDays(1)); // AddDays(1) to include the whole end day
            Console.WriteLine($"Blogindlæg publiceret mellem {startDate:yyyy-MM-dd} og {endDate:yyyy-MM-dd}:");
            await ListAllBlogPostsAsync(posts);
        }

        private async Task ListBlogPostsByTagsAsync()
        {
            Console.Clear();
            Console.WriteLine("Vis blogindlæg efter tags");
            Console.Write("Indtast tags (kommasepareret): ");
            string? tags = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(tags))
            {
                Console.WriteLine("Tags må ikke være tomme.");
                Console.ReadKey();
                return;
            }

            var posts = await _blogPostService.GetBlogPostsByTagsAsync(tags);
            Console.WriteLine($"Blogindlæg med tags: {tags}:");
            await ListAllBlogPostsAsync(posts);
        }

        private async Task ListLatestBlogPostsAsync()
        {
            Console.Clear();
            Console.WriteLine("Vis seneste blogindlæg");
            int count = GetIntFromUserInput("Indtast antal seneste blogindlæg der skal vises: ");

            if (count <= 0)
            {
                Console.WriteLine("Antal skal være et positivt heltal.");
                Console.ReadKey();
                return;
            }

            var posts = await _blogPostService.GetLatestBlogPostsAsync(count);
            Console.WriteLine($"De {count} seneste blogindlæg:");
            await ListAllBlogPostsAsync(posts);
        }

        // Hjælpemetoder
        private int GetIntFromUserInput(string prompt)
        {
            Console.Write(prompt);
            string? input = Console.ReadLine();
            int value;
            while (!int.TryParse(input, out value))
            {
                Console.WriteLine("Ugyldigt heltal. Prøv igen.");
                Console.Write(prompt);
                input = Console.ReadLine();
            }
            return value;
        }
        
        private DateTime GetDateFromUserInput(string prompt)
        {
            Console.Write(prompt);
            string? input = Console.ReadLine();
            DateTime date;
            while (!DateTime.TryParseExact(input, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
            {
                Console.WriteLine("Ugyldigt datoformat. Brug venligst YYYY-MM-DD. Prøv igen.");
                Console.Write(prompt);
                input = Console.ReadLine();
            }
            return date;
        }
    }
} 