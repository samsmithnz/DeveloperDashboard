namespace DeveloperDashboard.Web.Models;

public record RepositoryCard(string Slug, string Name, string Description, string Team,
    string Criticality, string Governance, string Security, string Flow, string Freshness);
public record Recommendation(string Repository, string Rule, string Title, string Severity,
    string Owner, string Explanation, string Fix);
public record PortfolioViewModel(IReadOnlyList<RepositoryCard> Repositories, string? Search, string? Team);

public static class PrototypeData
{
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
