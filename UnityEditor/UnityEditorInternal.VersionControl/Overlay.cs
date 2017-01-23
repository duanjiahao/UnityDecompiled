using System;
using UnityEditor;
using UnityEditor.VersionControl;
using UnityEngine;

namespace UnityEditorInternal.VersionControl
{
	public class Overlay
	{
		private static Texture2D s_BlueLeftParan;

		private static Texture2D s_BlueRightParan;

		private static Texture2D s_RedLeftParan;

		private static Texture2D s_RedRightParan;

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
			if (asset != null)
			{
				if (Event.current.type == EventType.Repaint)
				{
					string externalVersionControl = EditorSettings.externalVersionControl;
					if (!(externalVersionControl == ExternalVersionControl.Disabled) && !(externalVersionControl == ExternalVersionControl.AutoDetect) && !(externalVersionControl == ExternalVersionControl.Generic) && !(externalVersionControl == ExternalVersionControl.AssetServer))
					{
						Overlay.DrawOverlays(asset, null, itemRect);
					}
				}
			}
		}

		public static void DrawOverlay(Asset asset, Asset metaAsset, Rect itemRect)
		{
			if (asset != null && metaAsset != null)
			{
				if (Event.current.type == EventType.Repaint)
				{
					string externalVersionControl = EditorSettings.externalVersionControl;
					if (!(externalVersionControl == ExternalVersionControl.Disabled) && !(externalVersionControl == ExternalVersionControl.AutoDetect) && !(externalVersionControl == ExternalVersionControl.Generic) && !(externalVersionControl == ExternalVersionControl.AssetServer))
					{
						Overlay.DrawOverlays(asset, metaAsset, itemRect);
					}
				}
			}
		}

		private static void DrawMetaOverlay(Rect iconRect, bool isRemote)
		{
			iconRect.y -= 1f;
			if (isRemote)
			{
				iconRect.x -= 5f;
				GUI.DrawTexture(iconRect, Overlay.s_BlueLeftParan);
				iconRect.x += 8f;
				GUI.DrawTexture(iconRect, Overlay.s_BlueRightParan);
			}
			else
			{
				iconRect.x -= 5f;
				GUI.DrawTexture(iconRect, Overlay.s_RedLeftParan);
				iconRect.x += 8f;
				GUI.DrawTexture(iconRect, Overlay.s_RedRightParan);
			}
		}

		private static void DrawOverlay(Asset.States state, Rect iconRect)
		{
			Rect atlasRectForState = Provider.GetAtlasRectForState((int)state);
			if (atlasRectForState.width != 0f)
			{
				Texture2D overlayAtlas = Provider.overlayAtlas;
				if (!(overlayAtlas == null))
				{
					GUI.DrawTextureWithTexCoords(iconRect, overlayAtlas, atlasRectForState);
				}
			}
		}

