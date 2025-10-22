using System.Collections;
using System.Collections.Generic;
using Sonat.Enums;
using SonatFramework.Systems.InventoryManagement.GameResources;
using UnityEditor;
using UnityEngine;

public class StarChestEditor : CSVImporter
{
    [SerializeField] private StarChestConfig starChestConfig;
    private SerializedObject so;
    private SerializedProperty starChestsProp;

    [MenuItem("Tools/SkewerSort/StarChestEditor")]
    public static void Open()
    {
        GetWindow<StarChestEditor>("StarChest Editor");
    }
    private void OnEnable()
    {
        so = new SerializedObject(this);
        starChestsProp = so.FindProperty("starChests");
    }

    protected override void OnGUI()
    {
        so.Update();
        EditorGUILayout.PropertyField(so.FindProperty("starChestConfig"), true);
        so.ApplyModifiedProperties();
        base.OnGUI();
    }
    protected override void ImportCSV()
    {
        string[] lines = GetLines();

        starChestConfig.starChests.Clear();
        for (int i = 1; i < lines.Length; i++)
        {
            string[] values = GetValues(lines[i]);

            var requiredStar = int.Parse(string.IsNullOrEmpty(values[1]) ? "0" : values[1]);
            var lives = int.Parse(string.IsNullOrEmpty(values[4]) ? "0" : values[4]);
            var spatula = int.Parse(string.IsNullOrEmpty(values[5]) ? "0" : values[5]);
            var addPlate = int.Parse(string.IsNullOrEmpty(values[6]) ? "0" : values[6]);
            var shuffle = int.Parse(string.IsNullOrEmpty(values[7]) ? "0" : values[7]);
            var foodBox = int.Parse(string.IsNullOrEmpty(values[8]) ? "0" : values[8]);
            var coin = int.Parse(string.IsNullOrEmpty(values[10]) ? "0" : values[10]);

            var rewardData = new RewardData();

            if (lives > 0) rewardData.AddReward(new ResourceData(GameResource.Lives, lives));
            if (spatula > 0) rewardData.AddReward(new ResourceData(GameResource.BoosterSpatula, spatula));
            if (addPlate > 0) rewardData.AddReward(new ResourceData(GameResource.BoosterAddPlate, addPlate));
            if (shuffle > 0) rewardData.AddReward(new ResourceData(GameResource.BoosterShuffle, shuffle));
            if (foodBox > 0) rewardData.AddReward(new ResourceData(GameResource.BoosterFoodBox, foodBox));
            if (coin > 0) rewardData.AddReward(new ResourceData(GameResource.Coin, coin));
            starChestConfig.starChests.Add(new StarChest
            {
                starRequire = requiredStar,
                reward = rewardData
            });
        }

        so.ApplyModifiedProperties();
        AssetDatabase.SaveAssets();
    }
}
