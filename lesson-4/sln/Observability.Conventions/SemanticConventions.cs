namespace Observability.Conventions;

public static class SemanticConventions
{
    public const string AttributeCacheHit = "mffiot.is_cache_hit";
    
    public static class AttributeCacheHitValues
    {
        public const string True = "true";
        public const string False = "false";
    }
}
