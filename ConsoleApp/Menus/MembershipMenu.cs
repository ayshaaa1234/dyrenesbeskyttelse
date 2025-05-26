using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using ClassLibrary.Features.Customers.Application.Abstractions;
using ClassLibrary.Features.Memberships.Application.Abstractions;
using ClassLibrary.Features.Memberships.Core.Enums;
using ClassLibrary.Features.Memberships.Core.Models;
// using ClassLibrary.Features.Customers.Application.Abstractions; // For CustomerId validation

namespace ConsoleApp.Menus
{
    public class MembershipMenu
    {
        private readonly IMembershipService _membershipService;
        private readonly ICustomerService _customerService; // Tilføjet for kundeopslag
        // private readonly ICustomerService _customerService; // Tilføjes hvis validering af CustomerId er nødvendig

        public MembershipMenu(IMembershipService membershipService, ICustomerService customerService/*, ICustomerService customerService*/)
        {
            _membershipService = membershipService;
            _customerService = customerService;
            // _customerService = customerService;
        }

        public async Task ShowAsync()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Medlemskabsadministration");
                Console.WriteLine("----------------------------------------");
                Console.WriteLine("1. Håndter Medlemskabsprodukter");
                Console.WriteLine("2. Håndter Kundemedlemskaber");
                Console.WriteLine("0. Tilbage til hovedmenu");
                Console.WriteLine("----------------------------------------");
                Console.Write("Tag et valg: ");

