# Despliegue de MediCore v1.0.0

## Principios

Production no debe depender de valores de desarrollo, bootstrap inicial abierto ni creación automática del esquema. El despliegue se divide en preparación de infraestructura, migración controlada, despliegue de API/Web y validación post-deploy.

## Variables mínimas

```text
ASPNETCORE_ENVIRONMENT=Production
ConnectionStrings__DefaultConnection=<secret>
Jwt__SigningKey=<secret aleatorio de 32+ caracteres>
Jwt__Issuer=MediCore
Jwt__Audience=MediCore.Web
Auth__AllowBootstrapAdmin=false
Database__InitializeOnStartup=false
Cors__AllowedOrigins__0=https://medicore.example.com
```

No almacenar secretos reales en el repositorio.

## Base de datos

Restaurar las herramientas locales:

```bash
dotnet tool restore
```

Revisar migraciones pendientes:

```bash
dotnet ef migrations list --project src/MediCore.Infrastructure --startup-project src/MediCore.Api
```

Generar un script idempotente para revisión/aprobación:

```bash
dotnet ef migrations script --idempotent --project src/MediCore.Infrastructure --startup-project src/MediCore.Api --output artifacts/sql/medicore-v1.0.0.sql
```

Aplicar el script con una identidad SQL de despliegue con privilegios acotados. La cuenta de ejecución diaria de MediCore no necesita privilegios para alterar el esquema.

## Orden de despliegue

1. Backup verificado de SQL Server.
2. Aplicar migración revisada.
3. Desplegar API con variables Production.
4. Desplegar Web con `VITE_API_BASE_URL` apuntando a la API pública.
5. Validar `/api/health/live`.
6. Validar `/api/health/ready`.
7. Ejecutar smoke tests autenticados.
8. Revisar logs y latencias antes de abrir tráfico completo.

## Reverse proxy y TLS

Publicar únicamente HTTPS mediante IIS, Nginx, Azure App Service u otro reverse proxy administrado. El puerto de SQL Server no debe exponerse públicamente.

## Backups

Definir backup completo, diferencial/log según RPO/RTO, cifrado en reposo, retención y prueba periódica de restauración. Un backup no probado no se considera recuperable.

## Rollback

- conservar imagen/artefacto de la versión anterior;
- no revertir una migración destructiva sin un plan de datos;
- ante un fallo aplicativo, retirar tráfico, restaurar aplicación previa y evaluar compatibilidad de esquema;
- documentar incidentes con correlation IDs.

## Bootstrap inicial

`Auth:AllowBootstrapAdmin` debe permanecer `false` en Production. La cuenta administrativa inicial debe prepararse en un entorno controlado antes de exponer la aplicación o mediante un procedimiento administrativo explícito.
