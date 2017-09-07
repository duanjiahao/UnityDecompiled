using System;
using System.Collections.Generic;
using UnityEditor.IMGUI.Controls;
using UnityEditorInternal;
using UnityEngine;

namespace UnityEditor
{
	internal class UISystemProfiler
	{
		internal class Headers : MultiColumnHeader
		{
			public Headers(MultiColumnHeaderState state) : base(state)
			{
			}

			protected override void ColumnHeaderGUI(MultiColumnHeaderState.Column column, Rect headerRect, int columnIndex)
			{
				GUIStyle styleWrapped = this.GetStyleWrapped(column.headerTextAlignment);
				float num = styleWrapped.CalcHeight(column.headerContent, headerRect.width);
				Rect position = headerRect;
				position.yMin += position.height - num - 1f;
				GUI.Label(position, column.headerContent, styleWrapped);
				if (base.canSort && column.canSort)
				{
					base.SortingButton(column, headerRect, columnIndex);
				}
			}

			internal override void DrawDivider(Rect dividerRect, MultiColumnHeaderState.Column column)
			{
			}

			internal override Rect GetArrowRect(MultiColumnHeaderState.Column column, Rect headerRect)
			{
				return new Rect(headerRect.xMax - MultiColumnHeader.DefaultStyles.arrowStyle.fixedWidth, headerRect.y + 5f, MultiColumnHeader.DefaultStyles.arrowStyle.fixedWidth, headerRect.height - 10f);
			}

			private GUIStyle GetStyleWrapped(TextAlignment alignment)
			{
				GUIStyle result;
				switch (alignment)
				{
				case TextAlignment.Left:
					result = UISystemProfiler.Styles.columnHeader;
					break;
				case TextAlignment.Center:
					result = UISystemProfiler.Styles.columnHeaderCenterAligned;
					break;
				case TextAlignment.Right:
					result = UISystemProfiler.Styles.columnHeaderRightAligned;
					break;
				default:
					result = UISystemProfiler.Styles.columnHeader;
					break;
				}
				return result;
			}
		}

		internal static class Styles
		{
			internal enum RenderMode
			{
				Standard,
				Overdraw,
				CompositeOverdraw
			}

			internal enum PreviewBackgroundType
			{
				Checkerboard,
				Black,
				White
			}

			internal const string PrefCheckerBoard = "UGUIProfiler.CheckerBoard";

			internal const string PrefOverdraw = "UGUIProfiler.Overdraw";

			public static readonly GUIStyle columnHeader;

			public static readonly GUIStyle columnHeaderCenterAligned;

			public static readonly GUIStyle columnHeaderRightAligned;

			public static readonly GUIStyle background;

			public static readonly GUIStyle entryEven;

			public static readonly GUIStyle entryOdd;

			public static readonly GUIStyle header;

			public static readonly GUIContent noData;

			public static readonly GUIStyle rightHeader;

			public static GUIContent[] backgroundOptions;

			public static int[] backgroundValues;

			public static GUIContent contentDetachRender;

			public static GUIContent[] rendermodeOptions;

			public static int[] rendermodeValues;

			private static readonly Color m_SeparatorColorPro;

			private static readonly Color m_SeparatorColorNonPro;

			public static Color separatorColor
			{
				get
				{
					return (!EditorGUIUtility.isProSkin) ? UISystemProfiler.Styles.m_SeparatorColorNonPro : UISystemProfiler.Styles.m_SeparatorColorPro;
				}
			}

			static Styles()
			{
				UISystemProfiler.Styles.entryOdd = "OL EntryBackOdd";
				UISystemProfiler.Styles.entryEven = "OL EntryBackEven";
				UISystemProfiler.Styles.rightHeader = "OL title TextRight";
				UISystemProfiler.Styles.columnHeader = "OL title";
				UISystemProfiler.Styles.columnHeaderCenterAligned = new GUIStyle(UISystemProfiler.Styles.columnHeader)
				{
					alignment = TextAnchor.MiddleCenter
				};
				UISystemProfiler.Styles.columnHeaderRightAligned = new GUIStyle(UISystemProfiler.Styles.columnHeader)
				{
					alignment = TextAnchor.MiddleRight
				};
				UISystemProfiler.Styles.background = "OL Box";
				UISystemProfiler.Styles.header = "OL title";
				UISystemProfiler.Styles.header.alignment = TextAnchor.MiddleLeft;
				UISystemProfiler.Styles.noData = EditorGUIUtility.TextContent("No frame data available - UI profiling is only available when profiling in the editor");
				UISystemProfiler.Styles.contentDetachRender = new GUIContent("Detach");
				UISystemProfiler.Styles.backgroundOptions = new GUIContent[]
				{
					new GUIContent("Checkerboard"),
					new GUIContent("Black"),
					new GUIContent("White")
				};
				UISystemProfiler.Styles.backgroundValues = new int[]
				{
					0,
					1,
					2
				};
				UISystemProfiler.Styles.rendermodeOptions = new GUIContent[]
				{
					new GUIContent("Standard"),
					new GUIContent("Overdraw"),
					new GUIContent("Composite overdraw")
				};
				UISystemProfiler.Styles.rendermodeValues = new int[]
				{
					0,
					1,
					2
				};
				UISystemProfiler.Styles.m_SeparatorColorPro = new Color(0.15f, 0.15f, 0.15f);
				UISystemProfiler.Styles.m_SeparatorColorNonPro = new Color(0.6f, 0.6f, 0.6f);
			}
		}

