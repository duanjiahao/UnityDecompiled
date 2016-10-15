using System;
using UnityEngine.Scripting;

namespace UnityEditor.Audio
{
	[RequiredByNativeCode]
	internal struct ExposedAudioParameter
	{
		public GUID guid;

		public string name;
	}
}
