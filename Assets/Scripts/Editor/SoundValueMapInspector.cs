using Controllers.Singletons;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Editor
{
	[CustomPropertyDrawer(typeof(SoundValuePair))]
	public class SoundValuePairInspector : PropertyDrawer
	{
		public override void OnGUI(Rect pos, SerializedProperty property, GUIContent label)
		{
			Rect enumRect = new Rect(pos.x, pos.y, pos.width / 4 + 8, pos.height);
			Rect clipRect = new Rect(pos.x + pos.width / 4, pos.y, pos.width - pos.width / 4, pos.height);

			EditorGUI.BeginProperty(pos, label, property);

			SerializedProperty type = property.FindPropertyRelative("type");
			SerializedProperty clip = property.FindPropertyRelative("clip");
			type.enumValueIndex = (int)(SoundType)EditorGUI.EnumPopup(enumRect, (SoundType)type.enumValueIndex, EditorStyles.popup);
			EditorGUI.ObjectField(clipRect, clip, GUIContent.none);

			EditorGUI.EndProperty();
		}
	}
}