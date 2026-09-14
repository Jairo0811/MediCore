import { FormEvent, useEffect, useState } from 'react';
import { apiBaseUrl, authenticate, clearSession, readSession } from './api';
import AppointmentsPage from './pages/AppointmentsPage';
import AuditPage from './pages/AuditPage';
import ConsultationsPage from './pages/ConsultationsPage';
import PatientsPage from './pages/PatientsPage';
import StaffPage from './pages/StaffPage';
import InventoryPage from './pages/InventoryPage';
import LaboratoryPage from './pages/LaboratoryPage';
import AnalyticsPage from './pages/AnalyticsPage';
import PharmacyPage from './PharmacyPage';
import type { AuthResponse } from './types';

type ApiState = 'checking' | 'online' | 'offline';
type Section = 'overview' | 'patients' | 'staff' | 'appointments' | 'consultations' | 'pharmacy' | 'inventory' | 'laboratory' | 'analytics' | 'audit';

type NavigationItem = {
  id: Section;
  label: string;
  icon: string;
};

const BRAND_LOGO = '/branding/medicore-logo.png';
const BRAND_ISOTYPE = '/branding/favicon.ico';

function Brand({ sidebar = false }: { sidebar?: boolean }) {
  if (!sidebar) {
    return <div className="brand-inline brand-inline--official">
      <img
        className="brand-logo"
        src={BRAND_LOGO}
        alt="MediCore — La gestión médica en un solo lugar"
      />
    </div>;
  }

  return <div className="brand-inline brand-inline--sidebar brand-inline--official-sidebar">
    <img className="brand-isotype" src={BRAND_ISOTYPE} alt="" aria-hidden="true" />
    <div><strong>Medi<span>Core</span></strong><small>Clinical Platform</small></div>
  </div>;
}

function LoginView({ onAuthenticated }: { onAuthenticated: (session: AuthResponse) => void }) {
  const [mode, setMode] = useState<'login' | 'bootstrap'>('login');
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [fullName, setFullName] = useState('');
  const [error, setError] = useState<string | null>(null);
  const [busy, setBusy] = useState(false);

  async function submit(event: FormEvent) {
    event.preventDefault();
    setBusy(true);
    setError(null);

    try {
      onAuthenticated(await authenticate(mode, email, password, fullName));
    } catch (caught) {
      setError(caught instanceof Error ? caught.message : 'No fue posible iniciar sesión.');
    } finally {
      setBusy(false);
    }
  }

  function toggleMode() {
    setMode(mode === 'login' ? 'bootstrap' : 'login');
    setError(null);
  }

  return <>
    <a className="skip-link" href="#main-content">Saltar al contenido principal</a>
    <main id="main-content" className="auth-shell" tabIndex={-1}>
      <section className="auth-card" aria-labelledby="auth-title">
        <Brand />
        <p className="eyebrow">Clinical Operations Platform</p>
        <h1 id="auth-title">{mode === 'login' ? 'Acceso seguro' : 'Administrador inicial'}</h1>
        <p className="muted">{mode === 'login' ? 'Ingresa con una cuenta autorizada de MediCore.' : 'Disponible únicamente mientras no existan usuarios y el bootstrap esté habilitado.'}</p>
        {error && <div className="alert alert--error" role="alert">{error}</div>}
        <form onSubmit={submit} className="form-grid form-grid--single">
          {mode === 'bootstrap' && <label>Nombre completo<input value={fullName} onChange={(e) => setFullName(e.target.value)} autoComplete="name" required /></label>}
          <label>Correo electrónico<input type="email" value={email} onChange={(e) => setEmail(e.target.value)} autoComplete="email" required /></label>
          <label>Contraseña<input type="password" value={password} onChange={(e) => setPassword(e.target.value)} autoComplete={mode === 'login' ? 'current-password' : 'new-password'} minLength={10} required /></label>
          <button type="submit" className="button button--primary" disabled={busy}><i className="fa-solid fa-shield-halved" aria-hidden="true" /> {busy ? 'Procesando…' : mode === 'login' ? 'Iniciar sesión' : 'Crear administrador'}</button>
        </form>
        <button type="button" className="button button--link" onClick={toggleMode}>{mode === 'login' ? 'Configurar primer administrador' : 'Volver al inicio de sesión'}</button>
      </section>
    </main>
  </>;
}

