using System;
using System.Collections.Generic;
using ClassLibrary.SharedKernel.Domain.Abstractions; // For IEntity, ISoftDelete
using ClassLibrary.Features.AnimalManagement.Core.Enums; // For AnimalStatus, Species (senere)
// using ClassLibrary.Features.AnimalManagement.Core.Models; // For HealthRecord, Visit (selvreference/andre modeller i samme feature)

namespace ClassLibrary.Features.AnimalManagement.Core.Models
{
    /// <summary>
    /// Repræsenterer et dyr i systemet
    /// </summary>
    public class Animal : IEntity, ISoftDelete
    {
        /// <summary>
        /// Unikt ID for dyret
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Dyrets navn
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Dyrets art
        /// </summary>
        public Enums.Species Species { get; set; } // Opdateret type

        /// <summary>
        /// Dyrets race
        /// </summary>
        public string Breed { get; set; } = string.Empty;

        /// <summary>
        /// Dyrets fødselsdato
        /// </summary>
        public DateTime? BirthDate { get; set; }

        /// <summary>
        /// Dyrets køn
        /// </summary>
        public string Gender { get; set; } = string.Empty;

        /// <summary>
        /// Beskrivelse af dyret
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Dato for hvornår dyret blev indbragt
        /// </summary>
        public DateTime IntakeDate { get; set; }

        /// <summary>
        /// Dyrets vægt i kg
        /// </summary>
        public decimal Weight { get; set; }

        /// <summary>
        /// Dyrets sundhedsstatus
        /// </summary>
        public string HealthStatus { get; set; } = string.Empty;

        /// <summary>
        /// Liste over dyrets sundhedsjournaler
        /// </summary>
        public virtual ICollection<HealthRecord> HealthRecords { get; set; } // Type vil blive løst

        /// <summary>
        /// Liste over dyrets besøg
        /// </summary>
        public virtual ICollection<Visit> Visits { get; set; } // Type vil blive løst

        /// <summary>
        /// Angiver om dyret er adopteret
        /// </summary>
        public bool IsAdopted { get; set; }

        /// <summary>
        /// Dato for hvornår dyret blev adopteret (hvis relevant)
        /// </summary>
        public DateTime? AdoptionDate { get; set; }

        /// <summary>
        /// ID på den kunde der har adopteret dyret (hvis relevant)
        /// </summary>
        public int? AdoptedByCustomerId { get; set; }

        /// <summary>
        /// Angiver om dyret er slettet
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// Dato for hvornår dyret blev slettet
        /// </summary>
        public DateTime? DeletedAt { get; set; }

        /// <summary>
        /// Dyrets status
        /// </summary>
        public Enums.AnimalStatus Status { get; set; } // Vil bruge Enums.AnimalStatus når enum flyttes

        /// <summary>
        /// URL til et billede af dyret
        /// </summary>
        public string? PictureUrl { get; set; }

        /// <summary>
        /// Konstruktør
        /// </summary>
        public Animal()
        {
            HealthRecords = new List<HealthRecord>();
            Visits = new List<Visit>();
            IntakeDate = DateTime.Now;
            IsDeleted = false;
            DeletedAt = null;
        }

        /// <summary>
        /// Beregner dyrets alder i år
        /// </summary>
        public int GetAgeInYears()
        {
            if (!BirthDate.HasValue)
                return 0;

            var today = DateTime.Today;
            var age = today.Year - BirthDate.Value.Year;
            
            if (BirthDate.Value.Date > today.AddYears(-age))
                age--;

            return age;
        }

        /// <summary>
        /// Beregner dyrets alder i måneder
        /// </summary>
        public int GetAgeInMonths()
        {
            if (!BirthDate.HasValue)
                return 0;

            var today = DateTime.Today;
            var months = (today.Year - BirthDate.Value.Year) * 12 + today.Month - BirthDate.Value.Month;
            
            if (BirthDate.Value.Day > today.Day)
                months--;

            return months;
        }

        /// <summary>
        /// Beregner dyrets alder i uger
        /// </summary>
        public int GetAgeInWeeks()
        {
            if (!BirthDate.HasValue)
                return 0;

            var today = DateTime.Today;
            var weeks = (int)((today - BirthDate.Value).TotalDays / 7);
            return weeks;
        }

        /// <summary>
        /// Beregner dyrets alder i dage
        /// </summary>
        public int GetAgeInDays()
        {
            if (!BirthDate.HasValue)
                return 0;

            var today = DateTime.Today;
            return (int)(today - BirthDate.Value).TotalDays;
        }

        /// <summary>
        /// Returnerer en formateret aldersstreng baseret på dyrets alder
        /// </summary>
        public string GetFormattedAge()
        {
            if (!BirthDate.HasValue)
                return "Ukendt alder";

            var days = GetAgeInDays();
            var weeks = GetAgeInWeeks();
            var months = GetAgeInMonths();
            var years = GetAgeInYears();

            if (days < 7)
                return $"{days} dage";
            if (weeks < 4)
                return $"{weeks} uger";
            if (months < 12)
                return $"{months} måneder";
            return $"{years} år";
        }
    }
} 