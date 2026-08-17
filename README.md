# MediCore

> **La gestión médica en un solo lugar.**

MediCore es una plataforma de gestión médica orientada a consultorios, dispensarios y centros de salud que centraliza pacientes, personal médico, consultas, farmacia, inventario, laboratorio, reportes y administración en una sola solución.

El proyecto toma como **referencia funcional y académica** el repositorio grupal [DispensarioMedicoUnapec](https://github.com/JosesamuelPA/DispensarioMedicoUnapec), pero MediCore será una implementación independiente, rediseñada desde cero con una arquitectura, identidad visual, experiencia de usuario y stack tecnológico propios.

## Objetivo

Transformar la idea de un sistema de dispensario médico en una plataforma moderna, modular, mantenible y extensible que pueda crecer desde un escenario académico hasta un producto de portafolio con estándares profesionales de desarrollo de software.

## Dominios principales

- **Consultorios:** pacientes, médicos, citas, consultas e historia clínica.
- **Farmacia:** medicamentos, tipos de fármacos, marcas, lotes, ubicaciones, inventario, movimientos y vencimientos.
- **Laboratorio:** órdenes, estudios y resultados clínicos.
- **Administración:** usuarios, roles, permisos, configuración, auditoría, dashboard y reportes.

## Referencia funcional inicial

El proyecto de referencia gestiona inventario médico, pacientes, visitas y reportes, e incluye áreas relacionadas con medicamentos, médicos, pacientes, marcas, tipos de fármacos, ubicaciones, estantes, usuarios y autenticación.

MediCore conservará las ideas de dominio que aporten valor, pero **no será una copia ni un fork**. La intención es reinterpretarlas con mejores separaciones de responsabilidades, una API dedicada y una interfaz web desacoplada.

## Stack tecnológico

### Backend

- C#
- .NET 10
- ASP.NET Core Web API
- Entity Framework Core
- Microsoft SQL Server
- JWT + Refresh Tokens
- RBAC (Role-Based Access Control)

### Frontend

- React
- TypeScript
- Vite
- TanStack Query
- React Router
- React Hook Form
- Zod

### Ingeniería e infraestructura

- Clean Architecture
- REST API
- Docker / Docker Compose
- GitHub Actions
- Pruebas unitarias, integración y arquitectura
- OpenAPI / Swagger

## Arquitectura objetivo

```text
MediCore/
├── src/
│   ├── MediCore.Domain/
│   ├── MediCore.Application/
│   ├── MediCore.Infrastructure/
│   ├── MediCore.Api/
│   └── MediCore.Web/
├── tests/
│   ├── MediCore.UnitTests/
│   ├── MediCore.IntegrationTests/
│   └── MediCore.ArchitectureTests/
├── docs/
├── branding/
└── .github/workflows/
```

## Roadmap

| Fase | Alcance | Estado |
|---|---|---|
| 0 | Foundation, arquitectura y entorno | 🚧 En preparación |
| 1 | Identidad, autenticación, roles y permisos | ⏳ Pendiente |
| 2 | Pacientes | ⏳ Pendiente |
| 3 | Médicos y personal | ⏳ Pendiente |
| 4 | Agenda y citas | ⏳ Pendiente |
| 5 | Consultas e historia clínica | ⏳ Pendiente |
| 6 | Farmacia y medicamentos | ⏳ Pendiente |
| 7 | Inventario, lotes, kardex y vencimientos | ⏳ Pendiente |
| 8 | Laboratorio | ⏳ Pendiente |
| 9 | Dashboard, analítica y reportes | ⏳ Pendiente |
| 10 | Hardening, CI/CD, documentación y v1.0.0 | ⏳ Pendiente |

## Principios del proyecto

- Clean Code
- SOLID
- DRY
- Separación de responsabilidades
- Seguridad por diseño
- Trazabilidad y auditoría
- Validación de datos
- Diseño responsive y accesible
- Evolución incremental mediante ramas y Pull Requests

## Estado

MediCore se encuentra actualmente en su etapa inicial de construcción. La primera iteración establecerá la arquitectura base, convenciones del repositorio y contratos fundamentales antes de comenzar los módulos funcionales.

## Autor

**Jairo Matías**  
Software Developer · República Dominicana

---

**MediCore** — *La gestión médica en un solo lugar.*
