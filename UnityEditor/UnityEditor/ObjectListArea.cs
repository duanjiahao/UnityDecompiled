using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor.Collaboration;
using UnityEditor.VersionControl;
using UnityEditor.Web;
using UnityEditorInternal;
using UnityEditorInternal.VersionControl;
using UnityEngine;

namespace UnityEditor
{
	internal class ObjectListArea
	{
		private class Styles
		{
			public GUIStyle resultsLabel = new GUIStyle("PR Label");

			public GUIStyle resultsGridLabel = ObjectListArea.Styles.GetStyle("ProjectBrowserGridLabel");

			public GUIStyle resultsGrid = "ObjectPickerResultsGrid";

			public GUIStyle background = "ObjectPickerBackground";

			public GUIStyle previewTextureBackground = "ObjectPickerPreviewBackground";

			public GUIStyle groupHeaderMiddle = ObjectListArea.Styles.GetStyle("ProjectBrowserHeaderBgMiddle");

			public GUIStyle groupHeaderTop = ObjectListArea.Styles.GetStyle("ProjectBrowserHeaderBgTop");

			public GUIStyle groupHeaderLabel = "Label";

			public GUIStyle groupHeaderLabelCount = "MiniLabel";

			public GUIStyle groupFoldout = "IN Foldout";

			public GUIStyle toolbarBack = "ObjectPickerToolbar";

			public GUIStyle resultsFocusMarker;

			public GUIStyle miniRenameField = new GUIStyle("PR TextField");

			public GUIStyle ping = new GUIStyle("PR Ping");

			public GUIStyle miniPing = new GUIStyle("PR Ping");

			public GUIStyle iconDropShadow = ObjectListArea.Styles.GetStyle("ProjectBrowserIconDropShadow");

			public GUIStyle textureIconDropShadow = ObjectListArea.Styles.GetStyle("ProjectBrowserTextureIconDropShadow");

			public GUIStyle iconAreaBg = ObjectListArea.Styles.GetStyle("ProjectBrowserIconAreaBg");

			public GUIStyle previewBg = ObjectListArea.Styles.GetStyle("ProjectBrowserPreviewBg");

			public GUIStyle subAssetBg = ObjectListArea.Styles.GetStyle("ProjectBrowserSubAssetBg");

			public GUIStyle subAssetBgOpenEnded = ObjectListArea.Styles.GetStyle("ProjectBrowserSubAssetBgOpenEnded");

			public GUIStyle subAssetBgCloseEnded = ObjectListArea.Styles.GetStyle("ProjectBrowserSubAssetBgCloseEnded");

			public GUIStyle subAssetBgMiddle = ObjectListArea.Styles.GetStyle("ProjectBrowserSubAssetBgMiddle");

			public GUIStyle subAssetBgDivider = ObjectListArea.Styles.GetStyle("ProjectBrowserSubAssetBgDivider");

			public GUIStyle subAssetExpandButton = ObjectListArea.Styles.GetStyle("ProjectBrowserSubAssetExpandBtn");

			public GUIContent m_AssetStoreNotAvailableText = new GUIContent("The Asset Store is not available");

			public Styles()
			{
				this.resultsFocusMarker = new GUIStyle(this.resultsGridLabel);
				GUIStyle arg_1D5_0 = this.resultsFocusMarker;
				float num = 0f;
				this.resultsFocusMarker.fixedWidth = num;
				arg_1D5_0.fixedHeight = num;
				this.miniRenameField.font = EditorStyles.miniLabel.font;
				this.miniRenameField.alignment = TextAnchor.LowerCenter;
				this.ping.fixedHeight = 16f;
				this.ping.padding.right = 10;
				this.miniPing.font = EditorStyles.miniLabel.font;
				this.miniPing.alignment = TextAnchor.MiddleCenter;
				this.resultsLabel.alignment = TextAnchor.MiddleLeft;
			}

			private static GUIStyle GetStyle(string styleName)
			{
				return styleName;
			}
		}

		private class AssetStoreGroup : ObjectListArea.Group
		{
			public const int kDefaultRowsShown = 3;

			public const int kDefaultRowsShownListMode = 10;

			private const int kMoreButtonOffset = 3;

			private const int kMoreRowsAdded = 10;

			private const int kMoreRowsAddedListMode = 75;

			private const int kMaxQueryItems = 1000;

			private GUIContent m_Content = new GUIContent();

			private List<AssetStoreAsset> m_Assets;

			private string m_Name;

			private bool m_ListMode;

			private Vector3 m_ShowMoreDims;

			public string Name
			{
				get
				{
					return this.m_Name;
				}
			}

			public List<AssetStoreAsset> Assets
			{
				get
				{
					return this.m_Assets;
				}
				set
				{
					this.m_Assets = value;
				}
			}

			public override int ItemCount
			{
				get
				{
					return Math.Min(this.m_Assets.Count, this.ItemsWantedShown);
				}
			}

			public override bool ListMode
			{
				get
				{
					return this.m_ListMode;
				}
				set
				{
					this.m_ListMode = value;
				}
			}

			public bool NeedItems
			{
				get
				{
					int num = Math.Min(1000, this.ItemsWantedShown);
					int count = this.Assets.Count;
					return (this.ItemsAvailable >= num && count < num) || (this.ItemsAvailable < num && count < this.ItemsAvailable);
				}
			}

			public override bool NeedsRepaint
			{
				get;
				protected set;
			}

			public AssetStoreGroup(ObjectListArea owner, string groupTitle, string groupName) : base(owner, groupTitle)
			{
				this.m_Assets = new List<AssetStoreAsset>();
				this.m_Name = groupName;
				this.m_ListMode = false;
				this.m_ShowMoreDims = EditorStyles.miniButton.CalcSize(new GUIContent("Show more"));
				this.m_Owner.UpdateGroupSizes(this);
				this.ItemsWantedShown = 3 * this.m_Grid.columns;
			}

			public override void UpdateAssets()
			{
			}

			protected override void DrawInternal(int itemIdx, int endItem, float yOffset)
			{
				int count = this.m_Assets.Count;
				int num = itemIdx;
				yOffset += this.kGroupSeparatorHeight;
				bool flag = Event.current.type == EventType.Repaint;
				if (this.ListMode)
				{
					while (itemIdx < endItem && itemIdx < count)
					{
						Rect position = this.m_Grid.CalcRect(itemIdx, yOffset);
						int num2 = this.HandleMouse(position);
						if (num2 != 0)
						{
							this.m_Owner.SetSelection(this.m_Assets[itemIdx], num2 == 2);
						}
						if (flag)
						{
							bool selected = !AssetStoreAssetSelection.Empty && AssetStoreAssetSelection.ContainsAsset(this.m_Assets[itemIdx].id);
							this.DrawLabel(position, this.m_Assets[itemIdx], selected);
						}
						itemIdx++;
					}
				}
				else
				{
					while (itemIdx < endItem && itemIdx < count)
					{
						Rect position = this.m_Grid.CalcRect(itemIdx, yOffset);
						int num3 = this.HandleMouse(position);
						if (num3 != 0)
						{
							this.m_Owner.SetSelection(this.m_Assets[itemIdx], num3 == 2);
						}
						if (flag)
						{
							Rect position2 = new Rect(position.x, position.y, position.width, position.height - ObjectListArea.s_Styles.resultsGridLabel.fixedHeight);
							this.DrawIcon(position2, this.m_Assets[itemIdx]);
						}
						itemIdx++;
					}
					itemIdx = num;
					if (flag)
					{
						while (itemIdx < endItem && itemIdx < count)
						{
							Rect position = this.m_Grid.CalcRect(itemIdx, yOffset);
							bool selected2 = !AssetStoreAssetSelection.Empty && AssetStoreAssetSelection.ContainsAsset(this.m_Assets[itemIdx].id);
							this.DrawLabel(position, this.m_Assets[itemIdx], selected2);
							itemIdx++;
						}
					}
				}
				if (this.ItemsAvailable > this.m_Grid.rows * this.m_Grid.columns)
				{
					Rect position = new Rect(this.m_Owner.GetVisibleWidth() - this.m_ShowMoreDims.x - 6f, yOffset + this.m_Grid.height + 3f, this.m_ShowMoreDims.x, this.m_ShowMoreDims.y);
					if (this.ItemsAvailable > this.m_Grid.rows * this.m_Grid.columns && this.ItemsAvailable >= this.Assets.Count && this.Assets.Count < 1000)
					{
						Event current = Event.current;
						EventType type = current.type;
						if (type != EventType.MouseDown)
						{
							if (type == EventType.Repaint)
							{
								EditorStyles.miniButton.Draw(position, "More", false, false, false, false);
							}
						}
						else if (current.button == 0 && position.Contains(current.mousePosition))
						{
							if (this.ListMode)
							{
								this.ItemsWantedShown += 75;
							}
							else
							{
								int num4 = this.m_Grid.columns - this.ItemCount % this.m_Grid.columns;
								num4 %= this.m_Grid.columns;
								this.ItemsWantedShown += 10 * this.m_Grid.columns + num4;
							}
							if (this.NeedItems)
							{
								this.m_Owner.QueryAssetStore();
							}
							current.Use();
						}
					}
				}
			}

			private AssetStorePreviewManager.CachedAssetStoreImage GetIconForAssetStoreAsset(AssetStoreAsset assetStoreResource)
			{
				AssetStorePreviewManager.CachedAssetStoreImage result;
				if (!string.IsNullOrEmpty(assetStoreResource.staticPreviewURL))
				{
					this.m_Owner.LastScrollTime += 1.0;
					AssetStorePreviewManager.CachedAssetStoreImage cachedAssetStoreImage = AssetStorePreviewManager.TextureFromUrl(assetStoreResource.staticPreviewURL, assetStoreResource.name, this.m_Owner.gridSize, ObjectListArea.s_Styles.resultsGridLabel, ObjectListArea.s_Styles.previewBg, false);
					result = cachedAssetStoreImage;
				}
				else
				{
					result = null;
				}
				return result;
			}

			private void DrawIcon(Rect position, AssetStoreAsset assetStoreResource)
			{
				bool flag = false;
				this.m_Content.text = null;
				AssetStorePreviewManager.CachedAssetStoreImage iconForAssetStoreAsset = this.GetIconForAssetStoreAsset(assetStoreResource);
				if (iconForAssetStoreAsset == null)
				{
					Texture2D iconForFile = InternalEditorUtility.GetIconForFile(assetStoreResource.name);
					ObjectListArea.s_Styles.resultsGrid.Draw(position, iconForFile, false, false, flag, flag);
				}
				else
				{
					this.m_Content.image = iconForAssetStoreAsset.image;
					Color color = iconForAssetStoreAsset.color;
					Color color2 = GUI.color;
					if (color.a != 1f)
					{
						GUI.color = color;
					}
					ObjectListArea.s_Styles.resultsGrid.Draw(position, this.m_Content, false, false, flag, flag);
					if (color.a != 1f)
					{
						GUI.color = color2;
						this.NeedsRepaint = true;
					}
					base.DrawDropShadowOverlay(position, flag, false, false);
				}
			}

			private void DrawLabel(Rect position, AssetStoreAsset assetStoreResource, bool selected)
			{
				if (this.ListMode)
				{
					position.width = Mathf.Max(position.width, 500f);
					this.m_Content.text = assetStoreResource.displayName;
					this.m_Content.image = InternalEditorUtility.GetIconForFile(assetStoreResource.name);
					ObjectListArea.s_Styles.resultsLabel.Draw(position, this.m_Content, false, false, selected, selected);
				}
				else
				{
					int instanceID = assetStoreResource.id + 10000000;
					string croppedLabelText = this.m_Owner.GetCroppedLabelText(instanceID, assetStoreResource.displayName, position.width);
					position.height -= ObjectListArea.s_Styles.resultsGridLabel.fixedHeight;
					ObjectListArea.s_Styles.resultsGridLabel.Draw(new Rect(position.x, position.yMax + 1f, position.width - 1f, ObjectListArea.s_Styles.resultsGridLabel.fixedHeight), croppedLabelText, false, false, selected, this.m_Owner.HasFocus());
				}
			}

			public override void UpdateFilter(HierarchyType hierarchyType, SearchFilter searchFilter, bool showFoldersFirst)
			{
				this.ItemsWantedShown = ((!this.ListMode) ? (3 * this.m_Grid.columns) : 10);
				this.Assets.Clear();
			}

			public override void UpdateHeight()
			{
				this.m_Height = (float)((int)this.kGroupSeparatorHeight);
				if (this.Visible)
				{
					this.m_Height += this.m_Grid.height;
					if (this.ItemsAvailable > this.m_Grid.rows * this.m_Grid.columns)
					{
						this.m_Height += (float)(6 + (int)this.m_ShowMoreDims.y);
					}
				}
			}

			public int IndexOf(int assetID)
			{
				int num = 0;
				int result;
				foreach (AssetStoreAsset current in this.m_Assets)
				{
					if (current.id == assetID)
					{
						result = num;
						return result;
					}
					num++;
				}
				result = -1;
				return result;
			}

			public AssetStoreAsset AssetAtIndex(int selectedIdx)
			{
				AssetStoreAsset result;
				if (selectedIdx >= this.m_Grid.rows * this.m_Grid.columns)
				{
					result = null;
				}
				else if (selectedIdx < this.m_Grid.rows * this.m_Grid.columns && selectedIdx > this.ItemCount)
				{
					result = this.m_Assets.Last<AssetStoreAsset>();
				}
				else
				{
					int num = 0;
					foreach (AssetStoreAsset current in this.m_Assets)
					{
						if (selectedIdx == num)
						{
							result = current;
							return result;
						}
						num++;
					}
					result = null;
				}
				return result;
			}

			protected int HandleMouse(Rect position)
			{
				Event current = Event.current;
				EventType type = current.type;
				int result;
				if (type != EventType.MouseDown)
				{
					if (type == EventType.ContextClick)
					{
						if (position.Contains(current.mousePosition))
						{
							result = 1;
							return result;
						}
					}
				}
				else if (current.button == 0 && position.Contains(current.mousePosition))
				{
					this.m_Owner.Repaint();
					if (current.clickCount == 2)
					{
						current.Use();
						result = 2;
						return result;
					}
					this.m_Owner.ScrollToPosition(ObjectListArea.AdjustRectForFraming(position));
					current.Use();
					result = 1;
					return result;
				}
				result = 0;
				return result;
			}
		}

		private abstract class Group
		{
			protected readonly float kGroupSeparatorHeight = EditorStyles.toolbar.fixedHeight;

			protected string m_GroupSeparatorTitle;

			protected static int[] s_Empty;

			public ObjectListArea m_Owner;

			public VerticalGrid m_Grid = new VerticalGrid();

			public float m_Height;

			public bool Visible = true;

			public int ItemsAvailable = 0;

			public int ItemsWantedShown = 0;

			protected bool m_Collapsable = true;

			public double m_LastClickedDrawTime = 0.0;

			public float Height
			{
				get
				{
					return this.m_Height;
				}
			}

			public abstract int ItemCount
			{
				get;
			}

			public abstract bool ListMode
			{
				get;
				set;
			}

			public abstract bool NeedsRepaint
			{
				get;
				protected set;
			}

			public bool visiblePreference
			{
				get
				{
					return string.IsNullOrEmpty(this.m_GroupSeparatorTitle) || EditorPrefs.GetBool(this.m_GroupSeparatorTitle, true);
				}
				set
				{
					if (!string.IsNullOrEmpty(this.m_GroupSeparatorTitle))
					{
						EditorPrefs.SetBool(this.m_GroupSeparatorTitle, value);
					}
				}
			}

