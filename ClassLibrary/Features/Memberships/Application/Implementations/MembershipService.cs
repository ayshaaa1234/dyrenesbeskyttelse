using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClassLibrary.Features.Memberships.Core.Models;
using ClassLibrary.Features.Memberships.Core.Enums;
using ClassLibrary.Features.Memberships.Application.Abstractions;
using ClassLibrary.Features.Memberships.Infrastructure.Abstractions;
using ClassLibrary.Features.Customers.Infrastructure.Abstractions; // For at hente Customer
using ClassLibrary.SharedKernel.Exceptions; // For RepositoryException etc.

namespace ClassLibrary.Features.Memberships.Application.Implementations
{
    /// <summary>
    /// Service til håndtering af forretningslogik for medlemskabsprodukter og kundemedlemskaber.
    /// </summary>
    public class MembershipService : IMembershipService
    {
        private readonly ICustomerMembershipRepository _customerMembershipRepository;
        private readonly IMembershipProductRepository _membershipProductRepository;
        private readonly ICustomerRepository _customerRepository; // Nødvendig for at validere kundeeksistens

        /// <summary>
        /// Initialiserer en ny instans af <see cref="MembershipService"/> klassen.
        /// </summary>
        /// <param name="customerMembershipRepository">Repository for kundemedlemskaber.</param>
        /// <param name="membershipProductRepository">Repository for medlemskabsprodukter.</param>
        /// <param name="customerRepository">Repository for kunder.</param>
        /// <exception cref="ArgumentNullException">Kastes hvis et af de nødvendige repositories er null.</exception>
        public MembershipService(
            ICustomerMembershipRepository customerMembershipRepository,
            IMembershipProductRepository membershipProductRepository,
            ICustomerRepository customerRepository)
        {
            _customerMembershipRepository = customerMembershipRepository 
                ?? throw new ArgumentNullException(nameof(customerMembershipRepository));
            _membershipProductRepository = membershipProductRepository 
                ?? throw new ArgumentNullException(nameof(membershipProductRepository));
            _customerRepository = customerRepository 
                ?? throw new ArgumentNullException(nameof(customerRepository));
        }

        // Membership Product operations
        /// <summary>
        /// Henter alle medlemskabsprodukter.
        /// </summary>
        /// <returns>En samling af alle medlemskabsprodukter.</returns>
        public async Task<IEnumerable<MembershipProduct>> GetAllMembershipProductsAsync()
        {
            return await _membershipProductRepository.GetAllAsync();
        }

        /// <summary>
        /// Henter et specifikt medlemskabsprodukt baseret på ID.
        /// </summary>
        /// <param name="productId">ID på medlemskabsproduktet.</param>
        /// <returns>Det fundne medlemskabsprodukt eller null, hvis det ikke findes.</returns>
        public async Task<MembershipProduct?> GetMembershipProductByIdAsync(int productId)
        {
            return await _membershipProductRepository.GetByIdAsync(productId);
        }

        /// <summary>
        /// Henter alle tilgængelige (ikke slettede/udgåede) medlemskabsprodukter.
        /// </summary>
        /// <returns>En samling af tilgængelige medlemskabsprodukter.</returns>
        public async Task<IEnumerable<MembershipProduct>> GetAvailableMembershipProductsAsync()
        {
            return await _membershipProductRepository.GetAvailableProductsAsync();
        }

        /// <summary>
        /// Opretter et nyt medlemskabsprodukt.
        /// </summary>
        /// <param name="product">Medlemskabsproduktet der skal oprettes.</param>
        /// <returns>Det oprettede medlemskabsprodukt.</returns>
        /// <exception cref="ArgumentNullException">Kastes hvis produktet er null.</exception>
        public async Task<MembershipProduct> CreateMembershipProductAsync(MembershipProduct product)
        {
            if (product == null) throw new ArgumentNullException(nameof(product));
            // Yderligere validering kan tilføjes her baseret på forretningsregler
            return await _membershipProductRepository.AddAsync(product);
        }

