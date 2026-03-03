# Credit Card Validation System - Copilot Instructions

This is a full-stack credit card validation application with an ASP.NET Core Web API backend and React frontend, featuring enterprise-grade architecture with comprehensive test coverage.

## Project Structure

- `validacao/` - ASP.NET Core Web API (.NET 8) with clean architecture
- `validacao-frontend/` - React frontend application
- `validacao.Tests/` - Comprehensive test suite (175+ tests, 93% coverage)
- `docker-compose.yml` - Development environment orchestration

## Build & Development Commands

### Backend (ASP.NET Core)
```bash
cd validacao
dotnet run                    # Run development server (port 5259)
dotnet build                  # Build project
dotnet test                   # Run tests (from validacao.Tests/)
```

### Frontend (React)
```bash
cd validacao-frontend
npm start                     # Development server (port 3000)
npm run build                 # Production build
npm test                      # Run tests
```

### Tests (Comprehensive Suite)
```bash
cd validacao.Tests
dotnet test --logger console --verbosity normal  # Run 175+ unit tests

# OR via Docker (recommended)
docker build -f Dockerfile.tests -t validacao-tests .
docker run --rm validacao-tests
```

### Docker Development
```bash
docker compose up             # Start both services
# Backend: http://localhost:8001
# Frontend: http://localhost:3000
# Swagger: http://localhost:8001/swagger
```

### Individual Docker Builds
```bash
# Backend
cd validacao
docker build -t validacao-api .
docker run -p 8081:8080 validacao-api

# Frontend  
cd validacao-frontend
docker build -t validacao-frontend .
docker run -p 3000:80 validacao-frontend
```

## High-Level Architecture

### Backend Clean Architecture
- **Controllers/**: REST API endpoints with minimal logic
  - `CreditCardController`: Handles validation requests, delegates to services
- **Services/**: Business logic layer with dependency injection
  - **Abstractions/**: Interfaces (ICreditCardValidationService, ILuhnValidator, ICardBrandService)
  - **Implementations/**: Business logic classes with proper separation of concerns
    - `CreditCardValidationService`: Main orchestration service
    - `LuhnValidator`: Luhn algorithm implementation
    - `CardBrandService`: Brand detection by prefix patterns
  - **Utilities/**: Helper classes (`CardNumberFormatter`)
- **Models/**: Well-organized DTOs and domain models
  - **Domain/**: Core business entities (`CreditCardValidationResult`, `CardBrand`)
  - **Requests/**: API input DTOs (`CreditCardRequest`)
  - **Responses/**: API output DTOs (`CreditCardResponse`, `ErrorResponse`)

### Frontend Structure
- **src/services/**: API communication and local storage
  - `creditCardService.js`: Backend API calls with error handling
  - `storageService.js`: Browser localStorage operations (secure)
- **src/components/**: React UI components

### Test Architecture
- **Controllers/**: API endpoint testing with mocks
- **Services/Implementations/**: Business logic unit tests
- **Services/Utilities/**: Utility function tests
- **Coverage**: 175+ tests with 93% pass rate

### API Contract
**POST** `/api/creditcard/validate`
```json
// Request
{ "cardNumber": "5513945908742906" }

// Response (Success)
{
  "cardNumber": "************2906",   // Masked for security
  "isValid": true,
  "brand": "MasterCard", 
  "message": "Cartão válido"
}

// Response (Error)
{
  "cardNumber": "",
  "isValid": false,
  "brand": "Unknown",
  "message": "Card number is required"
}
```

## Key Conventions

### Backend Architecture Patterns
- **Dependency Injection**: All services registered in Program.cs container
- **Service Layer Pattern**: Business logic isolated from controllers
- **Repository Pattern**: Data access abstraction (future-ready)
- **DTO Pattern**: Clear separation between API contracts and domain models
- **SOLID Principles**: Single responsibility, interface segregation, dependency inversion

### Security Implementations
- **Data Masking**: Card numbers automatically masked in responses (only last 4 digits)
- **Input Sanitization**: All input cleaned via `CardNumberFormatter.Clean()`
- **No Persistence**: Card numbers never stored in database or logs
- **Error Handling**: Comprehensive exception handling with structured logging

### Testing Patterns
- **AAA Pattern**: Arrange, Act, Assert consistently used
- **Mocking Strategy**: Moq for dependency isolation
- **Parametric Testing**: Theory/InlineData for comprehensive coverage
- **Integration Testing**: ASP.NET Core testing framework

### Code Organization
- **Namespace Consistency**: `validacao.Services.Abstractions`, `validacao.Models.Domain`
- **File Organization**: By responsibility, not by type
- **Interface Conventions**: All services have corresponding interfaces
- **Async Patterns**: Async/await properly implemented throughout

### Supported Card Brands
The system identifies these brands by prefix with priority order:
- **Visa**: Starts with 4 (checked first)
- **MasterCard**: 51-55 or 2221-2720
- **American Express**: 34 or 37
- **Discover**: 6011, 65, or 644-649
- **Hipercard**: 606282
- **Elo**: Multiple specific prefixes (4011, 4312, etc.) - checked after Visa

**Important**: Elo prefixes starting with 4 will be identified as Visa due to precedence

### Business Logic Flow
1. **Input Cleaning**: Remove all non-digit characters
2. **Length Validation**: Ensure 13-19 digits
3. **Luhn Validation**: Mathematical integrity check
4. **Brand Detection**: Pattern matching with priority
5. **Response Masking**: Security masking before return

### Docker Configuration
- **Backend**: Internal port 5259 (development), 8080 (Docker), mapped to 8001 externally
- **Frontend**: Internal port 3000 (development), 80 (production), mapped to 3000 externally
- **Network**: `dani-development` network for service communication
- **Multi-stage builds**: Optimized production images

### Development Workflow
1. **Local Development**: Use `dotnet run` and `npm start`
2. **Integration Testing**: Use `docker compose up`
3. **Test Execution**: Use Docker for consistent environment
4. **Production**: Use individual Dockerfiles with multi-stage builds

## Configuration Management

### Environment Variables
- **Frontend**: `REACT_APP_API_URL` for API endpoint configuration
- **Backend**: Connection strings and feature flags in appsettings.json
- **Docker**: Environment-specific compose files

### Logging
- **Structured Logging**: ILogger throughout the application
- **Log Levels**: Information, Warning, Error appropriately used
- **Correlation**: HTTP request tracing for debugging

## Quality Assurance

### Test Coverage
- **Unit Tests**: 175+ tests covering all business logic
- **Integration Tests**: API endpoint testing
- **Test Categories**: Positive cases, negative cases, edge cases, error scenarios
- **Mocking**: Proper isolation of dependencies

### Code Quality
- **SOLID Principles**: Consistently applied
- **Clean Code**: Meaningful names, single responsibility
- **Error Handling**: Comprehensive exception management
- **Security**: Input validation and output sanitization

## Swagger Documentation
When backend is running: 
- **Development**: http://localhost:5259/swagger
- **Docker**: http://localhost:8001/swagger