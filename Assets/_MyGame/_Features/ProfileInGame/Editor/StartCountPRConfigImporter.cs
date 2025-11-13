using MyGame.Modules.ProfileInGame.Config;
using UnityEditor;
using UnityEngine;

namespace MyGame.Modules.ProfileInGame.Editor
{
    public class StartCountPRConfigImporter : CSVImporter
    {
        private string savePath = "Assets/_MyGame/_Features/ProfileInGame/ScriptableObjects/Configs/StartCount";

        [MenuItem("Tools/ProfileInGame/StartCountPRConfigImporter")]
        public static void Open()
        {
            GetWindow<StartCountPRConfigImporter>("ProfileInGame/StartCountPRConfigImporter");
        }


        protected override void OnGUI()
        {
            GUILayout.Label("Start Count PR Config Importer", EditorStyles.boldLabel);
            savePath = EditorGUILayout.TextField("Save Path", savePath);
            base.OnGUI();
        }

        protected override void ImportCSV()
        {
            // var lines = GetLines();
            // var rows = lines.Length;
            // var columns = lines[0].Split(',').Length;

            // for (int i = 0; i < rows; i++)
            // {
            //     var name = $"LogicOrderConfigSO_v2_{prefixNames[i]}.asset";
            //     string assetPath = $"{savePath}";

            //     var flowConfigSO_v2 = ScriptableObject.CreateInstance<FlowConfigSO_v2>();
            //     flowConfigSO_v2.listCurveIndices = new List<int>();
            //     for (int j = 0; j < columns; j++)
            //     {
            //         flowConfigSO_v2.listCurveIndices.Add(int.Parse(GetValues(lines[i])[j]));
            //     }
            //     AssetDatabase.CreateAsset(flowConfigSO_v2, $"{assetPath}/{name}");
            // }

            // AssetDatabase.SaveAssets();
            // AssetDatabase.Refresh();

            // Debug.Log($"✅ Created {columns} basic order configs in {savePath}");

        }
    }
}
