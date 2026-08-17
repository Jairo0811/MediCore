import { FormEvent, useEffect, useState } from 'react';
import { apiRequest } from './api';
import type { DrugType, Medication, PharmaceuticalBrand, StorageLocation } from './types';

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

export default function PharmacyPage({ canManage }: Props) {
  const [medications, setMedications] = useState<Medication[]>([]);
  const [drugTypes, setDrugTypes] = useState<DrugType[]>([]);
  const [brands, setBrands] = useState<PharmaceuticalBrand[]>([]);
  const [locations, setLocations] = useState<StorageLocation[]>([]);
  const [search, setSearch] = useState('');
  const [error, setError] = useState<string | null>(null);
  const [medicationForm, setMedicationForm] = useState<MedicationForm>(emptyMedication);
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

  async function createMedication(event: FormEvent) {
    event.preventDefault();
    try {
      await apiRequest<Medication>('/api/pharmacy/medications', {
        method: 'POST',
        body: JSON.stringify({
          ...medicationForm,
          pharmaceuticalBrandId: medicationForm.pharmaceuticalBrandId || null,
          storageLocationId: medicationForm.storageLocationId || null,
        }),
      });
      setMedicationForm(emptyMedication);
      await load();
    } catch (caught) {
      setError(caught instanceof Error ? caught.message : 'No fue posible registrar el medicamento.');
    }
  }

  async function deactivateMedication(id: string) {
    try {
      await apiRequest(`/api/pharmacy/medications/${id}`, { method: 'DELETE' });
      await load();
    } catch (caught) {
      setError(caught instanceof Error ? caught.message : 'No fue posible desactivar el medicamento.');
    }
  }

  return (
    <div className="workspace-grid">
      <section className="panel">
        <div className="panel-heading">
          <div><p className="eyebrow">Fase 6</p><h2>Farmacia y medicamentos</h2></div>
          <span className="counter">{medications.length}</span>
        </div>
        <div className="toolbar">
          <input
            placeholder="Buscar por código, nombre o principio activo"
            value={search}
            onChange={(event) => setSearch(event.target.value)}
            onKeyDown={(event) => { if (event.key === 'Enter') void load(); }}
          />
          <button className="button" onClick={() => void load()}>Buscar</button>
        </div>
        <ErrorMessage message={error} />
        <div className="table-wrap">
          <table>
            <thead><tr><th>Código</th><th>Medicamento</th><th>Tipo</th><th>Marca</th><th>Ubicación</th><th>Control</th>{canManage && <th>Acción</th>}</tr></thead>
            <tbody>
              {medications.map((medication) => (
                <tr key={medication.id}>
                  <td>{medication.code}</td>
                  <td><strong>{medication.name}</strong><small>{medication.genericName || medication.activeIngredient || '—'} {medication.strength || ''}</small></td>
                  <td>{medication.drugTypeName}</td>
                  <td>{medication.pharmaceuticalBrandName || '—'}</td>
                  <td>{medication.storageLocationName || '—'}</td>
                  <td>{medication.isControlledSubstance ? 'Controlado' : medication.requiresPrescription ? 'Receta' : 'Libre'}</td>
                  {canManage && <td><button className="button button--small" onClick={() => void deactivateMedication(medication.id)}>Desactivar</button></td>}
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </section>

      <section className="panel panel--form">
        <p className="eyebrow">Catálogo maestro</p>
        <h2>Configuración farmacéutica</h2>
        <p className="muted">La Fase 6 define los medicamentos y su clasificación. Lotes, existencias, vencimientos y kardex se implementan en la Fase 7.</p>

        {!canManage && <div className="alert">Tu rol tiene acceso de consulta. Las altas y modificaciones requieren `Administrator` o `Pharmacist`.</div>}

        {canManage && <>
          <h3>Tipo de fármaco</h3>
          <form onSubmit={createDrugType} className="form-grid">
            <label>Nombre<input value={drugTypeForm.name} onChange={(event) => setDrugTypeForm({ ...drugTypeForm, name: event.target.value })} required /></label>
            <label>Descripción<input value={drugTypeForm.description} onChange={(event) => setDrugTypeForm({ ...drugTypeForm, description: event.target.value })} /></label>
            <button className="button form-span">Agregar tipo</button>
          </form>

          <h3>Marca / laboratorio</h3>
          <form onSubmit={createBrand} className="form-grid">
            <label>Nombre<input value={brandForm.name} onChange={(event) => setBrandForm({ ...brandForm, name: event.target.value })} required /></label>
            <label>País<input value={brandForm.manufacturerCountry} onChange={(event) => setBrandForm({ ...brandForm, manufacturerCountry: event.target.value })} /></label>
            <label className="form-span">Sitio web<input value={brandForm.website} onChange={(event) => setBrandForm({ ...brandForm, website: event.target.value })} /></label>
            <button className="button form-span">Agregar marca</button>
          </form>

          <h3>Ubicación</h3>
          <form onSubmit={createLocation} className="form-grid">
            <label>Código<input value={locationForm.code} onChange={(event) => setLocationForm({ ...locationForm, code: event.target.value })} required /></label>
            <label>Nombre<input value={locationForm.name} onChange={(event) => setLocationForm({ ...locationForm, name: event.target.value })} required /></label>
            <label className="form-span">Descripción<input value={locationForm.description} onChange={(event) => setLocationForm({ ...locationForm, description: event.target.value })} /></label>
            <button className="button form-span">Agregar ubicación</button>
          </form>

          <h3>Nuevo medicamento</h3>
          <form onSubmit={createMedication} className="form-grid">
            <label>Código<input value={medicationForm.code} onChange={(event) => setMedicationForm({ ...medicationForm, code: event.target.value })} required /></label>
            <label>Nombre comercial<input value={medicationForm.name} onChange={(event) => setMedicationForm({ ...medicationForm, name: event.target.value })} required /></label>
            <label>Nombre genérico<input value={medicationForm.genericName} onChange={(event) => setMedicationForm({ ...medicationForm, genericName: event.target.value })} /></label>
            <label>Principio activo<input value={medicationForm.activeIngredient} onChange={(event) => setMedicationForm({ ...medicationForm, activeIngredient: event.target.value })} /></label>
            <label>Concentración<input value={medicationForm.strength} onChange={(event) => setMedicationForm({ ...medicationForm, strength: event.target.value })} /></label>
            <label>Forma farmacéutica<input value={medicationForm.dosageForm} onChange={(event) => setMedicationForm({ ...medicationForm, dosageForm: event.target.value })} /></label>
            <label>Unidad<input value={medicationForm.unitOfMeasure} onChange={(event) => setMedicationForm({ ...medicationForm, unitOfMeasure: event.target.value })} /></label>
            <label>Tipo<select value={medicationForm.drugTypeId} onChange={(event) => setMedicationForm({ ...medicationForm, drugTypeId: event.target.value })} required><option value="">Seleccionar…</option>{drugTypes.map((item) => <option key={item.id} value={item.id}>{item.name}</option>)}</select></label>
            <label>Marca<select value={medicationForm.pharmaceuticalBrandId} onChange={(event) => setMedicationForm({ ...medicationForm, pharmaceuticalBrandId: event.target.value })}><option value="">Sin marca</option>{brands.map((item) => <option key={item.id} value={item.id}>{item.name}</option>)}</select></label>
            <label>Ubicación<select value={medicationForm.storageLocationId} onChange={(event) => setMedicationForm({ ...medicationForm, storageLocationId: event.target.value })}><option value="">Sin ubicación</option>{locations.map((item) => <option key={item.id} value={item.id}>{item.code} · {item.name}</option>)}</select></label>
            <label><input type="checkbox" checked={medicationForm.requiresPrescription} onChange={(event) => setMedicationForm({ ...medicationForm, requiresPrescription: event.target.checked })} /> Requiere receta</label>
            <label><input type="checkbox" checked={medicationForm.isControlledSubstance} onChange={(event) => setMedicationForm({ ...medicationForm, isControlledSubstance: event.target.checked })} /> Sustancia controlada</label>
            <label className="form-span">Notas<textarea value={medicationForm.notes} onChange={(event) => setMedicationForm({ ...medicationForm, notes: event.target.value })} /></label>
            <button className="button button--primary form-span" disabled={drugTypes.length === 0}>Guardar medicamento</button>
          </form>
        </>}
      </section>
    </div>
  );
}
