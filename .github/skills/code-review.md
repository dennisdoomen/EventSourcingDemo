# Code Review Skill

Use this skill when reviewing pull requests in the ClubAdmin repository. Focus on issues that
**genuinely matter** — bugs, violated invariants, broken contracts, security issues, and
convention violations. Do not comment on style if the formatter would fix it automatically.

---

## Architecture & Domain

- [ ] **Aggregate invariants are enforced inside the aggregate**, not in command handlers or
  services. Command handlers only route — they contain no business logic.
- [ ] **Events are immutable records.** No mutable properties, no setters.
- [ ] **State is derived only from events.** Aggregates do not store state that is not rebuilt
  from the event stream.
- [ ] **Projections are read-only.** They never write to the event store or trigger side effects.
- [ ] **Module boundaries are respected.** Members domain code does not import Finances domain
  code and vice versa. Shared types live in `ClubAdmin.Shared` only.

## API & OpenAPI

- [ ] Every Azure Function endpoint has `[OpenApiOperation]`, `[OpenApiRequestBody]` (if POST/PUT),
  and `[OpenApiResponseWithBody]` attributes.
- [ ] HTTP verbs are semantically correct (`POST` for create, `PUT` for update, `GET` for read).
- [ ] All endpoints return typed `IActionResult` — no `HttpResponseMessage` raw returns.

## Tests

- [ ] New behaviour is covered by at least one spec.
- [ ] Test class names end with `Specs`.
- [ ] Test method names describe expected behaviour (past tense + outcome).
- [ ] Assertions use FluentAssertions — no `Assert.Equal` or similar.
- [ ] Mocking uses FakeItEasy — no `Mock<T>` (Moq) in this codebase.
- [ ] Tests do not depend on external I/O, services, or clocks.

## C# Conventions

- [ ] All types follow the naming conventions in `.github/skills/coding-conventions.md`.
- [ ] No `var` where the type is not obvious from the right-hand side.
- [ ] No magic strings or numbers — use constants or enums.
- [ ] Async methods have the `Async` suffix.
- [ ] `CancellationToken` is passed through all async call chains.
- [ ] No `.Result` or `.Wait()` calls.

## Package Management

- [ ] All NuGet package references in `.csproj` files have **no** `Version` attribute.
  Versions are centrally managed in `Directory.Packages.props`.
- [ ] No new packages added without a corresponding entry in `Directory.Packages.props`.

## Security

- [ ] No secrets, connection strings, or credentials committed to the repo.
- [ ] No hard-coded URLs or IP addresses — use configuration or Aspire service discovery.

## Build Health

- [ ] The change compiles with `TreatWarningsAsErrors=true`.
- [ ] No nullable warnings suppressed with `!` without a code comment explaining why.
- [ ] No `#pragma warning disable` without a comment explaining the suppression.
