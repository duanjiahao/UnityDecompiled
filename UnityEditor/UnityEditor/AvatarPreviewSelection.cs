using System;
using UnityEngine;

namespace UnityEditor
{
	internal class AvatarPreviewSelection : ScriptableSingleton<AvatarPreviewSelection>
	{
		[SerializeField]
		private GameObject[] m_PreviewModels;

		private void Awake()
		{
			int num = 4;
			if (this.m_PreviewModels == null || this.m_PreviewModels.Length != num)
			{
				this.m_PreviewModels = new GameObject[num];
			}
		}

		public static void SetPreview(ModelImporterAnimationType type, GameObject go)
		{
			if (!Enum.IsDefined(typeof(ModelImporterAnimationType), type))
			{
				return;
			}
			if (ScriptableSingleton<AvatarPreviewSelection>.instance.m_PreviewModels[(int)type] != go)
			{
				ScriptableSingleton<AvatarPreviewSelection>.instance.m_PreviewModels[(int)type] = go;
				ScriptableSingleton<AvatarPreviewSelection>.instance.Save(false);
			}
		}

		public static GameObject GetPreview(ModelImporterAnimationType type)
		{
			if (!Enum.IsDefined(typeof(ModelImporterAnimationType), type))
			{
				return null;
			}
			return ScriptableSingleton<AvatarPreviewSelection>.instance.m_PreviewModels[(int)type];
		}
	}
}
