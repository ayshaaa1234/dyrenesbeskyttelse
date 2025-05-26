using System;
using System.ComponentModel.DataAnnotations;
using ClassLibrary.SharedKernel.Domain.Abstractions; // For IEntity, ISoftDelete

namespace ClassLibrary.SharedKernel.Domain.Models
{
    public abstract class BaseUser : IEntity, ISoftDelete
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Fornavn er påkrævet.")]
        [StringLength(50, ErrorMessage = "Fornavn må højst være 50 tegn.")]
        [Display(Name = "Fornavn")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Efternavn er påkrævet.")]
        [StringLength(50, ErrorMessage = "Efternavn må højst være 50 tegn.")]
        [Display(Name = "Efternavn")]
        public string LastName { get; set; } = string.Empty;

        [Display(Name = "Fulde navn")]
        public virtual string FullName => $"{FirstName} {LastName}".Trim();

        [Required(ErrorMessage = "Email er påkrævet.")]
        [EmailAddress(ErrorMessage = "Ugyldig email-adresse.")]
        [StringLength(100, ErrorMessage = "Email må højst være 100 tegn.")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [StringLength(20, ErrorMessage = "Telefonnummer må højst være 20 tegn.")]
        [Display(Name = "Telefon")]
        public string? Phone { get; set; }

        /// <summary>
        /// Dato for hvornår brugeren blev registreret i systemet.
        /// For en Employee kan dette initielt sættes til HireDate.
        /// </summary>
        [Display(Name = "Registreringsdato")]
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