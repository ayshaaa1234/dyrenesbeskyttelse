namespace ClassLibrary.Features.Memberships.Core.Enums
{
    /// <summary>
    /// Angiver de forskellige typer af betalingsmetoder, der kan anvendes.
    /// </summary>
    public enum PaymentMethodType
    {
        /// <summary>
        /// Betalingsmetode er ikke angivet eller ukendt.
        /// </summary>
        NotSet,
        /// <summary>
        /// Betaling via MobilePay.
        /// </summary>
        MobilePay,
        /// <summary>
        /// Betaling via Betalingsservice (Leverandørservice/Nets).
        /// </summary>
        Betalingsservice, // Betalingsservice (Leverandørservice/Nets)
        /// <summary>
        /// Betaling med betalingskort (f.eks. Dankort, Visa, Mastercard).
        /// </summary>
        PaymentCard,
        /// <summary>
        /// Betaling via bankoverførsel.
        /// </summary>
        BankTransfer,
        /// <summary>
        /// Anden uspecificeret betalingsmetode.
        /// </summary>
        Other
    }
} 