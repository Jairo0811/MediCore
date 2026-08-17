namespace MediCore.Application.Identity;

public static class RoleNames
{
    public const string Administrator = "Administrator";
    public const string Doctor = "Doctor";
    public const string Nurse = "Nurse";
    public const string Receptionist = "Receptionist";
    public const string Pharmacist = "Pharmacist";
    public const string Laboratory = "Laboratory";
    public const string Auditor = "Auditor";

    public static readonly string[] All =
    [
        Administrator,
        Doctor,
        Nurse,
        Receptionist,
        Pharmacist,
        Laboratory,
        Auditor
    ];
}
