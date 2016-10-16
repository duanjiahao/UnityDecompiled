using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[ExecuteInEditMode, RequiredByNativeCode]
	[Serializable]
	public sealed class GUISkin : ScriptableObject
	{
		internal delegate void SkinChangedDelegate();

		[SerializeField]
		private Font m_Font;

		[SerializeField]
		private GUIStyle m_box;

		[SerializeField]
		private GUIStyle m_button;

		[SerializeField]
		private GUIStyle m_toggle;

		[SerializeField]
		private GUIStyle m_label;

		[SerializeField]
		private GUIStyle m_textField;

		[SerializeField]
		private GUIStyle m_textArea;

		[SerializeField]
		private GUIStyle m_window;

		[SerializeField]
		private GUIStyle m_horizontalSlider;

		[SerializeField]
		private GUIStyle m_horizontalSliderThumb;

		[SerializeField]
		private GUIStyle m_verticalSlider;

		[SerializeField]
		private GUIStyle m_verticalSliderThumb;

		[SerializeField]
		private GUIStyle m_horizontalScrollbar;

		[SerializeField]
		private GUIStyle m_horizontalScrollbarThumb;

		[SerializeField]
		private GUIStyle m_horizontalScrollbarLeftButton;

		[SerializeField]
		private GUIStyle m_horizontalScrollbarRightButton;

		[SerializeField]
		private GUIStyle m_verticalScrollbar;

		[SerializeField]
		private GUIStyle m_verticalScrollbarThumb;

		[SerializeField]
		private GUIStyle m_verticalScrollbarUpButton;

		[SerializeField]
		private GUIStyle m_verticalScrollbarDownButton;

		[SerializeField]
		private GUIStyle m_ScrollView;

		[SerializeField]
		internal GUIStyle[] m_CustomStyles;

		[SerializeField]
		private GUISettings m_Settings = new GUISettings();

		internal static GUIStyle ms_Error;

		private Dictionary<string, GUIStyle> m_Styles;

		internal static GUISkin.SkinChangedDelegate m_SkinChanged;

		internal static GUISkin current;

		public Font font
		{
			get
			{
				return this.m_Font;
			}
			set
			{
				this.m_Font = value;
				if (GUISkin.current == this)
				{
					GUIStyle.SetDefaultFont(this.m_Font);
				}
				this.Apply();
			}
		}

		public GUIStyle box
		{
			get
			{
				return this.m_box;
			}
			set
			{
				this.m_box = value;
				this.Apply();
			}
		}

		public GUIStyle label
		{
			get
			{
				return this.m_label;
			}
			set
			{
				this.m_label = value;
				this.Apply();
			}
		}

		public GUIStyle textField
		{
			get
			{
				return this.m_textField;
			}
			set
			{
				this.m_textField = value;
				this.Apply();
			}
		}

		public GUIStyle textArea
		{
			get
			{
				return this.m_textArea;
			}
			set
			{
				this.m_textArea = value;
				this.Apply();
			}
		}

		public GUIStyle button
		{
			get
			{
				return this.m_button;
			}
			set
			{
				this.m_button = value;
				this.Apply();
			}
		}

		public GUIStyle toggle
		{
			get
			{
				return this.m_toggle;
			}
			set
			{
				this.m_toggle = value;
				this.Apply();
			}
		}

		public GUIStyle window
		{
			get
			{
				return this.m_window;
			}
			set
			{
				this.m_window = value;
				this.Apply();
			}
		}

		public GUIStyle horizontalSlider
		{
			get
			{
				return this.m_horizontalSlider;
			}
			set
			{
				this.m_horizontalSlider = value;
				this.Apply();
			}
		}

		public GUIStyle horizontalSliderThumb
		{
			get
			{
				return this.m_horizontalSliderThumb;
			}
			set
			{
				this.m_horizontalSliderThumb = value;
				this.Apply();
			}
		}

		public GUIStyle verticalSlider
		{
			get
			{
				return this.m_verticalSlider;
			}
			set
			{
				this.m_verticalSlider = value;
				this.Apply();
			}
		}

		public GUIStyle verticalSliderThumb
		{
			get
			{
				return this.m_verticalSliderThumb;
			}
			set
			{
				this.m_verticalSliderThumb = value;
				this.Apply();
			}
		}

		public GUIStyle horizontalScrollbar
		{
			get
			{
				return this.m_horizontalScrollbar;
			}
			set
			{
				this.m_horizontalScrollbar = value;
				this.Apply();
			}
		}

		public GUIStyle horizontalScrollbarThumb
		{
			get
			{
				return this.m_horizontalScrollbarThumb;
			}
			set
			{
				this.m_horizontalScrollbarThumb = value;
				this.Apply();
			}
		}

		public GUIStyle horizontalScrollbarLeftButton
		{
			get
			{
				return this.m_horizontalScrollbarLeftButton;
			}
			set
			{
				this.m_horizontalScrollbarLeftButton = value;
				this.Apply();
			}
		}

		public GUIStyle horizontalScrollbarRightButton
		{
			get
			{
				return this.m_horizontalScrollbarRightButton;
			}
			set
			{
				this.m_horizontalScrollbarRightButton = value;
				this.Apply();
			}
		}

		public GUIStyle verticalScrollbar
		{
			get
			{
				return this.m_verticalScrollbar;
			}
			set
			{
				this.m_verticalScrollbar = value;
				this.Apply();
			}
		}

		public GUIStyle verticalScrollbarThumb
		{
			get
			{
				return this.m_verticalScrollbarThumb;
			}
			set
			{
				this.m_verticalScrollbarThumb = value;
				this.Apply();
			}
		}

		public GUIStyle verticalScrollbarUpButton
		{
			get
			{
				return this.m_verticalScrollbarUpButton;
			}
			set
			{
				this.m_verticalScrollbarUpButton = value;
				this.Apply();
			}
		}

		public GUIStyle verticalScrollbarDownButton
		{
			get
			{
				return this.m_verticalScrollbarDownButton;
			}
			set
			{
				this.m_verticalScrollbarDownButton = value;
				this.Apply();
			}
		}

		public GUIStyle scrollView
		{
			get
			{
				return this.m_ScrollView;
			}
			set
			{
				this.m_ScrollView = value;
				this.Apply();
			}
		}

		public GUIStyle[] customStyles
		{
			get
			{
				return this.m_CustomStyles;
			}
			set
			{
				this.m_CustomStyles = value;
				this.Apply();
			}
		}

		public GUISettings settings
		{
			get
			{
				return this.m_Settings;
			}
		}

		internal static GUIStyle error
		{
			get
			{
				if (GUISkin.ms_Error == null)
				{
					GUISkin.ms_Error = new GUIStyle();
				}
				return GUISkin.ms_Error;
			}
		}

		public GUISkin()
		{
			this.m_CustomStyles = new GUIStyle[1];
		}

		internal void OnEnable()
		{
			this.Apply();
		}

		internal static void CleanupRoots()
		{
			GUISkin.current = null;
			GUISkin.ms_Error = null;
		}

		internal void Apply()
		{
			if (this.m_CustomStyles == null)
			{
				Debug.Log("custom styles is null");
			}
			this.BuildStyleCache();
		}

		private void BuildStyleCache()
		{
			if (this.m_box == null)
			{
				this.m_box = new GUIStyle();
			}
			if (this.m_button == null)
			{
				this.m_button = new GUIStyle();
			}
			if (this.m_toggle == null)
			{
				this.m_toggle = new GUIStyle();
			}
			if (this.m_label == null)
			{
				this.m_label = new GUIStyle();
			}
			if (this.m_window == null)
			{
				this.m_window = new GUIStyle();
			}
			if (this.m_textField == null)
			{
				this.m_textField = new GUIStyle();
			}
			if (this.m_textArea == null)
			{
				this.m_textArea = new GUIStyle();
			}
			if (this.m_horizontalSlider == null)
			{
				this.m_horizontalSlider = new GUIStyle();
			}
			if (this.m_horizontalSliderThumb == null)
			{
				this.m_horizontalSliderThumb = new GUIStyle();
			}
			if (this.m_verticalSlider == null)
			{
				this.m_verticalSlider = new GUIStyle();
			}
			if (this.m_verticalSliderThumb == null)
			{
				this.m_verticalSliderThumb = new GUIStyle();
			}
			if (this.m_horizontalScrollbar == null)
			{
				this.m_horizontalScrollbar = new GUIStyle();
			}
			if (this.m_horizontalScrollbarThumb == null)
			{
				this.m_horizontalScrollbarThumb = new GUIStyle();
			}
			if (this.m_horizontalScrollbarLeftButton == null)
			{
				this.m_horizontalScrollbarLeftButton = new GUIStyle();
			}
			if (this.m_horizontalScrollbarRightButton == null)
			{
				this.m_horizontalScrollbarRightButton = new GUIStyle();
			}
			if (this.m_verticalScrollbar == null)
			{
				this.m_verticalScrollbar = new GUIStyle();
			}
			if (this.m_verticalScrollbarThumb == null)
			{
				this.m_verticalScrollbarThumb = new GUIStyle();
			}
			if (this.m_verticalScrollbarUpButton == null)
			{
				this.m_verticalScrollbarUpButton = new GUIStyle();
			}
			if (this.m_verticalScrollbarDownButton == null)
			{
				this.m_verticalScrollbarDownButton = new GUIStyle();
			}
			if (this.m_ScrollView == null)
			{
				this.m_ScrollView = new GUIStyle();
			}
			this.m_Styles = new Dictionary<string, GUIStyle>(StringComparer.OrdinalIgnoreCase);
			this.m_Styles["box"] = this.m_box;
			this.m_box.name = "box";
			this.m_Styles["button"] = this.m_button;
			this.m_button.name = "button";
			this.m_Styles["toggle"] = this.m_toggle;
			this.m_toggle.name = "toggle";
			this.m_Styles["label"] = this.m_label;
			this.m_label.name = "label";
			this.m_Styles["window"] = this.m_window;
			this.m_window.name = "window";
			this.m_Styles["textfield"] = this.m_textField;
			this.m_textField.name = "textfield";
			this.m_Styles["textarea"] = this.m_textArea;
			this.m_textArea.name = "textarea";
			this.m_Styles["horizontalslider"] = this.m_horizontalSlider;
			this.m_horizontalSlider.name = "horizontalslider";
			this.m_Styles["horizontalsliderthumb"] = this.m_horizontalSliderThumb;
			this.m_horizontalSliderThumb.name = "horizontalsliderthumb";
			this.m_Styles["verticalslider"] = this.m_verticalSlider;
			this.m_verticalSlider.name = "verticalslider";
			this.m_Styles["verticalsliderthumb"] = this.m_verticalSliderThumb;
			this.m_verticalSliderThumb.name = "verticalsliderthumb";
			this.m_Styles["horizontalscrollbar"] = this.m_horizontalScrollbar;
			this.m_horizontalScrollbar.name = "horizontalscrollbar";
			this.m_Styles["horizontalscrollbarthumb"] = this.m_horizontalScrollbarThumb;
			this.m_horizontalScrollbarThumb.name = "horizontalscrollbarthumb";
			this.m_Styles["horizontalscrollbarleftbutton"] = this.m_horizontalScrollbarLeftButton;
			this.m_horizontalScrollbarLeftButton.name = "horizontalscrollbarleftbutton";
			this.m_Styles["horizontalscrollbarrightbutton"] = this.m_horizontalScrollbarRightButton;
			this.m_horizontalScrollbarRightButton.name = "horizontalscrollbarrightbutton";
			this.m_Styles["verticalscrollbar"] = this.m_verticalScrollbar;
			this.m_verticalScrollbar.name = "verticalscrollbar";
			this.m_Styles["verticalscrollbarthumb"] = this.m_verticalScrollbarThumb;
			this.m_verticalScrollbarThumb.name = "verticalscrollbarthumb";
			this.m_Styles["verticalscrollbarupbutton"] = this.m_verticalScrollbarUpButton;
			this.m_verticalScrollbarUpButton.name = "verticalscrollbarupbutton";
			this.m_Styles["verticalscrollbardownbutton"] = this.m_verticalScrollbarDownButton;
			this.m_verticalScrollbarDownButton.name = "verticalscrollbardownbutton";
			this.m_Styles["scrollview"] = this.m_ScrollView;
			this.m_ScrollView.name = "scrollview";
			if (this.m_CustomStyles != null)
			{
				for (int i = 0; i < this.m_CustomStyles.Length; i++)
				{
					if (this.m_CustomStyles[i] != null)
					{
						this.m_Styles[this.m_CustomStyles[i].name] = this.m_CustomStyles[i];
					}
				}
			}
			GUISkin.error.stretchHeight = true;
			GUISkin.error.normal.textColor = Color.red;
		}

		public GUIStyle GetStyle(string styleName)
		{
			GUIStyle gUIStyle = this.FindStyle(styleName);
			if (gUIStyle != null)
			{
				return gUIStyle;
			}
			Debug.LogWarning(string.Concat(new object[]
			{
				"Unable to find style '",
				styleName,
				"' in skin '",
				base.name,
				"' ",
				Event.current.type
			}));
			return GUISkin.error;
		}

		public GUIStyle FindStyle(string styleName)
		{
			if (this == null)
			{
				Debug.LogError("GUISkin is NULL");
				return null;
			}
			if (this.m_Styles == null)
			{
				this.BuildStyleCache();
			}
			GUIStyle result;
			if (this.m_Styles.TryGetValue(styleName, out result))
			{
				return result;
			}
			return null;
		}

		internal void MakeCurrent()
		{
			GUISkin.current = this;
			GUIStyle.SetDefaultFont(this.font);
			if (GUISkin.m_SkinChanged != null)
			{
				GUISkin.m_SkinChanged();
			}
		}

		public IEnumerator GetEnumerator()
		{
			if (this.m_Styles == null)
			{
				this.BuildStyleCache();
			}
			return this.m_Styles.Values.GetEnumerator();
		}
	}
}
