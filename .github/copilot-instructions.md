# Credit Card Validation System - Copilot Instructions

This is a full-stack credit card validation application with an ASP.NET Core Web API backend and React frontend, both containerized with Docker.

## Project Structure

- `validacao/` - ASP.NET Core Web API (.NET 8)
- `validacao-frontend/` - React frontend application
- `docker-compose.yml` - Development environment orchestration

## Build & Development Commands

### Backend (ASP.NET Core)
```bash
cd validacao
dotnet run                    # Run development server (port 5259)
dotnet build                  # Build project
dotnet test                   # Run tests (if any exist)
```

### Frontend (React)
```bash
cd validacao-frontend
npm start                     # Development server (port 3000)
npm run build                 # Production build
npm test                      # Run tests
```

### Docker Development
```bash
docker compose up             # Start both services
# Backend: http://localhost:8001
# Frontend: http://localhost:3000
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

### Backend API Structure
- **Controllers/**: REST API endpoints (`CreditCardController`)
- **Service/**: Business logic layer
  - `LuhnValidator`: Implements Luhn algorithm for card validation
  - `CardBrandService`: Identifies card brands by prefix patterns
- **Model/**: Request/Response DTOs (`CreditCardRequest`, `CreditCardResponse`)

### Frontend Structure
- **src/services/**: API communication and local storage
  - `creditCardService.js`: Backend API calls
  - `storageService.js`: Browser localStorage operations
- **src/components/**: React UI components

### API Contract
**POST** `/api/creditcard/validate`
```json
// Request
{ "cardNumber": "5513945908742906" }

// Response
{
  "cardNumber": "5513945908742906",
  "isValid": true,
  "brand": "MasterCard", 
  "message": "Cartão válido"
}
```

## Key Conventions

### Backend (C#)
- Services are static classes with static methods
- Card number cleaning: Remove all non-digit characters before processing
- Brand identification happens only after Luhn validation passes
- Controllers follow REST naming: `/api/{resource}/{action}`
- Dependency injection not used - services are static utilities

### Frontend (React)
- API base URL: `http://localhost:8001` (development)
- All user input is stored in browser localStorage for history
- Card numbers accept spaces and are cleaned before API calls
- Bootstrap styling framework used throughout

### Supported Card Brands
The system identifies these brands by prefix:
- **Visa**: Starts with 4
- **MasterCard**: 51-55 or 2221-2720
- **American Express**: 34 or 37
- **Discover**: 6011, 65, or 644-649
- **Elo**: Multiple specific prefixes (4011, 4312, etc.)
- **Hipercard**: 606282

### Docker Configuration
- Backend runs on internal port 8080, mapped to 8001 externally
- Frontend production build served via Nginx on port 80
- Docker Compose creates `dani-development` network for service communication

## Swagger Documentation
When backend is running: http://localhost:8001/swagger (Docker) or http://localhost:5259/swagger (dotnet run)