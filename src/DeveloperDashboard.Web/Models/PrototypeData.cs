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
    public IReadOnlyList<DurationMetric> Metrics => PrototypeData.ProjectMetrics[Slug];
    public string Freshness => Repositories.All(r => r.Freshness == "Current") ? "Current" : "Stale";
    public int UnknownRepositories => Repositories.Count(r =>
        r.Governance == "Unknown" || r.Security == "Unknown" || r.Flow is "Unknown" or "Insufficient data");
}
public record DashboardViewModel(IReadOnlyList<ProjectCard> Projects, string? Search, string? Team);

public static class PrototypeData
{
    public static IReadOnlyDictionary<string, IReadOnlyList<DurationMetric>> ProjectMetrics { get; } =
        new Dictionary<string, IReadOnlyList<DurationMetric>>
        {
            ["checkout"] =
            [
                new("Change lead time", "Median elapsed time from commit to its linked successful production deployment.",
                    TimeScale.Hours, [new("20 Jul - 18 Aug", 72, 28), new("19 Aug - 17 Sep", 48, 32), new("18 Sep - 17 Oct", 5, 36)],
                    "Fictional linked commits and production deployments"),
                new("First review wait", "Median elapsed time from a PR becoming ready to its first human review; drafts and bot reviews excluded.",
                    TimeScale.Hours, [new("20 Jul - 18 Aug", 5, 40), new("19 Aug - 17 Sep", 4.5, 44), new("18 Sep - 17 Oct", 4, 48)],
                    "Fictional project PRs, deduplicated across supporting repositories"),
                new("Failed deployment recovery", "Median elapsed time from a failed production deployment to verified recovery; unrelated incidents excluded.",
                    TimeScale.Hours, [new("20 Jul - 18 Aug", 3, 4), new("19 Aug - 17 Sep", 28, 3), new("18 Sep - 17 Oct", 48, 4)],
                    "Fictional failed production deployments linked to recovery evidence")
            ],
            ["identity"] =
            [
                new("Change lead time", "Median elapsed time from commit to its linked successful production deployment.",
                    TimeScale.Hours, [new("20 Jul - 18 Aug", 240, 12), new("19 Aug - 17 Sep", 72, 14), new("18 Sep - 17 Oct", 48, 10)],
                    "Fictional linked commits and production deployments", "Collection is stale; bands are last-known values, not a current assessment.", IsStale: true),
                new("First review wait", "Median elapsed time from a PR becoming ready to its first human review; drafts and bot reviews excluded.",
                    TimeScale.Hours, [new("20 Jul - 18 Aug", null, 0), new("19 Aug - 17 Sep", null, 0), new("18 Sep - 17 Oct", null, 0)],
                    "Fictional project PRs", "Review evidence is unavailable. No duration or trend can be inferred."),
                new("Failed deployment recovery", "Median elapsed time from a failed production deployment to verified recovery; unrelated incidents excluded.",
                    TimeScale.Hours, [new("20 Jul - 18 Aug", null, 0), new("19 Aug - 17 Sep", null, 0), new("18 Sep - 17 Oct", null, 0)],
                    "Fictional deployment and incident records", "Incident evidence is not connected. Missing records do not mean instant recovery.")
            ],
            ["developer-platform"] =
            [
                new("Change lead time", "Median elapsed time from commit to its linked successful production deployment.",
                    TimeScale.Hours, [new("20 Jul - 18 Aug", null, 0), new("19 Aug - 17 Sep", null, 0), new("18 Sep - 17 Oct", null, 0)],
                    "Fictional project configuration", "This maintenance library has no production deployment target; package releases are not used as a deployment proxy.", IsApplicable: false),
                new("First review wait", "Median elapsed time from a PR becoming ready to its first human review; drafts and bot reviews excluded.",
                    TimeScale.Hours, [new("20 Jul - 18 Aug", null, 0), new("19 Aug - 17 Sep", null, 0), new("18 Sep - 17 Oct", 6, 8)],
                    "Fictional project PRs", "Only the current window has review evidence; history is insufficient for a trend."),
                new("Failed deployment recovery", "Median elapsed time from a failed production deployment to verified recovery; unrelated incidents excluded.",
                    TimeScale.Hours, [new("20 Jul - 18 Aug", null, 0), new("19 Aug - 17 Sep", null, 0), new("18 Sep - 17 Oct", null, 0)],
                    "Fictional project configuration", "No production deployment target.", IsApplicable: false)
            ]
        };

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
