using System;
using ClassLibrary.SharedKernel.Domain.Abstractions; // For IEntity, ISoftDelete
using ClassLibrary.Features.AnimalManagement.Core.Enums; // For VisitStatus

namespace ClassLibrary.Features.AnimalManagement.Core.Models
{
    /// <summary>
    /// Repræsenterer et besøg i dyrenes beskyttelse, der kan være både planlagt og gennemført
    /// </summary>
    public class Visit : IEntity, ISoftDelete
    {
        private string _type = string.Empty;
        private string _purpose = string.Empty;
        private string _visitor = string.Empty;
        private string _description = string.Empty;
        private string _notes = string.Empty;

        /// <summary>
        /// Unikt ID for besøget
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// ID for det besøgte dyr
        /// </summary>
        public int AnimalId { get; set; }

        /// <summary>
        /// ID for den besøgende kunde
        /// </summary>
        public int? CustomerId { get; set; }

        /// <summary>
        /// ID for den ansvarlige medarbejder
        /// </summary>
        public int? EmployeeId { get; set; }

        /// <summary>
        /// Planlagt dato og tid for besøget
        /// </summary>
        public DateTime PlannedDate { get; set; }

        /// <summary>
        /// Faktisk dato og tid for besøget (null hvis ikke gennemført)
        /// </summary>
        public DateTime? ActualDate { get; set; }

        /// <summary>
        /// Planlagt varighed af besøget i minutter
        /// </summary>
        public int PlannedDuration { get; set; }

        /// <summary>
        /// Faktisk varighed af besøget i minutter (null hvis ikke gennemført)
        /// </summary>
        public int? ActualDuration { get; set; }

        /// <summary>
        /// Type af besøg
        /// </summary>
        public string Type 
        { 
            get => _type;
            set => _type = value ?? string.Empty;
        }

        /// <summary>
        /// Formål med besøget
        /// </summary>
        public string Purpose 
        { 
            get => _purpose;
            set => _purpose = value ?? string.Empty;
        }

        /// <summary>
        /// Navn på besøgeren
        /// </summary>
        public string Visitor 
        { 
            get => _visitor;
            set => _visitor = value ?? string.Empty;
        }

        /// <summary>
        /// Beskrivelse af besøget
        /// </summary>
        public string Description 
        { 
            get => _description;
            set => _description = value ?? string.Empty;
        }

        /// <summary>
        /// Status på besøget (fx planlagt, bekræftet, aflyst, gennemført)
        /// </summary>
        public Enums.VisitStatus Status { get; set; } // Opdateret til Enums.VisitStatus

        /// <summary>
        /// Angiver om besøget var et lægebesøg
        /// </summary>
        public bool IsVeterinaryVisit { get; set; }

        /// <summary>
        /// Angiver om besøget resulterede i en adoption
        /// </summary>
        public bool ResultedInAdoption { get; set; }

        /// <summary>
        /// Noter tilknyttet besøget
        /// </summary>
        public string Notes 
        { 
            get => _notes;
            set => _notes = value ?? string.Empty;
        }

        /// <summary>
        /// Angiver om besøget er slettet
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// Dato for hvornår besøget blev slettet
        /// </summary>
        public DateTime? DeletedAt { get; set; }

        /// <summary>
        /// Konstruktør
        /// </summary>
        public Visit()
        {
            PlannedDate = DateTime.Now;
            Status = Enums.VisitStatus.Scheduled; // Opdateret til Enums.VisitStatus
            PlannedDuration = 30; // Standard varighed på 30 minutter
            IsVeterinaryVisit = false;
            ResultedInAdoption = false;
        }

        /// <summary>
        /// Konstruktør med varighed
        /// </summary>
        public Visit(int duration)
        {
            PlannedDate = DateTime.Now;
            Status = Enums.VisitStatus.Scheduled; // Opdateret til Enums.VisitStatus
            PlannedDuration = duration;
            IsVeterinaryVisit = false;
            ResultedInAdoption = false;
        }
    }
} 