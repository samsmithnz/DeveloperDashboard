# MVP architecture and GitHub App decisions

This document records the decisions for the read-only MVP. It supersedes earlier organization-only and unresolved cross-owner scope assumptions. Permission behavior that depends on a real GitHub installation remains subject to the verification plan below.

## Hosting, account scope, and access

- Each customer operates a self-hosted instance on Azure App Service, with one instance per dashboard. Horizontal scale-out and distributed locking are out of scope for MVP.
- A dashboard connects to exactly one GitHub owner: either one organization or one personal account. Cross-owner aggregation and sharing are out of scope.
- Support GitHub.com only for MVP. GHES support and compatibility promises are deferred.
- Configure GitHub web and API base URLs (defaulting to GitHub.com) and use them consistently for sign-in, API calls, and evidence links. Discover actual installation permissions, feature availability, and rate limits at runtime.
- Use the GitHub App's user-authorization flow for dashboard sign-in; do not create a separate OAuth app. Use short-lived GitHub App installation access tokens for background collection, not dashboard user tokens.
- For an organization, every authenticated organization member may view all assessed repositories and findings, including private-repository and security findings, even without direct repository access. This is an intentional organization-wide disclosure policy and must be made clear during setup. For a personal installation, only the account owner can view its findings.
- Bootstrap the first organization dashboard administrator only after server-side verification through the organization membership API that the authenticated installer is an organization owner. The personal account owner is the administrator for a personal installation. Organization members can view; only members of the configured GitHub admin team can use **Sync now**. Exceptions are changed through reviewed config-repository pull requests, not dashboard writes.
- Keep the GitHub App read-only. Do not request write permissions or create issues, pull requests, webhooks, or other GitHub resources.
- Keep the App private key in Azure configuration or Key Vault; never commit it to the repository. Findings must not expose credentials, secret values, or source snippets containing secret values. Evidence links remain subject to GitHub's access controls.

## Installation scope and initial experience

- Read the installation owner ID and type, and discover repositories with `GET /installation/repositories` using the installation token. Assess repositories accessible to that installation within the scope explicitly confirmed by the installer; a config repository, manual allowlist, team mapping, criticality, or production classification is not required for the first snapshot.
- Keep archived repositories visible in inventory, but exclude them from active-flow summaries by default. Explain exclusions, inaccessible repositories, and unclassified data rather than silently dropping them.
- Missing config is expected. Defaults apply. An invalid config is a visible validation error; retain the last valid config if one exists. Config that cannot be read because of permission, network, or API failure is unavailable, not absent; surface the error and retain the last valid config if available.
- Suggest organization groups from GitHub teams and CODEOWNERS with provenance and an **unconfirmed** label. Team access is not proof of accountable ownership. Keep unmapped repositories visible as unassigned. Personal accounts start with **My projects**. Do not infer development teams from individual contribution volume.
- Treat workflow identification as a heuristic: do not assume every Actions workflow is CI. Show ambiguous evidence as **Needs confirmation**, a setup/evidence qualifier rather than a healthy result.
- Each finding and metric exposes its definition/calculation version, evidence source/link, measurement window and sample size, last successful collection time, missing-data coverage, and known limitations. Repository text and issue content are untrusted input.

## GitHub App permissions and data availability

Request only the following **read** permissions. Repository Metadata is mandatory to list and inspect installation repositories. Administration is the broadest permission: it is required for classic branch protection and several security feature status checks. If it is declined, those checks are `unknown`, never reported as passing. Organization Members is required only for organization membership/team features; personal-account operation does not depend on it.

| Level | GitHub App permission | Needed for |
| -- | -- | -- |
| Repository | Metadata: Read (mandatory) | Installation repository inventory/details, license, custom properties, rulesets and active branch rules |
| Repository | Administration: Read | Classic branch protection, code-scanning default setup, Dependabot enabled status, and security settings |
| Repository | Contents: Read | Repository files, CODEOWNERS, README, SECURITY.md, community profile, and optional config repository |
| Repository | Pull requests: Read | Pull requests, reviews, review requests, and PR timeline events |
| Repository | Actions: Read | Workflows and workflow runs |
| Repository | Code scanning alerts: Read | Code-scanning alerts and analyses |
| Repository | Dependabot alerts: Read | Dependabot alerts |
| Repository | Secret scanning alerts: Read | Secret-scanning alerts |
| Organization | Members: Read | Organization membership, teams, and team-repository access |

No Checks, Issues, Commit statuses, Deployments, or write permission is needed.

