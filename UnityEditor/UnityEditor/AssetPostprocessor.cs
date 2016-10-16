using System;
using UnityEngine;
using UnityEngine.Internal;

namespace UnityEditor
{
	public class AssetPostprocessor
	{
		private string m_PathName;

		public string assetPath
		{
			get
			{
				return this.m_PathName;
			}
			set
			{
				this.m_PathName = value;
			}
		}

		public AssetImporter assetImporter
		{
			get
			{
				return AssetImporter.GetAtPath(this.assetPath);
			}
		}

		[Obsolete("To set or get the preview, call EditorUtility.SetAssetPreview or AssetPreview.GetAssetPreview instead", true)]
		public Texture2D preview
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		[ExcludeFromDocs]
		public void LogWarning(string warning)
		{
			UnityEngine.Object context = null;
			this.LogWarning(warning, context);
		}

		public void LogWarning(string warning, [DefaultValue("null")] UnityEngine.Object context)
		{
			Debug.LogWarning(warning, context);
		}

		[ExcludeFromDocs]
		public void LogError(string warning)
		{
			UnityEngine.Object context = null;
			this.LogError(warning, context);
		}

		public void LogError(string warning, [DefaultValue("null")] UnityEngine.Object context)
		{
			Debug.LogError(warning, context);
		}

		public virtual uint GetVersion()
		{
			return 0u;
		}

		public virtual int GetPostprocessOrder()
		{
			return 0;
		}
	}
}
