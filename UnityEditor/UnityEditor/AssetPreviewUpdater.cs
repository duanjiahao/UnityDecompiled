using System;
using System.Reflection;
using UnityEngine;

namespace UnityEditor
{
	internal class AssetPreviewUpdater
	{
		public static Texture2D CreatePreviewForAsset(UnityEngine.Object obj, UnityEngine.Object[] subAssets, string assetPath)
		{
			if (obj == null)
			{
				return null;
			}
			Type type = CustomEditorAttributes.FindCustomEditorType(obj, false);
			if (type == null)
			{
				return null;
			}
			MethodInfo method = type.GetMethod("RenderStaticPreview");
			if (method == null)
			{
				Debug.LogError("Fail to find RenderStaticPreview base method");
				return null;
			}
			if (method.DeclaringType == typeof(Editor))
			{
				return null;
			}
			Editor editor = Editor.CreateEditor(obj);
			if (editor == null)
			{
				return null;
			}
			Texture2D result = editor.RenderStaticPreview(assetPath, subAssets, 128, 128);
			UnityEngine.Object.DestroyImmediate(editor);
			return result;
		}
	}
}
