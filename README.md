# TrackWizz Bank Backend - ASP.Net Core Web API

## Overview
TrackWizz Bank is a secure and efficient banking system built with **ASP.NET Core** and **Entity Framework Core**, utilizing **MS SQL Server** as the database. This project provides essential banking functionalities like account management, transactions, authentication, and more.

## Features
- User authentication with **JWT & HTTP-only cookies**
- Role-based access control (**Customer, Bank Admin**)
- Account management (Create, View, Status updates)
- Money transfer between accounts
- Transaction history tracking
- Loan application & interest calculation
- Logging with **log4net** for monitoring & debugging
- Secure API endpoints with **ASP.NET Core Identity & Authorization**

## Tech Stack
- **Backend**: ASP.NET Core 8, C#
- **Database**: MS SQL Server, Entity Framework Core
- **Authentication**: JWT & Cookies-based authentication
- **Logging**: log4net
- **API Documentation**: Swagger (Swashbuckle)

## Project Structure
```
TrackWizz-Bank-ASP.Net-Core/
│-- Controllers/         # API Controllers
│-- DTO/                # Data Transfer Objects
│-- Interfaces/         # Service Interfaces
│-- Managers/           # Business Logic Layer
│-- Models/             # Database Models
│-- Utils/              # Helper functions
│-- appsettings.json    # Configuration settings
│-- log4net.config      # Logging configuration
│-- Program.cs          # Application Entry Point
│-- BankManagementSystem.sln # Solution file
```

## Setup Instructions
### Prerequisites
- .NET SDK 8.0+
- MS SQL Server
- Visual Studio / VS Code
- Postman (for API testing, optional)

### Installation & Setup
1. **Clone the repository**
   ```sh
   git clone https://github.com/your-username/TrackWizz-Bank-ASP.Net-Core.git
   cd TrackWizz-Bank-ASP.Net-Core
   ```
2. **Configure Database**
   - Update the **connection string** in `appsettings.json`:
     ```json
     "ConnectionStrings": {
       "DefaultConnection": "Server=YOUR_SERVER;Database=BankDB;Trusted_Connection=True;"
     }
     ```
   - Run the migrations:
     ```sh
     dotnet ef database update
     ```

3. **Run the Application**
   ```sh
   dotnet run
   ```

4. **Access API Documentation**
   - Open `http://localhost:5000/swagger` in your browser

## API Endpoints
| Method | Endpoint                  | Description               |
|--------|---------------------------|---------------------------|
| POST   | /auth/login               | User login                |
| POST   | /auth/register            | User registration         |
| GET    | /accounts                 | Get all accounts          |
| GET    | /accounts/{id}            | Get account by ID         |
| POST   | /transactions/transfer    | Money transfer            |
| GET    | /transactions/history     | Get transaction history   |

## Logging
This project uses **log4net** for logging.
- Logs are stored in `logs/app.log`
- Update `log4net.config` for custom log settings

## Contributing
1. Fork the repository
2. Create a feature branch (`git checkout -b feature-name`)
3. Commit your changes (`git commit -m 'feat: add new feature'`)
4. Push to GitHub (`git push origin feature-name`)
5. Open a Pull Request

## License
This project is licensed under the **MIT License**.

---
🚀 **Happy Coding!**

