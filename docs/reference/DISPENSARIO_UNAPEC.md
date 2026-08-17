# Referencia: DispensarioMedicoUnapec

MediCore toma inspiración funcional del proyecto grupal `JosesamuelPA/DispensarioMedicoUnapec`, desarrollado para la gestión del dispensario médico de UNAPEC.

## Origen del requerimiento académico

El proyecto académico original forma parte de una colección de proyectos desarrollados en la **Universidad APEC (UNAPEC)** tomando como referencia enunciados y proyectos propuestos por el profesor **Juan Pablo Valdez Reyes**.

El enunciado funcional utilizado como base proviene de un documento de **Proyecto Final de UNAPEC de 2015**, en cuya portada aparece identificado el docente como **Profesor: Juan P. Valdez**.

El documento plantea como requerimiento general desarrollar un sistema para el Dispensario Médico de UNAPEC que contemple:

- gestión de tipos de fármacos;
- gestión de marcas o laboratorios fabricantes;
- gestión de ubicaciones;
- gestión de medicamentos;
- gestión de pacientes;
- gestión de médicos;
- registro de visitas;
- consultas por criterios como médico, paciente o fecha;
- reportes de visitas entre fechas, por médico o por paciente;
- implementación utilizando la versión vigente de **.NET Framework con WinForms**.

El mismo documento define además datos mínimos para tipos de fármacos, marcas, ubicaciones, medicamentos, pacientes, médicos y registro de visitas.

## Continuidad académica

La continuidad de MediCore se documenta siguiendo el mismo criterio utilizado en otros proyectos académicos del portafolio, como **RadioEmisora RD** e **IngSoft Studio**: una relación se registra únicamente cuando existe una coincidencia académica verificable por **profesor**, **período**, **línea de referencia** o **compañero identificado inequívocamente por nombre y matrícula**.

Estas conexiones son de carácter académico y formativo. No significan que los proyectos compartan código, arquitectura o dominio funcional.

| Tipo de continuidad | Coincidencia | Proyecto relacionado | Relación |
|---|---|---|---|
| Profesor recurrente | **Ing. Omar Antonio De Jesus De La Cruz Gonzalez** | `Jairo0811/CineGest` — Desarrollo de Software con Tecnología Open Source 1 (ISO-610) | Mismo profesor durante Enero - Abril de 2026 |
| Período académico compartido | **Enero - Abril 2026** | `Jairo0811/CineGest` | ISO-605 e ISO-610 fueron cursadas durante el mismo cuatrimestre |
| Compañera recurrente | **Emely Marie Castillo Rivera (A00110380)** | `Jairo0811/Ecosoft` — Proyecto de Software 1 (ISO-705) | Coincidencia posterior en Mayo - Agosto de 2026 |
| Línea académica de referencia | **Juan Pablo Valdez Reyes** | `Jairo0811/CineGest` y `Jairo0811/RentCarRD` | Proyectos o enunciados de la colección académica asociados a su catálogo de ejercicios y proyectos |

### Continuidad docente

Durante **Enero - Abril de 2026**, Francis Jairo Matías Rosario cursó dos asignaturas complementarias con el **Ing. Omar Antonio De Jesus De La Cruz Gonzalez**:

- **Desarrollo de Software con Tecnología Propietaria 1 (ISO-605)**, cuyo proyecto final grupal dio origen al antecedente inmediato de MediCore;
- **Desarrollo de Software con Tecnología Open Source 1 (ISO-610)**, cuyo proyecto final fue posteriormente reconstruido como **CineGest**.

La relación entre MediCore y CineGest es, por tanto, una **continuidad docente directa** dentro del mismo período académico: dos asignaturas distintas, impartidas por el mismo profesor, con proyectos finales independientes y tecnologías diferentes.

### Reencuentro académico

**Emely Marie Castillo Rivera (A00110380)** formó parte del equipo académico original de **DispensarioMedicoUnapec / ISO-605** junto a Francis Jairo Matías Rosario y posteriormente volvió a coincidir con él en el equipo de **EcoSoft / Proyecto de Software 1 (ISO-705)** durante **Mayo - Agosto de 2026**.

Esta relación se documenta como un **reencuentro académico** porque existe coincidencia verificable tanto del nombre completo como de la matrícula. La continuidad conecta dos cuatrimestres consecutivos y dos proyectos grupales con dominios completamente diferentes: gestión médica y gestión de subastas energéticas y contratos PPA.

### Continuidad de referencia académica

El profesor **Juan Pablo Valdez Reyes** debe distinguirse del profesor que impartió ISO-605 en 2026. En MediCore su relación corresponde al **origen histórico del enunciado**, no a la docencia directa de la asignatura cursada por el equipo.

El PDF de 2015 identifica al docente como **Juan P. Valdez** y propone el sistema de gestión del Dispensario Médico de UNAPEC. Otros repositorios del portafolio, como **CineGest** y **RentCarRD**, también documentan proyectos o enunciados asociados a la colección académica de **Juan Pablo Valdez Reyes**. Esto permite registrar una continuidad de referencia sin confundirla con continuidad docente.

### Criterio de verificación

Para mantener trazabilidad académica:

- un **profesor recurrente** debe aparecer como docente de las asignaturas relacionadas;
- una **línea de referencia** puede vincular proyectos derivados de enunciados o catálogos académicos aunque el profesor de la asignatura haya sido otro;
- un **compañero recurrente** requiere coincidencia inequívoca de nombre y matrícula;
- el equipo del proyecto académico original se mantiene separado de la autoría de la reconstrucción moderna;
- ninguna continuidad implica reutilización automática de código entre proyectos.

## Capacidades observadas en la referencia

El repositorio de referencia incluye áreas para:

- autenticación y perfil de usuario;
- pacientes;
- médicos;
- visitas;
- medicamentos;
- marcas;
- tipos de fármacos;
- estantes y ubicación de medicamentos;
- reportes.

Su implementación utiliza ASP.NET Core MVC, Entity Framework Core y SQL Server.

## Cómo se utilizará como inspiración

MediCore no copiará el código ni conservará una arquitectura MVC monolítica. Las ideas de negocio se estudiarán como punto de partida y se reinterpretarán con:

- backend ASP.NET Core Web API;
- frontend React + TypeScript desacoplado;
- Clean Architecture;
- autenticación y autorización con un modelo de seguridad moderno;
- trazabilidad y auditoría;
- módulos clínicos ampliados;
- farmacia e inventario con mayor detalle;
- laboratorio;
- pruebas automatizadas y CI/CD.

## Criterio de independencia

Toda funcionalidad incorporada a MediCore debe ser diseñada nuevamente según los requisitos propios del producto. La referencia sirve para comprender el problema original y documentar su procedencia académica, no como base de código a reutilizar.
