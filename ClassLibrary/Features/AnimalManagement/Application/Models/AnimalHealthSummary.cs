using System;
using System.Collections.Generic;
using ClassLibrary.Features.AnimalManagement.Core.Models; // For Animal, HealthRecord, Visit

namespace ClassLibrary.Features.AnimalManagement.Application.Models // Ændret namespace til Application laget
{
    /// <summary>
    /// Repræsenterer en samlet oversigt over et dyrs sundhed og besøg
    /// </summary>
    public class AnimalHealthSummary
    {
        /// <summary>
        /// Det pågældende dyr.
        /// </summary>
        public Animal? Animal { get; set; }
        /// <summary>
        /// Den seneste sundhedsjournal for dyret.
        /// </summary>
        public HealthRecord? LatestHealthRecord { get; set; }
        /// <summary>
        /// Dato for næste planlagte vaccination, hvis relevant.
        /// </summary>
        public DateTime? NextVaccinationDate { get; set; }
        /// <summary>
        /// Liste over kommende planlagte besøg for dyret.
        /// </summary>
        public List<Visit> UpcomingVisits { get; set; }
        /// <summary>
        /// Liste over tidligere gennemførte eller aflyste besøg for dyret.
        /// </summary>
        public List<Visit> PastVisits { get; set; }
        /// <summary>
        /// Angiver om dyret har behov for vaccination.
        /// </summary>
        public bool NeedsVaccination { get; set; }
        /// <summary>
        /// En generel beskrivelse af dyrets nuværende sundhedsstatus.
        /// </summary>
        public string HealthStatus { get; set; } = string.Empty; // Sikrer initialisering
        /// <summary>
        /// Liste over eventuelle sundhedsadvarsler eller vigtige noter vedrørende dyrets helbred.
        /// </summary>
        public List<string> HealthAlerts { get; set; }

        public AnimalHealthSummary()
        {
            UpcomingVisits = new List<Visit>();
            PastVisits = new List<Visit>();
            // HealthStatus er allerede initialiseret
            HealthAlerts = new List<string>();
        }
    }
} 