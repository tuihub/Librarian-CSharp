# Angela LocalAdmin Access Feature

## Overview

This feature allows configuring trusted IP addresses that can access Angela management functions without authentication. When a request comes from a trusted IP, the system automatically grants "LocalAdmin" privileges.

## Configuration

Add trusted IP addresses to the `SystemConfig` section in your `appsettings.json`:

```json
{
  "SystemConfig": {
    // ... other config ...
    "AngelaTrustedIPs": ["127.0.0.1", "::1", "192.168.1.100"]
  }
}
```

### IP Address Format
- IPv4: `127.0.0.1`, `192.168.1.100`
- IPv6: `::1`, `2001:db8::1`

### Proxy Support
The feature automatically handles X-Forwarded-For headers when the server is behind a proxy or load balancer. The original client IP will be used for trusted IP checking.

### Security Considerations
⚠️ **Important**: Only add IP addresses you fully trust to this list. These IPs will have full administrative access to Angela management functions.

## How It Works

1. **Authenticated Users**: If a user is already logged in, their existing permissions apply (no change in behavior)
2. **Trusted IP Users**: If a request comes from a trusted IP and no authentication is provided:
   - The system automatically creates a "LocalAdmin" identity
   - This identity has full admin privileges for Angela operations
   - The username displayed will be "LocalAdmin"
   - A visual indicator (warning badge) appears in the navigation menu and admin page

## Visual Indicators

When accessing as LocalAdmin via trusted IP:

- **Navigation Menu**: A warning badge appears showing "⚠️ LocalAdmin Mode - Trusted IP Access"
- **Angela Admin Page**: A prominent warning alert displays the authentication type and user identity
- **User Display**: Shows "LocalAdmin" with a "Trusted IP" badge in system information
- **Automatic Detection**: The BlazorServer calls Angela's `CheckLocalAdmin` endpoint to detect if the current request is from a trusted IP

This provides immediate visual feedback so users can easily identify their authentication method. The detection is performed by calling the Angela backend service, which checks the actual request IP against the configured trusted IPs list.

## Angela Management Features

Once authenticated (either via login or trusted IP), users can access:

- **Sentinel Management**: Manage Sentinel instances that provide app binaries
- **Store App Management**: Manage store applications in the system
- **User Management**: Create and manage users
- **Porter Management**: View and manage Porter instances

## Navigation

In the Librarian.Angela.BlazorServer, the Angela Admin section is accessible via:
- URL: `/management/angela`
- Navigation Menu: "Angela Admin" link (visible to authenticated users)

## Implementation Details

### Authorization Policy and Authentication State

The feature uses a custom authorization policy called `AngelaAccess` on the backend:
- Checks if user is authenticated via JWT token
- If not authenticated, checks if request IP is in the trusted list
- Grants access with "LocalAdmin" identity for trusted IPs

The BlazorServer uses an enhanced `JwtAuthenticationStateProvider`:
- When no JWT token is present, it calls the `CheckLocalAdmin` endpoint
- If the request is from a trusted IP, creates an authenticated state with "LocalAdmin" identity
- This integrates LocalAdmin detection into Blazor's built-in authorization system
- The `AuthorizeView` component automatically recognizes LocalAdmin users without manual checks

### LocalAdmin Detection Service

A dedicated `CheckLocalAdmin` RPC endpoint is provided:
- **Endpoint**: `GET /api/v1/auth/check-local-admin`
- **Purpose**: Allows BlazorServer to detect if current request is from trusted IP
- **Response**: Returns `isLocalAdmin` boolean and username
- **Integration**: Called by `JwtAuthenticationStateProvider` during authentication state resolution

The unified approach means:
- No manual LocalAdmin checks needed in components
- `@attribute [Authorize]` works automatically for LocalAdmin users
- `<AuthorizeView>` components correctly show/hide content for LocalAdmin
- Authentication state is centrally managed by the framework

### Modified Services

All Angela management services now use the `AngelaAccess` policy:
- Sentinel services (ListSentinels, GetSentinel)
- StoreApp services (SearchStoreApps, GetStoreApp, ListStoreAppBinaries)
- User management (CreateUser, UpdateUser, ListUsers)
- Porter management (ListPorters)
- Gebura services (SearchAppInfos)

## Example Use Cases

1. **Local Development**: Add `127.0.0.1` and `::1` for localhost access
2. **Internal Network**: Add your internal network IPs for admin access from office
3. **VPN Access**: Add VPN IP range for remote admin access

## Troubleshooting

### Access Denied Even From Trusted IP

1. Verify the IP address format in configuration matches exactly
2. Check that the application has restarted after configuration change
3. Verify the actual remote IP being sent by checking logs
4. If behind a proxy, ensure the proxy is forwarding the X-Forwarded-For header
5. The handler automatically checks X-Forwarded-For, but ensure your proxy is configured correctly

### LocalAdmin Not Appearing

The "LocalAdmin" username will only appear when:
- Request is from a trusted IP
- User is NOT already authenticated
- Authorization succeeds for the Angela service

## Migration Notes

For existing deployments:
- The `AngelaTrustedIPs` array defaults to empty, so existing behavior is preserved
- Add IPs as needed for your deployment scenario
- No database migrations required
