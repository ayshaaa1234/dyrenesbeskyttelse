using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClassLibrary.Features.Memberships.Core.Models;
using ClassLibrary.Features.Memberships.Core.Enums;
using ClassLibrary.Features.Memberships.Infrastructure.Abstractions;
using ClassLibrary.SharedKernel.Persistence.Implementations;
using ClassLibrary.SharedKernel.Exceptions;
using ClassLibrary.Infrastructure.DataInitialization;

namespace ClassLibrary.Features.Memberships.Infrastructure.Implementations
{
    public class CustomerMembershipRepository : Repository<CustomerMembership>, ICustomerMembershipRepository
    {
        public CustomerMembershipRepository() : base(Path.Combine(JsonDataInitializer.CalculatedWorkspaceRoot, "Data", "Json", "customermemberships.json")) { }

        public override async Task<CustomerMembership> AddAsync(CustomerMembership entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            ValidateEntity(entity);

            var items = await LoadDataAsync();
            var existingActive = items.FirstOrDefault(cm =>
                cm.CustomerId == entity.CustomerId &&
                cm.MembershipProductId == entity.MembershipProductId &&
                cm.IsActive &&
                !cm.IsDeleted &&
                (cm.EndDate == null || cm.EndDate >= DateTime.UtcNow));

            if (existingActive != null)
            {
                throw new RepositoryException("Kunden har allerede et aktivt medlemskab af denne type.");
            }
            return await base.AddAsync(entity);
        }

        public override async Task<CustomerMembership> UpdateAsync(CustomerMembership entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            ValidateEntity(entity);
            return await base.UpdateAsync(entity);
        }

        public async Task<IEnumerable<CustomerMembership>> GetMembershipsByCustomerIdAsync(int customerId)
        {
            if (customerId <= 0) 
            {
                return Enumerable.Empty<CustomerMembership>();
            }
            return await FindAsync(cm => cm.CustomerId == customerId);
        }

        public async Task<IEnumerable<CustomerMembership>> GetMembershipsByProductIdAsync(int productId)
        {
            if (productId <= 0) 
            {
                return Enumerable.Empty<CustomerMembership>();
            }
            return await FindAsync(cm => cm.MembershipProductId == productId);
        }

        public async Task<IEnumerable<CustomerMembership>> GetActiveMembershipsAsync()
        {
            var utcNow = DateTime.UtcNow;
            return await FindAsync(cm => cm.IsActive && (cm.EndDate == null || cm.EndDate.Value.Date >= utcNow.Date));
        }

        public async Task<IEnumerable<CustomerMembership>> GetMembershipsExpiringSoonAsync(DateTime endDateThreshold)
        {
            var utcNow = DateTime.UtcNow;
            return await FindAsync(cm => 
                cm.IsActive && 
                cm.EndDate != null && 
                cm.EndDate.Value.Date < endDateThreshold.Date && 
                cm.EndDate.Value.Date >= utcNow.Date);
        }

        public async Task<IEnumerable<CustomerMembership>> GetMembershipsByPaymentMethodAsync(PaymentMethodType paymentMethod)
        {
            if (!Enum.IsDefined(typeof(PaymentMethodType), paymentMethod))
            {
                throw new ArgumentException("Ugyldig PaymentMethodType.", nameof(paymentMethod));
            }
            return await FindAsync(cm => cm.PaymentMethod == paymentMethod);
        }

        public async Task<CustomerMembership?> GetActiveMembershipByCustomerIdAndProductIdAsync(int customerId, int productId)
        {
            if (customerId <= 0 || productId <= 0)
            {
                 return null;
            }
            var utcNow = DateTime.UtcNow;
            var memberships = await FindAsync(cm => 
                cm.CustomerId == customerId && 
                cm.MembershipProductId == productId && 
                cm.IsActive && 
                (cm.EndDate == null || cm.EndDate.Value.Date >= utcNow.Date));
            return memberships.FirstOrDefault();
        }

        protected override void ValidateEntity(CustomerMembership entity)
        {
            base.ValidateEntity(entity);
            if (entity.CustomerId <= 0)
                throw new ArgumentOutOfRangeException(nameof(entity.CustomerId), "CustomerId skal være gyldigt (>0).");
            if (entity.MembershipProductId <= 0)
                throw new ArgumentOutOfRangeException(nameof(entity.MembershipProductId), "MembershipProductId skal være gyldigt (>0).");
            
            if (entity.StartDate == default(DateTime))
                throw new ArgumentException("Startdato skal være angivet.", nameof(entity.StartDate));
            
            if (entity.StartDate.Date > DateTime.UtcNow.Date.AddDays(1) && entity.IsActive)
                throw new ArgumentException("Startdato for et aktivt medlemskab kan ikke være langt ude i fremtiden.", nameof(entity.StartDate));

            if (entity.EndDate != null && entity.EndDate.Value.Date < entity.StartDate.Date)
                throw new ArgumentException("Slutdato kan ikke være før startdato.", nameof(entity.EndDate));
            
            if (entity.ActualDonationAmount.HasValue && entity.ActualDonationAmount < 0)
                throw new ArgumentOutOfRangeException(nameof(entity.ActualDonationAmount), "Donationsbeløb kan ikke være negativt.");

            if (!Enum.IsDefined(typeof(PaymentMethodType), entity.PaymentMethod))
                throw new ArgumentException("Ugyldig PaymentMethodType.", nameof(entity.PaymentMethod));
            
            if (entity.EndDate.HasValue && entity.EndDate.Value.Date < DateTime.UtcNow.Date && entity.IsActive)
            {
                 Console.WriteLine($"Advarsel: Medlemskab ID {entity.Id} er aktivt, men EndDate ({entity.EndDate.Value.ToShortDateString()}) er i fortiden.");
            }
            if (!entity.EndDate.HasValue && !entity.IsActive && entity.StartDate.Date <= DateTime.UtcNow.Date)
            {
                 Console.WriteLine($"Advarsel: Løbende medlemskab ID {entity.Id} er startet ({entity.StartDate.ToShortDateString()}) men er ikke aktivt.");
            }
        }
    }
} 