| Data point / use | Endpoint(s) | Permission | Unavailable behavior |
| -- | -- | -- | -- |
| Installation owner and repository inventory; visibility, archived/fork/template flags, description (inventory, GOV-09) | `GET /installation`, `GET /installation/repositories`, `GET /repos/{owner}/{repo}` | Metadata | Failure to discover the installation scope fails the sync with an access/collection error; do not treat the inventory as empty |
| License (GOV-10) | `GET /repos/{owner}/{repo}/license` | Metadata | `unknown` |
| Custom properties (classification, criticality, lifecycle) | `GET /repos/{owner}/{repo}/properties/values` | Metadata | Fall back to config overrides; otherwise `unknown` / **Needs confirmation** |
| Teams and team-repository access (inventory, GOV-06) | `GET /orgs/{org}/teams`, `GET /orgs/{org}/teams/{team_slug}/repos` | Organization Members | Organization ownership suggestion is `unknown`; not applicable to personal accounts |
| Repository files: CODEOWNERS, README, SECURITY.md, AGENTS.md, Copilot instructions, dependency-update config, `developer-dashboard.yml` (GOV-05/06/09, SEC-01/07, MNT-01/02, AI-01) | `GET /repos/{owner}/{repo}/contents/{path}`, `.../readme`, `.../git/trees/{sha}` | Contents | File checks are `unknown` |
| Community profile (GOV-09/10, optional SEC-07 shortcut) | `GET /repos/{owner}/{repo}/community/profile` | Contents | Fall back to file checks |
| CODEOWNERS validity (GOV-05) | `GET /repos/{owner}/{repo}/codeowners/errors` | Contents | `unknown` |
| Rulesets and active branch rules (GOV-01–04) | `GET /repos/{owner}/{repo}/rulesets`, `GET /repos/{owner}/{repo}/rules/branches/{branch}` | Metadata | `unknown`; evaluate alongside classic protection |
| Classic branch protection (GOV-01–04) | `GET /repos/{owner}/{repo}/branches/{branch}/protection` | Administration | If rulesets do not establish protection and this endpoint is inaccessible, `unknown`, never “unprotected” |
| Workflows and runs (GOV-07/08, CI metrics) | `GET /repos/{owner}/{repo}/actions/workflows`, `.../actions/runs` | Actions | `unknown`; workflow presence alone does not prove CI coverage |
| PRs, reviews, review requests, ready-for-review events (flow metrics) | `GET /repos/{owner}/{repo}/pulls`, `.../pulls/{number}/reviews`, `.../issues/{number}/timeline` | Pull requests | Flow metrics are `unknown` |
| Secret scanning and push-protection status (SEC-02/03) | `GET /repos/{owner}/{repo}` (`security_and_analysis`) | Administration | `unknown`; visibility with installation tokens must be verified |
| Code-scanning default setup (SEC-04) | `GET /repos/{owner}/{repo}/code-scanning/default-setup` | Administration | `unknown` |
| Code-scanning analyses and alerts (SEC-04) | `GET /repos/{owner}/{repo}/code-scanning/analyses`, `.../alerts` | Code scanning alerts | `unknown`; `not configured` only when GitHub confirms disabled |
| Dependabot enabled status (SEC-05) | `GET /repos/{owner}/{repo}/vulnerability-alerts` | Administration | `unknown` |
| Dependabot alerts (SEC-05) | `GET /repos/{owner}/{repo}/dependabot/alerts` | Dependabot alerts | `unknown`; `not configured` only when GitHub confirms disabled |
| Secret-scanning alerts (SEC-06) | `GET /repos/{owner}/{repo}/secret-scanning/alerts` | Secret scanning alerts | `unknown` |
| Organization membership and owner/admin-team authorization | `GET /orgs/{org}/memberships/{username}` and organization team endpoints | Organization Members: Read | Deny organization dashboard access or admin bootstrap when membership/owner status cannot be verified |

### State semantics and verification

- `unknown` means the system cannot determine the state: for example, an ambiguous 403/404, unsupported endpoint, permission denial, licensing ambiguity, throttling, or collection failure. Preserve an explanatory reason and collection status.
- `not configured` means GitHub positively confirms that the feature is disabled. Do not infer it from missing alerts, an empty response, or an inaccessible endpoint.
- Keep `not applicable`, `healthy`, `unknown`, and `not configured` distinct. Unavailable data is never a clean/no-findings result.
- GHAS features and data may depend on licensing and plan. Record observed API responses (including 403, 404, and disabled) and map them consistently using the rules above.
- Before MVP completion, validate the permission matrix with a restricted real GitHub App installation, including GHAS availability where feasible. Specifically verify `security_and_analysis` visibility with Administration read, permission behavior for each endpoint, and graceful degradation. Record observations and cover resulting states in automated tests. Until verified, the affected permission assumptions remain test items rather than guarantees.

## Optional central configuration

The optional central config repository belongs to the connected organization or personal owner and is read-only to the app (Contents: Read). Missing configuration is not an onboarding blocker. Changes are reviewed and audited through normal pull requests. Repository discovery still starts from the installer's confirmed installation scope; configuration can refine or exclude repositories, not silently widen access.

