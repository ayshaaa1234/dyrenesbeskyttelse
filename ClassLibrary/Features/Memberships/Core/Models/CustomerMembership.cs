using System;
using ClassLibrary.SharedKernel.Domain.Abstractions; // For IEntity, ISoftDelete
using ClassLibrary.Features.Customers.Core.Models; // For Customer
using ClassLibrary.Features.Memberships.Core.Enums; // For PaymentMethodType
// using ClassLibrary.Features.Memberships.Core.Models; // For MembershipProduct - MembershipProduct er i samme namespace

namespace ClassLibrary.Features.Memberships.Core.Models // Opdateret namespace
{
    public class CustomerMembership : IEntity, ISoftDelete
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public virtual Customer? Customer { get; set; } // Korrekt Customer type

        public int MembershipProductId { get; set; }
        public virtual MembershipProduct? Product { get; set; } // Korrekt MembershipProduct type

        // Hvis MembershipProduct.AllowsCustomAmount er true, bruges dette felt.
        // Ellers er prisen implicit fra MembershipProduct.Price.
        public decimal? ActualDonationAmount { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; } // Null for løbende medlemskaber/donationer
        public bool IsActive { get; set; }

        // Præferencer knyttet til denne specifikke medlemskabstegning
        public bool WantsPrintedMagazine { get; set; }
        public bool WantsDigitalMagazine { get; set; }
        public bool SubscribedToNewsletter { get; set; } // Kan også være på BaseUser hvis generelt
        public bool OptInForTaxDeduction { get; set; }

        public PaymentMethodType PaymentMethod { get; set; } // Korrekt PaymentMethodType enum
        public DateTime? LastPaymentDate { get; set; }
        public DateTime? NextPaymentDate { get; set; }

        // Fra ISoftDelete
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }

        public CustomerMembership()
        {
            StartDate = DateTime.UtcNow; // Standardiseret til UtcNow
            IsActive = true;
            PaymentMethod = PaymentMethodType.NotSet;
        }
    }
} 