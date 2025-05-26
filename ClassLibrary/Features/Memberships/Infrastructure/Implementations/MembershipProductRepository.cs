using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClassLibrary.Features.Memberships.Core.Models;
using ClassLibrary.Features.Memberships.Core.Enums;
using ClassLibrary.Features.Memberships.Infrastructure.Abstractions;
using ClassLibrary.SharedKernel.Persistence.Implementations;

namespace ClassLibrary.Features.Memberships.Infrastructure.Implementations
{
    public class MembershipProductRepository : Repository<MembershipProduct>, IMembershipProductRepository
    {
        private const string FilePath = "Data/Json/membershipproducts.json";

        public MembershipProductRepository() : base(FilePath) { }

        public override async Task<MembershipProduct> AddAsync(MembershipProduct entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            ValidateEntity(entity);
            return await base.AddAsync(entity);
        }

        public override async Task<MembershipProduct> UpdateAsync(MembershipProduct entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            ValidateEntity(entity);
            return await base.UpdateAsync(entity);
        }

        public async Task<IEnumerable<MembershipProduct>> GetAvailableProductsAsync()
        {
            return await FindAsync(p => p.IsAvailable);
        }

        public async Task<IEnumerable<MembershipProduct>> GetProductsByFrequencyAsync(BillingFrequency frequency)
        {
            if (!Enum.IsDefined(typeof(BillingFrequency), frequency))
            {
                throw new ArgumentException("Ugyldig BillingFrequency værdi.", nameof(frequency));
            }
            return await FindAsync(p => p.Frequency == frequency);
        }

        public async Task<IEnumerable<MembershipProduct>> GetDonationProductsAsync()
        {
            return await FindAsync(p => p.IsDonation);
        }

        protected override void ValidateEntity(MembershipProduct entity)
        {
            base.ValidateEntity(entity);
            if (string.IsNullOrWhiteSpace(entity.Name))
                throw new ArgumentException("Produktnavn kan ikke være tomt.", nameof(entity.Name));
            if (entity.Price < 0)
                throw new ArgumentOutOfRangeException(nameof(entity.Price), "Prisen kan ikke være negativ.");
            if (string.IsNullOrWhiteSpace(entity.Description))
                throw new ArgumentException("Beskrivelse kan ikke være tom.", nameof(entity.Description));
            if (!Enum.IsDefined(typeof(BillingFrequency), entity.Frequency))
                throw new ArgumentException("Ugyldig BillingFrequency værdi.", nameof(entity.Frequency));
        }
    }
} 