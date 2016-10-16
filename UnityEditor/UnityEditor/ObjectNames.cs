using System;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace UnityEditor
{
	public sealed class ObjectNames
	{
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string NicifyVariableName(string name);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string GetInspectorTitle(UnityEngine.Object obj);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string GetClassName(UnityEngine.Object obj);

		internal static string GetTypeName(UnityEngine.Object obj)
		{
			if (obj == null)
			{
				return "Object";
			}
			string text = AssetDatabase.GetAssetPath(obj).ToLower();
			if (text.EndsWith(".unity"))
			{
				return "Scene";
			}
			if (text.EndsWith(".guiskin"))
			{
				return "GUI Skin";
			}
			if (Directory.Exists(AssetDatabase.GetAssetPath(obj)))
			{
				return "Folder";
			}
			if (obj.GetType() == typeof(UnityEngine.Object))
			{
				return Path.GetExtension(text) + " File";
			}
			return ObjectNames.GetClassName(obj);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string GetDragAndDropTitle(UnityEngine.Object obj);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetNameSmart(UnityEngine.Object obj, string name);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetNameSmartWithInstanceID(int instanceID, string name);

		[Obsolete("Please use NicifyVariableName instead")]
		public static string MangleVariableName(string name)
		{
			return ObjectNames.NicifyVariableName(name);
		}

		[Obsolete("Please use GetInspectorTitle instead")]
		public static string GetPropertyEditorTitle(UnityEngine.Object obj)
		{
			return ObjectNames.GetInspectorTitle(obj);
		}
	}
}
