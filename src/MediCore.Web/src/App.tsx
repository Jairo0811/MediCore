import { FormEvent, useEffect, useMemo, useState } from 'react';
import { apiBaseUrl, apiRequest, authenticate, clearSession, readSession } from './api';
import type { Appointment, AuthResponse, Consultation, MedicalStaff, Patient } from './types';

type ApiState = 'checking' | 'online' | 'offline';
type Section = 'overview' | 'patients' | 'staff' | 'appointments' | 'consultations';

const patientTypes = ['No definido', 'Estudiante', 'Empleado', 'Profesor', 'Otro'];
const staffTypes = ['No definido', 'Médico', 'Enfermería', 'Recepción', 'Farmacia', 'Laboratorio', 'Administrativo', 'Otro'];
const appointmentStatuses = ['No definido', 'Programada', 'Confirmada', 'En curso', 'Completada', 'Cancelada', 'No asistió'];
const consultationStatuses = ['No definido', 'Borrador', 'Completada', 'Cancelada'];

function formatDate(value: string): string {
  return new Intl.DateTimeFormat('es-DO', { dateStyle: 'medium', timeStyle: 'short' }).format(new Date(value));
}

function toUtc(localValue: string): string {
  return new Date(localValue).toISOString();
}

function ErrorMessage({ message }: { message: string | null }) {
  return message ? <div className="alert alert--error" role="alert">{message}</div> : null;
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

  return (
    <main className="auth-shell">
      <section className="auth-card">
        <div className="brand-inline"><span className="brand-mark">+</span><div><strong>Medi<span>Core</span></strong><small>La gestión médica en un solo lugar.</small></div></div>
        <p className="eyebrow">Clinical Operations Platform</p>
        <h1>{mode === 'login' ? 'Acceso seguro' : 'Administrador inicial'}</h1>
        <p className="muted">{mode === 'login' ? 'Ingresa con una cuenta autorizada de MediCore.' : 'Disponible únicamente mientras no existan usuarios y el bootstrap esté habilitado.'}</p>
        <ErrorMessage message={error} />
        <form onSubmit={submit} className="form-grid form-grid--single">
          {mode === 'bootstrap' && <label>Nombre completo<input value={fullName} onChange={(event) => setFullName(event.target.value)} required /></label>}
          <label>Correo electrónico<input type="email" value={email} onChange={(event) => setEmail(event.target.value)} required /></label>
          <label>Contraseña<input type="password" value={password} onChange={(event) => setPassword(event.target.value)} minLength={10} required /></label>
          <button className="button button--primary" disabled={busy}>{busy ? 'Procesando…' : mode === 'login' ? 'Iniciar sesión' : 'Crear administrador'}</button>
        </form>
        <button className="button button--link" onClick={() => { setMode(mode === 'login' ? 'bootstrap' : 'login'); setError(null); }}>
          {mode === 'login' ? 'Configurar primer administrador' : 'Volver al inicio de sesión'}
        </button>
      </section>
    </main>
  );
}

