namespace admin_application.Queries;

public sealed class GetEnvironmentByIdQuery
{
    public Guid Id { get; init; }
}

public sealed class ListEnvironmentsQuery
{
    public Guid? ProjectId { get; init; }
}


