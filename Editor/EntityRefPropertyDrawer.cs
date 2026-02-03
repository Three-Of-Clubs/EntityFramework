using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace EntityFramework
{
    [CustomPropertyDrawer(typeof(EntityRef), true)]
    public class EntityRefPropertyDrawer : PropertyDrawer
    {
		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return EditorGUIUtility.singleLineHeight;
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, property);

			SerializedProperty idProp = property.FindPropertyRelative("id");

			uint entityID = idProp.FindPropertyRelative("idEntity").uintValue;
			ushort sceneID = (ushort)idProp.FindPropertyRelative("idScene").uintValue;

			//null ref
			if (entityID == 0)
			{
				Object refObj = EditorGUI.ObjectField(position, label, null, GetEntityType(), true);

				if (refObj)
					AssignNewObject(property, idProp, refObj);
			}
			else
			{
				//asset
				if (sceneID == 0)
				{
					RefField(position, label, property, idProp, EntityAssetMap.Instance);
				}
				//scene object
				else
				{
					//get scene from scene ID
					//2 cases: scene unload = display "in other scene"
					//scene load = object field

					if (!EntityIDsManager.Instance.DoesSceneExist(sceneID))
					{
						AssignID(idProp, 0, 0, 0);
					}
					else
					{
						Scene scene = EntityIDsManager.Instance.GetScene(sceneID);

						if (!scene.IsValid() || !scene.isLoaded)
						{
							Rect infoBoxRect = position;
							infoBoxRect.width -= 40;

							infoBoxRect = EditorGUI.PrefixLabel(infoBoxRect, label);

							string helpMessage = "In another scene";
							string sceneName = property.FindPropertyRelative("lastSceneName").stringValue;

							if (!string.IsNullOrWhiteSpace(sceneName))
								helpMessage += $": {sceneName}";

							EditorGUI.HelpBox(infoBoxRect, helpMessage, MessageType.Info);

							Rect buttonRect = position;
							buttonRect.width = 18;
							buttonRect.x += position.width - 38;

							if (GUI.Button(buttonRect, EditorGUIUtility.IconContent("animationvisibilitytoggleon")))
							{
								EditorSceneManager.OpenScene(EntityIDsManager.Instance.GetScenePath(sceneID));
							}

							buttonRect.x += 20;

							if (GUI.Button(buttonRect, EditorGUIUtility.IconContent("d_False")))
							{
								if (EditorUtility.DisplayDialog("Remove Reference?", "Are you sure you want to remove this entity reference?", "Yes", "No"))
								{
									AssignID(idProp, 0, 0, 0);
								}
							}
						}
						else
						{
							EntitySceneMap sceneMap = EntitySystem.GetSceneMapIn(scene);
							if (sceneMap != null)
							{
								RefField(position, label, property, idProp, sceneMap);
							}
						}
					}
				}
			}

			property.serializedObject.ApplyModifiedProperties();
			EditorGUI.EndProperty();
		}

		private void RefField(Rect position, GUIContent label, SerializedProperty refProperty, SerializedProperty idProp, IEntityMap map)
		{
			EntityID objID = GetID(idProp);

			Object oldObj = map.Get(objID).Object;
			Object newObj = EditorGUI.ObjectField(position, label, oldObj, GetEntityType(), true);

			if (oldObj != newObj)
			{
				if (!newObj)
					AssignID(idProp, 0, 0, 0);
				else
					AssignNewObject(refProperty, idProp, newObj);
			}
		}

		private Type GetEntityType()
		{
			if (!fieldInfo.FieldType.IsGenericType)
				return typeof(IEntity);

			return fieldInfo.FieldType.GetGenericArguments()[0];
		}

		EntityID GetID(SerializedProperty idProp)
		{
			return new EntityID
			(
				idProp.FindPropertyRelative("idEntity").uintValue,
				(ushort)idProp.FindPropertyRelative("idScene").uintValue,
				(ushort)idProp.FindPropertyRelative("flags").uintValue
			);
		}

		void AssignID(SerializedProperty idProp, uint entityID, ushort sceneID, ushort flags)
		{
			idProp.FindPropertyRelative("idEntity").uintValue = entityID;
			idProp.FindPropertyRelative("idScene").uintValue = sceneID;
			idProp.FindPropertyRelative("flags").uintValue = flags;
		}

		void AssignNewObject(SerializedProperty refProperty, SerializedProperty idProp, Object refObj)
		{
			EntityID id = EntitySystem.Get(refObj);

			SerializedProperty sceneNameProperty = refProperty.FindPropertyRelative("lastSceneName");

			if (refObj is EntityBehaviour sceneObject)
				sceneNameProperty.stringValue = sceneObject.gameObject.scene.name;
			else
				sceneNameProperty.stringValue = string.Empty;

			if (id.IsValid)
				AssignID(idProp, id.IDEntity, id.IDScene, (ushort)id.Flags);
			else
				Debug.Log("Given object isn't a valid entity", refObj);
		}
	}
}