function PatientsPage() {
  const [items, setItems] = useState<Patient[]>([]);
  const [error, setError] = useState<string | null>(null);
  const [search, setSearch] = useState('');
  const [form, setForm] = useState({ firstName: '', lastName: '', cedula: '', patientType: 1, sex: 0, dateOfBirth: '', email: '', phone: '' });

  async function load() {
    try {
      const suffix = search ? `?search=${encodeURIComponent(search)}&includeInactive=false` : '?includeInactive=false';
      setItems(await apiRequest<Patient[]>(`/api/patients/${suffix}`));
      setError(null);
    } catch (caught) {
      setError(caught instanceof Error ? caught.message : 'No fue posible cargar los pacientes.');
    }
  }

  useEffect(() => { void load(); }, []);

  async function submit(event: FormEvent) {
    event.preventDefault();
    try {
      await apiRequest<Patient>('/api/patients/', { method: 'POST', body: JSON.stringify({ ...form, dateOfBirth: form.dateOfBirth || null, address: null, emergencyContactName: null, emergencyContactPhone: null }) });
      setForm({ firstName: '', lastName: '', cedula: '', patientType: 1, sex: 0, dateOfBirth: '', email: '', phone: '' });
      await load();
    } catch (caught) {
      setError(caught instanceof Error ? caught.message : 'No fue posible registrar el paciente.');
    }
  }

  return <div className="workspace-grid">
    <section className="panel"><div className="panel-heading"><div><p className="eyebrow">Fase 2</p><h2>Pacientes</h2></div><span className="counter">{items.length}</span></div>
      <div className="toolbar"><input placeholder="Buscar por nombre, cédula o expediente" value={search} onChange={(event) => setSearch(event.target.value)} onKeyDown={(event) => { if (event.key === 'Enter') void load(); }} /><button className="button" onClick={() => void load()}>Buscar</button></div>
      <ErrorMessage message={error} />
      <div className="table-wrap"><table><thead><tr><th>Expediente</th><th>Paciente</th><th>Cédula</th><th>Tipo</th><th>Contacto</th></tr></thead><tbody>{items.map((patient) => <tr key={patient.id}><td>{patient.medicalRecordNumber}</td><td><strong>{patient.fullName}</strong></td><td>{patient.cedula}</td><td>{patientTypes[patient.patientType] ?? 'Otro'}</td><td>{patient.phone || patient.email || '—'}</td></tr>)}</tbody></table></div>
    </section>
    <section className="panel panel--form"><p className="eyebrow">Nuevo registro</p><h2>Registrar paciente</h2><p className="muted">La cédula se valida en el backend con checksum Luhn y lista segura de excepciones.</p>
      <form onSubmit={submit} className="form-grid"><label>Nombre<input value={form.firstName} onChange={(event) => setForm({ ...form, firstName: event.target.value })} required /></label><label>Apellido<input value={form.lastName} onChange={(event) => setForm({ ...form, lastName: event.target.value })} required /></label><label>Cédula<input placeholder="000-0000000-0" value={form.cedula} onChange={(event) => setForm({ ...form, cedula: event.target.value })} required /></label><label>Tipo<select value={form.patientType} onChange={(event) => setForm({ ...form, patientType: Number(event.target.value) })}><option value={1}>Estudiante</option><option value={2}>Empleado</option><option value={3}>Profesor</option><option value={4}>Otro</option></select></label><label>Sexo<select value={form.sex} onChange={(event) => setForm({ ...form, sex: Number(event.target.value) })}><option value={0}>No especificado</option><option value={1}>Femenino</option><option value={2}>Masculino</option><option value={3}>Otro</option></select></label><label>Fecha de nacimiento<input type="date" value={form.dateOfBirth} onChange={(event) => setForm({ ...form, dateOfBirth: event.target.value })} /></label><label>Teléfono<input value={form.phone} onChange={(event) => setForm({ ...form, phone: event.target.value })} /></label><label>Correo<input type="email" value={form.email} onChange={(event) => setForm({ ...form, email: event.target.value })} /></label><button className="button button--primary form-span">Guardar paciente</button></form>
    </section>
  </div>;
}

