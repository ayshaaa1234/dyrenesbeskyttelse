using System.Threading.Tasks;
using ClassLibrary.Features.AnimalManagement.Application.Abstractions;
using ClassLibrary.Features.AnimalManagement.Core.Models;
using ClassLibrary.Features.AnimalManagement.Core.Enums; // Tilføjet for VisitStatus direkte brug
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic; // For ICollection in Animal
using System.Linq; // Tilføjet for FirstOrDefault og lignende
using System;
using ClassLibrary.SharedKernel.Extensions; // Tilføjet for GetDisplayName

namespace RazorPagesApp.Pages.Admin.Animals
{
    // TODO: Tilføj [Authorize(Roles = "Administrator")]
    public class DetailsModel : PageModel
    {
        private readonly IAnimalManagementService _animalService;

        public DetailsModel(IAnimalManagementService animalService)
        {
            _animalService = animalService;
        }

        public Animal Animal { get; set; } = default!;
        public IList<HealthRecord> HealthRecords { get; set; } = new List<HealthRecord>();
        public IList<Visit> Visits { get; set; } = new List<Visit>();

        [BindProperty]
        public HealthRecord HealthRecord { get; set; } = default!;

        [BindProperty]
        public Visit Visit { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var animalFromDb = await _animalService.GetAnimalByIdAsync(id.Value);
            if (animalFromDb == null)
            {
                return NotFound();
            }
            Animal = animalFromDb;
            
            var healthRecordsFromService = await _animalService.GetHealthRecordsByAnimalIdAsync(id.Value);
            if (healthRecordsFromService != null) HealthRecords = new List<HealthRecord>(healthRecordsFromService.OrderByDescending(hr => hr.RecordDate));

            var visitsFromService = await _animalService.GetVisitsByAnimalIdAsync(id.Value);
            if (visitsFromService != null) Visits = new List<Visit>(visitsFromService.OrderByDescending(v => v.PlannedDate));
            
            return Page();
        }

        public IActionResult OnGetCreateHealthRecordForm(int animalId)
        {
            // Sikrer at Animal property er sat, hvis den bruges i partial view'en, f.eks. til en titel
            // For _HealthRecordFormFields forventer vi dog kun HealthRecord modellen
            HealthRecord = new HealthRecord { AnimalId = animalId, RecordDate = System.DateTime.Today };
            return Partial("_HealthRecordFormFields", HealthRecord);
        }

        public async Task<IActionResult> OnGetEditHealthRecordFormAsync(int recordId, int animalId)
        {
            // animalId er med for kontekst og routing, men recordId er nøglen her
            var record = await _animalService.GetHealthRecordByIdAsync(recordId);
            if (record == null || record.AnimalId != animalId)
            {
                return NotFound(); // Eller en anden fejlhåndtering
            }
            HealthRecord = record;
            return Partial("_HealthRecordFormFields", HealthRecord);
        }
        
        public async Task<IActionResult> OnPostCreateHealthRecordAsync(int animalId)
        {
            // Sørg for at Animal property er sat, hvis det skal bruges, f.eks. til en titel eller redirect.
            // For nu fokuserer vi på at gemme HealthRecord.
            // Animal = await _animalService.GetAnimalByIdAsync(animalId); 
            // if (Animal == null) return NotFound();

            if (!ModelState.IsValid)
            {
                // Returner formularen med valideringsfejl. Klienten skal håndtere dette.
                return Partial("_HealthRecordFormFields", HealthRecord); 
            }

            try
            {
                HealthRecord.AnimalId = animalId; // Sørg for at AnimalId er korrekt sat
                HealthRecord.CreatedAt = System.DateTime.UtcNow; // Sæt oprettelsesdato
                var createdRecord = await _animalService.AddHealthRecordAsync(animalId, HealthRecord);
                return new JsonResult(new { success = true, message = "Sundhedsnotat oprettet.", data = createdRecord });
            }
            catch (System.Exception ex)
            {
                // Log fejlen
                // Overvej at returnere en mere specifik fejlmeddelelse eller statuskode
                ModelState.AddModelError(string.Empty, $"Der opstod en fejl under oprettelse af sundhedsnotatet: {ex.Message}");
                return Partial("_HealthRecordFormFields", HealthRecord); // Returner form med fejl
            }
        }

