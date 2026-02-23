# Feature Completion Matrix

This document tracks the implementation status of every feature and module across the WingetTech.Directory.Service solution.

## Status Key

| Symbol | Meaning |
|--------|---------|
| ✅ | Complete |
| ⚠️ | Partially Implemented |
| 🔲 | Designed / Scaffolded only |
| ❌ | Missing |

---

## Feature Completion Matrix

| Feature | Designed | Partially Implemented | Complete | Missing | Notes |
|---------|:--------:|:---------------------:|:--------:|:-------:|-------|
| **Diagnostic** | | | ✅ | | `GET /api/diagnostics/health` — live LDAP health probe via `IDirectoryService.HealthCheckAsync` |
| **Health Check** | | | ✅ | | Both `GET /health` (ASP.NET built-in) and `GET /api/health` (controller) are wired up |
| **Test Bind** | | | ✅ | | `POST /api/diagnostics/test-bind` validates LDAP service account credentials via `IAuthenticationProbe.TestBindAsync` |
| **Settings** | | | ✅ | | `GET/POST /api/settings/directory` — full CRUD for LDAP config persisted to SQLite via `DirectorySettingsService` |
| **Groups** | | | ✅ | | `GET /api/groups/{identifier}` (by GUID or DN) and `GET /api/groups/search` — backed by `LdapDirectoryService` |
| **User Lookup** | | | ✅ | | `GET /api/users/{id}`, `/by-username/{username}`, `/search`, `/{id}/groups` — all backed by `LdapDirectoryService` |
| **JWT (token issuance)** | | | ✅ | | `AuthService` generates signed HMAC-SHA256 JWT access tokens and persisted refresh tokens stored in SQLite |
| **JWT (validation middleware)** | | ⚠️ | | | `AddAuthentication` / `AddJwtBearer` not registered in `Program.cs`; no `[Authorize]` attributes on controllers — issued tokens are not validated on inbound requests |
| **Encryption at rest** | | ⚠️ | | | `IEncryptionService` interface is defined; `PlainTextEncryptionService` is a no-op stub — bind passwords are stored unencrypted |
| **MFA** | | | | ❌ | No MFA interface, model, or implementation exists anywhere in the solution |
| **Multi-tenant scaffolding** | | | | ❌ | All directory operations use a single global `DirectorySettings` record; no tenant identifier, tenant resolver, or per-tenant configuration exists |
| **Unit / Integration Tests** | | | | ❌ | `LdapDirectoryServiceTests` class exists but contains no test methods; xUnit references pending .NET 10 GA support |
| **Organizational Units** | | | ✅ | | `GET /api/organizational-units` — look up OUs by DN via `LdapDirectoryService.GetOrganizationalUnitAsync` |

---

## Implementation Detail by Feature

### Diagnostic
- **Controller**: `DiagnosticsController` (`/api/diagnostics`)
- **Service**: `IDirectoryService.HealthCheckAsync` → `LdapDirectoryService` opens an LDAP connection, performs a bind, returns success/failure
- **Status**: Complete and wired end-to-end

### Health Check
- **Endpoints**: `GET /health` (ASP.NET `MapHealthChecks`) and `GET /api/health` (`HealthController`)
- **Status**: Complete — both paths return live status

### Test Bind
- **Endpoint**: `POST /api/diagnostics/test-bind`
- **Service**: `IAuthenticationProbe.TestBindAsync` → `LdapAuthenticationProbe` attempts a service-account bind
- **Status**: Complete — errors are caught and surfaced in the response body

### Settings
- **Endpoints**: `GET /api/settings/directory`, `POST /api/settings/directory`
- **Persistence**: SQLite via `AppDbContext` / `DirectorySettingsService`
- **Gap**: Bind password stored via `PlainTextEncryptionService` (no encryption); see Encryption row above

### Groups
- **Endpoints**: `GET /api/groups/{identifier}`, `GET /api/groups/search?q=`
- **Implementation**: Full LDAP filter construction, attribute mapping, member enumeration
- **Status**: Complete

### User Lookup
- **Endpoints**: `GET /api/users/{id}`, `GET /api/users/by-username/{username}`, `GET /api/users/search?q=`, `GET /api/users/{id}/groups`
- **Implementation**: Full LDAP queries with `objectGUID`/`sAMAccountName` filters, `userAccountControl` enabled-flag decoding, group membership via `memberOf`
- **Status**: Complete

