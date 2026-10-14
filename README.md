# Developer Dashboard

The goal of the Developer Dashboard is to create a team improvement tool the incorporates best practices for software development with AI - not a developer leaderboard. Its central question is: Are teams delivering valuable, reliable software more effectively, and is AI helping, without increasing risk or maintenance cost?

These are related, but should not become one opaque score.
| Dimension |	Question |	Useful signals |
| -- | -- | -- |
| Delivery performance	| How effectively do changes reach users?	| Production deployment frequency, change lead time, change failure rate, failed deployment recovery time
| Engineering flow	| Where does work wait?	| Time to first review, review-to-merge time, merge-to-production time, aging PRs, work in progress
| Project health	| Is the project reliable and maintainable today?	| Build reliability, flaky tests, escaped defects, security findings, dependency age, unsupported runtimes
| Engineering maturity| 	Are repeatable safeguards in place?	| Branch rules, required checks, CODEOWNERS, automated testing, deployment approvals, rollback capability, runbooks
| AI effectiveness and readiness	| Can humans and agents work effectively and safely?	| Agent task outcomes, review burden, rework, reproducible environments, useful instructions, scoped permissions
| Developer experience	| Is the development system helping the team?	| CI feedback time, onboarding time, tooling friction, developer satisfaction
| Product outcomes | 	Does delivery create value?	| Adoption, customer-impacting defects, support burden, progress toward product goals

**A mature repository can still have poor delivery performance; a fast team can still operate an unhealthy service.** 

### AI-assisted delivery quality

-	Time from task assignment to accepted, validated change.
-	Human review time and review iterations.
-	Reverts, follow-up fixes, and associated production defects.
-	Test and security gate results.
-	Cost per accepted task, segmented by task type.

### Agent readiness

-	Repository instructions such as AGENTS.md and Copilot instructions.
-	Documented build, test, lint, and local setup commands.
-	Reproducible development environments.
-	Clear architecture boundaries and ownership.
-	Fast, reliable automated validation.
-	Scoped agent credentials and human approval for consequential actions.

## Make cross-repository modeling a first-class feature
Avoid assuming one repository = one team = one production service.

Model:
-	Organizations and teams.
-	Repositories and components.
-	Services/products and deployment environments.
-	Ownership, business criticality, and lifecycle stage.
-	Dependencies between services and shared libraries.

This supports both monorepos with multiple services and products spread across repositories.

Useful portfolio questions include:
-	Which critical services depend on unsupported frameworks?
-	Which shared libraries expose the most downstream projects to risk?
-	Where is review ownership concentrated?
-	Which teams have growing review queues despite faster implementation?
-	Which production projects lack an accountable owner?

Exclude or separately classify archived repositories, experiments, templates, and maintenance-only projects.

## Be precise about metric definitions and evidence

DevOpsMetrics documents using successful pipeline runs as a deployment proxy under specific assumptions. DeveloperDashboard should make those assumptions explicit rather than silently treating them as facts.

| Metric | Important distinction |
| -- | -- |
| Deployment frequency	| Count production deployments—not every successful CI run
| Change lead time	| Link changes to production deployment; PR cycle time is a separate measure
| Change failure rate	| Production changes requiring remediation—not failed builds
| Failed deployment recovery time	| Recovery from a failed deployment—not every unrelated outage
| Security health	| No findings is different from scanning disabled or inaccessible
| Test quality	| Coverage indicates execution, not necessarily effective assertions

Every metric should expose:
-	Definition and calculation version.
-	Source and link to supporting evidence.
-	Measurement window and sample size.
-	Last successful collection time.
-	Missing data and known limitations.

Use unknown, not configured, not applicable, and healthy as distinct states. Missing incident data must not produce a reassuring 0% failure rate.

## Design scorecards that drive improvement

Prefer separate, explainable scorecards for security, delivery practices, maintainability, and AI readiness.
A possible maturity progression:
1. Foundational: Ownership, documented setup, basic CI.
2. Protected: Required reviews, tests, security checks.
3. Repeatable: Automated releases, dependency management, reproducible environments.
4. Measured: Delivery metrics, reliability objectives, trend monitoring.
5. Continuously improving: Evidence-backed experiments and tracked remediation.
6. 
Apply different requirements to production services, libraries, and experiments. Allow documented exceptions with owners and expiration dates.

Avoid a single “87/100 engineering score” that lets good documentation cancel out a critical security gap.

## Prioritize actionable dashboard views

### Portfolio overview

A searchable repository/service inventory with:
-	Owner and criticality.
- Health indicators and trends.
-	Maturity gaps.
-	Data freshness.
-	Highest-priority recommended action.

### Team view

-	Delivery and review-flow trends.
-	Review queues and aging work.
-	AI-related changes in throughput, review effort, and quality.
-	Developer-experience feedback.

### Repository/service detail

-	Governance checks and supporting evidence.
-	Dependencies and runtime support.
-	Builds, tests, vulnerabilities, releases, and deployments.
-	Recommendations with remediation guidance.

### Improvement queue

Turn findings into work with:
-	Severity, rationale, owner, and estimated effort.
-	Evidence links.
-	Exception/suppression handling.
-	Optional issue or remediation PR creation.
-	Verification that the action resolved the finding.
  
Start remediation with approval-based PRs, not automatic changes to production or repository security settings.

## Build trust and privacy into the design

-	Measure teams and systems, not individual developer rankings.
-	Do not equate commits, lines of code, PR counts, or suggestion acceptance with value.
-	Prefer within-team trends over comparisons between unrelated teams.
-	Supplement telemetry with short developer surveys; activity cannot reveal cognitive load.
-	Restrict access to private repository and security data.
-	Aggregate AI usage data and avoid collecting prompts or source code unless necessary.
-	Present AI-impact findings as correlations unless the evaluation supports a causal claim.

If the dashboard itself generates AI summaries, require evidence links and distinguish observations from hypotheses. Repository text and issue content should be treated as untrusted input, not instructions to the summarizer.
