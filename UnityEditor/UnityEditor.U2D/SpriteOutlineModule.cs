using System;
using System.Collections.Generic;
using UnityEditor.Sprites;
using UnityEditor.U2D.Interface;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.U2D.Interface;

namespace UnityEditor.U2D
{
	internal class SpriteOutlineModule : ISpriteEditorModule
	{
		private class Styles
		{
			public GUIContent generateOutlineLabel = EditorGUIUtility.TextContent("Update|Update new outline based on mesh detail value.");

			public GUIContent outlineTolerance = EditorGUIUtility.TextContent("Outline Tolerance|Sets how tight the outline should be from the sprite.");

			public GUIContent snapButtonLabel = EditorGUIUtility.TextContent("Snap|Snap points to nearest pixel");

			public GUIContent generatingOutlineDialogTitle = EditorGUIUtility.TextContent("Outline");

			public GUIContent generatingOutlineDialogContent = EditorGUIUtility.TextContent("Generating outline {0}/{1}");

			public Color spriteBorderColor = new Color(0.25f, 0.5f, 1f, 0.75f);
		}

		protected SpriteRect m_Selected;

		private const float k_HandleSize = 5f;

		private readonly string k_DeleteCommandName = "Delete";

		private readonly string k_SoftDeleteCommandName = "SoftDelete";

		private ShapeEditor[] m_ShapeEditors;

		private bool m_RequestRepaint;

		private Matrix4x4 m_HandleMatrix;

		private Vector2 m_MousePosition;

		private bool m_Snap;

		private ShapeEditorRectSelectionTool m_ShapeSelectionUI;

		private bool m_WasRectSelecting = false;

		private Rect? m_SelectionRect;

		private ITexture2D m_OutlineTexture;

		private SpriteOutlineModule.Styles m_Styles;

		public virtual string moduleName
		{
			get
			{
				return "Edit Outline";
			}
		}

		private SpriteOutlineModule.Styles styles
		{
			get
			{
				if (this.m_Styles == null)
				{
					this.m_Styles = new SpriteOutlineModule.Styles();
				}
				return this.m_Styles;
			}
		}

		private List<SpriteOutline> selectedShapeOutline
		{
			get
			{
				return this.m_Selected.outline;
			}
			set
			{
				this.m_Selected.outline = value;
			}
		}

		private bool shapeEditorDirty
		{
			get;
			set;
		}

		private bool editingDisabled
		{
			get
			{
				return this.spriteEditorWindow.editingDisabled;
			}
		}

		private ISpriteEditor spriteEditorWindow
		{
			get;
			set;
		}

		private IUndoSystem undoSystem
		{
			get;
			set;
		}

		private IEventSystem eventSystem
		{
			get;
			set;
		}

		private IAssetDatabase assetDatabase
		{
			get;
			set;
		}

		private IGUIUtility guiUtility
		{
			get;
			set;
		}

		private IShapeEditorFactory shapeEditorFactory
		{
			get;
			set;
		}

		public SpriteOutlineModule(ISpriteEditor sem, IEventSystem es, IUndoSystem us, IAssetDatabase ad, IGUIUtility gu, IShapeEditorFactory sef, ITexture2D outlineTexture)
		{
			this.spriteEditorWindow = sem;
			this.undoSystem = us;
			this.eventSystem = es;
			this.assetDatabase = ad;
			this.guiUtility = gu;
			this.shapeEditorFactory = sef;
			this.m_OutlineTexture = outlineTexture;
			this.m_ShapeSelectionUI = new ShapeEditorRectSelectionTool(gu);
			this.m_ShapeSelectionUI.RectSelect += new Action<Rect, ShapeEditor.SelectionType>(this.RectSelect);
			this.m_ShapeSelectionUI.ClearSelection += new Action(this.ClearSelection);
		}

