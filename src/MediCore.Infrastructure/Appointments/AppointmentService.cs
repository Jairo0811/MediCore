using MediCore.Application.Appointments;
using MediCore.Application.Common;
using MediCore.Domain.Appointments;
using MediCore.Domain.Staff;
using MediCore.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MediCore.Infrastructure.Appointments;

public sealed class AppointmentService(MediCoreDbContext dbContext) : IAppointmentService
{
    public async Task<IReadOnlyCollection<AppointmentResponse>> GetAllAsync(
        DateTime? fromUtc,
        DateTime? toUtc,
        Guid? patientId,
        Guid? medicalStaffId,
        CancellationToken cancellationToken)
    {
        var query = dbContext.Appointments
            .AsNoTracking()
            .Include(appointment => appointment.Patient)
            .Include(appointment => appointment.MedicalStaff)
            .AsQueryable();

        if (fromUtc.HasValue)
        {
            query = query.Where(appointment => appointment.ScheduledStartUtc >= fromUtc.Value);
        }

        if (toUtc.HasValue)
        {
            query = query.Where(appointment => appointment.ScheduledStartUtc <= toUtc.Value);
        }

        if (patientId.HasValue)
        {
            query = query.Where(appointment => appointment.PatientId == patientId.Value);
        }

        if (medicalStaffId.HasValue)
        {
            query = query.Where(appointment => appointment.MedicalStaffId == medicalStaffId.Value);
        }

        var appointments = await query
            .OrderBy(appointment => appointment.ScheduledStartUtc)
            .ToArrayAsync(cancellationToken);

        return appointments.Select(Map).ToArray();
    }

    public async Task<AppointmentResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var appointment = await dbContext.Appointments
            .AsNoTracking()
            .Include(item => item.Patient)
            .Include(item => item.MedicalStaff)
            .SingleOrDefaultAsync(item => item.Id == id, cancellationToken);

