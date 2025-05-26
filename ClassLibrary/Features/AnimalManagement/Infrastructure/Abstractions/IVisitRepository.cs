using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClassLibrary.Features.AnimalManagement.Core.Models;
using ClassLibrary.Features.AnimalManagement.Core.Enums;
using ClassLibrary.SharedKernel.Persistence.Abstractions;

namespace ClassLibrary.Features.AnimalManagement.Infrastructure.Abstractions
{
    public interface IVisitRepository : IRepository<Visit>
    {
        // AddAsync, GetByIdAsync, UpdateAsync, DeleteAsync arves fra IRepository<Visit>
        // For nu antager vi standard sletning, så vi kan fjerne den specifikke DeleteVisitAsync.

        Task<IEnumerable<Visit>> GetVisitsByAnimalAsync(int animalId);
        Task<IEnumerable<Visit>> GetVisitsByCustomerAsync(int customerId); // CustomerId vil komme fra Customer feature
        Task<IEnumerable<Visit>> GetVisitsByEmployeeAsync(int employeeId); // EmployeeId vil komme fra Employee feature
        Task<IEnumerable<Visit>> GetVisitsByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<Visit>> GetVisitsByStatusAsync(VisitStatus status);
        Task<IEnumerable<Visit>> GetVisitsByTypeAsync(string type);
        Task<IEnumerable<Visit>> GetCancelledVisitsAsync();
        Task<IEnumerable<Visit>> GetCompletedVisitsAsync();
        Task<IEnumerable<Visit>> GetWaitlistedVisitsAsync();
        Task<IEnumerable<Visit>> GetVeterinaryVisitsAsync();
        Task<IEnumerable<Visit>> GetAdoptionVisitsAsync(); // Indikerer et besøg der resulterede i adoption
        Task<Visit?> GetLatestVisitForAnimalAsync(int animalId);
        Task<Visit?> GetLatestVisitForCustomerAsync(int customerId); // CustomerId vil komme fra Customer feature
    }
} 