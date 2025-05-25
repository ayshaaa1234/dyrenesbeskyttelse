using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClassLibrary.Models;
using ClassLibrary.Interfaces;

namespace ClassLibrary.Services
{
    /// <summary>
    /// Service til håndtering af besøgslogge
    /// </summary>
    public class VisitLogService : IVisitLogService
    {
        private readonly IVisitLogRepository _visitLogRepository;

        /// <summary>
        /// Konstruktør
        /// </summary>
        public VisitLogService(IVisitLogRepository visitLogRepository)
        {
            _visitLogRepository = visitLogRepository ?? throw new ArgumentNullException(nameof(visitLogRepository));
        }

        /// <summary>
        /// Henter alle besøgslogge
        /// </summary>
        public async Task<IEnumerable<VisitLog>> GetAllVisitLogsAsync()
        {
            return await _visitLogRepository.GetAllAsync();
        }

        /// <summary>
        /// Henter en besøgslog baseret på ID
        /// </summary>
        public async Task<VisitLog> GetVisitLogByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("ID skal være større end 0");

            var visitLog = await _visitLogRepository.GetByIdAsync(id);
            if (visitLog == null)
                throw new KeyNotFoundException($"Ingen besøgslog fundet med ID: {id}");

            return visitLog;
        }

        /// <summary>
        /// Opretter en ny besøgslog
        /// </summary>
        public async Task<VisitLog> CreateVisitLogAsync(VisitLog visitLog)
        {
            if (visitLog == null)
                throw new ArgumentNullException(nameof(visitLog));

            ValidateVisitLog(visitLog);
            return await _visitLogRepository.AddAsync(visitLog);
        }

        /// <summary>
        /// Opdaterer en eksisterende besøgslog
        /// </summary>
        public async Task<VisitLog> UpdateVisitLogAsync(VisitLog visitLog)
        {
            if (visitLog == null)
                throw new ArgumentNullException(nameof(visitLog));

            ValidateVisitLog(visitLog);
            return await _visitLogRepository.UpdateAsync(visitLog);
        }

        /// <summary>
        /// Sletter en besøgslog
        /// </summary>
        public async Task DeleteVisitLogAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("ID skal være større end 0");

