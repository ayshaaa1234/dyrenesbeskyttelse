using ClassLibrary.Features.AnimalManagement.Application.Abstractions;
using ClassLibrary.Features.AnimalManagement.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ClassLibrary.SharedKernel.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace RazorPagesApp.Pages.Admin.Animals
{
    /// <summary>
    /// PageModel for visning af detaljer for et specifikt dyr, inklusiv sundhedsjournaler og besøg.
    /// Håndterer også AJAX-anmodninger for oprettelse, redigering og sletning af sundhedsjournaler og besøg.
    /// </summary>
    // TODO: Tilføj [Authorize(Roles = "Administrator")]
    public class DetailsModel : PageModel
    {
        private readonly IAnimalManagementService _animalService;

        /// <summary>
        /// Initialiserer en ny instans af <see cref="DetailsModel"/> klassen.
        /// </summary>
        /// <param name="animalService">Servicen til håndtering af dyredata.</param>
        public DetailsModel(IAnimalManagementService animalService)
        {
            _animalService = animalService;
        }

        /// <summary>
        /// Henter eller sætter det dyr, hvis detaljer vises.
        /// </summary>
        public Animal Animal { get; set; } = default!;

        /// <summary>
        /// Henter eller sætter listen af sundhedsjournaler for det aktuelle dyr.
        /// </summary>
        public IList<HealthRecord> HealthRecords { get; set; } = new List<HealthRecord>();

        /// <summary>
        /// Henter eller sætter listen af besøg for det aktuelle dyr.
        /// </summary>
        public IList<Visit> Visits { get; set; } = new List<Visit>();

        /// <summary>
        /// Henter eller sætter en sundhedsjournal, der bindes ved oprettelse/redigering via modal.
        /// </summary>
        [BindProperty]
        public HealthRecord HealthRecord { get; set; } = default!;

        /// <summary>
        /// Henter eller sætter et besøg, der bindes ved oprettelse/redigering via modal.
        /// </summary>
        [BindProperty]
        public Visit Visit { get; set; } = default!;

        /// <summary>
        /// Håndterer HTTP GET-anmodningen for at vise detaljer for et dyr.
        /// </summary>
        /// <param name="id">ID'et på det dyr, hvis detaljer skal vises.</param>
        /// <returns>En <see cref="IActionResult"/> der repræsenterer resultatet af operationen.</returns>
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

        // --- HealthRecord Handlers --- 

        /// <summary>
        /// Håndterer GET-anmodning for at hente partial view for oprettelse af en ny sundhedsjournal.
        /// Bruges til at indlæse formularen i en modal.
        /// </summary>
        /// <param name="animalId">ID'et på det dyr, som sundhedsjournalen tilhører.</param>
        /// <returns>En <see cref="PartialViewResult"/> med formularen.</returns>
        public IActionResult OnGetCreateHealthRecordForm(int animalId)
        {
            HealthRecord = new HealthRecord { AnimalId = animalId, RecordDate = System.DateTime.Today };
            return Partial("_HealthRecordFormFields", HealthRecord);
        }

        /// <summary>
        /// Håndterer GET-anmodning for at hente partial view for redigering af en eksisterende sundhedsjournal.
        /// Bruges til at indlæse formularen i en modal.
        /// </summary>
        /// <param name="recordId">ID'et på sundhedsjournalen, der skal redigeres.</param>
        /// <param name="animalId">ID'et på det dyr, som sundhedsjournalen tilhører (for validering).</param>
        /// <returns>En <see cref="IActionResult"/>; enten en <see cref="PartialViewResult"/> med formularen eller <see cref="NotFoundResult"/>.</returns>
        public async Task<IActionResult> OnGetEditHealthRecordFormAsync(int recordId, int animalId)
        {
            var record = await _animalService.GetHealthRecordByIdAsync(recordId);
            if (record == null || record.AnimalId != animalId)
            {
                return NotFound();
            }
            HealthRecord = record;
            return Partial("_HealthRecordFormFields", HealthRecord);
        }
        
        /// <summary>
        /// Håndterer POST-anmodning for at oprette en ny sundhedsjournal (AJAX).
        /// </summary>
        /// <param name="animalId">ID'et på det dyr, som sundhedsjournalen tilhører.</param>
        /// <returns>En <see cref="JsonResult"/> der indikerer succes/fejl og returnerer den oprettede data.</returns>
        public async Task<IActionResult> OnPostCreateHealthRecordAsync(int animalId)
        {
            if (!ModelState.IsValid)
            {
                return Partial("_HealthRecordFormFields", HealthRecord); 
            }

            try
            {
                HealthRecord.AnimalId = animalId;
                HealthRecord.CreatedAt = System.DateTime.UtcNow;
                var createdRecord = await _animalService.AddHealthRecordAsync(animalId, HealthRecord);
                return new JsonResult(new { success = true, message = "Sundhedsnotat oprettet.", data = createdRecord });
            }
            catch (System.Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Der opstod en fejl under oprettelse af sundhedsnotatet: {ex.Message}");
                return Partial("_HealthRecordFormFields", HealthRecord);
            }
        }

        /// <summary>
        /// Håndterer POST-anmodning for at redigere en eksisterende sundhedsjournal (AJAX).
        /// </summary>
        /// <param name="animalId">ID'et på det dyr, som sundhedsjournalen tilhører.</param>
        /// <param name="recordId">ID'et på sundhedsjournalen der redigeres.</param>
        /// <returns>En <see cref="JsonResult"/> der indikerer succes/fejl og returnerer den opdaterede data.</returns>
        public async Task<IActionResult> OnPostEditHealthRecordAsync(int animalId, int recordId) 
        {
            if (recordId != HealthRecord.Id || animalId != HealthRecord.AnimalId)
            {
                return BadRequest(new { message = "Data mismatch." });
            }

            if (!ModelState.IsValid)
            {
                return Partial("_HealthRecordFormFields", HealthRecord);
            }

            try
            {
                var existingRecord = await _animalService.GetHealthRecordByIdAsync(recordId);
                if (existingRecord == null) return NotFound();
                HealthRecord.CreatedAt = existingRecord.CreatedAt;

                var updatedRecord = await _animalService.UpdateHealthRecordAsync(HealthRecord);
                return new JsonResult(new { success = true, message = "Sundhedsnotat opdateret.", data = updatedRecord });
            }
            catch (System.Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Der opstod en fejl under opdatering af sundhedsnotatet: {ex.Message}");
                return Partial("_HealthRecordFormFields", HealthRecord);
            }
        }

        /// <summary>
        /// Håndterer POST-anmodning for at slette en sundhedsjournal (AJAX).
        /// </summary>
        /// <param name="recordId">ID'et på sundhedsjournalen der skal slettes.</param>
        /// <param name="animalId">ID'et på det dyr, som sundhedsjournalen tilhører.</param>
        /// <returns>En <see cref="JsonResult"/> der indikerer succes/fejl.</returns>
        public async Task<IActionResult> OnPostDeleteHealthRecordAsync(int recordId, int animalId)
        {
            var animal = await _animalService.GetAnimalByIdAsync(animalId);
            if (animal == null)
            {
                return new JsonResult(new { success = false, message = "Dyr ikke fundet." });
            }
            Animal = animal;

            var recordToDelete = await _animalService.GetHealthRecordByIdAsync(recordId);
            if (recordToDelete == null || recordToDelete.AnimalId != animalId)
            {
                return new JsonResult(new { success = false, message = "Sundhedsnotat ikke fundet eller tilhører ikke det angivne dyr." });
            }

            try
            {
                await _animalService.DeleteHealthRecordAsync(recordId);
                var updatedHealthRecords = await _animalService.GetHealthRecordsByAnimalIdAsync(animalId);
                HealthRecords = new List<HealthRecord>(updatedHealthRecords.OrderByDescending(hr => hr.RecordDate));
                
                return new JsonResult(new { success = true, message = "Sundhedsnotat slettet." });
            }
            catch (System.Exception ex)
            {
                return new JsonResult(new { success = false, message = $"Fejl under sletning: {ex.Message}" });
            }
        }

        // --- Visit Handlers (tilsvarende kommentarer som for HealthRecord Handlers kan tilføjes) ---
        public IActionResult OnGetCreateVisitForm(int animalId)
        {
            Visit = new Visit { AnimalId = animalId, PlannedDate = System.DateTime.Now };
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
            var animal = await _animalService.GetAnimalByIdAsync(animalId);
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

        // --- Visit Status Update Handlers ---

        /// <summary>
        /// Bekræfter et planlagt besøg (AJAX).
        /// </summary>
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

        /// <summary>
        /// Annullerer et planlagt eller bekræftet besøg (AJAX).
        /// </summary>
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

        /// <summary>
        /// Markerer et besøg som fuldført (AJAX).
        /// </summary>
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