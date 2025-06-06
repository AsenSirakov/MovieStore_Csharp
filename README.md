# C# Projects Repository

This repository contains two main C# projects showcasing different aspects of modern .NET development, messaging systems, and enterprise architecture patterns.

## 📁 Projects Overview

### 1. KafkaProject - Real-time Chat Application
A simple console-based chat application demonstrating Apache Kafka messaging capabilities.

### 2. MovieStoreB - Enterprise Movie Management System
A comprehensive NET Core Web API showcasing clean architecture, caching strategies, external API integration, and distributed messaging.

---

## 🚀 KafkaProject

### Overview
A lightweight real-time chat application built with .NET 8 and Apache Kafka for message distribution.

### Features
- Real-time messaging using Apache Kafka
- MessagePack serialization for efficient data transfer
- Multi-user chat support
- Console-based interface

### Key Technologies
- **.NET 8**
- **Confluent.Kafka** - Kafka client library
- **MessagePack** - Binary serialization

### Quick Start
1. Ensure Kafka is running on your system
2. Clone the repository
3. Navigate to `KafkaProject` directory
4. Run `dotnet run`
5. Enter your username and start chatting!

---

## 🎬 MovieStoreB - Enterprise Movie Management System

### Architecture Overview
MovieStoreB follows a **Clean Architecture** pattern with clear separation of concerns across multiple layers:

```
MovieStoreB/
├── MovieStoreB/              # Presentation Layer (Web API)
├── MovieStoreB.BL/           # Business Logic Layer
├── MovieStoreB.DL/           # Data Access Layer
├── MovieStoreB.Models/       # Shared Models & DTOs
└── MovieStoreB.Tests/        # Unit Tests
```

### 🏗️ Key Features

#### Core Functionality
- **Movie Management**: CRUD operations for movies with actor associations
- **Actor Management**: Complete actor lifecycle management
- **Full Movie Details**: Rich aggregated views combining movies with actor information

#### Advanced Features
- **Multi-layered Caching**: In-memory caching with optional Kafka distribution
- **External API Integration**: REST client with RestSharp for external data sources
- **Background Services**: Automated cache population and synchronization
- **Health Checks**: Application monitoring and diagnostics
- **Comprehensive Logging**: Structured logging with Serilog

#### Enterprise Patterns
- **Repository Pattern**: Clean data access abstraction
- **Dependency Injection**: Comprehensive IoC container usage
- **Service Layer**: Business logic encapsulation
- **Gateway Pattern**: External API integration abstraction

### 🛠️ Technology Stack

#### Core Framework
- **.NET 8** - Latest LTS version
- **ASP.NET Core** - Web API framework
- **C# 12** - Latest language features

#### Data & Persistence
- **MongoDB** - Primary database with Cloud Atlas integration
- **MongoDB.Driver** - Official .NET driver

#### Messaging & Caching
- **Apache Kafka** - Distributed event streaming (optional)
- **MessagePack** - High-performance binary serialization
- **In-Memory Caching** - Fast data access layer

#### External Integration
- **RestSharp** - HTTP client for external APIs
- **HttpClient** - .NET HTTP client integration

#### Development & Testing
- **xUnit** - Unit testing framework
- **Moq** - Mocking framework for unit tests
- **FluentAssertions** - Readable test assertions

#### Utilities & Mapping
- **Mapster** - Object mapping
- **FluentValidation** - Input validation
- **Serilog** - Structured logging
- **Swagger/OpenAPI** - API documentation

### 🗄️ Data Architecture

#### Models
```csharp
// Core entity with caching support
public record Movie : CacheItem<string>
{
    public string Id { get; set; }
    public string Title { get; set; }
    public int Year { get; set; }
    public List<string> ActorIds { get; set; }
    public DateTime DateInserted { get; set; }
}

public record Actor : CacheItem<string>
{
    public string Id { get; set; }
    public string Name { get; set; }
    public DateTime DateInserted { get; set; }
}
```

#### Repository Pattern Implementation
- **IMovieRepository** / **IActorRepository**: Core data access contracts
- **MongoDB Repositories**: Production-ready implementations
- **Cache Repository Interface**: Unified caching abstraction

### 🔄 Caching Strategy

#### Multi-Level Caching
1. **In-Memory Cache**: Ultra-fast local data access
2. **Kafka Distribution** (Optional): Cross-instance cache synchronization
3. **Background Refresh**: Automated cache population and updates

