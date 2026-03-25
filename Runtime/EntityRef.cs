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

        public void OnEntityLoaded(Action<Entity> action)
        {
            if (Entity.Object)
                action(Entity);
            else
                EntitySystem.Defer(action, id);
        }
    }

    [Serializable]
    public sealed class EntityRef<T> : EntityRef where T : IEntity
    {
        public T Component => EntitySystem.Get<T>(id);

        public void OnComponentLoaded(Action<T> action)
        {
            if (Component != null && Component.IsValidInstance())
                action(Component);
            else
                EntitySystem.Defer(InvokeCallbackWithComponent, id);

            void InvokeCallbackWithComponent(Entity obj)
            {
                if (Entity.Object is T t)
                    action(t);
            }
        }
	}
}