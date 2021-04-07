using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.IO.Compression;

public class TilingStuff
{
    [MenuItem("mushroom/tiling/re-tile level")]
    private static void TileLevel()
    {
        var tex = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/objects/tile/bricks.png");

        var mBase = new Material[6, 6];
        var mTerminal = new Material[6, 6];

        for (var x = 0; x < 6; ++x)
        {
            for (var y = 0; y < 6; ++y)
            {
                var mat1 = new Material(Shader.Find("Standard"));
                mat1.mainTexture = tex;
                mat1.color = Color.white;
                mat1.mainTextureScale = new Vector2(1 / 6f, 1 / 12f);
                mat1.mainTextureOffset = new Vector2(x / 6f, 11 / 12f - y / 12f);

                mat1.SetOverrideTag("RenderType", "TransparentCutout");
                mat1.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                mat1.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                mat1.SetInt("_ZWrite", 1);
                mat1.EnableKeyword("_ALPHATEST_ON");
                mat1.DisableKeyword("_ALPHABLEND_ON");
                mat1.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                mat1.renderQueue = (int)UnityEngine.Rendering.RenderQueue.AlphaTest;

                mBase[x, y] = mat1;

                var mat2 = new Material(Shader.Find("Standard"));
                mat2.mainTexture = tex;
                mat2.color = Color.white;
                mat2.mainTextureScale = new Vector2(1 / 6f, 1 / 12f);
                mat2.mainTextureOffset = new Vector2(x / 6f, 5 / 12f - y / 12f);

                mat2.SetOverrideTag("RenderType", "TransparentCutout");
                mat2.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                mat2.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                mat2.SetInt("_ZWrite", 1);
                mat2.EnableKeyword("_ALPHATEST_ON");
                mat2.DisableKeyword("_ALPHABLEND_ON");
                mat2.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                mat2.renderQueue = (int)UnityEngine.Rendering.RenderQueue.AlphaTest;

                mTerminal[x, y] = mat2;
                // AssetDatabase.CreateAsset(mat1, $"Assets/objects/tile/materials/{x:00}.{y:00}.mat");
                // AssetDatabase.CreateAsset(mat2, $"Assets/objects/tile/materials/{x:00}.{y:00}.t.mat");
                // mBase[x, y] = AssetDatabase.LoadAssetAtPath<Material>($"Assets/objects/tile/materials/{x:00}.{y:00}.mat");
                // mTerminal[x, y] = AssetDatabase.LoadAssetAtPath<Material>($"Assets/objects/tile/materials/{x:00}.{y:00}.t.mat");
            }
        }

        var allLevels = Component.FindObjectsOfType<Level>();

        var panels = new List<Rect>();
        var grid = new HashSet<Vector2>();

        foreach (var level in allLevels)
        {
            foreach (var o in FindPlatforms(level.gameObject))
            {
                grid.Add(o.transform.position);
            }
        }

        foreach (var panel in Component.FindObjectsOfType<FlipPanel>())
        {
            var rect = Util.RectFromCenterSize(panel.transform.position, panel.transform.lossyScale);
            panels.Add(rect);
        }

        foreach (var level in allLevels)
        {
            foreach (var child in FindPlatforms(level.gameObject))
            {
                var pos = (Vector2)child.transform.position;

                Predicate<Vector2> test = (Vector2 o) => grid.Contains(pos + o)
                    && !panels.Any(p => p.Contains(pos) != p.Contains(pos + o));

                var up = test(Vector2.up);
                var down = test(Vector2.down);
                var left = test(Vector2.left);
                var right = test(Vector2.right);

                var upleft = test(Vector2.up + Vector2.left);
                var upright = test(Vector2.up + Vector2.right);
                var downleft = test(Vector2.down + Vector2.left);
                var downright = test(Vector2.down + Vector2.right);

                var quarters = child.GetChildren()
                    .Select(c => c.GetComponent<MeshRenderer>())
                    .ToList();

                var topLeft = quarters[0];
                var topRight = quarters[1];
                var botLeft = quarters[2];
                var botRight = quarters[3];

                Undo.RecordObject(topLeft, "fix");
                Undo.RecordObject(topRight, "fix");
                Undo.RecordObject(botLeft, "fix");
                Undo.RecordObject(botRight, "fix");

                if (!up && !left) topLeft.material = mBase[0, 0];
                else if (!down && !left && !upleft) topLeft.material = mBase[0, 4];
                else if (!down && !left) topLeft.material = mTerminal[0, 4];
                else if (!left && !upleft) topLeft.material = mBase[0, 2];
                else if (!left) topLeft.material = mTerminal[0, 2];
                else if (!up && !right) topLeft.material = mBase[4, 0];
                else if (!up) topLeft.material = mBase[2, 0];
                else if (!upleft) topLeft.material = mBase[2, 2];
                else topLeft.material = mBase[1, 1];

                if (!up && !right) topRight.material = mBase[5, 0];
                else if (!down && !right && !upright) topRight.material = mBase[5, 4];
                else if (!down && !right) topRight.material = mTerminal[5, 4];
                else if (!right && !upright) topRight.material = mBase[5, 2];
                else if (!right) topRight.material = mTerminal[5, 2];
                else if (!up && !left) topRight.material = mBase[1, 0];
                else if (!up) topRight.material = mBase[3, 0];
                else if (!upright) topRight.material = mBase[3, 2];
                else topRight.material = mBase[1, 1];

                if (!down && !right) botRight.material = mBase[5, 5];
                else if (!up && !right && !downright) botRight.material = mBase[5, 1];
                else if (!up && !right) botRight.material = mTerminal[5, 1];
                else if (!right && !downright) botRight.material = mBase[5, 3];
                else if (!right) botRight.material = mTerminal[5, 3];
                else if (!down && !left) botRight.material = mBase[1, 5];
                else if (!down) botRight.material = mBase[3, 5];
                else if (!downright) botRight.material = mBase[3, 3];
                else botRight.material = mBase[1, 1];

                if (!down && !left) botLeft.material = mBase[0, 5];
                else if (!up && !left && !downleft) botLeft.material = mBase[0, 1];
                else if (!up && !left) botLeft.material = mTerminal[0, 1];
                else if (!left && !downleft) botLeft.material = mBase[0, 3];
                else if (!left) botLeft.material = mTerminal[0, 3];
                else if (!down && !right) botLeft.material = mBase[4, 5];
                else if (!down) botLeft.material = mBase[2, 5];
                else if (!downleft) botLeft.material = mBase[2, 3];
                else botLeft.material = mBase[1, 1];
            }
        }
    }

