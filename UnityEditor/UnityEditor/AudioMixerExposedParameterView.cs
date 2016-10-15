using System;
using UnityEditor.Audio;
using UnityEditorInternal;
using UnityEngine;

namespace UnityEditor
{
	internal class AudioMixerExposedParameterView
	{
		private ReorderableListWithRenameAndScrollView m_ReorderableListWithRenameAndScrollView;

		private AudioMixerController m_Controller;

		private SerializedObject m_ControllerSerialized;

		private ReorderableListWithRenameAndScrollView.State m_State;

		private float height
		{
			get
			{
				return this.m_ReorderableListWithRenameAndScrollView.list.GetHeight();
			}
		}

		public AudioMixerExposedParameterView(ReorderableListWithRenameAndScrollView.State state)
		{
			this.m_State = state;
		}

		public void OnMixerControllerChanged(AudioMixerController controller)
		{
			this.m_Controller = controller;
			if (this.m_Controller)
			{
				this.m_Controller.ChangedExposedParameter += new ChangedExposedParameterHandler(this.RecreateListControl);
			}
			this.RecreateListControl();
		}

		public void RecreateListControl()
		{
			if (this.m_Controller != null)
			{
				this.m_ControllerSerialized = new SerializedObject(this.m_Controller);
				SerializedProperty elements = this.m_ControllerSerialized.FindProperty("m_ExposedParameters");
				ReorderableList reorderableList = new ReorderableList(this.m_ControllerSerialized, elements, false, false, false, false);
				reorderableList.onReorderCallback = new ReorderableList.ReorderCallbackDelegate(this.EndDragChild);
				ReorderableList expr_57 = reorderableList;
				expr_57.drawElementCallback = (ReorderableList.ElementCallbackDelegate)Delegate.Combine(expr_57.drawElementCallback, new ReorderableList.ElementCallbackDelegate(this.DrawElement));
				reorderableList.elementHeight = 16f;
				reorderableList.headerHeight = 0f;
				reorderableList.footerHeight = 0f;
				reorderableList.showDefaultBackground = false;
				this.m_ReorderableListWithRenameAndScrollView = new ReorderableListWithRenameAndScrollView(reorderableList, this.m_State);
				ReorderableListWithRenameAndScrollView expr_B8 = this.m_ReorderableListWithRenameAndScrollView;
				expr_B8.onNameChangedAtIndex = (Action<int, string>)Delegate.Combine(expr_B8.onNameChangedAtIndex, new Action<int, string>(this.NameChanged));
				ReorderableListWithRenameAndScrollView expr_DF = this.m_ReorderableListWithRenameAndScrollView;
				expr_DF.onDeleteItemAtIndex = (Action<int>)Delegate.Combine(expr_DF.onDeleteItemAtIndex, new Action<int>(this.Delete));
				ReorderableListWithRenameAndScrollView expr_106 = this.m_ReorderableListWithRenameAndScrollView;
				expr_106.onGetNameAtIndex = (Func<int, string>)Delegate.Combine(expr_106.onGetNameAtIndex, new Func<int, string>(this.GetNameOfElement));
			}
		}

		public void OnGUI(Rect rect)
		{
			if (this.m_Controller == null)
			{
				return;
			}
			this.m_ReorderableListWithRenameAndScrollView.OnGUI(rect);
		}

		public void OnContextClick(int itemIndex)
		{
			GenericMenu genericMenu = new GenericMenu();
			genericMenu.AddItem(new GUIContent("Unexpose"), false, delegate(object data)
			{
				this.Delete((int)data);
			}, itemIndex);
			genericMenu.AddItem(new GUIContent("Rename"), false, delegate(object data)
			{
				this.m_ReorderableListWithRenameAndScrollView.BeginRename((int)data, 0f);
			}, itemIndex);
			genericMenu.ShowAsContext();
		}

		private void DrawElement(Rect rect, int index, bool isActive, bool isFocused)
		{
			Event current = Event.current;
			if (current.type == EventType.ContextClick && rect.Contains(current.mousePosition))
			{
				this.OnContextClick(index);
				current.Use();
			}
			if (Event.current.type != EventType.Repaint)
			{
				return;
			}
			using (new EditorGUI.DisabledScope(true))
			{
				this.m_ReorderableListWithRenameAndScrollView.elementStyleRightAligned.Draw(rect, this.GetInfoString(index), false, false, false, false);
			}
		}

		public Vector2 CalcSize()
		{
			float num = 0f;
			for (int i = 0; i < this.m_ReorderableListWithRenameAndScrollView.list.count; i++)
			{
				float num2 = this.WidthOfRow(i, this.m_ReorderableListWithRenameAndScrollView.elementStyle, this.m_ReorderableListWithRenameAndScrollView.elementStyleRightAligned);
				if (num2 > num)
				{
					num = num2;
				}
			}
			return new Vector2(num, this.height);
		}

		private string GetInfoString(int index)
		{
			ExposedAudioParameter exposedAudioParameter = this.m_Controller.exposedParameters[index];
			return this.m_Controller.ResolveExposedParameterPath(exposedAudioParameter.guid, false);
		}

		private float WidthOfRow(int index, GUIStyle leftStyle, GUIStyle rightStyle)
		{
			string infoString = this.GetInfoString(index);
			Vector2 vector = rightStyle.CalcSize(GUIContent.Temp(infoString));
			return leftStyle.CalcSize(GUIContent.Temp(this.GetNameOfElement(index))).x + vector.x + 25f;
		}

		private string GetNameOfElement(int index)
		{
			ExposedAudioParameter exposedAudioParameter = this.m_Controller.exposedParameters[index];
			return exposedAudioParameter.name;
		}

		public void NameChanged(int index, string newName)
		{
			if (newName.Length > 64)
			{
				newName = newName.Substring(0, 64);
				Debug.LogWarning(string.Concat(new object[]
				{
					"Maximum name length of an exposed parameter is ",
					64,
					" characters. Name truncated to '",
					newName,
					"'"
				}));
			}
			ExposedAudioParameter[] exposedParameters = this.m_Controller.exposedParameters;
			exposedParameters[index].name = newName;
			this.m_Controller.exposedParameters = exposedParameters;
		}

		private void Delete(int index)
		{
			if (this.m_Controller != null)
			{
				Undo.RecordObject(this.m_Controller, "Unexpose Mixer Parameter");
				ExposedAudioParameter exposedAudioParameter = this.m_Controller.exposedParameters[index];
				this.m_Controller.RemoveExposedParameter(exposedAudioParameter.guid);
			}
		}

		public void EndDragChild(ReorderableList list)
		{
			this.m_ControllerSerialized.ApplyModifiedProperties();
		}

		public void OnEvent()
		{
			this.m_ReorderableListWithRenameAndScrollView.OnEvent();
		}
	}
}
