using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ClassLibrary.Features.Adoptions.Application.Abstractions;
using ClassLibrary.Features.Adoptions.Core.Models;
using ClassLibrary.Features.Adoptions.Core.Enums;
using ClassLibrary.Features.AnimalManagement.Application.Abstractions;
using ClassLibrary.Features.AnimalManagement.Core.Models;
using ClassLibrary.Features.AnimalManagement.Core.Enums;
using ClassLibrary.Features.Customers.Application.Abstractions;
using ClassLibrary.Features.Customers.Core.Models;
using RazorPagesApp.Pages.Adoption.Models;
using System.Threading.Tasks;
using System;

namespace RazorPagesApp.Pages.Adoption
{
    // PageModel for adoptionsansøgningssiden.
    public class AnsogningModel : PageModel
    {
        // Services der bruges til at håndtere adoptioner, dyr og kunder.
        private readonly IAdoptionService _adoptionService;
        private readonly IAnimalManagementService _animalService;
        private readonly ICustomerService _customerService;

        // Dependency injection af de nødvendige services.
        public AnsogningModel(
            IAdoptionService adoptionService,
            IAnimalManagementService animalService,
            ICustomerService customerService)
        {
            _adoptionService = adoptionService;
            _animalService = animalService;
            _customerService = customerService;
        }

        [BindProperty] // Binder Input propertien til de data, der postes fra formularen.
        public AdoptionInputModel Input { get; set; } = new AdoptionInputModel(); // Inputmodel for ansøgningsformularen.
        
        public Animal? AnimalToAdopt { get; set; } // Holder information om det dyr, der ansøges om.

        // Handler for GET-requests. Kaldes når siden indlæses med et dyre-ID (dyrId).
        public async Task<IActionResult> OnGetAsync(int dyrId)
        {
            AnimalToAdopt = await _animalService.GetAnimalByIdAsync(dyrId); // Henter dyret fra databasen.

            // Tjekker om dyret findes og er tilgængeligt for adoption.
            if (AnimalToAdopt == null || AnimalToAdopt.Status != AnimalStatus.Available)
            {
                // Hvis dyret ikke kan adopteres, vises en fejlmeddelelse via TempData og brugeren omdirigeres.
                TempData["ErrorMessage"] = "Dette dyr kan desværre ikke adopteres i øjeblikket.";
                return RedirectToPage("./Index"); // Omdirigerer til adoptionsindexsiden.
            }

            // Udfylder Input modellens properties med data fra det valgte dyr.
            Input.AnimalId = dyrId;
            Input.AnimalName = AnimalToAdopt.Name;
            return Page(); // Returnerer Razor Page.
        }

        // Handler for POST-requests. Kaldes når ansøgningsformularen submittes.
        public async Task<IActionResult> OnPostAsync()
        {
            // Tjekker om modelstate er valid (dvs. om alle valideringsregler i AdoptionInputModel er overholdt).
            if (!ModelState.IsValid)
            {
                // Hvis validering fejler, genindlæses siden med de indtastede data og fejlmeddelelser.
                // Dyrets navn skal genhentes, da det ikke er en del af den postede formular, men vises i UI.
                if (Input.AnimalId > 0)
                {
                    var animal = await _animalService.GetAnimalByIdAsync(Input.AnimalId);
                    Input.AnimalName = animal?.Name;
                }
                return Page();
            }

            // Genhent dyret for at sikre, at status ikke har ændret sig siden OnGet.
            AnimalToAdopt = await _animalService.GetAnimalByIdAsync(Input.AnimalId);
            if (AnimalToAdopt == null || AnimalToAdopt.Status != AnimalStatus.Available)
            {
                ModelState.AddModelError(string.Empty, "Dette dyr er ikke længere tilgængeligt for adoption.");
                Input.AnimalName = AnimalToAdopt?.Name; // Sørg for at navnet er sat, hvis dyret stadig findes.
                return Page();
            }

            // Finder eller opretter en kunde baseret på den indtastede email.
            Customer? customer = await _customerService.GetByEmailAsync(Input.CustomerEmail);
            if (customer == null)
            {
                // Deler CustomerName op i fornavn og efternavn (simpel implementering).
                string firstName = Input.CustomerName;
                string lastName = string.Empty;
                int firstSpaceIndex = Input.CustomerName.IndexOf(' ');
                if (firstSpaceIndex > 0)
                {
                    firstName = Input.CustomerName.Substring(0, firstSpaceIndex);
                    lastName = Input.CustomerName.Substring(firstSpaceIndex + 1);
                }

                // Opretter en ny kunde.
                customer = new Customer
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Email = Input.CustomerEmail,
                    Phone = Input.CustomerPhone,
                    RegistrationDate = System.DateTime.UtcNow // Sætter registreringsdato til nu.
                };
                customer = await _customerService.CreateAsync(customer); // Gemmer kunden.
            }

            // Opretter en ny adoptionsansøgning.
            var newAdoption = new ClassLibrary.Features.Adoptions.Core.Models.Adoption
            {
                AnimalId = Input.AnimalId,
                CustomerId = customer.Id, // Bruger ID fra den fundne eller oprettede kunde.
                ApplicationDate = System.DateTime.UtcNow, // Sætter ansøgningsdato til nu.
                Status = ClassLibrary.Features.Adoptions.Core.Enums.AdoptionStatus.Pending, // Sætter status til Afventer.
                Notes = Input.Notes ?? string.Empty // Tilføjer noter, hvis de er angivet.
            };

            var createdAdoption = await _adoptionService.CreateAdoptionAsync(newAdoption); // Gemmer adoptionsansøgningen.

            if (createdAdoption != null)
            {
                // Opdaterer dyrets status til Reserveret, da der nu er en aktiv ansøgning.
                AnimalToAdopt.Status = AnimalStatus.Reserved;
                await _animalService.UpdateAnimalAsync(AnimalToAdopt);
                
                // Gemmer information i TempData til brug på bekræftelsessiden (TakForAnsogning).
                TempData["AdoptionId"] = createdAdoption.Id;
                TempData["AnimalName"] = AnimalToAdopt.Name;
                return RedirectToPage("./TakForAnsogning"); // Omdirigerer til bekræftelsessiden.
            }
            else
            {
                // Hvis oprettelse af adoption fejler, tilføjes en fejlmeddelelse til ModelState.
                ModelState.AddModelError(string.Empty, "Der opstod en fejl under oprettelse af din ansøgning. Prøv venligst igen.");
                Input.AnimalName = AnimalToAdopt.Name; // Sikrer at dyrets navn er sat for visning af fejlside.
                return Page(); // Returnerer siden med fejlmeddelelsen.
            }
        }
    }
} 