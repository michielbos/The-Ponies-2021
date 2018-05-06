using UnityEngine;
using UnityEditor;

public class ToolMapInspector : UnityEditor.Editor
{
    [CustomPropertyDrawer(typeof(ToolController.ToolMap))]
    public class SoundValuePairInspector : PropertyDrawer
    {
        public override void OnGUI(Rect pos, SerializedProperty property, GUIContent label)
        {
            Rect enumRect = new Rect(pos.x, pos.y, pos.width / 4 + 8, pos.height);
            Rect clipRect = new Rect(pos.x + pos.width / 4, pos.y, pos.width - pos.width / 4, pos.height);

            EditorGUI.BeginProperty(pos, label, property);

            SerializedProperty type = property.FindPropertyRelative("type");
            SerializedProperty tool = property.FindPropertyRelative("tool");
            type.enumValueIndex = (int)(ToolType)EditorGUI.EnumPopup(enumRect, (ToolType)type.enumValueIndex, EditorStyles.popup);
            EditorGUI.ObjectField(clipRect, tool, GUIContent.none);

            EditorGUI.EndProperty();
        }
    }
}