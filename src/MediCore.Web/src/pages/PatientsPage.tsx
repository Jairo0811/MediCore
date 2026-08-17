import { FormEvent, useEffect, useState } from 'react';
import { apiRequest } from '../api';
import ErrorMessage from '../components/ErrorMessage';
import type { Patient } from '../types';

const patientTypes = ['No definido', 'Estudiante', 'Empleado', 'Profesor', 'Otro'];

export default function PatientsPage() {
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

  return <div className="workspace-grid">
    <section className="panel">
      <div className="panel-heading"><div><p className="eyebrow">Fase 2</p><h2>Pacientes</h2></div><span className="counter">{items.length}</span></div>
      <div className="toolbar"><input placeholder="Buscar por nombre, cédula o expediente" value={search} onChange={(event) => setSearch(event.target.value)} onKeyDown={(event) => { if (event.key === 'Enter') void load(); }} /><button className="button" onClick={() => void load()}>Buscar</button></div>
      <ErrorMessage message={error} />
      <div className="table-wrap"><table><thead><tr><th>Expediente</th><th>Paciente</th><th>Cédula</th><th>Tipo</th><th>Contacto</th></tr></thead><tbody>{items.map((patient) => <tr key={patient.id}><td>{patient.medicalRecordNumber}</td><td><strong>{patient.fullName}</strong></td><td>{patient.cedula}</td><td>{patientTypes[patient.patientType] ?? 'Otro'}</td><td>{patient.phone || patient.email || '—'}</td></tr>)}</tbody></table></div>
    </section>
    <section className="panel panel--form">
      <p className="eyebrow">Nuevo registro</p><h2>Registrar paciente</h2><p className="muted">La cédula se valida en el backend con checksum Luhn y lista segura de excepciones.</p>
      <form onSubmit={submit} className="form-grid">
        <label>Nombre<input value={form.firstName} onChange={(event) => setForm({ ...form, firstName: event.target.value })} required /></label>
        <label>Apellido<input value={form.lastName} onChange={(event) => setForm({ ...form, lastName: event.target.value })} required /></label>
        <label>Cédula<input placeholder="000-0000000-0" value={form.cedula} onChange={(event) => setForm({ ...form, cedula: event.target.value })} required /></label>
        <label>Tipo<select value={form.patientType} onChange={(event) => setForm({ ...form, patientType: Number(event.target.value) })}><option value={1}>Estudiante</option><option value={2}>Empleado</option><option value={3}>Profesor</option><option value={4}>Otro</option></select></label>
        <label>Sexo<select value={form.sex} onChange={(event) => setForm({ ...form, sex: Number(event.target.value) })}><option value={0}>No especificado</option><option value={1}>Femenino</option><option value={2}>Masculino</option><option value={3}>Otro</option></select></label>
        <label>Fecha de nacimiento<input type="date" value={form.dateOfBirth} onChange={(event) => setForm({ ...form, dateOfBirth: event.target.value })} /></label>
        <label>Teléfono<input value={form.phone} onChange={(event) => setForm({ ...form, phone: event.target.value })} /></label>
        <label>Correo<input type="email" value={form.email} onChange={(event) => setForm({ ...form, email: event.target.value })} /></label>
        <button className="button button--primary form-span">Guardar paciente</button>
      </form>
    </section>
  </div>;
}
