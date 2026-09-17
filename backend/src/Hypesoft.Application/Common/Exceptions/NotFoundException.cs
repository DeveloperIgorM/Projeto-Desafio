namespace Hypesoft.Application.Common.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException(string entity, string id)
        : base($"{entity} com id '{id}' não foi encontrado.")
    {
    }
}
