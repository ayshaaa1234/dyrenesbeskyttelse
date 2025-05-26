using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ClassLibrary.Features.Employees.Core.Models; // For Employee
using ClassLibrary.Features.Employees.Infrastructure.Abstractions; // For IEmployeeRepository
using ClassLibrary.SharedKernel.Persistence.Implementations; // For Repository<T>
using ClassLibrary.SharedKernel.Exceptions; // For RepositoryException
using ClassLibrary.Infrastructure.DataInitialization; // Tilføjet for JsonDataInitializer

namespace ClassLibrary.Features.Employees.Infrastructure.Implementations // Opdateret namespace
{
    /// <summary>
    /// Repository til håndtering af medarbejderdata, gemt i en JSON-fil.
    /// </summary>
    public class EmployeeRepository : Repository<Employee>, IEmployeeRepository
    {
        // Fjernet: private const string FilePath = "Data/Json/employees.json";

        // Regex fra BaseUser kan bruges hvis de er protected static, ellers definer dem her.
        // For nu antager vi, at de er tilgængelige eller gen-defineres efter behov.
        private static readonly Regex EmailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);
        private static readonly Regex PhoneRegex = new Regex(@"^(\+45\s?)?\d{8}$", RegexOptions.Compiled);

        /// <summary>
        /// Initialiserer en ny instans af <see cref="EmployeeRepository"/> klassen.
        /// Stien til JSON-filen bestemmes via <see cref="JsonDataInitializer"/>.
        /// </summary>
        public EmployeeRepository() : base(Path.Combine(JsonDataInitializer.CalculatedWorkspaceRoot, "Data", "Json", "employees.json")) { }

        /// <summary>
        /// Validerer en medarbejderentitet.
        /// </summary>
        /// <param name="entity">Medarbejderen der skal valideres.</param>
        /// <exception cref="ArgumentException">Kastes hvis påkrævede felter (Fornavn, Efternavn, Email, Telefon, Stilling) er tomme eller har ugyldigt format.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Kastes hvis Ansættelsesdato er ugyldig eller Løn er negativ.</exception>
        protected override void ValidateEntity(Employee entity)
        {
            base.ValidateEntity(entity); // Generel null-tjek etc.

            if (string.IsNullOrWhiteSpace(entity.FirstName))
                throw new ArgumentException("Fornavn kan ikke være tomt.", nameof(entity.FirstName));
            if (string.IsNullOrWhiteSpace(entity.LastName))
                throw new ArgumentException("Efternavn kan ikke være tomt.", nameof(entity.LastName));
            if (string.IsNullOrWhiteSpace(entity.Email))
                throw new ArgumentException("Email kan ikke være tom.", nameof(entity.Email));
            if (!EmailRegex.IsMatch(entity.Email))
                throw new ArgumentException("Ugyldigt email format.", nameof(entity.Email));
            if (string.IsNullOrWhiteSpace(entity.Phone))
                throw new ArgumentException("Telefonnummer kan ikke være tomt.", nameof(entity.Phone));
            if (!PhoneRegex.IsMatch(entity.Phone.Replace(" ", "")))
                throw new ArgumentException("Ugyldigt telefonnummer format. Forventer 8 cifre, evt. med +45.", nameof(entity.Phone));
            if (string.IsNullOrWhiteSpace(entity.Position))
                throw new ArgumentException("Stilling kan ikke være tom.", nameof(entity.Position));
            
            if (entity.HireDate == default(DateTime))
                 throw new ArgumentOutOfRangeException(nameof(entity.HireDate), "Ansættelsesdato skal være angivet.");
            if (entity.HireDate > DateTime.UtcNow.AddMinutes(1)) 
                throw new ArgumentOutOfRangeException(nameof(entity.HireDate), "Ansættelsesdato kan ikke være i fremtiden.");

            if (entity.Salary < 0)
                throw new ArgumentOutOfRangeException(nameof(entity.Salary), "Løn kan ikke være negativ.");
        }

        /// <summary>
        /// Tilføjer en ny medarbejder asynkront efter validering og tjek for unik email.
        /// </summary>
        /// <param name="employee">Medarbejderen der skal tilføjes.</param>
        /// <returns>Den tilføjede medarbejder.</returns>
        /// <exception cref="RepositoryException">Kastes hvis en aktiv medarbejder med samme email allerede eksisterer.</exception>
        public override async Task<Employee> AddAsync(Employee employee)
        {
            ValidateEntity(employee);
            if (await EmailExistsForAnotherActiveEmployeeAsync(employee.Email, 0))
                throw new RepositoryException($"En aktiv medarbejder med email {employee.Email} eksisterer allerede.");

            // Id og RegistrationDate sættes af hhv. Repository<T> og BaseUser
            return await base.AddAsync(employee);
        }

