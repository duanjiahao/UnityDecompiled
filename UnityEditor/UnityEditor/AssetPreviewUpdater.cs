using System;
using System.Reflection;
using UnityEngine;

namespace UnityEditor
{
	internal class AssetPreviewUpdater
	{
		public static Texture2D CreatePreviewForAsset(UnityEngine.Object obj, UnityEngine.Object[] subAssets, string assetPath)
		{
			Texture2D result;
			if (obj == null)
			{
				result = null;
			}
			else
			{
				Type type = CustomEditorAttributes.FindCustomEditorType(obj, false);
				if (type == null)
				{
					result = null;
				}
				else
				{
					MethodInfo method = type.GetMethod("RenderStaticPreview");
					if (method == null)
					{
						Debug.LogError("Fail to find RenderStaticPreview base method");
						result = null;
					}
					else if (method.DeclaringType == typeof(Editor))
					{
						result = null;
					}
					else
					{
						Editor editor = Editor.CreateEditor(obj);
						if (editor == null)
						{
							result = null;
						}
						else
						{
							Texture2D texture2D = editor.RenderStaticPreview(assetPath, subAssets, 128, 128);
							UnityEngine.Object.DestroyImmediate(editor);
							result = texture2D;
						}
					}
				}
			}
			return result;
		}
	}
}
