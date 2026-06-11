# 7. Azure deployment topology

Date: 2026-06-11

## Status

Accepted

## Context

We want a low-cost, low-ops Azure deployment that can scale modestly as the user base grows. The team is small and we don't want to manage Kubernetes.

## Decision

| Resource | Azure service | Notes |
|----------|---------------|-------|
| Frontend (React SPA) | **Azure Static Web Apps** (Standard tier) | Free SSL, global CDN, GitHub Actions CI/CD, built-in auth integration if we ever need it. |
| Backend (.NET 8 API) | **Azure App Service** (Linux, B1 to start) | Easy slot swap, auto-restart, Application Insights integration. |
| Database | **Azure SQL Database** (serverless, Gen5, 0.5–2 vCore) | Auto-pause after 1 hr idle to minimize dev cost. |
| Secrets | **Azure Key Vault** | Holds Firebase service account JSON, SQL conn string, etc. Backend reads via Managed Identity. |
| Telemetry | **Application Insights** (Log Analytics workspace) | Both FE (browser SDK) and BE (Serilog sink). |
| Static media (avatars, lesson images/audio) | **Azure Blob Storage** (Hot tier) + Azure CDN | Public-read container for content; SAS for any user-uploaded media (later). |
| CI/CD | **GitHub Actions** | Two workflows: `frontend.yml` (build + deploy to Static Web Apps), `backend.yml` (build, test, deploy to App Service). |
| IaC | **Bicep** under `infra/` | One main `main.bicep` plus per-resource modules. |

### Environments

- `dev` — single resource group, cheapest tiers, auto-pause SQL.
- `prod` — separate resource group, standard tiers, geo-redundant SQL backup.

### Network

- Phase 1: public endpoints + CORS allowlist on the API for the Static Web Apps domain.
- Phase 4+: optional VNet integration + Private Endpoints for SQL and Key Vault.

## Consequences

- Pay-as-you-go for nearly everything; dev environment can cost ~$5–15/month.
- Static Web Apps + App Service avoids us learning AKS or Container Apps for v1.
- Managed Identity removes the need to store SQL/Key Vault secrets in the API config.
- We accept vendor lock-in to Azure for hosting; the app itself stays portable (standard SQL, standard .NET, standard React).
