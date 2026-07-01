using UnityEngine;

namespace RetroFootball76.Visual
{
    /// <summary>
    /// Procedural textures and mesh helpers for detailed 3D pitch assets.
    /// </summary>
    public static class ProceduralAssets
    {
        public static Material CreateGrassMaterial(Color light, Color dark, int stripes = 14)
        {
            var tex = new Texture2D(512, 512, TextureFormat.RGBA32, false);
            tex.filterMode = FilterMode.Bilinear;
            tex.wrapMode = TextureWrapMode.Repeat;
            var stripeW = tex.width / stripes;
            for (var y = 0; y < tex.height; y++)
            for (var x = 0; x < tex.width; x++)
            {
                var stripe = x / stripeW;
                var baseC = stripe % 2 == 0 ? light : dark;
                var noise = (Mathf.PerlinNoise(x * 0.04f, y * 0.04f) - 0.5f) * 0.08f;
                tex.SetPixel(x, y, new Color(
                    Mathf.Clamp01(baseC.r + noise),
                    Mathf.Clamp01(baseC.g + noise),
                    Mathf.Clamp01(baseC.b + noise * 0.5f)));
            }
            tex.Apply();

            var mat = LitMat(light);
            mat.mainTexture = tex;
            if (mat.HasProperty("_BaseMap")) mat.SetTexture("_BaseMap", tex);
            return mat;
        }

        public static Material CreateBallMaterial()
        {
            var tex = new Texture2D(256, 256, TextureFormat.RGBA32, false);
            tex.filterMode = FilterMode.Bilinear;
            var white = Color.white;
            var black = new Color(0.12f, 0.12f, 0.12f);
            for (var y = 0; y < tex.height; y++)
            for (var x = 0; x < tex.width; x++)
            {
                var u = x / (float)tex.width;
                var v = y / (float)tex.height;
                var pent = Mathf.Abs(Mathf.Sin(u * 18f) * Mathf.Cos(v * 14f) + Mathf.Sin((u + v) * 22f));
                tex.SetPixel(x, y, pent > 0.55f ? black : white);
            }
            tex.Apply();

            var mat = LitMat(white);
            mat.mainTexture = tex;
            if (mat.HasProperty("_BaseMap")) mat.SetTexture("_BaseMap", tex);
            if (mat.HasProperty("_Smoothness")) mat.SetFloat("_Smoothness", 0.65f);
            return mat;
        }

        public static Transform CreatePlayerFigure(Transform parent, string name, Color kit, int number)
        {
            var root = new GameObject(name).transform;
            root.SetParent(parent, false);

            var skin = new Color(0.92f, 0.76f, 0.62f);
            var shorts = Color.Lerp(kit, Color.black, 0.35f);
            var boots = new Color(0.15f, 0.15f, 0.18f);

            // Boots / legs
            CreatePart(root, "LegL", PrimitiveType.Capsule, new Vector3(-0.14f, 0.22f, 0), new Vector3(0.14f, 0.28f, 0.14f), boots);
            CreatePart(root, "LegR", PrimitiveType.Capsule, new Vector3(0.14f, 0.22f, 0), new Vector3(0.14f, 0.28f, 0.14f), boots);

            // Shorts
            CreatePart(root, "Shorts", PrimitiveType.Capsule, new Vector3(0, 0.48f, 0), new Vector3(0.38f, 0.22f, 0.32f), shorts);

            // Torso / shirt
            CreatePart(root, "Torso", PrimitiveType.Capsule, new Vector3(0, 0.78f, 0), new Vector3(0.42f, 0.42f, 0.3f), kit);

            // Arms
            CreatePart(root, "ArmL", PrimitiveType.Capsule, new Vector3(-0.32f, 0.72f, 0), new Vector3(0.1f, 0.32f, 0.1f), kit);
            CreatePart(root, "ArmR", PrimitiveType.Capsule, new Vector3(0.32f, 0.72f, 0), new Vector3(0.1f, 0.32f, 0.1f), kit);

            // Head
            CreatePart(root, "Head", PrimitiveType.Sphere, new Vector3(0, 1.12f, 0), new Vector3(0.28f, 0.28f, 0.28f), skin);

            // Number bib
            var label = new GameObject("Number");
            label.transform.SetParent(root, false);
            label.transform.localPosition = new Vector3(0, 0.82f, 0.18f);
            label.transform.localRotation = Quaternion.Euler(0, 180, 0);
            var tm = label.AddComponent<TextMesh>();
            tm.text = number.ToString();
            tm.fontSize = 72;
            tm.characterSize = 0.045f;
            tm.anchor = TextAnchor.MiddleCenter;
            tm.color = Color.white;
            tm.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

            return root;
        }

