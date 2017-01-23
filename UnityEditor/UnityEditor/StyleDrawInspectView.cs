using System;
using UnityEditorInternal;
using UnityEngine;

namespace UnityEditor
{
	internal class StyleDrawInspectView : BaseInspectView
	{
		private class GUIInstruction
		{
			public Rect rect;

			public GUIStyle usedGUIStyle = GUIStyle.none;

			public GUIContent usedGUIContent = GUIContent.none;

			public StackFrame[] stackframes;

			public void Reset()
			{
				this.rect = default(Rect);
				this.usedGUIStyle = GUIStyle.none;
				this.usedGUIContent = GUIContent.none;
			}
		}

		[Serializable]
		private class CachedInstructionInfo
		{
			public SerializedObject styleContainerSerializedObject = null;

			public SerializedProperty styleSerializedProperty = null;

			public readonly GUIStyleHolder styleContainer;

			public CachedInstructionInfo()
			{
				this.styleContainer = ScriptableObject.CreateInstance<GUIStyleHolder>();
			}
		}

		[NonSerialized]
		private StyleDrawInspectView.GUIInstruction m_Instruction;

		[NonSerialized]
		private StyleDrawInspectView.CachedInstructionInfo m_CachedinstructionInfo;

		private Vector2 m_StacktraceScrollPos = default(Vector2);

		public StyleDrawInspectView(GUIViewDebuggerWindow guiViewDebuggerWindow) : base(guiViewDebuggerWindow)
		{
		}

		public override void UpdateInstructions()
		{
		}

		protected override int GetInstructionCount()
		{
			return GUIViewDebuggerHelper.GetInstructionCount();
		}

		protected override void DoDrawInstruction(ListViewElement el, int id)
		{
			string instructionListName = this.GetInstructionListName(el.row);
			GUIContent content = GUIContent.Temp(instructionListName);
			GUIViewDebuggerWindow.s_Styles.listItemBackground.Draw(el.position, false, false, this.m_ListViewState.row == el.row, false);
			GUIViewDebuggerWindow.s_Styles.listItem.Draw(el.position, content, id, this.m_ListViewState.row == el.row);
		}

		internal override void OnDoubleClickInstruction(int index)
		{
			this.ShowInstructionInExternalEditor(GUIViewDebuggerHelper.GetManagedStackTrace(index));
		}

		private int GetInterestingFrameIndex(StackFrame[] stacktrace)
		{
			string dataPath = Application.dataPath;
			int num = -1;
			int result;
			for (int i = 0; i < stacktrace.Length; i++)
			{
				StackFrame stackFrame = stacktrace[i];
				if (!string.IsNullOrEmpty(stackFrame.sourceFile))
				{
					if (!stackFrame.signature.StartsWith("UnityEngine.GUI"))
					{
						if (!stackFrame.signature.StartsWith("UnityEditor.EditorGUI"))
						{
							if (num == -1)
							{
								num = i;
							}
							if (stackFrame.sourceFile.StartsWith(dataPath))
							{
								result = i;
								return result;
							}
						}
					}
				}
			}
			if (num != -1)
			{
				result = num;
				return result;
			}
			result = stacktrace.Length - 1;
			return result;
		}

		internal override void DoDrawSelectedInstructionDetails(int index)
		{
			using (new EditorGUI.DisabledScope(true))
			{
				base.DrawInspectedRect(this.m_Instruction.rect);
			}
			this.DrawInspectedStyle();
			using (new EditorGUI.DisabledScope(true))
			{
				this.DrawInspectedGUIContent();
			}
		}

		protected override bool HasSelectedinstruction()
		{
			return this.m_Instruction != null;
		}

		private void DrawInspectedGUIContent()
		{
			GUILayout.Label(GUIContent.Temp("GUIContent"), new GUILayoutOption[0]);
			EditorGUI.indentLevel++;
			EditorGUILayout.TextField(this.m_Instruction.usedGUIContent.text, new GUILayoutOption[0]);
			EditorGUILayout.ObjectField(this.m_Instruction.usedGUIContent.image, typeof(Texture2D), false, new GUILayoutOption[0]);
			EditorGUI.indentLevel--;
		}

		private void DrawInspectedStyle()
		{
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField(this.m_CachedinstructionInfo.styleSerializedProperty, GUIContent.Temp("Style"), true, new GUILayoutOption[0]);
			if (EditorGUI.EndChangeCheck())
			{
				this.m_CachedinstructionInfo.styleContainerSerializedObject.ApplyModifiedPropertiesWithoutUndo();
				this.m_GuiViewDebuggerWindow.m_Inspected.Repaint();
			}
		}

		protected override void DrawInspectedStacktrace()
		{
			this.m_StacktraceScrollPos = EditorGUILayout.BeginScrollView(this.m_StacktraceScrollPos, GUIViewDebuggerWindow.s_Styles.stacktraceBackground, new GUILayoutOption[]
			{
				GUILayout.ExpandHeight(false)
			});
			base.DrawStackFrameList(this.m_Instruction.stackframes);
			EditorGUILayout.EndScrollView();
		}