                string? choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        await ShowMembershipProductMenuAsync();
                        break;
                    case "2":
                        await ShowCustomerMembershipMenuAsync();
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Ugyldigt valg. Prøv igen.");
                        Console.ReadKey();
                        break;
                }
            }
        }

        private async Task ShowMembershipProductMenuAsync()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Håndtering af Medlemskabsprodukter");
                Console.WriteLine("----------------------------------------");
                Console.WriteLine("1. Vis alle medlemskabsprodukter");
                Console.WriteLine("2. Opret nyt medlemskabsprodukt");
                Console.WriteLine("3. Opdater medlemskabsprodukt");
                Console.WriteLine("4. Slet medlemskabsprodukt");
                Console.WriteLine("5. Vis tilgængelige medlemskabsprodukter");
                Console.WriteLine("0. Tilbage til Medlemskabsadministration");
                Console.WriteLine("----------------------------------------");
                Console.Write("Tag et valg: ");

                string? choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        await ListAllMembershipProductsAsync();
                        break;
                    case "2":
                        await CreateMembershipProductAsync();
                        break;
                    case "3":
                        await UpdateMembershipProductAsync();
                        break;
                    case "4":
                        await DeleteMembershipProductAsync();
                        break;
                    case "5":
                        await ListAvailableMembershipProductsAsync();
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Ugyldigt valg. Prøv igen.");
                        Console.ReadKey();
                        break;
                }
            }
        }

        private async Task ListAllMembershipProductsAsync(IEnumerable<MembershipProduct>? productsToDisplay = null)
        {
            Console.Clear();
            var products = productsToDisplay ?? await _membershipService.GetAllMembershipProductsAsync();

            if (!products.Any())
            {
                Console.WriteLine("Ingen medlemskabsprodukter fundet.");
            }
            else
            {
                Console.WriteLine("Medlemskabsprodukter:");
                foreach (var p in products)
                {
                    Console.WriteLine($"- ID: {p.Id}, Navn: {p.Name}, Pris: {p.Price:C}, Frekvens: {p.Frequency}");
                    Console.WriteLine($"  Beskrivelse: {p.Description ?? "Ingen"}");
                    Console.WriteLine($"  Donation: {(p.IsDonation ? "Ja" : "Nej")}, Tillader brugerdef. beløb: {(p.AllowsCustomAmount ? "Ja" : "Nej")}, Tilgængelig: {(p.IsAvailable ? "Ja" : "Nej")}");
                    Console.WriteLine("---");
                }
            }
            Console.WriteLine("Tryk på en tast for at fortsætte...");
            Console.ReadKey();
        }

        private async Task CreateMembershipProductAsync()
        {
            Console.Clear();
            Console.WriteLine("Opret nyt Medlemskabsprodukt");

            string name = GetStringFromUserInput("Navn: ");
            string? description = GetStringFromUserInput("Beskrivelse (valgfri, Enter for ingen): ", true);
            if (string.IsNullOrWhiteSpace(description)) description = null;

            decimal price = GetDecimalFromUserInput("Pris (0 hvis 'Andet beløb' for donationer)");
            BillingFrequency frequency = GetEnumFromUserInput<BillingFrequency>("Vælg faktureringsfrekvens:");
            bool isDonation = GetBoolFromUserInput("Er dette primært en donation?");
            bool allowsCustomAmount = false;
            if (isDonation)
            {
                allowsCustomAmount = GetBoolFromUserInput("Tillader dette produkt et brugerdefineret beløb?");
            }
            bool isAvailable = GetBoolFromUserInput("Er produktet tilgængeligt med det samme?", true);

            var newProduct = new MembershipProduct
            {
                Name = name,
                Description = description,
                Price = price,
                Frequency = frequency,
                IsDonation = isDonation,
                AllowsCustomAmount = allowsCustomAmount,
                IsAvailable = isAvailable
            };

            try
            {
                var createdProduct = await _membershipService.CreateMembershipProductAsync(newProduct);
                Console.WriteLine($"Medlemskabsprodukt '{createdProduct.Name}' oprettet med ID: {createdProduct.Id}.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fejl ved oprettelse af medlemskabsprodukt: {ex.Message}");
            }
            Console.WriteLine("Tryk på en tast for at fortsætte...");
            Console.ReadKey();
        }

        private async Task UpdateMembershipProductAsync()
        {
            Console.Clear();
            int productId = GetIntFromUserInput("Indtast ID på medlemskabsproduktet der skal opdateres");
            var product = await _membershipService.GetMembershipProductByIdAsync(productId);

            if (product == null)
            {
                Console.WriteLine($"Medlemskabsprodukt med ID {productId} ikke fundet.");
                Console.ReadKey();
                return;
            }

            Console.WriteLine($"Opdaterer '{product.Name}' (ID: {product.Id})");

            string newName = GetStringFromUserInput($"Nyt navn (nuværende: '{product.Name}', Enter for at beholde): ", true);
            if (!string.IsNullOrWhiteSpace(newName)) product.Name = newName;
            else product.Name = product.Name; // Bevar eksisterende hvis tomt

            string? newDescriptionInput = GetStringFromUserInput($"Ny beskrivelse (nuværende: '{product.Description ?? "Ingen"}', Enter for at beholde, skriv 'fjern' for at slette): ", true);
            if (newDescriptionInput?.ToLower() == "fjern")
            {
                product.Description = null;
            }
            else if (!string.IsNullOrWhiteSpace(newDescriptionInput))
            {
                product.Description = newDescriptionInput;
            }
            // Ellers beholdes den eksisterende beskrivelse

            product.Price = GetDecimalFromUserInput($"Ny pris (nuværende: {product.Price:C})", product.Price);
            product.Frequency = GetEnumFromUserInput<BillingFrequency>($"Ny faktureringsfrekvens (nuværende: {product.Frequency})", product.Frequency);
            product.IsDonation = GetBoolFromUserInput($"Er dette primært en donation (nuværende: {(product.IsDonation ? "Ja" : "Nej")})?", product.IsDonation);
            if (product.IsDonation)
            {
                product.AllowsCustomAmount = GetBoolFromUserInput($"Tillader brugerdef. beløb (nuværende: {(product.AllowsCustomAmount ? "Ja" : "Nej")})?", product.AllowsCustomAmount);
            }
            else
            {
                product.AllowsCustomAmount = false;
            }
            product.IsAvailable = GetBoolFromUserInput($"Er produktet tilgængeligt (nuværende: {(product.IsAvailable ? "Ja" : "Nej")})?", product.IsAvailable);

            try
            {
                await _membershipService.UpdateMembershipProductAsync(product);
                Console.WriteLine($"Medlemskabsprodukt '{product.Name}' opdateret.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fejl ved opdatering: {ex.Message}");
            }
            Console.WriteLine("Tryk på en tast for at fortsætte...");
            Console.ReadKey();
        }

        private async Task DeleteMembershipProductAsync()
        {
            Console.Clear();
            int productId = GetIntFromUserInput("Indtast ID på medlemskabsproduktet der skal slettes");
            var product = await _membershipService.GetMembershipProductByIdAsync(productId);

            if (product == null)
            {
                Console.WriteLine($"Medlemskabsprodukt med ID {productId} ikke fundet.");
            }
            else
            {
                if (GetBoolFromUserInput($"Er du sikker på du vil slette '{product.Name}' (ID: {productId})? Dette er en soft delete. (ja/nej): "))
                {
                    try
                    {
                        await _membershipService.DeleteMembershipProductAsync(productId);
                        Console.WriteLine($"Medlemskabsprodukt '{product.Name}' (ID: {productId}) blev slettet (soft delete).");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Fejl ved sletning: {ex.Message}");
                    }
                }
                else
                {
                    Console.WriteLine("Sletning annulleret.");
                }
            }
            Console.WriteLine("Tryk på en tast for at fortsætte...");
            Console.ReadKey();
        }

        private async Task ListAvailableMembershipProductsAsync()
        {
            Console.Clear();
            Console.WriteLine("Tilgængelige medlemskabsprodukter:");
            var availableProducts = await _membershipService.GetAvailableMembershipProductsAsync();
            await ListAllMembershipProductsAsync(availableProducts);
        }

        private async Task ShowCustomerMembershipMenuAsync()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Håndtering af Kundemedlemskaber");
                Console.WriteLine("----------------------------------------");
                Console.WriteLine("1. Vis alle kundemedlemskaber");
                Console.WriteLine("2. Tilmeld kunde til medlemskab");
                Console.WriteLine("3. Opdater kundemedlemskab");
                Console.WriteLine("4. Annuller kundemedlemskab");
                Console.WriteLine("5. Forny kundemedlemskab");
                Console.WriteLine("6. Registrer betaling for medlemskab");
                Console.WriteLine("7. Vis medlemskaber for en kunde");
                Console.WriteLine("8. Vis aktive medlemskaber for en kunde");
                Console.WriteLine("9. Vis medlemskaber der snart udløber");
                Console.WriteLine("0. Tilbage til Medlemskabsadministration");
                Console.WriteLine("----------------------------------------");
                Console.Write("Tag et valg: ");

                string? choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        await ListAllCustomerMembershipsAsync();
                        break;
                    case "2":
                        await AddMembershipToCustomerAsync();
                        break;
                    case "3":
                        await UpdateCustomerMembershipAsync();
                        break;
                    case "4":
                        await CancelCustomerMembershipAsync();
                        break;
                    case "5":
                        await RenewCustomerMembershipAsync();
                        break;
                    case "6":
                        await RecordPaymentForCustomerMembershipAsync();
                        break;
                    case "7":
                        await ListMembershipsForCustomerAsync();
                        break;
                    case "8":
                        await ListActiveMembershipsForCustomerAsync();
                        break;
                    case "9":
                        await ListExpiringMembershipsAsync();
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Ugyldigt valg. Prøv igen.");
                        Console.ReadKey();
                        break;
                }
            }
        }

        private async Task ListAllCustomerMembershipsAsync(IEnumerable<CustomerMembership>? membershipsToDisplay = null, string? header = null)
        {
            Console.Clear();
            IEnumerable<CustomerMembership> memberships;
            if (membershipsToDisplay != null)
            {
                memberships = membershipsToDisplay;
            }
            else
            {
                // Workaround for manglende GetAllCustomerMembershipsAsync(): Hent for alle kunder.
                Console.WriteLine("Henter alle kundemedlemskaber (via workaround)...");
                var allCustomers = await _customerService.GetAllAsync();
                var allMembershipsList = new List<CustomerMembership>();
                foreach (var customer in allCustomers)
                {
                    allMembershipsList.AddRange(await _membershipService.GetMembershipsForCustomerAsync(customer.Id));
                }
                memberships = allMembershipsList;
            }
            
            Console.WriteLine(header ?? "Alle Kundemedlemskaber:");

            if (!memberships.Any())
            {
                Console.WriteLine("Ingen kundemedlemskaber fundet.");
            }
            else
            {
                foreach (var cm in memberships)
                {
                    string customerName = "Ukendt kunde";
                    var customer = await _customerService.GetByIdAsync(cm.CustomerId);
                    if (customer != null) customerName = $"{customer.FirstName} {customer.LastName}";

                    string productName = "Ukendt produkt";
                    var product = await _membershipService.GetMembershipProductByIdAsync(cm.MembershipProductId);
                    if (product != null) productName = product.Name;

                    Console.WriteLine($"- ID: {cm.Id}, Kunde: {customerName} (ID: {cm.CustomerId}), Produkt: {productName} (ID: {cm.MembershipProductId})");
                    Console.WriteLine($"  Aktiv: {(cm.IsActive ? "Ja" : "Nej")}, Start: {cm.StartDate:yyyy-MM-dd}, Slut: {(cm.EndDate.HasValue ? cm.EndDate.Value.ToString("yyyy-MM-dd") : "Løbende")}");
                    Console.WriteLine($"  Beløb: {(cm.ActualDonationAmount.HasValue ? cm.ActualDonationAmount.Value.ToString("C") : product?.Price.ToString("C") ?? "N/A")}, Bet.metode: {cm.PaymentMethod}");
                    Console.WriteLine($"  Præf: Trykt mag.: {(cm.WantsPrintedMagazine ? "Ja" : "Nej")}, Dig. mag.: {(cm.WantsDigitalMagazine ? "Ja" : "Nej")}, Nyhedsbrev: {(cm.SubscribedToNewsletter ? "Ja" : "Nej")}, Skattefradrag: {(cm.OptInForTaxDeduction ? "Ja" : "Nej")}");
                    Console.WriteLine($"  Seneste betaling: {(cm.LastPaymentDate.HasValue ? cm.LastPaymentDate.Value.ToString("yyyy-MM-dd") : "Ingen")}, Næste betaling: {(cm.NextPaymentDate.HasValue ? cm.NextPaymentDate.Value.ToString("yyyy-MM-dd") : "N/A")}");
                    Console.WriteLine("---");
                }
            }
            Console.WriteLine("Tryk på en tast for at fortsætte...");
            Console.ReadKey();
        }

        private async Task AddMembershipToCustomerAsync()
        {
            Console.Clear();
            Console.WriteLine("Tilføj Medlemskab til Kunde");

            int customerId = GetIntFromUserInput("Kunde ID: ");
            var customer = await _customerService.GetByIdAsync(customerId);
            if (customer == null)
            {
                Console.WriteLine($"Fejl: Kunde med ID {customerId} blev ikke fundet.");
                Console.ReadKey();
                return;
            }
            Console.WriteLine($"Kunde fundet: {customer.FirstName} {customer.LastName}");

            int productId = GetIntFromUserInput("Medlemskabsprodukt ID: ");
            var product = await _membershipService.GetMembershipProductByIdAsync(productId);
            if (product == null)
            {
                Console.WriteLine($"Fejl: Medlemskabsprodukt med ID {productId} blev ikke fundet.");
                Console.ReadKey();
                return;
            }
            if (!product.IsAvailable)
            {
                 Console.WriteLine($"Fejl: Medlemskabsprodukt '{product.Name}' (ID: {productId}) er ikke tilgængeligt.");
                 Console.ReadKey();
                 return;
            }
            Console.WriteLine($"Produkt fundet: {product.Name} - {product.Price:C} ({product.Frequency})");

            decimal? actualAmount = product.AllowsCustomAmount ? 
                                    GetDecimalFromUserInput($"Indtast aftalt beløb (standard: {product.Price:C})", product.Price) :
                                    product.Price;
            
            DateTime startDate = GetDateTimeFromUserInput("Startdato (YYYY-MM-DD)", DateTime.Today);
            DateTime? endDate = null; 
            if (product.Frequency != BillingFrequency.OneTime && product.Frequency != BillingFrequency.None) 
            {
                endDate = GetNullableDateTimeFromUserInput("Slutdato (YYYY-MM-DD, Enter for løbende/automatisk)");
            }

            PaymentMethodType paymentMethod = GetEnumFromUserInput<PaymentMethodType>("Vælg betalingsmetode:");
            bool wantsPrinted = GetBoolFromUserInput("Ønsker kunden trykt magasin?", false);
            bool wantsDigital = GetBoolFromUserInput("Ønsker kunden digitalt magasin?", true);
            bool newsletter = GetBoolFromUserInput("Skal kunden tilmeldes nyhedsbrev for dette medlemskab?");
            bool taxDeduct = GetBoolFromUserInput("Ønsker kunden skattefradrag (hvis muligt)?");

            try
            {
                var createdMembership = await _membershipService.AddMembershipToCustomerAsync(customerId, productId, actualAmount, startDate, endDate, paymentMethod, wantsPrinted, wantsDigital, newsletter, taxDeduct);
                Console.WriteLine($"Medlemskab oprettet med ID: {createdMembership.Id} for {customer.FirstName} {customer.LastName} på produktet {product.Name}.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fejl ved oprettelse af medlemskab: {ex.Message}");
            }
            Console.WriteLine("Tryk på en tast for at fortsætte...");
            Console.ReadKey();
        }

        private async Task UpdateCustomerMembershipAsync()
        {
            Console.Clear();
            Console.WriteLine("Opdater Kundemedlemskab");

            int customerMembershipId = GetIntFromUserInput("Indtast ID på kundemedlemskabet der skal opdateres: ");
            var existingMembership = await _membershipService.GetCustomerMembershipByIdAsync(customerMembershipId);

            if (existingMembership == null)
            {
                Console.WriteLine($"Kundemedlemskab med ID {customerMembershipId} ikke fundet.");
                Console.ReadKey();
                return;
            }

            // Hent nuværende kunde og produkt for visning
            var customer = await _customerService.GetByIdAsync(existingMembership.CustomerId);
            var currentProduct = await _membershipService.GetMembershipProductByIdAsync(existingMembership.MembershipProductId);

            Console.WriteLine($"Opdaterer medlemskab ID: {existingMembership.Id}");
            Console.WriteLine($"Kunde: {customer?.FirstName} {customer?.LastName} (ID: {existingMembership.CustomerId})");
            Console.WriteLine($"Nuværende produkt: {currentProduct?.Name} (ID: {existingMembership.MembershipProductId})");
            Console.WriteLine("----------------------------------------");

            int newProductId = GetIntFromUserInput($"Nyt Medlemskabsprodukt ID (nuværende: {existingMembership.MembershipProductId}, Enter for at beholde): ", existingMembership.MembershipProductId);
            if (newProductId != existingMembership.MembershipProductId)
            {
                var product = await _membershipService.GetMembershipProductByIdAsync(newProductId);
                if (product == null)
                {
                    Console.WriteLine($"Fejl: Medlemskabsprodukt med ID {newProductId} blev ikke fundet.");
                    Console.ReadKey();
                    return;
                }
                if (!product.IsAvailable)
                {
                    Console.WriteLine($"Fejl: Medlemskabsprodukt '{product.Name}' (ID: {newProductId}) er ikke tilgængeligt.");
                    Console.ReadKey();
                    return;
                }
                existingMembership.MembershipProductId = newProductId;
                Console.WriteLine($"Produkt ændret til: {product.Name}");
                currentProduct = product; // Opdater currentProduct til det nye valgte produkt for evt. pris-opslag nedenfor
            }

            // Opdater det faktiske donationsbeløb, hvis produktet tillader det, eller brug produktets pris
            if (currentProduct != null && currentProduct.IsDonation && currentProduct.AllowsCustomAmount)
            {
                existingMembership.ActualDonationAmount = GetDecimalFromUserInput($"Aftalt beløb (nuværende: {existingMembership.ActualDonationAmount ?? currentProduct.Price:C}, Enter for produktpris '{currentProduct.Price:C}'): ", existingMembership.ActualDonationAmount ?? currentProduct.Price);
            }
            else if (currentProduct != null)
            {
                existingMembership.ActualDonationAmount = currentProduct.Price; // Sæt til produktets faste pris hvis ikke custom donation
            }

            existingMembership.StartDate = GetDateTimeFromUserInput($"Ny startdato (nuværende: {existingMembership.StartDate:yyyy-MM-dd}, Enter for at beholde): ", existingMembership.StartDate, false);
            existingMembership.EndDate = GetNullableDateTimeFromUserInput($"Ny slutdato (nuværende: {(existingMembership.EndDate.HasValue ? existingMembership.EndDate.Value.ToString("yyyy-MM-dd") : "Løbende")}, Enter for ingen/løbende eller at beholde): ", existingMembership.EndDate);
            existingMembership.IsActive = GetBoolFromUserInput($"Er medlemskabet aktivt (nuværende: {(existingMembership.IsActive ? "Ja" : "Nej")})? ", existingMembership.IsActive);
            existingMembership.PaymentMethod = GetEnumFromUserInput<PaymentMethodType>($"Ny betalingsmetode (nuværende: {existingMembership.PaymentMethod}): ", existingMembership.PaymentMethod);
            existingMembership.WantsPrintedMagazine = GetBoolFromUserInput($"Ønsker trykt magasin (nuværende: {(existingMembership.WantsPrintedMagazine ? "Ja" : "Nej")})? ", existingMembership.WantsPrintedMagazine);
            existingMembership.WantsDigitalMagazine = GetBoolFromUserInput($"Ønsker digitalt magasin (nuværende: {(existingMembership.WantsDigitalMagazine ? "Ja" : "Nej")})? ", existingMembership.WantsDigitalMagazine);
            existingMembership.SubscribedToNewsletter = GetBoolFromUserInput($"Tilmeldt nyhedsbrev (nuværende: {(existingMembership.SubscribedToNewsletter ? "Ja" : "Nej")})? ", existingMembership.SubscribedToNewsletter);
            existingMembership.OptInForTaxDeduction = GetBoolFromUserInput($"Ønsker skattefradrag (nuværende: {(existingMembership.OptInForTaxDeduction ? "Ja" : "Nej")})? ", existingMembership.OptInForTaxDeduction);

            try
            {
                await _membershipService.UpdateCustomerMembershipAsync(existingMembership);
                Console.WriteLine("Kundemedlemskab opdateret.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fejl ved opdatering: {ex.Message}");
            }
            Console.WriteLine("Tryk på en tast for at fortsætte...");
            Console.ReadKey();
        }

        private async Task CancelCustomerMembershipAsync()
        {
            Console.Clear();
            int membershipId = GetIntFromUserInput("Indtast ID på kundemedlemskabet der skal annulleres");
            var cm = await _membershipService.GetCustomerMembershipByIdAsync(membershipId);

            if (cm == null)
            {
                Console.WriteLine($"Kundemedlemskab med ID {membershipId} ikke fundet.");
            }
            else
            {
                var customer = await _customerService.GetByIdAsync(cm.CustomerId);
                var product = await _membershipService.GetMembershipProductByIdAsync(cm.MembershipProductId);
                Console.WriteLine($"Annullerer medlemskab ID: {cm.Id} (Kunde: {customer?.FirstName} {customer?.LastName}, Produkt: {product?.Name})");
                if (GetBoolFromUserInput("Er du sikker på du vil annullere dette medlemskab?"))
                {
                    try
                    {
                        await _membershipService.CancelCustomerMembershipAsync(membershipId);
                        Console.WriteLine("Medlemskab annulleret.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Fejl ved annullering: {ex.Message}");
                    }
                }
                else
                {
                    Console.WriteLine("Annullering afbrudt.");
                }
            }
            Console.WriteLine("Tryk på en tast for at fortsætte...");
            Console.ReadKey();
        }

        private async Task RenewCustomerMembershipAsync()
        {
            Console.Clear();
            Console.WriteLine("Forny Kundemedlemskab");
            int customerMembershipIdToRenew = GetIntFromUserInput("Indtast ID på kundemedlemskabet der skal fornyes: ");
            var membershipToRenew = await _membershipService.GetCustomerMembershipByIdAsync(customerMembershipIdToRenew);

            if (membershipToRenew == null)
            {
                Console.WriteLine($"Kundemedlemskab med ID {customerMembershipIdToRenew} ikke fundet.");
                Console.ReadKey();
                return;
            }

            var productForRenewal = await _membershipService.GetMembershipProductByIdAsync(membershipToRenew.MembershipProductId);
            if (productForRenewal == null)
            {
                Console.WriteLine($"Fejl: Det tilknyttede medlemskabsprodukt (ID: {membershipToRenew.MembershipProductId}) blev ikke fundet. Kan ikke forny.");
                Console.ReadKey();
                return;
            }
             if (!productForRenewal.IsAvailable)
            {
                 Console.WriteLine($"Fejl: Medlemskabsprodukt '{productForRenewal.Name}' (ID: {productForRenewal.Id}) er ikke længere tilgængeligt for fornyelse.");
                 Console.ReadKey();
                 return;
            }

            Console.WriteLine($"Fornyer medlemskab: {productForRenewal.Name} for kunde ID {membershipToRenew.CustomerId}.");

            DateTime newEndDate = GetDateTimeFromUserInput("Indtast ny slutdato for fornyelsen", membershipToRenew.EndDate ?? DateTime.Today.AddYears(1));
            decimal? newAmount = productForRenewal.AllowsCustomAmount == true ? 
                                GetDecimalFromUserInput("Indtast evt. nyt beløb for fornyelsesperioden (Enter for uændret)", membershipToRenew.ActualDonationAmount) :
                                membershipToRenew.ActualDonationAmount; // Bevar eksisterende hvis ikke custom

            try
            {
                await _membershipService.RenewCustomerMembershipAsync(customerMembershipIdToRenew, newEndDate, newAmount);
                Console.WriteLine("Medlemskab fornyet.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fejl ved fornyelse: {ex.Message}");
            }
            Console.WriteLine("Tryk på en tast for at fortsætte...");
            Console.ReadKey();
        }

        private async Task RecordPaymentForCustomerMembershipAsync()
        {
            Console.Clear();
            int membershipId = GetIntFromUserInput("Indtast ID på kundemedlemskabet hvor betaling skal registreres");
            var cm = await _membershipService.GetCustomerMembershipByIdAsync(membershipId);

            if (cm == null)
            {
                Console.WriteLine($"Kundemedlemskab med ID {membershipId} ikke fundet.");
                Console.ReadKey();
                return;
            }
            Console.WriteLine($"Registrerer betaling for medlemskab ID: {cm.Id}");

            DateTime paymentDate = GetDateTimeFromUserInput("Betalingsdato", DateTime.Today, false);
            decimal amountPaid = GetDecimalFromUserInput("Betalt beløb", cm.ActualDonationAmount ?? (await _membershipService.GetMembershipProductByIdAsync(cm.MembershipProductId))?.Price ?? 0);

            try
            {
                await _membershipService.RecordPaymentAsync(membershipId, paymentDate, amountPaid);
                Console.WriteLine("Betaling registreret.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fejl ved registrering af betaling: {ex.Message}");
            }
            Console.WriteLine("Tryk på en tast for at fortsætte...");
            Console.ReadKey();
        }

        private async Task ListMembershipsForCustomerAsync()
        {
            Console.Clear();
            int customerId = GetIntFromUserInput("Indtast Kunde ID for at se medlemskaber");
            var customer = await _customerService.GetByIdAsync(customerId);
            if (customer == null)
            {
                Console.WriteLine($"Kunde med ID {customerId} ikke fundet.");
                Console.ReadKey();
                return;
            }

            var memberships = await _membershipService.GetMembershipsForCustomerAsync(customerId);
            await ListAllCustomerMembershipsAsync(memberships, $"Medlemskaber for {customer.FirstName} {customer.LastName}:");
        }

        private async Task ListActiveMembershipsForCustomerAsync()
        {
            Console.Clear();
            int customerId = GetIntFromUserInput("Indtast Kunde ID for at se aktive medlemskaber");
            var customer = await _customerService.GetByIdAsync(customerId);
            if (customer == null)
            {
                Console.WriteLine($"Kunde med ID {customerId} ikke fundet.");
                Console.ReadKey();
                return;
            }

            var activeMemberships = await _membershipService.GetActiveMembershipsForCustomerAsync(customerId);
            await ListAllCustomerMembershipsAsync(activeMemberships, $"Aktive medlemskaber for {customer.FirstName} {customer.LastName}:");
        }

        private async Task ListExpiringMembershipsAsync()
        {
            Console.Clear();
            DateTime thresholdDate = GetDateTimeFromUserInput("Vis medlemskaber der udløber før eller på dato (YYYY-MM-DD)", DateTime.Today.AddMonths(1), false);
            var expiringMemberships = await _membershipService.GetMembershipsExpiringSoonAsync(thresholdDate);
            await ListAllCustomerMembershipsAsync(expiringMemberships, $"Medlemskaber der udløber senest {thresholdDate:yyyy-MM-dd}:");
        }

        // --- Hjælpemetoder ---
        private string GetStringFromUserInput(string prompt, bool allowNullOrEmpty = false, string? defaultValue = null)
        {
            while (true)
            {
                Console.Write(prompt);
                if (defaultValue != null) Console.Write($" (standard: {defaultValue}): ");
                string? input = Console.ReadLine();
                if (defaultValue != null && string.IsNullOrWhiteSpace(input))
                {
                    return defaultValue;
                }
                if (!string.IsNullOrWhiteSpace(input))
                {
                    return input;
                }
                if (allowNullOrEmpty && input == null)
                {
                    return string.Empty;
                }
                Console.WriteLine("Input må ikke være tomt. Prøv igen.");
            }
        }

        private int GetIntFromUserInput(string prompt, int? defaultValue = null)
        {
            while (true)
            {
                Console.Write(prompt);
                if (defaultValue.HasValue) Console.Write($" (standard: {defaultValue.Value}): "); else Console.Write(": ");
                string? input = Console.ReadLine();
                if (defaultValue.HasValue && string.IsNullOrWhiteSpace(input))
                {
                    return defaultValue.Value;
                }
                if (int.TryParse(input, out int value))
                {
                    return value;
                }
                Console.WriteLine("Ugyldigt heltal. Prøv igen.");
            }
        }

        private decimal GetDecimalFromUserInput(string prompt, decimal? defaultValue = null)
        {
            while (true)
            {
                Console.Write(prompt);
                if (defaultValue.HasValue) Console.Write($" (standard: {defaultValue.Value:F2}): "); else Console.Write(": ");
                string? input = Console.ReadLine();
                if (defaultValue.HasValue && string.IsNullOrWhiteSpace(input))
                {
                    return defaultValue.Value;
                }
                if (decimal.TryParse(input, NumberStyles.Any, CultureInfo.CurrentCulture, out decimal value) || 
                    decimal.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
                {
                    return value;
                }
                Console.WriteLine("Ugyldigt decimaltal. Brug systemets decimaltegn (f.eks. komma eller punktum). Prøv igen.");
            }
        }

        private bool GetBoolFromUserInput(string prompt, bool? defaultValue = null)
        {
            while (true)
            {
                Console.Write(prompt);
                string defaultHint = defaultValue.HasValue ? (defaultValue.Value ? " (ja/nej, standard: ja): " : " (ja/nej, standard: nej): ") : " (ja/nej): ";
                Console.Write(defaultHint);
                string? input = Console.ReadLine()?.ToLower().Trim();

                if (defaultValue.HasValue && string.IsNullOrWhiteSpace(input))
                {
                    return defaultValue.Value;
                }

                if (input == "ja" || input == "j") return true;
                if (input == "nej" || input == "n") return false;
                
                Console.WriteLine("Ugyldigt input. Skriv 'ja' eller 'nej'.");
            }
        }
        
        private DateTime GetDateTimeFromUserInput(string prompt, DateTime? defaultValue = null, bool allowEmptyAndReturnMin = false)
        {
            while (true)
            {
                Console.Write(prompt);
                string format = "yyyy-MM-dd";
                if (defaultValue.HasValue) Console.Write($" ({format}, standard: {defaultValue.Value.ToString(format)}): "); 
                else Console.Write($" ({format}): ");
                string? input = Console.ReadLine();

                if (allowEmptyAndReturnMin && string.IsNullOrWhiteSpace(input))
                {
                    return DateTime.MinValue; 
                }
                if (defaultValue.HasValue && string.IsNullOrWhiteSpace(input))
                {
                    return defaultValue.Value;
                }
                if (DateTime.TryParseExact(input, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date))
                {
                    return date;
                }
                if (DateTime.TryParse(input, CultureInfo.CurrentCulture, DateTimeStyles.None, out date)) 
                {
                    return date;
                }
                Console.WriteLine($"Ugyldigt datoformat. Brug {format}. Prøv igen.");
            }
        }
        
        private DateTime? GetNullableDateTimeFromUserInput(string prompt, DateTime? defaultValue = null)
        {
            while (true)
            {
                Console.Write(prompt);
                string format = "yyyy-MM-dd";
                string defaultHint = defaultValue.HasValue ? $" ({format}, standard: {defaultValue.Value.ToString(format)}, Enter for ingen): " : $" ({format}, Enter for ingen): ";
                Console.Write(defaultHint);
                string? input = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(input))
                {
                    return defaultValue;
                }
                if (DateTime.TryParseExact(input, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date))
                {
                    return date;
                }
                if (DateTime.TryParse(input, CultureInfo.CurrentCulture, DateTimeStyles.None, out date)) 
                {
                    return date;
                }
                Console.WriteLine($"Ugyldigt datoformat. Brug {format} eller tryk Enter for ingen dato. Prøv igen.");
            }
        }

        private TEnum GetEnumFromUserInput<TEnum>(string prompt, TEnum? defaultValue = null) where TEnum : struct, Enum
        {
            Console.WriteLine(prompt);
            var enumValues = Enum.GetValues(typeof(TEnum)).Cast<TEnum>().ToList();
            for (int i = 0; i < enumValues.Count; i++)
            {
                Console.WriteLine($"  {i + 1}. {enumValues[i]}");
            }
            
            while (true)
            {
                string defaultHint = "";
                if (defaultValue.HasValue)
                {
                    int defaultIndex = enumValues.IndexOf(defaultValue.Value);
                    defaultHint = defaultIndex >= 0 ? $" (standard: {defaultValue.Value} - valg nr. {defaultIndex + 1}): " : $" (standard: {defaultValue.Value}): ";
                }
                else
                {
                    defaultHint = " (tal): ";
                }
                Console.Write($"Vælg et tal{defaultHint}");
                string? input = Console.ReadLine();

                if (defaultValue.HasValue && string.IsNullOrWhiteSpace(input))
                {
                    return defaultValue.Value;
                }

                if (int.TryParse(input, out int choice) && choice >= 1 && choice <= enumValues.Count)
                {
                    return enumValues[choice - 1];
                }
                Console.WriteLine("Ugyldigt valg. Prøv igen.");
            }
        }
    }
} 