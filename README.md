# Sales & Accounting System (.NET 10)

## Overview

A console-based business application that simulates a simplified ERP sales and accounting workflow.

The project models real-world processes used in sales and accounting systems, including customer management, product catalog, quotations, orders, invoices, and payment tracking.

The main goal of the project was to practice building a business application using C#, .NET 10, Entity Framework Core, SQL Server, and layered architecture.

The current version uses a CLI interface. REST API and web interface are planned as future development steps.

---

## Business Workflow

The application simulates the following sales process:


Customer
↓
Quotation
↓
Order
↓
Invoice
↓
Payment


The system supports creating and managing business documents through the entire sales lifecycle.

---

## Features

## Customer Management

- Create customers
- Update customer information
- Delete customers
- View customer list

## Product Management

- Create products
- Update product information
- Delete products
- View product catalog

## Quotation Management

- Create quotations
- Manage quotation items
- Calculate quotation totals
- Track quotation status

## Order Management

- Create orders
- Manage order items
- Calculate order totals
- Track order status
- Convert quotations into orders

## Invoice Management

- Create invoices
- Calculate invoice totals
- Track invoice status
- Generate invoices from orders

## Payment Management

- Register payments
- Link payments to invoices
- Track payment status
- Monitor invoice settlement

---

# Technologies

- C#
- .NET 10
- Entity Framework Core
- SQL Server
- LINQ
- Dependency Injection
- Repository Pattern
- Unit of Work Pattern
- DTO Mapping
- Fluent API
- Validation
- Layered Architecture

---

# Architecture

The project follows a layered architecture approach.

Application flow:


UI
↓
Application Services
↓
Repositories
↓
Entity Framework Core
↓
SQL Server


## Domain

Responsible for core business models and rules.

Contains:

- Entities
- Enums
- Business-related logic

Examples:

- Customer
- Product
- Order
- Invoice
- Payment

---

## Application

Responsible for application logic and business workflows.

Contains:

- Services
- DTOs
- Interfaces
- Mappers
- Validation logic

This layer does not depend directly on database implementation.

---

## Infrastructure

Responsible for external dependencies.

Contains:

- Entity Framework Core configuration
- DbContext
- Repository implementations
- Database migrations

---

## UI

Console-based interface responsible for user interaction.

The UI communicates only with the application layer.

---

# Project Structure


SalesAccountingSystem

│
├── AccountingSystem.Domain
│
├── AccountingSystem.Application
│
├── AccountingSystem.Infrastructure
│
├── AccountingSystem.UI
│
└── AccountingSystem.Tests


---

# Database

The application uses SQL Server with Entity Framework Core.

Database changes are managed using EF Core migrations.

Implemented:

- Entity relationships
- Database constraints
- Fluent API configuration
- Schema evolution through migrations


Main entities:

- Customer
- Product
- Quotation
- Order
- Invoice
- Payment

---

# Design Decisions

Some important design choices:

- Business logic is separated from UI and database layers.
- DTOs are used to avoid exposing domain entities directly.
- Repository pattern isolates database access.
- Services handle business workflows.
- Dependency Injection is used for better maintainability and testing.
- EF Core migrations are used to manage database changes.

---

# Getting Started

## Prerequisites

Required:

- .NET 10 SDK
- SQL Server or SQL Server LocalDB

---

## Clone Repository

```bash
git clone https://github.com/grzesiek1201/SalesAccountingSystem.git

cd SalesAccountingSystem
Apply Database Migrations
dotnet ef database update
Run Application
dotnet run
Example Usage

Typical workflow:

Create customer
Add products
Create quotation
Convert quotation into order
Generate invoice
Register payment
Learning Goals

This project was created to practice:

Object-Oriented Programming
SOLID principles
Clean code practices
Layered architecture
Entity Framework Core
SQL Server integration
Database migrations
Repository pattern
Dependency Injection
Business process modeling
Planned Improvements

Current development roadmap:

Unit Tests (xUnit)
Logging
User authentication and authorization
Roles and permissions
REST API with ASP.NET Core
Reporting module
ASP.NET Core frontend
Future Direction

The long-term goal is to transform the application into a small business management system with:

Web API
User management
Role-based access
Better reporting
Web interface