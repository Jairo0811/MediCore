namespace MediCore.Application.Common;

public interface ICedulaValidator
{
    bool IsValid(string cedula);
    string Normalize(string cedula);
}
