using System;

namespace ClassLibrary.SharedKernel.Domain.Abstractions
{
    /// <summary>
    /// Interface for enheder der kan soft-deletes
    /// </summary>
    public interface ISoftDelete
    {
        /// <summary>
        /// Angiver om enheden er slettet
        /// </summary>
        bool IsDeleted { get; set; }

        /// <summary>
        /// Dato for hvornår enheden blev slettet
        /// </summary>
        DateTime? DeletedAt { get; set; }
    }
} 