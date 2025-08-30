```markdown
# Legal Contract Management System - Complete Setup Guide

## Overview

This system provides a complete legal contract management solution with:
- **.NET 9 Web API Backend** - RESTful API with CQRS pattern
- **Angular 20 Frontend** - Modern SPA with responsive design
- **PostgreSQL Database** - Reliable data storage
- **JWT Authentication** - Secure user authentication

## System Architecture

```
Frontend (Angular)  ??  Backend (.NET 9 API)  ??  Database (PostgreSQL)
     ?                        ?                         ?
- User Interface          - Business Logic          - Data Storage
- Authentication          - CQRS Handlers          - Entity Framework
- Contract CRUD           - AutoMapper             - Migrations
- Responsive Design       - JWT Tokens             - Audit Trails
```

## Backend Components

### API Project: `Legal.Api.WebApi`
- **Location**: `Api/Legal.Api.WebApi/`
- **Purpose**: Main entry point for all API requests
- **Key Features**:
  - Command/Query execution endpoints
  - JWT authentication
  - CORS configuration
  - File upload/download support

### Application Layer: `Legal.Application.Admin`
- **Location**: `Application/Legal.Application.Admin/`
- **Purpose**: Business logic and CQRS implementation
- **Key Components**:
  - Command Handlers (Save, Delete)
  - Query Handlers (Get, GetPaged)
  - Entity Framework Context
  - AutoMapper configuration

### Shared Models: `Legal.Shared.SharedModel`
- **Location**: `Shared/Legal.Shared.SharedModel/`
- **Purpose**: DTOs and shared contracts
- **Models**:
  - ContractParameterModel
  - ContractResponseModel
  - Authentication models

## Frontend Components

### Angular Application: `Legal.Website`
- **Location**: `Legal.Website/`
- **Purpose**: User interface for contract management
- **Key Features**:
  - Login/Registration
  - Contract CRUD operations
  - Responsive design with Bootstrap
  - JWT token management

## Setup Instructions

### 1. Backend Setup

1. **Database Setup**:
   ```bash
   # Ensure PostgreSQL is running
   # Update connection string in appsettings.json
   ```

2. **Run Migrations**:
   ```bash
   cd Application/Legal.Application.Admin
   dotnet ef database update
   ```

3. **Start API**:
   ```bash
   cd Api/Legal.Api.WebApi
   dotnet run
   ```
   - API will be available at `https://localhost:7001`

### 2. Frontend Setup

1. **Install Dependencies**:
   ```bash
   cd Legal.Website
   npm install
   ```

2. **Configure API URL**:
   - Development: Update `src/environments/environment.ts`
   - Production: Update `src/environments/environment.prod.ts`

3. **Start Development Server**:
   ```bash
   npm start
   ```
   - Application will be available at `http://127.0.0.1:4200`

## API Endpoints

### Authentication
- **POST** `/api/Command/Execute/ADMIN`
  - Login: `LogInCommandHandler`
  - Register: `RegistrationCommandHandler`

### Contract Management
- **POST** `/api/Query/Execute/ADMIN`
  - Get Contracts: `GetByPagedContractQueryHandlers`
  - Get Contract: `GetContractQueryHandler`
- **POST** `/api/Command/Execute/ADMIN`
  - Save Contract: `SaveContractCommandHandler`
  - Delete Contract: `DeleteContractCommandHandler`

## Contract Model Structure

```typescript
interface Contract {
  id: string;
  name: string;
  author: string;
  description: string;
  created: Date;
  updated?: Date;
}
```

## User Workflow

### 1. User Registration/Login
1. Navigate to the application
2. Register new account or login
3. Receive JWT token for subsequent requests

### 2. Contract Management
1. View paginated list of contracts
2. Create new contracts with form validation
3. Edit existing contracts
4. View contract details
5. Delete contracts with confirmation

### 3. Navigation
- Protected routes require authentication
- Automatic logout on token expiration
- Breadcrumb navigation for easy movement

## Security Features

- **JWT Authentication**: Stateless token-based authentication
- **Route Guards**: Protected routes in Angular
- **HTTP Interceptors**: Automatic token attachment
- **Password Hashing**: Secure password storage in backend
- **CORS Configuration**: Proper cross-origin request handling

## Development Features

- **Hot Reload**: Both frontend and backend support hot reload
- **Error Handling**: Comprehensive error handling throughout
- **Validation**: Client and server-side validation
- **Logging**: Structured logging in backend
- **Testing**: Ready for unit and integration tests

## Production Deployment

### Backend Deployment
1. Configure production database connection
2. Set up environment variables
3. Deploy to IIS, Docker, or cloud service
4. Configure HTTPS certificates

### Frontend Deployment
1. Build for production: `npm run build`
2. Deploy `dist/` contents to web server
3. Configure URL rewriting for SPA routes
4. Ensure API endpoints are accessible

## Configuration Files

### Backend Configuration
- `appsettings.json`: Database connections, JWT settings
- `Program.cs`: Service registration and middleware

### Frontend Configuration
- `environment.ts`: Development API URLs
- `environment.prod.ts`: Production API URLs
- `angular.json`: Build and serve configuration

## Database Schema

### Tables
- **user**: User accounts and authentication
- **contract**: Contract data with audit fields

### Key Features
- Audit trails (created/modified by/time)
- Soft delete capability
- Foreign key relationships
- Proper indexing

## Troubleshooting

### Common Issues
1. **CORS Errors**: Check CORS configuration in backend
2. **Authentication Failures**: Verify JWT configuration
3. **Database Connection**: Check connection string and PostgreSQL service
4. **Build Errors**: Ensure all dependencies are installed

### Logs and Debugging
- Backend logs: Check console output and log files
- Frontend logs: Use browser developer tools
- Database logs: Check PostgreSQL logs for query issues

## Future Enhancements

- Email verification for registration
- Password reset functionality
- Role-based access control
- Document attachments for contracts
- Contract versioning
- Advanced search and filtering
- Audit log viewer
- Multi-tenant support

## Support

For issues and questions:
1. Check the README files in each project
2. Review the API documentation
3. Check application logs
4. Verify configuration settings
```