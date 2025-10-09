# Cherish

A .NET 8 Web API project for the Cherish platform with PostgreSQL database.

## Project Structure

```
Cherish/
├── RestApi/               # Web API project
│   ├── Controllers/       # API controllers
│   ├── Models/           # Request/Response models
│   ├── Services/         # Business logic services
│   └── Program.cs        # Application entry point
├── Domain/               # Domain layer
│   ├── Interfaces/       # Service and provider interfaces
│   └── Models/          # Domain models
├── Provider/            # Data access layer
│   └── PostgreSQLUserProvider.cs  # PostgreSQL implementation
└── Cherish.sln          # Solution file
```

## Features

- User authentication with JWT tokens
- PostgreSQL database integration
- Three main APIs:
  - Login
  - Update Password
  - Logout

## Database Configuration

## API Endpoints

### 1. Login
- **POST** `/api/auth/login`
- **Body**: `{ "username": "string", "password": "string" }`
- **Response**: `{ "token": "string", "username": "string", "expiresAt": "datetime" }`

### 2. Update Password
- **POST** `/api/auth/update-password`
- **Headers**: `Authorization: Bearer <token>`
- **Body**: `{ "currentPassword": "string", "newPassword": "string" }`

### 3. Logout
- **POST** `/api/auth/logout`
- **Headers**: `Authorization: Bearer <token>`

## Running the Application

1. Ensure you have .NET 8 SDK installed
2. Navigate to the project directory
3. Run: `dotnet run --project RestApi`
4. The API will be available at `https://localhost:5001` or `http://localhost:5000`
5. Swagger UI will be available at `/swagger` in development mode

## Database Setup

The application will automatically create the `users` table on first run with the following structure:

```sql
CREATE TABLE users (
    id SERIAL PRIMARY KEY,
    username VARCHAR(50) UNIQUE NOT NULL,
    password VARCHAR(255) NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
```

## Security Notes

- Passwords are stored in plain text (as requested)
- JWT tokens are used for authentication
- Tokens expire after 60 minutes
- Active tokens are tracked in memory for logout functionality
