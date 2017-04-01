using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor.VersionControl;
using UnityEngine;

namespace UnityEditor
{
	[CustomEditor(typeof(MonoManager))]
	internal class ScriptExecutionOrderInspector : Editor
	{
		public class SortMonoScriptExecutionOrder : IComparer<MonoScript>
		{
			private ScriptExecutionOrderInspector inspector;

			public SortMonoScriptExecutionOrder(ScriptExecutionOrderInspector inspector)
			{
				this.inspector = inspector;
			}

			public int Compare(MonoScript x, MonoScript y)
			{
				int result;
				if (x != null && y != null)
				{
					int executionOrder = this.inspector.GetExecutionOrder(x);
					int executionOrder2 = this.inspector.GetExecutionOrder(y);
					if (executionOrder == executionOrder2)
					{
						result = x.name.CompareTo(y.name);
					}
					else
					{
						result = executionOrder.CompareTo(executionOrder2);
					}
				}
				else
				{
					result = -1;
				}
				return result;
			}
		}

		public class SortMonoScriptNameOrder : IComparer<MonoScript>
		{
			public int Compare(MonoScript x, MonoScript y)
			{
				int result;
				if (x != null && y != null)
				{
					result = x.name.CompareTo(y.name);
				}
				else
				{
					result = -1;
				}
				return result;
			}
		}

		public class Styles
		{
			public GUIContent helpText = EditorGUIUtility.TextContent("Add scripts to the custom order and drag them to reorder.\n\nScripts in the custom order can execute before or after the default time and are executed from top to bottom. All other scripts execute at the default time in the order they are loaded.\n\n(Changing the order of a script may modify the meta data for more than one script.)");

			public GUIContent iconToolbarPlus = EditorGUIUtility.IconContent("Toolbar Plus", "|Add script to custom order");

			public GUIContent iconToolbarMinus = EditorGUIUtility.IconContent("Toolbar Minus", "|Remove script from custom order");

			public GUIContent defaultTimeContent = EditorGUIUtility.TextContent("Default Time|All scripts not in the custom order are executed at the default time.");

			public GUIStyle toolbar = "TE Toolbar";

			public GUIStyle toolbarDropDown = "TE ToolbarDropDown";

			public GUIStyle boxBackground = "TE NodeBackground";

			public GUIStyle removeButton = "InvisibleButton";

			public GUIStyle elementBackground = new GUIStyle("OL Box");

			public GUIStyle defaultTime = new GUIStyle(EditorStyles.inspectorBig);

			public GUIStyle draggingHandle = "WindowBottomResize";

			public GUIStyle dropField = new GUIStyle(EditorStyles.objectFieldThumb);

			public Styles()
			{
				this.boxBackground.margin = new RectOffset();
				this.boxBackground.padding = new RectOffset(1, 1, 1, 0);
				this.elementBackground.overflow = new RectOffset(1, 1, 1, 0);
				this.defaultTime.alignment = TextAnchor.MiddleCenter;
				this.defaultTime.overflow = new RectOffset(0, 0, 1, 0);
				this.dropField.overflow = new RectOffset(2, 2, 2, 2);
				this.dropField.normal.background = null;
				this.dropField.hover.background = null;
				this.dropField.active.background = null;
				this.dropField.focused.background = null;
			}
		}

		private class DragReorderGUI
		{
			public delegate void DrawElementDelegate(Rect r, object obj, bool dragging);

			private static int s_ReorderingDraggedElement;

			private static float[] s_ReorderingPositions;

			private static int[] s_ReorderingGoals;

			private static int s_DragReorderGUIHash = "DragReorderGUI".GetHashCode();

			private static bool IsDefaultTimeElement(MonoScript element)
			{
				return element.name == string.Empty;
			}

