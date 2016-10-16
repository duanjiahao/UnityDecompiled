using System;

namespace UnityEditor
{
	public enum PrefabType
	{
		None,
		Prefab,
		ModelPrefab,
		PrefabInstance,
		ModelPrefabInstance,
		MissingPrefabInstance,
		DisconnectedPrefabInstance,
		DisconnectedModelPrefabInstance
	}
}
