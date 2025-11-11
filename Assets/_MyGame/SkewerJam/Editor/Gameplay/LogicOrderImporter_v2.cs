using System;
using System.Collections;
using System.Collections.Generic;
using MyGame.SkewerJam.Gameplay.LogicOrder.Configs;
using UnityEditor;
using UnityEngine;

public class LogicOrderImporter_v2 : CSVImporter
{
    private string savePath = "Assets/_MyGame/SkewerJam/ScriptableObjects/Gameplay_SkewerJam/Gameplay/LogicOrder_v2";

    [MenuItem("Tools/SkewerSort/LogicOrderImporter_v2")]
    public static void Open()
    {
        GetWindow<LogicOrderImporter_v2>("Gameplay/LogicOrderImporter_v2");
    }


    protected override void OnGUI()
    {
        GUILayout.Label("Logic Order Importer", EditorStyles.boldLabel);
        savePath = EditorGUILayout.TextField("Save Path", savePath);
        base.OnGUI();
    }

    protected override void ImportCSV()
    {
        var lines = GetLines();
        var rows = lines.Length;
        var columns = lines[0].Split(',').Length;

        string[] prefixNames = new string[] {
            "SE_0",
            "SE_1",
            "SE_2",
            "E1_0",
            "E1_1",
            "E1_2",
            "E1_3",
            "E2_0",
            "E2_1",
            "E2_2",
            "E2_3",
            "M1_0",
            "M1_1",
            "M1_2",
            "M1_3",
            "M2_0",
            "M2_1",
            "M2_2",
            "M2_3",
            "H1_0",
            "H1_1",
            "H1_2",
            "H1_3",
            "H2_0",
            "H2_1",
            "H2_2",
            "H2_3",
            "SH1_0",
            "SH1_1",
            "SH1_2",
            "SH1_3",
            "SH2_0",
            "SH2_1",
            "SH2_2",
            "SH2_3"
            };
        for (int i = 0; i < rows; i++)
        {
            var name = $"LogicOrderConfigSO_v2_{prefixNames[i]}.asset";
            string assetPath = $"{savePath}";

            var flowConfigSO_v2 = ScriptableObject.CreateInstance<FlowConfigSO_v2>();
            flowConfigSO_v2.listCurveIndices = new List<int>();
            for (int j = 0; j < columns; j++)
            {
                flowConfigSO_v2.listCurveIndices.Add(int.Parse(GetValues(lines[i])[j]));
            }
            AssetDatabase.CreateAsset(flowConfigSO_v2, $"{assetPath}/{name}");
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"✅ Created {columns} basic order configs in {savePath}");
    }
}
