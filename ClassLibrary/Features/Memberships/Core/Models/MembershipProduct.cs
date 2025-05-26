using ClassLibrary.SharedKernel.Domain.Abstractions; // For IEntity, ISoftDelete
using ClassLibrary.Features.Memberships.Core.Enums; // For BillingFrequency

namespace ClassLibrary.Features.Memberships.Core.Models // Opdateret namespace
{
    public class MembershipProduct : IEntity, ISoftDelete // Tilføjet ISoftDelete
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty; // F.eks. "Årsmedlemskab", "Månedlig Donation 50 kr"
        public string? Description { get; set; }
        public decimal Price { get; set; } // Prisen hvis fastsat, ellers 0 hvis "Andet beløb"
        public BillingFrequency Frequency { get; set; } // Bruger nu den flyttede enum
        public bool IsDonation { get; set; } // True hvis det primært er en donation, false hvis det er et "produkt"-medlemskab
        public bool AllowsCustomAmount { get; set; } // True for "Andet beløb" donationer
        public bool IsAvailable { get; set; } // Kan produktet vælges lige nu?

        // Fra ISoftDelete
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }

        // Valgfrit: specifikke fordele ved dette produkt
        // public bool IncludesPrintedMagazineOffer { get; set; }
        // public bool IncludesDigitalMagazineOffer { get; set; }

        public MembershipProduct()
        {
            IsAvailable = true;
            IsDeleted = false; // Default værdi
        }
    }
} 