function StaffPage() {
  const [items, setItems] = useState<MedicalStaff[]>([]);
  const [error, setError] = useState<string | null>(null);
  const [form, setForm] = useState({ firstName: '', lastName: '', cedula: '', staffType: 1, specialty: '', licenseNumber: '', workShift: '', email: '', phone: '' });

  async function load() { try { setItems(await apiRequest<MedicalStaff[]>('/api/staff/?includeInactive=false')); setError(null); } catch (caught) { setError(caught instanceof Error ? caught.message : 'No fue posible cargar el personal.'); } }
  useEffect(() => { void load(); }, []);

  async function submit(event: FormEvent) {
    event.preventDefault();
    try {
      await apiRequest<MedicalStaff>('/api/staff/', { method: 'POST', body: JSON.stringify(form) });
      setForm({ firstName: '', lastName: '', cedula: '', staffType: 1, specialty: '', licenseNumber: '', workShift: '', email: '', phone: '' });
      await load();
    } catch (caught) { setError(caught instanceof Error ? caught.message : 'No fue posible registrar el personal.'); }
  }

  return <div className="workspace-grid"><section className="panel"><div className="panel-heading"><div><p className="eyebrow">Fase 3</p><h2>Médicos y personal</h2></div><span className="counter">{items.length}</span></div><ErrorMessage message={error} /><div className="table-wrap"><table><thead><tr><th>Código</th><th>Nombre</th><th>Rol clínico</th><th>Especialidad</th><th>Tanda</th></tr></thead><tbody>{items.map((staff) => <tr key={staff.id}><td>{staff.employeeCode}</td><td><strong>{staff.fullName}</strong><small>{staff.cedula}</small></td><td>{staffTypes[staff.staffType] ?? 'Otro'}</td><td>{staff.specialty || '—'}</td><td>{staff.workShift || '—'}</td></tr>)}</tbody></table></div></section>
    <section className="panel panel--form"><p className="eyebrow">Directorio</p><h2>Registrar personal</h2><form onSubmit={submit} className="form-grid"><label>Nombre<input value={form.firstName} onChange={(event) => setForm({ ...form, firstName: event.target.value })} required /></label><label>Apellido<input value={form.lastName} onChange={(event) => setForm({ ...form, lastName: event.target.value })} required /></label><label>Cédula<input value={form.cedula} onChange={(event) => setForm({ ...form, cedula: event.target.value })} required /></label><label>Tipo<select value={form.staffType} onChange={(event) => setForm({ ...form, staffType: Number(event.target.value) })}>{staffTypes.slice(1).map((label, index) => <option key={label} value={index + 1}>{label}</option>)}</select></label><label>Especialidad<input value={form.specialty} onChange={(event) => setForm({ ...form, specialty: event.target.value })} /></label><label>Exequátur / licencia<input value={form.licenseNumber} onChange={(event) => setForm({ ...form, licenseNumber: event.target.value })} /></label><label>Tanda<input value={form.workShift} onChange={(event) => setForm({ ...form, workShift: event.target.value })} /></label><label>Teléfono<input value={form.phone} onChange={(event) => setForm({ ...form, phone: event.target.value })} /></label><button className="button button--primary form-span">Guardar personal</button></form></section></div>;
}

