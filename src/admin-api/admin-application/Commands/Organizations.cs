namespace admin_application.Commands;

public sealed class CreateOrganizationCommand
{
    public string Name { get; init; } = string.Empty;
}

public sealed class UpdateOrganizationCommand
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
}

public sealed class DeleteOrganizationCommand
{
    public Guid Id { get; init; }
}