			public Group(ObjectListArea owner, string groupTitle)
			{
				this.m_GroupSeparatorTitle = groupTitle;
				if (ObjectListArea.Group.s_Empty == null)
				{
					ObjectListArea.Group.s_Empty = new int[0];
				}
				this.m_Owner = owner;
				this.Visible = this.visiblePreference;
			}

			public abstract void UpdateAssets();

			public abstract void UpdateHeight();

			protected abstract void DrawInternal(int itemIdx, int endItem, float yOffset);

			public abstract void UpdateFilter(HierarchyType hierarchyType, SearchFilter searchFilter, bool showFoldersFirst);

			protected virtual float GetHeaderHeight()
			{
				return this.kGroupSeparatorHeight;
			}

			protected virtual void HandleUnusedDragEvents(float yOffset)
			{
			}

			private int FirstVisibleRow(float yOffset, Vector2 scrollPos)
			{
				int result;
				if (!this.Visible)
				{
					result = -1;
				}
				else
				{
					float num = scrollPos.y - (yOffset + this.GetHeaderHeight());
					int num2 = 0;
					if (num > 0f)
					{
						float num3 = this.m_Grid.itemSize.y + this.m_Grid.verticalSpacing;
						num2 = (int)Mathf.Max(0f, Mathf.Floor(num / num3));
					}
					result = num2;
				}
				return result;
			}

			private bool IsInView(float yOffset, Vector2 scrollPos, float scrollViewHeight)
			{
				return scrollPos.y + scrollViewHeight >= yOffset && yOffset + this.Height >= scrollPos.y;
			}

			public void Draw(float yOffset, Vector2 scrollPos)
			{
				this.NeedsRepaint = false;
				bool flag = Event.current.type == EventType.Repaint || Event.current.type == EventType.Layout;
				if (!flag)
				{
					this.DrawHeader(yOffset, this.m_Collapsable);
				}
				if (this.IsInView(yOffset, scrollPos, this.m_Owner.m_VisibleRect.height))
				{
					int num = this.FirstVisibleRow(yOffset, scrollPos);
					int num2 = num * this.m_Grid.columns;
					int itemCount = this.ItemCount;
					if (num2 >= 0 && num2 < itemCount)
					{
						int num3 = num2;
						int num4 = Math.Min(itemCount, this.m_Grid.rows * this.m_Grid.columns);
						float num5 = this.m_Grid.itemSize.y + this.m_Grid.verticalSpacing;
						int num6 = (int)Math.Ceiling((double)(this.m_Owner.m_VisibleRect.height / num5));
						num4 = Math.Min(num4, num3 + num6 * this.m_Grid.columns + this.m_Grid.columns);
						this.DrawInternal(num3, num4, yOffset);
					}
					if (flag)
					{
						this.DrawHeader(yOffset, this.m_Collapsable);
					}
					this.HandleUnusedDragEvents(yOffset);
				}
			}

			protected void DrawObjectIcon(Rect position, Texture icon)
			{
				if (!(icon == null))
				{
					int width = icon.width;
					FilterMode filterMode = icon.filterMode;
					icon.filterMode = FilterMode.Point;
					GUI.DrawTexture(new Rect(position.x + (float)(((int)position.width - width) / 2), position.y + (float)(((int)position.height - width) / 2), (float)width, (float)width), icon, ScaleMode.ScaleToFit);
					icon.filterMode = filterMode;
				}
			}

			protected void DrawDropShadowOverlay(Rect position, bool selected, bool isDropTarget, bool isRenaming)
			{
				float num = position.width / 128f;
				Rect position2 = new Rect(position.x - 4f * num, position.y - 2f * num, position.width + 8f * num, position.height + 12f * num - 0.5f);
				ObjectListArea.s_Styles.iconDropShadow.Draw(position2, GUIContent.none, false, false, selected || isDropTarget, this.m_Owner.HasFocus() || isRenaming || isDropTarget);
			}

			protected void DrawHeaderBackground(Rect rect, bool firstHeader)
			{
				if (Event.current.type == EventType.Repaint)
				{
					GUI.Label(rect, GUIContent.none, (!firstHeader) ? ObjectListArea.s_Styles.groupHeaderMiddle : ObjectListArea.s_Styles.groupHeaderTop);
				}
			}

			protected float GetHeaderYPosInScrollArea(float yOffset)
			{
				float result = yOffset;
				float y = this.m_Owner.m_State.m_ScrollPosition.y;
				if (y > yOffset)
				{
					result = Mathf.Min(y, yOffset + this.Height - this.kGroupSeparatorHeight);
				}
				return result;
			}

			protected virtual void DrawHeader(float yOffset, bool collapsable)
			{
				Rect rect = new Rect(0f, this.GetHeaderYPosInScrollArea(yOffset), this.m_Owner.GetVisibleWidth(), this.kGroupSeparatorHeight - 1f);
				this.DrawHeaderBackground(rect, yOffset == 0f);
				rect.x += 7f;
				if (collapsable)
				{
					bool visible = this.Visible;
					this.Visible = GUI.Toggle(rect, this.Visible, GUIContent.none, ObjectListArea.s_Styles.groupFoldout);
					if (visible ^ this.Visible)
					{
						this.visiblePreference = this.Visible;
					}
				}
				GUIStyle groupHeaderLabel = ObjectListArea.s_Styles.groupHeaderLabel;
				if (collapsable)
				{
					rect.x += ObjectListArea.s_Styles.groupFoldout.fixedWidth + 3f;
				}
				rect.y += 1f;
				if (!string.IsNullOrEmpty(this.m_GroupSeparatorTitle))
				{
					GUI.Label(rect, this.m_GroupSeparatorTitle, groupHeaderLabel);
				}
				if (ObjectListArea.s_Debug)
				{
					Rect position = rect;
					position.x += 120f;
					GUI.Label(position, AssetStorePreviewManager.StatsString());
				}
				rect.y -= 1f;
				if (this.m_Owner.GetVisibleWidth() > 150f)
				{
					this.DrawItemCount(rect);
				}
			}

			protected void DrawItemCount(Rect rect)
			{
				string text = this.ItemsAvailable.ToString() + " Total";
				Vector2 vector = ObjectListArea.s_Styles.groupHeaderLabelCount.CalcSize(new GUIContent(text));
				if (vector.x < rect.width)
				{
					rect.x = this.m_Owner.GetVisibleWidth() - vector.x - 4f;
				}
				rect.width = vector.x;
				rect.y += 2f;
				GUI.Label(rect, text, ObjectListArea.s_Styles.groupHeaderLabelCount);
			}

			private UnityEngine.Object[] GetSelectedReferences()
			{
				return Selection.objects;
			}

			private static string[] GetMainSelectedPaths()
			{
				List<string> list = new List<string>();
				int[] instanceIDs = Selection.instanceIDs;
				for (int i = 0; i < instanceIDs.Length; i++)
				{
					int instanceID = instanceIDs[i];
					if (AssetDatabase.IsMainAsset(instanceID))
					{
						string assetPath = AssetDatabase.GetAssetPath(instanceID);
						list.Add(assetPath);
					}
				}
				return list.ToArray();
			}
		}

		private class LocalGroup : ObjectListArea.Group
		{
			private class ItemFader
			{
				private double m_FadeDuration = 0.3;

				private double m_FirstToLastDuration = 0.3;

				private double m_FadeStartTime;

				private double m_TimeBetweenEachItem;

				private List<int> m_InstanceIDs;

				public void Start(List<int> instanceIDs)
				{
					this.m_InstanceIDs = instanceIDs;
					this.m_FadeStartTime = EditorApplication.timeSinceStartup;
					this.m_FirstToLastDuration = Math.Min(0.5, (double)instanceIDs.Count * 0.03);
					this.m_TimeBetweenEachItem = 0.0;
					if (this.m_InstanceIDs.Count > 1)
					{
						this.m_TimeBetweenEachItem = this.m_FirstToLastDuration / (double)(this.m_InstanceIDs.Count - 1);
					}
				}

				public float GetAlpha(int instanceID)
				{
					float result;
					if (this.m_InstanceIDs == null)
					{
						result = 1f;
					}
					else if (EditorApplication.timeSinceStartup > this.m_FadeStartTime + this.m_FadeDuration + this.m_FirstToLastDuration)
					{
						this.m_InstanceIDs = null;
						result = 1f;
					}
					else
					{
						int num = this.m_InstanceIDs.IndexOf(instanceID);
						if (num >= 0)
						{
							double num2 = EditorApplication.timeSinceStartup - this.m_FadeStartTime;
							double num3 = this.m_TimeBetweenEachItem * (double)num;
							float num4 = 0f;
							if (num3 < num2)
							{
								num4 = Mathf.Clamp((float)((num2 - num3) / this.m_FadeDuration), 0f, 1f);
							}
							result = num4;
						}
						else
						{
							result = 1f;
						}
					}
					return result;
				}
			}

			private BuiltinResource[] m_NoneList;

			private GUIContent m_Content = new GUIContent();

			private List<int> m_DragSelection = new List<int>();

			private int m_DropTargetControlID = 0;

			private Dictionary<string, BuiltinResource[]> m_BuiltinResourceMap;

			private BuiltinResource[] m_CurrentBuiltinResources;

			private bool m_ShowNoneItem;

			private List<int> m_LastRenderedAssetInstanceIDs = new List<int>();

			private List<int> m_LastRenderedAssetDirtyIDs = new List<int>();

			public bool m_ListMode = false;

			private FilteredHierarchy m_FilteredHierarchy;

			private BuiltinResource[] m_ActiveBuiltinList;

			public const int k_ListModeLeftPadding = 16;

			public const int k_ListModeLeftPaddingForSubAssets = 28;

			public const int k_ListModeVersionControlOverlayPadding = 14;

			private const float k_IconWidth = 16f;

			private const float k_SpaceBetweenIconAndText = 2f;

			private ObjectListArea.LocalGroup.ItemFader m_ItemFader = new ObjectListArea.LocalGroup.ItemFader();

			public bool ShowNone
			{
				get
				{
					return this.m_ShowNoneItem;
				}
			}

			public override bool NeedsRepaint
			{
				get
				{
					return false;
				}
				protected set
				{
				}
			}

			public SearchFilter searchFilter
			{
				get
				{
					return this.m_FilteredHierarchy.searchFilter;
				}
			}

			public override bool ListMode
			{
				get
				{
					return this.m_ListMode;
				}
				set
				{
					this.m_ListMode = value;
				}
			}

			public bool HasBuiltinResources
			{
				get
				{
					return this.m_CurrentBuiltinResources.Length > 0;
				}
			}

			public override int ItemCount
			{
				get
				{
					int num = this.m_FilteredHierarchy.results.Length;
					int num2 = num + this.m_ActiveBuiltinList.Length;
					int num3 = (!this.m_ShowNoneItem) ? 0 : 1;
					int num4 = (this.m_Owner.m_State.m_NewAssetIndexInList == -1) ? 0 : 1;
					return num2 + num3 + num4;
				}
			}

			public LocalGroup(ObjectListArea owner, string groupTitle, bool showNone) : base(owner, groupTitle)
			{
				this.m_ShowNoneItem = showNone;
				this.m_ListMode = false;
				this.InitBuiltinResources();
				this.ItemsWantedShown = 2147483647;
				this.m_Collapsable = false;
			}

			public override void UpdateAssets()
			{
				if (this.m_FilteredHierarchy.hierarchyType == HierarchyType.Assets)
				{
					this.m_ActiveBuiltinList = this.m_CurrentBuiltinResources;
				}
				else
				{
					this.m_ActiveBuiltinList = new BuiltinResource[0];
				}
				this.ItemsAvailable = this.m_FilteredHierarchy.results.Length + this.m_ActiveBuiltinList.Length;
			}

			protected override float GetHeaderHeight()
			{
				return 0f;
			}

			protected override void DrawHeader(float yOffset, bool collapsable)
			{
				if (this.GetHeaderHeight() > 0f)
				{
					Rect rect = new Rect(0f, base.GetHeaderYPosInScrollArea(yOffset), this.m_Owner.GetVisibleWidth(), this.kGroupSeparatorHeight);
					base.DrawHeaderBackground(rect, true);
					if (collapsable)
					{
						rect.x += 7f;
						bool visible = this.Visible;
						this.Visible = GUI.Toggle(new Rect(rect.x, rect.y, 14f, rect.height), this.Visible, GUIContent.none, ObjectListArea.s_Styles.groupFoldout);
						if (visible ^ this.Visible)
						{
							EditorPrefs.SetBool(this.m_GroupSeparatorTitle, this.Visible);
						}
						rect.x += 7f;
					}
					float num = 0f;
					if (this.m_Owner.drawLocalAssetHeader != null)
					{
						num = this.m_Owner.drawLocalAssetHeader(rect) + 10f;
					}
					rect.x += num;
					rect.width -= num;
					if (rect.width > 0f)
					{
						base.DrawItemCount(rect);
					}
				}
			}

			public override void UpdateHeight()
			{
				this.m_Height = this.GetHeaderHeight();
				if (this.Visible)
				{
					this.m_Height += this.m_Grid.height;
				}
			}

			private bool IsCreatingAtThisIndex(int itemIdx)
			{
				return this.m_Owner.m_State.m_NewAssetIndexInList == itemIdx;
			}

			protected override void DrawInternal(int beginIndex, int endIndex, float yOffset)
			{
				int num = beginIndex;
				int num2 = 0;
				FilteredHierarchy.FilterResult[] results = this.m_FilteredHierarchy.results;
				bool flag = this.m_FilteredHierarchy.searchFilter.GetState() == SearchFilter.State.FolderBrowsing;
				yOffset += this.GetHeaderHeight();
				if (this.m_NoneList.Length > 0)
				{
					if (beginIndex < 1)
					{
						Rect position = this.m_Grid.CalcRect(num, yOffset);
						this.DrawItem(position, null, this.m_NoneList[0], flag);
						num++;
					}
					num2++;
				}
				if (!this.ListMode && flag)
				{
					this.DrawSubAssetBackground(beginIndex, endIndex, yOffset);
				}
				if (Event.current.type == EventType.Repaint)
				{
					this.ClearDirtyStateTracking();
				}
				int num3 = num - num2;
				while (true)
				{
					if (this.IsCreatingAtThisIndex(num))
					{
						BuiltinResource builtinResource = new BuiltinResource();
						builtinResource.m_Name = this.m_Owner.GetCreateAssetUtility().originalName;
						builtinResource.m_InstanceID = this.m_Owner.GetCreateAssetUtility().instanceID;
						this.DrawItem(this.m_Grid.CalcRect(num, yOffset), null, builtinResource, flag);
						num++;
						num2++;
					}
					if (num > endIndex)
					{
						break;
					}
					if (num3 >= results.Length)
					{
						break;
					}
					FilteredHierarchy.FilterResult filterItem = results[num3];
					Rect position = this.m_Grid.CalcRect(num, yOffset);
					this.DrawItem(position, filterItem, null, flag);
					num++;
					num3++;
				}
				num2 += results.Length;
				if (this.m_ActiveBuiltinList.Length > 0)
				{
					int num4 = beginIndex - num2;
					num4 = Math.Max(num4, 0);
					int num5 = num4;
					while (num5 < this.m_ActiveBuiltinList.Length && num <= endIndex)
					{
						this.DrawItem(this.m_Grid.CalcRect(num, yOffset), null, this.m_ActiveBuiltinList[num5], flag);
						num++;
						num5++;
					}
				}
				if (!this.ListMode && AssetPreview.IsLoadingAssetPreviews(this.m_Owner.GetAssetPreviewManagerID()))
				{
					this.m_Owner.Repaint();
				}
			}

