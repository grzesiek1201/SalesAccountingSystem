using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using AccountingSystem.Application.Services;
using AccountingSystem.Application.Validation.Customers;
using AccountingSystem.Application.Validation.Products;
using AccountingSystem.Application.Validation.Quotations;
using AccountingSystem.UI;
using Microsoft.EntityFrameworkCore;
using AccountingSystem.Infrastructure.Data;

namespace AccountingSystem
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var host = Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) =>
                {
                    // DB
                    services.AddDbContext<AppDbContext>(options =>
                        options.UseSqlServer(
                            "Server=localhost\\SQLEXPRESS;Database=AccountingSystemDb;Trusted_Connection=True;TrustServerCertificate=True;"
                        ));

                    // VALIDATORS
                    services.AddScoped<CustomerValidator>();
                    services.AddScoped<ProductValidator>();
                    services.AddScoped<QuotationValidator>();

                    // SERVICES
                    services.AddScoped<CustomerService>();
                    services.AddScoped<ProductService>();
                    services.AddScoped<QuotationService>();

                    // UI
                    services.AddScoped<MenuConsole>();
                    services.AddScoped<CustomerUI>();
                    services.AddScoped<ProductUI>();
                    services.AddScoped<QuotationUI>();
                })
                .Build();

            using var scope = host.Services.CreateScope();

            var menu = scope.ServiceProvider.GetRequiredService<MenuConsole>();

            menu.MainMenu();
        }
    }
}