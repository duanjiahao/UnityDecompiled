using System;
using System.Collections.Generic;
using System.Text;
using UnityEditor.IMGUI.Controls;
using UnityEditorInternal;
using UnityEngine;

namespace UnityEditor
{
	internal class UISystemProfilerTreeView : TreeView
	{
		internal class State : TreeViewState
		{
			public int lastFrame;

			public ProfilerWindow profilerWindow;
		}

		internal class CanvasBatchComparer : IComparer<TreeViewItem>
		{
			internal UISystemProfilerTreeView.Column Col;

			internal bool isAscending;

			public int Compare(TreeViewItem x, TreeViewItem y)
			{
				int num = (!this.isAscending) ? -1 : 1;
				UISystemProfilerTreeView.BaseTreeViewItem baseTreeViewItem = (UISystemProfilerTreeView.BaseTreeViewItem)x;
				UISystemProfilerTreeView.BaseTreeViewItem baseTreeViewItem2 = (UISystemProfilerTreeView.BaseTreeViewItem)y;
				int result;
				if (baseTreeViewItem.info.isBatch == baseTreeViewItem2.info.isBatch)
				{
					switch (this.Col)
					{
					case UISystemProfilerTreeView.Column.Element:
						if (baseTreeViewItem.info.isBatch)
						{
							result = -1;
							return result;
						}
						result = baseTreeViewItem.displayName.CompareTo(baseTreeViewItem2.displayName) * num;
						return result;
					case UISystemProfilerTreeView.Column.BatchCount:
						if (baseTreeViewItem.info.isBatch)
						{
							result = -1;
							return result;
						}
						result = baseTreeViewItem.info.batchCount.CompareTo(baseTreeViewItem2.info.batchCount) * num;
						return result;
					case UISystemProfilerTreeView.Column.TotalBatchCount:
						if (baseTreeViewItem.info.isBatch)
						{
							result = -1;
							return result;
						}
						result = baseTreeViewItem.info.totalBatchCount.CompareTo(baseTreeViewItem2.info.totalBatchCount) * num;
						return result;
					case UISystemProfilerTreeView.Column.VertexCount:
						if (baseTreeViewItem.info.isBatch)
						{
							result = baseTreeViewItem.info.vertexCount.CompareTo(baseTreeViewItem2.info.vertexCount) * num;
							return result;
						}
						result = string.CompareOrdinal(baseTreeViewItem.displayName, baseTreeViewItem2.displayName);
						return result;
					case UISystemProfilerTreeView.Column.TotalVertexCount:
						result = baseTreeViewItem.info.totalVertexCount.CompareTo(baseTreeViewItem2.info.totalVertexCount) * num;
						return result;
					case UISystemProfilerTreeView.Column.GameObjectCount:
						result = baseTreeViewItem.info.instanceIDsCount.CompareTo(baseTreeViewItem2.info.instanceIDsCount) * num;
						return result;
					}
					throw new ArgumentOutOfRangeException();
				}
				result = ((!baseTreeViewItem.info.isBatch) ? -1 : 1);
				return result;
			}
		}

		internal class RootTreeViewItem : TreeViewItem
		{
			public int gameObjectCount;

			public int totalBatchCount;

			public int totalVertexCount;

			public RootTreeViewItem() : base(1, 0, null, "All Canvases")
			{
			}
		}

		internal class BaseTreeViewItem : TreeViewItem
		{
			protected static readonly Texture2D s_CanvasIcon = EditorGUIUtility.LoadIcon("RectTool On");

			public UISystemProfilerInfo info;

			public int renderDataIndex;

			internal BaseTreeViewItem(UISystemProfilerInfo info, int depth, string displayName) : base(info.objectInstanceId, depth, displayName)
			{
				this.info = info;
			}
		}

		internal sealed class CanvasTreeViewItem : UISystemProfilerTreeView.BaseTreeViewItem
		{
			public CanvasTreeViewItem(UISystemProfilerInfo info, int depth, string displayName) : base(info, depth, displayName)
			{
				this.icon = UISystemProfilerTreeView.BaseTreeViewItem.s_CanvasIcon;
			}
		}

		internal sealed class BatchTreeViewItem : UISystemProfilerTreeView.BaseTreeViewItem
		{
			public int[] instanceIDs;

