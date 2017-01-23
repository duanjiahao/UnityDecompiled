using System;
using System.IO;
using UnityEngine;

namespace UnityEditor.Collaboration
{
	internal static class TextureUtility
	{
		public static Texture2D LoadTextureFromApplicationContents(string path)
		{
			Texture2D texture2D = new Texture2D(2, 2);
			string path2 = Path.Combine(Path.Combine(Path.Combine(EditorApplication.applicationContentsPath, "Resources"), "Collab"), "overlays");
			path = Path.Combine(path2, path);
			Texture2D result;
			try
			{
				FileStream fileStream = File.OpenRead(path);
				byte[] array = new byte[fileStream.Length];
				fileStream.Read(array, 0, (int)fileStream.Length);
				if (!texture2D.LoadImage(array))
				{
					result = null;
					return result;
				}
			}
			catch (Exception)
			{
				Debug.LogWarning("Collab Overlay Texture load fail, path: " + path);
				result = null;
				return result;
			}
			result = texture2D;
			return result;
		}
	}
}
