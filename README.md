# Reminder App

## Overview

Reminder App is a full-stack web application that allows users to manage personal reminders securely and efficiently. Users can authenticate, and maintain their own reminders through a simple and intuitive interface.

The solution consists of:

- **Backend:** ASP.NET 8 Web API
- **Frontend:** Angular 18
- **Database:** SQL Server
- **Architecture:** Clean Architecture

The project was designed following separation of concerns principles, keeping business logic, infrastructure, and presentation layers independent and maintainable.

---

## Main Use Cases

### User Authentication

Registered users can log in using their email and password.

After successful authentication:

- A JWT token is generated.
- The token is stored on the client side.
- Access to protected features is granted.

### Create Reminders

Authenticated users can create reminders containing:

- Title
- Description
- Reminder date and time

### View Reminders

Users can view all reminders associated with their account.

### Update Reminders

Existing reminders can be edited whenever necessary.

### Delete Reminders

Users can remove reminders that are no longer needed.

---

## Architecture

The solution follows the principles of Clean Architecture and is organized into the following layers:

### Domain

Contains:

- Entities

Examples:

- User
- Reminder

### Application

Contains:

- DTOs
- Services
- Exceptions
- Repository Interfaces

### Infrastructure

Contains:

- Database access using ADO.NET
- Repository implementations
- JWT configuration
- External dependencies e.g. Mapster

### Presentation

Contains:

- API Controllers
- Dependency injection configuration
- Middleware
- HTTP endpoints

---

## Technologies

### Backend

- ASP.NET 8 Web API
- C#
- SQL Server
- JWT Authentication
- ADO.NET
- Mapster
- xUnit

### Frontend

- Angular 18
- TypeScript
- RxJS
- Angular Signals
- Bootstrap 5

---

## Prerequisites

Before running the application, make sure the following tools are installed:

- .NET SDK 8
- SQL Server
- Node.js 20+
- Angular CLI 18

Verify the installation:

```bash
dotnet --version
node --version
npm --version
ng version
```

---

## Database Setup

Create a SQL Server database and update the connection string in the API configuration file:

**appsettings.json**

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=ReminderAppDb;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

Execute the SQL script included in the project to create the required tables.

---

## Running the Backend

Navigate to the API project:

```bash
cd src/ReminderApp.Presentation
```

Restore dependencies:

```bash
dotnet restore
```

Build the solution:

```bash
dotnet build
```

Run the API:

```bash
dotnet run
```

The API will be available at:

```text
https://localhost:7093
```

Swagger UI:

```text
https://localhost:7093/swagger
```

---

## Running the Frontend

Navigate to the Angular project:

```bash
cd ui
```

Install dependencies:

```bash
npm install
```

Start the development server:

```bash
ng serve
```

The application will be available at:

```text
http://localhost:4200
```

---

## API Endpoints

### Users

| Method | Endpoint | Description |
|----------|----------|----------|
| POST | `/api/users/login` | Authenticate a user |
| GET | `/api/users` | Retrieve users |

### Reminders

| Method | Endpoint | Description |
|----------|----------|----------|
| GET | `/api/reminders` | Get all reminders |
| GET | `/api/reminders/{id}` | Get reminder by id |
| POST | `/api/reminders` | Create a reminder |
| PUT | `/api/reminders/{id}` | Update a reminder |
| DELETE | `/api/reminders/{id}` | Delete a reminder |

---

## Authentication

The API uses JWT Bearer Authentication.

Example request header:

```http
Authorization: Bearer <token>
```

Reminder endpoints require a valid authenticated user.

---

## Running Tests

Execute all tests:

```bash
dotnet test
```

Generate code coverage results:

```bash
dotnet test --collect:"XPlat Code Coverage"
```

---

## Notes

- Each user can only manage their own reminders.
- Passwords are stored securely using hashing mechanisms.
- Protected endpoints require a valid JWT token.
- The application follows Clean Architecture principles.
- Database access is implemented using ADO.NET without Entity Framework, Dapper or any other ORM.