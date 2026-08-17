# 🏥 MediCore

> **La gestión médica en un solo lugar.**

MediCore es una plataforma moderna de gestión médica orientada a consultorios, dispensarios y centros de salud. Centraliza procesos clínicos, farmacéuticos, de laboratorio y administrativos mediante una arquitectura desacoplada y preparada para crecer por módulos.

## 💡 Origen e inspiración

MediCore toma como **referencia funcional y académica** el proyecto grupal [DispensarioMedicoUnapec](https://github.com/JosesamuelPA/DispensarioMedicoUnapec), desarrollado como proyecto final para el dispensario médico de la Universidad APEC (UNAPEC).

Este repositorio **no es un fork ni una copia**. MediCore representa una evolución individual de aquella experiencia académica: la solución se reconstruye desde cero con identidad visual, experiencia de usuario, arquitectura y decisiones tecnológicas propias.

### 📜 Antecedente del enunciado académico

El proyecto académico original forma parte de una colección de proyectos desarrollados en la **Universidad APEC (UNAPEC)**, tomando como referencia enunciados y proyectos propuestos por el profesor **Juan Pablo Valdez Reyes**.

La base funcional utilizada para `DispensarioMedicoUnapec` proviene de un documento de **Proyecto Final de UNAPEC de 2015**, identificado en el material original como **Profesor: Juan P. Valdez**. Dicho documento plantea el desarrollo de un sistema para el Dispensario Médico de UNAPEC con gestión de tipos de fármacos, marcas, ubicaciones, medicamentos, pacientes, médicos, registro de visitas, consultas por criterios y reportes de visitas. La propuesta original especificaba su implementación con la versión vigente de **.NET Framework utilizando WinForms**.

MediCore conserva ese problema de negocio como **antecedente académico y funcional**, pero lo reinterpreta con una arquitectura moderna, frontend y backend desacoplados, tecnologías actuales y un alcance considerablemente ampliado.

## 👥 Equipo Académico Original

| 👤 Integrante | 🆔 Matrícula |
|---|---|
| 👩🏻‍💻 Zodelys Luciano Francisco | A00114484 |
| 👨🏻‍💻 José Samuel Peña Acevedo | A00107391 |
| 👨🏻‍💻 Francis Jairo Matías Rosario | A00115261 |
| 👩🏻‍💻 Emely Castillo Rivera | A00116415 |
| 👨🏻‍💻 Jeuel Ortiz Medrano | A00115584 |

> El equipo anterior corresponde al proyecto académico original **DispensarioMedicoUnapec**. **MediCore** es una reconstrucción y evolución independiente desarrollada por Francis Jairo Matías Rosario.

## 🎓 Información Académica

| Información | Detalle |
|---|---|
| 📖 **Asignatura** | Desarrollo de Software con Tecnología Propietaria 1 (ISO-605) |
| 👨‍🏫 **Profesor** | Ing. Omar Antonio De Jesus De La Cruz Gonzalez |
| 🏫 **Institución** | Universidad APEC (UNAPEC) |
| 📅 **Período académico** | Enero - Abril 2026 |
| 📁 **Tipo de entrega** | Proyecto Final |

## 🧩 Dominios

| Área | Alcance previsto |
|---|---|
| 🩺 **Consultorios** | Pacientes, médicos, citas, consultas e historia clínica |
| 💊 **Farmacia** | Medicamentos, tipos de fármacos, marcas, lotes, ubicaciones, inventario, kardex y vencimientos |
| 🧪 **Laboratorio** | Órdenes, estudios y resultados clínicos |
| ⚙️ **Administración** | Usuarios, roles, permisos, configuración, auditoría, dashboard y reportes |

## 🛠️ Stack Tecnológico

### 🔙 Backend

| Tecnología | Uso en MediCore |
|---|---|
| 🟣 **C#** | Lenguaje principal del backend |
| 💜 **.NET 10** | Plataforma de desarrollo |
| 🌐 **ASP.NET Core Web API** | API REST desacoplada |
| 🗃️ **Entity Framework Core** | ORM y acceso a datos |
| 🛢️ **Microsoft SQL Server** | Base de datos relacional |
| 📖 **OpenAPI** | Contrato y documentación de la API |
| ❤️ **ASP.NET Core Health Checks** | Liveness y readiness de servicios |

### 🎨 Frontend

| Tecnología | Uso en MediCore |
|---|---|
| ⚛️ **React 19** | Construcción de la interfaz web |
| 🔷 **TypeScript** | Tipado estático y mantenibilidad |
| ⚡ **Vite** | Tooling, desarrollo y build del frontend |

### 🧱 Arquitectura, calidad e infraestructura

| Tecnología / práctica | Uso en MediCore |
|---|---|
| 🧱 **Clean Architecture** | Separación de responsabilidades y dependencias |
| 🧭 **REST API** | Comunicación entre frontend y backend |
| 🐳 **Docker** | Contenedorización de servicios |
| 🐳 **Docker Compose** | Orquestación local de Web, API y SQL Server |
| 🔁 **GitHub Actions** | Integración continua |
| 🧪 **xUnit** | Pruebas unitarias, integración y arquitectura |
| 🔒 **Security by Design** | Base para autenticación, autorización y auditoría |
| 🌿 **Git / GitHub** | Control de versiones y flujo mediante ramas y Pull Requests |

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

### Backend

```bash
dotnet restore MediCore.slnx
dotnet build MediCore.slnx --configuration Release
dotnet test MediCore.slnx --configuration Release
```

### Frontend

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
- `docs/reference/DISPENSARIO_UNAPEC.md` — relación con el proyecto académico de referencia y procedencia del enunciado original.
- `docs/development/PHASE_0.md` — alcance y Definition of Done de la Fase 0.

## 👨‍💻 Desarrollo de MediCore

**Francis Jairo Matías Rosario**  
🆔 A00115261  
Software Developer · República Dominicana

---

**MediCore** — *La gestión médica en un solo lugar.*
