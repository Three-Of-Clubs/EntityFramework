using UnityEditor;
using UnityEngine;

namespace EntityFramework.Editor
{
    [CustomPropertyDrawer(typeof(Entity))]
    public class EntityPropertyDrawer : PropertyDrawer
    {
		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return EditorGUIUtility.singleLineHeight;
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, property);

			SerializedProperty objectProperty = property.FindPropertyRelative("gameObject");
			SerializedProperty assetProperty = property.FindPropertyRelative("scriptableObject");

			if (objectProperty.objectReferenceValue)
			{
				EditorGUI.PropertyField(position, objectProperty, label);
			}
			else if (assetProperty.objectReferenceValue)
			{
				EditorGUI.PropertyField(position, assetProperty, label);
			}
			else
			{
				Object newObject = EditorGUI.ObjectField(position, label, null, typeof(Object), true);

				if (newObject is GameObject gameObject && gameObject.TryGetComponent(out EntityBehaviour entityComp))
					newObject = entityComp;

				if (newObject)
				{
					if (newObject is EntityBehaviour)
						objectProperty.objectReferenceValue = newObject;
					else if (newObject is EntityAsset)
						assetProperty.objectReferenceValue = newObject;
					else
						Debug.LogError("Can only use entity types as Entity");
				}
			}

			property.serializedObject.ApplyModifiedProperties();
			EditorGUI.EndProperty();
		}
	}
}