		private void RectSelect(Rect r, ShapeEditor.SelectionType selectionType)
		{
			Rect value = EditorGUIExt.FromToRect(this.ScreenToLocal(r.min), this.ScreenToLocal(r.max));
			this.m_SelectionRect = new Rect?(value);
		}

		private void ClearSelection()
		{
			this.m_RequestRepaint = true;
		}

		public void OnModuleActivate()
		{
			this.GenerateOutlineIfNotExist();
			this.undoSystem.RegisterUndoCallback(new Undo.UndoRedoCallback(this.UndoRedoPerformed));
			this.shapeEditorDirty = true;
			this.SetupShapeEditor();
			this.spriteEditorWindow.enableMouseMoveEvent = true;
		}

		private void GenerateOutlineIfNotExist()
		{
			ISpriteRectCache spriteRects = this.spriteEditorWindow.spriteRects;
			if (spriteRects != null)
			{
				for (int i = 0; i < spriteRects.Count; i++)
				{
					SpriteRect spriteRect = spriteRects.RectAt(i);
					if (spriteRect.outline == null || spriteRect.outline.Count == 0)
					{
						this.spriteEditorWindow.DisplayProgressBar(this.styles.generatingOutlineDialogTitle.text, string.Format(this.styles.generatingOutlineDialogContent.text, i + 1, spriteRects.Count), (float)i / (float)spriteRects.Count);
						this.SetupShapeEditorOutline(spriteRect);
					}
				}
				this.spriteEditorWindow.ClearProgressBar();
			}
		}

		public void OnModuleDeactivate()
		{
			this.undoSystem.UnregisterUndoCallback(new Undo.UndoRedoCallback(this.UndoRedoPerformed));
			this.CleanupShapeEditors();
			this.m_Selected = null;
			this.spriteEditorWindow.enableMouseMoveEvent = false;
		}

		public void DoTextureGUI()
		{
			IEvent current = this.eventSystem.current;
			this.m_RequestRepaint = false;
			this.m_HandleMatrix = Handles.matrix;
			this.m_MousePosition = Handles.inverseMatrix.MultiplyPoint(this.eventSystem.current.mousePosition);
			if (this.m_Selected == null || (!this.m_Selected.rect.Contains(this.m_MousePosition) && !this.IsMouseOverOutlinePoints() && !current.shift))
			{
				this.spriteEditorWindow.HandleSpriteSelection();
			}
			this.HandleCreateNewOutline();
			this.m_WasRectSelecting = this.m_ShapeSelectionUI.isSelecting;
			this.UpdateShapeEditors();
			this.m_ShapeSelectionUI.OnGUI();
			this.DrawGizmos();
			if (this.m_RequestRepaint || current.type == EventType.MouseMove)
			{
				this.spriteEditorWindow.RequestRepaint();
			}
		}