        /// <summary>
        /// Opdaterer et eksisterende medlemskabsprodukt.
        /// </summary>
        /// <param name="product">Medlemskabsproduktet med de opdaterede værdier.</param>
        /// <returns>Det opdaterede medlemskabsprodukt.</returns>
        /// <exception cref="ArgumentNullException">Kastes hvis produktet er null.</exception>
        /// <exception cref="KeyNotFoundException">Kastes hvis medlemskabsproduktet ikke findes.</exception>
        public async Task<MembershipProduct> UpdateMembershipProductAsync(MembershipProduct product)
        {
            if (product == null) throw new ArgumentNullException(nameof(product));
            var existing = await _membershipProductRepository.GetByIdAsync(product.Id);
            if (existing == null) 
                throw new KeyNotFoundException($"Medlemskabsprodukt med ID {product.Id} blev ikke fundet.");
            
            // Her kan man kopiere felter eller bruge AutoMapper
            existing.Name = product.Name;
            existing.Description = product.Description;
            existing.Price = product.Price;
            existing.Frequency = product.Frequency;
            existing.IsDonation = product.IsDonation;
            existing.AllowsCustomAmount = product.AllowsCustomAmount;
            existing.IsAvailable = product.IsAvailable;
            // existing.IncludesPrintedMagazineOffer = product.IncludesPrintedMagazineOffer; // Hvis felterne tilføjes
            // existing.IncludesDigitalMagazineOffer = product.IncludesDigitalMagazineOffer;

            return await _membershipProductRepository.UpdateAsync(existing);
        }

        /// <summary>
        /// Sletter et medlemskabsprodukt (soft delete).
        /// </summary>
        /// <param name="productId">ID på medlemskabsproduktet der skal slettes.</param>
        /// <returns>En opgave, der repræsenterer den asynkrone operation.</returns>
        /// <exception cref="KeyNotFoundException">Kastes hvis medlemskabsproduktet ikke findes.</exception>
        public async Task DeleteMembershipProductAsync(int productId)
        {
            // IRepository.DeleteAsync udfører soft delete
            var product = await _membershipProductRepository.GetByIdAsync(productId);
            if (product == null) 
                throw new KeyNotFoundException($"Medlemskabsprodukt med ID {productId} blev ikke fundet for sletning.");
            
            // Overvej logik: Skal man kunne slette et produkt, hvis der er aktive medlemskaber knyttet til det?
            // For nu antager vi ja, og soft delete er tilstrækkeligt.
            await _membershipProductRepository.DeleteAsync(productId);
        }

        // Customer Membership operations
        /// <summary>
        /// Henter et specifikt kundemedlemskab baseret på ID.
        /// </summary>
        /// <param name="customerMembershipId">ID på kundemedlemskabet.</param>
        /// <returns>Det fundne kundemedlemskab eller null, hvis det ikke findes.</returns>
        public async Task<CustomerMembership?> GetCustomerMembershipByIdAsync(int customerMembershipId)
        {
            return await _customerMembershipRepository.GetByIdAsync(customerMembershipId);
        }

        /// <summary>
        /// Henter alle medlemskaber for en specifik kunde.
        /// </summary>
        /// <param name="customerId">ID på kunden.</param>
        /// <returns>En samling af kundens medlemskaber.</returns>
        public async Task<IEnumerable<CustomerMembership>> GetMembershipsForCustomerAsync(int customerId)
        {
            return await _customerMembershipRepository.GetMembershipsByCustomerIdAsync(customerId);
        }

        /// <summary>
        /// Henter alle aktive medlemskaber for en specifik kunde.
        /// Et medlemskab betragtes som aktivt, hvis IsActive er true og slutdatoen enten er null eller i fremtiden.
        /// </summary>
        /// <param name="customerId">ID på kunden.</param>
        /// <returns>En samling af kundens aktive medlemskaber.</returns>
        public async Task<IEnumerable<CustomerMembership>> GetActiveMembershipsForCustomerAsync(int customerId)
        {
            var allMemberships = await _customerMembershipRepository.GetMembershipsByCustomerIdAsync(customerId);
            return allMemberships.Where(cm => cm.IsActive && (cm.EndDate == null || cm.EndDate >= DateTime.UtcNow));
        }