		public void GetSelectedStyleProperty(out SerializedObject serializedObject, out SerializedProperty styleProperty)
		{
			GUISkin gUISkin = null;
			GUISkin current = GUISkin.current;
			GUIStyle gUIStyle = current.FindStyle(this.m_Instruction.usedGUIStyle.name);
			if (gUIStyle != null && gUIStyle == this.m_Instruction.usedGUIStyle)
			{
				gUISkin = current;
			}
			styleProperty = null;
			if (gUISkin != null)
			{
				serializedObject = new SerializedObject(gUISkin);
				SerializedProperty iterator = serializedObject.GetIterator();
				bool enterChildren = true;
				while (iterator.NextVisible(enterChildren))
				{
					if (iterator.type == "GUIStyle")
					{
						enterChildren = false;
						SerializedProperty serializedProperty = iterator.FindPropertyRelative("m_Name");
						if (serializedProperty.stringValue == this.m_Instruction.usedGUIStyle.name)
						{
							styleProperty = iterator;
							return;
						}
					}
					else
					{
						enterChildren = true;
					}
				}
				Debug.Log(string.Format("Showing editable Style from GUISkin: {0}, IsPersistant: {1}", gUISkin.name, EditorUtility.IsPersistent(gUISkin)));
			}
			serializedObject = new SerializedObject(this.m_CachedinstructionInfo.styleContainer);
			styleProperty = serializedObject.FindProperty("inspectedStyle");
		}

		internal override void OnSelectedInstructionChanged(int index)
		{
			this.m_ListViewState.row = index;
			if (this.m_ListViewState.row >= 0)
			{
				if (this.m_Instruction == null)
				{
					this.m_Instruction = new StyleDrawInspectView.GUIInstruction();
				}
				if (this.m_CachedinstructionInfo == null)
				{
					this.m_CachedinstructionInfo = new StyleDrawInspectView.CachedInstructionInfo();
				}
				this.m_Instruction.rect = GUIViewDebuggerHelper.GetRectFromInstruction(this.m_ListViewState.row);
				this.m_Instruction.usedGUIStyle = GUIViewDebuggerHelper.GetStyleFromInstruction(this.m_ListViewState.row);
				this.m_Instruction.usedGUIContent = GUIViewDebuggerHelper.GetContentFromInstruction(this.m_ListViewState.row);
				this.m_Instruction.stackframes = GUIViewDebuggerHelper.GetManagedStackTrace(this.m_ListViewState.row);
				this.m_CachedinstructionInfo.styleContainer.inspectedStyle = this.m_Instruction.usedGUIStyle;
				this.m_CachedinstructionInfo.styleContainerSerializedObject = null;
				this.m_CachedinstructionInfo.styleSerializedProperty = null;
				this.GetSelectedStyleProperty(out this.m_CachedinstructionInfo.styleContainerSerializedObject, out this.m_CachedinstructionInfo.styleSerializedProperty);
				this.m_GuiViewDebuggerWindow.HighlightInstruction(this.m_GuiViewDebuggerWindow.m_Inspected, this.m_Instruction.rect, this.m_Instruction.usedGUIStyle);
			}
			else
			{
				this.m_Instruction = null;
				this.m_CachedinstructionInfo = null;
				if (this.m_GuiViewDebuggerWindow.InstructionOverlayWindow != null)
				{
					this.m_GuiViewDebuggerWindow.InstructionOverlayWindow.Close();
				}
			}
		}

		private void ShowInstructionInExternalEditor(StackFrame[] frames)
		{
			int interestingFrameIndex = this.GetInterestingFrameIndex(frames);
			StackFrame stackFrame = frames[interestingFrameIndex];
			InternalEditorUtility.OpenFileAtLineExternal(stackFrame.sourceFile, (int)stackFrame.lineNumber);
		}

		internal override string GetInstructionListName(int index)
		{
			StackFrame[] managedStackTrace = GUIViewDebuggerHelper.GetManagedStackTrace(index);
			string instructionListName = this.GetInstructionListName(managedStackTrace);
			return string.Format("{0}. {1}", index, instructionListName);
		}

		protected string GetInstructionListName(StackFrame[] stacktrace)
		{
			int num = this.GetInterestingFrameIndex(stacktrace);
			if (num > 0)
			{
				num--;
			}
			StackFrame stackFrame = stacktrace[num];
			return stackFrame.methodName;
		}

		public override void Unselect()
		{
			base.Unselect();
			this.m_Instruction = null;
		}

		public override void ShowOverlay()
		{
			if (this.m_Instruction != null)
			{
				this.m_GuiViewDebuggerWindow.HighlightInstruction(this.m_GuiViewDebuggerWindow.m_Inspected, this.m_Instruction.rect, this.m_Instruction.usedGUIStyle);
			}
		}
	}
}
