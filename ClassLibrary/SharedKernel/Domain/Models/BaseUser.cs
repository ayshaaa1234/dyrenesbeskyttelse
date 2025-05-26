using System;
using ClassLibrary.SharedKernel.Domain.Abstractions; // For IEntity, ISoftDelete

namespace ClassLibrary.SharedKernel.Domain.Models
{
    public abstract class BaseUser : IEntity, ISoftDelete
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public virtual string Name => $"{FirstName} {LastName}".Trim();
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;

        /// <summary>
        /// Dato for hvornår brugeren blev registreret i systemet.
        /// For en Employee kan dette initielt sættes til HireDate.
        /// </summary>
        public DateTime RegistrationDate { get; set; }

        // Fra ISoftDelete
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }

        protected BaseUser()
        {
            IsDeleted = false;
            RegistrationDate = DateTime.UtcNow; // Standardiseret til UtcNow
        }
    }
} 