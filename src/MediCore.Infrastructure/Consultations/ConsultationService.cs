using MediCore.Application.Common;
using MediCore.Application.Consultations;
using MediCore.Domain.Appointments;
using MediCore.Domain.Consultations;
using MediCore.Domain.Staff;
using MediCore.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MediCore.Infrastructure.Consultations;

public sealed class ConsultationService(MediCoreDbContext dbContext) : IConsultationService
{
    public async Task<IReadOnlyCollection<ConsultationResponse>> GetAllAsync(
        Guid? patientId,
        Guid? medicalStaffId,
        DateTime? fromUtc,
        DateTime? toUtc,
        CancellationToken cancellationToken)
    {
        var query = BaseQuery();

        if (patientId.HasValue)
        {
            query = query.Where(consultation => consultation.PatientId == patientId.Value);
        }

        if (medicalStaffId.HasValue)
        {
            query = query.Where(consultation => consultation.MedicalStaffId == medicalStaffId.Value);
        }

        if (fromUtc.HasValue)
        {
            query = query.Where(consultation => consultation.ConsultationDateUtc >= fromUtc.Value);
        }

        if (toUtc.HasValue)
        {
            query = query.Where(consultation => consultation.ConsultationDateUtc <= toUtc.Value);
        }

        var consultations = await query
            .OrderByDescending(consultation => consultation.ConsultationDateUtc)
            .ToArrayAsync(cancellationToken);

        return consultations.Select(Map).ToArray();
    }

    public async Task<ConsultationResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var consultation = await BaseQuery()
            .SingleOrDefaultAsync(candidate => candidate.Id == id, cancellationToken);

