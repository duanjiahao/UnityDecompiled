using System;
using System.Collections.Generic;
using UnityEditor.U2D.Interface;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.U2D.Interface;

namespace UnityEditor
{
	internal abstract class SpriteFrameModuleBase : ISpriteEditorModule
	{
		protected enum GizmoMode
		{
			BorderEditing,
			RectEditing
		}

		protected class Styles
		{
			public readonly GUIStyle dragdot = "U2D.dragDot";

			public readonly GUIStyle dragdotactive = "U2D.dragDotActive";

			public readonly GUIStyle createRect = "U2D.createRect";

			public readonly GUIStyle pivotdotactive = "U2D.pivotDotActive";

			public readonly GUIStyle pivotdot = "U2D.pivotDot";

			public readonly GUIStyle dragBorderdot = new GUIStyle();

			public readonly GUIStyle dragBorderDotActive = new GUIStyle();

			public readonly GUIStyle toolbar;

			public readonly GUIContent[] spriteAlignmentOptions = new GUIContent[]
			{
				EditorGUIUtility.TextContent("Center"),
				EditorGUIUtility.TextContent("Top Left"),
				EditorGUIUtility.TextContent("Top"),
				EditorGUIUtility.TextContent("Top Right"),
				EditorGUIUtility.TextContent("Left"),
				EditorGUIUtility.TextContent("Right"),
				EditorGUIUtility.TextContent("Bottom Left"),
				EditorGUIUtility.TextContent("Bottom"),
				EditorGUIUtility.TextContent("Bottom Right"),
				EditorGUIUtility.TextContent("Custom")
			};

			public readonly GUIContent pivotLabel = EditorGUIUtility.TextContent("Pivot");

			public readonly GUIContent spriteLabel = EditorGUIUtility.TextContent("Sprite");

			public readonly GUIContent customPivotLabel = EditorGUIUtility.TextContent("Custom Pivot");

			public readonly GUIContent borderLabel = EditorGUIUtility.TextContent("Border");

			public readonly GUIContent lLabel = EditorGUIUtility.TextContent("L");

			public readonly GUIContent tLabel = EditorGUIUtility.TextContent("T");

			public readonly GUIContent rLabel = EditorGUIUtility.TextContent("R");

			public readonly GUIContent bLabel = EditorGUIUtility.TextContent("B");

			public readonly GUIContent positionLabel = EditorGUIUtility.TextContent("Position");

			public readonly GUIContent xLabel = EditorGUIUtility.TextContent("X");

			public readonly GUIContent yLabel = EditorGUIUtility.TextContent("Y");

			public readonly GUIContent wLabel = EditorGUIUtility.TextContent("W");

			public readonly GUIContent hLabel = EditorGUIUtility.TextContent("H");

			public readonly GUIContent nameLabel = EditorGUIUtility.TextContent("Name");

			public Styles()
			{
				this.toolbar = new GUIStyle(EditorStyles.inspectorBig);
				this.toolbar.margin.top = 0;
				this.toolbar.margin.bottom = 0;
				this.createRect.border = new RectOffset(3, 3, 3, 3);
				this.dragBorderdot.fixedHeight = 5f;
				this.dragBorderdot.fixedWidth = 5f;
				this.dragBorderdot.normal.background = EditorGUIUtility.whiteTexture;
				this.dragBorderDotActive.fixedHeight = this.dragBorderdot.fixedHeight;
				this.dragBorderDotActive.fixedWidth = this.dragBorderdot.fixedWidth;
				this.dragBorderDotActive.normal.background = EditorGUIUtility.whiteTexture;
			}
		}

		protected ISpriteRectCache m_RectsCache;

		private static SpriteFrameModuleBase.Styles s_Styles;

		private const float kScrollbarMargin = 16f;

		private const float kInspectorWindowMargin = 8f;

		private const float kInspectorWidth = 330f;

		private const float kInspectorHeight = 160f;

		private float m_Zoom = 1f;

		private SpriteFrameModuleBase.GizmoMode m_GizmoMode;

		public string moduleName
		{
			get;
			private set;
		}