        /// <summary>
        /// Henter alle kundemedlemskaber tilknyttet et specifikt medlemskabsprodukt.
        /// </summary>
        /// <param name="productId">ID på medlemskabsproduktet.</param>
        /// <returns>En samling af kundemedlemskaber for det angivne produkt.</returns>
        public async Task<IEnumerable<CustomerMembership>> GetMembershipsByProductAsync(int productId)
        {
            return await _customerMembershipRepository.GetMembershipsByProductIdAsync(productId);
        }

        /// <summary>
        /// Tilføjer et nyt medlemskab til en kunde.
        /// </summary>
        /// <param name="customerId">ID på kunden.</param>
        /// <param name="productId">ID på medlemskabsproduktet.</param>
        /// <param name="actualDonationAmount">Det faktiske donationsbeløb. Nødvendigt hvis produktet tillader brugerdefineret beløb, ellers valgfrit (bruger produktpris).</param>
        /// <param name="startDate">Startdato for medlemskabet.</param>
        /// <param name="endDate">Slutdato for medlemskabet (valgfri, kan beregnes baseret på produkt).</param>
        /// <param name="paymentMethod">Betalingsmetode anvendt.</param>
        /// <param name="wantsPrintedMagazine">Angiver om kunden ønsker trykt magasin.</param>
        /// <param name="wantsDigitalMagazine">Angiver om kunden ønsker digitalt magasin.</param>
        /// <param name="subscribedToNewsletter">Angiver om kunden er tilmeldt nyhedsbrevet.</param>
        /// <param name="optInForTaxDeduction">Angiver om kunden ønsker skattefradrag.</param>
        /// <returns>Det oprettede kundemedlemskab.</returns>
        /// <exception cref="ArgumentException">Kastes hvis kunden eller produktet ikke findes, produktet ikke er tilgængeligt, eller donationsbeløbet er ugyldigt i forhold til produktets konfiguration.</exception>
        public async Task<CustomerMembership> AddMembershipToCustomerAsync(int customerId, int productId, decimal? actualDonationAmount, DateTime startDate, DateTime? endDate, PaymentMethodType paymentMethod, bool wantsPrintedMagazine, bool wantsDigitalMagazine, bool subscribedToNewsletter, bool optInForTaxDeduction)
        {
            var customer = await _customerRepository.GetByIdAsync(customerId);
            if (customer == null || customer.IsDeleted) 
                throw new ArgumentException("Kunde ikke fundet eller er slettet.", nameof(customerId));

            var product = await _membershipProductRepository.GetByIdAsync(productId);
            if (product == null)
                throw new ArgumentException("Medlemskabsprodukt ikke fundet.", nameof(productId));
            if (product.IsDeleted || !product.IsAvailable)
                throw new ArgumentException("Medlemskabsprodukt er slettet eller er ikke tilgængeligt.", nameof(productId));

            if (product.AllowsCustomAmount == false && actualDonationAmount.HasValue && actualDonationAmount != product.Price)
                throw new ArgumentException("Dette produkt tillader ikke et specificeret donationsbeløb forskelligt fra produktprisen.", nameof(actualDonationAmount));
            
            if (!product.AllowsCustomAmount && !actualDonationAmount.HasValue)
                actualDonationAmount = product.Price; // Sæt default pris hvis ikke custom og intet angivet
            else if (product.AllowsCustomAmount && !actualDonationAmount.HasValue)
                 throw new ArgumentException("Donationsbeløb skal angives for dette produkt.", nameof(actualDonationAmount));
            
            var newMembership = new CustomerMembership
            {
                CustomerId = customerId,
                MembershipProductId = productId,
                ActualDonationAmount = actualDonationAmount,
                StartDate = startDate,
                EndDate = endDate,
                IsActive = true, // Nyt medlemskab er aktivt
                PaymentMethod = paymentMethod,
                WantsPrintedMagazine = wantsPrintedMagazine,
                WantsDigitalMagazine = wantsDigitalMagazine,
                SubscribedToNewsletter = subscribedToNewsletter,
                OptInForTaxDeduction = optInForTaxDeduction,
                LastPaymentDate = null, // Sættes ved første betaling
                NextPaymentDate = null  // Beregnes baseret på frekvens og startdato
            };
            
            // CustomerMembershipRepository.AddAsync har allerede et tjek for duplikat aktive medlemskaber
            return await _customerMembershipRepository.AddAsync(newMembership);
        }

