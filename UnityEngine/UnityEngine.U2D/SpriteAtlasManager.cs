using System;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine.Scripting;

namespace UnityEngine.U2D
{
	public sealed class SpriteAtlasManager
	{
		public delegate void RequestAtlasCallback(string tag, Action<SpriteAtlas> action);

		[CompilerGenerated]
		private static Action<SpriteAtlas> <>f__mg$cache0;

		public static event SpriteAtlasManager.RequestAtlasCallback atlasRequested
		{
			add
			{
				SpriteAtlasManager.RequestAtlasCallback requestAtlasCallback = SpriteAtlasManager.atlasRequested;
				SpriteAtlasManager.RequestAtlasCallback requestAtlasCallback2;
				do
				{
					requestAtlasCallback2 = requestAtlasCallback;
					requestAtlasCallback = Interlocked.CompareExchange<SpriteAtlasManager.RequestAtlasCallback>(ref SpriteAtlasManager.atlasRequested, (SpriteAtlasManager.RequestAtlasCallback)Delegate.Combine(requestAtlasCallback2, value), requestAtlasCallback);
				}
				while (requestAtlasCallback != requestAtlasCallback2);
			}
			remove
			{
				SpriteAtlasManager.RequestAtlasCallback requestAtlasCallback = SpriteAtlasManager.atlasRequested;
				SpriteAtlasManager.RequestAtlasCallback requestAtlasCallback2;
				do
				{
					requestAtlasCallback2 = requestAtlasCallback;
					requestAtlasCallback = Interlocked.CompareExchange<SpriteAtlasManager.RequestAtlasCallback>(ref SpriteAtlasManager.atlasRequested, (SpriteAtlasManager.RequestAtlasCallback)Delegate.Remove(requestAtlasCallback2, value), requestAtlasCallback);
				}
				while (requestAtlasCallback != requestAtlasCallback2);
			}
		}

		[RequiredByNativeCode]
		private static bool RequestAtlas(string tag)
		{
			bool result;
			if (SpriteAtlasManager.atlasRequested != null)
			{
				SpriteAtlasManager.RequestAtlasCallback arg_2F_0 = SpriteAtlasManager.atlasRequested;
				if (SpriteAtlasManager.<>f__mg$cache0 == null)
				{
					SpriteAtlasManager.<>f__mg$cache0 = new Action<SpriteAtlas>(SpriteAtlasManager.Register);
				}
				arg_2F_0(tag, SpriteAtlasManager.<>f__mg$cache0);
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void Register(SpriteAtlas spriteAtlas);

		static SpriteAtlasManager()
		{
			// Note: this type is marked as 'beforefieldinit'.
			SpriteAtlasManager.atlasRequested = null;
		}
	}
}
