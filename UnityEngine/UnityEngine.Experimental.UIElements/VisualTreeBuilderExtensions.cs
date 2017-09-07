using System;

namespace UnityEngine.Experimental.UIElements
{
	internal static class VisualTreeBuilderExtensions
	{
		public static IMBox Box(this VisualTreeBuilder cache, Rect position, GUIContent content, GUIStyle style)
		{
			IMBox iMBox;
			cache.NextElement<IMBox>(out iMBox);
			iMBox.GenerateControlID();
			iMBox.position = position;
			iMBox.text = content.text;
			iMBox.style = style;
			return iMBox;
		}

		public static IMButton Button(this VisualTreeBuilder cache, Rect position, GUIContent content, GUIStyle style)
		{
			IMButton iMButton;
			cache.NextElement<IMButton>(out iMButton);
			iMButton.GenerateControlID();
			iMButton.position = position;
			iMButton.text = content.text;
			iMButton.style = style;
			return iMButton;
		}

		public static IMButtonGrid ButtonGrid(this VisualTreeBuilder cache, Rect position, int selected, GUIContent[] contents, int xCount, GUIStyle style, GUIStyle firstStyle, GUIStyle midStyle, GUIStyle lastStyle)
		{
			IMButtonGrid iMButtonGrid;
			cache.NextElement<IMButtonGrid>(out iMButtonGrid);
			iMButtonGrid.GenerateControlID();
			iMButtonGrid.position = position;
			iMButtonGrid.contents = contents;
			iMButtonGrid.style = style;
			iMButtonGrid.xCount = xCount;
			iMButtonGrid.selected = selected;
			iMButtonGrid.firstStyle = firstStyle;
			iMButtonGrid.midStyle = midStyle;
			iMButtonGrid.lastStyle = lastStyle;
			return iMButtonGrid;
		}

		public static IMImage DrawTexture(this VisualTreeBuilder cache, Rect position, Texture image, ScaleMode scaleMode, bool alphaBlend, float imageAspect)
		{
			IMImage iMImage;
			cache.NextElement<IMImage>(out iMImage);
			iMImage.position = position;
			iMImage.image = image;
			iMImage.scaleMode = scaleMode;
			iMImage.alphaBlend = alphaBlend;
			iMImage.imageAspect = imageAspect;
			return iMImage;
		}

		public static IMGroup Group(this VisualTreeBuilder cache, Rect position, GUIContent content, GUIStyle style)
		{
			IMGroup iMGroup;
			cache.NextView<IMGroup>(out iMGroup);
			iMGroup.GenerateControlID();
			iMGroup.position = position;
			iMGroup.text = content.text;
			iMGroup.style = style;
			return iMGroup;
		}

		public static IMLabel Label(this VisualTreeBuilder cache, Rect position, GUIContent content, GUIStyle style)
		{
			IMLabel iMLabel;
			cache.NextElement<IMLabel>(out iMLabel);
			iMLabel.position = position;
			iMLabel.text = content.text;
			iMLabel.style = style;
			return iMLabel;
		}

		public static IMTextField PasswordField(this VisualTreeBuilder cache, Rect position, string passwordToShow, string password, char maskChar, int maxLength, GUIStyle style)
		{
			GUIContent content = GUIContent.Temp(passwordToShow);
			IMTextField result;
			if (TouchScreenKeyboard.isSupported)
			{
				result = cache.TextField(position, GUIUtility.GetControlID(FocusType.Keyboard), content, false, maxLength, style, password, maskChar);
			}
			else
			{
				result = cache.TextField(position, GUIUtility.GetControlID(FocusType.Keyboard, position), content, false, maxLength, style, password, maskChar);
			}
			return result;
		}

		public static IMRepeatButton RepeatButton(this VisualTreeBuilder cache, Rect position, GUIContent content, GUIStyle style, FocusType focusType)
		{
			IMRepeatButton iMRepeatButton;
			cache.NextElement<IMRepeatButton>(out iMRepeatButton);
			iMRepeatButton.GenerateControlID();
			iMRepeatButton.position = position;
			iMRepeatButton.style = style;
			iMRepeatButton.text = content.text;
			iMRepeatButton.focusType = focusType;
			return iMRepeatButton;
		}