function AppointmentsPage() {
  const [items, setItems] = useState<Appointment[]>([]);
  const [patients, setPatients] = useState<Patient[]>([]);
  const [staff, setStaff] = useState<MedicalStaff[]>([]);
  const [error, setError] = useState<string | null>(null);
  const [form, setForm] = useState({ patientId: '', medicalStaffId: '', scheduledStartUtc: '', scheduledEndUtc: '', reason: '', notes: '' });
  const doctors = useMemo(() => staff.filter((member) => member.staffType === 1 && member.isActive), [staff]);

  async function load() {
    try {
      const [appointments, patientList, staffList] = await Promise.all([apiRequest<Appointment[]>('/api/appointments/'), apiRequest<Patient[]>('/api/patients/?includeInactive=false'), apiRequest<MedicalStaff[]>('/api/staff/?includeInactive=false')]);
      setItems(appointments); setPatients(patientList); setStaff(staffList); setError(null);
    } catch (caught) { setError(caught instanceof Error ? caught.message : 'No fue posible cargar la agenda.'); }
  }
  useEffect(() => { void load(); }, []);

  async function submit(event: FormEvent) {
    event.preventDefault();
    try {
      await apiRequest<Appointment>('/api/appointments/', { method: 'POST', body: JSON.stringify({ ...form, scheduledStartUtc: toUtc(form.scheduledStartUtc), scheduledEndUtc: toUtc(form.scheduledEndUtc) }) });
      setForm({ patientId: '', medicalStaffId: '', scheduledStartUtc: '', scheduledEndUtc: '', reason: '', notes: '' });
      await load();
    } catch (caught) { setError(caught instanceof Error ? caught.message : 'No fue posible crear la cita.'); }
  }

  async function changeStatus(id: string, status: number) { try { await apiRequest(`/api/appointments/${id}/status`, { method: 'PATCH', body: JSON.stringify({ status, cancellationReason: status === 5 ? 'Cancelada desde agenda' : null }) }); await load(); } catch (caught) { setError(caught instanceof Error ? caught.message : 'No fue posible cambiar el estado.'); } }

  return <div className="workspace-grid"><section className="panel"><div className="panel-heading"><div><p className="eyebrow">Fase 4</p><h2>Agenda y citas</h2></div><span className="counter">{items.length}</span></div><ErrorMessage message={error} /><div className="cards-list">{items.map((appointment) => <article className="appointment-card" key={appointment.id}><div><strong>{appointment.patientName}</strong><small>{appointment.medicalStaffName} · {appointment.specialty || 'Medicina general'}</small><time>{formatDate(appointment.scheduledStartUtc)}</time></div><div className="card-actions"><span className={`pill pill--status-${appointment.status}`}>{appointmentStatuses[appointment.status] ?? 'Estado'}</span>{appointment.status < 4 && <button className="button button--small" onClick={() => void changeStatus(appointment.id, appointment.status === 1 ? 2 : 3)}>Avanzar</button>}</div></article>)}</div></section>
    <section className="panel panel--form"><p className="eyebrow">Planificación</p><h2>Nueva cita</h2><p className="muted">MediCore impide cruces de horario para un mismo médico.</p><form onSubmit={submit} className="form-grid form-grid--single"><label>Paciente<select value={form.patientId} onChange={(event) => setForm({ ...form, patientId: event.target.value })} required><option value="">Seleccionar…</option>{patients.map((patient) => <option key={patient.id} value={patient.id}>{patient.fullName} · {patient.medicalRecordNumber}</option>)}</select></label><label>Médico<select value={form.medicalStaffId} onChange={(event) => setForm({ ...form, medicalStaffId: event.target.value })} required><option value="">Seleccionar…</option>{doctors.map((doctor) => <option key={doctor.id} value={doctor.id}>{doctor.fullName} · {doctor.specialty || 'Medicina general'}</option>)}</select></label><label>Inicio<input type="datetime-local" value={form.scheduledStartUtc} onChange={(event) => setForm({ ...form, scheduledStartUtc: event.target.value })} required /></label><label>Fin<input type="datetime-local" value={form.scheduledEndUtc} onChange={(event) => setForm({ ...form, scheduledEndUtc: event.target.value })} required /></label><label>Motivo<textarea value={form.reason} onChange={(event) => setForm({ ...form, reason: event.target.value })} required /></label><button className="button button--primary">Programar cita</button></form></section></div>;
}

