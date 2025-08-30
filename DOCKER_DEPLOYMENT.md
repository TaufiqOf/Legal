# Legal System - Docker Deployment

This document describes how to deploy the Legal System using Docker and Docker Compose.

## Architecture

The Legal System consists of three main services:

- **Legal.Website** - Angular frontend application served by nginx
- **Legal.Api.WebApi** - .NET 9 Web API backend
- **Legal.Service.Migration** - Database migration service
- **PostgreSQL** - Database server

## Prerequisites

- Docker (20.10+)
- Docker Compose (2.0+)

## Quick Start

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd Legal
   ```

2. **Set environment variables** (optional)
   Create a `.env` file in the root directory:
   ```env
   # Database Configuration
   POSTGRES_DB=legaldb
   POSTGRES_USER=legaluser
   POSTGRES_PASSWORD=legalpass
   POSTGRES_PORT=5432
   
   # Application Ports
   API_PORT=4020
   WEB_PORT=8080
   
   # Storage
   STORAGE_PATH=./uploads
   ```

3. **Start the services**
   ```bash
   docker-compose up -d
   ```

4. **Access the application**
   - Frontend: http://localhost:8080
   - API: http://localhost:4020
   - API Documentation: http://localhost:4020/swagger

## Service Configuration

### Legal.Website (Frontend)

- **Port**: 8080 (configurable via WEB_PORT)
- **Technology**: Angular 20 + nginx
- **Build**: Multi-stage Docker build
- **Routing**: SPA routing handled by nginx

#### Environment Configuration

The frontend automatically configures the API URL based on the environment:

- **Development**: http://localhost:4020/api
- **Production**: Dynamic based on current host
- **Docker**: http://localhost:4020/api

### Legal.Api.WebApi (Backend)

- **Port**: 4020 (configurable via API_PORT)
- **Technology**: .NET 9 Web API
- **Database**: PostgreSQL
- **Features**: 
  - JWT Authentication
  - Swagger Documentation
  - CORS Configuration

### Legal.Service.Migration (Database)

- **Purpose**: Database schema migrations
- **Technology**: .NET 9 Console Application
- **Execution**: Runs automatically on startup
- **Configuration**: Environment variables

### PostgreSQL Database

- **Port**: 5432 (configurable via POSTGRES_PORT)
- **Version**: PostgreSQL 16 Alpine
- **Data Persistence**: Docker volume
- **Health Checks**: Enabled

## Building Individual Services

### Build Frontend
```bash
# Build the Angular application
docker build -t legal-website -f Website/Legal.Website/Dockerfile .

# Run standalone
docker run -p 8080:80 legal-website
```

### Build Backend API
```bash
# Build the .NET API
docker build -t legal-api -f Api/Legal.Api.WebApi/Dockerfile .

# Run with environment variables
docker run -p 4020:4020 -e "ConnectionStrings__Postgres=Host=localhost;Database=legaldb;Username=user;Password=pass" legal-api
```

### Build Migration Service
```bash
# Build the migration service
docker build -t legal-migration -f OtherServices/MigrationService/Dockerfile .

# Run migration
docker run --rm -e "ConnectionStrings__Postgres=Host=localhost;Database=legaldb;Username=user;Password=pass" legal-migration
```

## Development Workflow

### Local Development with Docker

1. **Start database only**
   ```bash
   docker-compose up postgres -d
   ```

2. **Run frontend locally**
   ```bash
   cd Website/Legal.Website
   npm install
   npm start
   ```

3. **Run backend locally**
   ```bash
   cd Api/Legal.Api.WebApi
   dotnet run
   ```

### Hot Reload Development

For development with hot reload:

```bash
# Start all services in development mode
docker-compose -f docker-compose.yml -f docker-compose.dev.yml up
```

## Production Deployment

### Environment Variables

For production, set these environment variables:

```env
# Database (use strong passwords)
POSTGRES_PASSWORD=<strong-password>
POSTGRES_USER=<production-user>

# Application
ASPNETCORE_ENVIRONMENT=Production
NODE_ENV=production

# Ports (if different from defaults)
API_PORT=80
WEB_PORT=443

