# Multi-Project Repository

This repository contains two main projects: a Kafka-based chat application and a movie store system with MongoDB integration. Both projects demonstrate different aspects of .NET 8 development with various technologies.

## 📁 Projects Overview

### 1. KafkaProject - Real-time Chat Application
A console-based chat application built with Apache Kafka for real-time messaging.

### 2. MovieStoreB - Movie Management System
A web API for managing movies and actors with MongoDB storage, caching, and Kafka integration.

---

## 🚀 KafkaProject - Chat Application

### Features
- Real-time messaging using Apache Kafka
- Console-based interface
- MessagePack serialization for efficient data transfer
- Multi-user support with unique usernames

### Tech Stack
- .NET 8
- Confluent.Kafka
- MessagePack

### Prerequisites
- .NET 8 SDK
- Access to Kafka broker (configured for CloudClusters)

### Getting Started

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd KafkaProject
   ```

2. **Run the application**
   ```bash
   dotnet run
   ```

3. **Usage**
   - Enter your username when prompted
   - Start typing messages
   - Messages are broadcast to all connected users

### Configuration
The Kafka connection is configured for CloudClusters in the consumer and producer classes:
- Bootstrap Server: `kafka-193981-0.cloudclusters.net:10300`
- SASL/SSL authentication enabled

---

## 🎬 MovieStoreB - Movie Management System

### Features
- RESTful API for movie and actor management
- MongoDB integration with automatic ID generation
- Background caching system with Kafka integration
- Health checks
- Swagger documentation
- FluentValidation for request validation
- Mapster for object mapping
- Comprehensive unit testing

### Tech Stack
- .NET 8 Web API
- MongoDB
- Apache Kafka
- FluentValidation
- Mapster
- Serilog
- xUnit (testing)
- Moq (mocking)

### Architecture
The project follows a clean architecture pattern:
- **Controllers**: API endpoints
- **Business Logic (BL)**: Service layer
- **Data Layer (DL)**: Repository pattern with MongoDB
- **Models**: DTOs, requests, responses
- **Tests**: Unit tests with mocking

### Prerequisites
- .NET 8 SDK
- MongoDB instance
- Access to Kafka broker (optional for caching)

### Getting Started

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd MovieStoreB
   ```

2. **Configure MongoDB**
   Update `appsettings.json` with your MongoDB connection string:
   ```json
   {
     "MongoDbConfiguration": {
       "ConnectionString": "your-mongodb-connection-string",
       "DatabaseName": "MoviesDb"
     }
   }
   ```

3. **Run the application**
   ```bash
   dotnet run --project MovieStoreB
   ```

4. **Access Swagger UI**
   Navigate to `https://localhost:7166/swagger` in your browser

### API Endpoints

#### Movies
- `GET /Movies/GetAll` - Get all movies
- `GET /Movies/GetById?id={id}` - Get movie by ID
- `POST /Movies/AddMovie` - Add a new movie
- `DELETE /Movies/Delete?id={id}` - Delete a movie

#### Health Checks
- `GET /healthz` - Application health status

### Database Models

#### Movie
```csharp
{
  "id": "string",
  "title": "string",
  "year": 2024,
  "actorIds": ["string"]
}
```

#### Actor
```csharp
{
  "id": "string",
  "name": "string"
}
```

### Caching System
The application includes a sophisticated caching system:
- Background services for cache population
- Kafka integration for cache invalidation
- Configurable refresh intervals
- Differential loading for performance

### Testing

Run unit tests:
```bash
dotnet test
```

The test suite includes:
- Service layer tests with mocking
- Repository tests
- Validation tests

### Monitoring & Logging
- Serilog integration with console output
- Health checks for monitoring
- Structured logging with themes

---

## 🛠️ Development

### Project Structure
```
Repository/
├── KafkaProject/
│   ├── Models/
│   ├── ConsoleHelper.cs
│   ├── KafkaConsumer.cs
│   ├── KafkaProducer.cs
│   └── Program.cs
└── MovieStoreB/
    ├── MovieStoreB/          # Web API
    ├── MovieStoreB.BL/       # Business Logic
    ├── MovieStoreB.DL/       # Data Layer
    ├── MovieStoreB.Models/   # Shared Models
    └── MovieStoreB.Tests/    # Unit Tests
```

### Future Enhancements
- Authentication and authorization for MovieStore API
- User interface for the movie management system
- Enhanced error handling and logging
- Database migrations
- Docker containerization
- CI/CD pipeline setup

---