#### Cache Configuration
```json
{
  "KafkaConfiguration": {
    "Enabled": false,
    "BootstrapServers": "localhost:9092"
  },
  "MoviesCacheConfiguration": {
    "Topic": "movies_cache",
    "RefreshInterval": 15
  }
}
```

### 🌐 API Endpoints

#### Movies Management
- `GET /Movies/GetAll` - Retrieve all movies
- `GET /Movies/GetById/{id}` - Get specific movie
- `POST /Movies/AddMovie` - Create new movie
- `DELETE /Movies/Delete/{id}` - Remove movie

#### Actors Management
- `GET /Actors/GetAll` - Retrieve all actors
- `GET /Actors/GetById/{id}` - Get specific actor
- `POST /Actors/Add` - Create new actor
- `PUT /Actors/Update/{id}` - Update actor
- `DELETE /Actors/Delete/{id}` - Remove actor

#### External API Integration
- `GET /ExternalApi/import-movies` - Import from external sources
- `GET /ExternalApi/external-actor/{name}` - Fetch external actor data
- `GET /ExternalApi/cache-status` - Monitor cache health

### 🧪 Testing Strategy

#### Unit Tests Coverage
- **Service Layer Testing**: Business logic validation
- **Repository Mocking**: Isolated data access testing
- **Integration Testing**: End-to-end workflow validation

#### Test Structure
```csharp
[Fact]
public async Task GetAllMovieDetails_ReturnsData()
{
    // Arrange - Setup mocks and test data
    // Act - Execute the method under test
    // Assert - Verify expected outcomes
}
```

### ⚙️ Configuration & Setup

#### Prerequisites
1. **.NET 8 SDK** installed
2. **MongoDB** instance (local or cloud)
3. **Apache Kafka** (optional, for distributed caching)

#### Environment Setup

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd MovieStoreB
   ```

2. **Configure MongoDB**
   Update `appsettings.json`:
   ```json
   {
     "MongoDbConfiguration": {
       "ConnectionString": "your-mongodb-connection-string",
       "DatabaseName": "MoviesDb"
     }
   }
   ```

3. **Optional: Setup Kafka**
   ```bash
   docker-compose up -d  # Starts Kafka, Zookeeper, and Kafka UI
   ```

4. **Run the application**
   ```bash
   dotnet run --project MovieStoreB
   ```

5. **Access the API**
   - Swagger UI: `https://localhost:7030/swagger`
   - Health Check: `https://localhost:7030/healthz`

#### Docker Support
The project includes a `docker-compose.yml` for easy Kafka setup:
- **Kafka**: `localhost:9092`
- **Kafka UI**: `http://localhost:8080`
- **Zookeeper**: `localhost:2181`

### 🔧 Development Guidelines

#### Adding New Features
1. **Models**: Add DTOs to `MovieStoreB.Models`
2. **Data Layer**: Implement repositories in `MovieStoreB.DL`
3. **Business Logic**: Create services in `MovieStoreB.BL`
4. **API**: Add controllers in `MovieStoreB/Controllers`
5. **Tests**: Write unit tests in `MovieStoreB.Tests`

#### Dependency Injection
The application uses a modular DI approach:
```csharp
services
    .AddConfigurations(config)
    .AddDataDependencies(config)
    .AddBusinessDependencies();
```

### 📊 Monitoring & Observability

#### Health Checks
- Custom health check implementations
- Built-in endpoint at `/healthz`
- Integration with monitoring tools

#### Logging
- Structured logging with Serilog
- Console output with syntax highlighting
- Configurable log levels per namespace

#### Cache Monitoring
- Real-time cache statistics via API
- Cache hit/miss metrics
- Background service health monitoring

### 🔒 Production Considerations

#### Security
- Input validation with FluentValidation
- Secure MongoDB connections
- HTTPS enforcement

#### Performance
- Efficient MessagePack serialization
- Optimized database queries
- In-memory caching for frequently accessed data

#### Scalability
- Kafka-based cache distribution for horizontal scaling
- Background services for async processing
- Clean architecture supporting microservices decomposition

### 📜 License

This project is provided as-is for educational and demonstration purposes.

---

## 🎯 Learning Outcomes

This repository demonstrates:
- **Enterprise .NET Development** patterns and practices
- **Clean Architecture** implementation
- **Distributed Systems** concepts with Kafka
- **Modern C#** features and best practices
- **Test-Driven Development** approaches
- **External API Integration** strategies
- **Caching** strategies for performance optimization
- **MongoDB** integration in .NET applications
