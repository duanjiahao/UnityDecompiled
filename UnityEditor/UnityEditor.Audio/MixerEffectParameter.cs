using System;
using UnityEngine.Scripting;

namespace UnityEditor.Audio
{
	[RequiredByNativeCode]
	internal struct MixerEffectParameter
	{
		public string parameterName;

		public GUID GUID;
	}
}
