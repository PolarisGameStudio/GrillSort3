using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MyGame.Modules.CardCollection
{
    [CreateAssetMenu(fileName = "TextMaterialSO", menuName = "MyGame/SkewerJam/Features/CardCollection/TextMaterialSO")]
    public class TextMaterialSO : ScriptableObject
    {
        public List<MaterialData> materialDatas;

        private Dictionary<TextColorType, MaterialData> _dictMaterialData = new();

        public MaterialData GetMaterialData(TextColorType type)
        {
            if (_dictMaterialData.ContainsKey(type) == false)
            {
                var materialData = materialDatas.Find(e => e.type == type);
                if (materialData != null)
                {
                    _dictMaterialData.Add(type, materialData);
                }
            }
            return _dictMaterialData[type];
        }

    }

    [Serializable]
    public class MaterialData
    {
        [GUIColor(0, 1, 0)]
        public TextColorType type;
        // public Material material;
        // public Color color;
        public string materialTerm;
    }
}