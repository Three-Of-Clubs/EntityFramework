using UnityEditor;

namespace EntityFramework.Editor
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(EntityBehaviour), true)]
	public class EntityBehaviourEditor : EntityAssetEditor
	{
	}
}