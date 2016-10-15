using System;
using System.Collections.Generic;
using System.IO;
using UnityEditorInternal;
using UnityEngine;

namespace UnityEditor
{
	internal class HeapshotWindow : EditorWindow
	{
		public class HeapshotUIObject
		{
			private string name;

			private HeapshotReader.ObjectInfo obj;

			private List<HeapshotWindow.HeapshotUIObject> children = new List<HeapshotWindow.HeapshotUIObject>();

			private bool inverseReference;

			private bool isDummyObject;

			public bool HasChildren
			{
				get
				{
					return (!this.inverseReference) ? (this.obj.references.Count > 0) : (this.obj.inverseReferences.Count > 0);
				}
			}

			public bool IsExpanded
			{
				get
				{
					return this.HasChildren && this.children.Count > 0;
				}
			}

			public string Name
			{
				get
				{
					return this.name;
				}
			}

			public uint Code
			{
				get
				{
					return this.obj.code;
				}
			}

			public uint Size
			{
				get
				{
					return this.obj.size;
				}
			}

			public int ReferenceCount
			{
				get
				{
					return (!this.inverseReference) ? this.obj.references.Count : this.obj.inverseReferences.Count;
				}
			}

			public int InverseReferenceCount
			{
				get
				{
					return (!this.inverseReference) ? this.obj.inverseReferences.Count : this.obj.references.Count;
				}
			}

			public bool IsDummyObject
			{
				get
				{
					return this.isDummyObject;
				}
				set
				{
					this.isDummyObject = value;
				}
			}

			public string TypeName
			{
				get
				{
					return this.obj.typeInfo.name;
				}
			}

			public HeapshotReader.ObjectInfo ObjectInfo
			{
				get
				{
					return this.obj;
				}
			}

			public List<HeapshotWindow.HeapshotUIObject> Children
			{
				get
				{
					if (this.HasChildren && this.IsExpanded)
					{
						return this.children;
					}
					return null;
				}
			}

			public HeapshotUIObject(string name, HeapshotReader.ObjectInfo refObject, bool inverseReference)
			{
				this.name = name;
				this.obj = refObject;
				this.inverseReference = inverseReference;
			}

			public void Expand()
			{
				if (this.IsExpanded)
				{
					return;
				}
				if (this.HasChildren)
				{
					if (this.inverseReference)
					{
						foreach (HeapshotReader.BackReferenceInfo current in this.obj.inverseReferences)
						{
							this.children.Add(new HeapshotWindow.HeapshotUIObject(current.fieldInfo.name, current.parentObject, true));
						}
					}
					else
					{
						foreach (HeapshotReader.ReferenceInfo current2 in this.obj.references)
						{
							this.children.Add(new HeapshotWindow.HeapshotUIObject(current2.fieldInfo.name, current2.referencedObject, false));
						}
					}
				}
			}

			public void Collapse()
			{
				if (!this.IsExpanded)
				{
					return;
				}
				this.children.Clear();
			}
		}

		internal class UIStyles
		{
			public GUIStyle background = "OL Box";

			public GUIStyle header = "OL title";

			public GUIStyle rightHeader = "OL title TextRight";

			public GUIStyle entryEven = "OL EntryBackEven";

			public GUIStyle entryOdd = "OL EntryBackOdd";

			public GUIStyle numberLabel = "OL Label";

			public GUIStyle foldout = "IN foldout";
		}

		internal class UIOptions
		{
			public const float height = 16f;

			public const float foldoutWidth = 14f;

			public const float tabWidth = 50f;
		}

		private delegate void OnSelect(HeapshotWindow.HeapshotUIObject o);

		private delegate void DelegateReceivedHeapshot(string fileName);

		private const string heapshotExtension = ".heapshot";

		private HeapshotReader heapshotReader;

		private List<string> heapshotFiles = new List<string>();

		private int itemIndex = -1;

		private Rect guiRect;

		private int selectedItem = -1;

		private int currentTab;

