using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MyGame.SkewerJam.Level;
using Newtonsoft.Json;
using Sonat.Enums;
using UnityEditor;
using UnityEngine;

namespace MyGame.Editor.LevelData
{
    public class LevelDataEditor : CSVImporter
    {
        private string jsonFolderPath;
        private List<LevelData_SkewerJam> levels;

        [MenuItem("Tools/SkewerSort/LevelDataEditor")]
        public static void Open()
        {
            GetWindow<LevelDataEditor>("Album Importer");
        }

        protected override void OnGUI()
        {
            base.OnGUI(); // Giao diện chọn CSV sẵn có

            EditorGUILayout.Space();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            EditorGUILayout.LabelField("-----JSON FILE-----", EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            if (GUILayout.Button("Chọn file JSON"))
            {
                jsonFolderPath = EditorUtility.OpenFolderPanel("Chọn thư mục JSON", "", "");
            }

            EditorGUILayout.Space();
            if (!string.IsNullOrEmpty(filePath) && !string.IsNullOrEmpty(jsonFolderPath))
            {
                if (GUILayout.Button("Import CSV vào JSON"))
                {
                    ImportCSV();
                }
            }
        }

        protected override void ImportCSV()
        {
            if (string.IsNullOrEmpty(filePath) || string.IsNullOrEmpty(jsonFolderPath))
            {
                Debug.LogError("Chưa chọn đủ file CSV và JSON.");
                return;
            }

            // 1. Đọc tất cả các file JSON
            string[] jsonFiles = Directory.GetFiles(jsonFolderPath, "*.json");
            levels = new List<LevelData_SkewerJam>();
            foreach (string file in jsonFiles)
            {
                string json = File.ReadAllText(file);
                var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto };
                var levelData = JsonConvert.DeserializeObject<LevelData_SkewerJam>(json, settings);
                if (levelData != null) levels.Add(levelData);
            }

            string[] lines = GetLines();
            string[] headers = GetValues(lines[0]); // dòng đầu tiên là tên cột

            // 3. Map CSV vào LevelData
            for (int i = 0; i < lines.Length; i++)
            {
                var level = i + 1;
                var lData = levels.Find(l => l.level == level);
                if (lData != null)
                {
                    var values = GetValues(lines[i]);
                    var seq = values[0];
                    var difficulty = values[1];
                    Debug.Log($"Level {lData.level} " + seq + "- " + difficulty);
                    lData.difficulty = GetLevelDifficulty(difficulty);
                    lData.sequenceLogicOrderIndex = GetSequenceIndex(seq);
                }
                else{
                    Debug.LogError($"Level {level} not found");
                }
            }
            // for (int i = 0; i < levels.Count; i++)
            // {
            //     var values = GetValues(lines[i]);
            //     var lData = levels[i];
            //     var seq = values[0];
            //     var difficulty = values[1];
            //     Debug.Log($"Level {lData.level} " + seq + "- " + difficulty);
            //     lData.difficulty = GetLevelDifficulty(difficulty);
            //     lData.sequenceLogicOrderIndex = GetSequenceIndex(seq);
            // }

            // 4. Ghi lại JSON
            int idx = 0;
            foreach (string file in jsonFiles)
            {
                LevelData_SkewerJam l = levels[idx];

                string newJson = JsonConvert.SerializeObject(l, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto });
                File.WriteAllText(file, newJson);
                idx += 1;
            }


            Debug.Log("Đã import CSV vào JSON thành công!");
        }

        private LevelDifficulty GetLevelDifficulty(string difficulty)
        {
            if (difficulty == "E1") return LevelDifficulty.Easy1;
            if (difficulty == "E2") return LevelDifficulty.Easy2;
            if (difficulty == "M1") return LevelDifficulty.Medium1;
            if (difficulty == "M2") return LevelDifficulty.Medium2;
            if (difficulty == "H1") return LevelDifficulty.Hard1;
            if (difficulty == "H2") return LevelDifficulty.Hard2;

            return LevelDifficulty.Easy1;
        }

        private int GetSequenceIndex(string seq)
        {
            if (seq == "S0") return 0;
            if (seq == "S1") return 1;
            if (seq == "S2") return 2;
            if (seq == "S3") return 3;
            if (seq == "S4") return 4;
            if (seq == "S5") return 5;
            if (seq == "S6") return 6;
            if (seq == "S7") return 7;
            if (seq == "S8") return 8;
            return 9;
        }
    }
}