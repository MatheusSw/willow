namespace admin_domain;

public static class CacheKeys
{
    public static string FeatureConfig(Guid projectId, string featureName)
    {
        return $"ft:cfg:{projectId}:{featureName}";
    }

    public static string ApiKey(string hashedKey)
    {
        return $"ft:apikey:{hashedKey}";
    }
}