		private string lastOpenedHeapshotFile = string.Empty;

		private string lastOpenedProfiler = string.Empty;

		private static HeapshotWindow.DelegateReceivedHeapshot onReceivedHeapshot;

		private List<HeapshotWindow.HeapshotUIObject> hsRoots = new List<HeapshotWindow.HeapshotUIObject>();

		private List<HeapshotWindow.HeapshotUIObject> hsAllObjects = new List<HeapshotWindow.HeapshotUIObject>();

		private List<HeapshotWindow.HeapshotUIObject> hsBackTraceObjects = new List<HeapshotWindow.HeapshotUIObject>();

		private Vector2 leftViewScrollPosition = Vector2.zero;

		private Vector2 rightViewScrollPosition = Vector2.zero;

		private static HeapshotWindow.UIStyles ms_Styles;

		private SplitterState viewSplit = new SplitterState(new float[]
		{
			50f,
			50f
		}, null, null);

		private string[] titleNames = new string[]
		{
			"Field Name",
			"Type",
			"Pointer",
			"Size",
			"References/Referenced"
		};

		private SplitterState titleSplit1 = new SplitterState(new float[]
		{
			30f,
			25f,
			15f,
			15f,
			15f
		}, new int[]
		{
			200,
			200,
			50,
			50,
			50
		}, null);

		private SplitterState titleSplit2 = new SplitterState(new float[]
		{
			30f,
			25f,
			15f,
			15f,
			15f
		}, new int[]
		{
			200,
			200,
			50,
			50,
			50
		}, null);

		private int selectedHeapshot = -1;

		private int[] connectionGuids;

		private string HeapshotPath
		{
			get
			{
				return Application.dataPath + "/../Heapshots";
			}
		}

		private static HeapshotWindow.UIStyles Styles
		{
			get
			{
				HeapshotWindow.UIStyles arg_17_0;
				if ((arg_17_0 = HeapshotWindow.ms_Styles) == null)
				{
					arg_17_0 = (HeapshotWindow.ms_Styles = new HeapshotWindow.UIStyles());
				}
				return arg_17_0;
			}
		}

		private static void Init()
		{
			EditorWindow window = EditorWindow.GetWindow(typeof(HeapshotWindow));
			window.titleContent = EditorGUIUtility.TextContent("Mono heapshot");
		}

		private static void EventHeapShotReceived(string name)
		{
			Debug.Log("Received " + name);
			if (HeapshotWindow.onReceivedHeapshot != null)
			{
				HeapshotWindow.onReceivedHeapshot(name);
			}
		}

		private void OnReceivedHeapshot(string name)
		{
			this.SearchForHeapShots();
			this.OpenHeapshot(name);
		}

		private void SearchForHeapShots()
		{
			this.heapshotFiles.Clear();
			if (!Directory.Exists(this.HeapshotPath))
			{
				return;
			}
			string[] files = Directory.GetFiles(this.HeapshotPath, "*.heapshot");
			this.selectedHeapshot = -1;
			string[] array = files;
			for (int i = 0; i < array.Length; i++)
			{
				string text = array[i];
				string text2 = text.Substring(text.LastIndexOf("\\") + 1);
				text2 = text2.Substring(0, text2.IndexOf(".heapshot"));
				this.heapshotFiles.Add(text2);
			}
			if (this.heapshotFiles.Count > 0)
			{
				this.selectedHeapshot = this.heapshotFiles.Count - 1;
			}
		}

		private void OnEnable()
		{
			HeapshotWindow.onReceivedHeapshot = new HeapshotWindow.DelegateReceivedHeapshot(this.OnReceivedHeapshot);
		}

		private void OnDisable()
		{
			HeapshotWindow.onReceivedHeapshot = null;
		}

		private void OnFocus()
		{
			this.SearchForHeapShots();
		}

