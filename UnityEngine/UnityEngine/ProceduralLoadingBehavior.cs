using System;

namespace UnityEngine
{
	public enum ProceduralLoadingBehavior
	{
		DoNothing,
		Generate,
		BakeAndKeep,
		BakeAndDiscard,
		Cache,
		DoNothingAndCache
	}
}
