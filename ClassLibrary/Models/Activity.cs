using System;
using System.Collections.Generic;
using ClassLibrary.Interfaces;

namespace ClassLibrary.Models
{
    /// <summary>
    /// Repræsenterer en aktivitet i dyrenes beskyttelse
    /// </summary>
    public class Activity : IEntity, ISoftDelete
    {
        /// <summary>
        /// Unikt ID for aktiviteten
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Navn på aktiviteten
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Beskrivelse af aktiviteten
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Dato og tid for aktiviteten
        /// </summary>
        public DateTime ActivityDate { get; set; }

        /// <summary>
        /// Varighed af aktiviteten i minutter
        /// </summary>
        public int DurationMinutes { get; set; }

        /// <summary>
        /// Maksimalt antal deltagere
        /// </summary>
        public int MaxParticipants { get; set; }

        /// <summary>
        /// ID for den ansvarlige medarbejder
        /// </summary>
        public int EmployeeId { get; set; }

        /// <summary>
        /// Status for aktiviteten
        /// </summary>
        public ActivityStatus Status { get; set; }

        /// <summary>
        /// Liste over tilmeldte deltagere
        /// </summary>
        public List<int> ParticipantIds { get; set; } = new List<int>();

        /// <summary>
        /// Lokation for aktiviteten
        /// </summary>
        public string Location { get; set; } = string.Empty;

        /// <summary>
        /// Pris for deltagelse
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Angiver om aktiviteten er slettet
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// Dato for hvornår aktiviteten blev slettet
        /// </summary>
        public DateTime? DeletedAt { get; set; }

        /// <summary>
        /// Angiver om aktiviteten er fuldt booket
        /// </summary>
        public bool IsFullyBooked => ParticipantIds.Count >= MaxParticipants;

        /// <summary>
        /// Angiver antal ledige pladser
        /// </summary>
        public int AvailableSpots => MaxParticipants - ParticipantIds.Count;

        /// <summary>
        /// Konstruktør
        /// </summary>
        public Activity()
        {
            ActivityDate = DateTime.Now;
            Status = ActivityStatus.Planned;
            DurationMinutes = 60; // Standard varighed på 1 time
            MaxParticipants = 10; // Standard maksimalt antal deltagere
        }

        /// <summary>
        /// Tjekker om en deltager er tilmeldt aktiviteten
        /// </summary>
        public bool IsParticipantRegistered(int participantId)
        {
            return ParticipantIds.Contains(participantId);
        }

        /// <summary>
        /// Tilføjer en deltager til aktiviteten
        /// </summary>
        public void AddParticipant(int participantId)
        {
            if (IsFullyBooked)
                throw new InvalidOperationException("Aktiviteten er fuldt booket");
            
            if (IsParticipantRegistered(participantId))
                throw new InvalidOperationException("Deltageren er allerede tilmeldt");

            ParticipantIds.Add(participantId);
        }

        /// <summary>
        /// Fjerner en deltager fra aktiviteten
        /// </summary>
        public void RemoveParticipant(int participantId)
        {
            if (!IsParticipantRegistered(participantId))
                throw new InvalidOperationException("Deltageren er ikke tilmeldt");

            ParticipantIds.Remove(participantId);
        }
    }

    /// <summary>
    /// Repræsenterer status for en aktivitet
    /// </summary>
    public enum ActivityStatus
    {
        /// <summary>
        /// Aktiviteten er planlagt
        /// </summary>
        Planned,

        /// <summary>
        /// Aktiviteten er i gang
        /// </summary>
        InProgress,

        /// <summary>
        /// Aktiviteten er afsluttet
        /// </summary>
        Completed,

        /// <summary>
        /// Aktiviteten er aflyst
        /// </summary>
        Cancelled
    }
} 