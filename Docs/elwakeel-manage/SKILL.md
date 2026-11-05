---
name: elwakeel-manage
description: Turn the Discover Egypt SRS into a dependency-ordered work proposal of phases, features, cycles, and tasks. Use when asked to plan or sequence project work from the SRS.
---

# Elwakeel Manage

Create a suggested work order only. Do not assign people, dates, or sprints. Do not write or modify application code.

## Sources

- Read `Docs/SRS.md`.
- Also read `Docs/CLASSES_SPECS.md` only if that exact file exists.
- Do not use other files as input. Do not fill gaps by guessing.
- Carry unresolved items from the SRS `Open Questions` section into an `Open Questions` section in the work order. Do not turn them into committed requirements or cycles until clarified.

## Step 1: Order FRs into phases

Order work by dependency first, then by value:

1. **Phase 1 — Foundations:** requirements other FRs depend on.
2. **Phase 2 — Core flow.**
3. **Phase 3 — Secondary.**
4. **Phase 4 — Later:** only if time remains.

For every FR, list the BRs it enforces, the NFRs that apply, and its prerequisite FRs as `needs: FR-xx`. Put NFRs that apply across the product (such as authentication, validation, and logging) under **Cross-cutting**, in the earliest phase where each is needed.

## Step 2: Break features into cycles

Represent each feature as action-focused cycles, with one action on one entity per cycle. Include only actions required by the FR, such as Add, List, Details, Edit, or Delete/Cancel.

Each cycle owns that action end to end:

`form contract + validation → Controller GET/POST → View → Application handler/service with its BRs → Repository/DbContext → tests for its BRs`

- Use a DTO directly for a form or view when it already represents the required data. Keep a ViewModel only when it has a distinct presentation or form purpose; do not plan DTO-to-ViewModel mappings.
- Follow the project's established application pattern for the feature (for example, its CQRS handler). Do not add layers, patterns, or packages solely for the work order.
- Choose the simplest design that satisfies the FR and BRs.

## Step 3: Break cycles into tasks

List small tasks that can each be completed and tested independently. Order tasks by layer:

`DAL → BLL → PL → Test`

## Output

Write the proposal to `Docs/WORK_ORDER.md` as a tree:

`Phase → Features (FR + BR + NFR + needs) → Cycles → Tasks`

Keep the `Open Questions` separate from committed work. The work order is a proposal, not an assignment or implementation.

Next: send each approved cycle to **ElwakeelBuild**, for example: `plan FR-02 Add Booking`.
