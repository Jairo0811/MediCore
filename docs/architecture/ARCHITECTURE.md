# Arquitectura de MediCore

## Enfoque

MediCore adopta Clean Architecture para mantener el dominio médico independiente de frameworks, persistencia e interfaz de usuario.

```text
MediCore.Web
    |
    | HTTP/JSON
    v
MediCore.Api
    |
    v
MediCore.Application
    |
    v
MediCore.Domain

MediCore.Infrastructure --> Application + Domain
MediCore.Api -----------> Infrastructure
```

## Capas

### MediCore.Domain

Contiene entidades, value objects, enums, reglas e invariantes del negocio. No debe depender de Entity Framework Core, ASP.NET Core, React ni detalles de infraestructura.

### MediCore.Application

Contiene casos de uso, contratos, DTOs, validación de aplicación y puertos que serán implementados por infraestructura.

### MediCore.Infrastructure

Implementará persistencia con SQL Server y Entity Framework Core, autenticación, almacenamiento y servicios externos. Los detalles concretos deben quedar encapsulados aquí.

### MediCore.Api

Expone la aplicación mediante HTTP. Se encargará de autenticación/autorización, endpoints, middleware, manejo uniforme de errores y documentación OpenAPI.

### MediCore.Web

SPA construida con React y TypeScript. No tendrá acceso directo a la base de datos; toda operación de negocio se realizará mediante la API.

## Dominios funcionales

1. Identity & Access
2. Patients
3. Medical Staff
4. Scheduling
5. Clinical Records
6. Pharmacy
7. Inventory
8. Laboratory
9. Reporting & Analytics
10. Administration & Audit

## Reglas arquitectónicas iniciales

- Domain no depende de ninguna otra capa del proyecto.
- Application solo puede depender de Domain.
- Infrastructure puede depender de Application y Domain.
- Api compone Application e Infrastructure.
- Web se comunica con Api por contratos HTTP.
- Las entidades de dominio no se exponen directamente como contratos de API.
- Toda operación sensible debe ser auditable.
- Los secretos nunca se versionan en Git.
