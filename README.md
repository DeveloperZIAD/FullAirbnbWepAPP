# Airbnb Clone Application

A professional-grade full-stack Airbnb clone application, featuring a robust .NET 8 backend, a modern React frontend, and a containerized infrastructure designed for production-ready deployment.

## Tech Stack

- **Backend:** .NET 8, ASP.NET Core Web API, SQL Server (Azure SQL Edge)
- **Frontend:** React, Axios
- **Infrastructure:** Docker, Docker Compose
- **CI/CD:** GitHub Actions (Automated Builds & Container Registry)
- **Deployment:** Railway

## Key Features

- **Production-Ready Architecture:** Multi-stage Docker builds for optimized performance and security.
- **Database Persistence:** Managed SQL data volumes with SQL Server.
- **Security First:**
  - Implemented **HSTS** and **HTTPS** redirection.
  - Custom **Security Headers Middleware** (XSS, Clickjacking, MIME sniffing protection).
  - **Rate Limiting** to prevent brute-force and DDoS attacks.
  - **Strict CORS Policy** configured dynamically for environment-specific origins.
- **Real-time Communication:** Built-in SignalR hub for real-time messaging.

## CI/CD Pipeline

The project utilizes GitHub Actions to automate the build and push process:

1. **Login:** Authenticates with GitHub Container Registry (GHCR).
2. **Build:** Efficient multi-stage builds with layer caching for speed.
3. **Push:** Pushes optimized images to the registry.
4. **Deploy:** Ready for automated deployment via Railway.

## Local Setup

1. Clone the repository.
2. Run the services using Docker:
   ```bash
   docker-compose up -d
   ```
