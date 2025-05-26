using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using ClassLibrary.Services;
using ClassLibrary.Models;

namespace ConsoleApp.Menus
{
    /// <summary>
    /// Menu til håndtering af sundhedsjournaler
    /// </summary>
    public class HealthRecordMenu : MenuBase
    {
        private readonly HealthRecordService _healthRecordService;

        public HealthRecordMenu(HealthRecordService healthRecordService)
        {
            _healthRecordService = healthRecordService;
        }

        public override async Task ShowAsync()
        {
            while (true)
            {
                ShowHeader("Sundhedsjournal");
                Console.WriteLine("1. Vis alle sundhedsjournaler");
                Console.WriteLine("2. Vis sundhedsjournaler for dyr");
                Console.WriteLine("3. Vis sundhedsjournaler efter dato");
                Console.WriteLine("4. Vis sundhedsjournaler efter diagnose");
                Console.WriteLine("5. Vis sundhedsjournaler efter behandling");
                Console.WriteLine("6. Vis vaccinerede dyr");
                Console.WriteLine("7. Vis ikke-vaccinerede dyr");
                Console.WriteLine("8. Vis sundhedsjournaler efter medicin");
                Console.WriteLine("9. Vis sundhedsjournaler efter alvorlighedsgrad");
                Console.WriteLine("10. Vis sundhedsjournaler efter dyrlæge");
                Console.WriteLine("11. Vis dyr der mangler vaccination");
                Console.WriteLine("12. Opret ny sundhedsjournal");
                Console.WriteLine("13. Opdater sundhedsjournal");
                Console.WriteLine("14. Slet sundhedsjournal");
                Console.WriteLine("15. Registrer vaccination");
                Console.WriteLine("16. Tilføj medicin");
                Console.WriteLine("17. Opdater alvorlighedsgrad");
                Console.WriteLine("18. Planlæg aftale");
                Console.WriteLine("0. Tilbage til hovedmenu");
                Console.Write("\nVælg en mulighed: ");

                var choice = Console.ReadLine();

                try
                {
                    switch (choice)
                    {
                        case "1":
                            await ShowAllHealthRecords();
                            break;
                        case "2":
                            await ShowHealthRecordsByAnimal();
                            break;
                        case "3":
                            await ShowHealthRecordsByDate();
                            break;
                        case "4":
                            await ShowHealthRecordsByDiagnosis();
                            break;
                        case "5":
                            await ShowHealthRecordsByTreatment();
                            break;
                        case "6":
                            await ShowVaccinatedAnimals();
                            break;
                        case "7":
                            await ShowUnvaccinatedAnimals();
                            break;
                        case "8":
                            await ShowHealthRecordsByMedication();
                            break;
                        case "9":
                            await ShowHealthRecordsBySeverity();
                            break;
                        case "10":
                            await ShowHealthRecordsByVeterinarian();
                            break;
                        case "11":
                            await ShowAnimalsNeedingVaccination();
                            break;
                        case "12":
                            await CreateNewHealthRecord();
                            break;
                        case "13":
                            await UpdateHealthRecord();
                            break;
                        case "14":
                            await DeleteHealthRecord();
                            break;
                        case "15":
                            await RegisterVaccination();
                            break;
                        case "16":
                            await AddMedication();
                            break;
                        case "17":
                            await UpdateSeverity();
                            break;
                        case "18":
                            await ScheduleAppointment();
                            break;
                        case "0":
                            return;
                        default:
                            ShowError("Ugyldigt valg");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    HandleException(ex);
                }
            }
        }

        private async Task ShowAllHealthRecords()
        {
            ShowHeader("Alle sundhedsjournaler");
            var records = await _healthRecordService.GetAllHealthRecordsAsync();
            DisplayHealthRecords(records);
        }

        private async Task ShowHealthRecordsByAnimal()
        {
            ShowHeader("Sundhedsjournaler for dyr");
            Console.Write("Indtast dyrets ID: ");
            if (!int.TryParse(Console.ReadLine(), out int animalId))
            {
                ShowError("Ugyldigt dyre ID");
                return;
            }

            var records = await _healthRecordService.GetHealthRecordsByAnimalIdAsync(animalId);
            DisplayHealthRecords(records);
        }

        private async Task ShowHealthRecordsByDate()
        {
            ShowHeader("Sundhedsjournaler efter dato");
            Console.Write("Indtast startdato (dd/mm/yyyy): ");
            if (!DateTime.TryParse(Console.ReadLine(), out DateTime startDate))
            {
                ShowError("Ugyldig startdato");
                return;
            }

            Console.Write("Indtast slutdato (dd/mm/yyyy): ");
            if (!DateTime.TryParse(Console.ReadLine(), out DateTime endDate))
            {
                ShowError("Ugyldig slutdato");
                return;
            }

            var records = await _healthRecordService.GetHealthRecordsByDateRangeAsync(startDate, endDate);
            DisplayHealthRecords(records);
        }

        private async Task ShowHealthRecordsByDiagnosis()
        {
            ShowHeader("Sundhedsjournaler efter diagnose");
            Console.Write("Indtast diagnose: ");
            var diagnosis = Console.ReadLine() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(diagnosis))
            {
                ShowError("Diagnose kan ikke være tom");
                return;
            }

            var records = await _healthRecordService.GetHealthRecordsByDiagnosisAsync(diagnosis);
            DisplayHealthRecords(records);
        }