		private void RefreshHeapshotUIObjects()
		{
			this.hsRoots.Clear();
			this.hsAllObjects.Clear();
			foreach (HeapshotReader.ReferenceInfo current in this.heapshotReader.Roots)
			{
				string name = current.fieldInfo.name;
				this.hsRoots.Add(new HeapshotWindow.HeapshotUIObject(name, current.referencedObject, false));
			}
			SortedDictionary<string, List<HeapshotReader.ObjectInfo>> sortedDictionary = new SortedDictionary<string, List<HeapshotReader.ObjectInfo>>();
			foreach (HeapshotReader.ObjectInfo current2 in this.heapshotReader.Objects)
			{
				if (current2.type == HeapshotReader.ObjectType.Managed)
				{
					string name2 = current2.typeInfo.name;
					if (!sortedDictionary.ContainsKey(name2))
					{
						sortedDictionary.Add(name2, new List<HeapshotReader.ObjectInfo>());
					}
					sortedDictionary[name2].Add(current2);
				}
			}
			foreach (KeyValuePair<string, List<HeapshotReader.ObjectInfo>> current3 in sortedDictionary)
			{
				HeapshotReader.ObjectInfo objectInfo = new HeapshotReader.ObjectInfo();
				HeapshotReader.FieldInfo field = new HeapshotReader.FieldInfo("(Unknown)");
				foreach (HeapshotReader.ObjectInfo current4 in current3.Value)
				{
					objectInfo.references.Add(new HeapshotReader.ReferenceInfo(current4, field));
				}
				HeapshotWindow.HeapshotUIObject heapshotUIObject = new HeapshotWindow.HeapshotUIObject(current3.Key + " x " + current3.Value.Count, objectInfo, false);
				heapshotUIObject.IsDummyObject = true;
				this.hsAllObjects.Add(heapshotUIObject);
			}
		}

		private int GetItemCount(List<HeapshotWindow.HeapshotUIObject> objects)
		{
			int num = 0;
			foreach (HeapshotWindow.HeapshotUIObject current in objects)
			{
				num++;
				if (current.IsExpanded)
				{
					num += this.GetItemCount(current.Children);
				}
			}
			return num;
		}

		private void OpenHeapshot(string fileName)
		{
			this.heapshotReader = new HeapshotReader();
			string text = this.HeapshotPath + "/" + fileName;
			if (this.heapshotReader.Open(text))
			{
				this.lastOpenedHeapshotFile = fileName;
				this.RefreshHeapshotUIObjects();
			}
			else
			{
				Debug.LogError("Failed to read " + text);
			}
		}

