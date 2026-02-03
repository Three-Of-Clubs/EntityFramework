using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EntityFramework
{
	[Serializable]
    public class EntityIDsManager : AutoSingleton<EntityIDsManager>
    {
		[SerializeField, HideInInspector] ushort newSceneID;
		[SerializeField, HideInInspector] uint newEntityID;

		[SerializeField] SceneIDsMap sceneIDMap = new SceneIDsMap();

		public EntityID RequestNewID(ushort sceneID, EntityFlags flags)
		{
			uint newID = ++newEntityID;

#if UNITY_EDITOR
			UnityEditor.EditorUtility.SetDirty(this);
#endif

			return new EntityID(sceneID, newID, flags);
		}


		public ushort GetIDFor(Scene scene)
		{
			SceneReference scRef = new SceneReference() { ScenePath = scene.path };

			if (sceneIDMap.TryGet(scRef, out ushort id))
				return id;

			ushort sceneID = ++newSceneID;

			sceneIDMap.Add(sceneID, scRef);

#if UNITY_EDITOR
			UnityEditor.EditorUtility.SetDirty(this);
#endif

			return sceneID;
		}

		public bool DoesSceneExist(ushort sceneID)
		{
			return sceneIDMap.Contains(sceneID);
		}

		public Scene GetScene(ushort sceneID)
		{
			if (!sceneIDMap.TryGet(sceneID, out SceneReference sceneRef))
				return default;

#if UNITY_EDITOR
			for (int i = 0; i < SceneManager.sceneCount; i++)
			{
				Scene sc = SceneManager.GetSceneAt(i);

				if (sc.path == sceneRef.ScenePath)
					return sc;
			}

			return default;
#else
			return SceneManager.GetSceneByPath(sceneRef.ScenePath);
#endif
		}

		public string GetScenePath(ushort sceneID)
		{
			if (sceneIDMap.TryGet(sceneID, out SceneReference sceneRef))
				return sceneRef.ScenePath;

			return string.Empty;
		}
	}
}