		protected IEventSystem eventSystem
		{
			get;
			private set;
		}

		protected IUndoSystem undoSystem
		{
			get;
			private set;
		}

		protected ISpriteEditor spriteEditor
		{
			get;
			private set;
		}

		protected IAssetDatabase assetDatabase
		{
			get;
			private set;
		}

		protected SpriteRect selected
		{
			get
			{
				return this.spriteEditor.selectedSpriteRect;
			}
			set
			{
				this.spriteEditor.selectedSpriteRect = value;
			}
		}

		protected SpriteImportMode spriteImportMode
		{
			get;
			private set;
		}

		protected string spriteAssetPath
		{
			get
			{
				return this.assetDatabase.GetAssetPath(this.spriteEditor.selectedTexture);
			}
		}

		protected ITexture2D previewTexture
		{
			get
			{
				return this.spriteEditor.previewTexture;
			}
		}

		public bool hasSelected
		{
			get
			{
				return this.spriteEditor.selectedSpriteRect != null;
			}
		}

		public SpriteAlignment selectedSpriteAlignment
		{
			get
			{
				return this.selected.alignment;
			}
		}

		public Vector2 selectedSpritePivot
		{
			get
			{
				return this.selected.pivot;
			}
		}

		public Vector4 selectedSpriteBorder
		{
			get
			{
				return SpriteFrameModuleBase.ClampSpriteBorderToRect(this.selected.border, this.selected.rect);
			}
			set
			{
				this.undoSystem.RegisterCompleteObjectUndo(this.m_RectsCache, "Change Sprite Border");
				this.spriteEditor.SetDataModified();
				this.selected.border = SpriteFrameModuleBase.ClampSpriteBorderToRect(value, this.selected.rect);
			}
		}

		public Rect selectedSpriteRect
		{
			get
			{
				return this.selected.rect;
			}
			set
			{
				this.undoSystem.RegisterCompleteObjectUndo(this.m_RectsCache, "Change Sprite rect");
				this.spriteEditor.SetDataModified();
				this.selected.rect = SpriteFrameModuleBase.ClampSpriteRect(value, (float)this.previewTexture.width, (float)this.previewTexture.height);
			}
		}

		public string selectedSpriteName
		{
			get
			{
				return this.selected.name;
			}
			set
			{
				this.undoSystem.RegisterCompleteObjectUndo(this.m_RectsCache, "Change Sprite Name");
				this.spriteEditor.SetDataModified();
				string name = this.selected.name;
				string text = InternalEditorUtility.RemoveInvalidCharsFromFileName(value, true);
				if (string.IsNullOrEmpty(this.selected.originalName) && text != name)
				{
					this.selected.originalName = name;
				}
				if (string.IsNullOrEmpty(text))
				{
					text = name;
				}
				for (int i = 0; i < this.m_RectsCache.Count; i++)
				{
					if (this.m_RectsCache.RectAt(i).name == text)
					{
						text = this.selected.originalName;
						break;
					}
				}
				this.selected.name = text;
			}
		}

		public int spriteCount
		{
			get
			{
				return this.m_RectsCache.Count;
			}
		}

		public bool containsMultipleSprites
		{
			get
			{
				return this.spriteImportMode == SpriteImportMode.Multiple;
			}
		}

		protected static SpriteFrameModuleBase.Styles styles
		{
			get
			{
				if (SpriteFrameModuleBase.s_Styles == null)
				{
					SpriteFrameModuleBase.s_Styles = new SpriteFrameModuleBase.Styles();
				}
				return SpriteFrameModuleBase.s_Styles;
			}
		}

		private Rect inspectorRect
		{
			get
			{
				Rect windowDimension = this.spriteEditor.windowDimension;
				return new Rect(windowDimension.width - 330f - 8f - 16f, windowDimension.height - 160f - 8f - 16f, 330f, 160f);
			}
		}

		protected SpriteFrameModuleBase(string name, ISpriteEditor sw, IEventSystem es, IUndoSystem us, IAssetDatabase ad)
		{
			this.spriteEditor = sw;
			this.eventSystem = es;
			this.undoSystem = us;
			this.assetDatabase = ad;
			this.moduleName = name;
		}