			public static int DragReorder(Rect position, int elementHeight, List<MonoScript> elements, ScriptExecutionOrderInspector.DragReorderGUI.DrawElementDelegate drawElementDelegate)
			{
				int controlID = GUIUtility.GetControlID(ScriptExecutionOrderInspector.DragReorderGUI.s_DragReorderGUIHash, FocusType.Passive);
				Rect r = position;
				r.height = (float)elementHeight;
				int num = 0;
				Rect position2;
				if (GUIUtility.hotControl == controlID && Event.current.type == EventType.Repaint)
				{
					for (int i = 0; i < elements.Count; i++)
					{
						if (i != ScriptExecutionOrderInspector.DragReorderGUI.s_ReorderingDraggedElement)
						{
							if (ScriptExecutionOrderInspector.DragReorderGUI.IsDefaultTimeElement(elements[i]))
							{
								num = i;
								i++;
							}
							else
							{
								r.y = position.y + ScriptExecutionOrderInspector.DragReorderGUI.s_ReorderingPositions[i] * (float)elementHeight;
								drawElementDelegate(r, elements[i], false);
							}
						}
					}
					position2 = new Rect(r.x, position.y + ScriptExecutionOrderInspector.DragReorderGUI.s_ReorderingPositions[num] * (float)elementHeight, r.width, (ScriptExecutionOrderInspector.DragReorderGUI.s_ReorderingPositions[num + 1] - ScriptExecutionOrderInspector.DragReorderGUI.s_ReorderingPositions[num] + 1f) * (float)elementHeight);
				}
				else
				{
					for (int j = 0; j < elements.Count; j++)
					{
						r.y = position.y + (float)(j * elementHeight);
						if (ScriptExecutionOrderInspector.DragReorderGUI.IsDefaultTimeElement(elements[j]))
						{
							num = j;
							j++;
						}
						else
						{
							drawElementDelegate(r, elements[j], false);
						}
					}
					position2 = new Rect(r.x, position.y + (float)(num * elementHeight), r.width, (float)(elementHeight * 2));
				}
				GUI.Label(position2, ScriptExecutionOrderInspector.m_Styles.defaultTimeContent, ScriptExecutionOrderInspector.m_Styles.defaultTime);
				bool flag = position2.height > (float)elementHeight * 2.5f;
				if (GUIUtility.hotControl == controlID)
				{
					if (flag)
					{
						GUI.color = new Color(1f, 1f, 1f, 0.5f);
					}
					r.y = position.y + ScriptExecutionOrderInspector.DragReorderGUI.s_ReorderingPositions[ScriptExecutionOrderInspector.DragReorderGUI.s_ReorderingDraggedElement] * (float)elementHeight;
					drawElementDelegate(r, elements[ScriptExecutionOrderInspector.DragReorderGUI.s_ReorderingDraggedElement], true);
					GUI.color = Color.white;
				}
				int result = -1;
				switch (Event.current.GetTypeForControl(controlID))
				{
				case EventType.MouseDown:
					if (position.Contains(Event.current.mousePosition))
					{
						GUIUtility.keyboardControl = 0;
						EditorGUI.EndEditingActiveTextField();
						ScriptExecutionOrderInspector.DragReorderGUI.s_ReorderingDraggedElement = Mathf.FloorToInt((Event.current.mousePosition.y - position.y) / (float)elementHeight);
						if (!ScriptExecutionOrderInspector.DragReorderGUI.IsDefaultTimeElement(elements[ScriptExecutionOrderInspector.DragReorderGUI.s_ReorderingDraggedElement]))
						{
							ScriptExecutionOrderInspector.DragReorderGUI.s_ReorderingPositions = new float[elements.Count];
							ScriptExecutionOrderInspector.DragReorderGUI.s_ReorderingGoals = new int[elements.Count];
							for (int k = 0; k < elements.Count; k++)
							{
								ScriptExecutionOrderInspector.DragReorderGUI.s_ReorderingGoals[k] = k;
								ScriptExecutionOrderInspector.DragReorderGUI.s_ReorderingPositions[k] = (float)k;
							}
							GUIUtility.hotControl = controlID;
							Event.current.Use();
						}
					}
					break;
				case EventType.MouseUp:
					if (GUIUtility.hotControl == controlID)
					{
						if (ScriptExecutionOrderInspector.DragReorderGUI.s_ReorderingGoals[ScriptExecutionOrderInspector.DragReorderGUI.s_ReorderingDraggedElement] != ScriptExecutionOrderInspector.DragReorderGUI.s_ReorderingDraggedElement)
						{
							List<MonoScript> list = new List<MonoScript>(elements);
							for (int l = 0; l < elements.Count; l++)
							{
								elements[ScriptExecutionOrderInspector.DragReorderGUI.s_ReorderingGoals[l]] = list[l];
							}
							result = ScriptExecutionOrderInspector.DragReorderGUI.s_ReorderingGoals[ScriptExecutionOrderInspector.DragReorderGUI.s_ReorderingDraggedElement];
						}
						ScriptExecutionOrderInspector.DragReorderGUI.s_ReorderingGoals = null;
						ScriptExecutionOrderInspector.DragReorderGUI.s_ReorderingPositions = null;
						ScriptExecutionOrderInspector.DragReorderGUI.s_ReorderingDraggedElement = -1;
						GUIUtility.hotControl = 0;
						Event.current.Use();
					}
					break;
				case EventType.MouseDrag:
					if (GUIUtility.hotControl == controlID)
					{
						ScriptExecutionOrderInspector.DragReorderGUI.s_ReorderingPositions[ScriptExecutionOrderInspector.DragReorderGUI.s_ReorderingDraggedElement] = (Event.current.mousePosition.y - position.y) / (float)elementHeight - 0.5f;
						ScriptExecutionOrderInspector.DragReorderGUI.s_ReorderingPositions[ScriptExecutionOrderInspector.DragReorderGUI.s_ReorderingDraggedElement] = Mathf.Clamp(ScriptExecutionOrderInspector.DragReorderGUI.s_ReorderingPositions[ScriptExecutionOrderInspector.DragReorderGUI.s_ReorderingDraggedElement], 0f, (float)(elements.Count - 1));
						int num2 = Mathf.RoundToInt(ScriptExecutionOrderInspector.DragReorderGUI.s_ReorderingPositions[ScriptExecutionOrderInspector.DragReorderGUI.s_ReorderingDraggedElement]);
						if (num2 != ScriptExecutionOrderInspector.DragReorderGUI.s_ReorderingGoals[ScriptExecutionOrderInspector.DragReorderGUI.s_ReorderingDraggedElement])
						{
							for (int m = 0; m < elements.Count; m++)
							{
								ScriptExecutionOrderInspector.DragReorderGUI.s_ReorderingGoals[m] = m;
							}
							int num3 = (num2 <= ScriptExecutionOrderInspector.DragReorderGUI.s_ReorderingDraggedElement) ? 1 : -1;
							for (int num4 = ScriptExecutionOrderInspector.DragReorderGUI.s_ReorderingDraggedElement; num4 != num2; num4 -= num3)
							{
								ScriptExecutionOrderInspector.DragReorderGUI.s_ReorderingGoals[num4 - num3] = num4;
							}
							ScriptExecutionOrderInspector.DragReorderGUI.s_ReorderingGoals[ScriptExecutionOrderInspector.DragReorderGUI.s_ReorderingDraggedElement] = num2;
						}
						Event.current.Use();
					}
					break;
				case EventType.Repaint:
					if (GUIUtility.hotControl == controlID)
					{
						for (int n = 0; n < elements.Count; n++)
						{
							if (n != ScriptExecutionOrderInspector.DragReorderGUI.s_ReorderingDraggedElement)
							{
								ScriptExecutionOrderInspector.DragReorderGUI.s_ReorderingPositions[n] = Mathf.MoveTowards(ScriptExecutionOrderInspector.DragReorderGUI.s_ReorderingPositions[n], (float)ScriptExecutionOrderInspector.DragReorderGUI.s_ReorderingGoals[n], 0.075f);
							}
						}
						GUIView.current.Repaint();
					}
					break;
				}
				return result;
			}
		}