			public BatchTreeViewItem(UISystemProfilerInfo info, int depth, string displayName, int[] allBatchesInstanceIDs) : base(info, depth, displayName)
			{
				this.icon = null;
				this.instanceIDs = new int[info.instanceIDsCount];
				Array.Copy(allBatchesInstanceIDs, info.instanceIDsIndex, this.instanceIDs, 0, info.instanceIDsCount);
				this.renderDataIndex = info.renderDataIndex;
			}
		}

		internal enum Column
		{
			Element,
			BatchCount,
			TotalBatchCount,
			VertexCount,
			TotalVertexCount,
			BatchBreakingReason,
			GameObjectCount,
			InstanceIds,
			Rerender
		}

		private readonly UISystemProfilerTreeView.CanvasBatchComparer m_Comparer;

		public ProfilerProperty property;

		private UISystemProfilerTreeView.RootTreeViewItem m_AllCanvasesItem;

		public UISystemProfilerTreeView.State profilerState
		{
			get
			{
				return (UISystemProfilerTreeView.State)base.state;
			}
		}

		public UISystemProfilerTreeView(UISystemProfilerTreeView.State state, MultiColumnHeader multiColumnHeader) : base(state, multiColumnHeader)
		{
			this.m_Comparer = new UISystemProfilerTreeView.CanvasBatchComparer();
			base.showBorder = false;
			base.showAlternatingRowBackgrounds = true;
		}

		protected override TreeViewItem BuildRoot()
		{
			return new TreeViewItem(0, -1);
		}

		protected override IList<TreeViewItem> BuildRows(TreeViewItem root)
		{
			this.profilerState.lastFrame = this.profilerState.profilerWindow.GetActiveVisibleFrameIndex();
			List<TreeViewItem> list = new List<TreeViewItem>();
			IList<TreeViewItem> result;
			if (this.property == null || !this.property.frameDataReady)
			{
				result = list;
			}
			else
			{
				this.m_AllCanvasesItem = new UISystemProfilerTreeView.RootTreeViewItem();
				base.SetExpanded(this.m_AllCanvasesItem.id, true);
				root.AddChild(this.m_AllCanvasesItem);
				UISystemProfilerInfo[] uISystemProfilerInfo = this.property.GetUISystemProfilerInfo();
				int[] uISystemBatchInstanceIDs = this.property.GetUISystemBatchInstanceIDs();
				if (uISystemProfilerInfo != null)
				{
					Dictionary<int, TreeViewItem> dictionary = new Dictionary<int, TreeViewItem>();
					int num = 0;
					UISystemProfilerInfo[] array = uISystemProfilerInfo;
					for (int i = 0; i < array.Length; i++)
					{
						UISystemProfilerInfo info = array[i];
						TreeViewItem allCanvasesItem;
						if (!dictionary.TryGetValue(info.parentId, out allCanvasesItem))
						{
							allCanvasesItem = this.m_AllCanvasesItem;
							this.m_AllCanvasesItem.totalBatchCount += info.totalBatchCount;
							this.m_AllCanvasesItem.totalVertexCount += info.totalVertexCount;
							this.m_AllCanvasesItem.gameObjectCount += info.instanceIDsCount;
						}
						UISystemProfilerTreeView.BaseTreeViewItem baseTreeViewItem;
						if (info.isBatch)
						{
							string displayName = "Batch " + num++;
							baseTreeViewItem = new UISystemProfilerTreeView.BatchTreeViewItem(info, allCanvasesItem.depth + 1, displayName, uISystemBatchInstanceIDs);
						}
						else
						{
							string displayName = this.property.GetUISystemProfilerNameByOffset(info.objectNameOffset);
							baseTreeViewItem = new UISystemProfilerTreeView.CanvasTreeViewItem(info, allCanvasesItem.depth + 1, displayName);
							num = 0;
							dictionary[info.objectInstanceId] = baseTreeViewItem;
						}
						if (!base.IsExpanded(allCanvasesItem.id))
						{
							if (!allCanvasesItem.hasChildren)
							{
								allCanvasesItem.children = TreeView.CreateChildListForCollapsedParent();
							}
						}
						else
						{
							allCanvasesItem.AddChild(baseTreeViewItem);
						}
					}
					this.m_Comparer.Col = UISystemProfilerTreeView.Column.Element;
					if (base.multiColumnHeader.sortedColumnIndex != -1)
					{
						this.m_Comparer.Col = (UISystemProfilerTreeView.Column)base.multiColumnHeader.sortedColumnIndex;
					}
					this.m_Comparer.isAscending = base.multiColumnHeader.GetColumn((int)this.m_Comparer.Col).sortedAscending;
					this.SetupRows(this.m_AllCanvasesItem, list);
				}
				result = list;
			}
			return result;
		}