		public abstract bool CanBeActivated();

		public virtual void OnModuleActivate()
		{
			this.spriteImportMode = SpriteUtility.GetSpriteImportMode(this.assetDatabase, this.spriteEditor.selectedTexture);
		}

		public abstract void OnModuleDeactivate();

		public int CurrentSelectedSpriteIndex()
		{
			int result;
			for (int i = 0; i < this.m_RectsCache.Count; i++)
			{
				if (this.m_RectsCache.RectAt(i) == this.selected)
				{
					result = i;
					return result;
				}
			}
			result = -1;
			return result;
		}

		public Vector4 GetSpriteBorderAt(int i)
		{
			return this.m_RectsCache.RectAt(i).border;
		}

		public Rect GetSpriteRectAt(int i)
		{
			return this.m_RectsCache.RectAt(i).rect;
		}

		public List<SpriteOutline> GetSpriteOutlineAt(int i)
		{
			return this.m_RectsCache.RectAt(i).outline;
		}

		public void SetSpritePivotAndAlignment(Vector2 pivot, SpriteAlignment alignment)
		{
			this.undoSystem.RegisterCompleteObjectUndo(this.m_RectsCache, "Change Sprite Pivot");
			this.spriteEditor.SetDataModified();
			this.selected.alignment = alignment;
			this.selected.pivot = SpriteEditorUtility.GetPivotValue(alignment, pivot);
		}

		protected void SnapPivot(Vector2 pivot, out Vector2 outPivot, out SpriteAlignment outAlignment)
		{
			Rect selectedSpriteRect = this.selectedSpriteRect;
			Vector2 a = new Vector2(selectedSpriteRect.xMin + selectedSpriteRect.width * pivot.x, selectedSpriteRect.yMin + selectedSpriteRect.height * pivot.y);
			Vector2[] snapPointsArray = SpriteFrameModuleBase.GetSnapPointsArray(selectedSpriteRect);
			SpriteAlignment spriteAlignment = SpriteAlignment.Custom;
			float num = 3.40282347E+38f;
			for (int i = 0; i < snapPointsArray.Length; i++)
			{
				float num2 = (a - snapPointsArray[i]).magnitude * this.m_Zoom;
				if (num2 < num)
				{
					spriteAlignment = (SpriteAlignment)i;
					num = num2;
				}
			}
			outAlignment = spriteAlignment;
			outPivot = SpriteFrameModuleBase.ConvertFromTextureToNormalizedSpace(snapPointsArray[(int)spriteAlignment], selectedSpriteRect);
		}

		protected static Rect ClampSpriteRect(Rect rect, float maxX, float maxY)
		{
			Rect rect2 = default(Rect);
			rect2.xMin = Mathf.Clamp(rect.xMin, 0f, maxX - 1f);
			rect2.yMin = Mathf.Clamp(rect.yMin, 0f, maxY - 1f);
			rect2.xMax = Mathf.Clamp(rect.xMax, 1f, maxX);
			rect2.yMax = Mathf.Clamp(rect.yMax, 1f, maxY);
			if (Mathf.RoundToInt(rect2.width) == 0)
			{
				rect2.width = 1f;
			}
			if (Mathf.RoundToInt(rect2.height) == 0)
			{
				rect2.height = 1f;
			}
			return SpriteEditorUtility.RoundedRect(rect2);
		}

		protected static Rect FlipNegativeRect(Rect rect)
		{
			return new Rect
			{
				xMin = Mathf.Min(rect.xMin, rect.xMax),
				yMin = Mathf.Min(rect.yMin, rect.yMax),
				xMax = Mathf.Max(rect.xMin, rect.xMax),
				yMax = Mathf.Max(rect.yMin, rect.yMax)
			};
		}

