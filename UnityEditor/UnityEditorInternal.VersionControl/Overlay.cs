using System;
using UnityEditor;
using UnityEditor.VersionControl;
using UnityEngine;
namespace UnityEditorInternal.VersionControl
{
	public class Overlay
	{
		private static Asset m_Asset;
		private static Rect m_ItemRect;
		private static string m_IconPrefix;
		public static Rect GetOverlayRect(Rect itemRect)
		{
			if (itemRect.width > itemRect.height)
			{
				itemRect.x += 16f;
				itemRect.width = 20f;
			}
			else
			{
				itemRect.width = 12f;
			}
			itemRect.height = itemRect.width;
			return itemRect;
		}
		public static void DrawOverlay(Asset asset, Rect itemRect)
		{
			if (asset == null)
			{
				return;
			}
			if (Event.current.type != EventType.Repaint)
			{
				return;
			}
			Overlay.m_Asset = asset;
			Overlay.m_ItemRect = itemRect;
			Overlay.m_IconPrefix = EditorSettings.externalVersionControl;
			if (Overlay.m_IconPrefix == ExternalVersionControl.Disabled || Overlay.m_IconPrefix == ExternalVersionControl.AutoDetect || Overlay.m_IconPrefix == ExternalVersionControl.Generic || Overlay.m_IconPrefix == ExternalVersionControl.AssetServer)
			{
				return;
			}
			Overlay.DrawOverlays();
		}
		private static void DrawOverlay(Asset.States state, Rect iconRect)
		{
			Rect atlasRectForState = Provider.GetAtlasRectForState((int)state);
			if (atlasRectForState.width == 0f)
			{
				return;
			}
			Texture2D overlayAtlas = Provider.overlayAtlas;
			if (overlayAtlas == null)
			{
				return;
			}
			GUI.DrawTextureWithTexCoords(iconRect, overlayAtlas, atlasRectForState);
		}
		private static void DrawOverlays()
		{
			float num = 16f;
			float num2 = 1f;
			float num3 = 4f;
			float num4 = -4f;
			Rect iconRect = new Rect(Overlay.m_ItemRect.x - num2, Overlay.m_ItemRect.y - num3, num, num);
			Rect iconRect2 = new Rect(Overlay.m_ItemRect.xMax - num + num2, Overlay.m_ItemRect.y - num3, num, num);
			Rect iconRect3 = new Rect(Overlay.m_ItemRect.x - num2, Overlay.m_ItemRect.yMax - num + num3, num, num);
			Rect iconRect4 = new Rect(Overlay.m_ItemRect.xMax - num + num4, Overlay.m_ItemRect.yMax - num + num3, num, num);
			if (Overlay.IsState(Asset.States.AddedLocal))
			{
				Overlay.DrawOverlay(Asset.States.AddedLocal, iconRect);
			}
			if (Overlay.IsState(Asset.States.AddedRemote))
			{
				Overlay.DrawOverlay(Asset.States.AddedRemote, iconRect2);
			}
			if (Overlay.IsState(Asset.States.CheckedOutLocal) && !Overlay.IsState(Asset.States.LockedLocal) && !Overlay.IsState(Asset.States.AddedLocal))
			{
				Overlay.DrawOverlay(Asset.States.CheckedOutLocal, iconRect);
			}
			if (Overlay.IsState(Asset.States.CheckedOutRemote) && !Overlay.IsState(Asset.States.LockedRemote) && !Overlay.IsState(Asset.States.AddedRemote))
			{
				Overlay.DrawOverlay(Asset.States.CheckedOutRemote, iconRect2);
			}
			if (Overlay.IsState(Asset.States.DeletedLocal))
			{
				Overlay.DrawOverlay(Asset.States.DeletedLocal, iconRect);
			}
			if (Overlay.IsState(Asset.States.DeletedRemote))
			{
				Overlay.DrawOverlay(Asset.States.DeletedRemote, iconRect2);
			}
			if (Overlay.IsState(Asset.States.Local) && !Overlay.IsState(Asset.States.OutOfSync) && !Overlay.IsState(Asset.States.Synced) && !Overlay.IsState(Asset.States.AddedLocal))
			{
				Overlay.DrawOverlay(Asset.States.Local, iconRect3);
			}
			if (Overlay.IsState(Asset.States.LockedLocal))
			{
				Overlay.DrawOverlay(Asset.States.LockedLocal, iconRect);
			}
			if (Overlay.IsState(Asset.States.LockedRemote))
			{
				Overlay.DrawOverlay(Asset.States.LockedRemote, iconRect2);
			}
			if (Overlay.IsState(Asset.States.OutOfSync))
			{
				Overlay.DrawOverlay(Asset.States.OutOfSync, iconRect4);
			}
			if (Overlay.IsState(Asset.States.Conflicted))
			{
				Overlay.DrawOverlay(Asset.States.Conflicted, iconRect3);
			}
		}
		private static bool IsState(Asset.States state)
		{
			return Overlay.m_Asset.IsState(state);
		}
	}
}
