using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClassLibrary.Features.Memberships.Core.Models;
using ClassLibrary.SharedKernel.Persistence.Abstractions;
using ClassLibrary.Features.Memberships.Core.Enums;

namespace ClassLibrary.Features.Memberships.Infrastructure.Abstractions
{
    /// <summary>
    /// Interface for repository til håndtering af <see cref="CustomerMembership"/> entiteter.
    /// </summary>
    public interface ICustomerMembershipRepository : IRepository<CustomerMembership>
    {
        /// <summary>
        /// Henter alle medlemskaber for en specifik kunde.
        /// </summary>
        /// <param name="customerId">ID på kunden.</param>
        /// <returns>En opgave, der repræsenterer den asynkrone operation. Opgavens resultat indeholder en samling af kundens medlemskaber.</returns>
        Task<IEnumerable<CustomerMembership>> GetMembershipsByCustomerIdAsync(int customerId);

        /// <summary>
        /// Henter alle medlemskaber tilknyttet et specifikt medlemskabsprodukt.
        /// </summary>
        /// <param name="productId">ID på medlemskabsproduktet.</param>
        /// <returns>En opgave, der repræsenterer den asynkrone operation. Opgavens resultat indeholder en samling af medlemskaber for det angivne produkt.</returns>
        Task<IEnumerable<CustomerMembership>> GetMembershipsByProductIdAsync(int productId);

        /// <summary>
        /// Henter alle aktive medlemskaber.
        /// </summary>
        /// <returns>En opgave, der repræsenterer den asynkrone operation. Opgavens resultat indeholder en samling af alle aktive medlemskaber.</returns>
        Task<IEnumerable<CustomerMembership>> GetActiveMembershipsAsync();

        /// <summary>
        /// Henter medlemskaber der snart udløber, baseret på en given tærskeldato.
        /// </summary>
        /// <param name="endDateThreshold">Tærskeldatoen. Medlemskaber med slutdato før eller på denne dato inkluderes.</param>
        /// <returns>En opgave, der repræsenterer den asynkrone operation. Opgavens resultat indeholder en samling af medlemskaber, der snart udløber.</returns>
        Task<IEnumerable<CustomerMembership>> GetMembershipsExpiringSoonAsync(DateTime endDateThreshold);

        /// <summary>
        /// Henter medlemskaber baseret på den anvendte betalingsmetode.
        /// </summary>
        /// <param name="paymentMethod">Typen af betalingsmetode der søges efter.</param>
        /// <returns>En opgave, der repræsenterer den asynkrone operation. Opgavens resultat indeholder en samling af medlemskaber med den specificerede betalingsmetode.</returns>
        Task<IEnumerable<CustomerMembership>> GetMembershipsByPaymentMethodAsync(PaymentMethodType paymentMethod);

        /// <summary>
        /// Henter et specifikt aktivt medlemskab for en given kunde og et givent medlemskabsprodukt.
        /// </summary>
        /// <param name="customerId">ID på kunden.</param>
        /// <param name="productId">ID på medlemskabsproduktet.</param>
        /// <returns>En opgave, der repræsenterer den asynkrone operation. Opgavens resultat indeholder det fundne aktive kundemedlemskab eller null, hvis intet match findes.</returns>
        Task<CustomerMembership?> GetActiveMembershipByCustomerIdAndProductIdAsync(int customerId, int productId);
    }
} 