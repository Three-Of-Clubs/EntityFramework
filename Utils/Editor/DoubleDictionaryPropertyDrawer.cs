using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(DoubleDictionary<,>), true)]
public class DoubleDictionaryPropertyDrawer : PropertyDrawer
{
	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		if (!property.isExpanded)
			return EditorGUIUtility.singleLineHeight;

		float heightSum = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

		SerializedProperty keysProp = property.FindPropertyRelative("serializedKeys");
		SerializedProperty valuesProp = property.FindPropertyRelative("serializedValues");

		for (int i = 0; i < keysProp.arraySize; i++)
		{
			heightSum += EditorGUI.GetPropertyHeight(keysProp.GetArrayElementAtIndex(i));
			heightSum += EditorGUIUtility.standardVerticalSpacing;
		}

		for (int i = 0; i < valuesProp.arraySize; i++)
		{
			heightSum += EditorGUI.GetPropertyHeight(valuesProp.GetArrayElementAtIndex(i));
			heightSum += EditorGUIUtility.standardVerticalSpacing;
		}

		return heightSum;
	}

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		EditorGUI.BeginProperty(position, label, property);

		position.height = EditorGUIUtility.singleLineHeight;
		property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, label);

		bool wasGUIenabled = GUI.enabled;
		GUI.enabled = false;

		EditorGUI.indentLevel++;

		if (property.isExpanded)
			DisplayMapContent(position, property);

		EditorGUI.indentLevel--;

		GUI.enabled = wasGUIenabled;
		property.serializedObject.ApplyModifiedProperties();
		EditorGUI.EndProperty();
	}

	void DisplayMapContent(Rect position, SerializedProperty property)
	{
		SerializedProperty keysProp = property.FindPropertyRelative("serializedKeys");
		SerializedProperty valuesProp = property.FindPropertyRelative("serializedValues");

		for (int i = 0; i < valuesProp.arraySize; i++)
		{
			SerializedProperty key = keysProp.GetArrayElementAtIndex(i);
			SerializedProperty value = valuesProp.GetArrayElementAtIndex(i);

			position.y += position.height + EditorGUIUtility.standardVerticalSpacing;
			position.height = EditorGUI.GetPropertyHeight(key);
			EditorGUI.PropertyField(position, keysProp.GetArrayElementAtIndex(i), new GUIContent("Key"), true);

			position.y += position.height + EditorGUIUtility.standardVerticalSpacing;
			position.height = EditorGUI.GetPropertyHeight(value);
			EditorGUI.PropertyField(position, valuesProp.GetArrayElementAtIndex(i), new GUIContent("Value"), true);
		}
	}
}