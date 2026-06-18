using AccountingSystem.Application.Interfaces;
using AccountingSystem.Application.Repositories;
using AccountingSystem.Application.Services;
using AccountingSystem.Application.Validation.Customers;
using AccountingSystem.Application.Validation.Invoices;
using AccountingSystem.Application.Validation.Orders;
using AccountingSystem.Application.Validation.Products;
using AccountingSystem.Application.Validation.Quotations;
using AccountingSystem.Application.Validation.Payments;
using AccountingSystem.Infrastructure.Data;
using AccountingSystem.Infrastructure.Repositories;
using AccountingSystem.Infrastructure.UnitOfWork;
using AccountingSystem.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using AccountingSystem.Application.Mappers;


namespace AccountingSystem
{
    public class Program
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

                    services.AddScoped<NumberSequenceService>();
                    services.AddScoped<QuotationToOrderMapper>();
                    services.AddScoped<OrderToInvoiceMapper>();
                    services.AddScoped<DocumentConversionService>();

                    // VALIDATORS
                    services.AddScoped<CustomerValidator>();
                    services.AddScoped<ProductValidator>();
                    services.AddScoped<QuotationValidator>();
                    services.AddScoped<OrderValidator>();
                    services.AddScoped<InvoiceValidator>();
                    services.AddScoped<PaymentValidator>();

                    // SERVICES
                    services.AddScoped<CustomerService>();
                    services.AddScoped<ProductService>();
                    services.AddScoped<OrderService>();
                    services.AddScoped<QuotationService>();
                    services.AddScoped<InvoiceService>();
                    services.AddScoped<PaymentService>();

                    // UI
                    services.AddScoped<MenuConsole>();
                    services.AddScoped<CustomerUI>();
                    services.AddScoped<ProductUI>();
                    services.AddScoped<QuotationUI>();
                    services.AddScoped<OrderUI>();
                    services.AddScoped<InvoiceUI>();
                    services.AddScoped<PaymentUI>();

                    services.AddScoped<IUnitOfWork, UnitOfWork>();
                    services.AddScoped<ICustomerRepository, CustomerRepository>();
                    services.AddScoped<IProductRepository, ProductRepository>();
                    services.AddScoped<IQuotationRepository, QuotationRepository>();
                    services.AddScoped<IOrderRepository, OrderRepository>();
                    services.AddScoped<IInvoiceRepository, InvoiceRepository>();
                    services.AddScoped<IPaymentRepository, PaymentRepository>();
                    services.AddScoped<INumberSequenceRepository, NumberSequenceRepository>();

                })
                .Build();

            using var scope = host.Services.CreateScope();

            var menu = scope.ServiceProvider.GetRequiredService<MenuConsole>();

            menu.MainMenu();
        }
    }
}
