using System;
using System.Collections;
using UnityEngine;

namespace UnityEditor
{
	internal abstract class BaseInspectView : IBaseInspectView
	{
		private class Styles
		{
			public GUIStyle centeredLabel = new GUIStyle("PR Label");

			public Styles()
			{
				this.centeredLabel.alignment = TextAnchor.MiddleCenter;
				this.centeredLabel.padding.right = 0;
				this.centeredLabel.padding.left = 0;
			}
		}

		private BaseInspectView.Styles m_Styles;

		protected GUIViewDebuggerWindow m_GuiViewDebuggerWindow;

		[NonSerialized]
		public readonly ListViewState m_ListViewState = new ListViewState();

		protected Vector2 m_InstructionDetailsScrollPos = default(Vector2);

		protected readonly SplitterState m_InstructionDetailStacktraceSplitter = new SplitterState(new float[]
		{
			80f,
			20f
		}, new int[]
		{
			100,
			100
		}, null);

		private BaseInspectView.Styles styles
		{
			get
			{
				if (this.m_Styles == null)
				{
					this.m_Styles = new BaseInspectView.Styles();
				}
				return this.m_Styles;
			}
		}

		public BaseInspectView(GUIViewDebuggerWindow guiViewDebuggerWindow)
		{
			this.m_GuiViewDebuggerWindow = guiViewDebuggerWindow;
		}

