# Shared AI Agent Instructions

This repository is the source of truth for AI World Studio. Read README.md, WORKFLOW.md, TASKS.md and ASSET_MANIFEST.md before making changes.

## Team and authority
- Human creative director: owns vision, approves scope, paid generation, publishing and destructive changes.
- ChatGPT: producer/planner/reviewer. Writes specifications, acceptance criteria and prioritized tasks. Does not claim a build works without evidence.
- Claude Code: primary technical implementation owner. Writes Unity C# and tooling, imports assets, runs tests and proposes changes.
- n8n: future workflow coordinator for approved API jobs, statuses, retries and notifications. Not the game engine or technical owner.
- Higgsfield: visual references, cinematic outputs and supported 3D assets; verify output type and suitability before Unity import.
- Blender: inspect, repair, scale, optimize and export 3D meshes; prefer repeatable scripts.
- ElevenLabs: voice/dialogue and sound generation, using approved scripts and authorized voices.
- Unity: source of truth for playable behavior and builds.
- GitHub: versioned source of truth for code, plans, issues and asset metadata.

## Rules
1. Deliver small playable increments; default first milestone is a 5–10 minute vertical slice.
2. Claude Code is the only primary writer to Unity implementation files for a task. Other agents submit specs or pull requests.
3. Use branches and pull requests for substantial work. Do not overwrite unrelated changes.
4. Do not store API keys, tokens, personal information or paid licensed binaries in git. Use environment variables or secret managers.
5. No paid API calls, publishing, deletion, merges to main, or external spending without explicit human approval.
6. Track generated assets by provenance, usage rights, format, scale, performance constraints and storage location.
7. Never mistake images or video for interactive meshes.
8. Log tests, limitations, failed jobs and costs; do not fabricate successful runs.
9. If a connection is not configured, report it as blocked and continue with independent tasks.
10. Keep changes understandable for a non-programmer creative director.
