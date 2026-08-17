# Referencia: DispensarioMedicoUnapec

MediCore toma inspiración funcional del proyecto grupal `JosesamuelPA/DispensarioMedicoUnapec`, desarrollado para la gestión del dispensario médico de UNAPEC.

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

Toda funcionalidad incorporada a MediCore debe ser diseñada nuevamente según los requisitos propios del producto. La referencia sirve para comprender el problema original, no como base de código a reutilizar.