		protected static Vector4 ClampSpriteBorderToRect(Vector4 border, Rect rect)
		{
			Rect rect2 = SpriteFrameModuleBase.FlipNegativeRect(rect);
			float width = rect2.width;
			float height = rect2.height;
			Vector4 result = default(Vector4);
			result.x = (float)Mathf.RoundToInt(Mathf.Clamp(border.x, 0f, Mathf.Min(Mathf.Abs(width - border.z), width)));
			result.z = (float)Mathf.RoundToInt(Mathf.Clamp(border.z, 0f, Mathf.Min(Mathf.Abs(width - result.x), width)));
			result.y = (float)Mathf.RoundToInt(Mathf.Clamp(border.y, 0f, Mathf.Min(Mathf.Abs(height - border.w), height)));
			result.w = (float)Mathf.RoundToInt(Mathf.Clamp(border.w, 0f, Mathf.Min(Mathf.Abs(height - result.y), height)));
			return result;
		}

		private bool ShouldShowRectScaling()
		{
			return this.hasSelected && this.m_GizmoMode == SpriteFrameModuleBase.GizmoMode.RectEditing;
		}

		private void DoPivotFields()
		{
			EditorGUI.BeginChangeCheck();
			SpriteAlignment spriteAlignment = this.selectedSpriteAlignment;
			spriteAlignment = (SpriteAlignment)EditorGUILayout.Popup(SpriteFrameModuleBase.styles.pivotLabel, (int)spriteAlignment, SpriteFrameModuleBase.styles.spriteAlignmentOptions, new GUILayoutOption[0]);
			Vector2 selectedSpritePivot = this.selectedSpritePivot;
			Vector2 pivot = selectedSpritePivot;
			using (new EditorGUI.DisabledScope(spriteAlignment != SpriteAlignment.Custom))
			{
				Rect rect = GUILayoutUtility.GetRect(322f, EditorGUI.GetPropertyHeight(SerializedPropertyType.Vector2, SpriteFrameModuleBase.styles.customPivotLabel));
				GUI.SetNextControlName("PivotField");
				pivot = EditorGUI.Vector2Field(rect, SpriteFrameModuleBase.styles.customPivotLabel, selectedSpritePivot);
			}
			if (EditorGUI.EndChangeCheck())
			{
				this.SetSpritePivotAndAlignment(pivot, spriteAlignment);
			}
		}

		private void DoBorderFields()
		{
			EditorGUI.BeginChangeCheck();
			Vector4 selectedSpriteBorder = this.selectedSpriteBorder;
			int num = Mathf.RoundToInt(selectedSpriteBorder.x);
			int num2 = Mathf.RoundToInt(selectedSpriteBorder.y);
			int num3 = Mathf.RoundToInt(selectedSpriteBorder.z);
			int num4 = Mathf.RoundToInt(selectedSpriteBorder.w);
			SpriteEditorUtility.FourIntFields(new Vector2(322f, 32f), SpriteFrameModuleBase.styles.borderLabel, SpriteFrameModuleBase.styles.lLabel, SpriteFrameModuleBase.styles.tLabel, SpriteFrameModuleBase.styles.rLabel, SpriteFrameModuleBase.styles.bLabel, ref num, ref num4, ref num3, ref num2);
			Vector4 selectedSpriteBorder2 = new Vector4((float)num, (float)num2, (float)num3, (float)num4);
			if (EditorGUI.EndChangeCheck())
			{
				this.selectedSpriteBorder = selectedSpriteBorder2;
			}
		}

		private void DoPositionField()
		{
			EditorGUI.BeginChangeCheck();
			Rect selectedSpriteRect = this.selectedSpriteRect;
			int num = Mathf.RoundToInt(selectedSpriteRect.x);
			int num2 = Mathf.RoundToInt(selectedSpriteRect.y);
			int num3 = Mathf.RoundToInt(selectedSpriteRect.width);
			int num4 = Mathf.RoundToInt(selectedSpriteRect.height);
			SpriteEditorUtility.FourIntFields(new Vector2(322f, 32f), SpriteFrameModuleBase.styles.positionLabel, SpriteFrameModuleBase.styles.xLabel, SpriteFrameModuleBase.styles.yLabel, SpriteFrameModuleBase.styles.wLabel, SpriteFrameModuleBase.styles.hLabel, ref num, ref num2, ref num3, ref num4);
			if (EditorGUI.EndChangeCheck())
			{
				this.selectedSpriteRect = new Rect((float)num, (float)num2, (float)num3, (float)num4);
			}
		}