		private const int kOrderRangeMin = -32000;

		private const int kOrderRangeMax = 32000;

		private const int kListElementHeight = 21;

		private const int kIntFieldWidth = 50;

		private const int kPreferredSpacing = 100;

		private int[] kRoundingAmounts = new int[]
		{
			1000,
			500,
			100,
			50,
			10,
			5,
			1
		};

		private MonoScript m_Edited = null;

		private List<MonoScript> m_CustomTimeScripts;

		private List<MonoScript> m_DefaultTimeScripts;

		private static MonoScript sDummyScript;

		private Vector2 m_Scroll = Vector2.zero;

		private static readonly List<ScriptExecutionOrderInspector> m_Instances = new List<ScriptExecutionOrderInspector>();

		private MonoScript[] m_AllScripts;

		private int[] m_AllOrders;

		private bool m_DirtyOrders = false;

		private static int s_DropFieldHash = "DropField".GetHashCode();

		public static ScriptExecutionOrderInspector.Styles m_Styles;

		[CompilerGenerated]
		private static EditorGUI.ObjectFieldValidator <>f__mg$cache0;

		internal override string targetTitle
		{
			get
			{
				return "Script Execution Order";
			}
		}

		[MenuItem("CONTEXT/MonoManager/Reset")]
		private static void Reset(MenuCommand cmd)
		{
			List<ScriptExecutionOrderInspector> instances = ScriptExecutionOrderInspector.GetInstances();
			foreach (ScriptExecutionOrderInspector current in instances)
			{
				for (int i = 0; i < current.m_AllOrders.Length; i++)
				{
					current.m_AllOrders[i] = 0;
				}
				current.Apply();
			}
		}

