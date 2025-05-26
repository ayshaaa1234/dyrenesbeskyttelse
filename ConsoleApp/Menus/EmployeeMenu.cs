using System;
using System.Linq;
using System.Threading.Tasks;
using ClassLibrary.Features.Employees.Application.Abstractions;
using ClassLibrary.Features.Employees.Core.Models;

namespace ConsoleApp.Menus
{
    public class EmployeeMenu
    {
        private readonly IEmployeeService _employeeService;

        public EmployeeMenu(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        public async Task ShowAsync()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Medarbejderadministration");
                Console.WriteLine("----------------------------------------");
                Console.WriteLine("1. Vis alle medarbejdere");
                Console.WriteLine("2. Tilføj ny medarbejder");
                Console.WriteLine("3. Opdater medarbejder");
                Console.WriteLine("4. Slet medarbejder");
                Console.WriteLine("0. Tilbage til hovedmenu");
                Console.WriteLine("----------------------------------------");
                Console.Write("Tag et valg: ");

                string? choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        await ListAllEmployeesAsync();
                        break;
                    case "2":
                        await AddEmployeeAsync();
                        break;
                    case "3":
                        await UpdateEmployeeAsync();
                        break;
                    case "4":
                        await DeleteEmployeeAsync();
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

        private async Task ListAllEmployeesAsync()
        {
            Console.Clear();
            Console.WriteLine("Alle registrerede medarbejdere:");
            var employees = await _employeeService.GetAllAsync();
            if (!employees.Any())
            {
                Console.WriteLine("Ingen medarbejdere fundet.");
            }
            else
            {
                foreach (var employee in employees)
                {
                    Console.WriteLine($"- ID: {employee.Id}, Navn: {employee.FirstName} {employee.LastName}, Stilling: {employee.Position}");
                }
            }
            Console.WriteLine("Tryk på en tast for at fortsætte...");
            Console.ReadKey();
        }

        private async Task AddEmployeeAsync()
        {
            Console.Clear();
            Console.WriteLine("Tilføj ny medarbejder");

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

            Console.Write("Stilling: ");
            string? position = Console.ReadLine();
            while (string.IsNullOrWhiteSpace(position))
            {
                Console.WriteLine("Stilling må ikke være tom. Prøv igen.");
                Console.Write("Stilling: ");
                position = Console.ReadLine();
            }

            Console.Write("Afdeling: ");
            string? department = Console.ReadLine();

            Console.Write("Billed-URL (valgfri): ");
            string? pictureUrl = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(pictureUrl)) pictureUrl = null;
            
            Console.Write("Specialiseringer (kommasepareret, valgfri): ");
            string? specializationsInput = Console.ReadLine();
            List<string> specializations = string.IsNullOrWhiteSpace(specializationsInput) 
                ? new List<string>() 
                : specializationsInput.Split(',').Select(s => s.Trim()).ToList();

            var newEmployee = new Employee()
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                Phone = phoneNumber ?? string.Empty,
                Position = position,
                Department = department ?? string.Empty,
                PictureUrl = pictureUrl,
                Specializations = specializations
            };

            try
            {
                var createdEmployee = await _employeeService.CreateAsync(newEmployee);
                Console.WriteLine($"Medarbejder '{createdEmployee.FirstName} {createdEmployee.LastName}' blev tilføjet med ID: {createdEmployee.Id}.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fejl ved tilføjelse af medarbejder: {ex.Message}");
            }

            Console.WriteLine("Tryk på en tast for at fortsætte...");
            Console.ReadKey();
        }

