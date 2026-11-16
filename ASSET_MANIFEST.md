# Asset Manifest

Keep a record for every external or generated asset. Do not commit large binaries unless the repository's storage strategy explicitly allows them.

| Asset ID | Purpose | Source/provider | Type/format | Storage path or URL | License/rights | Scale/poly budget | Status | Cost |
|---|---|---|---|---|---|---|---|---|
| placeholder-town-001 | Graybox environment | Unity primitives | Prefabs | TBD | Original | Low | Planned | $0 |

## Required checks
- Interactive 3D assets need geometry, UVs/materials, suitable scale, pivots and collision strategy.
- Video and images are cinematic/reference material unless converted through a validated 3D pipeline.
- Record provider model, prompt/version, generation date, job ID, and applicable license terms.
- Store API credentials outside the repository.
