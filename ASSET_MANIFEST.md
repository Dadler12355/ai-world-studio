# Asset Manifest

Keep a record for every external or generated asset. Do not commit large binaries unless the repository's storage strategy explicitly allows them.

| Asset ID | Purpose | Source/provider | Type/format | Storage path or URL | License/rights | Scale/poly budget | Status | Cost |
|---|---|---|---|---|---|---|---|---|
| placeholder-town-001 | Graybox town (6 buildings, props, street) | Unity primitives via PrototypeSceneBuilder | Scene + .mat | GoldRushPrototype/Assets/_Project/Scenes/GoldRush_Graybox.unity (generated) | Original | Low (primitives) | Built, unverified | $0 |
| placeholder-merchant-001 | Merchant NPC placeholder | Unity primitives | Prefab | GoldRushPrototype/Assets/_Project/Prefabs/Merchant_NPC_Placeholder.prefab (generated) | Original | Low (primitives) | Built, unverified | $0 |
| dialogue-merchant-intro-001 | Merchant greeting text (placeholder copy) | Claude Code | JSON | GoldRushPrototype/Assets/_Project/Content/Dialogue/merchant_intro.json | Original | n/a | Draft, needs ChatGPT/director rewrite | $0 |

## Required checks
- Interactive 3D assets need geometry, UVs/materials, suitable scale, pivots and collision strategy.
- Video and images are cinematic/reference material unless converted through a validated 3D pipeline.
- Record provider model, prompt/version, generation date, job ID, and applicable license terms.
- Store API credentials outside the repository.
