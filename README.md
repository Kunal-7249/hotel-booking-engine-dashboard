# Hotel Booking Engine

A full-stack hotel booking platform with role-based access, automated CI/CD,
and structured logging. Built as a personal project to practice clean
architecture and end-to-end deployment.

**Live demo:** https://hotel-booking-engine-dashboard.vercel.app

## Tech Stack

- **Backend:** ASP.NET Core Web API, C#
- **Frontend:** Angular
- **Database:** SQL Server
- **Hosting:** Azure App Service (backend), Vercel (frontend)
- **CI/CD:** GitHub Actions

## Features

- Role-based access control for Admin and Customer users
- JWT-based authentication and authorization
- Clean Architecture (separation of API, application, domain, and
  infrastructure layers)
- Structured logging with Serilog
- Unit tests for both backend and frontend
- Timer-triggered Azure Function to keep the free-tier backend warm and
  avoid cold-start latency

## Architecture

The solution follows Clean Architecture principles, keeping business logic
independent of frameworks and infrastructure.

CI/CD is handled by three GitHub Actions workflows:
1. **Backend** — builds, tests, and deploys the API to Azure App Service
2. **Frontend** — builds and deploys the Angular app to Vercel
3. **Warmup function** — deploys a timer-triggered Azure Function that pings
   the backend on a schedule to prevent cold starts on the free tier

## Getting Started

### Prerequisites
- .NET SDK (8.0 or later)
- Node.js and npm
- SQL Server

### Backend
```bash
cd <backend-folder>
dotnet restore
dotnet run
