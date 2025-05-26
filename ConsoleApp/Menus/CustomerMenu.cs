using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using ClassLibrary.Services;
using ClassLibrary.Models;

namespace ConsoleApp.Menus
{
    /// <summary>
    /// Menu til håndtering af kunder
    /// </summary>
    public class CustomerMenu : MenuBase
    {
        private readonly CustomerService _customerService;

        public CustomerMenu(CustomerService customerService)
        {
            _customerService = customerService;
        }

        public override async Task ShowAsync()
        {
            while (true)
            {
                ShowHeader("Kunde");
                Console.WriteLine("1. Vis alle kunder");
                Console.WriteLine("2. Vis kunde efter ID");
                Console.WriteLine("3. Vis kunde efter email");
                Console.WriteLine("4. Vis kunder efter navn");
                Console.WriteLine("5. Vis kunder efter telefon");
                Console.WriteLine("6. Vis kunder efter adresse");
                Console.WriteLine("7. Vis kunder efter postnummer");
                Console.WriteLine("8. Vis kunder efter by");
                Console.WriteLine("9. Vis medlemmer");
                Console.WriteLine("10. Vis ikke-medlemmer");
                Console.WriteLine("11. Vis kunder efter medlemskabstype");
                Console.WriteLine("12. Vis kunder efter registreringsdato");
                Console.WriteLine("13. Vis kunder med adoptioner");
                Console.WriteLine("14. Opret ny kunde");
                Console.WriteLine("15. Opdater kunde");
                Console.WriteLine("16. Slet kunde");
                Console.WriteLine("17. Tilføj medlemskab");
                Console.WriteLine("18. Fjern medlemskab");
                Console.WriteLine("19. Opdater medlemskabstype");
                Console.WriteLine("0. Tilbage til hovedmenu");
                Console.Write("\nVælg en mulighed: ");

                var choice = Console.ReadLine();

                try
                {
                    switch (choice)
                    {
                        case "1":
                            await ShowAllCustomers();
                            break;
                        case "2":
                            await ShowCustomerById();
                            break;
                        case "3":
                            await ShowCustomerByEmail();
                            break;
                        case "4":
                            await ShowCustomersByName();
                            break;
                        case "5":
                            await ShowCustomersByPhone();
                            break;
                        case "6":
                            await ShowCustomersByAddress();
                            break;
                        case "7":
                            await ShowCustomersByPostalCode();
                            break;
                        case "8":
                            await ShowCustomersByCity();
                            break;
                        case "9":
                            await ShowMembers();
                            break;
                        case "10":
                            await ShowNonMembers();
                            break;
                        case "11":
                            await ShowCustomersByMembershipType();
                            break;
                        case "12":
                            await ShowCustomersByRegistrationDate();
                            break;
                        case "13":
                            await ShowCustomersWithAdoptions();
                            break;
                        case "14":
                            await CreateNewCustomer();
                            break;
                        case "15":
                            await UpdateCustomer();
                            break;
                        case "16":
                            await DeleteCustomer();
                            break;
                        case "17":
                            await AddMembership();
                            break;
                        case "18":
                            await RemoveMembership();
                            break;
                        case "19":
                            await UpdateMembershipType();
                            break;
                        case "0":
                            return;
                        default:
                            ShowError("Ugyldigt valg");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    HandleException(ex);
                }
            }
        }

        private async Task ShowAllCustomers()
        {
            ShowHeader("Alle kunder");
            var customers = await _customerService.GetAllCustomersAsync();
            DisplayCustomers(customers);
        }

        private async Task ShowCustomerById()
        {
            ShowHeader("Kunde efter ID");
            Console.Write("Indtast kunde ID: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                ShowError("Ugyldigt ID");
                return;
            }

            var customer = await _customerService.GetCustomerByIdAsync(id);
            DisplayCustomerInfo(customer);
        }

        private async Task ShowCustomerByEmail()
        {
            ShowHeader("Kunde efter email");
            Console.Write("Indtast email: ");
            var email = Console.ReadLine() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(email))
            {
                ShowError("Email kan ikke være tom");
                return;
            }

            var customer = await _customerService.GetCustomerByEmailAsync(email);
            DisplayCustomerInfo(customer);
        }

        private async Task ShowCustomersByName()
        {
            ShowHeader("Kunder efter navn");
            Console.Write("Indtast navn: ");
            var name = Console.ReadLine() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(name))
            {
                ShowError("Navn kan ikke være tomt");
                return;
            }

            var customers = await _customerService.SearchCustomersByNameAsync(name);
            DisplayCustomers(customers);
        }

        private async Task ShowCustomersByPhone()
        {
            ShowHeader("Kunder efter telefon");
            Console.Write("Indtast telefonnummer: ");
            var phone = Console.ReadLine() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(phone))
            {
                ShowError("Telefonnummer kan ikke være tomt");
                return;
            }

