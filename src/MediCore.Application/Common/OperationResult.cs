namespace MediCore.Application.Common;

public sealed record OperationResult<T>(
    bool Succeeded,
    T? Value,
    string? ErrorCode,
    string? ErrorMessage)
{
    public static OperationResult<T> Success(T value) =>
        new(true, value, null, null);

    public static OperationResult<T> Failure(string code, string message) =>
        new(false, default, code, message);
}