        private async Task ShowHealthRecordsByTreatment()
        {
            ShowHeader("Sundhedsjournaler efter behandling");
            Console.Write("Indtast behandling: ");
            var treatment = Console.ReadLine() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(treatment))
            {
                ShowError("Behandling kan ikke være tom");
                return;
            }

            var records = await _healthRecordService.GetHealthRecordsByTreatmentAsync(treatment);
            DisplayHealthRecords(records);
        }

        private async Task ShowVaccinatedAnimals()
        {
            ShowHeader("Vaccinerede dyr");
            var records = await _healthRecordService.GetHealthRecordsByVaccinationStatusAsync(true);
            DisplayHealthRecords(records);
        }

        private async Task ShowUnvaccinatedAnimals()
        {
            ShowHeader("Ikke-vaccinerede dyr");
            var records = await _healthRecordService.GetHealthRecordsByVaccinationStatusAsync(false);
            DisplayHealthRecords(records);
        }

        private async Task ShowHealthRecordsByMedication()
        {
            ShowHeader("Sundhedsjournaler efter medicin");
            Console.Write("Indtast medicin: ");
            var medication = Console.ReadLine() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(medication))
            {
                ShowError("Medicin kan ikke være tom");
                return;
            }

            var records = await _healthRecordService.GetHealthRecordsByMedicationAsync(medication);
            DisplayHealthRecords(records);
        }

        private async Task ShowHealthRecordsBySeverity()
        {
            ShowHeader("Sundhedsjournaler efter alvorlighedsgrad");
            Console.Write("Indtast alvorlighedsgrad: ");
            var severity = Console.ReadLine() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(severity))
            {
                ShowError("Alvorlighedsgrad kan ikke være tom");
                return;
            }

            var records = await _healthRecordService.GetHealthRecordsBySeverityAsync(severity);
            DisplayHealthRecords(records);
        }

        private async Task ShowHealthRecordsByVeterinarian()
        {
            ShowHeader("Sundhedsjournaler efter dyrlæge");
            Console.Write("Indtast dyrlægenavn: ");
            var veterinarian = Console.ReadLine() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(veterinarian))
            {
                ShowError("Dyrlægenavn kan ikke være tomt");
                return;
            }

            var records = await _healthRecordService.GetHealthRecordsByVeterinarianAsync(veterinarian);
            DisplayHealthRecords(records);
        }

        private async Task ShowAnimalsNeedingVaccination()
        {
            ShowHeader("Dyr der mangler vaccination");
            var records = await _healthRecordService.GetHealthRecordsNeedingVaccinationAsync();
            DisplayHealthRecords(records);
        }

        private async Task CreateNewHealthRecord()
        {
            ShowHeader("Opret ny sundhedsjournal");

            Console.Write("Indtast dyrets ID: ");
            if (!int.TryParse(Console.ReadLine(), out int animalId))
            {
                ShowError("Ugyldigt dyre ID");
                return;
            }

            Console.Write("Indtast dyrlægenavn: ");
            var veterinarianName = Console.ReadLine() ?? string.Empty;

            Console.Write("Indtast diagnose: ");
            var diagnosis = Console.ReadLine() ?? string.Empty;

            Console.Write("Indtast behandling: ");
            var treatment = Console.ReadLine() ?? string.Empty;

            Console.Write("Indtast medicin: ");
            var medication = Console.ReadLine() ?? string.Empty;

            Console.Write("Indtast vægt (kg): ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal weight))
            {
                ShowError("Ugyldig vægt");
                return;
            }

            Console.Write("Indtast noter: ");
            var notes = Console.ReadLine() ?? string.Empty;

            Console.Write("Indtast alvorlighedsgrad: ");
            var severity = Console.ReadLine() ?? string.Empty;

            var healthRecord = new HealthRecord
            {
                AnimalId = animalId,
                VeterinarianName = veterinarianName,
                Diagnosis = diagnosis,
                Treatment = treatment,
                Medication = medication,
                Weight = weight,
                Notes = notes,
                Severity = severity,
                RecordDate = DateTime.Now
            };

            await _healthRecordService.CreateHealthRecordAsync(healthRecord);
            ShowSuccess("Sundhedsjournal oprettet succesfuldt!");
        }

        private async Task UpdateHealthRecord()
        {
            ShowHeader("Opdater sundhedsjournal");

            Console.Write("Indtast sundhedsjournal ID: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                ShowError("Ugyldigt ID");
                return;
            }

            var healthRecord = await _healthRecordService.GetHealthRecordByIdAsync(id);
            if (healthRecord == null)
            {
                ShowError("Sundhedsjournal ikke fundet");
                return;
            }

            Console.WriteLine("\nNuværende information:");
            DisplayHealthRecordInfo(healthRecord);
            Console.WriteLine("\nIndtast ny information (tryk Enter for at beholde nuværende værdi):");

            Console.Write($"Dyrlægenavn [{healthRecord.VeterinarianName}]: ");
            var veterinarianName = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(veterinarianName))
                healthRecord.VeterinarianName = veterinarianName;

            Console.Write($"Diagnose [{healthRecord.Diagnosis}]: ");
            var diagnosis = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(diagnosis))
                healthRecord.Diagnosis = diagnosis;

            Console.Write($"Behandling [{healthRecord.Treatment}]: ");
            var treatment = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(treatment))
                healthRecord.Treatment = treatment;

            Console.Write($"Medicin [{healthRecord.Medication}]: ");
            var medication = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(medication))
                healthRecord.Medication = medication;

            Console.Write($"Vægt [{healthRecord.Weight}]: ");
            var weightStr = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(weightStr) && decimal.TryParse(weightStr, out decimal weight))
                healthRecord.Weight = weight;

            Console.Write($"Noter [{healthRecord.Notes}]: ");
            var notes = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(notes))
                healthRecord.Notes = notes;

            Console.Write($"Alvorlighedsgrad [{healthRecord.Severity}]: ");
            var severity = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(severity))
                healthRecord.Severity = severity;

            await _healthRecordService.UpdateHealthRecordAsync(healthRecord);
            ShowSuccess("Sundhedsjournal opdateret succesfuldt!");
        }

        private async Task DeleteHealthRecord()
        {
            ShowHeader("Slet sundhedsjournal");

            Console.Write("Indtast sundhedsjournal ID: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                ShowError("Ugyldigt ID");
                return;
            }

            var healthRecord = await _healthRecordService.GetHealthRecordByIdAsync(id);
            if (healthRecord == null)
            {
                ShowError("Sundhedsjournal ikke fundet");
                return;
            }

            Console.WriteLine("\nEr du sikker på, at du vil slette denne sundhedsjournal?");
            DisplayHealthRecordInfo(healthRecord);
            Console.Write("\nSkriv 'JA' for at bekræfte: ");
            
            if (Console.ReadLine()?.ToUpper() != "JA")
            {
                Console.WriteLine("Sletning annulleret.");
                Console.WriteLine("\nTryk på en tast for at fortsætte...");
                Console.ReadKey();
                return;
            }

            await _healthRecordService.DeleteHealthRecordAsync(id);
            ShowSuccess("Sundhedsjournal slettet succesfuldt!");
        }

        private async Task RegisterVaccination()
        {
            ShowHeader("Registrer vaccination");

            Console.Write("Indtast sundhedsjournal ID: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                ShowError("Ugyldigt ID");
                return;
            }

            Console.Write("Indtast næste vaccinationsdato (dd/mm/yyyy): ");
            if (!DateTime.TryParse(Console.ReadLine(), out DateTime nextVaccinationDate))
            {
                ShowError("Ugyldig dato");
                return;
            }

            await _healthRecordService.RegisterVaccinationAsync(id, nextVaccinationDate);
            ShowSuccess("Vaccination registreret succesfuldt!");
        }

        private async Task AddMedication()
        {
            ShowHeader("Tilføj medicin");

            Console.Write("Indtast sundhedsjournal ID: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                ShowError("Ugyldigt ID");
                return;
            }

            Console.Write("Indtast medicin: ");
            var medication = Console.ReadLine() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(medication))
            {
                ShowError("Medicin kan ikke være tom");
                return;
            }

            await _healthRecordService.AddMedicationAsync(id, medication);
            ShowSuccess("Medicin tilføjet succesfuldt!");
        }

        private async Task UpdateSeverity()
        {
            ShowHeader("Opdater alvorlighedsgrad");

            Console.Write("Indtast sundhedsjournal ID: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                ShowError("Ugyldigt ID");
                return;
            }

            Console.Write("Indtast ny alvorlighedsgrad: ");
            var severity = Console.ReadLine() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(severity))
            {
                ShowError("Alvorlighedsgrad kan ikke være tom");
                return;
            }

            await _healthRecordService.UpdateSeverityAsync(id, severity);
            ShowSuccess("Alvorlighedsgrad opdateret succesfuldt!");
        }

        private async Task ScheduleAppointment()
        {
            ShowHeader("Planlæg aftale");

            Console.Write("Indtast sundhedsjournal ID: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                ShowError("Ugyldigt ID");
                return;
            }

            Console.Write("Indtast aftaledato (dd/mm/yyyy): ");
            if (!DateTime.TryParse(Console.ReadLine(), out DateTime appointmentDate))
            {
                ShowError("Ugyldig dato");
                return;
            }

            await _healthRecordService.ScheduleAppointmentAsync(id, appointmentDate);
            ShowSuccess("Aftale planlagt succesfuldt!");
        }

        private void DisplayHealthRecords(IEnumerable<HealthRecord> healthRecords)
        {
            if (!healthRecords.Any())
            {
                Console.WriteLine("Ingen sundhedsjournaler fundet.");
            }
            else
            {
                foreach (var record in healthRecords)
                {
                    DisplayHealthRecordInfo(record);
                    Console.WriteLine(new string('-', 50));
                }
            }

            Console.WriteLine("\nTryk på en tast for at fortsætte...");
            Console.ReadKey();
        }

        private void DisplayHealthRecordInfo(HealthRecord record)
        {
            Console.WriteLine($"ID: {record.Id}");
            Console.WriteLine($"Dyr ID: {record.AnimalId}");
            Console.WriteLine($"Dato: {record.RecordDate:dd/MM/yyyy HH:mm}");
            Console.WriteLine($"Dyrlæge: {record.VeterinarianName}");
            Console.WriteLine($"Diagnose: {record.Diagnosis}");
            Console.WriteLine($"Behandling: {record.Treatment}");
            Console.WriteLine($"Medicin: {record.Medication}");
            Console.WriteLine($"Vægt: {record.Weight} kg");
            Console.WriteLine($"Noter: {record.Notes}");
            Console.WriteLine($"Alvorlighedsgrad: {record.Severity}");
            Console.WriteLine($"Vaccineret: {(record.IsVaccinated ? "Ja" : "Nej")}");
            if (record.NextVaccinationDate.HasValue)
                Console.WriteLine($"Næste vaccination: {record.NextVaccinationDate.Value:dd/MM/yyyy}");
            if (record.AppointmentDate.HasValue)
                Console.WriteLine($"Aftale: {record.AppointmentDate.Value:dd/MM/yyyy HH:mm}");
        }
    }
}
