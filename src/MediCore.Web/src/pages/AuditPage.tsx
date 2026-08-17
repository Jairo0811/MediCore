import { useEffect, useState } from 'react';
import { apiRequest } from '../api';
import ErrorMessage from '../components/ErrorMessage';
import type { AuditLog } from '../types';

export default function AuditPage() {
  const [items, setItems] = useState<AuditLog[]>([]);
  const [entityName, setEntityName] = useState('');
  const [take, setTake] = useState(100);
  const [error, setError] = useState<string | null>(null);

  async function load() {
    try {
      const query = new URLSearchParams({ take: String(take) });
      if (entityName.trim()) query.set('entityName', entityName.trim());
      setItems(await apiRequest<AuditLog[]>(`/api/audit/logs?${query.toString()}`));
      setError(null);
    } catch (caught) { setError(caught instanceof Error ? caught.message : 'No fue posible cargar la auditoría.'); }
  }
  useEffect(() => { void load(); }, []);

  return <section className="panel"><div className="panel-heading"><div><p className="eyebrow">Fase 10</p><h2>Auditoría y trazabilidad</h2></div><span className="counter">{items.length}</span></div><div className="toolbar"><input placeholder="Entidad: patients, pharmacy, inventory…" value={entityName} onChange={(e) => setEntityName(e.target.value)} onKeyDown={(e) => { if (e.key === 'Enter') void load(); }} /><select value={take} onChange={(e) => setTake(Number(e.target.value))}><option value={50}>50</option><option value={100}>100</option><option value={250}>250</option><option value={500}>500</option></select><button className="button" onClick={() => void load()}>Filtrar</button></div><ErrorMessage message={error} /><div className="table-wrap"><table><thead><tr><th>Fecha UTC</th><th>Acción</th><th>Entidad</th><th>ID</th><th>Usuario</th><th>IP</th><th>Detalle</th></tr></thead><tbody>{items.map((item) => <tr key={item.id}><td>{new Date(item.createdAtUtc).toLocaleString('es-DO')}</td><td><strong>{item.action}</strong></td><td>{item.entityName}</td><td>{item.entityId || '—'}</td><td>{item.userId || 'Anónimo / sistema'}</td><td>{item.ipAddress || '—'}</td><td><small>{item.details || '—'}</small></td></tr>)}</tbody></table></div></section>;
}
