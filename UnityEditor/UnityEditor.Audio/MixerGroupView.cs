using System;
using UnityEngine.Scripting;

namespace UnityEditor.Audio
{
	[RequiredByNativeCode]
	internal struct MixerGroupView
	{
		public GUID[] guids;

		public string name;
	}
}