        return consultation is null ? null : Map(consultation);
    }

    public async Task<ClinicalHistoryResponse?> GetClinicalHistoryAsync(
        Guid patientId,
        CancellationToken cancellationToken)
    {
        var patient = await dbContext.Patients
            .AsNoTracking()
            .SingleOrDefaultAsync(candidate => candidate.Id == patientId, cancellationToken);

        if (patient is null)
        {
            return null;
        }

        var consultations = await BaseQuery()
            .Where(consultation => consultation.PatientId == patientId &&
                consultation.Status == ConsultationStatus.Completed)
            .OrderByDescending(consultation => consultation.ConsultationDateUtc)
            .ToArrayAsync(cancellationToken);

        return new ClinicalHistoryResponse(
            patient.Id,
            patient.FullName,
            patient.MedicalRecordNumber,
            consultations.Select(Map).ToArray());
    }

    public async Task<OperationResult<ConsultationResponse>> CreateAsync(
        CreateConsultationRequest request,
        CancellationToken cancellationToken)
    {
        var validation = await ValidateRelationshipsAsync(
            request.PatientId,
            request.MedicalStaffId,
            request.AppointmentId,
            cancellationToken);

        if (!validation.Succeeded)
        {
            return OperationResult<ConsultationResponse>.Failure(
                validation.ErrorCode ?? "validation_error",
                validation.ErrorMessage ?? "La consulta no es válida.");
        }

        if (string.IsNullOrWhiteSpace(request.Reason))
        {
            return OperationResult<ConsultationResponse>.Failure(
                "reason_required",
                "El motivo de consulta es obligatorio.");
        }

        var consultation = new Consultation(
            request.PatientId,
            request.MedicalStaffId,
            request.AppointmentId,
            request.ConsultationDateUtc,
            request.Reason.Trim(),
            NormalizeOptional(request.Symptoms),
            NormalizeOptional(request.Diagnosis),
            NormalizeOptional(request.Recommendations),
            NormalizeOptional(request.Notes),
            NormalizeOptional(request.BloodPressure),
            request.TemperatureCelsius,
            request.HeartRate,
            request.WeightKg,
            request.HeightCm);

        dbContext.Consultations.Add(consultation);

        if (request.AppointmentId.HasValue)
        {
            var appointment = await dbContext.Appointments
                .SingleAsync(item => item.Id == request.AppointmentId.Value, cancellationToken);
            appointment.ChangeStatus(AppointmentStatus.InProgress);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        await LoadReferencesAsync(consultation, cancellationToken);
        return OperationResult<ConsultationResponse>.Success(Map(consultation));
    }

    public async Task<OperationResult<ConsultationResponse>> UpdateAsync(
        Guid id,
        UpdateConsultationRequest request,
        CancellationToken cancellationToken)
    {
        var consultation = await dbContext.Consultations
            .Include(item => item.Patient)
            .Include(item => item.MedicalStaff)
            .SingleOrDefaultAsync(item => item.Id == id, cancellationToken);

        if (consultation is null)
        {
            return OperationResult<ConsultationResponse>.Failure("not_found", "Consulta no encontrada.");
        }

        if (consultation.Status != ConsultationStatus.Draft)
        {
            return OperationResult<ConsultationResponse>.Failure(
                "consultation_closed",
                "Solo las consultas en borrador pueden modificarse.");
        }

        if (string.IsNullOrWhiteSpace(request.Reason))
        {
            return OperationResult<ConsultationResponse>.Failure(
                "reason_required",
                "El motivo de consulta es obligatorio.");
        }

        consultation.Update(
            request.ConsultationDateUtc,
            request.Reason.Trim(),
            NormalizeOptional(request.Symptoms),
            NormalizeOptional(request.Diagnosis),
            NormalizeOptional(request.Recommendations),
            NormalizeOptional(request.Notes),
            NormalizeOptional(request.BloodPressure),
            request.TemperatureCelsius,
            request.HeartRate,
            request.WeightKg,
            request.HeightCm);

        await dbContext.SaveChangesAsync(cancellationToken);
        return OperationResult<ConsultationResponse>.Success(Map(consultation));
    }

    public async Task<OperationResult<ConsultationResponse>> ChangeStatusAsync(
        Guid id,
        ChangeConsultationStatusRequest request,
        CancellationToken cancellationToken)
    {
        var consultation = await dbContext.Consultations
            .Include(item => item.Patient)
            .Include(item => item.MedicalStaff)
            .SingleOrDefaultAsync(item => item.Id == id, cancellationToken);

        if (consultation is null)
        {
            return OperationResult<ConsultationResponse>.Failure("not_found", "Consulta no encontrada.");
        }

        if (request.Status == ConsultationStatus.Completed)
        {
            if (string.IsNullOrWhiteSpace(consultation.Diagnosis))
            {
                return OperationResult<ConsultationResponse>.Failure(
                    "diagnosis_required",
                    "Debe registrar un diagnóstico antes de completar la consulta.");
            }

            consultation.Complete();
            if (consultation.AppointmentId.HasValue)
            {
                var appointment = await dbContext.Appointments
                    .SingleOrDefaultAsync(
                        item => item.Id == consultation.AppointmentId.Value,
                        cancellationToken);
                appointment?.ChangeStatus(AppointmentStatus.Completed);
            }
        }
        else if (request.Status == ConsultationStatus.Cancelled)
        {
            consultation.Cancel();
        }
        else
        {
            return OperationResult<ConsultationResponse>.Failure(
                "invalid_status_transition",
                "La consulta solo puede completarse o cancelarse desde este endpoint.");
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return OperationResult<ConsultationResponse>.Success(Map(consultation));
    }

    private IQueryable<Consultation> BaseQuery() =>
        dbContext.Consultations
            .AsNoTracking()
            .Include(consultation => consultation.Patient)
            .Include(consultation => consultation.MedicalStaff);

    private async Task<OperationResult<bool>> ValidateRelationshipsAsync(
        Guid patientId,
        Guid medicalStaffId,
        Guid? appointmentId,
        CancellationToken cancellationToken)
    {
        var patientExists = await dbContext.Patients.AnyAsync(
            patient => patient.Id == patientId && patient.IsActive,
            cancellationToken);

        if (!patientExists)
        {
            return OperationResult<bool>.Failure("patient_not_found", "Paciente activo no encontrado.");
        }

        var doctorExists = await dbContext.MedicalStaff.AnyAsync(
            staff => staff.Id == medicalStaffId &&
                staff.IsActive &&
                staff.StaffType == StaffType.Doctor,
            cancellationToken);

        if (!doctorExists)
        {
            return OperationResult<bool>.Failure("doctor_not_found", "Médico activo no encontrado.");
        }

        if (!appointmentId.HasValue)
        {
            return OperationResult<bool>.Success(true);
        }

        var appointment = await dbContext.Appointments
            .AsNoTracking()
            .SingleOrDefaultAsync(item => item.Id == appointmentId.Value, cancellationToken);

        if (appointment is null)
        {
            return OperationResult<bool>.Failure("appointment_not_found", "Cita no encontrada.");
        }

        if (appointment.PatientId != patientId || appointment.MedicalStaffId != medicalStaffId)
        {
            return OperationResult<bool>.Failure(
                "appointment_mismatch",
                "La cita no corresponde al paciente y médico seleccionados.");
        }

        if (appointment.Status is AppointmentStatus.Cancelled or AppointmentStatus.NoShow or AppointmentStatus.Completed)
        {
            return OperationResult<bool>.Failure(
                "appointment_closed",
                "No se puede iniciar una consulta desde una cita cerrada.");
        }

        return OperationResult<bool>.Success(true);
    }

    private async Task LoadReferencesAsync(
        Consultation consultation,
        CancellationToken cancellationToken)
    {
        await dbContext.Entry(consultation).Reference(item => item.Patient).LoadAsync(cancellationToken);
        await dbContext.Entry(consultation).Reference(item => item.MedicalStaff).LoadAsync(cancellationToken);
    }

    private static string? NormalizeOptional(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static ConsultationResponse Map(Consultation consultation) =>
        new(
            consultation.Id,
            consultation.PatientId,
            consultation.Patient.FullName,
            consultation.Patient.MedicalRecordNumber,
            consultation.MedicalStaffId,
            consultation.MedicalStaff.FullName,
            consultation.MedicalStaff.Specialty,
            consultation.AppointmentId,
            consultation.ConsultationDateUtc,
            consultation.Reason,
            consultation.Symptoms,
            consultation.Diagnosis,
            consultation.Recommendations,
            consultation.Notes,
            consultation.BloodPressure,
            consultation.TemperatureCelsius,
            consultation.HeartRate,
            consultation.WeightKg,
            consultation.HeightCm,
            consultation.Status,
            consultation.CreatedAtUtc,
            consultation.UpdatedAtUtc);
}