        /// <summary>
        /// Opdaterer et eksisterende kundemedlemskab.
        /// </summary>
        /// <param name="customerMembership">Kundemedlemskabet med de opdaterede værdier.</param>
        /// <returns>Det opdaterede kundemedlemskab.</returns>
        /// <exception cref="ArgumentNullException">Kastes hvis customerMembership er null.</exception>
        /// <exception cref="KeyNotFoundException">Kastes hvis kundemedlemskabet ikke findes.</exception>
        /// <exception cref="ArgumentException">Kastes hvis et opdateret MembershipProductId peger på et ugyldigt produkt.</exception>
        public async Task<CustomerMembership> UpdateCustomerMembershipAsync(CustomerMembership customerMembership)
        {
            if (customerMembership == null) throw new ArgumentNullException(nameof(customerMembership));
            var existing = await _customerMembershipRepository.GetByIdAsync(customerMembership.Id);
            if (existing == null)
                throw new KeyNotFoundException($"Kundemedlemskab med ID {customerMembership.Id} blev ikke fundet.");

            // Her bør man overføre ændringer fra customerMembership til existing, og så kalde UpdateAsync(existing)
            // For at undgå at overskrive felter utilsigtet.
            // For simpelt eksempel, direkte update:
            existing.ActualDonationAmount = customerMembership.ActualDonationAmount;
            existing.StartDate = customerMembership.StartDate;
            existing.EndDate = customerMembership.EndDate;
            existing.IsActive = customerMembership.IsActive;
            existing.PaymentMethod = customerMembership.PaymentMethod;
            existing.WantsPrintedMagazine = customerMembership.WantsPrintedMagazine;
            existing.WantsDigitalMagazine = customerMembership.WantsDigitalMagazine;
            existing.SubscribedToNewsletter = customerMembership.SubscribedToNewsletter;
            existing.OptInForTaxDeduction = customerMembership.OptInForTaxDeduction;
            existing.LastPaymentDate = customerMembership.LastPaymentDate;
            existing.NextPaymentDate = customerMembership.NextPaymentDate;
            
            // Valider at produktet stadig er gyldigt, hvis MembershipProductId ændres (ikke typisk for update, men for fuldstændighed)
            if (existing.MembershipProductId != customerMembership.MembershipProductId) {
                 var product = await _membershipProductRepository.GetByIdAsync(customerMembership.MembershipProductId);
                 if (product == null) 
                    throw new ArgumentException("Nyt medlemskabsprodukt er ugyldigt (ikke fundet).", nameof(customerMembership.MembershipProductId));
                 if (product.IsDeleted || !product.IsAvailable)
                    throw new ArgumentException("Nyt medlemskabsprodukt er ugyldigt (slettet eller ikke tilgængeligt).", nameof(customerMembership.MembershipProductId));
                 existing.MembershipProductId = customerMembership.MembershipProductId; // Opdater hvis det er en del af use-case
            }

            return await _customerMembershipRepository.UpdateAsync(existing);
        }

