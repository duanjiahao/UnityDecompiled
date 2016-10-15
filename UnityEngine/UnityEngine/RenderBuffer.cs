using System;
using UnityEngine.Rendering;

namespace UnityEngine
{
	public struct RenderBuffer
	{
		internal int m_RenderTextureInstanceID;

		internal IntPtr m_BufferPtr;

		internal RenderBufferLoadAction loadAction
		{
			get
			{
				return (RenderBufferLoadAction)RenderBufferHelper.GetLoadAction(out this);
			}
			set
			{
				this.SetLoadAction(value);
			}
		}

		internal RenderBufferStoreAction storeAction
		{
			get
			{
				return (RenderBufferStoreAction)RenderBufferHelper.GetStoreAction(out this);
			}
			set
			{
				this.SetStoreAction(value);
			}
		}

		internal void SetLoadAction(RenderBufferLoadAction action)
		{
			RenderBufferHelper.SetLoadAction(out this, (int)action);
		}

		internal void SetStoreAction(RenderBufferStoreAction action)
		{
			RenderBufferHelper.SetStoreAction(out this, (int)action);
		}

		public IntPtr GetNativeRenderBufferPtr()
		{
			return RenderBufferHelper.GetNativeRenderBufferPtr(this.m_BufferPtr);
		}
	}
}
