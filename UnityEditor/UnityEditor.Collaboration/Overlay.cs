using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEditor.Collaboration
{
	internal class Overlay
	{
		private static readonly Dictionary<Collab.CollabStates, Texture2D> s_Overlays = new Dictionary<Collab.CollabStates, Texture2D>();

		protected static void LoadOverlays()
		{
			Overlay.s_Overlays.Clear();
			Overlay.s_Overlays.Add(Collab.CollabStates.kCollabConflicted, TextureUtility.LoadTextureFromApplicationContents("conflict.png"));
			Overlay.s_Overlays.Add(Collab.CollabStates.kCollabPendingMerge, TextureUtility.LoadTextureFromApplicationContents("conflict.png"));
			Overlay.s_Overlays.Add(Collab.CollabStates.kCollabChanges, TextureUtility.LoadTextureFromApplicationContents("changes.png"));
			Overlay.s_Overlays.Add(Collab.CollabStates.kCollabCheckedOutLocal | Collab.CollabStates.kCollabMovedLocal, TextureUtility.LoadTextureFromApplicationContents("modif-local.png"));
			Overlay.s_Overlays.Add(Collab.CollabStates.kCollabAddedLocal, TextureUtility.LoadTextureFromApplicationContents("added-local.png"));
			Overlay.s_Overlays.Add(Collab.CollabStates.kCollabCheckedOutLocal, TextureUtility.LoadTextureFromApplicationContents("modif-local.png"));
			Overlay.s_Overlays.Add(Collab.CollabStates.kCollabDeletedLocal, TextureUtility.LoadTextureFromApplicationContents("deleted-local.png"));
			Overlay.s_Overlays.Add(Collab.CollabStates.kCollabMovedLocal, TextureUtility.LoadTextureFromApplicationContents("modif-local.png"));
		}

		protected static bool AreOverlaysLoaded()
		{
			bool result;
			if (Overlay.s_Overlays.Count == 0)
			{
				result = false;
			}
			else
			{
				foreach (Texture2D current in Overlay.s_Overlays.Values)
				{
					if (current == null)
					{
						result = false;
						return result;
					}
				}
				result = true;
			}
			return result;
		}

		protected static Collab.CollabStates GetOverlayStateForAsset(Collab.CollabStates assetStates)
		{
			Collab.CollabStates result;
			foreach (Collab.CollabStates current in Overlay.s_Overlays.Keys)
			{
				if (Overlay.HasState(assetStates, current))
				{
					result = current;
					return result;
				}
			}
			result = Collab.CollabStates.kCollabNone;
			return result;
		}

		protected static void DrawOverlayElement(Collab.CollabStates singleState, Rect itemRect)
		{
			Texture2D texture2D;
			if (Overlay.s_Overlays.TryGetValue(singleState, out texture2D))
			{
				if (texture2D != null)
				{
					GUI.DrawTexture(itemRect, texture2D);
				}
			}
		}

		protected static bool HasState(Collab.CollabStates assetStates, Collab.CollabStates includesState)
		{
			return (assetStates & includesState) == includesState;
		}

		public static void DrawOverlays(Collab.CollabStates assetState, Rect itemRect)
		{
			if (assetState != Collab.CollabStates.kCollabInvalidState && assetState != Collab.CollabStates.kCollabNone)
			{
				if (Event.current.type == EventType.Repaint)
				{
					if (!Overlay.AreOverlaysLoaded())
					{
						Overlay.LoadOverlays();
					}
					Collab.CollabStates overlayStateForAsset = Overlay.GetOverlayStateForAsset(assetState);
					Overlay.DrawOverlayElement(overlayStateForAsset, itemRect);
				}
			}
		}
	}
}
