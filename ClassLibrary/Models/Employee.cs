using System;
using System.Collections.Generic;
using ClassLibrary.Interfaces;

namespace ClassLibrary.Models
{
    /// <summary>
    /// Repræsenterer en medarbejder i systemet
    /// </summary>
    public class Employee : IEntity, ISoftDelete
    {
        /// <summary>
        /// Unikt ID for medarbejderen
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Medarbejderens fornavn
        /// </summary>
        public string FirstName { get; set; } = string.Empty;

        /// <summary>
        /// Medarbejderens efternavn
        /// </summary>
        public string LastName { get; set; } = string.Empty;

        /// <summary>
        /// Medarbejderens fulde navn
        /// </summary>
        public string Name => $"{FirstName} {LastName}".Trim();

        /// <summary>
        /// Medarbejderens email
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Medarbejderens telefonnummer
        /// </summary>
        public string Phone { get; set; } = string.Empty;

        /// <summary>
        /// Medarbejderens stilling
        /// </summary>
        public string Position { get; set; } = string.Empty;

        /// <summary>
        /// Medarbejderens afdeling
        /// </summary>
        public string Department { get; set; } = string.Empty;

        /// <summary>
        /// Medarbejderens specialisering
        /// </summary>
        public string Specialization { get; set; } = string.Empty;

        /// <summary>
        /// Liste over medarbejderens specialiseringer
        /// </summary>
        public List<string> Specializations { get; set; } = new List<string>();

        /// <summary>
        /// Medarbejderens løn
        /// </summary>
        public decimal Salary { get; set; }

        /// <summary>
        /// Dato for hvornår medarbejderen blev ansat
        /// </summary>
        public DateTime HireDate { get; set; }

        /// <summary>
        /// Angiver om medarbejderen er aktiv
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Liste over medarbejderens sundhedsjournaler
        /// </summary>
        public List<HealthRecord> HealthRecords { get; set; }

        /// <summary>
        /// Liste over medarbejderens besøg
        /// </summary>
        public List<VisitLog> VisitLogs { get; set; }

        /// <summary>
        /// Liste over medarbejderens aktiviteter
        /// </summary>
        public List<Activity> Activities { get; set; }

        /// <summary>
        /// Liste over medarbejderens blogindlæg
        /// </summary>
        public List<BlogPost> BlogPosts { get; set; }

        /// <summary>
        /// Angiver om medarbejderen er slettet
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// Dato for hvornår medarbejderen blev slettet
        /// </summary>
        public DateTime? DeletedAt { get; set; }

        /// <summary>
        /// Konstruktør
        /// </summary>
        public Employee()
        {
            HealthRecords = new List<HealthRecord>();
            VisitLogs = new List<VisitLog>();
            Activities = new List<Activity>();
            BlogPosts = new List<BlogPost>();
            Specializations = new List<string>();
            IsActive = true;
        }
    }
} 