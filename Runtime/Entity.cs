using System;
using UnityEngine;

using Object = UnityEngine.Object;

namespace EntityFramework
{
    [Serializable]
    public struct Entity
    {
        [SerializeField] EntityBehaviour gameObject;
        [SerializeField] EntityAsset scriptableObject;

        public Object Object => gameObject ? gameObject : scriptableObject;

        public bool IsGameObject => gameObject;
        public bool IsScriptableObject => scriptableObject;

        public bool IsValidEntity => gameObject || scriptableObject;

		public static implicit operator Entity(EntityBehaviour entity) => new Entity { gameObject = entity };
        public static implicit operator Entity(EntityAsset entity) => new Entity { scriptableObject = entity };

        public static implicit operator EntityBehaviour(Entity entity) => entity.gameObject;
        public static implicit operator EntityAsset(Entity entity) => entity.scriptableObject;

        public static implicit operator bool(Entity entity) => entity.IsValidEntity;

        public static bool operator ==(Entity e1, object e2) => e1.Equals(e2);
        public static bool operator !=(Entity e1, object e2) => !e1.Equals(e2);

        public override bool Equals(object obj)
		{
            if (obj is Entity entity)
                return entity.Object == Object;

            if (obj is Object unityObj)
                return Object == unityObj;

            if (obj is null)
                return !Object;

            return false;
		}

		public override int GetHashCode()
		{
            return Object ? Object.GetHashCode() : 0;
		}
	}
}