		private static void DrawOverlays(Asset asset, Asset metaAsset, Rect itemRect)
		{
			Overlay.CreateStaticResources();
			float num = 16f;
			float num2 = 1f;
			float num3 = 4f;
			float num4 = -4f;
			Rect iconRect = new Rect(itemRect.x - num2, itemRect.y - num3, num, num);
			Rect iconRect2 = new Rect(itemRect.xMax - num + num2, itemRect.y - num3, num, num);
			Rect iconRect3 = new Rect(itemRect.x - num2, itemRect.yMax - num + num3, num, num);
			Rect iconRect4 = new Rect(itemRect.xMax - num + num4, itemRect.yMax - num + num3, num, num);
			Asset.States states = Asset.States.Local | Asset.States.Synced | Asset.States.ReadOnly | Asset.States.MetaFile;
			bool flag = metaAsset == null || (metaAsset.state & states) == states;
			Asset.States states2 = (metaAsset != null) ? (metaAsset.state & (Asset.States.CheckedOutLocal | Asset.States.DeletedLocal | Asset.States.AddedLocal | Asset.States.LockedLocal)) : Asset.States.None;
			Asset.States states3 = (metaAsset != null) ? (metaAsset.state & (Asset.States.CheckedOutRemote | Asset.States.DeletedRemote | Asset.States.AddedRemote | Asset.States.LockedRemote)) : Asset.States.None;
			bool flag2 = asset.isFolder && Provider.isVersioningFolders;
			if (asset.IsState(Asset.States.AddedLocal))
			{
				Overlay.DrawOverlay(Asset.States.AddedLocal, iconRect);
				if (metaAsset != null && (states2 & Asset.States.AddedLocal) == Asset.States.None && !flag)
				{
					Overlay.DrawMetaOverlay(iconRect, false);
				}
			}
			else if (asset.IsState(Asset.States.DeletedLocal))
			{
				Overlay.DrawOverlay(Asset.States.DeletedLocal, iconRect);
				if (metaAsset != null && (states2 & Asset.States.DeletedLocal) == Asset.States.None && metaAsset.IsState(Asset.States.Local | Asset.States.Missing))
				{
					Overlay.DrawMetaOverlay(iconRect, false);
				}
			}
			else if (asset.IsState(Asset.States.LockedLocal))
			{
				Overlay.DrawOverlay(Asset.States.LockedLocal, iconRect);
				if (metaAsset != null && (states2 & Asset.States.LockedLocal) == Asset.States.None && !flag)
				{
					Overlay.DrawMetaOverlay(iconRect, false);
				}
			}
			else if (asset.IsState(Asset.States.CheckedOutLocal))
			{
				Overlay.DrawOverlay(Asset.States.CheckedOutLocal, iconRect);
				if (metaAsset != null && (states2 & Asset.States.CheckedOutLocal) == Asset.States.None && !flag)
				{
					Overlay.DrawMetaOverlay(iconRect, false);
				}
			}
			else if (asset.IsState(Asset.States.Local) && !asset.IsState(Asset.States.OutOfSync) && !asset.IsState(Asset.States.Synced))
			{
				Overlay.DrawOverlay(Asset.States.Local, iconRect3);
				if (metaAsset != null && (metaAsset.IsUnderVersionControl || !metaAsset.IsState(Asset.States.Local)))
				{
					Overlay.DrawMetaOverlay(iconRect3, false);
				}
			}
			else if (metaAsset != null && metaAsset.IsState(Asset.States.AddedLocal))
			{
				Overlay.DrawOverlay(Asset.States.AddedLocal, iconRect);
				if (flag2)
				{
					Overlay.DrawMetaOverlay(iconRect, false);
				}
			}
			else if (metaAsset != null && metaAsset.IsState(Asset.States.DeletedLocal))
			{
				Overlay.DrawOverlay(Asset.States.DeletedLocal, iconRect);
				if (flag2)
				{
					Overlay.DrawMetaOverlay(iconRect, false);
				}
			}
			else if (metaAsset != null && metaAsset.IsState(Asset.States.LockedLocal))
			{
				Overlay.DrawOverlay(Asset.States.LockedLocal, iconRect);
				if (flag2)
				{
					Overlay.DrawMetaOverlay(iconRect, false);
				}
			}
			else if (metaAsset != null && metaAsset.IsState(Asset.States.CheckedOutLocal))
			{
				Overlay.DrawOverlay(Asset.States.CheckedOutLocal, iconRect);
				if (flag2)
				{
					Overlay.DrawMetaOverlay(iconRect, false);
				}
			}
			else if (metaAsset != null && metaAsset.IsState(Asset.States.Local) && !metaAsset.IsState(Asset.States.OutOfSync) && !metaAsset.IsState(Asset.States.Synced) && !asset.IsState(Asset.States.Conflicted) && (metaAsset == null || !metaAsset.IsState(Asset.States.Conflicted)))
			{
				Overlay.DrawOverlay(Asset.States.Local, iconRect3);
				if (flag2)
				{
					Overlay.DrawMetaOverlay(iconRect3, false);
				}
			}
			if (asset.IsState(Asset.States.Conflicted) || (metaAsset != null && metaAsset.IsState(Asset.States.Conflicted)))
			{
				Overlay.DrawOverlay(Asset.States.Conflicted, iconRect3);
			}
			if (asset.IsState(Asset.States.AddedRemote))
			{
				Overlay.DrawOverlay(Asset.States.AddedRemote, iconRect2);
				if (metaAsset != null && (states3 & Asset.States.AddedRemote) == Asset.States.None)
				{
					Overlay.DrawMetaOverlay(iconRect2, true);
				}
			}
			else if (asset.IsState(Asset.States.DeletedRemote))
			{
				Overlay.DrawOverlay(Asset.States.DeletedRemote, iconRect2);
				if (metaAsset != null && (states3 & Asset.States.DeletedRemote) == Asset.States.None)
				{
					Overlay.DrawMetaOverlay(iconRect2, true);
				}
			}
			else if (asset.IsState(Asset.States.LockedRemote))
			{
				Overlay.DrawOverlay(Asset.States.LockedRemote, iconRect2);
				if (metaAsset != null && (states3 & Asset.States.LockedRemote) == Asset.States.None)
				{
					Overlay.DrawMetaOverlay(iconRect2, true);
				}
			}
			else if (asset.IsState(Asset.States.CheckedOutRemote))
			{
				Overlay.DrawOverlay(Asset.States.CheckedOutRemote, iconRect2);
				if (metaAsset != null && (states3 & Asset.States.CheckedOutRemote) == Asset.States.None)
				{
					Overlay.DrawMetaOverlay(iconRect2, true);
				}
			}
			else if (metaAsset != null && metaAsset.IsState(Asset.States.AddedRemote))
			{
				Overlay.DrawOverlay(Asset.States.AddedRemote, iconRect2);
				if (flag2)
				{
					Overlay.DrawMetaOverlay(iconRect2, true);
				}
			}
			else if (metaAsset != null && metaAsset.IsState(Asset.States.DeletedRemote))
			{
				Overlay.DrawOverlay(Asset.States.DeletedRemote, iconRect2);
				if (flag2)
				{
					Overlay.DrawMetaOverlay(iconRect2, true);
				}
			}
			else if (metaAsset != null && metaAsset.IsState(Asset.States.LockedRemote))
			{
				Overlay.DrawOverlay(Asset.States.LockedRemote, iconRect2);
				if (flag2)
				{
					Overlay.DrawMetaOverlay(iconRect2, true);
				}
			}
			else if (metaAsset != null && metaAsset.IsState(Asset.States.CheckedOutRemote))
			{
				Overlay.DrawOverlay(Asset.States.CheckedOutRemote, iconRect2);
				if (flag2)
				{
					Overlay.DrawMetaOverlay(iconRect2, true);
				}
			}
			if (asset.IsState(Asset.States.OutOfSync) || (metaAsset != null && metaAsset.IsState(Asset.States.OutOfSync)))
			{
				Overlay.DrawOverlay(Asset.States.OutOfSync, iconRect4);
			}
		}

		private static void CreateStaticResources()
		{
			if (Overlay.s_BlueLeftParan == null)
			{
				Overlay.s_BlueLeftParan = EditorGUIUtility.LoadIcon("P4_BlueLeftParenthesis");
				Overlay.s_BlueLeftParan.hideFlags = HideFlags.HideAndDontSave;
				Overlay.s_BlueRightParan = EditorGUIUtility.LoadIcon("P4_BlueRightParenthesis");
				Overlay.s_BlueRightParan.hideFlags = HideFlags.HideAndDontSave;
				Overlay.s_RedLeftParan = EditorGUIUtility.LoadIcon("P4_RedLeftParenthesis");
				Overlay.s_RedLeftParan.hideFlags = HideFlags.HideAndDontSave;
				Overlay.s_RedRightParan = EditorGUIUtility.LoadIcon("P4_RedRightParenthesis");
				Overlay.s_RedRightParan.hideFlags = HideFlags.HideAndDontSave;
			}
		}
	}
}
