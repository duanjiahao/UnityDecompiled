using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
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
			[MethodImpl(MethodImplOptions.Synchronized)]
			add
			{
				Font.textureRebuilt = (Action<Font>)Delegate.Combine(Font.textureRebuilt, value);
			}
			[MethodImpl(MethodImplOptions.Synchronized)]
			remove
			{
				Font.textureRebuilt = (Action<Font>)Delegate.Remove(Font.textureRebuilt, value);
			}
		}

		private event Font.FontTextureRebuildCallback m_FontTextureRebuildCallback
		{
			[MethodImpl(MethodImplOptions.Synchronized)]
			add
			{
				this.m_FontTextureRebuildCallback = (Font.FontTextureRebuildCallback)Delegate.Combine(this.m_FontTextureRebuildCallback, value);
			}
			[MethodImpl(MethodImplOptions.Synchronized)]
			remove
			{
				this.m_FontTextureRebuildCallback = (Font.FontTextureRebuildCallback)Delegate.Remove(this.m_FontTextureRebuildCallback, value);
			}
		}

		public extern Material material
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern string[] fontNames
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern CharacterInfo[] characterInfo
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
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
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern int ascent
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern int lineHeight
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern int fontSize
		{
			[WrapperlessIcall]
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

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string[] GetOSInstalledFontNames();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_CreateFont([Writable] Font _font, string name);

		[WrapperlessIcall]
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

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool HasCharacter(char c);

		[WrapperlessIcall]
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

		[WrapperlessIcall]
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