        /// <summary>
        /// Annullerer et kundemedlemskab. Medlemskabet markeres som inaktivt, og slutdatoen sættes til nuværende tidspunkt, hvis den ikke allerede er i fortiden.
        /// </summary>
        /// <param name="customerMembershipId">ID på kundemedlemskabet der skal annulleres.</param>
        /// <returns>En opgave, der repræsenterer den asynkrone operation.</returns>
        /// <exception cref="KeyNotFoundException">Kastes hvis kundemedlemskabet ikke findes.</exception>
        /// <exception cref="InvalidOperationException">Kastes hvis medlemskabet allerede er inaktivt.</exception>
        public async Task CancelCustomerMembershipAsync(int customerMembershipId)
        {
            var membership = await _customerMembershipRepository.GetByIdAsync(customerMembershipId);
            if (membership == null)
                throw new KeyNotFoundException($"Kundemedlemskab med ID {customerMembershipId} blev ikke fundet.");

            if (!membership.IsActive)
                throw new InvalidOperationException("Medlemskabet er allerede inaktivt.");

            membership.IsActive = false;
            if (membership.EndDate == null || membership.EndDate > DateTime.UtcNow)
            {
                membership.EndDate = DateTime.UtcNow; // Sæt slutdato til nu, hvis ikke allerede i fortiden
            }
            await _customerMembershipRepository.UpdateAsync(membership);
        }

        /// <summary>
        /// Fornyer et eksisterende kundemedlemskab ved at opdatere slutdato og eventuelt donationsbeløb.
        /// Medlemskabet markeres som aktivt.
        /// </summary>
        /// <param name="customerMembershipId">ID på kundemedlemskabet der skal fornyes.</param>
        /// <param name="newEndDate">Den nye slutdato for medlemskabet.</param>
        /// <param name="newActualDonationAmount">Det nye faktiske donationsbeløb (valgfri; bruger produktpris eller eksisterende beløb afhængig af produktkonfiguration).</param>
        /// <returns>En opgave, der repræsenterer den asynkrone operation.</returns>
        /// <exception cref="KeyNotFoundException">Kastes hvis kundemedlemskabet ikke findes.</exception>
        /// <exception cref="InvalidOperationException">Kastes hvis det tilhørende medlemskabsprodukt ikke findes eller er ugyldigt.</exception>
        /// <exception cref="ArgumentException">Kastes hvis den nye slutdato ikke er efter den nuværende, eller hvis donationsbeløbet er ugyldigt i forhold til produktet.</exception>
        public async Task RenewCustomerMembershipAsync(int customerMembershipId, DateTime newEndDate, decimal? newActualDonationAmount)
        {
            var membership = await _customerMembershipRepository.GetByIdAsync(customerMembershipId);
            if (membership == null)
                throw new KeyNotFoundException($"Kundemedlemskab med ID {customerMembershipId} blev ikke fundet.");

            var product = await _membershipProductRepository.GetByIdAsync(membership.MembershipProductId);
            if (product == null) 
                throw new InvalidOperationException("Tilhørende medlemskabsprodukt blev ikke fundet.");
            if (product.IsDeleted || !product.IsAvailable)
                throw new InvalidOperationException("Tilhørende medlemskabsprodukt er ikke længere gyldigt (slettet eller ikke tilgængeligt).");

            if (newEndDate <= (membership.EndDate ?? membership.StartDate))
                throw new ArgumentException("Ny slutdato skal være efter den nuværende slutdato (eller startdato hvis ingen slutdato).", nameof(newEndDate));

            if (product.AllowsCustomAmount == false && newActualDonationAmount.HasValue && newActualDonationAmount != product.Price)
                throw new ArgumentException("Dette produkt tillader ikke et specificeret donationsbeløb forskelligt fra produktprisen.", nameof(newActualDonationAmount));
            
            membership.EndDate = newEndDate;
            membership.ActualDonationAmount = newActualDonationAmount ?? (product.AllowsCustomAmount ? membership.ActualDonationAmount : product.Price);
            membership.IsActive = true; // Sikrer at det er aktivt
            // Overvej at opdatere NextPaymentDate her

            await _customerMembershipRepository.UpdateAsync(membership);
        }

