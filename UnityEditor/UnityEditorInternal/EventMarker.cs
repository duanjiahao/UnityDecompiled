using System;
using UnityEngine.Scripting;

namespace UnityEditorInternal
{
	[RequiredByNativeCode]
	[Serializable]
	public struct EventMarker
	{
		public int objectInstanceId;

		public int nameOffset;

		public int frame;
	}
}
