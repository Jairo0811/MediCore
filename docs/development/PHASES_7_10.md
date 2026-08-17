# Fases 7–10 — Cierre funcional de MediCore

## Fase 7 — Inventario, lotes, kardex y vencimientos

La Fase 7 separa el catálogo farmacéutico de la existencia física. Cada medicamento puede tener múltiples lotes por ubicación, costo, fecha de vencimiento, cantidad actual y punto de reposición.

### Implementado

- lotes por medicamento y ubicación;
- existencia inicial y costo unitario;
- punto de reposición;
- entradas, dispensaciones y ajustes;
- prevención de existencias negativas;
- kardex inmutable por lote;
- usuario responsable, referencia, notas y fecha UTC por movimiento;
- alertas de stock bajo;
- consulta de lotes próximos a vencer;
- RBAC con escritura exclusiva para `Administrator` y `Pharmacist`.

## Fase 8 — Laboratorio

### Implementado

- catálogo de pruebas de laboratorio;
- código, muestra, unidad y rango de referencia;
- órdenes por paciente y médico solicitante;
- múltiples pruebas por orden;
- registro de resultados y comentarios;
- usuario y fecha de validación del resultado;
- transición Pendiente → En proceso → Completada;
- RBAC diferenciado para solicitud médica y procesamiento de laboratorio.

## Fase 9 — Dashboard, analítica y reportes

### Implementado

- pacientes y personal activos;
- citas del día;
- consultas abiertas y actividad de 30 días;
- medicamentos activos;
- lotes con stock bajo;
- lotes próximos a vencer en 30 días;
- órdenes de laboratorio pendientes y completadas;
- feed de alertas de inventario;
- reporte operacional por rango de fechas con citas, consultas, laboratorio y movimientos de inventario.

Los indicadores se calculan desde las fuentes transaccionales de MediCore; no se mantienen contadores duplicados.

## Fase 10 — Hardening, observabilidad, auditoría, QA y v1.0.0

### Seguridad

- validación estricta de clave JWT en Production;
- bootstrap de administrador prohibido en Production;
- rate limiting para login, refresh, logout y bootstrap;
- headers `X-Content-Type-Options`, `X-Frame-Options`, `Referrer-Policy` y `Permissions-Policy`;
- RBAC validado en backend y navegación frontend alineada con permisos.

### Auditoría

- registro central de operaciones exitosas `POST`, `PUT`, `PATCH` y `DELETE`;
- usuario autenticado cuando está disponible;
- entidad, identificador, IP, fecha UTC, estado HTTP y correlation ID;
- consulta de logs restringida a `Administrator` y `Auditor`.

### Observabilidad

- `X-Correlation-ID` por solicitud;
- `TraceIdentifier` alineado con el correlation ID;
- logging estructurado de método, ruta, estado, latencia y correlation ID;
- health checks de liveness y readiness con estado de base de datos.

### Persistencia

MediCore v1.0.0 incorpora una migración EF Core inicial versionada y un `ModelSnapshot`. En desarrollo puede aplicarse automáticamente al iniciar; en Production la migración debe revisarse y aplicarse explícitamente antes de iniciar la API.

### QA

- unit tests de dominio para inventario y laboratorio;
- suite existente de pruebas unitarias, integración y arquitectura;
- type-check y build de React;
- CI de GitHub Actions para backend y frontend.

## API agregada

```text
/api/inventory/lots
/api/inventory/lots/{id}/kardex
/api/inventory/lots/{id}/movements
/api/laboratory/tests
/api/laboratory/orders
/api/laboratory/items/{id}/result
/api/analytics/dashboard
/api/analytics/inventory-alerts
/api/analytics/operational-report
/api/audit/logs
```

## Definition of Done

- Fases 0–10 integradas.
- Build backend y frontend en Release.
- Tests verdes.
- CI verde.
- Migración EF Core versionada.
- Documentación de despliegue y seguridad.
- Versión de API `1.0.0`.