        return appointment is null ? null : Map(appointment);
    }

    public async Task<OperationResult<AppointmentResponse>> CreateAsync(
        CreateAppointmentRequest request,
        CancellationToken cancellationToken)
    {
        var validation = await ValidateAsync(
            request.PatientId,
            request.MedicalStaffId,
            request.ScheduledStartUtc,
            request.ScheduledEndUtc,
            null,
            cancellationToken);

        if (!validation.Succeeded)
        {
            return OperationResult<AppointmentResponse>.Failure(
                validation.ErrorCode ?? "validation_error",
                validation.ErrorMessage ?? "La cita no es válida.");
        }

        if (string.IsNullOrWhiteSpace(request.Reason))
        {
            return OperationResult<AppointmentResponse>.Failure(
                "reason_required",
                "El motivo de la cita es obligatorio.");
        }

        var appointment = new Appointment(
            request.PatientId,
            request.MedicalStaffId,
            request.ScheduledStartUtc,
            request.ScheduledEndUtc,
            request.Reason.Trim(),
            NormalizeOptional(request.Notes));

        dbContext.Appointments.Add(appointment);
        await dbContext.SaveChangesAsync(cancellationToken);

        await dbContext.Entry(appointment).Reference(item => item.Patient).LoadAsync(cancellationToken);
        await dbContext.Entry(appointment).Reference(item => item.MedicalStaff).LoadAsync(cancellationToken);

        return OperationResult<AppointmentResponse>.Success(Map(appointment));
    }

    public async Task<OperationResult<AppointmentResponse>> UpdateAsync(
        Guid id,
        UpdateAppointmentRequest request,
        CancellationToken cancellationToken)
    {
        var appointment = await dbContext.Appointments
            .Include(item => item.Patient)
            .Include(item => item.MedicalStaff)
            .SingleOrDefaultAsync(item => item.Id == id, cancellationToken);

        if (appointment is null)
        {
            return OperationResult<AppointmentResponse>.Failure("not_found", "Cita no encontrada.");
        }

        if (appointment.Status is AppointmentStatus.Completed or AppointmentStatus.Cancelled)
        {
            return OperationResult<AppointmentResponse>.Failure(
                "appointment_closed",
                "Una cita completada o cancelada no puede reprogramarse.");
        }

        var validation = await ValidateAsync(
            request.PatientId,
            request.MedicalStaffId,
            request.ScheduledStartUtc,
            request.ScheduledEndUtc,
            id,
            cancellationToken);

        if (!validation.Succeeded)
        {
            return OperationResult<AppointmentResponse>.Failure(
                validation.ErrorCode ?? "validation_error",
                validation.ErrorMessage ?? "La cita no es válida.");
        }

        appointment.Update(
            request.PatientId,
            request.MedicalStaffId,
            request.ScheduledStartUtc,
            request.ScheduledEndUtc,
            request.Reason.Trim(),
            NormalizeOptional(request.Notes));

        await dbContext.SaveChangesAsync(cancellationToken);
        await dbContext.Entry(appointment).Reference(item => item.Patient).LoadAsync(cancellationToken);
        await dbContext.Entry(appointment).Reference(item => item.MedicalStaff).LoadAsync(cancellationToken);

        return OperationResult<AppointmentResponse>.Success(Map(appointment));
    }

    public async Task<OperationResult<AppointmentResponse>> ChangeStatusAsync(
        Guid id,
        ChangeAppointmentStatusRequest request,
        CancellationToken cancellationToken)
    {
        var appointment = await dbContext.Appointments
            .Include(item => item.Patient)
            .Include(item => item.MedicalStaff)
            .SingleOrDefaultAsync(item => item.Id == id, cancellationToken);

        if (appointment is null)
        {
            return OperationResult<AppointmentResponse>.Failure("not_found", "Cita no encontrada.");
        }

        if (request.Status == AppointmentStatus.Cancelled &&
            string.IsNullOrWhiteSpace(request.CancellationReason))
        {
            return OperationResult<AppointmentResponse>.Failure(
                "cancellation_reason_required",
                "Debe indicar el motivo de cancelación.");
        }

        appointment.ChangeStatus(request.Status, NormalizeOptional(request.CancellationReason));
        await dbContext.SaveChangesAsync(cancellationToken);
        return OperationResult<AppointmentResponse>.Success(Map(appointment));
    }

    private async Task<OperationResult<bool>> ValidateAsync(
        Guid patientId,
        Guid medicalStaffId,
        DateTime startUtc,
        DateTime endUtc,
        Guid? currentAppointmentId,
        CancellationToken cancellationToken)
    {
        if (startUtc >= endUtc)
        {
            return OperationResult<bool>.Failure(
                "invalid_time_range",
                "La hora de finalización debe ser posterior a la hora de inicio.");
        }

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
            return OperationResult<bool>.Failure(
                "doctor_not_found",
                "El profesional asignado debe ser un médico activo.");
        }

        var hasConflict = await dbContext.Appointments.AnyAsync(
            appointment => appointment.MedicalStaffId == medicalStaffId &&
                appointment.Status != AppointmentStatus.Cancelled &&
                appointment.Status != AppointmentStatus.NoShow &&
                (!currentAppointmentId.HasValue || appointment.Id != currentAppointmentId.Value) &&
                appointment.ScheduledStartUtc < endUtc &&
                appointment.ScheduledEndUtc > startUtc,
            cancellationToken);

        return hasConflict
            ? OperationResult<bool>.Failure(
                "schedule_conflict",
                "El médico ya tiene una cita que se cruza con ese horario.")
            : OperationResult<bool>.Success(true);
    }

    private static string? NormalizeOptional(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static AppointmentResponse Map(Appointment appointment) =>
        new(
            appointment.Id,
            appointment.PatientId,
            appointment.Patient.FullName,
            appointment.Patient.MedicalRecordNumber,
            appointment.MedicalStaffId,
            appointment.MedicalStaff.FullName,
            appointment.MedicalStaff.Specialty,
            appointment.ScheduledStartUtc,
            appointment.ScheduledEndUtc,
            appointment.Reason,
            appointment.Notes,
            appointment.Status,
            appointment.CancellationReason,
            appointment.CreatedAtUtc,
            appointment.UpdatedAtUtc);
}
