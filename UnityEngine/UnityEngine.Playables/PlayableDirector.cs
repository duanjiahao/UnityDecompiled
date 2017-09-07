using System;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine.Playables
{
	[RequiredByNativeCode]
	public class PlayableDirector : Behaviour, IExposedPropertyTable
	{
		public extern PlayState state
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public PlayableAsset playableAsset
		{
			get
			{
				return this.GetPlayableAssetInternal() as PlayableAsset;
			}
			set
			{
				this.SetPlayableAssetInternal(value);
			}
		}

		public extern DirectorWrapMode extrapolationMode
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern DirectorUpdateMode timeUpdateMode
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern double time
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern double initialTime
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern double duration
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public PlayableGraph playableGraph
		{
			get
			{
				PlayableGraph result = default(PlayableGraph);
				this.InternalGetCurrentGraph(ref result);
				return result;
			}
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Evaluate();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void DeferredEvaluate();

		public void Play(PlayableAsset asset)
		{
			if (asset == null)
			{
				throw new ArgumentNullException("asset");
			}
			this.Play(asset, this.extrapolationMode);
		}

		public void Play(PlayableAsset asset, DirectorWrapMode mode)
		{
			if (asset == null)
			{
				throw new ArgumentNullException("asset");
			}
			this.playableAsset = asset;
			this.extrapolationMode = mode;
			this.Play();
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetPlayableAssetInternal(ScriptableObject asset);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern ScriptableObject GetPlayableAssetInternal();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Play();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Stop();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void StopImmediately();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Pause();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Resume();

		public void SetReferenceValue(PropertyName id, UnityEngine.Object value)
		{
			PlayableDirector.INTERNAL_CALL_SetReferenceValue(this, ref id, value);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_SetReferenceValue(PlayableDirector self, ref PropertyName id, UnityEngine.Object value);

		public UnityEngine.Object GetReferenceValue(PropertyName id, out bool idValid)
		{
			return PlayableDirector.INTERNAL_CALL_GetReferenceValue(this, ref id, out idValid);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern UnityEngine.Object INTERNAL_CALL_GetReferenceValue(PlayableDirector self, ref PropertyName id, out bool idValid);

		public void ClearReferenceValue(PropertyName id)
		{
			PlayableDirector.INTERNAL_CALL_ClearReferenceValue(this, ref id);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_ClearReferenceValue(PlayableDirector self, ref PropertyName id);

		private void InternalGetCurrentGraph(ref PlayableGraph graph)
		{
			PlayableDirector.INTERNAL_CALL_InternalGetCurrentGraph(this, ref graph);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_InternalGetCurrentGraph(PlayableDirector self, ref PlayableGraph graph);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetGenericBinding(UnityEngine.Object key, UnityEngine.Object value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern UnityEngine.Object GetGenericBinding(UnityEngine.Object key);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern bool HasGenericBinding(UnityEngine.Object key);
	}
}
