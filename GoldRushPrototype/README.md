# Gold Rush Prototype (Unity 6)

Desktop-first graybox for the California Gold Rush vertical slice. Everything is built from Unity
primitive shapes, with no paid or generated assets.

## How to play it
1. Install Unity Hub and Unity 6.3 LTS (see [docs/SETUP.md](../docs/SETUP.md)).
2. Unity Hub → Projects → Add → Add project from disk → select this `GoldRushPrototype` folder.
3. Wait for the first import. The scene **builds itself** and opens:
   `Assets/_Project/Scenes/GoldRush_Graybox.unity`.
   If it doesn't open, use the menu **AI World Studio → Open Gold Rush Graybox Scene**.
4. Press ▶ Play.

**Controls:** WASD / arrows move · mouse looks · Shift sprints · **E** talks to the merchant · Esc frees the mouse (click to resume).

**What you should see:** a dusty main street with six false-front buildings (General Store, Saloon,
Assay Office, Livery Stable, Hotel, Post Office), a town entrance sign, barrels, crates and a wagon.
A merchant in a hat and apron stands at a table in front of the General Store. Walk up to them for the "[E] Talk" prompt.

## Folder layout
```
GoldRushPrototype/
  Assets/
    _Project/                  ← everything we make lives here
      Content/Dialogue/        ← dialogue text (JSON), safe for ChatGPT/writers to edit
      Scripts/Runtime/         ← game code (player, merchant, HUD, input)
      Scripts/Editor/          ← scene builder tool (editor only, not in builds)
      Scenes/                  ← generated on first open
      Prefabs/                 ← generated on first open (Player, Merchant_NPC_Placeholder)
      Materials/Graybox/       ← generated on first open (flat colors)
  Packages/manifest.json       ← Unity packages (Input System, UI)
  ProjectSettings/             ← Unity fills this in on first open
```
Future: `Assets/ThirdParty/` for store assets, and imported generated assets will go under
`Assets/_Project/Art/` and `Assets/_Project/Audio/` after they pass the ASSET_MANIFEST checks.

## Changing things without code
- **Merchant dialogue:** edit `Assets/_Project/Content/Dialogue/merchant_intro.json`.
- **Walk speed / mouse sensitivity:** select the Player in the scene → Inspector → Player Controller.
- **Talk distance:** select the merchant → Merchant NPC → Interact Radius.
- **Start over:** menu **AI World Studio → Rebuild Gold Rush Graybox Scene** (keeps prefabs, materials and dialogue).

## Technical notes
- Render pipeline: Built-in, for the simplest first open. Switching to URP is planned before Quest work. The scene builder already picks a URP shader if URP is active.
- Input: works with both the new Input System and the old Input Manager.
- UI: Unity's immediate-mode GUI (OnGUI) for the graybox. It will be replaced with real UI later.
- Automation entry point (for a future n8n/CI worker):
  `Unity -batchmode -quit -projectPath GoldRushPrototype -executeMethod AIWorldStudio.EditorTools.PrototypeSceneBuilder.BuildFromCommandLine`
