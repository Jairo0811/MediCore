import { FormEvent, useEffect, useState } from 'react';
import { apiRequest } from '../api';
import ErrorMessage from '../components/ErrorMessage';
import type { MedicalStaff } from '../types';

const staffTypes = ['No definido', 'Médico', 'Enfermería', 'Recepción', 'Farmacia', 'Laboratorio', 'Administrativo', 'Otro'];

export default function StaffPage() {
  const [items, setItems] = useState<MedicalStaff[]>([]);
  const [error, setError] = useState<string | null>(null);
  const [form, setForm] = useState({ firstName: '', lastName: '', cedula: '', staffType: 1, specialty: '', licenseNumber: '', workShift: '', email: '', phone: '' });

  async function load() {
    try {
      setItems(await apiRequest<MedicalStaff[]>('/api/staff/?includeInactive=false'));
      setError(null);
    } catch (caught) {
      setError(caught instanceof Error ? caught.message : 'No fue posible cargar el personal.');
    }
  }

  useEffect(() => { void load(); }, []);

  async function submit(event: FormEvent) {
    event.preventDefault();
    try {
      await apiRequest<MedicalStaff>('/api/staff/', { method: 'POST', body: JSON.stringify(form) });
      setForm({ firstName: '', lastName: '', cedula: '', staffType: 1, specialty: '', licenseNumber: '', workShift: '', email: '', phone: '' });
      await load();
    } catch (caught) {
      setError(caught instanceof Error ? caught.message : 'No fue posible registrar el personal.');
    }
  }

  return <div className="workspace-grid">
    <section className="panel"><div className="panel-heading"><div><p className="eyebrow">Fase 3</p><h2>Médicos y personal</h2></div><span className="counter">{items.length}</span></div><ErrorMessage message={error} /><div className="table-wrap"><table><thead><tr><th>Código</th><th>Nombre</th><th>Rol clínico</th><th>Especialidad</th><th>Tanda</th></tr></thead><tbody>{items.map((staff) => <tr key={staff.id}><td>{staff.employeeCode}</td><td><strong>{staff.fullName}</strong><small>{staff.cedula}</small></td><td>{staffTypes[staff.staffType] ?? 'Otro'}</td><td>{staff.specialty || '—'}</td><td>{staff.workShift || '—'}</td></tr>)}</tbody></table></div></section>
    <section className="panel panel--form"><p className="eyebrow">Directorio</p><h2>Registrar personal</h2><form onSubmit={submit} className="form-grid">
      <label>Nombre<input value={form.firstName} onChange={(event) => setForm({ ...form, firstName: event.target.value })} required /></label><label>Apellido<input value={form.lastName} onChange={(event) => setForm({ ...form, lastName: event.target.value })} required /></label><label>Cédula<input value={form.cedula} onChange={(event) => setForm({ ...form, cedula: event.target.value })} required /></label><label>Tipo<select value={form.staffType} onChange={(event) => setForm({ ...form, staffType: Number(event.target.value) })}>{staffTypes.slice(1).map((label, index) => <option key={label} value={index + 1}>{label}</option>)}</select></label><label>Especialidad<input value={form.specialty} onChange={(event) => setForm({ ...form, specialty: event.target.value })} /></label><label>Exequátur / licencia<input value={form.licenseNumber} onChange={(event) => setForm({ ...form, licenseNumber: event.target.value })} /></label><label>Tanda<input value={form.workShift} onChange={(event) => setForm({ ...form, workShift: event.target.value })} /></label><label>Teléfono<input value={form.phone} onChange={(event) => setForm({ ...form, phone: event.target.value })} /></label><button className="button button--primary form-span">Guardar personal</button>
    </form></section>
  </div>;
}
