using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class DoubleDictionary<T1, T2> : ISerializationCallbackReceiver
{
	[SerializeField] List<T1> serializedKeys;
	[SerializeField] List<T2> serializedValues;

	readonly Dictionary<T1, T2> indexedValues = new();
    readonly Dictionary<T2, T1> indexedKeys = new();

	public bool Contains(T1 obj)
	{
		return indexedValues.ContainsKey(obj);
	}

	public bool Contains(T2 obj)
	{
		return indexedKeys.ContainsKey(obj);
	}

	public bool TryGet(T1 key, out T2 value)
	{
		return indexedValues.TryGetValue(key, out value);
	}

	public bool TryGet(T2 key, out T1 value)
	{
		return indexedKeys.TryGetValue(key, out value);
	}

	public void Add(T1 key, T2 value)
	{
		if (indexedValues.ContainsKey(key) || indexedKeys.ContainsKey(value))
			throw new InvalidOperationException();

		indexedValues.Add(key, value);
		indexedKeys.Add(value, key);
	}

	public void OnBeforeSerialize()
	{
		serializedKeys = indexedValues.Keys.ToList();
		serializedValues = indexedValues.Values.ToList();
	}

	public void OnAfterDeserialize()
	{
		if (serializedKeys.Count != serializedValues.Count)
		{
			Debug.LogError("Can't deserialize double dictionary data");
			return;
		}

		indexedKeys.Clear();
		indexedValues.Clear();

		for (int i = 0; i < serializedKeys.Count; i++)
		{
			if (!IsValid(serializedKeys[i]) || !IsValid(serializedValues[i]))
				continue;

			indexedValues.Add(serializedKeys[i], serializedValues[i]);
			indexedKeys.Add(serializedValues[i], serializedKeys[i]);
		}
	}

	protected virtual bool IsValid(T1 key) => true;
	protected virtual bool IsValid(T2 value) => true;
}