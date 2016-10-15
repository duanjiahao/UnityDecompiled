using System;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[Obsolete("this API is not for public use."), UsedByNativeCode]
	public struct CacheIndex
	{
		public string name;

		public int bytesUsed;

		public int expires;
	}
}
