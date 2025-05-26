using System.Collections.Generic;
using System.Threading.Tasks;
using ClassLibrary.Features.Memberships.Core.Models;
using ClassLibrary.SharedKernel.Persistence.Abstractions;
using ClassLibrary.Features.Memberships.Core.Enums;

namespace ClassLibrary.Features.Memberships.Infrastructure.Abstractions
{
    /// <summary>
    /// Interface for repository til håndtering af <see cref="MembershipProduct"/> entiteter.
    /// </summary>
    public interface IMembershipProductRepository : IRepository<MembershipProduct>
    {
        /// <summary>
        /// Henter alle tilgængelige (ikke slettede/udgåede) medlemskabsprodukter.
        /// </summary>
        /// <returns>En opgave, der repræsenterer den asynkrone operation. Opgavens resultat indeholder en samling af tilgængelige medlemskabsprodukter.</returns>
        Task<IEnumerable<MembershipProduct>> GetAvailableProductsAsync();

        /// <summary>
        /// Henter medlemskabsprodukter baseret på deres faktureringsfrekvens.
        /// </summary>
        /// <param name="frequency">Faktureringsfrekvensen der søges efter.</param>
        /// <returns>En opgave, der repræsenterer den asynkrone operation. Opgavens resultat indeholder en samling af medlemskabsprodukter med den specificerede frekvens.</returns>
        Task<IEnumerable<MembershipProduct>> GetProductsByFrequencyAsync(BillingFrequency frequency);

        /// <summary>
        /// Henter alle medlemskabsprodukter, der er markeret som donationer (<see cref="MembershipProduct.IsDonation"/> er true).
        /// </summary>
        /// <returns>En opgave, der repræsenterer den asynkrone operation. Opgavens resultat indeholder en samling af donationsprodukter.</returns>
        Task<IEnumerable<MembershipProduct>> GetDonationProductsAsync(); // Produkter hvor IsDonation = true
    }
} 