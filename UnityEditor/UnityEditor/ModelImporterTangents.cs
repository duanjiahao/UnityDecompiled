using System;

namespace UnityEditor
{
	public enum ModelImporterTangents
	{
		Import,
		CalculateLegacy,
		CalculateLegacyWithSplitTangents = 4,
		CalculateMikk = 3,
		None = 2
	}
}
