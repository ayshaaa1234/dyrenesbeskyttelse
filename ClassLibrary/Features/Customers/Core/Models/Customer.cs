using System;
using System.Collections.Generic;
using ClassLibrary.SharedKernel.Domain.Models; // For BaseUser
using ClassLibrary.Features.Adoptions.Core.Models; // For Adoption
using ClassLibrary.Features.AnimalManagement.Core.Models; // For Visit
using ClassLibrary.Features.Memberships.Core.Models; // For CustomerMembership
// using ClassLibrary.Features.Memberships.Core.Models; // For CustomerMembership - afventer afklaring

namespace ClassLibrary.Features.Customers.Core.Models // Opdateret namespace
{
    /// <summary>
    /// Repræsenterer en kunde i systemet
    /// </summary>
    public class Customer : BaseUser
    {
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
        /// Kundens medlemskaber
        /// Afventer afklaring af CustomerMembership placering
        /// </summary>
        public virtual ICollection<CustomerMembership> Memberships { get; set; } // Skal muligvis opdateres

        /// <summary>
        /// Liste over kundens adoptioner
        /// </summary>
        public virtual ICollection<Adoption> Adoptions { get; set; }

        /// <summary>
        /// Liste over kundens besøg
        /// </summary>
        public virtual ICollection<Visit> VisitsLog { get; set; }

        /// <summary>
        /// Konstruktør
        /// </summary>
        public Customer() : base()
        {
            Memberships = new List<CustomerMembership>(); // Skal muligvis opdateres
            Adoptions = new List<Adoption>();
            VisitsLog = new List<Visit>();
        }
    }
} 