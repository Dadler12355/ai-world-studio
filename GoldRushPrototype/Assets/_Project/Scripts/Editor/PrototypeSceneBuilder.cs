using System.IO;
using System.Linq;
using AIWorldStudio.GoldRush;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

namespace AIWorldStudio.EditorTools
{
    /// <summary>
    /// Builds the Gold Rush graybox scene from Unity primitives (cubes, cylinders, capsules).
    ///
    /// - Runs automatically the first time the project is opened (if the scene does not exist yet).
    /// - Menu: AI World Studio > Rebuild Gold Rush Graybox Scene.
    /// - Command line: Unity -batchmode -quit -projectPath GoldRushPrototype
    ///       -executeMethod AIWorldStudio.EditorTools.PrototypeSceneBuilder.BuildFromCommandLine
    ///
    /// Existing prefabs and materials are reused, never overwritten, so hand-made changes survive.
    /// </summary>
    [InitializeOnLoad]
    public static class PrototypeSceneBuilder
    {
        private const string Root = "Assets/_Project";
        private const string ScenePath = Root + "/Scenes/GoldRush_Graybox.unity";
        private const string PrefabDir = Root + "/Prefabs";
        private const string MaterialDir = Root + "/Materials/Graybox";
        private const string MerchantPrefabPath = PrefabDir + "/Merchant_NPC_Placeholder.prefab";
        private const string PlayerPrefabPath = PrefabDir + "/Player.prefab";
        private const string DialoguePath = Root + "/Content/Dialogue/merchant_intro.json";
        private const string AutoBuildFlag = "AIWorldStudio.GoldRush.AutoBuildChecked";

        static PrototypeSceneBuilder()
        {
            if (Application.isBatchMode) return;
            EditorApplication.delayCall += TryAutoBuild;
        }

        private static void TryAutoBuild()
        {
            if (SessionState.GetBool(AutoBuildFlag, false)) return;
            if (EditorApplication.isCompiling || EditorApplication.isUpdating)
            {
                EditorApplication.delayCall += TryAutoBuild;
                return;
            }
            SessionState.SetBool(AutoBuildFlag, true);
            if (EditorApplication.isPlayingOrWillChangePlaymode) return;
            if (File.Exists(ScenePath)) return;

            Debug.Log("[AI World Studio] Graybox scene not found — building it now.");
            Build();
        }

        [MenuItem("AI World Studio/Rebuild Gold Rush Graybox Scene")]
        private static void RebuildFromMenu()
        {
            if (File.Exists(ScenePath) && !EditorUtility.DisplayDialog(
                    "Rebuild graybox scene?",
                    "This replaces GoldRush_Graybox.unity with a freshly generated copy.\n\n" +
                    "Prefabs, materials and dialogue files are kept. Any objects you added by hand " +
                    "to the scene itself will be lost.",
                    "Rebuild", "Cancel"))
            {
                return;
            }
            Build();
        }

        [MenuItem("AI World Studio/Open Gold Rush Graybox Scene")]
        private static void OpenFromMenu()
        {
            if (!File.Exists(ScenePath)) { Build(); return; }
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                EditorSceneManager.OpenScene(ScenePath);
        }

        /// <summary>Entry point for automation (CI, n8n worker). Only builds if the scene is missing,
        /// unless -forceRebuild is passed.</summary>
        public static void BuildFromCommandLine()
        {
            bool force = System.Environment.GetCommandLineArgs().Contains("-forceRebuild");
            if (File.Exists(ScenePath) && !force)
            {
                Debug.Log("[AI World Studio] Scene already exists; pass -forceRebuild to regenerate.");
                return;
            }
            Build();
        }

