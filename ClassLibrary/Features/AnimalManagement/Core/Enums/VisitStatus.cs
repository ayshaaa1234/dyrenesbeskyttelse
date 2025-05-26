using System.ComponentModel.DataAnnotations;

namespace ClassLibrary.Features.AnimalManagement.Core.Enums
{
    public enum VisitStatus
    {
        [Display(Name = "Planlagt")] Scheduled,
        [Display(Name = "Bekræftet")] Confirmed,
        [Display(Name = "Gennemført")] Completed,
        [Display(Name = "Annulleret")] Cancelled,
        [Display(Name = "Venteliste")] Waitlisted
    }
} 