			private void ClearDirtyStateTracking()
			{
				this.m_LastRenderedAssetInstanceIDs.Clear();
				this.m_LastRenderedAssetDirtyIDs.Clear();
			}

			private void AddDirtyStateFor(int instanceID)
			{
				this.m_LastRenderedAssetInstanceIDs.Add(instanceID);
				this.m_LastRenderedAssetDirtyIDs.Add(EditorUtility.GetDirtyIndex(instanceID));
			}

			public bool IsAnyLastRenderedAssetsDirty()
			{
				bool result;
				for (int i = 0; i < this.m_LastRenderedAssetInstanceIDs.Count; i++)
				{
					int dirtyIndex = EditorUtility.GetDirtyIndex(this.m_LastRenderedAssetInstanceIDs[i]);
					if (dirtyIndex != this.m_LastRenderedAssetDirtyIDs[i])
					{
						this.m_LastRenderedAssetDirtyIDs[i] = dirtyIndex;
						result = true;
						return result;
					}
				}
				result = false;
				return result;
			}

			protected override void HandleUnusedDragEvents(float yOffset)
			{
				if (this.m_Owner.allowDragging)
				{
					Event current = Event.current;
					EventType type = current.type;
					if (type == EventType.DragUpdated || type == EventType.DragPerform)
					{
						Rect rect = new Rect(0f, yOffset, this.m_Owner.m_TotalRect.width, (this.m_Owner.m_TotalRect.height <= base.Height) ? base.Height : this.m_Owner.m_TotalRect.height);
						if (rect.Contains(current.mousePosition))
						{
							bool flag = this.m_FilteredHierarchy.searchFilter.GetState() == SearchFilter.State.FolderBrowsing;
							DragAndDropVisualMode dragAndDropVisualMode;
							if (flag && this.m_FilteredHierarchy.searchFilter.folders.Length == 1)
							{
								string assetPath = this.m_FilteredHierarchy.searchFilter.folders[0];
								int mainAssetInstanceID = AssetDatabase.GetMainAssetInstanceID(assetPath);
								bool flag2 = current.type == EventType.DragPerform;
								dragAndDropVisualMode = this.DoDrag(mainAssetInstanceID, flag2);
								if (flag2 && dragAndDropVisualMode != DragAndDropVisualMode.None)
								{
									DragAndDrop.AcceptDrag();
								}
							}
							else
							{
								dragAndDropVisualMode = DragAndDropVisualMode.None;
							}
							DragAndDrop.visualMode = dragAndDropVisualMode;
							current.Use();
						}
					}
				}
			}

			private void HandleMouseWithDragging(int instanceID, int controlID, Rect rect)
			{
				Event current = Event.current;
				EventType typeForControl = current.GetTypeForControl(controlID);
				switch (typeForControl)
				{
				case EventType.MouseDown:
					if (Event.current.button == 0 && rect.Contains(Event.current.mousePosition))
					{
						if (current.clickCount == 2)
						{
							this.m_Owner.SetSelection(new int[]
							{
								instanceID
							}, true);
							this.m_DragSelection.Clear();
						}
						else
						{
							this.m_DragSelection = this.GetNewSelection(instanceID, true, false);
							GUIUtility.hotControl = controlID;
							DragAndDropDelay dragAndDropDelay = (DragAndDropDelay)GUIUtility.GetStateObject(typeof(DragAndDropDelay), controlID);
							dragAndDropDelay.mouseDownPosition = Event.current.mousePosition;
							this.m_Owner.ScrollToPosition(ObjectListArea.AdjustRectForFraming(rect));
						}
						current.Use();
					}
					else if (Event.current.button == 1 && rect.Contains(Event.current.mousePosition))
					{
						this.m_Owner.SetSelection(this.GetNewSelection(instanceID, true, false).ToArray(), false);
					}
					return;
				case EventType.MouseUp:
					if (GUIUtility.hotControl == controlID)
					{
						if (rect.Contains(current.mousePosition))
						{
							bool flag;
							if (this.ListMode)
							{
								rect.x += 28f;
								rect.width += 28f;
								flag = rect.Contains(current.mousePosition);
							}
							else
							{
								rect.y = rect.y + rect.height - ObjectListArea.s_Styles.resultsGridLabel.fixedHeight;
								rect.height = ObjectListArea.s_Styles.resultsGridLabel.fixedHeight;
								flag = rect.Contains(current.mousePosition);
							}
							List<int> selectedInstanceIDs = this.m_Owner.m_State.m_SelectedInstanceIDs;
							if (flag && this.m_Owner.allowRenaming && this.m_Owner.m_AllowRenameOnMouseUp && selectedInstanceIDs.Count == 1 && selectedInstanceIDs[0] == instanceID && !EditorGUIUtility.HasHolddownKeyModifiers(current))
							{
								this.m_Owner.BeginRename(0.5f);
							}
							else
							{
								List<int> newSelection = this.GetNewSelection(instanceID, false, false);
								this.m_Owner.SetSelection(newSelection.ToArray(), false);
							}
							GUIUtility.hotControl = 0;
							current.Use();
						}
						this.m_DragSelection.Clear();
					}
					return;
				case EventType.MouseMove:
				{
					IL_25:
					if (typeForControl == EventType.DragUpdated || typeForControl == EventType.DragPerform)
					{
						bool flag2 = current.type == EventType.DragPerform;
						if (rect.Contains(current.mousePosition))
						{
							DragAndDropVisualMode dragAndDropVisualMode = this.DoDrag(instanceID, flag2);
							if (dragAndDropVisualMode != DragAndDropVisualMode.None)
							{
								if (flag2)
								{
									DragAndDrop.AcceptDrag();
								}
								this.m_DropTargetControlID = controlID;
								DragAndDrop.visualMode = dragAndDropVisualMode;
								current.Use();
							}
							if (flag2)
							{
								this.m_DropTargetControlID = 0;
							}
						}
						if (flag2)
						{
							this.m_DragSelection.Clear();
						}
						return;
					}
					if (typeForControl == EventType.DragExited)
					{
						this.m_DragSelection.Clear();
						return;
					}
					if (typeForControl != EventType.ContextClick)
					{
						return;
					}
					Rect drawRect = rect;
					drawRect.x += 2f;
					drawRect = ProjectHooks.GetOverlayRect(drawRect);
					if (drawRect.width != rect.width && Provider.isActive && drawRect.Contains(current.mousePosition))
					{
						EditorUtility.DisplayPopupMenu(new Rect(current.mousePosition.x, current.mousePosition.y, 0f, 0f), "Assets/Version Control", new MenuCommand(null, 0));
						current.Use();
					}
					return;
				}
				case EventType.MouseDrag:
					if (GUIUtility.hotControl == controlID)
					{
						DragAndDropDelay dragAndDropDelay2 = (DragAndDropDelay)GUIUtility.GetStateObject(typeof(DragAndDropDelay), controlID);
						if (dragAndDropDelay2.CanStartDrag())
						{
							this.StartDrag(instanceID, this.m_DragSelection);
							GUIUtility.hotControl = 0;
						}
						current.Use();
					}
					return;
				}
				goto IL_25;
			}

			private void HandleMouseWithoutDragging(int instanceID, int controlID, Rect position)
			{
				Event current = Event.current;
				EventType typeForControl = current.GetTypeForControl(controlID);
				if (typeForControl != EventType.MouseDown)
				{
					if (typeForControl == EventType.ContextClick)
					{
						if (position.Contains(current.mousePosition))
						{
							this.m_Owner.SetSelection(new int[]
							{
								instanceID
							}, false);
							Rect drawRect = position;
							drawRect.x += 2f;
							drawRect = ProjectHooks.GetOverlayRect(drawRect);
							if (drawRect.width != position.width && Provider.isActive && drawRect.Contains(current.mousePosition))
							{
								EditorUtility.DisplayPopupMenu(new Rect(current.mousePosition.x, current.mousePosition.y, 0f, 0f), "Assets/Version Control", new MenuCommand(null, 0));
								current.Use();
							}
						}
					}
				}
				else if (current.button == 0 && position.Contains(current.mousePosition))
				{
					this.m_Owner.Repaint();
					if (current.clickCount == 1)
					{
						this.m_Owner.ScrollToPosition(ObjectListArea.AdjustRectForFraming(position));
					}
					current.Use();
					List<int> newSelection = this.GetNewSelection(instanceID, false, false);
					this.m_Owner.SetSelection(newSelection.ToArray(), current.clickCount == 2);
				}
			}

			public void ChangeExpandedState(int instanceID, bool expanded)
			{
				this.m_Owner.m_State.m_ExpandedInstanceIDs.Remove(instanceID);
				if (expanded)
				{
					this.m_Owner.m_State.m_ExpandedInstanceIDs.Add(instanceID);
				}
				this.m_FilteredHierarchy.RefreshVisibleItems(this.m_Owner.m_State.m_ExpandedInstanceIDs);
			}

			private bool IsExpanded(int instanceID)
			{
				return this.m_Owner.m_State.m_ExpandedInstanceIDs.IndexOf(instanceID) >= 0;
			}

			private void SelectAndFrameParentOf(int instanceID)
			{
				int num = 0;
				FilteredHierarchy.FilterResult[] results = this.m_FilteredHierarchy.results;
				for (int i = 0; i < results.Length; i++)
				{
					if (results[i].instanceID == instanceID)
					{
						if (results[i].isMainRepresentation)
						{
							num = 0;
						}
						break;
					}
					if (results[i].isMainRepresentation)
					{
						num = results[i].instanceID;
					}
				}
				if (num != 0)
				{
					this.m_Owner.SetSelection(new int[]
					{
						num
					}, false);
					this.m_Owner.Frame(num, true, false);
				}
			}

			private bool IsRenaming(int instanceID)
			{
				RenameOverlay renameOverlay = this.m_Owner.GetRenameOverlay();
				return renameOverlay.IsRenaming() && renameOverlay.userData == instanceID && !renameOverlay.isWaitingForDelay;
			}

			protected void DrawSubAssetRowBg(int startSubAssetIndex, int endSubAssetIndex, bool continued, float yOffset)
			{
				Rect rect = this.m_Grid.CalcRect(startSubAssetIndex, yOffset);
				Rect rect2 = this.m_Grid.CalcRect(endSubAssetIndex, yOffset);
				float num = 30f;
				float num2 = 128f;
				float num3 = rect.width / num2;
				float num4 = 9f * num3;
				float num5 = 4f;
				bool flag = startSubAssetIndex % this.m_Grid.columns == 0;
				float num6 = (!flag) ? (this.m_Grid.horizontalSpacing + num3 * 10f) : (18f * num3);
				Rect position = new Rect(rect.x - num6, rect.y + num5, num * num3, rect.width - num5 * 2f + num4 - 1f);
				position.y = Mathf.Round(position.y);
				position.height = Mathf.Ceil(position.height);
				ObjectListArea.s_Styles.subAssetBg.Draw(position, GUIContent.none, false, false, false, false);
				float num7 = num * num3;
				bool flag2 = endSubAssetIndex % this.m_Grid.columns == this.m_Grid.columns - 1;
				float num8 = (!continued && !flag2) ? (8f * num3) : (16f * num3);
				Rect position2 = new Rect(rect2.xMax - num7 + num8, rect2.y + num5, num7, position.height);
				position2.y = Mathf.Round(position2.y);
				position2.height = Mathf.Ceil(position2.height);
				GUIStyle gUIStyle = (!continued) ? ObjectListArea.s_Styles.subAssetBgCloseEnded : ObjectListArea.s_Styles.subAssetBgOpenEnded;
				gUIStyle.Draw(position2, GUIContent.none, false, false, false, false);
				position = new Rect(position.xMax, position.y, position2.xMin - position.xMax, position.height);
				position.y = Mathf.Round(position.y);
				position.height = Mathf.Ceil(position.height);
				ObjectListArea.s_Styles.subAssetBgMiddle.Draw(position, GUIContent.none, false, false, false, false);
			}

			private void DrawSubAssetBackground(int beginIndex, int endIndex, float yOffset)
			{
				if (Event.current.type == EventType.Repaint)
				{
					FilteredHierarchy.FilterResult[] results = this.m_FilteredHierarchy.results;
					int columns = this.m_Grid.columns;
					int num = (endIndex - beginIndex) / columns + 1;
					for (int i = 0; i < num; i++)
					{
						int num2 = -1;
						int endSubAssetIndex = -1;
						for (int j = 0; j < columns; j++)
						{
							int num3 = beginIndex + (j + i * columns);
							if (num3 >= results.Length)
							{
								break;
							}
							FilteredHierarchy.FilterResult filterResult = results[num3];
							if (!filterResult.isMainRepresentation)
							{
								if (num2 == -1)
								{
									num2 = num3;
								}
								endSubAssetIndex = num3;
							}
							else if (num2 != -1)
							{
								this.DrawSubAssetRowBg(num2, endSubAssetIndex, false, yOffset);
								num2 = -1;
								endSubAssetIndex = -1;
							}
						}
						if (num2 != -1)
						{
							bool continued = false;
							if (i < num - 1)
							{
								int num4 = beginIndex + (i + 1) * columns;
								if (num4 < results.Length)
								{
									continued = !results[num4].isMainRepresentation;
								}
							}
							this.DrawSubAssetRowBg(num2, endSubAssetIndex, continued, yOffset);
						}
					}
				}
			}

