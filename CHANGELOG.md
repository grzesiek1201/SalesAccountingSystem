# Changelog

All notable changes to this project are documented here.

## [1.0.0] - 2026-09-15

### Added

* Added React + TypeScript web frontend based on Vite
* Added customer management in the web interface
* Added product management in the web interface
* Added quotation management in the web interface
* Added order management in the web interface
* Added invoice management in the web interface
* Added payment management in the web interface
* Added document status operations through the REST API
* Added quotation → order conversion
* Added order → invoice conversion
* Added payment creation and deletion
* Added invoice payment validation:

  * positive payment amount
  * payment amount cannot exceed remaining balance
  * payments cannot be created for archived invoices
* Added invoice due date and issue date handling
* Added invoice payment status calculation
* Added API logging for document and payment operations
* Added frontend API integration using Axios
* Added client-side routing using React Router
* Added frontend linting using Oxlint

### Changed

* Completed migration from the previous console-based UI towards a web-based React frontend
* Expanded the ASP.NET Core Web API to support the complete document workflow
* Improved quotation, order and invoice lifecycle handling
* Introduced explicit domain operations for document status changes
* Separated document statuses from payment statuses
* Improved document conversion workflow:

  * Quotation → Order
  * Order → Invoice
* Improved document snapshot handling to preserve historical product and customer data
* Refactored invoice creation from orders to use document converters and domain rules
* Improved payment handling and invoice settlement workflow
* Updated application configuration and project structure for the full-stack application
* Updated project documentation and architecture description

### Fixed

* Fixed document status transition inconsistencies
* Fixed validation issues during quotation and order conversion
* Fixed invoice creation issues caused by missing required dates
* Fixed payment validation and remaining-balance calculation issues
* Fixed API routing issues
* Fixed outdated frontend API paths
* Fixed inconsistencies between domain entities, validators, services and controllers
* Updated tests after domain and application architecture changes

### Architecture

* Maintained layered architecture:

  * Domain
  * Application
  * Infrastructure
  * API
  * WebUI
* Continued use of:

  * Entity Framework Core
  * SQL Server
  * Repository pattern
  * Unit of Work
  * DTOs
  * Response mappers
  * Factories
  * Document converters
  * Domain business rules
  * xUnit and Moq

### Current Workflow

The application currently supports the following business workflow:

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

Documents use explicit lifecycle operations and preserve historical product and customer information through snapshots.

---

## [0.6.0] - 2026-07-16

### Added

* Added database seeder project
* Added new initial EF Core migration after domain model refactoring
* Expanded domain tests for entity business rules
* Added additional test coverage for converters and services

### Changed

* Refactored domain entities to handle their own business rules
* Improved separation between document statuses and payment statuses
* Updated Invoice, Order and Quotation lifecycle handling
* Moved validation of domain operations from services into entities
* Reworked EF Core model configuration after domain changes
* Updated service tests to match new domain behavior

### Fixed

* Fixed inconsistencies between entities, validators and services
* Fixed outdated tests after domain responsibility changes
* Fixed document conversion tests
* Fixed validation scenarios after introducing required business fields

---

## [0.5.0] - 2026-07-12

### Added

* Added customer NIP field and validation
* Expanded unit test coverage for services and domain utilities
* Added additional validation scenarios

### Changed

* Updated service tests after architectural refactoring
* Improved dependency injection registrations
* Improved separation between domain rules and application services

### Fixed

* Fixed issues caused by changes in domain rules
* Fixed inconsistencies between services, validators and entities

---

## [0.4.0] - 2026-07-08

### Added

* Added document factory pattern for entity creation
* Added reusable document snapshot logic

### Changed

* Replaced document mappers with conversion services
* Simplified document service responsibilities
* Unified document conversion workflow
* Improved quotation → order → invoice lifecycle handling
* Reduced coupling between document-related services

### Fixed

* Fixed architectural issues caused by duplicated conversion logic
* Improved maintainability of document processing workflow

---

## [0.3.1] - 2026-07-03

