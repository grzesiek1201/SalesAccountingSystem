# AccountingSystem (CLI)

## Opis
Konsolowy system do symulacji podstawowych procesów:
- zarządzanie klientami
- przygotowanie struktury pod faktury, zamówienia i oferty
- przygotowanie pod integrację z bazą danych (PostgreSQL)

Projekt zbudowany w architekturze warstwowej:
- Domain
- Application
- Infrastructure
- UI (CLI)

---

## Struktura projektu

- **Domain** – encje biznesowe 
- **Application** – logika aplikacji 
- **Infrastructure** – dostęp do danych
- **UI** – interfejs konsolowy

---

## Uruchomienie

Wymagania:
- .NET 8 SDK (lub wersja zgodna z projektem)

Uruchomienie:
```bash
dotnet run