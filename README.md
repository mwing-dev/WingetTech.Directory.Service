# WingetTech.Directory.Service

A Windows-native .NET 10 Directory Service for LDAP operations on Windows Server 2025 Core.

## Overview

This solution provides a clean, enterprise-ready ASP.NET Core Web API service for interacting with LDAP directory services. Built specifically for Windows Server 2025 Core environments, it follows clean architecture principles with separated concerns across multiple projects.

## Solution Structure

```
WingetTech.Directory.Service/
├── WingetTech.Directory.Service.Api          # ASP.NET Core Web API
├── WingetTech.Directory.Service.Core         # Domain interfaces and models
├── WingetTech.Directory.Service.Infrastructure  # LDAP implementation
├── WingetTech.Directory.Service.Contracts    # Data transfer objects
└── WingetTech.Directory.Service.Tests        # Unit tests
```

## Features

- **Clean Architecture**: Separation of concerns with distinct layers
- **Strongly-Typed Configuration**: Type-safe directory service configuration
- **Health Checks**: Built-in health monitoring endpoints
- **Windows Service Ready**: Designed to run as a Windows Service
- **LDAP Support**: Ready for LDAP/Active Directory integration

## Requirements

- .NET 10.0 SDK
- Windows Server 2025 Core (or Windows 10/11 for development)
- LDAP/Active Directory server for directory operations

## Getting Started

### Building the Solution

```powershell
dotnet build
```

### Running the API

```powershell
cd WingetTech.Directory.Service.Api
dotnet run
```

The API will be available at `https://localhost:5001` (or the configured port).

### Configuration

Configure directory service settings in `appsettings.json`:

```json
{
  "Directory": {
    "Host": "ldap.example.com",
    "Port": 389,
    "UseSsl": false,
    "BaseDn": "dc=example,dc=com",
    "BindDn": "cn=admin,dc=example,dc=com",
    "BindPassword": "password"
  }
}
```

## API Endpoints

### Health Check

- **GET** `/health` - General health status
- **GET** `/api/health` - Detailed health information

## Project Details

### Core Project

Contains domain interfaces and configuration models:
- `IDirectoryService` - Main directory service interface
- `DirectoryOptions` - Strongly-typed configuration class

### Infrastructure Project

Implements LDAP operations:
- `LdapDirectoryService` - LDAP implementation of `IDirectoryService`

### Contracts Project

Data transfer objects for API communication:
- `UserDto` - User information
- `GroupDto` - Group information
- `AuthRequestDto` - Authentication request
- `AuthResponseDto` - Authentication response

### API Project

ASP.NET Core Web API with:
- Dependency injection configuration
- Health check endpoints
- Windows Service hosting support

## Development Status

🚧 **Work in Progress**: LDAP operations are stubbed and ready for implementation.

## License

[Specify your license here]

## Contributing

[Specify contribution guidelines here]
