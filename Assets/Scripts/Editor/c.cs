using UnityEngine;
using UnityEditor;
using Sonat.Enums;

[CustomPropertyDrawer(typeof(GameResource))]
public class EnemyTypeDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        // Lấy enum hiện tại
        var value = (GameResource)property.enumValueIndex;
        Color color = Color.white;

        switch (value)
        {
            case GameResource.BuffAddPlate: color = Color.green; break;
            case GameResource.BuffAddOrder: color = Color.yellow; break;
        }

        var prevColor = GUI.color;
        GUI.color = color;

        property.enumValueIndex = EditorGUI.Popup(position, label.text, property.enumValueIndex, property.enumDisplayNames);

        GUI.color = prevColor;
        EditorGUI.EndProperty();
    }
}
