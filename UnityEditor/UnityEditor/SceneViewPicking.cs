using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace UnityEditor
{
	internal class SceneViewPicking
	{
		private static bool s_RetainHashes;

		private static int s_PreviousTopmostHash;

		private static int s_PreviousPrefixHash;

		[CompilerGenerated]
		private static Action <>f__mg$cache0;

		static SceneViewPicking()
		{
			SceneViewPicking.s_RetainHashes = false;
			SceneViewPicking.s_PreviousTopmostHash = 0;
			SceneViewPicking.s_PreviousPrefixHash = 0;
			Delegate arg_35_0 = Selection.selectionChanged;
			if (SceneViewPicking.<>f__mg$cache0 == null)
			{
				SceneViewPicking.<>f__mg$cache0 = new Action(SceneViewPicking.ResetHashes);
			}
			Selection.selectionChanged = (Action)Delegate.Combine(arg_35_0, SceneViewPicking.<>f__mg$cache0);
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
			GameObject result;
			if (!enumerator.MoveNext())
			{
				result = null;
			}
			else
			{
				GameObject current = enumerator.Current;
				GameObject gameObject = HandleUtility.FindSelectionBase(current);
				GameObject gameObject2 = (!(gameObject == null)) ? gameObject : current;
				int hashCode = current.GetHashCode();
				int num = hashCode;
				if (Selection.activeGameObject == null)
				{
					SceneViewPicking.s_PreviousTopmostHash = hashCode;
					SceneViewPicking.s_PreviousPrefixHash = num;
					result = gameObject2;
				}
				else if (hashCode != SceneViewPicking.s_PreviousTopmostHash)
				{
					SceneViewPicking.s_PreviousTopmostHash = hashCode;
					SceneViewPicking.s_PreviousPrefixHash = num;
					result = ((!(Selection.activeGameObject == gameObject)) ? gameObject2 : current);
				}
				else
				{
					SceneViewPicking.s_PreviousTopmostHash = hashCode;
					if (Selection.activeGameObject == gameObject)
					{
						if (num == SceneViewPicking.s_PreviousPrefixHash)
						{
							result = current;
						}
						else
						{
							SceneViewPicking.s_PreviousPrefixHash = num;
							result = gameObject;
						}
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
									result = gameObject2;
									return result;
								}
								SceneViewPicking.UpdateHash(ref num, enumerator.Current);
							}
						}
						if (num != SceneViewPicking.s_PreviousPrefixHash)
						{
							SceneViewPicking.s_PreviousPrefixHash = hashCode;
							result = gameObject2;
						}
						else if (!enumerator.MoveNext())
						{
							SceneViewPicking.s_PreviousPrefixHash = hashCode;
							result = gameObject2;
						}
						else
						{
							SceneViewPicking.UpdateHash(ref num, enumerator.Current);
							if (enumerator.Current == gameObject)
							{
								if (!enumerator.MoveNext())
								{
									SceneViewPicking.s_PreviousPrefixHash = hashCode;
									result = gameObject2;
									return result;
								}
								SceneViewPicking.UpdateHash(ref num, enumerator.Current);
							}
							SceneViewPicking.s_PreviousPrefixHash = num;
							result = enumerator.Current;
						}
					}
				}
			}
			return result;
		}

		[DebuggerHidden]
		private static IEnumerable<GameObject> GetAllOverlapping(Vector2 position)
		{
			SceneViewPicking.<GetAllOverlapping>c__Iterator0 <GetAllOverlapping>c__Iterator = new SceneViewPicking.<GetAllOverlapping>c__Iterator0();
			<GetAllOverlapping>c__Iterator.position = position;
			SceneViewPicking.<GetAllOverlapping>c__Iterator0 expr_0E = <GetAllOverlapping>c__Iterator;
			expr_0E.$PC = -2;
			return expr_0E;
		}

		private static void UpdateHash(ref int hash, object obj)
		{
			hash = hash * 33 + obj.GetHashCode();
		}
	}
}
