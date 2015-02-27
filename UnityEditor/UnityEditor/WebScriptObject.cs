using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Internal;
namespace UnityEditor
{
	[StructLayout(LayoutKind.Sequential)]
	public sealed class WebScriptObject : IDisposable
	{
		private IntPtr webScriptObjectWrapper;
		public void Dispose()
		{
			this.DestroyScriptObject();
		}
		~WebScriptObject()
		{
			this.DestroyScriptObject();
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void DestroyScriptObject();
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern object MonoObjectFromJSON(string json);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern WebScriptObject EvalJavaScript(string script);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern WebScriptObject InvokeMethodArray(string method, object[] args);
		public WebScriptObject InvokeMethod(string method, params object[] args)
		{
			return this.InvokeMethodArray(method, args);
		}
		public WebScriptObject Invoke(params object[] args)
		{
			object[] array = new object[args.Length + 1];
			args.CopyTo(array, 1);
			array[0] = this;
			return this.InvokeMethodArray("call", array);
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern WebScriptObject Internal_GetKey(string key);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Internal_SetKey(string key, object value);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Remove(string key);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern WebScriptObject Internal_GetIndex(int i);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Internal_SetIndex(int i, object value);
		public WebScriptObject Get(int key)
		{
			return this.Internal_GetIndex(key);
		}
		public WebScriptObject Get(string key)
		{
			WebScriptObject webScriptObject = this;
			string[] array = key.Split(new char[]
			{
				'.'
			});
			for (int i = 0; i < array.Length; i++)
			{
				string key2 = array[i];
				webScriptObject = webScriptObject.Internal_GetKey(key2);
				if (webScriptObject == null)
				{
					break;
				}
			}
			return webScriptObject;
		}
		public T Get<T>(int key)
		{
			return this.Internal_GetIndex(key).Convert<T>();
		}
		public T Get<T>(string key)
		{
			return this.Get<WebScriptObject>(key).Convert<T>();
		}
		public void Set<T>(int key, T value)
		{
			this.Internal_SetIndex(key, value);
		}
		public void Set<T>(string key, T value)
		{
			this.Internal_SetKey(key, value);
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern object ConvertToType(Type t);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool IsValid();
		public T Convert<T>()
		{
			return (T)((object)this.ConvertToType(typeof(T)));
		}
		[ExcludeFromDocs]
		public string GetAsJSON(string key)
		{
			int indent = 0;
			return this.GetAsJSON(key, indent);
		}
		public string GetAsJSON(string key, [DefaultValue("0")] int indent)
		{
			return this.Internal_GetKey(key).ToJSON(indent);
		}
		[ExcludeFromDocs]
		public string GetAsJSON(int i)
		{
			int indent = 0;
			return this.GetAsJSON(i, indent);
		}
		public string GetAsJSON(int i, [DefaultValue("0")] int indent)
		{
			return this.Internal_GetIndex(i).ToJSON(indent);
		}
		public void SetFromJSON(string key, string value)
		{
			this.Internal_SetKey(key, this.ParseJSON(value));
		}
		public void SetFromJSON(int i, string value)
		{
			this.Internal_SetIndex(i, this.ParseJSON(value));
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern string ToJSON([DefaultValue("0")] int indent);
		[ExcludeFromDocs]
		public string ToJSON()
		{
			int indent = 0;
			return this.ToJSON(indent);
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public override extern string ToString();
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern WebScriptObject ParseJSON(string json);
		public static implicit operator bool(WebScriptObject o)
		{
			return o != null && o.IsValid() && o.Convert<bool>();
		}
		public static implicit operator int(WebScriptObject o)
		{
			return o.Convert<int>();
		}
		public static implicit operator float(WebScriptObject o)
		{
			return o.Convert<float>();
		}
		public static implicit operator double(WebScriptObject o)
		{
			return o.Convert<double>();
		}
		public static implicit operator string(WebScriptObject o)
		{
			if (o == null || !o.IsValid())
			{
				return string.Empty;
			}
			return o.ToString();
		}
	}
}