		public void OnEnable()
		{
			if (ScriptExecutionOrderInspector.sDummyScript == null)
			{
				ScriptExecutionOrderInspector.sDummyScript = new MonoScript();
			}
			if (this.m_AllScripts == null || !this.m_DirtyOrders)
			{
				this.PopulateScriptArray();
			}
			if (!ScriptExecutionOrderInspector.m_Instances.Contains(this))
			{
				ScriptExecutionOrderInspector.m_Instances.Add(this);
			}
		}

		private static UnityEngine.Object MonoScriptValidatorCallback(UnityEngine.Object[] references, Type objType, SerializedProperty property)
		{
			UnityEngine.Object result;
			for (int i = 0; i < references.Length; i++)
			{
				UnityEngine.Object @object = references[i];
				MonoScript monoScript = @object as MonoScript;
				if (monoScript != null && ScriptExecutionOrderInspector.IsValidScript(monoScript))
				{
					result = monoScript;
					return result;
				}
			}
			result = null;
			return result;
		}

		private static bool IsValidScript(MonoScript script)
		{
			bool result;
			if (script == null)
			{
				result = false;
			}
			else if (script.GetClass() == null)
			{
				result = false;
			}
			else
			{
				bool flag = typeof(MonoBehaviour).IsAssignableFrom(script.GetClass());
				bool flag2 = typeof(ScriptableObject).IsAssignableFrom(script.GetClass());
				result = ((flag || flag2) && AssetDatabase.GetAssetPath(script).IndexOf("Assets/") == 0);
			}
			return result;
		}

		internal static List<ScriptExecutionOrderInspector> GetInstances()
		{
			return ScriptExecutionOrderInspector.m_Instances;
		}

