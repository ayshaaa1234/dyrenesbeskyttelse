using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClassLibrary.Features.Memberships.Core.Models;
using ClassLibrary.SharedKernel.Persistence.Abstractions;

namespace ClassLibrary.Features.Memberships.Infrastructure.Abstractions
{
    public interface ICustomerMembershipRepository : IRepository<CustomerMembership>
    {
        Task<IEnumerable<CustomerMembership>> GetMembershipsByCustomerIdAsync(int customerId);
        Task<IEnumerable<CustomerMembership>> GetMembershipsByProductIdAsync(int productId);
        Task<IEnumerable<CustomerMembership>> GetActiveMembershipsAsync();
        Task<IEnumerable<CustomerMembership>> GetMembershipsExpiringSoonAsync(DateTime endDateThreshold);
        Task<IEnumerable<CustomerMembership>> GetMembershipsByPaymentMethodAsync(Core.Enums.PaymentMethodType paymentMethod);
        Task<CustomerMembership?> GetActiveMembershipByCustomerIdAndProductIdAsync(int customerId, int productId);
    }
} 