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
			Overlay.DrawOverlay(Provider.GetAssetByGUID(guid), drawRect);
		}
		public static Rect GetOverlayRect(Rect drawRect)
		{
			return Overlay.GetOverlayRect(drawRect);
		}
	}
}
