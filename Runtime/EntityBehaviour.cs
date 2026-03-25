using UnityEngine;

namespace EntityFramework
{
	[DisallowMultipleComponent]
	public abstract class EntityBehaviour : MonoBehaviour, IEntity
	{
		protected virtual void Awake()
		{
			EntitySystem.OnEntityAwoken(this);
		}

		protected virtual void OnDestroy()
		{
			EntitySystem.OnEntityDestroyed(this);
		}

		bool IEntity.IsValidInstance()
		{
			return this;
		}
	}
}
