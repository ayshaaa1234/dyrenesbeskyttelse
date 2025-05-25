using System;
using ClassLibrary.Interfaces;

namespace ClassLibrary.Models
{
    /// <summary>
    /// Repræsenterer en besøgslog i dyrenes beskyttelse
    /// </summary>
    public class VisitLog : IEntity, ISoftDelete
    {
        private string _description = string.Empty;
        private string _notes = string.Empty;
        private string _visitType = string.Empty;
        private string _visitor = string.Empty;
        private string _purpose = string.Empty;

        /// <summary>
        /// Unikt ID for besøgsloggen
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// ID for det besøgte dyr
        /// </summary>
        public int AnimalId { get; set; }

        /// <summary>
        /// ID for den besøgende kunde
        /// </summary>
        public int CustomerId { get; set; }

        /// <summary>
        /// ID for den ansvarlige medarbejder
        /// </summary>
        public int EmployeeId { get; set; }

        /// <summary>
        /// Dato og tid for besøget
        /// </summary>
        public DateTime VisitDate { get; set; }

        /// <summary>
        /// Type af besøg
        /// </summary>
        public string VisitType 
        { 
            get => _visitType;
            set => _visitType = value ?? string.Empty;
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
        /// Formål med besøget
        /// </summary>
        public string Purpose 
        { 
            get => _purpose;
            set => _purpose = value ?? string.Empty;
        }

        /// <summary>
        /// Varighed af besøget i minutter
        /// </summary>
        public int Duration { get; set; }

        /// <summary>
        /// Beskrivelse af besøget
        /// </summary>
        public string Description 
        { 
            get => _description;
            set => _description = value ?? string.Empty;
        }

        /// <summary>
        /// Angiver om besøget var et lægebesøg
        /// </summary>
        public bool IsVeterinaryVisit { get; set; }

        /// <summary>
        /// Noter fra besøget
        /// </summary>
        public string Notes 
        { 
            get => _notes;
            set => _notes = value ?? string.Empty;
        }

        /// <summary>
        /// Angiver om besøget resulterede i en adoption
        /// </summary>
        public bool ResultedInAdoption { get; set; }

        /// <summary>
        /// Angiver om besøgsloggen er slettet
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// Dato for hvornår besøgsloggen blev slettet
        /// </summary>
        public DateTime? DeletedAt { get; set; }

        /// <summary>
        /// Konstruktør
        /// </summary>
        public VisitLog()
        {
            VisitDate = DateTime.Now;
            IsVeterinaryVisit = false;
            ResultedInAdoption = false;
            Duration = 30; // Standard varighed på 30 minutter
        }

        /// <summary>
        /// Konstruktør med varighed
        /// </summary>
        public VisitLog(int duration)
        {
            VisitDate = DateTime.Now;
            IsVeterinaryVisit = false;
            ResultedInAdoption = false;
            Duration = duration;
        }
    }
} 