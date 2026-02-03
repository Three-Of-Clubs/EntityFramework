using UnityEngine;

namespace EntityFramework
{
    public interface IEntityMap
    {
		EntityID Get(Entity entity);

		Entity Get(EntityID entityID);
	}
}