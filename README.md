# Vogel API – Social media API

⚠️ Not production ready

>Vogel API provides a robust backend for social media applications, enabling developers to manage users, posts, comments, likes, and real-time interactions.
  It’s designed for seamless integration, scalability, and secure handling of social data, powering modern social experiences efficiently.

## 📖 Overview

**Vogel API** is a modular monolithic social media backend built using Domain-Driven Design (DDD) principles. Its modular architecture organizes each domain into independent,
maintainable components while running as a single deployable application

The API implements CQRS (Command Query Responsibility Segregation) for its database operations:

  - SQL Server handles commands (write operations), ensuring transactional integrity and relational consistency.

  - MongoDB handles queries (read operations), providing fast, scalable access for feeds, posts, and other read-heavy data.

By combining DDD, modular monolith architecture, and CQRS with dual databases, Vogel API delivers a scalable, maintainable, and high-performance backend for modern social media applications.

---
## Features
  - **User management** – Create, update, and manage user accounts and profiles.
  - **Friend Requests** - Send, receive, accept, or reject friend requests.
  - **Unfriend / Remove Connections** – Remove existing friends easily.
  - **Mutual Connections** – Discover mutual friends with other users.  
  - **Post management** - Handles creation, editing, deletion, and interaction with user posts through comments or interactions emojis.
  - **Messanger** - Enables real-time private messaging between users.

## Architecture
Vogel API is designed as a modular monolithic application using Domain-Driven Design (DDD) principles, combining maintainability, scalability, and performance in a single deployable system.
The architecture is organized into multiple layers and modules, each responsible for a specific part of the system.

1. **Layers** :
    - **Presentation / API Layer** – Exposes endpoints for clients, handling request validation, authentication, and response formatting.
       This layer interacts with the application layer to execute business use cases.
    
    - **Application Layer** – Contains application services, orchestrates use cases, enforces business rules, and interacts with the domain layer.
    
    - **Domain Layer** – Represents the core business logic, including entities, aggregates, value objects, and domain services. All domain rules and behaviors reside here, independent of external infrastructure.
    
    - **Infrastructure Layer** – Handles database access, messaging, external services, and file storage. This layer provides concrete implementations required by the domain and application layers.
  
2. **Modular Structure** :
   
   Each domain (Social,  Content, Messenger) is implemented as a separate module, with its own domain, application, and infrastructure components. This modularity allows:
    - Independent development and testing of modules
    - Clear separation of responsibilities
    - Easier future scaling or refactoring
3. **Data Management & CQRS** :
   
   Vogel API applies the CQRS (Command Query Responsibility Segregation) pattern for database operations:
    - Commands (Writes) → Handled by SQL Server, ensuring transactional consistency and relational integrity.
    - Queries (Reads) → Handled by MongoDB, optimized for fast and scalable read operations, such as users , posts , comments.
      
4. **Key Benefits** :
    - **Maintainability** – Clear separation between layers and modules simplifies updates and enhancements. 
    - **Scalability** - Dual-database and modular design allow scaling specific parts of the system without affecting the entire application.
    - **Performance** - CQRS and specialized databases optimize read and write operations for social media workloads.
    - **Extensibility** - New modules or features can be added with minimal impact on existing functionality.
  

## 📂 Project Structure

  ### Project Modularity structure:
  ```bash
  📁 src/
  └── 📁 Modules/
      ├── 📁 Social/
      ├── 📁 Content/
      └── 📁 Messanger/
```
  ### Module structure
  ```bash
    📁 Social/
    ├── 📁 Vogel.Social.Application/
    ├── 📁 Vogel.Social.Domain/
    ├── 📁 Vogel.Social.Infrastructure/
    ├── 📁 Vogel.Social.MongoEntities/
    ├── 📁 Vogel.Social.Presentation/
    └── 📁 Vogel.Social.Shared/
  
  ```
  ### Test structure
  ```bash
   📁 tests/
    ├── 📁 Modules/
    │   ├── 📁 Content/
    │   │   └── 📁 Vogel.Content.Application.Tests/
    │   ├── 📁 Social/
    │   │   └── 📁 Vogel.Social.Application.Tests/
    │   └── 📁 Messanger/
    │       └── 📁 Vogel.Messanger.Application.Tests/
    └── 📁 Vogel.Application.Tests/
  ```