		private void DoNameField()
		{
			EditorGUI.BeginChangeCheck();
			string selectedSpriteName = this.selectedSpriteName;
			GUI.SetNextControlName("SpriteName");
			string selectedSpriteName2 = EditorGUILayout.TextField(SpriteFrameModuleBase.styles.nameLabel, selectedSpriteName, new GUILayoutOption[0]);
			if (EditorGUI.EndChangeCheck())
			{
				this.selectedSpriteName = selectedSpriteName2;
			}
		}

		private void DoSelectedFrameInspector()
		{
			if (this.hasSelected)
			{
				EditorGUIUtility.wideMode = true;
				float labelWidth = EditorGUIUtility.labelWidth;
				EditorGUIUtility.labelWidth = 135f;
				GUILayout.BeginArea(this.inspectorRect);
				GUILayout.BeginVertical(SpriteFrameModuleBase.styles.spriteLabel, GUI.skin.window, new GUILayoutOption[0]);
				using (new EditorGUI.DisabledScope(!this.containsMultipleSprites))
				{
					this.DoNameField();
					this.DoPositionField();
				}
				this.DoBorderFields();
				this.DoPivotFields();
				GUILayout.EndVertical();
				GUILayout.EndArea();
				EditorGUIUtility.labelWidth = labelWidth;
			}
		}

		private static Vector2 ApplySpriteAlignmentToPivot(Vector2 pivot, Rect rect, SpriteAlignment alignment)
		{
			Vector2 result;
			if (alignment != SpriteAlignment.Custom)
			{
				Vector2[] snapPointsArray = SpriteFrameModuleBase.GetSnapPointsArray(rect);
				Vector2 texturePos = snapPointsArray[(int)alignment];
				result = SpriteFrameModuleBase.ConvertFromTextureToNormalizedSpace(texturePos, rect);
			}
			else
			{
				result = pivot;
			}
			return result;
		}

		private static Vector2 ConvertFromTextureToNormalizedSpace(Vector2 texturePos, Rect rect)
		{
			return new Vector2((texturePos.x - rect.xMin) / rect.width, (texturePos.y - rect.yMin) / rect.height);
		}

		private static Vector2[] GetSnapPointsArray(Rect rect)
		{
			Vector2[] array = new Vector2[9];
			array[1] = new Vector2(rect.xMin, rect.yMax);
			array[2] = new Vector2(rect.center.x, rect.yMax);
			array[3] = new Vector2(rect.xMax, rect.yMax);
			array[4] = new Vector2(rect.xMin, rect.center.y);
			array[0] = new Vector2(rect.center.x, rect.center.y);
			array[5] = new Vector2(rect.xMax, rect.center.y);
			array[6] = new Vector2(rect.xMin, rect.yMin);
			array[7] = new Vector2(rect.center.x, rect.yMin);
			array[8] = new Vector2(rect.xMax, rect.yMin);
			return array;
		}

		protected void Repaint()
		{
			this.spriteEditor.RequestRepaint();
		}

		protected void HandleGizmoMode()
		{
			SpriteFrameModuleBase.GizmoMode gizmoMode = this.m_GizmoMode;
			IEvent current = this.eventSystem.current;
			if (current.control)
			{
				this.m_GizmoMode = SpriteFrameModuleBase.GizmoMode.BorderEditing;
			}
			else
			{
				this.m_GizmoMode = SpriteFrameModuleBase.GizmoMode.RectEditing;
			}
			if (gizmoMode != this.m_GizmoMode && (current.type == EventType.KeyDown || current.type == EventType.KeyUp) && (current.keyCode == KeyCode.LeftControl || current.keyCode == KeyCode.RightControl || current.keyCode == KeyCode.LeftAlt || current.keyCode == KeyCode.RightAlt))
			{
				this.Repaint();
			}
		}

