using System;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Scripting;

namespace UnityEditor
{
	public sealed class ObjectNames
	{
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string NicifyVariableName(string name);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string GetInspectorTitle(UnityEngine.Object obj);

		[GeneratedByOldBindingsGenerator]
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

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string GetDragAndDropTitle(UnityEngine.Object obj);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetNameSmart(UnityEngine.Object obj, string name);

		[GeneratedByOldBindingsGenerator]
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

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string GetUniqueName(string[] existingNames, string name);
	}
}
