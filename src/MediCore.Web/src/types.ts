export type SessionUser = {
  id: string;
  email: string;
  fullName: string;
  roles: string[];
};

export type AuthResponse = {
  accessToken: string;
  refreshToken: string;
  expiresAtUtc: string;
  user: SessionUser;
};

export type Patient = {
  id: string;
  medicalRecordNumber: string;
  firstName: string;
  lastName: string;
  fullName: string;
  cedula: string;
  patientType: number;
  sex: number;
  dateOfBirth?: string | null;
  email?: string | null;
  phone?: string | null;
  address?: string | null;
  emergencyContactName?: string | null;
  emergencyContactPhone?: string | null;
  isActive: boolean;
};

export type MedicalStaff = {
  id: string;
  employeeCode: string;
  firstName: string;
  lastName: string;
  fullName: string;
  cedula: string;
  staffType: number;
  specialty?: string | null;
  licenseNumber?: string | null;
  workShift?: string | null;
  email?: string | null;
  phone?: string | null;
  isActive: boolean;
};

export type Appointment = {
  id: string;
  patientId: string;
  patientName: string;
  medicalRecordNumber: string;
  medicalStaffId: string;
  medicalStaffName: string;
  specialty?: string | null;
  scheduledStartUtc: string;
  scheduledEndUtc: string;
  reason: string;
  notes?: string | null;
  status: number;
  cancellationReason?: string | null;
};

export type Consultation = {
  id: string;
  patientId: string;
  patientName: string;
  medicalRecordNumber: string;
  medicalStaffId: string;
  medicalStaffName: string;
  specialty?: string | null;
  appointmentId?: string | null;
  consultationDateUtc: string;
  reason: string;
  symptoms?: string | null;
  diagnosis?: string | null;
  recommendations?: string | null;
  notes?: string | null;
  bloodPressure?: string | null;
  temperatureCelsius?: number | null;
  heartRate?: number | null;
  weightKg?: number | null;
  heightCm?: number | null;
  status: number;
};
