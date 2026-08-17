import { FormEvent, useEffect, useState } from 'react';
import { apiRequest } from '../api';
import ErrorMessage from '../components/ErrorMessage';
import type { Appointment, Consultation, MedicalStaff, Patient } from '../types';
import { formatDate, toUtc } from '../utils/date';

const consultationStatuses = ['No definido', 'Borrador', 'Completada', 'Cancelada'];

export default function ConsultationsPage() {
  const [items, setItems] = useState<Consultation[]>([]);
  const [patients, setPatients] = useState<Patient[]>([]);
  const [staff, setStaff] = useState<MedicalStaff[]>([]);
  const [appointments, setAppointments] = useState<Appointment[]>([]);
  const [error, setError] = useState<string | null>(null);
  const [form, setForm] = useState({ patientId: '', medicalStaffId: '', appointmentId: '', consultationDateUtc: '', reason: '', symptoms: '', diagnosis: '', recommendations: '', bloodPressure: '', temperatureCelsius: '', heartRate: '', weightKg: '', heightCm: '' });
  const doctors = staff.filter((member) => member.staffType === 1 && member.isActive);

  async function load() {
    try {
      const [consultations, patientList, staffList, appointmentList] = await Promise.all([
        apiRequest<Consultation[]>('/api/consultations/'),
        apiRequest<Patient[]>('/api/patients/?includeInactive=false'),
        apiRequest<MedicalStaff[]>('/api/staff/?includeInactive=false'),
        apiRequest<Appointment[]>('/api/appointments/'),
      ]);
      setItems(consultations);
      setPatients(patientList);
      setStaff(staffList);
      setAppointments(appointmentList);
      setError(null);
    } catch (caught) {
      setError(caught instanceof Error ? caught.message : 'No fue posible cargar las consultas.');
    }
  }

  useEffect(() => { void load(); }, []);

  async function submit(event: FormEvent) {
    event.preventDefault();
    try {
      const numberOrNull = (value: string) => value === '' ? null : Number(value);
      await apiRequest<Consultation>('/api/consultations/', {
        method: 'POST',
        body: JSON.stringify({
          ...form,
          appointmentId: form.appointmentId || null,
          consultationDateUtc: toUtc(form.consultationDateUtc),
          temperatureCelsius: numberOrNull(form.temperatureCelsius),
          heartRate: numberOrNull(form.heartRate),
          weightKg: numberOrNull(form.weightKg),
          heightCm: numberOrNull(form.heightCm),
          notes: null,
        }),
      });
      setForm({ patientId: '', medicalStaffId: '', appointmentId: '', consultationDateUtc: '', reason: '', symptoms: '', diagnosis: '', recommendations: '', bloodPressure: '', temperatureCelsius: '', heartRate: '', weightKg: '', heightCm: '' });
      await load();
    } catch (caught) {
      setError(caught instanceof Error ? caught.message : 'No fue posible registrar la consulta.');
    }
  }

  async function complete(id: string) {
    try {
      await apiRequest(`/api/consultations/${id}/status`, { method: 'PATCH', body: JSON.stringify({ status: 2 }) });
      await load();
    } catch (caught) {
      setError(caught instanceof Error ? caught.message : 'No fue posible completar la consulta.');
    }
  }

  return <div className="workspace-grid">
    <section className="panel">
      <div className="panel-heading"><div><p className="eyebrow">Fase 5</p><h2>Consultas e historia clínica</h2></div><span className="counter">{items.length}</span></div>
      <ErrorMessage message={error} />
      <div className="cards-list">{items.map((consultation) => <article className="clinical-card" key={consultation.id}><div className="clinical-card__header"><div><strong>{consultation.patientName}</strong><small>{consultation.medicalRecordNumber} · {consultation.medicalStaffName}</small></div><span className={`pill pill--consult-${consultation.status}`}>{consultationStatuses[consultation.status]}</span></div><p><b>Motivo:</b> {consultation.reason}</p><p><b>Diagnóstico:</b> {consultation.diagnosis || 'Pendiente'}</p>{consultation.status === 1 && <button className="button button--small" onClick={() => void complete(consultation.id)}>Completar consulta</button>}</article>)}</div>
    </section>
    <section className="panel panel--form">
      <p className="eyebrow">Atención clínica</p><h2>Nueva consulta</h2>
      <form onSubmit={submit} className="form-grid">
        <label>Paciente<select value={form.patientId} onChange={(event) => setForm({ ...form, patientId: event.target.value })} required><option value="">Seleccionar…</option>{patients.map((patient) => <option key={patient.id} value={patient.id}>{patient.fullName}</option>)}</select></label>
        <label>Médico<select value={form.medicalStaffId} onChange={(event) => setForm({ ...form, medicalStaffId: event.target.value })} required><option value="">Seleccionar…</option>{doctors.map((doctor) => <option key={doctor.id} value={doctor.id}>{doctor.fullName}</option>)}</select></label>
        <label className="form-span">Cita vinculada<select value={form.appointmentId} onChange={(event) => setForm({ ...form, appointmentId: event.target.value })}><option value="">Consulta sin cita previa</option>{appointments.filter((appointment) => appointment.status < 4).map((appointment) => <option key={appointment.id} value={appointment.id}>{appointment.patientName} · {formatDate(appointment.scheduledStartUtc)}</option>)}</select></label>
        <label>Fecha y hora<input type="datetime-local" value={form.consultationDateUtc} onChange={(event) => setForm({ ...form, consultationDateUtc: event.target.value })} required /></label>
        <label>Presión arterial<input placeholder="120/80" value={form.bloodPressure} onChange={(event) => setForm({ ...form, bloodPressure: event.target.value })} /></label>
        <label className="form-span">Motivo<textarea value={form.reason} onChange={(event) => setForm({ ...form, reason: event.target.value })} required /></label>
        <label className="form-span">Síntomas<textarea value={form.symptoms} onChange={(event) => setForm({ ...form, symptoms: event.target.value })} /></label>
        <label className="form-span">Diagnóstico<textarea value={form.diagnosis} onChange={(event) => setForm({ ...form, diagnosis: event.target.value })} /></label>
        <label className="form-span">Recomendaciones<textarea value={form.recommendations} onChange={(event) => setForm({ ...form, recommendations: event.target.value })} /></label>
        <button className="button button--primary form-span">Abrir consulta</button>
      </form>
    </section>
  </div>;
}
