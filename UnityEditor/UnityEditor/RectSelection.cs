using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnityEditor
{
	internal class RectSelection
	{
		private enum SelectionType
		{
			Normal,
			Additive,
			Subtractive
		}

		private Vector2 m_SelectStartPoint;

		private Vector2 m_SelectMousePoint;

		private UnityEngine.Object[] m_SelectionStart;

		private bool m_RectSelecting;

		private Dictionary<GameObject, bool> m_LastSelection;

		private UnityEngine.Object[] m_CurrentSelection = null;

		private EditorWindow m_Window;

		private static int s_RectSelectionID = GUIUtility.GetPermanentControlID();

		public RectSelection(EditorWindow window)
		{
			this.m_Window = window;
		}

		public void OnGUI()
		{
			Event current = Event.current;
			Handles.BeginGUI();
			Vector2 mousePosition = current.mousePosition;
			int num = RectSelection.s_RectSelectionID;
			EventType typeForControl = current.GetTypeForControl(num);
			switch (typeForControl)
			{
			case EventType.MouseDown:
				if (HandleUtility.nearestControl == num && current.button == 0)
				{
					GUIUtility.hotControl = num;
					this.m_SelectStartPoint = mousePosition;
					this.m_SelectionStart = Selection.objects;
					this.m_RectSelecting = false;
				}
				goto IL_4F7;
			case EventType.MouseUp:
				if (GUIUtility.hotControl == num && current.button == 0)
				{
					GUIUtility.hotControl = 0;
					if (this.m_RectSelecting)
					{
						EditorApplication.modifierKeysChanged = (EditorApplication.CallbackFunction)Delegate.Remove(EditorApplication.modifierKeysChanged, new EditorApplication.CallbackFunction(this.SendCommandsOnModifierKeys));
						this.m_RectSelecting = false;
						this.m_SelectionStart = new UnityEngine.Object[0];
						current.Use();
					}
					else
					{
						if (current.shift || EditorGUI.actionKey)
						{
							GameObject gameObject = HandleUtility.PickGameObject(current.mousePosition, false);
							if ((!EditorGUI.actionKey) ? (Selection.activeGameObject == gameObject) : Selection.gameObjects.Contains(gameObject))
							{
								RectSelection.UpdateSelection(this.m_SelectionStart, gameObject, RectSelection.SelectionType.Subtractive, this.m_RectSelecting);
							}
							else
							{
								RectSelection.UpdateSelection(this.m_SelectionStart, HandleUtility.PickGameObject(current.mousePosition, true), RectSelection.SelectionType.Additive, this.m_RectSelecting);
							}
						}
						else
						{
							GameObject newObject = SceneViewPicking.PickGameObject(current.mousePosition);
							RectSelection.UpdateSelection(this.m_SelectionStart, newObject, RectSelection.SelectionType.Normal, this.m_RectSelecting);
						}
						current.Use();
					}
				}
				goto IL_4F7;
			case EventType.MouseMove:
			case EventType.KeyDown:
			case EventType.KeyUp:
			case EventType.ScrollWheel:
				IL_4B:
				if (typeForControl != EventType.ExecuteCommand)
				{
					goto IL_4F7;
				}
				if (num == GUIUtility.hotControl && current.commandName == "ModifierKeysChanged")
				{
					if (current.shift)
					{
						RectSelection.UpdateSelection(this.m_SelectionStart, this.m_CurrentSelection, RectSelection.SelectionType.Additive, this.m_RectSelecting);
					}
					else if (EditorGUI.actionKey)
					{
						RectSelection.UpdateSelection(this.m_SelectionStart, this.m_CurrentSelection, RectSelection.SelectionType.Subtractive, this.m_RectSelecting);
					}
					else
					{
						RectSelection.UpdateSelection(this.m_SelectionStart, this.m_CurrentSelection, RectSelection.SelectionType.Normal, this.m_RectSelecting);
					}
					current.Use();
				}
				goto IL_4F7;
			case EventType.MouseDrag:
				if (GUIUtility.hotControl == num)
				{
					if (!this.m_RectSelecting && (mousePosition - this.m_SelectStartPoint).magnitude > 6f)
					{
						EditorApplication.modifierKeysChanged = (EditorApplication.CallbackFunction)Delegate.Combine(EditorApplication.modifierKeysChanged, new EditorApplication.CallbackFunction(this.SendCommandsOnModifierKeys));
						this.m_RectSelecting = true;
						this.m_LastSelection = null;
						this.m_CurrentSelection = null;
					}
					if (this.m_RectSelecting)
					{
						this.m_SelectMousePoint = new Vector2(Mathf.Max(mousePosition.x, 0f), Mathf.Max(mousePosition.y, 0f));
						GameObject[] array = HandleUtility.PickRectObjects(EditorGUIExt.FromToRect(this.m_SelectStartPoint, this.m_SelectMousePoint));
						this.m_CurrentSelection = array;
						bool flag = false;
						if (this.m_LastSelection == null)
						{
							this.m_LastSelection = new Dictionary<GameObject, bool>();
							flag = true;
						}
						flag |= (this.m_LastSelection.Count != array.Length);
						if (!flag)
						{
							Dictionary<GameObject, bool> dictionary = new Dictionary<GameObject, bool>(array.Length);
							GameObject[] array2 = array;
							for (int i = 0; i < array2.Length; i++)
							{
								GameObject key = array2[i];
								dictionary.Add(key, false);
							}
							foreach (GameObject current2 in this.m_LastSelection.Keys)
							{
								if (!dictionary.ContainsKey(current2))
								{
									flag = true;
									break;
								}
							}
						}
						if (flag)
						{
							this.m_LastSelection = new Dictionary<GameObject, bool>(array.Length);
							GameObject[] array3 = array;
							for (int j = 0; j < array3.Length; j++)
							{
								GameObject key2 = array3[j];
								this.m_LastSelection.Add(key2, false);
							}
							if (array != null)
							{
								if (current.shift)
								{
									RectSelection.UpdateSelection(this.m_SelectionStart, array, RectSelection.SelectionType.Additive, this.m_RectSelecting);
								}
								else if (EditorGUI.actionKey)
								{
									RectSelection.UpdateSelection(this.m_SelectionStart, array, RectSelection.SelectionType.Subtractive, this.m_RectSelecting);
								}
								else
								{
									RectSelection.UpdateSelection(this.m_SelectionStart, array, RectSelection.SelectionType.Normal, this.m_RectSelecting);
								}
							}
						}
					}
					current.Use();
				}
				goto IL_4F7;
			case EventType.Repaint:
				if (GUIUtility.hotControl == num && this.m_RectSelecting)
				{
					EditorStyles.selectionRect.Draw(EditorGUIExt.FromToRect(this.m_SelectStartPoint, this.m_SelectMousePoint), GUIContent.none, false, false, false, false);
				}
				goto IL_4F7;
			case EventType.Layout:
				if (!Tools.viewToolActive)
				{
					HandleUtility.AddDefaultControl(num);
				}
				goto IL_4F7;
			}
			goto IL_4B;
			IL_4F7:
			Handles.EndGUI();
		}

		private static void UpdateSelection(UnityEngine.Object[] existingSelection, UnityEngine.Object newObject, RectSelection.SelectionType type, bool isRectSelection)
		{
			UnityEngine.Object[] newObjects;
			if (newObject == null)
			{
				newObjects = new UnityEngine.Object[0];
			}
			else
			{
				newObjects = new UnityEngine.Object[]
				{
					newObject
				};
			}
			RectSelection.UpdateSelection(existingSelection, newObjects, type, isRectSelection);
		}

		private static void UpdateSelection(UnityEngine.Object[] existingSelection, UnityEngine.Object[] newObjects, RectSelection.SelectionType type, bool isRectSelection)
		{
			switch (type)
			{
			case RectSelection.SelectionType.Additive:
				if (newObjects.Length > 0)
				{
					UnityEngine.Object[] array = new UnityEngine.Object[existingSelection.Length + newObjects.Length];
					Array.Copy(existingSelection, array, existingSelection.Length);
					for (int i = 0; i < newObjects.Length; i++)
					{
						array[existingSelection.Length + i] = newObjects[i];
					}
					if (!isRectSelection)
					{
						Selection.activeObject = newObjects[0];
					}
					else
					{
						Selection.activeObject = array[0];
					}
					Selection.objects = array;
				}
				else
				{
					Selection.objects = existingSelection;
				}
				return;
			case RectSelection.SelectionType.Subtractive:
			{
				Dictionary<UnityEngine.Object, bool> dictionary = new Dictionary<UnityEngine.Object, bool>(existingSelection.Length);
				for (int j = 0; j < existingSelection.Length; j++)
				{
					UnityEngine.Object key = existingSelection[j];
					dictionary.Add(key, false);
				}
				for (int k = 0; k < newObjects.Length; k++)
				{
					UnityEngine.Object key2 = newObjects[k];
					if (dictionary.ContainsKey(key2))
					{
						dictionary.Remove(key2);
					}
				}
				UnityEngine.Object[] array = new UnityEngine.Object[dictionary.Keys.Count];
				dictionary.Keys.CopyTo(array, 0);
				Selection.objects = array;
				return;
			}
			}
			Selection.objects = newObjects;
		}

		internal void SendCommandsOnModifierKeys()
		{
			this.m_Window.SendEvent(EditorGUIUtility.CommandEvent("ModifierKeysChanged"));
		}
	}
}
