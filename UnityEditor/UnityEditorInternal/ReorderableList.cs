using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UnityEditorInternal
{
	public class ReorderableList
	{
		public delegate void HeaderCallbackDelegate(Rect rect);

		public delegate void FooterCallbackDelegate(Rect rect);

		public delegate void ElementCallbackDelegate(Rect rect, int index, bool isActive, bool isFocused);

		public delegate float ElementHeightCallbackDelegate(int index);

		public delegate void ReorderCallbackDelegate(ReorderableList list);

		public delegate void SelectCallbackDelegate(ReorderableList list);

		public delegate void AddCallbackDelegate(ReorderableList list);

		public delegate void AddDropdownCallbackDelegate(Rect buttonRect, ReorderableList list);

		public delegate void RemoveCallbackDelegate(ReorderableList list);

		public delegate void ChangedCallbackDelegate(ReorderableList list);

		public delegate bool CanRemoveCallbackDelegate(ReorderableList list);

		public class Defaults
		{
			public GUIContent iconToolbarPlus = EditorGUIUtility.IconContent("Toolbar Plus", "|Add to list");

			public GUIContent iconToolbarPlusMore = EditorGUIUtility.IconContent("Toolbar Plus More", "|Choose to add to list");

			public GUIContent iconToolbarMinus = EditorGUIUtility.IconContent("Toolbar Minus", "|Remove selection from list");

			public readonly GUIStyle draggingHandle = "RL DragHandle";

			public readonly GUIStyle headerBackground = "RL Header";

			public readonly GUIStyle footerBackground = "RL Footer";

			public readonly GUIStyle boxBackground = "RL Background";

			public readonly GUIStyle preButton = "RL FooterButton";

			public GUIStyle elementBackground = new GUIStyle("RL Element");

			private const int buttonWidth = 25;

			public const int padding = 6;

			public const int dragHandleWidth = 20;

			public void DrawFooter(Rect rect, ReorderableList list)
			{
				float xMax = rect.xMax;
				float num = xMax - 8f;
				if (list.displayAdd)
				{
					num -= 25f;
				}
				if (list.displayRemove)
				{
					num -= 25f;
				}
				rect = new Rect(num, rect.y, xMax - num, rect.height);
				Rect rect2 = new Rect(num + 4f, rect.y - 3f, 25f, 13f);
				Rect position = new Rect(xMax - 29f, rect.y - 3f, 25f, 13f);
				if (Event.current.type == EventType.Repaint)
				{
					this.footerBackground.Draw(rect, false, false, false, false);
				}
				if (list.displayAdd)
				{
					if (GUI.Button(rect2, (list.onAddDropdownCallback == null) ? this.iconToolbarPlus : this.iconToolbarPlusMore, this.preButton))
					{
						if (list.onAddDropdownCallback != null)
						{
							list.onAddDropdownCallback(rect2, list);
						}
						else if (list.onAddCallback != null)
						{
							list.onAddCallback(list);
						}
						else
						{
							this.DoAddButton(list);
						}
						if (list.onChangedCallback != null)
						{
							list.onChangedCallback(list);
						}
					}
				}
				if (list.displayRemove)
				{
					using (new EditorGUI.DisabledScope(list.index < 0 || list.index >= list.count || (list.onCanRemoveCallback != null && !list.onCanRemoveCallback(list))))
					{
						if (GUI.Button(position, this.iconToolbarMinus, this.preButton))
						{
							if (list.onRemoveCallback == null)
							{
								this.DoRemoveButton(list);
							}
							else
							{
								list.onRemoveCallback(list);
							}
							if (list.onChangedCallback != null)
							{
								list.onChangedCallback(list);
							}
						}
					}
				}
			}

			public void DoAddButton(ReorderableList list)
			{
				if (list.serializedProperty != null)
				{
					list.serializedProperty.arraySize++;
					list.index = list.serializedProperty.arraySize - 1;
				}
				else
				{
					Type elementType = list.list.GetType().GetElementType();
					if (elementType == typeof(string))
					{
						list.index = list.list.Add("");
					}
					else if (elementType != null && elementType.GetConstructor(Type.EmptyTypes) == null)
					{
						Debug.LogError("Cannot add element. Type " + elementType.ToString() + " has no default constructor. Implement a default constructor or implement your own add behaviour.");
					}
					else if (list.list.GetType().GetGenericArguments()[0] != null)
					{
						list.index = list.list.Add(Activator.CreateInstance(list.list.GetType().GetGenericArguments()[0]));
					}
					else if (elementType != null)
					{
						list.index = list.list.Add(Activator.CreateInstance(elementType));
					}
					else
					{
						Debug.LogError("Cannot add element of type Null.");
					}
				}
			}

			public void DoRemoveButton(ReorderableList list)
			{
				if (list.serializedProperty != null)
				{
					list.serializedProperty.DeleteArrayElementAtIndex(list.index);
					if (list.index >= list.serializedProperty.arraySize - 1)
					{
						list.index = list.serializedProperty.arraySize - 1;
					}
				}
				else
				{
					list.list.RemoveAt(list.index);
					if (list.index >= list.list.Count - 1)
					{
						list.index = list.list.Count - 1;
					}
				}
			}

			public void DrawHeaderBackground(Rect headerRect)
			{
				if (Event.current.type == EventType.Repaint)
				{
					this.headerBackground.Draw(headerRect, false, false, false, false);
				}
			}

			public void DrawHeader(Rect headerRect, SerializedObject serializedObject, SerializedProperty element, IList elementList)
			{
				EditorGUI.LabelField(headerRect, EditorGUIUtility.TempContent((element == null) ? "IList" : "Serialized Property"));
			}

			public void DrawElementBackground(Rect rect, int index, bool selected, bool focused, bool draggable)
			{
				if (Event.current.type == EventType.Repaint)
				{
					this.elementBackground.Draw(rect, false, selected, selected, focused);
				}
			}

			public void DrawElementDraggingHandle(Rect rect, int index, bool selected, bool focused, bool draggable)
			{
				if (Event.current.type == EventType.Repaint)
				{
					if (draggable)
					{
						this.draggingHandle.Draw(new Rect(rect.x + 5f, rect.y + 7f, 10f, rect.height - (rect.height - 7f)), false, false, false, false);
					}
				}
			}

			public void DrawElement(Rect rect, SerializedProperty element, object listItem, bool selected, bool focused, bool draggable)
			{
				EditorGUI.LabelField(rect, EditorGUIUtility.TempContent((element == null) ? listItem.ToString() : element.displayName));
			}

			public void DrawNoneElement(Rect rect, bool draggable)
			{
				EditorGUI.LabelField(rect, EditorGUIUtility.TempContent("List is Empty"));
			}
		}

		public ReorderableList.HeaderCallbackDelegate drawHeaderCallback;

		public ReorderableList.FooterCallbackDelegate drawFooterCallback = null;

		public ReorderableList.ElementCallbackDelegate drawElementCallback;

		public ReorderableList.ElementCallbackDelegate drawElementBackgroundCallback;

		public ReorderableList.ElementHeightCallbackDelegate elementHeightCallback;

		public ReorderableList.ReorderCallbackDelegate onReorderCallback;

		public ReorderableList.SelectCallbackDelegate onSelectCallback;

		public ReorderableList.AddCallbackDelegate onAddCallback;

		public ReorderableList.AddDropdownCallbackDelegate onAddDropdownCallback;

		public ReorderableList.RemoveCallbackDelegate onRemoveCallback;

		public ReorderableList.SelectCallbackDelegate onMouseUpCallback;

		public ReorderableList.CanRemoveCallbackDelegate onCanRemoveCallback;

		public ReorderableList.ChangedCallbackDelegate onChangedCallback;

		private int m_ActiveElement = -1;

		private float m_DragOffset = 0f;

		private GUISlideGroup m_SlideGroup;

		private SerializedObject m_SerializedObject;

		private SerializedProperty m_Elements;

		private IList m_ElementList;

		private bool m_Draggable;

		private float m_DraggedY;

		private bool m_Dragging;

		private List<int> m_NonDragTargetIndices;

		private bool m_DisplayHeader;

		public bool displayAdd;

		public bool displayRemove;

		private int id = -1;

		private static ReorderableList.Defaults s_Defaults;

		public float elementHeight = 21f;

		public float headerHeight = 18f;

		public float footerHeight = 13f;

		public bool showDefaultBackground = true;

		public static ReorderableList.Defaults defaultBehaviours
		{
			get
			{
				return ReorderableList.s_Defaults;
			}
		}

		public SerializedProperty serializedProperty
		{
			get
			{
				return this.m_Elements;
			}
			set
			{
				this.m_Elements = value;
			}
		}

		public IList list
		{
			get
			{
				return this.m_ElementList;
			}
			set
			{
				this.m_ElementList = value;
			}
		}

		public int index
		{
			get
			{
				return this.m_ActiveElement;
			}
			set
			{
				this.m_ActiveElement = value;
			}
		}

		public bool draggable
		{
			get
			{
				return this.m_Draggable;
			}
			set
			{
				this.m_Draggable = value;
			}
		}

		public int count
		{
			get
			{
				int result;
				if (this.m_Elements != null)
				{
					if (!this.m_Elements.hasMultipleDifferentValues)
					{
						result = this.m_Elements.arraySize;
					}
					else
					{
						int num = this.m_Elements.arraySize;
						UnityEngine.Object[] targetObjects = this.m_Elements.serializedObject.targetObjects;
						for (int i = 0; i < targetObjects.Length; i++)
						{
							UnityEngine.Object obj = targetObjects[i];
							SerializedObject serializedObject = new SerializedObject(obj);
							SerializedProperty serializedProperty = serializedObject.FindProperty(this.m_Elements.propertyPath);
							num = Math.Min(serializedProperty.arraySize, num);
						}
						result = num;
					}
				}
				else
				{
					result = this.m_ElementList.Count;
				}
				return result;
			}
		}

		public ReorderableList(IList elements, Type elementType)
		{
			this.InitList(null, null, elements, true, true, true, true);
		}

		public ReorderableList(IList elements, Type elementType, bool draggable, bool displayHeader, bool displayAddButton, bool displayRemoveButton)
		{
			this.InitList(null, null, elements, draggable, displayHeader, displayAddButton, displayRemoveButton);
		}

		public ReorderableList(SerializedObject serializedObject, SerializedProperty elements)
		{
			this.InitList(serializedObject, elements, null, true, true, true, true);
		}

		public ReorderableList(SerializedObject serializedObject, SerializedProperty elements, bool draggable, bool displayHeader, bool displayAddButton, bool displayRemoveButton)
		{
			this.InitList(serializedObject, elements, null, draggable, displayHeader, displayAddButton, displayRemoveButton);
		}

		private void InitList(SerializedObject serializedObject, SerializedProperty elements, IList elementList, bool draggable, bool displayHeader, bool displayAddButton, bool displayRemoveButton)
		{
			this.id = GUIUtility.GetPermanentControlID();
			this.m_SerializedObject = serializedObject;
			this.m_Elements = elements;
			this.m_ElementList = elementList;
			this.m_Draggable = draggable;
			this.m_Dragging = false;
			this.m_SlideGroup = new GUISlideGroup();
			this.displayAdd = displayAddButton;
			this.m_DisplayHeader = displayHeader;
			this.displayRemove = displayRemoveButton;
			if (this.m_Elements != null && !this.m_Elements.editable)
			{
				this.m_Draggable = false;
			}
			if (this.m_Elements != null && !this.m_Elements.isArray)
			{
				Debug.LogError("Input elements should be an Array SerializedProperty");
			}
		}

		private Rect GetContentRect(Rect rect)
		{
			Rect result = rect;
			if (this.draggable)
			{
				result.xMin += 20f;
			}
			else
			{
				result.xMin += 6f;
			}
			result.xMax -= 6f;
			return result;
		}

		private float GetElementYOffset(int index)
		{
			return this.GetElementYOffset(index, -1);
		}

		private float GetElementYOffset(int index, int skipIndex)
		{
			float result;
			if (this.elementHeightCallback == null)
			{
				result = (float)index * this.elementHeight;
			}
			else
			{
				float num = 0f;
				for (int i = 0; i < index; i++)
				{
					if (i != skipIndex)
					{
						num += this.elementHeightCallback(i);
					}
				}
				result = num;
			}
			return result;
		}

		private float GetElementHeight(int index)
		{
			float result;
			if (this.elementHeightCallback == null)
			{
				result = this.elementHeight;
			}
			else
			{
				result = this.elementHeightCallback(index);
			}
			return result;
		}

		private Rect GetRowRect(int index, Rect listRect)
		{
			return new Rect(listRect.x, listRect.y + this.GetElementYOffset(index), listRect.width, this.GetElementHeight(index));
		}

		public void DoLayoutList()
		{
			if (ReorderableList.s_Defaults == null)
			{
				ReorderableList.s_Defaults = new ReorderableList.Defaults();
			}
			Rect rect = GUILayoutUtility.GetRect(0f, this.headerHeight, new GUILayoutOption[]
			{
				GUILayout.ExpandWidth(true)
			});
			Rect rect2 = GUILayoutUtility.GetRect(10f, this.GetListElementHeight(), new GUILayoutOption[]
			{
				GUILayout.ExpandWidth(true)
			});
			Rect rect3 = GUILayoutUtility.GetRect(4f, this.footerHeight, new GUILayoutOption[]
			{
				GUILayout.ExpandWidth(true)
			});
			this.DoListHeader(rect);
			this.DoListElements(rect2);
			this.DoListFooter(rect3);
		}

		public void DoList(Rect rect)
		{
			if (ReorderableList.s_Defaults == null)
			{
				ReorderableList.s_Defaults = new ReorderableList.Defaults();
			}
			Rect headerRect = new Rect(rect.x, rect.y, rect.width, this.headerHeight);
			Rect listRect = new Rect(rect.x, headerRect.y + headerRect.height, rect.width, this.GetListElementHeight());
			Rect footerRect = new Rect(rect.x, listRect.y + listRect.height, rect.width, this.footerHeight);
			this.DoListHeader(headerRect);
			this.DoListElements(listRect);
			this.DoListFooter(footerRect);
		}

		public float GetHeight()
		{
			float num = 0f;
			num += this.GetListElementHeight();
			num += this.headerHeight;
			return num + this.footerHeight;
		}

		private float GetListElementHeight()
		{
			int count = this.count;
			float result;
			if (count == 0)
			{
				result = this.elementHeight + 7f;
			}
			else if (this.elementHeightCallback != null)
			{
				result = this.GetElementYOffset(count - 1) + this.GetElementHeight(count - 1) + 7f;
			}
			else
			{
				result = this.elementHeight * (float)count + 7f;
			}
			return result;
		}

		private void DoListElements(Rect listRect)
		{
			int count = this.count;
			if (this.showDefaultBackground && Event.current.type == EventType.Repaint)
			{
				ReorderableList.s_Defaults.boxBackground.Draw(listRect, false, false, false, false);
			}
			listRect.yMin += 2f;
			listRect.yMax -= 5f;
			Rect rect = listRect;
			rect.height = this.elementHeight;
			Rect rect2 = rect;
			if (((this.m_Elements != null && this.m_Elements.isArray) || this.m_ElementList != null) && count > 0)
			{
				if (this.IsDragging() && Event.current.type == EventType.Repaint)
				{
					int index = this.CalculateRowIndex();
					this.m_NonDragTargetIndices.Clear();
					for (int i = 0; i < count; i++)
					{
						if (i != this.m_ActiveElement)
						{
							this.m_NonDragTargetIndices.Add(i);
						}
					}
					this.m_NonDragTargetIndices.Insert(index, -1);
					bool flag = false;
					for (int j = 0; j < this.m_NonDragTargetIndices.Count; j++)
					{
						if (this.m_NonDragTargetIndices[j] != -1)
						{
							if (this.elementHeightCallback == null)
							{
								rect.y = listRect.y + this.GetElementYOffset(j, this.m_ActiveElement);
							}
							else
							{
								rect.y = listRect.y + this.GetElementYOffset(this.m_NonDragTargetIndices[j], this.m_ActiveElement);
								if (flag)
								{
									rect.y += this.elementHeightCallback(this.m_ActiveElement);
								}
							}
							rect = this.m_SlideGroup.GetRect(this.m_NonDragTargetIndices[j], rect);
							if (this.drawElementBackgroundCallback == null)
							{
								ReorderableList.s_Defaults.DrawElementBackground(rect, j, false, false, this.m_Draggable);
							}
							else
							{
								this.drawElementBackgroundCallback(rect, j, false, false);
							}
							ReorderableList.s_Defaults.DrawElementDraggingHandle(rect, j, false, false, this.m_Draggable);
							rect2 = this.GetContentRect(rect);
							if (this.drawElementCallback == null)
							{
								if (this.m_Elements != null)
								{
									ReorderableList.s_Defaults.DrawElement(rect2, this.m_Elements.GetArrayElementAtIndex(this.m_NonDragTargetIndices[j]), null, false, false, this.m_Draggable);
								}
								else
								{
									ReorderableList.s_Defaults.DrawElement(rect2, null, this.m_ElementList[this.m_NonDragTargetIndices[j]], false, false, this.m_Draggable);
								}
							}
							else
							{
								this.drawElementCallback(rect2, this.m_NonDragTargetIndices[j], false, false);
							}
						}
						else
						{
							flag = true;
						}
					}
					rect.y = this.m_DraggedY - this.m_DragOffset + listRect.y;
					if (this.drawElementBackgroundCallback == null)
					{
						ReorderableList.s_Defaults.DrawElementBackground(rect, this.m_ActiveElement, true, true, this.m_Draggable);
					}
					else
					{
						this.drawElementBackgroundCallback(rect, this.m_ActiveElement, true, true);
					}
					ReorderableList.s_Defaults.DrawElementDraggingHandle(rect, this.m_ActiveElement, true, true, this.m_Draggable);
					rect2 = this.GetContentRect(rect);
					if (this.drawElementCallback == null)
					{
						if (this.m_Elements != null)
						{
							ReorderableList.s_Defaults.DrawElement(rect2, this.m_Elements.GetArrayElementAtIndex(this.m_ActiveElement), null, true, true, this.m_Draggable);
						}
						else
						{
							ReorderableList.s_Defaults.DrawElement(rect2, null, this.m_ElementList[this.m_ActiveElement], true, true, this.m_Draggable);
						}
					}
					else
					{
						this.drawElementCallback(rect2, this.m_ActiveElement, true, true);
					}
				}
				else
				{
					for (int k = 0; k < count; k++)
					{
						bool flag2 = k == this.m_ActiveElement;
						bool flag3 = k == this.m_ActiveElement && this.HasKeyboardControl();
						rect.y = listRect.y + this.GetElementYOffset(k);
						if (this.drawElementBackgroundCallback == null)
						{
							ReorderableList.s_Defaults.DrawElementBackground(rect, k, flag2, flag3, this.m_Draggable);
						}
						else
						{
							this.drawElementBackgroundCallback(rect, k, flag2, flag3);
						}
						ReorderableList.s_Defaults.DrawElementDraggingHandle(rect, k, flag2, flag3, this.m_Draggable);
						rect2 = this.GetContentRect(rect);
						if (this.drawElementCallback == null)
						{
							if (this.m_Elements != null)
							{
								ReorderableList.s_Defaults.DrawElement(rect2, this.m_Elements.GetArrayElementAtIndex(k), null, flag2, flag3, this.m_Draggable);
							}
							else
							{
								ReorderableList.s_Defaults.DrawElement(rect2, null, this.m_ElementList[k], flag2, flag3, this.m_Draggable);
							}
						}
						else
						{
							this.drawElementCallback(rect2, k, flag2, flag3);
						}
					}
				}
				this.DoDraggingAndSelection(listRect);
			}
			else
			{
				rect.y = listRect.y;
				if (this.drawElementBackgroundCallback == null)
				{
					ReorderableList.s_Defaults.DrawElementBackground(rect, -1, false, false, false);
				}
				else
				{
					this.drawElementBackgroundCallback(rect, -1, false, false);
				}
				ReorderableList.s_Defaults.DrawElementDraggingHandle(rect, -1, false, false, false);
				rect2 = rect;
				rect2.xMin += 6f;
				rect2.xMax -= 6f;
				ReorderableList.s_Defaults.DrawNoneElement(rect2, this.m_Draggable);
			}
		}

		private void DoListHeader(Rect headerRect)
		{
			if (this.showDefaultBackground && Event.current.type == EventType.Repaint)
			{
				ReorderableList.s_Defaults.DrawHeaderBackground(headerRect);
			}
			headerRect.xMin += 6f;
			headerRect.xMax -= 6f;
			headerRect.height -= 2f;
			headerRect.y += 1f;
			if (this.drawHeaderCallback != null)
			{
				this.drawHeaderCallback(headerRect);
			}
			else if (this.m_DisplayHeader)
			{
				ReorderableList.s_Defaults.DrawHeader(headerRect, this.m_SerializedObject, this.m_Elements, this.m_ElementList);
			}
		}

		private void DoListFooter(Rect footerRect)
		{
			if (this.drawFooterCallback != null)
			{
				this.drawFooterCallback(footerRect);
			}
			else if (this.displayAdd || this.displayRemove)
			{
				ReorderableList.s_Defaults.DrawFooter(footerRect, this);
			}
		}

		private void DoDraggingAndSelection(Rect listRect)
		{
			Event current = Event.current;
			int activeElement = this.m_ActiveElement;
			bool flag = false;
			switch (current.GetTypeForControl(this.id))
			{
			case EventType.MouseDown:
				if (listRect.Contains(Event.current.mousePosition) && Event.current.button == 0)
				{
					EditorGUI.EndEditingActiveTextField();
					this.m_ActiveElement = this.GetRowIndex(Event.current.mousePosition.y - listRect.y);
					if (this.m_Draggable)
					{
						this.m_DragOffset = Event.current.mousePosition.y - listRect.y - this.GetElementYOffset(this.m_ActiveElement);
						this.UpdateDraggedY(listRect);
						GUIUtility.hotControl = this.id;
						this.m_SlideGroup.Reset();
						this.m_NonDragTargetIndices = new List<int>();
					}
					this.GrabKeyboardFocus();
					current.Use();
					flag = true;
				}
				break;
			case EventType.MouseUp:
				if (!this.m_Draggable)
				{
					if (this.onMouseUpCallback != null && this.IsMouseInsideActiveElement(listRect))
					{
						this.onMouseUpCallback(this);
					}
				}
				else if (GUIUtility.hotControl == this.id)
				{
					current.Use();
					this.m_Dragging = false;
					try
					{
						int num = this.CalculateRowIndex();
						if (this.m_ActiveElement != num)
						{
							if (this.m_SerializedObject != null && this.m_Elements != null)
							{
								this.m_Elements.MoveArrayElement(this.m_ActiveElement, num);
								this.m_SerializedObject.ApplyModifiedProperties();
								this.m_SerializedObject.Update();
							}
							else if (this.m_ElementList != null)
							{
								object value = this.m_ElementList[this.m_ActiveElement];
								for (int i = 0; i < this.m_ElementList.Count - 1; i++)
								{
									if (i >= this.m_ActiveElement)
									{
										this.m_ElementList[i] = this.m_ElementList[i + 1];
									}
								}
								for (int j = this.m_ElementList.Count - 1; j > 0; j--)
								{
									if (j > num)
									{
										this.m_ElementList[j] = this.m_ElementList[j - 1];
									}
								}
								this.m_ElementList[num] = value;
							}
							this.m_ActiveElement = num;
							if (this.onReorderCallback != null)
							{
								this.onReorderCallback(this);
							}
							if (this.onChangedCallback != null)
							{
								this.onChangedCallback(this);
							}
						}
						else if (this.onMouseUpCallback != null)
						{
							this.onMouseUpCallback(this);
						}
					}
					finally
					{
						GUIUtility.hotControl = 0;
						this.m_NonDragTargetIndices = null;
					}
				}
				break;
			case EventType.MouseDrag:
				if (this.m_Draggable && GUIUtility.hotControl == this.id)
				{
					this.m_Dragging = true;
					this.UpdateDraggedY(listRect);
					current.Use();
				}
				break;
			case EventType.KeyDown:
				if (GUIUtility.keyboardControl != this.id)
				{
					return;
				}
				if (current.keyCode == KeyCode.DownArrow)
				{
					this.m_ActiveElement++;
					current.Use();
				}
				if (current.keyCode == KeyCode.UpArrow)
				{
					this.m_ActiveElement--;
					current.Use();
				}
				if (current.keyCode == KeyCode.Escape && GUIUtility.hotControl == this.id)
				{
					GUIUtility.hotControl = 0;
					this.m_Dragging = false;
					current.Use();
				}
				this.m_ActiveElement = Mathf.Clamp(this.m_ActiveElement, 0, (this.m_Elements == null) ? (this.m_ElementList.Count - 1) : (this.m_Elements.arraySize - 1));
				break;
			}
			if ((this.m_ActiveElement != activeElement || flag) && this.onSelectCallback != null)
			{
				this.onSelectCallback(this);
			}
		}

		private bool IsMouseInsideActiveElement(Rect listRect)
		{
			int rowIndex = this.GetRowIndex(Event.current.mousePosition.y - listRect.y);
			return rowIndex == this.m_ActiveElement && this.GetRowRect(rowIndex, listRect).Contains(Event.current.mousePosition);
		}

		private void UpdateDraggedY(Rect listRect)
		{
			this.m_DraggedY = Mathf.Clamp(Event.current.mousePosition.y - listRect.y, this.m_DragOffset, listRect.height - (this.GetElementHeight(this.m_ActiveElement) - this.m_DragOffset));
		}

		private int CalculateRowIndex()
		{
			return this.GetRowIndex(this.m_DraggedY);
		}

		private int GetRowIndex(float localY)
		{
			int result;
			if (this.elementHeightCallback == null)
			{
				result = Mathf.Clamp(Mathf.FloorToInt(localY / this.elementHeight), 0, this.count - 1);
			}
			else
			{
				float num = 0f;
				for (int i = 0; i < this.count; i++)
				{
					float num2 = this.elementHeightCallback(i);
					float num3 = num + num2;
					if (localY >= num && localY < num3)
					{
						result = i;
						return result;
					}
					num += num2;
				}
				result = this.count - 1;
			}
			return result;
		}

		private bool IsDragging()
		{
			return this.m_Dragging;
		}

		public void GrabKeyboardFocus()
		{
			GUIUtility.keyboardControl = this.id;
		}

		public void ReleaseKeyboardFocus()
		{
			if (GUIUtility.keyboardControl == this.id)
			{
				GUIUtility.keyboardControl = 0;
			}
		}

		public bool HasKeyboardControl()
		{
			return GUIUtility.keyboardControl == this.id;
		}
	}
}
