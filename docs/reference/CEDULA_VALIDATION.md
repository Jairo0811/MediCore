# Validación de cédula dominicana

MediCore incorpora validación de cédula dominicana en el backend para los módulos de **Pacientes** y **Médicos/Personal**.

## Referencia técnica

La estrategia fue adaptada del proyecto público **OGTIC Cuenta Única Registry** (`ogticrd/cuenta-unica-registry`), específicamente de `src/common/helpers/validations.ts`.

La referencia implementa dos reglas relevantes:

1. validación del número mediante el algoritmo **Luhn**;
2. soporte para casos excepcionales mediante una lista de **hashes SHA-256**, evitando exponer números de cédula en la configuración.

MediCore traslada esa estrategia al backend en C# mediante `DominicanCedulaValidator` y la abstracción `ICedulaValidator`.

## Reglas de MediCore

- Se eliminan guiones y cualquier carácter no numérico antes de validar.
- El resultado normalizado debe contener exactamente **11 dígitos**.
- Se ejecuta el checksum Luhn siguiendo el mismo patrón de la referencia.
- Si Luhn falla, se calcula SHA-256 sobre la cédula normalizada y se compara contra `CedulaValidation:LuhnExceptionHashes`.
- Las excepciones se configuran exclusivamente como hashes; MediCore no requiere almacenar cédulas excepcionales en texto plano dentro del repositorio.
- Pacientes y miembros del personal no pueden duplicar una cédula dentro de su propio catálogo.

Ejemplo de configuración:

```json
{
  "CedulaValidation": {
    "LuhnExceptionHashes": []
  }
}
```

## Alcance de la validación

El checksum demuestra únicamente que el número tiene una estructura compatible con la regla implementada. **No verifica que una persona exista, que la cédula esté activa, ni sustituye una consulta contra una fuente oficial de identidad.**

Si en el futuro MediCore necesita validación de identidad oficial, deberá integrarse mediante un proveedor autorizado, con base legal, control de acceso, auditoría y minimización de datos.

## Licencia y atribución

`ogticrd/cuenta-unica-registry` se distribuye bajo licencia MIT y atribuye copyright a la **Oficina Gubernamental de Tecnologías de la Información y Comunicación (OGTIC), 2023**. MediCore conserva la atribución en `THIRD_PARTY_NOTICES.md`.