		public void DrawToolbarGUI(Rect drawArea)
		{
			SpriteOutlineModule.Styles styles = this.styles;
			Rect position = new Rect(drawArea.x, drawArea.y, EditorStyles.toolbarButton.CalcSize(styles.snapButtonLabel).x, drawArea.height);
			this.m_Snap = GUI.Toggle(position, this.m_Snap, styles.snapButtonLabel, EditorStyles.toolbarButton);
			using (new EditorGUI.DisabledScope(this.editingDisabled || this.m_Selected == null))
			{
				float num = drawArea.width - position.width;
				drawArea.x = position.xMax;
				drawArea.width = EditorStyles.toolbarButton.CalcSize(styles.outlineTolerance).x;
				num -= drawArea.width;
				if (num < 0f)
				{
					drawArea.width += num;
				}
				if (drawArea.width > 0f)
				{
					GUI.Label(drawArea, styles.outlineTolerance, EditorStyles.miniLabel);
				}
				drawArea.x += drawArea.width;
				drawArea.width = 100f;
				num -= drawArea.width;
				if (num < 0f)
				{
					drawArea.width += num;
				}
				if (drawArea.width > 0f)
				{
					float num2 = (this.m_Selected == null) ? 0f : this.m_Selected.tessellationDetail;
					EditorGUI.BeginChangeCheck();
					float fieldWidth = EditorGUIUtility.fieldWidth;
					float labelWidth = EditorGUIUtility.labelWidth;
					EditorGUIUtility.fieldWidth = 30f;
					EditorGUIUtility.labelWidth = 1f;
					num2 = EditorGUI.Slider(drawArea, Mathf.Clamp01(num2), 0f, 1f);
					if (EditorGUI.EndChangeCheck())
					{
						this.RecordUndo();
						this.m_Selected.tessellationDetail = num2;
					}
					EditorGUIUtility.fieldWidth = fieldWidth;
					EditorGUIUtility.labelWidth = labelWidth;
				}
				drawArea.x += drawArea.width;
				drawArea.width = EditorStyles.toolbarButton.CalcSize(styles.generateOutlineLabel).x;
				num -= drawArea.width;
				if (num < 0f)
				{
					drawArea.width += num;
				}
				if (drawArea.width > 0f && GUI.Button(drawArea, styles.generateOutlineLabel, EditorStyles.toolbarButton))
				{
					this.RecordUndo();
					this.selectedShapeOutline.Clear();
					this.SetupShapeEditorOutline(this.m_Selected);
					this.selectedShapeOutline = this.m_Selected.outline;
					this.spriteEditorWindow.SetDataModified();
					this.shapeEditorDirty = true;
				}
			}
		}

		public void OnPostGUI()
		{
		}

		public bool CanBeActivated()
		{
			return UnityEditor.SpriteUtility.GetSpriteImportMode(this.assetDatabase, this.spriteEditorWindow.selectedTexture) != SpriteImportMode.None;
		}

		private void RecordUndo()
		{
			this.undoSystem.RegisterCompleteObjectUndo(this.spriteEditorWindow.spriteRects, "Outline changed");
		}

		public void CreateNewOutline(Rect rectOutline)
		{
			Rect rect = this.m_Selected.rect;
			if (rect.Contains(rectOutline.min) && rect.Contains(rectOutline.max))
			{
				this.RecordUndo();
				SpriteOutline spriteOutline = new SpriteOutline();
				Vector2 b = new Vector2(0.5f * rect.width + rect.x, 0.5f * rect.height + rect.y);
				Rect rect2 = new Rect(rectOutline);
				rect2.min = this.SnapPoint(rectOutline.min);
				rect2.max = this.SnapPoint(rectOutline.max);
				spriteOutline.Add(SpriteOutlineModule.CapPointToRect(new Vector2(rect2.xMin, rect2.yMin), rect) - b);
				spriteOutline.Add(SpriteOutlineModule.CapPointToRect(new Vector2(rect2.xMin, rect2.yMax), rect) - b);
				spriteOutline.Add(SpriteOutlineModule.CapPointToRect(new Vector2(rect2.xMax, rect2.yMax), rect) - b);
				spriteOutline.Add(SpriteOutlineModule.CapPointToRect(new Vector2(rect2.xMax, rect2.yMin), rect) - b);
				this.selectedShapeOutline.Add(spriteOutline);
				this.spriteEditorWindow.SetDataModified();
				this.shapeEditorDirty = true;
			}
		}

		private void HandleCreateNewOutline()
		{
			if (this.m_WasRectSelecting && !this.m_ShapeSelectionUI.isSelecting)
			{
				Rect? selectionRect = this.m_SelectionRect;
				if (selectionRect.HasValue && this.m_Selected != null)
				{
					bool flag = true;
					ShapeEditor[] shapeEditors = this.m_ShapeEditors;
					for (int i = 0; i < shapeEditors.Length; i++)
					{
						ShapeEditor shapeEditor = shapeEditors[i];
						if (shapeEditor.selectedPoints.Count != 0)
						{
							flag = false;
							break;
						}
					}
					if (flag)
					{
						this.CreateNewOutline(this.m_SelectionRect.Value);
					}
				}
			}
			this.m_SelectionRect = null;
		}

