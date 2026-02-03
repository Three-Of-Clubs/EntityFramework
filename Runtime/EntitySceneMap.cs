using UnityEngine;
using UnityEngine.SceneManagement;

namespace EntityFramework
{
	public class EntitySceneMap : MonoBehaviour, IEntityMap
	{
		[field: SerializeField, ReadOnly]
		public ushort SceneID { get; private set; }

		[SerializeField] EntityMap entityMap = new EntityMap();

		public void Initialize(Scene scene)
		{
			SceneID = EntityIDsManager.Instance.GetIDFor(scene);
		}

		public bool Contains(EntityBehaviour sceneObj)
		{
			return entityMap.Contains(sceneObj);
		}

		public EntityID Get(Entity sceneObj)
		{
			if (entityMap.TryGet(sceneObj, out EntityID id))
				return id;

			return EntityID.InvalidID;
		}

		public Entity Get(EntityID entityID)
		{
			if (entityMap.TryGet(entityID, out Entity entity))
				return entity;

			return default;
		}

		public void Record(EntityBehaviour sceneObj)
		{
			entityMap.Add(EntityIDsManager.Instance.RequestNewID(SceneID, EntityFlags.IsGameObject), sceneObj);

#if UNITY_EDITOR
			UnityEditor.EditorUtility.SetDirty(this);
#endif
		}
	}
}