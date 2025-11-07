using System.Collections;
using System.Collections.Generic;
using Sonat.Enums;
using SonatFramework.Systems.InventoryManagement.GameResources;
using UnityEditor;
using UnityEngine;

namespace MyGame.Modules.QuestEvent
{
    public class QuestEventImporter : CSVImporter
    {
        [SerializeField] private QuestEventConfigSO questEventConfig;

        private SerializedObject so;
        private SerializedProperty questEventConfigProp;

        // Lưu scroll position ở cấp class thay vì trong OnGUI
        private Vector2 scrollPosition;

        [MenuItem("Tools/QuestEvent/QuestEvent Importer")]
        public static void Open()
        {
            GetWindow<QuestEventImporter>("QuestEvent Importer");
        }


        private void OnEnable()
        {
            so = new SerializedObject(this);
            questEventConfigProp = so.FindProperty("questEventConfig");
        }

        protected override void OnGUI()
        {
            GUILayout.Label("Import data into card config", EditorStyles.boldLabel);
            EditorGUILayout.Space();
            GUILayout.Label("Note: This tool will import data into the card configuration (CardConfigSO)");

            EditorGUILayout.Space(20);

            so.Update();

            EditorGUILayout.PropertyField(questEventConfigProp, true);
            so.ApplyModifiedProperties();

            base.OnGUI();
        }

        protected override void ImportCSV()
        {
            string[] lines = GetLines();
            for (int i = 0; i < lines.Length; i++)
            {
                string[] values = lines[i].Split(',');
                var rewardData = new RewardData();
                for (int j = 7; j >= 0; j--)
                {
                    if (string.IsNullOrEmpty(values[8 + j]) == false)
                    {
                        var resource = GetResourceData(j, values[8 + j]);
                        if (resource != null)
                        {
                            rewardData.AddReward(resource);
                        }
                    }
                }

                if (i < questEventConfig.listMilestones.Count)
                {
                    questEventConfig.listMilestones[i].numItem = int.Parse(values[1]);
                    questEventConfig.listMilestones[i].rewardData = rewardData;
                }
                else
                {
                    var milestone = new MilestoneConfig();
                    milestone.index = i;
                    milestone.numItem = int.Parse(values[1]);
                    milestone.rewardData = rewardData;
                    questEventConfig.listMilestones.Add(milestone);
                }
            }
            EditorUtility.SetDirty(questEventConfig);
            AssetDatabase.SaveAssets();
            Debug.Log("<color=green>Import data to QuestEventConfig success</color>");
        }

        private ResourceData GetResourceData(int index, string value)
        {
            var resourceData = new ResourceData();
            var num = float.Parse(value);

            var gameResource = GameResource.None;
            switch (index)
            {
                case 0:
                    gameResource = GameResource.Lives;
                    break;
                case 1:
                    gameResource = GameResource.BoosterSpatula;
                    break;
                case 2:
                    gameResource = GameResource.BoosterUndo;
                    break;
                case 3:
                    gameResource = GameResource.BoosterShuffle;
                    break;
                case 4:
                    gameResource = GameResource.BoosterFoodBox;
                    break;
                case 5:
                    return null;
                    break;
                case 6:
                    gameResource = (GameResource)((int)GameResource.Card_Randomx1 + (int)num - 1);
                    break;
                case 7:
                    gameResource = GameResource.Coin;
                    break;
            }

            resourceData.resource = gameResource;
            if (GameResourceHelper.ResourceType(gameResource) == GameResourceType.Card)
            {
                resourceData.quantity = 1;
            }
            else if (gameResource == GameResource.Lives)
            {
                resourceData.quantity = (int)(num * 3600);
            }
            else
            {
                resourceData.quantity = (int)num;
            }
            return resourceData;
        }
    }
}