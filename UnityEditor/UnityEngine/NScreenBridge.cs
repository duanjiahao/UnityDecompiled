using System;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public sealed class NScreenBridge : Object
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern NScreenBridge();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void InitServer(int id);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Update();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void StartWatchdogForPid(int pid);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetResolution(int x, int y);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetInput(int x, int y, int button, int key, int type);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void ResetInput();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern Texture2D GetScreenTexture();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Shutdown();
	}
}
