using ClassLibrary.Features.Adoptions.Application.Abstractions;
using ClassLibrary.Features.Adoptions.Core.Enums;
using ClassLibrary.Features.AnimalManagement.Application.Abstractions;
using ClassLibrary.Features.Customers.Application.Abstractions;
using ClassLibrary.Features.Employees.Application.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ClassLibrary.SharedKernel.Extensions;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RazorPagesApp.Pages.Admin.Adoptions
{
    // TODO: Tilføj [Authorize(Roles = "Administrator,AdoptionCoordinator")] eller lignende
    /// <summary>
    /// PageModel for administrationssiden for adoptioner.
    /// Håndterer visning, filtrering, søgning og statusændringer af adoptioner.
    /// </summary>
    public class IndexModel : PageModel
    {
        private readonly IAdoptionService _adoptionService;
        private readonly IAnimalManagementService _animalService;
        private readonly ICustomerService _customerService;
        private readonly IEmployeeService _employeeService;
        // Overvej IEmployeeService hvis medarbejdernavn skal vises for "Håndteret Af"

        /// <summary>
        /// Initialiserer en ny instans af <see cref="IndexModel"/> klassen.
        /// </summary>
        /// <param name="adoptionService">Service til håndtering af adoptionsdata.</param>
        /// <param name="animalService">Service til håndtering af dyredata.</param>
        /// <param name="customerService">Service til håndtering af kundedata.</param>
        /// <param name="employeeService">Service til håndtering af medarbejderdata.</param>
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

        /// <summary>
        /// BindProperty for den aktuelle adoption, der redigeres eller oprettes.
        /// </summary>
        [BindProperty]
        public ClassLibrary.Features.Adoptions.Core.Models.Adoption Adoption { get; set; } = default!;

        /// <summary>
        /// Liste over adoptioner, der skal vises på siden.
        /// </summary>
        public IList<ClassLibrary.Features.Adoptions.Core.Models.Adoption> Adoptions { get; set; } = new List<ClassLibrary.Features.Adoptions.Core.Models.Adoption>();
        
        // Bruges til at cache navne for at undgå gentagne DB-kald i loop
        private Dictionary<int, string> _animalNamesCache = new Dictionary<int, string>();
        private Dictionary<int, string> _customerNamesCache = new Dictionary<int, string>();
        private Dictionary<int, string> _employeeNamesCache = new Dictionary<int, string>();

        /// <summary>
        /// Besked til brugeren, typisk efter en handling (f.eks. succes eller fejl).
        /// Vises via TempData.
        /// </summary>
        [TempData]
        public string? Message { get; set; }
        /// <summary>
        /// Typen af besked (f.eks. "success", "danger") for styling af Message.
        /// Vises via TempData.
        /// </summary>
        [TempData]
        public string? MessageType { get; set; }

        /// <summary>
        /// Søgeterm indtastet af brugeren.
        /// Bindes fra querystring.
        /// </summary>
        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        /// <summary>
        /// Valgt status til filtrering af adoptioner.
        /// Bindes fra querystring.
        /// </summary>
        [BindProperty(SupportsGet = true)]
        public AdoptionStatus? FilterStatus { get; set; }

        /// <summary>
        /// SelectList med muligheder for at filtrere på adoptionsstatus.
        /// </summary>
        public SelectList StatusOptions { get; set; } = default!;

        /// <summary>
        /// Håndterer GET requests til siden.
        /// Henter adoptioner, anvender eventuel filtrering og søgning, og forbereder data til visning.
        /// </summary>
        /// <returns>En <see cref="IActionResult"/> der repræsenterer sidens resultat.</returns>
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
                var allAnimalIds = adoptionsFromService.Select(a => a.AnimalId).Distinct();
                var allCustomerIds = adoptionsFromService.Select(a => a.CustomerId).Distinct();
                var allEmployeeIds = adoptionsFromService.Where(a => a.EmployeeId.HasValue).Select(a => a.EmployeeId!.Value).Distinct();

                foreach (var id in allAnimalIds)
                {
                    if (!_animalNamesCache.ContainsKey(id))
                    {
                        _animalNamesCache[id] = await GetAnimalNameFromServiceAsync(id);
                    }
                }
                foreach (var id in allCustomerIds)
                {
                    if (!_customerNamesCache.ContainsKey(id))
                    {
                        _customerNamesCache[id] = await GetCustomerNameFromServiceAsync(id);
                    }
                }
                foreach (var id in allEmployeeIds)
                {
                    if (!_employeeNamesCache.ContainsKey(id))
                    {
                        _employeeNamesCache[id] = await GetEmployeeNameFromServiceAsync(id);
                    }
                }

                if (!string.IsNullOrWhiteSpace(SearchTerm))
                {
                    string lowerSearchTerm = SearchTerm.ToLowerInvariant().Trim();
                    filteredAdoptions = filteredAdoptions.Where(a => 
                        (_animalNamesCache.TryGetValue(a.AnimalId, out var animalName) && animalName.ToLowerInvariant().Contains(lowerSearchTerm)) ||
                        (_customerNamesCache.TryGetValue(a.CustomerId, out var customerName) && customerName.ToLowerInvariant().Contains(lowerSearchTerm)) ||
                        (a.EmployeeId.HasValue && _employeeNamesCache.TryGetValue(a.EmployeeId.Value, out var empName) && empName.ToLowerInvariant().Contains(lowerSearchTerm)) ||
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

        /// <summary>
        /// Henter navnet på et dyr baseret på dets ID. Bruger en intern cache for at minimere databasekald.
        /// </summary>
        /// <param name="animalId">ID på dyret.</param>
        /// <returns>Dyrets navn eller en standardtekst hvis ikke fundet.</returns>
        public string GetAnimalName(int animalId)
        {
            return _animalNamesCache.TryGetValue(animalId, out var name) ? name : "Laster...";
        }

        /// <summary>
        /// Henter navnet på en kunde baseret på dets ID. Bruger en intern cache for at minimere databasekald.
        /// </summary>
        /// <param name="customerId">ID på kunden.</param>
        /// <returns>Kundens navn eller en standardtekst hvis ikke fundet.</returns>
        public string GetCustomerName(int customerId)
        {
            return _customerNamesCache.TryGetValue(customerId, out var name) ? name : "Laster...";
        }

        /// <summary>
        /// Henter navnet på en medarbejder baseret på dets ID. Bruger en intern cache.
        /// Returnerer "Ikke tildelt" hvis employeeId er null.
        /// </summary>
        /// <param name="employeeId">ID på medarbejderen (nullable).</param>
        /// <returns>Medarbejderens navn, "Ikke tildelt", eller en standardtekst hvis ID er sat men navn ikke fundet.</returns>
        public string GetEmployeeName(int? employeeId)
        {
            if (!employeeId.HasValue) return "Ikke tildelt";
            return _employeeNamesCache.TryGetValue(employeeId.Value, out var name) ? name : "Laster...";
        }

        /// <summary>
        /// Bestemmer CSS-klassen for et status-badge baseret på adoptionsstatus.
        /// </summary>
        /// <param name="status">Adoptionens status.</param>
        /// <returns>En streng med CSS-klassen (f.eks. "success", "warning text-dark").</returns>
        public string GetStatusBadgeClass(AdoptionStatus status)
        {
            return status switch
            {
                AdoptionStatus.Pending => "warning text-dark",
                AdoptionStatus.Approved => "info text-dark",
                AdoptionStatus.Completed => "success",
                AdoptionStatus.Rejected => "danger",
                AdoptionStatus.Cancelled => "secondary",
                _ => "light text-dark" // Fallback
            };
        }

        // --- Handlers for AJAX status updates --- 

        /// <summary>
        /// Handler for AJAX POST request til at godkende en adoption.
        /// </summary>
        /// <param name="adoptionId">ID på adoptionen der skal godkendes.</param>
        /// <param name="employeeIdToAssign">ID på medarbejderen der godkender.</param>
        /// <returns>JsonResult med status for handlingen.</returns>
        public async Task<IActionResult> OnPostApproveAdoptionAsync(int adoptionId, int employeeIdToAssign) // employeeId forventes fra admin input
        {
            // TODO: Hent nuværende logget ind admin/employee ID i stedet for at sende det
            // For nu antager vi at employeeIdToAssign er gyldigt og kommer fra et input
            if (employeeIdToAssign <= 0) employeeIdToAssign = 1; // Fallback til en default admin, indtil vi har brugerlogin

            try
            {
                await _adoptionService.ApproveAdoptionAsync(adoptionId, employeeIdToAssign);
                return await CreateStatusUpdateSuccessResponse(adoptionId, "Adoption godkendt!");
            }
            catch (System.Exception ex)
            {
                return new JsonResult(new { success = false, message = $"Fejl ved godkendelse: {ex.Message}" });
            }
        }

        /// <summary>
        /// Handler for AJAX POST request til at afvise en adoption.
        /// </summary>
        /// <param name="adoptionId">ID på adoptionen der skal afvises.</param>
        /// <param name="employeeIdToAssign">ID på medarbejderen der afviser.</param>
        /// <returns>JsonResult med status for handlingen.</returns>
        public async Task<IActionResult> OnPostRejectAdoptionAsync(int adoptionId, int employeeIdToAssign) // employeeId forventes fra admin input
        {
            if (employeeIdToAssign <= 0) employeeIdToAssign = 1; // Fallback
            try
            {
                await _adoptionService.RejectAdoptionAsync(adoptionId, employeeIdToAssign);
                return await CreateStatusUpdateSuccessResponse(adoptionId, "Adoption afvist.");
            }
            catch (System.Exception ex)
            {
                return new JsonResult(new { success = false, message = $"Fejl ved afvisning: {ex.Message}" });
            }
        }

        /// <summary>
        /// Handler for AJAX POST request til at markere en adoption som gennemført.
        /// </summary>
        /// <param name="adoptionId">ID på adoptionen der skal markeres som gennemført.</param>
        /// <returns>JsonResult med status for handlingen.</returns>
        public async Task<IActionResult> OnPostCompleteAdoptionAsync(int adoptionId)
        {
            try
            {
                await _adoptionService.CompleteAdoptionAsync(adoptionId);
                return await CreateStatusUpdateSuccessResponse(adoptionId, "Adoption gennemført!");
            }
            catch (System.Exception ex)
            {
                return new JsonResult(new { success = false, message = $"Fejl ved gennemførsel: {ex.Message}" });
            }
        }
        
        /// <summary>
        /// Handler for AJAX POST request til at annullere en adoption (eller dens godkendelse).
        /// </summary>
        /// <param name="adoptionId">ID på adoptionen der skal annulleres.</param>
        /// <param name="employeeIdToAssign">ID på medarbejderen der annullerer.</param>
        /// <param name="reason">Årsag til annulleringen.</param>
        /// <returns>JsonResult med status for handlingen.</returns>
        public async Task<IActionResult> OnPostCancelAdoptionAsync(int adoptionId, int employeeIdToAssign, string reason = "Annulleret af administrator")
        {
            if (employeeIdToAssign <= 0) employeeIdToAssign = 1; // Fallback

            try
            {
                await _adoptionService.CancelAdoptionAsync(adoptionId, employeeIdToAssign, reason); 
                return await CreateStatusUpdateSuccessResponse(adoptionId, "Adoption annulleret.");
            }
            catch (System.Exception ex)
            {
                return new JsonResult(new { success = false, message = $"Fejl ved annullering: {ex.Message}" });
            }
        }

        /// <summary>
        /// Handler for AJAX GET request til at hente en partial view med detaljer for en adoption.
        /// Bruges til at indlæse indhold i en modal.
        /// </summary>
        /// <param name="adoptionId">ID på den ønskede adoption.</param>
        /// <returns>PartialViewResult med adoptionsdetaljer eller NotFoundResult.</returns>
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
            ViewData["StatusBadgeClass"] = GetStatusBadgeClass(adoption.Status);
            
            return Partial("_AdoptionDetailsPartial", adoption); 
        }

        /// <summary>
        /// Handler for AJAX GET request til at hente en partial view med et redigeringsformular for en adoption.
        /// Bruges til at indlæse indhold i en modal for redigering.
        /// </summary>
        /// <param name="adoptionId">ID på den adoption, der skal redigeres.</param>
        /// <returns>PartialViewResult med redigeringsformularen eller NotFoundResult.</returns>
        public async Task<IActionResult> OnGetEditAdoptionFormAsync(int adoptionId)
        {
            Adoption = await _adoptionService.GetAdoptionByIdAsync(adoptionId);
            if (Adoption == null) return NotFound();

            var employees = await _employeeService.GetAllAsync();
            ViewData["EmployeeList"] = new SelectList(employees.OrderBy(e => e.LastName).ThenBy(e => e.FirstName), "Id", "FullName", Adoption.EmployeeId);

            return Partial("_AdoptionFormFields", Adoption);
        }

        /// <summary>
        /// Handler for AJAX POST request til at gemme ændringer til en adoption.
        /// </summary>
        /// <returns>JsonResult med status for handlingen eller PartialViewResult med formularen ved valideringsfejl.</returns>
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

        private async Task<JsonResult> CreateStatusUpdateSuccessResponse(int adoptionId, string message)
        {
            var updatedAdoption = await _adoptionService.GetAdoptionByIdAsync(adoptionId);
            if (updatedAdoption == null) return new JsonResult(new { success = false, message = "Adoption ikke fundet efter opdatering." });

            return new JsonResult(new
            {
                success = true,
                message = message,
                adoptionId = adoptionId,
                newStatusValue = (int)updatedAdoption.Status,
                newStatusDisplay = updatedAdoption.Status.GetDisplayName(),
                employeeId = updatedAdoption.EmployeeId,
                employeeName = updatedAdoption.EmployeeId.HasValue ? await GetEmployeeNameFromServiceAsync(updatedAdoption.EmployeeId.Value) : "Ikke tildelt"
            });
        }

        private async Task<string> GetAnimalNameFromServiceAsync(int animalId) => (await _animalService.GetAnimalByIdAsync(animalId))?.Name ?? "N/A";
        private async Task<string> GetCustomerNameFromServiceAsync(int customerId) { var c = await _customerService.GetByIdAsync(customerId); return c != null ? $"{c.FirstName} {c.LastName}" : "N/A"; }
        private async Task<string> GetEmployeeNameFromServiceAsync(int employeeId) => (await _employeeService.GetByIdAsync(employeeId))?.FullName ?? "N/A";
    }
} 