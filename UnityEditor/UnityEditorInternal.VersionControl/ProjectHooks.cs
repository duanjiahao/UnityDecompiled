using System;
using UnityEditor.VersionControl;
using UnityEngine;

namespace UnityEditorInternal.VersionControl
{
	internal class ProjectHooks
	{
		public static void OnProjectWindowItem(string guid, Rect drawRect)
		{
			if (!Provider.isActive)
			{
				return;
			}
			Asset assetByGUID = Provider.GetAssetByGUID(guid);
			if (assetByGUID != null)
			{
				string unityPath = assetByGUID.path.Trim(new char[]
				{
					'/'
				}) + ".meta";
				Asset assetByPath = Provider.GetAssetByPath(unityPath);
				Overlay.DrawOverlay(assetByGUID, assetByPath, drawRect);
			}
		}

		public static Rect GetOverlayRect(Rect drawRect)
		{
			return Overlay.GetOverlayRect(drawRect);
		}
	}
}
