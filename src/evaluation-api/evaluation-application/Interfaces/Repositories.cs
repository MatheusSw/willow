namespace evaluation_application.Interfaces;

public interface IFeatureConfigRepository
{
    Task<(bool Found, FeatureConfig? Config)> TryGetConfigAsync(string project, string environment, string feature, CancellationToken cancellationToken);
}

public interface IApiKeyRepository
{
    Task<bool> ValidateAsync(string apiKey, CancellationToken cancellationToken);
}


