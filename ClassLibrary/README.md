# Dyrenes Beskyttelse Class Library

## Projektoversigt
Dette Class Library implementerer forretningslogik og datahåndtering for Dyrenes Beskyttelse systemet.
Biblioteket er designet med en feature-baseret tilgang, inspireret af Clean Architecture principper og følger SOLID. Datapersistens håndteres primært via JSON-filer.

## Projektstruktur

Den overordnede struktur for `ClassLibrary` er som følger:

```
ClassLibrary/
├───Features/                  # Hovedmappe for alle systemets features
│   └───[FeatureName]/         # Mappe for en specifik feature (f.eks. AnimalManagement, Customers)
│       ├───Application/       # Applikationslogik for featuren
│       │   ├───Abstractions/  # Service interfaces (f.eks. IAnimalManagementService)
│       │   └───Implementations/ # Service implementeringer (f.eks. AnimalManagementService)
│       │   └───Models/        # DTOs/Request-Response modeller (hvis brugt specifikt for dette lag)
│       ├───Core/              # Kerne-domænelogik og modeller for featuren
│       │   ├───Models/        # Domænemodeller (f.eks. Animal, Customer)
│       │   ├───Enums/         # Enumerationer specifikt for denne feature (f.eks. AnimalStatus)
│       │   └───Abstractions/  # Domæne-niveau abstractions (kan inkludere repository interfaces)
│       ├───Infrastructure/    # Infrastruktur-specifikke implementeringer for featuren
│       │   ├───Abstractions/  # Repository interfaces (f.eks. IAnimalRepository)
│       │   └───Implementations/ # Repository implementeringer (f.eks. AnimalRepository for JSON)
│       └───Exceptions/        # Feature-specifikke exceptions
│
├───Infrastructure/            # Generel infrastruktur for biblioteket
│   └───DataInitialization/    # Logik for data initialisering og seeding (f.eks. JsonDataInitializer)
│
├───SharedKernel/              # Kode der deles på tværs af flere features/lag
│   ├───Application/           # Delte applikations-komponenter (f.eks. BaseUserService)
│   │   ├───Abstractions/
│   │   └───Implementations/
│   ├───Domain/                # Delte domæne-koncepter (f.eks. BaseUser, IEntity)
│   │   ├───Abstractions/
│   │   └───Models/
│   ├───Persistence/           # Delte persistens-komponenter (f.eks. generisk Repository<T>)
│   │   ├───Abstractions/
│   │   └───Implementations/
│   └───Exceptions/            # Generelle exceptions for biblioteket
│
├───README.md                  # Denne fil
└───ClassLibrary.csproj        # Projektfil
```

## Kernekomponenter

### Features og Services
Biblioteket er organiseret omkring følgende features, hver med deres primære service:

1.  **AnimalManagement** (`AnimalManagementService`)
    *   Håndterer dyr (`Animal`), sundhedsjournaler (`HealthRecord`) og besøg (`Visit`).
    *   Inkluderer CRUD, specifikke søgninger som `GetAnimalsNeedingVaccinationAsync`, og logik for vaccination og besøg.

2.  **Adoptions** (`AdoptionService`)
    *   Håndterer adoptionsprocessen (`Adoption`).
    *   Inkluderer CRUD, søgning, godkendelse, afvisning og gennemførelse af adoptioner.

3.  **Customers** (`CustomerService`)
    *   Håndterer kundeinformation (`Customer`).
    *   Inkluderer CRUD, specifikke søgninger, og tjek for aktive adoptioner ved sletning.

4.  **Employees** (`EmployeeService`)
    *   Håndterer medarbejderinformation (`Employee`).
    *   Inkluderer CRUD, specifikke søgninger, og håndtering af medarbejderdata som stilling og specialiseringer.

5.  **Blog** (`BlogPostService`)
    *   Håndterer den fulde livscyklus for blogindlæg (`BlogPost`), herunder alle aspekter fra oprettelse til sletning (CRUD).
    *   Dette inkluderer administration af centrale datafelter såsom titel, indhold, resumé, `PictureUrl`, forfatter, kategori, publiceringsdato (`PublishDate`) og status (`IsPublished`), samt håndtering af `Tags` og `Likes`.