        public async Task<IActionResult> OnPostEditHealthRecordAsync(int animalId, int recordId) 
        {
            // Animal = await _animalService.GetAnimalByIdAsync(animalId);
            // if (Animal == null) return NotFound();

            if (recordId != HealthRecord.Id || animalId != HealthRecord.AnimalId)
            {
                // Sikkerhedstjek eller dataintegritet
                return BadRequest(new { message = "Data mismatch." });
            }

            if (!ModelState.IsValid)
            {
                return Partial("_HealthRecordFormFields", HealthRecord);
            }

            try
            {
                // CreatedAt bør ikke ændres ved redigering, så hent den oprindelige værdi hvis nødvendigt
                // Hvis _HealthRecordFormFields ikke poster CreatedAt, og HealthRecord objektet er hentet
                // korrekt i OnGetEditHealthRecordFormAsync, så skulle den eksisterende CreatedAt være bevaret.
                // For en sikkerheds skyld, hent den eksisterende record for at bevare CreatedAt
                var existingRecord = await _animalService.GetHealthRecordByIdAsync(recordId);
                if (existingRecord == null) return NotFound();
                HealthRecord.CreatedAt = existingRecord.CreatedAt; // Bevar oprindelig oprettelsesdato

                var updatedRecord = await _animalService.UpdateHealthRecordAsync(HealthRecord);
                return new JsonResult(new { success = true, message = "Sundhedsnotat opdateret.", data = updatedRecord });
            }
            catch (System.Exception ex)
            {
                // Log fejlen
                ModelState.AddModelError(string.Empty, $"Der opstod en fejl under opdatering af sundhedsnotatet: {ex.Message}");
                return Partial("_HealthRecordFormFields", HealthRecord); // Returner form med fejl
            }
        }

        public async Task<IActionResult> OnPostDeleteHealthRecordAsync(int recordId, int animalId)
        {
            // Sørg for at Animal er loaded for at kunne returnere til den korrekte side/kontekst, hvis nødvendigt,
            // eller hvis du vil genindlæse Animal data efter sletning.
            // For en ren API-agtig tilgang er det måske ikke nødvendigt at hente Animal her.
            var animal = await _animalService.GetAnimalByIdAsync(animalId);
            if (animal == null)
            {
                return new JsonResult(new { success = false, message = "Dyr ikke fundet." });
            }
            Animal = animal; // Sæt Animal property for evt. brug efterfølgende

            var recordToDelete = await _animalService.GetHealthRecordByIdAsync(recordId);
            if (recordToDelete == null || recordToDelete.AnimalId != animalId)
            {
                return new JsonResult(new { success = false, message = "Sundhedsnotat ikke fundet eller tilhører ikke det angivne dyr." });
            }

            try
            {
                await _animalService.DeleteHealthRecordAsync(recordId);
                // Efter sletning, hent den opdaterede liste af sundhedsjournaler
                var updatedHealthRecords = await _animalService.GetHealthRecordsByAnimalIdAsync(animalId);
                HealthRecords = new List<HealthRecord>(updatedHealthRecords.OrderByDescending(hr => hr.RecordDate));
                
                // I stedet for at returnere en hel partial view med tabellen,
                // er det ofte bedre kun at signalere succes og lade client-side JS håndtere DOM-opdatering.
                // Men for nu, kan vi prøve at returnere en partial view, der indeholder selve listen/tabellen.
                // Dette kræver, at vi har en partial view specifikt for HealthRecord-tabellen.
                // Alternativt, returner blot succes og lad klienten genindlæse hele Details-siden eller kun HealthRecord-sektionen.
                return new JsonResult(new { success = true, message = "Sundhedsnotat slettet." });
            }
            catch (System.Exception ex)
            {
                // Log fejlen
                return new JsonResult(new { success = false, message = $"Fejl under sletning: {ex.Message}" });
            }
        }

        // --- Handlers for Visits ---
        public IActionResult OnGetCreateVisitForm(int animalId)
        {
            Visit = new Visit { AnimalId = animalId, PlannedDate = System.DateTime.Now };
            // ViewData["VisitStatusOptions"] = new SelectList(Enum.GetValues(typeof(ClassLibrary.Features.AnimalManagement.Core.Enums.VisitStatus)));
            // ^ Dette er ikke nødvendigt, da vi bruger Html.GetEnumSelectList<VisitStatus>() i partial view
            return Partial("Animals/Visits/_VisitFormFields", Visit);
        }

        public async Task<IActionResult> OnGetEditVisitFormAsync(int visitId, int animalId)
        {
            var visit = await _animalService.GetVisitByIdAsync(visitId);
            if (visit == null || visit.AnimalId != animalId)
            {
                return NotFound();
            }
            Visit = visit;
            return Partial("Animals/Visits/_VisitFormFields", Visit);
        }

        public async Task<IActionResult> OnPostCreateVisitAsync(int animalId)
        {
            if (!ModelState.IsValid)
            {
                return Partial("Animals/Visits/_VisitFormFields", Visit);
            }
            try
            {
                Visit.AnimalId = animalId;
                var createdVisit = await _animalService.CreateVisitAsync(Visit);
                return new JsonResult(new { success = true, message = "Besøg registreret.", data = createdVisit });
            }
            catch (System.Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Fejl under registrering af besøg: {ex.Message}");
                return Partial("Animals/Visits/_VisitFormFields", Visit);
            }
        }

        public async Task<IActionResult> OnPostEditVisitAsync(int animalId, int visitId)
        {
            if (visitId != Visit.Id || animalId != Visit.AnimalId)
            {
                return BadRequest(new { message = "Data mismatch for visit." });
            }

            if (!ModelState.IsValid)
            {
                return Partial("Animals/Visits/_VisitFormFields", Visit);
            }
            try
            {
                var updatedVisit = await _animalService.UpdateVisitAsync(Visit);
                return new JsonResult(new { success = true, message = "Besøg opdateret.", data = updatedVisit });
            }
            catch (System.Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Fejl under opdatering af besøg: {ex.Message}");
                return Partial("Animals/Visits/_VisitFormFields", Visit);
            }
        }