        /// <summary>
        /// Opdaterer en eksisterende medarbejder asynkront efter validering og tjek for unik email (hvis ændret).
        /// Oprindelig ansættelsesdato og registreringsdato bevares.
        /// </summary>
        /// <param name="employee">Medarbejderen med de opdaterede værdier.</param>
        /// <returns>Den opdaterede medarbejder.</returns>
        /// <exception cref="KeyNotFoundException">Kastes hvis ingen aktiv medarbejder findes med det angivne ID.</exception>
        /// <exception cref="RepositoryException">Kastes hvis en anden aktiv medarbejder med den nye email allerede eksisterer.</exception>
        public override async Task<Employee> UpdateAsync(Employee employee)
        {
            ValidateEntity(employee);
            var items = await LoadDataAsync();
            var existingEmployee = items.FirstOrDefault(e => e.Id == employee.Id && !e.IsDeleted);

            if (existingEmployee == null) 
                throw new KeyNotFoundException($"Ingen aktiv medarbejder fundet med ID: {employee.Id} for opdatering.");

            if (!existingEmployee.Email.Equals(employee.Email, StringComparison.OrdinalIgnoreCase) &&
                await EmailExistsForAnotherActiveEmployeeAsync(employee.Email, employee.Id))
                throw new RepositoryException($"En anden aktiv medarbejder med email {employee.Email} eksisterer allerede.");

            // Bevar oprindelig HireDate og RegistrationDate
            employee.HireDate = existingEmployee.HireDate;
            employee.RegistrationDate = existingEmployee.RegistrationDate;
            
            return await base.UpdateAsync(employee);
        }

        /// <summary>
        /// Tjekker om en given email allerede eksisterer for en *anden* aktiv medarbejder.
        /// </summary>
        /// <param name="email">Emailen der skal tjekkes.</param>
        /// <param name="currentEmployeeId">ID på den nuværende medarbejder (0 hvis det er en ny medarbejder).</param>
        /// <returns>True hvis emailen findes hos en anden aktiv medarbejder, ellers false.</returns>
        private async Task<bool> EmailExistsForAnotherActiveEmployeeAsync(string email, int currentEmployeeId)
        {
            if (string.IsNullOrWhiteSpace(email) || !EmailRegex.IsMatch(email)) return false;
            
            var employeesWithEmail = await base.FindAsync(e => 
                !string.IsNullOrWhiteSpace(e.Email) && 
                e.Email.Equals(email, StringComparison.OrdinalIgnoreCase)); 
            
            return employeesWithEmail.Any(e => e.Id != currentEmployeeId);
        }

