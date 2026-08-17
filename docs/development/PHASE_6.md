# Fase 6 — Farmacia y medicamentos

## Objetivo

Recuperar y modernizar el núcleo farmacéutico del Dispensario Médico, separando el catálogo maestro de medicamentos de las existencias físicas que serán responsabilidad de la Fase 7.

## Alcance implementado

- Tipos de fármacos.
- Marcas/laboratorios farmacéuticos.
- Ubicaciones de almacenamiento.
- Medicamentos con código único, nombre comercial, nombre genérico, principio activo, concentración, forma farmacéutica y unidad de medida.
- Asociación con tipo de fármaco, marca y ubicación.
- Indicadores de receta requerida y sustancia controlada.
- Búsqueda por código, nombre, nombre genérico o principio activo.
- Filtrado por tipo y estado.
- Baja lógica.
- Integridad: no se desactiva un catálogo mientras existan medicamentos activos que lo utilicen.
- RBAC: lectura para perfiles clínicos autorizados; escritura para `Administrator` y `Pharmacist`.

## Frontera con Fase 7

La Fase 6 define **qué medicamento existe y cómo se clasifica**. No almacena cantidades, lotes, fechas de vencimiento, costos, movimientos ni kardex. Esos conceptos pertenecen a la Fase 7.

## API

`/api/pharmacy/drug-types`, `/api/pharmacy/brands`, `/api/pharmacy/locations` y `/api/pharmacy/medications` exponen operaciones de consulta y mantenimiento según rol.

## Persistencia local

El entorno todavía usa `EnsureCreated`. Un volumen Docker creado antes de esta fase no incorporará las nuevas tablas automáticamente. Para probar esta fase sobre un entorno de desarrollo existente:

```powershell
docker compose down -v
docker compose up --build
```

La estrategia de migraciones versionadas se incorporará antes de producción.

## Definition of Done

- Dominio de farmacia separado de inventario.
- Configuración EF Core con índices y relaciones restrictivas.
- API protegida por roles.
- Validaciones de unicidad y referencias activas.
- Baja lógica e integridad referencial.
- Prueba unitaria del ciclo de vida del medicamento.
- CI verde.