			private void DrawItem(Rect position, FilteredHierarchy.FilterResult filterItem, BuiltinResource builtinResource, bool isFolderBrowsing)
			{
				Event current = Event.current;
				Rect selectionRect = position;
				int num = 0;
				bool flag = false;
				if (filterItem != null)
				{
					num = filterItem.instanceID;
					flag = (filterItem.hasChildren && !filterItem.isFolder && isFolderBrowsing);
				}
				else if (builtinResource != null)
				{
					num = builtinResource.m_InstanceID;
				}
				int controlIDFromInstanceID = ObjectListArea.LocalGroup.GetControlIDFromInstanceID(num);
				bool flag2;
				if (this.m_Owner.allowDragging)
				{
					flag2 = ((this.m_DragSelection.Count <= 0) ? this.m_Owner.IsSelected(num) : this.m_DragSelection.Contains(num));
				}
				else
				{
					flag2 = this.m_Owner.IsSelected(num);
				}
				if (flag2 && num == this.m_Owner.m_State.m_LastClickedInstanceID)
				{
					this.m_LastClickedDrawTime = EditorApplication.timeSinceStartup;
				}
				Rect position2 = new Rect(position.x + (float)ObjectListArea.s_Styles.groupFoldout.margin.left, position.y, (float)ObjectListArea.s_Styles.groupFoldout.padding.left, position.height);
				if (flag && !this.ListMode)
				{
					float num2 = position.height / 128f;
					float num3 = 28f * num2;
					float height = 32f * num2;
					position2 = new Rect(position.xMax - num3 * 0.5f, position.y + (position.height - ObjectListArea.s_Styles.resultsGridLabel.fixedHeight) * 0.5f - num3 * 0.5f, num3, height);
				}
				bool flag3 = false;
				if (flag2 && current.type == EventType.KeyDown && this.m_Owner.HasFocus())
				{
					KeyCode keyCode = current.keyCode;
					if (keyCode != KeyCode.LeftArrow)
					{
						if (keyCode == KeyCode.RightArrow)
						{
							if (this.ListMode || this.m_Owner.IsPreviewIconExpansionModifierPressed())
							{
								if (!this.IsExpanded(num))
								{
									flag3 = true;
								}
								current.Use();
							}
						}
					}
					else if (this.ListMode || this.m_Owner.IsPreviewIconExpansionModifierPressed())
					{
						if (this.IsExpanded(num))
						{
							flag3 = true;
						}
						else
						{
							this.SelectAndFrameParentOf(num);
						}
						current.Use();
					}
				}
				if (flag && current.type == EventType.MouseDown && current.button == 0 && position2.Contains(current.mousePosition))
				{
					flag3 = true;
				}
				if (flag3)
				{
					bool flag4 = !this.IsExpanded(num);
					if (flag4)
					{
						this.m_ItemFader.Start(this.m_FilteredHierarchy.GetSubAssetInstanceIDs(num));
					}
					this.ChangeExpandedState(num, flag4);
					current.Use();
					GUIUtility.ExitGUI();
				}
				bool flag5 = this.IsRenaming(num);
				Rect rect = position;
				if (!this.ListMode)
				{
					rect = new Rect(position.x, position.yMax + 1f - ObjectListArea.s_Styles.resultsGridLabel.fixedHeight, position.width - 1f, ObjectListArea.s_Styles.resultsGridLabel.fixedHeight);
				}
				int num4 = (!Provider.isActive || !this.ListMode) ? 0 : 14;
				float num5 = position2.xMax;
				if (this.ListMode)
				{
					selectionRect.x = num5;
					if (filterItem != null && !filterItem.isMainRepresentation && isFolderBrowsing)
					{
						num5 = 28f;
						selectionRect.x = 28f + (float)num4 * 0.5f;
					}
					selectionRect.width -= selectionRect.x;
				}
				if (Event.current.type == EventType.Repaint)
				{
					if (this.m_DropTargetControlID == controlIDFromInstanceID && !position.Contains(current.mousePosition))
					{
						this.m_DropTargetControlID = 0;
					}
					bool flag6 = controlIDFromInstanceID == this.m_DropTargetControlID && this.m_DragSelection.IndexOf(this.m_DropTargetControlID) == -1;
					string text = (filterItem == null) ? builtinResource.m_Name : filterItem.name;
					if (this.ListMode)
					{
						if (flag5)
						{
							flag2 = false;
							text = "";
						}
						position.width = Mathf.Max(position.width, 500f);
						this.m_Content.text = text;
						this.m_Content.image = null;
						Texture2D texture2D = (filterItem == null) ? AssetPreview.GetAssetPreview(num, this.m_Owner.GetAssetPreviewManagerID()) : filterItem.icon;
						if (texture2D == null && this.m_Owner.GetCreateAssetUtility().icon != null)
						{
							texture2D = this.m_Owner.GetCreateAssetUtility().icon;
						}
						if (flag2)
						{
							ObjectListArea.s_Styles.resultsLabel.Draw(position, GUIContent.none, false, false, flag2, this.m_Owner.HasFocus());
						}
						if (flag6)
						{
							ObjectListArea.s_Styles.resultsLabel.Draw(position, GUIContent.none, true, true, false, false);
						}
						ObjectListArea.LocalGroup.DrawIconAndLabel(new Rect(num5, position.y, position.width - num5, position.height), filterItem, text, texture2D, flag2, this.m_Owner.HasFocus());
						if (flag)
						{
							ObjectListArea.s_Styles.groupFoldout.Draw(position2, !this.ListMode, !this.ListMode, this.IsExpanded(num), false);
						}
					}
					else
					{
						bool flag7 = false;
						if (this.m_Owner.GetCreateAssetUtility().instanceID == num && this.m_Owner.GetCreateAssetUtility().icon != null)
						{
							this.m_Content.image = this.m_Owner.GetCreateAssetUtility().icon;
						}
						else
						{
							this.m_Content.image = AssetPreview.GetAssetPreview(num, this.m_Owner.GetAssetPreviewManagerID());
							if (this.m_Content.image != null)
							{
								flag7 = true;
							}
							if (filterItem != null)
							{
								if (this.m_Content.image == null)
								{
									this.m_Content.image = filterItem.icon;
								}
								if (isFolderBrowsing && !filterItem.isMainRepresentation)
								{
									flag7 = false;
								}
							}
						}
						float num6 = (!flag7) ? 0f : 2f;
						position.height -= ObjectListArea.s_Styles.resultsGridLabel.fixedHeight + 2f * num6;
						position.y += num6;
						Rect rect2 = (!(this.m_Content.image == null)) ? ObjectListArea.LocalGroup.ActualImageDrawPosition(position, (float)this.m_Content.image.width, (float)this.m_Content.image.height) : default(Rect);
						this.m_Content.text = null;
						float num7 = 1f;
						if (filterItem != null)
						{
							this.AddDirtyStateFor(filterItem.instanceID);
							if (!filterItem.isMainRepresentation && isFolderBrowsing)
							{
								position.x += 4f;
								position.y += 4f;
								position.width -= 8f;
								position.height -= 8f;
								rect2 = ((!(this.m_Content.image == null)) ? ObjectListArea.LocalGroup.ActualImageDrawPosition(position, (float)this.m_Content.image.width, (float)this.m_Content.image.height) : default(Rect));
								num7 = this.m_ItemFader.GetAlpha(filterItem.instanceID);
								if (num7 < 1f)
								{
									this.m_Owner.Repaint();
								}
							}
							if (flag7 && filterItem.iconDrawStyle == IconDrawStyle.NonTexture)
							{
								ObjectListArea.s_Styles.previewBg.Draw(rect2, GUIContent.none, false, false, false, false);
							}
						}
						Color color = GUI.color;
						if (flag2)
						{
							GUI.color *= new Color(0.85f, 0.9f, 1f);
						}
						if (this.m_Content.image != null)
						{
							Color color2 = GUI.color;
							if (num7 < 1f)
							{
								GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, num7);
							}
							ObjectListArea.s_Styles.resultsGrid.Draw(rect2, this.m_Content, false, false, flag2, this.m_Owner.HasFocus());
							if (num7 < 1f)
							{
								GUI.color = color2;
							}
						}
						if (flag2)
						{
							GUI.color = color;
						}
						if (flag7)
						{
							Rect position3 = new RectOffset(1, 1, 1, 1).Remove(ObjectListArea.s_Styles.textureIconDropShadow.border.Add(rect2));
							ObjectListArea.s_Styles.textureIconDropShadow.Draw(position3, GUIContent.none, false, false, flag2 || flag6, this.m_Owner.HasFocus() || flag5 || flag6);
						}
						if (!flag5)
						{
							if (flag6)
							{
								ObjectListArea.s_Styles.resultsLabel.Draw(new Rect(rect.x - 10f, rect.y, rect.width + 20f, rect.height), GUIContent.none, true, true, false, false);
							}
							text = this.m_Owner.GetCroppedLabelText(num, text, position.width);
							ObjectListArea.s_Styles.resultsGridLabel.Draw(rect, text, false, false, flag2, this.m_Owner.HasFocus());
						}
						if (flag)
						{
							ObjectListArea.s_Styles.subAssetExpandButton.Draw(position2, !this.ListMode, !this.ListMode, this.IsExpanded(num), false);
						}
						if (filterItem != null && filterItem.isMainRepresentation)
						{
							bool flag8 = CollabAccess.Instance.IsServiceEnabled();
							if (flag8 && Collab.instance.GetCollabInfo().whitelisted)
							{
								CollabProjectHook.OnProjectWindowItemIconOverlay(filterItem.guid, position);
							}
							ProjectHooks.OnProjectWindowItem(filterItem.guid, position);
						}
					}
				}
				if (flag5)
				{
					if (this.ListMode)
					{
						float num8 = (float)num4 + 16f + 2f + (float)ObjectListArea.s_Styles.resultsLabel.margin.left;
						rect.x = selectionRect.x + num8;
						rect.width -= rect.x;
					}
					else
					{
						rect.x -= 4f;
						rect.width += 8f;
					}
					this.m_Owner.GetRenameOverlay().editFieldRect = rect;
					this.m_Owner.HandleRenameOverlay();
				}
				if (EditorApplication.projectWindowItemOnGUI != null && filterItem != null && this.m_Owner.allowUserRenderingHook)
				{
					EditorApplication.projectWindowItemOnGUI(filterItem.guid, selectionRect);
				}
				if (this.m_Owner.allowDragging)
				{
					this.HandleMouseWithDragging(num, controlIDFromInstanceID, position);
				}
				else
				{
					this.HandleMouseWithoutDragging(num, controlIDFromInstanceID, position);
				}
			}

			private static Rect ActualImageDrawPosition(Rect position, float imageWidth, float imageHeight)
			{
				Rect result;
				if (imageWidth > position.width || imageHeight > position.height)
				{
					Rect rect = default(Rect);
					Rect rect2 = default(Rect);
					float imageAspect = imageWidth / imageHeight;
					GUI.CalculateScaledTextureRects(position, ScaleMode.ScaleToFit, imageAspect, ref rect, ref rect2);
					result = rect;
				}
				else
				{
					float x = position.x + Mathf.Round((position.width - imageWidth) / 2f);
					float y = position.y + Mathf.Round((position.height - imageHeight) / 2f);
					result = new Rect(x, y, imageWidth, imageHeight);
				}
				return result;
			}

			public List<KeyValuePair<string, int>> GetVisibleNameAndInstanceIDs()
			{
				List<KeyValuePair<string, int>> list = new List<KeyValuePair<string, int>>();
				if (this.m_NoneList.Length > 0)
				{
					list.Add(new KeyValuePair<string, int>(this.m_NoneList[0].m_Name, this.m_NoneList[0].m_InstanceID));
				}
				FilteredHierarchy.FilterResult[] results = this.m_FilteredHierarchy.results;
				for (int i = 0; i < results.Length; i++)
				{
					FilteredHierarchy.FilterResult filterResult = results[i];
					list.Add(new KeyValuePair<string, int>(filterResult.name, filterResult.instanceID));
				}
				for (int j = 0; j < this.m_ActiveBuiltinList.Length; j++)
				{
					list.Add(new KeyValuePair<string, int>(this.m_ActiveBuiltinList[j].m_Name, this.m_ActiveBuiltinList[j].m_InstanceID));
				}
				return list;
			}

			private void BeginPing(int instanceID)
			{
			}

			public List<int> GetInstanceIDs()
			{
				List<int> list = new List<int>();
				if (this.m_NoneList.Length > 0)
				{
					list.Add(this.m_NoneList[0].m_InstanceID);
				}
				FilteredHierarchy.FilterResult[] results = this.m_FilteredHierarchy.results;
				for (int i = 0; i < results.Length; i++)
				{
					FilteredHierarchy.FilterResult filterResult = results[i];
					list.Add(filterResult.instanceID);
				}
				if (this.m_Owner.m_State.m_NewAssetIndexInList >= 0)
				{
					list.Add(this.m_Owner.GetCreateAssetUtility().instanceID);
				}
				for (int j = 0; j < this.m_ActiveBuiltinList.Length; j++)
				{
					list.Add(this.m_ActiveBuiltinList[j].m_InstanceID);
				}
				return list;
			}

			public List<int> GetNewSelection(int clickedInstanceID, bool beginOfDrag, bool useShiftAsActionKey)
			{
				List<int> instanceIDs = this.GetInstanceIDs();
				List<int> selectedInstanceIDs = this.m_Owner.m_State.m_SelectedInstanceIDs;
				int lastClickedInstanceID = this.m_Owner.m_State.m_LastClickedInstanceID;
				bool allowMultiSelect = this.m_Owner.allowMultiSelect;
				return InternalEditorUtility.GetNewSelection(clickedInstanceID, instanceIDs, selectedInstanceIDs, lastClickedInstanceID, beginOfDrag, useShiftAsActionKey, allowMultiSelect);
			}

			public override void UpdateFilter(HierarchyType hierarchyType, SearchFilter searchFilter, bool foldersFirst)
			{
				this.RefreshHierarchy(hierarchyType, searchFilter, foldersFirst);
				this.RefreshBuiltinResourceList(searchFilter);
			}

			private void RefreshHierarchy(HierarchyType hierarchyType, SearchFilter searchFilter, bool foldersFirst)
			{
				this.m_FilteredHierarchy = new FilteredHierarchy(hierarchyType);
				this.m_FilteredHierarchy.foldersFirst = foldersFirst;
				this.m_FilteredHierarchy.searchFilter = searchFilter;
				this.m_FilteredHierarchy.RefreshVisibleItems(this.m_Owner.m_State.m_ExpandedInstanceIDs);
			}

			private void RefreshBuiltinResourceList(SearchFilter searchFilter)
			{
				if (!this.m_Owner.allowBuiltinResources || searchFilter.GetState() == SearchFilter.State.FolderBrowsing || searchFilter.GetState() == SearchFilter.State.EmptySearchFilter)
				{
					this.m_CurrentBuiltinResources = new BuiltinResource[0];
				}
				else
				{
					List<BuiltinResource> list = new List<BuiltinResource>();
					if (searchFilter.assetLabels != null && searchFilter.assetLabels.Length > 0)
					{
						this.m_CurrentBuiltinResources = list.ToArray();
					}
					else
					{
						List<int> list2 = new List<int>();
						string[] classNames = searchFilter.classNames;
						for (int i = 0; i < classNames.Length; i++)
						{
							string classString = classNames[i];
							int num = BaseObjectTools.StringToClassIDCaseInsensitive(classString);
							if (num >= 0)
							{
								list2.Add(num);
							}
						}
						if (list2.Count > 0)
						{
							foreach (KeyValuePair<string, BuiltinResource[]> current in this.m_BuiltinResourceMap)
							{
								int classID = BaseObjectTools.StringToClassID(current.Key);
								foreach (int current2 in list2)
								{
									if (BaseObjectTools.IsDerivedFromClassID(classID, current2))
									{
										list.AddRange(current.Value);
									}
								}
							}
						}
						BuiltinResource[] array = list.ToArray();
						if (array.Length > 0 && !string.IsNullOrEmpty(searchFilter.nameFilter))
						{
							List<BuiltinResource> list3 = new List<BuiltinResource>();
							string value = searchFilter.nameFilter.ToLower();
							BuiltinResource[] array2 = array;
							for (int j = 0; j < array2.Length; j++)
							{
								BuiltinResource builtinResource = array2[j];
								if (builtinResource.m_Name.ToLower().Contains(value))
								{
									list3.Add(builtinResource);
								}
							}
							array = list3.ToArray();
						}
						this.m_CurrentBuiltinResources = array;
					}
				}
			}

			public string GetNameOfLocalAsset(int instanceID)
			{
				FilteredHierarchy.FilterResult[] results = this.m_FilteredHierarchy.results;
				string result;
				for (int i = 0; i < results.Length; i++)
				{
					FilteredHierarchy.FilterResult filterResult = results[i];
					if (filterResult.instanceID == instanceID)
					{
						result = filterResult.name;
						return result;
					}
				}
				result = null;
				return result;
			}

			public bool IsBuiltinAsset(int instanceID)
			{
				bool result;
				foreach (KeyValuePair<string, BuiltinResource[]> current in this.m_BuiltinResourceMap)
				{
					BuiltinResource[] value = current.Value;
					for (int i = 0; i < value.Length; i++)
					{
						if (value[i].m_InstanceID == instanceID)
						{
							result = true;
							return result;
						}
					}
				}
				result = false;
				return result;
			}

