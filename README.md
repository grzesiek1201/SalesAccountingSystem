# Sales & Accounting System (.NET 10)

A simplified sales and accounting system built with C# and .NET 10.

The project simulates a business workflow covering customers, products, quotations, orders, invoices, and payments. It is designed as a small ERP-style application with a layered architecture, REST API, SQL Server database, automated tests, and a React/TypeScript web interface.

The main goal of the project is to model business processes and demonstrate practical use of .NET, Entity Framework Core, SQL Server, domain logic, application services, and API development.

---

## Business Workflow

The application follows a simplified sales lifecycle:

```text
Customer
   ↓
Quotation
   ↓
Order
   ↓
Invoice
   ↓
Payment
```

Documents are connected through explicit business operations and status transitions.

For example:

```text
Quotation
Draft → Sent → Accepted
                 ↓
               Order
                 ↓
          Confirmed → Completed
                         ↓
                      Invoice
                         ↓
                    Payments
```

---

## Current Features

### Customer Management

* Create customers
* Update customer data
* Delete customers
* Browse customers
* Store customer information as document snapshots

### Product Management

* Create products
* Update product data
* Delete products
* Manage product catalog
* Store product information as document snapshots

### Quotation Management

* Create quotations
* Add and edit quotation items
* Calculate document totals
* Manage quotation lifecycle
* Send quotations
* Accept or reject quotations
* Convert accepted quotations into orders

Supported quotation statuses:

```text
Draft
Sent
Accepted
Rejected
Expired
Converted
```

### Order Management

* Create orders
* Add and edit order items
* Calculate document totals
* Manage order lifecycle
* Confirm orders
* Complete orders
* Cancel orders
* Convert completed orders into invoices

Supported order statuses:

```text
Draft
Confirmed
InProgress
Completed
Invoiced
Cancelled
```

### Invoice Management

* Create invoices
* Generate invoices from orders
* Add and edit invoice items
* Calculate invoice totals
* Issue invoices
* Cancel invoices
* Archive invoices
* Track invoice due dates

Supported invoice statuses:

```text
Draft
Issued
Overdue
Cancelled
```

### Payment Management

* Register payments against invoices
* Validate payment amounts
* Prevent payments exceeding the remaining invoice balance
* Track payment status
* Calculate total payments for an invoice
* Delete payments
* Recalculate invoice state based on payment and due-date information

Supported payment statuses:

```text
Pending
Paid
Cancelled
```

---

# REST API

The application exposes a REST API built with ASP.NET Core.

Main API areas:

```text
/api/customers
/api/products
/api/quotations
/api/orders
/api/invoices
/api/payments
```

Examples of business operations:

```text
POST /api/quotations/{id}/send
POST /api/quotations/{id}/accept
POST /api/quotations/{id}/reject

POST /api/orders/{id}/confirm
POST /api/orders/{id}/complete
POST /api/orders/{id}/cancel

POST /api/invoices/{id}/issue
POST /api/invoices/{id}/cancel

POST /api/orders/from-quotation/{quotationId}
POST /api/invoices/from-order/{orderId}

POST /api/payments
GET  /api/payments/invoice/{invoiceId}
DELETE /api/payments/{paymentId}
```

Business operations are exposed as explicit API endpoints instead of allowing arbitrary status changes.

---

# Web Interface

The project includes a React + TypeScript web interface.

The frontend is built with:

* React
* TypeScript
* Vite
* Axios
* React Router
* Oxlint

The web interface communicates with the ASP.NET Core REST API.

Current areas include:

* Dashboard
* Customers
* Products
* Sales documents
* Invoices
* Payments
* Reusable tables, forms, modals and status components

Frontend project:

```text
AccountingSystem.WebUI
```

---

# Technologies

### Backend

* C#
* .NET 10
* ASP.NET Core Web API
* Entity Framework Core
* SQL Server / SQL Server Express
* LINQ
* Dependency Injection
* REST API
* Swagger / OpenAPI

### Frontend

* React
* TypeScript
* Vite
* Axios
* React Router
* Oxlint

### Architecture & Design

* Layered Architecture
* Domain-driven business operations
* Repository Pattern
* Unit of Work Pattern
* DTOs
* Mappers
* Converters
* Factory Pattern
* Validation
* Dependency Injection
* Snapshot-based document data

### Testing

* xUnit
* Moq

---

# Architecture

The backend follows a layered architecture:

```text
                    React / WebUI
                         │
                         ▼
                  ASP.NET Core API
                         │
                         ▼
                Application Services
                         │
             ┌───────────┴───────────┐
             ▼                       ▼
          Domain                Repositories
             │                       │
             └───────────┬───────────┘
                         ▼
                  Entity Framework
                         │
                         ▼
                     SQL Server
```

## Project Structure

```text
AccountingSystem
│
├── AccountingSystem.Domain
│
├── AccountingSystem.Application
│
├── AccountingSystem.Infrastructure
│
├── AccountingSystem.API
│
├── AccountingSystem.Tests
│
└── AccountingSystem.WebUI
```

---

# Layers

## Domain

Contains the core business entities, enums, and domain operations.

Main entities:

* Customer
* Product
* Quotation
* QuotationItem
* Order
* OrderItem
* Invoice
* InvoiceItem
* Payment

Business state transitions are implemented through domain methods such as:

