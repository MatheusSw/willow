namespace admin_infrastructure.Utilities;

public static class CacheKeys
{
    public static string FeatureConfig(Guid projectId, string environment, string feature)
    {
        return $"ft:cfg:{projectId}:{environment}:{feature}";
    }

    public static string ApiKey(string hashedKey)
    {
        return $"ft:apikey:{hashedKey}";
    }
}


