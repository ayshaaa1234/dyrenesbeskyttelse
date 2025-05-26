using System;
using ClassLibrary.SharedKernel.Domain.Abstractions; // For IEntity, ISoftDelete
using ClassLibrary.Features.Customers.Core.Models; // For Customer
using ClassLibrary.Features.Memberships.Core.Enums; // For PaymentMethodType
// using ClassLibrary.Features.Memberships.Core.Models; // For MembershipProduct - MembershipProduct er i samme namespace

namespace ClassLibrary.Features.Memberships.Core.Models // Opdateret namespace
{
    /// <summary>
    /// Repræsenterer et specifikt medlemskab eller en donation knyttet til en kunde.
    /// </summary>
    public class CustomerMembership : IEntity, ISoftDelete
    {
        /// <summary>
        /// Unikt ID for kundemedlemskabet.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// ID på kunden, som dette medlemskab tilhører.
        /// </summary>
        public int CustomerId { get; set; }
        /// <summary>
        /// Navigationsproperty til den tilknyttede kunde.
        /// </summary>
        public virtual Customer? Customer { get; set; } // Korrekt Customer type

        /// <summary>
        /// ID på det medlemskabsprodukt, dette medlemskab er baseret på.
        /// </summary>
        public int MembershipProductId { get; set; }
        /// <summary>
        /// Navigationsproperty til det tilknyttede medlemskabsprodukt.
        /// </summary>
        public virtual MembershipProduct? Product { get; set; } // Korrekt MembershipProduct type

        /// <summary>
        /// Det faktiske beløb betalt for dette medlemskab/donation, hvis det afviger fra produktets standardpris (f.eks. ved valgfrit donationsbeløb).
        /// </summary>
        public decimal? ActualDonationAmount { get; set; }

        /// <summary>
        /// Startdatoen for medlemskabet.
        /// </summary>
        public DateTime StartDate { get; set; }
        /// <summary>
        /// Slutdatoen for medlemskabet. Kan være null for løbende medlemskaber eller donationer uden udløb.
        /// </summary>
        public DateTime? EndDate { get; set; } // Null for løbende medlemskaber/donationer
        /// <summary>
        /// Angiver om medlemskabet er aktivt.
        /// </summary>
        public bool IsActive { get; set; }

        // Præferencer knyttet til denne specifikke medlemskabstegning
        /// <summary>
        /// Angiver om kunden ønsker at modtage det trykte magasin som en del af dette medlemskab.
        /// </summary>
        public bool WantsPrintedMagazine { get; set; }
        /// <summary>
        /// Angiver om kunden ønsker at modtage det digitale magasin som en del af dette medlemskab.
        /// </summary>
        public bool WantsDigitalMagazine { get; set; }
        /// <summary>
        /// Angiver om kunden er tilmeldt nyhedsbrevet i forbindelse med dette medlemskab.
        /// </summary>
        public bool SubscribedToNewsletter { get; set; } // Kan også være på BaseUser hvis generelt
        /// <summary>
        /// Angiver om kunden har valgt at få skattefradrag for dette medlemskab/donation.
        /// </summary>
        public bool OptInForTaxDeduction { get; set; }

        /// <summary>
        /// Den anvendte betalingsmetode for dette medlemskab.
        /// </summary>
        public PaymentMethodType PaymentMethod { get; set; } // Korrekt PaymentMethodType enum
        /// <summary>
        /// Datoen for den seneste registrerede betaling for dette medlemskab.
        /// </summary>
        public DateTime? LastPaymentDate { get; set; }
        /// <summary>
        /// Datoen for den næste forventede betaling for dette medlemskab.
        /// </summary>
        public DateTime? NextPaymentDate { get; set; }

        // Fra ISoftDelete
        /// <summary>
        /// Angiver om medlemskabet er soft-deleted.
        /// </summary>
        public bool IsDeleted { get; set; }
        /// <summary>
        /// Dato og tidspunkt for hvornår medlemskabet blev soft-deleted. Null hvis ikke slettet.
        /// </summary>
        public DateTime? DeletedAt { get; set; }

        /// <summary>
        /// Initialiserer en ny instans af <see cref="CustomerMembership"/> klassen med standardværdier.
        /// StartDate sættes til nuværende UTC-tid, IsActive sættes til true, og PaymentMethod sættes til NotSet.
        /// </summary>
        public CustomerMembership()
        {
            StartDate = DateTime.UtcNow; // Standardiseret til UtcNow
            IsActive = true;
            PaymentMethod = PaymentMethodType.NotSet;
        }
    }
} 