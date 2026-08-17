import { useEffect, useState } from 'react';
import { apiRequest } from '../api';
import ErrorMessage from '../components/ErrorMessage';
import type { DashboardSummary, InventoryAlert, OperationalReport } from '../types';

function dateValue(date: Date): string { return date.toISOString().slice(0, 10); }
const today = new Date();
const thirtyDaysAgo = new Date(today); thirtyDaysAgo.setDate(today.getDate() - 29);

export default function AnalyticsPage() {
  const [summary, setSummary] = useState<DashboardSummary | null>(null);
  const [alerts, setAlerts] = useState<InventoryAlert[]>([]);
  const [report, setReport] = useState<OperationalReport | null>(null);
  const [from, setFrom] = useState(dateValue(thirtyDaysAgo));
  const [to, setTo] = useState(dateValue(today));
  const [error, setError] = useState<string | null>(null);

  async function load() {
    try {
      const [dashboard, inventoryAlerts, operationalReport] = await Promise.all([
        apiRequest<DashboardSummary>('/api/analytics/dashboard'),
        apiRequest<InventoryAlert[]>('/api/analytics/inventory-alerts'),
        apiRequest<OperationalReport>(`/api/analytics/operational-report?from=${from}&to=${to}`),
      ]);
      setSummary(dashboard); setAlerts(inventoryAlerts); setReport(operationalReport); setError(null);
    } catch (caught) { setError(caught instanceof Error ? caught.message : 'No fue posible cargar la analítica.'); }
  }
  useEffect(() => { void load(); }, []);

  return <section className="overview"><div className="welcome-card"><div className="panel-heading"><div><p className="eyebrow">Fase 9</p><h2>Dashboard operacional</h2></div><button className="button" onClick={() => void load()}>Actualizar</button></div><p>Indicadores clínicos, farmacéuticos y de laboratorio calculados desde las fuentes transaccionales.</p><ErrorMessage message={error} /></div>{summary && <div className="feature-grid"><article><span>{summary.activePatients}</span><strong>Pacientes activos</strong><small>Expedientes habilitados</small></article><article><span>{summary.appointmentsToday}</span><strong>Citas hoy</strong><small>Agenda del día</small></article><article><span>{summary.consultations30Days}</span><strong>Consultas 30 días</strong><small>Actividad clínica</small></article><article><span>{summary.activeMedications}</span><strong>Medicamentos</strong><small>Catálogo activo</small></article><article><span>{summary.lowStockLots}</span><strong>Stock bajo</strong><small>Lotes en reposición</small></article><article><span>{summary.expiringLots30Days}</span><strong>Próximos a vencer</strong><small>30 días</small></article><article><span>{summary.pendingLabOrders}</span><strong>Laboratorio pendiente</strong><small>Órdenes abiertas</small></article><article><span>{summary.completedLabOrders30Days}</span><strong>Resultados 30 días</strong><small>Órdenes completadas</small></article></div>}<section className="panel"><div className="panel-heading"><div><p className="eyebrow">Reporte</p><h2>Resumen operacional por período</h2></div></div><div className="toolbar"><label>Desde<input type="date" value={from} onChange={(e) => setFrom(e.target.value)} /></label><label>Hasta<input type="date" value={to} onChange={(e) => setTo(e.target.value)} /></label><button className="button" onClick={() => void load()}>Generar</button></div>{report && <div className="feature-grid"><article><span>{report.appointments}</span><strong>Citas</strong><small>{report.from} → {report.to}</small></article><article><span>{report.consultations}</span><strong>Consultas</strong><small>{report.completedConsultations} completadas</small></article><article><span>{report.labOrders}</span><strong>Órdenes laboratorio</strong><small>{report.completedLabOrders} completadas</small></article><article><span>{report.inventoryMovements}</span><strong>Movimientos</strong><small>{report.inventoryUnitsMoved} unidades movilizadas</small></article></div>}</section><section className="panel"><div className="panel-heading"><div><p className="eyebrow">Alertas</p><h2>Inventario que requiere atención</h2></div><span className="counter">{alerts.length}</span></div><div className="table-wrap"><table><thead><tr><th>Medicamento</th><th>Lote</th><th>Existencia</th><th>Vencimiento</th><th>Alerta</th></tr></thead><tbody>{alerts.map((a) => <tr key={a.lotId}><td><strong>{a.medicationName}</strong><small>{a.medicationCode}</small></td><td>{a.lotNumber}</td><td>{a.quantityOnHand} / mín. {a.reorderPoint}</td><td>{a.expirationDate}</td><td><span className="pill">{a.alertType === 'LOW_STOCK' ? 'Stock bajo' : 'Próximo a vencer'}</span></td></tr>)}</tbody></table></div></section></section>;
}
