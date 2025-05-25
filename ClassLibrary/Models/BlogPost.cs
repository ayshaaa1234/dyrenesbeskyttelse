using System;
using System.Collections.Generic;
using System.Linq;
using ClassLibrary.Interfaces;

namespace ClassLibrary.Models
{
    /// <summary>
    /// Repræsenterer et blogindlæg i dyrenes beskyttelse
    /// </summary>
    public class BlogPost : IEntity, ISoftDelete
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
        public string? ImageUrl { get; set; }

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
        public string Tags { get; set; } = string.Empty;

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
            IsPublished = false;
            Likes = 0;
        }

        /// <summary>
        /// Tjekker om indlægget har et bestemt tag
        /// </summary>
        public bool HasTag(string tag)
        {
            if (string.IsNullOrWhiteSpace(tag))
                return false;

            return Tags.Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Any(t => t.Trim().Equals(tag, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Tilføjer et tag til indlægget
        /// </summary>
        public void AddTag(string tag)
        {
            if (string.IsNullOrWhiteSpace(tag))
                return;

            var tags = Tags.Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(t => t.Trim())
                .ToList();

            if (!tags.Contains(tag, StringComparer.OrdinalIgnoreCase))
            {
                tags.Add(tag);
                Tags = string.Join(",", tags);
            }
        }

        /// <summary>
        /// Fjerner et tag fra indlægget
        /// </summary>
        public void RemoveTag(string tag)
        {
            if (string.IsNullOrWhiteSpace(tag))
                return;

            var tags = Tags.Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(t => t.Trim())
                .Where(t => !t.Equals(tag, StringComparison.OrdinalIgnoreCase))
                .ToList();

            Tags = string.Join(",", tags);
        }
    }
} 