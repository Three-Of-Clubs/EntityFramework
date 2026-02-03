using System;

namespace EntityFramework
{
    [Serializable]
    public class SceneIDsMap : DoubleDictionary<ushort, SceneReference>
    {
		protected override bool IsValid(SceneReference sceneRef)
		{
			return !string.IsNullOrEmpty(sceneRef.RawScenePath);
		}
	}
}
