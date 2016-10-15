using System;
using UnityEngine;

namespace UnityEditor
{
	internal class SpriteEditorTexturePostprocessor : AssetPostprocessor
	{
		public override int GetPostprocessOrder()
		{
			return 1;
		}

		public void OnPostprocessTexture(Texture2D tex)
		{
			if (SpriteEditorWindow.s_Instance != null && base.assetPath.Equals(SpriteEditorWindow.s_Instance.m_SelectedAssetPath))
			{
				if (!SpriteEditorWindow.s_Instance.m_IgnoreNextPostprocessEvent)
				{
					SpriteEditorWindow.s_Instance.m_ResetOnNextRepaint = true;
					SpriteEditorWindow.s_Instance.Repaint();
				}
				else
				{
					SpriteEditorWindow.s_Instance.m_IgnoreNextPostprocessEvent = false;
				}
			}
		}
	}
}
