# Fase 0 — Foundation

La Fase 0 establece la base técnica de MediCore antes de implementar módulos clínicos.

## Objetivos completados

- Solución .NET 10 separada en Domain, Application, Infrastructure y Api.
- Frontend React + TypeScript + Vite ejecutable.
- SQL Server integrado con Entity Framework Core.
- Health checks de proceso y base de datos.
- Docker Compose para SQL Server, API y frontend.
- Pruebas unitarias, de integración y de arquitectura.
- Pipeline de CI para backend y frontend.
- Configuración de CORS y OpenAPI.
- Convenciones de repositorio, `.editorconfig`, `global.json` y `.gitignore`.
- Documentación de arquitectura y procedencia funcional.

## Health checks

| Endpoint | Propósito |
|---|---|
| `GET /api/health/live` | Confirma que el proceso de la API responde. |
| `GET /api/health/ready` | Confirma que las dependencias requeridas, incluida SQL Server, están disponibles. |
| `GET /api/health` | Estado agregado de todos los checks registrados. |

## Ejecución con Docker

1. Copiar `.env.example` a `.env`.
2. Sustituir `MSSQL_SA_PASSWORD` por una contraseña local fuerte.
3. Ejecutar `docker compose up --build`.
4. Abrir `http://localhost:5173`.
5. API: `http://localhost:8080/api`.

## Definition of Done

La Fase 0 se considera terminada cuando:

- `dotnet build` finaliza sin errores;
- `dotnet test` finaliza correctamente;
- `npm run build` finaliza correctamente;
- GitHub Actions queda en verde;
- el PR de la fase puede fusionarse a `main`.
