using UnityEditor;
using UnityEngine;

namespace EntityFramework.Editor
{
    [CustomPropertyDrawer(typeof(EntityID))]
    public class EntityIDPropertyDrawer : PropertyDrawer
    {
		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return EditorGUIUtility.singleLineHeight;
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, property);
			
			position = EditorGUI.PrefixLabel(position, label);

			Rect sceneIDRect = position;
			Rect entityIDRect = position;
			Rect flagsButtonRect = position;

			sceneIDRect.width *= .3f;
			entityIDRect.width *= .7f;
			entityIDRect.width -= 20;
			entityIDRect.x += sceneIDRect.width;
			flagsButtonRect.width = 20;
			flagsButtonRect.x += sceneIDRect.width + entityIDRect.width;

			int oldIndexLevel = EditorGUI.indentLevel;
			EditorGUI.indentLevel = 0;

			EditorGUI.PropertyField(sceneIDRect, property.FindPropertyRelative("idScene"), GUIContent.none);
			EditorGUI.PropertyField(entityIDRect, property.FindPropertyRelative("idEntity"), GUIContent.none);

			SerializedProperty flagsProperty = property.FindPropertyRelative("flags");

			EntityFlags flags = (EntityFlags)flagsProperty.uintValue;
			EntityFlags newFlags = (EntityFlags)EditorGUI.EnumFlagsField(flagsButtonRect, GUIContent.none, flags);

			if (flags != newFlags)
				flagsProperty.uintValue = (ushort)newFlags;

			EditorGUI.indentLevel = oldIndexLevel;

			property.serializedObject.ApplyModifiedProperties();
			EditorGUI.EndProperty();
		}
	}
}