        public static void Build()
        {
            if (!Application.isBatchMode && !EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;

            EnsureFolder(Root + "/Scenes");
            EnsureFolder(PrefabDir);
            EnsureFolder(MaterialDir);

            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            BuildLighting();
            Transform env = new GameObject("Environment").transform;
            BuildGroundAndStreet(env);
            BuildTown(env);
            BuildProps(env);
            BuildBoundaries(env);

            GameObject player = GetOrCreatePrefab(PlayerPrefabPath, CreatePlayer);
            player.transform.SetPositionAndRotation(new Vector3(0f, 0f, -22f), Quaternion.identity);

            GameObject merchant = GetOrCreatePrefab(MerchantPrefabPath, CreateMerchant);
            // Stands at a market table in front of the General Store, facing the street.
            merchant.transform.SetPositionAndRotation(new Vector3(-2.7f, 0f, -6f), Quaternion.Euler(0f, 90f, 0f));

            var hud = new GameObject("HUD").AddComponent<PrototypeHud>();
            hud.player = player.GetComponent<PlayerController>();

            EditorSceneManager.SaveScene(scene, ScenePath);
            AddSceneToBuildSettings(ScenePath);
            AssetDatabase.SaveAssets();
            Debug.Log($"[AI World Studio] Built {ScenePath}. Press Play to walk around.");
        }

        // ---------------------------------------------------------------- Lighting

        private static void BuildLighting()
        {
            var sun = new GameObject("Sun").AddComponent<Light>();
            sun.type = LightType.Directional;
            sun.color = new Color(1f, 0.93f, 0.8f);
            sun.intensity = 1.2f;
            sun.shadows = LightShadows.Soft;
            sun.transform.rotation = Quaternion.Euler(45f, -35f, 0f);
            RenderSettings.sun = sun;

            RenderSettings.ambientMode = AmbientMode.Trilight;
            RenderSettings.ambientSkyColor = new Color(0.75f, 0.72f, 0.65f);
            RenderSettings.ambientEquatorColor = new Color(0.62f, 0.52f, 0.4f);
            RenderSettings.ambientGroundColor = new Color(0.35f, 0.27f, 0.2f);

            RenderSettings.fog = true;
            RenderSettings.fogMode = FogMode.ExponentialSquared;
            RenderSettings.fogDensity = 0.012f;
            RenderSettings.fogColor = new Color(0.86f, 0.78f, 0.64f);
        }

        // ---------------------------------------------------------------- Environment

        private static void BuildGroundAndStreet(Transform parent)
        {
            var ground = Prim(PrimitiveType.Plane, "Ground", parent, Vector3.zero, new Vector3(10f, 1f, 10f), Mat("Dirt", 0.62f, 0.5f, 0.34f));
            ground.isStatic = true;
            var street = Prim(PrimitiveType.Cube, "MainStreet", parent, new Vector3(0f, 0.01f, 0f), new Vector3(9f, 0.02f, 60f), Mat("StreetDirt", 0.52f, 0.41f, 0.28f));
            Object.DestroyImmediate(street.GetComponent<Collider>());
            street.isStatic = true;
        }

        private static void BuildTown(Transform parent)
        {
            Transform town = new GameObject("Town").transform;
            town.SetParent(parent, false);

            // West side of the street (fronts face east, +X).
            Building(town, "GENERAL STORE", new Vector3(-9f, 0f, -6f), Vector3.right, 8f, 4f, Mat("WoodLight", 0.66f, 0.52f, 0.36f));
            Building(town, "ASSAY OFFICE", new Vector3(-9f, 0f, 6f), Vector3.right, 7f, 3.6f, Mat("WoodGray", 0.55f, 0.52f, 0.48f));
            Building(town, "HOTEL", new Vector3(-9f, 0f, 18f), Vector3.right, 9f, 6f, Mat("WoodRed", 0.55f, 0.3f, 0.22f));

            // East side (fronts face west, -X).
            Building(town, "SALOON", new Vector3(9f, 0f, -4f), Vector3.left, 9f, 5f, Mat("WoodDark", 0.42f, 0.3f, 0.2f));
            Building(town, "LIVERY STABLE", new Vector3(9f, 0f, 8f), Vector3.left, 9f, 4.5f, Mat("WoodBarn", 0.5f, 0.36f, 0.24f));
            Building(town, "POST OFFICE", new Vector3(9f, 0f, 18f), Vector3.left, 6f, 3.6f, Mat("WoodPale", 0.72f, 0.66f, 0.55f));

            // Town entrance arch near the player start.
            var arch = new GameObject("EntranceSign").transform;
            arch.SetParent(town, false);
            arch.position = new Vector3(0f, 0f, -16f);
            Material post = Mat("WoodDark", 0.42f, 0.3f, 0.2f);
            Prim(PrimitiveType.Cube, "PostL", arch, new Vector3(-4f, 2.5f, 0f), new Vector3(0.3f, 5f, 0.3f), post);
            Prim(PrimitiveType.Cube, "PostR", arch, new Vector3(4f, 2.5f, 0f), new Vector3(0.3f, 5f, 0.3f), post);
            Prim(PrimitiveType.Cube, "Board", arch, new Vector3(0f, 4.6f, 0f), new Vector3(8.6f, 0.9f, 0.15f), Mat("Signboard", 0.3f, 0.22f, 0.15f));
            Label(arch, "PLACEHOLDER TOWN  ·  EST. 1849", new Vector3(0f, 4.6f, -0.1f), Quaternion.identity, 0.11f);
        }

        /// <summary>A western "false-front" building: box body + taller flat facade + door + porch + sign.</summary>
        private static void Building(Transform parent, string sign, Vector3 position, Vector3 front, float width, float height, Material wall)
        {
            const float depth = 8f;
            var root = new GameObject("Building_" + sign.Replace(' ', '_')).transform;
            root.SetParent(parent, false);
            root.SetPositionAndRotation(position, Quaternion.LookRotation(front, Vector3.up));

            Material trim = Mat("Trim", 0.3f, 0.22f, 0.15f);
            float faceZ = depth / 2f;

            Prim(PrimitiveType.Cube, "Body", root, new Vector3(0f, height / 2f, 0f), new Vector3(width, height, depth), wall);
            Prim(PrimitiveType.Cube, "Roof", root, new Vector3(0f, height + 0.1f, -0.3f), new Vector3(width + 0.3f, 0.2f, depth + 0.6f), trim);
            Prim(PrimitiveType.Cube, "FalseFront", root, new Vector3(0f, (height + 1.6f) / 2f, faceZ + 0.1f), new Vector3(width, height + 1.6f, 0.2f), wall);
            Prim(PrimitiveType.Cube, "Door", root, new Vector3(0f, 1.1f, faceZ + 0.23f), new Vector3(1.3f, 2.2f, 0.06f), trim);
            Prim(PrimitiveType.Cube, "WindowL", root, new Vector3(-width * 0.3f, 1.6f, faceZ + 0.23f), new Vector3(1.2f, 1f, 0.05f), Mat("Window", 0.18f, 0.2f, 0.22f));
            Prim(PrimitiveType.Cube, "WindowR", root, new Vector3(width * 0.3f, 1.6f, faceZ + 0.23f), new Vector3(1.2f, 1f, 0.05f), Mat("Window", 0.18f, 0.2f, 0.22f));
            Prim(PrimitiveType.Cube, "Porch", root, new Vector3(0f, 0.1f, faceZ + 0.95f), new Vector3(width, 0.2f, 1.5f), trim);

            float signY = height + 0.6f;
            Prim(PrimitiveType.Cube, "SignBoard", root, new Vector3(0f, signY, faceZ + 0.25f), new Vector3(width * 0.8f, 0.8f, 0.08f), Mat("Signboard", 0.3f, 0.22f, 0.15f));
            // Text faces whoever stands in front of the building.
            Label(root, sign, new Vector3(0f, signY, faceZ + 0.3f), Quaternion.Euler(0f, 180f, 0f), 0.09f);

            root.gameObject.isStatic = true;
            foreach (Transform t in root) t.gameObject.isStatic = true;
        }

        private static void BuildProps(Transform parent)
        {
            Transform props = new GameObject("Props").transform;
            props.SetParent(parent, false);
            Material barrel = Mat("Barrel", 0.45f, 0.32f, 0.2f);
            Material crate = Mat("Crate", 0.7f, 0.56f, 0.36f);

            // Merchant's market table in front of the General Store.
            Prim(PrimitiveType.Cube, "MarketTable", props, new Vector3(-1.6f, 0.45f, -6f), new Vector3(0.8f, 0.9f, 2.4f), Mat("WoodLight", 0.66f, 0.52f, 0.36f));
            Prim(PrimitiveType.Cube, "GoodsCrate", props, new Vector3(-1.6f, 1.05f, -6.6f), new Vector3(0.4f, 0.3f, 0.4f), crate);
            Prim(PrimitiveType.Cylinder, "FlourSack", props, new Vector3(-1.6f, 1.1f, -5.5f), new Vector3(0.35f, 0.2f, 0.35f), Mat("Sack", 0.85f, 0.8f, 0.68f));

            Prim(PrimitiveType.Cylinder, "Barrel1", props, new Vector3(-4.3f, 0.6f, -10f), new Vector3(0.8f, 0.6f, 0.8f), barrel);
            Prim(PrimitiveType.Cylinder, "Barrel2", props, new Vector3(-4.3f, 0.6f, -9f), new Vector3(0.8f, 0.6f, 0.8f), barrel);
            Prim(PrimitiveType.Cube, "Crate1", props, new Vector3(-4.2f, 0.4f, -2f), new Vector3(0.8f, 0.8f, 0.8f), crate);
            Prim(PrimitiveType.Cube, "Crate2", props, new Vector3(-4.2f, 1.1f, -2.1f), new Vector3(0.6f, 0.6f, 0.6f), crate);

            // Water trough and hitching post by the Saloon.
            Prim(PrimitiveType.Cube, "WaterTrough", props, new Vector3(2.8f, 0.35f, -4f), new Vector3(0.8f, 0.7f, 2.5f), barrel);
            Prim(PrimitiveType.Cube, "HitchRail", props, new Vector3(3f, 1f, 1.5f), new Vector3(0.12f, 0.12f, 3f), barrel);
            Prim(PrimitiveType.Cube, "HitchPostA", props, new Vector3(3f, 0.5f, 0.1f), new Vector3(0.15f, 1f, 0.15f), barrel);
            Prim(PrimitiveType.Cube, "HitchPostB", props, new Vector3(3f, 0.5f, 2.9f), new Vector3(0.15f, 1f, 0.15f), barrel);

            // A wagon at the far end of the street.
            var wagon = new GameObject("Wagon").transform;
            wagon.SetParent(props, false);
            wagon.SetPositionAndRotation(new Vector3(1.5f, 0f, 24f), Quaternion.Euler(0f, 20f, 0f));
            Prim(PrimitiveType.Cube, "Bed", wagon, new Vector3(0f, 1f, 0f), new Vector3(1.8f, 0.6f, 3.5f), crate);
            Prim(PrimitiveType.Cylinder, "Cover", wagon, new Vector3(0f, 1.6f, 0f), new Vector3(1.8f, 1.6f, 1.6f), Mat("Canvas", 0.9f, 0.87f, 0.78f))
                .transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
            Material wheel = Mat("Trim", 0.3f, 0.22f, 0.15f);
            foreach (var p in new[] { new Vector3(-1f, 0.55f, 1.2f), new Vector3(1f, 0.55f, 1.2f), new Vector3(-1f, 0.55f, -1.2f), new Vector3(1f, 0.55f, -1.2f) })
            {
                Prim(PrimitiveType.Cylinder, "Wheel", wagon, p, new Vector3(1.1f, 0.05f, 1.1f), wheel)
                    .transform.localRotation = Quaternion.Euler(0f, 0f, 90f);
            }
        }

        private static void BuildBoundaries(Transform parent)
        {
            Transform walls = new GameObject("InvisibleBoundaries").transform;
            walls.SetParent(parent, false);
            const float half = 30f;
            Wall(walls, new Vector3(0f, 2f, half), new Vector3(half * 2f, 4f, 1f));
            Wall(walls, new Vector3(0f, 2f, -half), new Vector3(half * 2f, 4f, 1f));
            Wall(walls, new Vector3(half, 2f, 0f), new Vector3(1f, 4f, half * 2f));
            Wall(walls, new Vector3(-half, 2f, 0f), new Vector3(1f, 4f, half * 2f));
        }

        private static void Wall(Transform parent, Vector3 pos, Vector3 size)
        {
            var go = new GameObject("Wall");
            go.transform.SetParent(parent, false);
            go.transform.localPosition = pos;
            go.AddComponent<BoxCollider>().size = size;
            go.isStatic = true;
        }

        // ---------------------------------------------------------------- Characters

        private static GameObject CreatePlayer()
        {
            var player = new GameObject("Player");
            var cc = player.AddComponent<CharacterController>();
            cc.height = 1.8f;
            cc.radius = 0.35f;
            cc.center = new Vector3(0f, 0.9f, 0f);
            cc.stepOffset = 0.4f;

            var camGo = new GameObject("PlayerCamera");
            camGo.tag = "MainCamera";
            camGo.transform.SetParent(player.transform, false);
            camGo.transform.localPosition = new Vector3(0f, 1.6f, 0f);
            var cam = camGo.AddComponent<Camera>();
            cam.nearClipPlane = 0.05f;
            cam.fieldOfView = 70f;
            camGo.AddComponent<AudioListener>();

            player.AddComponent<PlayerController>().cameraPivot = camGo.transform;
            return player;
        }

        private static GameObject CreateMerchant()
        {
            var merchant = new GameObject("Merchant_NPC_Placeholder");

            Prim(PrimitiveType.Capsule, "Body", merchant.transform, new Vector3(0f, 0.9f, 0f), new Vector3(0.7f, 0.9f, 0.7f), Mat("MerchantShirt", 0.36f, 0.42f, 0.55f));
            Prim(PrimitiveType.Sphere, "Head", merchant.transform, new Vector3(0f, 1.95f, 0f), new Vector3(0.38f, 0.38f, 0.38f), Mat("Skin", 0.86f, 0.68f, 0.54f));
            // A small "nose" so you can tell which way the merchant faces.
            Prim(PrimitiveType.Cube, "FacingMarker", merchant.transform, new Vector3(0f, 1.95f, 0.2f), new Vector3(0.06f, 0.06f, 0.08f), Mat("Skin", 0.86f, 0.68f, 0.54f));
            Material hat = Mat("Hat", 0.22f, 0.16f, 0.11f);
            Prim(PrimitiveType.Cylinder, "HatBrim", merchant.transform, new Vector3(0f, 2.12f, 0f), new Vector3(0.6f, 0.015f, 0.6f), hat);
            Prim(PrimitiveType.Cylinder, "HatCrown", merchant.transform, new Vector3(0f, 2.25f, 0f), new Vector3(0.34f, 0.12f, 0.34f), hat);
            // Apron so the merchant reads as a shopkeeper.
            Prim(PrimitiveType.Cube, "Apron", merchant.transform, new Vector3(0f, 0.85f, 0.25f), new Vector3(0.5f, 0.8f, 0.04f), Mat("Canvas", 0.9f, 0.87f, 0.78f));

            // Remove colliders from decoration; one capsule collider on the root blocks the player.
            foreach (var c in merchant.GetComponentsInChildren<Collider>()) Object.DestroyImmediate(c);
            var col = merchant.AddComponent<CapsuleCollider>();
            col.center = new Vector3(0f, 1.1f, 0f);
            col.height = 2.2f;
            col.radius = 0.35f;

            // Floating name label that always faces the street side it was built on.
            Label(merchant.transform, "MERCHANT", new Vector3(0f, 2.65f, 0f), Quaternion.Euler(0f, 180f, 0f), 0.06f);

            var npc = merchant.AddComponent<MerchantNPC>();
            npc.dialogueFile = AssetDatabase.LoadAssetAtPath<TextAsset>(DialoguePath);
            if (npc.dialogueFile == null) Debug.LogWarning($"[AI World Studio] Dialogue file missing at {DialoguePath}; using fallback lines.");
            return merchant;
        }

        // ---------------------------------------------------------------- Helpers

        /// <summary>Instantiates the prefab if it exists; otherwise builds it, saves it as a prefab, and returns the instance.</summary>
        private static GameObject GetOrCreatePrefab(string path, System.Func<GameObject> factory)
        {
            var existing = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (existing != null) return (GameObject)PrefabUtility.InstantiatePrefab(existing);

            GameObject instance = factory();
            PrefabUtility.SaveAsPrefabAssetAndConnect(instance, path, InteractionMode.AutomatedAction);
            return instance;
        }

        private static GameObject Prim(PrimitiveType type, string name, Transform parent, Vector3 localPos, Vector3 localScale, Material mat)
        {
            var go = GameObject.CreatePrimitive(type);
            go.name = name;
            go.transform.SetParent(parent, false);
            go.transform.localPosition = localPos;
            go.transform.localScale = localScale;
            if (mat != null) go.GetComponent<Renderer>().sharedMaterial = mat;
            return go;
        }

        private static void Label(Transform parent, string text, Vector3 localPos, Quaternion localRot, float charSize)
        {
            var go = new GameObject("Label_" + text);
            go.transform.SetParent(parent, false);
            go.transform.localPosition = localPos;
            go.transform.localRotation = localRot;
            var tm = go.AddComponent<TextMesh>();
            tm.text = text;
            tm.anchor = TextAnchor.MiddleCenter;
            tm.alignment = TextAlignment.Center;
            tm.fontSize = 64;
            tm.characterSize = charSize;
            tm.color = new Color(0.97f, 0.9f, 0.72f);
            Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            if (font != null)
            {
                tm.font = font;
                go.GetComponent<MeshRenderer>().sharedMaterial = font.material;
            }
        }

        /// <summary>Loads a graybox material by name, creating it once if needed (works with Built-in or URP).</summary>
        private static Material Mat(string name, float r, float g, float b)
        {
            string path = $"{MaterialDir}/{name}.mat";
            var mat = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (mat != null) return mat;

            Shader shader = GraphicsSettings.currentRenderPipeline != null
                ? Shader.Find("Universal Render Pipeline/Lit")
                : Shader.Find("Standard");
            if (shader == null) shader = Shader.Find("Standard") ?? Shader.Find("Diffuse");

            mat = new Material(shader) { color = new Color(r, g, b) };
            if (mat.HasProperty("_Glossiness")) mat.SetFloat("_Glossiness", 0.1f);
            if (mat.HasProperty("_Smoothness")) mat.SetFloat("_Smoothness", 0.1f);
            AssetDatabase.CreateAsset(mat, path);
            return mat;
        }

        private static void EnsureFolder(string path)
        {
            if (AssetDatabase.IsValidFolder(path)) return;
            string parent = Path.GetDirectoryName(path)?.Replace('\\', '/');
            if (!string.IsNullOrEmpty(parent)) EnsureFolder(parent);
            AssetDatabase.CreateFolder(parent, Path.GetFileName(path));
        }

        private static void AddSceneToBuildSettings(string path)
        {
            var scenes = EditorBuildSettings.scenes.ToList();
            if (scenes.Any(s => s.path == path)) return;
            scenes.Insert(0, new EditorBuildSettingsScene(path, true));
            EditorBuildSettings.scenes = scenes.ToArray();
        }
    }
}
