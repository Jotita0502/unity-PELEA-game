#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Linq;
using UnityEditorInternal;
using System.Text;
using UnityEngine.Rendering;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public static class ProjectSnapshotExporter
{
    [MenuItem("Tools/Export Project Snapshot")]
    public static void Export()
    {
        var dir = "Assets/_Project/Diagnostics";
        Directory.CreateDirectory(dir);
        var path = Path.Combine(dir, "ProjectSnapshot.md");

        var sb = new StringBuilder();
        sb.AppendLine("# Project Snapshot");
        sb.AppendLine($"- **Unity**: {Application.unityVersion}");
        sb.AppendLine($"- **Build Target**: {EditorUserBuildSettings.activeBuildTarget}");
        var rp = GraphicsSettings.currentRenderPipeline;
        sb.AppendLine($"- **Render Pipeline**: {(rp ? rp.name : "Built-in")}");

        // Time & Physics
        sb.AppendLine("\n## Time & Physics");
        sb.AppendLine($"- Time.fixedDeltaTime: {Time.fixedDeltaTime}");
        sb.AppendLine($"- Physics.defaultSolverIterations: {Physics.defaultSolverIterations}");
        sb.AppendLine($"- Physics.defaultSolverVelocityIterations: {Physics.defaultSolverVelocityIterations}");
        sb.AppendLine($"- Physics.defaultContactOffset: {Physics.defaultContactOffset}");
        sb.AppendLine($"- Physics.reuseCollisionCallbacks: {Physics.reuseCollisionCallbacks}");

        // Quality (estado actual)
        sb.AppendLine("\n## Quality");
        sb.AppendLine($"- Current Quality Level: {QualitySettings.names[QualitySettings.GetQualityLevel()]}");
        sb.AppendLine($"- VSync: {QualitySettings.vSyncCount}, AntiAliasing: {QualitySettings.antiAliasing}, Shadows: {QualitySettings.shadows}");

        // Layers & Tags
        sb.AppendLine("\n## Layers & Tags");
        sb.AppendLine("- Layers: " + string.Join(", ", InternalEditorUtility.layers));
        sb.AppendLine("- Tags: " + string.Join(", ", InternalEditorUtility.tags));

        // Collision Matrix (pares ignorados)
        sb.AppendLine("\n### Layer Collision Matrix (pairs ignored)");
        var layers = InternalEditorUtility.layers;
        for (int i = 0; i < layers.Length; i++)
        {
            for (int j = i; j < layers.Length; j++)
            {
                int li = LayerMask.NameToLayer(layers[i]);
                int lj = LayerMask.NameToLayer(layers[j]);
                if (li >= 0 && lj >= 0 && Physics.GetIgnoreLayerCollision(li, lj))
                    sb.AppendLine($"- {layers[i]} ✕ {layers[j]}");
            }
        }

        // Build Settings scenes
        sb.AppendLine("\n## Build Settings Scenes");
        foreach (var s in EditorBuildSettings.scenes)
            sb.AppendLine($"- {(s.enabled ? "[x]" : "[ ]")} {s.path}");

        // Input System assets
#if ENABLE_INPUT_SYSTEM
        sb.AppendLine("\n## Input System");
        var guids = AssetDatabase.FindAssets("t:InputActionAsset");
        if (guids.Length == 0) sb.AppendLine("- No se encontraron .inputactions");
        foreach (var g in guids)
        {
            var p = AssetDatabase.GUIDToAssetPath(g);
            sb.AppendLine($"- {Path.GetFileName(p)} (ruta: {p})");
        }
#else
        sb.AppendLine("\n## Input System");
        sb.AppendLine("- (Paquete nuevo no habilitado o se usa el antiguo)");
#endif

        // Prefabs candidatos a jugador (PlayerInput / Animator Humanoid)
        sb.AppendLine("\n## Candidate Player Prefabs");
        var prefabGuids = AssetDatabase.FindAssets("t:Prefab");
        foreach (var g in prefabGuids.Take(500)) // límite por seguridad
        {
            var p = AssetDatabase.GUIDToAssetPath(g);
            var go = AssetDatabase.LoadAssetAtPath<GameObject>(p);
            if (!go) continue;
#if ENABLE_INPUT_SYSTEM
            bool hasPI = go.GetComponentInChildren<PlayerInput>(true) != null;
#else
            bool hasPI = false;
#endif
            bool hasAnimator = go.GetComponentInChildren<Animator>(true) != null && go.GetComponentInChildren<Animator>(true).isHuman;
            if (hasPI || hasAnimator)
                sb.AppendLine($"- {p} {(hasPI ? "[PlayerInput]" : "")} {(hasAnimator ? "[Animator Humanoid]" : "")}");
        }

        // Render Pipeline Asset
        sb.AppendLine("\n## Render Pipeline Asset");
        var rpAsset = GraphicsSettings.currentRenderPipeline;
        if (rpAsset) sb.AppendLine($"- GraphicsSettings RP Asset: {rpAsset.name}");

        // Guardar
        File.WriteAllText(path, sb.ToString(), Encoding.UTF8);
        AssetDatabase.Refresh();
        Debug.Log("Snapshot exportado: " + path);
    }
}
#endif
