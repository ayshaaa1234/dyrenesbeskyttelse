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
        public Animal? Animal { get; set; }
        public HealthRecord? LatestHealthRecord { get; set; }
        public DateTime? NextVaccinationDate { get; set; }
        public List<Visit> UpcomingVisits { get; set; }
        public List<Visit> PastVisits { get; set; }
        public bool NeedsVaccination { get; set; }
        public string HealthStatus { get; set; } = string.Empty; // Sikrer initialisering
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