    private static IEnumerable<GameObject> FindPlatforms(GameObject o)
    {
        var root = PrefabUtility.GetNearestPrefabInstanceRoot(o);
        var path = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(o);

        if (root == o && path == "Assets/levels/prefabs/platform.prefab")
        {
            yield return o;
        }
        else
        {
            foreach (var child in o.GetChildren())
            {
                foreach (var inner in FindPlatforms(child))
                {
                    yield return inner;
                }
            }
        }
    }


    // [MenuItem("mushroom/tiling/level image")]
    // private static void LevelImage()
    // {
    //     var levels = Component.FindObjectsOfType<Level>();

    //     var min = Physics.FromUnity(new Vector2(
    //         levels.Min(l => l.transform.position.x) - 6,
    //         levels.Min(l => l.transform.position.y) - 6
    //     ));

    //     var max = Physics.FromUnity(new Vector2(
    //         levels.Max(l => l.transform.position.x) + 6,
    //         levels.Max(l => l.transform.position.y) + 6
    //     ));

    //     var viewport = Rect.MinMaxRect(min.x, min.y, max.x, max.y);

    //     var obj = new GameObject("test camera");
    //     var camera = obj.AddComponent<Camera>();
    //     var oldTexture = RenderTexture.active;

    //     try
    //     {
    //         var texture = new RenderTexture((int)viewport.width, (int)viewport.height, 24);

    //         RenderTexture.active = texture;
    //         camera.targetTexture = texture;
    //         camera.orthographic = true;
    //         camera.orthographicSize = viewport.height / 64;
    //         camera.transform.position = (Vector3)Physics.ToUnity(viewport.center) + new Vector3(0, 0, -32);

    //         camera.Render();

    //         var image = new Texture2D(texture.width, texture.height);
    //         image.ReadPixels(new Rect(0, 0, texture.width, texture.height), 0, 0);
    //         image.Apply();

    //         File.WriteAllBytes("level.png", image.EncodeToPNG());
    //     }
    //     finally
    //     {
    //         RenderTexture.active = oldTexture;
    //         Component.DestroyImmediate(obj);
    //     }
    // }
}
