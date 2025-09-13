# Angela BlazorServer Sentinel and StoreApp Integration

This document describes the new functionality added to the Angela BlazorServer for managing Sentinels and Store Apps.

## New Features

### 1. Sentinel Management
- **Page**: `/management/sentinels`
- **Description**: Manage Sentinel instances that provide app binaries and files
- **Features**:
  - List all available Sentinels
  - View Sentinel details (ID, URL, paths, alternative URLs)
  - Monitor Sentinel connectivity status

### 2. Store App Management  
- **Page**: `/management/store-apps`
- **Description**: Manage store applications available in the system
- **Features**:
  - Search store apps by name
  - View app details (name, type, developer, publisher)
  - Filter by public/private status
  - View app descriptions

## API Integration

The BlazorServer now includes:

1. **Angela Service Client** (`IAngelaService`): HTTP client for calling Angela API endpoints
2. **Enhanced Authentication**: Improved login with proper password hashing validation  
3. **Navigation**: New menu items for authenticated users to access management features

## Angela Service Enhancements

The Librarian.Angela service has been enhanced with new gRPC/HTTP endpoints:

### Sentinel Endpoints
- `GET /api/v1/sentinels` - List all sentinels
- `GET /api/v1/sentinels/{id}` - Get specific sentinel

### Store App Endpoints  
- `GET /api/v1/store-apps` - Search store apps (with optional name filter)
- `GET /api/v1/store-apps/{id}` - Get specific store app
- `GET /api/v1/store-apps/{id}/binaries` - List store app binaries

### Authentication Endpoints (Enhanced)
- `POST /api/v1/auth/token` - Login (now with proper password hashing)
- `POST /api/v1/auth/refresh` - Refresh token (admin-only access)

## Configuration

Add the following to `appsettings.json`:

```json
{
  "AngelaApi": {
    "BaseUrl": "http://localhost:5147"
  }
}
```

## Usage

1. Start the Angela/Sephirah server
2. Start the BlazorServer 
3. Login with admin credentials
4. Navigate to "Sentinels" or "Store Apps" in the menu
5. Manage your Sentinel instances and Store Apps

## Security

- All management pages require authentication
- Only admin users can access Angela endpoints
- Enhanced password verification with proper hashing
- JWT token-based authentication for API calls