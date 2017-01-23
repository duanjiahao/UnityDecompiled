using System;
using System.Runtime.CompilerServices;
using UnityEngine.Internal;

namespace UnityEngine
{
	public sealed class WebCamTexture : Texture
	{
		public extern bool isPlaying
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern string deviceName
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float requestedFPS
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern int requestedWidth
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern int requestedHeight
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[Obsolete("since Unity 5.0 iOS WebCamTexture always goes through CVTextureCache and is read to memory on-demand")]
		public extern bool isReadable
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern WebCamDevice[] devices
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern int videoRotationAngle
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern bool videoVerticallyMirrored
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern bool didUpdateThisFrame
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public WebCamTexture(string deviceName, int requestedWidth, int requestedHeight, int requestedFPS)
		{
			WebCamTexture.Internal_CreateWebCamTexture(this, deviceName, requestedWidth, requestedHeight, requestedFPS);
		}

		public WebCamTexture(string deviceName, int requestedWidth, int requestedHeight)
		{
			WebCamTexture.Internal_CreateWebCamTexture(this, deviceName, requestedWidth, requestedHeight, 0);
		}

		public WebCamTexture(string deviceName)
		{
			WebCamTexture.Internal_CreateWebCamTexture(this, deviceName, 0, 0, 0);
		}

		public WebCamTexture(int requestedWidth, int requestedHeight, int requestedFPS)
		{
			WebCamTexture.Internal_CreateWebCamTexture(this, "", requestedWidth, requestedHeight, requestedFPS);
		}

		public WebCamTexture(int requestedWidth, int requestedHeight)
		{
			WebCamTexture.Internal_CreateWebCamTexture(this, "", requestedWidth, requestedHeight, 0);
		}

		public WebCamTexture()
		{
			WebCamTexture.Internal_CreateWebCamTexture(this, "", 0, 0, 0);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_CreateWebCamTexture([Writable] WebCamTexture self, string scriptingDevice, int requestedWidth, int requestedHeight, int maxFramerate);

		public void Play()
		{
			WebCamTexture.INTERNAL_CALL_Play(this);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_Play(WebCamTexture self);

		public void Pause()
		{
			WebCamTexture.INTERNAL_CALL_Pause(this);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_Pause(WebCamTexture self);

		public void Stop()
		{
			WebCamTexture.INTERNAL_CALL_Stop(this);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_Stop(WebCamTexture self);

		[Obsolete("since Unity 5.0 iOS WebCamTexture always goes through CVTextureCache and is read to memory on-demand")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void MarkNonReadable();

		public Color GetPixel(int x, int y)
		{
			Color result;
			WebCamTexture.INTERNAL_CALL_GetPixel(this, x, y, out result);
			return result;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GetPixel(WebCamTexture self, int x, int y, out Color value);

		public Color[] GetPixels()
		{
			return this.GetPixels(0, 0, this.width, this.height);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern Color[] GetPixels(int x, int y, int blockWidth, int blockHeight);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern Color32[] GetPixels32([DefaultValue("null")] Color32[] colors);

		[ExcludeFromDocs]
		public Color32[] GetPixels32()
		{
			Color32[] colors = null;
			return this.GetPixels32(colors);
		}
	}
}
