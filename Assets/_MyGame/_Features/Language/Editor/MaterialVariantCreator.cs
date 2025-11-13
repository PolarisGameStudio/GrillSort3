using UnityEngine;
using UnityEditor;
using UnityEditor.Rendering;
using System.Collections.Generic;

public class MaterialVariantCreator : EditorWindow
{
    private Material baseMaterial;
    private string variantBaseName = "MyMaterialVariant";
    private int variantCount = 3;
    private string savePath = "Assets/";

    [MenuItem("Tools/Language/Create Material Variants")]
    public static void ShowWindow()
    {
        GetWindow<MaterialVariantCreator>("Language/Material Variant Creator");
    }

    void OnGUI()
    {
        GUILayout.Label("Material Variant Generator", EditorStyles.boldLabel);

        baseMaterial = (Material)EditorGUILayout.ObjectField("Base Material", baseMaterial, typeof(Material), false);
        variantBaseName = EditorGUILayout.TextField("Variant Base Name", variantBaseName);
        variantCount = EditorGUILayout.IntField("Number of Variants", variantCount);
        savePath = EditorGUILayout.TextField("Save Path", savePath);

        if (GUILayout.Button("Generate Variants"))
        {
            CreateVariants();
        }
    }

    void CreateVariants()
    {
        if (baseMaterial == null)
        {
            Debug.LogError("Base Material is missing!");
            return;
        }

        if (variantCount < 1)
        {
            Debug.LogWarning("Variant count must be at least 1.");
            return;
        }

        List<Material> createdMaterials = new List<Material>();

        var colors = new string[] {
            "Black",
            "Blue",
            "Brown",
            "Gray",
            "Green",
            "Purple",
            "Pink",
            "Purple",
            "Red",
            "Yellow"};
        for (int i = 0; i < variantCount; i++)
        {
            string newName = $"{variantBaseName} {colors[i]}";
            string assetPath = $"{savePath}/{newName}.mat";

            // Không có API CreateMaterialVariant trong Unity, nên để tạo "variant",
            // thông thường bạn clone một material mới từ baseMaterial.
            Material newMat = new Material(baseMaterial);
            newMat.name = newName;
            AssetDatabase.CreateAsset(newMat, assetPath);

            createdMaterials.Add(newMat);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"✅ Created {createdMaterials.Count} material variants in {savePath}");
    }
}