        public static void CreateStadium(Transform root, float fieldW, float fieldL)
        {
            var standColor = new Color(0.14f, 0.18f, 0.24f);
            var seatColor = new Color(0.22f, 0.28f, 0.36f);
            var margin = 3f;

            BuildStand(root, "StandNorth", new Vector3(0, 1.2f, fieldL / 2f + margin),
                new Vector3(fieldW + 8f, 2.4f, 4f), standColor, seatColor, 3);
            BuildStand(root, "StandSouth", new Vector3(0, 1.2f, -fieldL / 2f - margin),
                new Vector3(fieldW + 8f, 2.4f, 4f), standColor, seatColor, 3);
            BuildStand(root, "StandWest", new Vector3(-fieldW / 2f - margin, 0.9f, 0),
                new Vector3(4f, 1.8f, fieldL + 6f), standColor, seatColor, 2);
            BuildStand(root, "StandEast", new Vector3(fieldW / 2f + margin, 0.9f, 0),
                new Vector3(4f, 1.8f, fieldL + 6f), standColor, seatColor, 2);

            for (var i = 0; i < 4; i++)
            {
                var angle = i * 90f + 45f;
                var rad = angle * Mathf.Deg2Rad;
                var pos = new Vector3(Mathf.Cos(rad) * (fieldW / 2f + 5f), 0, Mathf.Sin(rad) * (fieldL / 2f + 5f));
                BuildFloodlight(root, $"Floodlight_{i}", pos);
            }
        }

        static void BuildStand(Transform parent, string name, Vector3 pos, Vector3 size, Color wall, Color seat, int tiers)
        {
            var stand = new GameObject(name).transform;
            stand.SetParent(parent, false);
            stand.localPosition = pos;

            CreatePart(stand, "Wall", PrimitiveType.Cube, Vector3.zero, size, wall);

            for (var t = 0; t < tiers; t++)
            {
                var h = size.y * (0.25f + t * 0.22f);
                var tier = new Vector3(size.x * 0.92f, 0.18f, size.z * (0.5f + t * 0.12f));
                var y = -size.y * 0.35f + t * 0.35f;
                CreatePart(stand, $"Tier{t}", PrimitiveType.Cube, new Vector3(0, y, 0), tier, seat);
            }
        }

        static void BuildFloodlight(Transform parent, string name, Vector3 pos)
        {
            var pole = new GameObject(name).transform;
            pole.SetParent(parent, false);
            pole.localPosition = pos;
            CreatePart(pole, "Pole", PrimitiveType.Cylinder, new Vector3(0, 3.5f, 0), new Vector3(0.12f, 3.5f, 0.12f), new Color(0.3f, 0.32f, 0.35f));
            CreatePart(pole, "Head", PrimitiveType.Cube, new Vector3(0, 7f, 0), new Vector3(0.8f, 0.25f, 0.4f), new Color(0.9f, 0.9f, 0.85f));

            var lightGo = new GameObject("Light");
            lightGo.transform.SetParent(pole, false);
            lightGo.transform.localPosition = new Vector3(0, 6.8f, 0);
            var light = lightGo.AddComponent<Light>();
            light.type = LightType.Spot;
            light.range = 28f;
            light.intensity = 0.35f;
            light.spotAngle = 65f;
            light.color = new Color(1f, 0.95f, 0.85f);
        }

