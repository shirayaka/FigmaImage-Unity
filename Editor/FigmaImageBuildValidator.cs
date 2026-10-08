#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace ProjectArea.UI.Editor
{
    /// <summary>
    /// Validates that FigmaImage shader and default resource material are included
    /// before any player build (Android, iOS, Standalone) to prevent runtime shader stripping.
    /// </summary>
    public class FigmaImageBuildValidator : IPreprocessBuildWithReport
    {
        public int callbackOrder => 0;

        public void OnPreprocessBuild(BuildReport report)
        {
            var mat = Resources.Load<Material>("FigmaImage-Default");
            if (mat == null || mat.shader == null)
            {
                // Attempt to locate shader and create the resource material if missing
                var shader = Shader.Find("UI/FigmaImage");
                if (shader != null)
                {
                    string resDir = "Packages/com.shirayaka.figma-image/Runtime/Resources";
                    if (!AssetDatabase.IsValidFolder(resDir))
                    {
                        resDir = "Assets/Scripts/UI/FigmaImage/Resources";
                    }
                    if (!AssetDatabase.IsValidFolder(resDir) && AssetDatabase.IsValidFolder("Packages/com.shirayaka.figma-image/Runtime"))
                    {
                        AssetDatabase.CreateFolder("Packages/com.shirayaka.figma-image/Runtime", "Resources");
                        resDir = "Packages/com.shirayaka.figma-image/Runtime/Resources";
                    }
                    else if (!AssetDatabase.IsValidFolder(resDir) && AssetDatabase.IsValidFolder("Assets/Scripts/UI/FigmaImage"))
                    {
                        AssetDatabase.CreateFolder("Assets/Scripts/UI/FigmaImage", "Resources");
                        resDir = "Assets/Scripts/UI/FigmaImage/Resources";
                    }

                    string matPath = $"{resDir}/FigmaImage-Default.mat";
                    var existing = AssetDatabase.LoadAssetAtPath<Material>(matPath);
                    if (existing == null)
                    {
                        var newMat = new Material(shader) { name = "FigmaImage-Default" };
                        AssetDatabase.CreateAsset(newMat, matPath);
                        AssetDatabase.SaveAssets();
                        Debug.Log("[FigmaImage] Created missing 'FigmaImage-Default.mat' in Resources to guarantee player build shader inclusion.");
                    }
                }
                else
                {
                    Debug.LogWarning("[FigmaImage] Could not find shader 'UI/FigmaImage' during build pre-processing. UI may fall back to default rectangular Image.");
                }
            }
            else
            {
                Debug.Log($"[FigmaImage] Build validation passed for {report.summary.platform}: 'FigmaImage-Default.mat' ({mat.shader.name}) is present in Resources.");
            }
        }
    }
}
#endif
