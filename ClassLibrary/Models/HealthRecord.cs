using System;
using ClassLibrary.Interfaces;

namespace ClassLibrary.Models
{
    /// <summary>
    /// Repræsenterer en sundhedsjournal for et dyr
    /// </summary>
    public class HealthRecord : IEntity, ISoftDelete
    {
        private decimal _weight;

        /// <summary>
        /// Unikt ID for sundhedsjournalen
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// ID på dyret
        /// </summary>
        public int AnimalId { get; set; }

        /// <summary>
        /// Dato for journalen
        /// </summary>
        public DateTime RecordDate { get; set; }

        /// <summary>
        /// Dato for aftalen
        /// </summary>
        public DateTime? AppointmentDate { get; set; }

        /// <summary>
        /// Navn på dyrlægen
        /// </summary>
        public string VeterinarianName { get; set; } = string.Empty;

        /// <summary>
        /// Diagnose
        /// </summary>
        public string Diagnosis { get; set; } = string.Empty;

        /// <summary>
        /// Behandling
        /// </summary>
        public string Treatment { get; set; } = string.Empty;

        /// <summary>
        /// Medicin
        /// </summary>
        public string Medication { get; set; } = string.Empty;

        /// <summary>
        /// Dyrets vægt i kg
        /// </summary>
        public decimal Weight 
        { 
            get => _weight;
            set
            {
                if (value < 0)
                    throw new ArgumentException("Vægt kan ikke være negativ");
                _weight = value;
            }
        }

        /// <summary>
        /// Noter
        /// </summary>
        public string Notes { get; set; } = string.Empty;

        /// <summary>
        /// Alvorlighedsgrad af tilstanden
        /// </summary>
        public string Severity { get; set; } = string.Empty;

        /// <summary>
        /// Angiver om dyret er vaccineret
        /// </summary>
        public bool IsVaccinated { get; set; }

        /// <summary>
        /// Dato for næste vaccination
        /// </summary>
        public DateTime? NextVaccinationDate { get; set; }

        /// <summary>
        /// Angiver om sundhedsjournalen er slettet
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// Dato for hvornår sundhedsjournalen blev slettet
        /// </summary>
        public DateTime? DeletedAt { get; set; }

        /// <summary>
        /// Konstruktør
        /// </summary>
        public HealthRecord()
        {
            RecordDate = DateTime.Now;
            IsVaccinated = false;
            _weight = 0;
        }

        /// <summary>
        /// Konstruktør med vægt
        /// </summary>
        public HealthRecord(decimal weight)
        {
            RecordDate = DateTime.Now;
            IsVaccinated = false;
            Weight = weight;
        }
    }
} 