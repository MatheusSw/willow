namespace admin_application.Queries;

public sealed class GetFeatureByIdQuery
{
    public Guid Id { get; init; }
}

public sealed class ListFeaturesQuery
{
    public Guid? ProjectId { get; init; }
}