function ConsultationsPage() {
  const [items, setItems] = useState<Consultation[]>([]);
  const [patients, setPatients] = useState<Patient[]>([]);
  const [staff, setStaff] = useState<MedicalStaff[]>([]);
  const [appointments, setAppointments] = useState<Appointment[]>([]);
  const [error, setError] = useState<string | null>(null);
  const [form, setForm] = useState({ patientId: '', medicalStaffId: '', appointmentId: '', consultationDateUtc: '', reason: '', symptoms: '', diagnosis: '', recommendations: '', bloodPressure: '', temperatureCelsius: '', heartRate: '', weightKg: '', heightCm: '' });
  const doctors = staff.filter((member) => member.staffType === 1 && member.isActive);

  async function load() {
    try {
      const [consultations, patientList, staffList, appointmentList] = await Promise.all([apiRequest<Consultation[]>('/api/consultations/'), apiRequest<Patient[]>('/api/patients/?includeInactive=false'), apiRequest<MedicalStaff[]>('/api/staff/?includeInactive=false'), apiRequest<Appointment[]>('/api/appointments/')]);
      setItems(consultations); setPatients(patientList); setStaff(staffList); setAppointments(appointmentList); setError(null);
    } catch (caught) { setError(caught instanceof Error ? caught.message : 'No fue posible cargar las consultas.'); }
  }
  useEffect(() => { void load(); }, []);

  async function submit(event: FormEvent) {
    event.preventDefault();
    try {
      const numberOrNull = (value: string) => value === '' ? null : Number(value);
      await apiRequest<Consultation>('/api/consultations/', { method: 'POST', body: JSON.stringify({ ...form, appointmentId: form.appointmentId || null, consultationDateUtc: toUtc(form.consultationDateUtc), temperatureCelsius: numberOrNull(form.temperatureCelsius), heartRate: numberOrNull(form.heartRate), weightKg: numberOrNull(form.weightKg), heightCm: numberOrNull(form.heightCm), notes: null }) });
      setForm({ patientId: '', medicalStaffId: '', appointmentId: '', consultationDateUtc: '', reason: '', symptoms: '', diagnosis: '', recommendations: '', bloodPressure: '', temperatureCelsius: '', heartRate: '', weightKg: '', heightCm: '' });
      await load();
    } catch (caught) { setError(caught instanceof Error ? caught.message : 'No fue posible registrar la consulta.'); }
  }

  async function complete(id: string) { try { await apiRequest(`/api/consultations/${id}/status`, { method: 'PATCH', body: JSON.stringify({ status: 2 }) }); await load(); } catch (caught) { setError(caught instanceof Error ? caught.message : 'No fue posible completar la consulta.'); } }

  return <div className="workspace-grid"><section className="panel"><div className="panel-heading"><div><p className="eyebrow">Fase 5</p><h2>Consultas e historia clínica</h2></div><span className="counter">{items.length}</span></div><ErrorMessage message={error} /><div className="cards-list">{items.map((consultation) => <article className="clinical-card" key={consultation.id}><div className="clinical-card__header"><div><strong>{consultation.patientName}</strong><small>{consultation.medicalRecordNumber} · {consultation.medicalStaffName}</small></div><span className={`pill pill--consult-${consultation.status}`}>{consultationStatuses[consultation.status]}</span></div><p><b>Motivo:</b> {consultation.reason}</p><p><b>Diagnóstico:</b> {consultation.diagnosis || 'Pendiente'}</p>{consultation.status === 1 && <button className="button button--small" onClick={() => void complete(consultation.id)}>Completar consulta</button>}</article>)}</div></section>
    <section className="panel panel--form"><p className="eyebrow">Atención clínica</p><h2>Nueva consulta</h2><form onSubmit={submit} className="form-grid"><label>Paciente<select value={form.patientId} onChange={(event) => setForm({ ...form, patientId: event.target.value })} required><option value="">Seleccionar…</option>{patients.map((patient) => <option key={patient.id} value={patient.id}>{patient.fullName}</option>)}</select></label><label>Médico<select value={form.medicalStaffId} onChange={(event) => setForm({ ...form, medicalStaffId: event.target.value })} required><option value="">Seleccionar…</option>{doctors.map((doctor) => <option key={doctor.id} value={doctor.id}>{doctor.fullName}</option>)}</select></label><label className="form-span">Cita vinculada<select value={form.appointmentId} onChange={(event) => setForm({ ...form, appointmentId: event.target.value })}><option value="">Consulta sin cita previa</option>{appointments.filter((appointment) => appointment.status < 4).map((appointment) => <option key={appointment.id} value={appointment.id}>{appointment.patientName} · {formatDate(appointment.scheduledStartUtc)}</option>)}</select></label><label>Fecha y hora<input type="datetime-local" value={form.consultationDateUtc} onChange={(event) => setForm({ ...form, consultationDateUtc: event.target.value })} required /></label><label>Presión arterial<input placeholder="120/80" value={form.bloodPressure} onChange={(event) => setForm({ ...form, bloodPressure: event.target.value })} /></label><label className="form-span">Motivo<textarea value={form.reason} onChange={(event) => setForm({ ...form, reason: event.target.value })} required /></label><label className="form-span">Síntomas<textarea value={form.symptoms} onChange={(event) => setForm({ ...form, symptoms: event.target.value })} /></label><label className="form-span">Diagnóstico<textarea value={form.diagnosis} onChange={(event) => setForm({ ...form, diagnosis: event.target.value })} /></label><label className="form-span">Recomendaciones<textarea value={form.recommendations} onChange={(event) => setForm({ ...form, recommendations: event.target.value })} /></label><button className="button button--primary form-span">Abrir consulta</button></form></section></div>;
}

