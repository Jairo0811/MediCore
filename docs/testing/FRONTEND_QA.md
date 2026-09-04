# QA del frontend

## Comandos

Desde `src/MediCore.Web`:

```bash
npm install
npm run typecheck
npm run test:unit
npm run build
npm run test:e2e:install
npm run test:e2e
```

## Cobertura inicial

La suite comprueba:

- renderizado accesible de errores;
- normalización UTC de fechas;
- estructura y atributos del formulario de autenticación;
- transición entre login y bootstrap administrativo;
- skip link y foco inicial por teclado;
- auditoría WCAG automatizada del shell de autenticación mediante axe-core.

## Regla de accesibilidad automatizada

El smoke test E2E inspecciona reglas WCAG 2.0/2.1 A y AA y falla cuando axe-core encuentra una violación de impacto `serious` o `critical`.

Las incidencias `moderate` o `minor` no se ignoran como política de producto: deben revisarse manualmente antes de declarar una entrega académica definitiva. Se mantienen fuera del bloqueo automático inicial para evitar falsos positivos en reglas que requieren evaluación humana.

## CI

GitHub Actions ejecuta:

1. instalación de dependencias;
2. type-check;
3. pruebas unitarias/de componentes;
4. build Vite;
5. instalación de Chromium;
6. Playwright + axe.

El backend mantiene en paralelo restore, build y pruebas .NET.
