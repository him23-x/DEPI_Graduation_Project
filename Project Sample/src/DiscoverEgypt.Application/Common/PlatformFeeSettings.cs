namespace DiscoverEgypt.Application.Common;

/// <summary>Configuration value the Application needs. The Web API fills it from appsettings.</summary>
public sealed class PlatformFeeSettings
{
    public decimal ExperienceRate { get; init; }
}
