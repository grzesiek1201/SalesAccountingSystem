# Sales Accounting System (CLI)

## Overview

A console-based system for managing sales processes and accounting-ready documents such as quotations, orders, invoices, and payments.

The project simulates core business processes used in small and medium-sized companies, including customer management, product management, quotations, orders, invoices, and payment tracking.

The application follows a layered architecture and uses Entity Framework Core with SQL Server for data persistence.

---

## Features

### Customer Management

* Create customers
* Update customer information
* Delete customers
* View customer list

### Product Management

* Create products
* Update product information
* Delete products
* View product catalog

### Quotation Management

* Create quotations
* Manage quotation items
* Calculate quotation totals
* Track quotation status

### Order Management

* Create orders
* Manage order items
* Calculate order totals
* Track order status

### Invoice Management

* Create invoices
* Calculate invoice totals
* Track invoice status

### Payment Management

* Register payments
* Track payment status
* Link payments to invoices
* Monitor invoice settlement

---

## Technologies

* C#
* .NET 8
* Entity Framework Core
* SQL Server
* Dependency Injection
* Layered Architecture

---

## Architecture

The project is organized into four main layers:

### Domain

Contains business entities, enums, and business rules.

### Application

Contains application services, DTOs, validation logic, and business workflows.

### Infrastructure

Handles database access, Entity Framework Core configuration, repositories, and data persistence.

### UI

Console-based user interface responsible for user interaction.

---

## Project Structure

```text
AccountingSystem
│
├── Domain
├── Application
├── Infrastructure
└── UI
```

---

## Database

The application uses SQL Server together with Entity Framework Core.

Main entities:

* Customer
* Product
* Quotation
* Order
* Invoice
* Payment

Relationships between entities are configured using Entity Framework Core Fluent API.

---

## Getting Started

### Prerequisites

* .NET 8 SDK
* SQL Server or SQL Server LocalDB

### Clone Repository

```bash
git clone https://github.com/grzesiek1201/AccountingSystem.git
cd AccountingSystem
```

### Apply Migrations

```bash
dotnet ef database update
```

### Run Application

```bash
dotnet run
```

---

## Learning Goals

This project was created to practice:

* Object-Oriented Programming (OOP)
* SOLID Principles
* Layered Architecture
* Entity Framework Core
* SQL Server Integration
* Data Validation
* Dependency Injection
* Business Process Modeling

---

## Planned Improvements

* Unit Tests
* Logging
* Quotation → Order → Invoice workflow
* Reporting Module
* REST API
* ASP.NET Core Frontend
