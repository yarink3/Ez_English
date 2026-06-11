# Ez_English — Custom Copilot CLI Agents

This folder defines a small, focused **agents team** that runs inside
[GitHub Copilot CLI](https://docs.github.com/en/copilot/how-tos/use-copilot-agents/use-copilot-cli)
when invoked from this repository. Each agent owns one stage of the
feature-delivery pipeline:

| Stage | Agent | File | What it produces |
|-------|-------|------|------------------|
| 1. Plan | `planner` | [`planner.agent.md`](./planner.agent.md) | `docs/features/<feature>.plan.md` — problem, pillars, UX, success criteria |
| 2. Design | `architect` | [`architect.agent.md`](./architect.agent.md) | `docs/features/<feature>.architecture.md` — modules, entities, APIs, components, algorithms, ordered implementation checklist |
| 3. Build | `developer` | [`developer.agent.md`](./developer.agent.md) | Production code + tests, built strictly with TDD (Red → Green → Refactor), one commit per cycle |
| 4. Review | `code-reviewer` | [`code-reviewer.agent.md`](./code-reviewer.agent.md) | High-signal review of the diff: bugs, auth bypasses, missing tests, RTL/i18n/a11y regressions |

## How they hand off

```
You ──▶ planner ──▶ architect ──▶ developer ──▶ code-reviewer ──▶ You
         │            │              │               │
         │            │              │               └─ blocks merge until clean
         │            │              └─ commits & pushes per TDD cycle
         │            └─ writes architecture doc the developer follows step by step
         └─ writes plan doc the architect designs against
```

Each agent's output is a concrete artifact in the repo
(`docs/features/<feature>.{plan,architecture}.md` for the first two,
real code + tests for the third). The next agent in the chain reads
that artifact instead of re-deriving context, so you keep working
context small and the work auditable.

## Invoking an agent

From inside the repository:

```bash
copilot
```

then in the Copilot CLI prompt, use any of the trigger phrases listed
in each agent's frontmatter, e.g.:

- `Plan a "letters drag-and-drop" game for ages 5–7`
- `Architect the letters drag-and-drop feature based on docs/features/letters-dnd.plan.md`
- `Implement the letters drag-and-drop feature from docs/features/letters-dnd.architecture.md`
- `Review my changes` (or `Review PR <url> and post comments`)

You can also browse and pick the agent explicitly with `/agent`.

## Project context every agent assumes

- **Frontend**: React 19 + TypeScript + Vite, Tailwind (RTL), i18next
  (he/en), `@dnd-kit/core`, Framer Motion, React Router v7, React
  Query, Firebase JS SDK.
- **Backend**: .NET 8 Web API, EF Core 8, FluentValidation, Serilog.
- **Database**: Azure SQL (serverless), EF Core migrations.
- **Auth**: Firebase Authentication — **parents only**. Children are
  domain entities owned by a parent.
- **Hosting**: Azure Static Web Apps + App Service + Azure SQL + Key
  Vault. IaC under [`infra/`](../../infra/).
- **ADRs**: [`docs/adr/`](../../docs/adr/) — the source of truth for
  prior architecture decisions.

## Editing an agent

Agent files are plain Markdown with YAML frontmatter. Edit them in
place and commit; the next `copilot` session in this repo will pick up
the changes automatically. Keep them concise — long agent prompts cost
context window for no benefit.
