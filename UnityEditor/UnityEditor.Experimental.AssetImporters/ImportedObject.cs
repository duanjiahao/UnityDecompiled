using System;
using UnityEngine;

namespace UnityEditor.Experimental.AssetImporters
{
	internal class ImportedObject
	{
		public bool mainAsset
		{
			get;
			set;
		}

		public UnityEngine.Object asset
		{
			get;
			set;
		}

		public string identifier
		{
			get;
			set;
		}

		public Texture2D thumbnail
		{
			get;
			set;
		}
	}
}