The YAML file is `developer-dashboard.yml` at the repository root and must include a supported `version`. The following illustrates the v1 shape; optional fields can be omitted:

```yaml
version: 1
repositories:
  - name: owner/repository
    include: true
    groupIds: [payments]
    owner: payments-team
    criticality: high
    lifecycle: production
    thresholds: {}
developmentTeams:
  - id: payments
    displayName: Payments
    repositories:
      - owner/repository
exceptions:
  - id: exception-id
    repository: owner/repository
    rule: rule-id
    owner: person-or-team
    rationale: Reviewed reason
    expiresAt: 2027-01-31
thresholds: {}
```

Validation requirements:

- Repository references must identify repositories in the confirmed installation scope. Reject unknown references, duplicate repository membership within a development team, duplicate team IDs, malformed values, unsupported schema versions, and invalid exception dates with clear file/field errors. Expired exceptions are inactive and must not suppress findings.
- Development-team IDs are stable identifiers; display names are presentation. Teams explicitly list repositories; repositories may be shared across teams. Keep these groups distinct from GitHub admin teams and CODEOWNERS ownership signals.
- Config can refine repository inclusion/exclusion, groups, owner/criticality/lifecycle, thresholds, and time-bounded exceptions. A central config repository name/location and the admin GitHub team are instance settings.
- Invalid configuration is surfaced to administrators and does not replace the last valid version. An unavailable config source is reported separately from an absent config file and likewise does not erase last valid configuration. Persist the config version/scope used for every snapshot.

## Persistence, history, and trends

- Store historical snapshots in Azure Table Storage behind a persistence abstraction so the backing store can be replaced later (for example, with Cosmos DB).
- Retain snapshots for 90 days. Expiry cleanup applies only to historical snapshot entities; it must not delete current configuration or active exceptions.
- Use a snapshot table with one entity per repository per collection snapshot:
  - `PartitionKey`: stable repository identity, composed from GitHub host, installation owner type/ID, installation ID, and repository ID. This colocates that repository's history for efficient time-range trend queries without depending on a mutable repository name.
  - `RowKey`: UTC collection timestamp in fixed-width sortable form (`yyyyMMddHHmmssfff`) followed by a unique snapshot ID. This supports ordered range queries within a repository and avoids collisions for repeated/on-demand syncs.
  - Store owner/repository display names, snapshot/sync status, config version and scope, metric/check definition versions, collection timestamp, observed data and unavailable-state reasons. Repository identity remains stable if its name changes.
- Query repository trends by partition and row-key time range. For development-team trends, query each explicitly configured member repository and aggregate raw observations; do not average repository medians, percentages, or already-bucketed labels. Persist the source window and sample information needed to reproduce the aggregation. A separate cross-repository index is not required for MVP.
- Store current instance configuration and active exceptions separately from expiring snapshot history. Preserve no-config defaults and config/group versions so trends can explain scope or classification changes rather than imply a performance change.

## Synchronization and rate limits

- Run an hourly scheduled sync and provide admin-team-only **Sync now**. Use installation access tokens for all collection.
- One App Service instance is assumed. Use a shared in-process concurrency guard so scheduled and manual syncs cannot overlap in that instance. Distributed locking is deferred until horizontal scaling is supported.
- Target approximately 200 assessed repositories in total; this is not a guaranteed request budget. GitHub documents a 15,000 REST requests/hour primary limit for installations on GitHub Enterprise Cloud organizations. Other installations start at 5,000/hour and scale with repository/user count, capped at 12,500/hour. Do not assume Enterprise Cloud or a 15,000/hour limit.
- Read `X-RateLimit-Limit`, `X-RateLimit-Remaining`, and `X-RateLimit-Reset` from responses and budget calls across scheduled and on-demand syncs. Bound concurrency; respect secondary rate limits and `Retry-After`. Report throttling and delayed/partial collection explicitly, never as a successful complete sync.
- Do not mandate tiered refresh cadence. Validate the endpoint/request budget for the target installation during implementation.

Reference: [GitHub REST API rate limits](https://docs.github.com/en/rest/using-the-rest-api/rate-limits-for-the-rest-api#primary-rate-limit-for-github-app-installations).

## Team views and metric interpretation

- Keep individual repository views and explicit development-team views for both account types. Group names can be suggested from repository signals but remain unconfirmed until configured.
- Show contributing repositories, measurement window, sample size, freshness, and missing-data coverage beside combined results. Do not rank developers or calculate a composite engineering score.
- Use the versioned duration bands consistently: **Hours** is less than 24 elapsed hours (including sub-hour durations); **Days** is at least 24 hours and less than 7 days; **Weeks+** is at least 7 days. These describe timescales, not universal healthy/unhealthy thresholds.
- PR review/merge time and CI duration are GitHub flow metrics, not production change lead time or DORA measures. Production DORA collection is out of scope.
