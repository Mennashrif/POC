---
description: Use whenever the user wants to push the current branch to the remote (e.g. "push", "push this", "send to origin"). Pushes the current branch to origin with no force and no skipped hooks. Always prefer this over running git push directly.
allowed-tools: Bash(git status:*), Bash(git remote:*), Bash(git rev-parse:*), Bash(git push:*)
---

# /push

Push the current branch to `origin`, setting upstream if it isn't tracked yet.

## When to use

- The user says "push" or `/push`.
- After committing work that is ready to share.

## Related skills (disambiguation)

- **Use `/push`** for a one-off push of already-committed work. No PR is opened.
- **Use `/create-pr`** when the goal is to share work for review — it pushes *and* opens a PR with a structured body. Don't run `/push` then manually `gh pr create`; let `/create-pr` do both.
- **Use `/commit` first** if `git status` shows uncommitted changes — `/push` will not stage or commit for you.

## Steps

1. **Pre-flight** — run in parallel:
   - `git status` — confirm no uncommitted changes that should be part of this push.
   - `git remote -v` — confirm `origin` exists.
   - `git rev-parse --abbrev-ref HEAD` — capture the current branch name.
2. **Push** — `git push -u origin <current-branch>`.
3. **Report** — surface the remote URL and any PR-creation link GitHub prints in the response.

## Constraints

- Do **not** force-push (`--force`, `--force-with-lease`) unless the user explicitly asks. Never force-push `master`/`main`.
- Do **not** use `--no-verify` or skip hooks. If a pre-push hook fails, diagnose and fix.
- If there are uncommitted changes, stop and ask the user whether to commit, stash, or push as-is.

## Output

Report:
- Branch name and remote tracking ref.
- The PR-creation URL printed by GitHub, if any.
