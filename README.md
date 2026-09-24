# Sales & Accounting System (.NET 10)

A simplified **ERP-style sales and accounting system** built with **C# and .NET 10**.

The project models a business workflow:

**Customer → Quotation → Order → Invoice → Payment**

The main goal is to practice building a business-oriented application with domain logic, database persistence, REST API, layered architecture and automated tests.

---

## Features

* Customer and product management
* Quotation → Order → Invoice workflow
* Payment management
* Business rules and document status transitions
* Document data snapshots
* REST API
* SQL Server database
* React/TypeScript web interface
* Automated tests

---

## Architecture

```text
React / TypeScript
        ↓
ASP.NET Core Web API
        ↓
Application
        ↓
Domain
        ↓
Infrastructure / EF Core
        ↓
SQL Server
```

The backend is divided into:

```text
AccountingSystem.Domain
AccountingSystem.Application
AccountingSystem.Infrastructure
AccountingSystem.API
AccountingSystem.Tests
AccountingSystem.WebUI
```

Business operations are handled through explicit domain methods rather than arbitrary status changes.

Examples:

```text
Quotation.Accept()
Quotation.ConvertToOrder()

Order.Complete()
Order.ConvertToInvoice()

Invoice.Issue()

Payment.Complete()
```

---

## Technologies

### Backend

* C#
* .NET 10
* ASP.NET Core Web API
* Entity Framework Core
* SQL Server
* LINQ
* Dependency Injection
* REST API

### Architecture

* Layered Architecture
* Domain-driven business logic
* Repository Pattern
* Unit of Work
* DTOs
* Factory Pattern
* Validation
* Transactions

### Frontend

* React
* TypeScript
* Vite
* Axios
* React Router

### Testing

* xUnit
* Moq

### Tools

* Git
* GitHub
* Visual Studio
* VS Code

---

## Business Rules

The application implements rules such as:

* A quotation must be sent before it can be accepted.
* Only accepted quotations can be converted into orders.
* Completed orders can be converted into invoices.
* Payments cannot exceed the remaining invoice balance.
* Important document operations are handled through business methods.

---

## Document Snapshots

Sales documents store important customer and product information as snapshots.

This allows historical documents to preserve the data that was valid when they were created, even if the original customer or product data changes later.

---

## Testing

The project contains automated tests using **xUnit** and **Moq**, covering selected business logic, validation and application scenarios.

---

## Project Goals

The project was created to practice:

* C# and .NET
* Object-Oriented Programming
* ASP.NET Core Web API
* Entity Framework Core
* SQL Server
* REST API
* Client-server architecture
* Domain and business logic
* Layered architecture
* Automated testing
* Frontend/backend integration

The project is intentionally kept small enough to understand end-to-end while modelling processes commonly found in business and ERP software.

---

## Repository

[GitHub – SalesAccountingSystem](https://github.com/grzesiek1201/SalesAccountingSystem)
