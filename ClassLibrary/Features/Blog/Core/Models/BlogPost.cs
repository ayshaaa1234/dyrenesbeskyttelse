using System;
using System.Collections.Generic;
using System.Linq;
using ClassLibrary.SharedKernel.Domain.Abstractions; // Opdateret

namespace ClassLibrary.Features.Blog.Core.Models
{
    /// <summary>
    /// Repræsenterer et blogindlæg i dyrenes beskyttelse
    /// </summary>
    public class BlogPost : IEntity, ISoftDelete // Disse interfaces kommer fra SharedKernel
    {
        /// <summary>
        /// Unikt ID for blogindlægget
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Titel på blogindlægget
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Indholdet af blogindlægget
        /// </summary>
        public string Content { get; set; } = string.Empty;

        /// <summary>
        /// URL til det tilhørende billede
        /// </summary>
        public string? PictureUrl { get; set; }

        /// <summary>
        /// ID for forfatteren (medarbejder)
        /// </summary>
        public int AuthorId { get; set; }

        /// <summary>
        /// Navn på forfatteren
        /// </summary>
        public string Author { get; set; } = string.Empty;

        /// <summary>
        /// Dato for publicering
        /// </summary>
        public DateTime PublishDate { get; set; }

        /// <summary>
        /// Dato for hvornår blogindlægget blev oprettet
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Angiver om indlægget er publiceret
        /// </summary>
        public bool IsPublished { get; set; }

        /// <summary>
        /// Kategori for indlægget
        /// </summary>
        public string Category { get; set; } = string.Empty;

        /// <summary>
        /// Tags tilknyttet indlægget
        /// </summary>
        public List<string> Tags { get; set; } = new List<string>();

        /// <summary>
        /// Kort beskrivelse af indlægget
        /// </summary>
        public string Summary { get; set; } = string.Empty;

        /// <summary>
        /// Antal likes på indlægget
        /// </summary>
        public int Likes { get; set; }

        /// <summary>
        /// Angiver om blogindlægget er slettet
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// Dato for hvornår blogindlægget blev slettet
        /// </summary>
        public DateTime? DeletedAt { get; set; }

        /// <summary>
        /// Konstruktør
        /// </summary>
        public BlogPost()
        {
            PublishDate = DateTime.Now;
            CreatedAt = DateTime.Now;
            IsPublished = false;
            Likes = 0;
            Tags = new List<string>();
        }

        /// <summary>
        /// Tjekker om indlægget har et bestemt tag
        /// </summary>
        public bool HasTag(string tag)
        {
            if (string.IsNullOrWhiteSpace(tag))
                return false;

            return Tags.Contains(tag, StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Tilføjer et tag til indlægget
        /// </summary>
        public void AddTag(string tag)
        {
            if (string.IsNullOrWhiteSpace(tag))
                return;

            if (!Tags.Contains(tag, StringComparer.OrdinalIgnoreCase))
            {
                Tags.Add(tag.Trim());
            }
        }

        /// <summary>
        /// Fjerner et tag fra indlægget
        /// </summary>
        public void RemoveTag(string tag)
        {
            if (string.IsNullOrWhiteSpace(tag))
                return;

            Tags.RemoveAll(t => t.Equals(tag, StringComparison.OrdinalIgnoreCase));
        }
    }
} 