using System;
using System.Collections.Generic;
using ClassLibrary.Interfaces;

namespace ClassLibrary.Models
{
    /// <summary>
    /// Repræsenterer en kunde i systemet
    /// </summary>
    public class Customer : IEntity, ISoftDelete
    {
        /// <summary>
        /// Unikt ID for kunden
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Kundens fornavn
        /// </summary>
        public string FirstName { get; set; } = string.Empty;

        /// <summary>
        /// Kundens efternavn
        /// </summary>
        public string LastName { get; set; } = string.Empty;

        /// <summary>
        /// Kundens fulde navn
        /// </summary>
        public string Name => $"{FirstName} {LastName}".Trim();

        /// <summary>
        /// Kundens email
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Kundens telefonnummer
        /// </summary>
        public string Phone { get; set; } = string.Empty;

        /// <summary>
        /// Kundens adresse
        /// </summary>
        public string Address { get; set; } = string.Empty;

        /// <summary>
        /// Kundens postnummer
        /// </summary>
        public string PostalCode { get; set; } = string.Empty;

        /// <summary>
        /// Kundens by
        /// </summary>
        public string City { get; set; } = string.Empty;

        /// <summary>
        /// Dato for hvornår kunden blev registreret
        /// </summary>
        public DateTime RegistrationDate { get; set; }

        /// <summary>
        /// Angiver om kunden er medlem
        /// </summary>
        public bool IsMember { get; set; }

        /// <summary>
        /// Kundens medlemskabstype
        /// </summary>
        public string MembershipType { get; set; } = string.Empty;

        /// <summary>
        /// Liste over kundens adoptioner
        /// </summary>
        public List<Adoption> Adoptions { get; set; }

        /// <summary>
        /// Liste over kundens besøg
        /// </summary>
        public List<VisitLog> VisitLogs { get; set; }

        /// <summary>
        /// Angiver om kunden er slettet
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// Dato for hvornår kunden blev slettet
        /// </summary>
        public DateTime? DeletedAt { get; set; }

        /// <summary>
        /// Konstruktør
        /// </summary>
        public Customer()
        {
            Adoptions = new List<Adoption>();
            VisitLogs = new List<VisitLog>();
            RegistrationDate = DateTime.Now;
        }
    }
} 