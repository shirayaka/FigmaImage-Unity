using UnityEditor;
using UnityEngine;
using ProjectArea.UI;

namespace ProjectArea.UI.Editor
{
    [CustomEditor(typeof(RoundedImage), true)]
    [CanEditMultipleObjects]
    public class RoundedImageEditor : FigmaImageEditor
    {
        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox("RoundedImage is deprecated. Consider upgrading to FigmaImage for full stroke and corner radius features.", MessageType.Info);

            if (GUILayout.Button("Upgrade to FigmaImage"))
            {
                foreach (var t in targets)
                {
                    if (t is RoundedImage ri)
                    {
                        var menuCommand = new MenuCommand(ri);
                        FigmaImageEditor.UpgradeRoundedImageToFigmaImage(menuCommand);
                    }
                }
                return;
            }

            base.OnInspectorGUI();
        }
    }
}
