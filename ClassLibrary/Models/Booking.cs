using System;
using ClassLibrary.Interfaces;

namespace ClassLibrary.Models
{
    /// <summary>
    /// Repræsenterer en booking i dyrenes beskyttelse
    /// </summary>
    public class Booking : IEntity, ISoftDelete
    {
        private string _purpose = string.Empty;
        private string _notes = string.Empty;
        private string _type = string.Empty;

        /// <summary>
        /// Unikt ID for bookingen
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// ID for det bookede dyr
        /// </summary>
        public int AnimalId { get; set; }

        /// <summary>
        /// ID for den kunde der har booket
        /// </summary>
        public int CustomerId { get; set; }

        /// <summary>
        /// ID for den ansvarlige medarbejder
        /// </summary>
        public int EmployeeId { get; set; }

        /// <summary>
        /// Dato og tid for bookingen
        /// </summary>
        public DateTime BookingDate { get; set; }

        /// <summary>
        /// Varighed af bookingen i minutter
        /// </summary>
        public int DurationMinutes { get; set; }

        /// <summary>
        /// Type af booking
        /// </summary>
        public string Type 
        { 
            get => _type;
            set => _type = value ?? string.Empty;
        }

        /// <summary>
        /// Formål med bookingen
        /// </summary>
        public string Purpose 
        { 
            get => _purpose;
            set => _purpose = value ?? string.Empty;
        }

        /// <summary>
        /// Status for bookingen
        /// </summary>
        public BookingStatus Status { get; set; }

        /// <summary>
        /// Noter tilknyttet bookingen
        /// </summary>
        public string Notes 
        { 
            get => _notes;
            set => _notes = value ?? string.Empty;
        }

        /// <summary>
        /// Angiver om bookingen er slettet
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// Dato for hvornår bookingen blev slettet
        /// </summary>
        public DateTime? DeletedAt { get; set; }

        /// <summary>
        /// Konstruktør
        /// </summary>
        public Booking()
        {
            BookingDate = DateTime.Now;
            Status = BookingStatus.Confirmed;
            DurationMinutes = 30; // Standard varighed på 30 minutter
        }

        /// <summary>
        /// Repræsenterer status for en booking
        /// </summary>
        public enum BookingStatus
        {
            /// <summary>
            /// Bookingen er bekræftet
            /// </summary>
            Confirmed,

            /// <summary>
            /// Bookingen er aflyst
            /// </summary>
            Cancelled,

            /// <summary>
            /// Bookingen er afsluttet
            /// </summary>
            Completed,

            /// <summary>
            /// Bookingen er på venteliste
            /// </summary>
            Waitlisted
        }
    }
} 