# Legal Contract Management System - Frontend

This is an Angular web application for managing legal contracts. It provides a complete CRUD interface for contract management with user authentication.

## Features

- **User Authentication**
  - User registration
  - User login
  - JWT token-based authentication
  - Automatic token management

- **Contract Management**
  - View paginated list of contracts
  - Create new contracts
  - Edit existing contracts
  - View contract details
  - Delete contracts

- **User Interface**
  - Responsive design with Bootstrap 5
  - Clean and professional UI
  - Form validation
  - Loading indicators
  - Error handling

## Technology Stack

- Angular 20.2.0
- Bootstrap 5.3.0
- TypeScript 5.9.2
- RxJS 7.8.0
- JWT for authentication

## Prerequisites

- Node.js (v18 or higher)
- npm (v9 or higher)
- Angular CLI (v20 or higher)

## Installation

1. Install dependencies:

   ```bash
   npm install
   ```

2. Install Angular CLI globally (if not already installed):

   ```bash
   npm install -g @angular/cli
   ```

## Development Server

Run the development server:

```bash
npm start
```

Navigate to `http://127.0.0.1:4200/`. The application will automatically reload if you change any of the source files.

## Backend Configuration

The application is configured to communicate with the Legal API backend. Make sure to:

1. Update the API URL in `src/environments/environment.ts` for development
2. Update the API URL in `src/environments/environment.prod.ts` for production

Default configuration:
- Development: `https://localhost:7001/api`
- Production: `/api`

## API Integration

The application integrates with the Legal API using the following endpoints:

### Authentication
- **Login**: `POST /api/Command/Execute/ADMIN` with `LogInCommandHandler`
- **Register**: `POST /api/Command/Execute/ADMIN` with `RegistrationCommandHandler`

### Contract Management
- **Get Contracts (Paged)**: `POST /api/Query/Execute/ADMIN` with `GetByPagedContractQueryHandlers`
- **Get Contract**: `POST /api/Query/Execute/ADMIN` with `GetContractQueryHandler`
- **Save Contract**: `POST /api/Command/Execute/ADMIN` with `SaveContractCommandHandler`
- **Delete Contract**: `POST /api/Command/Execute/ADMIN` with `DeleteContractCommandHandler`

## Project Structure

```
src/
├── app/
│   ├── components/
│   │   ├── contract-form/          # Create/Edit contract form
│   │   ├── contract-list/          # Contract list with pagination
│   │   ├── contract-view/          # Contract details view
│   │   ├── login/                  # User login
│   │   └── register/               # User registration
│   ├── guards/
│   │   └── auth.guard.ts           # Route protection
│   ├── interceptors/
│   │   └── auth.interceptor.ts     # HTTP token interceptor
│   ├── models/
│   │   ├── auth.model.ts           # Authentication models
│   │   └── contract.model.ts       # Contract models
│   ├── services/
│   │   ├── api.service.ts          # Base API service
│   │   ├── auth.service.ts         # Authentication service
│   │   └── contract.service.ts     # Contract CRUD service
│   ├── app-module.ts               # Main app module
│   ├── app-routing-module.ts       # Routing configuration
│   └── app.ts                      # Root component
├── environments/
│   ├── environment.ts              # Development environment
│   └── environment.prod.ts         # Production environment
├── index.html                      # Main HTML file
└── styles.css                      # Global styles
```

## Usage

### Login/Registration
1. Navigate to the application URL
2. If not authenticated, you'll be redirected to the login page
3. Register a new account or login with existing credentials
4. Upon successful authentication, you'll be redirected to the contracts page

### Contract Management
1. **View Contracts**: The main page shows a paginated list of all contracts
2. **Create Contract**: Click "Create New Contract" to add a new contract
3. **Edit Contract**: Click the edit button on any contract in the list
4. **View Contract**: Click the view button to see contract details
5. **Delete Contract**: Click the delete button and confirm to remove a contract

### Navigation
- Use the breadcrumb navigation to move between pages
- The logout button is available in the top navigation

## Building for Production

Build the project for production:

```bash
npm run build
```

The build artifacts will be stored in the `dist/` directory.

## Error Handling

The application includes comprehensive error handling:
- Network errors are displayed to the user
- Form validation errors are shown inline
- Authentication errors redirect to login
- Loading states are shown during API calls

## Authentication Flow

1. User enters credentials
2. Application sends request to backend
3. Backend returns JWT token and user information
4. Token is stored in localStorage
5. Subsequent API requests include the token in headers
6. Token expiration is checked on app initialization

## Security Features

- JWT token authentication
- Route guards for protected pages
- Automatic token injection in HTTP requests
- Token expiration handling
- Secure logout functionality

## Browser Support

- Chrome (latest)
- Firefox (latest)
- Safari (latest)
- Edge (latest)

## Contributing

1. Follow Angular style guide
2. Use TypeScript strict mode
3. Include proper error handling
4. Write meaningful component and variable names
5. Follow the existing project structure

## License

This project is part of the Legal System and follows the same licensing terms.
