# 🏥 MediCore

> **La gestión médica en un solo lugar.**

MediCore es una plataforma moderna de gestión médica orientada a consultorios, dispensarios y centros de salud. Centraliza procesos clínicos, farmacéuticos, de laboratorio y administrativos mediante una arquitectura desacoplada y preparada para crecer por módulos.

## 💡 Origen e inspiración

MediCore toma como **referencia funcional y académica** el proyecto grupal [DispensarioMedicoUnapec](https://github.com/JosesamuelPA/DispensarioMedicoUnapec), desarrollado para el dispensario médico de UNAPEC.

Este repositorio **no es un fork ni una copia**. La solución se reconstruye desde cero con identidad visual, experiencia de usuario, arquitectura y decisiones tecnológicas propias.

## 🧩 Dominios

| Área | Alcance previsto |
|---|---|
| 🩺 **Consultorios** | Pacientes, médicos, citas, consultas e historia clínica |
| 💊 **Farmacia** | Medicamentos, tipos de fármacos, marcas, lotes, ubicaciones, inventario, kardex y vencimientos |
| 🧪 **Laboratorio** | Órdenes, estudios y resultados clínicos |
| ⚙️ **Administración** | Usuarios, roles, permisos, configuración, auditoría, dashboard y reportes |

## 🛠️ Stack tecnológico

### Backend

- 🟣 **C# / .NET 10**
- 🌐 **ASP.NET Core Web API**
- 🗃️ **Entity Framework Core**
- 🛢️ **Microsoft SQL Server**
- 📖 **OpenAPI**
- ❤️ **ASP.NET Core Health Checks**

### Frontend

- ⚛️ **React 19**
- 🔷 **TypeScript**
- ⚡ **Vite**

### Ingeniería e infraestructura

- 🧱 **Clean Architecture**
- 🐳 **Docker / Docker Compose**
- 🔁 **GitHub Actions**
- 🧪 **xUnit**
- 🔒 **Seguridad por diseño**
- 🧭 **REST API**

## 🏗️ Arquitectura

```text
MediCore/
├── src/
│   ├── MediCore.Domain/
│   ├── MediCore.Application/
│   ├── MediCore.Infrastructure/
│   ├── MediCore.Api/
│   └── MediCore.Web/          # React + TypeScript + Vite
├── tests/
│   ├── MediCore.UnitTests/
│   ├── MediCore.IntegrationTests/
│   └── MediCore.ArchitectureTests/
├── docs/
├── branding/
└── .github/workflows/
```

La regla principal de dependencias es:

```text
Domain <- Application <- Infrastructure <- Api

React Web -------------------------------> Api
```

`Domain` no depende de infraestructura, persistencia ni presentación.

## 🚀 Inicio rápido con Docker

### 1. Crear configuración local

```bash
cp .env.example .env
```

En Windows PowerShell:

```powershell
Copy-Item .env.example .env
```

Cambia la contraseña de SQL Server dentro de `.env`.

### 2. Levantar la plataforma

```bash
docker compose up --build
```

Servicios locales:

- 🌐 Web: `http://localhost:5173`
- 🔌 API: `http://localhost:8080/api`
- ❤️ Liveness: `http://localhost:8080/api/health/live`
- 🩺 Readiness: `http://localhost:8080/api/health/ready`
- 🛢️ SQL Server: `localhost:1433`

## 🧪 Validación local

Backend:

```bash
dotnet restore MediCore.slnx
dotnet build MediCore.slnx --configuration Release
dotnet test MediCore.slnx --configuration Release
```

Frontend:

```bash
cd src/MediCore.Web
npm install
npm run build
npm run dev
```

## 🗺️ Roadmap

| Fase | Alcance | Estado |
|---|---|---|
| **0** | Foundation, arquitectura, React, SQL Server, Docker, pruebas y CI | ✅ Implementada |
| **1** | Identidad, autenticación, roles y permisos | ⏳ Pendiente |
| **2** | Pacientes | ⏳ Pendiente |
| **3** | Médicos y personal | ⏳ Pendiente |
| **4** | Agenda y citas | ⏳ Pendiente |
| **5** | Consultas e historia clínica | ⏳ Pendiente |
| **6** | Farmacia y medicamentos | ⏳ Pendiente |
| **7** | Inventario, lotes, kardex y vencimientos | ⏳ Pendiente |
| **8** | Laboratorio | ⏳ Pendiente |
| **9** | Dashboard, analítica y reportes | ⏳ Pendiente |
| **10** | Hardening, observabilidad, documentación y `v1.0.0` | ⏳ Pendiente |

## ✅ Principios

- Clean Code
- SOLID
- DRY
- Separación de responsabilidades
- Seguridad por diseño
- Trazabilidad y auditoría
- Validación de datos
- Accesibilidad y diseño responsive
- Desarrollo incremental mediante ramas y Pull Requests

## 📚 Documentación

- `docs/architecture/ARCHITECTURE.md` — decisiones de arquitectura.
- `docs/reference/DISPENSARIO_UNAPEC.md` — relación con el proyecto académico de referencia.
- `docs/development/PHASE_0.md` — alcance y Definition of Done de la Fase 0.

## 👨‍💻 Autor

**Jairo Matías**  
Software Developer · República Dominicana

---

**MediCore** — *La gestión médica en un solo lugar.*
