using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
namespace UnityEngine
{
	public sealed class CanvasRenderer : Component
	{
		public delegate void OnRequestRebuild();
		public static event CanvasRenderer.OnRequestRebuild onRequestRebuild
		{
			[MethodImpl(MethodImplOptions.Synchronized)]
			add
			{
				CanvasRenderer.onRequestRebuild = (CanvasRenderer.OnRequestRebuild)Delegate.Combine(CanvasRenderer.onRequestRebuild, value);
			}
			[MethodImpl(MethodImplOptions.Synchronized)]
			remove
			{
				CanvasRenderer.onRequestRebuild = (CanvasRenderer.OnRequestRebuild)Delegate.Remove(CanvasRenderer.onRequestRebuild, value);
			}
		}
		public extern bool isMask
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
		public extern int relativeDepth
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		public extern int absoluteDepth
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		public void SetColor(Color color)
		{
			CanvasRenderer.INTERNAL_CALL_SetColor(this, ref color);
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_SetColor(CanvasRenderer self, ref Color color);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern Color GetColor();
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern float GetAlpha();
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetAlpha(float alpha);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetMaterial(Material material, Texture texture);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern Material GetMaterial();
		public void SetVertices(List<UIVertex> vertices)
		{
			if (vertices.Count > 65535)
			{
				Debug.LogWarning(UnityString.Format("Number of vertices set exceeds {0}, rendering of this element will be skipped", new object[]
				{
					65535
				}), this);
				vertices.Clear();
			}
			this.SetVerticesInternal(vertices);
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetVerticesInternal(object vertices);
		public void SetVertices(UIVertex[] vertices, int size)
		{
			if (size > 65535)
			{
				Debug.LogWarning(UnityString.Format("Number of vertices set exceeds {0}, rendering of this element will be skipped", new object[]
				{
					65535
				}), this);
				size = 0;
			}
			this.SetVerticesInternalArray(vertices, size);
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetVerticesInternalArray(UIVertex[] vertices, int size);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Clear();
		private static void RequestRefresh()
		{
			if (CanvasRenderer.onRequestRebuild != null)
			{
				CanvasRenderer.onRequestRebuild();
			}
		}
	}
}
