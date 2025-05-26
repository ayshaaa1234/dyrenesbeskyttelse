using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClassLibrary.Features.Memberships.Core.Models;
using ClassLibrary.Features.Memberships.Core.Enums;

namespace ClassLibrary.Features.Memberships.Application.Abstractions
{
    /// <summary>
    /// Interface for service til håndtering af medlemskabsprodukter og kundemedlemskaber.
    /// </summary>
    public interface IMembershipService
    {
        // Membership Product operations
        /// <summary>
        /// Henter alle medlemskabsprodukter.
        /// </summary>
        /// <returns>En opgave, der repræsenterer den asynkrone operation. Opgavens resultat indeholder en samling af alle medlemskabsprodukter.</returns>
        Task<IEnumerable<MembershipProduct>> GetAllMembershipProductsAsync();
        
        /// <summary>
        /// Henter et specifikt medlemskabsprodukt baseret på ID.
        /// </summary>
        /// <param name="productId">ID på medlemskabsproduktet.</param>
        /// <returns>En opgave, der repræsenterer den asynkrone operation. Opgavens resultat indeholder det fundne medlemskabsprodukt eller null, hvis det ikke findes.</returns>
        Task<MembershipProduct?> GetMembershipProductByIdAsync(int productId);
        
        /// <summary>
        /// Henter alle tilgængelige (ikke slettede/udgåede) medlemskabsprodukter.
        /// </summary>
        /// <returns>En opgave, der repræsenterer den asynkrone operation. Opgavens resultat indeholder en samling af tilgængelige medlemskabsprodukter.</returns>
        Task<IEnumerable<MembershipProduct>> GetAvailableMembershipProductsAsync();
        
        /// <summary>
        /// Opretter et nyt medlemskabsprodukt.
        /// </summary>
        /// <param name="product">Medlemskabsproduktet der skal oprettes.</param>
        /// <returns>En opgave, der repræsenterer den asynkrone operation. Opgavens resultat indeholder det oprettede medlemskabsprodukt.</returns>
        Task<MembershipProduct> CreateMembershipProductAsync(MembershipProduct product);
        
        /// <summary>
        /// Opdaterer et eksisterende medlemskabsprodukt.
        /// </summary>
        /// <param name="product">Medlemskabsproduktet med de opdaterede værdier.</param>
        /// <returns>En opgave, der repræsenterer den asynkrone operation. Opgavens resultat indeholder det opdaterede medlemskabsprodukt.</returns>
        Task<MembershipProduct> UpdateMembershipProductAsync(MembershipProduct product);
        
        /// <summary>
        /// Sletter et medlemskabsprodukt (soft delete).
        /// </summary>
        /// <param name="productId">ID på medlemskabsproduktet der skal slettes.</param>
        /// <returns>En opgave, der repræsenterer den asynkrone operation.</returns>
        Task DeleteMembershipProductAsync(int productId); // Soft delete

        // Customer Membership operations
        /// <summary>
        /// Henter et specifikt kundemedlemskab baseret på ID.
        /// </summary>
        /// <param name="customerMembershipId">ID på kundemedlemskabet.</param>
        /// <returns>En opgave, der repræsenterer den asynkrone operation. Opgavens resultat indeholder det fundne kundemedlemskab eller null, hvis det ikke findes.</returns>
        Task<CustomerMembership?> GetCustomerMembershipByIdAsync(int customerMembershipId);
        
        /// <summary>
        /// Henter alle medlemskaber for en specifik kunde.
        /// </summary>
        /// <param name="customerId">ID på kunden.</param>
        /// <returns>En opgave, der repræsenterer den asynkrone operation. Opgavens resultat indeholder en samling af kundens medlemskaber.</returns>
        Task<IEnumerable<CustomerMembership>> GetMembershipsForCustomerAsync(int customerId);
        
        /// <summary>
        /// Henter alle aktive medlemskaber for en specifik kunde.
        /// </summary>
        /// <param name="customerId">ID på kunden.</param>
        /// <returns>En opgave, der repræsenterer den asynkrone operation. Opgavens resultat indeholder en samling af kundens aktive medlemskaber.</returns>
        Task<IEnumerable<CustomerMembership>> GetActiveMembershipsForCustomerAsync(int customerId);
        
        /// <summary>
        /// Henter alle kundemedlemskaber tilknyttet et specifikt medlemskabsprodukt.
        /// </summary>
        /// <param name="productId">ID på medlemskabsproduktet.</param>
        /// <returns>En opgave, der repræsenterer den asynkrone operation. Opgavens resultat indeholder en samling af kundemedlemskaber for det angivne produkt.</returns>
        Task<IEnumerable<CustomerMembership>> GetMembershipsByProductAsync(int productId);
        
