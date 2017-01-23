using System;
using UnityEngine;

namespace UnityEditorInternal
{
	[Serializable]
	internal class AnimationWindowPolicy
	{
		public delegate bool SynchronizeGeometryDelegate(ref int[] sizes, ref int[] minSizes);

		public delegate bool SynchronizeFrameRateDelegate(ref float frameRate);

		public delegate bool SynchronizeCurrentTimeDelegate(ref float time);

		public delegate bool SynchronizeZoomableAreaDelegate(ref float horizontalScale, ref float horizontalTranslation);

		public delegate void OnGeometryChangeDelegate(int[] sizes);

		public delegate void OnCurrentTimeChangeDelegate(float time);

		public delegate void OnZoomableAreaChangeDelegate(float horizontalScale, float horizontalTranslation);

		[SerializeField]
		public bool triggerFramingOnSelection = true;

		[NonSerialized]
		public bool unitialized = true;

		public AnimationWindowPolicy.SynchronizeGeometryDelegate SynchronizeGeometry;

		public AnimationWindowPolicy.SynchronizeFrameRateDelegate SynchronizeFrameRate;

		public AnimationWindowPolicy.SynchronizeCurrentTimeDelegate SynchronizeCurrentTime;

		public AnimationWindowPolicy.SynchronizeZoomableAreaDelegate SynchronizeZoomableArea;

		public AnimationWindowPolicy.OnGeometryChangeDelegate OnGeometryChange;

		public AnimationWindowPolicy.OnCurrentTimeChangeDelegate OnCurrentTimeChange;

		public AnimationWindowPolicy.OnZoomableAreaChangeDelegate OnZoomableAreaChange;

		public AnimationWindowPolicy()
		{
			this.SynchronizeGeometry = (AnimationWindowPolicy.SynchronizeGeometryDelegate)Delegate.Combine(this.SynchronizeGeometry, new AnimationWindowPolicy.SynchronizeGeometryDelegate(delegate(ref int[] sizes, ref int[] minSizes)
			{
				return false;
			}));
			this.SynchronizeFrameRate = (AnimationWindowPolicy.SynchronizeFrameRateDelegate)Delegate.Combine(this.SynchronizeFrameRate, new AnimationWindowPolicy.SynchronizeFrameRateDelegate(delegate(ref float frameRate)
			{
				return false;
			}));
			this.SynchronizeCurrentTime = (AnimationWindowPolicy.SynchronizeCurrentTimeDelegate)Delegate.Combine(this.SynchronizeCurrentTime, new AnimationWindowPolicy.SynchronizeCurrentTimeDelegate(delegate(ref float time)
			{
				return false;
			}));
			this.SynchronizeZoomableArea = (AnimationWindowPolicy.SynchronizeZoomableAreaDelegate)Delegate.Combine(this.SynchronizeZoomableArea, new AnimationWindowPolicy.SynchronizeZoomableAreaDelegate(delegate(ref float horizontalScale, ref float horizontalTranslation)
			{
				return false;
			}));
			this.OnGeometryChange = (AnimationWindowPolicy.OnGeometryChangeDelegate)Delegate.Combine(this.OnGeometryChange, new AnimationWindowPolicy.OnGeometryChangeDelegate(delegate(int[] sizes)
			{
			}));
			this.OnCurrentTimeChange = (AnimationWindowPolicy.OnCurrentTimeChangeDelegate)Delegate.Combine(this.OnCurrentTimeChange, new AnimationWindowPolicy.OnCurrentTimeChangeDelegate(delegate(float time)
			{
			}));
			this.OnZoomableAreaChange = (AnimationWindowPolicy.OnZoomableAreaChangeDelegate)Delegate.Combine(this.OnZoomableAreaChange, new AnimationWindowPolicy.OnZoomableAreaChangeDelegate(delegate(float horizontalScale, float horizontalTranslation)
			{
			}));
		}
	}
}
