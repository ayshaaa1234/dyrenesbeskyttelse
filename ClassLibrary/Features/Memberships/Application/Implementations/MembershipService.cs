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
    public class MembershipService : IMembershipService
    {
        private readonly ICustomerMembershipRepository _customerMembershipRepository;
        private readonly IMembershipProductRepository _membershipProductRepository;
        private readonly ICustomerRepository _customerRepository; // Nødvendig for at validere kundeeksistens

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
        public async Task<IEnumerable<MembershipProduct>> GetAllMembershipProductsAsync()
        {
            return await _membershipProductRepository.GetAllAsync();
        }

        public async Task<MembershipProduct?> GetMembershipProductByIdAsync(int productId)
        {
            return await _membershipProductRepository.GetByIdAsync(productId);
        }

        public async Task<IEnumerable<MembershipProduct>> GetAvailableMembershipProductsAsync()
        {
            return await _membershipProductRepository.GetAvailableProductsAsync();
        }

        public async Task<MembershipProduct> CreateMembershipProductAsync(MembershipProduct product)
        {
            if (product == null) throw new ArgumentNullException(nameof(product));
            // Yderligere validering kan tilføjes her baseret på forretningsregler
            return await _membershipProductRepository.AddAsync(product);
        }

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
        public async Task<CustomerMembership?> GetCustomerMembershipByIdAsync(int customerMembershipId)
        {
            return await _customerMembershipRepository.GetByIdAsync(customerMembershipId);
        }

        public async Task<IEnumerable<CustomerMembership>> GetMembershipsForCustomerAsync(int customerId)
        {
            return await _customerMembershipRepository.GetMembershipsByCustomerIdAsync(customerId);
        }

        public async Task<IEnumerable<CustomerMembership>> GetActiveMembershipsForCustomerAsync(int customerId)
        {
            var allMemberships = await _customerMembershipRepository.GetMembershipsByCustomerIdAsync(customerId);
            return allMemberships.Where(cm => cm.IsActive && (cm.EndDate == null || cm.EndDate >= DateTime.UtcNow));
        }

        public async Task<IEnumerable<CustomerMembership>> GetMembershipsByProductAsync(int productId)
        {
            return await _customerMembershipRepository.GetMembershipsByProductIdAsync(productId);
        }

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
        public async Task<IEnumerable<CustomerMembership>> GetMembershipsExpiringSoonAsync(DateTime endDateThreshold)
        {
            return await _customerMembershipRepository.GetMembershipsExpiringSoonAsync(endDateThreshold);
        }

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