using System;
using UnityEngine;

namespace EntityFramework
{
    [Serializable]
    public class EntityRef
    {
        [SerializeField] protected EntityID id;
        [SerializeField] string lastSceneName;

        public Entity Entity => EntitySystem.Get(id);
    }

    [Serializable]
    public sealed class EntityRef<T> : EntityRef where T : IEntity
    {
        public T Component => EntitySystem.Get<T>(id);
	}
}