using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using ClassLibrary.Services;
using ClassLibrary.Models;

namespace ConsoleApp.Menus
{
    /// <summary>
    /// Menu til håndtering af medarbejdere
    /// </summary>
    public class EmployeeMenu : MenuBase
    {
        private readonly EmployeeService _employeeService;

        public EmployeeMenu(EmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        public override async Task ShowAsync()
        {
            while (true)
            {
                ShowHeader("Medarbejder");
                Console.WriteLine("1. Vis alle medarbejdere");
                Console.WriteLine("2. Vis medarbejder efter ID");
                Console.WriteLine("3. Vis medarbejder efter email");
                Console.WriteLine("4. Vis medarbejdere efter navn");
                Console.WriteLine("5. Vis medarbejdere efter telefon");
                Console.WriteLine("6. Vis medarbejdere efter stilling");
                Console.WriteLine("7. Vis medarbejdere efter afdeling");
                Console.WriteLine("8. Vis medarbejdere efter ansættelsesdato");
                Console.WriteLine("9. Vis aktive medarbejdere");
                Console.WriteLine("10. Vis inaktive medarbejdere");
                Console.WriteLine("11. Vis medarbejdere efter specialisering");
                Console.WriteLine("12. Vis medarbejdere efter løninterval");
                Console.WriteLine("13. Opret ny medarbejder");
                Console.WriteLine("14. Opdater medarbejder");
                Console.WriteLine("15. Slet medarbejder");
                Console.WriteLine("16. Aktiver medarbejder");
                Console.WriteLine("17. Deaktiver medarbejder");
                Console.WriteLine("18. Opdater løn");
                Console.WriteLine("19. Tilføj specialisering");
                Console.WriteLine("20. Fjern specialisering");
                Console.WriteLine("0. Tilbage til hovedmenu");
                Console.Write("\nVælg en mulighed: ");

                var choice = Console.ReadLine();

                try
                {
                    switch (choice)
                    {
                        case "1":
                            await ShowAllEmployees();
                            break;
                        case "2":
                            await ShowEmployeeById();
                            break;
                        case "3":
                            await ShowEmployeeByEmail();
                            break;
                        case "4":
                            await ShowEmployeesByName();
                            break;
                        case "5":
                            await ShowEmployeesByPhone();
                            break;
                        case "6":
                            await ShowEmployeesByPosition();
                            break;
                        case "7":
                            await ShowEmployeesByDepartment();
                            break;
                        case "8":
                            await ShowEmployeesByHireDate();
                            break;
                        case "9":
                            await ShowActiveEmployees();
                            break;
                        case "10":
                            await ShowInactiveEmployees();
                            break;
                        case "11":
                            await ShowEmployeesBySpecialization();
                            break;
                        case "12":
                            await ShowEmployeesBySalaryRange();
                            break;
                        case "13":
                            await CreateNewEmployee();
                            break;
                        case "14":
                            await UpdateEmployee();
                            break;
                        case "15":
                            await DeleteEmployee();
                            break;
                        case "16":
                            await ActivateEmployee();
                            break;
                        case "17":
                            await DeactivateEmployee();
                            break;
                        case "18":
                            await UpdateSalary();
                            break;
                        case "19":
                            await AddSpecialization();
                            break;
                        case "20":
                            await RemoveSpecialization();
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

        private async Task ShowAllEmployees()
        {
            ShowHeader("Alle medarbejdere");
            var employees = await _employeeService.GetAllEmployeesAsync();
            DisplayEmployees(employees);
        }

        private async Task ShowEmployeeById()
        {
            ShowHeader("Medarbejder efter ID");
            Console.Write("Indtast medarbejder ID: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                ShowError("Ugyldigt ID");
                return;
            }

            var employee = await _employeeService.GetEmployeeByIdAsync(id);
            DisplayEmployeeInfo(employee);
        }

        private async Task ShowEmployeeByEmail()
        {
            ShowHeader("Medarbejder efter email");
            Console.Write("Indtast email: ");
            var email = Console.ReadLine() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(email))
            {
                ShowError("Email kan ikke være tom");
                return;
            }

            var employee = await _employeeService.GetEmployeeByEmailAsync(email);
            DisplayEmployeeInfo(employee);
        }

        private async Task ShowEmployeesByName()
        {
            ShowHeader("Medarbejdere efter navn");
            Console.Write("Indtast navn: ");
            var name = Console.ReadLine() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(name))
            {
                ShowError("Navn kan ikke være tomt");
                return;
            }

            var employees = await _employeeService.SearchEmployeesByNameAsync(name);
            DisplayEmployees(employees);
        }

        private async Task ShowEmployeesByPhone()
        {
            ShowHeader("Medarbejdere efter telefon");
            Console.Write("Indtast telefonnummer: ");
            var phone = Console.ReadLine() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(phone))
            {
                ShowError("Telefonnummer kan ikke være tomt");
                return;
            }

            var employees = await _employeeService.GetEmployeesByPhoneAsync(phone);
            DisplayEmployees(employees);
        }

        private async Task ShowEmployeesByPosition()
        {
            ShowHeader("Medarbejdere efter stilling");
            Console.Write("Indtast stilling: ");
            var position = Console.ReadLine() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(position))
            {
                ShowError("Stilling kan ikke være tom");
                return;
            }

            var employees = await _employeeService.GetEmployeesByPositionAsync(position);
            DisplayEmployees(employees);
        }

        private async Task ShowEmployeesByDepartment()
        {
            ShowHeader("Medarbejdere efter afdeling");
            Console.Write("Indtast afdeling: ");
            var department = Console.ReadLine() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(department))
            {
                ShowError("Afdeling kan ikke være tom");
                return;
            }

            var employees = await _employeeService.GetEmployeesByDepartmentAsync(department);
            DisplayEmployees(employees);
        }

