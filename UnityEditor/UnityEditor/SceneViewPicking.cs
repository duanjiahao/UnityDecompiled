using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace UnityEditor
{
	internal class SceneViewPicking
	{
		private static bool s_RetainHashes;

		private static int s_PreviousTopmostHash;

		private static int s_PreviousPrefixHash;

		static SceneViewPicking()
		{
			Selection.selectionChanged = (Action)Delegate.Combine(Selection.selectionChanged, new Action(SceneViewPicking.ResetHashes));
		}

		private static void ResetHashes()
		{
			if (!SceneViewPicking.s_RetainHashes)
			{
				SceneViewPicking.s_PreviousTopmostHash = 0;
				SceneViewPicking.s_PreviousPrefixHash = 0;
			}
			SceneViewPicking.s_RetainHashes = false;
		}

		public static GameObject PickGameObject(Vector2 mousePosition)
		{
			SceneViewPicking.s_RetainHashes = true;
			IEnumerator<GameObject> enumerator = SceneViewPicking.GetAllOverlapping(mousePosition).GetEnumerator();
			if (!enumerator.MoveNext())
			{
				return null;
			}
			GameObject current = enumerator.Current;
			GameObject gameObject = HandleUtility.FindSelectionBase(current);
			GameObject gameObject2 = (!(gameObject == null)) ? gameObject : current;
			int hashCode = current.GetHashCode();
			int num = hashCode;
			if (Selection.activeGameObject == null)
			{
				SceneViewPicking.s_PreviousTopmostHash = hashCode;
				SceneViewPicking.s_PreviousPrefixHash = num;
				return gameObject2;
			}
			if (hashCode != SceneViewPicking.s_PreviousTopmostHash)
			{
				SceneViewPicking.s_PreviousTopmostHash = hashCode;
				SceneViewPicking.s_PreviousPrefixHash = num;
				return (!(Selection.activeGameObject == gameObject)) ? gameObject2 : current;
			}
			SceneViewPicking.s_PreviousTopmostHash = hashCode;
			if (Selection.activeGameObject == gameObject)
			{
				if (num == SceneViewPicking.s_PreviousPrefixHash)
				{
					return current;
				}
				SceneViewPicking.s_PreviousPrefixHash = num;
				return gameObject;
			}
			else
			{
				GameObject x = HandleUtility.PickGameObject(mousePosition, false, null, new GameObject[]
				{
					Selection.activeGameObject
				});
				if (x == Selection.activeGameObject)
				{
					while (enumerator.Current != Selection.activeGameObject)
					{
						if (!enumerator.MoveNext())
						{
							SceneViewPicking.s_PreviousPrefixHash = hashCode;
							return gameObject2;
						}
						SceneViewPicking.UpdateHash(ref num, enumerator.Current);
					}
				}
				if (num != SceneViewPicking.s_PreviousPrefixHash)
				{
					SceneViewPicking.s_PreviousPrefixHash = hashCode;
					return gameObject2;
				}
				if (!enumerator.MoveNext())
				{
					SceneViewPicking.s_PreviousPrefixHash = hashCode;
					return gameObject2;
				}
				SceneViewPicking.UpdateHash(ref num, enumerator.Current);
				if (enumerator.Current == gameObject)
				{
					if (!enumerator.MoveNext())
					{
						SceneViewPicking.s_PreviousPrefixHash = hashCode;
						return gameObject2;
					}
					SceneViewPicking.UpdateHash(ref num, enumerator.Current);
				}
				SceneViewPicking.s_PreviousPrefixHash = num;
				return enumerator.Current;
			}
		}

		[DebuggerHidden]
		private static IEnumerable<GameObject> GetAllOverlapping(Vector2 position)
		{
			SceneViewPicking.<GetAllOverlapping>c__Iterator8 <GetAllOverlapping>c__Iterator = new SceneViewPicking.<GetAllOverlapping>c__Iterator8();
			<GetAllOverlapping>c__Iterator.position = position;
			<GetAllOverlapping>c__Iterator.<$>position = position;
			SceneViewPicking.<GetAllOverlapping>c__Iterator8 expr_15 = <GetAllOverlapping>c__Iterator;
			expr_15.$PC = -2;
			return expr_15;
		}

		private static void UpdateHash(ref int hash, object obj)
		{
			hash = hash * 33 + obj.GetHashCode();
		}
	}
}
