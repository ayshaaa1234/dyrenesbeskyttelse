using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ClassLibrary.Features.Employees.Core.Models;
using ClassLibrary.Features.Employees.Application.Abstractions;
using ClassLibrary.Features.Employees.Infrastructure.Abstractions;
using ClassLibrary.SharedKernel.Application.Implementations; // For BaseUserService
using ClassLibrary.SharedKernel.Exceptions; // For RepositoryException

namespace ClassLibrary.Features.Employees.Application.Implementations
{
    /// <summary>
    /// Service til håndtering af medarbejderdata og -operationer.
    /// </summary>
    public class EmployeeService : BaseUserService<Employee>, IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;

        /// <summary>
        /// Initialiserer en ny instans af <see cref="EmployeeService"/> klassen.
        /// </summary>
        /// <param name="employeeRepository">Repository for medarbejderdata.</param>
        public EmployeeService(IEmployeeRepository employeeRepository) : base(employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        // Implementering af abstrakte metoder fra BaseUserService
        /// <summary>
        /// Henter medarbejdere baseret på navn.
        /// </summary>
        /// <param name="name">Navnet der søges efter.</param>
        /// <returns>En samling af medarbejdere, der matcher navnet.</returns>
        /// <exception cref="ArgumentException">Kastes hvis navnet er tomt eller null.</exception>
        public override async Task<IEnumerable<Employee>> GetByNameAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Navn kan ikke være tomt.", nameof(name));
            return await _employeeRepository.GetByNameAsync(name);
        }

