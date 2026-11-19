# dotnet-commerce-platform

Applicazione CRUD didattica per prepararsi a colloqui .NET: Customers, Products e Orders con ASP.NET Core .NET 10, EF Core e SQL Server.

## Development plan

- [x] 1. Scaffold solution and projects
- [x] 2. Domain entities, EF configurations, initial migration and seed
- [x] 3. Customer and Product services + APIs
- [x] 4. Order service, stock transaction and status rules + API
- [x] 5. Global error handling and OpenAPI
- [x] 6. MVC Customer views
- [x] 7. xUnit tests with SQLite in-memory
- [x] 8. Docker and docker-compose
- [x] 9. Documentation and verification

## Stack

- .NET 10 / ASP.NET Core MVC and Web API
- Entity Framework Core 10 / SQL Server 2022
- Docker Compose
- xUnit 2.9
- SQLite in-memory for tests
- Bootstrap 5.3

## Project structure

- `src/Commerce.Api/Domain` — entities and enums
- `src/Commerce.Api/Data` — DbContext, configurations, migration and seed data
- `src/Commerce.Api/Services` — service interfaces and implementations
- `src/Commerce.Api/Dtos` — immutable API request/response records
- `src/Commerce.Api/Controllers/Api` — REST controllers
- `src/Commerce.Api/Controllers/Mvc` and `Views` — frontend MVC Razor (`.cshtml`)
- `src/Commerce.Api/Middleware` — global ProblemDetails exception handling
- `src/Commerce.Blazor` — frontend Blazor alternativo che consuma la API
- `src/Commerce.React` — frontend React alternativo (Vite + TypeScript) che consuma la API
- `tests/Commerce.Api.Tests` — xUnit and SQLite tests

## Frontend alternatives

Il repository include tre opzioni UI alternative:

- MVC Razor (`.cshtml`) integrato nell'API: `http://localhost:8080/Customers`
- Blazor Server: `http://localhost:8081`
- React (Vite) servito con nginx: `http://localhost:8082`

## Run with Docker

```bash
cp .env.example .env
# Replace MSSQL_SA_PASSWORD with a strong password if needed
docker compose up --build
```

API: `http://localhost:8080`
MVC: `http://localhost:8080/Customers`
OpenAPI document: `http://localhost:8080/openapi/v1.json`
Blazor frontend: `http://localhost:8081`
React frontend: `http://localhost:8082`

## Run locally

Start SQL Server using Docker and run the API:

```bash
docker compose up db -d
dotnet run --project src/Commerce.Api
```

Per usare anche Blazor in locale:

```bash
dotnet run --project src/Commerce.Blazor
```

Per usare React in locale serve Node.js installato:

```bash
cd src/Commerce.React
npm install
npm run dev
```

In alternativa, puoi avviare tutti i frontend via Docker Compose.

Update `src/Commerce.Api/appsettings.json` or set `ConnectionStrings__Default` for the local database.

## Tests

```bash
dotnet build Commerce.sln
dotnet test Commerce.sln
```

## EF Core migration

```bash
~/.dotnet/tools/dotnet-ef migrations add MigrationName \
  --project src/Commerce.Api/Commerce.Api.csproj \
  --startup-project src/Commerce.Api/Commerce.Api.csproj \
  --output-dir Data/Migrations
```

## API endpoints

| Method | Route | Description |
|---|---|---|
| GET | `/api/customers` | List customers |
| GET | `/api/customers/{id}` | Get a customer |
| POST | `/api/customers` | Create a customer |
| PUT | `/api/customers/{id}` | Update a customer |
| DELETE | `/api/customers/{id}` | Delete a customer |
| GET | `/api/products` | List products |
| GET | `/api/products/{id}` | Get a product |
| POST | `/api/products` | Create a product |
| PUT | `/api/products/{id}` | Update a product |
| DELETE | `/api/products/{id}` | Delete a product |
| POST | `/api/orders` | Create an order and decrement stock |
| GET | `/api/orders/{id}` | Get an order with items |
| GET | `/api/orders?page=1&pageSize=20&customerId=&status=` | Paged and filtered orders |
| PATCH | `/api/orders/{id}/status` | Change order status |
| DELETE | `/api/orders/{id}` | Delete pending order only |
