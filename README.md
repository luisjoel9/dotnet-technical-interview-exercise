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

To create the SQL Server database and populate it with initial seed data, navigate to the project's root directory and execute the SQL scripts located in the Scripts folder in the following order:

01_CreateDatabase.sql – Creates the database schema, tables, and required objects.
02_SeedData.sql – Inserts sample data required for testing and development.

Note: Ensure that SQL Server is running and that you have the necessary permissions to create databases and execute scripts before running these files.

### Credentials
#### User 1:
```bash
"email": "pepe@asdf.com",
"password": "joel1234" 
```
#### User 2:
```bash
"email": "cris@asdf.com",
"password": "123456" 
```

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

## Generative AI tools
For this exercise I used GitHub Copilot as the Generative AI coding assistant.

## Generative AI Prompt

Develop a complete task management application using ASP.NET Core 8 Web API and Angular 18.

### Backend Requirements

- Use ASP.NET Core 8 Web API.
- Follow Clean Architecture principles.
- Create the following layers:
  - Domain
  - Application
  - Infrastructure
  - Presentation
- Do not use Entity Framework, Dapper, or MediatR.
- Use SQL Server as the database.
- Implement repositories using ADO.NET.
- Use dependency injection.
- Implement JWT authentication.

### User Features

- User registration.
- User login.
- Password hashing.
- JWT token generation.
- Public and protected endpoints.

### Task Features

- Create, read, update, and delete tasks.
- Each task must contain:
  - Id (Guid)
  - Title
  - Description
  - Status
  - DueDate
  - UserId
- A task must belong to a user.
- Only authenticated users can manage their tasks.

### Validation Rules

- Title is required.
- Title maximum length is 100 characters.
- Due date cannot be earlier than the current date.
- Return appropriate validation messages and HTTP status codes.

### Tests

- Generate unit tests using xUnit.
- Include tests for controllers, services, and repositories.

### Frontend Requirements

- Use Angular 18 with standalone components.
- Create a responsive user interface.
- Use Angular routing.
- Implement JWT authentication.
- Store the token securely in local storage.
- Create login and registration pages.
- Create a task dashboard.
- Implement task creation, editing, deletion, and listing.
- Use Angular services to communicate with the backend.
- Implement route guards for protected pages.
- Display validation messages in forms.
- Organize the code using feature-based folders.

### Project Structure

- Provide a recommended folder structure for both backend and frontend.
- Include dependency injection configuration.
- Include sample SQL scripts for database creation.
- Include seeded data with a demo user and sample tasks.
- Include a README with setup instructions.

#### Validating AI Suggestions

I used the generated code as a starting point but I reviewed everything before using it in the project.

Some of the things I checked were:

- That responsibilities were separated among controllers, services, and repositories.
- That business logic was not implemented directly in the controllers.
- That endpoints returned the correct HTTP status codes.
- That authentication and JWT generation worked correctly.
- That the SQL queries matched the database schema.
- That application layer did not use any third party implementation like mappers, jwt helpers, etc.

### Corrections and improvements made

The generated code was useful but some changes were necessary before could be used.

#### Additional validations

Some validations were missing, so I added checks such as:

The title cannot be empty.
Expiration dates cannot be set in the past.

#### Authentication Improvements

The initial code stored passwords in plain text, which is unacceptable. I fixed this by hashing the passwords before storing them and validating the hash during login, jwt tokens were also configured.

#### Error Handling

I added centralized exception handling so that the API returns consistent error responses instead of exposing internal exceptions.

#### API Behavior

I also verified that the API returns the expected status codes based on the operations result, such as success responses, validation errors, unauthorized access and not-found resource scenarios.

#### Outcome
The GitHub Copilot output is represented by images in the genai-screenshots folder located in the project root directory.