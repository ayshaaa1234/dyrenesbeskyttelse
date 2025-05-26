using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using ClassLibrary.Features.AnimalManagement.Core.Models;
using ClassLibrary.Features.AnimalManagement.Core.Enums;
using ClassLibrary.Features.Blog.Core.Models;
using ClassLibrary.Features.Customers.Core.Models;
using ClassLibrary.Features.Employees.Core.Models;
using ClassLibrary.Features.Adoptions.Core.Models;
using ClassLibrary.Features.Adoptions.Core.Enums;
using ClassLibrary.Features.Memberships.Core.Models;
using ClassLibrary.Features.Memberships.Core.Enums;

namespace ClassLibrary.Infrastructure.DataInitialization
{
    public static class JsonDataInitializer
    {
        public static string CalculatedWorkspaceRoot { get; private set; } = string.Empty;

        private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            Converters = { new JsonStringEnumConverter() }
        };

        // Liste over forventede datafiler og deres eventuelle seed-data generatorer
        private static readonly Dictionary<string, Func<Task<string>>> _dataFilesToInitialize = new Dictionary<string, Func<Task<string>>>
        {
            { "Data/Json/animals.json", GetDefaultAnimalsJson },
            { "Data/Json/blogposts.json", GetDefaultBlogPostsJson },
            { "Data/Json/customers.json", GetDefaultCustomersJson },
            { "Data/Json/employees.json", GetDefaultEmployeesJson },
            { "Data/Json/adoptions.json", GetDefaultAdoptionsJson },
            { "Data/Json/healthrecords.json", GetDefaultHealthRecordsJson },
            { "Data/Json/visits.json", GetDefaultVisitsJson },
            { "Data/Json/membershipproducts.json", GetDefaultMembershipProductsJson },
            { "Data/Json/customermemberships.json", GetDefaultCustomerMembershipsJson }
        };

