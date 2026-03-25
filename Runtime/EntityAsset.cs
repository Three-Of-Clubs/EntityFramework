using UnityEngine;

namespace EntityFramework
{
	public abstract class EntityAsset : ScriptableObject, IEntity
	{
		bool IEntity.IsValidInstance()
		{
			return true;
		}
	}
}