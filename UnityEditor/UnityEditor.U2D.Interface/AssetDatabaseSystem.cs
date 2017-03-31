using System;
using UnityEngine;

namespace UnityEditor.U2D.Interface
{
	internal class AssetDatabaseSystem : IAssetDatabase
	{
		public string GetAssetPath(UnityEngine.Object o)
		{
			return AssetDatabase.GetAssetPath(o);
		}

		public ITextureImporter GetAssetImporterFromPath(string path)
		{
			AssetImporter assetImporter = AssetImporter.GetAtPath(path) as UnityEditor.TextureImporter;
			return (!(assetImporter == null)) ? new TextureImporter((UnityEditor.TextureImporter)assetImporter) : null;
		}
	}
}
