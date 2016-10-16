using System;
using UnityEngine;

namespace UnityEditor.Web
{
	internal class PreviewGenerator
	{
		private const string kPreviewBuildFolder = "builds";

		protected static PreviewGenerator s_Instance;

		public static PreviewGenerator GetInstance()
		{
			if (PreviewGenerator.s_Instance == null)
			{
				return new PreviewGenerator();
			}
			return PreviewGenerator.s_Instance;
		}

		public byte[] GeneratePreview(string assetPath, int width, int height)
		{
			UnityEngine.Object @object = AssetDatabase.LoadMainAssetAtPath(assetPath);
			if (@object == null)
			{
				return null;
			}
			Editor editor = Editor.CreateEditor(@object);
			if (editor == null)
			{
				return null;
			}
			Texture2D texture2D = editor.RenderStaticPreview(assetPath, null, width, height);
			if (texture2D == null)
			{
				UnityEngine.Object.DestroyImmediate(editor);
				return null;
			}
			byte[] result = texture2D.EncodeToPNG();
			UnityEngine.Object.DestroyImmediate(texture2D);
			UnityEngine.Object.DestroyImmediate(editor);
			return result;
		}
	}
}
