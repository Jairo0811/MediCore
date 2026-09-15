import { FormEvent, useEffect, useState } from 'react';
import { apiRequest } from './api';
import type { DrugType, Medication, PharmaceuticalBrand, StorageLocation } from './types';
import './pharmacy.css';

type Props = {
  canManage: boolean;
};

type MedicationForm = {
  code: string;
  name: string;
  genericName: string;
  activeIngredient: string;
  strength: string;
  dosageForm: string;
  unitOfMeasure: string;
  drugTypeId: string;
  pharmaceuticalBrandId: string;
  storageLocationId: string;
  requiresPrescription: boolean;
  isControlledSubstance: boolean;
  notes: string;
};

const emptyMedication: MedicationForm = {
  code: '',
  name: '',
  genericName: '',
  activeIngredient: '',
  strength: '',
  dosageForm: '',
  unitOfMeasure: '',
  drugTypeId: '',
  pharmaceuticalBrandId: '',
  storageLocationId: '',
  requiresPrescription: false,
  isControlledSubstance: false,
  notes: '',
};

function ErrorMessage({ message }: { message: string | null }) {
  return message ? <div className="alert alert--error" role="alert">{message}</div> : null;
}

function ConfigHeader({ icon, title, description }: { icon: string; title: string; description: string }) {
  return <div className="pharmacy-config-card__header">
    <span className="pharmacy-config-card__icon" aria-hidden="true"><i className={icon} /></span>
    <div>
      <h3>{title}</h3>
      <p>{description}</p>
    </div>
  </div>;
}

