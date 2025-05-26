using System.ComponentModel.DataAnnotations; // Tilføjet for Display
using ClassLibrary.SharedKernel.Domain.Models; // For BaseUser
using ClassLibrary.Features.AnimalManagement.Core.Models; // For HealthRecord, Visit
using ClassLibrary.Features.Blog.Core.Models; // For BlogPost

namespace ClassLibrary.Features.Employees.Core.Models // Opdateret namespace
{
    /// <summary>
    /// Repræsenterer en medarbejder i systemet
    /// </summary>
    public class Employee : BaseUser 
    {
        /// <summary>
        /// Medarbejderens fulde navn (fornavn + efternavn)
        /// </summary>
        [Display(Name = "Fulde navn")]
        public override string FullName => $"{FirstName} {LastName}".Trim();

        /// <summary>
        /// Medarbejderens stilling
        /// </summary>
        [Display(Name = "Stilling")]
        public string Position { get; set; } = string.Empty;

        /// <summary>
        /// Medarbejderens afdeling
        /// </summary>
        [Display(Name = "Afdeling")]
        public string Department { get; set; } = string.Empty;

        /// <summary>
        /// Liste over medarbejderens specialiseringer
        /// </summary>
        [Display(Name = "Specialiseringer")]
        public List<string> Specializations { get; set; }

        /// <summary>
        /// Medarbejderens løn
        /// </summary>
        [Display(Name = "Løn")]
        public decimal Salary { get; set; }

        /// <summary>
        /// Dato for hvornår medarbejderen blev ansat
        /// (RegistrationDate fra BaseUser kan afspejle dette)
        /// </summary>
        [Display(Name = "Ansættelsesdato")]
        public DateTime HireDate { get; set; }

        /// <summary>
        /// Liste over sundhedsjournaler oprettet af medarbejderen
        /// </summary>
        public virtual ICollection<HealthRecord> CreatedHealthRecords { get; set; }

        /// <summary>
        /// Liste over besøg håndteret af medarbejderen
        /// </summary>
        public virtual ICollection<Visit> ManagedVisits { get; set; }

        /// <summary>
        /// Liste over blogindlæg skrevet af medarbejderen
        /// </summary>
        public virtual ICollection<BlogPost> BlogPosts { get; set; }

        /// <summary>
        /// URL til et billede af medarbejderen
        /// </summary>
        [Display(Name = "Billed-URL")]
        public string? PictureUrl { get; set; }

        /// <summary>
        /// Konstruktør
        /// </summary>
        public Employee() : base()
        {
            Specializations = new List<string>();
            CreatedHealthRecords = new List<HealthRecord>();
            ManagedVisits = new List<Visit>();
            BlogPosts = new List<BlogPost>();
            HireDate = DateTime.UtcNow; // Standardiser til UtcNow
            // RegistrationDate i BaseUser vil også blive sat til UtcNow som default.
            // En service kan evt. synkronisere RegistrationDate = HireDate ved oprettelse hvis ønsket.
        }
    }
} 