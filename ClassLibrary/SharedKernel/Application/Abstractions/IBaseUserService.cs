using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClassLibrary.SharedKernel.Domain.Models; // Opdateret for BaseUser

namespace ClassLibrary.SharedKernel.Application.Abstractions // Opdateret namespace
{
    /// <summary>
    /// Generisk interface for services der håndterer brugere (f.eks. kunder, medarbejdere)
    /// </summary>
    /// <typeparam name="TUser">Bruger typen, skal arve fra BaseUser</typeparam>
    public interface IBaseUserService<TUser> where TUser : BaseUser
    {
        /// <summary>
        /// Henter alle brugere af typen TUser
        /// </summary>
        Task<IEnumerable<TUser>> GetAllAsync();

        /// <summary>
        /// Henter en bruger baseret på ID
        /// </summary>
        Task<TUser?> GetByIdAsync(int id);

        /// <summary>
        /// Opretter en ny bruger
        /// </summary>
        Task<TUser> CreateAsync(TUser user);

        /// <summary>
        /// Opdaterer en eksisterende bruger
        /// </summary>
        Task<TUser> UpdateAsync(TUser user);

        /// <summary>
        /// Sletter en bruger (soft delete)
        /// </summary>
        Task DeleteAsync(int id);

        /// <summary>
        /// Henter brugere baseret på navn
        /// </summary>
        Task<IEnumerable<TUser>> GetByNameAsync(string name);

        /// <summary>
        /// Henter en bruger baseret på email
        /// </summary>
        Task<TUser?> GetByEmailAsync(string email);

        /// <summary>
        /// Henter brugere baseret på telefonnummer
        /// </summary>
        Task<IEnumerable<TUser>> GetByPhoneAsync(string phone);

        /// <summary>
        /// Henter brugere der er registreret i et bestemt datointerval
        /// </summary>
        Task<IEnumerable<TUser>> GetByRegistrationDateRangeAsync(DateTime startDate, DateTime endDate);

        /// <summary>
        /// Søger efter brugere baseret på en generel søgestreng (f.eks. navn, email)
        /// </summary>
        Task<IEnumerable<TUser>> SearchAsync(string searchTerm);
    }
} 