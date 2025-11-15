using System;
using System.Collections;
using System.Collections.Generic;
using MyGame.SkewerJam.Gameplay.LogicOrder.Configs;
using UnityEditor;
using UnityEngine;

public class BasicOrderConfigImporter_v2 : CSVImporter
{
    private string savePath = "Assets/_MyGame/SkewerJam/ScriptableObjects/Gameplay_SkewerJam/Gameplay/LogicOrder_v2";

    [MenuItem("Tools/SkewerSort/v2/BasicOrderConfigImporter_v2")]
    public static void Open()
    {
        GetWindow<BasicOrderConfigImporter_v2>("Gameplay/v2/BasicOrderConfigImporter_v2");
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

        // Tạo một ScriptableObject mới kiểu CurveConfigSO và lưu nó
        string name = $"BasicOrderConfigSO_v2_{DateTime.Now.ToString("yyyyMMddHHmmss")}.asset";
        string assetPath = $"{savePath}";
        var basicOrderConfigSO_v2 = ScriptableObject.CreateInstance<BasicOrderConfigSO_v2>();
        basicOrderConfigSO_v2.listCurveConfigs = new List<CurveConfig>();
        for (int i = 0; i < columns; i++)
        {
            var listRateConfigs = new List<RateConfig>();
            for (int j = 0; j < rows; j++)
            {
                var str = GetValues(lines[j])[i];
                var rate = int.Parse(str.Replace("%", ""));


                var rateConfig = new RateConfig();
                rateConfig.rate = rate / 100f;
                listRateConfigs.Add(rateConfig);
            }
            var curveConfig = new CurveConfig();
            curveConfig.listRateConfigs = listRateConfigs;
            basicOrderConfigSO_v2.listCurveConfigs.Add(curveConfig);
        }
        AssetDatabase.CreateAsset(basicOrderConfigSO_v2, $"{assetPath}/{name}");

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"✅ Created {columns} basic order configs in {savePath}");
    }
}
