using System;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace UnityEditor
{
	public sealed class ObjectNames
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string NicifyVariableName(string name);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string GetInspectorTitle(UnityEngine.Object obj);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string GetClassName(UnityEngine.Object obj);

		internal static string GetTypeName(UnityEngine.Object obj)
		{
			string result;
			if (obj == null)
			{
				result = "Object";
			}
			else
			{
				string text = AssetDatabase.GetAssetPath(obj).ToLower();
				if (text.EndsWith(".unity"))
				{
					result = "Scene";
				}
				else if (text.EndsWith(".guiskin"))
				{
					result = "GUI Skin";
				}
				else if (Directory.Exists(AssetDatabase.GetAssetPath(obj)))
				{
					result = "Folder";
				}
				else if (obj.GetType() == typeof(UnityEngine.Object))
				{
					result = Path.GetExtension(text) + " File";
				}
				else
				{
					result = ObjectNames.GetClassName(obj);
				}
			}
			return result;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string GetDragAndDropTitle(UnityEngine.Object obj);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetNameSmart(UnityEngine.Object obj, string name);

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

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string GetUniqueName(string[] existingNames, string name);
	}
}
