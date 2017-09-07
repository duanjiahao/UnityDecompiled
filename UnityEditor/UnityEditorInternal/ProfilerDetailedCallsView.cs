using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace UnityEditorInternal
{
	internal class ProfilerDetailedCallsView : ProfilerDetailedView
	{
		private struct CallsData
		{
			public List<ProfilerDetailedCallsView.CallInformation> calls;

			public float totalSelectedPropertyTime;
		}

		private class CallInformation
		{
			public string name;

			public string path;

			public int callsCount;

			public int gcAllocBytes;

			public double totalCallTimeMs;

			public double totalSelfTimeMs;

			public double timePercent;
		}

		private class CallsTreeView : TreeView
		{
			public enum Type
			{
				Callers,
				Callees
			}

			public enum Column
			{
				Name,
				Calls,
				GcAlloc,
				TimeMs,
				TimePercent,
				Count
			}

			internal ProfilerDetailedCallsView.CallsData m_CallsData;

			private ProfilerDetailedCallsView.CallsTreeView.Type m_Type;

			private static string s_NoneText = LocalizationDatabase.GetLocalizedString("None");

			public CallsTreeView(ProfilerDetailedCallsView.CallsTreeView.Type type, TreeViewState treeViewState, MultiColumnHeader multicolumnHeader) : base(treeViewState, multicolumnHeader)
			{
				this.m_Type = type;
				base.showBorder = true;
				base.showAlternatingRowBackgrounds = true;
				multicolumnHeader.sortingChanged += new MultiColumnHeader.HeaderCallback(this.OnSortingChanged);
				base.Reload();
			}

			public void SetCallsData(ProfilerDetailedCallsView.CallsData callsData)
			{
				this.m_CallsData = callsData;
				foreach (ProfilerDetailedCallsView.CallInformation current in this.m_CallsData.calls)
				{
					current.timePercent = ((this.m_Type != ProfilerDetailedCallsView.CallsTreeView.Type.Callees) ? (current.totalSelfTimeMs / current.totalCallTimeMs) : (current.totalCallTimeMs / (double)this.m_CallsData.totalSelectedPropertyTime));
				}
				this.OnSortingChanged(base.multiColumnHeader);
			}

			protected override TreeViewItem BuildRoot()
			{
				TreeViewItem treeViewItem = new TreeViewItem
				{
					id = 0,
					depth = -1,
					displayName = "Root"
				};
				List<TreeViewItem> list = new List<TreeViewItem>();
				if (this.m_CallsData.calls != null && this.m_CallsData.calls.Count != 0)
				{
					list.Capacity = this.m_CallsData.calls.Count;
					for (int i = 0; i < this.m_CallsData.calls.Count; i++)
					{
						list.Add(new TreeViewItem
						{
							id = i + 1,
							depth = 0,
							displayName = this.m_CallsData.calls[i].name
						});
					}
				}
				else
				{
					list.Add(new TreeViewItem
					{
						id = 1,
						depth = 0,
						displayName = ProfilerDetailedCallsView.CallsTreeView.s_NoneText
					});
				}
				TreeView.SetupParentsAndChildrenFromDepths(treeViewItem, list);
				return treeViewItem;
			}

			protected override void RowGUI(TreeView.RowGUIArgs args)
			{
				TreeViewItem item = args.item;
				for (int i = 0; i < args.GetNumVisibleColumns(); i++)
				{
					this.CellGUI(args.GetCellRect(i), item, (ProfilerDetailedCallsView.CallsTreeView.Column)args.GetColumn(i), ref args);
				}
			}

			private void CellGUI(Rect cellRect, TreeViewItem item, ProfilerDetailedCallsView.CallsTreeView.Column column, ref TreeView.RowGUIArgs args)
			{
				if (this.m_CallsData.calls.Count == 0)
				{
					base.RowGUI(args);
				}
				else
				{
					ProfilerDetailedCallsView.CallInformation callInformation = this.m_CallsData.calls[args.item.id - 1];
					base.CenterRectUsingSingleLineHeight(ref cellRect);
					switch (column)
					{
					case ProfilerDetailedCallsView.CallsTreeView.Column.Name:
						TreeView.DefaultGUI.Label(cellRect, callInformation.name, args.selected, args.focused);
						break;
					case ProfilerDetailedCallsView.CallsTreeView.Column.Calls:
					{
						string label = callInformation.callsCount.ToString();
						TreeView.DefaultGUI.LabelRightAligned(cellRect, label, args.selected, args.focused);
						break;
					}
					case ProfilerDetailedCallsView.CallsTreeView.Column.GcAlloc:
					{
						int gcAllocBytes = callInformation.gcAllocBytes;
						TreeView.DefaultGUI.LabelRightAligned(cellRect, gcAllocBytes.ToString(), args.selected, args.focused);
						break;
					}
					case ProfilerDetailedCallsView.CallsTreeView.Column.TimeMs:
					{
						double num = (this.m_Type != ProfilerDetailedCallsView.CallsTreeView.Type.Callees) ? callInformation.totalSelfTimeMs : callInformation.totalCallTimeMs;
						TreeView.DefaultGUI.LabelRightAligned(cellRect, num.ToString("f2"), args.selected, args.focused);
						break;
					}
					case ProfilerDetailedCallsView.CallsTreeView.Column.TimePercent:
						TreeView.DefaultGUI.LabelRightAligned(cellRect, (callInformation.timePercent * 100.0).ToString("f2"), args.selected, args.focused);
						break;
					}
				}
			}

			private void OnSortingChanged(MultiColumnHeader header)
			{
				if (header.sortedColumnIndex != -1)
				{
					int orderMultiplier = (!header.IsSortedAscending(header.sortedColumnIndex)) ? -1 : 1;
					Comparison<ProfilerDetailedCallsView.CallInformation> comparison;
					switch (header.sortedColumnIndex)
					{
					case 0:
						comparison = ((ProfilerDetailedCallsView.CallInformation callInfo1, ProfilerDetailedCallsView.CallInformation callInfo2) => callInfo1.name.CompareTo(callInfo2.name) * orderMultiplier);
						break;
					case 1:
						comparison = ((ProfilerDetailedCallsView.CallInformation callInfo1, ProfilerDetailedCallsView.CallInformation callInfo2) => callInfo1.callsCount.CompareTo(callInfo2.callsCount) * orderMultiplier);
						break;
					case 2:
						comparison = ((ProfilerDetailedCallsView.CallInformation callInfo1, ProfilerDetailedCallsView.CallInformation callInfo2) => callInfo1.gcAllocBytes.CompareTo(callInfo2.gcAllocBytes) * orderMultiplier);
						break;
					case 3:
						comparison = ((ProfilerDetailedCallsView.CallInformation callInfo1, ProfilerDetailedCallsView.CallInformation callInfo2) => callInfo1.totalCallTimeMs.CompareTo(callInfo2.totalCallTimeMs) * orderMultiplier);
						break;
					case 4:
						comparison = ((ProfilerDetailedCallsView.CallInformation callInfo1, ProfilerDetailedCallsView.CallInformation callInfo2) => callInfo1.timePercent.CompareTo(callInfo2.timePercent) * orderMultiplier);
						break;
					case 5:
						comparison = ((ProfilerDetailedCallsView.CallInformation callInfo1, ProfilerDetailedCallsView.CallInformation callInfo2) => callInfo1.callsCount.CompareTo(callInfo2.callsCount) * orderMultiplier);
						break;
					default:
						return;
					}
					this.m_CallsData.calls.Sort(comparison);
					base.Reload();
				}
			}
		}

		[Serializable]
		private class CallsTreeViewController
		{
			private static class Styles
			{
				public static GUIContent callersLabel = new GUIContent("Called From", "Parents the selected function is called from\n\n(Press 'F' for frame selection)");

				public static GUIContent calleesLabel = new GUIContent("Calls To", "Functions which are called from the selected function\n\n(Press 'F' for frame selection)");

				public static GUIContent callsLabel = new GUIContent("Calls", "Total number of calls in a selected frame");

				public static GUIContent gcAllocLabel = new GUIContent("GC Alloc");

				public static GUIContent timeMsCallersLabel = new GUIContent("Time ms", "Total time the selected function spend within a parent");

				public static GUIContent timeMsCalleesLabel = new GUIContent("Time ms", "Total time the child call spend within selected function");

				public static GUIContent timePctCallersLabel = new GUIContent("Time %", "Shows how often the selected function was called from the parent call");

				public static GUIContent timePctCalleesLabel = new GUIContent("Time %", "Shows how often child call was called from the selected function");
			}

			public delegate void CallSelectedCallback(string path, Event evt);

			[NonSerialized]
			private ProfilerDetailedCallsView.CallsTreeView m_View;

			[NonSerialized]
			private bool m_Initialized;

			[SerializeField]
			private TreeViewState m_ViewState;

			[SerializeField]
			private MultiColumnHeaderState m_ViewHeaderState;

			[SerializeField]
			private ProfilerDetailedCallsView.CallsTreeView.Type m_Type;

			public event ProfilerDetailedCallsView.CallsTreeViewController.CallSelectedCallback callSelected
			{
				add
				{
					ProfilerDetailedCallsView.CallsTreeViewController.CallSelectedCallback callSelectedCallback = this.callSelected;
					ProfilerDetailedCallsView.CallsTreeViewController.CallSelectedCallback callSelectedCallback2;
					do
					{
						callSelectedCallback2 = callSelectedCallback;
						callSelectedCallback = Interlocked.CompareExchange<ProfilerDetailedCallsView.CallsTreeViewController.CallSelectedCallback>(ref this.callSelected, (ProfilerDetailedCallsView.CallsTreeViewController.CallSelectedCallback)Delegate.Combine(callSelectedCallback2, value), callSelectedCallback);
					}
					while (callSelectedCallback != callSelectedCallback2);
				}
				remove
				{
					ProfilerDetailedCallsView.CallsTreeViewController.CallSelectedCallback callSelectedCallback = this.callSelected;
					ProfilerDetailedCallsView.CallsTreeViewController.CallSelectedCallback callSelectedCallback2;
					do
					{
						callSelectedCallback2 = callSelectedCallback;
						callSelectedCallback = Interlocked.CompareExchange<ProfilerDetailedCallsView.CallsTreeViewController.CallSelectedCallback>(ref this.callSelected, (ProfilerDetailedCallsView.CallsTreeViewController.CallSelectedCallback)Delegate.Remove(callSelectedCallback2, value), callSelectedCallback);
					}
					while (callSelectedCallback != callSelectedCallback2);
				}
			}

			public CallsTreeViewController(ProfilerDetailedCallsView.CallsTreeView.Type type)
			{
				this.m_Type = type;
			}

			private void InitIfNeeded()
			{
				if (!this.m_Initialized)
				{
					if (this.m_ViewState == null)
					{
						this.m_ViewState = new TreeViewState();
					}
					bool flag = this.m_ViewHeaderState == null;
					MultiColumnHeaderState multiColumnHeaderState = this.CreateDefaultMultiColumnHeaderState();
					if (MultiColumnHeaderState.CanOverwriteSerializedFields(this.m_ViewHeaderState, multiColumnHeaderState))
					{
						MultiColumnHeaderState.OverwriteSerializedFields(this.m_ViewHeaderState, multiColumnHeaderState);
					}
					this.m_ViewHeaderState = multiColumnHeaderState;
					MultiColumnHeader multiColumnHeader = new MultiColumnHeader(this.m_ViewHeaderState)
					{
						height = 25f
					};
					if (flag)
					{
						multiColumnHeader.state.visibleColumns = new int[]
						{
							0,
							1,
							3,
							4
						};
						multiColumnHeader.ResizeToFit();
					}
					this.m_View = new ProfilerDetailedCallsView.CallsTreeView(this.m_Type, this.m_ViewState, multiColumnHeader);
					this.m_Initialized = true;
				}
			}

			private MultiColumnHeaderState CreateDefaultMultiColumnHeaderState()
			{
				MultiColumnHeaderState.Column[] columns = new MultiColumnHeaderState.Column[]
				{
					new MultiColumnHeaderState.Column
					{
						headerContent = ((this.m_Type != ProfilerDetailedCallsView.CallsTreeView.Type.Callers) ? ProfilerDetailedCallsView.CallsTreeViewController.Styles.calleesLabel : ProfilerDetailedCallsView.CallsTreeViewController.Styles.callersLabel),
						headerTextAlignment = TextAlignment.Left,
						sortedAscending = true,
						sortingArrowAlignment = TextAlignment.Center,
						width = 150f,
						minWidth = 150f,
						autoResize = true,
						allowToggleVisibility = false
					},
					new MultiColumnHeaderState.Column
					{
						headerContent = ProfilerDetailedCallsView.CallsTreeViewController.Styles.callsLabel,
						headerTextAlignment = TextAlignment.Right,
						sortedAscending = false,
						sortingArrowAlignment = TextAlignment.Center,
						width = 60f,
						minWidth = 60f,
						autoResize = false,
						allowToggleVisibility = true
					},
					new MultiColumnHeaderState.Column
					{
						headerContent = ProfilerDetailedCallsView.CallsTreeViewController.Styles.gcAllocLabel,
						headerTextAlignment = TextAlignment.Right,
						sortedAscending = false,
						sortingArrowAlignment = TextAlignment.Center,
						width = 60f,
						minWidth = 60f,
						autoResize = false,
						allowToggleVisibility = true
					},
					new MultiColumnHeaderState.Column
					{
						headerContent = ((this.m_Type != ProfilerDetailedCallsView.CallsTreeView.Type.Callers) ? ProfilerDetailedCallsView.CallsTreeViewController.Styles.timeMsCalleesLabel : ProfilerDetailedCallsView.CallsTreeViewController.Styles.timeMsCallersLabel),
						headerTextAlignment = TextAlignment.Right,
						sortedAscending = false,
						sortingArrowAlignment = TextAlignment.Center,
						width = 60f,
						minWidth = 60f,
						autoResize = false,
						allowToggleVisibility = true
					},
					new MultiColumnHeaderState.Column
					{
						headerContent = ((this.m_Type != ProfilerDetailedCallsView.CallsTreeView.Type.Callers) ? ProfilerDetailedCallsView.CallsTreeViewController.Styles.timePctCalleesLabel : ProfilerDetailedCallsView.CallsTreeViewController.Styles.timePctCallersLabel),
						headerTextAlignment = TextAlignment.Right,
						sortedAscending = false,
						sortingArrowAlignment = TextAlignment.Center,
						width = 60f,
						minWidth = 60f,
						autoResize = false,
						allowToggleVisibility = true
					}
				};
				return new MultiColumnHeaderState(columns)
				{
					sortedColumnIndex = 3
				};
			}

			public void SetCallsData(ProfilerDetailedCallsView.CallsData callsData)
			{
				this.InitIfNeeded();
				this.m_View.SetCallsData(callsData);
			}

			public void OnGUI(Rect r)
			{
				this.InitIfNeeded();
				this.m_View.OnGUI(r);
				this.HandleCommandEvents();
			}

			private void HandleCommandEvents()
			{
				if (GUIUtility.keyboardControl == this.m_View.treeViewControlID)
				{
					if (this.m_ViewState.selectedIDs.Count != 0)
					{
						int num = this.m_ViewState.selectedIDs.First<int>() - 1;
						if (num < this.m_View.m_CallsData.calls.Count)
						{
							ProfilerDetailedCallsView.CallInformation callInformation = this.m_View.m_CallsData.calls[num];
							if (this.callSelected != null)
							{
								Event current = Event.current;
								this.callSelected(callInformation.path, current);
							}
						}
					}
				}
			}
		}

		private struct ParentCallInfo
		{
			public string name;

			public string path;

			public float timeMs;
		}

		[NonSerialized]
		private bool m_Initialized = false;

		[NonSerialized]
		private float m_TotalSelectedPropertyTime;

		[NonSerialized]
		private GUIContent m_TotalSelectedPropertyTimeLabel = new GUIContent("", "Total time of all calls of the selected function in the frame.");

		[SerializeField]
		private SplitterState m_VertSplit = new SplitterState(new float[]
		{
			40f,
			60f
		}, new int[]
		{
			50,
			50
		}, null);

		[SerializeField]
		private ProfilerDetailedCallsView.CallsTreeViewController m_CallersTreeView;

		[SerializeField]
		private ProfilerDetailedCallsView.CallsTreeViewController m_CalleesTreeView;

		public ProfilerDetailedCallsView(ProfilerHierarchyGUI mainProfilerHierarchyGUI) : base(mainProfilerHierarchyGUI)
		{
		}

		private void InitIfNeeded()
		{
			if (!this.m_Initialized)
			{
				if (this.m_CallersTreeView == null)
				{
					this.m_CallersTreeView = new ProfilerDetailedCallsView.CallsTreeViewController(ProfilerDetailedCallsView.CallsTreeView.Type.Callers);
				}
				this.m_CallersTreeView.callSelected += new ProfilerDetailedCallsView.CallsTreeViewController.CallSelectedCallback(this.OnCallSelected);
				if (this.m_CalleesTreeView == null)
				{
					this.m_CalleesTreeView = new ProfilerDetailedCallsView.CallsTreeViewController(ProfilerDetailedCallsView.CallsTreeView.Type.Callees);
				}
				this.m_CalleesTreeView.callSelected += new ProfilerDetailedCallsView.CallsTreeViewController.CallSelectedCallback(this.OnCallSelected);
				this.m_Initialized = true;
			}
		}

		public void DoGUI(GUIStyle headerStyle, int frameIndex, ProfilerViewType viewType)
		{
			string selectedPropertyPath = ProfilerDriver.selectedPropertyPath;
			if (string.IsNullOrEmpty(selectedPropertyPath))
			{
				base.DrawEmptyPane(headerStyle);
			}
			else
			{
				this.InitIfNeeded();
				this.UpdateIfNeeded(frameIndex, viewType, selectedPropertyPath);
				GUILayout.BeginVertical(new GUILayoutOption[0]);
				GUILayout.Label(this.m_TotalSelectedPropertyTimeLabel, EditorStyles.label, new GUILayoutOption[0]);
				SplitterGUILayout.BeginVerticalSplit(this.m_VertSplit, new GUILayoutOption[]
				{
					GUILayout.ExpandWidth(true),
					GUILayout.ExpandHeight(true)
				});
				Rect r = EditorGUILayout.BeginVertical(new GUILayoutOption[0]);
				this.m_CalleesTreeView.OnGUI(r);
				EditorGUILayout.EndVertical();
				r = EditorGUILayout.BeginVertical(new GUILayoutOption[0]);
				this.m_CallersTreeView.OnGUI(r);
				EditorGUILayout.EndVertical();
				SplitterGUILayout.EndHorizontalSplit();
				GUILayout.EndVertical();
			}
		}

		private void UpdateIfNeeded(int frameIndex, ProfilerViewType viewType, string selectedPropertyPath)
		{
			if (!this.m_CachedProfilerPropertyConfig.EqualsTo(frameIndex, viewType, ProfilerColumn.DontSort))
			{
				ProfilerProperty rootProperty = this.m_MainProfilerHierarchyGUI.GetRootProperty();
				string profilerPropertyName = ProfilerDetailedCallsView.GetProfilerPropertyName(selectedPropertyPath);
				this.m_TotalSelectedPropertyTime = 0f;
				Dictionary<string, ProfilerDetailedCallsView.CallInformation> dictionary = new Dictionary<string, ProfilerDetailedCallsView.CallInformation>();
				Dictionary<string, ProfilerDetailedCallsView.CallInformation> dictionary2 = new Dictionary<string, ProfilerDetailedCallsView.CallInformation>();
				Stack<ProfilerDetailedCallsView.ParentCallInfo> stack = new Stack<ProfilerDetailedCallsView.ParentCallInfo>();
				bool flag = false;
				while (rootProperty.Next(true))
				{
					string propertyName = rootProperty.propertyName;
					int depth = rootProperty.depth;
					if (stack.Count + 1 != depth)
					{
						while (stack.Count + 1 > depth)
						{
							stack.Pop();
						}
						flag = (stack.Count != 0 && profilerPropertyName == stack.Peek().name);
					}
					if (stack.Count != 0)
					{
						ProfilerDetailedCallsView.ParentCallInfo parentCallInfo = stack.Peek();
						if (profilerPropertyName == propertyName)
						{
							float columnAsSingle = rootProperty.GetColumnAsSingle(ProfilerColumn.TotalTime);
							int num = (int)rootProperty.GetColumnAsSingle(ProfilerColumn.Calls);
							int num2 = (int)rootProperty.GetColumnAsSingle(ProfilerColumn.GCMemory);
							ProfilerDetailedCallsView.CallInformation callInformation;
							if (!dictionary.TryGetValue(parentCallInfo.name, out callInformation))
							{
								dictionary.Add(parentCallInfo.name, new ProfilerDetailedCallsView.CallInformation
								{
									name = parentCallInfo.name,
									path = parentCallInfo.path,
									callsCount = num,
									gcAllocBytes = num2,
									totalCallTimeMs = (double)parentCallInfo.timeMs,
									totalSelfTimeMs = (double)columnAsSingle
								});
							}
							else
							{
								callInformation.callsCount += num;
								callInformation.gcAllocBytes += num2;
								callInformation.totalCallTimeMs += (double)parentCallInfo.timeMs;
								callInformation.totalSelfTimeMs += (double)columnAsSingle;
							}
							this.m_TotalSelectedPropertyTime += columnAsSingle;
						}
						if (flag)
						{
							float columnAsSingle = rootProperty.GetColumnAsSingle(ProfilerColumn.TotalTime);
							int num = (int)rootProperty.GetColumnAsSingle(ProfilerColumn.Calls);
							int num2 = (int)rootProperty.GetColumnAsSingle(ProfilerColumn.GCMemory);
							ProfilerDetailedCallsView.CallInformation callInformation;
							if (!dictionary2.TryGetValue(propertyName, out callInformation))
							{
								dictionary2.Add(propertyName, new ProfilerDetailedCallsView.CallInformation
								{
									name = propertyName,
									path = rootProperty.propertyPath,
									callsCount = num,
									gcAllocBytes = num2,
									totalCallTimeMs = (double)columnAsSingle,
									totalSelfTimeMs = 0.0
								});
							}
							else
							{
								callInformation.callsCount += num;
								callInformation.gcAllocBytes += num2;
								callInformation.totalCallTimeMs += (double)columnAsSingle;
							}
						}
					}
					else if (profilerPropertyName == propertyName)
					{
						float columnAsSingle = rootProperty.GetColumnAsSingle(ProfilerColumn.TotalTime);
						this.m_TotalSelectedPropertyTime += columnAsSingle;
					}
					if (rootProperty.HasChildren)
					{
						float columnAsSingle = rootProperty.GetColumnAsSingle(ProfilerColumn.TotalTime);
						stack.Push(new ProfilerDetailedCallsView.ParentCallInfo
						{
							name = propertyName,
							path = rootProperty.propertyPath,
							timeMs = columnAsSingle
						});
						flag = (profilerPropertyName == propertyName);
					}
				}
				this.m_CallersTreeView.SetCallsData(new ProfilerDetailedCallsView.CallsData
				{
					calls = dictionary.Values.ToList<ProfilerDetailedCallsView.CallInformation>(),
					totalSelectedPropertyTime = this.m_TotalSelectedPropertyTime
				});
				this.m_CalleesTreeView.SetCallsData(new ProfilerDetailedCallsView.CallsData
				{
					calls = dictionary2.Values.ToList<ProfilerDetailedCallsView.CallInformation>(),
					totalSelectedPropertyTime = this.m_TotalSelectedPropertyTime
				});
				this.m_TotalSelectedPropertyTimeLabel.text = profilerPropertyName + string.Format(" - Total time: {0:f2} ms", this.m_TotalSelectedPropertyTime);
				this.m_CachedProfilerPropertyConfig.Set(frameIndex, viewType, ProfilerColumn.TotalTime);
			}
		}

		private static string GetProfilerPropertyName(string propertyPath)
		{
			int num = propertyPath.LastIndexOf('/');
			return (num != -1) ? propertyPath.Substring(num + 1) : propertyPath;
		}

		private void OnCallSelected(string path, Event evt)
		{
			EventType type = evt.type;
			if (type == EventType.ExecuteCommand || type == EventType.ValidateCommand)
			{
				if (!(evt.commandName != "FrameSelected"))
				{
					if (type == EventType.ExecuteCommand)
					{
						this.m_MainProfilerHierarchyGUI.SelectPath(path);
					}
					evt.Use();
				}
			}
		}
	}
}
