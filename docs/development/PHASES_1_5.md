# MediCore — Fases 1 a 5

Este incremento construye el núcleo clínico de MediCore sobre la fundación técnica de la Fase 0.

## Fase 1 — Identidad y acceso

Implementado:

- ASP.NET Core Identity con claves `Guid`;
- usuarios activos/inactivos;
- roles iniciales: Administrator, Doctor, Nurse, Receptionist, Pharmacist, Laboratory y Auditor;
- autenticación JWT;
- access tokens de corta duración;
- refresh tokens aleatorios almacenados únicamente como SHA-256;
- rotación y revocación de refresh tokens;
- bloqueo temporal por intentos fallidos;
- bootstrap del primer administrador controlado por configuración;
- creación de usuarios restringida al rol Administrator;
- endpoint de usuario actual.

Endpoints principales:

```text
POST /api/auth/bootstrap-admin
POST /api/auth/login
POST /api/auth/refresh
POST /api/auth/logout
POST /api/auth/users
GET  /api/auth/me
```

## Fase 2 — Pacientes

Implementado:

- expediente médico único;
- cédula normalizada y única;
- validación dominicana basada en la estrategia Luhn + excepciones SHA-256 de OGTIC Cuenta Única Registry;
- clasificación Estudiante / Empleado / Profesor / Otro;
- fecha de nacimiento, sexo y datos de contacto;
- contacto de emergencia;
- búsqueda y consulta;
- actualización y baja lógica.

Endpoints:

```text
GET    /api/patients
GET    /api/patients/{id}
POST   /api/patients
PUT    /api/patients/{id}
DELETE /api/patients/{id}
```

## Fase 3 — Médicos y personal

Implementado:

- directorio de médicos, enfermería, recepción, farmacia, laboratorio y administración;
- código interno único;
- cédula dominicana validada;
- especialidad;
- exequátur/licencia;
- tanda laboral;
- búsqueda, edición y baja lógica;
- operaciones sensibles restringidas a Administrator.

Endpoints:

```text
GET    /api/staff
GET    /api/staff/{id}
POST   /api/staff
PUT    /api/staff/{id}
DELETE /api/staff/{id}
```

## Fase 4 — Agenda y citas

Implementado:

- relación paciente ↔ médico;
- inicio y fin en UTC;
- estados Scheduled, Confirmed, InProgress, Completed, Cancelled y NoShow;
- motivo y notas;
- cancelación justificada;
- detección de solapamientos de agenda por médico;
- filtros por fechas, paciente y médico;
- solo personal de tipo Doctor puede recibir citas clínicas.

Endpoints:

```text
GET   /api/appointments
GET   /api/appointments/{id}
POST  /api/appointments
PUT   /api/appointments/{id}
PATCH /api/appointments/{id}/status
```

## Fase 5 — Consultas e historia clínica

Implementado:

- consulta asociada a paciente y médico;
- vínculo opcional a una cita;
- síntomas, diagnóstico, recomendaciones y notas;
- signos vitales iniciales: presión arterial, temperatura, frecuencia cardíaca, peso y altura;
- estado Draft / Completed / Cancelled;
- solo consultas en borrador pueden editarse;
- completar una consulta requiere diagnóstico;
- una consulta vinculada mueve la cita a InProgress y, al completarse, a Completed;
- historia clínica formada exclusivamente por consultas completadas.

Endpoints:

```text
GET   /api/consultations
GET   /api/consultations/{id}
POST  /api/consultations
PUT   /api/consultations/{id}
PATCH /api/consultations/{id}/status
GET   /api/patients/{patientId}/clinical-history
```

## Frontend

La aplicación React incluye una consola operacional para:

- login y bootstrap inicial;
- navegación por módulos;
- registro y búsqueda de pacientes;
- directorio y alta de personal;
- programación y seguimiento de citas;
- creación y finalización de consultas;
- visualización de estado de la API;
- renovación automática del access token mediante refresh token.

## Configuración sensible

Los secretos no deben versionarse. Para Docker se requieren, como mínimo:

```text
MSSQL_SA_PASSWORD
JWT_SIGNING_KEY
```

Utiliza `.env.example` como plantilla y nunca reutilices sus valores demostrativos en producción.

## Definition of Done

Una Fase 1–5 se considera integrada cuando backend, frontend, pruebas y CI compilan sin advertencias ni errores y las reglas de autorización permanecen aplicadas en la API, independientemente de la visibilidad del frontend.