const overviewFeatures = [
  { phase: '01', icon: 'fa-solid fa-fingerprint', title: 'Identidad', detail: 'JWT, refresh tokens y RBAC' },
  { phase: '02–05', icon: 'fa-solid fa-stethoscope', title: 'Core clínico', detail: 'Pacientes, personal, agenda e historia' },
  { phase: '06', icon: 'fa-solid fa-capsules', title: 'Farmacia', detail: 'Catálogo farmacéutico' },
  { phase: '07', icon: 'fa-solid fa-boxes-stacked', title: 'Inventario', detail: 'Lotes, kardex y vencimientos' },
  { phase: '08', icon: 'fa-solid fa-flask-vial', title: 'Laboratorio', detail: 'Órdenes, pruebas y resultados' },
  { phase: '09', icon: 'fa-solid fa-chart-line', title: 'Analítica', detail: 'KPIs, alertas y reportes' },
  { phase: '10', icon: 'fa-solid fa-shield-halved', title: 'Producción', detail: 'Auditoría, observabilidad y QA' },
];

function Overview({ apiState }: { apiState: ApiState }) {
  return <section className="overview" aria-labelledby="overview-title">
    <div className="welcome-card">
      <div className="welcome-card__content">
        <p className="eyebrow">MediCore v1.0.0</p>
        <h2 id="overview-title">Operación médica centralizada</h2>
        <p>Plataforma completa con identidad, atención clínica, farmacia, inventario, laboratorio, analítica, auditoría y hardening de producción.</p>
        <div className={`api-status api-status--${apiState}`} role="status" aria-live="polite"><span className="status-dot" aria-hidden="true" /> API {apiState === 'checking' ? 'verificando' : apiState === 'online' ? 'en línea' : 'sin conexión'}</div>
      </div>
      <div className="welcome-card__mark" aria-hidden="true">
        <img className="welcome-card__isotype" src={BRAND_ISOTYPE} alt="" />
      </div>
    </div>
    <div className="feature-grid">
      {overviewFeatures.map((feature) => <article key={feature.phase}>
        <div className="feature-card__top"><span>{feature.phase}</span><i className={feature.icon} aria-hidden="true" /></div>
        <strong>{feature.title}</strong><small>{feature.detail}</small>
      </article>)}
    </div>
  </section>;
}

