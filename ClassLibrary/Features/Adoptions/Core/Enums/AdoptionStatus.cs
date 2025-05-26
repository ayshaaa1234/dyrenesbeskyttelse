using System.ComponentModel.DataAnnotations;

namespace ClassLibrary.Features.Adoptions.Core.Enums
{
    /// <summary>
    /// Repræsenterer status for en adoption
    /// </summary>
    public enum AdoptionStatus
    {
        /// <summary>
        /// Adoptionen er under behandling
        /// </summary>
        [Display(Name = "Afventer behandling")]
        Pending,

        /// <summary>
        /// Adoptionen er godkendt
        /// </summary>
        [Display(Name = "Godkendt")]
        Approved,

        /// <summary>
        /// Adoptionen er afvist
        /// </summary>
        [Display(Name = "Afvist")]
        Rejected,

        /// <summary>
        /// Adoptionen er gennemført
        /// </summary>
        [Display(Name = "Gennemført")]
        Completed,

        /// <summary>
        /// Adoptionen er annulleret
        /// </summary>
        [Display(Name = "Annulleret")]
        Cancelled
    }
} 