		public static IMSlider Slider(this VisualTreeBuilder cache, Rect position, float value, float size, float start, float end, GUIStyle sliderStyle, GUIStyle thumbStyle, bool horiz, int id)
		{
			IMSlider iMSlider;
			cache.NextElement<IMSlider>(out iMSlider);
			if (id != 0)
			{
				iMSlider.AssignControlID(id);
			}
			else
			{
				iMSlider.GenerateControlID();
			}
			iMSlider.SetProperties(position, value, size, start, end, sliderStyle, thumbStyle, horiz);
			return iMSlider;
		}

		public static IMScrollView ScrollView(this VisualTreeBuilder cache, Rect position, Vector2 scrollPosition, Rect viewRect, bool alwaysShowHorizontal, bool alwaysShowVertical, GUIStyle horizontalScrollbar, GUIStyle verticalScrollbar, GUIStyle background)
		{
			IMScrollView iMScrollView;
			cache.NextView<IMScrollView>(out iMScrollView);
			iMScrollView.GenerateControlID();
			iMScrollView.SetProperties(position, scrollPosition, viewRect, alwaysShowHorizontal, alwaysShowVertical, horizontalScrollbar, verticalScrollbar, background);
			return iMScrollView;
		}

		public static IMScroller Scroller(this VisualTreeBuilder cache, Rect position, float value, float size, float leftValue, float rightValue, GUIStyle slider, GUIStyle thumb, GUIStyle leftButton, GUIStyle rightButton, bool horiz)
		{
			IMScroller iMScroller;
			cache.NextElement<IMScroller>(out iMScroller);
			iMScroller.GenerateControlID();
			iMScroller.SetProperties(position, value, size, leftValue, rightValue, slider, thumb, leftButton, rightButton, horiz);
			return iMScroller;
		}

		public static IMTextField TextField(this VisualTreeBuilder cache, Rect position, int id, GUIContent content, bool multiline, int maxLength, GUIStyle style, string secureText, char maskChar)
		{
			IMTextField result;
			if (TouchScreenKeyboard.isSupported)
			{
				IMTouchScreenTextField iMTouchScreenTextField;
				cache.NextElement<IMTouchScreenTextField>(out iMTouchScreenTextField);
				if (id != 0)
				{
					iMTouchScreenTextField.AssignControlID(id);
				}
				else
				{
					iMTouchScreenTextField.GenerateControlID();
				}
				iMTouchScreenTextField.position = position;
				iMTouchScreenTextField.text = content.text;
				iMTouchScreenTextField.style = style;
				iMTouchScreenTextField.maxLength = maxLength;
				iMTouchScreenTextField.multiline = multiline;
				iMTouchScreenTextField.secureText = secureText;
				iMTouchScreenTextField.maskChar = maskChar;
				result = iMTouchScreenTextField;
			}
			else
			{
				IMKeyboardTextField iMKeyboardTextField;
				cache.NextElement<IMKeyboardTextField>(out iMKeyboardTextField);
				if (id != 0)
				{
					iMKeyboardTextField.AssignControlID(id);
				}
				else
				{
					iMKeyboardTextField.GenerateControlID();
				}
				iMKeyboardTextField.position = position;
				iMKeyboardTextField.text = content.text;
				iMKeyboardTextField.style = style;
				iMKeyboardTextField.maxLength = maxLength;
				iMKeyboardTextField.multiline = multiline;
				result = iMKeyboardTextField;
			}
			return result;
		}

		public static IMToggle Toggle(this VisualTreeBuilder cache, Rect position, int id, bool value, GUIContent content, GUIStyle style)
		{
			IMToggle iMToggle;
			cache.NextElement<IMToggle>(out iMToggle);
			if (id != 0)
			{
				iMToggle.AssignControlID(id);
			}
			else
			{
				iMToggle.GenerateControlID();
			}
			iMToggle.position = position;
			iMToggle.text = content.text;
			iMToggle.style = style;
			iMToggle.value = value;
			return iMToggle;
		}
	}
}
