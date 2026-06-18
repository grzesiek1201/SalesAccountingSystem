# Changelog

All notable changes to this project are documented here.

---

## [0.3.0] - 2026-06-18

### Added
- Implemented Order → Invoice conversion workflow
- Added mapper responsible for transforming orders into invoices

### Changed
- Refactored services to improve code structure and maintainability

### Fixed
- Fixed database migration issues
- Fixed missing database tables required by application workflow

---

## [0.2.0] - 2026-06-15

### Added
- Implemented Quotation → Order conversion workflow
- Added document number sequence generation for quotations, orders and invoices
- Added mapper for quotation to order transformation
- Extended tests for quotation and order workflow

### Changed
- Improved quotation validation rules
- Refactored document handling workflow

---

## [0.1.0] - 2026-05-27

### Added
- Implemented payment module
- Integrated payments with invoice settlement workflow

---

## [0.0.9] - 2026-05-27

### Added
- Implemented Order and Invoice entities
- Added validation logic
- Added service layer logic
- Added console UI support for orders and invoices

---

## [0.0.8] - 2026-05-24

### Added
- Added QuotationValidator

### Fixed
- Fixed issues in quotation module workflow

---

## [0.0.7] - 2026-05-22

### Fixed
- Fixed issues in customer, product and quotation modules

---

## [0.0.6] - 2026-05-21

### Added
- Added quotation validation rules

### Fixed
- Fixed quotation UI flow issues

---

## [0.0.5] - 2026-05-20

### Added
- Implemented quotation model
- Added quotation management UI

### Changed
- Improved product validation
- Integrated quotation workflow with application structure

---

## [0.0.4] - 2026-05-19

### Changed
- Improved product module validation
- Improved product service logic
- Improved product management flow

---

## [0.0.3] - 2026-05-16

### Added
- Implemented Product module
- Added Product entity and category model
- Added ProductService and validation logic
- Added Product UI

### Changed
- Improved product service structure and validation flow

---

## [0.0.2] - 2026-05-11

### Added
- Added customer validation messages

### Changed
- Refactored CustomerService
- Added validation system with DTO responses

---

## [0.0.1] - 2026-04-16

### Added
- Initialized repository structure
- Created layered architecture:
  - Domain
  - Application
  - Infrastructure
  - UI

- Added Customer entity
- Added CustomerService
- Added basic CLI menu
- Added customer management features:
  - Create customers
  - Edit customer data
  - Search customers

- Prepared project structure for future database integration

---