			private void InitBuiltinAssetType(Type type)
			{
				if (type == null)
				{
					Debug.LogWarning("ObjectSelector::InitBuiltinAssetType: type is null!");
				}
				else
				{
					string text = type.ToString().Substring(type.Namespace.ToString().Length + 1);
					int num = BaseObjectTools.StringToClassID(text);
					if (num < 0)
					{
						Debug.LogWarning("ObjectSelector::InitBuiltinAssetType: class '" + text + "' not found");
					}
					else
					{
						BuiltinResource[] builtinResourceList = EditorGUIUtility.GetBuiltinResourceList(num);
						if (builtinResourceList != null)
						{
							this.m_BuiltinResourceMap.Add(text, builtinResourceList);
						}
					}
				}
			}

			public void InitBuiltinResources()
			{
				if (this.m_BuiltinResourceMap == null)
				{
					this.m_BuiltinResourceMap = new Dictionary<string, BuiltinResource[]>();
					if (this.m_ShowNoneItem)
					{
						this.m_NoneList = new BuiltinResource[1];
						this.m_NoneList[0] = new BuiltinResource();
						this.m_NoneList[0].m_InstanceID = 0;
						this.m_NoneList[0].m_Name = "None";
					}
					else
					{
						this.m_NoneList = new BuiltinResource[0];
					}
					this.InitBuiltinAssetType(typeof(Mesh));
					this.InitBuiltinAssetType(typeof(Material));
					this.InitBuiltinAssetType(typeof(Texture2D));
					this.InitBuiltinAssetType(typeof(Font));
					this.InitBuiltinAssetType(typeof(Shader));
					this.InitBuiltinAssetType(typeof(Sprite));
					this.InitBuiltinAssetType(typeof(LightmapParameters));
				}
			}

			public void PrintBuiltinResourcesAvailable()
			{
				string text = "";
				text += "ObjectSelector -Builtin Assets Available:\n";
				foreach (KeyValuePair<string, BuiltinResource[]> current in this.m_BuiltinResourceMap)
				{
					BuiltinResource[] value = current.Value;
					text = text + "    " + current.Key + ": ";
					for (int i = 0; i < value.Length; i++)
					{
						if (i != 0)
						{
							text += ", ";
						}
						text += value[i].m_Name;
					}
					text += "\n";
				}
				Debug.Log(text);
			}

			public int IndexOfNewText(string newText, bool isCreatingNewFolder, bool foldersFirst)
			{
				int i = 0;
				if (this.m_ShowNoneItem)
				{
					i++;
				}
				int result;
				while (i < this.m_FilteredHierarchy.results.Length)
				{
					FilteredHierarchy.FilterResult filterResult = this.m_FilteredHierarchy.results[i];
					if (!foldersFirst || !filterResult.isFolder || isCreatingNewFolder)
					{
						if (foldersFirst && !filterResult.isFolder && isCreatingNewFolder)
						{
							break;
						}
						string assetPath = AssetDatabase.GetAssetPath(filterResult.instanceID);
						if (EditorUtility.NaturalCompare(Path.GetFileNameWithoutExtension(assetPath), newText) > 0)
						{
							result = i;
							return result;
						}
					}
					i++;
				}
				result = i;
				return result;
			}

			public int IndexOf(int instanceID)
			{
				int num = 0;
				int result;
				if (this.m_ShowNoneItem)
				{
					if (instanceID == 0)
					{
						result = 0;
						return result;
					}
					num++;
				}
				else if (instanceID == 0)
				{
					result = -1;
					return result;
				}
				FilteredHierarchy.FilterResult[] results = this.m_FilteredHierarchy.results;
				for (int i = 0; i < results.Length; i++)
				{
					FilteredHierarchy.FilterResult filterResult = results[i];
					if (this.m_Owner.m_State.m_NewAssetIndexInList == num)
					{
						num++;
					}
					if (filterResult.instanceID == instanceID)
					{
						result = num;
						return result;
					}
					num++;
				}
				BuiltinResource[] activeBuiltinList = this.m_ActiveBuiltinList;
				for (int j = 0; j < activeBuiltinList.Length; j++)
				{
					BuiltinResource builtinResource = activeBuiltinList[j];
					if (instanceID == builtinResource.m_InstanceID)
					{
						result = num;
						return result;
					}
					num++;
				}
				result = -1;
				return result;
			}

			public FilteredHierarchy.FilterResult LookupByInstanceID(int instanceID)
			{
				FilteredHierarchy.FilterResult result;
				if (instanceID == 0)
				{
					result = null;
				}
				else
				{
					int num = 0;
					FilteredHierarchy.FilterResult[] results = this.m_FilteredHierarchy.results;
					for (int i = 0; i < results.Length; i++)
					{
						FilteredHierarchy.FilterResult filterResult = results[i];
						if (this.m_Owner.m_State.m_NewAssetIndexInList == num)
						{
							num++;
						}
						if (filterResult.instanceID == instanceID)
						{
							result = filterResult;
							return result;
						}
						num++;
					}
					result = null;
				}
				return result;
			}

			public bool InstanceIdAtIndex(int index, out int instanceID)
			{
				instanceID = 0;
				bool result;
				if (index >= this.m_Grid.rows * this.m_Grid.columns)
				{
					result = false;
				}
				else
				{
					int num = 0;
					if (this.m_ShowNoneItem)
					{
						if (index == 0)
						{
							result = true;
							return result;
						}
						num++;
					}
					FilteredHierarchy.FilterResult[] results = this.m_FilteredHierarchy.results;
					for (int i = 0; i < results.Length; i++)
					{
						FilteredHierarchy.FilterResult filterResult = results[i];
						instanceID = filterResult.instanceID;
						if (num == index)
						{
							result = true;
							return result;
						}
						num++;
					}
					BuiltinResource[] activeBuiltinList = this.m_ActiveBuiltinList;
					for (int j = 0; j < activeBuiltinList.Length; j++)
					{
						BuiltinResource builtinResource = activeBuiltinList[j];
						instanceID = builtinResource.m_InstanceID;
						if (num == index)
						{
							result = true;
							return result;
						}
						num++;
					}
					result = (index < this.m_Grid.rows * this.m_Grid.columns);
				}
				return result;
			}

			public virtual void StartDrag(int draggedInstanceID, List<int> selectedInstanceIDs)
			{
				ProjectWindowUtil.StartDrag(draggedInstanceID, selectedInstanceIDs);
			}

			public DragAndDropVisualMode DoDrag(int dragToInstanceID, bool perform)
			{
				HierarchyProperty hierarchyProperty = new HierarchyProperty(HierarchyType.Assets);
				if (!hierarchyProperty.Find(dragToInstanceID, null))
				{
					hierarchyProperty = null;
				}
				return InternalEditorUtility.ProjectWindowDrag(hierarchyProperty, perform);
			}

			internal static int GetControlIDFromInstanceID(int instanceID)
			{
				return instanceID + 100000000;
			}

			public bool DoCharacterOffsetSelection()
			{
				bool result;
				if (Event.current.type == EventType.KeyDown && Event.current.shift)
				{
					StringComparison comparisonType = StringComparison.CurrentCultureIgnoreCase;
					string text = "";
					if (Selection.activeObject != null)
					{
						text = Selection.activeObject.name;
					}
					string value = new string(new char[]
					{
						Event.current.character
					});
					List<KeyValuePair<string, int>> visibleNameAndInstanceIDs = this.GetVisibleNameAndInstanceIDs();
					if (visibleNameAndInstanceIDs.Count == 0)
					{
						result = false;
						return result;
					}
					int num = 0;
					if (text.StartsWith(value, comparisonType))
					{
						for (int i = 0; i < visibleNameAndInstanceIDs.Count; i++)
						{
							if (visibleNameAndInstanceIDs[i].Key == text)
							{
								num = i + 1;
							}
						}
					}
					for (int j = 0; j < visibleNameAndInstanceIDs.Count; j++)
					{
						int index = (j + num) % visibleNameAndInstanceIDs.Count;
						if (visibleNameAndInstanceIDs[index].Key.StartsWith(value, comparisonType))
						{
							Selection.activeInstanceID = visibleNameAndInstanceIDs[index].Value;
							this.m_Owner.Repaint();
							result = true;
							return result;
						}
					}
				}
				result = false;
				return result;
			}

			public void ShowObjectsInList(int[] instanceIDs)
			{
				this.m_FilteredHierarchy = new FilteredHierarchy(HierarchyType.Assets);
				this.m_FilteredHierarchy.SetResults(instanceIDs);
			}

			public static void DrawIconAndLabel(Rect rect, FilteredHierarchy.FilterResult filterItem, string label, Texture2D icon, bool selected, bool focus)
			{
				float num = (!ObjectListArea.s_VCEnabled) ? 0f : 14f;
				rect.xMin += (float)ObjectListArea.s_Styles.resultsLabel.margin.left;
				ObjectListArea.s_Styles.resultsLabel.padding.left = (int)(num + 16f + 2f);
				ObjectListArea.s_Styles.resultsLabel.Draw(rect, label, false, false, selected, focus);
				Rect position = rect;
				position.width = 16f;
				position.x += num * 0.5f;
				if (icon != null)
				{
					GUI.DrawTexture(position, icon, ScaleMode.ScaleToFit);
				}
				if (filterItem != null && filterItem.isMainRepresentation)
				{
					Rect drawRect = rect;
					drawRect.width = num + 16f;
					bool flag = CollabAccess.Instance.IsServiceEnabled();
					if (flag && Collab.instance.GetCollabInfo().whitelisted)
					{
						CollabProjectHook.OnProjectWindowItemIconOverlay(filterItem.guid, drawRect);
					}
					ProjectHooks.OnProjectWindowItem(filterItem.guid, drawRect);
				}
			}
		}

		private static ObjectListArea.Styles s_Styles;

		private ObjectListAreaState m_State;

		private const int kHome = -2147483648;

		private const int kEnd = 2147483647;

		private const int kPageDown = 2147483646;

		private const int kPageUp = -2147483647;

		private int m_SelectionOffset = 0;

		private const float k_ListModeVersionControlOverlayPadding = 14f;

		private static bool s_VCEnabled = false;

		private PingData m_Ping = new PingData();

		private EditorWindow m_Owner;

		private int m_KeyboardControlID;

		private Dictionary<int, string> m_InstanceIDToCroppedNameMap = new Dictionary<int, string>();

		private int m_WidthUsedForCroppingName;

		private bool m_AllowRenameOnMouseUp = true;

		private Vector2 m_LastScrollPosition = new Vector2(0f, 0f);

		private double LastScrollTime = 0.0;

		private const double kDelayQueryAfterScroll = 0.0;

		public bool selectedAssetStoreAsset;

		internal Texture m_SelectedObjectIcon = null;

		private ObjectListArea.LocalGroup m_LocalAssets;

		private List<ObjectListArea.AssetStoreGroup> m_StoreAssets;

		private string m_AssetStoreError = "";

		private List<ObjectListArea.Group> m_Groups;

		private Rect m_TotalRect;

		private Rect m_VisibleRect;

		private const int kListLineHeight = 16;

		private int m_pingIndex;

		private int m_MinIconSize = 32;

		private int m_MinGridSize = 16;

		private int m_MaxGridSize = 96;

		private bool m_AllowThumbnails = true;

		private const int kSpaceForScrollBar = 16;

		private int m_LeftPaddingForPinging = 0;

		private bool m_FrameLastClickedItem = false;

		private bool m_ShowLocalAssetsOnly = true;

		private const double kQueryDelay = 0.2;

		public bool m_RequeryAssetStore;

		private bool m_QueryInProgress = false;

		private int m_ResizePreviewCacheTo = 0;

		private string m_LastAssetStoreQuerySearchFilter = "";

		private string[] m_LastAssetStoreQueryClassName = new string[0];

		private string[] m_LastAssetStoreQueryLabels = new string[0];

		private double m_LastAssetStoreQueryChangeTime = 0.0;

		private double m_NextDirtyCheck = 0.0;

		private Action m_RepaintWantedCallback;

		private Action<bool> m_ItemSelectedCallback;

		private Action m_KeyboardInputCallback;

		private Action m_GotKeyboardFocus;

		private Func<Rect, float> m_DrawLocalAssetHeader;

		private Action m_AssetStoreSearchEnded;

		internal static bool s_Debug = false;

		public float m_SpaceBetween = 6f;

		public float m_TopMargin = 10f;

		public float m_BottomMargin = 10f;

		public float m_RightMargin = 10f;

		public float m_LeftMargin = 10f;

		public bool allowDragging
		{
			get;
			set;
		}

		public bool allowRenaming
		{
			get;
			set;
		}

		public bool allowMultiSelect
		{
			get;
			set;
		}

		public bool allowDeselection
		{
			get;
			set;
		}

		public bool allowFocusRendering
		{
			get;
			set;
		}

		public bool allowBuiltinResources
		{
			get;
			set;
		}

		public bool allowUserRenderingHook
		{
			get;
			set;
		}

		public bool allowFindNextShortcut
		{
			get;
			set;
		}

		public bool foldersFirst
		{
			get;
			set;
		}

		public Action repaintCallback
		{
			get
			{
				return this.m_RepaintWantedCallback;
			}
			set
			{
				this.m_RepaintWantedCallback = value;
			}
		}

		public Action<bool> itemSelectedCallback
		{
			get
			{
				return this.m_ItemSelectedCallback;
			}
			set
			{
				this.m_ItemSelectedCallback = value;
			}
		}

		public Action keyboardCallback
		{
			get
			{
				return this.m_KeyboardInputCallback;
			}
			set
			{
				this.m_KeyboardInputCallback = value;
			}
		}

		public Action gotKeyboardFocus
		{
			get
			{
				return this.m_GotKeyboardFocus;
			}
			set
			{
				this.m_GotKeyboardFocus = value;
			}
		}

		public Action assetStoreSearchEnded
		{
			get
			{
				return this.m_AssetStoreSearchEnded;
			}
			set
			{
				this.m_AssetStoreSearchEnded = value;
			}
		}

		public Func<Rect, float> drawLocalAssetHeader
		{
			get
			{
				return this.m_DrawLocalAssetHeader;
			}
			set
			{
				this.m_DrawLocalAssetHeader = value;
			}
		}

		public int gridSize
		{
			get
			{
				return this.m_State.m_GridSize;
			}
			set
			{
				if (this.m_State.m_GridSize != value)
				{
					this.m_State.m_GridSize = value;
					this.m_FrameLastClickedItem = true;
				}
			}
		}

		public int minGridSize
		{
			get
			{
				return this.m_MinGridSize;
			}
		}

		public int maxGridSize
		{
			get
			{
				return this.m_MaxGridSize;
			}
		}

		public int numItemsDisplayed
		{
			get
			{
				return this.m_LocalAssets.ItemCount;
			}
		}

		public ObjectListArea(ObjectListAreaState state, EditorWindow owner, bool showNoneItem)
		{
			this.m_State = state;
			this.m_Owner = owner;
			AssetStorePreviewManager.MaxCachedImages = 72;
			this.m_StoreAssets = new List<ObjectListArea.AssetStoreGroup>();
			this.m_RequeryAssetStore = false;
			this.m_LocalAssets = new ObjectListArea.LocalGroup(this, "", showNoneItem);
			this.m_Groups = new List<ObjectListArea.Group>();
			this.m_Groups.Add(this.m_LocalAssets);
		}

		public void ShowObjectsInList(int[] instanceIDs)
		{
			this.Init(this.m_TotalRect, HierarchyType.Assets, new SearchFilter(), false);
			this.m_LocalAssets.ShowObjectsInList(instanceIDs);
		}