		private readonly SplitterState m_TreePreviewHorizontalSplitState = new SplitterState(new float[]
		{
			70f,
			30f
		}, new int[]
		{
			100,
			100
		}, null);

		private Material m_CompositeOverdrawMaterial;

		private MultiColumnHeaderState m_MulticolumnHeaderState;

		private UISystemProfilerRenderService m_RenderService;

		private UISystemProfilerTreeView m_TreeViewControl;

		private UISystemProfilerTreeView.State m_UGUIProfilerTreeViewState;

		private ZoomableArea m_ZoomablePreview;

		private UISystemPreviewWindow m_DetachedPreview;

		private static UISystemProfiler.Styles.RenderMode PreviewRenderMode
		{
			get
			{
				return (UISystemProfiler.Styles.RenderMode)EditorPrefs.GetInt("UGUIProfiler.Overdraw", 0);
			}
			set
			{
				EditorPrefs.SetInt("UGUIProfiler.Overdraw", (int)value);
			}
		}

		private static UISystemProfiler.Styles.PreviewBackgroundType PreviewBackground
		{
			get
			{
				return (UISystemProfiler.Styles.PreviewBackgroundType)EditorPrefs.GetInt("UGUIProfiler.CheckerBoard", 0);
			}
			set
			{
				EditorPrefs.SetInt("UGUIProfiler.CheckerBoard", (int)value);
			}
		}