		private void PopulateScriptArray()
		{
			this.m_AllScripts = MonoImporter.GetAllRuntimeMonoScripts();
			this.m_AllOrders = new int[this.m_AllScripts.Length];
			this.m_CustomTimeScripts = new List<MonoScript>();
			this.m_DefaultTimeScripts = new List<MonoScript>();
			for (int i = 0; i < this.m_AllScripts.Length; i++)
			{
				MonoScript monoScript = this.m_AllScripts[i];
				this.m_AllOrders[i] = MonoImporter.GetExecutionOrder(monoScript);
				if (ScriptExecutionOrderInspector.IsValidScript(monoScript))
				{
					if (this.GetExecutionOrder(monoScript) == 0)
					{
						this.m_DefaultTimeScripts.Add(monoScript);
					}
					else
					{
						this.m_CustomTimeScripts.Add(monoScript);
					}
				}
			}
			this.m_CustomTimeScripts.Add(ScriptExecutionOrderInspector.sDummyScript);
			this.m_CustomTimeScripts.Add(ScriptExecutionOrderInspector.sDummyScript);
			this.m_CustomTimeScripts.Sort(new ScriptExecutionOrderInspector.SortMonoScriptExecutionOrder(this));
			this.m_DefaultTimeScripts.Sort(new ScriptExecutionOrderInspector.SortMonoScriptNameOrder());
			this.m_Edited = null;
			this.m_DirtyOrders = false;
		}

		private int GetExecutionOrder(MonoScript script)
		{
			int num = Array.IndexOf<MonoScript>(this.m_AllScripts, script);
			int result;
			if (num >= 0)
			{
				result = this.m_AllOrders[num];
			}
			else
			{
				result = 0;
			}
			return result;
		}

		private void SetExecutionOrder(MonoScript script, int order)
		{
			int num = Array.IndexOf<MonoScript>(this.m_AllScripts, script);
			if (num >= 0)
			{
				this.m_AllOrders[num] = Mathf.Clamp(order, -32000, 32000);
				this.m_DirtyOrders = true;
			}
		}

		private void Apply()
		{
			List<int> list = new List<int>();
			List<MonoScript> list2 = new List<MonoScript>();
			for (int i = 0; i < this.m_AllScripts.Length; i++)
			{
				if (MonoImporter.GetExecutionOrder(this.m_AllScripts[i]) != this.m_AllOrders[i])
				{
					list.Add(i);
					list2.Add(this.m_AllScripts[i]);
				}
			}
			bool flag = true;
			if (Provider.enabled)
			{
				Task task = Provider.Checkout(list2.ToArray(), CheckoutMode.Meta);
				task.Wait();
				flag = task.success;
			}
			if (flag)
			{
				foreach (int current in list)
				{
					MonoImporter.SetExecutionOrder(this.m_AllScripts[current], this.m_AllOrders[current]);
				}
				this.PopulateScriptArray();
			}
			else
			{
				Debug.LogError("Could not checkout scrips in version control for changing script execution order");
			}
		}

		private void Revert()
		{
			this.PopulateScriptArray();
		}

		private void OnDestroy()
		{
			if (this.m_DirtyOrders)
			{
				if (EditorUtility.DisplayDialog("Unapplied execution order", "Unapplied script execution order", "Apply", "Revert"))
				{
					this.Apply();
				}
			}
			if (ScriptExecutionOrderInspector.m_Instances.Contains(this))
			{
				ScriptExecutionOrderInspector.m_Instances.Remove(this);
			}
		}

		private void ApplyRevertGUI()
		{
			EditorGUILayout.Space();
			bool enabled = GUI.enabled;
			GUI.enabled = this.m_DirtyOrders;
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.FlexibleSpace();
			if (GUILayout.Button("Revert", new GUILayoutOption[0]))
			{
				this.Revert();
			}
			if (GUILayout.Button("Apply", new GUILayoutOption[0]))
			{
				this.Apply();
			}
			GUILayout.EndHorizontal();
			GUI.enabled = enabled;
		}

		private void MenuSelection(object userData, string[] options, int selected)
		{
			this.AddScriptToCustomOrder(this.m_DefaultTimeScripts[selected]);
		}

		private void AddScriptToCustomOrder(MonoScript script)
		{
			if (ScriptExecutionOrderInspector.IsValidScript(script))
			{
				if (!this.m_CustomTimeScripts.Contains(script))
				{
					int order = this.RoundByAmount(this.GetExecutionOrderAtIndex(this.m_CustomTimeScripts.Count - 1) + 100, 100);
					this.SetExecutionOrder(script, order);
					this.m_CustomTimeScripts.Add(script);
					this.m_DefaultTimeScripts.Remove(script);
				}
			}
		}