            await _visitLogRepository.DeleteAsync(id);
        }

        /// <summary>
        /// Henter besøgslogge for et bestemt dyr
        /// </summary>
        public async Task<IEnumerable<VisitLog>> GetVisitLogsByAnimalIdAsync(int animalId)
        {
            if (animalId <= 0)
                throw new ArgumentException("AnimalId skal være større end 0");

            return await _visitLogRepository.GetByAnimalIdAsync(animalId);
        }

        /// <summary>
        /// Henter besøgslogge for en bestemt kunde
        /// </summary>
        public async Task<IEnumerable<VisitLog>> GetVisitLogsByCustomerIdAsync(int customerId)
        {
            if (customerId <= 0)
                throw new ArgumentException("CustomerId skal være større end 0");

            return await _visitLogRepository.GetByCustomerIdAsync(customerId);
        }

        /// <summary>
        /// Henter besøgslogge for en bestemt medarbejder
        /// </summary>
        public async Task<IEnumerable<VisitLog>> GetVisitLogsByEmployeeIdAsync(int employeeId)
        {
            if (employeeId <= 0)
                throw new ArgumentException("EmployeeId skal være større end 0");

            return await _visitLogRepository.GetByEmployeeIdAsync(employeeId);
        }

        /// <summary>
        /// Henter besøgslogge baseret på datointerval
        /// </summary>
        public async Task<IEnumerable<VisitLog>> GetVisitLogsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
                throw new ArgumentException("Startdato skal være før slutdato");

            if (startDate > DateTime.Now)
                throw new ArgumentException("Startdato kan ikke være i fremtiden");

            return await _visitLogRepository.GetByDateRangeAsync(startDate, endDate);
        }

        /// <summary>
        /// Henter besøgslogge baseret på besøgstype
        /// </summary>
        public async Task<IEnumerable<VisitLog>> GetVisitLogsByVisitTypeAsync(string visitType)
        {
            if (string.IsNullOrWhiteSpace(visitType))
                throw new ArgumentException("Besøgstype kan ikke være tom");

            return await _visitLogRepository.GetByVisitTypeAsync(visitType);
        }

        /// <summary>
        /// Henter besøgslogge baseret på besøger
        /// </summary>
        public async Task<IEnumerable<VisitLog>> GetVisitLogsByVisitorAsync(string visitor)
        {
            if (string.IsNullOrWhiteSpace(visitor))
                throw new ArgumentException("Besøger kan ikke være tom");

            return await _visitLogRepository.GetByVisitorAsync(visitor);
        }

        /// <summary>
        /// Henter besøgslogge baseret på formål
        /// </summary>
        public async Task<IEnumerable<VisitLog>> GetVisitLogsByPurposeAsync(string purpose)
        {
            if (string.IsNullOrWhiteSpace(purpose))
                throw new ArgumentException("Formål kan ikke være tomt");

            return await _visitLogRepository.GetByPurposeAsync(purpose);
        }

        /// <summary>
        /// Henter besøgslogge baseret på varighedsinterval
        /// </summary>
        public async Task<IEnumerable<VisitLog>> GetVisitLogsByDurationRangeAsync(int minDuration, int maxDuration)
        {
            if (minDuration < 0)
                throw new ArgumentException("Minimumsvarighed kan ikke være negativ");
            if (maxDuration < minDuration)
                throw new ArgumentException("Maksimumsvarighed skal være større end minimumsvarighed");

            return await _visitLogRepository.GetByDurationRangeAsync(minDuration, maxDuration);
        }

        /// <summary>
        /// Henter besøgslogge baseret på lægebesøg
        /// </summary>
        public async Task<IEnumerable<VisitLog>> GetVisitLogsByVeterinaryVisitAsync(bool isVeterinaryVisit)
        {
            return await _visitLogRepository.GetByVeterinaryVisitAsync(isVeterinaryVisit);
        }

        /// <summary>
        /// Henter besøgslogge baseret på adoptionsresultat
        /// </summary>
        public async Task<IEnumerable<VisitLog>> GetVisitLogsByAdoptionResultAsync(bool resultedInAdoption)
        {
            return await _visitLogRepository.GetByAdoptionResultAsync(resultedInAdoption);
        }

        /// <summary>
        /// Henter alle lægebesøg
        /// </summary>
        public async Task<IEnumerable<VisitLog>> GetVeterinaryVisitsAsync()
        {
            return await _visitLogRepository.GetVeterinaryVisitsAsync();
        }

        /// <summary>
        /// Henter besøg der resulterede i adoption
        /// </summary>
        public async Task<IEnumerable<VisitLog>> GetVisitsResultingInAdoptionAsync()
        {
            return await _visitLogRepository.GetVisitsResultingInAdoptionAsync();
        }

        /// <summary>
        /// Henter det seneste besøg for et dyr
        /// </summary>
        public async Task<VisitLog> GetLatestVisitForAnimalAsync(int animalId)
        {
            if (animalId <= 0)
                throw new ArgumentException("AnimalId skal være større end 0");

            return await _visitLogRepository.GetLatestVisitForAnimalAsync(animalId);
        }

        /// <summary>
        /// Marker et besøg som lægebesøg
        /// </summary>
        public async Task MarkAsVeterinaryVisitAsync(int visitLogId)
        {
            if (visitLogId <= 0)
                throw new ArgumentException("VisitLogId skal være større end 0");

            var visitLog = await GetVisitLogByIdAsync(visitLogId);
            visitLog.IsVeterinaryVisit = true;
            await _visitLogRepository.UpdateAsync(visitLog);
        }

        /// <summary>
        /// Marker et besøg som resulteret i adoption
        /// </summary>
        public async Task MarkAsResultedInAdoptionAsync(int visitLogId)
        {
            if (visitLogId <= 0)
                throw new ArgumentException("VisitLogId skal være større end 0");

            var visitLog = await GetVisitLogByIdAsync(visitLogId);
            visitLog.ResultedInAdoption = true;
            await _visitLogRepository.UpdateAsync(visitLog);
        }

        /// <summary>
        /// Tilføjer noter til en besøgslog
        /// </summary>
        public async Task AddNotesAsync(int visitLogId, string notes)
        {
            if (visitLogId <= 0)
                throw new ArgumentException("VisitLogId skal være større end 0");

            if (string.IsNullOrWhiteSpace(notes))
                throw new ArgumentException("Noter kan ikke være tomme");

            var visitLog = await GetVisitLogByIdAsync(visitLogId);
            if (!string.IsNullOrEmpty(visitLog.Notes))
                visitLog.Notes += "\n";
            visitLog.Notes += notes;
            await _visitLogRepository.UpdateAsync(visitLog);
        }

        /// <summary>
        /// Opdaterer besøgsvarigheden
        /// </summary>
        public async Task UpdateDurationAsync(int visitLogId, int duration)
        {
            if (visitLogId <= 0)
                throw new ArgumentException("VisitLogId skal være større end 0");

            if (duration <= 0)
                throw new ArgumentException("Varighed skal være større end 0");

            var visitLog = await GetVisitLogByIdAsync(visitLogId);
            visitLog.Duration = duration;
            await _visitLogRepository.UpdateAsync(visitLog);
        }

        /// <summary>
        /// Validerer en besøgslog
        /// </summary>
        private void ValidateVisitLog(VisitLog visitLog)
        {
            if (visitLog.AnimalId <= 0)
                throw new ArgumentException("AnimalId skal være større end 0");

            if (visitLog.CustomerId <= 0)
                throw new ArgumentException("CustomerId skal være større end 0");

            if (visitLog.EmployeeId <= 0)
                throw new ArgumentException("EmployeeId skal være større end 0");

            if (string.IsNullOrWhiteSpace(visitLog.VisitType))
                throw new ArgumentException("Besøgstype kan ikke være tom");

            if (string.IsNullOrWhiteSpace(visitLog.Visitor))
                throw new ArgumentException("Besøger kan ikke være tom");

            if (string.IsNullOrWhiteSpace(visitLog.Purpose))
                throw new ArgumentException("Formål kan ikke være tomt");

            if (visitLog.Duration <= 0)
                throw new ArgumentException("Varighed skal være større end 0");

            if (visitLog.VisitDate > DateTime.Now)
                throw new ArgumentException("Besøgsdato kan ikke være i fremtiden");
        }
    }
} 