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
        Pending,

        /// <summary>
        /// Adoptionen er godkendt
        /// </summary>
        Approved,

        /// <summary>
        /// Adoptionen er afvist
        /// </summary>
        Rejected,

        /// <summary>
        /// Adoptionen er gennemført
        /// </summary>
        Completed,

        /// <summary>
        /// Adoptionen er annulleret
        /// </summary>
        Cancelled
    }
} 