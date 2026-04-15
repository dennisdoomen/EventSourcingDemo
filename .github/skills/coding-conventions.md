# C# Coding Conventions

This skill captures the C# coding guidelines used in the ClubAdmin project, based on
[csharpcodingguidelines.com](https://csharpcodingguidelines.com/) by Dennis Doomen.

---

## Basic Principles

- Follow **SOLID**, **DRY**, **KISS**, **YAGNI**, and the **Principle of Least Surprise**.
- Prefer **composition over inheritance**.
- Keep types small and focused — a class should have a **single reason to change**.
- Avoid premature optimization; write readable code first.

---

## Class Design

- Give a class one responsibility. If it does too much, split it.
- Prefer `sealed` classes unless designed for inheritance.
- Avoid deep inheritance hierarchies (more than 2 levels is a warning sign).
- Mark all fields `private` or `private readonly` unless there is a strong reason otherwise.
- Prefer `record` types for value objects and DTOs — they are immutable by default.
- Use `static` methods for pure functions with no side effects.

---

## Member Design

- Methods must do **one thing**. If a method needs an `And` in its name, split it.
- Limit method parameters to **≤ 3**. Introduce a parameter object when you need more.
- Return early to avoid deep nesting. Use guard clauses at the top.
- Avoid `out` and `ref` parameters — they indicate a design problem.
- Prefer `async`/`await` over `Task.Result` or `Task.Wait()` — always.
- Suffix async methods with `Async`.
- Do not expose `IQueryable<T>` outside of infrastructure layers.

---

## Naming

| Element | Convention | Example |
|---|---|---|
| Types, methods, properties | PascalCase | `RegisterMember`, `MemberId` |
| Local variables, parameters | camelCase | `memberId`, `amount` |
| Private fields | camelCase with leading underscore | `_connectionString` |
| Constants | PascalCase | `MaxRetryCount` |
| Interfaces | `I` prefix | `IAuditLog` |
| Enums | PascalCase, singular | `MembershipType.Regular` |
| Test classes | Noun + `Specs` suffix | `MemberRegistrationSpecs` |

- Use **meaningful, intent-revealing names**. Avoid abbreviations (`cnt`, `mgr`, `tmp`).
- Do not use Hungarian notation (`strName`, `iCount`).
- Avoid noise words (`Helper`, `Manager`, `Processor`) unless they genuinely describe a role.

---

## Statements and Expressions

- One statement per line.
- No magic numbers or strings — use named constants or enums.
- Prefer `var` when the type is apparent from the right-hand side.
- Do not use `var` when the type is not obvious.
- Use pattern matching and switch expressions over long if-else chains.
- Prefer expression bodies (`=>`) only for trivially short members (one expression).

---

## Layout and Formatting

- Braces on **new lines** (Allman style).
- Use **blank lines** to separate logical groups of code within a method.
- Keep lines under **130 characters**.
- One class per file; file name matches class name.
- Organize members: constants → fields → constructors → properties → public methods → private methods.

---

## Null Handling

- Enable **nullable reference types** (`<Nullable>enable</Nullable>`).
- Avoid returning `null` from methods — prefer `Option<T>` or throw when contract is violated.
- Use the **null-object pattern** to avoid null checks in callers.
- Never suppress nullable warnings with `!` unless you have a very specific reason and a comment.

---

## Async Guidelines

- Every async method must be `await`-ed — never fire-and-forget.
- Never call `.Result` or `.Wait()` on a `Task` — it causes deadlocks in some contexts.
- Pass `CancellationToken` through all async call chains.
- Configure `ConfigureAwait(false)` in library code; not required in application code.

---

## LINQ

- Prefer **method syntax** (`Where`, `Select`, `FirstOrDefault`) over query syntax.
- Do not perform side effects inside a LINQ query.
- Materialize sequences (`ToList()`, `ToArray()`) before returning from a method if deferred
  execution could cause issues.

---

## Documentation

- Write XML documentation (`///`) on all **public API surface**.
- Do not document private implementation details unless the logic is non-obvious.
- Comments should explain **why**, not **what** — the code should explain what.

---

## Testing Conventions

- Test class names end with `Specs` — they are specifications, not tests.
- Test method names describe the **expected behavior**, not the method being called.
  - ✅ `Registering_a_member_with_valid_data_raises_MemberRegistered`
  - ❌ `TestRegisterMember`
- Use **FluentAssertions** for all assertions.
- Use **FakeItEasy** for mocking. Prefer fake over mock when you need a test double.
- Use **Mockly** for HTTP-level mocking.
- Tests must not depend on external services or I/O — mock everything.
- Arrange–Act–Assert structure with a blank line separating each section.

---

## Event Sourcing Conventions

- **Events** are immutable `record` types. Never mutate an event.
- **Aggregates** enforce invariants — do not enforce business rules in command handlers.
- **Command handlers** only route commands to the aggregate — they contain no business logic.
- **Projections** are read-only views — never modify the event stream from a projection.
- Event names are past-tense verbs: `MemberRegistered`, `TransactionImported`.
- Command names are present-tense imperatives: `RegisterMember`, `ImportTransaction`.
