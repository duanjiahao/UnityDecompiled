using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnityEditor.Experimental.AssetImporters
{
	public class AssetImportContext
	{
		private List<ImportedObject> m_SubAssets = new List<ImportedObject>();

		public string assetPath
		{
			get;
			internal set;
		}

		public BuildTarget selectedBuildTarget
		{
			get;
			internal set;
		}

		internal List<ImportedObject> subAssets
		{
			get
			{
				return this.m_SubAssets;
			}
		}

		internal AssetImportContext()
		{
		}

		public void SetMainAsset(string identifier, UnityEngine.Object asset)
		{
			this.AddAsset(true, identifier, asset, null);
		}

		public void SetMainAsset(string identifier, UnityEngine.Object asset, Texture2D thumbnail)
		{
			this.AddAsset(true, identifier, asset, thumbnail);
		}

		public void AddSubAsset(string identifier, UnityEngine.Object asset)
		{
			this.AddAsset(false, identifier, asset, null);
		}

		public void AddSubAsset(string identifier, UnityEngine.Object asset, Texture2D thumbnail)
		{
			this.AddAsset(false, identifier, asset, thumbnail);
		}

		private void AddAsset(bool main, string identifier, UnityEngine.Object asset, Texture2D thumbnail)
		{
			if (asset == null)
			{
				throw new ArgumentNullException("asset", "Cannot add a null asset : " + (identifier ?? "<null>"));
			}
			ImportedObject importedObject = this.m_SubAssets.FirstOrDefault((ImportedObject x) => x.mainAsset);
			if (main && importedObject != null)
			{
				throw new Exception(string.Format("A Main asset has already been added and only one is allowed: \"{0}\" conflicting on \"{1}\" and \"{2}\"", this.assetPath, importedObject.identifier, identifier));
			}
			ImportedObject item = new ImportedObject
			{
				mainAsset = main,
				identifier = identifier,
				asset = asset,
				thumbnail = thumbnail
			};
			if (main)
			{
				this.m_SubAssets.Insert(0, item);
			}
			else
			{
				this.m_SubAssets.Add(item);
			}
		}
	}
}
