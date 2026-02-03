using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
public class ReadOnlyAttributePropertyDrawer : PropertyDrawer
{
	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		return EditorGUI.GetPropertyHeight(property);
	}

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		EditorGUI.BeginProperty(position, label, property);

		bool wasGUIenabled = GUI.enabled;
		GUI.enabled = false;

		EditorGUI.PropertyField(position, property, label, true);

		GUI.enabled = wasGUIenabled;

		EditorGUI.EndProperty();
	}
}