        // Implementering af IEmployeeRepository metoder
        /// <summary>
        /// Henter en aktiv medarbejder baseret på email.
        /// </summary>
        /// <param name="email">Email der søges efter.</param>
        /// <returns>Den fundne medarbejder, eller null hvis ingen aktiv medarbejder matcher emailen.</returns>
        /// <exception cref="ArgumentException">Kastes hvis emailen er tom, null eller i ugyldigt format.</exception>
        public async Task<Employee?> GetByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email) || !EmailRegex.IsMatch(email))
                throw new ArgumentException("Ugyldig eller tom email.");
            return (await base.FindAsync(e => e.Email.Equals(email, StringComparison.OrdinalIgnoreCase) && !e.IsDeleted)).FirstOrDefault();
        }

        /// <summary>
        /// Henter aktive medarbejdere baseret på stilling (delvis matchning, case-insensitive).
        /// </summary>
        /// <param name="position">Stillingen der søges efter.</param>
        /// <returns>En samling af aktive medarbejdere, der matcher stillingen.</returns>
        /// <exception cref="ArgumentException">Kastes hvis stillingen er tom eller null.</exception>
        public async Task<IEnumerable<Employee>> GetByPositionAsync(string position)
        {
            if (string.IsNullOrWhiteSpace(position))
                throw new ArgumentException("Stilling kan ikke være tom.", nameof(position));
            return await base.FindAsync(e => e.Position.Contains(position, StringComparison.OrdinalIgnoreCase) && !e.IsDeleted);
        }

        /// <summary>
        /// Henter aktive medarbejdere baseret på afdeling (delvis matchning, case-insensitive).
        /// </summary>
        /// <param name="department">Afdelingen der søges efter.</param>
        /// <returns>En samling af aktive medarbejdere, der matcher afdelingen.</returns>
        /// <exception cref="ArgumentException">Kastes hvis afdelingen er tom eller null.</exception>
        public async Task<IEnumerable<Employee>> GetByDepartmentAsync(string department)
        {
            if (string.IsNullOrWhiteSpace(department))
                throw new ArgumentException("Afdeling kan ikke være tom.", nameof(department));
            return await base.FindAsync(e => e.Department.Contains(department, StringComparison.OrdinalIgnoreCase) && !e.IsDeleted);
        }

        /// <summary>
        /// Henter aktive medarbejdere, der har en specifik specialisering (eksakt matchning, case-insensitive).
        /// </summary>
        /// <param name="specialization">Specialiseringen der søges efter.</param>
        /// <returns>En samling af aktive medarbejdere, der har den angivne specialisering.</returns>
        /// <exception cref="ArgumentException">Kastes hvis specialiseringen er tom eller null.</exception>
        public async Task<IEnumerable<Employee>> GetBySpecializationAsync(string specialization)
        {
            if (string.IsNullOrWhiteSpace(specialization))
                throw new ArgumentException("Specialisering kan ikke være tom.", nameof(specialization));
            return await base.FindAsync(e => e.Specializations.Contains(specialization, StringComparer.OrdinalIgnoreCase) && !e.IsDeleted);
        }

        /// <summary>
        /// Henter aktive medarbejdere ansat inden for et specificeret datointerval.
        /// </summary>
        /// <param name="startDate">Startdato for ansættelsesintervallet.</param>
        /// <param name="endDate">Slutdato for ansættelsesintervallet.</param>
        /// <returns>En samling af aktive medarbejdere ansat inden for det angivne interval.</returns>
        /// <exception cref="ArgumentException">Kastes hvis startdato er efter slutdato.</exception>
        public async Task<IEnumerable<Employee>> GetByHireDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
                throw new ArgumentException("Startdato skal være før slutdato.");
            return await base.FindAsync(e => e.HireDate.Date >= startDate.Date && e.HireDate.Date <= endDate.Date && !e.IsDeleted);
        }

        /// <summary>
        /// Henter aktive medarbejdere baseret på navn (delvis matchning på fornavn, efternavn eller fulde navn, case-insensitive).
        /// </summary>
        /// <param name="name">Navnet der søges efter.</param>
        /// <returns>En samling af aktive medarbejdere, der matcher navnet.</returns>
        /// <exception cref="ArgumentException">Kastes hvis navnet er tomt eller null.</exception>
        public async Task<IEnumerable<Employee>> GetByNameAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Navn kan ikke være tomt.", nameof(name));
            return await base.FindAsync(e => 
                (e.FirstName.Contains(name, StringComparison.OrdinalIgnoreCase) || 
                 e.LastName.Contains(name, StringComparison.OrdinalIgnoreCase) || 
                 e.FullName.Contains(name, StringComparison.OrdinalIgnoreCase)) && !e.IsDeleted);
        }

        /// <summary>
        /// Henter aktive medarbejdere baseret på telefonnummer (delvis matchning af cifre).
        /// </summary>
        /// <param name="phone">Telefonnummeret der søges efter.</param>
        /// <returns>En samling af aktive medarbejdere, der matcher telefonnummeret.</returns>
        /// <exception cref="ArgumentException">Kastes hvis telefonnummeret er tomt, null eller ikke indeholder cifre.</exception>
        public async Task<IEnumerable<Employee>> GetByPhoneAsync(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                throw new ArgumentException("Telefonnummer kan ikke være tomt.", nameof(phone));
            
            var normalizedPhone = new string(phone.Where(char.IsDigit).ToArray());
            if (string.IsNullOrWhiteSpace(normalizedPhone)) 
                throw new ArgumentException("Telefonnummer indeholder ingen cifre.");

            return await base.FindAsync(e => 
                e.Phone != null &&
                new string(e.Phone.Where(char.IsDigit).ToArray()).Contains(normalizedPhone) && 
                !e.IsDeleted
            );
        }

        /// <summary>
        /// Henter aktive medarbejdere inden for et specificeret løninterval.
        /// </summary>
        /// <param name="minSalary">Minimumsløn.</param>
        /// <param name="maxSalary">Maksimumsløn.</param>
        /// <returns>En samling af aktive medarbejdere inden for det angivne løninterval.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Kastes hvis lønninger er negative, eller hvis minimumsløn er større end maksimumsløn.</exception>
        public async Task<IEnumerable<Employee>> GetBySalaryRangeAsync(decimal minSalary, decimal maxSalary)
        {
            if (minSalary < 0 || maxSalary < 0)
                throw new ArgumentOutOfRangeException(nameof(minSalary), "Løn kan ikke være negativ.");
            if (minSalary > maxSalary)
                throw new ArgumentOutOfRangeException(nameof(minSalary), "Minimumsløn kan ikke være større end maksimumsløn.");
            return await base.FindAsync(e => e.Salary >= minSalary && e.Salary <= maxSalary && !e.IsDeleted);
        }

        /// <summary>
        /// Henter en medarbejder baseret på ID, uanset om medarbejderen er markeret som slettet (soft-deleted).
        /// </summary>
        /// <param name="id">ID på medarbejderen der søges efter.</param>
        /// <returns>Den fundne medarbejder (aktiv eller soft-deleted), eller null hvis ingen medarbejder matcher ID'et.</returns>
        public async Task<Employee?> GetByIdIncludeDeletedAsync(int id)
        {
            var items = await LoadDataAsync(); 
            return items.FirstOrDefault(x => x.Id == id);
        }
    }
} 