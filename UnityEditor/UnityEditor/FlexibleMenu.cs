using System;
using UnityEngine;

namespace UnityEditor
{
	internal class FlexibleMenu : PopupWindowContent
	{
		private class Styles
		{
			public GUIStyle menuItem = "MenuItem";

			public GUIContent plusButtonText = new GUIContent(string.Empty, "Add New Item");
		}

		internal static class ItemContextMenu
		{
			private static FlexibleMenu s_Caller;

			public static void Show(int itemIndex, FlexibleMenu caller)
			{
				FlexibleMenu.ItemContextMenu.s_Caller = caller;
				GenericMenu genericMenu = new GenericMenu();
				genericMenu.AddItem(new GUIContent("Edit..."), false, new GenericMenu.MenuFunction2(FlexibleMenu.ItemContextMenu.Edit), itemIndex);
				genericMenu.AddItem(new GUIContent("Delete"), false, new GenericMenu.MenuFunction2(FlexibleMenu.ItemContextMenu.Delete), itemIndex);
				genericMenu.ShowAsContext();
				GUIUtility.ExitGUI();
			}

			private static void Delete(object userData)
			{
				int index = (int)userData;
				FlexibleMenu.ItemContextMenu.s_Caller.DeleteItem(index);
			}

			private static void Edit(object userData)
			{
				int showEditWindowForIndex = (int)userData;
				FlexibleMenu.ItemContextMenu.s_Caller.m_ShowEditWindowForIndex = showEditWindowForIndex;
			}
		}

		private const float lineHeight = 18f;

		private const float seperatorHeight = 8f;

		private const float leftMargin = 25f;

		private static FlexibleMenu.Styles s_Styles;

		private IFlexibleMenuItemProvider m_ItemProvider;

		private FlexibleMenuModifyItemUI m_ModifyItemUI;

		private readonly Action<int, object> m_ItemClickedCallback;

		private Vector2 m_ScrollPosition = Vector2.zero;

		private bool m_ShowAddNewPresetItem;

		private int m_ShowEditWindowForIndex = -1;

		private int m_HoverIndex;

		private int[] m_SeperatorIndices;

		private float m_MaxTextWidth = -1f;

		private int maxIndex
		{
			get
			{
				return (!this.m_ShowAddNewPresetItem) ? (this.m_ItemProvider.Count() - 1) : this.m_ItemProvider.Count();
			}
		}

		public int selectedIndex
		{
			get;
			set;
		}

		public FlexibleMenu(IFlexibleMenuItemProvider itemProvider, int selectionIndex, FlexibleMenuModifyItemUI modifyItemUi, Action<int, object> itemClickedCallback)
		{
			this.m_ItemProvider = itemProvider;
			this.m_ModifyItemUI = modifyItemUi;
			this.m_ItemClickedCallback = itemClickedCallback;
			this.m_SeperatorIndices = this.m_ItemProvider.GetSeperatorIndices();
			this.selectedIndex = selectionIndex;
			this.m_ShowAddNewPresetItem = (this.m_ModifyItemUI != null);
		}

		public override Vector2 GetWindowSize()
		{
			return this.CalcSize();
		}

		private bool IsDeleteModiferPressed()
		{
			return Event.current.alt;
		}

		private bool AllowDeleteClick(int index)
		{
			return this.IsDeleteModiferPressed() && this.m_ItemProvider.IsModificationAllowed(index) && GUIUtility.hotControl == 0;
		}