		private void ShowScriptPopup(Rect r)
		{
			int count = this.m_DefaultTimeScripts.Count;
			string[] array = new string[count];
			bool[] array2 = new bool[count];
			for (int i = 0; i < count; i++)
			{
				array[i] = this.m_DefaultTimeScripts[i].GetClass().FullName;
				array2[i] = true;
			}
			EditorUtility.DisplayCustomMenu(r, array, array2, null, new EditorUtility.SelectMenuItemFunction(this.MenuSelection), null);
		}

		private int RoundBasedOnContext(int val, int lowerBound, int upperBound)
		{
			int num = Mathf.Max(0, (upperBound - lowerBound) / 6);
			lowerBound += num;
			upperBound -= num;
			int result;
			for (int i = 0; i < this.kRoundingAmounts.Length; i++)
			{
				int num2 = this.RoundByAmount(val, this.kRoundingAmounts[i]);
				if (num2 > lowerBound && num2 < upperBound)
				{
					result = num2;
					return result;
				}
			}
			result = val;
			return result;
		}

		private int RoundByAmount(int val, int rounding)
		{
			return Mathf.RoundToInt((float)val / (float)rounding) * rounding;
		}

		private int GetAverageRoundedAwayFromZero(int a, int b)
		{
			int result;
			if ((a + b) % 2 == 0)
			{
				result = (a + b) / 2;
			}
			else
			{
				result = (a + b + Math.Sign(a + b)) / 2;
			}
			return result;
		}

		private void SetExecutionOrderAtIndexAccordingToNeighbors(int indexOfChangedItem, int pushDirection)
		{
			if (indexOfChangedItem >= 0 && indexOfChangedItem < this.m_CustomTimeScripts.Count)
			{
				if (indexOfChangedItem == 0)
				{
					this.SetExecutionOrderAtIndex(indexOfChangedItem, this.RoundByAmount(this.GetExecutionOrderAtIndex(indexOfChangedItem + 1) - 100, 100));
				}
				else if (indexOfChangedItem == this.m_CustomTimeScripts.Count - 1)
				{
					this.SetExecutionOrderAtIndex(indexOfChangedItem, this.RoundByAmount(this.GetExecutionOrderAtIndex(indexOfChangedItem - 1) + 100, 100));
				}
				else
				{
					int executionOrderAtIndex = this.GetExecutionOrderAtIndex(indexOfChangedItem - 1);
					int executionOrderAtIndex2 = this.GetExecutionOrderAtIndex(indexOfChangedItem + 1);
					int num = this.RoundBasedOnContext(this.GetAverageRoundedAwayFromZero(executionOrderAtIndex, executionOrderAtIndex2), executionOrderAtIndex, executionOrderAtIndex2);
					if (num != 0)
					{
						if (pushDirection == 0)
						{
							pushDirection = this.GetBestPushDirectionForOrderValue(num);
						}
						if (pushDirection > 0)
						{
							num = Mathf.Max(num, executionOrderAtIndex + 1);
						}
						else
						{
							num = Mathf.Min(num, executionOrderAtIndex2 - 1);
						}
					}
					this.SetExecutionOrderAtIndex(indexOfChangedItem, num);
				}
			}
		}

		private void UpdateOrder(MonoScript changedScript)
		{
			this.m_CustomTimeScripts.Remove(changedScript);
			int executionOrder = this.GetExecutionOrder(changedScript);
			if (executionOrder == 0)
			{
				this.m_DefaultTimeScripts.Add(changedScript);
				this.m_DefaultTimeScripts.Sort(new ScriptExecutionOrderInspector.SortMonoScriptNameOrder());
			}
			else
			{
				int num = -1;
				for (int i = 0; i < this.m_CustomTimeScripts.Count; i++)
				{
					if (this.GetExecutionOrderAtIndex(i) == executionOrder)
					{
						num = i;
						break;
					}
				}
				if (num == -1)
				{
					this.m_CustomTimeScripts.Add(changedScript);
					this.m_CustomTimeScripts.Sort(new ScriptExecutionOrderInspector.SortMonoScriptExecutionOrder(this));
				}
				else
				{
					int bestPushDirectionForOrderValue = this.GetBestPushDirectionForOrderValue(executionOrder);
					if (bestPushDirectionForOrderValue == 1)
					{
						this.m_CustomTimeScripts.Insert(num, changedScript);
						num++;
					}
					else
					{
						this.m_CustomTimeScripts.Insert(num + 1, changedScript);
					}
					this.PushAwayToAvoidConflicts(num, bestPushDirectionForOrderValue);
				}
			}
		}

