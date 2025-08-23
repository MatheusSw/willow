namespace admin_application.Queries;

public sealed class GetOrganizationByIdQuery
{
	public Guid Id { get; init; }
}

public sealed class ListOrganizationsQuery
{
	public string? Name { get; init; }
}