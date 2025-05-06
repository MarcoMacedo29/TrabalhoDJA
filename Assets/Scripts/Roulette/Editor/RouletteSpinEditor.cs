using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(RouletteSlot))]
public class RouletteSlotDrawer : PropertyDrawer
{
    private readonly int[] noRewardIndices = { 6, 11, 14 };

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (!property.isExpanded) return EditorGUIUtility.singleLineHeight;


        int lines = 3;

        if (ShouldShowNoReward(property)) lines++;

        if (ShouldHideSwordPart(property)) lines--;

        return EditorGUIUtility.singleLineHeight * (lines + 1);
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        int index = GetIndex(property);

        property.isExpanded = EditorGUI.Foldout(
            new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight),
            property.isExpanded,
            $"Element {index}",
            true
        );

        if (!property.isExpanded) return;

        EditorGUI.indentLevel++;
        float y = position.y + EditorGUIUtility.singleLineHeight;

        EditorGUI.PropertyField(
            new Rect(position.x, y, position.width, EditorGUIUtility.singleLineHeight),
            property.FindPropertyRelative("image"));
        y += EditorGUIUtility.singleLineHeight;

        EditorGUI.PropertyField(
            new Rect(position.x, y, position.width, EditorGUIUtility.singleLineHeight),
            property.FindPropertyRelative("centerAngle"));
        y += EditorGUIUtility.singleLineHeight;

        if (!ShouldHideSwordPart(property))
        {
            EditorGUI.PropertyField(
                new Rect(position.x, y, position.width, EditorGUIUtility.singleLineHeight),
                property.FindPropertyRelative("swordPart"));
            y += EditorGUIUtility.singleLineHeight;
        }

        if (ShouldShowNoReward(property))
        {
            EditorGUI.PropertyField(
                new Rect(position.x, y, position.width, EditorGUIUtility.singleLineHeight),
                property.FindPropertyRelative("noReward"));
        }

        EditorGUI.indentLevel--;
    }

    private bool ShouldShowNoReward(SerializedProperty property)
    {
        int index = GetIndex(property);
        return System.Array.Exists(noRewardIndices, i => i == index);
    }

    private bool ShouldHideSwordPart(SerializedProperty property)
    {
        int index = GetIndex(property);
        return System.Array.Exists(noRewardIndices, i => i == index);
    }

    private int GetIndex(SerializedProperty property)
    {
        string path = property.propertyPath;
        int startIndex = path.IndexOf("[") + 1;
        int endIndex = path.IndexOf("]");
        string indexStr = path.Substring(startIndex, endIndex - startIndex);
        int.TryParse(indexStr, out int index);
        return index;
    }
}
