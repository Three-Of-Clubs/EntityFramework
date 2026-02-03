using System.IO;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public abstract class AutoSingleton<T> : ScriptableObject where T : AutoSingleton<T>
{
	static string loadPath => $"Singletons/{typeof(T).Name}";
	static string assetPath => $"Assets/Resources/{loadPath}.asset";
	static string completePath => $"{Path.Combine(Application.dataPath, "Resources", loadPath)}.asset";

	static T instance;
    public static T Instance
	{
		get
		{
			if (instance)
				return instance;

			instance = Resources.Load<T>(loadPath);

			if (instance)
				return instance;

			instance = CreateInstance<T>();

#if UNITY_EDITOR
			Directory.CreateDirectory(Directory.GetParent(completePath).FullName);
			AssetDatabase.CreateAsset(instance, assetPath);
			AssetDatabase.SaveAssets();
#else
			Debug.LogError($"Created a singleton of type {typeof(T).Name} but asset wasn't created");
#endif
			return instance;
		}
	}
}
