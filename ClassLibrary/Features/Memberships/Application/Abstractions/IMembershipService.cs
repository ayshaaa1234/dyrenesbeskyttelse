using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClassLibrary.Features.Memberships.Core.Models;
using ClassLibrary.Features.Memberships.Core.Enums;

namespace ClassLibrary.Features.Memberships.Application.Abstractions
{
    public interface IMembershipService
    {
        // Membership Product operations
        Task<IEnumerable<MembershipProduct>> GetAllMembershipProductsAsync();
        Task<MembershipProduct?> GetMembershipProductByIdAsync(int productId);
        Task<IEnumerable<MembershipProduct>> GetAvailableMembershipProductsAsync();
        Task<MembershipProduct> CreateMembershipProductAsync(MembershipProduct product);
        Task<MembershipProduct> UpdateMembershipProductAsync(MembershipProduct product);
        Task DeleteMembershipProductAsync(int productId); // Soft delete

        // Customer Membership operations
        Task<CustomerMembership?> GetCustomerMembershipByIdAsync(int customerMembershipId);
        Task<IEnumerable<CustomerMembership>> GetMembershipsForCustomerAsync(int customerId);
        Task<IEnumerable<CustomerMembership>> GetActiveMembershipsForCustomerAsync(int customerId);
        Task<IEnumerable<CustomerMembership>> GetMembershipsByProductAsync(int productId);
        Task<CustomerMembership> AddMembershipToCustomerAsync(int customerId, int productId, decimal? actualDonationAmount, DateTime startDate, DateTime? endDate, PaymentMethodType paymentMethod, bool wantsPrintedMagazine, bool wantsDigitalMagazine, bool subscribedToNewsletter, bool optInForTaxDeduction);
        Task<CustomerMembership> UpdateCustomerMembershipAsync(CustomerMembership customerMembership);
        Task CancelCustomerMembershipAsync(int customerMembershipId); // Sætter IsActive=false, evt. EndDate
        Task RenewCustomerMembershipAsync(int customerMembershipId, DateTime newEndDate, decimal? newActualDonationAmount); // Fornyer et eksisterende
        Task RecordPaymentAsync(int customerMembershipId, DateTime paymentDate, decimal amountPaid);

        // Rapportering/opslag
        Task<IEnumerable<CustomerMembership>> GetMembershipsExpiringSoonAsync(DateTime endDateThreshold);
        Task<IEnumerable<CustomerMembership>> GetMembershipsByPaymentStatusAsync(bool paymentOverdue); // Kræver logik for at bestemme payment status
    }
} 