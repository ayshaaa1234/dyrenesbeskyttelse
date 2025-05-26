using System.ComponentModel.DataAnnotations;

namespace ClassLibrary.Features.AnimalManagement.Core.Enums
{
    /// <summary>
    /// Repræsenterer de forskellige statusser et besøg kan have.
    /// </summary>
    public enum VisitStatus
    {
        /// <summary>
        /// Besøget er planlagt, men endnu ikke bekræftet.
        /// </summary>
        [Display(Name = "Planlagt")] Scheduled,
        /// <summary>
        /// Besøget er blevet bekræftet af relevante parter.
        /// </summary>
        [Display(Name = "Bekræftet")] Confirmed,
        /// <summary>
        /// Besøget er blevet gennemført.
        /// </summary>
        [Display(Name = "Gennemført")] Completed,
        /// <summary>
        /// Besøget er blevet annulleret.
        /// </summary>
        [Display(Name = "Annulleret")] Cancelled,
        /// <summary>
        /// Besøget er på en venteliste, typisk pga. fuld booking.
        /// </summary>
        [Display(Name = "Venteliste")] Waitlisted
    }
} 