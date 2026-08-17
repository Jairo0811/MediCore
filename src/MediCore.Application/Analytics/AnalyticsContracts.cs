namespace MediCore.Application.Analytics;

public sealed record DashboardSummaryResponse(int ActivePatients, int ActiveStaff, int AppointmentsToday, int OpenConsultations, int ActiveMedications, int LowStockLots, int ExpiringLots30Days, int PendingLabOrders, int CompletedLabOrders30Days, int Consultations30Days);
public sealed record InventoryAlertResponse(Guid LotId, string MedicationCode, string MedicationName, string LotNumber, int QuantityOnHand, int ReorderPoint, DateOnly ExpirationDate, string AlertType);
public sealed record OperationalReportResponse(DateOnly From, DateOnly To, int Appointments, int Consultations, int CompletedConsultations, int LabOrders, int CompletedLabOrders, int InventoryMovements, int InventoryUnitsMoved);
