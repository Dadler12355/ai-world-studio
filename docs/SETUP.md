# Studio Setup Guide (for the Director)

Plain-language checklist of the software and accounts AI World Studio needs, in the order you need them.
**Nothing here requires spending money to get the first prototype running.**

Status key: ✅ done · ⬜ you need to do it · ⏸ later (not needed yet)

Last audited: 2026-11-16 by Claude Code (cloud session). The cloud session has **no** Unity, Blender, n8n, or API keys installed. That's expected: Unity and Blender run on *your* computer.

---

## 1. GitHub ✅ (mostly)
- ✅ Repository exists and Claude Code can clone it.
- ⬜ **Install GitHub Desktop** (free, https://desktop.github.com) so you can download ("pull") Claude's work and upload ("push") Unity's generated files without the command line.
- ⏸ Git LFS (large file storage) – only once we add real 3D models, textures or audio. GitHub's free LFS quota is small, so we'll decide on storage before using it (see TASKS.md, Milestone 0).

## 2. Unity 6 ⬜ — needed now
1. Install **Unity Hub** (free): https://unity.com/download
2. Sign in with a free Unity ID. When asked about a license, choose **Unity Personal** (free, for individuals/small teams under Unity's revenue limit).
3. In Unity Hub → **Installs** → **Install Editor** → pick the newest **Unity 6.3 LTS** (LTS = long-term support, the stable one).
   - Modules: tick your computer's build support (Windows / Mac). You can skip Android for now; add **Android Build Support** later for Meta Quest.
4. Open our project: Unity Hub → **Projects** → **Add** → **Add project from disk** → choose the `GoldRushPrototype` folder inside this repo.
   - If the Hub says the editor version differs (we pinned 6000.3.0f1), pick the 6.3 version you installed. That's safe.
   - If Unity asks to **enable the new Input System backends and restart**, click **Yes**.
5. On first open, Unity spends a few minutes importing, then **automatically builds the graybox scene**. Press ▶ **Play**.
6. Afterwards, Unity creates `.meta` files and project settings. Commit and push them with GitHub Desktop (message: "First Unity open – generated meta files"). This keeps everyone's project identical.

**Optional – let Claude Code drive the Unity Editor (MCP).** An MCP server lets an AI control the open Unity Editor (create objects, run tests, read errors). Options:
- Unity's official AI Assistant / MCP (open beta, requires a Unity subscription per Unity's docs, and the setup has been changing — check the current docs before installing).
- **MCP for Unity** by CoplayDev (free, open source, MIT).

This only helps when Claude Code runs **on your computer** (desktop app or terminal), not in this cloud session. Not required for the first prototype. Recommended later.

## 3. Claude Code on your computer ⬜ — recommended soon
Right now Claude works in a cloud container that can't run Unity, so I can write files but can't press Play. To let Claude test the game directly:
- Install the Claude desktop app or Claude Code CLI on the same computer as Unity, open this repo there, and (optionally) add a Unity MCP server.
- Until then, the loop is: Claude pushes → you pull with GitHub Desktop → you press Play and tell Claude/ChatGPT what you see.

## 4. Blender ⏸ — needed when we get real 3D models
- Free from https://www.blender.org/download/ (choose the current LTS).
- Used to check and fix meshes (scale, pivots, polygon count) and export `.fbx`/`.glb` files for Unity.
- Optional later: a Blender MCP add-on so Claude can run cleanup scripts. Claude must review any script before running it.

## 5. Higgsfield ⏸ — needed for concept art and cinematics
- What it's good for: concept images, mood boards, cinematic video. Its developer API (platform.higgsfield.ai) is documented for **image and video** generation. I did **not** find a documented 3D-model endpoint, so don't assume it can produce walkable/interactive game models. Treat its output as reference art unless we verify otherwise.
- The API is **pay-as-you-go** (you top up a balance). ⚠️ Every generation costs money → director approval required (AGENTS.md rule 5).
- Setup when ready: create an API key in the Higgsfield developer console. **Never paste it into chat or GitHub.** Store it as described in section 8.

## 6. ElevenLabs ⏸ — needed for voices and sound
- Free tier available with a limited monthly character quota; paid plans for more. Check the usage terms of whichever voice you pick before using it in a game.
- Setup when ready: ElevenLabs account → Profile → **API Keys**. Store it as described in section 8.
- Claude Code can connect through an ElevenLabs MCP server. ElevenLabs publishes one on GitHub (`elevenlabs/elevenlabs-mcp`). Confirm it's the official one before installing.
- First use: ChatGPT writes the merchant's lines → you approve → generate 3–5 voice clips → recorded in ASSET_MANIFEST.md.

## 7. n8n ⏸ — after the manual pipeline works (Milestone 3)
- Options: n8n Cloud (paid subscription with a trial) or self-hosted (free Community Edition, needs Docker or Node.js on a computer/server that stays on).
- It will run: brief → your approval → paid API job (Higgsfield/ElevenLabs) → status polling → notify Claude → Unity checks → your review.
- Build it only after we've done the whole loop once by hand (WORKFLOW.md Phase 6).

## 8. Secrets (API keys) — rules
- Never commit keys to GitHub and never paste them into ChatGPT or Claude chat.
- On your computer: put them in a `.env` file at the repo root (already ignored by git). Use `.env.example` as the template.
- For Claude Code cloud sessions: add keys through the environment's settings (Claude can open a secure form) rather than in chat.
- In n8n: use n8n's built-in Credentials store.

---

## Connection checklist

| Connection | Needed for | Status | Who acts |
|---|---|---|---|
| GitHub ↔ Claude Code | All work | ✅ Working (clone verified) | — |
| GitHub ↔ ChatGPT | Briefs, reviews | ⬜ Not verified | Director connects GitHub in ChatGPT |
| GitHub Desktop on your PC | Pull/push Unity files | ⬜ | Director |
| Unity Hub + Unity 6.3 LTS | Playing the prototype | ⬜ | Director |
| Claude Code on Unity machine | Claude tests in-editor | ⬜ Recommended | Director |
| Unity MCP | Claude drives the editor | ⏸ Optional | Director installs, Claude configures |
| Blender | Mesh cleanup | ⏸ | Director |
| Higgsfield API key | Concept art / video | ⏸ Paid, needs approval | Director |
| ElevenLabs API key | Voices | ⏸ Free tier possible | Director |
| n8n | Automation | ⏸ Milestone 3 | Later |
