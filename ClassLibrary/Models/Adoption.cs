using System;
using System.Globalization;
using ClassLibrary.Interfaces;

namespace ClassLibrary.Models
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
        public int EmployeeId { get; set; }

        /// <summary>
        /// Navn på adoptanten
        /// </summary>
        public string AdopterName { get; set; } = string.Empty;

        /// <summary>
        /// Email på adoptanten
        /// </summary>
        public string AdopterEmail { get; set; } = string.Empty;

        /// <summary>
        /// Telefonnummer på adoptanten
        /// </summary>
        public string AdopterPhone { get; set; } = string.Empty;

        /// <summary>
        /// Dato for adoptionen
        /// </summary>
        public DateTime AdoptionDate { get; set; }

        /// <summary>
        /// Status for adoptionen
        /// </summary>
        public AdoptionStatus Status { get; set; }

        /// <summary>
        /// Type af adoption
        /// </summary>
        public string AdoptionType { get; set; } = string.Empty;

        /// <summary>
        /// Det adopterede dyr
        /// </summary>
        public Animal? Animal { get; set; }

        /// <summary>
        /// Noter tilknyttet adoptionen
        /// </summary>
        public string Notes { get; set; } = string.Empty;

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
            AdoptionDate = DateTime.Now;
            Status = AdoptionStatus.Pending;
        }
    }

    /// <summary>
    /// Repræsenterer status for en adoption
    /// </summary>
    public enum AdoptionStatus
    {
        /// <summary>
        /// Adoptionen er under behandling
        /// </summary>
        Pending,

        /// <summary>
        /// Adoptionen er godkendt
        /// </summary>
        Approved,

        /// <summary>
        /// Adoptionen er afvist
        /// </summary>
        Rejected,

        /// <summary>
        /// Adoptionen er gennemført
        /// </summary>
        Completed
    }
} 