		public void UpdateShapeEditors()
		{
			this.SetupShapeEditor();
			if (this.m_Selected != null)
			{
				IEvent current = this.eventSystem.current;
				bool flag = current.type == EventType.ExecuteCommand && (current.commandName == this.k_SoftDeleteCommandName || current.commandName == this.k_DeleteCommandName);
				for (int i = 0; i < this.m_ShapeEditors.Length; i++)
				{
					if (this.m_ShapeEditors[i].GetPointsCount() != 0)
					{
						this.m_ShapeEditors[i].inEditMode = true;
						this.m_ShapeEditors[i].OnGUI();
						if (this.shapeEditorDirty)
						{
							break;
						}
					}
				}
				if (flag)
				{
					for (int j = this.selectedShapeOutline.Count - 1; j >= 0; j--)
					{
						if (this.selectedShapeOutline[j].Count < 3)
						{
							this.selectedShapeOutline.RemoveAt(j);
							this.shapeEditorDirty = true;
						}
					}
				}
			}
		}

		private bool IsMouseOverOutlinePoints()
		{
			bool result;
			if (this.m_Selected == null)
			{
				result = false;
			}
			else
			{
				Vector2 b = new Vector2(0.5f * this.m_Selected.rect.width + this.m_Selected.rect.x, 0.5f * this.m_Selected.rect.height + this.m_Selected.rect.y);
				float handleSize = this.GetHandleSize();
				Rect rect = new Rect(0f, 0f, handleSize * 2f, handleSize * 2f);
				for (int i = 0; i < this.selectedShapeOutline.Count; i++)
				{
					SpriteOutline spriteOutline = this.selectedShapeOutline[i];
					for (int j = 0; j < spriteOutline.Count; j++)
					{
						rect.center = spriteOutline[j] + b;
						if (rect.Contains(this.m_MousePosition))
						{
							result = true;
							return result;
						}
					}
				}
				result = false;
			}
			return result;
		}

		private float GetHandleSize()
		{
			return 5f / this.m_HandleMatrix.m00;
		}

		private void CleanupShapeEditors()
		{
			if (this.m_ShapeEditors != null)
			{
				for (int i = 0; i < this.m_ShapeEditors.Length; i++)
				{
					for (int j = 0; j < this.m_ShapeEditors.Length; j++)
					{
						if (i != j)
						{
							this.m_ShapeEditors[j].UnregisterFromShapeEditor(this.m_ShapeEditors[i]);
						}
					}
					this.m_ShapeEditors[i].OnDisable();
				}
			}
			this.m_ShapeEditors = null;
		}

