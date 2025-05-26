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

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// Registrer ClassLibrary services
// Animal Management
builder.Services.AddScoped<IAnimalRepository, AnimalRepository>();
builder.Services.AddScoped<IHealthRecordRepository, HealthRecordRepository>();
builder.Services.AddScoped<IVisitRepository, VisitRepository>();
builder.Services.AddScoped<IAnimalManagementService, AnimalManagementService>();
// Customers
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
// Employees
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
// Adoptions
builder.Services.AddScoped<IAdoptionRepository, AdoptionRepository>();
builder.Services.AddScoped<IAdoptionService, AdoptionService>();
// Blog
builder.Services.AddScoped<IBlogPostRepository, BlogPostRepository>();
builder.Services.AddScoped<IBlogPostService, BlogPostService>();
// Memberships
builder.Services.AddScoped<IMembershipProductRepository, MembershipProductRepository>();
builder.Services.AddScoped<ICustomerMembershipRepository, CustomerMembershipRepository>();
builder.Services.AddScoped<IMembershipService, MembershipService>();

var app = builder.Build();

// Initialiser data (kan evt. kun gøres i Development miljøet)
if (app.Environment.IsDevelopment())
{
    Console.WriteLine("Initialiserer JSON datafiler for RazorPagesApp (Development)...");
    await JsonDataInitializer.InitializeAsync();
    Console.WriteLine("JSON data initialisering færdig for RazorPagesApp.");
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); // Sørg for at UseStaticFiles er tilføjet for CSS, JS osv.

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages(); // MapRazorPages uden .WithStaticAssets() her, da vi bruger UseStaticFiles() separat

app.Run();