function Overview({ apiState }: { apiState: ApiState }) {
  return <section className="overview"><div className="welcome-card"><p className="eyebrow">Core clínico</p><h2>Operación médica centralizada</h2><p>Las fases 1–5 conectan identidad, pacientes, personal, agenda y expediente clínico sobre la misma API y base de datos.</p><div className={`api-status api-status--${apiState}`}><span className="status-dot" /> API {apiState === 'checking' ? 'verificando' : apiState === 'online' ? 'en línea' : 'sin conexión'}</div></div><div className="feature-grid"><article><span>01</span><strong>Identidad</strong><small>JWT, refresh tokens y RBAC</small></article><article><span>02</span><strong>Pacientes</strong><small>Expediente y cédula dominicana</small></article><article><span>03</span><strong>Personal</strong><small>Directorio médico y especialidades</small></article><article><span>04</span><strong>Agenda</strong><small>Citas y conflictos de horario</small></article><article><span>05</span><strong>Historia clínica</strong><small>Consultas, diagnóstico y signos vitales</small></article></div></section>;
}

export default function App() {
  const [session, setSession] = useState<AuthResponse | null>(() => readSession());
  const [section, setSection] = useState<Section>('overview');
  const [apiState, setApiState] = useState<ApiState>('checking');

  useEffect(() => {
    const controller = new AbortController();
    fetch(`${apiBaseUrl}/api/health/live`, { signal: controller.signal }).then((response) => setApiState(response.ok ? 'online' : 'offline')).catch((error: unknown) => { if (!(error instanceof DOMException && error.name === 'AbortError')) setApiState('offline'); });
    return () => controller.abort();
  }, []);

  if (!session) return <LoginView onAuthenticated={setSession} />;

  const navigation: Array<{ id: Section; label: string; icon: string }> = [{ id: 'overview', label: 'Resumen', icon: '⌂' }, { id: 'patients', label: 'Pacientes', icon: '◉' }, { id: 'staff', label: 'Personal', icon: '✚' }, { id: 'appointments', label: 'Agenda', icon: '◷' }, { id: 'consultations', label: 'Consultas', icon: '▤' }];

  return <div className="app-shell"><aside className="sidebar"><div className="brand-inline brand-inline--sidebar"><span className="brand-mark">+</span><div><strong>Medi<span>Core</span></strong><small>Clinical Platform</small></div></div><nav>{navigation.map((item) => <button key={item.id} className={section === item.id ? 'nav-item nav-item--active' : 'nav-item'} onClick={() => setSection(item.id)}><span>{item.icon}</span>{item.label}</button>)}</nav><div className="sidebar-footer"><small>Sesión activa</small><strong>{session.user.fullName}</strong><span>{session.user.roles.join(', ')}</span><button className="button button--ghost" onClick={() => { clearSession(); setSession(null); }}>Cerrar sesión</button></div></aside><main className="content"><header className="topbar"><div><p className="eyebrow">MediCore · fases 1–5</p><h1>{navigation.find((item) => item.id === section)?.label}</h1></div><div className={`api-status api-status--${apiState}`}><span className="status-dot" />{apiState === 'online' ? 'API disponible' : apiState === 'checking' ? 'Verificando' : 'API sin conexión'}</div></header>{section === 'overview' && <Overview apiState={apiState} />}{section === 'patients' && <PatientsPage />}{section === 'staff' && <StaffPage />}{section === 'appointments' && <AppointmentsPage />}{section === 'consultations' && <ConsultationsPage />}</main></div>;
}