		protected override void RowGUI(TreeView.RowGUIArgs args)
		{
			if (Event.current.type == EventType.Repaint)
			{
				int i = 0;
				int numVisibleColumns = args.GetNumVisibleColumns();
				while (i < numVisibleColumns)
				{
					int column = args.GetColumn(i);
					Rect cellRect = args.GetCellRect(i);
					if (column == 0)
					{
						GUIStyle label = TreeView.DefaultStyles.label;
						cellRect.xMin += (float)label.margin.left + base.GetContentIndent(args.item);
						int num = 16;
						int num2 = 2;
						Rect position = cellRect;
						position.width = (float)num;
						Texture icon = args.item.icon;
						if (icon != null)
						{
							GUI.DrawTexture(position, icon, ScaleMode.ScaleToFit);
						}
						label.padding.left = ((!(icon == null)) ? (num + num2) : 0);
						label.Draw(cellRect, args.item.displayName, false, false, args.selected, args.focused);
					}
					else
					{
						string itemcontent = this.GetItemcontent(args, column);
						if (itemcontent != null)
						{
							TreeView.DefaultGUI.LabelRightAligned(cellRect, itemcontent, args.selected, args.focused);
						}
						else
						{
							GUI.enabled = false;
							TreeView.DefaultGUI.LabelRightAligned(cellRect, "-", false, false);
							GUI.enabled = true;
						}
					}
					i++;
				}
			}
		}

		protected override void ContextClickedItem(int id)
		{
			GenericMenu genericMenu = new GenericMenu();
			genericMenu.AddItem(new GUIContent("Find matching objects in scene"), false, delegate
			{
				this.DoubleClickedItem(id);
			});
			genericMenu.ShowAsContext();
		}

		protected override void DoubleClickedItem(int id)
		{
			IList<TreeViewItem> rowsFromIDs = this.GetRowsFromIDs(new List<int>
			{
				id
			});
			UISystemProfilerTreeView.HighlightRowsMatchingObjects(rowsFromIDs);
		}

		private static void HighlightRowsMatchingObjects(IList<TreeViewItem> rows)
		{
			List<int> list = new List<int>();
			foreach (TreeViewItem current in rows)
			{
				UISystemProfilerTreeView.BatchTreeViewItem batchTreeViewItem = current as UISystemProfilerTreeView.BatchTreeViewItem;
				if (batchTreeViewItem != null)
				{
					list.AddRange(batchTreeViewItem.instanceIDs);
				}
				else
				{
					UISystemProfilerTreeView.CanvasTreeViewItem canvasTreeViewItem = current as UISystemProfilerTreeView.CanvasTreeViewItem;
					if (canvasTreeViewItem != null)
					{
						Canvas canvas = EditorUtility.InstanceIDToObject(canvasTreeViewItem.info.objectInstanceId) as Canvas;
						if (!(canvas == null) && !(canvas.gameObject == null))
						{
							list.Add(canvas.gameObject.GetInstanceID());
						}
					}
				}
			}
			if (list.Count > 0)
			{
				Selection.instanceIDs = list.ToArray();
			}
		}

		private void SetupRows(TreeViewItem item, IList<TreeViewItem> rows)
		{
			rows.Add(item);
			if (item.hasChildren && !TreeView.IsChildListForACollapsedParent(item.children))
			{
				if (this.m_Comparer.Col != UISystemProfilerTreeView.Column.Element || this.m_Comparer.isAscending)
				{
					item.children.Sort(this.m_Comparer);
				}
				foreach (TreeViewItem current in item.children)
				{
					this.SetupRows(current, rows);
				}
			}
		}

