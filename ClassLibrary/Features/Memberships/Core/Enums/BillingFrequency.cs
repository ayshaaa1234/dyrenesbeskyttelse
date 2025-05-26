namespace ClassLibrary.Features.Memberships.Core.Enums
{
    /// <summary>
    /// Angiver frekvensen for fakturering eller fornyelse af et medlemskab.
    /// </summary>
    public enum BillingFrequency
    {
        /// <summary>
        /// Ingen specificeret faktureringsfrekvens.
        /// </summary>
        None,
        /// <summary>
        /// Faktureres eller fornyes månedligt.
        /// </summary>
        Monthly,
        /// <summary>
        /// Faktureres eller fornyes kvartalsvist.
        /// </summary>
        Quarterly,
        /// <summary>
        /// Faktureres eller fornyes årligt.
        /// </summary>
        Annually,
        /// <summary>
        /// Faktureres eller fornyes årligt (alternativ stavemåde for Annually).
        /// </summary>
        Yearly,
        /// <summary>
        /// Faktureres eller fornyes hvert andet år.
        /// </summary>
        Biennially,
        /// <summary>
        /// Engangsbetaling, typisk for donationer der ikke er et løbende medlemskab.
        /// </summary>
        OneTime // For donationer der ikke er et fast medlemskab
    }
} 