		internal void DrawUIPane(ProfilerWindow win, ProfilerArea profilerArea, UISystemProfilerChart detailsChart)
		{
			this.InitIfNeeded(win);
			EditorGUILayout.BeginVertical(new GUILayoutOption[0]);
			if (this.m_DetachedPreview != null && !this.m_DetachedPreview)
			{
				this.m_DetachedPreview = null;
			}
			bool flag = this.m_DetachedPreview;
			if (!flag)
			{
				GUILayout.BeginHorizontal(new GUILayoutOption[0]);
				SplitterGUILayout.BeginHorizontalSplit(this.m_TreePreviewHorizontalSplitState, new GUILayoutOption[0]);
			}
			Rect controlRect = EditorGUILayout.GetControlRect(new GUILayoutOption[]
			{
				GUILayout.ExpandWidth(true),
				GUILayout.ExpandHeight(true)
			});
			controlRect.yMin -= EditorGUIUtility.standardVerticalSpacing;
			this.m_TreeViewControl.property = win.CreateProperty(ProfilerColumn.DontSort);
			if (!this.m_TreeViewControl.property.frameDataReady)
			{
				this.m_TreeViewControl.property.Cleanup();
				this.m_TreeViewControl.property = null;
				GUI.Label(controlRect, UISystemProfiler.Styles.noData);
			}
			else
			{
				int activeVisibleFrameIndex = win.GetActiveVisibleFrameIndex();
				if (this.m_UGUIProfilerTreeViewState != null && this.m_UGUIProfilerTreeViewState.lastFrame != activeVisibleFrameIndex)
				{
					this.m_TreeViewControl.Reload();
				}
				this.m_TreeViewControl.OnGUI(controlRect);
				this.m_TreeViewControl.property.Cleanup();
			}
			if (!flag)
			{
				using (new EditorGUILayout.VerticalScope(new GUILayoutOption[0]))
				{
					using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar, new GUILayoutOption[]
					{
						GUILayout.ExpandWidth(true)
					}))
					{
						flag = GUILayout.Button(UISystemProfiler.Styles.contentDetachRender, EditorStyles.toolbarButton, new GUILayoutOption[]
						{
							GUILayout.Width(75f)
						});
						if (flag)
						{
							this.m_DetachedPreview = EditorWindow.GetWindow<UISystemPreviewWindow>();
							this.m_DetachedPreview.profiler = this;
							this.m_DetachedPreview.Show();
						}
						UISystemProfiler.DrawPreviewToolbarButtons();
					}
					this.DrawRenderUI();
				}
				GUILayout.EndHorizontal();
				SplitterGUILayout.EndHorizontalSplit();
				EditorGUI.DrawRect(new Rect((float)this.m_TreePreviewHorizontalSplitState.realSizes[0] + controlRect.xMin, controlRect.y, 1f, controlRect.height), UISystemProfiler.Styles.separatorColor);
			}
			EditorGUILayout.EndVertical();
			if (this.m_DetachedPreview)
			{
				this.m_DetachedPreview.Repaint();
			}
		}

		internal static void DrawPreviewToolbarButtons()
		{
			UISystemProfiler.PreviewBackground = (UISystemProfiler.Styles.PreviewBackgroundType)EditorGUILayout.IntPopup(GUIContent.none, (int)UISystemProfiler.PreviewBackground, UISystemProfiler.Styles.backgroundOptions, UISystemProfiler.Styles.backgroundValues, EditorStyles.toolbarDropDown, new GUILayoutOption[]
			{
				GUILayout.MaxWidth(100f)
			});
			UISystemProfiler.PreviewRenderMode = (UISystemProfiler.Styles.RenderMode)EditorGUILayout.IntPopup(GUIContent.none, (int)UISystemProfiler.PreviewRenderMode, UISystemProfiler.Styles.rendermodeOptions, UISystemProfiler.Styles.rendermodeValues, EditorStyles.toolbarDropDown, new GUILayoutOption[]
			{
				GUILayout.MaxWidth(100f)
			});
		}

		internal void DrawRenderUI()
		{
			Rect controlRect = EditorGUILayout.GetControlRect(new GUILayoutOption[]
			{
				GUILayout.ExpandWidth(true),
				GUILayout.ExpandHeight(true)
			});
			GUI.Box(controlRect, GUIContent.none);
			this.m_ZoomablePreview.BeginViewGUI();
			bool flag = true;
			if (this.m_UGUIProfilerTreeViewState != null && Event.current.type == EventType.Repaint)
			{
				IList<int> selection = this.m_TreeViewControl.GetSelection();
				if (selection.Count > 0)
				{
					IList<TreeViewItem> rowsFromIDs = this.m_TreeViewControl.GetRowsFromIDs(selection);
					foreach (TreeViewItem current in rowsFromIDs)
					{
						Texture2D texture2D = null;
						UISystemProfilerTreeView.BatchTreeViewItem batchTreeViewItem = current as UISystemProfilerTreeView.BatchTreeViewItem;
						UISystemProfiler.Styles.RenderMode previewRenderMode = UISystemProfiler.PreviewRenderMode;
						if (this.m_RenderService == null)
						{
							this.m_RenderService = new UISystemProfilerRenderService();
						}
						if (batchTreeViewItem != null)
						{
							texture2D = this.m_RenderService.GetThumbnail(batchTreeViewItem.renderDataIndex, 1, previewRenderMode != UISystemProfiler.Styles.RenderMode.Standard);
						}
						UISystemProfilerTreeView.CanvasTreeViewItem canvasTreeViewItem = current as UISystemProfilerTreeView.CanvasTreeViewItem;
						if (canvasTreeViewItem != null)
						{
							texture2D = this.m_RenderService.GetThumbnail(canvasTreeViewItem.info.renderDataIndex, canvasTreeViewItem.info.renderDataCount, previewRenderMode != UISystemProfiler.Styles.RenderMode.Standard);
						}
						if (previewRenderMode == UISystemProfiler.Styles.RenderMode.CompositeOverdraw)
						{
							if (this.m_CompositeOverdrawMaterial == null)
							{
								Shader shader = Shader.Find("Hidden/UI/CompositeOverdraw");
								if (shader)
								{
									this.m_CompositeOverdrawMaterial = new Material(shader);
								}
							}
						}
						if (texture2D)
						{
							float num = (float)texture2D.width;
							float num2 = (float)texture2D.height;
							float num3 = Math.Min(controlRect.width / num, controlRect.height / num2);
							num *= num3;
							num2 *= num3;
							Rect rect = new Rect(controlRect.x + (controlRect.width - num) / 2f, controlRect.y + (controlRect.height - num2) / 2f, num, num2);
							if (flag)
							{
								flag = false;
								this.m_ZoomablePreview.rect = rect;
								UISystemProfiler.Styles.PreviewBackgroundType previewBackground = UISystemProfiler.PreviewBackground;
								if (previewBackground == UISystemProfiler.Styles.PreviewBackgroundType.Checkerboard)
								{
									EditorGUI.DrawTransparencyCheckerTexture(this.m_ZoomablePreview.drawRect, ScaleMode.ScaleAndCrop, 0f);
								}
								else
								{
									EditorGUI.DrawRect(this.m_ZoomablePreview.drawRect, (previewBackground != UISystemProfiler.Styles.PreviewBackgroundType.Black) ? Color.white : Color.black);
								}
							}
							Graphics.DrawTexture(this.m_ZoomablePreview.drawRect, texture2D, this.m_ZoomablePreview.shownArea, 0, 0, 0, 0, (previewRenderMode != UISystemProfiler.Styles.RenderMode.CompositeOverdraw) ? EditorGUI.transparentMaterial : this.m_CompositeOverdrawMaterial);
						}
						if (previewRenderMode != UISystemProfiler.Styles.RenderMode.Standard)
						{
							break;
						}
					}
				}
			}
			if (flag && Event.current.type == EventType.Repaint)
			{
				this.m_ZoomablePreview.rect = controlRect;
			}
			this.m_ZoomablePreview.EndViewGUI();
		}

		private void InitIfNeeded(ProfilerWindow win)
		{
			if (this.m_ZoomablePreview == null)
			{
				this.m_ZoomablePreview = new ZoomableArea(true, false)
				{
					hRangeMin = 0f,
					vRangeMin = 0f,
					hRangeMax = 1f,
					vRangeMax = 1f
				};
				this.m_ZoomablePreview.SetShownHRange(0f, 1f);
				this.m_ZoomablePreview.SetShownVRange(0f, 1f);
				this.m_ZoomablePreview.uniformScale = true;
				this.m_ZoomablePreview.scaleWithWindow = true;
				int num = 100;
				int num2 = 200;
				this.m_MulticolumnHeaderState = new MultiColumnHeaderState(new MultiColumnHeaderState.Column[]
				{
					new MultiColumnHeaderState.Column
					{
						headerContent = EditorGUIUtility.TextContent("Object"),
						width = 220f,
						maxWidth = 400f,
						canSort = true
					},
					new MultiColumnHeaderState.Column
					{
						headerContent = EditorGUIUtility.TextContent("Self Batch Count"),
						width = (float)num,
						maxWidth = (float)num2
					},
					new MultiColumnHeaderState.Column
					{
						headerContent = EditorGUIUtility.TextContent("Cumulative Batch Count"),
						width = (float)num,
						maxWidth = (float)num2
					},
					new MultiColumnHeaderState.Column
					{
						headerContent = EditorGUIUtility.TextContent("Self Vertex Count"),
						width = (float)num,
						maxWidth = (float)num2
					},
					new MultiColumnHeaderState.Column
					{
						headerContent = EditorGUIUtility.TextContent("Cumulative Vertex Count"),
						width = (float)num,
						maxWidth = (float)num2
					},
					new MultiColumnHeaderState.Column
					{
						headerContent = EditorGUIUtility.TextContent("Batch Breaking Reason"),
						width = 220f,
						maxWidth = 400f,
						canSort = false
					},
					new MultiColumnHeaderState.Column
					{
						headerContent = EditorGUIUtility.TextContent("GameObject Count"),
						width = (float)num,
						maxWidth = 400f
					},
					new MultiColumnHeaderState.Column
					{
						headerContent = EditorGUIUtility.TextContent("GameObjects"),
						width = 150f,
						maxWidth = 400f,
						canSort = false
					}
				});
				MultiColumnHeaderState.Column[] columns = this.m_MulticolumnHeaderState.columns;
				for (int i = 0; i < columns.Length; i++)
				{
					MultiColumnHeaderState.Column column = columns[i];
					column.sortingArrowAlignment = TextAlignment.Right;
				}
				this.m_UGUIProfilerTreeViewState = new UISystemProfilerTreeView.State
				{
					profilerWindow = win
				};
				UISystemProfiler.Headers headers = new UISystemProfiler.Headers(this.m_MulticolumnHeaderState)
				{
					canSort = true,
					height = 21f
				};
				headers.sortingChanged += delegate(MultiColumnHeader header)
				{
					this.m_TreeViewControl.Reload();
				};
				this.m_TreeViewControl = new UISystemProfilerTreeView(this.m_UGUIProfilerTreeViewState, headers);
				this.m_TreeViewControl.Reload();
			}
		}

		public void CurrentAreaChanged(ProfilerArea profilerArea)
		{
			if (profilerArea != ProfilerArea.UI && profilerArea != ProfilerArea.UIDetails)
			{
				if (this.m_DetachedPreview)
				{
					this.m_DetachedPreview.Close();
					this.m_DetachedPreview = null;
				}
				if (this.m_RenderService != null)
				{
					this.m_RenderService.Dispose();
					this.m_RenderService = null;
				}
			}
		}
	}
}