        private async Task ShowEmployeesByHireDate()
        {
            ShowHeader("Medarbejdere efter ansættelsesdato");
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

            var employees = await _employeeService.GetEmployeesByHireDateRangeAsync(startDate, endDate);
            DisplayEmployees(employees);
        }

        private async Task ShowActiveEmployees()
        {
            ShowHeader("Aktive medarbejdere");
            var employees = await _employeeService.GetActiveEmployeesAsync();
            DisplayEmployees(employees);
        }

        private async Task ShowInactiveEmployees()
        {
            ShowHeader("Inaktive medarbejdere");
            var employees = await _employeeService.GetEmployeesByStatusAsync(false);
            DisplayEmployees(employees);
        }

        private async Task ShowEmployeesBySpecialization()
        {
            ShowHeader("Medarbejdere efter specialisering");
            Console.Write("Indtast specialisering: ");
            var specialization = Console.ReadLine() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(specialization))
            {
                ShowError("Specialisering kan ikke være tom");
                return;
            }

            var employees = await _employeeService.GetEmployeesBySpecializationAsync(specialization);
            DisplayEmployees(employees);
        }

        private async Task ShowEmployeesBySalaryRange()
        {
            ShowHeader("Medarbejdere efter løninterval");
            Console.Write("Indtast minimumsløn: ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal minSalary))
            {
                ShowError("Ugyldig minimumsløn");
                return;
            }

            Console.Write("Indtast maksimumsløn: ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal maxSalary))
            {
                ShowError("Ugyldig maksimumsløn");
                return;
            }

            var employees = await _employeeService.GetEmployeesBySalaryRangeAsync(minSalary, maxSalary);
            DisplayEmployees(employees);
        }

        private async Task CreateNewEmployee()
        {
            ShowHeader("Opret ny medarbejder");

            Console.Write("Indtast fornavn: ");
            var firstName = Console.ReadLine() ?? string.Empty;

            Console.Write("Indtast efternavn: ");
            var lastName = Console.ReadLine() ?? string.Empty;

            Console.Write("Indtast email: ");
            var email = Console.ReadLine() ?? string.Empty;

            Console.Write("Indtast telefon: ");
            var phone = Console.ReadLine() ?? string.Empty;

            Console.Write("Indtast stilling: ");
            var position = Console.ReadLine() ?? string.Empty;

            Console.Write("Indtast afdeling: ");
            var department = Console.ReadLine() ?? string.Empty;

            Console.Write("Indtast specialisering: ");
            var specialization = Console.ReadLine() ?? string.Empty;

            Console.Write("Indtast løn: ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal salary))
            {
                ShowError("Ugyldig løn");
                return;
            }

            Console.Write("Indtast ansættelsesdato (dd/mm/yyyy): ");
            if (!DateTime.TryParse(Console.ReadLine(), out DateTime hireDate))
            {
                ShowError("Ugyldig dato");
                return;
            }

            var employee = new Employee
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                Phone = phone,
                Position = position,
                Department = department,
                Specialization = specialization,
                Salary = salary,
                HireDate = hireDate,
                IsActive = true
            };

            await _employeeService.CreateEmployeeAsync(employee);
            ShowSuccess("Medarbejder oprettet succesfuldt!");
        }

        private async Task UpdateEmployee()
        {
            ShowHeader("Opdater medarbejder");

            Console.Write("Indtast medarbejder ID: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                ShowError("Ugyldigt ID");
                return;
            }

            var employee = await _employeeService.GetEmployeeByIdAsync(id);
            if (employee == null)
            {
                ShowError("Medarbejder ikke fundet");
                return;
            }

            Console.WriteLine("\nNuværende information:");
            DisplayEmployeeInfo(employee);
            Console.WriteLine("\nIndtast ny information (tryk Enter for at beholde nuværende værdi):");

            Console.Write($"Fornavn [{employee.FirstName}]: ");
            var firstName = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(firstName))
                employee.FirstName = firstName;

            Console.Write($"Efternavn [{employee.LastName}]: ");
            var lastName = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(lastName))
                employee.LastName = lastName;

            Console.Write($"Email [{employee.Email}]: ");
            var email = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(email))
                employee.Email = email;