### JWT
- **Token issuance** (`AuthService`): Signs access tokens (HMAC-SHA256), generates cryptographically random refresh tokens, stores refresh tokens in SQLite with expiry and revocation flag
- **Token validation**: ❌ Not wired — `Program.cs` does not call `AddAuthentication().AddJwtBearer()` and no `[Authorize]` attribute protects any endpoint
- **Configuration**: `JwtOptions` (secret, issuer, audience, access/refresh expiry) consumed from `appsettings.json`

### MFA
- No interface, DTO, entity, or implementation exists
- Not planned in the current architecture

### Multi-tenant Scaffolding
- All settings, LDAP connections, and token storage are global (single tenant)
- No tenant header resolution, tenant context, or per-tenant `DirectorySettings` row exists

### Encryption at Rest
- `IEncryptionService` contract is defined with `Encrypt` / `Decrypt`
- `PlainTextEncryptionService` passes values through unmodified with a `WARNING` comment in source
- Bind passwords and any future secrets stored in SQLite are unprotected

### Tests
- `WingetTech.Directory.Service.Tests` project is scaffolded
- `LdapDirectoryServiceTests` class has no test methods
- xUnit packages are pending .NET 10 availability

### Organizational Units
- **Endpoint**: `GET /api/organizational-units`
- **Service**: `IDirectoryService.GetOrganizationalUnitAsync` → `LdapDirectoryService` queries by distinguished name
- **Status**: Complete

---

## Pre-Production Checklists

### Internal Production

The following must be completed before deploying the service in any internal production environment:

- [ ] **JWT validation middleware**: Register `AddAuthentication().AddJwtBearer(...)` in `Program.cs` and apply `[Authorize]` to all non-auth endpoints
- [ ] **Real encryption**: Replace `PlainTextEncryptionService` with a proper implementation (e.g. AES-256-GCM using `System.Security.Cryptography` or Windows DPAPI)
- [ ] **HTTPS enforcement**: Verify TLS certificate is configured for the hosting environment (currently `UseHttpsRedirection()` is in place but requires a cert)
- [ ] **Refresh token cleanup**: Add a background job or scheduled task to purge expired/revoked refresh tokens from SQLite
- [ ] **Secrets management**: Move `Jwt:Secret` and bind credentials out of `appsettings.json` and into environment variables, Azure Key Vault, or Windows Credential Manager
- [ ] **Input validation**: Add `[Required]` / `[MaxLength]` data annotations and model-state validation to all request DTOs
- [ ] **Error handling**: Add a global exception handler middleware so unhandled exceptions do not leak stack traces
- [ ] **Logging**: Integrate structured logging (Serilog or built-in ILogger) with appropriate log levels and sinks
- [ ] **Minimum unit tests**: Write tests covering `AuthService`, `LdapAuthenticationProbe`, and `DirectorySettingsService` at minimum

---

### External SaaS

All Internal Production items must be done, plus:

- [ ] **Rate limiting**: Apply `AddRateLimiter` to auth endpoints to prevent brute-force attacks
- [ ] **API key / tenant authentication**: Issue API keys or client credentials per customer organisation
- [ ] **Audit logging**: Record every authentication attempt, settings change, and directory query with the acting identity and timestamp
- [ ] **TLS mutual authentication or token scoping**: Ensure tokens are scoped to the issuing tenant and cannot be replayed across tenants
- [ ] **Health check authentication**: Protect `/api/diagnostics/health` and `/api/diagnostics/test-bind` so they are not publicly accessible
- [ ] **OpenAPI security definitions**: Add bearer-token security scheme to the Scalar / OpenAPI spec
- [ ] **SLA-grade error responses**: Standardise error response bodies (RFC 7807 Problem Details)
- [ ] **Integration test suite**: Cover every public API endpoint with automated integration tests

---

### Multi-tenant Support

All Internal Production and External SaaS items must be done, plus:

- [ ] **Tenant identifier model**: Define a `Tenant` entity with a stable ID (GUID), display name, and per-tenant `DirectorySettings` foreign key
- [ ] **Tenant resolution middleware**: Extract tenant from subdomain, `X-Tenant-Id` header, or JWT claim on every request
- [ ] **Per-tenant LDAP settings**: Extend `DirectorySettingsService` to scope reads/writes by tenant ID
- [ ] **Per-tenant token storage**: Scope `RefreshToken` rows to a tenant so tokens cannot be used across tenants
- [ ] **Tenant provisioning API**: Endpoints to create, update, and deactivate tenants (admin only)
- [ ] **Tenant isolation tests**: Verify that one tenant cannot access another tenant's users, groups, or settings
- [ ] **Database migration strategy**: Move from `EnsureCreated()` to EF Core Migrations to support schema evolution across tenants
- [ ] **Connection pooling per tenant**: Manage LDAP connection lifecycle so one tenant's directory latency does not affect others
