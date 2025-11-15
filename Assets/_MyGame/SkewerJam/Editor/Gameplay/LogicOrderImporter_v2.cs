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
            "E0",
            "E1",
            "M0",
            "M0",
            "M0",
            "M1",
            "M1",
            "M1",
            "M2",
            "M2",
            "M2",
            "H0",
            "H0",
            "H0",
            "H1",
            "H1",
            "H1",
            "H2",
            "H2",
            "H2",
            "SH0",
            "SH0",
            "SH0",
            "SH1",
            "SH1",
            "SH1",
            "SH2",
            "SH2",
            "SH2",
            };
        for (int i = 0; i < rows; i++)
        {
            var name = $"LogicOrderConfigSO_v2_{prefixNames[i]}_{i}.asset";
            string assetPath = $"{savePath}";

            var flowConfigSO_v2 = ScriptableObject.CreateInstance<HardFlowConfigSO_v2>();
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
