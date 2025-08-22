namespace evaluation_infrastructure.Utilities;

/// <summary>
/// Helpers for building namespaced cache keys.
/// </summary>
public static class CacheKeys
{
    /// <summary>
    /// Cache key for feature configuration by project/environment/feature.
    /// </summary>
    /// <param name="projectId">Project identifier.</param>
    /// <param name="environment">Environment key.</param>
    /// <param name="feature">Feature name.</param>
    /// <returns>Namespaced cache key.</returns>
    public static string FeatureConfig(Guid projectId, string environment, string feature)
    {
        return $"ft:cfg:{projectId}:{environment}:{feature}";
    }

    /// <summary>
    /// Cache key used to store API key validation results.
    /// </summary>
    /// <param name="hashedKey">Base64 SHA-256 hash of the API key.</param>
    /// <returns>Namespaced cache key.</returns>
    public static string ApiKey(string hashedKey)
    {
        return $"ft:apikey:{hashedKey}";
    }
}



