using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEditor.Collaboration
{
	internal class Overlay
	{
		public const double k_OverlaySizeOnSmallIcon = 0.6;

		public const double k_OverlaySizeOnLargeIcon = 0.35;

		private static readonly Dictionary<Collab.CollabStates, GUIContent> s_Overlays = new Dictionary<Collab.CollabStates, GUIContent>();

		protected static void LoadOverlays()
		{
			Overlay.s_Overlays.Clear();
			Overlay.s_Overlays.Add(Collab.CollabStates.kCollabIgnored, EditorGUIUtility.IconContent("CollabExclude Icon"));
			Overlay.s_Overlays.Add(Collab.CollabStates.kCollabConflicted, EditorGUIUtility.IconContent("CollabConflict Icon"));
			Overlay.s_Overlays.Add(Collab.CollabStates.kCollabPendingMerge, EditorGUIUtility.IconContent("CollabConflict Icon"));
			Overlay.s_Overlays.Add(Collab.CollabStates.kCollabMovedLocal, EditorGUIUtility.IconContent("CollabMoved Icon"));
			Overlay.s_Overlays.Add(Collab.CollabStates.kCollabCheckedOutLocal | Collab.CollabStates.kCollabMovedLocal, EditorGUIUtility.IconContent("CollabMoved Icon"));
			Overlay.s_Overlays.Add(Collab.CollabStates.kCollabCheckedOutLocal, EditorGUIUtility.IconContent("CollabEdit Icon"));
			Overlay.s_Overlays.Add(Collab.CollabStates.kCollabAddedLocal, EditorGUIUtility.IconContent("CollabCreate Icon"));
			Overlay.s_Overlays.Add(Collab.CollabStates.kCollabDeletedLocal, EditorGUIUtility.IconContent("CollabDeleted Icon"));
			Overlay.s_Overlays.Add(Collab.CollabStates.KCollabContentConflicted, EditorGUIUtility.IconContent("CollabChangesConflict Icon"));
			Overlay.s_Overlays.Add(Collab.CollabStates.KCollabContentChanged, EditorGUIUtility.IconContent("CollabChanges Icon"));
			Overlay.s_Overlays.Add(Collab.CollabStates.KCollabContentDeleted, EditorGUIUtility.IconContent("CollabChangesDeleted Icon"));
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
				using (Dictionary<Collab.CollabStates, GUIContent>.ValueCollection.Enumerator enumerator = Overlay.s_Overlays.Values.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current == null)
						{
							result = false;
							return result;
						}
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
			GUIContent gUIContent;
			if (Overlay.s_Overlays.TryGetValue(singleState, out gUIContent))
			{
				Texture image = gUIContent.image;
				if (image != null)
				{
					GUI.DrawTexture(itemRect, image, ScaleMode.ScaleToFit);
				}
			}
		}

		protected static bool HasState(Collab.CollabStates assetStates, Collab.CollabStates includesState)
		{
			return (assetStates & includesState) == includesState;
		}

		public static void DrawOverlays(Collab.CollabStates assetState, Rect itemRect, bool isListMode)
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
					Overlay.DrawOverlayElement(overlayStateForAsset, Overlay.GetRectForTopRight(itemRect, Overlay.GetScale(itemRect, isListMode)));
				}
			}
		}

		public static Rect ScaleRect(Rect rect, double scale)
		{
			return new Rect(rect)
			{
				width = (float)Convert.ToInt32(Math.Ceiling((double)rect.width * scale)),
				height = (float)Convert.ToInt32(Math.Ceiling((double)rect.height * scale))
			};
		}

		public static double GetScale(Rect rect, bool isListMode)
		{
			double result = 0.35;
			if (isListMode)
			{
				result = 0.6;
			}
			return result;
		}

		public static Rect GetRectForTopRight(Rect projectBrowserDrawRect, double scale)
		{
			Rect result = Overlay.ScaleRect(projectBrowserDrawRect, scale);
			result.x += projectBrowserDrawRect.width - result.width;
			return result;
		}

		public static Rect GetRectForBottomRight(Rect projectBrowserDrawRect, double scale)
		{
			Rect result = Overlay.ScaleRect(projectBrowserDrawRect, scale);
			result.x += projectBrowserDrawRect.width - result.width;
			result.y += projectBrowserDrawRect.height - result.height;
			return result;
		}
	}
}
