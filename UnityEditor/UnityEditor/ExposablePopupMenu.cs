using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEditor
{
	internal class ExposablePopupMenu
	{
		public class ItemData
		{
			public GUIContent m_GUIContent;

			public GUIStyle m_Style;

			public bool m_On;

			public bool m_Enabled;

			public object m_UserData;

			public float m_Width;

			public ItemData(GUIContent content, GUIStyle style, bool on, bool enabled, object userData)
			{
				this.m_GUIContent = content;
				this.m_Style = style;
				this.m_On = on;
				this.m_Enabled = enabled;
				this.m_UserData = userData;
			}
		}

		public class PopupButtonData
		{
			public GUIContent m_GUIContent;

			public GUIStyle m_Style;

			public PopupButtonData(GUIContent content, GUIStyle style)
			{
				this.m_GUIContent = content;
				this.m_Style = style;
			}
		}

		internal class PopUpMenu
		{
			private static List<ExposablePopupMenu.ItemData> m_Data;

			private static ExposablePopupMenu m_Caller;

			internal static void Show(Rect activatorRect, List<ExposablePopupMenu.ItemData> buttonData, ExposablePopupMenu caller)
			{
				ExposablePopupMenu.PopUpMenu.m_Data = buttonData;
				ExposablePopupMenu.PopUpMenu.m_Caller = caller;
				GenericMenu genericMenu = new GenericMenu();
				foreach (ExposablePopupMenu.ItemData current in ExposablePopupMenu.PopUpMenu.m_Data)
				{
					if (current.m_Enabled)
					{
						genericMenu.AddItem(current.m_GUIContent, current.m_On, new GenericMenu.MenuFunction2(ExposablePopupMenu.PopUpMenu.SelectionCallback), current);
					}
					else
					{
						genericMenu.AddDisabledItem(current.m_GUIContent);
					}
				}
				genericMenu.DropDown(activatorRect);
			}

			private static void SelectionCallback(object userData)
			{
				ExposablePopupMenu.ItemData item = (ExposablePopupMenu.ItemData)userData;
				ExposablePopupMenu.PopUpMenu.m_Caller.SelectionChanged(item);
				ExposablePopupMenu.PopUpMenu.m_Caller = null;
				ExposablePopupMenu.PopUpMenu.m_Data = null;
			}
		}

		private List<ExposablePopupMenu.ItemData> m_Items;

		private float m_WidthOfButtons;

		private float m_ItemSpacing;

		private ExposablePopupMenu.PopupButtonData m_PopupButtonData;

		private float m_WidthOfPopup;

		private float m_MinWidthOfPopup;

		private Action<ExposablePopupMenu.ItemData> m_SelectionChangedCallback;

		public void Init(List<ExposablePopupMenu.ItemData> items, float itemSpacing, float minWidthOfPopup, ExposablePopupMenu.PopupButtonData popupButtonData, Action<ExposablePopupMenu.ItemData> selectionChangedCallback)
		{
			this.m_Items = items;
			this.m_ItemSpacing = itemSpacing;
			this.m_PopupButtonData = popupButtonData;
			this.m_SelectionChangedCallback = selectionChangedCallback;
			this.m_MinWidthOfPopup = minWidthOfPopup;
			this.CalcWidths();
		}

		public float OnGUI(Rect rect)
		{
			if (rect.width >= this.m_WidthOfButtons && rect.width > this.m_MinWidthOfPopup)
			{
				Rect position = rect;
				foreach (ExposablePopupMenu.ItemData current in this.m_Items)
				{
					position.width = current.m_Width;
					EditorGUI.BeginChangeCheck();
					using (new EditorGUI.DisabledScope(!current.m_Enabled))
					{
						GUI.Toggle(position, current.m_On, current.m_GUIContent, current.m_Style);
					}
					if (EditorGUI.EndChangeCheck())
					{
						this.SelectionChanged(current);
						GUIUtility.ExitGUI();
					}
					position.x += current.m_Width + this.m_ItemSpacing;
				}
				return this.m_WidthOfButtons;
			}
			if (this.m_WidthOfPopup < rect.width)
			{
				rect.width = this.m_WidthOfPopup;
			}
			if (EditorGUI.ButtonMouseDown(rect, this.m_PopupButtonData.m_GUIContent, FocusType.Passive, this.m_PopupButtonData.m_Style))
			{
				ExposablePopupMenu.PopUpMenu.Show(rect, this.m_Items, this);
			}
			return this.m_WidthOfPopup;
		}

		private void CalcWidths()
		{
			this.m_WidthOfButtons = 0f;
			foreach (ExposablePopupMenu.ItemData current in this.m_Items)
			{
				current.m_Width = current.m_Style.CalcSize(current.m_GUIContent).x;
				this.m_WidthOfButtons += current.m_Width;
			}
			this.m_WidthOfButtons += (float)(this.m_Items.Count - 1) * this.m_ItemSpacing;
			Vector2 vector = this.m_PopupButtonData.m_Style.CalcSize(this.m_PopupButtonData.m_GUIContent);
			vector.x += 3f;
			this.m_WidthOfPopup = vector.x;
		}

		private void SelectionChanged(ExposablePopupMenu.ItemData item)
		{
			if (this.m_SelectionChangedCallback != null)
			{
				this.m_SelectionChangedCallback(item);
			}
			else
			{
				Debug.LogError("Callback is null");
			}
		}
	}
}
