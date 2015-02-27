using System;
using System.Runtime.CompilerServices;
using UnityEngine.Internal;
namespace UnityEngine
{
	[Obsolete("iPhoneKeyboard class is deprecated. Please use TouchScreenKeyboard instead.")]
	public sealed class iPhoneKeyboard
	{
		private IntPtr keyboardWrapper;
		public extern string text
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
		public static extern bool hideInput
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
		public extern bool active
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
		public extern bool done
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		public static extern Rect area
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		public static extern bool visible
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		public iPhoneKeyboard(string text, iPhoneKeyboardType keyboardType, bool autocorrection, bool multiline, bool secure, bool alert, string textPlaceholder)
		{
			this.iPhoneKeyboard_InternalConstructorHelper(new iPhoneKeyboard_InternalConstructorHelperArguments
			{
				text = text,
				keyboardType = Convert.ToUInt32(keyboardType),
				autocorrection = Convert.ToUInt32(autocorrection),
				multiline = Convert.ToUInt32(multiline),
				secure = Convert.ToUInt32(secure),
				alert = Convert.ToUInt32(alert),
				textPlaceholder = textPlaceholder
			});
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Destroy();
		~iPhoneKeyboard()
		{
			this.Destroy();
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void iPhoneKeyboard_InternalConstructorHelper(iPhoneKeyboard_InternalConstructorHelperArguments arguments);
		[ExcludeFromDocs]
		public static iPhoneKeyboard Open(string text, iPhoneKeyboardType keyboardType, bool autocorrection, bool multiline, bool secure, bool alert)
		{
			string empty = string.Empty;
			return iPhoneKeyboard.Open(text, keyboardType, autocorrection, multiline, secure, alert, empty);
		}
		[ExcludeFromDocs]
		public static iPhoneKeyboard Open(string text, iPhoneKeyboardType keyboardType, bool autocorrection, bool multiline, bool secure)
		{
			string empty = string.Empty;
			bool alert = false;
			return iPhoneKeyboard.Open(text, keyboardType, autocorrection, multiline, secure, alert, empty);
		}
		[ExcludeFromDocs]
		public static iPhoneKeyboard Open(string text, iPhoneKeyboardType keyboardType, bool autocorrection, bool multiline)
		{
			string empty = string.Empty;
			bool alert = false;
			bool secure = false;
			return iPhoneKeyboard.Open(text, keyboardType, autocorrection, multiline, secure, alert, empty);
		}
		[ExcludeFromDocs]
		public static iPhoneKeyboard Open(string text, iPhoneKeyboardType keyboardType, bool autocorrection)
		{
			string empty = string.Empty;
			bool alert = false;
			bool secure = false;
			bool multiline = false;
			return iPhoneKeyboard.Open(text, keyboardType, autocorrection, multiline, secure, alert, empty);
		}
		[ExcludeFromDocs]
		public static iPhoneKeyboard Open(string text, iPhoneKeyboardType keyboardType)
		{
			string empty = string.Empty;
			bool alert = false;
			bool secure = false;
			bool multiline = false;
			bool autocorrection = true;
			return iPhoneKeyboard.Open(text, keyboardType, autocorrection, multiline, secure, alert, empty);
		}
		[ExcludeFromDocs]
		public static iPhoneKeyboard Open(string text)
		{
			string empty = string.Empty;
			bool alert = false;
			bool secure = false;
			bool multiline = false;
			bool autocorrection = true;
			iPhoneKeyboardType keyboardType = iPhoneKeyboardType.Default;
			return iPhoneKeyboard.Open(text, keyboardType, autocorrection, multiline, secure, alert, empty);
		}
		public static iPhoneKeyboard Open(string text, [DefaultValue("iPhoneKeyboardType.Default")] iPhoneKeyboardType keyboardType, [DefaultValue("true")] bool autocorrection, [DefaultValue("false")] bool multiline, [DefaultValue("false")] bool secure, [DefaultValue("false")] bool alert, [DefaultValue("\"\"")] string textPlaceholder)
		{
			return new iPhoneKeyboard(text, keyboardType, autocorrection, multiline, secure, alert, textPlaceholder);
		}
	}
}
