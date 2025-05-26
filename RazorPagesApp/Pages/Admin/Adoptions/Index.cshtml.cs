using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClassLibrary.Features.Adoptions.Application.Abstractions;
using ClassLibrary.Features.Adoptions.Core.Models;
using ClassLibrary.Features.Adoptions.Core.Enums;
using ClassLibrary.Features.AnimalManagement.Application.Abstractions;
using ClassLibrary.Features.Customers.Application.Abstractions;
using ClassLibrary.Features.Employees.Application.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ClassLibrary.SharedKernel.Extensions;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace RazorPagesApp.Pages.Admin.Adoptions
{
    // TODO: Tilføj [Authorize(Roles = "Administrator,AdoptionCoordinator")] eller lignende
    public class IndexModel : PageModel
    {
        private readonly IAdoptionService _adoptionService;
        private readonly IAnimalManagementService _animalService;
        private readonly ICustomerService _customerService;
        private readonly IEmployeeService _employeeService;
        // Overvej IEmployeeService hvis medarbejdernavn skal vises for "Håndteret Af"

        public IndexModel(
            IAdoptionService adoptionService, 
            IAnimalManagementService animalService,
            ICustomerService customerService,
            IEmployeeService employeeService)
        {
            _adoptionService = adoptionService;
            _animalService = animalService;
            _customerService = customerService;
            _employeeService = employeeService;
        }

        [BindProperty]
        public ClassLibrary.Features.Adoptions.Core.Models.Adoption Adoption { get; set; } = default!;

        public IList<ClassLibrary.Features.Adoptions.Core.Models.Adoption> Adoptions { get; set; } = new List<ClassLibrary.Features.Adoptions.Core.Models.Adoption>();
        
        // Bruges til at cache navne for at undgå gentagne DB-kald i loop
        private Dictionary<int, string> _animalNamesCache = new Dictionary<int, string>();
        private Dictionary<int, string> _customerNamesCache = new Dictionary<int, string>();

        [TempData]
        public string? Message { get; set; }
        [TempData]
        public string? MessageType { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        [BindProperty(SupportsGet = true)]
        public AdoptionStatus? FilterStatus { get; set; }

        public SelectList StatusOptions { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync()
        {
            // Initialiser StatusOptions for dropdown
            var enumValues = System.Enum.GetValues(typeof(AdoptionStatus)).Cast<AdoptionStatus>();
            StatusOptions = new SelectList(enumValues.Select(s => new SelectListItem
            {
                Value = s.ToString(),
                Text = s.GetDisplayName()
            }), "Value", "Text", FilterStatus);

            var adoptionsFromService = await _adoptionService.GetAllAdoptionsAsync();
            if (adoptionsFromService != null)
            {
                IEnumerable<ClassLibrary.Features.Adoptions.Core.Models.Adoption> filteredAdoptions = adoptionsFromService;

                // Pre-load animal and customer names for display and potential search
                // Dette gøres bedst FØR filtrering på navne, for at cachen er populær
                foreach (var adoption in filteredAdoptions) // Brug filteredAdoptions her, hvis det skal være dynamisk
                {
                    if (!_animalNamesCache.ContainsKey(adoption.AnimalId))
                    {
                        var animal = await _animalService.GetAnimalByIdAsync(adoption.AnimalId);
                        _animalNamesCache[adoption.AnimalId] = animal?.Name ?? "Ukendt Dyr";
                    }
                    if (!_customerNamesCache.ContainsKey(adoption.CustomerId))
                    {
                        var customer = await _customerService.GetByIdAsync(adoption.CustomerId);
                        _customerNamesCache[adoption.CustomerId] = customer != null ? $"{customer.FirstName} {customer.LastName}" : "Ukendt Kunde";
                    }
                }

                if (!string.IsNullOrWhiteSpace(SearchTerm))
                {
                    string lowerSearchTerm = SearchTerm.ToLowerInvariant().Trim();
                    filteredAdoptions = filteredAdoptions.Where(a => 
                        (_animalNamesCache.TryGetValue(a.AnimalId, out var animalName) && animalName.ToLowerInvariant().Contains(lowerSearchTerm)) ||
                        (_customerNamesCache.TryGetValue(a.CustomerId, out var customerName) && customerName.ToLowerInvariant().Contains(lowerSearchTerm)) ||
                        (a.Id.ToString().Contains(lowerSearchTerm)) // Søg også på Adoptions ID
                    );
                }

                if (FilterStatus.HasValue)
                {
                    filteredAdoptions = filteredAdoptions.Where(a => a.Status == FilterStatus.Value);
                }

                Adoptions = filteredAdoptions.OrderByDescending(a => a.ApplicationDate).ToList();
            }
            return Page();
        }

        // Hjælpefunktioner til Razor view
        public string GetAnimalName(int animalId)
        {
            return _animalNamesCache.TryGetValue(animalId, out var name) ? name : "Henter...";
        }

        public string GetCustomerName(int customerId)
        {
            return _customerNamesCache.TryGetValue(customerId, out var name) ? name : "Henter...";
        }

        // --- Handlers for AJAX status updates --- 

        public async Task<IActionResult> OnPostApproveAdoptionAsync(int adoptionId, int employeeIdToAssign) // employeeId forventes fra admin input
        {
            // TODO: Hent nuværende logget ind admin/employee ID i stedet for at sende det
            // For nu antager vi at employeeIdToAssign er gyldigt og kommer fra et input
            if (employeeIdToAssign <= 0) employeeIdToAssign = 1; // Fallback til en default admin, indtil vi har brugerlogin

            try
            {
                await _adoptionService.ApproveAdoptionAsync(adoptionId, employeeIdToAssign);
                var updatedAdoption = await _adoptionService.GetAdoptionByIdAsync(adoptionId);
                return new JsonResult(new { 
                    success = true, 
                    message = "Adoption godkendt!", 
                    adoptionId = adoptionId, 
                    newStatus = updatedAdoption.Status.GetDisplayName(), 
                    employeeId = updatedAdoption.EmployeeId 
                });
            }
            catch (System.Exception ex)
            {
                return new JsonResult(new { success = false, message = $"Fejl ved godkendelse: {ex.Message}" });
            }
        }

        public async Task<IActionResult> OnPostRejectAdoptionAsync(int adoptionId, int employeeIdToAssign) // employeeId forventes fra admin input
        {
            if (employeeIdToAssign <= 0) employeeIdToAssign = 1; // Fallback
            try
            {
                await _adoptionService.RejectAdoptionAsync(adoptionId, employeeIdToAssign);
                var updatedAdoption = await _adoptionService.GetAdoptionByIdAsync(adoptionId);
                return new JsonResult(new { 
                    success = true, 
                    message = "Adoption afvist.", 
                    adoptionId = adoptionId, 
                    newStatus = updatedAdoption.Status.GetDisplayName(), 
                    employeeId = updatedAdoption.EmployeeId 
                });
            }
            catch (System.Exception ex)
            {
                return new JsonResult(new { success = false, message = $"Fejl ved afvisning: {ex.Message}" });
            }
        }

        public async Task<IActionResult> OnPostCompleteAdoptionAsync(int adoptionId)
        {
            try
            {
                await _adoptionService.CompleteAdoptionAsync(adoptionId);
                var updatedAdoption = await _adoptionService.GetAdoptionByIdAsync(adoptionId);
                return new JsonResult(new { 
                    success = true, 
                    message = "Adoption gennemført!", 
                    adoptionId = adoptionId, 
                    newStatus = updatedAdoption.Status.GetDisplayName() 
                });
            }
            catch (System.Exception ex)
            {
                return new JsonResult(new { success = false, message = $"Fejl ved gennemførsel: {ex.Message}" });
            }
        }
        
        public async Task<IActionResult> OnPostCancelAdoptionAsync(int adoptionId, int employeeIdToAssign, string reason = "Annulleret af administrator")
        {
            if (employeeIdToAssign <= 0) employeeIdToAssign = 1; // Fallback

            try
            {
                await _adoptionService.CancelAdoptionAsync(adoptionId, employeeIdToAssign, reason); 
                var updatedAdoption = await _adoptionService.GetAdoptionByIdAsync(adoptionId);
                 return new JsonResult(new { 
                    success = true, 
                    message = "Adoption annulleret.", 
                    adoptionId = adoptionId, 
                    newStatus = updatedAdoption.Status.GetDisplayName(),
                    employeeId = updatedAdoption.EmployeeId // Send medarbejder ID tilbage til UI
                });
            }
            catch (System.Exception ex)
            {
                return new JsonResult(new { success = false, message = $"Fejl ved annullering: {ex.Message}" });
            }
        }

        // Handler for at hente detaljer til modal
        public async Task<IActionResult> OnGetAdoptionDetailsPartialAsync(int adoptionId)
        {
            var adoption = await _adoptionService.GetAdoptionByIdAsync(adoptionId);
            if (adoption == null)
            {
                return NotFound();
            }
            // Her kan vi også hente Animal og Customer navne hvis de ikke allerede er loaded
            // og evt. Employee navn.
            // For nu, sender vi bare adoption objektet.
            // ViewModel specifikt for modalen kunne være en fordel her.
            ViewData["AnimalName"] = (await _animalService.GetAnimalByIdAsync(adoption.AnimalId))?.Name ?? "N/A";
            var customer = await _customerService.GetByIdAsync(adoption.CustomerId);
            ViewData["CustomerName"] = customer != null ? $"{customer.FirstName} {customer.LastName}" : "N/A";
            var employee = adoption.EmployeeId.HasValue ? await _employeeService.GetByIdAsync(adoption.EmployeeId.Value) : null;
            ViewData["EmployeeName"] = employee != null ? $"{employee.FirstName} {employee.LastName}" : "Ikke tildelt";
            
            return Partial("_AdoptionDetailsPartial", adoption); 
        }

        // NY HANDLER: Hent redigeringsformular for adoption
        public async Task<IActionResult> OnGetEditAdoptionFormAsync(int adoptionId)
        {
            Adoption = await _adoptionService.GetAdoptionByIdAsync(adoptionId);
            if (Adoption == null) return NotFound();

            var employees = await _employeeService.GetAllAsync();
            ViewData["EmployeeList"] = new SelectList(employees.OrderBy(e => e.LastName).ThenBy(e => e.FirstName), "Id", "FullName", Adoption.EmployeeId);

            return Partial("_AdoptionFormFields", Adoption);
        }

        // NY HANDLER: Post redigering af adoption
        public async Task<IActionResult> OnPostEditAdoptionAsync()
        {
            if (Request.Form["Adoption.EmployeeId"] == "")
            {
                Adoption.EmployeeId = null;
            }

            if (!ModelState.IsValid)
            {
                var employees = await _employeeService.GetAllAsync();
                ViewData["EmployeeList"] = new SelectList(employees.OrderBy(e => e.LastName).ThenBy(e => e.FirstName), "Id", "FullName", Adoption.EmployeeId);
                return Partial("_AdoptionFormFields", Adoption);
            }
            try
            {
                var updatedAdoption = await _adoptionService.UpdateAdoptionAsync(Adoption);
                return new JsonResult(new { success = true, message = "Adoption opdateret!", data = updatedAdoption });
            }
            catch (System.Exception ex)
            {
                var employees = await _employeeService.GetAllAsync();
                ViewData["EmployeeList"] = new SelectList(employees.OrderBy(e => e.LastName).ThenBy(e => e.FirstName), "Id", "FullName", Adoption.EmployeeId);
                ModelState.AddModelError(string.Empty, $"Fejl under opdatering: {ex.Message}");
                return Partial("_AdoptionFormFields", Adoption); 
            }
        }
    }
} 