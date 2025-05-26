# ConsoleApp for Dyrenes Beskyttelse

## Projektoversigt
Denne `ConsoleApp` tjener som en test- og demonstrationsplatform for `ClassLibrary` til Dyrenes Beskyttelse systemet. Applikationen giver en tekstbaseret brugergrænseflade (TUI) til at interagere med de forskellige features og services, der er implementeret i `ClassLibrary`.

Formålet er at give en enkel måde at:
*   Teste funktionaliteten af de forskellige services (`AnimalManagementService`, `CustomerService`, etc.).
*   Demonstrere CRUD-operationer og anden forretningslogik.
*   Manuelt inspicere og manipulere data, der persisteres (primært via JSON-filer af `ClassLibrary`).

## Projektstruktur

Den overordnede struktur for `ConsoleApp` er som følger:

```
ConsoleApp/
├───Menus/                     # Indeholder alle menu-klasser for brugergrænsefladen
│   ├───MainMenu.cs            # Hovedmenuen, der navigerer til undermenuer
│   ├───AnimalMenu.cs          # Menu for håndtering af dyr
│   ├───CustomerMenu.cs        # Menu for håndtering af kunder
│   ├───EmployeeMenu.cs        # Menu for håndtering af medarbejdere
│   ├───AdoptionMenu.cs        # Menu for håndtering af adoptioner
│   ├───BlogMenu.cs            # Menu for håndtering af blogindlæg
│   └───MembershipMenu.cs      # Menu for håndtering af medlemskaber
│
├───Properties/
│   └───launchSettings.json    # Konfiguration for hvordan applikationen startes
│
├───bin/
├───obj/
│
├───ConsoleApp.csproj          # Projektfilen for konsolapplikationen
├───Dockerfile                 # Dockerfile til containerisering (hvis relevant)
├───Program.cs                 # Hovedindgangspunktet for applikationen, opsætter DI og starter MainMenu
└───README.md                  # Denne fil
```

## Kernekomponenter

### `Program.cs`
*   **Dependency Injection (DI):** Opsætter en DI container (`ServiceProvider`) til at registrere og resolvere alle nødvendige services og repositories fra `ClassLibrary`.
*   **Data Initialisering:** Kalder `JsonDataInitializer.InitializeAsync()` fra `ClassLibrary` for at sikre, at de nødvendige JSON-datafiler er oprettet og eventuelt seeded.
*   **Menu Start:** Initialiserer og starter `MainMenu`, som er udgangspunktet for brugerinteraktion.
*   **Global Fejlhåndtering:** Indeholder en overordnet try-catch blok til at håndtere uventede fejl under kørsel.

### Menuer (`ConsoleApp/Menus/`)
Systemet anvender en række menu-klasser til at strukturere brugerinteraktionen:

*   **`MainMenu.cs`:**
    *   Det centrale navigationspunkt.
    *   Tillader brugeren at vælge mellem de forskellige feature-specifikke undermenuer.
    *   Injecter de nødvendige services fra `ClassLibrary` og videresender dem til de respektive undermenuer.

*   **Feature-Specifikke Menuer (f.eks. `AnimalMenu.cs`, `CustomerMenu.cs`):**
    *   Hver menu er ansvarlig for at præsentere muligheder relateret til en specifik feature i `ClassLibrary` (f.eks. dyreadministration, kundeadministration).
    *   Implementerer metoder til at vise lister, oprette, opdatere, slette og udføre andre specifikke handlinger ved at kalde de relevante metoder på de injectede services.
    *   Indeholder logik for at indsamle brugerinput og validere det i et vist omfang før det sendes til service-laget.
    *   Anvender en række private hjælpemetoder til at standardisere input-opsamling for heltal, decimaltal, datoer, boolean værdier og enums.

## Forudsætninger

*   .NET SDK (version specificeret i `ConsoleApp.csproj` eller en kompatibel version - typisk .NET 6.0 eller nyere).
*   En reference til `ClassLibrary` projektet.

## Kørsel af Applikationen

1.  **Sørg for at `ClassLibrary` er bygget:** Da `ConsoleApp` afhænger af `ClassLibrary`, skal sidstnævnte kunne bygges uden fejl.
2.  **Åbn en terminal eller kommandoprompt** i roden af `ConsoleApp` mappen.
3.  **Kør applikationen** ved hjælp af .NET CLI:
    ```bash
    dotnet run
    ```
    Eller, hvis du har konfigureret en specifik launch profil (f.eks. i Visual Studio eller Rider):
    ```bash
    dotnet run --launch-profile [DinLaunchProfilNavn]
    ```
4.  **Følg instruktionerne på skærmen** for at navigere i menuerne og interagere med systemet.

### Datafiler
`ConsoleApp` interagerer med data, der administreres af `ClassLibrary`. Disse datafiler (typisk JSON) vil blive oprettet/opdateret i en `Data/` mappe i output-biblioteket for `ClassLibrary` (f.eks. `ClassLibrary/bin/Debug/netX.X/Data/`), når `JsonDataInitializer.InitializeAsync()` køres.

## Afhængigheder

*   **`ClassLibrary`:** Den primære afhængighed, som indeholder al forretningslogik og dataadgang.
*   **`Microsoft.Extensions.DependencyInjection`:** Bruges til at opsætte dependency injection i `Program.cs`.

Se `ConsoleApp.csproj` for en komplet liste over NuGet-pakker og projektreferencer. 