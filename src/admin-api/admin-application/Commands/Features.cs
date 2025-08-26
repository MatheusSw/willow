namespace admin_application.Commands;

public sealed class CreateFeatureCommand
{
	public Guid ProjectId { get; init; }
	public string Name { get; init; } = string.Empty;
	public string? Description { get; init; }
}

public sealed class UpdateFeatureCommand
{
	public Guid Id { get; init; }
	public Guid ProjectId { get; init; }
	public string Name { get; init; } = string.Empty;
	public string? Description { get; init; }
}

public sealed class DeleteFeatureCommand
{
	public Guid Id { get; init; }
}