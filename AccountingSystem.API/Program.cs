using AccountingSystem.Application.Converters;
using AccountingSystem.Application.Helpers;
using AccountingSystem.Application.Interfaces;
using AccountingSystem.Application.Mappers;
using AccountingSystem.Application.Repositories;
using AccountingSystem.Application.Services;
using AccountingSystem.Application.Validation.Customers;
using AccountingSystem.Application.Validation.Invoices;
using AccountingSystem.Application.Validation.Orders;
using AccountingSystem.Application.Validation.Payments;
using AccountingSystem.Application.Validation.ProductCategories;
using AccountingSystem.Application.Validation.Products;
using AccountingSystem.Application.Validation.Quotations;
using AccountingSystem.Application.Factories;
using AccountingSystem.Infrastructure.Data;
using AccountingSystem.Infrastructure.Repositories;
using AccountingSystem.Infrastructure.UnitOfWork;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// =====================
// DB
// =====================
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    ));

// =====================
// Controllers + Swagger
// =====================
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(
            new System.Text.Json.Serialization.JsonStringEnumConverter());
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// =====================
// CORS - allow frontend dev server (Vite)
// =====================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// =====================
// Application services
// =====================
// FACTORIES
builder.Services.AddScoped<QuotationFactory>();
builder.Services.AddScoped<OrderFactory>();
builder.Services.AddScoped<InvoiceFactory>();

// CONVERTERS
builder.Services.AddScoped<QuotationToOrderConverter>();
builder.Services.AddScoped<OrderToInvoiceConverter>();

// RESPONSE MAPPERS
builder.Services.AddScoped<QuotationResponseMapper>();
builder.Services.AddScoped<OrderResponseMapper>();
builder.Services.AddScoped<InvoiceResponseMapper>();

// VALIDATORS
builder.Services.AddScoped<CustomerValidator>();
builder.Services.AddScoped<ProductValidator>();
builder.Services.AddScoped<QuotationValidator>();
builder.Services.AddScoped<OrderValidator>();
builder.Services.AddScoped<InvoiceValidator>();
builder.Services.AddScoped<PaymentValidator>();
builder.Services.AddScoped<IInvoiceStatusCalculator, InvoiceStatusCalculator>();
builder.Services.AddScoped<ProductCategoryValidator>();

// SERVICES
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IQuotationService, QuotationService>();
builder.Services.AddScoped<IInvoiceService, InvoiceService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<INumberSequenceService, NumberSequenceService>();
builder.Services.AddScoped<IProductCategoryService, ProductCategoryService>();



// REPOSITORIES + UOW
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IQuotationRepository, QuotationRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<INumberSequenceRepository, NumberSequenceRepository>();
builder.Services.AddScoped<IProductCategoryRepository, ProductCategoryRepository>();

var app = builder.Build();

app.UseCors("AllowFrontend");

// =====================
// Middleware
// =====================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();