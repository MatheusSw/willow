namespace admin_application.Queries;

public sealed class GetFeatureStateByIdQuery
{
    public Guid Id { get; init; }
}

public sealed class ListFeatureStatesQuery
{
    public Guid? FeatureId { get; init; }
    public Guid? EnvironmentId { get; init; }
}


