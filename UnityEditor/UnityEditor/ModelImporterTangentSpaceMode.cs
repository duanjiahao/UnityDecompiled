using System;

namespace UnityEditor
{
	[Obsolete("Use ModelImporterNormals or ModelImporterTangents instead")]
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
