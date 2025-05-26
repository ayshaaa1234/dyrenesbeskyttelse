using System.ComponentModel.DataAnnotations;

namespace ClassLibrary.Features.AnimalManagement.Core.Enums
{
    /// <summary>
    /// Repræsenterer de forskellige statusser et dyr kan have.
    /// </summary>
    public enum AnimalStatus
    {
        /// <summary>
        /// Dyret er tilgængeligt for adoption eller anden aktivitet.
        /// </summary>
        [Display(Name = "Tilgængelig")] Available,
        /// <summary>
        /// Dyret er blevet adopteret.
        /// </summary>
        [Display(Name = "Adopteret")] Adopted,
        /// <summary>
        /// Dyret er reserveret, typisk i forbindelse med en igangværende adoptionsproces.
        /// </summary>
        [Display(Name = "Reserveret")] Reserved,
        /// <summary>
        /// Dyret modtager behandling eller er under observation.
        /// </summary>
        [Display(Name = "Under behandling")] InTreatment,
        /// <summary>
        /// Dyret er afgået ved døden.
        /// </summary>
        [Display(Name = "Afdød")] Deceased
    }
} 