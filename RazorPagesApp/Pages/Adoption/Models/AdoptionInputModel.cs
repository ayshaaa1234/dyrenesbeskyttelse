using System.ComponentModel.DataAnnotations;

namespace RazorPagesApp.Pages.Adoption.Models
{
    public class AdoptionInputModel
    {
        [Required]
        public int AnimalId { get; set; }

        public string? AnimalName { get; set; } // Bruges til visning på siden

        [Required(ErrorMessage = "Navn er påkrævet.")]
        [StringLength(100, ErrorMessage = "Navn må højst være 100 tegn.")]
        public string CustomerName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email er påkrævet.")]
        [EmailAddress(ErrorMessage = "Ugyldig email-adresse.")]
        public string CustomerEmail { get; set; } = string.Empty;

        [Required(ErrorMessage = "Telefonnummer er påkrævet.")]
        [Phone(ErrorMessage = "Ugyldigt telefonnummer.")]
        public string CustomerPhone { get; set; } = string.Empty;
        
        [StringLength(1000, ErrorMessage = "Note må højst være 1000 tegn.")]
        public string? Notes { get; set; }
    }
} 