import { FormEvent, useEffect, useState } from 'react';
import { apiRequest } from '../api';
import ErrorMessage from '../components/ErrorMessage';
import type { Patient } from '../types';
import { formatDominicanCedula, normalizeDominicanCedula, passesDominicanCedulaLuhn } from '../utils/cedula';

const patientTypes = ['No definido', 'Estudiante', 'Empleado', 'Profesor', 'Otro'];

export default function PatientsPage() {
  const [items, setItems] = useState<Patient[]>([]);
  const [error, setError] = useState<string | null>(null);
  const [search, setSearch] = useState('');
  const [form, setForm] = useState({ firstName: '', lastName: '', cedula: '', patientType: 1, sex: 0, dateOfBirth: '', email: '', phone: '' });

  const cedulaDigits = normalizeDominicanCedula(form.cedula);
  const cedulaComplete = cedulaDigits.length === 11;
  const cedulaPassesLuhn = cedulaComplete && passesDominicanCedulaLuhn(form.cedula);

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
      await apiRequest<Patient>('/api/patients/', {
        method: 'POST',
        body: JSON.stringify({ ...form, dateOfBirth: form.dateOfBirth || null, address: null, emergencyContactName: null, emergencyContactPhone: null }),
      });
      setForm({ firstName: '', lastName: '', cedula: '', patientType: 1, sex: 0, dateOfBirth: '', email: '', phone: '' });
      await load();
    } catch (caught) {
      setError(caught instanceof Error ? caught.message : 'No fue posible registrar el paciente.');
    }
  }

  function cedulaFeedback() {
    if (!cedulaDigits.length) {
      return <span id="patient-cedula-help" className="field-feedback field-feedback--neutral"><i className="fa-solid fa-circle-info" aria-hidden="true" />11 dígitos · Luhn OGTIC y excepciones seguras verificadas en el backend.</span>;
    }
    if (!cedulaComplete) {
      return <span id="patient-cedula-help" className="field-feedback field-feedback--neutral"><i className="fa-solid fa-keyboard" aria-hidden="true" />Faltan {11 - cedulaDigits.length} dígitos.</span>;
    }
    if (cedulaPassesLuhn) {
      return <span id="patient-cedula-help" className="field-feedback field-feedback--valid"><i className="fa-solid fa-circle-check" aria-hidden="true" />La cédula supera la prevalidación Luhn.</span>;
    }
    return <span id="patient-cedula-help" className="field-feedback field-feedback--invalid"><i className="fa-solid fa-triangle-exclamation" aria-hidden="true" />No supera Luhn; al guardar, el backend comprobará también las excepciones seguras configuradas.</span>;
  }

  return <div className="workspace-grid">
    <section className="panel">
      <div className="panel-heading"><div><p className="eyebrow">Fase 2</p><h2>Pacientes</h2></div><span className="counter">{items.length}</span></div>
      <div className="toolbar"><input placeholder="Buscar por nombre, cédula o expediente" value={search} onChange={(event) => setSearch(event.target.value)} onKeyDown={(event) => { if (event.key === 'Enter') void load(); }} /><button type="button" className="button" onClick={() => void load()}><i className="fa-solid fa-magnifying-glass" aria-hidden="true" /> Buscar</button></div>
      <ErrorMessage message={error} />
      <div className="table-wrap"><table><thead><tr><th>Expediente</th><th>Paciente</th><th>Cédula</th><th>Tipo</th><th>Contacto</th></tr></thead><tbody>{items.map((patient) => <tr key={patient.id}><td>{patient.medicalRecordNumber}</td><td><strong>{patient.fullName}</strong></td><td>{formatDominicanCedula(patient.cedula)}</td><td>{patientTypes[patient.patientType] ?? 'Otro'}</td><td>{patient.phone || patient.email || '—'}</td></tr>)}</tbody></table></div>
      {items.length === 0 && !error && <div className="empty-state"><i className="fa-solid fa-hospital-user" aria-hidden="true" /><strong>Sin pacientes registrados</strong><p>El expediente clínico comenzará cuando registres el primer paciente.</p></div>}
    </section>
    <section className="panel panel--form">
      <p className="eyebrow">Nuevo registro</p><h2>Registrar paciente</h2>
      <div className="info-strip"><i className="fa-solid fa-id-card" aria-hidden="true" /><span>MediCore previsualiza la validación de cédula en el formulario y confirma la regla completa en el backend antes de registrar.</span></div>
      <form onSubmit={submit} className="form-grid">
        <label>Nombre<input value={form.firstName} onChange={(event) => setForm({ ...form, firstName: event.target.value })} autoComplete="given-name" required /></label>
        <label>Apellido<input value={form.lastName} onChange={(event) => setForm({ ...form, lastName: event.target.value })} autoComplete="family-name" required /></label>
        <label>Cédula<input placeholder="000-0000000-0" value={form.cedula} onChange={(event) => setForm({ ...form, cedula: formatDominicanCedula(event.target.value) })} inputMode="numeric" maxLength={13} aria-describedby="patient-cedula-help" required />{cedulaFeedback()}</label>
        <label>Tipo<select value={form.patientType} onChange={(event) => setForm({ ...form, patientType: Number(event.target.value) })}><option value={1}>Estudiante</option><option value={2}>Empleado</option><option value={3}>Profesor</option><option value={4}>Otro</option></select></label>
        <label>Sexo<select value={form.sex} onChange={(event) => setForm({ ...form, sex: Number(event.target.value) })}><option value={0}>No especificado</option><option value={1}>Femenino</option><option value={2}>Masculino</option><option value={3}>Otro</option></select></label>
        <label>Fecha de nacimiento<input type="date" value={form.dateOfBirth} onChange={(event) => setForm({ ...form, dateOfBirth: event.target.value })} /></label>
        <label>Teléfono<input value={form.phone} onChange={(event) => setForm({ ...form, phone: event.target.value })} autoComplete="tel" /></label>
        <label>Correo<input type="email" value={form.email} onChange={(event) => setForm({ ...form, email: event.target.value })} autoComplete="email" /></label>
        <button type="submit" className="button button--primary form-span"><i className="fa-solid fa-user-plus" aria-hidden="true" /> Guardar paciente</button>
      </form>
    </section>
  </div>;
}