        /// <summary>
        /// Henter en medarbejder baseret på email.
        /// </summary>
        /// <param name="email">Email der søges efter.</param>
        /// <returns>Medarbejderen der matcher emailen, eller null hvis ingen findes.</returns>
        /// <exception cref="ArgumentException">Kastes hvis emailen er tom, null eller i ugyldigt format.</exception>
        public override async Task<Employee?> GetByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email) || !EmailRegex.IsMatch(email))
                throw new ArgumentException("Ugyldigt eller tomt email format.", nameof(email));
            return await _employeeRepository.GetByEmailAsync(email);
        }

        /// <summary>
        /// Henter medarbejdere baseret på telefonnummer.
        /// </summary>
        /// <param name="phone">Telefonnummeret der søges efter.</param>
        /// <returns>En samling af medarbejdere, der matcher telefonnummeret.</returns>
        /// <exception cref="ArgumentException">Kastes hvis telefonnummeret er tomt eller null.</exception>
        public override async Task<IEnumerable<Employee>> GetByPhoneAsync(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                throw new ArgumentException("Telefonnummer kan ikke være tomt.", nameof(phone));
            return await _employeeRepository.GetByPhoneAsync(phone);
        }

        /// <summary>
        /// Henter medarbejdere baseret på ansættelsesdato inden for et interval.
        /// </summary>
        /// <param name="startDate">Startdato for intervallet.</param>
        /// <param name="endDate">Slutdato for intervallet.</param>
        /// <returns>En samling af medarbejdere ansat inden for det angivne datointerval.</returns>
        /// <exception cref="ArgumentException">Kastes hvis startdato er efter slutdato.</exception>
        public override async Task<IEnumerable<Employee>> GetByRegistrationDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            // For Employees, RegistrationDate er typisk lig HireDate.
            // Det er op til implementeringen i repository at bruge det korrekte felt.
            // Her kalder vi bare repository-metoden, som forventes at søge på RegistrationDate/HireDate.
            if (startDate > endDate)
                throw new ArgumentException("Startdato skal være før slutdato.");
            return await _employeeRepository.GetByHireDateRangeAsync(startDate, endDate); // Bruger HireDate specifikt her
        }

        /// <summary>
        /// Søger efter medarbejdere baseret på et generelt søgeord (matcher på navn, email, telefon, stilling, afdeling).
        /// </summary>
        /// <param name="searchTerm">Søgeordet.</param>
        /// <returns>En samling af unikke medarbejdere, der matcher søgeordet. Returnerer en tom liste, hvis søgeordet er tomt eller null.</returns>
        public override async Task<IEnumerable<Employee>> SearchAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return new List<Employee>();

            var results = new List<Employee>();
            results.AddRange(await _employeeRepository.GetByNameAsync(searchTerm));
            var byEmail = await _employeeRepository.GetByEmailAsync(searchTerm);
            if (byEmail != null) results.Add(byEmail);
            results.AddRange(await _employeeRepository.GetByPhoneAsync(searchTerm));
            results.AddRange(await _employeeRepository.GetByPositionAsync(searchTerm));
            results.AddRange(await _employeeRepository.GetByDepartmentAsync(searchTerm));
            
            return results.Distinct(new EmployeeComparer());
        }

        // Override CreateAsync & UpdateAsync for Employee specifik logik
        /// <summary>
        /// Opretter en ny medarbejder.
        /// </summary>
        /// <param name="employee">Medarbejderen der skal oprettes.</param>
        /// <returns>Den oprettede medarbejder.</returns>
        /// <exception cref="ArgumentNullException">Kastes hvis employee er null.</exception>
        /// <exception cref="RepositoryException">Kastes hvis en aktiv medarbejder med samme email allerede eksisterer.</exception>
        public override async Task<Employee> CreateAsync(Employee employee)
        {
            if (employee == null) throw new ArgumentNullException(nameof(employee));
            
            // Sæt RegistrationDate til HireDate før validering, hvis HireDate er sat.
            if(employee.HireDate != default)
            {
                employee.RegistrationDate = employee.HireDate;
            }
            else // Hvis HireDate ikke er sat, brug UtcNow (som BaseUser også ville gøre for RegistrationDate)
            {
                employee.HireDate = DateTime.UtcNow;
                employee.RegistrationDate = employee.HireDate;
            }
            
            ValidateUser(employee); // Kalder den overstyrede ValidateUser nedenfor

            var existingByEmail = await _employeeRepository.GetByEmailAsync(employee.Email);
            if (existingByEmail != null && !existingByEmail.IsDeleted)
            {
                throw new RepositoryException($"En aktiv medarbejder med emailen '{employee.Email}' eksisterer allerede.");
            }
            return await base.CreateAsync(employee); // Kalder BaseUserService.CreateAsync
        }

        /// <summary>
        /// Opdaterer en eksisterende medarbejder.
        /// </summary>
        /// <param name="employee">Medarbejderen med de opdaterede værdier.</param>
        /// <returns>Den opdaterede medarbejder.</returns>
        /// <exception cref="ArgumentNullException">Kastes hvis employee er null.</exception>
        /// <exception cref="KeyNotFoundException">Kastes hvis ingen aktiv medarbejder findes med det angivne ID.</exception>
        /// <exception cref="RepositoryException">Kastes hvis en anden aktiv medarbejder med den nye email allerede eksisterer.</exception>
        public override async Task<Employee> UpdateAsync(Employee employee)
        {
            if (employee == null) throw new ArgumentNullException(nameof(employee));
            
            var existingEmployee = await _employeeRepository.GetByIdAsync(employee.Id);
            if (existingEmployee == null || existingEmployee.IsDeleted)
            {
                throw new KeyNotFoundException($"Ingen aktiv medarbejder fundet med ID: {employee.Id} til opdatering.");
            }

            // Bevar oprindelig HireDate og RegistrationDate fra den eksisterende medarbejder
            // Medmindre de specifikt ændres i 'employee' input objektet før ValidateUser kaldes.
            // Vi sætter dem her for at sikre, de ikke utilsigtet overskrives af BaseUser logik,
            // hvis 'employee' objektet ikke har dem sat korrekt.
            employee.HireDate = existingEmployee.HireDate; 
            employee.RegistrationDate = existingEmployee.RegistrationDate;

            ValidateUser(employee); // Kalder den overstyrede ValidateUser

            if (!existingEmployee.Email.Equals(employee.Email, StringComparison.OrdinalIgnoreCase))
            {
                var conflictingCustomer = await _employeeRepository.GetByEmailAsync(employee.Email);
                if (conflictingCustomer != null && conflictingCustomer.Id != employee.Id && !conflictingCustomer.IsDeleted)
                {
                    throw new RepositoryException($"En anden aktiv medarbejder med emailen '{employee.Email}' eksisterer allerede.");
                }
            }
            return await base.UpdateAsync(employee); // Kalder BaseUserService.UpdateAsync
        }

        /// <summary>
        /// Validerer en medarbejderentitet, inklusiv BaseUser validering og medarbejderspecifikke regler.
        /// </summary>
        /// <param name="employee">Medarbejderen der skal valideres.</param>
        /// <exception cref="ArgumentException">Kastes hvis Stilling er tom.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Kastes hvis Løn er negativ, eller Ansættelsesdato/Registreringsdato er ugyldig.</exception>
        protected override void ValidateUser(Employee employee) // Employee-specifik validering
        {
            base.ValidateUser(employee); // Generel BaseUser validering

            if (string.IsNullOrWhiteSpace(employee.Position))
                throw new ArgumentException("Stilling kan ikke være tom.", nameof(employee.Position));
            if (employee.Salary < 0)
                throw new ArgumentOutOfRangeException(nameof(employee.Salary), "Løn kan ikke være negativ.");
            if (employee.HireDate == default || employee.HireDate > DateTime.UtcNow.AddMinutes(1)) // Tillad lille udsving for "nu"
                throw new ArgumentOutOfRangeException(nameof(employee.HireDate), "Ugyldig ansættelsesdato.");
            
            // Sikrer at RegistrationDate er sat (normalt til HireDate for Employees)
            if (employee.RegistrationDate == default || employee.RegistrationDate > DateTime.UtcNow.AddMinutes(1))
                throw new ArgumentOutOfRangeException(nameof(employee.RegistrationDate), "Ugyldig registreringsdato.");
        }

        // Implementering af IEmployeeService specifikke metoder
        /// <summary>
        /// Henter medarbejdere baseret på deres stilling.
        /// </summary>
        /// <param name="position">Stillingen der søges efter.</param>
        /// <returns>En samling af medarbejdere med den angivne stilling.</returns>
        /// <exception cref="ArgumentException">Kastes hvis stillingen er tom eller null.</exception>
        public async Task<IEnumerable<Employee>> GetEmployeesByPositionAsync(string position)
        {
            if (string.IsNullOrWhiteSpace(position))
                throw new ArgumentException("Stilling kan ikke være tom", nameof(position));
            return await _employeeRepository.GetByPositionAsync(position);
        }

        /// <summary>
        /// Henter medarbejdere baseret på deres afdeling.
        /// </summary>
        /// <param name="department">Afdelingen der søges efter.</param>
        /// <returns>En samling af medarbejdere i den angivne afdeling.</returns>
        /// <exception cref="ArgumentException">Kastes hvis afdelingen er tom eller null.</exception>
        public async Task<IEnumerable<Employee>> GetEmployeesByDepartmentAsync(string department)
        {
            if (string.IsNullOrWhiteSpace(department))
                throw new ArgumentException("Afdeling kan ikke være tom", nameof(department));
            return await _employeeRepository.GetByDepartmentAsync(department);
        }

        /// <summary>
        /// Henter medarbejdere ansat inden for et specificeret datointerval.
        /// </summary>
        /// <param name="startDate">Startdato for ansættelsesintervallet.</param>
        /// <param name="endDate">Slutdato for ansættelsesintervallet.</param>
        /// <returns>En samling af medarbejdere ansat inden for det angivne interval.</returns>
        /// <exception cref="ArgumentException">Kastes hvis startdato er efter slutdato.</exception>
        public async Task<IEnumerable<Employee>> GetEmployeesByHireDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
                throw new ArgumentException("Startdato skal være før slutdato.");
            return await _employeeRepository.GetByHireDateRangeAsync(startDate, endDate);
        }

        /// <summary>
        /// Henter medarbejdere inden for et specificeret løninterval.
        /// </summary>
        /// <param name="minSalary">Minimumsløn.</param>
        /// <param name="maxSalary">Maksimumsløn.</param>
        /// <returns>En samling af medarbejdere inden for det angivne løninterval.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Kastes hvis lønninger er negative, eller hvis minimumsløn er større end maksimumsløn.</exception>
        public async Task<IEnumerable<Employee>> GetEmployeesBySalaryRangeAsync(decimal minSalary, decimal maxSalary)
        {
            if (minSalary < 0 || maxSalary < 0)
                throw new ArgumentOutOfRangeException(nameof(minSalary), "Løn kan ikke være negativ.");
            if (minSalary > maxSalary)
                 throw new ArgumentOutOfRangeException(nameof(minSalary), "Minimumsløn kan ikke være større end maksimumsløn.");
            return await _employeeRepository.GetBySalaryRangeAsync(minSalary, maxSalary);
        }

        /// <summary>
        /// Henter medarbejdere baseret på deres specialisering.
        /// </summary>
        /// <param name="specialization">Specialiseringen der søges efter.</param>
        /// <returns>En samling af medarbejdere med den angivne specialisering.</returns>
        /// <exception cref="ArgumentException">Kastes hvis specialiseringen er tom eller null.</exception>
        public async Task<IEnumerable<Employee>> GetEmployeesBySpecializationAsync(string specialization)
        {
            if (string.IsNullOrWhiteSpace(specialization))
                throw new ArgumentException("Specialisering kan ikke være tom", nameof(specialization));
            return await _employeeRepository.GetBySpecializationAsync(specialization);
        }

        /// <summary>
        /// Reaktiverer en tidligere slettet (deaktiveret) medarbejder.
        /// </summary>
        /// <param name="employeeId">ID på medarbejderen der skal reaktiveres.</param>
        /// <returns>En opgave der repræsenterer den asynkrone operation.</returns>
        /// <exception cref="KeyNotFoundException">Kastes hvis ingen medarbejder findes med det angivne ID.</exception>
        /// <exception cref="InvalidOperationException">Kastes hvis medarbejderen allerede er aktiv.</exception>
        public async Task ReactivateEmployeeAsync(int employeeId)
        {
            var employee = await _employeeRepository.GetByIdIncludeDeletedAsync(employeeId); // Antager en metode der kan hente selvom IsDeleted=true
            if (employee == null)
                throw new KeyNotFoundException($"Ingen medarbejder fundet med ID: {employeeId}.");
            if (!employee.IsDeleted)
                throw new InvalidOperationException("Medarbejder er allerede aktiv.");

            employee.IsDeleted = false;
            employee.DeletedAt = null;
            await _employeeRepository.UpdateAsync(employee); // Repository.UpdateAsync bør håndtere dette korrekt.
        }

        /// <summary>
        /// Opdaterer en medarbejders løn.
        /// </summary>
        /// <param name="employeeId">ID på medarbejderen hvis løn skal opdateres.</param>
        /// <param name="newSalary">Den nye løn.</param>
        /// <returns>En opgave der repræsenterer den asynkrone operation.</returns>
        /// <exception cref="KeyNotFoundException">Kastes hvis ingen aktiv medarbejder findes med det angivne ID.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Kastes hvis den nye løn er negativ.</exception>
        public async Task UpdateEmployeeSalaryAsync(int employeeId, decimal newSalary)
        {
            var employee = await _employeeRepository.GetByIdAsync(employeeId);
            if (employee == null || employee.IsDeleted)
                throw new KeyNotFoundException($"Ingen aktiv medarbejder fundet med ID: {employeeId}.");
            if (newSalary < 0)
                throw new ArgumentOutOfRangeException(nameof(newSalary), "Løn kan ikke være negativ.");

            employee.Salary = newSalary;
            await _employeeRepository.UpdateAsync(employee);
        }

        /// <summary>
        /// Tilføjer en specialisering til en medarbejder.
        /// </summary>
        /// <param name="employeeId">ID på medarbejderen.</param>
        /// <param name="specialization">Specialiseringen der skal tilføjes.</param>
        /// <returns>En opgave der repræsenterer den asynkrone operation.</returns>
        /// <exception cref="KeyNotFoundException">Kastes hvis ingen aktiv medarbejder findes med det angivne ID.</exception>
        /// <exception cref="ArgumentException">Kastes hvis specialiseringen er tom eller null.</exception>
        /// <exception cref="InvalidOperationException">Kastes hvis medarbejderen allerede har den angivne specialisering.</exception>
        public async Task AddSpecializationAsync(int employeeId, string specialization)
        {
            var employee = await _employeeRepository.GetByIdAsync(employeeId);
            if (employee == null || employee.IsDeleted)
                throw new KeyNotFoundException($"Ingen aktiv medarbejder fundet med ID: {employeeId}.");
            if (string.IsNullOrWhiteSpace(specialization))
                throw new ArgumentException("Specialisering kan ikke være tom.", nameof(specialization));

            if (employee.Specializations == null) employee.Specializations = new List<string>();
            if (employee.Specializations.Contains(specialization, StringComparer.OrdinalIgnoreCase))
                throw new InvalidOperationException("Medarbejder har allerede denne specialisering.");

            employee.Specializations.Add(specialization);
            await _employeeRepository.UpdateAsync(employee);
        }

        /// <summary>
        /// Fjerner en specialisering fra en medarbejder.
        /// </summary>
        /// <param name="employeeId">ID på medarbejderen.</param>
        /// <param name="specialization">Specialiseringen der skal fjernes.</param>
        /// <returns>En opgave der repræsenterer den asynkrone operation.</returns>
        /// <exception cref="KeyNotFoundException">Kastes hvis ingen aktiv medarbejder findes med det angivne ID.</exception>
        /// <exception cref="ArgumentException">Kastes hvis specialiseringen er tom eller null.</exception>
        /// <exception cref="InvalidOperationException">Kastes hvis medarbejderen ikke har den angivne specialisering.</exception>
        public async Task RemoveSpecializationAsync(int employeeId, string specialization)
        {
            var employee = await _employeeRepository.GetByIdAsync(employeeId);
            if (employee == null || employee.IsDeleted)
                throw new KeyNotFoundException($"Ingen aktiv medarbejder fundet med ID: {employeeId}.");
            if (string.IsNullOrWhiteSpace(specialization))
                throw new ArgumentException("Specialisering kan ikke være tom.", nameof(specialization));

            // Check if Specializations list is null or if the specialization was not found and thus not removed.
            // RemoveAll returns the number of elements removed. If 0, it means the specialization wasn't there.
            if (employee.Specializations == null || employee.Specializations.RemoveAll(s => s.Equals(specialization, StringComparison.OrdinalIgnoreCase)) == 0)
                throw new InvalidOperationException("Medarbejder har ikke denne specialisering eller den kunne ikke fjernes.");
            
            await _employeeRepository.UpdateAsync(employee);
        }

        /// <summary>
        /// En intern hjælpeklasse til at sammenligne medarbejdere baseret på deres ID for Distinct() operationer.
        /// </summary>
        private class EmployeeComparer : IEqualityComparer<Employee>
        {
            /// <summary>
            /// Bestemmer om to Employee objekter er ens baseret på deres ID.
            /// </summary>
            public bool Equals(Employee? x, Employee? y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (x is null || y is null) return false;
                return x.Id == y.Id;
            }
            /// <summary>
            /// Returnerer en hash-kode for det specificerede Employee objekt, baseret på dets ID.
            /// </summary>
            public int GetHashCode(Employee obj) => obj.Id.GetHashCode();
        }
    }
} 