        private async Task UpdateEmployeeAsync()
        {
            Console.Clear();
            Console.WriteLine("Opdater medarbejder");
            Console.Write("Indtast ID på medarbejderen der skal opdateres: ");
            string? idInput = Console.ReadLine();
            if (!int.TryParse(idInput, out int employeeId))
            {
                Console.WriteLine("Ugyldigt ID format. ID skal være et heltal.");
                Console.ReadKey();
                return;
            }

            var employeeToUpdate = await _employeeService.GetByIdAsync(employeeId);
            if (employeeToUpdate == null)
            {
                Console.WriteLine($"Medarbejder med ID {employeeId} blev ikke fundet.");
                Console.ReadKey();
                return;
            }

            Console.WriteLine($"Opdaterer medarbejder: {employeeToUpdate.FirstName} {employeeToUpdate.LastName} (ID: {employeeToUpdate.Id})");
            Console.WriteLine($"Nuværende værdier vises i parentes. Tryk Enter for at beholde nuværende værdi.");

            Console.Write($"Fornavn ({employeeToUpdate.FirstName}): ");
            string? firstName = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(firstName)) employeeToUpdate.FirstName = firstName;

            Console.Write($"Efternavn ({employeeToUpdate.LastName}): ");
            string? lastName = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(lastName)) employeeToUpdate.LastName = lastName;

            Console.Write($"Email ({employeeToUpdate.Email}): ");
            string? email = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(email) && email.Contains("@")) employeeToUpdate.Email = email;
            else if (!string.IsNullOrWhiteSpace(email) && !email.Contains("@")) Console.WriteLine("Email ikke opdateret - ugyldigt format.");

            Console.Write($"Telefonnummer ({employeeToUpdate.Phone}): ");
            string? phoneNumber = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(phoneNumber)) employeeToUpdate.Phone = phoneNumber;

            Console.Write($"Stilling ({employeeToUpdate.Position}): ");
            string? position = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(position)) employeeToUpdate.Position = position;

            Console.Write($"Afdeling ({employeeToUpdate.Department}): ");
            string? department = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(department)) employeeToUpdate.Department = department;
            
            Console.Write($"Billed-URL ({employeeToUpdate.PictureUrl ?? "Ingen"}): ");
            string? pictureUrl = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(pictureUrl)) 
            {
                employeeToUpdate.PictureUrl = pictureUrl;
            } else if (pictureUrl == "") {
                 // Behold eksisterende hvis tom streng og der var en værdi
            } else {
                 employeeToUpdate.PictureUrl = null; 
            }
            
            Console.Write($"Specialiseringer (nuværende: {string.Join(", ", employeeToUpdate.Specializations ?? new List<string>())}). Indtast nye kommaseparerede værdier, eller lad stå tom for ingen ændring: ");
            string? specializationsInput = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(specializationsInput))
            {
                employeeToUpdate.Specializations = specializationsInput.Split(',').Select(s => s.Trim()).ToList();
            }

            try
            {
                await _employeeService.UpdateAsync(employeeToUpdate);
                Console.WriteLine($"Medarbejder '{employeeToUpdate.FirstName} {employeeToUpdate.LastName}' blev opdateret.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fejl ved opdatering af medarbejder: {ex.Message}");
            }

            Console.WriteLine("Tryk på en tast for at fortsætte...");
            Console.ReadKey();
        }

        private async Task DeleteEmployeeAsync()
        {
            Console.Clear();
            Console.WriteLine("Slet medarbejder");
            Console.Write("Indtast ID på medarbejderen der skal slettes: ");
            string? idInput = Console.ReadLine();
            if (!int.TryParse(idInput, out int employeeId))
            {
                Console.WriteLine("Ugyldigt ID format. ID skal være et heltal.");
                Console.ReadKey();
                return;
            }

            var employeeToDelete = await _employeeService.GetByIdAsync(employeeId);
            if (employeeToDelete == null)
            {
                Console.WriteLine($"Medarbejder med ID {employeeId} blev ikke fundet.");
                Console.ReadKey();
                return;
            }

            Console.WriteLine($"Er du sikker på, at du vil slette {employeeToDelete.FirstName} {employeeToDelete.LastName} (ID: {employeeToDelete.Id})? (j/n)");
            string? confirmation = Console.ReadLine();
            if (confirmation?.ToLower() == "j")
            {
                try
                {
                    await _employeeService.DeleteAsync(employeeId);
                    Console.WriteLine($"Medarbejder '{employeeToDelete.FirstName} {employeeToDelete.LastName}' blev slettet.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Fejl ved sletning af medarbejder: {ex.Message}");
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