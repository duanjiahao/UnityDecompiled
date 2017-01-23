using System;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine.Scripting;

namespace UnityEngine
{
	public sealed class Canvas : Behaviour
	{
		public delegate void WillRenderCanvases();

		public static event Canvas.WillRenderCanvases willRenderCanvases
		{
			add
			{
				Canvas.WillRenderCanvases willRenderCanvases = Canvas.willRenderCanvases;
				Canvas.WillRenderCanvases willRenderCanvases2;
				do
				{
					willRenderCanvases2 = willRenderCanvases;
					willRenderCanvases = Interlocked.CompareExchange<Canvas.WillRenderCanvases>(ref Canvas.willRenderCanvases, (Canvas.WillRenderCanvases)Delegate.Combine(willRenderCanvases2, value), willRenderCanvases);
				}
				while (willRenderCanvases != willRenderCanvases2);
			}
			remove
			{
				Canvas.WillRenderCanvases willRenderCanvases = Canvas.willRenderCanvases;
				Canvas.WillRenderCanvases willRenderCanvases2;
				do
				{
					willRenderCanvases2 = willRenderCanvases;
					willRenderCanvases = Interlocked.CompareExchange<Canvas.WillRenderCanvases>(ref Canvas.willRenderCanvases, (Canvas.WillRenderCanvases)Delegate.Remove(willRenderCanvases2, value), willRenderCanvases);
				}
				while (willRenderCanvases != willRenderCanvases2);
			}
		}

		public extern RenderMode renderMode
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool isRootCanvas
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern Camera worldCamera
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public Rect pixelRect
		{
			get
			{
				Rect result;
				this.INTERNAL_get_pixelRect(out result);
				return result;
			}
		}

		public extern float scaleFactor
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float referencePixelsPerUnit
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool overridePixelPerfect
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool pixelPerfect
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float planeDistance
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern int renderOrder
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern bool overrideSorting
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern int sortingOrder
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern int targetDisplay
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[Obsolete("Setting normalizedSize via a int is not supported. Please use normalizedSortingGridSize")]
		public extern int sortingGridNormalizedSize
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float normalizedSortingGridSize
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern int sortingLayerID
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern int cachedSortingLayerValue
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern string sortingLayerName
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern Canvas rootCanvas
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_pixelRect(out Rect value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern Material GetDefaultCanvasMaterial();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern Material GetETC1SupportedCanvasMaterial();

		[Obsolete("Shared default material now used for text and general UI elements, call Canvas.GetDefaultCanvasMaterial()")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern Material GetDefaultCanvasTextMaterial();

		[RequiredByNativeCode]
		private static void SendWillRenderCanvases()
		{
			if (Canvas.willRenderCanvases != null)
			{
				Canvas.willRenderCanvases();
			}
		}

		public static void ForceUpdateCanvases()
		{
			Canvas.SendWillRenderCanvases();
		}
	}
}