            Console.Write($"Telefon [{employee.Phone}]: ");
            var phone = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(phone))
                employee.Phone = phone;

            Console.Write($"Stilling [{employee.Position}]: ");
            var position = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(position))
                employee.Position = position;

            Console.Write($"Afdeling [{employee.Department}]: ");
            var department = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(department))
                employee.Department = department;

            Console.Write($"Specialisering [{employee.Specialization}]: ");
            var specialization = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(specialization))
                employee.Specialization = specialization;

            Console.Write($"Løn [{employee.Salary}]: ");
            var salaryStr = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(salaryStr) && decimal.TryParse(salaryStr, out decimal salary))
                employee.Salary = salary;

            await _employeeService.UpdateEmployeeAsync(employee);
            ShowSuccess("Medarbejder opdateret succesfuldt!");
        }

        private async Task DeleteEmployee()
        {
            ShowHeader("Slet medarbejder");

            Console.Write("Indtast medarbejder ID: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                ShowError("Ugyldigt ID");
                return;
            }

            var employee = await _employeeService.GetEmployeeByIdAsync(id);
            if (employee == null)
            {
                ShowError("Medarbejder ikke fundet");
                return;
            }

            Console.WriteLine("\nEr du sikker på, at du vil slette denne medarbejder?");
            DisplayEmployeeInfo(employee);
            Console.Write("\nSkriv 'JA' for at bekræfte: ");
            
            if (Console.ReadLine()?.ToUpper() != "JA")
            {
                Console.WriteLine("Sletning annulleret.");
                Console.WriteLine("\nTryk på en tast for at fortsætte...");
                Console.ReadKey();
                return;
            }

            await _employeeService.DeleteEmployeeAsync(id);
            ShowSuccess("Medarbejder slettet succesfuldt!");
        }

        private async Task ActivateEmployee()
        {
            ShowHeader("Aktiver medarbejder");

            Console.Write("Indtast medarbejder ID: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                ShowError("Ugyldigt ID");
                return;
            }

            await _employeeService.ActivateEmployeeAsync(id);
            ShowSuccess("Medarbejder aktiveret succesfuldt!");
        }

        private async Task DeactivateEmployee()
        {
            ShowHeader("Deaktiver medarbejder");

            Console.Write("Indtast medarbejder ID: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                ShowError("Ugyldigt ID");
                return;
            }

            await _employeeService.DeactivateEmployeeAsync(id);
            ShowSuccess("Medarbejder deaktiveret succesfuldt!");
        }

        private async Task UpdateSalary()
        {
            ShowHeader("Opdater løn");

            Console.Write("Indtast medarbejder ID: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                ShowError("Ugyldigt ID");
                return;
            }

            Console.Write("Indtast ny løn: ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal newSalary))
            {
                ShowError("Ugyldig løn");
                return;
            }

            await _employeeService.UpdateEmployeeSalaryAsync(id, newSalary);
            ShowSuccess("Løn opdateret succesfuldt!");
        }

        private async Task AddSpecialization()
        {
            ShowHeader("Tilføj specialisering");

            Console.Write("Indtast medarbejder ID: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                ShowError("Ugyldigt ID");
                return;
            }

            Console.Write("Indtast specialisering: ");
            var specialization = Console.ReadLine() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(specialization))
            {
                ShowError("Specialisering kan ikke være tom");
                return;
            }

            await _employeeService.AddSpecializationAsync(id, specialization);
            ShowSuccess("Specialisering tilføjet succesfuldt!");
        }

        private async Task RemoveSpecialization()
        {
            ShowHeader("Fjern specialisering");

            Console.Write("Indtast medarbejder ID: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                ShowError("Ugyldigt ID");
                return;
            }

            Console.Write("Indtast specialisering: ");
            var specialization = Console.ReadLine() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(specialization))
            {
                ShowError("Specialisering kan ikke være tom");
                return;
            }

            await _employeeService.RemoveSpecializationAsync(id, specialization);
            ShowSuccess("Specialisering fjernet succesfuldt!");
        }

        private void DisplayEmployees(IEnumerable<Employee> employees)
        {
            if (!employees.Any())
            {
                Console.WriteLine("Ingen medarbejdere fundet.");
            }
            else
            {
                foreach (var employee in employees)
                {
                    DisplayEmployeeInfo(employee);
                    Console.WriteLine(new string('-', 50));
                }
            }

            Console.WriteLine("\nTryk på en tast for at fortsætte...");
            Console.ReadKey();
        }

        private void DisplayEmployeeInfo(Employee employee)
        {
            Console.WriteLine($"ID: {employee.Id}");
            Console.WriteLine($"Navn: {employee.Name}");
            Console.WriteLine($"Email: {employee.Email}");
            Console.WriteLine($"Telefon: {employee.Phone}");
            Console.WriteLine($"Stilling: {employee.Position}");
            Console.WriteLine($"Afdeling: {employee.Department}");
            Console.WriteLine($"Specialisering: {employee.Specialization}");
            Console.WriteLine($"Løn: {employee.Salary:C}");
            Console.WriteLine($"Ansættelsesdato: {employee.HireDate:dd/MM/yyyy}");
            Console.WriteLine($"Status: {(employee.IsActive ? "Aktiv" : "Inaktiv")}");
            if (employee.Specializations.Any())
            {
                Console.WriteLine("Specialiseringer:");
                foreach (var spec in employee.Specializations)
                {
                    Console.WriteLine($"  - {spec}");
                }
            }
        }
    }
}
