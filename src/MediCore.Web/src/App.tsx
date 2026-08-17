import { useEffect, useState } from 'react';

const apiBaseUrl = import.meta.env.VITE_API_BASE_URL ?? 'http://localhost:8080';

type ApiState = 'checking' | 'online' | 'offline';

export default function App() {
  const [apiState, setApiState] = useState<ApiState>('checking');

  useEffect(() => {
    const controller = new AbortController();

    fetch(`${apiBaseUrl}/api/health/live`, { signal: controller.signal })
      .then((response) => setApiState(response.ok ? 'online' : 'offline'))
      .catch((error: unknown) => {
        if (error instanceof DOMException && error.name === 'AbortError') {
          return;
        }

        setApiState('offline');
      });

    return () => controller.abort();
  }, []);

  return (
    <main className="shell">
      <section className="hero" aria-labelledby="medicore-title">
        <div className="brand-mark" aria-hidden="true">
          <span>+</span>
        </div>

        <p className="eyebrow">Healthcare Management Platform</p>
        <h1 id="medicore-title">
          Medi<span>Core</span>
        </h1>
        <p className="tagline">La gestión médica en un solo lugar.</p>
        <p className="description">
          Base técnica de una plataforma modular para consultorios, farmacia,
          laboratorio y administración clínica.
        </p>

        <div className="domains" aria-label="Dominios principales de MediCore">
          <article>
            <strong>Consultorios</strong>
            <small>Pacientes, médicos y atención clínica</small>
          </article>
          <article>
            <strong>Farmacia</strong>
            <small>Medicamentos, inventario y trazabilidad</small>
          </article>
          <article>
            <strong>Laboratorio</strong>
            <small>Órdenes, estudios y resultados</small>
          </article>
          <article>
            <strong>Administración</strong>
            <small>Seguridad, auditoría y reportes</small>
          </article>
        </div>

        <div className={`api-status api-status--${apiState}`} role="status">
          <span className="status-dot" aria-hidden="true" />
          API {apiState === 'checking' ? 'verificando' : apiState === 'online' ? 'en línea' : 'sin conexión'}
        </div>
      </section>
    </main>
  );
}
