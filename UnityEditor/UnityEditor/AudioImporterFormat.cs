using System;
using System.ComponentModel;

namespace UnityEditor
{
	[EditorBrowsable(EditorBrowsableState.Never), Obsolete("UnityEditor.AudioImporterFormat has been deprecated. Use UnityEngine.AudioCompressionFormat instead.")]
	public enum AudioImporterFormat
	{
		Native = -1,
		Compressed
	}
}
