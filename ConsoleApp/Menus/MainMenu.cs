using System;
using System.Threading.Tasks;
using ClassLibrary.Features.AnimalManagement.Application.Abstractions;
using ClassLibrary.Features.Adoptions.Application.Abstractions;
using ClassLibrary.Features.Blog.Application.Abstractions;
using ClassLibrary.Features.Customers.Application.Abstractions;
using ClassLibrary.Features.Employees.Application.Abstractions;
using ClassLibrary.Features.Memberships.Application.Abstractions;

namespace ConsoleApp.Menus
{
    public class MainMenu
    {
        private readonly IAnimalManagementService _animalManagementService;
        private readonly ICustomerService _customerService;
        private readonly IEmployeeService _employeeService;
        private readonly IAdoptionService _adoptionService;
        private readonly IBlogPostService _blogPostService;
        private readonly IMembershipService _membershipService;

        public MainMenu(
            IAnimalManagementService animalManagementService,
            ICustomerService customerService,
            IEmployeeService employeeService,
            IAdoptionService adoptionService,
            IBlogPostService blogPostService,
            IMembershipService membershipService)
        {
            _animalManagementService = animalManagementService;
            _customerService = customerService;
            _employeeService = employeeService;
            _adoptionService = adoptionService;
            _blogPostService = blogPostService;
            _membershipService = membershipService;
        }

        public async Task ShowAsync()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Hovedmenu");
                Console.WriteLine("----------------------------------------");
                Console.WriteLine("1. Dyreadministration");
                Console.WriteLine("2. Kundeadministration");
                Console.WriteLine("3. Medarbejderadministration");
                Console.WriteLine("4. Adoptionsadministration");
                Console.WriteLine("5. Blogadministration");
                Console.WriteLine("6. Medlemskabsadministration");
                Console.WriteLine("0. Afslut");
                Console.WriteLine("----------------------------------------");
                Console.Write("Tag et valg: ");

                string? choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        AnimalMenu animalMenu = new AnimalMenu(_animalManagementService);
                        await animalMenu.ShowAsync();
                        break;
                    case "2":
                        CustomerMenu customerMenu = new CustomerMenu(_customerService);
                        await customerMenu.ShowAsync();
                        break;
                    case "3":
                        EmployeeMenu employeeMenu = new EmployeeMenu(_employeeService);
                        await employeeMenu.ShowAsync();
                        break;
                    case "4":
                        AdoptionMenu adoptionMenu = new AdoptionMenu(_adoptionService, _animalManagementService, _customerService);
                        await adoptionMenu.ShowAsync();
                        break;
                    case "5":
                        BlogMenu blogMenu = new BlogMenu(_blogPostService);
                        await blogMenu.ShowAsync();
                        break;
                    case "6":
                        MembershipMenu membershipMenu = new MembershipMenu(_membershipService, _customerService);
                        await membershipMenu.ShowAsync();
                        break;
                    case "0":
                        Console.WriteLine("Afslutter programmet.");
                        return;
                    default:
                        Console.WriteLine("Ugyldigt valg. Prøv igen.");
                        Console.ReadKey();
                        break;
                }
            }
        }
    }
} 