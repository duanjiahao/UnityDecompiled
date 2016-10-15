using System;
using System.ComponentModel;

namespace UnityEditor
{
	[EditorBrowsable(EditorBrowsableState.Never), Obsolete("UnityEditor.AudioImporterLoadType has been deprecated. Use UnityEngine.AudioClipLoadType instead (UnityUpgradable) -> [UnityEngine] UnityEngine.AudioClipLoadType", true)]
	public enum AudioImporterLoadType
	{
		DecompressOnLoad = -1,
		CompressedInMemory = -1,
		[Obsolete("UnityEditor.AudioImporterLoadType.StreamFromDisc has been deprecated. Use UnityEngine.AudioClipLoadType.Streaming instead (UnityUpgradable) -> UnityEngine.AudioClipLoadType.Streaming", true)]
		StreamFromDisc = -1
	}
}
