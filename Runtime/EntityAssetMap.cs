using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace EntityFramework
{
    public class EntityAssetMap : AutoSingleton<EntityAssetMap>, IEntityMap
    {
        [SerializeField] EntityMap entityMap = new EntityMap();

		public bool Contains(Entity entity)
		{
			return entityMap.Contains(entity);
		}

		public EntityID Get(Entity asset)
		{
			if (entityMap.TryGet(asset, out EntityID id))
				return id;

			return EntityID.InvalidID;
		}

		public Entity Get(EntityID objID)
		{
			if (entityMap.TryGet(objID, out Entity entity))
				return entity;

			return default;
		}

		public void Record(Entity entity)
		{
			entityMap.Add(EntityIDsManager.Instance.RequestNewID(0, entity.IsGameObject ? EntityFlags.IsGameObject | EntityFlags.IsPrefab : 0), entity);

#if UNITY_EDITOR
			UnityEditor.EditorUtility.SetDirty(this);
#endif
		}
	}
}