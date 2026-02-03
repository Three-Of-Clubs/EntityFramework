using UnityEditor;
using UnityEngine;

namespace EntityFramework.Editor
{
	[CanEditMultipleObjects]
    [CustomEditor(typeof(EntityAsset), true)]
    public class EntityAssetEditor : UnityEditor.Editor
	{
		public override void OnInspectorGUI()
		{
			if (targets != null)
			{
				foreach (Object obj in targets)
					EntitySystem.Record(obj);
			}
			else if (target)
				EntitySystem.Record(target);

			base.OnInspectorGUI();
		}
	}
}