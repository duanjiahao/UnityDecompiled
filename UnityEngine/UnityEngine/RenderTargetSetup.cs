using System;
using UnityEngine.Rendering;

namespace UnityEngine
{
	public struct RenderTargetSetup
	{
		public RenderBuffer[] color;

		public RenderBuffer depth;

		public int mipLevel;

		public CubemapFace cubemapFace;

		public int depthSlice;

		public RenderBufferLoadAction[] colorLoad;

		public RenderBufferStoreAction[] colorStore;

		public RenderBufferLoadAction depthLoad;

		public RenderBufferStoreAction depthStore;

		public RenderTargetSetup(RenderBuffer[] color, RenderBuffer depth, int mip, CubemapFace face, RenderBufferLoadAction[] colorLoad, RenderBufferStoreAction[] colorStore, RenderBufferLoadAction depthLoad, RenderBufferStoreAction depthStore)
		{
			this.color = color;
			this.depth = depth;
			this.mipLevel = mip;
			this.cubemapFace = face;
			this.depthSlice = 0;
			this.colorLoad = colorLoad;
			this.colorStore = colorStore;
			this.depthLoad = depthLoad;
			this.depthStore = depthStore;
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

		public RenderTargetSetup(RenderBuffer color, RenderBuffer depth, int mipLevel, CubemapFace face, int depthSlice)
		{
			this = new RenderTargetSetup(new RenderBuffer[]
			{
				color
			}, depth, mipLevel, face);
			this.depthSlice = depthSlice;
		}

		public RenderTargetSetup(RenderBuffer[] color, RenderBuffer depth)
		{
			this = new RenderTargetSetup(color, depth, 0, CubemapFace.Unknown);
		}

		public RenderTargetSetup(RenderBuffer[] color, RenderBuffer depth, int mipLevel)
		{
			this = new RenderTargetSetup(color, depth, mipLevel, CubemapFace.Unknown);
		}

		public RenderTargetSetup(RenderBuffer[] color, RenderBuffer depth, int mip, CubemapFace face)
		{
			this = new RenderTargetSetup(color, depth, mip, face, RenderTargetSetup.LoadActions(color), RenderTargetSetup.StoreActions(color), depth.loadAction, depth.storeAction);
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