		public void Init(Rect rect, HierarchyType hierarchyType, SearchFilter searchFilter, bool checkThumbnails)
		{
			this.m_VisibleRect = rect;
			this.m_TotalRect = rect;
			this.m_LocalAssets.UpdateFilter(hierarchyType, searchFilter, this.foldersFirst);
			this.m_LocalAssets.UpdateAssets();
			foreach (ObjectListArea.AssetStoreGroup current in this.m_StoreAssets)
			{
				current.UpdateFilter(hierarchyType, searchFilter, this.foldersFirst);
			}
			bool flag = searchFilter.GetState() == SearchFilter.State.FolderBrowsing;
			if (flag)
			{
				this.m_LastAssetStoreQuerySearchFilter = "";
				this.m_LastAssetStoreQueryClassName = new string[0];
				this.m_LastAssetStoreQueryLabels = new string[0];
			}
			else
			{
				this.m_LastAssetStoreQuerySearchFilter = ((searchFilter.nameFilter != null) ? searchFilter.nameFilter : "");
				bool flag2 = searchFilter.classNames == null || Array.IndexOf<string>(searchFilter.classNames, "Object") >= 0;
				this.m_LastAssetStoreQueryClassName = ((!flag2) ? searchFilter.classNames : new string[0]);
				this.m_LastAssetStoreQueryLabels = ((searchFilter.assetLabels != null) ? searchFilter.assetLabels : new string[0]);
			}
			this.m_LastAssetStoreQueryChangeTime = EditorApplication.timeSinceStartup;
			this.m_RequeryAssetStore = true;
			this.m_ShowLocalAssetsOnly = (flag || searchFilter.GetState() != SearchFilter.State.SearchingInAssetStore);
			this.m_AssetStoreError = "";
			if (checkThumbnails)
			{
				this.m_AllowThumbnails = this.ObjectsHaveThumbnails(hierarchyType, searchFilter);
			}
			else
			{
				this.m_AllowThumbnails = true;
			}
			this.Repaint();
			this.ClearCroppedLabelCache();
			this.SetupData(true);
		}

		private bool HasFocus()
		{
			return !this.allowFocusRendering || (this.m_KeyboardControlID == GUIUtility.keyboardControl && this.m_Owner.m_Parent.hasFocus);
		}

		private void QueryAssetStore()
		{
			bool requeryAssetStore = this.m_RequeryAssetStore;
			this.m_RequeryAssetStore = false;
			if (!this.m_ShowLocalAssetsOnly || this.ShowAssetStoreHitsWhileSearchingLocalAssets())
			{
				bool flag = this.m_LastAssetStoreQuerySearchFilter != "" || this.m_LastAssetStoreQueryClassName.Length != 0 || this.m_LastAssetStoreQueryLabels.Length != 0;
				if (!this.m_QueryInProgress)
				{
					if (!flag)
					{
						this.ClearAssetStoreGroups();
					}
					else if (this.m_LastAssetStoreQueryChangeTime + 0.2 > EditorApplication.timeSinceStartup)
					{
						this.m_RequeryAssetStore = true;
						this.Repaint();
					}
					else
					{
						this.m_QueryInProgress = true;
						string queryFilter = this.m_LastAssetStoreQuerySearchFilter + this.m_LastAssetStoreQueryClassName + this.m_LastAssetStoreQueryLabels;
						AssetStoreResultBase<AssetStoreSearchResults>.Callback callback = delegate(AssetStoreSearchResults results)
						{
							this.m_QueryInProgress = false;
							if (queryFilter != this.m_LastAssetStoreQuerySearchFilter + this.m_LastAssetStoreQueryClassName + this.m_LastAssetStoreQueryLabels)
							{
								this.m_RequeryAssetStore = true;
							}
							if (results.error != null && results.error != "")
							{
								if (ObjectListArea.s_Debug)
								{
									Debug.LogError("Error performing Asset Store search: " + results.error);
								}
								else
								{
									Console.Write("Error performing Asset Store search: " + results.error);
								}
								this.m_AssetStoreError = results.error;
								this.m_Groups.Clear();
								this.m_Groups.Add(this.m_LocalAssets);
								this.Repaint();
								if (this.assetStoreSearchEnded != null)
								{
									this.assetStoreSearchEnded();
								}
							}
							else
							{
								this.m_AssetStoreError = "";
								List<string> list2 = new List<string>();
								foreach (ObjectListArea.AssetStoreGroup current2 in this.m_StoreAssets)
								{
									list2.Add(current2.Name);
								}
								this.m_Groups.Clear();
								this.m_Groups.Add(this.m_LocalAssets);
								using (List<AssetStoreSearchResults.Group>.Enumerator enumerator3 = results.groups.GetEnumerator())
								{
									while (enumerator3.MoveNext())
									{
										AssetStoreSearchResults.Group inGroup = enumerator3.Current;
										list2.Remove(inGroup.name);
										ObjectListArea.AssetStoreGroup assetStoreGroup = this.m_StoreAssets.Find((ObjectListArea.AssetStoreGroup g) => g.Name == inGroup.name);
										if (assetStoreGroup == null)
										{
											assetStoreGroup = new ObjectListArea.AssetStoreGroup(this, inGroup.label, inGroup.name);
											this.m_StoreAssets.Add(assetStoreGroup);
										}
										this.m_Groups.Add(assetStoreGroup);
										if (inGroup.limit != 0)
										{
											assetStoreGroup.ItemsAvailable = inGroup.totalFound;
										}
										if (inGroup.offset == 0 && inGroup.limit != 0)
										{
											assetStoreGroup.Assets = inGroup.assets;
										}
										else
										{
											assetStoreGroup.Assets.AddRange(inGroup.assets);
										}
									}
								}
								using (List<string>.Enumerator enumerator4 = list2.GetEnumerator())
								{
									while (enumerator4.MoveNext())
									{
										string k = enumerator4.Current;
										this.m_StoreAssets.RemoveAll((ObjectListArea.AssetStoreGroup g) => g.Name == k);
									}
								}
								this.EnsureAssetStoreGroupsAreOpenIfAllClosed();
								this.Repaint();
								if (this.assetStoreSearchEnded != null)
								{
									this.assetStoreSearchEnded();
								}
							}
						};
						List<AssetStoreClient.SearchCount> list = new List<AssetStoreClient.SearchCount>();
						if (!requeryAssetStore)
						{
							foreach (ObjectListArea.AssetStoreGroup current in this.m_StoreAssets)
							{
								AssetStoreClient.SearchCount item = default(AssetStoreClient.SearchCount);
								if (current.Visible && current.NeedItems)
								{
									item.offset = current.Assets.Count;
									item.limit = current.ItemsWantedShown - item.offset;
								}
								item.name = current.Name;
								list.Add(item);
							}
						}
						AssetStoreClient.SearchAssets(this.m_LastAssetStoreQuerySearchFilter, this.m_LastAssetStoreQueryClassName, this.m_LastAssetStoreQueryLabels, list, callback);
					}
				}
			}
		}

		private void EnsureAssetStoreGroupsAreOpenIfAllClosed()
		{
			if (this.m_StoreAssets.Count > 0)
			{
				int num = 0;
				foreach (ObjectListArea.AssetStoreGroup current in this.m_StoreAssets)
				{
					if (current.Visible)
					{
						num++;
					}
				}
				if (num == 0)
				{
					foreach (ObjectListArea.AssetStoreGroup current2 in this.m_StoreAssets)
					{
						ObjectListArea.Group arg_8C_0 = current2;
						bool flag = true;
						current2.visiblePreference = flag;
						arg_8C_0.Visible = flag;
					}
				}
			}
		}

		private void RequeryAssetStore()
		{
			this.m_RequeryAssetStore = true;
		}

		private void ClearAssetStoreGroups()
		{
			this.m_Groups.Clear();
			this.m_Groups.Add(this.m_LocalAssets);
			this.m_StoreAssets.Clear();
			this.Repaint();
		}

		public string GetAssetStoreButtonText()
		{
			string text = "Asset Store";
			if (this.ShowAssetStoreHitsWhileSearchingLocalAssets())
			{
				for (int i = 0; i < this.m_StoreAssets.Count; i++)
				{
					if (i == 0)
					{
						text += ": ";
					}
					else
					{
						text += " ∕ ";
					}
					ObjectListArea.AssetStoreGroup assetStoreGroup = this.m_StoreAssets[i];
					text += ((assetStoreGroup.ItemsAvailable <= 999) ? assetStoreGroup.ItemsAvailable.ToString() : "999+");
				}
			}
			return text;
		}

		private bool ShowAssetStoreHitsWhileSearchingLocalAssets()
		{
			return EditorPrefs.GetBool("ShowAssetStoreSearchHits", true);
		}

		public void ShowAssetStoreHitCountWhileSearchingLocalAssetsChanged()
		{
			if (this.ShowAssetStoreHitsWhileSearchingLocalAssets())
			{
				this.RequeryAssetStore();
			}
			else if (this.m_ShowLocalAssetsOnly)
			{
				this.ClearAssetStoreGroups();
			}
			this.Repaint();
		}

		internal float GetVisibleWidth()
		{
			return this.m_VisibleRect.width;
		}

		public void OnGUI(Rect position, int keyboardControlID)
		{
			if (ObjectListArea.s_Styles == null)
			{
				ObjectListArea.s_Styles = new ObjectListArea.Styles();
			}
			ObjectListArea.s_VCEnabled = Provider.isActive;
			Event current = Event.current;
			this.m_TotalRect = position;
			this.FrameLastClickedItemIfWanted();
			GUI.Label(this.m_TotalRect, GUIContent.none, ObjectListArea.s_Styles.iconAreaBg);
			this.m_KeyboardControlID = keyboardControlID;
			if (current.type == EventType.MouseDown && position.Contains(Event.current.mousePosition))
			{
				GUIUtility.keyboardControl = this.m_KeyboardControlID;
				this.m_AllowRenameOnMouseUp = true;
				this.Repaint();
			}
			bool flag = this.m_KeyboardControlID == GUIUtility.keyboardControl;
			if (flag != this.m_State.m_HadKeyboardFocusLastEvent)
			{
				this.m_State.m_HadKeyboardFocusLastEvent = flag;
				if (flag)
				{
					if (current.type == EventType.MouseDown)
					{
						this.m_AllowRenameOnMouseUp = false;
					}
					if (this.m_GotKeyboardFocus != null)
					{
						this.m_GotKeyboardFocus();
					}
				}
			}
			if (current.keyCode == KeyCode.Tab && current.type == EventType.KeyDown && !flag && !this.IsShowingAny(this.GetSelection()))
			{
				int activeInstanceID;
				if (this.m_LocalAssets.InstanceIdAtIndex(0, out activeInstanceID))
				{
					Selection.activeInstanceID = activeInstanceID;
				}
			}
			this.HandleKeyboard(true);
			this.HandleZoomScrolling();
			this.HandleListArea();
			this.DoOffsetSelection();
			this.HandleUnusedEvents();
		}

		private void FrameLastClickedItemIfWanted()
		{
			if (this.m_FrameLastClickedItem && Event.current.type == EventType.Repaint)
			{
				this.m_FrameLastClickedItem = false;
				double num = EditorApplication.timeSinceStartup - this.m_LocalAssets.m_LastClickedDrawTime;
				if (this.m_State.m_SelectedInstanceIDs.Count > 0 && num < 0.2)
				{
					this.Frame(this.m_State.m_LastClickedInstanceID, true, false);
				}
			}
		}

		private void HandleUnusedEvents()
		{
			if (this.allowDeselection && Event.current.type == EventType.MouseDown && Event.current.button == 0 && this.m_TotalRect.Contains(Event.current.mousePosition))
			{
				this.SetSelection(new int[0], false);
			}
		}

		public bool CanShowThumbnails()
		{
			return this.m_AllowThumbnails;
		}

		private static string CreateFilterString(string searchString, string requiredClassName)
		{
			string text = searchString;
			if (!string.IsNullOrEmpty(requiredClassName))
			{
				text = text + " t:" + requiredClassName;
			}
			return text;
		}

		private bool ObjectsHaveThumbnails(HierarchyType type, SearchFilter searchFilter)
		{
			bool result;
			if (this.m_LocalAssets.HasBuiltinResources)
			{
				result = true;
			}
			else
			{
				IHierarchyProperty hierarchyProperty = FilteredHierarchyProperty.CreateHierarchyPropertyForFilter(new FilteredHierarchy(type)
				{
					searchFilter = searchFilter
				});
				int[] expanded = new int[0];
				if (hierarchyProperty.CountRemaining(expanded) == 0)
				{
					result = true;
				}
				else
				{
					hierarchyProperty.Reset();
					while (hierarchyProperty.Next(expanded))
					{
						if (hierarchyProperty.hasFullPreviewImage)
						{
							result = true;
							return result;
						}
					}
					result = false;
				}
			}
			return result;
		}

		internal void OnDestroy()
		{
			AssetPreview.DeletePreviewTextureManagerByID(this.GetAssetPreviewManagerID());
		}

		private void Repaint()
		{
			if (this.m_RepaintWantedCallback != null)
			{
				this.m_RepaintWantedCallback();
			}
		}

		public void OnEvent()
		{
			this.GetRenameOverlay().OnEvent();
		}

		private CreateAssetUtility GetCreateAssetUtility()
		{
			return this.m_State.m_CreateAssetUtility;
		}

		private RenameOverlay GetRenameOverlay()
		{
			return this.m_State.m_RenameOverlay;
		}

		internal void BeginNamingNewAsset(string newAssetName, int instanceID, bool isCreatingNewFolder)
		{
			this.m_State.m_NewAssetIndexInList = this.m_LocalAssets.IndexOfNewText(newAssetName, isCreatingNewFolder, this.foldersFirst);
			if (this.m_State.m_NewAssetIndexInList != -1)
			{
				this.Frame(instanceID, true, false);
				this.GetRenameOverlay().BeginRename(newAssetName, instanceID, 0f);
			}
			else
			{
				Debug.LogError("Failed to insert new asset into list");
			}
			this.Repaint();
		}

		public bool BeginRename(float delay)
		{
			bool result;
			if (!this.allowRenaming)
			{
				result = false;
			}
			else if (this.m_State.m_SelectedInstanceIDs.Count != 1)
			{
				result = false;
			}
			else
			{
				int num = this.m_State.m_SelectedInstanceIDs[0];
				if (AssetDatabase.IsSubAsset(num))
				{
					result = false;
				}
				else if (this.m_LocalAssets.IsBuiltinAsset(num))
				{
					result = false;
				}
				else if (!AssetDatabase.Contains(num))
				{
					result = false;
				}
				else
				{
					string nameOfLocalAsset = this.m_LocalAssets.GetNameOfLocalAsset(num);
					result = (nameOfLocalAsset != null && this.GetRenameOverlay().BeginRename(nameOfLocalAsset, num, delay));
				}
			}
			return result;
		}

		public void EndRename(bool acceptChanges)
		{
			if (this.GetRenameOverlay().IsRenaming())
			{
				this.GetRenameOverlay().EndRename(acceptChanges);
				this.RenameEnded();
			}
		}

		private void RenameEnded()
		{
			string name = (!string.IsNullOrEmpty(this.GetRenameOverlay().name)) ? this.GetRenameOverlay().name : this.GetRenameOverlay().originalName;
			int userData = this.GetRenameOverlay().userData;
			if (this.GetCreateAssetUtility().IsCreatingNewAsset())
			{
				if (this.GetRenameOverlay().userAcceptedRename)
				{
					this.GetCreateAssetUtility().EndNewAssetCreation(name);
				}
			}
			else if (this.GetRenameOverlay().userAcceptedRename)
			{
				ObjectNames.SetNameSmartWithInstanceID(userData, name);
			}
			if (this.GetRenameOverlay().HasKeyboardFocus())
			{
				GUIUtility.keyboardControl = this.m_KeyboardControlID;
			}
			if (this.GetRenameOverlay().userAcceptedRename)
			{
				this.Frame(userData, true, false);
			}
			this.ClearRenameState();
		}

