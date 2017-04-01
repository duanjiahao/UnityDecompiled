using System;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental.Director
{
	[RequiredByNativeCode]
	public abstract class ScriptPlayable : Playable
	{
		public virtual void OnGraphStart()
		{
		}

		public virtual void OnGraphStop()
		{
		}

		public virtual void OnDestroy()
		{
		}

		public virtual void PrepareFrame(FrameData info)
		{
		}

		public virtual void ProcessFrame(FrameData info, object playerData)
		{
		}

		public virtual void OnPlayStateChanged(FrameData info, PlayState newState)
		{
		}
	}
}
