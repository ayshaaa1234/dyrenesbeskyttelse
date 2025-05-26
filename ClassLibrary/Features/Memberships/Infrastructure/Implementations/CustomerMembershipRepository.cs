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
    /// <summary>
    /// Repository til håndtering af <see cref="CustomerMembership"/> data, gemt i en JSON-fil.
    /// </summary>
    public class CustomerMembershipRepository : Repository<CustomerMembership>, ICustomerMembershipRepository
    {
        /// <summary>
        /// Initialiserer en ny instans af <see cref="CustomerMembershipRepository"/> klassen.
        /// Stien til JSON-filen bestemmes via <see cref="JsonDataInitializer"/>.
        /// </summary>
        public CustomerMembershipRepository() : base(Path.Combine(JsonDataInitializer.CalculatedWorkspaceRoot, "Data", "Json", "customermemberships.json")) { }

        /// <summary>
        /// Tilføjer et nyt kundemedlemskab asynkront efter validering og tjek for eksisterende aktivt medlemskab af samme type.
        /// </summary>
        /// <param name="entity">Kundemedlemskabet der skal tilføjes.</param>
        /// <returns>Det tilføjede kundemedlemskab.</returns>
        /// <exception cref="ArgumentNullException">Kastes hvis <paramref name="entity"/> er null.</exception>
        /// <exception cref="RepositoryException">Kastes hvis kunden allerede har et aktivt medlemskab af denne type.</exception>
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

        /// <summary>
        /// Opdaterer et eksisterende kundemedlemskab asynkront efter validering.
        /// </summary>
        /// <param name="entity">Kundemedlemskabet med de opdaterede værdier.</param>
        /// <returns>Det opdaterede kundemedlemskab.</returns>
        /// <exception cref="ArgumentNullException">Kastes hvis <paramref name="entity"/> er null.</exception>
        public override async Task<CustomerMembership> UpdateAsync(CustomerMembership entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            ValidateEntity(entity);
            return await base.UpdateAsync(entity);
        }

        /// <summary>
        /// Henter alle medlemskaber for en specifik kunde.
        /// </summary>
        /// <param name="customerId">ID på kunden.</param>
        /// <returns>En samling af kundens medlemskaber. Returnerer en tom samling, hvis kunde-ID er ugyldigt.</returns>
        public async Task<IEnumerable<CustomerMembership>> GetMembershipsByCustomerIdAsync(int customerId)
        {
            if (customerId <= 0) 
            {
                return Enumerable.Empty<CustomerMembership>();
            }
            return await FindAsync(cm => cm.CustomerId == customerId);
        }

        /// <summary>
        /// Henter alle medlemskaber tilknyttet et specifikt medlemskabsprodukt.
        /// </summary>
        /// <param name="productId">ID på medlemskabsproduktet.</param>
        /// <returns>En samling af medlemskaber for det angivne produkt. Returnerer en tom samling, hvis produkt-ID er ugyldigt.</returns>
        public async Task<IEnumerable<CustomerMembership>> GetMembershipsByProductIdAsync(int productId)
        {
            if (productId <= 0) 
            {
                return Enumerable.Empty<CustomerMembership>();
            }
            return await FindAsync(cm => cm.MembershipProductId == productId);
        }

        /// <summary>
        /// Henter alle aktive medlemskaber.
        /// Et medlemskab betragtes som aktivt, hvis <see cref="CustomerMembership.IsActive"/> er true, og <see cref="CustomerMembership.EndDate"/> enten er null eller ikke er passeret.
        /// </summary>
        /// <returns>En samling af alle aktive medlemskaber.</returns>
        public async Task<IEnumerable<CustomerMembership>> GetActiveMembershipsAsync()
        {
            var utcNow = DateTime.UtcNow;
            return await FindAsync(cm => cm.IsActive && (cm.EndDate == null || cm.EndDate.Value.Date >= utcNow.Date));
        }

        /// <summary>
        /// Henter aktive medlemskaber der snart udløber.
        /// Et medlemskab betragtes som "snart udløbende" hvis det er aktivt, har en slutdato, denne slutdato er før <paramref name="endDateThreshold"/>, og slutdatoen ikke allerede er passeret.
        /// </summary>
        /// <param name="endDateThreshold">Tærskeldatoen. Medlemskaber med slutdato før denne dato (men efter i dag) inkluderes.</param>
        /// <returns>En samling af aktive medlemskaber, der snart udløber.</returns>
        public async Task<IEnumerable<CustomerMembership>> GetMembershipsExpiringSoonAsync(DateTime endDateThreshold)
        {
            var utcNow = DateTime.UtcNow;
            return await FindAsync(cm => 
                cm.IsActive && 
                cm.EndDate != null && 
                cm.EndDate.Value.Date < endDateThreshold.Date && 
                cm.EndDate.Value.Date >= utcNow.Date);
        }

        /// <summary>
        /// Henter medlemskaber baseret på den anvendte betalingsmetode.
        /// </summary>
        /// <param name="paymentMethod">Typen af betalingsmetode der søges efter.</param>
        /// <returns>En samling af medlemskaber med den specificerede betalingsmetode.</returns>
        /// <exception cref="ArgumentException">Kastes hvis <paramref name="paymentMethod"/> er en ugyldig enum-værdi.</exception>
        public async Task<IEnumerable<CustomerMembership>> GetMembershipsByPaymentMethodAsync(PaymentMethodType paymentMethod)
        {
            if (!Enum.IsDefined(typeof(PaymentMethodType), paymentMethod))
            {
                throw new ArgumentException("Ugyldig PaymentMethodType.", nameof(paymentMethod));
            }
            return await FindAsync(cm => cm.PaymentMethod == paymentMethod);
        }

        /// <summary>
        /// Henter et specifikt aktivt medlemskab for en given kunde og et givent medlemskabsprodukt.
        /// Et medlemskab betragtes som aktivt, hvis <see cref="CustomerMembership.IsActive"/> er true, og <see cref="CustomerMembership.EndDate"/> enten er null eller ikke er passeret.
        /// </summary>
        /// <param name="customerId">ID på kunden.</param>
        /// <param name="productId">ID på medlemskabsproduktet.</param>
        /// <returns>Det fundne aktive kundemedlemskab eller null, hvis intet match findes eller input ID'er er ugyldige.</returns>
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

        /// <summary>
        /// Validerer en <see cref="CustomerMembership"/> entitet.
        /// </summary>
        /// <param name="entity">Entiteten der skal valideres.</param>
        /// <exception cref="ArgumentOutOfRangeException">Kastes hvis CustomerId, MembershipProductId er ugyldige, eller ActualDonationAmount er negativt.</exception>
        /// <exception cref="ArgumentException">Kastes hvis StartDate er ugyldig, EndDate er før StartDate, eller PaymentMethodType er ugyldig. Kaster også hvis StartDate for et aktivt medlemskab er for langt ude i fremtiden.</exception>
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