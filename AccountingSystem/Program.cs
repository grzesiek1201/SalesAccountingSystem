using AccountingSystem.AccountingSystem.Application.Services;
using AccountingSystem.AccountingSystem.Application.Validation.Customers;
using AccountingSystem.AccountingSystem.Application.Validation.Invoices;
using AccountingSystem.AccountingSystem.Application.Validation.Orders;
using AccountingSystem.AccountingSystem.Application.Validation.Products;
using AccountingSystem.AccountingSystem.Application.Validation.Quotations;
using AccountingSystem.AccountingSystem.Infrastructure.Data;
using AccountingSystem.AccountingSystem.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
                    services.AddScoped<OrderValidator>();
                    services.AddScoped<InvoiceValidator>();

                    // SERVICES
                    services.AddScoped<CustomerService>();
                    services.AddScoped<ProductService>();
                    services.AddScoped<QuotationService>();
                    services.AddScoped<OrderService>();
                    services.AddScoped<InvoiceService>();

                    // UI
                    services.AddScoped<MenuConsole>();
                    services.AddScoped<CustomerUI>();
                    services.AddScoped<ProductUI>();
                    services.AddScoped<QuotationUI>();
                    services.AddScoped<OrderUI>();
                    services.AddScoped<InvoiceUI>();

                    services.AddScoped<IUnitOfWork, UnitOfWork>();
                })
                .Build();

            using var scope = host.Services.CreateScope();

            var menu = scope.ServiceProvider.GetRequiredService<MenuConsole>();

            menu.MainMenu();
        }
    }
}