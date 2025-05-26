using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClassLibrary.Features.Memberships.Core.Models;
using ClassLibrary.Features.Memberships.Core.Enums;
using ClassLibrary.Features.Memberships.Infrastructure.Abstractions;
using ClassLibrary.SharedKernel.Persistence.Implementations;
using ClassLibrary.Infrastructure.DataInitialization;

namespace ClassLibrary.Features.Memberships.Infrastructure.Implementations
{
    /// <summary>
    /// Repository til håndtering af <see cref="MembershipProduct"/> data, gemt i en JSON-fil.
    /// </summary>
    public class MembershipProductRepository : Repository<MembershipProduct>, IMembershipProductRepository
    {
        /// <summary>
        /// Initialiserer en ny instans af <see cref="MembershipProductRepository"/> klassen.
        /// Stien til JSON-filen bestemmes via <see cref="JsonDataInitializer"/>.
        /// </summary>
        public MembershipProductRepository() : base(Path.Combine(JsonDataInitializer.CalculatedWorkspaceRoot, "Data", "Json", "membershipproducts.json")) { }

        /// <summary>
        /// Tilføjer et nyt medlemskabsprodukt asynkront efter validering.
        /// </summary>
        /// <param name="entity">Medlemskabsproduktet der skal tilføjes.</param>
        /// <returns>Det tilføjede medlemskabsprodukt.</returns>
        /// <exception cref="ArgumentNullException">Kastes hvis <paramref name="entity"/> er null.</exception>
        public override async Task<MembershipProduct> AddAsync(MembershipProduct entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            ValidateEntity(entity);
            return await base.AddAsync(entity);
        }

        /// <summary>
        /// Opdaterer et eksisterende medlemskabsprodukt asynkront efter validering.
        /// </summary>
        /// <param name="entity">Medlemskabsproduktet med de opdaterede værdier.</param>
        /// <returns>Det opdaterede medlemskabsprodukt.</returns>
        /// <exception cref="ArgumentNullException">Kastes hvis <paramref name="entity"/> er null.</exception>
        public override async Task<MembershipProduct> UpdateAsync(MembershipProduct entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            ValidateEntity(entity);
            return await base.UpdateAsync(entity);
        }

        /// <summary>
        /// Henter alle tilgængelige (ikke slettede og <see cref="MembershipProduct.IsAvailable"/> er true) medlemskabsprodukter.
        /// </summary>
        /// <returns>En samling af tilgængelige medlemskabsprodukter.</returns>
        public async Task<IEnumerable<MembershipProduct>> GetAvailableProductsAsync()
        {
            return await FindAsync(p => p.IsAvailable);
        }

        /// <summary>
        /// Henter medlemskabsprodukter baseret på deres faktureringsfrekvens.
        /// </summary>
        /// <param name="frequency">Faktureringsfrekvensen der søges efter.</param>
        /// <returns>En samling af medlemskabsprodukter med den specificerede frekvens.</returns>
        /// <exception cref="ArgumentException">Kastes hvis <paramref name="frequency"/> er en ugyldig enum-værdi.</exception>
        public async Task<IEnumerable<MembershipProduct>> GetProductsByFrequencyAsync(BillingFrequency frequency)
        {
            if (!Enum.IsDefined(typeof(BillingFrequency), frequency))
            {
                throw new ArgumentException("Ugyldig BillingFrequency værdi.", nameof(frequency));
            }
            return await FindAsync(p => p.Frequency == frequency);
        }

        /// <summary>
        /// Henter alle medlemskabsprodukter, der er markeret som donationer (<see cref="MembershipProduct.IsDonation"/> er true).
        /// </summary>
        /// <returns>En samling af donationsprodukter.</returns>
        public async Task<IEnumerable<MembershipProduct>> GetDonationProductsAsync()
        {
            return await FindAsync(p => p.IsDonation);
        }

        /// <summary>
        /// Validerer en <see cref="MembershipProduct"/> entitet.
        /// </summary>
        /// <param name="entity">Entiteten der skal valideres.</param>
        /// <exception cref="ArgumentException">Kastes hvis Name eller Description er tom, eller hvis Frequency er en ugyldig enum-værdi.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Kastes hvis Price er negativ.</exception>
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