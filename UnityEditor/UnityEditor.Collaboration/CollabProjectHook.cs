using System;
using UnityEditor.Web;
using UnityEngine;

namespace UnityEditor.Collaboration
{
	internal class CollabProjectHook
	{
		public static void OnProjectWindowItemIconOverlay(string guid, Rect drawRect)
		{
			bool flag = CollabAccess.Instance.IsServiceEnabled();
			if (flag)
			{
				Collab instance = Collab.instance;
				Collab.CollabStates assetState = instance.GetAssetState(guid);
				Overlay.DrawOverlays(assetState, drawRect);
			}
		}
	}
}
