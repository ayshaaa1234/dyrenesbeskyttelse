using System;
using System.Linq;
using System.Threading.Tasks;
using ClassLibrary.Features.Customers.Application.Abstractions;
using ClassLibrary.Features.Customers.Core.Models;

namespace ConsoleApp.Menus
{
    public class CustomerMenu
    {
        private readonly ICustomerService _customerService;

        public CustomerMenu(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        public async Task ShowAsync()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Kundeadministration");
                Console.WriteLine("----------------------------------------");
                Console.WriteLine("1. Vis alle kunder");
                Console.WriteLine("2. Tilføj ny kunde");
                Console.WriteLine("3. Opdater kunde");
                Console.WriteLine("4. Slet kunde");
                Console.WriteLine("0. Tilbage til hovedmenu");
                Console.WriteLine("----------------------------------------");
                Console.Write("Tag et valg: ");

                string? choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        await ListAllCustomersAsync();
                        break;
                    case "2":
                        await AddCustomerAsync();
                        break;
                    case "3":
                        await UpdateCustomerAsync();
                        break;
                    case "4":
                        await DeleteCustomerAsync();
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

        private async Task ListAllCustomersAsync()
        {
            Console.Clear();
            Console.WriteLine("Alle registrerede kunder:");
            var customers = await _customerService.GetAllAsync();
            if (!customers.Any())
            {
                Console.WriteLine("Ingen kunder fundet.");
            }
            else
            {
                foreach (var customer in customers)
                {
                    Console.WriteLine($"- ID: {customer.Id}, Navn: {customer.FirstName} {customer.LastName}, Email: {customer.Email}");
                }
            }
            Console.WriteLine("Tryk på en tast for at fortsætte...");
            Console.ReadKey();
        }

        private async Task AddCustomerAsync()
        {
            Console.Clear();
            Console.WriteLine("Tilføj ny kunde");

            Console.Write("Fornavn: ");
            string? firstName = Console.ReadLine();
            while (string.IsNullOrWhiteSpace(firstName))
            {
                Console.WriteLine("Fornavn må ikke være tomt. Prøv igen.");
                Console.Write("Fornavn: ");
                firstName = Console.ReadLine();
            }

            Console.Write("Efternavn: ");
            string? lastName = Console.ReadLine();
            while (string.IsNullOrWhiteSpace(lastName))
            {
                Console.WriteLine("Efternavn må ikke være tomt. Prøv igen.");
                Console.Write("Efternavn: ");
                lastName = Console.ReadLine();
            }

            Console.Write("Email: ");
            string? email = Console.ReadLine();
            while (string.IsNullOrWhiteSpace(email) || !email.Contains("@"))
            {
                Console.WriteLine("Ugyldig email. Email må ikke være tom og skal indeholde '@'. Prøv igen.");
                Console.Write("Email: ");
                email = Console.ReadLine();
            }

            Console.Write("Telefonnummer: ");
            string? phoneNumber = Console.ReadLine();

            Console.Write("Adresse: ");
            string? address = Console.ReadLine();
            
            Console.Write("Postnummer: ");
            string? postalCode = Console.ReadLine();
            
            Console.Write("By: ");
            string? city = Console.ReadLine();

            var newCustomer = new Customer()
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                Phone = phoneNumber ?? string.Empty,
                Address = address ?? string.Empty,
                PostalCode = postalCode ?? string.Empty,
                City = city ?? string.Empty
            };

            try
            {
                var createdCustomer = await _customerService.CreateAsync(newCustomer);
                Console.WriteLine($"Kunde '{createdCustomer.FirstName} {createdCustomer.LastName}' blev tilføjet med ID: {createdCustomer.Id}.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fejl ved tilføjelse af kunde: {ex.Message}");
            }

            Console.WriteLine("Tryk på en tast for at fortsætte...");
            Console.ReadKey();
        }

