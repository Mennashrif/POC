# /feature-deployed-skill

**description:** Use after a feature has been deployed to production. Triggered by the CI/CD pipeline after a confirmed merge to Development. Reads each service's feature-WIP.md, appends a summary to feature.md, then deletes feature-WIP.md.

**allowed-tools:** `Bash(git status:*)`, `Bash(git diff:*)`, `Bash(git log:*)`, `Bash(git push:*)`, `Bash(cat:*)`, `Bash(rm:*)`, `Bash(echo:*)`, `Bash(find:*)`, `Write`, `Read`, `Edit`

---

## When to use

- The CI/CD pipeline has confirmed a merge to the `Development` branch.
- One or more services have a `.features/feature-WIP.md` file.

---

## Step 1 — Find all feature-WIP files

Locate every `feature-WIP.md` across all services:

```bash
find . -path "*/.features/feature-WIP.md" -not -path "*/.git/*"
```

If none are found, log "No feature-WIP.md files found" and exit cleanly.

---

## Step 2 — For each service, read feature-WIP.md

For each file found (e.g. `RoomManagement/.features/feature-WIP.md`):

- Read the full contents.
- Extract:
  - The feature name (from the `# WIP:` heading).
  - The goal.
  - The commit entries under `## Commits`.
  - Any decisions recorded under `## Decisions`.

---

## Step 3 — Summarize into feature.md

For each service, open (or create) `<ServiceName>/.features/feature.md`.

Append a new section at the bottom:

```markdown
## <Feature Name> — <YYYY-MM-DD>

**Goal:** <one-line goal from WIP>

**Changes:**
- <summarized change 1>
- <summarized change 2>

**Decisions:**
- <decision 1>
```

Rules:
- Never overwrite existing content — always append.
- Keep each entry concise: a few lines, not a narrative.
- Only include changes that are meaningful to a future contributor reading this service's history.
- If nothing meaningful happened in a service (only docs or config tweaks), still record the entry but keep it to one line.

---

## Step 4 — Delete feature-WIP.md

After successfully writing to `feature.md`, delete the WIP file:

```bash
rm <ServiceName>/.features/feature-WIP.md
```

Do this for every service processed in Step 2.

---

## Step 5 — Commit and push

Stage and commit all changes (updated `feature.md` files and deleted `feature-WIP.md` files):

```
docs: update feature.md and remove WIP for <feature name>
```

Then push:

```bash
git push origin Development
```

Never force-push.

---

## Constraints

- Never overwrite `feature.md` — always append to it.
- Never delete `feature-WIP.md` before successfully writing to `feature.md`.
- Never force-push.
- If multiple services were affected, process all of them before committing — one commit covers all.

---

## Output

Log what was done:
- Which services were processed.
- Whether `feature.md` was updated (and with what feature name).
- Confirmation that `feature-WIP.md` was deleted per service.
