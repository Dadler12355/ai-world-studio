# Production Backlog

Legend: [ ] todo · [~] built, needs verification · [x] done

## Milestone 0 — Studio setup
- [ ] Install and verify Unity 6.3 LTS editor on build machine. (Steps: docs/SETUP.md §2)
- [x] Connect Claude Code to this GitHub repository. (Verified 2026-11-16: clone works from Claude Code cloud.)
- [ ] Decide desktop-first target and later Quest requirements. (Proposed: desktop Windows/Mac first, Built-in RP now, URP before Quest.)
- [ ] Optional: run Claude Code on the Unity machine + Unity MCP so Claude can test in-editor.
- [ ] Verify Blender installation if required.
- [ ] Verify Higgsfield 3D output/API capability and ElevenLabs API access before automation.
- [ ] Choose external asset storage and secret management.

## Milestone 1 — First playable graybox
- [~] Create a Unity project and a walkable prototype scene. (Files ready in GoldRushPrototype/; awaiting first open + playtest by director.)
- [~] Add a merchant NPC placeholder with interaction prompt. (Talk with E, dialogue from JSON; awaiting playtest.)
- [ ] Implement a simple trading choice and feedback.
- [ ] Add basic UI and scene restart.
- [ ] Test desktop build and document actual results.

## Milestone 2 — Content
- [ ] Define Gold Rush art direction and 5-asset manifest.
- [ ] Approve generation costs and create/validate assets.
- [ ] Add approved voice lines and ambience.
- [ ] Replace placeholders and measure performance.

## Milestone 3 — Automation
- [ ] Create n8n orchestration only after manual pipeline works.
- [ ] Add approval gates, job tracking, retry policy and cost limits.
- [ ] Validate one end-to-end automated task.
