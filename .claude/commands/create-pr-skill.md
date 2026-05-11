# /create-pr

**description:** Use whenever the user wants to open a pull request (e.g. "create a PR", "open a pull request", "make a PR"). Runs the full pre-push + PR creation workflow. Always prefer this over running `gh pr create` directly.

**argument-hint:** `[optional PR title]`

**allowed-tools:** `Bash(git status:*)`, `Bash(git diff:*)`, `Bash(git log:*)`, `Bash(git push:*)`, `Bash(git rev-parse:*)`, `Bash(gh pr create:*)`, `Bash(gh pr view:*)`, `Bash(cat:*)`, `Bash(echo:*)`, `Bash(mkdir:*)`, `Write`, `Read`, `Edit`

---

## When to use

- The user says "create a PR", "open a PR", or `/create-pr`.
- The branch has commits not yet on `Development` and is ready for review.

---

## File system

Each service has its own `.features/` folder containing two files:

### `.features/feature-WIP.md` (short-term)
- Lives on the feature branch inside each affected service folder.
- Records commit-by-commit details of what happened during this feature.
- Written/updated at PR creation time by this skill.
- Cleaned up after deploy by `/feature-deployed-skill`.

### `.features/feature.md` (long-term)
- Lives permanently inside each service folder.
- Accumulates a summary of every feature that touched this service.
- Updated at deploy time by `/feature-deployed-skill`. Not touched by this skill.

---

## Step 0 — Ensure the tree is committed

Run `git status`.

- If there are uncommitted changes (staged or unstaged) that belong in this PR, **stop and invoke `/commit`** to record them. Do not stage or commit from inside this skill.
- If the only remaining changes are intentionally unstaged (e.g. local config), confirm with the user before proceeding.

---

## Step 1 — Detect affected services

Run:

```bash
git diff Development...HEAD --name-only
```

Group changed files by their top-level folder (e.g. `RoomManagement`, `FileManagement`). These are the affected services. Only consider folders that represent actual services (not `.github`, `.claude`, root-level files).

---

## Step 2 — Update feature-WIP.md per service

For each affected service, check if `<ServiceName>/.features/feature-WIP.md` exists.

**If it does not exist**, create the folder and file:

```bash
mkdir -p <ServiceName>/.features
```

```markdown
# WIP: <feature name>

## Goal
<one-line description of what this feature is>

## Commits
<!-- added below -->

## Decisions
<!-- notable choices and reasoning -->
```

Then, for each affected service, read the commit log for files in that service:

```bash
git log Development..HEAD --oneline -- <ServiceName>/
```

Append each commit as an entry under `## Commits`:

```markdown
### <commit message> (<short hash>)
- <what changed in this service for this commit>
- <any decision or pattern introduced>
```

Pull the detail from:
```bash
git diff Development...HEAD -- <ServiceName>/
```

If `feature-WIP.md` already exists and is current (commits already recorded), skip the update.

Commit any changes to feature-WIP files: `docs: update feature-WIP for <feature name>`.

---

## Step 3 — Push

Run these checks:

```bash
git status                        # confirm clean tree
git log Development..HEAD --oneline    # review commits going into the PR
git diff Development...HEAD            # review cumulative diff
```

Then push:

```bash
git push -u origin <current-branch>
```

Never force-push.

---

## Step 4 — Create the PR

Use `gh pr create`:

- **Title** — ≤ 70 chars, conventional-commit-style prefix optional. Use `$ARGUMENTS` if the user provided one.
- **Body** — built from three sources: all `feature-WIP.md` files (for the *why*), the branch diff (for the *what*), and the commit log (for the *sequence*).

```bash
gh pr create --title "<title>" --body "$(cat <<'EOF'
## Summary
- <bullet 1: what changed and why>
- <bullet 2>
- <bullet 3>

## Test plan
- [ ] <how to verify>
- [ ] <edge case to check>
EOF
)"
```

Do not invent test-plan items; base them on the actual change.

---

## Constraints

- Do not force-push.
- Do not skip hooks (`--no-verify`).
- Do not open a PR against Development from Development — abort if the branch is Development/main.
- Do not invent test-plan items; base them on the actual change.
- Never overwrite `feature-WIP.md` — always append to the Commits section.
- `feature.md` is not updated by this skill. That happens at deploy time.

---

## Note on feature-WIP.md lifecycle

`feature-WIP.md` is cleaned up after the feature deploys. The post-deploy pipeline triggers `/feature-deployed-skill`, which reads the WIP, appends a summary to `feature.md`, and deletes `feature-WIP.md`.

---

## Output

Return the PR URL to the user.