		public void SetupShapeEditor()
		{
			if (this.shapeEditorDirty || this.m_Selected != this.spriteEditorWindow.selectedSpriteRect)
			{
				this.m_Selected = this.spriteEditorWindow.selectedSpriteRect;
				this.CleanupShapeEditors();
				if (this.m_Selected != null)
				{
					this.SetupShapeEditorOutline(this.m_Selected);
					this.selectedShapeOutline = this.m_Selected.outline;
					this.m_ShapeEditors = new ShapeEditor[this.selectedShapeOutline.Count];
					for (int i = 0; i < this.selectedShapeOutline.Count; i++)
					{
						int outlineIndex = i;
						this.m_ShapeEditors[i] = this.shapeEditorFactory.CreateShapeEditor();
						this.m_ShapeEditors[i].SetRectSelectionTool(this.m_ShapeSelectionUI);
						this.m_ShapeEditors[i].LocalToWorldMatrix = (() => this.m_HandleMatrix);
						this.m_ShapeEditors[i].LocalToScreen = ((Vector3 point) => Handles.matrix.MultiplyPoint(point));
						this.m_ShapeEditors[i].ScreenToLocal = new Func<Vector2, Vector3>(this.ScreenToLocal);
						this.m_ShapeEditors[i].RecordUndo = new Action(this.RecordUndo);
						this.m_ShapeEditors[i].GetHandleSize = new Func<float>(this.GetHandleSize);
						this.m_ShapeEditors[i].lineTexture = this.m_OutlineTexture;
						this.m_ShapeEditors[i].Snap = new Func<Vector3, Vector3>(this.SnapPoint);
						this.m_ShapeEditors[i].GetPointPosition = ((int index) => this.GetPointPosition(outlineIndex, index));
						this.m_ShapeEditors[i].SetPointPosition = delegate(int index, Vector3 position)
						{
							this.SetPointPosition(outlineIndex, index, position);
						};
						this.m_ShapeEditors[i].InsertPointAt = delegate(int index, Vector3 position)
						{
							this.InsertPointAt(outlineIndex, index, position);
						};
						this.m_ShapeEditors[i].RemovePointAt = delegate(int index)
						{
							this.RemovePointAt(outlineIndex, index);
						};
						this.m_ShapeEditors[i].GetPointsCount = (() => this.GetPointsCount(outlineIndex));
					}
					for (int j = 0; j < this.selectedShapeOutline.Count; j++)
					{
						for (int k = 0; k < this.selectedShapeOutline.Count; k++)
						{
							if (j != k)
							{
								this.m_ShapeEditors[k].RegisterToShapeEditor(this.m_ShapeEditors[j]);
							}
						}
					}
				}
				else
				{
					this.m_ShapeEditors = new ShapeEditor[0];
				}
			}
			this.shapeEditorDirty = false;
		}

		protected virtual void SetupShapeEditorOutline(SpriteRect spriteRect)
		{
			if (spriteRect.outline == null || spriteRect.outline.Count == 0)
			{
				spriteRect.outline = SpriteOutlineModule.GenerateSpriteRectOutline(spriteRect.rect, this.spriteEditorWindow.selectedTexture, spriteRect.tessellationDetail, 0);
				if (spriteRect.outline.Count == 0)
				{
					Vector2 vector = spriteRect.rect.size * 0.5f;
					spriteRect.outline = new List<SpriteOutline>
					{
						new SpriteOutline
						{
							m_Path = new List<Vector2>
							{
								new Vector2(-vector.x, -vector.y),
								new Vector2(-vector.x, vector.y),
								new Vector2(vector.x, vector.y),
								new Vector2(vector.x, -vector.y)
							}
						}
					};
				}
				this.spriteEditorWindow.SetDataModified();
			}
		}

		public Vector3 SnapPoint(Vector3 position)
		{
			if (this.m_Snap)
			{
				position.x = (float)Mathf.RoundToInt(position.x);
				position.y = (float)Mathf.RoundToInt(position.y);
			}
			return position;
		}

		public Vector3 GetPointPosition(int outlineIndex, int pointIndex)
		{
			Vector3 result;
			if (outlineIndex >= 0 && outlineIndex < this.selectedShapeOutline.Count)
			{
				SpriteOutline spriteOutline = this.selectedShapeOutline[outlineIndex];
				if (pointIndex >= 0 && pointIndex < spriteOutline.Count)
				{
					result = this.ConvertSpriteRectSpaceToTextureSpace(spriteOutline[pointIndex]);
					return result;
				}
			}
			result = new Vector3(float.NaN, float.NaN, float.NaN);
			return result;
		}

		public void SetPointPosition(int outlineIndex, int pointIndex, Vector3 position)
		{
			this.selectedShapeOutline[outlineIndex][pointIndex] = this.ConvertTextureSpaceToSpriteRectSpace(SpriteOutlineModule.CapPointToRect(position, this.m_Selected.rect));
			this.spriteEditorWindow.SetDataModified();
		}

