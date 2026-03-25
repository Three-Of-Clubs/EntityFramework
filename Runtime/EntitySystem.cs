using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

using Object = UnityEngine.Object;

namespace EntityFramework
{
	public static class EntitySystem
	{
		static Dictionary<EntityID, List<Action<Entity>>> deferredEntityActions = new Dictionary<EntityID, List<Action<Entity>>>();

		#region getters
		public static Entity Get(EntityID id)
		{
			if (id.IDScene == 0)
				return EntityAssetMap.Instance.Get(id);

			Scene scene = EntityIDsManager.Instance.GetScene(id.IDScene);
			EntitySceneMap sceneMap = GetSceneMapIn(scene);

			return sceneMap ? sceneMap.Get(id) : default;
		}

		public static T Get<T>(EntityID id) where T : IEntity
		{
			Entity entity = Get(id);

			if (!entity)
				return default;

			if (entity.Object is T scriptableObjectType)
				return scriptableObjectType;

			if (entity.Object is EntityBehaviour entityBehaviour)
				return entityBehaviour.GetComponent<T>();

			return default;
		}

		public static EntityID Get(Object obj)
		{
			if (obj is GameObject go && go.TryGetComponent(out EntityBehaviour eb))
				obj = eb;

			if (obj is EntityBehaviour behaviour)
				return Get(behaviour);
			else if (obj is EntityAsset asset)
				return EntityAssetMap.Instance.Get(asset);

			return EntityID.InvalidID;
		}

		public static EntityID Get(EntityBehaviour behaviour)
		{
			Scene scene = behaviour.gameObject.scene;

			if (!scene.IsValid())
				return EntityAssetMap.Instance.Get(behaviour);

			EntitySceneMap sceneMap = GetSceneMapOf(behaviour);

			if (!sceneMap)
				return EntityID.InvalidID;

			return sceneMap.Get(behaviour);
		}
		#endregion

		#region record
#if UNITY_EDITOR
		[InitializeOnLoadMethod]
		static void OnInitialize()
		{
			EditorSceneManager.sceneOpened += OnSceneOpened;
			EditorApplication.playModeStateChanged += OnPlayModeChanged;
		}

		private static void OnPlayModeChanged(PlayModeStateChange state)
		{
			if (state == PlayModeStateChange.ExitingEditMode)
				RecordAll();
		}

		static void OnSceneOpened(Scene scene, OpenSceneMode mode)
		{
			RecordAll();
		}
#endif

		public static void RecordAll()
		{
			EntityBehaviour[] allEntities = Object.FindObjectsByType<EntityBehaviour>(FindObjectsSortMode.None);

			foreach (EntityBehaviour behaviour in allEntities)
				RecordGameObject(behaviour);
		}

		public static void Record(Object target)
		{
			if (target is GameObject gameObject && gameObject.TryGetComponent(out EntityBehaviour entity))
				target = entity;

			if (target is EntityBehaviour sceneObj)
				RecordGameObject(sceneObj);
			else if (target is EntityAsset assetObj)
				RecordAsset(assetObj);
		}

		static void RecordGameObject(EntityBehaviour sceneObj)
		{
			Scene scene = sceneObj.gameObject.scene;

			if (!scene.IsValid())
			{
				RecordAsset(sceneObj);
				return;
			}

#if UNITY_EDITOR
			//if in prefab edition mode, not a proper scene, don't record
			if (PrefabStageUtility.GetCurrentPrefabStage() != null)
				return;
#endif

			EntitySceneMap sceneMap = GetSceneMapOf(sceneObj);

			if (!sceneMap)
			{
				sceneMap = new GameObject($"{scene.name}_EntityMap").AddComponent<EntitySceneMap>();
				SceneManager.MoveGameObjectToScene(sceneMap.gameObject, scene);
				sceneMap.Initialize(scene);
#if UNITY_EDITOR
				EditorSceneManager.MarkSceneDirty(scene);
#endif
			}

			if (sceneMap.Contains(sceneObj))
				return;

			sceneMap.Record(sceneObj);
		}

		static void RecordAsset(Object assetObj)
		{
			Entity entity;

			if (assetObj is EntityAsset entityAsset)
				entity = entityAsset;
			else if (assetObj is EntityBehaviour behaviour)
				entity = behaviour;
			else
				throw new InvalidOperationException("Can't record an asset that is neither an EntityAsset or an EntityBehaviour");

			if (EntityAssetMap.Instance.Contains(entity))
				return;

			EntityAssetMap.Instance.Record(entity);
		}
		#endregion

		#region scene maps
		static EntitySceneMap GetSceneMapOf(EntityBehaviour entity)
		{
			return GetSceneMapIn(entity.gameObject.scene);
		}

		public static EntitySceneMap GetSceneMapIn(Scene scene)
		{
			if (!scene.IsValid() || !scene.isLoaded)
				return null;

			GameObject[] rootObjs = scene.GetRootGameObjects();

			foreach (GameObject obj in rootObjs)
			{
				if (obj.TryGetComponent(out EntitySceneMap sceneMap))
					return sceneMap;
			}

			return null;
		}
		#endregion

		#region instances management
		internal static void OnEntityAwoken(EntityBehaviour entity)
		{
			if (!deferredEntityActions.TryGetValue(Get(entity), out List<Action<Entity>> callbacks))
				return;

			foreach (Action<Entity> callback in callbacks)
			{
				callback(entity);

				if (!entity)
					break;
			}
		}

		internal static void OnEntityDestroyed(EntityBehaviour entity)
		{
			deferredEntityActions.Remove(Get(entity));
		}

		internal static void Defer(Action<Entity> action, EntityID id)
		{
			if (!deferredEntityActions.ContainsKey(id))
				deferredEntityActions.Add(id, new());

			deferredEntityActions[id].Add(action);
		}
		#endregion
	}
}