# Developer Dashboard

The goal of the Developer Dashboard is to create a team improvement tool- not a developer leaderboard. Its central question is: Are teams delivering valuable, reliable software more effectively, and is AI helping, without increasing risk or maintenance cost?

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