		private void ClearRenameState()
		{
			this.GetRenameOverlay().Clear();
			this.GetCreateAssetUtility().Clear();
			this.m_State.m_NewAssetIndexInList = -1;
		}

		internal void HandleRenameOverlay()
		{
			if (this.GetRenameOverlay().IsRenaming())
			{
				GUIStyle textFieldStyle = (!this.IsListMode()) ? ObjectListArea.s_Styles.miniRenameField : null;
				if (!this.GetRenameOverlay().OnGUI(textFieldStyle))
				{
					this.RenameEnded();
					GUIUtility.ExitGUI();
				}
			}
		}

		public bool IsSelected(int instanceID)
		{
			return this.m_State.m_SelectedInstanceIDs.Contains(instanceID);
		}

		public int[] GetSelection()
		{
			return this.m_State.m_SelectedInstanceIDs.ToArray();
		}

		public bool IsLastClickedItemVisible()
		{
			return this.GetSelectedAssetIdx() >= 0;
		}

		public void SelectAll()
		{
			List<int> instanceIDs = this.m_LocalAssets.GetInstanceIDs();
			this.SetSelection(instanceIDs.ToArray(), false);
		}

		private void SetSelection(int[] selectedInstanceIDs, bool doubleClicked)
		{
			this.InitSelection(selectedInstanceIDs);
			if (this.m_ItemSelectedCallback != null)
			{
				this.Repaint();
				this.m_ItemSelectedCallback(doubleClicked);
			}
		}

		public void InitSelection(int[] selectedInstanceIDs)
		{
			this.m_State.m_SelectedInstanceIDs = new List<int>(selectedInstanceIDs);
			if (this.m_State.m_SelectedInstanceIDs.Count > 0)
			{
				if (!this.m_State.m_SelectedInstanceIDs.Contains(this.m_State.m_LastClickedInstanceID))
				{
					this.m_State.m_LastClickedInstanceID = this.m_State.m_SelectedInstanceIDs[this.m_State.m_SelectedInstanceIDs.Count - 1];
				}
			}
			else
			{
				this.m_State.m_LastClickedInstanceID = 0;
			}
			if (Selection.activeObject == null || Selection.activeObject.GetType() != typeof(AssetStoreAssetInspector))
			{
				this.selectedAssetStoreAsset = false;
				AssetStoreAssetSelection.Clear();
			}
		}

		private void SetSelection(AssetStoreAsset assetStoreResult, bool doubleClicked)
		{
			this.m_State.m_SelectedInstanceIDs.Clear();
			this.selectedAssetStoreAsset = true;
			AssetStoreAssetSelection.Clear();
			AssetStorePreviewManager.CachedAssetStoreImage cachedAssetStoreImage = AssetStorePreviewManager.TextureFromUrl(assetStoreResult.staticPreviewURL, assetStoreResult.name, this.gridSize, ObjectListArea.s_Styles.resultsGridLabel, ObjectListArea.s_Styles.resultsGrid, true);
			Texture2D image = cachedAssetStoreImage.image;
			AssetStoreAssetSelection.AddAsset(assetStoreResult, image);
			if (this.m_ItemSelectedCallback != null)
			{
				this.Repaint();
				this.m_ItemSelectedCallback(doubleClicked);
			}
		}

		private void HandleZoomScrolling()
		{
			if (EditorGUI.actionKey && Event.current.type == EventType.ScrollWheel && this.m_TotalRect.Contains(Event.current.mousePosition))
			{
				int num = (Event.current.delta.y <= 0f) ? 1 : -1;
				this.gridSize = Mathf.Clamp(this.gridSize + num * 7, this.minGridSize, this.maxGridSize);
				if (num < 0 && this.gridSize < this.m_MinIconSize)
				{
					this.gridSize = this.m_MinGridSize;
				}
				if (num > 0 && this.gridSize < this.m_MinIconSize)
				{
					this.gridSize = this.m_MinIconSize;
				}
				Event.current.Use();
				GUI.changed = true;
			}
		}

		private bool IsPreviewIconExpansionModifierPressed()
		{
			return Event.current.alt;
		}

		private bool AllowLeftRightArrowNavigation()
		{
			bool flag = !this.m_LocalAssets.ListMode && !this.IsPreviewIconExpansionModifierPressed();
			bool flag2 = !this.m_ShowLocalAssetsOnly || this.m_LocalAssets.ItemCount > 1;
			return flag && flag2;
		}

		public void HandleKeyboard(bool checkKeyboardControl)
		{
			if ((!checkKeyboardControl || GUIUtility.keyboardControl == this.m_KeyboardControlID) && GUI.enabled)
			{
				if (this.m_KeyboardInputCallback != null)
				{
					this.m_KeyboardInputCallback();
				}
				if (Event.current.type == EventType.KeyDown)
				{
					int num = 0;
					if (this.IsLastClickedItemVisible())
					{
						switch (Event.current.keyCode)
						{
						case KeyCode.UpArrow:
							num = -this.m_LocalAssets.m_Grid.columns;
							break;
						case KeyCode.DownArrow:
							num = this.m_LocalAssets.m_Grid.columns;
							break;
						case KeyCode.RightArrow:
							if (this.AllowLeftRightArrowNavigation())
							{
								num = 1;
							}
							break;
						case KeyCode.LeftArrow:
							if (this.AllowLeftRightArrowNavigation())
							{
								num = -1;
							}
							break;
						case KeyCode.Home:
							num = -2147483648;
							break;
						case KeyCode.End:
							num = 2147483647;
							break;
						case KeyCode.PageUp:
							num = -2147483647;
							break;
						case KeyCode.PageDown:
							num = 2147483646;
							break;
						}
					}
					else
					{
						bool flag = false;
						switch (Event.current.keyCode)
						{
						case KeyCode.UpArrow:
						case KeyCode.DownArrow:
						case KeyCode.Home:
						case KeyCode.End:
						case KeyCode.PageUp:
						case KeyCode.PageDown:
							flag = true;
							break;
						case KeyCode.RightArrow:
						case KeyCode.LeftArrow:
							flag = this.AllowLeftRightArrowNavigation();
							break;
						}
						if (flag)
						{
							this.SelectFirst();
							Event.current.Use();
						}
					}
					if (num != 0)
					{
						if (this.GetSelectedAssetIdx() < 0 && !this.m_LocalAssets.ShowNone)
						{
							this.SetSelectedAssetByIdx(0);
						}
						else
						{
							this.m_SelectionOffset = num;
						}
						Event.current.Use();
						GUI.changed = true;
					}
					else if (this.allowFindNextShortcut && this.m_LocalAssets.DoCharacterOffsetSelection())
					{
						Event.current.Use();
					}
				}
			}
		}

		private void DoOffsetSelectionSpecialKeys(int idx, int maxIndex)
		{
			float num = this.m_LocalAssets.m_Grid.itemSize.y + this.m_LocalAssets.m_Grid.verticalSpacing;
			int columns = this.m_LocalAssets.m_Grid.columns;
			int selectionOffset = this.m_SelectionOffset;
			if (selectionOffset != 2147483646)
			{
				if (selectionOffset != 2147483647)
				{
					if (selectionOffset != -2147483648)
					{
						if (selectionOffset == -2147483647)
						{
							if (Application.platform == RuntimePlatform.OSXEditor)
							{
								ObjectListAreaState expr_8A_cp_0 = this.m_State;
								expr_8A_cp_0.m_ScrollPosition.y = expr_8A_cp_0.m_ScrollPosition.y - this.m_TotalRect.height;
								this.m_SelectionOffset = 0;
							}
							else
							{
								this.m_SelectionOffset = -Mathf.RoundToInt(this.m_TotalRect.height / num) * columns;
								this.m_SelectionOffset = Mathf.Max(-Mathf.FloorToInt((float)idx / (float)columns) * columns, this.m_SelectionOffset);
							}
						}
					}
					else
					{
						this.m_SelectionOffset = 0;
						this.SetSelectedAssetByIdx(0);
					}
				}
				else
				{
					this.m_SelectionOffset = maxIndex - idx;
				}
			}
			else if (Application.platform == RuntimePlatform.OSXEditor)
			{
				ObjectListAreaState expr_103_cp_0 = this.m_State;
				expr_103_cp_0.m_ScrollPosition.y = expr_103_cp_0.m_ScrollPosition.y + this.m_TotalRect.height;
				this.m_SelectionOffset = 0;
			}
			else
			{
				this.m_SelectionOffset = Mathf.RoundToInt(this.m_TotalRect.height / num) * columns;
				int num2 = maxIndex - idx;
				this.m_SelectionOffset = Mathf.Min(Mathf.FloorToInt((float)num2 / (float)columns) * columns, this.m_SelectionOffset);
			}
		}

		private void DoOffsetSelection()
		{
			if (this.m_SelectionOffset != 0)
			{
				int maxIdx = this.GetMaxIdx();
				if (this.maxGridSize != -1)
				{
					int num = this.GetSelectedAssetIdx();
					num = ((num >= 0) ? num : 0);
					this.DoOffsetSelectionSpecialKeys(num, maxIdx);
					if (this.m_SelectionOffset != 0)
					{
						int num2 = num + this.m_SelectionOffset;
						this.m_SelectionOffset = 0;
						if (num2 < 0)
						{
							num2 = num;
						}
						else
						{
							num2 = Mathf.Min(num2, maxIdx);
						}
						int selectedAssetByIdx = num2;
						this.SetSelectedAssetByIdx(selectedAssetByIdx);
					}
				}
			}
		}

		public void OffsetSelection(int selectionOffset)
		{
			this.m_SelectionOffset = selectionOffset;
		}

		public void SelectFirst()
		{
			int selectedAssetByIdx = 0;
			if (this.m_ShowLocalAssetsOnly && this.m_LocalAssets.ShowNone && this.m_LocalAssets.ItemCount > 1)
			{
				selectedAssetByIdx = 1;
			}
			this.SetSelectedAssetByIdx(selectedAssetByIdx);
		}

		public int GetInstanceIDByIndex(int index)
		{
			int num;
			int result;
			if (this.m_LocalAssets.InstanceIdAtIndex(index, out num))
			{
				result = num;
			}
			else
			{
				result = 0;
			}
			return result;
		}

		private void SetSelectedAssetByIdx(int selectedIdx)
		{
			int num;
			if (this.m_LocalAssets.InstanceIdAtIndex(selectedIdx, out num))
			{
				Rect r = this.m_LocalAssets.m_Grid.CalcRect(selectedIdx, 0f);
				this.ScrollToPosition(ObjectListArea.AdjustRectForFraming(r));
				this.Repaint();
				int[] selectedInstanceIDs;
				if (this.IsLocalAssetsCurrentlySelected())
				{
					selectedInstanceIDs = this.m_LocalAssets.GetNewSelection(num, false, true).ToArray();
				}
				else
				{
					selectedInstanceIDs = new int[]
					{
						num
					};
				}
				this.SetSelection(selectedInstanceIDs, false);
				this.m_State.m_LastClickedInstanceID = num;
			}
			else
			{
				selectedIdx -= this.m_LocalAssets.m_Grid.rows * this.m_LocalAssets.m_Grid.columns;
				float num2 = this.m_LocalAssets.Height;
				foreach (ObjectListArea.AssetStoreGroup current in this.m_StoreAssets)
				{
					if (!current.Visible)
					{
						num2 += current.Height;
					}
					else
					{
						AssetStoreAsset assetStoreAsset = current.AssetAtIndex(selectedIdx);
						if (assetStoreAsset != null)
						{
							Rect r2 = current.m_Grid.CalcRect(selectedIdx, num2);
							this.ScrollToPosition(ObjectListArea.AdjustRectForFraming(r2));
							this.Repaint();
							this.SetSelection(assetStoreAsset, false);
							break;
						}
						selectedIdx -= current.m_Grid.rows * current.m_Grid.columns;
						num2 += current.Height;
					}
				}
			}
		}

		private void Reveal(int instanceID)
		{
			if (AssetDatabase.Contains(instanceID))
			{
				int mainAssetInstanceID = AssetDatabase.GetMainAssetInstanceID(AssetDatabase.GetAssetPath(instanceID));
				bool flag = mainAssetInstanceID != instanceID;
				if (flag)
				{
					this.m_LocalAssets.ChangeExpandedState(mainAssetInstanceID, true);
				}
			}
		}