		public void InsertPointAt(int outlineIndex, int pointIndex, Vector3 position)
		{
			this.selectedShapeOutline[outlineIndex].Insert(pointIndex, this.ConvertTextureSpaceToSpriteRectSpace(SpriteOutlineModule.CapPointToRect(position, this.m_Selected.rect)));
			this.spriteEditorWindow.SetDataModified();
		}

		public void RemovePointAt(int outlineIndex, int i)
		{
			this.selectedShapeOutline[outlineIndex].RemoveAt(i);
			this.spriteEditorWindow.SetDataModified();
		}

		public int GetPointsCount(int outlineIndex)
		{
			return this.selectedShapeOutline[outlineIndex].Count;
		}

		private Vector2 ConvertSpriteRectSpaceToTextureSpace(Vector2 value)
		{
			Vector2 b = new Vector2(0.5f * this.m_Selected.rect.width + this.m_Selected.rect.x, 0.5f * this.m_Selected.rect.height + this.m_Selected.rect.y);
			value += b;
			return value;
		}

		private Vector2 ConvertTextureSpaceToSpriteRectSpace(Vector2 value)
		{
			Vector2 b = new Vector2(0.5f * this.m_Selected.rect.width + this.m_Selected.rect.x, 0.5f * this.m_Selected.rect.height + this.m_Selected.rect.y);
			value -= b;
			return value;
		}

		private Vector3 ScreenToLocal(Vector2 point)
		{
			return Handles.inverseMatrix.MultiplyPoint(point);
		}

		private void UndoRedoPerformed()
		{
			this.shapeEditorDirty = true;
		}

		private void DrawGizmos()
		{
			if (this.eventSystem.current.type == EventType.Layout || this.eventSystem.current.type == EventType.Repaint)
			{
				SpriteRect selectedSpriteRect = this.spriteEditorWindow.selectedSpriteRect;
				if (selectedSpriteRect != null)
				{
					SpriteEditorUtility.BeginLines(this.styles.spriteBorderColor);
					SpriteEditorUtility.DrawBox(selectedSpriteRect.rect);
					SpriteEditorUtility.EndLines();
				}
			}
		}

		private static List<SpriteOutline> GenerateSpriteRectOutline(Rect rect, ITexture2D texture, float detail, byte alphaTolerance)
		{
			List<SpriteOutline> list = new List<SpriteOutline>();
			if (texture != null)
			{
				int num = 0;
				int num2 = 0;
				UnityEditor.TextureImporter textureImporter = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(texture)) as UnityEditor.TextureImporter;
				textureImporter.GetWidthAndHeight(ref num, ref num2);
				int width = texture.width;
				int height = texture.height;
				Vector2 vector = new Vector2((float)width / (float)num, (float)height / (float)num2);
				Rect rect2 = rect;
				rect2.xMin *= vector.x;
				rect2.xMax *= vector.x;
				rect2.yMin *= vector.y;
				rect2.yMax *= vector.y;
				Vector2[][] array;
				UnityEditor.Sprites.SpriteUtility.GenerateOutline(texture, rect2, detail, alphaTolerance, true, out array);
				Rect r = default(Rect);
				r.size = rect.size;
				r.center = Vector2.zero;
				for (int i = 0; i < array.Length; i++)
				{
					SpriteOutline spriteOutline = new SpriteOutline();
					Vector2[] array2 = array[i];
					for (int j = 0; j < array2.Length; j++)
					{
						Vector2 vector2 = array2[j];
						spriteOutline.Add(SpriteOutlineModule.CapPointToRect(new Vector2(vector2.x / vector.x, vector2.y / vector.y), r));
					}
					list.Add(spriteOutline);
				}
			}
			return list;
		}

		private static Vector2 CapPointToRect(Vector2 so, Rect r)
		{
			so.x = Mathf.Min(r.xMax, so.x);
			so.x = Mathf.Max(r.xMin, so.x);
			so.y = Mathf.Min(r.yMax, so.y);
			so.y = Mathf.Max(r.yMin, so.y);
			return so;
		}
	}
}
