# Verificación de exequátur profesional

MediCore incorpora en el módulo **Médicos y personal** un acceso explícito a la consulta oficial de exequátur del **Ministerio de Salud Pública y Asistencia Social (MISPAS)**.

## Fuentes oficiales

- Consulta pública de exequátur: `https://intranet.msp.gob.do/intranet/juridica/publica/consultaexequatur.aspx`
- Portal/API indicada por MISPAS: `https://digital.msp.gob.do/#/api/exequatur`
- Portal de desarrolladores MISPAS: `https://developers.apis.msp.gob.do/`

## Comportamiento actual

La interfaz de MediCore permite registrar el número de exequátur/licencia y, cuando el tipo de personal es **Médico**, presenta acciones para:

1. abrir la consulta oficial pública de MISPAS;
2. abrir la documentación/portal oficial de la API de exequátur.

MediCore **no afirma que un exequátur ha sido validado automáticamente** mientras no exista una integración autenticada con el servicio oficial.

## Integración automática futura

El portal de APIs de MISPAS utiliza acceso controlado. Una integración automática deberá ejecutarse únicamente desde el backend y deberá configurarse cuando MISPAS entregue las credenciales y el contrato técnico aplicable.

Reglas de seguridad para esa integración:

- no almacenar `ClientId`, `ApiKey`, tokens u otros secretos en React/Vite;
- usar secretos de entorno o un gestor de secretos en el backend;
- aplicar timeout, cancelación y manejo explícito de indisponibilidad del proveedor;
- registrar la consulta en auditoría sin persistir más datos personales de los necesarios;
- distinguir claramente entre `verificado`, `no encontrado`, `no disponible` y `no consultado`;
- no bloquear operaciones clínicas existentes por una caída temporal de un servicio externo salvo que una política institucional lo requiera.

## Alcance

El campo `LicenseNumber` sigue siendo parte del expediente de personal. La consulta oficial funciona como verificación complementaria y no sustituye las políticas internas de Recursos Humanos, Jurídica o habilitación profesional.