		public virtual void DrawInstructionList()
		{
			Event current = Event.current;
			this.m_ListViewState.totalRows = this.GetInstructionCount();
			EditorGUILayout.BeginVertical(GUIViewDebuggerWindow.s_Styles.listBackgroundStyle, new GUILayoutOption[0]);
			GUILayout.Label("Instructions", new GUILayoutOption[0]);
			int controlID = GUIUtility.GetControlID(FocusType.Keyboard);
			IEnumerator enumerator = ListViewGUI.ListView(this.m_ListViewState, GUIViewDebuggerWindow.s_Styles.listBackgroundStyle, new GUILayoutOption[0]).GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					ListViewElement el = (ListViewElement)enumerator.Current;
					if (current.type == EventType.MouseDown && current.button == 0 && el.position.Contains(current.mousePosition))
					{
						if (current.clickCount == 2)
						{
							this.OnDoubleClickInstruction(el.row);
						}
					}
					if (current.type == EventType.Repaint)
					{
						this.DoDrawInstruction(el, controlID);
					}
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = (enumerator as IDisposable)) != null)
				{
					disposable.Dispose();
				}
			}
			EditorGUILayout.EndVertical();
		}

		public abstract void UpdateInstructions();

		protected abstract int GetInstructionCount();

		protected abstract void DoDrawInstruction(ListViewElement el, int controlId);

		internal abstract string GetInstructionListName(int index);

		internal abstract void OnDoubleClickInstruction(int index);

		public virtual void DrawSelectedInstructionDetails()
		{
			if (this.m_ListViewState.selectionChanged)
			{
				this.OnSelectedInstructionChanged(this.m_ListViewState.row);
			}
			if (!this.HasSelectedinstruction())
			{
				this.DoDrawNothingSelected();
			}
			else
			{
				SplitterGUILayout.BeginVerticalSplit(this.m_InstructionDetailStacktraceSplitter, new GUILayoutOption[0]);
				this.m_InstructionDetailsScrollPos = EditorGUILayout.BeginScrollView(this.m_InstructionDetailsScrollPos, GUIViewDebuggerWindow.s_Styles.boxStyle, new GUILayoutOption[0]);
				this.DoDrawSelectedInstructionDetails(this.m_ListViewState.row);
				EditorGUILayout.EndScrollView();
				this.DrawInspectedStacktrace();
				SplitterGUILayout.EndVerticalSplit();
			}
		}

		protected abstract void DrawInspectedStacktrace();

		internal abstract void DoDrawSelectedInstructionDetails(int index);

		protected virtual bool HasSelectedinstruction()
		{
			return this.m_ListViewState.row >= 0;
		}

		protected virtual void DoDrawNothingSelected()
		{
			EditorGUILayout.BeginVertical(new GUILayoutOption[0]);
			GUILayout.FlexibleSpace();
			GUILayout.Label("Select a Instruction on the left to see details", GUIViewDebuggerWindow.s_Styles.centeredText, new GUILayoutOption[0]);
			GUILayout.FlexibleSpace();
			EditorGUILayout.EndVertical();
		}

		internal abstract void OnSelectedInstructionChanged(int newSelectionIndex);

		public virtual void Unselect()
		{
			this.m_ListViewState.row = -1;
			this.m_ListViewState.selectionChanged = true;
		}

		public virtual void SelectRow(int index)
		{
			this.m_ListViewState.row = index;
			this.m_ListViewState.selectionChanged = true;
		}

		public abstract void ShowOverlay();

		protected void DrawStackFrameList(StackFrame[] stackframes)
		{
			if (stackframes != null)
			{
				for (int i = 0; i < stackframes.Length; i++)
				{
					StackFrame stackFrame = stackframes[i];
					if (!string.IsNullOrEmpty(stackFrame.sourceFile))
					{
						GUILayout.Label(string.Format("{0} [{1}:{2}]", stackFrame.signature, stackFrame.sourceFile, stackFrame.lineNumber), GUIViewDebuggerWindow.s_Styles.stackframeStyle, new GUILayoutOption[0]);
					}
				}
			}
		}

		protected void DrawInspectedRect(Rect instructionRect)
		{
			Rect rect = GUILayoutUtility.GetRect(0f, 100f);
			int top = Mathf.CeilToInt(34f);
			int bottom = Mathf.CeilToInt(16f);
			int right = 100;
			RectOffset rectOffset = new RectOffset(50, right, top, bottom);
			Rect rect2 = rectOffset.Remove(rect);
			float imageAspect = instructionRect.width / instructionRect.height;
			Rect rect3 = default(Rect);
			Rect rect4 = default(Rect);
			GUI.CalculateScaledTextureRects(rect2, ScaleMode.ScaleToFit, imageAspect, ref rect3, ref rect4);
			rect2 = rect3;
			rect2.width = Mathf.Max(80f, rect2.width);
			rect2.height = Mathf.Max(26f, rect2.height);
			Rect position = default(Rect);
			position.height = 16f;
			position.width = (float)(rectOffset.left * 2);
			position.y = rect2.y - (float)rectOffset.top;
			position.x = rect2.x - position.width / 2f;
			Rect position2 = new Rect
			{
				height = 16f,
				width = (float)(rectOffset.right * 2),
				y = rect2.yMax
			};
			position2.x = rect2.xMax - position2.width / 2f;
			Rect rect5 = new Rect
			{
				x = rect2.x,
				y = position.yMax + 2f,
				width = rect2.width,
				height = 16f
			};
			Rect position3 = rect5;
			position3.width = rect5.width / 3f;
			position3.x = rect5.x + (rect5.width - position3.width) / 2f;
			Rect rect6 = rect2;
			rect6.x = rect2.xMax;
			rect6.width = 16f;
			Rect position4 = rect6;
			position4.height = 16f;
			position4.width = (float)rectOffset.right;
			position4.y += (rect6.height - position4.height) / 2f;
			GUI.Label(position, string.Format("({0},{1})", instructionRect.x, instructionRect.y), this.styles.centeredLabel);
			Handles.color = new Color(1f, 1f, 1f, 0.5f);
			Vector3 p = new Vector3(rect5.x, position3.y);
			Vector3 p2 = new Vector3(rect5.x, position3.yMax);
			Handles.DrawLine(p, p2);
			p.x = (p2.x = rect5.xMax);
			Handles.DrawLine(p, p2);
			p.x = rect5.x;
			p.y = (p2.y = Mathf.Lerp(p.y, p2.y, 0.5f));
			p2.x = position3.x;
			Handles.DrawLine(p, p2);
			p.x = position3.xMax;
			p2.x = rect5.xMax;
			Handles.DrawLine(p, p2);
			GUI.Label(position3, instructionRect.width.ToString(), this.styles.centeredLabel);
			p = new Vector3(rect6.x, rect6.y);
			p2 = new Vector3(rect6.xMax, rect6.y);
			Handles.DrawLine(p, p2);
			p.y = (p2.y = rect6.yMax);
			Handles.DrawLine(p, p2);
			p.x = (p2.x = Mathf.Lerp(p.x, p2.x, 0.5f));
			p.y = rect6.y;
			p2.y = position4.y;
			Handles.DrawLine(p, p2);
			p.y = position4.yMax;
			p2.y = rect6.yMax;
			Handles.DrawLine(p, p2);
			GUI.Label(position4, instructionRect.height.ToString());
			GUI.Label(position2, string.Format("({0},{1})", instructionRect.xMax, instructionRect.yMax), this.styles.centeredLabel);
			GUI.Box(rect2, GUIContent.none);
		}
	}
}
