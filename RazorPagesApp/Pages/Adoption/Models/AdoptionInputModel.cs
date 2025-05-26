using System.ComponentModel.DataAnnotations;

namespace RazorPagesApp.Pages.Adoption.Models
{
    // Denne klasse repræsenterer inputmodellen for en adoptionsansøgning.
    // Den bruges til at binde data fra ansøgningsformularen og validere input.
    public class AdoptionInputModel
    {
        [Required] // Angiver at AnimalId er påkrævet.
        public int AnimalId { get; set; }

        public string? AnimalName { get; set; } // Bruges til at vise dyrets navn på bekræftelsessider eller i UI.

        [Required(ErrorMessage = "Navn er påkrævet.")] // Valideringsattributter for kundens navn.
        [StringLength(100, ErrorMessage = "Navn må højst være 100 tegn.")]
        public string CustomerName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email er påkrævet.")] // Valideringsattributter for kundens email.
        [EmailAddress(ErrorMessage = "Ugyldig email-adresse.")]
        public string CustomerEmail { get; set; } = string.Empty;

        [Required(ErrorMessage = "Telefonnummer er påkrævet.")] // Valideringsattributter for kundens telefonnummer.
        [Phone(ErrorMessage = "Ugyldigt telefonnummer.")]
        public string CustomerPhone { get; set; } = string.Empty;
        
        [StringLength(1000, ErrorMessage = "Note må højst være 1000 tegn.")] // Validering for noter.
        public string? Notes { get; set; } // Valgfri noter fra ansøgeren.
    }
} 