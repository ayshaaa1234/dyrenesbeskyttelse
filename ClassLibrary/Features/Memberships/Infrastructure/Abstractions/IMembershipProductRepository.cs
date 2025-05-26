using System.Collections.Generic;
using System.Threading.Tasks;
using ClassLibrary.Features.Memberships.Core.Models;
using ClassLibrary.SharedKernel.Persistence.Abstractions;

namespace ClassLibrary.Features.Memberships.Infrastructure.Abstractions
{
    public interface IMembershipProductRepository : IRepository<MembershipProduct>
    {
        Task<IEnumerable<MembershipProduct>> GetAvailableProductsAsync();
        Task<IEnumerable<MembershipProduct>> GetProductsByFrequencyAsync(Core.Enums.BillingFrequency frequency);
        Task<IEnumerable<MembershipProduct>> GetDonationProductsAsync(); // Produkter hvor IsDonation = true
    }
} 