# BookTracking API

This is a .NET 8 Web API project for tracking books and authors.

## Prerequisites

- **Docker Desktop**: Required to run the PostgreSQL database.
- **.NET 8 SDK**: Required to build and run the application.

## How to Run

Follow these simple steps to get the project running locally and test it with Swagger.

### 1. Clone the Repository

```bash
git clone <repository-url>
cd BookTracking
```

### 2. Start the Database

Run the following command in the root directory (where `docker-compose.yml` is located) to start the PostgreSQL database container:

```bash
docker-compose up -d
```

### 3. Run the API

Navigate to the API project directory and run the application:

```bash
cd src/BookTracking.API
dotnet run
```

The application will automatically apply any pending database migrations and seed initial data (Books and Authors) when it starts.

### 4. Open Swagger UI

Once the application is running, open your browser and navigate to the following URL to access the Swagger UI and test the endpoints:

[http://localhost:5063/swagger](http://localhost:5063/swagger)

*(Or https://localhost:7092/swagger if you prefer HTTPS)*

## Seeded Data

When the application starts for the first time, it automatically seeds the database with the following initial data:

- **Authors**: J.K. Rowling, J.R.R. Tolkien, George Orwell
- **Books**: Harry Potter, The Hobbit, The Lord of the Rings, 1984
- **Audit Logs**: Creation logs for all the above authors and books
