using System;
using ClassLibrary.SharedKernel.Domain.Abstractions; // Opdateret for IEntity, ISoftDelete
using ClassLibrary.Features.AnimalManagement.Core.Models; // Tilføjet for Animal
using ClassLibrary.Features.Adoptions.Core.Enums;

namespace ClassLibrary.Features.Adoptions.Core.Models
{
    /// <summary>
    /// Repræsenterer en adoption i dyrenes beskyttelse
    /// </summary>
    public class Adoption : IEntity, ISoftDelete
    {
        /// <summary>
        /// Unikt ID for adoptionen
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// ID på det adopterede dyr
        /// </summary>
        public int AnimalId { get; set; }

        /// <summary>
        /// ID på kunden der adopterer
        /// </summary>
        public int CustomerId { get; set; }

        /// <summary>
        /// ID på medarbejderen der håndterer adoptionen
        /// </summary>
        public int? EmployeeId { get; set; }

        /// <summary>
        /// Dato for hvornår ansøgningen blev modtaget
        /// </summary>
        public DateTime ApplicationDate { get; set; }

        /// <summary>
        /// Dato for adoptionen
        /// </summary>
        public DateTime AdoptionDate { get; set; }

        /// <summary>
        /// Status for adoptionen
        /// </summary>
        public Enums.AdoptionStatus Status { get; set; } // Opdateret til at bruge Enums namespace

        /// <summary>
        /// Type af adoption
        /// </summary>
        public string AdoptionType { get; set; } = string.Empty;

        /// <summary>
        /// Det adopterede dyr
        /// </summary>
        public Animal? Animal { get; set; } // Nu korrekt type

        /// <summary>
        /// Noter tilknyttet adoptionen
        /// </summary>
        public string Notes { get; set; } = string.Empty;

        /// <summary>
        /// Dato for hvornår adoptionen blev godkendt
        /// </summary>
        public DateTime? ApprovalDate { get; set; }

        /// <summary>
        /// Dato for hvornår adoptionen blev afvist
        /// </summary>
        public DateTime? RejectionDate { get; set; }

        /// <summary>
        /// Dato for hvornår adoptionen blev gennemført
        /// </summary>
        public DateTime? CompletionDate { get; set; }

        /// <summary>
        /// Angiver om adoptionen er slettet
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// Dato for hvornår adoptionen blev slettet
        /// </summary>
        public DateTime? DeletedAt { get; set; }

        /// <summary>
        /// Konstruktør
        /// </summary>
        public Adoption()
        {
            ApplicationDate = DateTime.UtcNow; // Sæt ansøgningsdato ved oprettelse
            AdoptionDate = DateTime.Now;
            Status = Enums.AdoptionStatus.Pending; // Opdateret
        }
    }
} 