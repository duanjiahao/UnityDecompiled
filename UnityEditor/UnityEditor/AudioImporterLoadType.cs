using System;
using System.ComponentModel;
namespace UnityEditor
{
	[EditorBrowsable(EditorBrowsableState.Never), Obsolete("UnityEditor.AudioImporterLoadType has been deprecated. Use UnityEngine.AudioClipLoadType instead (UnityUpgradable).", true)]
	public enum AudioImporterLoadType
	{
		DecompressOnLoad = -1,
		CompressedInMemory = -1,
		StreamFromDisc = -1
	}
}
