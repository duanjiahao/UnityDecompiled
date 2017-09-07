using System;
using UnityEditor.Web;
using UnityEngine;

namespace UnityEditor.Collaboration
{
	internal class CollabProjectHook
	{
		public static void OnProjectWindowIconOverlay(Rect iconRect, string guid, bool isListMode)
		{
			CollabProjectHook.DrawProjectBrowserIconOverlay(iconRect, guid, isListMode);
		}

		public static void OnProjectBrowserNavPanelIconOverlay(Rect iconRect, string guid)
		{
			CollabProjectHook.DrawProjectBrowserIconOverlay(iconRect, guid, true);
		}

		private static void DrawProjectBrowserIconOverlay(Rect iconRect, string guid, bool isListMode)
		{
			if (CollabAccess.Instance.IsServiceEnabled())
			{
				Collab.CollabStates assetState = CollabProjectHook.GetAssetState(guid);
				Overlay.DrawOverlays(assetState, iconRect, isListMode);
			}
		}

		public static Collab.CollabStates GetAssetState(string assetGuid)
		{
			Collab.CollabStates result;
			if (!CollabAccess.Instance.IsServiceEnabled())
			{
				result = Collab.CollabStates.kCollabNone;
			}
			else
			{
				Collab.CollabStates assetState = Collab.instance.GetAssetState(assetGuid);
				result = assetState;
			}
			return result;
		}
	}
}
