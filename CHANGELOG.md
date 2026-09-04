# Changelog

Todas las modificaciones relevantes de MediCore se documentan en este archivo.

## [1.0.0] - 2026-08-17

### Añadido

- Autenticación JWT, refresh tokens, roles y bootstrap inicial controlado.
- Gestión de pacientes con expediente y validación de cédula dominicana.
- Directorio de médicos y personal.
- Agenda, citas y prevención de solapamientos.
- Consultas, signos vitales e historia clínica.
- Catálogo de farmacia: tipos, marcas, ubicaciones y medicamentos.
- Inventario por lotes, vencimientos, stock mínimo, movimientos y kardex.
- Laboratorio: catálogo de pruebas, órdenes y resultados.
- Dashboard, alertas y reporte operacional por rango de fechas.
- Auditoría central de operaciones de escritura.
- Correlation IDs, health checks, logging estructurado y security headers.
- Migración EF Core inicial versionada.
- UI React modular y navegación basada en roles.
- Docker Compose, CI de GitHub Actions y documentación de producción.

### Seguridad

- Rate limiting en endpoints de autenticación.
- Bootstrap deshabilitado obligatoriamente en Production.
- Validación estricta de secretos JWT para Production.
- RBAC aplicado en backend para cada dominio funcional.

### Academic Final Edition — 2026-09-04

- Versión del paquete React alineada con MediCore `1.0.0`.
- QA frontend incorporado con Vitest y React Testing Library.
- Smoke tests E2E incorporados con Playwright.
- Auditoría automatizada de accesibilidad mediante axe-core.
- Skip link, foco visible, navegación semántica, regiones de estado y soporte de reducción de movimiento.
- CI ampliado para ejecutar type-check, unit/component tests, build, Playwright y axe.
- Documento de cierre académico y guía específica de QA frontend.
