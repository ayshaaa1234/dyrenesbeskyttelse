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
    public class AnsogningModel : PageModel
    {
        private readonly IAdoptionService _adoptionService;
        private readonly IAnimalManagementService _animalService;
        private readonly ICustomerService _customerService;

        public AnsogningModel(
            IAdoptionService adoptionService,
            IAnimalManagementService animalService,
            ICustomerService customerService)
        {
            _adoptionService = adoptionService;
            _animalService = animalService;
            _customerService = customerService;
        }

        [BindProperty]
        public AdoptionInputModel Input { get; set; } = new AdoptionInputModel();
        
        public Animal? AnimalToAdopt { get; set; }

        public async Task<IActionResult> OnGetAsync(int dyrId)
        {
            AnimalToAdopt = await _animalService.GetAnimalByIdAsync(dyrId);

            if (AnimalToAdopt == null || AnimalToAdopt.Status != AnimalStatus.Available)
            {
                // Dyr ikke fundet eller ikke tilgængeligt
                // Overvej TempData besked og redirect til Adoptions Index eller en fejlside
                TempData["ErrorMessage"] = "Dette dyr kan desværre ikke adopteres i øjeblikket.";
                return RedirectToPage("./Index");
            }

            Input.AnimalId = dyrId;
            Input.AnimalName = AnimalToAdopt.Name;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                // Gen-hent dyrenavn hvis validering fejler, da det ikke er en del af postet model
                if (Input.AnimalId > 0)
                {
                    var animal = await _animalService.GetAnimalByIdAsync(Input.AnimalId);
                    Input.AnimalName = animal?.Name;
                }
                return Page();
            }

            AnimalToAdopt = await _animalService.GetAnimalByIdAsync(Input.AnimalId);
            if (AnimalToAdopt == null || AnimalToAdopt.Status != AnimalStatus.Available)
            {
                ModelState.AddModelError(string.Empty, "Dette dyr er ikke længere tilgængeligt for adoption.");
                return Page();
            }

            // Find eller opret kunde
            Customer? customer = await _customerService.GetByEmailAsync(Input.CustomerEmail);
            if (customer == null)
            {
                string firstName = Input.CustomerName;
                string lastName = string.Empty;
                int firstSpaceIndex = Input.CustomerName.IndexOf(' ');
                if (firstSpaceIndex > 0)
                {
                    firstName = Input.CustomerName.Substring(0, firstSpaceIndex);
                    lastName = Input.CustomerName.Substring(firstSpaceIndex + 1);
                }

                customer = new Customer
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Email = Input.CustomerEmail,
                    Phone = Input.CustomerPhone,
                    RegistrationDate = System.DateTime.UtcNow
                };
                customer = await _customerService.CreateAsync(customer);
            }

            var newAdoption = new ClassLibrary.Features.Adoptions.Core.Models.Adoption
            {
                AnimalId = Input.AnimalId,
                CustomerId = customer.Id,
                ApplicationDate = System.DateTime.UtcNow,
                Status = ClassLibrary.Features.Adoptions.Core.Enums.AdoptionStatus.Pending,
                Notes = Input.Notes ?? string.Empty
            };

            var createdAdoption = await _adoptionService.CreateAdoptionAsync(newAdoption);

            if (createdAdoption != null)
            {
                // Opdater dyrets status til Reserveret
                AnimalToAdopt.Status = AnimalStatus.Reserved;
                await _animalService.UpdateAnimalAsync(AnimalToAdopt);
                
                // Gem info til bekræftelsesside
                TempData["AdoptionId"] = createdAdoption.Id;
                TempData["AnimalName"] = AnimalToAdopt.Name;
                return RedirectToPage("./TakForAnsogning");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Der opstod en fejl under oprettelse af din ansøgning. Prøv venligst igen.");
                // Gen-hent dyrenavn
                Input.AnimalName = AnimalToAdopt.Name;
                return Page();
            }
        }
    }
} 