        /// <summary>
        /// Registrerer en betaling for et kundemedlemskab og beregner næste betalingsdato baseret på produktets frekvens.
        /// </summary>
        /// <param name="customerMembershipId">ID på kundemedlemskabet.</param>
        /// <param name="paymentDate">Dato for betalingen.</param>
        /// <param name="amountPaid">Betalt beløb.</param>
        /// <returns>En opgave, der repræsenterer den asynkrone operation.</returns>
        /// <exception cref="KeyNotFoundException">Kastes hvis kundemedlemskabet ikke findes.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Kastes hvis det betalte beløb ikke er positivt.</exception>
        /// <exception cref="InvalidOperationException">Kastes hvis det tilknyttede medlemskabsprodukt ikke kan findes.</exception>
        public async Task RecordPaymentAsync(int customerMembershipId, DateTime paymentDate, decimal amountPaid)
        {
            var membership = await _customerMembershipRepository.GetByIdAsync(customerMembershipId);
            if (membership == null)
                throw new KeyNotFoundException($"Kundemedlemskab med ID {customerMembershipId} blev ikke fundet.");
            
            if (amountPaid <= 0) 
                throw new ArgumentOutOfRangeException(nameof(amountPaid), "Betalt beløb skal være positivt.");

            var product = await _membershipProductRepository.GetByIdAsync(membership.MembershipProductId);
            if (product == null)
                throw new InvalidOperationException($"Tilknyttet medlemskabsprodukt med ID {membership.MembershipProductId} blev ikke fundet.");

            membership.LastPaymentDate = paymentDate;
            
            // Beregn NextPaymentDate baseret på produktets frekvens
            switch (product.Frequency)
            {
                case BillingFrequency.Monthly:
                    membership.NextPaymentDate = paymentDate.AddMonths(1);
                    break;
                case BillingFrequency.Quarterly:
                    membership.NextPaymentDate = paymentDate.AddMonths(3);
                    break;
                case BillingFrequency.Annually:
                case BillingFrequency.Yearly: // Behandler Yearly som Annually
                    membership.NextPaymentDate = paymentDate.AddYears(1);
                    break;
                case BillingFrequency.Biennially:
                    membership.NextPaymentDate = paymentDate.AddYears(2);
                    break;
                case BillingFrequency.OneTime:
                default:
                    membership.NextPaymentDate = null; // Ingen næste betaling for engangs eller ukendt frekvens
                    break;
            }
            
            await _customerMembershipRepository.UpdateAsync(membership);
        }

        // Rapportering/opslag
        /// <summary>
        /// Henter kundemedlemskaber der snart udløber (hvor slutdatoen er før eller lig med den angivne tærskeldato).
        /// </summary>
        /// <param name="endDateThreshold">Tærskeldato for hvornår et medlemskab betragtes som snart udløbende.</param>
        /// <returns>En samling af kundemedlemskaber, der snart udløber.</returns>
        public async Task<IEnumerable<CustomerMembership>> GetMembershipsExpiringSoonAsync(DateTime endDateThreshold)
        {
            return await _customerMembershipRepository.GetMembershipsExpiringSoonAsync(endDateThreshold);
        }

        /// <summary>
        /// Henter aktive kundemedlemskaber baseret på deres betalingsstatus.
        /// </summary>
        /// <param name="paymentOverdue">True for at hente medlemskaber med forfalden betaling (NextPaymentDate er i fortiden), False for at hente dem uden forfalden betaling.</param>
        /// <returns>En samling af aktive kundemedlemskaber, der matcher den angivne betalingsstatus.</returns>
        public async Task<IEnumerable<CustomerMembership>> GetMembershipsByPaymentStatusAsync(bool paymentOverdue)
        {
            var allActiveMemberships = await _customerMembershipRepository.FindAsync(cm => cm.IsActive);
            var today = DateTime.UtcNow.Date; // Brug .Date for at sammenligne kun dato-delen

            if (paymentOverdue)
            {
                // Find medlemskaber hvor NextPaymentDate er sat og er i fortiden
                return allActiveMemberships.Where(cm => cm.NextPaymentDate.HasValue && cm.NextPaymentDate.Value.Date < today);
            }
            else
            {
                // Find medlemskaber hvor NextPaymentDate enten ikke er sat, eller er i dag eller fremtiden
                return allActiveMemberships.Where(cm => !cm.NextPaymentDate.HasValue || cm.NextPaymentDate.Value.Date >= today);
            }
        }
    }
} 