6.  **Memberships** (`MembershipService`)
    *   Håndterer medlemskabsprodukter (`MembershipProduct`) og kundemedlemskaber (`CustomerMembership`).
    *   Inkluderer CRUD, registrering af betalinger (`RecordPaymentAsync`), og opslag som `GetMembershipsByPaymentStatusAsync`.

### Repositories
Hver feature har tilhørende repositories (f.eks. `IAnimalRepository`, `ICustomerRepository`) der håndterer dataadgang. Implementeringer findes typisk under `Features/[FeatureName]/Infrastructure/Implementations/` og benytter ofte `Repository<T>` fra `SharedKernel` for JSON-baseret lagring.

### Domænemodeller og Enums
Primære modeller og enums er placeret indenfor deres feature-mapper (`Core/Models` og `Core/Enums`):

*   **AnimalManagement:**
    *   `Animal` (Model - inkluderer `PictureUrl`)
    *   `HealthRecord` (Model)
    *   `Visit` (Model)
    *   `AnimalStatus` (Enum)
    *   `Species` (Enum)
    *   `VisitStatus` (Enum)
*   **Adoptions:**
    *   `Adoption` (Model)
    *   `AdoptionStatus` (Enum)
*   **Customers:**
    *   `Customer` (Model, arver fra `BaseUser`)
*   **Employees:**
    *   `Employee` (Model, arver fra `BaseUser` - inkluderer `PictureUrl`)
*   **Blog:**
    *   `BlogPost` (Model - inkluderer `PictureUrl`)
*   **Memberships:**
    *   `CustomerMembership` (Model)
    *   `MembershipProduct` (Model)
    *   `BillingFrequency` (Enum)
    *   `PaymentMethodType` (Enum)

*   **SharedKernel:**
    *   `BaseUser` (Abstrakt Model)
    *   `IEntity` (Interface)
    *   `ISoftDelete` (Interface)

## Brug i andre projekter

### Installation
1.  Tilføj `ClassLibrary` som en projekt reference i dit hovedprojekt.
2.  Konfigurer Dependency Injection (DI) for services og repositories.

### Eksempel på DI-konfiguration (.NET Core):
```csharp
// I Program.cs eller Startup.cs

// Animal Management
services.AddScoped<IAnimalRepository, AnimalRepository>();
services.AddScoped<IHealthRecordRepository, HealthRecordRepository>();
services.AddScoped<IVisitRepository, VisitRepository>();
services.AddScoped<IAnimalManagementService, AnimalManagementService>();

// Customers
services.AddScoped<ICustomerRepository, CustomerRepository>();
services.AddScoped<ICustomerService, CustomerService>();

// Employees
services.AddScoped<IEmployeeRepository, EmployeeRepository>();
services.AddScoped<IEmployeeService, EmployeeService>();

// Adoptions
services.AddScoped<IAdoptionRepository, AdoptionRepository>();
services.AddScoped<IAdoptionService, AdoptionService>();

// Blog
services.AddScoped<IBlogPostRepository, BlogPostRepository>();
services.AddScoped<IBlogPostService, BlogPostService>();

// Memberships
services.AddScoped<IMembershipProductRepository, MembershipProductRepository>();
services.AddScoped<ICustomerMembershipRepository, CustomerMembershipRepository>();
services.AddScoped<IMembershipService, MembershipService>();
```

### Data Initialisering
Kald `JsonDataInitializer.InitializeAsync()` ved applikationsstart for at sikre oprettelse/seeding af JSON-datafiler:
```csharp
// I Program.cs (f.eks. før app.Run())
await ClassLibrary.Infrastructure.DataInitialization.JsonDataInitializer.InitializeAsync();
```

**Note:** Overvej på sigt at flytte repository interfaces (f.eks. `IAnimalRepository`) fra `Features/[FeatureName]/Infrastructure/Abstractions/` til `Features/[FeatureName]/Core/Abstractions/` for en endnu renere arkitektur.