using System;

namespace EntityFramework
{
	[Serializable]
    public class EntityMap : DoubleDictionary<EntityID, Entity>
    {
		protected override bool IsValid(Entity entity)
		{
			return entity.IsValidEntity;
		}
	}
}