        public static async Task InitializeAsync()
        {
            Console.WriteLine($"JsonDataInitializer: AppContext.BaseDirectory = {AppContext.BaseDirectory}");
            // Bestem workspace-roden ved at gå fire niveauer op fra output-mappen
            // Dette antager en standard projektstruktur: [WorkspaceRoD]/[ProjektNavn]/bin/[Konfiguration]/[Framework]/
            string workspaceRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));
            CalculatedWorkspaceRoot = workspaceRoot;
            Console.WriteLine($"JsonDataInitializer: Calculated Workspace Root = {CalculatedWorkspaceRoot}");

            string targetJsonDir = Path.Combine(CalculatedWorkspaceRoot, "Data", "Json");
            Console.WriteLine($"JsonDataInitializer: Target JSON Directory = {targetJsonDir}");

            string blogPostsPath = Path.Combine(targetJsonDir, "blogposts.json");
            Console.WriteLine($"JsonDataInitializer: Expected blogposts.json path = {blogPostsPath}");

            Console.WriteLine("Starting JSON data initialization...");

            foreach (var entry in _dataFilesToInitialize)
            {
                // Kombiner workspace-roden med den relative sti fra entry.Key (f.eks. "Data/Json/animals.json")
                var filePath = Path.Combine(CalculatedWorkspaceRoot, entry.Key);
                var directory = Path.GetDirectoryName(filePath);

                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                    Console.WriteLine($"Created directory: {directory}");
                }

                if (!File.Exists(filePath))
                {
                    Console.WriteLine($"File not found: {filePath}. Creating with default data...");
                    var jsonData = await entry.Value.Invoke(); // Kald den specifikke seed-data generator
                    await File.WriteAllTextAsync(filePath, jsonData);
                    Console.WriteLine($"Created and seeded: {filePath}");
                }
                else
                {
                    Console.WriteLine($"File already exists: {filePath}. No action taken.");
                }
            }
            Console.WriteLine("JSON data initialization finished.");
        }

        // Eksempler på seed-data metoder (kan returnere "[]" for tom liste)

        private static Task<string> GetDefaultAnimalsJson()
        {
            var animals = new List<Animal>
            {
                new Animal { Id = 1, Name = "Max", Species = Species.Hund, Breed = "Labrador Retriever", BirthDate = new DateTime(2020, 5, 15), Gender = "Han", Weight = 30.5m, IntakeDate = DateTime.UtcNow.AddDays(-90), Status = AnimalStatus.Available, Description = "Venlig og legesyg familiehund. Elsker lange gåture.", PictureUrl = "https://placehold.co/600x400/orange/white?text=Max+Hund", HealthStatus = "Sund og rask", IsAdopted = false, IsDeleted = false },
                new Animal { Id = 2, Name = "Bella", Species = Species.Kat, Breed = "Siameser", BirthDate = new DateTime(2021, 1, 10), Gender = "Hun", Weight = 4.2m, IntakeDate = DateTime.UtcNow.AddDays(-60), Status = AnimalStatus.Available, Description = "Kælen og rolig indekat. Kan lide at putte.", PictureUrl = "https://placehold.co/600x400/blue/white?text=Bella+Kat", HealthStatus = "Har brug for specialfoder", IsAdopted = false, IsDeleted = false },
                new Animal { Id = 3, Name = "Charlie", Species = Species.Hund, Breed = "Golden Retriever", BirthDate = new DateTime(2019, 8, 22), Gender = "Han", Weight = 35.1m, IntakeDate = DateTime.UtcNow.AddDays(-120), Status = AnimalStatus.Reserved, Description = "Meget aktiv og træningsparat. Kræver erfaren ejer.", PictureUrl = "https://placehold.co/600x400/green/white?text=Charlie+Hund", HealthStatus = "God form", IsAdopted = false, IsDeleted = false },
                new Animal { Id = 4, Name = "Lucy", Species = Species.Kat, Breed = "Maine Coon", BirthDate = new DateTime(2022, 3, 5), Gender = "Hun", Weight = 6.8m, IntakeDate = DateTime.UtcNow.AddDays(-30), Status = AnimalStatus.Adopted, Description = "Stor og flot kat, meget social.", PictureUrl = "https://placehold.co/600x400/purple/white?text=Lucy+Kat", HealthStatus = "Sund", AdoptionDate = DateTime.UtcNow.AddDays(-10), IsAdopted = true, IsDeleted = false },
                new Animal { Id = 5, Name = "Cooper", Species = Species.Kanin, Breed = "Dværgvædder", BirthDate = new DateTime(2023, 1, 15), Gender = "Han", Weight = 1.5m, IntakeDate = DateTime.UtcNow.AddDays(-15), Status = AnimalStatus.Available, Description = "Sød og nysgerrig kanin. Stueren.", PictureUrl = "https://placehold.co/600x400/red/white?text=Cooper+Kanin", HealthStatus = "Tjekket af dyrlæge", IsAdopted = false, IsDeleted = false },
                new Animal { Id = 6, Name = "Daisy", Species = Species.Fugl, Breed = "Undulat", BirthDate = new DateTime(2022, 11, 1), Gender = "Hun", Weight = 0.05m, IntakeDate = DateTime.UtcNow.AddDays(-45), Status = AnimalStatus.Available, Description = "Synger smukt. Tam.", PictureUrl = "https://placehold.co/600x400/yellow/black?text=Daisy+Fugl", HealthStatus = "Ok", IsAdopted = false, IsDeleted = false },
                new Animal { Id = 7, Name = "Rocky", Species = Species.Hund, Breed = "Schæfer", BirthDate = new DateTime(2018, 6, 10), Gender = "Han", Weight = 38.0m, IntakeDate = DateTime.UtcNow.AddDays(-200), Status = AnimalStatus.Available, Description = "Tidligere tjenestehund. Rolig og loyal. Har et gammelt brud på ben, der kræver skånebehov.", PictureUrl = "https://placehold.co/600x400/grey/white?text=Rocky+Hund", HealthStatus = "Gammelt brud på ben, kræver skånebehov, ellers ok", IsAdopted = false, IsDeleted = false },
                new Animal { Id = 8, Name = "Misty", Species = Species.Kat, Breed = "Perser", BirthDate = new DateTime(2020, 2, 20), Gender = "Hun", Weight = 3.5m, IntakeDate = DateTime.UtcNow.AddDays(-50), Status = AnimalStatus.Available, Description = "Langhåret, kræver pelspleje. Lidt genert.", PictureUrl = "https://placehold.co/600x400/pink/white?text=Misty+Kat", HealthStatus = "Ok, men genert", IsAdopted = false, IsDeleted = false }
            };
            return Task.FromResult(JsonSerializer.Serialize(animals, _jsonOptions));
        }

        private static Task<string> GetDefaultBlogPostsJson()
        {
            var posts = new List<BlogPost>
            {
                new BlogPost { Id = 1, Title = "Velkommen til Dyrenes Beskyttelses nye blog!", Content = "Dette er det første indlæg på vores nye blog. Her vil vi dele historier, tips og nyheder om vores arbejde med at hjælpe dyr i nød. Følg med!", Summary = "En kort velkomst til vores platform.", AuthorId = 1, /* Admin Bruger */ PublishDate = DateTime.UtcNow.AddDays(-7), IsPublished = true, Tags = new List<string> { "velkommen", "nyheder", "internat" }, PictureUrl = "https://placehold.co/800x400/orange/white?text=Velkommen", Likes = 15 },
                new BlogPost { Id = 2, Title = "Tips til pasning af din nye hvalp", Content = "At få en hvalp er en stor glæde, men også et stort ansvar. Her er nogle gode råd til, hvordan du bedst tager dig af din nye firbenede ven i den første tid. Husk tålmodighed og masser af kærlighed!", Summary = "Gode råd til nye hvalpeejere.", AuthorId = 2, /* Peter Plys */ PublishDate = DateTime.UtcNow.AddDays(-5), IsPublished = true, Tags = new List<string> { "hvalpe", "tips", "hund" }, PictureUrl = "https://placehold.co/800x400/green/white?text=HvalpeTips", Likes = 42 },
                new BlogPost { Id = 3, Title = "Vinterhi for pindsvin: Sådan hjælper du", Content = "Vinteren nærmer sig, og det er tid til at tænke på havens små beboere. Lær hvordan du kan lave et sikkert og hyggeligt vinterhi for pindsvin.", Summary = "Hjælp pindsvinene gennem vinteren.", AuthorId = 3, /* Mette Munk */ PublishDate = DateTime.UtcNow.AddDays(-2), IsPublished = true, Tags = new List<string> { "pindsvin", "vinter", "natur" }, PictureUrl = "https://placehold.co/800x400/brown/white?text=PindsvinHjælp", Likes = 28 },
                new BlogPost { Id = 4, Title = "Mød Max: En glad labrador søger nyt hjem", Content = "Max er en energisk og kærlig labrador på 3 år, som leder efter en aktiv familie. Læs hans historie og se om I er et match!", Summary = "Adoptionshistorie for Max.", AuthorId = 1, /* Admin Bruger */ PublishDate = DateTime.UtcNow.AddDays(-1), IsPublished = true, Tags = new List<string> { "adoption", "hund", "max" }, PictureUrl = "https://placehold.co/600x400/orange/white?text=Max+Hund", Likes = 55 },
                new BlogPost { Id = 5, Title = "Sådan spotter du tegn på mistrivsel hos din kat", Content = "Katte er mestre i at skjule smerte. Lær de subtile tegn på, at din kat måske ikke har det godt, og hvornår du bør kontakte dyrlægen.", Summary = "Forstå din kats signaler.", AuthorId = 4, /* Sofie Sørensen */ PublishDate = DateTime.UtcNow.AddHours(-12), IsPublished = true, Tags = new List<string> { "kat", "sundhed", "adfærd" }, PictureUrl = "https://placehold.co/800x400/blue/white?text=Kat+Sundhed", Likes = 33 }
            };
            // Antager at AuthorId 1, 2, 3, 4 er Employees der oprettes i GetDefaultEmployeesJson
            return Task.FromResult(JsonSerializer.Serialize(posts, _jsonOptions));
        }
        
        private static Task<string> GetDefaultCustomersJson()
        {
             var customers = new List<Customer>
            {
                new Customer { Id = 1, FirstName = "Anders", LastName = "Andersen", Email = "anders.a@example.com", Phone = "12345678", Address = "Solvej 1", PostalCode = "1234", City = "Solby", RegistrationDate = DateTime.UtcNow.AddMonths(-6) },
                new Customer { Id = 2, FirstName = "Bente", LastName = "Bentsen", Email = "bente.b@example.net", Phone = "87654321", Address = "Månevej 22", PostalCode = "4321", City = "Måneby", RegistrationDate = DateTime.UtcNow.AddMonths(-3) },
                new Customer { Id = 3, FirstName = "Carla", LastName = "Carlsen", Email = "carla.c@example.org", Phone = "11223344", Address = "Stjernevej 3", PostalCode = "5678", City = "Stjernekøbing", RegistrationDate = DateTime.UtcNow.AddMonths(-1) },
                new Customer { Id = 4, FirstName = "Dennis", LastName = "Danielsen", Email = "dennis.d@example.com", Phone = "55667788", Address = "Galaksevej 44", PostalCode = "8765", City = "Galakseborg", RegistrationDate = DateTime.UtcNow.AddDays(-15) },
                new Customer { Id = 5, FirstName = "Eva", LastName = "Eriksen", Email = "eva.e@example.net", Phone = "99887766", Address = "Planetvej 5", PostalCode = "3456", City = "Planetbyen", RegistrationDate = DateTime.UtcNow.AddDays(-5) }
            };
            return Task.FromResult(JsonSerializer.Serialize(customers, _jsonOptions));
        }

        private static Task<string> GetDefaultEmployeesJson()
        {
            var employees = new List<Employee>
            {
                new Employee { Id = 1, FirstName = "Admin", LastName = "Bruger", Email = "admin@dyrenesbeskyttelse.dk", Phone = "10203040", Position = "System Administrator", Salary = 55000, Department = "IT", HireDate = new DateTime(2018,1,1), Specializations = new List<string>{"Systemvedligehold", "Datasikkerhed"}, PictureUrl = "https://placehold.co/300x300/black/white?text=Admin" },
                new Employee { Id = 2, FirstName = "Peter", LastName = "Plys", Email = "peter.plys@dyrenesbeskyttelse.dk", Phone = "20304050", Position = "Ledende Dyrepasser", Salary = 42000, Department = "Internat", HireDate = new DateTime(2019,3,15), Specializations = new List<string>{"Hundeadfærd", "Kattepleje"}, PictureUrl = "https://placehold.co/300x300/brown/white?text=Peter" },
                new Employee { Id = 3, FirstName = "Mette", LastName = "Munk", Email = "mette.munk@dyrenesbeskyttelse.dk", Phone = "30405060", Position = "Biolog & Naturvejleder", Salary = 38000, Department = "Udekørende Enhed", HireDate = new DateTime(2020,6,1), Specializations = new List<string>{"Vilde dyr", "Pindsvin", "Fugle"}, PictureUrl = "https://placehold.co/300x300/green/white?text=Mette" },
                new Employee { Id = 4, FirstName = "Sofie", LastName = "Sørensen", Email = "sofie.s@dyrenesbeskyttelse.dk", Phone = "40506070", Position = "Veterinærsygeplejerske", Salary = 36000, Department = "Klinik", HireDate = new DateTime(2021,9,10), Specializations = new List<string>{"Operationassistance", "Laboratoriearbejde"}, PictureUrl = "https://placehold.co/300x300/blue/white?text=Sofie" },
                new Employee { Id = 5, FirstName = "Jens", LastName = "Jensen", Email = "jens.j@dyrenesbeskyttelse.dk", Phone = "50607080", Position = "Internatmedarbejder", Salary = 30000, Department = "Internat", HireDate = new DateTime(2022,2,20), Specializations = new List<string>{"Rengøring", "Fodring", "Socialisering af dyr"}, PictureUrl = "https://placehold.co/300x300/grey/white?text=Jens" }
            };
            return Task.FromResult(JsonSerializer.Serialize(employees, _jsonOptions));
        }
        
        private static Task<string> GetDefaultAdoptionsJson()
        {
            var adoptions = new List<Adoption>
            {
                // Max (AnimalId=1) reserveret af Carla (CustomerId=3)
                new Adoption { Id = 1, AnimalId = 1, CustomerId = 3, ApplicationDate = DateTime.UtcNow.AddDays(-5), Status = AdoptionStatus.Pending, Notes = "Carla virker som et godt match til Max. Hjemmebesøg planlagt.", EmployeeId = 2 /* Peter Plys */ },
                // Bella (AnimalId=2) ansøgt af Anders (CustomerId=1) - Afventer godkendelse
                new Adoption { Id = 2, AnimalId = 2, CustomerId = 1, ApplicationDate = DateTime.UtcNow.AddDays(-2), Status = AdoptionStatus.Pending, Notes = "Anders har erfaring med siamesere.", EmployeeId = 4 /* Sofie Sørensen */ },
                // Lucy (AnimalId=4) adopteret af Bente (CustomerId=2)
                new Adoption { Id = 3, AnimalId = 4, CustomerId = 2, ApplicationDate = DateTime.UtcNow.AddDays(-15), ApprovalDate = DateTime.UtcNow.AddDays(-12), CompletionDate = DateTime.UtcNow.AddDays(-10), Status = AdoptionStatus.Completed, Notes = "Lucy trives hos Bente. Alt forløb glat.", EmployeeId = 2 }
            };
            return Task.FromResult(JsonSerializer.Serialize(adoptions, _jsonOptions));
        }

        private static Task<string> GetDefaultHealthRecordsJson()
        {
             var records = new List<HealthRecord>
            {
                new HealthRecord { Id = 1, AnimalId = 1, RecordDate = DateTime.UtcNow.AddDays(-80), Diagnosis = "Rutinetjek og vaccine", Treatment = "Vaccination (standard)", Notes = "Alt ser fint ud. Max er sund.", VeterinarianName = "Dr. Dyregod", IsVaccinated = true, NextVaccinationDate = DateTime.UtcNow.AddDays(-80).AddYears(1), Weight = 30.2m, CreatedAt = DateTime.UtcNow.AddDays(-80) },
                new HealthRecord { Id = 2, AnimalId = 2, RecordDate = DateTime.UtcNow.AddDays(-50), Diagnosis = "Let øjenbetændelse", Treatment = "Øjendråber 2x dagligt i 7 dage.", Notes = "Skal have dråber i 7 dage. Kontrol om 10 dage.", VeterinarianName = "Dr. Andersen", IsVaccinated = true, NextVaccinationDate = DateTime.UtcNow.AddDays(-50).AddYears(1), Weight = 4.1m, CreatedAt = DateTime.UtcNow.AddDays(-50) },
                new HealthRecord { Id = 3, AnimalId = 1, RecordDate = DateTime.UtcNow.AddDays(-20), Diagnosis = "Kontrol efter mindre skramme", Treatment = "Renset og observeret", Notes = "Skramme på forben heler fint.", VeterinarianName = "Dr. Dyregod", IsVaccinated = true, Weight = 30.5m, CreatedAt = DateTime.UtcNow.AddDays(-20) },
                new HealthRecord { Id = 4, AnimalId = 3, RecordDate = DateTime.UtcNow.AddDays(-110), Diagnosis = "Adfærdsvurdering", Treatment = "Ingen medicinsk behandling", Notes = "Charlie er meget energisk, anbefaler træning.", VeterinarianName = "Adfærdsterapeut Olsen", IsVaccinated = true, NextVaccinationDate = DateTime.UtcNow.AddDays(-110).AddYears(1), Weight = 34.8m, CreatedAt = DateTime.UtcNow.AddDays(-110) },
                new HealthRecord { Id = 5, AnimalId = 5, RecordDate = DateTime.UtcNow.AddDays(-14), Diagnosis = "Sundhedstjek ved ankomst", Treatment = "Ok", Notes = "Cooper er en sund ung kanin.", VeterinarianName = "Dr. Nielsen", IsVaccinated = false, Weight = 1.5m, CreatedAt = DateTime.UtcNow.AddDays(-14) },
                new HealthRecord { Id = 6, AnimalId = 7, RecordDate = DateTime.UtcNow.AddDays(-190), Diagnosis = "Gammelt benbrud vurdering", Treatment = "Smertestillende ved behov", Notes = "Rocky skal undgå for voldsom leg. Let skånekost.", VeterinarianName = "Dr. Speciallæge Hansen", IsVaccinated = true, NextVaccinationDate = DateTime.UtcNow.AddDays(-190).AddYears(1), Weight = 38.5m, CreatedAt = DateTime.UtcNow.AddDays(-190) },
                new HealthRecord { Id = 7, AnimalId = 7, RecordDate = DateTime.UtcNow.AddDays(-30), Diagnosis = "Opfølgning ben", Treatment = "Fortsat observation", Notes = "Ingen forværring. Smertestillende dosis kan evt. reduceres.", VeterinarianName = "Dr. Speciallæge Hansen", IsVaccinated = true, Weight = 38.0m, CreatedAt = DateTime.UtcNow.AddDays(-30) }
            };
            // Antager AnimalId 1, 2, 3, 5, 7 eksisterer fra GetDefaultAnimalsJson
            return Task.FromResult(JsonSerializer.Serialize(records, _jsonOptions));
        }

        private static Task<string> GetDefaultVisitsJson()
        {
             var visits = new List<Visit>
            {
                new Visit { Id = 1, AnimalId = 1, CustomerId = 3, PlannedDate = DateTime.UtcNow.AddDays(2), Type = "Hjemmebesøg (Adoption Max)", Status = VisitStatus.Scheduled, PlannedDuration = 60, Visitor = "Carla Carlsen", Notes = "Vurdering af hjem for adoption af Max.", EmployeeId = 2 },
                new Visit { Id = 2, AnimalId = 2, CustomerId = 1, PlannedDate = DateTime.UtcNow.AddDays(-1), ActualDate = DateTime.UtcNow.AddDays(-1), Type = "Fremvisning (Bella)", Status = VisitStatus.Completed, PlannedDuration = 30, ActualDuration = 45, Visitor = "Anders Andersen", Notes = "Anders mødte Bella og var meget interesseret.", EmployeeId = 4, ResultedInAdoption = false /* Endnu */ },
                new Visit { Id = 3, AnimalId = 5, /* Cooper */ PlannedDate = DateTime.UtcNow.AddDays(5), Type = "Interesse (Kanin)", Status = VisitStatus.Scheduled, PlannedDuration = 30, Visitor = "Familien Hansen", Notes = "Familie med børn vil gerne se på kaniner.", EmployeeId = 5 },
                new Visit { Id = 4, AnimalId = 7, /* Rocky */ PlannedDate = DateTime.UtcNow.AddDays(-10), ActualDate = DateTime.UtcNow.AddDays(-10), Type = "Adfærdskonsultation", Status = VisitStatus.Completed, PlannedDuration = 90, ActualDuration = 85, Visitor = "Hundetræner Karen", Notes = "Vurdering af Rockys adfærd og træningsbehov.", EmployeeId = 3 },
                new Visit { Id = 5, AnimalId = 1, /* Max */ PlannedDate = DateTime.UtcNow.AddDays(-80), ActualDate = DateTime.UtcNow.AddDays(-80), Type = "Første møde", Status = VisitStatus.Completed, PlannedDuration = 45, ActualDuration = 40, Visitor = "Potentiel adoptant (ikke Carla)", EmployeeId = 2, Notes = "God kemi, men de valgte en anden hund."}
            };
            // Antager AnimalId og CustomerId eksisterer
            return Task.FromResult(JsonSerializer.Serialize(visits, _jsonOptions));
        }

        private static Task<string> GetDefaultMembershipProductsJson()
        {
            var products = new List<MembershipProduct>
            {
                new MembershipProduct { Id = 1, Name = "Basis Støttemedlemskab", Description = "Årligt medlemskab der støtter vores daglige drift og dyrevelfærd. Inkluderer digitalt nyhedsbrev og 5% rabat i webshop.", Price = 250m, Frequency = BillingFrequency.Annually, IsAvailable = true, IsDonation = false },
                new MembershipProduct { Id = 2, Name = "Sølv Støttemedlemskab", Description = "Et større årligt bidrag med flere fordele, inklusiv vores trykte magasin, 10% rabat i webshop og invitation til årligt medlemsarrangement.", Price = 600m, Frequency = BillingFrequency.Annually, IsAvailable = true, IsDonation = false },
                new MembershipProduct { Id = 3, Name = "Guld Støttemedlemskab", Description = "Vores mest omfattende støttemedlemskab med eksklusive fordele: trykt magasin, 15% rabat i webshop, invitation til medlemsarrangement med ledsager, og særlig tak på hjemmesiden (valgfrit).", Price = 1200m, Frequency = BillingFrequency.Annually, IsAvailable = true, IsDonation = false },
                new MembershipProduct { Id = 4, Name = "Månedlig Støtte", Description = "Støt os med et fast månedligt beløb. Alle bidrag gør en forskel. Inkluderer digitalt nyhedsbrev.", Price = 75m, Frequency = BillingFrequency.Monthly, IsAvailable = true, IsDonation = true, AllowsCustomAmount = false },
                new MembershipProduct { Id = 5, Name = "Giv en Engangsdonation", Description = "Ethvert beløb hjælper os med at redde og passe på dyr i nød. Mindstebeløb 50 kr, maksimum 10.000 kr via denne formular.", Price = 0m, /* Pris sættes af bruger */ Frequency = BillingFrequency.OneTime, IsAvailable = true, IsDonation = true, AllowsCustomAmount = true }
            };
            return Task.FromResult(JsonSerializer.Serialize(products, _jsonOptions));
        }
        
        private static Task<string> GetDefaultCustomerMembershipsJson()
        {
            var customerMemberships = new List<CustomerMembership>
            {
                // Carla (CustomerId=3) har et Sølv medlemskab (ProductId=2)
                new CustomerMembership { Id = 1, CustomerId = 3, MembershipProductId = 2, StartDate = DateTime.UtcNow.AddMonths(-2), EndDate = DateTime.UtcNow.AddMonths(-2).AddYears(1), IsActive = true, PaymentMethod = PaymentMethodType.PaymentCard, LastPaymentDate = DateTime.UtcNow.AddMonths(-2), NextPaymentDate = DateTime.UtcNow.AddMonths(-2).AddYears(1), WantsDigitalMagazine = true, SubscribedToNewsletter = true, OptInForTaxDeduction = true },
                // Anders (CustomerId=1) har en Månedlig Støtte (ProductId=4)
                new CustomerMembership { Id = 2, CustomerId = 1, MembershipProductId = 4, StartDate = DateTime.UtcNow.AddDays(-40), IsActive = true, PaymentMethod = PaymentMethodType.Betalingsservice, LastPaymentDate = DateTime.UtcNow.AddDays(-10), /* Forudsat månedlig betaling */ NextPaymentDate = DateTime.UtcNow.AddDays(-10).AddMonths(1), WantsDigitalMagazine = true, SubscribedToNewsletter = false, OptInForTaxDeduction = false }
            };
            // Antager CustomerId og MembershipProductId eksisterer
            return Task.FromResult(JsonSerializer.Serialize(customerMemberships, _jsonOptions));
        }
    }
} 