		private void OnGUI()
		{
			GUI.Label(new Rect(0f, 0f, base.position.width, 20f), "Heapshots are located here: " + Path.Combine(Application.dataPath, "Heapshots"));
			GUI.Label(new Rect(0f, 20f, base.position.width, 20f), "Currently opened: " + this.lastOpenedHeapshotFile);
			GUI.Label(new Rect(100f, 40f, base.position.width, 20f), "Profiling: " + this.lastOpenedProfiler);
			this.DoActiveProfilerButton(new Rect(0f, 40f, 100f, 30f));
			if (GUI.Button(new Rect(0f, 70f, 200f, 20f), "CaptureHeapShot", EditorStyles.toolbarDropDown))
			{
				ProfilerDriver.CaptureHeapshot();
			}
			GUI.changed = false;
			this.selectedHeapshot = EditorGUI.Popup(new Rect(250f, 70f, 500f, 30f), "Click to open -->", this.selectedHeapshot, this.heapshotFiles.ToArray());
			if (GUI.changed && this.heapshotFiles[this.selectedHeapshot].Length > 0)
			{
				this.OpenHeapshot(this.heapshotFiles[this.selectedHeapshot] + ".heapshot");
			}
			GUILayout.BeginArea(new Rect(0f, 90f, base.position.width, 60f));
			SplitterGUILayout.BeginHorizontalSplit(this.viewSplit, new GUILayoutOption[0]);
			GUILayout.BeginVertical(new GUILayoutOption[0]);
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			string[] array = new string[]
			{
				"Roots",
				"All Objects"
			};
			for (int i = 0; i < array.Length; i++)
			{
				bool flag = GUILayout.Toggle(this.currentTab == i, array[i], EditorStyles.toolbarButton, new GUILayoutOption[]
				{
					GUILayout.MaxHeight(16f)
				});
				if (flag)
				{
					this.currentTab = i;
				}
			}
			GUILayout.EndHorizontal();
			this.DoTitles(this.titleSplit1);
			GUILayout.EndVertical();
			GUILayout.BeginVertical(new GUILayoutOption[0]);
			GUILayout.Label("Back trace references", EditorStyles.toolbarButton, new GUILayoutOption[]
			{
				GUILayout.MaxHeight(16f)
			});
			this.DoTitles(this.titleSplit2);
			GUILayout.EndVertical();
			SplitterGUILayout.EndHorizontalSplit();
			GUILayout.EndArea();
			this.guiRect = new Rect(0f, 130f, (float)this.viewSplit.realSizes[0], 16f);
			float height = (float)this.GetItemCount(this.hsAllObjects) * 16f;
			Rect position = new Rect(this.guiRect.x, this.guiRect.y, this.guiRect.width, base.position.height - this.guiRect.y);
			this.leftViewScrollPosition = GUI.BeginScrollView(position, this.leftViewScrollPosition, new Rect(0f, 0f, position.width - 20f, height));
			this.itemIndex = 0;
			this.guiRect.y = 0f;
			int num = this.currentTab;
			if (num != 0)
			{
				if (num == 1)
				{
					this.DoHeapshotObjects(this.hsAllObjects, this.titleSplit1, 0, new HeapshotWindow.OnSelect(this.OnSelectObject));
				}
			}
			else
			{
				this.DoHeapshotObjects(this.hsRoots, this.titleSplit1, 0, new HeapshotWindow.OnSelect(this.OnSelectObject));
			}
			GUI.EndScrollView();
			this.guiRect = new Rect((float)this.viewSplit.realSizes[0], 130f, (float)this.viewSplit.realSizes[1], 16f);
			float height2 = (float)this.GetItemCount(this.hsBackTraceObjects) * 16f;
			position = new Rect(this.guiRect.x, this.guiRect.y, this.guiRect.width, base.position.height - this.guiRect.y);
			this.rightViewScrollPosition = GUI.BeginScrollView(position, this.rightViewScrollPosition, new Rect(0f, 0f, position.width - 20f, height2));
			if (this.hsBackTraceObjects.Count > 0)
			{
				this.guiRect.y = 0f;
				this.itemIndex = 0;
				this.DoHeapshotObjects(this.hsBackTraceObjects, this.titleSplit2, 0, null);
			}
			GUI.EndScrollView();
		}

		private void OnSelectObject(HeapshotWindow.HeapshotUIObject o)
		{
			this.hsBackTraceObjects.Clear();
			this.hsBackTraceObjects.Add(new HeapshotWindow.HeapshotUIObject(o.Name, o.ObjectInfo, true));
		}

		private void DoActiveProfilerButton(Rect position)
		{
			if (EditorGUI.ButtonMouseDown(position, new GUIContent("Active Profler"), FocusType.Native, EditorStyles.toolbarDropDown))
			{
				int connectedProfiler = ProfilerDriver.connectedProfiler;
				this.connectionGuids = ProfilerDriver.GetAvailableProfilers();
				int num = this.connectionGuids.Length;
				int[] array = new int[1];
				bool[] array2 = new bool[num];
				string[] array3 = new string[num];
				for (int i = 0; i < num; i++)
				{
					int num2 = this.connectionGuids[i];
					bool flag = ProfilerDriver.IsIdentifierConnectable(num2);
					array2[i] = flag;
					string text = ProfilerDriver.GetConnectionIdentifier(num2);
					if (!flag)
					{
						text += " (Version mismatch)";
					}
					array3[i] = text;
					if (num2 == connectedProfiler)
					{
						array[0] = i;
					}
				}
				EditorUtility.DisplayCustomMenu(position, array3, array2, array, new EditorUtility.SelectMenuItemFunction(this.SelectProfilerClick), null);
			}
		}

