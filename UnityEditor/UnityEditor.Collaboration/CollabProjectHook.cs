using System;
using UnityEditor.Connect;
using UnityEditor.Web;
using UnityEngine;

namespace UnityEditor.Collaboration
{
	internal class CollabProjectHook
	{
		public static void OnProjectWindowItemIconOverlay(string guid, Rect drawRect)
		{
			bool flag = CollabAccess.Instance.IsServiceEnabled();
			if (flag && UnityConnect.instance.userInfo.whitelisted)
			{
				Collab instance = Collab.instance;
				if (instance.collabInfo.whitelisted)
				{
					Collab.CollabStates assetState = instance.GetAssetState(guid);
					Overlay.DrawOverlays(assetState, drawRect);
				}
			}
		}
	}
}
