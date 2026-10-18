namespace DeveloperDashboard.Web.Models;

public record RepositoryCard(string Slug, string Name, string Description, string Team,
    string Criticality, string Governance, string Security, string Flow, string Freshness);
public record Recommendation(string Repository, string Rule, string Title, string Severity,
    string Owner, string Explanation, string Fix);
public record ProjectCard(string Slug, string Name, string Description, string Team,
    string Criticality, string Lifecycle, IReadOnlyList<string> RepositorySlugs)
{
    public IReadOnlyList<RepositoryCard> Repositories =>
        PrototypeData.Repositories.Where(r => RepositorySlugs.Contains(r.Slug)).ToArray();
    public IReadOnlyList<Recommendation> Recommendations =>
        PrototypeData.Recommendations.Where(r => RepositorySlugs.Contains(r.Repository)).ToArray();
    public string Governance => Rollup(Repositories.Select(r => r.Governance));
    public string Security => Rollup(Repositories.Select(r => r.Security));
    public string Flow => Rollup(Repositories.Select(r => r.Flow));
    public string Freshness => Repositories.All(r => r.Freshness == "Current") ? "Current" : "Stale";
    public int UnknownRepositories => Repositories.Count(r =>
        r.Governance == "Unknown" || r.Security == "Unknown" || r.Flow is "Unknown" or "Insufficient data");

    private static string Rollup(IEnumerable<string> states)
    {
        var values = states.ToArray();
        return new[] { "Unhealthy", "Needs attention", "Attention", "Not configured", "Unknown", "Insufficient data" }
            .FirstOrDefault(values.Contains) ?? "Healthy";
    }
}
public record DashboardViewModel(IReadOnlyList<ProjectCard> Projects, string? Search, string? Team);

public static class PrototypeData
{
    public static IReadOnlyList<ProjectCard> Projects { get; } =
    [
        new("checkout", "Customer checkout", "Help customers complete a secure, reliable purchase.",
            "Commerce", "Critical", "Production", ["payments-api", "customer-web", "platform-sdk"]),
        new("identity", "Identity & access", "Give every product a trusted sign-in and access foundation.",
            "Platform", "Critical", "Production", ["identity-service", "platform-sdk"]),
        new("developer-platform", "Developer platform", "Make it easier for teams to build and integrate products.",
            "Platform", "High", "Maintenance", ["platform-sdk"])
    ];

    public static IReadOnlyList<RepositoryCard> Repositories { get; } =
    [
        new("payments-api", "payments-api", "Payment processing and checkout", "Commerce", "Critical", "Needs attention", "Unhealthy", "Attention", "Current"),
        new("customer-web", "customer-web", "Customer-facing web experience", "Experience", "High", "Healthy", "Healthy", "Healthy", "Current"),
        new("platform-sdk", "platform-sdk", "Shared client libraries", "Platform", "High", "Healthy", "Unknown", "Insufficient data", "Current"),
        new("identity-service", "identity-service", "Authentication and identity", "Platform", "Critical", "Not configured", "Unknown", "Unknown", "Stale")
    ];

    public static IReadOnlyList<Recommendation> Recommendations { get; } =
    [
        new("payments-api", "SEC-05", "Update a vulnerable dependency", "High", "Commerce",
            "One critical Dependabot alert is open. Critical alerts require attention immediately.",
            "Review the alert in GitHub, upgrade to the patched version, and run the repository's validation checks."),
        new("payments-api", "GOV-02", "Require an approving review", "High", "Commerce",
            "The sample default-branch policy does not require an approving review.",
            "Update the applicable branch rule to require at least one approving pull request review."),
        new("identity-service", "GOV-03", "Configure required status checks", "Medium", "Platform",
            "No required CI checks are configured in this sample.",
            "Select the validation workflow in the branch rules. Confirm it runs before enforcing the requirement.")
    ];

    public static string Tone(string state) => state switch
    {
        "Healthy" or "Current" => "good",
        "High" or "Unhealthy" => "danger",
        "Attention" or "Needs attention" or "Medium" or "Stale" or "Not configured" => "warning",
        _ => "neutral"
    };
}
