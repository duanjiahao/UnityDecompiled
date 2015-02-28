using System;
using UnityEngine.Rendering;
namespace UnityEngine
{
	internal struct RenderTargetSetup
	{
		public RenderBuffer[] color;
		public RenderBuffer depth;
		public int mipLevel;
		public int cubemapFace;
		public RenderBufferLoadAction[] colorLoad;
		public RenderBufferStoreAction[] colorStore;
		public RenderBufferLoadAction depthLoad;
		public RenderBufferStoreAction depthStore;
		internal RenderTargetSetup(RenderBuffer[] color, RenderBuffer depth, int mip, int face, RenderBufferLoadAction[] colorLoad, RenderBufferStoreAction[] colorStore, RenderBufferLoadAction depthLoad, RenderBufferStoreAction depthStore)
		{
			this.color = color;
			this.depth = depth;
			this.mipLevel = mip;
			this.cubemapFace = face;
			this.colorLoad = colorLoad;
			this.colorStore = colorStore;
			this.depthLoad = depthLoad;
			this.depthStore = depthStore;
		}
		internal RenderTargetSetup(RenderBuffer[] color, RenderBuffer depth, int mip, int face)
		{
			this = new RenderTargetSetup(color, depth, mip, face, RenderTargetSetup.LoadActions(color), RenderTargetSetup.StoreActions(color), depth.loadAction, depth.storeAction);
		}
		public RenderTargetSetup(RenderBuffer color, RenderBuffer depth)
		{
			this = new RenderTargetSetup(new RenderBuffer[]
			{
				color
			}, depth);
		}
		public RenderTargetSetup(RenderBuffer color, RenderBuffer depth, int mipLevel)
		{
			this = new RenderTargetSetup(new RenderBuffer[]
			{
				color
			}, depth, mipLevel);
		}
		public RenderTargetSetup(RenderBuffer color, RenderBuffer depth, int mipLevel, CubemapFace face)
		{
			this = new RenderTargetSetup(new RenderBuffer[]
			{
				color
			}, depth, mipLevel, face);
		}
		public RenderTargetSetup(RenderBuffer[] color, RenderBuffer depth)
		{
			this = new RenderTargetSetup(color, depth, 0, -1);
		}
		public RenderTargetSetup(RenderBuffer[] color, RenderBuffer depth, int mipLevel)
		{
			this = new RenderTargetSetup(color, depth, mipLevel, -1);
		}
		public RenderTargetSetup(RenderBuffer[] color, RenderBuffer depth, int mipLevel, CubemapFace face)
		{
			this = new RenderTargetSetup(color, depth, mipLevel, (int)face);
		}
		internal static RenderBufferLoadAction[] LoadActions(RenderBuffer[] buf)
		{
			RenderBufferLoadAction[] array = new RenderBufferLoadAction[buf.Length];
			for (int i = 0; i < buf.Length; i++)
			{
				array[i] = buf[i].loadAction;
				buf[i].loadAction = RenderBufferLoadAction.Load;
			}
			return array;
		}
		internal static RenderBufferStoreAction[] StoreActions(RenderBuffer[] buf)
		{
			RenderBufferStoreAction[] array = new RenderBufferStoreAction[buf.Length];
			for (int i = 0; i < buf.Length; i++)
			{
				array[i] = buf[i].storeAction;
				buf[i].storeAction = RenderBufferStoreAction.Store;
			}
			return array;
		}
	}
}
