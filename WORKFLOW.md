# Production Workflow

## Phase 0 — Connections (manual, once)
1. GitHub repository accessible to ChatGPT and Claude Code.
2. Claude Code authorized to clone and work on the repo on a machine with Unity installed.
3. Unity editor version and target platform documented.
4. Blender installed only when actual 3D processing is required.
5. Higgsfield and ElevenLabs accounts/API access verified for each desired operation.
6. n8n added after a successful manual end-to-end vertical slice.

## Phase 1 — Creative brief
Director tells ChatGPT the player fantasy, intended device, visual style, playtime and constraints. ChatGPT writes a concise spec, task breakdown and acceptance criteria in GitHub.

## Phase 2 — Technical graybox
Claude Code implements the playable interaction with placeholder objects in Unity. Human tests movement, pacing and interaction before asset spending.

## Phase 3 — Asset requests
ChatGPT creates asset records with IDs, reference images or descriptions, format requirements and budgets. Human approves paid jobs. Supported tools generate results; Blender validates meshes and exports Unity-ready files. Audio generated from approved dialogue.

## Phase 4 — Integration and tests
Claude Code imports assets, hooks up dialogue/audio/logic, runs checks, documents performance and prepares a playable build. Keep scene code separate from generated content.

## Phase 5 — Review
Human plays and provides feedback. ChatGPT triages feedback into GitHub tasks. Repeat until acceptance criteria pass.

## Phase 6 — n8n automation (later)
Trigger -> validate brief -> produce structured tasks -> human approval -> dispatch supported API jobs -> poll statuses -> validate outputs -> notify Claude worker -> Unity tests -> human review. Use idempotent job IDs, capped retries, spending limits and secrets storage. Do not presume every service offers the needed API.

## Prompt locations
- First creative prompt: ChatGPT.
- First coding prompt: Claude Code with repository access.
- n8n prompt: structured production-planner node only after working manual pipeline.