		public bool Frame(int instanceID, bool frame, bool ping)
		{
			if (ObjectListArea.s_Styles == null)
			{
				ObjectListArea.s_Styles = new ObjectListArea.Styles();
			}
			int num = -1;
			if (this.GetCreateAssetUtility().IsCreatingNewAsset() && this.m_State.m_NewAssetIndexInList != -1 && this.GetCreateAssetUtility().instanceID == instanceID)
			{
				num = this.m_State.m_NewAssetIndexInList;
			}
			if (frame)
			{
				this.Reveal(instanceID);
			}
			if (num == -1)
			{
				num = this.m_LocalAssets.IndexOf(instanceID);
			}
			bool result;
			if (num != -1)
			{
				if (frame)
				{
					float yOffset = 0f;
					Rect r = this.m_LocalAssets.m_Grid.CalcRect(num, yOffset);
					this.CenterRect(ObjectListArea.AdjustRectForFraming(r));
					this.Repaint();
				}
				if (ping)
				{
					this.BeginPing(instanceID);
				}
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		private int GetSelectedAssetIdx()
		{
			int num = this.m_LocalAssets.IndexOf(this.m_State.m_LastClickedInstanceID);
			int result;
			if (num != -1)
			{
				result = num;
			}
			else
			{
				num = this.m_LocalAssets.m_Grid.rows * this.m_LocalAssets.m_Grid.columns;
				if (AssetStoreAssetSelection.Count == 0)
				{
					result = -1;
				}
				else
				{
					AssetStoreAsset firstAsset = AssetStoreAssetSelection.GetFirstAsset();
					if (firstAsset == null)
					{
						result = -1;
					}
					else
					{
						int id = firstAsset.id;
						foreach (ObjectListArea.AssetStoreGroup current in this.m_StoreAssets)
						{
							if (current.Visible)
							{
								int num2 = current.IndexOf(id);
								if (num2 != -1)
								{
									result = num + num2;
									return result;
								}
								num += current.m_Grid.rows * current.m_Grid.columns;
							}
						}
						result = -1;
					}
				}
			}
			return result;
		}

		private bool SkipGroup(ObjectListArea.Group group)
		{
			bool result;
			if (this.m_ShowLocalAssetsOnly)
			{
				if (group is ObjectListArea.AssetStoreGroup)
				{
					result = true;
					return result;
				}
			}
			else if (group is ObjectListArea.LocalGroup)
			{
				result = true;
				return result;
			}
			result = false;
			return result;
		}

		private int GetMaxIdx()
		{
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			foreach (ObjectListArea.Group current in this.m_Groups)
			{
				if (!this.SkipGroup(current))
				{
					if (current.Visible)
					{
						num2 += num3;
						num3 = current.m_Grid.rows * current.m_Grid.columns;
						num = current.ItemCount - 1;
					}
				}
			}
			int num4 = num2 + num;
			return (num3 + num4 != 0) ? num4 : -1;
		}

		private bool IsLocalAssetsCurrentlySelected()
		{
			int num = this.m_State.m_SelectedInstanceIDs.FirstOrDefault<int>();
			bool result;
			if (num != 0)
			{
				int num2 = this.m_LocalAssets.IndexOf(num);
				result = (num2 != -1);
			}
			else
			{
				result = false;
			}
			return result;
		}

		private void SetupData(bool forceReflow)
		{
			foreach (ObjectListArea.Group current in this.m_Groups)
			{
				if (!this.SkipGroup(current))
				{
					current.UpdateAssets();
				}
			}
			if (forceReflow || Event.current.type == EventType.Repaint)
			{
				this.Reflow();
			}
		}

		private bool IsObjectSelector()
		{
			return this.m_LocalAssets.ShowNone;
		}

		private void HandleListArea()
		{
			this.SetupData(false);
			if (!this.IsObjectSelector() && !this.m_QueryInProgress)
			{
				if (this.m_StoreAssets.Exists((ObjectListArea.AssetStoreGroup g) => g.NeedItems) || this.m_RequeryAssetStore)
				{
					this.QueryAssetStore();
				}
			}
			float num = 0f;
			foreach (ObjectListArea.Group current in this.m_Groups)
			{
				if (!this.SkipGroup(current))
				{
					num += current.Height;
					if (this.m_LocalAssets.ShowNone)
					{
						break;
					}
				}
			}
			Rect totalRect = this.m_TotalRect;
			Rect viewRect = new Rect(0f, 0f, 1f, num);
			bool flag = num > this.m_TotalRect.height;
			this.m_VisibleRect = this.m_TotalRect;
			if (flag)
			{
				this.m_VisibleRect.width = this.m_VisibleRect.width - 16f;
			}
			double timeSinceStartup = EditorApplication.timeSinceStartup;
			this.m_LastScrollPosition = this.m_State.m_ScrollPosition;
			bool flag2 = false;
			this.m_State.m_ScrollPosition = GUI.BeginScrollView(totalRect, this.m_State.m_ScrollPosition, viewRect);
			Vector2 scrollPosition = this.m_State.m_ScrollPosition;
			if (this.m_LastScrollPosition != this.m_State.m_ScrollPosition)
			{
				this.LastScrollTime = timeSinceStartup;
			}
			float num2 = 0f;
			foreach (ObjectListArea.Group current2 in this.m_Groups)
			{
				if (!this.SkipGroup(current2))
				{
					current2.Draw(num2, scrollPosition);
					flag2 = (flag2 || current2.NeedsRepaint);
					num2 += current2.Height;
					if (this.m_LocalAssets.ShowNone)
					{
						break;
					}
				}
			}
			this.HandlePing();
			if (flag2)
			{
				this.Repaint();
			}
			GUI.EndScrollView();
			if (this.m_ResizePreviewCacheTo > 0 && AssetStorePreviewManager.MaxCachedImages != this.m_ResizePreviewCacheTo)
			{
				AssetStorePreviewManager.MaxCachedImages = this.m_ResizePreviewCacheTo;
			}
			if (Event.current.type == EventType.Repaint)
			{
				AssetStorePreviewManager.AbortOlderThan(timeSinceStartup);
			}
			if (!this.m_ShowLocalAssetsOnly && !string.IsNullOrEmpty(this.m_AssetStoreError))
			{
				Vector2 vector = EditorStyles.label.CalcSize(ObjectListArea.s_Styles.m_AssetStoreNotAvailableText);
				Rect position = new Rect(this.m_TotalRect.x + 2f + Mathf.Max(0f, (this.m_TotalRect.width - vector.x) * 0.5f), this.m_TotalRect.y + 10f, vector.x, 20f);
				using (new EditorGUI.DisabledScope(true))
				{
					GUI.Label(position, ObjectListArea.s_Styles.m_AssetStoreNotAvailableText, EditorStyles.label);
				}
			}
		}

		private bool IsListMode()
		{
			bool result;
			if (this.allowMultiSelect)
			{
				result = (this.gridSize == 16);
			}
			else
			{
				result = (this.gridSize == 16 || !this.CanShowThumbnails());
			}
			return result;
		}

		private void Reflow()
		{
			if (this.gridSize < 20)
			{
				this.gridSize = this.m_MinGridSize;
			}
			else if (this.gridSize < this.m_MinIconSize)
			{
				this.gridSize = this.m_MinIconSize;
			}
			if (this.IsListMode())
			{
				foreach (ObjectListArea.Group current in this.m_Groups)
				{
					if (!this.SkipGroup(current))
					{
						current.ListMode = true;
						this.UpdateGroupSizes(current);
						if (this.m_LocalAssets.ShowNone)
						{
							break;
						}
					}
				}
				this.m_ResizePreviewCacheTo = Mathf.CeilToInt(this.m_TotalRect.height / 16f) + 10;
			}
			else
			{
				float num = 0f;
				foreach (ObjectListArea.Group current2 in this.m_Groups)
				{
					if (!this.SkipGroup(current2))
					{
						current2.ListMode = false;
						this.UpdateGroupSizes(current2);
						num += current2.Height;
						if (this.m_LocalAssets.ShowNone)
						{
							break;
						}
					}
				}
				bool flag = this.m_TotalRect.height < num;
				if (flag)
				{
					foreach (ObjectListArea.Group current3 in this.m_Groups)
					{
						if (!this.SkipGroup(current3))
						{
							current3.m_Grid.fixedWidth = this.m_TotalRect.width - 16f;
							current3.m_Grid.InitNumRowsAndColumns(current3.ItemCount, current3.m_Grid.CalcRows(current3.ItemsWantedShown));
							current3.UpdateHeight();
							if (this.m_LocalAssets.ShowNone)
							{
								break;
							}
						}
					}
				}
				int maxNumVisibleItems = this.GetMaxNumVisibleItems();
				this.m_ResizePreviewCacheTo = maxNumVisibleItems * 2;
				AssetPreview.SetPreviewTextureCacheSize(maxNumVisibleItems * 2 + 30, this.GetAssetPreviewManagerID());
			}
		}

		private void UpdateGroupSizes(ObjectListArea.Group g)
		{
			if (g.ListMode)
			{
				g.m_Grid.fixedWidth = this.m_VisibleRect.width;
				g.m_Grid.itemSize = new Vector2(this.m_VisibleRect.width, 16f);
				g.m_Grid.topMargin = 0f;
				g.m_Grid.bottomMargin = 0f;
				g.m_Grid.leftMargin = 0f;
				g.m_Grid.rightMargin = 0f;
				g.m_Grid.verticalSpacing = 0f;
				g.m_Grid.minHorizontalSpacing = 0f;
				g.m_Grid.InitNumRowsAndColumns(g.ItemCount, g.ItemsWantedShown);
				g.UpdateHeight();
			}
			else
			{
				g.m_Grid.fixedWidth = this.m_TotalRect.width;
				g.m_Grid.itemSize = new Vector2((float)this.gridSize, (float)(this.gridSize + 14));
				g.m_Grid.topMargin = 10f;
				g.m_Grid.bottomMargin = 10f;
				g.m_Grid.leftMargin = 10f;
				g.m_Grid.rightMargin = 10f;
				g.m_Grid.verticalSpacing = 15f;
				g.m_Grid.minHorizontalSpacing = 12f;
				g.m_Grid.InitNumRowsAndColumns(g.ItemCount, g.m_Grid.CalcRows(g.ItemsWantedShown));
				g.UpdateHeight();
			}
		}

		private int GetMaxNumVisibleItems()
		{
			int result;
			foreach (ObjectListArea.Group current in this.m_Groups)
			{
				if (!this.SkipGroup(current))
				{
					result = current.m_Grid.GetMaxVisibleItems(this.m_TotalRect.height);
					return result;
				}
			}
			result = 0;
			return result;
		}

		private static Rect AdjustRectForFraming(Rect r)
		{
			r.height += ObjectListArea.s_Styles.resultsGridLabel.fixedHeight * 2f;
			r.y -= ObjectListArea.s_Styles.resultsGridLabel.fixedHeight;
			return r;
		}

		private void CenterRect(Rect r)
		{
			float num = (r.yMax + r.yMin) / 2f;
			float num2 = this.m_TotalRect.height / 2f;
			this.m_State.m_ScrollPosition.y = num - num2;
			this.ScrollToPosition(r);
		}

		private void ScrollToPosition(Rect r)
		{
			float y = r.y;
			float yMax = r.yMax;
			float height = this.m_TotalRect.height;
			if (yMax > height + this.m_State.m_ScrollPosition.y)
			{
				this.m_State.m_ScrollPosition.y = yMax - height;
			}
			if (y < this.m_State.m_ScrollPosition.y)
			{
				this.m_State.m_ScrollPosition.y = y;
			}
			this.m_State.m_ScrollPosition.y = Mathf.Max(this.m_State.m_ScrollPosition.y, 0f);
		}

		public void OnInspectorUpdate()
		{
			if (EditorApplication.timeSinceStartup > this.m_NextDirtyCheck && this.m_LocalAssets.IsAnyLastRenderedAssetsDirty())
			{
				AssetPreview.ClearTemporaryAssetPreviews();
				this.Repaint();
				this.m_NextDirtyCheck = EditorApplication.timeSinceStartup + 0.77;
			}
			if (AssetStorePreviewManager.CheckRepaint())
			{
				this.Repaint();
			}
		}

		private void ClearCroppedLabelCache()
		{
			this.m_InstanceIDToCroppedNameMap.Clear();
		}

		protected string GetCroppedLabelText(int instanceID, string fullText, float cropWidth)
		{
			if (this.m_WidthUsedForCroppingName != (int)cropWidth)
			{
				this.ClearCroppedLabelCache();
			}
			string text;
			string result;
			if (!this.m_InstanceIDToCroppedNameMap.TryGetValue(instanceID, out text))
			{
				if (this.m_InstanceIDToCroppedNameMap.Count > this.GetMaxNumVisibleItems() * 2 + 30)
				{
					this.ClearCroppedLabelCache();
				}
				int numCharactersThatFitWithinWidth = ObjectListArea.s_Styles.resultsGridLabel.GetNumCharactersThatFitWithinWidth(fullText, cropWidth);
				if (numCharactersThatFitWithinWidth == -1)
				{
					this.Repaint();
					result = fullText;
					return result;
				}
				if (numCharactersThatFitWithinWidth > 1 && numCharactersThatFitWithinWidth != fullText.Length)
				{
					text = fullText.Substring(0, numCharactersThatFitWithinWidth - 1) + "…";
				}
				else
				{
					text = fullText;
				}
				this.m_InstanceIDToCroppedNameMap[instanceID] = text;
				this.m_WidthUsedForCroppingName = (int)cropWidth;
			}
			result = text;
			return result;
		}

		public bool IsShowing(int instanceID)
		{
			return this.m_LocalAssets.IndexOf(instanceID) >= 0;
		}

		public bool IsShowingAny(int[] instanceIDs)
		{
			bool result;
			if (instanceIDs.Length == 0)
			{
				result = false;
			}
			else
			{
				for (int i = 0; i < instanceIDs.Length; i++)
				{
					int instanceID = instanceIDs[i];
					if (this.IsShowing(instanceID))
					{
						result = true;
						return result;
					}
				}
				result = false;
			}
			return result;
		}

		protected Texture GetIconByInstanceID(int instanceID)
		{
			Texture result = null;
			if (instanceID != 0)
			{
				string assetPath = AssetDatabase.GetAssetPath(instanceID);
				result = AssetDatabase.GetCachedIcon(assetPath);
			}
			return result;
		}

		internal int GetAssetPreviewManagerID()
		{
			return this.m_Owner.GetInstanceID();
		}

		public void BeginPing(int instanceID)
		{
			if (ObjectListArea.s_Styles == null)
			{
				ObjectListArea.s_Styles = new ObjectListArea.Styles();
			}
			int num = this.m_LocalAssets.IndexOf(instanceID);
			if (num != -1)
			{
				string text = null;
				HierarchyProperty hierarchyProperty = new HierarchyProperty(HierarchyType.Assets);
				if (hierarchyProperty.Find(instanceID, null))
				{
					text = hierarchyProperty.name;
				}
				if (text != null)
				{
					this.m_Ping.m_TimeStart = Time.realtimeSinceStartup;
					this.m_Ping.m_AvailableWidth = this.m_VisibleRect.width;
					this.m_pingIndex = num;
					float num2 = (!ObjectListArea.s_VCEnabled) ? 0f : 14f;
					GUIContent gUIContent = new GUIContent((!this.m_LocalAssets.ListMode) ? this.GetCroppedLabelText(instanceID, text, (float)this.m_WidthUsedForCroppingName) : text);
					string label = gUIContent.text;
					if (this.m_LocalAssets.ListMode)
					{
						this.m_Ping.m_PingStyle = ObjectListArea.s_Styles.ping;
						Vector2 vector = this.m_Ping.m_PingStyle.CalcSize(gUIContent);
						this.m_Ping.m_ContentRect.width = vector.x + num2 + 16f;
						this.m_Ping.m_ContentRect.height = vector.y;
						this.m_LeftPaddingForPinging = ((!hierarchyProperty.isMainRepresentation) ? 28 : 16);
						FilteredHierarchy.FilterResult res = this.m_LocalAssets.LookupByInstanceID(instanceID);
						this.m_Ping.m_ContentDraw = delegate(Rect r)
						{
							ObjectListArea.LocalGroup.DrawIconAndLabel(r, res, label, hierarchyProperty.icon, false, false);
						};
					}
					else
					{
						this.m_Ping.m_PingStyle = ObjectListArea.s_Styles.miniPing;
						Vector2 vector2 = this.m_Ping.m_PingStyle.CalcSize(gUIContent);
						this.m_Ping.m_ContentRect.width = vector2.x;
						this.m_Ping.m_ContentRect.height = vector2.y;
						this.m_Ping.m_ContentDraw = delegate(Rect r)
						{
							TextAnchor alignment = ObjectListArea.s_Styles.resultsGridLabel.alignment;
							ObjectListArea.s_Styles.resultsGridLabel.alignment = TextAnchor.UpperLeft;
							ObjectListArea.s_Styles.resultsGridLabel.Draw(r, label, false, false, false, false);
							ObjectListArea.s_Styles.resultsGridLabel.alignment = alignment;
						};
					}
					Vector2 vector3 = this.CalculatePingPosition();
					this.m_Ping.m_ContentRect.x = vector3.x;
					this.m_Ping.m_ContentRect.y = vector3.y;
					this.Repaint();
				}
			}
		}

		public void EndPing()
		{
			this.m_Ping.m_TimeStart = -1f;
		}

		private void HandlePing()
		{
			if (this.m_Ping.isPinging && !this.m_LocalAssets.ListMode)
			{
				Vector2 vector = this.CalculatePingPosition();
				this.m_Ping.m_ContentRect.x = vector.x;
				this.m_Ping.m_ContentRect.y = vector.y;
			}
			this.m_Ping.HandlePing();
			if (this.m_Ping.isPinging)
			{
				this.Repaint();
			}
		}

		private Vector2 CalculatePingPosition()
		{
			Rect rect = this.m_LocalAssets.m_Grid.CalcRect(this.m_pingIndex, 0f);
			Vector2 result;
			if (this.m_LocalAssets.ListMode)
			{
				result = new Vector2((float)this.m_LeftPaddingForPinging, rect.y);
			}
			else
			{
				float width = this.m_Ping.m_ContentRect.width;
				result = new Vector2(rect.center.x - width / 2f + (float)this.m_Ping.m_PingStyle.padding.left, rect.yMax - ObjectListArea.s_Styles.resultsGridLabel.fixedHeight + 3f);
			}
			return result;
		}
	}
}