        public static void CreateDetailedGoal(Transform parent, Vector3 pos, float depthSign)
        {
            var goal = new GameObject("Goal").transform;
            goal.SetParent(parent, false);
            goal.localPosition = pos;

            var postMat = LitMat(Color.white);
            if (postMat.HasProperty("_Smoothness")) postMat.SetFloat("_Smoothness", 0.7f);

            CreatePart(goal, "PostL", PrimitiveType.Cylinder, new Vector3(-1.35f, 0.7f, 0), new Vector3(0.1f, 0.7f, 0.1f), Color.white);
            CreatePart(goal, "PostR", PrimitiveType.Cylinder, new Vector3(1.35f, 0.7f, 0), new Vector3(0.1f, 0.7f, 0.1f), Color.white);
            CreatePart(goal, "Crossbar", PrimitiveType.Cylinder, new Vector3(0, 1.35f, 0), new Vector3(0.1f, 0.1f, 2.8f), Color.white)
                .transform.localRotation = Quaternion.Euler(0, 0, 90);

            // Net grid
            var netRoot = new GameObject("Net").transform;
            netRoot.SetParent(goal, false);
            netRoot.localPosition = new Vector3(0, 0.7f, depthSign * 0.55f);
            var netMat = LitMat(new Color(1, 1, 1, 0.35f));
            netMat.SetFloat("_Surface", 1); // transparent if URP

            for (var i = 0; i <= 8; i++)
            {
                var x = -1.2f + i * 0.3f;
                CreatePart(netRoot, $"NetV{i}", PrimitiveType.Cube, new Vector3(x, 0, 0),
                    new Vector3(0.02f, 1.2f, depthSign * 1.1f), new Color(1, 1, 1, 0.25f));
            }
            for (var j = 0; j <= 4; j++)
            {
                var y = j * 0.3f;
                CreatePart(netRoot, $"NetH{j}", PrimitiveType.Cube, new Vector3(0, y, depthSign * 0.55f),
                    new Vector3(2.5f, 0.02f, 0.02f), new Color(1, 1, 1, 0.25f));
            }
        }

        public static void CreateSky(Transform root)
        {
            var sky = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sky.name = "Sky";
            sky.transform.SetParent(root, false);
            sky.transform.localScale = Vector3.one * -120f;
            sky.GetComponent<Renderer>().material = LitMat(new Color(0.12f, 0.18f, 0.32f));
            Object.Destroy(sky.GetComponent<Collider>());
        }

        public static void SpawnConfetti(Transform root, Vector3 worldPos)
        {
            var burst = new GameObject("Confetti").transform;
            burst.SetParent(root, false);
            burst.localPosition = worldPos;

            var colors = new[]
            {
                new Color(1f, 0.85f, 0.1f),
                new Color(0.95f, 0.2f, 0.2f),
                new Color(0.2f, 0.75f, 1f),
                Color.white
            };

            for (var i = 0; i < 36; i++)
            {
                var bit = GameObject.CreatePrimitive(PrimitiveType.Cube);
                bit.transform.SetParent(burst, false);
                bit.transform.localPosition = Random.insideUnitSphere * 0.4f;
                bit.transform.localScale = Vector3.one * Random.Range(0.06f, 0.14f);
                bit.transform.rotation = Random.rotation;
                bit.GetComponent<Renderer>().material = LitMat(colors[i % colors.Length]);
                Object.Destroy(bit.GetComponent<Collider>());

                var rb = bit.AddComponent<Rigidbody>();
                rb.useGravity = true;
                rb.mass = 0.02f;
                rb.AddForce(Random.insideUnitSphere * 3.5f + Vector3.up * 4f, ForceMode.Impulse);
                rb.AddTorque(Random.insideUnitSphere * 2f, ForceMode.Impulse);
                Object.Destroy(bit, 2.5f);
            }

            Object.Destroy(burst.gameObject, 3f);
        }

        public static void CreateCornerFlag(Transform parent, Vector3 pos)
        {
            var flag = new GameObject("CornerFlag").transform;
            flag.SetParent(parent, false);
            flag.localPosition = pos;
            CreatePart(flag, "Pole", PrimitiveType.Cylinder, new Vector3(0, 0.6f, 0), new Vector3(0.03f, 0.6f, 0.03f), Color.white);
            CreatePart(flag, "Flag", PrimitiveType.Cube, new Vector3(0.25f, 1.1f, 0), new Vector3(0.4f, 0.25f, 0.02f), new Color(0.95f, 0.2f, 0.15f));
        }

        static GameObject CreatePart(Transform parent, string name, PrimitiveType type, Vector3 pos, Vector3 scale, Color color)
        {
            var go = GameObject.CreatePrimitive(type);
            go.name = name;
            go.transform.SetParent(parent, false);
            go.transform.localPosition = pos;
            go.transform.localScale = scale;
            go.GetComponent<Renderer>().material = LitMat(color);
            Object.Destroy(go.GetComponent<Collider>());
            return go;
        }

        public static Material LitMat(Color color)
        {
            var shader = Shader.Find("Universal Render Pipeline/Lit")
                ?? Shader.Find("Standard")
                ?? Shader.Find("Diffuse");
            var m = new Material(shader);
            if (m.HasProperty("_BaseColor")) m.SetColor("_BaseColor", color);
            else m.color = color;
            return m;
        }
    }
}
