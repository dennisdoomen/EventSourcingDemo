# Check Rework Skill

Use this skill to verify that a pull request has properly addressed all review comments before
merging. Run through this checklist after the author pushes a "rework" commit.

---

## Review Comment Coverage

- [ ] Every open review thread has either been:
  - **Resolved** by a code change, or
  - **Replied to** with a clear explanation of why the requested change was not made.
- [ ] No threads were silently closed without a code change or explanation.
- [ ] Reviewer has re-approved (or the PR has the required approvals) after the rework.

## Build Verification

- [ ] The latest CI run on the rework branch is **green** (all jobs passed).
- [ ] No new compiler warnings were introduced (`TreatWarningsAsErrors=true` must pass).
- [ ] No new `#pragma warning disable` added without a comment.

## Test Coverage

- [ ] All existing tests still pass.
- [ ] If the rework changes behaviour, at least one spec was added or updated to cover the change.
- [ ] No tests were deleted to make the build pass.

## Package Management

- [ ] No new `Version` attributes snuck into `.csproj` files — versions belong in `Directory.Packages.props`.
- [ ] If a new package was added, it appears in `Directory.Packages.props` with a pinned version.

## Architecture Compliance

- [ ] Aggregate invariants are still enforced inside the aggregate.
- [ ] Events remain immutable records.
- [ ] Module boundary (Members vs Finances) is not violated.
- [ ] Projections do not write to the event store.

## Security & Secrets

- [ ] No secrets or credentials introduced in the rework.
- [ ] No hard-coded connection strings or URLs.

---

**If all items above are checked, the rework is complete and the PR is ready to merge.**
