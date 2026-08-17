# Política de seguridad

## Versiones soportadas

| Versión | Soporte |
|---|:---:|
| 1.0.x | ✅ |
| < 1.0 | ❌ |

## Reportar una vulnerabilidad

No publiques vulnerabilidades, credenciales, tokens, datos personales ni detalles explotables en un issue público.

Utiliza **GitHub Security Advisories / Report a vulnerability** del repositorio cuando esté disponible. Incluye una descripción reproducible, impacto estimado y el componente afectado, evitando datos reales de pacientes o credenciales.

## Principios de seguridad

MediCore aplica autenticación JWT, refresh tokens rotativos, RBAC en backend, lockout de acceso, rate limiting de autenticación, headers de seguridad, trazabilidad de escrituras, secretos fuera del repositorio y despliegues de base de datos mediante migraciones revisadas.

Las validaciones internas de identidad —incluido el checksum de cédula dominicana— no sustituyen una verificación oficial de identidad.
