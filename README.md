# Ez_English

English-learning web app for Israeli children (ages 3–12). Parents create child profiles; each child picks an animated mascot and progresses through age- and level-appropriate games (colors, animals, family members, letters, …) built around drag-and-drop interactions.

> 📘 All major architecture decisions are documented in [`docs/adr/`](docs/adr/).

## Stack

| Layer | Tech |
|-------|------|
| Frontend | React 18 + TypeScript + Vite, Tailwind CSS (RTL), i18next (he/en), @dnd-kit, Framer Motion |
| Backend | .NET 8 Web API, EF Core 8, FluentValidation, Serilog |
| Database | Azure SQL Database (serverless) |
| Auth | Firebase Authentication (parents only — children are domain entities) |
| Hosting | Azure Static Web Apps (FE) + App Service (BE) + Azure SQL + Key Vault |
| IaC | Bicep (under `infra/`) |

## Repository layout

```
.
├── docs/adr/         # Architecture Decision Records (Markdown)
├── frontend/         # Vite + React + TS + Tailwind
├── backend/          # .NET 8 Web API solution
└── infra/            # Bicep templates (Azure deployment)
```

## Local development

### Prerequisites

- Node.js 20+
- .NET 8 (or 9) SDK
- A Firebase project (Authentication → Email/Password + Google providers enabled)

### Frontend

```powershell
cd frontend
copy .env.example .env.local   # then edit .env.local with your Firebase web config
npm install
npm run dev                    # http://localhost:5173
```

Available env vars (all `VITE_FIREBASE_*` are public-by-design for Firebase web apps):

```
VITE_FIREBASE_API_KEY=
VITE_FIREBASE_AUTH_DOMAIN=
VITE_FIREBASE_PROJECT_ID=
VITE_FIREBASE_STORAGE_BUCKET=
VITE_FIREBASE_MESSAGING_SENDER_ID=
VITE_FIREBASE_APP_ID=
VITE_API_BASE_URL=http://localhost:5192
```

### Backend

```powershell
cd backend
dotnet build
dotnet run --project EzEnglish.Api
```

The backend reads its Firebase **Admin** service-account JSON from either the
`Firebase:ServiceAccountPath` setting in `appsettings.json` or the standard
Google env var:

```
GOOGLE_APPLICATION_CREDENTIALS=C:\path\to\firebase-service-account.json
```

Service-account JSON files are gitignored by name pattern
(`firebase-service-account*.json`). Do **not** commit them.

## Roadmap

See [`plan.md` in the session workspace](#) and the ADRs for full context. Short version:

1. ✅ Phase 1 — Foundations (ADRs, project skeletons, auth UI)
2. ⏳ Phase 2 — Backend domain model + EF migrations + Firebase token validation
3. ⏳ Phase 3 — Drag-and-drop game engine + content modules (colors, animals, family, letters)
4. ⏳ Phase 4 — Azure deployment (Bicep + GitHub Actions)
