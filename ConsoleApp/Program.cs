using System;
using System.Linq;
using System.Threading.Tasks;
using ClassLibrary.Features.AnimalManagement.Application.Abstractions;
using ClassLibrary.Features.AnimalManagement.Application.Implementations;
using ClassLibrary.Features.AnimalManagement.Infrastructure.Abstractions;
using ClassLibrary.Features.AnimalManagement.Infrastructure.Implementations;
using ClassLibrary.Features.Customers.Application.Abstractions;
using ClassLibrary.Features.Customers.Application.Implementations;
using ClassLibrary.Features.Customers.Infrastructure.Abstractions;
using ClassLibrary.Features.Customers.Infrastructure.Implementations;
using ClassLibrary.Features.Employees.Application.Abstractions;
using ClassLibrary.Features.Employees.Application.Implementations;
using ClassLibrary.Features.Employees.Infrastructure.Abstractions;
using ClassLibrary.Features.Employees.Infrastructure.Implementations;
using ClassLibrary.Features.Adoptions.Application.Abstractions;
using ClassLibrary.Features.Adoptions.Application.Implementations;
using ClassLibrary.Features.Adoptions.Infrastructure.Abstractions;
using ClassLibrary.Features.Adoptions.Infrastructure.Implementations;
using ClassLibrary.Features.Blog.Application.Abstractions;
using ClassLibrary.Features.Blog.Application.Implementations;
using ClassLibrary.Features.Blog.Infrastructure.Abstractions;
using ClassLibrary.Features.Blog.Infrastructure.Implementations;
using ClassLibrary.Features.Memberships.Application.Abstractions;
using ClassLibrary.Features.Memberships.Application.Implementations;
using ClassLibrary.Features.Memberships.Infrastructure.Abstractions;
using ClassLibrary.Features.Memberships.Infrastructure.Implementations;
using ClassLibrary.Infrastructure.DataInitialization; // For JsonDataInitializer
using Microsoft.Extensions.DependencyInjection;
using ConsoleApp.Menus; // For MainMenu

namespace ConsoleApp
{
    /// <summary>
    /// Hovedklassen for konsolapplikationen.
    /// Ansvarlig for opsætning af dependency injection, datainitialisering og start af hovedmenuen.
    /// </summary>
    class Program
    {
        /// <summary>
        /// Applikationens indgangspunkt.
        /// Konfigurerer services, initialiserer data, og starter brugerinteraktionen via MainMenu.
        /// Håndterer også overordnet fejlhåndtering for applikationen.
        /// </summary>
        /// <param name="args">Kommandolinjeargumenter (bruges ikke i denne applikation).</param>
        /// <returns>En Task, der repræsenterer den asynkrone udførelse af programmet.</returns>
        static async Task Main(string[] args)
        {
            try
            {
                // Opret DI container
                var serviceProvider = new ServiceCollection()
                    // Animal Management
                    .AddScoped<IAnimalRepository, AnimalRepository>()
                    .AddScoped<IHealthRecordRepository, HealthRecordRepository>()
                    .AddScoped<IVisitRepository, VisitRepository>()
                    .AddScoped<IAnimalManagementService, AnimalManagementService>()
                    // Customers
                    .AddScoped<ICustomerRepository, CustomerRepository>()
                    .AddScoped<ICustomerService, CustomerService>()
                    // Employees
                    .AddScoped<IEmployeeRepository, EmployeeRepository>()
                    .AddScoped<IEmployeeService, EmployeeService>()
                    // Adoptions
                    .AddScoped<IAdoptionRepository, AdoptionRepository>()
                    .AddScoped<IAdoptionService, AdoptionService>()
                    // Blog
                    .AddScoped<IBlogPostRepository, BlogPostRepository>()
                    .AddScoped<IBlogPostService, BlogPostService>()
                    // Memberships
                    .AddScoped<IMembershipProductRepository, MembershipProductRepository>()
                    .AddScoped<ICustomerMembershipRepository, CustomerMembershipRepository>()
                    .AddScoped<IMembershipService, MembershipService>()
                    .BuildServiceProvider();

                // Initialiser data
                Console.WriteLine("Initialiserer JSON datafiler...");
                await JsonDataInitializer.InitializeAsync();
                Console.WriteLine("JSON data initialisering færdig.");

                // Hent services fra DI containeren
                var animalManagementService = serviceProvider.GetRequiredService<IAnimalManagementService>();
                var customerService = serviceProvider.GetRequiredService<ICustomerService>();
                var employeeService = serviceProvider.GetRequiredService<IEmployeeService>();
                var adoptionService = serviceProvider.GetRequiredService<IAdoptionService>();
                var blogPostService = serviceProvider.GetRequiredService<IBlogPostService>();
                var membershipService = serviceProvider.GetRequiredService<IMembershipService>();


                // Kør MainMenu
                var mainMenu = new MainMenu(
                    animalManagementService, 
                    customerService, 
                    employeeService, 
                    adoptionService, 
                    blogPostService,
                    membershipService
                );
                await mainMenu.ShowAsync();

                Console.WriteLine("\nTryk på en tast for at afslutte...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Der opstod en kritisk fejl: {ex.Message}");
                Console.WriteLine(ex.StackTrace); // Nyttigt for debugging
                Console.WriteLine("\nTryk på en tast for at afslutte...");
                Console.ReadKey();
            }
        }
    }
}
