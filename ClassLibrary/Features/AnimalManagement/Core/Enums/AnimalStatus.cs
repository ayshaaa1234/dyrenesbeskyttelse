using System.ComponentModel.DataAnnotations;

namespace ClassLibrary.Features.AnimalManagement.Core.Enums
{
    public enum AnimalStatus
    {
        [Display(Name = "Tilgængelig")] Available,
        [Display(Name = "Adopteret")] Adopted,
        [Display(Name = "Reserveret")] Reserved,
        [Display(Name = "Under behandling")] InTreatment,
        [Display(Name = "Afdød")] Deceased
    }
} 