        public async Task<IActionResult> OnPostDeleteVisitAsync(int visitId, int animalId)
        {
            var animal = await _animalService.GetAnimalByIdAsync(animalId); // For kontekst
            if (animal == null) return new JsonResult(new { success = false, message = "Dyr ikke fundet." });
            Animal = animal;

            var visitToDelete = await _animalService.GetVisitByIdAsync(visitId);
            if (visitToDelete == null || visitToDelete.AnimalId != animalId)
            {
                return new JsonResult(new { success = false, message = "Besøg ikke fundet eller tilhører ikke det angivne dyr." });
            }
            try
            {
                await _animalService.DeleteVisitAsync(visitId);
                return new JsonResult(new { success = true, message = "Besøg slettet!", visitId = visitId });
            }
            catch (Exception ex)
            {            
                return new JsonResult(new { success = false, message = $"Fejl under sletning af besøg: {ex.Message}" });
            }
        }

        // Nye handlers for Visit Status opdatering
        public async Task<IActionResult> OnPostConfirmVisitAsync(int visitId, int animalId)
        {
            if (visitId <= 0 || animalId <= 0) 
            {
                return new JsonResult(new { success = false, message = "Ugyldigt ID for besøg eller dyr." });
            }
            try
            {
                var updatedVisit = await _animalService.ConfirmVisitAsync(visitId);
                if (updatedVisit == null)
                {
                    return new JsonResult(new { success = false, message = "Besøg blev ikke fundet eller kunne ikke bekræftes." });
                }
                return new JsonResult(new { 
                    success = true, 
                    message = "Besøgsstatus ændret til Bekræftet!", 
                    data = new { 
                        updatedVisit.Id, 
                        updatedVisit.AnimalId, 
                        updatedVisit.PlannedDate, 
                        updatedVisit.PlannedDuration, 
                        updatedVisit.Type, 
                        updatedVisit.Purpose, 
                        updatedVisit.Visitor, 
                        updatedVisit.Status, 
                        StatusDisplay = updatedVisit.Status.GetDisplayName(),
                        updatedVisit.Notes, 
                        updatedVisit.ActualDate, 
                        updatedVisit.ActualDuration 
                    }
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = $"Fejl: {ex.Message}" });
            }
        }

        public async Task<IActionResult> OnPostCancelVisitAsync(int visitId, int animalId)
        {
             if (visitId <= 0 || animalId <= 0) 
            {
                return new JsonResult(new { success = false, message = "Ugyldigt ID for besøg eller dyr." });
            }
            try
            {
                var updatedVisit = await _animalService.CancelVisitAsync(visitId);
                if (updatedVisit == null)
                {
                    return new JsonResult(new { success = false, message = "Besøg blev ikke fundet eller kunne ikke annulleres." });
                }
                return new JsonResult(new { 
                    success = true, 
                    message = "Besøgsstatus ændret til Annulleret!", 
                    data = new { 
                        updatedVisit.Id, 
                        updatedVisit.AnimalId, 
                        updatedVisit.PlannedDate, 
                        updatedVisit.PlannedDuration, 
                        updatedVisit.Type, 
                        updatedVisit.Purpose, 
                        updatedVisit.Visitor, 
                        updatedVisit.Status, 
                        StatusDisplay = updatedVisit.Status.GetDisplayName(),
                        updatedVisit.Notes, 
                        updatedVisit.ActualDate, 
                        updatedVisit.ActualDuration 
                    }
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = $"Fejl: {ex.Message}" });
            }
        }

        public async Task<IActionResult> OnPostCompleteVisitAsync(int visitId, int animalId)
        {
            if (visitId <= 0 || animalId <= 0) 
            {
                return new JsonResult(new { success = false, message = "Ugyldigt ID for besøg eller dyr." });
            }
            try
            {
                var updatedVisit = await _animalService.CompleteVisitAsync(visitId, DateTime.Now, 0, string.Empty); 
                if (updatedVisit == null)
                {
                    return new JsonResult(new { success = false, message = "Besøg blev ikke fundet eller kunne ikke fuldføres." });
                }
                return new JsonResult(new { 
                    success = true, 
                    message = "Besøgsstatus ændret til Fuldført!", 
                    data = new { 
                        updatedVisit.Id, 
                        updatedVisit.AnimalId, 
                        updatedVisit.PlannedDate, 
                        updatedVisit.PlannedDuration, 
                        updatedVisit.Type, 
                        updatedVisit.Purpose, 
                        updatedVisit.Visitor, 
                        updatedVisit.Status, 
                        StatusDisplay = updatedVisit.Status.GetDisplayName(),
                        updatedVisit.Notes, 
                        updatedVisit.ActualDate, 
                        updatedVisit.ActualDuration 
                    }
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = $"Fejl: {ex.Message}" });
            }
        }
    }
} 