		protected bool MouseOnTopOfInspector()
		{
			bool result;
			if (!this.hasSelected)
			{
				result = false;
			}
			else
			{
				Vector2 point = GUIClip.Unclip(this.eventSystem.current.mousePosition) - (GUIClip.topmostRect.position - GUIClip.GetTopRect().position);
				result = this.inspectorRect.Contains(point);
			}
			return result;
		}

		protected void HandlePivotHandle()
		{
			if (this.hasSelected)
			{
				EditorGUI.BeginChangeCheck();
				SpriteAlignment alignment = this.selectedSpriteAlignment;
				Vector2 vector = this.selectedSpritePivot;
				Rect selectedSpriteRect = this.selectedSpriteRect;
				vector = SpriteFrameModuleBase.ApplySpriteAlignmentToPivot(vector, selectedSpriteRect, alignment);
				Vector2 vector2 = SpriteEditorHandles.PivotSlider(selectedSpriteRect, vector, SpriteFrameModuleBase.styles.pivotdot, SpriteFrameModuleBase.styles.pivotdotactive);
				if (EditorGUI.EndChangeCheck())
				{
					if (this.eventSystem.current.control)
					{
						this.SnapPivot(vector2, out vector, out alignment);
					}
					else
					{
						vector = vector2;
						alignment = SpriteAlignment.Custom;
					}
					this.SetSpritePivotAndAlignment(vector, alignment);
				}
			}
		}

		protected void HandleBorderSidePointScalingSliders()
		{
			if (this.hasSelected)
			{
				GUIStyle dragBorderdot = SpriteFrameModuleBase.styles.dragBorderdot;
				GUIStyle dragBorderDotActive = SpriteFrameModuleBase.styles.dragBorderDotActive;
				Color color = new Color(0f, 1f, 0f);
				Rect selectedSpriteRect = this.selectedSpriteRect;
				Vector4 selectedSpriteBorder = this.selectedSpriteBorder;
				float num = selectedSpriteRect.xMin + selectedSpriteBorder.x;
				float num2 = selectedSpriteRect.xMax - selectedSpriteBorder.z;
				float num3 = selectedSpriteRect.yMax - selectedSpriteBorder.w;
				float num4 = selectedSpriteRect.yMin + selectedSpriteBorder.y;
				EditorGUI.BeginChangeCheck();
				float num5 = num4 - (num4 - num3) / 2f;
				float num6 = num - (num - num2) / 2f;
				float num7 = num5;
				this.HandleBorderPointSlider(ref num, ref num7, MouseCursor.ResizeHorizontal, false, dragBorderdot, dragBorderDotActive, color);
				num7 = num5;
				this.HandleBorderPointSlider(ref num2, ref num7, MouseCursor.ResizeHorizontal, false, dragBorderdot, dragBorderDotActive, color);
				num7 = num6;
				this.HandleBorderPointSlider(ref num7, ref num3, MouseCursor.ResizeVertical, false, dragBorderdot, dragBorderDotActive, color);
				num7 = num6;
				this.HandleBorderPointSlider(ref num7, ref num4, MouseCursor.ResizeVertical, false, dragBorderdot, dragBorderDotActive, color);
				if (EditorGUI.EndChangeCheck())
				{
					selectedSpriteBorder.x = num - selectedSpriteRect.xMin;
					selectedSpriteBorder.z = selectedSpriteRect.xMax - num2;
					selectedSpriteBorder.w = selectedSpriteRect.yMax - num3;
					selectedSpriteBorder.y = num4 - selectedSpriteRect.yMin;
					this.selectedSpriteBorder = selectedSpriteBorder;
				}
			}
		}

