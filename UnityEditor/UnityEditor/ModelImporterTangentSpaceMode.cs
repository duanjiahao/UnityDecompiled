using System;

namespace UnityEditor
{
	public enum ModelImporterTangentSpaceMode
	{
		[Obsolete("Use ModelImporterNormals.Import instead")]
		Import,
		[Obsolete("Use ModelImporterNormals.Calculate instead")]
		Calculate,
		[Obsolete("Use ModelImporterNormals.None instead")]
		None
	}
}
