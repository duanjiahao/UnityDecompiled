using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace UnityEditor.Collaboration
{
	internal class Overlay
	{
		private static readonly Dictionary<Collab.CollabStates, Texture2D> s_Overlays = new Dictionary<Collab.CollabStates, Texture2D>();

		protected static Texture2D LoadTextureFromApplicationContents(string path)
		{
			Texture2D texture2D = new Texture2D(2, 2);
			string path2 = Path.Combine(Path.Combine(Path.Combine(EditorApplication.applicationContentsPath, "Resources"), "Collab"), "overlays");
			path = Path.Combine(path2, path);
			try
			{
				FileStream fileStream = File.OpenRead(path);
				byte[] array = new byte[fileStream.Length];
				fileStream.Read(array, 0, (int)fileStream.Length);
				if (!texture2D.LoadImage(array))
				{
					Texture2D result = null;
					return result;
				}
			}
			catch (Exception)
			{
				Debug.LogWarning("Collab Overlay Texture load fail, path: " + path);
				Texture2D result = null;
				return result;
			}
			return texture2D;
		}

		protected static void LoadOverlays()
		{
			Overlay.s_Overlays.Clear();
			Overlay.s_Overlays.Add(Collab.CollabStates.kCollabConflicted, Overlay.LoadTextureFromApplicationContents("conflict.png"));
			Overlay.s_Overlays.Add(Collab.CollabStates.kCollabPendingMerge, Overlay.LoadTextureFromApplicationContents("conflict.png"));
			Overlay.s_Overlays.Add(Collab.CollabStates.kCollabChanges, Overlay.LoadTextureFromApplicationContents("changes.png"));
			Overlay.s_Overlays.Add(Collab.CollabStates.kCollabCheckedOutLocal | Collab.CollabStates.kCollabMovedLocal, Overlay.LoadTextureFromApplicationContents("modif-local.png"));
			Overlay.s_Overlays.Add(Collab.CollabStates.kCollabAddedLocal, Overlay.LoadTextureFromApplicationContents("added-local.png"));
			Overlay.s_Overlays.Add(Collab.CollabStates.kCollabCheckedOutLocal, Overlay.LoadTextureFromApplicationContents("modif-local.png"));
			Overlay.s_Overlays.Add(Collab.CollabStates.kCollabDeletedLocal, Overlay.LoadTextureFromApplicationContents("deleted-local.png"));
			Overlay.s_Overlays.Add(Collab.CollabStates.kCollabMovedLocal, Overlay.LoadTextureFromApplicationContents("modif-local.png"));
		}

		protected static bool AreOverlaysLoaded()
		{
			if (Overlay.s_Overlays.Count == 0)
			{
				return false;
			}
			foreach (Texture2D current in Overlay.s_Overlays.Values)
			{
				if (current == null)
				{
					return false;
				}
			}
			return true;
		}

		protected static Collab.CollabStates GetOverlayStateForAsset(Collab.CollabStates assetStates)
		{
			foreach (Collab.CollabStates current in Overlay.s_Overlays.Keys)
			{
				if (Overlay.HasState(assetStates, current))
				{
					return current;
				}
			}
			return Collab.CollabStates.kCollabNone;
		}

		protected static void DrawOverlayElement(Collab.CollabStates singleState, Rect itemRect)
		{
			Texture2D texture2D;
			if (Overlay.s_Overlays.TryGetValue(singleState, out texture2D) && texture2D != null)
			{
				GUI.DrawTexture(itemRect, texture2D);
			}
		}

		protected static bool HasState(Collab.CollabStates assetStates, Collab.CollabStates includesState)
		{
			return (assetStates & includesState) == includesState;
		}

		public static void DrawOverlays(Collab.CollabStates assetState, Rect itemRect)
		{
			if (assetState == Collab.CollabStates.kCollabInvalidState || assetState == Collab.CollabStates.kCollabNone)
			{
				return;
			}
			if (Event.current.type != EventType.Repaint)
			{
				return;
			}
			if (!Overlay.AreOverlaysLoaded())
			{
				Overlay.LoadOverlays();
			}
			Collab.CollabStates overlayStateForAsset = Overlay.GetOverlayStateForAsset(assetState);
			Overlay.DrawOverlayElement(overlayStateForAsset, itemRect);
		}
	}
}