		protected void HandleBorderCornerScalingHandles()
		{
			if (this.hasSelected)
			{
				GUIStyle dragBorderdot = SpriteFrameModuleBase.styles.dragBorderdot;
				GUIStyle dragBorderDotActive = SpriteFrameModuleBase.styles.dragBorderDotActive;
				Color color = new Color(0f, 1f, 0f);
				Rect selectedSpriteRect = this.selectedSpriteRect;
				Vector4 selectedSpriteBorder = this.selectedSpriteBorder;
				float num = selectedSpriteRect.xMin + selectedSpriteBorder.x;
				float num2 = selectedSpriteRect.xMax - selectedSpriteBorder.z;
				float num3 = selectedSpriteRect.yMax - selectedSpriteBorder.w;
				float num4 = selectedSpriteRect.yMin + selectedSpriteBorder.y;
				EditorGUI.BeginChangeCheck();
				this.HandleBorderPointSlider(ref num, ref num3, MouseCursor.ResizeUpLeft, selectedSpriteBorder.x < 1f && selectedSpriteBorder.w < 1f, dragBorderdot, dragBorderDotActive, color);
				this.HandleBorderPointSlider(ref num2, ref num3, MouseCursor.ResizeUpRight, selectedSpriteBorder.z < 1f && selectedSpriteBorder.w < 1f, dragBorderdot, dragBorderDotActive, color);
				this.HandleBorderPointSlider(ref num, ref num4, MouseCursor.ResizeUpRight, selectedSpriteBorder.x < 1f && selectedSpriteBorder.y < 1f, dragBorderdot, dragBorderDotActive, color);
				this.HandleBorderPointSlider(ref num2, ref num4, MouseCursor.ResizeUpLeft, selectedSpriteBorder.z < 1f && selectedSpriteBorder.y < 1f, dragBorderdot, dragBorderDotActive, color);
				if (EditorGUI.EndChangeCheck())
				{
					selectedSpriteBorder.x = num - selectedSpriteRect.xMin;
					selectedSpriteBorder.z = selectedSpriteRect.xMax - num2;
					selectedSpriteBorder.w = selectedSpriteRect.yMax - num3;
					selectedSpriteBorder.y = num4 - selectedSpriteRect.yMin;
					this.selectedSpriteBorder = selectedSpriteBorder;
				}
			}
		}

		protected void HandleBorderSideScalingHandles()
		{
			if (this.hasSelected)
			{
				Rect rect = new Rect(this.selectedSpriteRect);
				Vector4 selectedSpriteBorder = this.selectedSpriteBorder;
				float num = rect.xMin + selectedSpriteBorder.x;
				float num2 = rect.xMax - selectedSpriteBorder.z;
				float num3 = rect.yMax - selectedSpriteBorder.w;
				float num4 = rect.yMin + selectedSpriteBorder.y;
				Vector2 vector = Handles.matrix.MultiplyPoint(new Vector3(rect.xMin, rect.yMin));
				Vector2 vector2 = Handles.matrix.MultiplyPoint(new Vector3(rect.xMax, rect.yMax));
				float width = Mathf.Abs(vector2.x - vector.x);
				float height = Mathf.Abs(vector2.y - vector.y);
				EditorGUI.BeginChangeCheck();
				num = this.HandleBorderScaleSlider(num, rect.yMax, width, height, true);
				num2 = this.HandleBorderScaleSlider(num2, rect.yMax, width, height, true);
				num3 = this.HandleBorderScaleSlider(rect.xMin, num3, width, height, false);
				num4 = this.HandleBorderScaleSlider(rect.xMin, num4, width, height, false);
				if (EditorGUI.EndChangeCheck())
				{
					selectedSpriteBorder.x = num - rect.xMin;
					selectedSpriteBorder.z = rect.xMax - num2;
					selectedSpriteBorder.w = rect.yMax - num3;
					selectedSpriteBorder.y = num4 - rect.yMin;
					this.selectedSpriteBorder = selectedSpriteBorder;
				}
			}
		}

		protected void HandleBorderPointSlider(ref float x, ref float y, MouseCursor mouseCursor, bool isHidden, GUIStyle dragDot, GUIStyle dragDotActive, Color color)
		{
			Color color2 = GUI.color;
			if (isHidden)
			{
				GUI.color = new Color(0f, 0f, 0f, 0f);
			}
			else
			{
				GUI.color = color;
			}
			Vector2 vector = SpriteEditorHandles.PointSlider(new Vector2(x, y), mouseCursor, dragDot, dragDotActive);
			x = vector.x;
			y = vector.y;
			GUI.color = color2;
		}

