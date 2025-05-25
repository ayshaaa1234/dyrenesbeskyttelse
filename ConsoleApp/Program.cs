using System;
using System.Threading.Tasks;
using ClassLibrary.Services;
using ClassLibrary.Repositories;
using ConsoleApp.Menus;

namespace ConsoleApp
{
    class Program
    {
        private static async Task Main(string[] args)
        {
            try
            {
                // Initialiser repositories
                var animalRepository = new AnimalRepository();
                var customerRepository = new CustomerRepository();
                var employeeRepository = new EmployeeRepository();
                var healthRecordRepository = new HealthRecordRepository();
                var visitLogRepository = new VisitLogRepository();
                var adoptionRepository = new AdoptionRepository();
                var bookingRepository = new BookingRepository();
                var blogPostRepository = new BlogPostRepository();
                var activityRepository = new ActivityRepository();

                // Initialiser services
                var animalService = new AnimalService(animalRepository);
                var customerService = new CustomerService(customerRepository);
                var employeeService = new EmployeeService(employeeRepository);
                var healthRecordService = new HealthRecordService(healthRecordRepository);
                var visitLogService = new VisitLogService(visitLogRepository);
                var adoptionService = new AdoptionService(adoptionRepository);
                var bookingService = new BookingService(bookingRepository);
                var blogPostService = new BlogPostService(blogPostRepository);
                var activityService = new ActivityService(activityRepository);

                // Opret hovedmenu
                var mainMenu = new MainMenu(
                    animalService,
                    customerService,
                    employeeService,
                    healthRecordService,
                    visitLogService,
                    adoptionService,
                    bookingService,
                    blogPostService,
                    activityService);

                // Start hovedmenu
                await mainMenu.ShowAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Der opstod en fejl: {ex.Message}");
                Console.WriteLine("\nTryk på en tast for at afslutte...");
                Console.ReadKey();
            }
        }
    }
}
