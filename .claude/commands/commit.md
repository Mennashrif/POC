---
description: Use whenever the user wants to commit, stage, or save changes to git (e.g. "commit this", "save these changes", "make a commit"). Stages changed files and creates a well-formed conventional commit. Always prefer this over running git add/commit directly.
argument-hint: "[optional message hint]"
allowed-tools: Bash(git status:*), Bash(git diff:*), Bash(git log:*), Bash(git add:*), Bash(git commit:*)
---

# /commit

Create a single, well-formed git commit from the current working tree changes. Stages files explicitly by name (never `-A` or `.`) and writes a conventional-commit-style message focused on *why*, not what.

## When to use

- The user says "commit", "commit this", or `/commit`.
- After a logical unit of work is finished and ready to be recorded.
- Skip if the working tree is clean — report that instead of creating an empty commit.

## Steps

1. **Inspect** — run in parallel:
   - `git status` — list staged, unstaged, and untracked files.
   - `git diff` and `git diff --staged` — review actual changes.
   - `git log -5 --oneline` — match the repo's commit-message style.
2. **Filter** — exclude likely-secret files (`.env`, `*.pem`, `credentials.*`, etc.). If the user explicitly asks to commit one, warn first.
3. **Stage** — `git add <file> <file> ...` with explicit paths. Never `git add -A` or `git add .`.
4. **Compose message**:
   - Subject ≤ 72 chars, imperative mood, lowercase after the prefix.
   - Conventional-commit prefix: `feat`, `fix`, `docs`, `refactor`, `style`, `test`, `chore`, `perf`, `build`, `ci`.
   - Body (optional) explains *why* and notable trade-offs — not a file-by-file changelog.
5. **Commit** — pass the message via HEREDOC to preserve formatting:
   ```bash
   git commit -m "$(cat <<'EOF'
   feat: short why-focused subject

   Optional body paragraph.
   EOF
   )"
   ```
6. **Verify** — run `git status` to confirm a clean tree (or remaining intentionally-unstaged files).

## Related skills

- **Pushing is a separate step** — after committing, if the user also wants the changes on the remote, invoke `/push` (push only) or `/create-pr` (push + open PR). Do not run `git push` directly from this skill.
- If the user said "commit and push", run `/commit` then immediately invoke `/push`.
- If the user said "commit and open a PR", run `/commit` then invoke `/create-pr`.

## Constraints

- Do **not** push.
- Do **not** amend an existing commit unless the user explicitly asks.
- Do **not** use `--no-verify` or skip hooks. If a hook fails, fix the underlying issue and create a new commit.
- Do **not** commit files that look like secrets without explicit confirmation.

## Output

Report:
- The commit SHA and subject line.
- Any files intentionally left unstaged.