# SSL (if using HTTPS)
ASPNETCORE_HTTPS_PORTS=443
```

### SSL/HTTPS Configuration

For production with HTTPS:

1. **Update nginx configuration** to handle SSL
2. **Mount SSL certificates** into the nginx container
3. **Configure ASPNETCORE_HTTPS_PORTS** for the API

### Persistent Data

Database data is stored in a Docker volume named `legal-postgres-data`. To backup:

```bash
# Backup database
docker exec legal-postgres pg_dump -U legaluser legaldb > backup.sql

# Restore database
docker exec -i legal-postgres psql -U legaluser legaldb < backup.sql
```

## Monitoring and Logging

### View Logs

```bash
# All services
docker-compose logs -f

# Specific service
docker-compose logs -f Legal.api.webapi
docker-compose logs -f Legal.website
docker-compose logs -f postgres
```

### Health Checks

The PostgreSQL service includes health checks. Monitor with:

```bash
docker-compose ps
```

### Application Monitoring

- **Frontend**: nginx access logs
- **Backend**: ASP.NET Core structured logging
- **Database**: PostgreSQL logs

## Troubleshooting

### Common Issues

1. **Port Conflicts**
   ```bash
   # Check if ports are in use
   netstat -an | grep :8080
   netstat -an | grep :4020
   
   # Use different ports
   WEB_PORT=8081 API_PORT=4021 docker-compose up
   ```

2. **Database Connection Issues**
   ```bash
   # Check database health
   docker-compose exec postgres pg_isready -U legaluser -d legaldb
   
   # View database logs
   docker-compose logs postgres
   ```

3. **Migration Failures**
   ```bash
   # Run migration manually
   docker-compose run --rm Legal.service.migration
   
   # Check migration logs
   docker-compose logs Legal.service.migration
   ```

4. **Frontend Build Issues**
   ```bash
   # Rebuild frontend with no cache
   docker-compose build --no-cache Legal.website
   ```

### Cleanup

```bash
# Stop and remove containers
docker-compose down

# Remove volumes (WARNING: This deletes all data)
docker-compose down -v

# Remove images
docker-compose down --rmi all

# Complete cleanup
docker system prune -a
```

## Security Considerations

### Database Security

- Use strong passwords for database users
- Limit database access to application services only
- Regular security updates for PostgreSQL image

### Application Security

- Configure CORS properly for production
- Use HTTPS in production
- Implement proper JWT token management
- Regular security updates for .NET and Node.js images

### Network Security

- Use Docker networks to isolate services
- Expose only necessary ports
- Consider using a reverse proxy for production

## Scaling

### Horizontal Scaling

The architecture supports horizontal scaling:

```yaml
# Scale frontend
docker-compose up --scale Legal.website=3

# Scale backend (requires load balancer)
docker-compose up --scale Legal.api.webapi=2
```

### Load Balancing

For production scaling, consider:

- nginx as load balancer for multiple backend instances
- Database connection pooling
- Redis for session management
- CDN for static assets

## CI/CD Integration

### GitHub Actions Example

```yaml
name: Docker Build and Deploy

on:
  push:
    branches: [ main ]

jobs:
  build:
    runs-on: ubuntu-latest
    steps:
    - uses: actions/checkout@v2
    
    - name: Build and push Docker images
      run: |
        docker-compose build
        docker-compose push
    
    - name: Deploy to production
      run: |
        docker-compose -f docker-compose.prod.yml up -d
```

### Azure DevOps Example

```yaml
trigger:
- main

pool:
  vmImage: 'ubuntu-latest'

steps:
- task: DockerCompose@0
  displayName: 'Build Docker Images'
  inputs:
    dockerComposeFile: 'docker-compose.yml'
    action: 'Build services'

- task: DockerCompose@0
  displayName: 'Deploy Services'
  inputs:
    dockerComposeFile: 'docker-compose.yml'
    action: 'Run services'
```

## Support

For issues and questions:

1. Check the logs: `docker-compose logs`
2. Verify service health: `docker-compose ps`
3. Review environment variables
4. Check Docker and Docker Compose versions

## License
MIT License © Taufiq Abdur Rahman
You may not use this codebase without permission. For Evolution purposes only.