```text
Quotation.Send()
Quotation.Accept()
Quotation.Reject()
Quotation.ConvertToOrder()

Order.Confirm()
Order.Complete()
Order.Cancel()
Order.ConvertToInvoice()

Invoice.Issue()
Invoice.Cancel()
Invoice.MarkAsOverdue()

Payment.Complete()
Payment.Cancel()
```

This prevents application code from freely assigning invalid business states.

---

## Application

Contains application workflows and business logic.

Includes:

* Application services
* DTOs
* Interfaces
* Validators
* Mappers
* Converters
* Factories
* Business helpers

Examples:

```text
QuotationService
OrderService
InvoiceService
PaymentService
```

Application services coordinate repositories, domain operations, validation, and transactions.

---

## Infrastructure

Responsible for external dependencies and data persistence.

Includes:

* Entity Framework Core
* DbContext
* Repository implementations
* Database configuration
* Fluent API configuration
* Database migrations

---

## API

Contains ASP.NET Core controllers responsible for exposing application functionality through HTTP endpoints.

Controllers delegate business operations to application services rather than implementing business rules directly.

---

## WebUI

Contains the React/TypeScript frontend.

The frontend is separated from the backend and communicates with it exclusively through the REST API.

---

# Document Snapshots

Documents store snapshots of important customer and product information.

For example, an invoice item stores:

```text
ProductName
ProductCode
VatRate
Unit
BaseUnitPrice
Quantity
DiscountPercent
Total
```

This means historical documents do not depend on the current state of the product catalog.

The same approach is used for customer information stored on sales documents.

This is important for preserving historical document data when products or customers are changed later.

---

# Database

The application uses SQL Server with Entity Framework Core.

The database contains entities corresponding to:

```text
Customers
Products
Quotations
QuotationItems
Orders
OrderItems
Invoices
InvoiceItems
Payments
```

Entity relationships and database constraints are configured using Entity Framework Core Fluent API.

Database schema changes are handled through EF Core migrations.

---

# Business Rules

Examples of implemented business rules include:

* A quotation must be sent before it can be accepted.
* A quotation can be converted into an order only through the appropriate business operation.
* An order follows its own lifecycle and cannot arbitrarily change status.
* An order can be converted into an invoice.
* An invoice must be issued before normal invoice processing.
* A payment amount must be greater than zero.
* A payment cannot exceed the remaining invoice balance.
* Archived invoices cannot receive new payments.
* Paid payments are included when calculating the invoice's remaining balance.
* Paid invoices cannot be cancelled through the normal cancellation operation.

Business rules are implemented in the domain and application layers rather than directly in controllers.

---

# Transactions

Operations that modify multiple related entities use the Unit of Work abstraction.

For example, converting an order into an invoice involves:

```text
Create invoice
      ↓
Add invoice
      ↓
Change order state
      ↓
Save changes
      ↓
Commit transaction
```

If the operation fails, the transaction can be rolled back.

---

# Getting Started

## Requirements

* .NET 10 SDK
* SQL Server or SQL Server Express
* Node.js / npm

## Clone repository

```bash
git clone https://github.com/grzesiek1201/AccountingSystem.git

cd AccountingSystem
```

## Database

Configure the SQL Server connection string in the application configuration.

Apply EF Core migrations:

```bash
dotnet ef database update
```

## Run API

```bash
dotnet run --project AccountingSystem.API
```

The API exposes Swagger/OpenAPI documentation when running in the appropriate environment.

## Run WebUI

Open another terminal:

```bash
cd AccountingSystem.WebUI
npm install
npm run dev
```

The Vite development server will start the React application.

---

# Tests

The project contains automated tests using:

* xUnit
* Moq

Tests cover application logic, validation, and business scenarios.

---

# Design Decisions

The project intentionally separates responsibilities between layers.

Key decisions include:

* Business rules are kept outside controllers.
* Domain entities expose explicit operations for state transitions.
* DTOs are used between API and application layers.
* Repositories abstract persistence.
* Unit of Work handles transaction boundaries.
* Converters are responsible for document-to-document transformations.
* Snapshot data preserves historical document information.
* Services coordinate business workflows.
* Dependency Injection is used throughout the application.
* Frontend and backend are separated through a REST API.

---

# Project Goals

The project was created to practice building a realistic business application rather than only isolated programming exercises.

Main areas of practice:

* Object-Oriented Programming
* C# and .NET
* ASP.NET Core Web API
* Entity Framework Core
* SQL Server
* REST API design
* Domain modeling
* Business process modeling
* Layered architecture
* Repository and Unit of Work patterns
* DTOs and mapping
* Validation
* Transactions
* Automated testing
* React and TypeScript
* Frontend/backend integration

---

# Future Improvements

Possible future development areas include:

* Authentication and authorization
* User roles and permissions
* Improved reporting
* More advanced payment lifecycle
* Automatic invoice/payment status processing
* More comprehensive test coverage
* Docker support
* CI/CD pipeline
* Additional ERP-style modules

---

# Project Direction

The long-term goal is to evolve the project into a small ERP-style business management system combining:

```text
React WebUI
      ↓
ASP.NET Core Web API
      ↓
Business/Application Layer
      ↓
Domain
      ↓
Entity Framework Core
      ↓
SQL Server
```

The project is intentionally kept small enough to understand end-to-end while still demonstrating concepts commonly used in business and ERP software.
