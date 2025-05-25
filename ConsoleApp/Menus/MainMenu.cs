using System;
using System.Threading.Tasks;
using ClassLibrary.Services;

namespace ConsoleApp.Menus
{
    /// <summary>
    /// Hovedmenu for applikationen
    /// </summary>
    public class MainMenu : MenuBase
    {
        private readonly AnimalMenu _animalMenu;
        private readonly CustomerMenu _customerMenu;
        private readonly EmployeeMenu _employeeMenu;
        private readonly HealthRecordMenu _healthRecordMenu;
        private readonly VisitLogMenu _visitLogMenu;
        private readonly AdoptionMenu _adoptionMenu;
        private readonly BookingMenu _bookingMenu;
        private readonly BlogPostMenu _blogPostMenu;
        private readonly ActivityMenu _activityMenu;

        public MainMenu(
            AnimalService animalService,
            CustomerService customerService,
            EmployeeService employeeService,
            HealthRecordService healthRecordService,
            VisitLogService visitLogService,
            AdoptionService adoptionService,
            BookingService bookingService,
            BlogPostService blogPostService,
            ActivityService activityService)
        {
            _animalMenu = new AnimalMenu(animalService);
            _customerMenu = new CustomerMenu(customerService);
            _employeeMenu = new EmployeeMenu(employeeService);
            _healthRecordMenu = new HealthRecordMenu(healthRecordService);
            _visitLogMenu = new VisitLogMenu(visitLogService);
            _adoptionMenu = new AdoptionMenu(adoptionService);
            _bookingMenu = new BookingMenu(bookingService);
            _blogPostMenu = new BlogPostMenu(blogPostService);
            _activityMenu = new ActivityMenu(activityService);
        }

        public override async Task ShowAsync()
        {
            while (true)
            {
                ShowHeader("Dyrenes Beskyttelse - Hovedmenu");
                Console.WriteLine("1. Dyr");
                Console.WriteLine("2. Kunder");
                Console.WriteLine("3. Medarbejdere");
                Console.WriteLine("4. Sundhedsjournaler");
                Console.WriteLine("5. Besøgslog");
                Console.WriteLine("6. Adoptioner");
                Console.WriteLine("7. Bookinger");
                Console.WriteLine("8. Blogindlæg");
                Console.WriteLine("9. Aktiviteter");
                Console.WriteLine("0. Afslut");
                Console.Write("\nVælg en mulighed: ");

                var choice = Console.ReadLine();

                try
                {
                    switch (choice)
                    {
                        case "1":
                            await _animalMenu.ShowAsync();
                            break;
                        case "2":
                            await _customerMenu.ShowAsync();
                            break;
                        case "3":
                            await _employeeMenu.ShowAsync();
                            break;
                        case "4":
                            await _healthRecordMenu.ShowAsync();
                            break;
                        case "5":
                            await _visitLogMenu.ShowAsync();
                            break;
                        case "6":
                            await _adoptionMenu.ShowAsync();
                            break;
                        case "7":
                            await _bookingMenu.ShowAsync();
                            break;
                        case "8":
                            await _blogPostMenu.ShowAsync();
                            break;
                        case "9":
                            await _activityMenu.ShowAsync();
                            break;
                        case "0":
                            return;
                        default:
                            ShowError("Ugyldigt valg");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    HandleException(ex);
                }
            }
        }
    }
} 