        private async Task UpdateCustomerAsync()
        {
            Console.Clear();
            Console.WriteLine("Opdater kunde");
            Console.Write("Indtast ID på kunden der skal opdateres: ");
            string? idInput = Console.ReadLine();
            if (!int.TryParse(idInput, out int customerId))
            {
                Console.WriteLine("Ugyldigt ID format. ID skal være et heltal.");
                Console.ReadKey();
                return;
            }

            var customerToUpdate = await _customerService.GetByIdAsync(customerId);
            if (customerToUpdate == null)
            {
                Console.WriteLine($"Kunde med ID {customerId} blev ikke fundet.");
                Console.ReadKey();
                return;
            }

            Console.WriteLine($"Opdaterer kunde: {customerToUpdate.FirstName} {customerToUpdate.LastName} (ID: {customerToUpdate.Id})");
            Console.WriteLine($"Nuværende værdier vises i parentes. Tryk Enter for at beholde nuværende værdi.");

            Console.Write($"Fornavn ({customerToUpdate.FirstName}): ");
            string? firstName = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(firstName)) customerToUpdate.FirstName = firstName;

            Console.Write($"Efternavn ({customerToUpdate.LastName}): ");
            string? lastName = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(lastName)) customerToUpdate.LastName = lastName;

            Console.Write($"Email ({customerToUpdate.Email}): ");
            string? email = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(email) && email.Contains("@")) customerToUpdate.Email = email;
            else if (!string.IsNullOrWhiteSpace(email) && !email.Contains("@")) Console.WriteLine("Email ikke opdateret - ugyldigt format.");

            Console.Write($"Telefonnummer ({customerToUpdate.Phone}): ");
            string? phoneNumber = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(phoneNumber)) customerToUpdate.Phone = phoneNumber;

            Console.Write($"Adresse ({customerToUpdate.Address}): ");
            string? address = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(address)) customerToUpdate.Address = address;
            
            Console.Write($"Postnummer ({customerToUpdate.PostalCode}): ");
            string? postalCode = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(postalCode)) customerToUpdate.PostalCode = postalCode;
            
            Console.Write($"By ({customerToUpdate.City}): ");
            string? city = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(city)) customerToUpdate.City = city;

            try
            {
                await _customerService.UpdateAsync(customerToUpdate);
                Console.WriteLine($"Kunde '{customerToUpdate.FirstName} {customerToUpdate.LastName}' blev opdateret.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fejl ved opdatering af kunde: {ex.Message}");
            }

            Console.WriteLine("Tryk på en tast for at fortsætte...");
            Console.ReadKey();
        }

        private async Task DeleteCustomerAsync()
        {
            Console.Clear();
            Console.WriteLine("Slet kunde");
            Console.Write("Indtast ID på kunden der skal slettes: ");
            string? idInput = Console.ReadLine();
            if (!int.TryParse(idInput, out int customerId))
            {
                Console.WriteLine("Ugyldigt ID format. ID skal være et heltal.");
                Console.ReadKey();
                return;
            }

            var customerToDelete = await _customerService.GetByIdAsync(customerId);
            if (customerToDelete == null)
            {
                Console.WriteLine($"Kunde med ID {customerId} blev ikke fundet.");
                Console.ReadKey();
                return;
            }

            Console.WriteLine($"Er du sikker på, at du vil slette {customerToDelete.FirstName} {customerToDelete.LastName} (ID: {customerToDelete.Id})? (j/n)");
            string? confirmation = Console.ReadLine();
            if (confirmation?.ToLower() == "j")
            {
                try
                {
                    await _customerService.DeleteAsync(customerId);
                    Console.WriteLine($"Kunde '{customerToDelete.FirstName} {customerToDelete.LastName}' blev slettet.");
                }
                catch (InvalidOperationException ex)
                {
                    Console.WriteLine($"Fejl ved sletning af kunde: {ex.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"En uventet fejl opstod ved sletning af kunde: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("Sletning annulleret.");
            }

            Console.WriteLine("Tryk på en tast for at fortsætte...");
            Console.ReadKey();
        }
    }
} 