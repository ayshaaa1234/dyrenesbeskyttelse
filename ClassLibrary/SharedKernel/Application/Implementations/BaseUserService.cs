using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ClassLibrary.SharedKernel.Domain.Models; // For BaseUser
using ClassLibrary.SharedKernel.Persistence.Abstractions; // For IRepository<T>
using ClassLibrary.SharedKernel.Application.Abstractions; // For IBaseUserService
using ClassLibrary.SharedKernel.Exceptions; // For RepositoryException

namespace ClassLibrary.SharedKernel.Application.Implementations // Opdateret namespace
{
    /// <summary>
    /// Abstrakt base service for håndtering af brugere.
    /// Indeholder grundlæggende CRUD-operationer og validering.
    /// Specifikke søgemetoder (GetByNameAsync etc.) skal implementeres i konkrete subklasser,
    /// da de afhænger af specifikke repository-interfaces.
    /// </summary>
    public abstract class BaseUserService<TUser> : IBaseUserService<TUser>
        where TUser : BaseUser
    {
        protected readonly IRepository<TUser> _repository;
        // Validerings-regex kan blive her, da de bruges i ValidateUser, som kaldes af Create/Update.
        protected static readonly Regex EmailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);
        protected static readonly Regex PhoneRegex = new Regex(@"^(\+45\s?)?[0-9]{8}$", RegexOptions.Compiled); // Matcher danske numre

        protected BaseUserService(IRepository<TUser> repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public virtual async Task<IEnumerable<TUser>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public virtual async Task<TUser?> GetByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentOutOfRangeException(nameof(id), "ID skal være større end 0.");
            return await _repository.GetByIdAsync(id);
        }

        public virtual async Task<TUser> CreateAsync(TUser user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            ValidateUser(user); // Generel validering

            // Unik email check - dette kræver en GetByEmail metode. 
            // Overvej om denne logik skal være i den konkrete service eller om IRepository skal udvides.
            // For nu antages det at konkrete services håndterer unikke tjek, der kræver specifikke repo metoder.
            // Hvis Repository<T> i SharedKernel får en FindByPredicate, kan det bruges her.
            // Eksempel på hvordan det *kunne* gøres, hvis repository understøtter det (men IRepository gør ikke pt):
            // var existing = (await _repository.FindAsync(u => u.Email.Equals(user.Email, StringComparison.OrdinalIgnoreCase))).FirstOrDefault();
            // if (existing != null) throw new RepositoryException($"En bruger med emailen '{user.Email}' eksisterer allerede.");

            // RegistrationDate sættes af BaseUser's konstruktør
            return await _repository.AddAsync(user); 
        }

        public virtual async Task<TUser> UpdateAsync(TUser user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            ValidateUser(user); // Generel validering

            var existingUser = await _repository.GetByIdAsync(user.Id);
            if (existingUser == null)
            {
                throw new KeyNotFoundException($"Ingen bruger fundet med ID: {user.Id} til opdatering.");
            }
            // Bevar oprindelig registreringsdato
            user.RegistrationDate = existingUser.RegistrationDate;

            // Unik email check ved ændring - samme problematik som i CreateAsync.
            // if (!existingUser.Email.Equals(user.Email, StringComparison.OrdinalIgnoreCase)) {
            //     var conflicting = (await _repository.FindAsync(u => u.Email.Equals(user.Email, StringComparison.OrdinalIgnoreCase) && u.Id != user.Id)).FirstOrDefault();
            //     if (conflicting != null) throw new RepositoryException($"En anden bruger med emailen '{user.Email}' eksisterer allerede.");
            // }

            return await _repository.UpdateAsync(user);
        }

        public virtual async Task DeleteAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentOutOfRangeException(nameof(id),"ID skal være større end 0.");
            await _repository.DeleteAsync(id); // IRepository.DeleteAsync udfører soft delete
        }

        // Disse metoder kræver specifikke repository-implementeringer og fjernes fra BaseUserService.
        // De skal implementeres i de konkrete service-klasser (f.eks. CustomerService).
        public abstract Task<IEnumerable<TUser>> GetByNameAsync(string name); 
        public abstract Task<TUser?> GetByEmailAsync(string email);
        public abstract Task<IEnumerable<TUser>> GetByPhoneAsync(string phone);
        public abstract Task<IEnumerable<TUser>> GetByRegistrationDateRangeAsync(DateTime startDate, DateTime endDate);
        public abstract Task<IEnumerable<TUser>> SearchAsync(string searchTerm);

        /// <summary>
        /// Validerer en bruger (kan overskrives i subklasser for specifik validering)
        /// </summary>
        protected virtual void ValidateUser(TUser user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            if (string.IsNullOrWhiteSpace(user.FirstName))
                throw new ArgumentException("Fornavn kan ikke være tomt.", nameof(user.FirstName));
            if (string.IsNullOrWhiteSpace(user.LastName))
                throw new ArgumentException("Efternavn kan ikke være tomt.", nameof(user.LastName));
            if (string.IsNullOrWhiteSpace(user.Email))
                throw new ArgumentException("Email kan ikke være tom.", nameof(user.Email));
            if (!EmailRegex.IsMatch(user.Email))
                throw new ArgumentException("Ugyldigt email format.", nameof(user.Email));
            if (string.IsNullOrWhiteSpace(user.Phone))
                throw new ArgumentException("Telefonnummer kan ikke være tomt.", nameof(user.Phone));
            if (!PhoneRegex.IsMatch(user.Phone.Replace(" ","")))
                throw new ArgumentException("Ugyldigt telefonnummer format. Forventer 8 cifre, evt. med +45.", nameof(user.Phone));
            
            // RegistrationDate sættes i BaseUser constructor til UtcNow. Check for fremtidig dato er stadig relevant.
            if (user.RegistrationDate > DateTime.UtcNow.AddMinutes(1)) // Tillad lille udsving pga. tidsforskel
                 throw new ArgumentOutOfRangeException(nameof(user.RegistrationDate), "Registreringsdato kan ikke være i fremtiden.");
        }
    }
} 