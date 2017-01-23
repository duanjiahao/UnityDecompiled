using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine.Internal;
using UnityEngine.Scripting;

namespace UnityEngine
{
	public sealed class Font : Object
	{
		[EditorBrowsable(EditorBrowsableState.Never)]
		public delegate void FontTextureRebuildCallback();

		public static event Action<Font> textureRebuilt
		{
			add
			{
				Action<Font> action = Font.textureRebuilt;
				Action<Font> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange<Action<Font>>(ref Font.textureRebuilt, (Action<Font>)Delegate.Combine(action2, value), action);
				}
				while (action != action2);
			}
			remove
			{
				Action<Font> action = Font.textureRebuilt;
				Action<Font> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange<Action<Font>>(ref Font.textureRebuilt, (Action<Font>)Delegate.Remove(action2, value), action);
				}
				while (action != action2);
			}
		}

		private event Font.FontTextureRebuildCallback m_FontTextureRebuildCallback
		{
			add
			{
				Font.FontTextureRebuildCallback fontTextureRebuildCallback = this.m_FontTextureRebuildCallback;
				Font.FontTextureRebuildCallback fontTextureRebuildCallback2;
				do
				{
					fontTextureRebuildCallback2 = fontTextureRebuildCallback;
					fontTextureRebuildCallback = Interlocked.CompareExchange<Font.FontTextureRebuildCallback>(ref this.m_FontTextureRebuildCallback, (Font.FontTextureRebuildCallback)Delegate.Combine(fontTextureRebuildCallback2, value), fontTextureRebuildCallback);
				}
				while (fontTextureRebuildCallback != fontTextureRebuildCallback2);
			}
			remove
			{
				Font.FontTextureRebuildCallback fontTextureRebuildCallback = this.m_FontTextureRebuildCallback;
				Font.FontTextureRebuildCallback fontTextureRebuildCallback2;
				do
				{
					fontTextureRebuildCallback2 = fontTextureRebuildCallback;
					fontTextureRebuildCallback = Interlocked.CompareExchange<Font.FontTextureRebuildCallback>(ref this.m_FontTextureRebuildCallback, (Font.FontTextureRebuildCallback)Delegate.Remove(fontTextureRebuildCallback2, value), fontTextureRebuildCallback);
				}
				while (fontTextureRebuildCallback != fontTextureRebuildCallback2);
			}
		}

		public extern Material material
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern string[] fontNames
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern CharacterInfo[] characterInfo
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[EditorBrowsable(EditorBrowsableState.Never), Obsolete("Font.textureRebuildCallback has been deprecated. Use Font.textureRebuilt instead.")]
		public Font.FontTextureRebuildCallback textureRebuildCallback
		{
			get
			{
				return this.m_FontTextureRebuildCallback;
			}
			set
			{
				this.m_FontTextureRebuildCallback = value;
			}
		}

		public extern bool dynamic
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern int ascent
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern int lineHeight
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern int fontSize
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public Font()
		{
			Font.Internal_CreateFont(this, null);
		}

		public Font(string name)
		{
			Font.Internal_CreateFont(this, name);
		}

		private Font(string[] names, int size)
		{
			Font.Internal_CreateDynamicFont(this, names, size);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string[] GetOSInstalledFontNames();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_CreateFont([Writable] Font _font, string name);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_CreateDynamicFont([Writable] Font _font, string[] _names, int size);

		public static Font CreateDynamicFontFromOSFont(string fontname, int size)
		{
			return new Font(new string[]
			{
				fontname
			}, size);
		}

		public static Font CreateDynamicFontFromOSFont(string[] fontnames, int size)
		{
			return new Font(fontnames, size);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool HasCharacter(char c);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void RequestCharactersInTexture(string characters, [UnityEngine.Internal.DefaultValue("0")] int size, [UnityEngine.Internal.DefaultValue("FontStyle.Normal")] FontStyle style);

		[ExcludeFromDocs]
		public void RequestCharactersInTexture(string characters, int size)
		{
			FontStyle style = FontStyle.Normal;
			this.RequestCharactersInTexture(characters, size, style);
		}

		[ExcludeFromDocs]
		public void RequestCharactersInTexture(string characters)
		{
			FontStyle style = FontStyle.Normal;
			int size = 0;
			this.RequestCharactersInTexture(characters, size, style);
		}

		[RequiredByNativeCode]
		private static void InvokeTextureRebuilt_Internal(Font font)
		{
			Action<Font> action = Font.textureRebuilt;
			if (action != null)
			{
				action(font);
			}
			if (font.m_FontTextureRebuildCallback != null)
			{
				font.m_FontTextureRebuildCallback();
			}
		}

		public static int GetMaxVertsForString(string str)
		{
			return str.Length * 4 + 4;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool GetCharacterInfo(char ch, out CharacterInfo info, [UnityEngine.Internal.DefaultValue("0")] int size, [UnityEngine.Internal.DefaultValue("FontStyle.Normal")] FontStyle style);

		[ExcludeFromDocs]
		public bool GetCharacterInfo(char ch, out CharacterInfo info, int size)
		{
			FontStyle style = FontStyle.Normal;
			return this.GetCharacterInfo(ch, out info, size, style);
		}

		[ExcludeFromDocs]
		public bool GetCharacterInfo(char ch, out CharacterInfo info)
		{
			FontStyle style = FontStyle.Normal;
			int size = 0;
			return this.GetCharacterInfo(ch, out info, size, style);
		}
	}
}
