using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace UnityEditorInternal
{
	[Serializable]
	internal class DopeSheetEditor : TimeArea, CurveUpdater
	{
		private struct DrawElement
		{
			public Rect position;

			public Color color;

			public Texture2D texture;

			public DrawElement(Rect position, Color color, Texture2D texture)
			{
				this.position = position;
				this.color = color;
				this.texture = texture;
			}
		}

		private class DopeSheetControlPointRenderer
		{
			private List<DopeSheetEditor.DrawElement> m_UnselectedKeysDrawBuffer = new List<DopeSheetEditor.DrawElement>();

			private List<DopeSheetEditor.DrawElement> m_SelectedKeysDrawBuffer = new List<DopeSheetEditor.DrawElement>();

			private List<DopeSheetEditor.DrawElement> m_DragDropKeysDrawBuffer = new List<DopeSheetEditor.DrawElement>();

			private ControlPointRenderer m_UnselectedKeysRenderer;

			private ControlPointRenderer m_SelectedKeysRenderer;

			private ControlPointRenderer m_DragDropKeysRenderer;

			private Texture2D m_DefaultDopeKeyIcon;

			public DopeSheetControlPointRenderer()
			{
				this.m_DefaultDopeKeyIcon = EditorGUIUtility.LoadIcon("blendKey");
				this.m_UnselectedKeysRenderer = new ControlPointRenderer(this.m_DefaultDopeKeyIcon);
				this.m_SelectedKeysRenderer = new ControlPointRenderer(this.m_DefaultDopeKeyIcon);
				this.m_DragDropKeysRenderer = new ControlPointRenderer(this.m_DefaultDopeKeyIcon);
			}

			public void FlushCache()
			{
				this.m_UnselectedKeysRenderer.FlushCache();
				this.m_SelectedKeysRenderer.FlushCache();
				this.m_DragDropKeysRenderer.FlushCache();
			}

			private void DrawElements(List<DopeSheetEditor.DrawElement> elements)
			{
				if (elements.Count != 0)
				{
					Color color = GUI.color;
					Color color2 = Color.white;
					GUI.color = color2;
					Texture defaultDopeKeyIcon = this.m_DefaultDopeKeyIcon;
					for (int i = 0; i < elements.Count; i++)
					{
						DopeSheetEditor.DrawElement drawElement = elements[i];
						if (drawElement.color != color2)
						{
							color2 = ((!GUI.enabled) ? (drawElement.color * 0.8f) : drawElement.color);
							GUI.color = color2;
						}
						if (drawElement.texture != null)
						{
							GUI.Label(drawElement.position, drawElement.texture, GUIStyle.none);
						}
						else
						{
							Rect position = new Rect(drawElement.position.center.x - (float)(defaultDopeKeyIcon.width / 2), drawElement.position.center.y - (float)(defaultDopeKeyIcon.height / 2), (float)defaultDopeKeyIcon.width, (float)defaultDopeKeyIcon.height);
							GUI.Label(position, defaultDopeKeyIcon, GUIStyle.none);
						}
					}
					GUI.color = color;
				}
			}

			public void Clear()
			{
				this.m_UnselectedKeysDrawBuffer.Clear();
				this.m_SelectedKeysDrawBuffer.Clear();
				this.m_DragDropKeysDrawBuffer.Clear();
				this.m_UnselectedKeysRenderer.Clear();
				this.m_SelectedKeysRenderer.Clear();
				this.m_DragDropKeysRenderer.Clear();
			}

			public void Render()
			{
				this.DrawElements(this.m_UnselectedKeysDrawBuffer);
				this.m_UnselectedKeysRenderer.Render();
				this.DrawElements(this.m_SelectedKeysDrawBuffer);
				this.m_SelectedKeysRenderer.Render();
				this.DrawElements(this.m_DragDropKeysDrawBuffer);
				this.m_DragDropKeysRenderer.Render();
			}

			public void AddUnselectedKey(DopeSheetEditor.DrawElement element)
			{
				if (element.texture != null)
				{
					this.m_UnselectedKeysDrawBuffer.Add(element);
				}
				else
				{
					Rect position = element.position;
					position.size = new Vector2((float)this.m_DefaultDopeKeyIcon.width, (float)this.m_DefaultDopeKeyIcon.height);
					this.m_UnselectedKeysRenderer.AddPoint(position, element.color);
				}
			}

			public void AddSelectedKey(DopeSheetEditor.DrawElement element)
			{
				if (element.texture != null)
				{
					this.m_SelectedKeysDrawBuffer.Add(element);
				}
				else
				{
					Rect position = element.position;
					position.size = new Vector2((float)this.m_DefaultDopeKeyIcon.width, (float)this.m_DefaultDopeKeyIcon.height);
					this.m_SelectedKeysRenderer.AddPoint(position, element.color);
				}
			}

			public void AddDragDropKey(DopeSheetEditor.DrawElement element)
			{
				if (element.texture != null)
				{
					this.m_DragDropKeysDrawBuffer.Add(element);
				}
				else
				{
					Rect position = element.position;
					position.size = new Vector2((float)this.m_DefaultDopeKeyIcon.width, (float)this.m_DefaultDopeKeyIcon.height);
					this.m_DragDropKeysRenderer.AddPoint(position, element.color);
				}
			}
		}

		private struct AddKeyToDopelineContext
		{
			public DopeLine dopeline;

			public AnimationKeyTime time;
		}

		internal class DopeSheetSelectionRect
		{
			private enum SelectionType
			{
				Normal,
				Additive,
				Subtractive
			}

			private Vector2 m_SelectStartPoint;

			private Vector2 m_SelectMousePoint;

			private bool m_ValidRect;

			private DopeSheetEditor owner;

			public readonly GUIStyle createRect = "U2D.createRect";

			private static int s_RectSelectionID = GUIUtility.GetPermanentControlID();

			public DopeSheetSelectionRect(DopeSheetEditor owner)
			{
				this.owner = owner;
			}

			public void OnGUI(Rect position)
			{
				Event current = Event.current;
				Vector2 mousePosition = current.mousePosition;
				int num = DopeSheetEditor.DopeSheetSelectionRect.s_RectSelectionID;
				switch (current.GetTypeForControl(num))
				{
				case EventType.MouseDown:
					if (current.button == 0 && position.Contains(mousePosition))
					{
						GUIUtility.hotControl = num;
						this.m_SelectStartPoint = mousePosition;
						this.m_ValidRect = false;
						current.Use();
					}
					break;
				case EventType.MouseUp:
					if (GUIUtility.hotControl == num && current.button == 0)
					{
						if (this.m_ValidRect)
						{
							if (!current.shift && !EditorGUI.actionKey)
							{
								this.owner.state.ClearSelections();
							}
							float frameRate = this.owner.state.frameRate;
							Rect currentTimeRect = this.GetCurrentTimeRect();
							GUI.changed = true;
							this.owner.state.ClearHierarchySelection();
							List<AnimationWindowKeyframe> list = new List<AnimationWindowKeyframe>();
							List<AnimationWindowKeyframe> list2 = new List<AnimationWindowKeyframe>();
							foreach (DopeLine current2 in this.owner.state.dopelines)
							{
								if (current2.position.yMin >= currentTimeRect.yMin && current2.position.yMax <= currentTimeRect.yMax)
								{
									foreach (AnimationWindowKeyframe current3 in current2.keys)
									{
										AnimationKeyTime animationKeyTime = AnimationKeyTime.Time(currentTimeRect.xMin - current3.curve.timeOffset, frameRate);
										AnimationKeyTime animationKeyTime2 = AnimationKeyTime.Time(currentTimeRect.xMax - current3.curve.timeOffset, frameRate);
										AnimationKeyTime animationKeyTime3 = AnimationKeyTime.Time(current3.time, frameRate);
										if ((!current2.tallMode && animationKeyTime3.frame >= animationKeyTime.frame && animationKeyTime3.frame <= animationKeyTime2.frame) || (current2.tallMode && animationKeyTime3.frame >= animationKeyTime.frame && animationKeyTime3.frame < animationKeyTime2.frame))
										{
											if (!list2.Contains(current3) && !list.Contains(current3))
											{
												if (!this.owner.state.KeyIsSelected(current3))
												{
													list2.Add(current3);
												}
												else if (this.owner.state.KeyIsSelected(current3))
												{
													list.Add(current3);
												}
											}
										}
									}
								}
							}
							if (list2.Count == 0)
							{
								foreach (AnimationWindowKeyframe current4 in list)
								{
									this.owner.state.UnselectKey(current4);
								}
							}
							foreach (AnimationWindowKeyframe current5 in list2)
							{
								this.owner.state.SelectKey(current5);
							}
							foreach (DopeLine current6 in this.owner.state.dopelines)
							{
								if (this.owner.state.AnyKeyIsSelected(current6))
								{
									this.owner.state.SelectHierarchyItem(current6, true, false);
								}
							}
						}
						else
						{
							this.owner.state.ClearSelections();
						}
						current.Use();
						GUIUtility.hotControl = 0;
					}
					break;
				case EventType.MouseDrag:
					if (GUIUtility.hotControl == num)
					{
						this.m_ValidRect = (Mathf.Abs((mousePosition - this.m_SelectStartPoint).x) > 1f);
						if (this.m_ValidRect)
						{
							this.m_SelectMousePoint = new Vector2(mousePosition.x, mousePosition.y);
						}
						current.Use();
					}
					break;
				case EventType.Repaint:
					if (GUIUtility.hotControl == num && this.m_ValidRect)
					{
						EditorStyles.selectionRect.Draw(this.GetCurrentPixelRect(), GUIContent.none, false, false, false, false);
					}
					break;
				}
			}

			public Rect GetCurrentPixelRect()
			{
				float num = 16f;
				Rect result = AnimationWindowUtility.FromToRect(this.m_SelectStartPoint, this.m_SelectMousePoint);
				result.xMin = this.owner.state.TimeToPixel(this.owner.state.PixelToTime(result.xMin, AnimationWindowState.SnapMode.SnapToClipFrame), AnimationWindowState.SnapMode.SnapToClipFrame);
				result.xMax = this.owner.state.TimeToPixel(this.owner.state.PixelToTime(result.xMax, AnimationWindowState.SnapMode.SnapToClipFrame), AnimationWindowState.SnapMode.SnapToClipFrame);
				result.yMin = Mathf.Floor(result.yMin / num) * num;
				result.yMax = (Mathf.Floor(result.yMax / num) + 1f) * num;
				return result;
			}

			public Rect GetCurrentTimeRect()
			{
				float num = 16f;
				Rect result = AnimationWindowUtility.FromToRect(this.m_SelectStartPoint, this.m_SelectMousePoint);
				result.xMin = this.owner.state.PixelToTime(result.xMin, AnimationWindowState.SnapMode.SnapToClipFrame);
				result.xMax = this.owner.state.PixelToTime(result.xMax, AnimationWindowState.SnapMode.SnapToClipFrame);
				result.yMin = Mathf.Floor(result.yMin / num) * num;
				result.yMax = (Mathf.Floor(result.yMax / num) + 1f) * num;
				return result;
			}
		}

		public AnimationWindowState state;

		private const float k_KeyframeOffset = -6.5f;

		private const float k_PptrKeyframeOffset = -1f;

		private const int kLabelMarginHorizontal = 8;

		private const int kLabelMarginVertical = 2;

		[SerializeField]
		public EditorWindow m_Owner;

		private DopeSheetEditor.DopeSheetSelectionRect m_SelectionRect;

		private Texture m_DefaultDopeKeyIcon;

		private float m_DragStartTime;

		private bool m_MousedownOnKeyframe;

		private bool m_IsDragging;

		private bool m_IsDraggingPlayheadStarted;

		private bool m_IsDraggingPlayhead;

		private bool m_Initialized;

		private bool m_SpritePreviewLoading;

		private int m_SpritePreviewCacheSize;

		public Bounds m_Bounds = new Bounds(Vector3.zero, Vector3.zero);

		private DopeSheetEditor.DopeSheetControlPointRenderer m_PointRenderer;

		private DopeSheetEditorRectangleTool m_RectangleTool;

		public float contentHeight
		{
			get
			{
				float num = 0f;
				foreach (DopeLine current in this.state.dopelines)
				{
					num += ((!current.tallMode) ? 16f : 32f);
				}
				num += 40f;
				return num;
			}
		}

		public override Bounds drawingBounds
		{
			get
			{
				return this.m_Bounds;
			}
		}

		public bool isDragging
		{
			get
			{
				return this.m_IsDragging;
			}
		}

		internal int assetPreviewManagerID
		{
			get
			{
				return (!(this.m_Owner != null)) ? 0 : this.m_Owner.GetInstanceID();
			}
		}

		public bool spritePreviewLoading
		{
			get
			{
				return this.m_SpritePreviewLoading;
			}
		}

		public DopeSheetEditor(EditorWindow owner) : base(false)
		{
			this.m_Owner = owner;
		}

		public void OnDisable()
		{
			if (this.m_PointRenderer != null)
			{
				this.m_PointRenderer.FlushCache();
			}
		}

		internal void OnDestroy()
		{
			AssetPreview.DeletePreviewTextureManagerByID(this.assetPreviewManagerID);
		}

		public void OnGUI(Rect position, Vector2 scrollPosition)
		{
			this.Init();
			this.HandleDragAndDropToEmptyArea();
			GUIClip.Push(position, scrollPosition, Vector2.zero, false);
			this.HandleRectangleToolEvents();
			Rect position2 = new Rect(0f, 0f, position.width, position.height);
			Rect rect = this.DopelinesGUI(position2, scrollPosition);
			this.HandleKeyboard();
			this.HandleDragging();
			this.HandleSelectionRect(rect);
			this.HandleDelete();
			this.RectangleToolGUI();
			GUIClip.Pop();
		}

		public void Init()
		{
			if (!this.m_Initialized)
			{
				base.hSlider = true;
				base.vSlider = false;
				base.hRangeLocked = false;
				base.vRangeLocked = true;
				base.hRangeMin = 0f;
				base.margin = 40f;
				base.scaleWithWindow = true;
				base.ignoreScrollWheelUntilClicked = false;
			}
			this.m_Initialized = true;
			if (this.m_PointRenderer == null)
			{
				this.m_PointRenderer = new DopeSheetEditor.DopeSheetControlPointRenderer();
			}
			if (this.m_RectangleTool == null)
			{
				this.m_RectangleTool = new DopeSheetEditorRectangleTool();
				this.m_RectangleTool.Initialize(this);
			}
		}

		public void RecalculateBounds()
		{
			if (!this.state.disabled)
			{
				Vector2 timeRange = this.state.timeRange;
				this.m_Bounds.SetMinMax(new Vector3(timeRange.x, 0f, 0f), new Vector3(timeRange.y, 0f, 0f));
			}
		}

		private Rect DopelinesGUI(Rect position, Vector2 scrollPosition)
		{
			Color color = GUI.color;
			Rect position2 = position;
			this.m_PointRenderer.Clear();
			if (Event.current.type == EventType.Repaint)
			{
				this.m_SpritePreviewLoading = false;
			}
			if (Event.current.type == EventType.MouseDown)
			{
				this.m_IsDragging = false;
			}
			this.UpdateSpritePreviewCacheSize();
			List<DopeLine> dopelines = this.state.dopelines;
			for (int i = 0; i < dopelines.Count; i++)
			{
				DopeLine dopeLine = dopelines[i];
				dopeLine.position = position2;
				dopeLine.position.height = ((!dopeLine.tallMode) ? 16f : 32f);
				if ((dopeLine.position.yMin + scrollPosition.y >= position.yMin && dopeLine.position.yMin + scrollPosition.y <= position.yMax) || (dopeLine.position.yMax + scrollPosition.y >= position.yMin && dopeLine.position.yMax + scrollPosition.y <= position.yMax))
				{
					Event current = Event.current;
					EventType type = current.type;
					switch (type)
					{
					case EventType.Repaint:
						this.DopeLineRepaint(dopeLine);
						goto IL_1A5;
					case EventType.Layout:
						IL_13D:
						if (type == EventType.MouseDown)
						{
							if (current.button == 0)
							{
								this.HandleMouseDown(dopeLine);
							}
							goto IL_1A5;
						}
						if (type != EventType.ContextClick)
						{
							goto IL_1A5;
						}
						if (!this.m_IsDraggingPlayhead)
						{
							this.HandleContextMenu(dopeLine);
						}
						goto IL_1A5;
					case EventType.DragUpdated:
					case EventType.DragPerform:
						this.HandleDragAndDrop(dopeLine);
						goto IL_1A5;
					}
					goto IL_13D;
					IL_1A5:;
				}
				position2.y += dopeLine.position.height;
			}
			if (Event.current.type == EventType.MouseUp)
			{
				this.m_IsDraggingPlayheadStarted = false;
				this.m_IsDraggingPlayhead = false;
			}
			Rect result = new Rect(position.xMin, position.yMin, position.width, position2.yMax - position.yMin);
			this.m_PointRenderer.Render();
			GUI.color = color;
			return result;
		}

		private void RectangleToolGUI()
		{
			this.m_RectangleTool.OnGUI();
		}

		private void DrawGrid(Rect position)
		{
			base.TimeRuler(position, this.state.frameRate, false, true, 0.2f);
		}

		public void DrawMasterDopelineBackground(Rect position)
		{
			if (Event.current.type == EventType.Repaint)
			{
				AnimationWindowStyles.eventBackground.Draw(position, false, false, false, false);
			}
		}

		private void UpdateSpritePreviewCacheSize()
		{
			int num = 1;
			foreach (DopeLine current in this.state.dopelines)
			{
				if (current.tallMode && current.isPptrDopeline)
				{
					num += current.keys.Count;
				}
			}
			num += DragAndDrop.objectReferences.Length;
			if (num > this.m_SpritePreviewCacheSize)
			{
				AssetPreview.SetPreviewTextureCacheSize(num, this.assetPreviewManagerID);
				this.m_SpritePreviewCacheSize = num;
			}
		}

		private void DopeLineRepaint(DopeLine dopeline)
		{
			Color color = GUI.color;
			AnimationWindowHierarchyNode animationWindowHierarchyNode = (AnimationWindowHierarchyNode)this.state.hierarchyData.FindItem(dopeline.hierarchyNodeID);
			bool flag = animationWindowHierarchyNode != null && animationWindowHierarchyNode.depth > 0;
			Color color2 = (!flag) ? Color.gray.AlphaMultiplied(0.16f) : Color.gray.AlphaMultiplied(0.05f);
			if (!dopeline.isMasterDopeline)
			{
				DopeSheetEditor.DrawBox(dopeline.position, color2);
			}
			int? num = null;
			int count = dopeline.keys.Count;
			for (int i = 0; i < count; i++)
			{
				AnimationWindowKeyframe animationWindowKeyframe = dopeline.keys[i];
				int num2 = animationWindowKeyframe.m_TimeHash ^ animationWindowKeyframe.curve.timeOffset.GetHashCode();
				if (!(num == num2))
				{
					num = new int?(num2);
					Rect rect = this.GetKeyframeRect(dopeline, animationWindowKeyframe);
					color2 = ((!dopeline.isMasterDopeline) ? Color.gray.RGBMultiplied(1.2f) : Color.gray.RGBMultiplied(0.85f));
					Texture2D texture2D = null;
					if (animationWindowKeyframe.isPPtrCurve && dopeline.tallMode)
					{
						texture2D = ((animationWindowKeyframe.value != null) ? AssetPreview.GetAssetPreview(((UnityEngine.Object)animationWindowKeyframe.value).GetInstanceID(), this.assetPreviewManagerID) : null);
					}
					if (texture2D != null)
					{
						rect = this.GetPreviewRectFromKeyFrameRect(rect);
						color2 = Color.white.AlphaMultiplied(0.5f);
					}
					else if (animationWindowKeyframe.value != null && animationWindowKeyframe.isPPtrCurve && dopeline.tallMode)
					{
						this.m_SpritePreviewLoading = true;
					}
					if (Mathf.Approximately(animationWindowKeyframe.time, 0f))
					{
						rect.xMin -= 0.01f;
					}
					if (this.AnyKeyIsSelectedAtTime(dopeline, i))
					{
						color2 = ((!dopeline.tallMode || !dopeline.isPptrDopeline) ? new Color(0.34f, 0.52f, 0.85f, 1f) : Color.white);
						if (dopeline.isMasterDopeline)
						{
							color2 = color2.RGBMultiplied(0.85f);
						}
						this.m_PointRenderer.AddSelectedKey(new DopeSheetEditor.DrawElement(rect, color2, texture2D));
					}
					else
					{
						this.m_PointRenderer.AddUnselectedKey(new DopeSheetEditor.DrawElement(rect, color2, texture2D));
					}
				}
			}
			if (this.DoDragAndDrop(dopeline, dopeline.position, false))
			{
				float num3 = Mathf.Max(this.state.PixelToTime(Event.current.mousePosition.x, AnimationWindowState.SnapMode.SnapToClipFrame), 0f);
				Color color3 = Color.gray.RGBMultiplied(1.2f);
				Texture2D texture2D2 = null;
				UnityEngine.Object[] sortedDragAndDropObjectReferences = this.GetSortedDragAndDropObjectReferences();
				for (int j = 0; j < sortedDragAndDropObjectReferences.Length; j++)
				{
					UnityEngine.Object @object = sortedDragAndDropObjectReferences[j];
					Rect rect2 = this.GetDragAndDropRect(dopeline, num3);
					if (dopeline.isPptrDopeline && dopeline.tallMode)
					{
						texture2D2 = AssetPreview.GetAssetPreview(@object.GetInstanceID(), this.assetPreviewManagerID);
					}
					if (texture2D2 != null)
					{
						rect2 = this.GetPreviewRectFromKeyFrameRect(rect2);
						color3 = Color.white.AlphaMultiplied(0.5f);
					}
					this.m_PointRenderer.AddDragDropKey(new DopeSheetEditor.DrawElement(rect2, color3, texture2D2));
					num3 += 1f / this.state.frameRate;
				}
			}
			GUI.color = color;
		}

		private Rect GetPreviewRectFromKeyFrameRect(Rect keyframeRect)
		{
			keyframeRect.width -= 2f;
			keyframeRect.height -= 2f;
			keyframeRect.xMin += 2f;
			keyframeRect.yMin += 2f;
			return keyframeRect;
		}

		private Rect GetDragAndDropRect(DopeLine dopeline, float time)
		{
			Rect keyframeRect = this.GetKeyframeRect(dopeline, null);
			float keyframeOffset = this.GetKeyframeOffset(dopeline, null);
			keyframeRect.center = new Vector2(this.state.TimeToPixel(time) + keyframeRect.width * 0.5f + keyframeOffset, keyframeRect.center.y);
			return keyframeRect;
		}

		private static void DrawBox(Rect position, Color color)
		{
			Color color2 = GUI.color;
			GUI.color = color;
			DopeLine.dopekeyStyle.Draw(position, GUIContent.none, 0, false);
			GUI.color = color2;
		}

		private GenericMenu GenerateMenu(DopeLine dopeline)
		{
			GenericMenu genericMenu = new GenericMenu();
			List<AnimationWindowKeyframe> list = new List<AnimationWindowKeyframe>();
			foreach (AnimationWindowKeyframe current in dopeline.keys)
			{
				if (this.GetKeyframeRect(dopeline, current).Contains(Event.current.mousePosition))
				{
					list.Add(current);
				}
			}
			AnimationKeyTime time = AnimationKeyTime.Time(this.state.PixelToTime(Event.current.mousePosition.x, AnimationWindowState.SnapMode.SnapToClipFrame), this.state.frameRate);
			this.state.StartRecording();
			string text = "Add Key";
			if (dopeline.isEditable && list.Count == 0)
			{
				genericMenu.AddItem(new GUIContent(text), false, new GenericMenu.MenuFunction2(this.AddKeyToDopeline), new DopeSheetEditor.AddKeyToDopelineContext
				{
					dopeline = dopeline,
					time = time
				});
			}
			else
			{
				genericMenu.AddDisabledItem(new GUIContent(text));
			}
			text = ((this.state.selectedKeys.Count <= 1) ? "Delete Key" : "Delete Keys");
			if (dopeline.isEditable && (this.state.selectedKeys.Count > 0 || list.Count > 0))
			{
				genericMenu.AddItem(new GUIContent(text), false, new GenericMenu.MenuFunction2(this.DeleteKeys), (this.state.selectedKeys.Count <= 0) ? list : this.state.selectedKeys);
			}
			else
			{
				genericMenu.AddDisabledItem(new GUIContent(text));
			}
			if (dopeline.isEditable && AnimationWindowUtility.ContainsFloatKeyframes(this.state.selectedKeys))
			{
				genericMenu.AddSeparator(string.Empty);
				List<KeyIdentifier> list2 = new List<KeyIdentifier>();
				Hashtable hashtable = new Hashtable();
				foreach (AnimationWindowKeyframe current2 in this.state.selectedKeys)
				{
					if (!current2.isPPtrCurve)
					{
						int keyframeIndex = current2.curve.GetKeyframeIndex(AnimationKeyTime.Time(current2.time, this.state.frameRate));
						if (keyframeIndex != -1)
						{
							int hashCode = current2.curve.GetHashCode();
							AnimationCurve animationCurve = (AnimationCurve)hashtable[hashCode];
							if (animationCurve == null)
							{
								animationCurve = AnimationUtility.GetEditorCurve(current2.curve.clip, current2.curve.binding);
								if (animationCurve == null)
								{
									animationCurve = new AnimationCurve();
								}
								hashtable.Add(hashCode, animationCurve);
							}
							list2.Add(new KeyIdentifier(animationCurve, hashCode, keyframeIndex, current2.curve.binding));
						}
					}
				}
				CurveMenuManager curveMenuManager = new CurveMenuManager(this);
				curveMenuManager.AddTangentMenuItems(genericMenu, list2);
			}
			return genericMenu;
		}

		private void HandleDragging()
		{
			int controlID = GUIUtility.GetControlID("dopesheetdrag".GetHashCode(), FocusType.Passive, default(Rect));
			EventType typeForControl = Event.current.GetTypeForControl(controlID);
			if ((typeForControl == EventType.MouseDrag || typeForControl == EventType.MouseUp) && this.m_MousedownOnKeyframe)
			{
				if (typeForControl == EventType.MouseDrag && !EditorGUI.actionKey && !Event.current.shift)
				{
					if (!this.m_IsDragging && this.state.selectedKeys.Count > 0)
					{
						this.m_IsDragging = true;
						this.m_IsDraggingPlayheadStarted = true;
						GUIUtility.hotControl = controlID;
						this.m_DragStartTime = this.state.PixelToTime(Event.current.mousePosition.x);
						this.m_RectangleTool.OnStartMove(new Vector2(this.m_DragStartTime, 0f), this.m_RectangleTool.rippleTimeClutch);
						Event.current.Use();
					}
				}
				float b = 3.40282347E+38f;
				foreach (AnimationWindowKeyframe current in this.state.selectedKeys)
				{
					b = Mathf.Min(current.time, b);
				}
				float num = this.state.SnapToFrame(this.state.PixelToTime(Event.current.mousePosition.x), AnimationWindowState.SnapMode.SnapToClipFrame);
				if (this.m_IsDragging)
				{
					if (!Mathf.Approximately(num, this.m_DragStartTime))
					{
						this.m_RectangleTool.OnMove(new Vector2(num, 0f));
						Event.current.Use();
					}
				}
				if (typeForControl == EventType.MouseUp)
				{
					if (this.m_IsDragging && GUIUtility.hotControl == controlID)
					{
						this.m_RectangleTool.OnEndMove();
						Event.current.Use();
						this.m_IsDragging = false;
					}
					this.m_MousedownOnKeyframe = false;
					GUIUtility.hotControl = 0;
				}
			}
			if (this.m_IsDraggingPlayheadStarted && typeForControl == EventType.MouseDrag && Event.current.button == 1)
			{
				this.m_IsDraggingPlayhead = true;
				Event.current.Use();
			}
			if (this.m_IsDragging)
			{
				Vector2 mousePosition = Event.current.mousePosition;
				Rect position = new Rect(mousePosition.x - 10f, mousePosition.y - 10f, 20f, 20f);
				EditorGUIUtility.AddCursorRect(position, MouseCursor.MoveArrow);
			}
		}

		private void HandleKeyboard()
		{
			if (Event.current.type == EventType.ValidateCommand || Event.current.type == EventType.ExecuteCommand)
			{
				string commandName = Event.current.commandName;
				if (commandName != null)
				{
					if (!(commandName == "SelectAll"))
					{
						if (commandName == "FrameSelected")
						{
							if (Event.current.type == EventType.ExecuteCommand)
							{
								this.FrameSelected();
							}
							Event.current.Use();
						}
					}
					else
					{
						if (Event.current.type == EventType.ExecuteCommand)
						{
							this.HandleSelectAll();
						}
						Event.current.Use();
					}
				}
			}
			if (Event.current.type == EventType.KeyDown)
			{
				if (Event.current.keyCode == KeyCode.A)
				{
					this.FrameClip();
					Event.current.Use();
				}
			}
		}

		private void HandleSelectAll()
		{
			foreach (DopeLine current in this.state.dopelines)
			{
				foreach (AnimationWindowKeyframe current2 in current.keys)
				{
					this.state.SelectKey(current2);
				}
				this.state.SelectHierarchyItem(current, true, false);
			}
		}

		private void HandleDelete()
		{
			if (this.state.selectedKeys.Count != 0)
			{
				EventType type = Event.current.type;
				if (type != EventType.ValidateCommand && type != EventType.ExecuteCommand)
				{
					if (type == EventType.KeyDown)
					{
						if (Event.current.keyCode == KeyCode.Backspace || Event.current.keyCode == KeyCode.Delete)
						{
							this.state.DeleteSelectedKeys();
							Event.current.Use();
						}
					}
				}
				else if (Event.current.commandName == "SoftDelete" || Event.current.commandName == "Delete")
				{
					if (Event.current.type == EventType.ExecuteCommand)
					{
						this.state.DeleteSelectedKeys();
					}
					Event.current.Use();
				}
			}
		}

		private void HandleSelectionRect(Rect rect)
		{
			if (this.m_SelectionRect == null)
			{
				this.m_SelectionRect = new DopeSheetEditor.DopeSheetSelectionRect(this);
			}
			if (!this.m_MousedownOnKeyframe)
			{
				this.m_SelectionRect.OnGUI(rect);
			}
		}

		private void HandleDragAndDropToEmptyArea()
		{
			Event current = Event.current;
			if (current.type == EventType.DragPerform || current.type == EventType.DragUpdated)
			{
				if (DopeSheetEditor.ValidateDragAndDropObjects())
				{
					if (DragAndDrop.objectReferences[0].GetType() == typeof(Sprite) || DragAndDrop.objectReferences[0].GetType() == typeof(Texture2D))
					{
						AnimationWindowSelectionItem[] array = this.state.selection.ToArray();
						for (int i = 0; i < array.Length; i++)
						{
							AnimationWindowSelectionItem animationWindowSelectionItem = array[i];
							if (animationWindowSelectionItem.clipIsEditable)
							{
								if (animationWindowSelectionItem.canAddCurves)
								{
									if (!this.DopelineForValueTypeExists(typeof(Sprite)))
									{
										if (current.type == EventType.DragPerform)
										{
											EditorCurveBinding? spriteBinding = this.CreateNewPptrDopeline(animationWindowSelectionItem, typeof(Sprite));
											if (spriteBinding.HasValue)
											{
												this.DoSpriteDropAfterGeneratingNewDopeline(animationWindowSelectionItem.animationClip, spriteBinding);
											}
										}
										DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
										current.Use();
										return;
									}
								}
							}
						}
					}
					DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;
				}
			}
		}

		private void DoSpriteDropAfterGeneratingNewDopeline(AnimationClip animationClip, EditorCurveBinding? spriteBinding)
		{
			if (DragAndDrop.objectReferences.Length == 1)
			{
				UsabilityAnalytics.Event("Sprite Drag and Drop", "Drop single sprite into empty dopesheet", "null", 1);
			}
			else
			{
				UsabilityAnalytics.Event("Sprite Drag and Drop", "Drop multiple sprites into empty dopesheet", "null", 1);
			}
			AnimationWindowCurve targetCurve = new AnimationWindowCurve(animationClip, spriteBinding.Value, typeof(Sprite));
			this.PerformDragAndDrop(targetCurve, 0f);
		}

		private void HandleRectangleToolEvents()
		{
			this.m_RectangleTool.HandleEvents();
		}

		private bool DopelineForValueTypeExists(Type valueType)
		{
			return this.state.allCurves.Exists((AnimationWindowCurve curve) => curve.valueType == valueType);
		}

		private EditorCurveBinding? CreateNewPptrDopeline(AnimationWindowSelectionItem selectedItem, Type valueType)
		{
			List<EditorCurveBinding> list = null;
			EditorCurveBinding? result;
			if (selectedItem.rootGameObject != null)
			{
				list = AnimationWindowUtility.GetAnimatableProperties(selectedItem.rootGameObject, selectedItem.rootGameObject, valueType);
				if (list.Count == 0 && valueType == typeof(Sprite))
				{
					result = this.CreateNewSpriteRendererDopeline(selectedItem.rootGameObject, selectedItem.rootGameObject);
					return result;
				}
			}
			else if (selectedItem.scriptableObject != null)
			{
				list = AnimationWindowUtility.GetAnimatableProperties(selectedItem.scriptableObject, valueType);
			}
			if (list == null || list.Count == 0)
			{
				result = null;
			}
			else if (list.Count == 1)
			{
				result = new EditorCurveBinding?(list[0]);
			}
			else
			{
				List<string> list2 = new List<string>();
				foreach (EditorCurveBinding current in list)
				{
					list2.Add(current.type.Name);
				}
				List<object> list3 = new List<object>();
				list3.Add(selectedItem.animationClip);
				list3.Add(list);
				Rect position = new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y, 1f, 1f);
				EditorUtility.DisplayCustomMenu(position, EditorGUIUtility.TempContent(list2.ToArray()), -1, new EditorUtility.SelectMenuItemFunction(this.SelectTypeForCreatingNewPptrDopeline), list3);
				result = null;
			}
			return result;
		}

		private void SelectTypeForCreatingNewPptrDopeline(object userData, string[] options, int selected)
		{
			List<object> list = userData as List<object>;
			AnimationClip animationClip = list[0] as AnimationClip;
			List<EditorCurveBinding> list2 = list[1] as List<EditorCurveBinding>;
			if (list2.Count > selected)
			{
				this.DoSpriteDropAfterGeneratingNewDopeline(animationClip, new EditorCurveBinding?(list2[selected]));
			}
		}

		private EditorCurveBinding? CreateNewSpriteRendererDopeline(GameObject targetGameObject, GameObject rootGameObject)
		{
			if (!targetGameObject.GetComponent<SpriteRenderer>())
			{
				targetGameObject.AddComponent<SpriteRenderer>();
			}
			List<EditorCurveBinding> animatableProperties = AnimationWindowUtility.GetAnimatableProperties(targetGameObject, rootGameObject, typeof(SpriteRenderer), typeof(Sprite));
			EditorCurveBinding? result;
			if (animatableProperties.Count == 1)
			{
				result = new EditorCurveBinding?(animatableProperties[0]);
			}
			else
			{
				Debug.LogError("Unable to create animatable SpriteRenderer component");
				result = null;
			}
			return result;
		}

		private void HandleDragAndDrop(DopeLine dopeline)
		{
			Event current = Event.current;
			if (current.type == EventType.DragPerform || current.type == EventType.DragUpdated)
			{
				if (this.DoDragAndDrop(dopeline, dopeline.position, current.type == EventType.DragPerform))
				{
					DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
					current.Use();
				}
				else
				{
					DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;
				}
			}
		}

		private void HandleMouseDown(DopeLine dopeline)
		{
			Event current = Event.current;
			if (dopeline.position.Contains(current.mousePosition))
			{
				bool flag = false;
				foreach (AnimationWindowKeyframe current2 in dopeline.keys)
				{
					if (this.GetKeyframeRect(dopeline, current2).Contains(current.mousePosition) && this.state.KeyIsSelected(current2))
					{
						flag = true;
						break;
					}
				}
				bool flag2 = flag && EditorGUI.actionKey;
				bool flag3 = !flag;
				if (!flag && !EditorGUI.actionKey && !current.shift)
				{
					this.state.ClearSelections();
				}
				float num = this.state.PixelToTime(Event.current.mousePosition.x);
				float num2 = num;
				if (Event.current.shift)
				{
					foreach (AnimationWindowKeyframe current3 in dopeline.keys)
					{
						if (this.state.KeyIsSelected(current3))
						{
							if (current3.time < num)
							{
								num = current3.time;
							}
							if (current3.time > num2)
							{
								num2 = current3.time;
							}
						}
					}
				}
				bool flag4 = false;
				foreach (AnimationWindowKeyframe current4 in dopeline.keys)
				{
					if (this.GetKeyframeRect(dopeline, current4).Contains(current.mousePosition))
					{
						flag4 = true;
						if (flag2)
						{
							if (this.state.KeyIsSelected(current4))
							{
								this.state.UnselectKey(current4);
								if (!this.state.AnyKeyIsSelected(dopeline))
								{
									this.state.UnSelectHierarchyItem(dopeline);
								}
							}
						}
						else if (flag3)
						{
							if (!this.state.KeyIsSelected(current4))
							{
								if (Event.current.shift)
								{
									foreach (AnimationWindowKeyframe current5 in dopeline.keys)
									{
										if (current5 == current4 || (current5.time > num && current5.time < num2))
										{
											this.state.SelectKey(current5);
										}
									}
								}
								else
								{
									this.state.SelectKey(current4);
								}
								if (!dopeline.isMasterDopeline)
								{
									this.state.SelectHierarchyItem(dopeline, EditorGUI.actionKey || current.shift);
								}
							}
						}
						this.state.activeKeyframe = current4;
						this.m_MousedownOnKeyframe = true;
						current.Use();
					}
				}
				if (dopeline.isMasterDopeline)
				{
					this.state.ClearHierarchySelection();
					List<int> affectedHierarchyIDs = this.state.GetAffectedHierarchyIDs(this.state.selectedKeys);
					foreach (int current6 in affectedHierarchyIDs)
					{
						this.state.SelectHierarchyItem(current6, true, true);
					}
				}
				if (current.clickCount == 2 && current.button == 0 && !Event.current.shift && !EditorGUI.actionKey)
				{
					this.HandleDopelineDoubleclick(dopeline);
				}
				if (current.button == 1 && !this.state.controlInterface.playing)
				{
					if (!flag4)
					{
						this.state.ClearSelections();
						this.m_IsDraggingPlayheadStarted = true;
						HandleUtility.Repaint();
						current.Use();
					}
				}
			}
		}

		private void HandleDopelineDoubleclick(DopeLine dopeline)
		{
			float time = this.state.PixelToTime(Event.current.mousePosition.x, AnimationWindowState.SnapMode.SnapToClipFrame);
			AnimationKeyTime time2 = AnimationKeyTime.Time(time, this.state.frameRate);
			AnimationWindowUtility.AddKeyframes(this.state, dopeline.curves.ToArray<AnimationWindowCurve>(), time2);
			Event.current.Use();
		}

		private void HandleContextMenu(DopeLine dopeline)
		{
			if (dopeline.position.Contains(Event.current.mousePosition))
			{
				this.GenerateMenu(dopeline).ShowAsContext();
			}
		}

		private Rect GetKeyframeRect(DopeLine dopeline, AnimationWindowKeyframe keyframe)
		{
			float time = (keyframe == null) ? 0f : (keyframe.time + keyframe.curve.timeOffset);
			float width = 10f;
			if (dopeline.isPptrDopeline && dopeline.tallMode && (keyframe == null || keyframe.value != null))
			{
				width = dopeline.position.height;
			}
			return new Rect(this.state.TimeToPixel(this.state.SnapToFrame(time, AnimationWindowState.SnapMode.SnapToClipFrame)) + this.GetKeyframeOffset(dopeline, keyframe), dopeline.position.yMin, width, dopeline.position.height);
		}

		private float GetKeyframeOffset(DopeLine dopeline, AnimationWindowKeyframe keyframe)
		{
			float result;
			if (dopeline.isPptrDopeline && dopeline.tallMode && (keyframe == null || keyframe.value != null))
			{
				result = -1f;
			}
			else
			{
				result = -6.5f;
			}
			return result;
		}

		public void FrameClip()
		{
			if (!this.state.disabled)
			{
				Vector2 timeRange = this.state.timeRange;
				timeRange.y = Mathf.Max(timeRange.x + 0.1f, timeRange.y);
				base.SetShownHRangeInsideMargins(timeRange.x, timeRange.y);
			}
		}

		public void FrameSelected()
		{
			Bounds bounds = default(Bounds);
			bool flag = true;
			bool flag2 = this.state.selectedKeys.Count > 0;
			if (flag2)
			{
				foreach (AnimationWindowKeyframe current in this.state.selectedKeys)
				{
					Vector2 v = new Vector2(current.time + current.curve.timeOffset, 0f);
					if (flag)
					{
						bounds.SetMinMax(v, v);
						flag = false;
					}
					else
					{
						bounds.Encapsulate(v);
					}
				}
			}
			bool flag3 = !flag2;
			if (!flag2)
			{
				bool flag4 = this.state.hierarchyState.selectedIDs.Count > 0;
				if (flag4)
				{
					foreach (AnimationWindowCurve current2 in this.state.activeCurves)
					{
						int count = current2.m_Keyframes.Count;
						if (count > 1)
						{
							Vector2 v2 = new Vector2(current2.m_Keyframes[0].time + current2.timeOffset, 0f);
							Vector2 v3 = new Vector2(current2.m_Keyframes[count - 1].time + current2.timeOffset, 0f);
							if (flag)
							{
								bounds.SetMinMax(v2, v3);
								flag = false;
							}
							else
							{
								bounds.Encapsulate(v2);
								bounds.Encapsulate(v3);
							}
							flag3 = false;
						}
					}
				}
			}
			if (flag3)
			{
				this.FrameClip();
			}
			else
			{
				bounds.size = new Vector3(Mathf.Max(bounds.size.x, 0.1f), Mathf.Max(bounds.size.y, 0.1f), 0f);
				base.SetShownHRangeInsideMargins(bounds.min.x, bounds.max.x);
			}
		}

		private bool DoDragAndDrop(DopeLine dopeLine, Rect position, bool perform)
		{
			bool result;
			if (!position.Contains(Event.current.mousePosition))
			{
				result = false;
			}
			else if (!DopeSheetEditor.ValidateDragAndDropObjects())
			{
				result = false;
			}
			else
			{
				Type type = DragAndDrop.objectReferences[0].GetType();
				AnimationWindowCurve animationWindowCurve = null;
				if (dopeLine.valueType == type)
				{
					animationWindowCurve = dopeLine.curves[0];
				}
				else
				{
					AnimationWindowCurve[] curves = dopeLine.curves;
					for (int i = 0; i < curves.Length; i++)
					{
						AnimationWindowCurve animationWindowCurve2 = curves[i];
						if (animationWindowCurve2.isPPtrCurve)
						{
							if (animationWindowCurve2.valueType == type)
							{
								animationWindowCurve = animationWindowCurve2;
							}
							List<Sprite> spriteFromPathsOrObjects = SpriteUtility.GetSpriteFromPathsOrObjects(DragAndDrop.objectReferences, DragAndDrop.paths, Event.current.type);
							if (animationWindowCurve2.valueType == typeof(Sprite) && spriteFromPathsOrObjects.Count > 0)
							{
								animationWindowCurve = animationWindowCurve2;
								type = typeof(Sprite);
							}
						}
					}
				}
				if (animationWindowCurve == null)
				{
					result = false;
				}
				else if (!animationWindowCurve.clipIsEditable)
				{
					result = false;
				}
				else
				{
					if (perform)
					{
						if (DragAndDrop.objectReferences.Length == 1)
						{
							UsabilityAnalytics.Event("Sprite Drag and Drop", "Drop single sprite into existing dopeline", "null", 1);
						}
						else
						{
							UsabilityAnalytics.Event("Sprite Drag and Drop", "Drop multiple sprites into existing dopeline", "null", 1);
						}
						float time = Mathf.Max(this.state.PixelToTime(Event.current.mousePosition.x, AnimationWindowState.SnapMode.SnapToClipFrame), 0f);
						AnimationWindowCurve curveOfType = this.GetCurveOfType(dopeLine, type);
						this.PerformDragAndDrop(curveOfType, time);
					}
					result = true;
				}
			}
			return result;
		}

		private void PerformDragAndDrop(AnimationWindowCurve targetCurve, float time)
		{
			if (DragAndDrop.objectReferences.Length != 0 && targetCurve != null)
			{
				string undoLabel = "Drop Key";
				this.state.SaveKeySelection(undoLabel);
				this.state.ClearSelections();
				UnityEngine.Object[] sortedDragAndDropObjectReferences = this.GetSortedDragAndDropObjectReferences();
				UnityEngine.Object[] array = sortedDragAndDropObjectReferences;
				for (int i = 0; i < array.Length; i++)
				{
					UnityEngine.Object @object = array[i];
					UnityEngine.Object object2 = @object;
					if (object2 is Texture2D)
					{
						object2 = SpriteUtility.TextureToSprite(@object as Texture2D);
					}
					this.CreateNewPPtrKeyframe(time, object2, targetCurve);
					time += 1f / targetCurve.clip.frameRate;
				}
				this.state.SaveCurve(targetCurve, undoLabel);
				DragAndDrop.AcceptDrag();
			}
		}

		private UnityEngine.Object[] GetSortedDragAndDropObjectReferences()
		{
			UnityEngine.Object[] objectReferences = DragAndDrop.objectReferences;
			Array.Sort<UnityEngine.Object>(objectReferences, (UnityEngine.Object a, UnityEngine.Object b) => EditorUtility.NaturalCompare(a.name, b.name));
			return objectReferences;
		}

		private void CreateNewPPtrKeyframe(float time, UnityEngine.Object value, AnimationWindowCurve targetCurve)
		{
			AnimationWindowKeyframe animationWindowKeyframe = new AnimationWindowKeyframe(targetCurve, new ObjectReferenceKeyframe
			{
				time = time,
				value = value
			});
			AnimationKeyTime keyTime = AnimationKeyTime.Time(animationWindowKeyframe.time, this.state.frameRate);
			targetCurve.AddKeyframe(animationWindowKeyframe, keyTime);
			this.state.SelectKey(animationWindowKeyframe);
		}

		private static bool ValidateDragAndDropObjects()
		{
			bool result;
			if (DragAndDrop.objectReferences.Length == 0)
			{
				result = false;
			}
			else
			{
				for (int i = 0; i < DragAndDrop.objectReferences.Length; i++)
				{
					UnityEngine.Object @object = DragAndDrop.objectReferences[i];
					if (@object == null)
					{
						result = false;
						return result;
					}
					if (i < DragAndDrop.objectReferences.Length - 1)
					{
						UnityEngine.Object object2 = DragAndDrop.objectReferences[i + 1];
						bool flag = (@object is Texture2D || @object is Sprite) && (object2 is Texture2D || object2 is Sprite);
						if (@object.GetType() != object2.GetType() && !flag)
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

		private AnimationWindowCurve GetCurveOfType(DopeLine dopeLine, Type type)
		{
			AnimationWindowCurve[] curves = dopeLine.curves;
			AnimationWindowCurve result;
			for (int i = 0; i < curves.Length; i++)
			{
				AnimationWindowCurve animationWindowCurve = curves[i];
				if (animationWindowCurve.valueType == type)
				{
					result = animationWindowCurve;
					return result;
				}
			}
			result = null;
			return result;
		}

		private bool AnyKeyIsSelectedAtTime(DopeLine dopeLine, int keyIndex)
		{
			AnimationWindowKeyframe animationWindowKeyframe = dopeLine.keys[keyIndex];
			int num = animationWindowKeyframe.m_TimeHash ^ animationWindowKeyframe.curve.timeOffset.GetHashCode();
			int count = dopeLine.keys.Count;
			int i = keyIndex;
			bool result;
			while (i < count)
			{
				animationWindowKeyframe = dopeLine.keys[i];
				int num2 = animationWindowKeyframe.m_TimeHash ^ animationWindowKeyframe.curve.timeOffset.GetHashCode();
				if (num2 != num)
				{
					result = false;
				}
				else
				{
					if (!this.state.KeyIsSelected(animationWindowKeyframe))
					{
						i++;
						continue;
					}
					result = true;
				}
				return result;
			}
			result = false;
			return result;
		}

		private void AddKeyToDopeline(object obj)
		{
			this.AddKeyToDopeline((DopeSheetEditor.AddKeyToDopelineContext)obj);
		}

		private void AddKeyToDopeline(DopeSheetEditor.AddKeyToDopelineContext context)
		{
			AnimationWindowUtility.AddKeyframes(this.state, context.dopeline.curves.ToArray<AnimationWindowCurve>(), context.time);
		}

		private void DeleteKeys(object obj)
		{
			this.DeleteKeys((List<AnimationWindowKeyframe>)obj);
		}

		private void DeleteKeys(List<AnimationWindowKeyframe> keys)
		{
			this.state.DeleteKeys(keys);
		}

		public void UpdateCurves(List<ChangedCurve> changedCurves, string undoText)
		{
			Undo.RegisterCompleteObjectUndo(this.state.activeAnimationClip, undoText);
			using (List<ChangedCurve>.Enumerator enumerator = changedCurves.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					ChangedCurve changedCurve = enumerator.Current;
					AnimationWindowCurve animationWindowCurve = this.state.allCurves.Find((AnimationWindowCurve c) => changedCurve.curveId == c.GetHashCode());
					if (animationWindowCurve != null)
					{
						AnimationUtility.SetEditorCurve(animationWindowCurve.clip, changedCurve.binding, changedCurve.curve);
					}
					else
					{
						Debug.LogError("Could not match ChangedCurve data to destination curves.");
					}
				}
			}
		}
	}
}
