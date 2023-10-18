namespace PaintyTest.Contracts.Exceptions;

public class EntityNotFoundException : BaseApiException
{
    public new const int DefaultCode = StatusCodes.Status400BadRequest;

    public EntityNotFoundException(Type entityType, string id) : base(DefaultCode,
        $"Entity {entityType.Name} with id {id} not found")
    {
    }
}