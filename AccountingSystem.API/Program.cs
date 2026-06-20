using AccountingSystem.Application.Interfaces;
using AccountingSystem.Application.Mappers;
using AccountingSystem.Application.Repositories;
using AccountingSystem.Application.Services;
using AccountingSystem.Application.Validation.Customers;
using AccountingSystem.Application.Validation.Invoices;
using AccountingSystem.Application.Validation.Orders;
using AccountingSystem.Application.Validation.Payments;
using AccountingSystem.Application.Validation.Products;
using AccountingSystem.Application.Validation.Quotations;
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
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// =====================
// Application services
// =====================
builder.Services.AddScoped<NumberSequenceService>();
builder.Services.AddScoped<QuotationToOrderMapper>();
builder.Services.AddScoped<OrderToInvoiceMapper>();
builder.Services.AddScoped<DocumentConversionService>();

// VALIDATORS
builder.Services.AddScoped<CustomerValidator>();
builder.Services.AddScoped<ProductValidator>();
builder.Services.AddScoped<QuotationValidator>();
builder.Services.AddScoped<OrderValidator>();
builder.Services.AddScoped<InvoiceValidator>();
builder.Services.AddScoped<PaymentValidator>();

// SERVICES
builder.Services.AddScoped<CustomerService>();
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<OrderService>();
builder.Services.AddScoped<QuotationService>();
builder.Services.AddScoped<InvoiceService>();
builder.Services.AddScoped<PaymentService>();

// REPOSITORIES + UOW
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IQuotationRepository, QuotationRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<INumberSequenceRepository, NumberSequenceRepository>();

var app = builder.Build();

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