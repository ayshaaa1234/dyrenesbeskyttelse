# Dyrenesbeskyttelse Projekt

Dette projekt indeholder tre hovedkomponenter:
- ClassLibrary: Et delt bibliotek med fælles funktionalitet
- ConsoleApp: En konsolapplikation til test
- RazorPagesApp: En demo webapplikation

## Krav
- .NET 9.0 SDK
- Visual Studio 2022 eller Visual Studio Code

## Kørsel af projekterne

### ClassLibrary
Dette er et delt bibliotek der bruges af både ConsoleApp og RazorPagesApp. Det behøver ikke at køres separat.
Dokumentation: [ClassLibrary README](./ClassLibrary/README.md)

### ConsoleApp (CLI/Terminal TEST)
1. Åbn en terminal i projektets rodmappe
2. dotnet run --project ./ConsoleApp
Dokumentation: [ConsoleApp README](./ConsoleApp/README.md)

### RazorPagesApp (Hjemmeside)
1. Åbn en terminal i projektets rodmappe
2. dotnet run --project ./RazorPagesApp