		public override void OnGUI(Rect rect)
		{
			if (FlexibleMenu.s_Styles == null)
			{
				FlexibleMenu.s_Styles = new FlexibleMenu.Styles();
			}
			Event current = Event.current;
			Rect viewRect = new Rect(0f, 0f, 1f, this.CalcSize().y);
			this.m_ScrollPosition = GUI.BeginScrollView(rect, this.m_ScrollPosition, viewRect);
			float num = 0f;
			for (int i = 0; i <= this.maxIndex; i++)
			{
				int num2 = i + 1000000;
				Rect rect2 = new Rect(0f, num, rect.width, 18f);
				bool flag = Array.IndexOf<int>(this.m_SeperatorIndices, i) >= 0;
				if (this.m_ShowAddNewPresetItem && i == this.m_ItemProvider.Count())
				{
					this.CreateNewItemButton(rect2);
				}
				else
				{
					if (this.m_ShowEditWindowForIndex == i)
					{
						this.m_ShowEditWindowForIndex = -1;
						this.EditExistingItem(rect2, i);
					}
					EventType type = current.type;
					switch (type)
					{
					case EventType.MouseDown:
						if (current.button == 0 && rect2.Contains(current.mousePosition))
						{
							GUIUtility.hotControl = num2;
							if (!this.IsDeleteModiferPressed() && current.clickCount == 1)
							{
								GUIUtility.hotControl = 0;
								this.SelectItem(i);
								base.editorWindow.Close();
								current.Use();
							}
						}
						goto IL_389;
					case EventType.MouseUp:
						if (GUIUtility.hotControl == num2)
						{
							GUIUtility.hotControl = 0;
							if (current.button == 0 && rect2.Contains(current.mousePosition) && this.AllowDeleteClick(i))
							{
								this.DeleteItem(i);
								current.Use();
							}
						}
						goto IL_389;
					case EventType.MouseMove:
						if (rect2.Contains(current.mousePosition))
						{
							if (this.m_HoverIndex != i)
							{
								this.m_HoverIndex = i;
								this.Repaint();
							}
						}
						else if (this.m_HoverIndex == i)
						{
							this.m_HoverIndex = -1;
							this.Repaint();
						}
						goto IL_389;
					case EventType.MouseDrag:
					case EventType.KeyDown:
					case EventType.KeyUp:
					case EventType.ScrollWheel:
						IL_109:
						if (type != EventType.ContextClick)
						{
							goto IL_389;
						}
						if (rect2.Contains(current.mousePosition))
						{
							current.Use();
							if (this.m_ModifyItemUI != null && this.m_ItemProvider.IsModificationAllowed(i))
							{
								FlexibleMenu.ItemContextMenu.Show(i, this);
							}
						}
						goto IL_389;
					case EventType.Repaint:
					{
						bool isHover = false;
						if (this.m_HoverIndex == i)
						{
							if (rect2.Contains(current.mousePosition))
							{
								isHover = true;
							}
							else
							{
								this.m_HoverIndex = -1;
							}
						}
						if (this.m_ModifyItemUI != null && this.m_ModifyItemUI.IsShowing())
						{
							isHover = (this.m_ItemProvider.GetItem(i) == this.m_ModifyItemUI.m_Object);
						}
						FlexibleMenu.s_Styles.menuItem.Draw(rect2, GUIContent.Temp(this.m_ItemProvider.GetName(i)), isHover, false, i == this.selectedIndex, false);
						if (flag)
						{
							Rect rect3 = new Rect(rect2.x + 4f, rect2.y + rect2.height + 4f, rect2.width - 8f, 1f);
							FlexibleMenu.DrawRect(rect3, (!EditorGUIUtility.isProSkin) ? new Color(0.6f, 0.6f, 0.6f, 1.333f) : new Color(0.32f, 0.32f, 0.32f, 1.333f));
						}
						if (this.AllowDeleteClick(i))
						{
							EditorGUIUtility.AddCursorRect(rect2, MouseCursor.ArrowMinus);
						}
						goto IL_389;
					}
					}
					goto IL_109;
					IL_389:
					num += 18f;
					if (flag)
					{
						num += 8f;
					}
				}
			}
			GUI.EndScrollView();
		}

		private void SelectItem(int index)
		{
			this.selectedIndex = index;
			if (this.m_ItemClickedCallback != null && index >= 0)
			{
				this.m_ItemClickedCallback(index, this.m_ItemProvider.GetItem(index));
			}
		}

		private Vector2 CalcSize()
		{
			float y = (float)(this.maxIndex + 1) * 18f + (float)this.m_SeperatorIndices.Length * 8f;
			if (this.m_MaxTextWidth < 0f)
			{
				this.m_MaxTextWidth = Math.Max(200f, this.CalcWidth());
			}
			return new Vector2(this.m_MaxTextWidth, y);
		}

		private void ClearCachedWidth()
		{
			this.m_MaxTextWidth = -1f;
		}

		private float CalcWidth()
		{
			if (FlexibleMenu.s_Styles == null)
			{
				FlexibleMenu.s_Styles = new FlexibleMenu.Styles();
			}
			float num = 0f;
			for (int i = 0; i < this.m_ItemProvider.Count(); i++)
			{
				float x = FlexibleMenu.s_Styles.menuItem.CalcSize(GUIContent.Temp(this.m_ItemProvider.GetName(i))).x;
				num = Mathf.Max(x, num);
			}
			return num + 6f;
		}

		private void Repaint()
		{
			HandleUtility.Repaint();
		}

		private void CreateNewItemButton(Rect itemRect)
		{
			if (this.m_ModifyItemUI == null)
			{
				return;
			}
			Rect rect = new Rect(itemRect.x + 25f, itemRect.y, 15f, 15f);
			if (GUI.Button(rect, FlexibleMenu.s_Styles.plusButtonText, "OL Plus"))
			{
				rect.y -= 15f;
				this.m_ModifyItemUI.Init(FlexibleMenuModifyItemUI.MenuType.Add, this.m_ItemProvider.Create(), delegate(object obj)
				{
					this.ClearCachedWidth();
					int index = this.m_ItemProvider.Add(obj);
					this.SelectItem(index);
					EditorApplication.RequestRepaintAllViews();
				});
				PopupWindow.Show(rect, this.m_ModifyItemUI);
			}
		}

		private void EditExistingItem(Rect itemRect, int index)
		{
			if (this.m_ModifyItemUI == null)
			{
				return;
			}
			itemRect.y -= itemRect.height;
			itemRect.x += itemRect.width;
			this.m_ModifyItemUI.Init(FlexibleMenuModifyItemUI.MenuType.Edit, this.m_ItemProvider.GetItem(index), delegate(object obj)
			{
				this.ClearCachedWidth();
				this.m_ItemProvider.Replace(index, obj);
				EditorApplication.RequestRepaintAllViews();
			});
			PopupWindow.Show(itemRect, this.m_ModifyItemUI);
		}

		private void DeleteItem(int index)
		{
			this.ClearCachedWidth();
			this.m_ItemProvider.Remove(index);
			this.selectedIndex = Mathf.Clamp(this.selectedIndex, 0, this.m_ItemProvider.Count() - 1);
		}

		public static void DrawRect(Rect rect, Color color)
		{
			if (Event.current.type != EventType.Repaint)
			{
				return;
			}
			Color color2 = GUI.color;
			GUI.color *= color;
			GUI.DrawTexture(rect, EditorGUIUtility.whiteTexture);
			GUI.color = color2;
		}
	}
}