		private void PushAwayToAvoidConflicts(int startIndex, int pushDirection)
		{
			int num = startIndex;
			while (num >= 0 && num < this.m_CustomTimeScripts.Count)
			{
				if ((this.GetExecutionOrderAtIndex(num) - this.GetExecutionOrderAtIndex(num - pushDirection)) * pushDirection >= 1)
				{
					break;
				}
				this.SetExecutionOrderAtIndexAccordingToNeighbors(num, pushDirection);
				num += pushDirection;
			}
		}

		private int GetBestPushDirectionForOrderValue(int order)
		{
			int num = (int)Mathf.Sign((float)order);
			if (order < -16000 || order > 16000)
			{
				num = -num;
			}
			return num;
		}

		public override bool UseDefaultMargins()
		{
			return false;
		}

		public override void OnInspectorGUI()
		{
			if (ScriptExecutionOrderInspector.m_Styles == null)
			{
				ScriptExecutionOrderInspector.m_Styles = new ScriptExecutionOrderInspector.Styles();
			}
			if (this.m_Edited)
			{
				this.UpdateOrder(this.m_Edited);
				this.m_Edited = null;
			}
			EditorGUILayout.BeginVertical(EditorStyles.inspectorFullWidthMargins, new GUILayoutOption[0]);
			GUILayout.Label(ScriptExecutionOrderInspector.m_Styles.helpText, EditorStyles.helpBox, new GUILayoutOption[0]);
			EditorGUILayout.Space();
			Rect rect = EditorGUILayout.BeginVertical(new GUILayoutOption[0]);
			int controlID = GUIUtility.GetControlID(ScriptExecutionOrderInspector.s_DropFieldHash, FocusType.Passive, rect);
			Rect arg_B9_0 = rect;
			int arg_B9_1 = controlID;
			Type arg_B9_2 = typeof(MonoScript);
			if (ScriptExecutionOrderInspector.<>f__mg$cache0 == null)
			{
				ScriptExecutionOrderInspector.<>f__mg$cache0 = new EditorGUI.ObjectFieldValidator(ScriptExecutionOrderInspector.MonoScriptValidatorCallback);
			}
			MonoScript monoScript = EditorGUI.DoDropField(arg_B9_0, arg_B9_1, arg_B9_2, ScriptExecutionOrderInspector.<>f__mg$cache0, false, ScriptExecutionOrderInspector.m_Styles.dropField) as MonoScript;
			if (monoScript)
			{
				this.AddScriptToCustomOrder(monoScript);
			}
			EditorGUILayout.BeginVertical(ScriptExecutionOrderInspector.m_Styles.boxBackground, new GUILayoutOption[0]);
			this.m_Scroll = EditorGUILayout.BeginVerticalScrollView(this.m_Scroll, new GUILayoutOption[0]);
			Rect rect2 = GUILayoutUtility.GetRect(10f, (float)(21 * this.m_CustomTimeScripts.Count), new GUILayoutOption[]
			{
				GUILayout.ExpandWidth(true)
			});
			int num = ScriptExecutionOrderInspector.DragReorderGUI.DragReorder(rect2, 21, this.m_CustomTimeScripts, new ScriptExecutionOrderInspector.DragReorderGUI.DrawElementDelegate(this.DrawElement));
			if (num >= 0)
			{
				this.SetExecutionOrderAtIndexAccordingToNeighbors(num, 0);
				this.UpdateOrder(this.m_CustomTimeScripts[num]);
				this.SetExecutionOrderAtIndexAccordingToNeighbors(num, 0);
			}
			EditorGUILayout.EndScrollView();
			EditorGUILayout.EndVertical();
			GUILayout.BeginHorizontal(ScriptExecutionOrderInspector.m_Styles.toolbar, new GUILayoutOption[0]);
			GUILayout.FlexibleSpace();
			GUIContent iconToolbarPlus = ScriptExecutionOrderInspector.m_Styles.iconToolbarPlus;
			Rect rect3 = GUILayoutUtility.GetRect(iconToolbarPlus, ScriptExecutionOrderInspector.m_Styles.toolbarDropDown);
			if (EditorGUI.DropdownButton(rect3, iconToolbarPlus, FocusType.Passive, ScriptExecutionOrderInspector.m_Styles.toolbarDropDown))
			{
				this.ShowScriptPopup(rect3);
			}
			GUILayout.EndHorizontal();
			GUILayout.EndVertical();
			this.ApplyRevertGUI();
			GUILayout.EndVertical();
			GUILayout.FlexibleSpace();
		}