### Added

* Added ProductCategory module
* Added product-category relationship
* Added additional service and validator tests

### Changed

* Refactored application services and DTO contracts
* Improved dependency injection configuration
* Improved validation flow
* Updated EF Core model configuration and migrations
* Improved payment status model

### Fixed

* Fixed issues after migration towards API architecture
* Fixed outdated interfaces and project references

---

## [0.3.0] - 2026-06-21

### Added

* Added ASP.NET Core Web API project
* Added Swagger configuration
* Added API endpoints for:

  * Customers
  * Orders
  * Invoices
* Added DTO-based communication
* Added response mappers
* Added document snapshot support

### Changed

* Refactored customer module for Web API architecture
* Moved DTOs from API layer into Application layer
* Improved document mapping:

  * Quotation → Order
  * Order → Invoice
* Updated product handling and database migrations

### Fixed

* Fixed issues related to API integration
* Fixed document mapping inconsistencies

---

## [0.2.0] - 2026-06-15

### Added

* Added quotation to order conversion workflow
* Added document number sequence generation
* Added quotation to order mapper
* Added logging support for application services
* Added payment module and invoice settlement workflow

### Changed

* Improved quotation validation rules
* Refactored document workflow handling
* Improved service validation flow
* Updated tests for quotation and service changes

### Fixed

* Fixed quotation conversion issues
* Fixed validation errors in quotation workflow

---

## [0.1.0] - 2026-05-27

### Added

* Implemented invoice module:

  * Invoice entity
  * Invoice service
  * Invoice validation
  * Invoice UI
* Implemented order module:

  * Order entity
  * Order service
  * Order validation
  * Order UI
* Added Unit of Work pattern
* Added SQL Server integration
* Added Entity Framework Core database support

### Changed

* Refactored product, customer and quotation modules for database integration
* Improved project architecture
* Improved dependency injection setup

### Fixed

* Fixed database persistence issues
* Fixed service layer issues after EF Core integration

---

## [0.0.9] - 2026-05-27

### Added

* Added Order and Invoice domain models
* Added order and invoice services
* Added order and invoice validation logic
* Added console UI support for orders and invoices

---

## [0.0.8] - 2026-05-24

### Added

* Added QuotationValidator

### Fixed

* Fixed issues in quotation workflow
* Fixed quotation module bugs

---

## [0.0.7] - 2026-05-22

### Fixed

* Fixed issues in customer, product and quotation modules
* Improved validation handling

---

## [0.0.6] - 2026-05-21

### Added

* Added quotation validation rules

### Fixed

* Fixed quotation UI workflow issues

---

## [0.0.5] - 2026-05-20

### Added

* Implemented quotation model
* Added quotation management UI

### Changed

* Improved product validation
* Integrated quotation workflow with application structure

---

## [0.0.4] - 2026-05-19

### Changed

* Improved product module validation
* Improved ProductService logic
* Improved product management flow

---

## [0.0.3] - 2026-05-16

### Added

* Implemented Product module
* Added Product entity and Category model
* Added ProductService
* Added product validation logic
* Added Product UI

### Changed

* Improved product service structure

---

## [0.0.2] - 2026-05-11

### Added

* Added customer validation messages

### Changed

* Refactored CustomerService
* Added validation system with DTO responses

---

## [0.0.1] - 2026-04-16

### Added

* Initialized repository structure
* Created layered architecture:

  * Domain
  * Application
  * Infrastructure
  * UI
* Added Customer entity
* Added CustomerService
* Added basic CLI menu
* Added customer management features:

  * Create customers
  * Edit customer data
  * Search customers
* Prepared project structure for future database integration

---

## Future Development

The `1.0.0` release represents the first complete version of the core application.

Possible future improvements may include:

* Authentication and authorization
* Role-based access control
* Serilog structured logging
* Docker support
* CI/CD pipeline
* Reporting and analytics
* More advanced invoice and payment workflows
* Additional automated tests
* Further frontend UX improvements
* Additional ERP-oriented modules
