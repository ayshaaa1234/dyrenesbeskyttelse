namespace ClassLibrary.SharedKernel.Domain.Abstractions
{
    /// <summary>
    /// Interface for enheder med et ID
    /// </summary>
    public interface IEntity
    {
        /// <summary>
        /// Unikt ID for enheden
        /// </summary>
        int Id { get; set; }
    }
} 