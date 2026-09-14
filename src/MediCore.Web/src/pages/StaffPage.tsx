import { FormEvent, useEffect, useState } from 'react';
import { apiRequest } from '../api';
import ErrorMessage from '../components/ErrorMessage';
import type { MedicalStaff } from '../types';
import { formatDominicanCedula, normalizeDominicanCedula, passesDominicanCedulaLuhn } from '../utils/cedula';

const staffTypes = ['No definido', 'Médico', 'Enfermería', 'Recepción', 'Farmacia', 'Laboratorio', 'Administrativo', 'Otro'];
const publicExequaturLookup = 'https://intranet.msp.gob.do/intranet/juridica/publica/consultaexequatur.aspx';
const exequaturApiPortal = 'https://digital.msp.gob.do/#/api/exequatur';

export default function StaffPage() {
  const [items, setItems] = useState<MedicalStaff[]>([]);
  const [error, setError] = useState<string | null>(null);
  const [form, setForm] = useState({ firstName: '', lastName: '', cedula: '', staffType: 1, specialty: '', licenseNumber: '', workShift: '', email: '', phone: '' });

  const cedulaDigits = normalizeDominicanCedula(form.cedula);
  const cedulaComplete = cedulaDigits.length === 11;
  const cedulaPassesLuhn = cedulaComplete && passesDominicanCedulaLuhn(form.cedula);
  const isDoctor = form.staffType === 1;

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

  function cedulaFeedback() {
    if (!cedulaDigits.length) {
      return <span id="staff-cedula-help" className="field-feedback field-feedback--neutral"><i className="fa-solid fa-circle-info" aria-hidden="true" />11 dígitos · validación OGTIC en backend.</span>;
    }
    if (!cedulaComplete) {
      return <span id="staff-cedula-help" className="field-feedback field-feedback--neutral"><i className="fa-solid fa-keyboard" aria-hidden="true" />Faltan {11 - cedulaDigits.length} dígitos.</span>;
    }
    if (cedulaPassesLuhn) {
      return <span id="staff-cedula-help" className="field-feedback field-feedback--valid"><i className="fa-solid fa-circle-check" aria-hidden="true" />La cédula supera la prevalidación Luhn.</span>;
    }
    return <span id="staff-cedula-help" className="field-feedback field-feedback--invalid"><i className="fa-solid fa-triangle-exclamation" aria-hidden="true" />No supera Luhn; el backend todavía revisará las excepciones seguras configuradas.</span>;
  }

  return <div className="workspace-grid">
    <section className="panel">
      <div className="panel-heading"><div><p className="eyebrow">Fase 3</p><h2>Médicos y personal</h2></div><span className="counter">{items.length}</span></div>
      <ErrorMessage message={error} />
      <div className="table-wrap"><table><thead><tr><th>Código</th><th>Nombre</th><th>Rol clínico</th><th>Especialidad</th><th>Exequátur</th><th>Tanda</th></tr></thead><tbody>{items.map((staff) => <tr key={staff.id}><td>{staff.employeeCode}</td><td><strong>{staff.fullName}</strong><small>{formatDominicanCedula(staff.cedula)}</small></td><td>{staffTypes[staff.staffType] ?? 'Otro'}</td><td>{staff.specialty || '—'}</td><td>{staff.licenseNumber || '—'}</td><td>{staff.workShift || '—'}</td></tr>)}</tbody></table></div>
      {items.length === 0 && !error && <div className="empty-state"><i className="fa-solid fa-user-doctor" aria-hidden="true" /><strong>Directorio clínico vacío</strong><p>Registra médicos y personal para habilitar la planificación de citas y consultas.</p></div>}
    </section>
    <section className="panel panel--form">
      <p className="eyebrow">Directorio</p><h2>Registrar personal</h2>
      <div className="info-strip"><i className="fa-solid fa-shield-heart" aria-hidden="true" /><span>La cédula se valida antes de crear el registro. Para médicos, MediCore facilita además la consulta oficial del exequátur en MISPAS.</span></div>
      <form onSubmit={submit} className="form-grid">
        <label>Nombre<input value={form.firstName} onChange={(event) => setForm({ ...form, firstName: event.target.value })} autoComplete="given-name" required /></label>
        <label>Apellido<input value={form.lastName} onChange={(event) => setForm({ ...form, lastName: event.target.value })} autoComplete="family-name" required /></label>
        <label>Cédula<input placeholder="000-0000000-0" value={form.cedula} onChange={(event) => setForm({ ...form, cedula: formatDominicanCedula(event.target.value) })} inputMode="numeric" maxLength={13} aria-describedby="staff-cedula-help" required />{cedulaFeedback()}</label>
        <label>Tipo<select value={form.staffType} onChange={(event) => setForm({ ...form, staffType: Number(event.target.value) })}>{staffTypes.slice(1).map((label, index) => <option key={label} value={index + 1}>{label}</option>)}</select></label>
        <label>Especialidad<input value={form.specialty} onChange={(event) => setForm({ ...form, specialty: event.target.value })} /></label>
        <label>Exequátur / licencia<input value={form.licenseNumber} onChange={(event) => setForm({ ...form, licenseNumber: event.target.value })} placeholder={isDoctor ? 'Número de exequátur' : 'Licencia / registro'} /></label>
        {isDoctor && <div className="verification-card">
          <div className="verification-card__header"><span className="verification-card__icon" aria-hidden="true"><i className="fa-solid fa-building-shield" /></span><div><h4>Verificación oficial de exequátur</h4><p>Consulta el registro público del Ministerio de Salud Pública. La validación automática requiere credenciales oficiales de la API y debe ejecutarse desde el backend, nunca exponiendo claves en el navegador.</p></div></div>
          <div className="verification-actions">
            <a className="button button--soft" href={publicExequaturLookup} target="_blank" rel="noreferrer"><i className="fa-solid fa-arrow-up-right-from-square" aria-hidden="true" /> Consultar en MISPAS</a>
            <a className="button button--external" href={exequaturApiPortal} target="_blank" rel="noreferrer"><i className="fa-solid fa-code" aria-hidden="true" /> API de exequátur</a>
          </div>
        </div>}
        <label>Tanda<input value={form.workShift} onChange={(event) => setForm({ ...form, workShift: event.target.value })} placeholder="Ej. Matutina" /></label>
        <label>Teléfono<input value={form.phone} onChange={(event) => setForm({ ...form, phone: event.target.value })} autoComplete="tel" /></label>
        <label className="form-span">Correo<input type="email" value={form.email} onChange={(event) => setForm({ ...form, email: event.target.value })} autoComplete="email" /></label>
        <button type="submit" className="button button--primary form-span"><i className="fa-solid fa-user-doctor" aria-hidden="true" /> Guardar personal</button>
      </form>
    </section>
  </div>;
}