        /// <summary>
        /// Tilføjer et nyt medlemskab til en kunde.
        /// </summary>
        /// <param name="customerId">ID på kunden.</param>
        /// <param name="productId">ID på medlemskabsproduktet.</param>
        /// <param name="actualDonationAmount">Det faktiske donationsbeløb, hvis forskelligt fra produktets pris.</param>
        /// <param name="startDate">Startdato for medlemskabet.</param>
        /// <param name="endDate">Slutdato for medlemskabet (valgfri, kan beregnes baseret på produkt).</param>
        /// <param name="paymentMethod">Betalingsmetode anvendt.</param>
        /// <param name="wantsPrintedMagazine">Angiver om kunden ønsker trykt magasin.</param>
        /// <param name="wantsDigitalMagazine">Angiver om kunden ønsker digitalt magasin.</param>
        /// <param name="subscribedToNewsletter">Angiver om kunden er tilmeldt nyhedsbrevet.</param>
        /// <param name="optInForTaxDeduction">Angiver om kunden ønsker skattefradrag.</param>
        /// <returns>En opgave, der repræsenterer den asynkrone operation. Opgavens resultat indeholder det oprettede kundemedlemskab.</returns>
        Task<CustomerMembership> AddMembershipToCustomerAsync(int customerId, int productId, decimal? actualDonationAmount, DateTime startDate, DateTime? endDate, PaymentMethodType paymentMethod, bool wantsPrintedMagazine, bool wantsDigitalMagazine, bool subscribedToNewsletter, bool optInForTaxDeduction);
        
        /// <summary>
        /// Opdaterer et eksisterende kundemedlemskab.
        /// </summary>
        /// <param name="customerMembership">Kundemedlemskabet med de opdaterede værdier.</param>
        /// <returns>En opgave, der repræsenterer den asynkrone operation. Opgavens resultat indeholder det opdaterede kundemedlemskab.</returns>
        Task<CustomerMembership> UpdateCustomerMembershipAsync(CustomerMembership customerMembership);
        
        /// <summary>
        /// Annullerer et kundemedlemskab (sætter IsActive=false og evt. EndDate).
        /// </summary>
        /// <param name="customerMembershipId">ID på kundemedlemskabet der skal annulleres.</param>
        /// <returns>En opgave, der repræsenterer den asynkrone operation.</returns>
        Task CancelCustomerMembershipAsync(int customerMembershipId); // Sætter IsActive=false, evt. EndDate
        
        /// <summary>
        /// Fornyer et eksisterende kundemedlemskab.
        /// </summary>
        /// <param name="customerMembershipId">ID på kundemedlemskabet der skal fornyes.</param>
        /// <param name="newEndDate">Den nye slutdato for medlemskabet.</param>
        /// <param name="newActualDonationAmount">Det nye faktiske donationsbeløb (valgfri).</param>
        /// <returns>En opgave, der repræsenterer den asynkrone operation. Opgavens resultat indeholder det fornyede kundemedlemskab.</returns>
        Task RenewCustomerMembershipAsync(int customerMembershipId, DateTime newEndDate, decimal? newActualDonationAmount); // Fornyer et eksisterende
        
        /// <summary>
        /// Registrerer en betaling for et kundemedlemskab.
        /// </summary>
        /// <param name="customerMembershipId">ID på kundemedlemskabet.</param>
        /// <param name="paymentDate">Dato for betalingen.</param>
        /// <param name="amountPaid">Betalt beløb.</param>
        /// <returns>En opgave, der repræsenterer den asynkrone operation.</returns>
        Task RecordPaymentAsync(int customerMembershipId, DateTime paymentDate, decimal amountPaid);

        // Rapportering/opslag
        /// <summary>
        /// Henter kundemedlemskaber der snart udløber.
        /// </summary>
        /// <param name="endDateThreshold">Tærskeldato for hvornår et medlemskab betragtes som snart udløbende.</param>
        /// <returns>En opgave, der repræsenterer den asynkrone operation. Opgavens resultat indeholder en samling af kundemedlemskaber, der snart udløber.</returns>
        Task<IEnumerable<CustomerMembership>> GetMembershipsExpiringSoonAsync(DateTime endDateThreshold);
        
        /// <summary>
        /// Henter kundemedlemskaber baseret på betalingsstatus (f.eks. forfaldne betalinger).
        /// </summary>
        /// <param name="paymentOverdue">Angiver om der skal søges efter medlemskaber med forfalden betaling (true) eller ikke-forfalden betaling (false).</param>
        /// <returns>En opgave, der repræsenterer den asynkrone operation. Opgavens resultat indeholder en samling af kundemedlemskaber, der matcher den angivne betalingsstatus.</returns>
        Task<IEnumerable<CustomerMembership>> GetMembershipsByPaymentStatusAsync(bool paymentOverdue); // Kræver logik for at bestemme payment status
    }
} 