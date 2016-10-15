using System;

namespace UnityEditor
{
	public enum SelectionMode
	{
		Unfiltered,
		TopLevel,
		Deep,
		ExcludePrefab = 4,
		Editable = 8,
		Assets = 16,
		DeepAssets = 32,
		OnlyUserModifiable = 8
	}
}
