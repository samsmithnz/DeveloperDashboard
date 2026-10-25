namespace DeveloperDashboard.Web.Models;

public enum TimeScale { Minutes, Hours, Days, Weeks, Months }

public record MetricPeriod(string Window, double? Hours, int SampleSize)
{
    public TimeScale? Scale => Hours is not { } hours || SampleSize == 0 ? null : hours switch
    {
        < 1 => TimeScale.Minutes,
        < 24 => TimeScale.Hours,
        < 168 => TimeScale.Days,
        < 720 => TimeScale.Weeks,
        _ => TimeScale.Months
    };
}

public record DurationMetric(string Name, string Definition, TimeScale Target,
    IReadOnlyList<MetricPeriod> History, string Source, string? Limitation = null,
    bool IsStale = false, bool IsApplicable = true)
{
    public MetricPeriod Current => History[^1];
    public string Value => !IsApplicable ? "Not applicable" : Current.Scale?.ToString() ?? "No data";
    public string Health => !IsApplicable ? "Not applicable" : IsStale ? "Stale"
        : Current.Scale is not { } scale ? "Insufficient data"
        : scale <= Target ? "Healthy" : "Unhealthy";
    public string Trend => !IsApplicable ? "Not applicable" : IsStale ? "History not current"
        : History.Count < 2 || History.Any(p => p.Scale is null) ? "No comparison"
        : Current.Scale < History[0].Scale ? "Improving"
        : Current.Scale > History[0].Scale ? "Declining" : "Same band";
    public string TrendTone => Trend == "Improving" ? "good" : Trend == "Declining" ? "warning" : "neutral";
}
