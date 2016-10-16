using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEditor
{
	internal static class MaskFieldGUI
	{
		private class MaskCallbackInfo
		{
			private const string kMaskMenuChangedMessage = "MaskMenuChanged";

			public static MaskFieldGUI.MaskCallbackInfo m_Instance;

			private readonly int m_ControlID;

			private int m_Mask;

			private bool m_SetAll;

			private bool m_ClearAll;

			private bool m_DoNothing;

			private readonly GUIView m_SourceView;

			public MaskCallbackInfo(int controlID)
			{
				this.m_ControlID = controlID;
				this.m_SourceView = GUIView.current;
			}

			public static int GetSelectedValueForControl(int controlID, int mask, out int changedFlags, out bool changedToValue)
			{
				Event current = Event.current;
				changedFlags = 0;
				changedToValue = false;
				if (current.type == EventType.ExecuteCommand && current.commandName == "MaskMenuChanged")
				{
					if (MaskFieldGUI.MaskCallbackInfo.m_Instance == null)
					{
						Debug.LogError("Mask menu has no instance");
						return mask;
					}
					if (MaskFieldGUI.MaskCallbackInfo.m_Instance.m_ControlID == controlID)
					{
						if (!MaskFieldGUI.MaskCallbackInfo.m_Instance.m_DoNothing)
						{
							if (MaskFieldGUI.MaskCallbackInfo.m_Instance.m_ClearAll)
							{
								mask = 0;
								changedFlags = -1;
								changedToValue = false;
							}
							else if (MaskFieldGUI.MaskCallbackInfo.m_Instance.m_SetAll)
							{
								mask = -1;
								changedFlags = -1;
								changedToValue = true;
							}
							else
							{
								mask ^= MaskFieldGUI.MaskCallbackInfo.m_Instance.m_Mask;
								changedFlags = MaskFieldGUI.MaskCallbackInfo.m_Instance.m_Mask;
								changedToValue = ((mask & MaskFieldGUI.MaskCallbackInfo.m_Instance.m_Mask) != 0);
							}
							GUI.changed = true;
						}
						MaskFieldGUI.MaskCallbackInfo.m_Instance.m_DoNothing = false;
						MaskFieldGUI.MaskCallbackInfo.m_Instance.m_ClearAll = false;
						MaskFieldGUI.MaskCallbackInfo.m_Instance.m_SetAll = false;
						MaskFieldGUI.MaskCallbackInfo.m_Instance = null;
						current.Use();
					}
				}
				return mask;
			}

			internal void SetMaskValueDelegate(object userData, string[] options, int selected)
			{
				if (selected != 0)
				{
					if (selected != 1)
					{
						this.m_Mask = 1 << selected - 2;
					}
					else
					{
						this.m_SetAll = true;
					}
				}
				else
				{
					this.m_ClearAll = true;
				}
				if (this.m_SourceView)
				{
					this.m_SourceView.SendEvent(EditorGUIUtility.CommandEvent("MaskMenuChanged"));
				}
			}
		}

		internal static int DoMaskField(Rect position, int controlID, int mask, string[] flagNames, GUIStyle style)
		{
			int num;
			bool flag;
			return MaskFieldGUI.DoMaskField(position, controlID, mask, flagNames, style, out num, out flag);
		}

		internal static int DoMaskField(Rect position, int controlID, int mask, string[] flagNames, GUIStyle style, out int changedFlags, out bool changedToValue)
		{
			mask = MaskFieldGUI.MaskCallbackInfo.GetSelectedValueForControl(controlID, mask, out changedFlags, out changedToValue);
			List<int> list = new List<int>();
			List<string> list2 = new List<string>
			{
				"Nothing",
				"Everything"
			};
			for (int i = 0; i < flagNames.Length; i++)
			{
				if ((mask & 1 << i) != 0)
				{
					list.Add(i + 2);
				}
			}
			list2.AddRange(flagNames);
			GUIContent content = EditorGUI.mixedValueContent;
			if (!EditorGUI.showMixedValue)
			{
				int count = list.Count;
				if (count != 0)
				{
					if (count != 1)
					{
						if (list.Count >= flagNames.Length)
						{
							content = EditorGUIUtility.TempContent("Everything");
							list.Add(1);
							mask = -1;
						}
						else
						{
							content = EditorGUIUtility.TempContent("Mixed ...");
						}
					}
					else
					{
						content = new GUIContent(list2[list[0]]);
					}
				}
				else
				{
					content = EditorGUIUtility.TempContent("Nothing");
					list.Add(0);
				}
			}
			Event current = Event.current;
			if (current.type == EventType.Repaint)
			{
				style.Draw(position, content, controlID, false);
			}
			else if ((current.type == EventType.MouseDown && position.Contains(current.mousePosition)) || current.MainActionKeyForControl(controlID))
			{
				MaskFieldGUI.MaskCallbackInfo.m_Instance = new MaskFieldGUI.MaskCallbackInfo(controlID);
				current.Use();
				EditorUtility.DisplayCustomMenu(position, list2.ToArray(), (!EditorGUI.showMixedValue) ? list.ToArray() : new int[0], new EditorUtility.SelectMenuItemFunction(MaskFieldGUI.MaskCallbackInfo.m_Instance.SetMaskValueDelegate), null);
			}
			return mask;
		}
	}
}