            var customers = await _customerService.GetCustomersByPhoneAsync(phone);
            DisplayCustomers(customers);
        }

        private async Task ShowCustomersByAddress()
        {
            ShowHeader("Kunder efter adresse");
            Console.Write("Indtast adresse: ");
            var address = Console.ReadLine() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(address))
            {
                ShowError("Adresse kan ikke være tom");
                return;
            }

            var customers = await _customerService.GetCustomersByAddressAsync(address);
            DisplayCustomers(customers);
        }

        private async Task ShowCustomersByPostalCode()
        {
            ShowHeader("Kunder efter postnummer");
            Console.Write("Indtast postnummer: ");
            var postalCode = Console.ReadLine() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(postalCode))
            {
                ShowError("Postnummer kan ikke være tomt");
                return;
            }

            var customers = await _customerService.GetCustomersByPostalCodeAsync(postalCode);
            DisplayCustomers(customers);
        }

        private async Task ShowCustomersByCity()
        {
            ShowHeader("Kunder efter by");
            Console.Write("Indtast by: ");
            var city = Console.ReadLine() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(city))
            {
                ShowError("By kan ikke være tom");
                return;
            }

            var customers = await _customerService.GetCustomersByCityAsync(city);
            DisplayCustomers(customers);
        }

        private async Task ShowMembers()
        {
            ShowHeader("Medlemmer");
            var customers = await _customerService.GetCustomersByMembershipStatusAsync(true);
            DisplayCustomers(customers);
        }

        private async Task ShowNonMembers()
        {
            ShowHeader("Ikke-medlemmer");
            var customers = await _customerService.GetCustomersByMembershipStatusAsync(false);
            DisplayCustomers(customers);
        }

        private async Task ShowCustomersByMembershipType()
        {
            ShowHeader("Kunder efter medlemskabstype");
            Console.Write("Indtast medlemskabstype: ");
            var membershipType = Console.ReadLine() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(membershipType))
            {
                ShowError("Medlemskabstype kan ikke være tom");
                return;
            }

            var customers = await _customerService.GetCustomersByMembershipTypeAsync(membershipType);
            DisplayCustomers(customers);
        }

        private async Task ShowCustomersByRegistrationDate()
        {
            ShowHeader("Kunder efter registreringsdato");
            Console.Write("Indtast startdato (dd/mm/yyyy): ");
            if (!DateTime.TryParse(Console.ReadLine(), out DateTime startDate))
            {
                ShowError("Ugyldig startdato");
                return;
            }

            Console.Write("Indtast slutdato (dd/mm/yyyy): ");
            if (!DateTime.TryParse(Console.ReadLine(), out DateTime endDate))
            {
                ShowError("Ugyldig slutdato");
                return;
            }

            var customers = await _customerService.GetCustomersByRegistrationDateRangeAsync(startDate, endDate);
            DisplayCustomers(customers);
        }

        private async Task ShowCustomersWithAdoptions()
        {
            ShowHeader("Kunder med adoptioner");
            var customers = await _customerService.GetCustomersWithAdoptionsAsync();
            DisplayCustomers(customers);
        }

        private async Task CreateNewCustomer()
        {
            ShowHeader("Opret ny kunde");

            Console.Write("Indtast fornavn: ");
            var firstName = Console.ReadLine() ?? string.Empty;

            Console.Write("Indtast efternavn: ");
            var lastName = Console.ReadLine() ?? string.Empty;

            Console.Write("Indtast email: ");
            var email = Console.ReadLine() ?? string.Empty;

            Console.Write("Indtast telefon: ");
            var phone = Console.ReadLine() ?? string.Empty;

            Console.Write("Indtast adresse: ");
            var address = Console.ReadLine() ?? string.Empty;

            Console.Write("Indtast postnummer: ");
            var postalCode = Console.ReadLine() ?? string.Empty;

            Console.Write("Indtast by: ");
            var city = Console.ReadLine() ?? string.Empty;

            var customer = new Customer
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                Phone = phone,
                Address = address,
                PostalCode = postalCode,
                City = city,
                RegistrationDate = DateTime.Now,
                IsMember = false
            };

            await _customerService.CreateCustomerAsync(customer);
            ShowSuccess("Kunde oprettet succesfuldt!");
        }

        private async Task UpdateCustomer()
        {
            ShowHeader("Opdater kunde");

            Console.Write("Indtast kunde ID: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                ShowError("Ugyldigt ID");
                return;
            }

            var customer = await _customerService.GetCustomerByIdAsync(id);
            if (customer == null)
            {
                ShowError("Kunde ikke fundet");
                return;
            }

            Console.WriteLine("\nNuværende information:");
            DisplayCustomerInfo(customer);
            Console.WriteLine("\nIndtast ny information (tryk Enter for at beholde nuværende værdi):");

            Console.Write($"Fornavn [{customer.FirstName}]: ");
            var firstName = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(firstName))
                customer.FirstName = firstName;

            Console.Write($"Efternavn [{customer.LastName}]: ");
            var lastName = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(lastName))
                customer.LastName = lastName;

            Console.Write($"Email [{customer.Email}]: ");
            var email = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(email))
                customer.Email = email;

            Console.Write($"Telefon [{customer.Phone}]: ");
            var phone = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(phone))
                customer.Phone = phone;

            Console.Write($"Adresse [{customer.Address}]: ");
            var address = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(address))
                customer.Address = address;

            Console.Write($"Postnummer [{customer.PostalCode}]: ");
            var postalCode = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(postalCode))
                customer.PostalCode = postalCode;

            Console.Write($"By [{customer.City}]: ");
            var city = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(city))
                customer.City = city;

            await _customerService.UpdateCustomerAsync(customer);
            ShowSuccess("Kunde opdateret succesfuldt!");
        }

        private async Task DeleteCustomer()
        {
            ShowHeader("Slet kunde");

            Console.Write("Indtast kunde ID: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                ShowError("Ugyldigt ID");
                return;
            }

            var customer = await _customerService.GetCustomerByIdAsync(id);
            if (customer == null)
            {
                ShowError("Kunde ikke fundet");
                return;
            }

            Console.WriteLine("\nEr du sikker på, at du vil slette denne kunde?");
            DisplayCustomerInfo(customer);
            Console.Write("\nSkriv 'JA' for at bekræfte: ");
            
            if (Console.ReadLine()?.ToUpper() != "JA")
            {
                Console.WriteLine("Sletning annulleret.");
                Console.WriteLine("\nTryk på en tast for at fortsætte...");
                Console.ReadKey();
                return;
            }

            await _customerService.DeleteCustomerAsync(id);
            ShowSuccess("Kunde slettet succesfuldt!");
        }

        private async Task AddMembership()
        {
            ShowHeader("Tilføj medlemskab");

            Console.Write("Indtast kunde ID: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                ShowError("Ugyldigt ID");
                return;
            }

            Console.Write("Indtast medlemskabstype: ");
            var membershipType = Console.ReadLine() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(membershipType))
            {
                ShowError("Medlemskabstype kan ikke være tom");
                return;
            }

            await _customerService.AddMembershipAsync(id, membershipType);
            ShowSuccess("Medlemskab tilføjet succesfuldt!");
        }

        private async Task RemoveMembership()
        {
            ShowHeader("Fjern medlemskab");

            Console.Write("Indtast kunde ID: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                ShowError("Ugyldigt ID");
                return;
            }

            await _customerService.RemoveMembershipAsync(id);
            ShowSuccess("Medlemskab fjernet succesfuldt!");
        }

        private async Task UpdateMembershipType()
        {
            ShowHeader("Opdater medlemskabstype");

            Console.Write("Indtast kunde ID: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                ShowError("Ugyldigt ID");
                return;
            }

            Console.Write("Indtast ny medlemskabstype: ");
            var membershipType = Console.ReadLine() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(membershipType))
            {
                ShowError("Medlemskabstype kan ikke være tom");
                return;
            }

            await _customerService.UpdateMembershipTypeAsync(id, membershipType);
            ShowSuccess("Medlemskabstype opdateret succesfuldt!");
        }

        private void DisplayCustomers(IEnumerable<Customer> customers)
        {
            if (!customers.Any())
            {
                Console.WriteLine("Ingen kunder fundet.");
            }
            else
            {
                foreach (var customer in customers)
                {
                    DisplayCustomerInfo(customer);
                    Console.WriteLine(new string('-', 50));
                }
            }

            Console.WriteLine("\nTryk på en tast for at fortsætte...");
            Console.ReadKey();
        }

        private void DisplayCustomerInfo(Customer customer)
        {
            Console.WriteLine($"ID: {customer.Id}");
            Console.WriteLine($"Navn: {customer.Name}");
            Console.WriteLine($"Email: {customer.Email}");
            Console.WriteLine($"Telefon: {customer.Phone}");
            Console.WriteLine($"Adresse: {customer.Address}");
            Console.WriteLine($"Postnummer: {customer.PostalCode}");
            Console.WriteLine($"By: {customer.City}");
            Console.WriteLine($"Registreringsdato: {customer.RegistrationDate:dd/MM/yyyy}");
            Console.WriteLine($"Medlem: {(customer.IsMember ? "Ja" : "Nej")}");
            if (customer.IsMember)
            {
                Console.WriteLine($"Medlemskabstype: {customer.MembershipType}");
            }
            if (customer.Adoptions.Any())
            {
                Console.WriteLine("Adoptioner:");
                foreach (var adoption in customer.Adoptions)
                {
                    if (adoption.Animal != null)
                    {
                        Console.WriteLine($"  - {adoption.Animal.Name} ({adoption.AdoptionDate:dd/MM/yyyy})");
                    }
                    else
                    {
                        Console.WriteLine($"  - Ukendt dyr ({adoption.AdoptionDate:dd/MM/yyyy})");
                    }
                }
            }
        }
    }
}
