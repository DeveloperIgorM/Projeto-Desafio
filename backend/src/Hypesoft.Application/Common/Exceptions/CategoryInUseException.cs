namespace Hypesoft.Application.Common.Exceptions;

// Regra simples: não deixo excluir uma categoria que ainda tem produto associado.
// Dá pra evoluir isso depois (ex: perguntar se quer mover os produtos pra outra categoria),
// mas por ora só bloqueia e explica o motivo.
public class CategoryInUseException : Exception
{
    public CategoryInUseException(string id)
        : base($"Categoria '{id}' não pode ser excluída porque existem produtos associados a ela.")
    {
    }
}
