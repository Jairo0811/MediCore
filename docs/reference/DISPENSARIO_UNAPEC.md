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
