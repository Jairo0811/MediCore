# MediCore — Academic Final Edition

## Propósito

MediCore se cierra como proyecto académico de **Universidad APEC (UNAPEC)** para la asignatura **Desarrollo de Software con Tecnología Propietaria 1 (ISO-605)**, período **Enero–Abril 2026**. La meta de esta edición no es transformar el proyecto en un SaaS comercial, sino conservar un alcance académico completo, técnicamente demostrable y mantenible.

## Alcance congelado

La versión académica estable comprende las Fases 0–10:

1. Foundation y Clean Architecture.
2. Identity & Access.
3. Pacientes.
4. Médicos y personal.
5. Agenda y citas.
6. Consultas e historia clínica.
7. Farmacia y medicamentos.
8. Inventario, lotes, kardex y vencimientos.
9. Laboratorio.
10. Analítica, reportes, auditoría y hardening de producción.

No se incorporarán módulos comerciales o clínicos de gran alcance únicamente para aumentar el tamaño del repositorio. Integraciones como seguros, facturación, telemedicina, HL7/FHIR o receta electrónica quedan documentadas como posibles líneas futuras, no como requisitos de la entrega académica.

## Criterios de cierre

La Academic Final Edition se considera lista cuando:

- backend restaura, compila y supera sus pruebas automatizadas;
- frontend supera type-check y build de producción;
- existen pruebas unitarias/de componentes del frontend;
- existe un smoke test E2E de la experiencia de autenticación;
- el smoke test ejecuta una auditoría automatizada WCAG con axe y bloquea violaciones `serious` o `critical`;
- la interfaz dispone de skip link, foco visible, semántica de navegación y soporte de `prefers-reduced-motion`;
- las versiones visibles del producto y del paquete frontend son `1.0.0`;
- CI ejecuta backend y frontend QA en cada PR a `main`;
- la documentación de despliegue, seguridad, arquitectura y alcance académico permanece versionada en el repositorio.

## Estrategia de QA

### Backend

- xUnit para pruebas unitarias.
- pruebas de integración.
- pruebas de arquitectura.
- build Release en GitHub Actions.

### Frontend

- Vitest para pruebas unitarias y de componentes.
- React Testing Library para comportamiento observable.
- Playwright para smoke tests E2E.
- axe-core sobre Playwright para controles automáticos de accesibilidad.
- TypeScript estricto mediante `tsc --noEmit`.
- build Vite de producción en CI.

Las pruebas automáticas complementan, pero no sustituyen, una revisión manual de teclado, lectura de contenido, contraste y flujos por rol.

## Accesibilidad

La interfaz base incorpora:

- enlace para saltar directamente al contenido principal;
- indicadores de foco visibles;
- `aria-current` en navegación activa;
- regiones de estado con `aria-live`;
- iconos decorativos fuera del árbol accesible;
- atributos `autocomplete` en autenticación;
- reducción de animaciones cuando el sistema operativo solicita `prefers-reduced-motion`.

## Operación y seguridad

La edición académica conserva las decisiones de producción ya documentadas: secretos fuera del repositorio, JWT y refresh rotation, RBAC en backend, rate limiting, security headers, auditoría, health checks, migraciones EF Core controladas y bootstrap administrativo deshabilitado en Production.

## Política de mantenimiento

Después del cierre académico, `main` debe recibir únicamente:

- correcciones de defectos;
- actualizaciones de seguridad y dependencias;
- mejoras de documentación;
- ajustes de compatibilidad necesarios para mantener el proyecto ejecutable.

Las nuevas capacidades sustanciales deben justificarse académicamente antes de ampliar el alcance.
