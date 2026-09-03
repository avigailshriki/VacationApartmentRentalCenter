# VacationApartmentRentalCenter

A full-stack vacation apartment rental platform. Property owners can list, edit and manage their apartments — including photos and a real availability calendar — while guests can browse, search, and book by contacting the owner directly, view listings on a map, and leave reviews.

## Features

- **Property listings** with search and filtering by city, price and guest capacity, plus pagination.
- **Property details page**: photo gallery, location on Google Maps, nearby attractions, and guest reviews.
- **Availability calendar**: owners block/unblock date ranges on a visual month calendar; every visitor sees the same calendar (read-only) on the property page, showing which dates are open and which are taken.
- **Owner tools**: add, edit and delete properties; upload and remove property photos (jpg/png/webp); manage availability.
- **Accounts & authentication**: email/password registration and login, or one-click **Google Sign-In**; sessions are secured with JWT.
- **Authorization**: every write action (edit/delete a property, manage its photos or availability, post a review) requires login, and the server always determines the owner from the signed-in token — never from client-supplied data.
- **Guest reviews** with star ratings.
- Clean custom UI for notifications and confirmations (no native browser `alert`/`confirm` popups).

## Tech stack

**Backend** — ASP.NET Core 8 Web API, Entity Framework Core (SQL Server), AutoMapper, JWT Bearer authentication, Google.Apis.Auth (Google Sign-In token verification).

**Frontend** — Angular 20 (standalone components, signals, the new `@if`/`@for` control-flow syntax), Reactive Forms, Google Maps JavaScript API & Places Autocomplete, Google Identity Services.

**Architecture** — layered backend: `Core` (models, DTOs, interfaces) → `Data` (EF Core repositories & migrations) → `Services` (business logic) → `WebAPI` (controllers).

## Project structure

```
VacationApartmentRentalCenter/
├── Server/
│   ├── Core/       # Models, resource DTOs, repository & service interfaces
│   ├── Data/        # EF Core DbContext, repositories, migrations
│   ├── Services/     # Business logic
│   └── WebAPI/       # Controllers, Program.cs, configuration
└── Client/           # Angular application
```

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [Node.js](https://nodejs.org/) 18+ and npm
- SQL Server (LocalDB or a full instance)
- A Google Cloud project with a **Maps JavaScript API** key (Maps + Places APIs enabled) and an **OAuth 2.0 Client ID** (for Google Sign-In)

## Getting started

### 1. Clone the repository

```bash
git clone <repo-url>
cd VacationApartmentRentalCenter
```

### 2. Backend setup

Secrets are kept out of source control via .NET User Secrets. From `Server/WebAPI`:

```bash
cd Server/WebAPI
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:Vacation apartments" "<your SQL Server connection string>"
dotnet user-secrets set "Jwt:Key" "<a random string, at least 32 characters>"
dotnet user-secrets set "EmailSettings:SenderEmail" "<sender email address>"
dotnet user-secrets set "EmailSettings:AppPassword" "<app password>"
```

(`EmailSettings` is only needed if you use the outgoing-email feature — otherwise leave it as-is.)

Apply the database migrations:

```bash
cd ..
dotnet ef database update --project Data --startup-project WebAPI
```

Run the API:

```bash
dotnet run --project WebAPI
```

The API listens on `https://localhost:7011` by default (Swagger UI is available at `/swagger` in development).

### 3. Frontend setup

```bash
cd Client
npm install
```

`src/environments/environment.ts` (and `environment.prod.ts`) already points `apiUrl` at `https://localhost:7011`. Set your own Google OAuth Client ID there if you don't want to reuse the one committed in the repo:

```ts
export const environment = {
  production: false,
  apiUrl: 'https://localhost:7011',
  googleClientId: '<your Google OAuth Client ID>'
};
```

Run the dev server:

```bash
npm start
```

The app is served at `http://localhost:4200`.

## Configuration notes

- **Google Maps API key** is referenced directly in `Client/src/index.html` and is restricted (in Google Cloud Console) to the `http://localhost:4200/*` referrer. Before deploying to another domain, either add that domain as an allowed referrer or swap in your own key.
- **JWT signing key** must be at least 32 characters and is read from configuration (`Jwt:Key`). Never commit it — set it locally with `dotnet user-secrets` and in production via your host's secret manager.
- **CORS** allowed origins are configured under `Cors:AllowedOrigins` in `appsettings.json` (defaults to `http://localhost:4200`).

## License

No license has been declared for this project yet.
