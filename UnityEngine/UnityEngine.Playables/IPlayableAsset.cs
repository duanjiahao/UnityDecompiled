using System;
using System.Collections.Generic;

namespace UnityEngine.Playables
{
	public interface IPlayableAsset
	{
		double duration
		{
			get;
		}

		IEnumerable<PlayableBinding> outputs
		{
			get;
		}

		Playable CreatePlayable(PlayableGraph graph, GameObject owner);
	}
}
