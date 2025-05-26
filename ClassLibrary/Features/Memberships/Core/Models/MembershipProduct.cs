using ClassLibrary.SharedKernel.Domain.Abstractions; // For IEntity, ISoftDelete
using ClassLibrary.Features.Memberships.Core.Enums; // For BillingFrequency

namespace ClassLibrary.Features.Memberships.Core.Models // Opdateret namespace
{
    /// <summary>
    /// Repræsenterer et produkt eller en type af medlemskab/donation, som kunder kan tegne.
    /// </summary>
    public class MembershipProduct : IEntity, ISoftDelete // Tilføjet ISoftDelete
    {
        /// <summary>
        /// Unikt ID for medlemskabsproduktet.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Navnet på medlemskabsproduktet (f.eks. "Årsmedlemskab", "Månedlig Donation 50 kr").
        /// </summary>
        public string Name { get; set; } = string.Empty; // F.eks. "Årsmedlemskab", "Månedlig Donation 50 kr"
        /// <summary>
        /// En valgfri beskrivelse af medlemskabsproduktet.
        /// </summary>
        public string? Description { get; set; }
        /// <summary>
        /// Standardprisen for produktet. Hvis <see cref="AllowsCustomAmount"/> er true, kan dette være 0 eller et vejledende beløb.
        /// </summary>
        public decimal Price { get; set; } // Prisen hvis fastsat, ellers 0 hvis "Andet beløb"
        /// <summary>
        /// Fakturerings- eller fornyelsesfrekvensen for produktet.
        /// </summary>
        public BillingFrequency Frequency { get; set; } // Bruger nu den flyttede enum
        /// <summary>
        /// Angiver om dette produkt primært betragtes som en donation.
        /// </summary>
        public bool IsDonation { get; set; } // True hvis det primært er en donation, false hvis det er et "produkt"-medlemskab
        /// <summary>
        /// Angiver om kunden kan angive et brugerdefineret beløb for dette produkt (f.eks. "Andet beløb" donationer).
        /// </summary>
        public bool AllowsCustomAmount { get; set; } // True for "Andet beløb" donationer
        /// <summary>
        /// Angiver om dette medlemskabsprodukt er tilgængeligt for nye tegninger.
        /// </summary>
        public bool IsAvailable { get; set; } // Kan produktet vælges lige nu?

        // Fra ISoftDelete
        /// <summary>
        /// Angiver om produktet er soft-deleted.
        /// </summary>
        public bool IsDeleted { get; set; }
        /// <summary>
        /// Dato og tidspunkt for hvornår produktet blev soft-deleted. Null hvis ikke slettet.
        /// </summary>
        public DateTime? DeletedAt { get; set; }

        // Valgfrit: specifikke fordele ved dette produkt
        // public bool IncludesPrintedMagazineOffer { get; set; }
        // public bool IncludesDigitalMagazineOffer { get; set; }

        /// <summary>
        /// Initialiserer en ny instans af <see cref="MembershipProduct"/> klassen med standardværdier.
        /// IsAvailable sættes til true og IsDeleted sættes til false.
        /// </summary>
        public MembershipProduct()
        {
            IsAvailable = true;
            IsDeleted = false; // Default værdi
        }
    }
} 