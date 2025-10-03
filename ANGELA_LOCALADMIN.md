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

### Security Considerations
⚠️ **Important**: Only add IP addresses you fully trust to this list. These IPs will have full administrative access to Angela management functions.

## How It Works

1. **Authenticated Users**: If a user is already logged in, their existing permissions apply (no change in behavior)
2. **Trusted IP Users**: If a request comes from a trusted IP and no authentication is provided:
   - The system automatically creates a "LocalAdmin" identity
   - This identity has full admin privileges for Angela operations
   - The username displayed will be "LocalAdmin"

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

### Authorization Policy

The feature uses a custom authorization policy called `AngelaAccess`:
- Checks if user is authenticated via JWT token
- If not authenticated, checks if request IP is in the trusted list
- Grants access with "LocalAdmin" identity for trusted IPs

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
4. If behind a proxy, the actual IP might be different (check X-Forwarded-For header)

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