---
## Configuration

1. **Identity provider** (You can use auth0 as identity provider or ther related identity provider)
  ```json
   "IdentityProvider": {
    "Authority": "IDENTITY_PROVIDER_AUTHORITY_SERVER",
    "Audience": "API_AUDIENECE_KEY"
  }
  ```
2. **RabbitMQ Configuration**
```json
  "RabbitMq": {
    "Host": "RABBITMQ_HOST",
    "UserName": "RABBITMQ_USERNAME",
    "Password": "RABBITMQ_PASSWORD"
  }
```
3. **S3 Storage**
 ```json
   "S3Storage": {
    "EndPoint": "STORAGE_ENDPOINT",
    "AccessKey": "STORAGE_ACCESS_KEY",
    "SecretKey": "STORAGE_SECRET_KEY",
    "BucketName": "YOUR_BUCKET_NAME",
    "WithSSL" : "PREFERED_TO_BE_TRUE_PRODUCTION_MODE"
  }
```  

4. **SQL Server Configuraiton**
```json
 "ConnectionStrings": {
    "Default": "YOUR_SQL_SERVER_DB_CONNECTION_STRING"
  }
```
---
5. **MongoDb configuration**
   ```json
    "MongoDb": {
      "ConnectionString": "YOUR_MONGO_DB_SERVER",
      "Database": "YOUR_MONGO_DB_DATABASE"
    }
   ```


## Getting Started

### Prerequisites

Ensure you have the following installed:
- **.NET 8 SDK**  [Download]( https://dotnet.microsoft.com/download/dotnet/8.0)
- **SQL Server**
- **Mongo Db**
- **MinIO** (optional but recommended for local deployment or you can use other related service)
- **Auth0 (IDP)** (optional but recommended or you can use other related service)
- **IDE (optional but recommended)**
  - Visual Studio
  - Visual Studio Code

### Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/mohamedgamal17/Vogel.git
   cd Vogel
   ```

2. **Install .NET 8 SDK** 
     Verify installation:
     ```bash
      dotnet --version
     ```

3. **Restore Dependencies**
   ```bash
     dotnet restore
   ```
4. **Configure Application Settings**
```json
  {
     "IdentityProvider": {
      "Authority": "IDENTITY_PROVIDER_AUTHORITY_SERVER",
      "Audience": "API_AUDIENECE_KEY"
    },
     "RabbitMq": {
      "Host": "RABBITMQ_HOST",
      "UserName": "RABBITMQ_USERNAME",
      "Password": "RABBITMQ_PASSWORD"
    },
    "S3Storage": {
      "EndPoint": "STORAGE_ENDPOINT",
      "AccessKey": "STORAGE_ACCESS_KEY",
      "SecretKey": "STORAGE_SECRET_KEY",
      "BucketName": "YOUR_BUCKET_NAME",
      "WithSSL" : "PREFERED_TO_BE_TRUE_PRODUCTION_MODE"
    },
   "ConnectionStrings": {
      "Default": "YOUR_SQL_SERVER_DB_CONNECTION_STRING"
    },
   "MongoDb": {
      "ConnectionString": "YOUR_MONGO_DB_SERVER",
      "Database": "YOUR_MONGO_DB_DATABASE"
    },
  }
```
5. **Apply Database Migrations**
  ```bash
    dotnet ef database update
  ```

6. **Build the Project**
  ```bash
dotnet build
  ```
7. **Run the Application**
```bash
dotnet run
```

The API will be available at:

   ```bash
     https://localhost:7254
   ```
---

## 🧪 Testing
The project uses integration tests to verify the behavior of the application as a whole. Tests focus on the Application Layer,
ensuring that use cases, module interactions, and infrastructure integrations work correctly.

### Running Tests
Execute the following command from the root of the project:
```base
dotnet test
```
During test execution, the required containers will automatically start and be disposed after the tests complete.

 
