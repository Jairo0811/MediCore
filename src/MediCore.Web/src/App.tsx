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

function LoginView({ onAuthenticated }: { onAuthenticated: (session: AuthResponse) => void }) {
  const [mode, setMode] = useState<'login' | 'bootstrap'>('login');
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [fullName, setFullName] = useState('');
  const [error, setError] = useState<string | null>(null);
  const [busy, setBusy] = useState(false);

  async function submit(event: FormEvent) {
    event.preventDefault(); setBusy(true); setError(null);
    try { onAuthenticated(await authenticate(mode, email, password, fullName)); }
    catch (caught) { setError(caught instanceof Error ? caught.message : 'No fue posible iniciar sesión.'); }
    finally { setBusy(false); }
  }

  return <main className="auth-shell"><section className="auth-card"><div className="brand-inline"><span className="brand-mark">+</span><div><strong>Medi<span>Core</span></strong><small>La gestión médica en un solo lugar.</small></div></div><p className="eyebrow">Clinical Operations Platform</p><h1>{mode === 'login' ? 'Acceso seguro' : 'Administrador inicial'}</h1><p className="muted">{mode === 'login' ? 'Ingresa con una cuenta autorizada de MediCore.' : 'Disponible únicamente mientras no existan usuarios y el bootstrap esté habilitado.'}</p>{error && <div className="alert alert--error" role="alert">{error}</div>}<form onSubmit={submit} className="form-grid form-grid--single">{mode === 'bootstrap' && <label>Nombre completo<input value={fullName} onChange={(e) => setFullName(e.target.value)} required /></label>}<label>Correo electrónico<input type="email" value={email} onChange={(e) => setEmail(e.target.value)} required /></label><label>Contraseña<input type="password" value={password} onChange={(e) => setPassword(e.target.value)} minLength={10} required /></label><button className="button button--primary" disabled={busy}>{busy ? 'Procesando…' : mode === 'login' ? 'Iniciar sesión' : 'Crear administrador'}</button></form><button className="button button--link" onClick={() => { setMode(mode === 'login' ? 'bootstrap' : 'login'); setError(null); }}>{mode === 'login' ? 'Configurar primer administrador' : 'Volver al inicio de sesión'}</button></section></main>;
}

function Overview({ apiState }: { apiState: ApiState }) {
  return <section className="overview"><div className="welcome-card"><p className="eyebrow">MediCore v1.0.0</p><h2>Operación médica centralizada</h2><p>Plataforma completa con identidad, atención clínica, farmacia, inventario, laboratorio, analítica, auditoría y hardening de producción.</p><div className={`api-status api-status--${apiState}`}><span className="status-dot" /> API {apiState === 'checking' ? 'verificando' : apiState === 'online' ? 'en línea' : 'sin conexión'}</div></div><div className="feature-grid"><article><span>01</span><strong>Identidad</strong><small>JWT, refresh tokens y RBAC</small></article><article><span>02–05</span><strong>Core clínico</strong><small>Pacientes, personal, agenda e historia</small></article><article><span>06</span><strong>Farmacia</strong><small>Catálogo farmacéutico</small></article><article><span>07</span><strong>Inventario</strong><small>Lotes, kardex y vencimientos</small></article><article><span>08</span><strong>Laboratorio</strong><small>Órdenes, pruebas y resultados</small></article><article><span>09</span><strong>Analítica</strong><small>KPIs, alertas y reportes</small></article><article><span>10</span><strong>Producción</strong><small>Auditoría, observabilidad y QA</small></article></div></section>;
}

export default function App() {
  const [session, setSession] = useState<AuthResponse | null>(() => readSession());
  const [section, setSection] = useState<Section>('overview');
  const [apiState, setApiState] = useState<ApiState>('checking');

  useEffect(() => {
    const controller = new AbortController();
    fetch(`${apiBaseUrl}/api/health/live`, { signal: controller.signal }).then((r) => setApiState(r.ok ? 'online' : 'offline')).catch((error: unknown) => { if (!(error instanceof DOMException && error.name === 'AbortError')) setApiState('offline'); });
    return () => controller.abort();
  }, []);

  if (!session) return <LoginView onAuthenticated={setSession} />;

  const roles = session.user.roles;
  const has = (...allowed: string[]) => roles.some((role) => allowed.includes(role));
  const navigation: Array<{ id: Section; label: string; icon: string }> = [{ id: 'overview', label: 'Resumen', icon: '⌂' }];

  if (has('Administrator', 'Doctor', 'Nurse', 'Receptionist')) navigation.push({ id: 'patients', label: 'Pacientes', icon: '◉' }, { id: 'staff', label: 'Personal', icon: '✚' }, { id: 'appointments', label: 'Agenda', icon: '◷' });
  if (has('Administrator', 'Doctor', 'Nurse')) navigation.push({ id: 'consultations', label: 'Consultas', icon: '▤' });
  if (has('Administrator', 'Pharmacist', 'Doctor', 'Nurse', 'Auditor')) navigation.push({ id: 'pharmacy', label: 'Farmacia', icon: '◆' }, { id: 'inventory', label: 'Inventario', icon: '▦' });
  if (has('Administrator', 'Doctor', 'Nurse', 'Laboratory', 'Auditor')) navigation.push({ id: 'laboratory', label: 'Laboratorio', icon: '◈' });
  if (has('Administrator', 'Doctor', 'Nurse', 'Pharmacist', 'Laboratory', 'Auditor')) navigation.push({ id: 'analytics', label: 'Analítica', icon: '▥' });
  if (has('Administrator', 'Auditor')) navigation.push({ id: 'audit', label: 'Auditoría', icon: '◎' });

  const canManagePharmacy = has('Administrator', 'Pharmacist');
  const canManageLab = has('Administrator', 'Laboratory');
  const canOrderLab = has('Administrator', 'Doctor');

  return <div className="app-shell"><aside className="sidebar"><div className="brand-inline brand-inline--sidebar"><span className="brand-mark">+</span><div><strong>Medi<span>Core</span></strong><small>Clinical Platform</small></div></div><nav>{navigation.map((item) => <button key={item.id} className={section === item.id ? 'nav-item nav-item--active' : 'nav-item'} onClick={() => setSection(item.id)}><span>{item.icon}</span>{item.label}</button>)}</nav><div className="sidebar-footer"><small>Sesión activa</small><strong>{session.user.fullName}</strong><span>{roles.join(', ')}</span><button className="button button--ghost" onClick={() => { clearSession(); setSession(null); }}>Cerrar sesión</button></div></aside><main className="content"><header className="topbar"><div><p className="eyebrow">MediCore · v1.0.0</p><h1>{navigation.find((item) => item.id === section)?.label}</h1></div><div className={`api-status api-status--${apiState}`}><span className="status-dot" />{apiState === 'online' ? 'API disponible' : apiState === 'checking' ? 'Verificando' : 'API sin conexión'}</div></header>{section === 'overview' && <Overview apiState={apiState} />}{section === 'patients' && <PatientsPage />}{section === 'staff' && <StaffPage />}{section === 'appointments' && <AppointmentsPage />}{section === 'consultations' && <ConsultationsPage />}{section === 'pharmacy' && <PharmacyPage canManage={canManagePharmacy} />}{section === 'inventory' && <InventoryPage canManage={canManagePharmacy} />}{section === 'laboratory' && <LaboratoryPage canManageDefinitions={canManageLab} canOrder={canOrderLab} canResult={canManageLab} />}{section === 'analytics' && <AnalyticsPage />}{section === 'audit' && <AuditPage />}</main></div>;
}
