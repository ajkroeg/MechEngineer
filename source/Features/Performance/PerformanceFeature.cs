namespace MechEngineer.Features.Performance
{
    internal class PerformanceFeature : Feature<PerformanceSettings>
    {
        internal static PerformanceFeature Shared = new();

        internal override PerformanceSettings Settings => Control.settings.Performance;

        internal static PerformanceSettings settings => Shared.Settings;
    }
}