		protected float HandleBorderScaleSlider(float x, float y, float width, float height, bool isHorizontal)
		{
			float fixedWidth = SpriteFrameModuleBase.styles.dragBorderdot.fixedWidth;
			Vector2 pos = Handles.matrix.MultiplyPoint(new Vector2(x, y));
			EditorGUI.BeginChangeCheck();
			float num;
			if (isHorizontal)
			{
				Rect cursorRect = new Rect(pos.x - fixedWidth * 0.5f, pos.y, fixedWidth, height);
				num = SpriteEditorHandles.ScaleSlider(pos, MouseCursor.ResizeHorizontal, cursorRect).x;
			}
			else
			{
				Rect cursorRect2 = new Rect(pos.x, pos.y - fixedWidth * 0.5f, width, fixedWidth);
				num = SpriteEditorHandles.ScaleSlider(pos, MouseCursor.ResizeVertical, cursorRect2).y;
			}
			float result;
			if (EditorGUI.EndChangeCheck())
			{
				result = num;
			}
			else
			{
				result = ((!isHorizontal) ? y : x);
			}
			return result;
		}

		protected void DrawSpriteRectGizmos()
		{
			if (this.eventSystem.current.type == EventType.Repaint)
			{
				SpriteEditorUtility.BeginLines(new Color(0f, 1f, 0f, 0.7f));
				int num = this.CurrentSelectedSpriteIndex();
				int i = 0;
				while (i < this.spriteCount)
				{
					Vector4 spriteBorderAt = this.GetSpriteBorderAt(i);
					if (num == i || this.m_GizmoMode == SpriteFrameModuleBase.GizmoMode.BorderEditing)
					{
						goto IL_80;
					}
					if (!Mathf.Approximately(spriteBorderAt.sqrMagnitude, 0f))
					{
						goto IL_80;
					}
					IL_175:
					i++;
					continue;
					IL_80:
					Rect spriteRectAt = this.GetSpriteRectAt(i);
					SpriteEditorUtility.DrawLine(new Vector3(spriteRectAt.xMin + spriteBorderAt.x, spriteRectAt.yMin), new Vector3(spriteRectAt.xMin + spriteBorderAt.x, spriteRectAt.yMax));
					SpriteEditorUtility.DrawLine(new Vector3(spriteRectAt.xMax - spriteBorderAt.z, spriteRectAt.yMin), new Vector3(spriteRectAt.xMax - spriteBorderAt.z, spriteRectAt.yMax));
					SpriteEditorUtility.DrawLine(new Vector3(spriteRectAt.xMin, spriteRectAt.yMin + spriteBorderAt.y), new Vector3(spriteRectAt.xMax, spriteRectAt.yMin + spriteBorderAt.y));
					SpriteEditorUtility.DrawLine(new Vector3(spriteRectAt.xMin, spriteRectAt.yMax - spriteBorderAt.w), new Vector3(spriteRectAt.xMax, spriteRectAt.yMax - spriteBorderAt.w));
					goto IL_175;
				}
				SpriteEditorUtility.EndLines();
				if (this.ShouldShowRectScaling())
				{
					Rect selectedSpriteRect = this.selectedSpriteRect;
					SpriteEditorUtility.BeginLines(new Color(0f, 0.1f, 0.3f, 0.25f));
					SpriteEditorUtility.DrawBox(new Rect(selectedSpriteRect.xMin + 1f / this.m_Zoom, selectedSpriteRect.yMin + 1f / this.m_Zoom, selectedSpriteRect.width, selectedSpriteRect.height));
					SpriteEditorUtility.EndLines();
					SpriteEditorUtility.BeginLines(new Color(0.25f, 0.5f, 1f, 0.75f));
					SpriteEditorUtility.DrawBox(selectedSpriteRect);
					SpriteEditorUtility.EndLines();
				}
			}
		}

		public virtual void DoTextureGUI()
		{
			this.m_Zoom = Handles.matrix.GetColumn(0).magnitude;
		}

		public virtual void OnPostGUI()
		{
			this.DoSelectedFrameInspector();
		}

		public abstract void DrawToolbarGUI(Rect drawArea);
	}
}
