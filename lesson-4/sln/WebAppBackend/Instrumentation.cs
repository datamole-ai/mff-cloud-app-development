using System.Diagnostics;

namespace WebAppBackend;

public static class Instrumentation
{
    internal const string ActivitySourceName = "Platform.WebAppBackend";
    internal static ActivitySource ActivitySource { get; } = new(ActivitySourceName);
}