export default function App() {
  const [session, setSession] = useState<AuthResponse | null>(() => readSession());
  const [section, setSection] = useState<Section>('overview');
  const [apiState, setApiState] = useState<ApiState>('checking');

  useEffect(() => {
    const controller = new AbortController();
    fetch(`${apiBaseUrl}/api/health/live`, { signal: controller.signal })
      .then((response) => setApiState(response.ok ? 'online' : 'offline'))
      .catch((error: unknown) => {
        if (!(error instanceof DOMException && error.name === 'AbortError')) setApiState('offline');
      });
    return () => controller.abort();
  }, []);

  if (!session) return <LoginView onAuthenticated={setSession} />;

  const roles = session.user.roles;
  const has = (...allowed: string[]) => roles.some((role) => allowed.includes(role));
  const navigation: NavigationItem[] = [{ id: 'overview', label: 'Resumen', icon: 'fa-solid fa-house-medical' }];

  if (has('Administrator', 'Doctor', 'Nurse', 'Receptionist')) navigation.push(
    { id: 'patients', label: 'Pacientes', icon: 'fa-solid fa-user-group' },
    { id: 'staff', label: 'Personal', icon: 'fa-solid fa-user-doctor' },
    { id: 'appointments', label: 'Agenda', icon: 'fa-solid fa-calendar-check' },
  );
  if (has('Administrator', 'Doctor', 'Nurse')) navigation.push({ id: 'consultations', label: 'Consultas', icon: 'fa-solid fa-notes-medical' });
  if (has('Administrator', 'Pharmacist', 'Doctor', 'Nurse', 'Auditor')) navigation.push(
    { id: 'pharmacy', label: 'Farmacia', icon: 'fa-solid fa-capsules' },
    { id: 'inventory', label: 'Inventario', icon: 'fa-solid fa-boxes-stacked' },
  );
  if (has('Administrator', 'Doctor', 'Nurse', 'Laboratory', 'Auditor')) navigation.push({ id: 'laboratory', label: 'Laboratorio', icon: 'fa-solid fa-flask-vial' });
  if (has('Administrator', 'Doctor', 'Nurse', 'Pharmacist', 'Laboratory', 'Auditor')) navigation.push({ id: 'analytics', label: 'Analítica', icon: 'fa-solid fa-chart-line' });
  if (has('Administrator', 'Auditor')) navigation.push({ id: 'audit', label: 'Auditoría', icon: 'fa-solid fa-shield-halved' });

  const canManagePharmacy = has('Administrator', 'Pharmacist');
  const canManageLab = has('Administrator', 'Laboratory');
  const canOrderLab = has('Administrator', 'Doctor');
  const activeItem = navigation.find((item) => item.id === section) ?? navigation[0];

  return <>
    <a className="skip-link" href="#main-content">Saltar al contenido principal</a>
    <div className="app-shell">
      <aside className="sidebar">
        <Brand sidebar />
        <p className="sidebar-section-label">Operación clínica</p>
        <nav aria-label="Navegación principal">{navigation.map((item) => <button type="button" key={item.id} className={section === item.id ? 'nav-item nav-item--active' : 'nav-item'} aria-current={section === item.id ? 'page' : undefined} onClick={() => setSection(item.id)}><span className="nav-item__icon" aria-hidden="true"><i className={item.icon} /></span><span>{item.label}</span></button>)}</nav>
        <div className="sidebar-footer">
          <div className="session-card"><span className="session-card__avatar" aria-hidden="true"><i className="fa-solid fa-user-shield" /></span><div><small>Sesión activa</small><strong>{session.user.fullName}</strong><span>{roles.join(', ')}</span></div></div>
          <button type="button" className="button button--ghost" onClick={() => { clearSession(); setSession(null); }}><i className="fa-solid fa-arrow-right-from-bracket" aria-hidden="true" /> Cerrar sesión</button>
        </div>
      </aside>
      <main id="main-content" className="content" tabIndex={-1}>
        <header className="topbar"><div className="topbar__title"><span className="topbar__icon" aria-hidden="true"><i className={activeItem.icon} /></span><div><p className="eyebrow">MediCore · v1.0.0</p><h1>{activeItem.label}</h1></div></div><div className={`api-status api-status--${apiState}`} role="status" aria-live="polite"><span className="status-dot" aria-hidden="true" />{apiState === 'online' ? 'API disponible' : apiState === 'checking' ? 'Verificando' : 'API sin conexión'}</div></header>
        {section === 'overview' && <Overview apiState={apiState} />}
        {section === 'patients' && <PatientsPage />}
        {section === 'staff' && <StaffPage />}
        {section === 'appointments' && <AppointmentsPage />}
        {section === 'consultations' && <ConsultationsPage />}
        {section === 'pharmacy' && <PharmacyPage canManage={canManagePharmacy} />}
        {section === 'inventory' && <InventoryPage canManage={canManagePharmacy} />}
        {section === 'laboratory' && <LaboratoryPage canManageDefinitions={canManageLab} canOrder={canOrderLab} canResult={canManageLab} />}
        {section === 'analytics' && <AnalyticsPage />}
        {section === 'audit' && <AuditPage />}
      </main>
    </div>
  </>;
}