		private void SelectProfilerClick(object userData, string[] options, int selected)
		{
			int num = this.connectionGuids[selected];
			this.lastOpenedProfiler = ProfilerDriver.GetConnectionIdentifier(num);
			ProfilerDriver.connectedProfiler = num;
		}

		private void DoTitles(SplitterState splitter)
		{
			SplitterGUILayout.BeginHorizontalSplit(splitter, new GUILayoutOption[0]);
			for (int i = 0; i < this.titleNames.Length; i++)
			{
				GUILayout.Toggle(false, this.titleNames[i], EditorStyles.toolbarButton, new GUILayoutOption[]
				{
					GUILayout.MaxHeight(16f)
				});
			}
			SplitterGUILayout.EndHorizontalSplit();
		}

		private void DoHeapshotObjects(List<HeapshotWindow.HeapshotUIObject> objects, SplitterState splitter, int indent, HeapshotWindow.OnSelect onSelect)
		{
			if (objects == null)
			{
				return;
			}
			Event current = Event.current;
			foreach (HeapshotWindow.HeapshotUIObject current2 in objects)
			{
				Rect position = new Rect(14f * (float)indent, this.guiRect.y, 14f, this.guiRect.height);
				Rect[] array = new Rect[this.titleNames.Length];
				float num = 14f * (float)(indent + 1);
				for (int i = 0; i < array.Length; i++)
				{
					float num2 = (i != 0) ? ((float)splitter.realSizes[i]) : ((float)splitter.realSizes[i] - num);
					array[i] = new Rect(num, this.guiRect.y, num2, this.guiRect.height);
					num += num2;
				}
				if (current.type == EventType.Repaint)
				{
					Rect position2 = new Rect(0f, 16f * (float)this.itemIndex, base.position.width, 16f);
					GUIStyle gUIStyle = ((this.itemIndex & 1) != 0) ? HeapshotWindow.Styles.entryOdd : HeapshotWindow.Styles.entryEven;
					gUIStyle.Draw(position2, GUIContent.none, false, false, this.itemIndex == this.selectedItem, false);
				}
				if (current2.HasChildren)
				{
					GUI.changed = false;
					bool flag = GUI.Toggle(position, current2.IsExpanded, GUIContent.none, HeapshotWindow.Styles.foldout);
					if (GUI.changed)
					{
						if (flag)
						{
							current2.Expand();
						}
						else
						{
							current2.Collapse();
						}
					}
				}
				GUI.changed = false;
				bool flag2 = GUI.Toggle(array[0], this.itemIndex == this.selectedItem, current2.Name, HeapshotWindow.Styles.numberLabel);
				if (!current2.IsDummyObject)
				{
					GUI.Toggle(array[1], this.itemIndex == this.selectedItem, current2.TypeName, HeapshotWindow.Styles.numberLabel);
					GUI.Toggle(array[2], this.itemIndex == this.selectedItem, "0x" + current2.Code.ToString("X"), HeapshotWindow.Styles.numberLabel);
					GUI.Toggle(array[3], this.itemIndex == this.selectedItem, current2.Size.ToString(), HeapshotWindow.Styles.numberLabel);
					GUI.Toggle(array[4], this.itemIndex == this.selectedItem, string.Format("{0} / {1}", current2.ReferenceCount, current2.InverseReferenceCount), HeapshotWindow.Styles.numberLabel);
					if (GUI.changed && flag2 && onSelect != null)
					{
						this.selectedItem = this.itemIndex;
						onSelect(current2);
					}
				}
				this.itemIndex++;
				this.guiRect.y = this.guiRect.y + 16f;
				this.DoHeapshotObjects(current2.Children, splitter, indent + 1, onSelect);
			}
		}
	}
}
