using System;
using UnityEngine;

namespace UnityEditor.Web
{
	internal class PreviewGenerator
	{
		private const string kPreviewBuildFolder = "builds";

		protected static PreviewGenerator s_Instance = null;

		public static PreviewGenerator GetInstance()
		{
			PreviewGenerator result;
			if (PreviewGenerator.s_Instance == null)
			{
				result = new PreviewGenerator();
			}
			else
			{
				result = PreviewGenerator.s_Instance;
			}
			return result;
		}

		public byte[] GeneratePreview(string assetPath, int width, int height)
		{
			UnityEngine.Object @object = AssetDatabase.LoadMainAssetAtPath(assetPath);
			byte[] result;
			if (@object == null)
			{
				result = null;
			}
			else
			{
				Editor editor = Editor.CreateEditor(@object);
				if (editor == null)
				{
					result = null;
				}
				else
				{
					Texture2D texture2D = editor.RenderStaticPreview(assetPath, null, width, height);
					if (texture2D == null)
					{
						UnityEngine.Object.DestroyImmediate(editor);
						result = null;
					}
					else
					{
						byte[] array = texture2D.EncodeToPNG();
						UnityEngine.Object.DestroyImmediate(texture2D);
						UnityEngine.Object.DestroyImmediate(editor);
						result = array;
					}
				}
			}
			return result;
		}
	}
}
