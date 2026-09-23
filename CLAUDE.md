# CLAUDE.md — operating rules for this repository

## Layering

Follow [`docs/ARCHITECTURE.md`](docs/ARCHITECTURE.md): `Engine/` is the
foundation layer and must never reference `Chinese_Chess_v3.Game.*` or
`StarAnimation.*`. `Game/`, `StarAnimation/`, and the client-side `Network/`
module each depend on `Engine/` but not on each other. Before adding any
`using` from `Engine/` into `Game`/`StarAnimation`, stop — that's the
violation this repo has already spent several fixes removing.

Work through `docs/PLAN.md`'s phases in order (Engine → Game → StarAnimation
→ Network). Don't start architectural work on a later phase before the
current one is actually done — "done" means every file in scope was read,
not just the items a prior audit happened to flag (see the audit-scope rule
below).

## Build verification

This machine cannot run WinForms (macOS has no Windows Desktop runtime), but
it can compile-check:

```
dotnet build Chinese-Chess-v3.csproj -p:EnableWindowsTargeting=true
```

Baseline is **0 errors, 305 warnings** (all pre-existing `CA1416` platform-
compatibility warnings — expected for a WinForms app, not a regression
signal). Run this after every code change and confirm the warning count
didn't move before committing. This only catches compile errors, never
visual/behavioral regressions — say so explicitly when reporting a change
that touches rendering or interaction, since it was never actually seen
running.

See [`docs/CROSS-PLATFORM.md`](docs/CROSS-PLATFORM.md) for exactly which
parts of the codebase are Windows-only today (GDI+/WinForms-touching) versus
already platform-agnostic (`Engine/Physics`, `Engine/Mathematics`,
`Engine/Randomization`, `Engine/Logging`, `Engine/Network`, and nearly all of
`Game/Core`). Keep those modules free of `System.Drawing`/
`System.Windows.Forms` references when editing them — that's what makes them
portable, don't reintroduce the coupling.

## Audit scope

When asked to review or clean up a module, read every file in it. A prior
audit that admits it skipped a module (or only spot-checked something)
doesn't count as coverage — go back and actually read it before declaring the
module done. Dispatch parallel sub-agents to read files if the module is
large enough that reading it all would blow out context.

## Before touching anything

- **Never remove or delete code** (including things identified as dead code,
  debug output, or unused) **unless explicitly told to.** Document findings
  in `docs/STATUS.md` instead and leave the code as-is. This includes debug
  `Console.WriteLine` calls, visible debug overlays (e.g. a colored rectangle
  or outline used to visualize layout) — those stay unless removal is
  requested.
- Before adding a field/member "for consistency" with a sibling type, verify
  the actual structural reason the sibling has it applies here too — don't
  just pattern-match the shape. (Example: `Position`/`Velocity` have a `Base`
  field because each is *driven by* the level below it in the physics chain;
  `Acceleration` isn't driven by anything, so it correctly has no `Base` —
  adding one "for symmetry" was wrong and got reverted.)
- Before assuming a design choice is right or wrong, check how it's actually
  done elsewhere (web search for the general engineering practice, e.g. how
  physics engines integrate motion) rather than only reasoning from what's
  already in this codebase. This codebase's `Physics2D.SmoothUpdate` was
  found to integrate without `deltaTime` — a real, externally-documented bug
  (frame-rate-dependent simulation), not a stylistic quirk — precisely
  because this was checked against standard practice instead of guessed.
- Tuning constants that depend on visual feedback (e.g. `SpringK`, `Damping`,
  `AccelerationLerpFactor` in `Physics2D`) are explicitly **not** to be
  changed or re-guessed from this machine — flag them and leave them for
  the author to retune where the app can actually be seen running.

## Workflow per change

1. Plan the specific change and state the reasoning before editing.
2. Implement it.
3. Build-verify (see above) and confirm the warning/error count against
   baseline.
4. Commit — one commit per completed, independently revertible change, never
   batching unrelated fixes together. No co-author trailer on any commit.
5. Report the before/after in Traditional Chinese, comparing what changed and
   why, before moving to the next item.

To revert a change that's already committed (and possibly pushed), use
`git revert <sha>` (or `git reset --hard` + `git push --force-with-lease` if
the goal is to erase a short in-progress mistake from history entirely, not
preserve a record of it) — never hand-edit the files back and commit that as
a new, separate change.

## `.md` file language

A doc meant for Claude to read (this file, agent/skill instructions) is
written in English. A doc meant for a human to read (`README.md`,
`docs/STATUS.md`, `docs/PLAN.md`, `docs/ARCHITECTURE.md`, everything else a
person is expected to open) is written in Traditional Chinese.
