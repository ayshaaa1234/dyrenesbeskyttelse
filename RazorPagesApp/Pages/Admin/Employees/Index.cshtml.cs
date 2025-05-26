using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClassLibrary.Features.Employees.Application.Abstractions;
using ClassLibrary.Features.Employees.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RazorPagesApp.Pages.Admin.Employees
{
    // TODO: Tilføj [Authorize(Roles = "Administrator,HRManager")] eller lignende
    public class IndexModel : PageModel
    {
        private readonly IEmployeeService _employeeService;

        public IndexModel(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        public IList<Employee> Employees { get; set; } = new List<Employee>();
        
        [TempData]
        public string? Message { get; set; }
        [TempData]
        public string? MessageType { get; set; } // Bruges til alert-success, alert-danger etc.

        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        [BindProperty]
        public Employee Employee { get; set; } = default!;

        // Tilføj en property til at modtage den kommaseparerede streng for specialiseringer
        [BindProperty]
        public string? SpecializationsString { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var employeesFromService = await _employeeService.GetAllAsync();
            if (employeesFromService != null)
            {
                IEnumerable<Employee> filteredEmployees = employeesFromService;
                if (!string.IsNullOrWhiteSpace(SearchTerm))
                {
                    string lowerSearchTerm = SearchTerm.ToLowerInvariant().Trim();
                    filteredEmployees = filteredEmployees.Where(e => 
                        (e.FirstName != null && e.FirstName.ToLowerInvariant().Contains(lowerSearchTerm)) || 
                        (e.LastName != null && e.LastName.ToLowerInvariant().Contains(lowerSearchTerm)) || 
                        (e.Email != null && e.Email.ToLowerInvariant().Contains(lowerSearchTerm))
                        // Overvej at søge i Position og Department også, hvis relevant
                        // (e.Position != null && e.Position.ToLowerInvariant().Contains(lowerSearchTerm)) ||
                        // (e.Department != null && e.Department.ToLowerInvariant().Contains(lowerSearchTerm))
                    );
                }
                Employees = filteredEmployees.OrderBy(e => e.LastName).ThenBy(e => e.FirstName).ToList();
            }
            return Page();
        }

        // Handler for at vise Opret Medarbejder formular (i modal)
        public IActionResult OnGetCreateEmployeeForm()
        {
            Employee = new Employee 
            { 
                HireDate = System.DateTime.Today, 
                Specializations = new List<string>() 
            }; // Initialiser med default værdier
            return Partial("_EmployeeFormFields", Employee);
        }

        // Handler for at vise Rediger Medarbejder formular (i modal)
        public async Task<IActionResult> OnGetEditEmployeeFormAsync(int employeeId)
        {
            var employee = await _employeeService.GetByIdAsync(employeeId);
            if (employee == null)
            {
                return NotFound(); 
            }
            Employee = employee;
            if(Employee.Specializations == null) Employee.Specializations = new List<string>();
            return Partial("_EmployeeFormFields", Employee);
        }

        // Handler for at Oprette en Medarbejder (AJAX)
        public async Task<IActionResult> OnPostCreateEmployeeAsync()
        {
            // Konverter SpecializationsString til Employee.Specializations
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
                // Returner form med valideringsfejl, som klienten kan vise i modalen
                return Partial("_EmployeeFormFields", Employee); 
            }
            try
            {
                // Sæt RegistrationDate lig HireDate, hvis ikke allerede håndteret i service
                Employee.RegistrationDate = Employee.HireDate; 
                var createdEmployee = await _employeeService.CreateAsync(Employee);
                return new JsonResult(new { success = true, message = "Medarbejder oprettet!", data = createdEmployee });
            }
            catch (System.Exception ex)
            {
                // Log fejlen (ikke vist her)
                ModelState.AddModelError(string.Empty, $"Fejl under oprettelse: {ex.Message}");
                return Partial("_EmployeeFormFields", Employee); // Returner form med fejl
            }
        }

        // Handler for at Opdatere en Medarbejder (AJAX)
        public async Task<IActionResult> OnPostEditEmployeeAsync(int employeeId)
        {
            if (employeeId != Employee.Id)
            {
                return BadRequest(new { message = "Data-uoverensstemmelse for medarbejder." });
            }

            // Konverter SpecializationsString til Employee.Specializations
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
                return Partial("_EmployeeFormFields", Employee);
            }
            try
            {
                // Sæt RegistrationDate lig HireDate, hvis ikke allerede håndteret i service
                Employee.RegistrationDate = Employee.HireDate;
                var updatedEmployee = await _employeeService.UpdateAsync(Employee);
                return new JsonResult(new { success = true, message = "Medarbejder opdateret!", data = updatedEmployee });
            }
            catch (System.Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Fejl under opdatering: {ex.Message}");
                return Partial("_EmployeeFormFields", Employee);
            }
        }

        // Handler for at Slette en Medarbejder (AJAX)
        public async Task<IActionResult> OnPostDeleteEmployeeAsync(int employeeId)
        {
            try
            {
                await _employeeService.DeleteAsync(employeeId); // Antager soft delete via IsDeleted
                return new JsonResult(new { success = true, message = "Medarbejder slettet!", employeeId = employeeId });
            }
            catch (System.Exception ex)
            {
                return new JsonResult(new { success = false, message = $"Fejl under sletning: {ex.Message}" });
            }
        }
    }
} 