		private int GetExecutionOrderAtIndex(int idx)
		{
			return this.GetExecutionOrder(this.m_CustomTimeScripts[idx]);
		}

		private void SetExecutionOrderAtIndex(int idx, int order)
		{
			this.SetExecutionOrder(this.m_CustomTimeScripts[idx], order);
		}

		private Rect GetButtonLabelRect(Rect r)
		{
			return new Rect(r.x + 20f, r.y + 1f, r.width - this.GetMinusButtonSize().x - 10f - 20f - 55f, r.height);
		}

		private Rect GetAddRemoveButtonRect(Rect r)
		{
			Vector2 minusButtonSize = this.GetMinusButtonSize();
			return new Rect(r.xMax - minusButtonSize.x - 5f, r.y + 1f, minusButtonSize.x, minusButtonSize.y);
		}

		private Rect GetFieldRect(Rect r)
		{
			return new Rect(r.xMax - 50f - this.GetMinusButtonSize().x - 10f, r.y + 2f, 50f, r.height - 5f);
		}

		private Vector2 GetMinusButtonSize()
		{
			return ScriptExecutionOrderInspector.m_Styles.removeButton.CalcSize(ScriptExecutionOrderInspector.m_Styles.iconToolbarMinus);
		}

		private Rect GetDraggingHandleRect(Rect r)
		{
			return new Rect(r.x + 5f, r.y + 7f, 10f, r.height - 14f);
		}

		public void DrawElement(Rect r, object obj, bool dragging)
		{
			MonoScript monoScript = obj as MonoScript;
			if (Event.current.type == EventType.Repaint)
			{
				ScriptExecutionOrderInspector.m_Styles.elementBackground.Draw(r, false, false, false, false);
				ScriptExecutionOrderInspector.m_Styles.draggingHandle.Draw(this.GetDraggingHandleRect(r), false, false, false, false);
			}
			GUI.Label(this.GetButtonLabelRect(r), monoScript.GetClass().FullName);
			int executionOrder = this.GetExecutionOrder(monoScript);
			Rect fieldRect = this.GetFieldRect(r);
			int controlID = GUIUtility.GetControlID(monoScript.GetHashCode(), FocusType.Keyboard, fieldRect);
			string s = EditorGUI.DelayedTextFieldInternal(fieldRect, controlID, GUIContent.none, executionOrder.ToString(), "0123456789-", EditorStyles.textField);
			int num = executionOrder;
			if (int.TryParse(s, out num) && num != executionOrder)
			{
				this.SetExecutionOrder(monoScript, num);
				this.m_Edited = monoScript;
			}
			if (GUI.Button(this.GetAddRemoveButtonRect(r), ScriptExecutionOrderInspector.m_Styles.iconToolbarMinus, ScriptExecutionOrderInspector.m_Styles.removeButton))
			{
				this.SetExecutionOrder(monoScript, 0);
				this.m_Edited = monoScript;
			}
		}
	}
}
