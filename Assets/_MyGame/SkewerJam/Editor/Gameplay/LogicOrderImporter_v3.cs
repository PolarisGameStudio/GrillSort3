using System;
using System.Collections;
using System.Collections.Generic;
using MyGame.SkewerJam.Gameplay.LogicOrder.Configs;
using UnityEditor;
using UnityEngine;

public class LogicOrderImporter_v3 : CSVImporter
{
    private string savePath = "Assets/_MyGame/SkewerJam/ScriptableObjects/Gameplay_SkewerJam/Gameplay/LogicOrder_v2";

    [MenuItem("Tools/SkewerSort/ForceHardLogicOrderImporter_v2")]
    public static void Open()
    {
        GetWindow<LogicOrderImporter_v3>("Gameplay/ForceHardLogicOrderImporter_v3");
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

        var name = $"ForceHardOrderConfigSO_v2.asset";
        string assetPath = $"{savePath}";
        var forceHardOrderConfigSO_v2 = ScriptableObject.CreateInstance<ForceHardOrderConfigSO_v2>();
        forceHardOrderConfigSO_v2.listForceHardOrderDatas = new List<ForceHardOrderConfigSO_v2.ForceHardOrderData>();
        for (int i = 0; i < rows; i++)
        {
            var data = new ForceHardOrderConfigSO_v2.ForceHardOrderData();
            data.rateP4 = float.Parse(GetValues(lines[i])[1].Replace("%", "")) / 100f;
            data.rateP5 = float.Parse(GetValues(lines[i])[2].Replace("%", "")) / 100f;
            data.rateDuplicate = float.Parse(GetValues(lines[i])[3].Replace("%", "")) / 100f;
            forceHardOrderConfigSO_v2.listForceHardOrderDatas.Add(data);
        }


        AssetDatabase.CreateAsset(forceHardOrderConfigSO_v2, $"{assetPath}/{name}");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"✅ Created {columns} basic order configs in {savePath}");
    }
}
