# Book Management Store

A full-stack book store management system built with **C# and .NET 8**: an ASP.NET Core Web API secured with JWT, and a Blazor WebAssembly front end.

Anyone can browse and search the catalogue. Administrators log in to add, edit and delete books, authors and categories.

## Features

- **Catalogue**: browse every book with its author, category, year, price and stock level; search by title, ISBN or author and filter by category.
- **Accounts**: register and log in. Passwords are hashed with BCrypt, and the API issues a JWT plus a refresh token that the client uses to renew expired sessions automatically.
- **Roles**: the first account registered becomes the store **Admin**; everyone after that is a regular **User**. Only admins can change the catalogue, which the API enforces with `[Authorize(Roles = "Admin")]`.
- **Catalogue management** (admin): create, edit and delete books, authors and categories, with validation on both client and server. An ISBN must be unique, and an author or category can't be deleted while it still has books.
- **Sample data**: the database is seeded with three categories, four authors and four books.
- **Swagger UI** with an "Authorize" button for trying the protected endpoints.

## Project structure

| Project | Purpose |
|---|---|
| `BaseLibrary` | Entities (`Book`, `Author`, `Category`, users and roles), DTOs and response types shared by server and client |
| `ServerLibrary` | Entity Framework Core `AppDbContext` with seed data, and repositories (generic CRUD repository interface, book search, user accounts and JWT generation) |
| `Server` | ASP.NET Core Web API: authentication, CORS, Swagger and controllers |
| `ClientLibrary` | Client-side services: API calls, token storage in local storage, and the `AuthenticationStateProvider` |
| `Client` | Blazor WebAssembly UI: catalogue, book editor, author and category management, login and register |

## Tech stack

C#, .NET 8, ASP.NET Core Web API, Entity Framework Core 8 (SQL Server), Blazor WebAssembly, JWT bearer authentication, BCrypt, Swagger/OpenAPI, Bootstrap.

## Getting started

### Prerequisites

- .NET 8 SDK
- SQL Server (the default connection string uses a local default instance with Windows authentication; LocalDB also works, see below)

### 1. Configure the database

The connection string is in `Server/appsettings.json`:

```json
"DefaultConnection": "Server=(local);Database=BookManagementStore;Trusted_Connection=True;TrustServerCertificate=True"
```

To use LocalDB instead, change `Server=(local)` to `Server=(localdb)\\MSSQLLocalDB`.

In development the API creates the database and sample data automatically on first run (`EnsureCreated`). If you later add Entity Framework migrations, switch that call in `Server/Program.cs` to `Database.Migrate()`.

### 2. Run the API

```bash
cd Server
dotnet run --launch-profile https
```

Swagger opens at https://localhost:7053/swagger.

### 3. Run the client

In a second terminal:

```bash
cd Client
dotnet run --launch-profile https
```

Open https://localhost:7145, click **Register**, and create the first account. It becomes the administrator.

## API endpoints

| Method | Route | Access |
|---|---|---|
| POST | `/api/authentication/register` | Public |
| POST | `/api/authentication/login` | Public |
| POST | `/api/authentication/refresh-token` | Public |
| GET | `/api/book`, `/api/book/{id}`, `/api/book/search?text=&authorId=&categoryId=` | Public |
| GET | `/api/author`, `/api/author/{id}`, `/api/category`, `/api/category/{id}` | Public |
| POST, PUT | `/api/book`, `/api/author`, `/api/category` | Admin |
| DELETE | `/api/book/{id}`, `/api/author/{id}`, `/api/category/{id}` | Admin |

`Server/Server.http` has ready-made requests for Visual Studio or the VS Code REST Client extension.

## Configuration and secrets

The JWT signing key is read from `JwtSection:Key`. A development-only key is set in `Server/appsettings.Development.json` so the project runs out of the box. For any real deployment, set your own key of at least 32 characters with user secrets or an environment variable, for example:

```bash
dotnet user-secrets set "JwtSection:Key" "<your long random secret>" --project Server
```

The API refuses to start if the key is missing or too short.

If you change the ports, update `AllowedOrigins` in `Server/appsettings.json` (CORS) and `ApiBaseUrl` in `Client/wwwroot/appsettings.json`.