export default function PharmacyPage({ canManage }: Props) {
  const [medications, setMedications] = useState<Medication[]>([]);
  const [drugTypes, setDrugTypes] = useState<DrugType[]>([]);
  const [brands, setBrands] = useState<PharmaceuticalBrand[]>([]);
  const [locations, setLocations] = useState<StorageLocation[]>([]);
  const [search, setSearch] = useState('');
  const [error, setError] = useState<string | null>(null);
  const [medicationForm, setMedicationForm] = useState<MedicationForm>(emptyMedication);
  const [editingMedicationId, setEditingMedicationId] = useState<string | null>(null);
  const [drugTypeForm, setDrugTypeForm] = useState({ name: '', description: '' });
  const [brandForm, setBrandForm] = useState({ name: '', manufacturerCountry: '', website: '' });
  const [locationForm, setLocationForm] = useState({ code: '', name: '', description: '' });

  async function load() {
    try {
      const medicationPath = search.trim()
        ? `/api/pharmacy/medications?search=${encodeURIComponent(search.trim())}&includeInactive=false`
        : '/api/pharmacy/medications?includeInactive=false';

      const [medicationList, drugTypeList, brandList, locationList] = await Promise.all([
        apiRequest<Medication[]>(medicationPath),
        apiRequest<DrugType[]>('/api/pharmacy/drug-types?includeInactive=false'),
        apiRequest<PharmaceuticalBrand[]>('/api/pharmacy/brands?includeInactive=false'),
        apiRequest<StorageLocation[]>('/api/pharmacy/locations?includeInactive=false'),
      ]);

      setMedications(medicationList);
      setDrugTypes(drugTypeList);
      setBrands(brandList);
      setLocations(locationList);
      setError(null);
    } catch (caught) {
      setError(caught instanceof Error ? caught.message : 'No fue posible cargar la farmacia.');
    }
  }

  useEffect(() => { void load(); }, []);

  async function createDrugType(event: FormEvent) {
    event.preventDefault();
    try {
      await apiRequest<DrugType>('/api/pharmacy/drug-types', {
        method: 'POST',
        body: JSON.stringify(drugTypeForm),
      });
      setDrugTypeForm({ name: '', description: '' });
      await load();
    } catch (caught) {
      setError(caught instanceof Error ? caught.message : 'No fue posible crear el tipo de fármaco.');
    }
  }

  async function createBrand(event: FormEvent) {
    event.preventDefault();
    try {
      await apiRequest<PharmaceuticalBrand>('/api/pharmacy/brands', {
        method: 'POST',
        body: JSON.stringify(brandForm),
      });
      setBrandForm({ name: '', manufacturerCountry: '', website: '' });
      await load();
    } catch (caught) {
      setError(caught instanceof Error ? caught.message : 'No fue posible crear la marca farmacéutica.');
    }
  }

  async function createLocation(event: FormEvent) {
    event.preventDefault();
    try {
      await apiRequest<StorageLocation>('/api/pharmacy/locations', {
        method: 'POST',
        body: JSON.stringify(locationForm),
      });
      setLocationForm({ code: '', name: '', description: '' });
      await load();
    } catch (caught) {
      setError(caught instanceof Error ? caught.message : 'No fue posible crear la ubicación.');
    }
  }

  function resetMedicationEditor() {
    setMedicationForm(emptyMedication);
    setEditingMedicationId(null);
  }

  function editMedication(medication: Medication) {
    setEditingMedicationId(medication.id);
    setMedicationForm({
      code: medication.code,
      name: medication.name,
      genericName: medication.genericName ?? '',
      activeIngredient: medication.activeIngredient ?? '',
      strength: medication.strength ?? '',
      dosageForm: medication.dosageForm ?? '',
      unitOfMeasure: medication.unitOfMeasure ?? '',
      drugTypeId: medication.drugTypeId,
      pharmaceuticalBrandId: medication.pharmaceuticalBrandId ?? '',
      storageLocationId: medication.storageLocationId ?? '',
      requiresPrescription: medication.requiresPrescription,
      isControlledSubstance: medication.isControlledSubstance,
      notes: medication.notes ?? '',
    });
    setError(null);
    window.requestAnimationFrame(() => {
      document.getElementById('medication-editor')?.scrollIntoView({ behavior: 'smooth', block: 'start' });
    });
  }

  async function saveMedication(event: FormEvent) {
    event.preventDefault();
    try {
      const payload = {
        ...medicationForm,
        pharmaceuticalBrandId: medicationForm.pharmaceuticalBrandId || null,
        storageLocationId: medicationForm.storageLocationId || null,
      };

      if (editingMedicationId) {
        await apiRequest<Medication>(`/api/pharmacy/medications/${editingMedicationId}`, {
          method: 'PUT',
          body: JSON.stringify({ ...payload, isActive: true }),
        });
      } else {
        await apiRequest<Medication>('/api/pharmacy/medications', {
          method: 'POST',
          body: JSON.stringify(payload),
        });
      }

      resetMedicationEditor();
      await load();
    } catch (caught) {
      setError(caught instanceof Error ? caught.message : editingMedicationId
        ? 'No fue posible actualizar el medicamento.'
        : 'No fue posible registrar el medicamento.');
    }
  }

  async function deactivateMedication(id: string) {
    try {
      await apiRequest(`/api/pharmacy/medications/${id}`, { method: 'DELETE' });
      if (editingMedicationId === id) resetMedicationEditor();
      await load();
    } catch (caught) {
      setError(caught instanceof Error ? caught.message : 'No fue posible desactivar el medicamento.');
    }
  }

  const medicationLabel = medications.length === 1 ? 'medicamento' : 'medicamentos';

  return (
    <div className="workspace-grid pharmacy-workspace">
      <section className="panel pharmacy-panel">
        <div className="panel-heading pharmacy-panel__heading">
          <div>
            <p className="eyebrow">Fase 6</p>
            <h2>Farmacia y medicamentos</h2>
            <p className="pharmacy-panel__subtitle">Consulta, gestiona y controla los medicamentos disponibles en la institución.</p>
          </div>
          <span className="pharmacy-count"><strong>{medications.length}</strong> {medicationLabel}</span>
        </div>

        <div className="toolbar pharmacy-toolbar">
          <div className="pharmacy-search">
            <i className="fa-solid fa-magnifying-glass" aria-hidden="true" />
            <input
              aria-label="Buscar medicamentos"
              placeholder="Buscar por código, nombre o principio activo..."
              value={search}
              onChange={(event) => setSearch(event.target.value)}
              onKeyDown={(event) => { if (event.key === 'Enter') void load(); }}
            />
          </div>
          <button className="button button--primary" onClick={() => void load()}>Buscar</button>
        </div>

        <ErrorMessage message={error} />

        <div className="table-wrap pharmacy-table-wrap">
          <table className="pharmacy-table">
            <thead>
              <tr>
                <th>Código</th>
                <th>Medicamento</th>
                <th>Tipo</th>
                <th>Marca</th>
                <th>Ubicación</th>
                <th>Control</th>
                {canManage && <th>Acción</th>}
              </tr>
            </thead>
            <tbody>
              {medications.map((medication) => (
                <tr key={medication.id} className={editingMedicationId === medication.id ? 'pharmacy-row--editing' : undefined}>
                  <td className="pharmacy-code">{medication.code}</td>
                  <td>
                    <strong>{medication.name}</strong>
                    <small>{medication.genericName || medication.activeIngredient || '—'} {medication.strength || ''}</small>
                  </td>
                  <td><span className="pharmacy-pill pharmacy-pill--type">{medication.drugTypeName}</span></td>
                  <td>{medication.pharmaceuticalBrandName || '—'}</td>
                  <td>{medication.storageLocationName || '—'}</td>
                  <td>
                    <span className={`pharmacy-pill ${medication.isControlledSubstance ? 'pharmacy-pill--controlled' : medication.requiresPrescription ? 'pharmacy-pill--prescription' : 'pharmacy-pill--free'}`}>
                      {medication.isControlledSubstance ? 'Controlado' : medication.requiresPrescription ? 'Receta' : 'Libre'}
                    </span>
                  </td>
                  {canManage && <td>
                    <div className="pharmacy-row-actions">
                      <button className="button button--small pharmacy-edit" onClick={() => editMedication(medication)}>
                        <i className="fa-solid fa-pen" aria-hidden="true" /> Editar
                      </button>
                      <button className="button button--small pharmacy-deactivate" onClick={() => void deactivateMedication(medication.id)}>
                        <i className="fa-solid fa-power-off" aria-hidden="true" /> Desactivar
                      </button>
                    </div>
                  </td>}
                </tr>
              ))}
            </tbody>
          </table>
        </div>

        {medications.length === 0
          ? <div className="empty-state pharmacy-empty-state">
              <i className="fa-solid fa-capsules" aria-hidden="true" />
              <strong>No hay medicamentos para mostrar</strong>
              <p>Crea el catálogo maestro y registra el primer medicamento de MediCore.</p>
            </div>
          : <div className="pharmacy-results">Mostrando {medications.length} de {medications.length} {medicationLabel}</div>}
      </section>

      <section className="panel panel--form pharmacy-config-panel">
        <p className="eyebrow">Catálogo maestro</p>
        <h2>Configuración farmacéutica</h2>
        <p className="muted pharmacy-config-panel__intro">La Fase 6 define los medicamentos y su clasificación. Lotes, existencias, vencimientos y kardex se implementan en la Fase 7.</p>

        {!canManage && <div className="alert">Tu rol tiene acceso de consulta. Las altas y modificaciones requieren `Administrator` o `Pharmacist`.</div>}

        {canManage && <div className="pharmacy-config-stack">
          <section className="pharmacy-config-card">
            <ConfigHeader icon="fa-solid fa-capsules" title="Tipo de fármaco" description="Clasifica el medicamento según su uso terapéutico." />
            <form onSubmit={createDrugType} className="form-grid">
              <label>Nombre *<input placeholder="Ej. Analgésico" value={drugTypeForm.name} onChange={(event) => setDrugTypeForm({ ...drugTypeForm, name: event.target.value })} required /></label>
              <label>Descripción<input placeholder="Ej. Alivia el dolor y la fiebre" value={drugTypeForm.description} onChange={(event) => setDrugTypeForm({ ...drugTypeForm, description: event.target.value })} /></label>
              <button className="button button--primary form-span"><i className="fa-solid fa-circle-plus" aria-hidden="true" /> Agregar tipo</button>
            </form>
          </section>

          <section className="pharmacy-config-card">
            <ConfigHeader icon="fa-solid fa-flask" title="Marca / laboratorio" description="Registra la marca comercial o laboratorio fabricante." />
            <form onSubmit={createBrand} className="form-grid pharmacy-brand-grid">
              <label>Nombre *<input placeholder="Ej. Genfar" value={brandForm.name} onChange={(event) => setBrandForm({ ...brandForm, name: event.target.value })} required /></label>
              <label>País<input placeholder="Ej. República Dominicana" value={brandForm.manufacturerCountry} onChange={(event) => setBrandForm({ ...brandForm, manufacturerCountry: event.target.value })} /></label>
              <label className="form-span">Sitio web<input placeholder="https://..." value={brandForm.website} onChange={(event) => setBrandForm({ ...brandForm, website: event.target.value })} /></label>
              <button className="button form-span"><i className="fa-solid fa-circle-plus" aria-hidden="true" /> Agregar marca</button>
            </form>
          </section>

          <section className="pharmacy-config-card">
            <ConfigHeader icon="fa-solid fa-location-dot" title="Ubicación" description="Define en qué lugar se almacena el medicamento." />
            <form onSubmit={createLocation} className="form-grid">
              <label>Código *<input placeholder="Ej. FAR-01" value={locationForm.code} onChange={(event) => setLocationForm({ ...locationForm, code: event.target.value })} required /></label>
              <label>Nombre *<input placeholder="Ej. Almacén principal" value={locationForm.name} onChange={(event) => setLocationForm({ ...locationForm, name: event.target.value })} required /></label>
              <label className="form-span">Descripción<input placeholder="Ej. Estante A1 · Farmacia principal" value={locationForm.description} onChange={(event) => setLocationForm({ ...locationForm, description: event.target.value })} /></label>
              <button className="button form-span"><i className="fa-solid fa-circle-plus" aria-hidden="true" /> Agregar ubicación</button>
            </form>
          </section>

          <section id="medication-editor" className={`pharmacy-config-card pharmacy-config-card--medication ${editingMedicationId ? 'pharmacy-config-card--editing' : ''}`}>
            <ConfigHeader
              icon={editingMedicationId ? 'fa-solid fa-pen-to-square' : 'fa-solid fa-prescription-bottle-medical'}
              title={editingMedicationId ? 'Editar medicamento' : 'Nuevo medicamento'}
              description={editingMedicationId ? 'Modifica la ficha del medicamento seleccionado y guarda los cambios.' : 'Completa la ficha farmacéutica y asígnala al catálogo maestro.'}
            />
            {editingMedicationId && <div className="pharmacy-editing-notice"><i className="fa-solid fa-circle-info" aria-hidden="true" /> Estás editando un medicamento existente.</div>}
            <form onSubmit={saveMedication} className="form-grid">
              <label>Código *<input placeholder="Ej. MED-001" value={medicationForm.code} onChange={(event) => setMedicationForm({ ...medicationForm, code: event.target.value })} required /></label>
              <label>Nombre comercial *<input placeholder="Ej. Paracetamol" value={medicationForm.name} onChange={(event) => setMedicationForm({ ...medicationForm, name: event.target.value })} required /></label>
              <label>Nombre genérico<input placeholder="Ej. Acetaminofén" value={medicationForm.genericName} onChange={(event) => setMedicationForm({ ...medicationForm, genericName: event.target.value })} /></label>
              <label>Principio activo<input placeholder="Ej. Paracetamol" value={medicationForm.activeIngredient} onChange={(event) => setMedicationForm({ ...medicationForm, activeIngredient: event.target.value })} /></label>
              <label>Concentración<input placeholder="Ej. 500 mg" value={medicationForm.strength} onChange={(event) => setMedicationForm({ ...medicationForm, strength: event.target.value })} /></label>
              <label>Forma farmacéutica<input placeholder="Ej. Tableta" value={medicationForm.dosageForm} onChange={(event) => setMedicationForm({ ...medicationForm, dosageForm: event.target.value })} /></label>
              <label>Unidad<input placeholder="Ej. Unidad" value={medicationForm.unitOfMeasure} onChange={(event) => setMedicationForm({ ...medicationForm, unitOfMeasure: event.target.value })} /></label>
              <label>Tipo *<select value={medicationForm.drugTypeId} onChange={(event) => setMedicationForm({ ...medicationForm, drugTypeId: event.target.value })} required><option value="">Seleccionar…</option>{drugTypes.map((item) => <option key={item.id} value={item.id}>{item.name}</option>)}</select></label>
              <label>Marca<select value={medicationForm.pharmaceuticalBrandId} onChange={(event) => setMedicationForm({ ...medicationForm, pharmaceuticalBrandId: event.target.value })}><option value="">Sin marca</option>{brands.map((item) => <option key={item.id} value={item.id}>{item.name}</option>)}</select></label>
              <label>Ubicación<select value={medicationForm.storageLocationId} onChange={(event) => setMedicationForm({ ...medicationForm, storageLocationId: event.target.value })}><option value="">Sin ubicación</option>{locations.map((item) => <option key={item.id} value={item.id}>{item.code} · {item.name}</option>)}</select></label>
              <label className="checkbox-field"><input type="checkbox" checked={medicationForm.requiresPrescription} onChange={(event) => setMedicationForm({ ...medicationForm, requiresPrescription: event.target.checked })} /> <span>Requiere receta</span></label>
              <label className="checkbox-field"><input type="checkbox" checked={medicationForm.isControlledSubstance} onChange={(event) => setMedicationForm({ ...medicationForm, isControlledSubstance: event.target.checked })} /> <span>Sustancia controlada</span></label>
              <label className="form-span">Notas<textarea value={medicationForm.notes} onChange={(event) => setMedicationForm({ ...medicationForm, notes: event.target.value })} /></label>
              <div className="form-span pharmacy-editor-actions">
                {editingMedicationId && <button type="button" className="button" onClick={resetMedicationEditor}><i className="fa-solid fa-xmark" aria-hidden="true" /> Cancelar</button>}
                <button className="button button--primary" disabled={drugTypes.length === 0}>
                  <i className={editingMedicationId ? 'fa-solid fa-floppy-disk' : 'fa-solid fa-circle-plus'} aria-hidden="true" />
                  {editingMedicationId ? 'Guardar cambios' : 'Guardar medicamento'}
                </button>
              </div>
            </form>
          </section>
        </div>}
      </section>
    </div>
  );
}
