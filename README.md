# BookTracking API

## Project Overview
This project is a .NET 8 Web API designed to manage **Books** and **Authors**. It also includes an **Audit Logging** system to record data creation and update events.

## How to Run

### Prerequisites
- Docker Desktop
- .NET 8 SDK

### Quick Start
1. **Start Database**:
   ```bash
   docker-compose up -d
   ```

2. **Run Application**:
   ```bash
   cd src/BookTracking.API
   dotnet run
   ```
   *The database will be automatically created and seeded with sample data.*

3. **Open Swagger**:
   [http://localhost:5063/swagger](http://localhost:5063/swagger)

## Key APIs
You can test the following endpoints via Swagger:

- **Books** (`/api/Book`): Manage complete book inventory (List, Create, Update, Delete).
- **Authors** (`/api/Author`): Manage author records (List, Create, Update, Delete).
- **Audit Logs** (`/api/AuditLog`): Filter and view the history of all data changes.
