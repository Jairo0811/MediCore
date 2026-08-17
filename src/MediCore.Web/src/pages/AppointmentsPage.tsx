import { FormEvent, useEffect, useMemo, useState } from 'react';
import { apiRequest } from '../api';
import ErrorMessage from '../components/ErrorMessage';
import type { Appointment, MedicalStaff, Patient } from '../types';
import { formatDate, toUtc } from '../utils/date';

const appointmentStatuses = ['No definido', 'Programada', 'Confirmada', 'En curso', 'Completada', 'Cancelada', 'No asistió'];

export default function AppointmentsPage() {
  const [items, setItems] = useState<Appointment[]>([]);
  const [patients, setPatients] = useState<Patient[]>([]);
  const [staff, setStaff] = useState<MedicalStaff[]>([]);
  const [error, setError] = useState<string | null>(null);
  const [form, setForm] = useState({ patientId: '', medicalStaffId: '', scheduledStartUtc: '', scheduledEndUtc: '', reason: '', notes: '' });
  const doctors = useMemo(() => staff.filter((member) => member.staffType === 1 && member.isActive), [staff]);

  async function load() {
    try {
      const [appointments, patientList, staffList] = await Promise.all([
        apiRequest<Appointment[]>('/api/appointments/'),
        apiRequest<Patient[]>('/api/patients/?includeInactive=false'),
        apiRequest<MedicalStaff[]>('/api/staff/?includeInactive=false'),
      ]);
      setItems(appointments);
      setPatients(patientList);
      setStaff(staffList);
      setError(null);
    } catch (caught) {
      setError(caught instanceof Error ? caught.message : 'No fue posible cargar la agenda.');
    }
  }

  useEffect(() => { void load(); }, []);

  async function submit(event: FormEvent) {
    event.preventDefault();
    try {
      await apiRequest<Appointment>('/api/appointments/', {
        method: 'POST',
        body: JSON.stringify({ ...form, scheduledStartUtc: toUtc(form.scheduledStartUtc), scheduledEndUtc: toUtc(form.scheduledEndUtc) }),
      });
      setForm({ patientId: '', medicalStaffId: '', scheduledStartUtc: '', scheduledEndUtc: '', reason: '', notes: '' });
      await load();
    } catch (caught) {
      setError(caught instanceof Error ? caught.message : 'No fue posible crear la cita.');
    }
  }

  async function changeStatus(id: string, status: number) {
    try {
      await apiRequest(`/api/appointments/${id}/status`, {
        method: 'PATCH',
        body: JSON.stringify({ status, cancellationReason: status === 5 ? 'Cancelada desde agenda' : null }),
      });
      await load();
    } catch (caught) {
      setError(caught instanceof Error ? caught.message : 'No fue posible cambiar el estado.');
    }
  }

  return <div className="workspace-grid">
    <section className="panel">
      <div className="panel-heading"><div><p className="eyebrow">Fase 4</p><h2>Agenda y citas</h2></div><span className="counter">{items.length}</span></div>
      <ErrorMessage message={error} />
      <div className="cards-list">{items.map((appointment) => <article className="appointment-card" key={appointment.id}><div><strong>{appointment.patientName}</strong><small>{appointment.medicalStaffName} · {appointment.specialty || 'Medicina general'}</small><time>{formatDate(appointment.scheduledStartUtc)}</time></div><div className="card-actions"><span className={`pill pill--status-${appointment.status}`}>{appointmentStatuses[appointment.status] ?? 'Estado'}</span>{appointment.status < 4 && <button className="button button--small" onClick={() => void changeStatus(appointment.id, appointment.status === 1 ? 2 : 3)}>Avanzar</button>}</div></article>)}</div>
    </section>
    <section className="panel panel--form">
      <p className="eyebrow">Planificación</p><h2>Nueva cita</h2><p className="muted">MediCore impide cruces de horario para un mismo médico.</p>
      <form onSubmit={submit} className="form-grid form-grid--single">
        <label>Paciente<select value={form.patientId} onChange={(event) => setForm({ ...form, patientId: event.target.value })} required><option value="">Seleccionar…</option>{patients.map((patient) => <option key={patient.id} value={patient.id}>{patient.fullName} · {patient.medicalRecordNumber}</option>)}</select></label>
        <label>Médico<select value={form.medicalStaffId} onChange={(event) => setForm({ ...form, medicalStaffId: event.target.value })} required><option value="">Seleccionar…</option>{doctors.map((doctor) => <option key={doctor.id} value={doctor.id}>{doctor.fullName} · {doctor.specialty || 'Medicina general'}</option>)}</select></label>
        <label>Inicio<input type="datetime-local" value={form.scheduledStartUtc} onChange={(event) => setForm({ ...form, scheduledStartUtc: event.target.value })} required /></label>
        <label>Fin<input type="datetime-local" value={form.scheduledEndUtc} onChange={(event) => setForm({ ...form, scheduledEndUtc: event.target.value })} required /></label>
        <label>Motivo<textarea value={form.reason} onChange={(event) => setForm({ ...form, reason: event.target.value })} required /></label>
        <button className="button button--primary">Programar cita</button>
      </form>
    </section>
  </div>;
}
