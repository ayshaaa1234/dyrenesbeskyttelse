using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClassLibrary.Features.Employees.Application.Abstractions;
using ClassLibrary.Features.Employees.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RazorPagesApp.Pages.Admin.Employees
{
    // TODO: Tilføj [Authorize(Roles = "Administrator,HRManager")] eller lignende for at begrænse adgang.
    public class IndexModel : PageModel
    {
        private readonly IEmployeeService _employeeService; // Service til håndtering af medarbejderdata.

        // Dependency Injection af IEmployeeService.
        public IndexModel(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        public IList<Employee> Employees { get; set; } = new List<Employee>(); // Liste over medarbejdere, der skal vises på siden.
        
        [TempData] // TempData bruges til at vise meddelelser efter en handling (f.eks. redirect).
        public string? Message { get; set; }
        [TempData]
        public string? MessageType { get; set; } // Bruges til at bestemme typen af alert (f.eks. "success", "danger").

        [BindProperty(SupportsGet = true)] // Binder SearchTerm til query-parametre i URL'en (for GET-requests).
        public string? SearchTerm { get; set; }

        [BindProperty] // Binder Employee til data sendt i POST-requests (f.eks. fra formularer).
        public Employee Employee { get; set; } = default!; // Initialiseres til default, da den typisk sættes i handlers.

        // Property til at modtage den kommaseparerede streng for specialiseringer fra formularen.
        [BindProperty]
        public string? SpecializationsString { get; set; }

        // Handler for GET-requests til siden (f.eks. når siden indlæses).
        public async Task<IActionResult> OnGetAsync()
        {            var employeesFromService = await _employeeService.GetAllAsync(); // Henter alle medarbejdere fra servicen.
            if (employeesFromService != null)
            {
                IEnumerable<Employee> filteredEmployees = employeesFromService;
                // Filtrerer medarbejdere baseret på SearchTerm, hvis det er angivet.
                if (!string.IsNullOrWhiteSpace(SearchTerm))
                {
                    string lowerSearchTerm = SearchTerm.ToLowerInvariant().Trim(); // Normaliserer søgetermen.
                    filteredEmployees = filteredEmployees.Where(e => 
                        (e.FirstName != null && e.FirstName.ToLowerInvariant().Contains(lowerSearchTerm)) || 
                        (e.LastName != null && e.LastName.ToLowerInvariant().Contains(lowerSearchTerm)) || 
                        (e.Email != null && e.Email.ToLowerInvariant().Contains(lowerSearchTerm))
                        // TODO: Overvej at udvide søgningen til Position og Department, hvis relevant.
                        // (e.Position != null && e.Position.ToLowerInvariant().Contains(lowerSearchTerm)) ||
                        // (e.Department != null && e.Department.ToLowerInvariant().Contains(lowerSearchTerm))
                    );
                }
                // Sorterer de filtrerede medarbejdere efter efternavn, derefter fornavn.
                Employees = filteredEmployees.OrderBy(e => e.LastName).ThenBy(e => e.FirstName).ToList();
            }
            return Page(); // Returnerer Razor Page.
        }

        // Handler for GET-request til at hente formularen for oprettelse af medarbejder (bruges til modal).
        public IActionResult OnGetCreateEmployeeForm()
        {
            // Initialiserer en ny Employee-model med standardværdier.
            Employee = new Employee 
            { 
                HireDate = System.DateTime.Today, // Sætter ansættelsesdato til i dag som standard.
                Specializations = new List<string>() // Initialiserer listen over specialiseringer.
            }; 
            return Partial("_EmployeeFormFields", Employee); // Returnerer _EmployeeFormFields.cshtml som en partial view.
        }

        // Handler for GET-request til at hente formularen for redigering af medarbejder (bruges til modal).
        public async Task<IActionResult> OnGetEditEmployeeFormAsync(int employeeId)
        {
            var employee = await _employeeService.GetByIdAsync(employeeId); // Henter medarbejderen baseret på ID.
            if (employee == null)
            {
                return NotFound(); // Returnerer 404 Not Found, hvis medarbejderen ikke findes.
            }
            Employee = employee;
            if(Employee.Specializations == null) Employee.Specializations = new List<string>(); // Sikrer at listen ikke er null.
            return Partial("_EmployeeFormFields", Employee); // Returnerer formularen med medarbejderens data.
        }

        // Handler for POST-request til at oprette en ny medarbejder (typisk kaldt via AJAX fra modalen).
        public async Task<IActionResult> OnPostCreateEmployeeAsync()
        {
            // Konverterer den kommaseparerede SpecializationsString til en List<string>.
            if (!string.IsNullOrWhiteSpace(SpecializationsString))
            {
                Employee.Specializations = SpecializationsString.Split(',').Select(s => s.Trim()).Where(s => !string.IsNullOrEmpty(s)).ToList();
            }
            else
            {
                Employee.Specializations = new List<string>();
            }

            if (!ModelState.IsValid) // Tjekker om modelvalideringen fejler.
            {
                // Returnerer formularen med valideringsfejl, så de kan vises i modalen.
                return Partial("_EmployeeFormFields", Employee); 
            }
            try
            {
                // TODO: Overvej om RegistrationDate skal sættes anderledes eller håndteres i servicen.
                Employee.RegistrationDate = Employee.HireDate; // Sætter RegistrationDate lig HireDate som en foreløbig løsning.
                var createdEmployee = await _employeeService.CreateAsync(Employee); // Kalder servicen for at oprette medarbejderen.
                // Returnerer et JSON-objekt med successtatus, meddelelse og den oprettede medarbejders data.
                return new JsonResult(new { success = true, message = "Medarbejder oprettet!", data = createdEmployee });
            }
            catch (System.Exception ex)
            {
                // TODO: Implementer logging af exceptions.
                ModelState.AddModelError(string.Empty, $"Fejl under oprettelse: {ex.Message}");
                return Partial("_EmployeeFormFields", Employee); // Returnerer formularen med en generel fejlmeddelelse.
            }
        }

        // Handler for POST-request til at opdatere en eksisterende medarbejder (typisk kaldt via AJAX).
        public async Task<IActionResult> OnPostEditEmployeeAsync(int employeeId)
        {
            // Sikrer at ID'et fra URL'en matcher ID'et i den medsendte model.
            if (employeeId != Employee.Id)
            {
                return BadRequest(new { message = "Data-uoverensstemmelse for medarbejder." });
            }

            // Konverterer SpecializationsString til Employee.Specializations, ligesom i Create.
            if (!string.IsNullOrWhiteSpace(SpecializationsString))
            {
                Employee.Specializations = SpecializationsString.Split(',').Select(s => s.Trim()).Where(s => !string.IsNullOrEmpty(s)).ToList();
            }
            else
            {
                Employee.Specializations = new List<string>();
            }

            if (!ModelState.IsValid)
            {
                return Partial("_EmployeeFormFields", Employee); // Returnerer formularen med valideringsfejl.
            }
            try
            {
                // TODO: Overvej håndtering af RegistrationDate ved opdatering.
                Employee.RegistrationDate = Employee.HireDate; // Opdaterer RegistrationDate, hvis HireDate ændres.
                var updatedEmployee = await _employeeService.UpdateAsync(Employee); // Kalder servicen for at opdatere.
                return new JsonResult(new { success = true, message = "Medarbejder opdateret!", data = updatedEmployee });
            }
            catch (System.Exception ex)
            {
                // TODO: Implementer logging.
                ModelState.AddModelError(string.Empty, $"Fejl under opdatering: {ex.Message}");
                return Partial("_EmployeeFormFields", Employee); // Returnerer formularen med fejl.
            }
        }

        // Handler for POST-request til at slette en medarbejder (typisk kaldt via AJAX).
        public async Task<IActionResult> OnPostDeleteEmployeeAsync(int employeeId)
        {
            try
            {
                await _employeeService.DeleteAsync(employeeId); // Kalder servicen for at slette (antager soft delete via IsDeleted flag).
                return new JsonResult(new { success = true, message = "Medarbejder slettet!", employeeId = employeeId });
            }
            catch (System.Exception ex)
            {
                // TODO: Implementer logging.
                return new JsonResult(new { success = false, message = $"Fejl under sletning: {ex.Message}" });
            }
        }
    }
} 