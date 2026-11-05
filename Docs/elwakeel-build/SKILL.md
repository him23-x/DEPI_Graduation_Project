---
name: elwakeel-build
description: Plan, implement, or review one Discover Egypt development cycle, or update session state when the user says done.
---

# Elwakeel Build

Use this workflow only for the requested development cycle. Do not scan the whole repository or implement work outside its task file.

## Read boundaries

- Read `AGENT.md` and `Docs/STATE.md` first.
- For `plan`, read only the requested cycle in `Docs/WORK_ORDER.md` and the relevant entity section in `Docs/CLASSES_SPECS.md`. No task file is expected yet.
- For `build` or `review`, read only the current cycle's task file.
- If a required file is missing, report it and stop. Do not search for substitute files.
- Do not paste project documents back. Refer to requirements by `FR-xx` and rules by `BR-xx`.

## Commands

### `plan FR-xx [Cycle]`

1. Start with **What to respect**. List every BR and NFR for the cycle from `Docs/WORK_ORDER.md`, the entity validation rules from `Docs/CLASSES_SPECS.md`, and the relevant `AGENT.md` rules. Cite the source file on each line.
2. Write `tasks/FR-xx-<cycle>.md` with no more than five steps. Include files to change and acceptance criteria; include an acceptance criterion for each item in **What to respect**.
3. Do not write code. Wait for the user.

### `build FR-xx [Cycle]`

Implement only the approved task file for that cycle. Follow its task order and complete its Execution Report. Do not expand the scope.

### `review FR-xx [Cycle]`

Read the cycle task file and the diff for its listed files only. Return numbered findings without praise. Do not edit files.

### `done`

Update `Docs/STATE.md` to no more than 20 lines, with **Done**, **Next**, and **Open questions**.

## Cycle design

- A cycle covers one action on one entity, such as Add, List, Details, Edit, or Delete/Cancel, only when required by its FR.
- Cover the action end to end: form contract and validation → Controller GET/POST → View → Application handler/service and its BRs → Repository/DbContext → tests for its BRs.
- Use a DTO directly when it already represents the form or view data. Keep a ViewModel only for a distinct presentation or form purpose; do not add DTO-to-ViewModel mappings.
- Follow the project's established Application pattern. Do not add layers, patterns, or packages solely for the cycle.
- Keep tasks small and independently testable. Order implementation tasks: DAL → BLL → PL → Test.

## Response constraints

- Show changes as diffs; do not reprint whole files.
- Do not recap the request or provide a step-by-step summary.
- Keep answers short unless the user asks for an explanation.

## Gemini

Configure `GEMINI.md` to import the same guidance:

- `@./AGENT.md`
- `@./.claude/skills/elwakeel-build/SKILL.md`