		private string GetItemcontent(TreeView.RowGUIArgs args, int column)
		{
			string result;
			if (this.m_AllCanvasesItem != null && args.item.id == this.m_AllCanvasesItem.id)
			{
				switch (column)
				{
				case 2:
					result = this.m_AllCanvasesItem.totalBatchCount.ToString();
					return result;
				case 4:
					result = this.m_AllCanvasesItem.totalVertexCount.ToString();
					return result;
				case 6:
					result = this.m_AllCanvasesItem.gameObjectCount.ToString();
					return result;
				}
				result = null;
			}
			else
			{
				UISystemProfilerTreeView.BatchTreeViewItem batchTreeViewItem = args.item as UISystemProfilerTreeView.BatchTreeViewItem;
				if (batchTreeViewItem != null)
				{
					UISystemProfilerInfo info = batchTreeViewItem.info;
					switch (column)
					{
					case 0:
					case 1:
					case 2:
						break;
					case 3:
						result = info.vertexCount.ToString();
						return result;
					case 4:
						result = info.totalVertexCount.ToString();
						return result;
					case 5:
						if (info.batchBreakingReason != BatchBreakingReason.NoBreaking)
						{
							result = UISystemProfilerTreeView.FormatBatchBreakingReason(info);
							return result;
						}
						break;
					case 6:
						result = info.instanceIDsCount.ToString();
						return result;
					case 7:
						if (batchTreeViewItem.instanceIDs.Length <= 5)
						{
							StringBuilder stringBuilder = new StringBuilder();
							for (int i = 0; i < batchTreeViewItem.instanceIDs.Length; i++)
							{
								if (i != 0)
								{
									stringBuilder.Append(", ");
								}
								int num = batchTreeViewItem.instanceIDs[i];
								UnityEngine.Object @object = EditorUtility.InstanceIDToObject(num);
								if (@object == null)
								{
									stringBuilder.Append(num);
								}
								else
								{
									stringBuilder.Append(@object.name);
								}
							}
							result = stringBuilder.ToString();
							return result;
						}
						result = string.Format("{0} objects", batchTreeViewItem.instanceIDs.Length);
						return result;
					case 8:
						result = info.renderDataIndex.ToString();
						return result;
					default:
						result = "Missing";
						return result;
					}
					result = null;
				}
				else
				{
					UISystemProfilerTreeView.CanvasTreeViewItem canvasTreeViewItem = args.item as UISystemProfilerTreeView.CanvasTreeViewItem;
					if (canvasTreeViewItem != null)
					{
						UISystemProfilerInfo info2 = canvasTreeViewItem.info;
						switch (column)
						{
						case 0:
						case 3:
						case 5:
						case 7:
							result = null;
							break;
						case 1:
							result = info2.batchCount.ToString();
							break;
						case 2:
							result = info2.totalBatchCount.ToString();
							break;
						case 4:
							result = info2.totalVertexCount.ToString();
							break;
						case 6:
							result = info2.instanceIDsCount.ToString();
							break;
						case 8:
							result = info2.renderDataIndex + " : " + info2.renderDataCount;
							break;
						default:
							result = "Missing";
							break;
						}
					}
					else
					{
						result = null;
					}
				}
			}
			return result;
		}

		internal IList<TreeViewItem> GetRowsFromIDs(IList<int> selection)
		{
			return base.FindRows(selection);
		}

		private static string FormatBatchBreakingReason(UISystemProfilerInfo info)
		{
			BatchBreakingReason batchBreakingReason = info.batchBreakingReason;
			switch (batchBreakingReason)
			{
			case BatchBreakingReason.NoBreaking:
			{
				string result = "NoBreaking";
				return result;
			}
			case BatchBreakingReason.NotCoplanarWithCanvas:
			{
				string result = "Not Coplanar With Canvas";
				return result;
			}
			case BatchBreakingReason.CanvasInjectionIndex:
			{
				string result = "Canvas Injection Index";
				return result;
			}
			case (BatchBreakingReason)3:
			case (BatchBreakingReason)5:
			case (BatchBreakingReason)6:
			case (BatchBreakingReason)7:
			{
				IL_33:
				string result;
				if (batchBreakingReason == BatchBreakingReason.DifferentTexture)
				{
					result = "Different Texture";
					return result;
				}
				if (batchBreakingReason == BatchBreakingReason.DifferentA8TextureUsage)
				{
					result = "Different A8 Texture Usage";
					return result;
				}
				if (batchBreakingReason == BatchBreakingReason.DifferentClipRect)
				{
					result = "Different Clip Rect";
					return result;
				}
				if (batchBreakingReason != BatchBreakingReason.Unknown)
				{
					throw new ArgumentOutOfRangeException();
				}
				result = "Unknown";
				return result;
			}
			case BatchBreakingReason.DifferentMaterialInstance:
			{
				string result = "Different Material Instance";
				return result;
			}
			case BatchBreakingReason.DifferentRectClipping:
			{
				string result = "Different Rect Clipping";
				return result;
			}
			}
			goto IL_33;
		}
	}
}
