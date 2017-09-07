using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.Experimental.UIElements.StyleEnums;
using UnityEngine.StyleSheets;

namespace UnityEngine.Experimental.UIElements.StyleSheets
{
	internal class VisualElementStyles : ICustomStyles
	{
		public static VisualElementStyles none = new VisualElementStyles(true);

		internal readonly bool isShared;

		private Dictionary<string, CustomProperty> m_CustomProperties;

		[StyleProperty("width", StylePropertyID.Width)]
		public Style<float> width;

		[StyleProperty("height", StylePropertyID.Height)]
		public Style<float> height;

		[StyleProperty("max-width", StylePropertyID.MaxWidth)]
		public Style<float> maxWidth;

		[StyleProperty("max-height", StylePropertyID.MaxHeight)]
		public Style<float> maxHeight;

		[StyleProperty("min-width", StylePropertyID.MinWidth)]
		public Style<float> minWidth;

		[StyleProperty("min-height", StylePropertyID.MinHeight)]
		public Style<float> minHeight;

		[StyleProperty("flex", StylePropertyID.Flex)]
		public Style<float> flex;

		[StyleProperty("overflow", StylePropertyID.Overflow)]
		public Style<int> overflow;

		[StyleProperty("position-left", StylePropertyID.PositionLeft)]
		public Style<float> positionLeft;

		[StyleProperty("position-top", StylePropertyID.PositionTop)]
		public Style<float> positionTop;

		[StyleProperty("position-right", StylePropertyID.PositionRight)]
		public Style<float> positionRight;

		[StyleProperty("position-bottom", StylePropertyID.PositionBottom)]
		public Style<float> positionBottom;

		[StyleProperty("margin-left", StylePropertyID.MarginLeft)]
		public Style<float> marginLeft;

		[StyleProperty("margin-top", StylePropertyID.MarginTop)]
		public Style<float> marginTop;

		[StyleProperty("margin-right", StylePropertyID.MarginRight)]
		public Style<float> marginRight;

		[StyleProperty("margin-bottom", StylePropertyID.MarginBottom)]
		public Style<float> marginBottom;

		[StyleProperty("border-left", StylePropertyID.BorderLeft)]
		public Style<float> borderLeft;

		[StyleProperty("border-top", StylePropertyID.BorderTop)]
		public Style<float> borderTop;

		[StyleProperty("border-right", StylePropertyID.BorderRight)]
		public Style<float> borderRight;

		[StyleProperty("border-bottom", StylePropertyID.BorderBottom)]
		public Style<float> borderBottom;

		[StyleProperty("padding-left", StylePropertyID.PaddingLeft)]
		public Style<float> paddingLeft;

		[StyleProperty("padding-top", StylePropertyID.PaddingTop)]
		public Style<float> paddingTop;

		[StyleProperty("padding-right", StylePropertyID.PaddingRight)]
		public Style<float> paddingRight;

		[StyleProperty("padding-bottom", StylePropertyID.PaddingBottom)]
		public Style<float> paddingBottom;

		[StyleProperty("position-type", StylePropertyID.PositionType)]
		public Style<int> positionType;

		[StyleProperty("align-self", StylePropertyID.AlignSelf)]
		public Style<int> alignSelf;

		[StyleProperty("text-alignment", StylePropertyID.TextAlignment)]
		public Style<int> textAlignment;

		[StyleProperty("font-style", StylePropertyID.FontStyle)]
		public Style<int> fontStyle;

		[StyleProperty("text-clipping", StylePropertyID.TextClipping)]
		public Style<int> textClipping;

		[StyleProperty("font", StylePropertyID.Font)]
		public Style<Font> font;

		[StyleProperty("font-size", StylePropertyID.FontSize)]
		public Style<int> fontSize;

		[StyleProperty("word-wrap", StylePropertyID.WordWrap)]
		public Style<bool> wordWrap;

		[StyleProperty("text-color", StylePropertyID.TextColor)]
		public Style<Color> textColor;

		[StyleProperty("flex-direction", StylePropertyID.FlexDirection)]
		public Style<int> flexDirection;

		[StyleProperty("background-color", StylePropertyID.BackgroundColor)]
		public Style<Color> backgroundColor;

		[StyleProperty("border-color", StylePropertyID.BorderColor)]
		public Style<Color> borderColor;

		[StyleProperty("background-image", StylePropertyID.BackgroundImage)]
		public Style<Texture2D> backgroundImage;

		[StyleProperty("background-size", StylePropertyID.BackgroundSize)]
		public Style<int> backgroundSize;

		[StyleProperty("align-items", StylePropertyID.AlignItems)]
		public Style<int> alignItems;

		[StyleProperty("align-content", StylePropertyID.AlignContent)]
		public Style<int> alignContent;

		[StyleProperty("justify-content", StylePropertyID.JustifyContent)]
		public Style<int> justifyContent;

		[StyleProperty("flex-wrap", StylePropertyID.FlexWrap)]
		public Style<int> flexWrap;

		[StyleProperty("border-width", StylePropertyID.BorderWidth)]
		public Style<float> borderWidth;

		[StyleProperty("border-radius", StylePropertyID.BorderRadius)]
		public Style<float> borderRadius;

		[StyleProperty("slice-left", StylePropertyID.SliceLeft)]
		public Style<int> sliceLeft;

		[StyleProperty("slice-top", StylePropertyID.SliceTop)]
		public Style<int> sliceTop;

		[StyleProperty("slice-right", StylePropertyID.SliceRight)]
		public Style<int> sliceRight;

		[StyleProperty("slice-bottom", StylePropertyID.SliceBottom)]
		public Style<int> sliceBottom;

		[StyleProperty("opacity", StylePropertyID.Opacity)]
		public Style<float> opacity;

		[CompilerGenerated]
		private static LoadResourceFunction <>f__mg$cache0;

		internal VisualElementStyles(bool isShared)
		{
			this.isShared = isShared;
		}

		public VisualElementStyles(VisualElementStyles other, bool isShared) : this(isShared)
		{
			this.Apply(other, StylePropertyApplyMode.Copy);
		}

		internal void Apply(VisualElementStyles other, StylePropertyApplyMode mode)
		{
			this.m_CustomProperties = other.m_CustomProperties;
			this.width.Apply(other.width, mode);
			this.height.Apply(other.height, mode);
			this.maxWidth.Apply(other.maxWidth, mode);
			this.maxHeight.Apply(other.maxHeight, mode);
			this.minWidth.Apply(other.minWidth, mode);
			this.minHeight.Apply(other.minHeight, mode);
			this.flex.Apply(other.flex, mode);
			this.overflow.Apply(other.overflow, mode);
			this.positionLeft.Apply(other.positionLeft, mode);
			this.positionTop.Apply(other.positionTop, mode);
			this.positionRight.Apply(other.positionRight, mode);
			this.positionBottom.Apply(other.positionBottom, mode);
			this.marginLeft.Apply(other.marginLeft, mode);
			this.marginTop.Apply(other.marginTop, mode);
			this.marginRight.Apply(other.marginRight, mode);
			this.marginBottom.Apply(other.marginBottom, mode);
			this.borderLeft.Apply(other.borderLeft, mode);
			this.borderTop.Apply(other.borderTop, mode);
			this.borderRight.Apply(other.borderRight, mode);
			this.borderBottom.Apply(other.borderBottom, mode);
			this.paddingLeft.Apply(other.paddingLeft, mode);
			this.paddingTop.Apply(other.paddingTop, mode);
			this.paddingRight.Apply(other.paddingRight, mode);
			this.paddingBottom.Apply(other.paddingBottom, mode);
			this.positionType.Apply(other.positionType, mode);
			this.alignSelf.Apply(other.alignSelf, mode);
			this.textAlignment.Apply(other.textAlignment, mode);
			this.fontStyle.Apply(other.fontStyle, mode);
			this.textClipping.Apply(other.textClipping, mode);
			this.fontSize.Apply(other.fontSize, mode);
			this.font.Apply(other.font, mode);
			this.wordWrap.Apply(other.wordWrap, mode);
			this.textColor.Apply(other.textColor, mode);
			this.flexDirection.Apply(other.flexDirection, mode);
			this.backgroundColor.Apply(other.backgroundColor, mode);
			this.borderColor.Apply(other.borderColor, mode);
			this.backgroundImage.Apply(other.backgroundImage, mode);
			this.backgroundSize.Apply(other.backgroundSize, mode);
			this.alignItems.Apply(other.alignItems, mode);
			this.alignContent.Apply(other.alignContent, mode);
			this.justifyContent.Apply(other.justifyContent, mode);
			this.flexWrap.Apply(other.flexWrap, mode);
			this.borderWidth.Apply(other.borderWidth, mode);
			this.borderRadius.Apply(other.borderRadius, mode);
			this.sliceLeft.Apply(other.sliceLeft, mode);
			this.sliceTop.Apply(other.sliceTop, mode);
			this.sliceRight.Apply(other.sliceRight, mode);
			this.sliceBottom.Apply(other.sliceBottom, mode);
			this.opacity.Apply(other.opacity, mode);
		}

		public void WriteToGUIStyle(GUIStyle style)
		{
			style.alignment = (TextAnchor)this.textAlignment.GetSpecifiedValueOrDefault((int)style.alignment);
			style.wordWrap = this.wordWrap.GetSpecifiedValueOrDefault(style.wordWrap);
			style.clipping = (TextClipping)this.textClipping.GetSpecifiedValueOrDefault((int)style.clipping);
			if (this.font.value != null)
			{
				style.font = this.font.value;
			}
			style.fontSize = this.fontSize.GetSpecifiedValueOrDefault(style.fontSize);
			style.fontStyle = (FontStyle)this.fontStyle.GetSpecifiedValueOrDefault((int)style.fontStyle);
			this.AssignRect(style.margin, ref this.marginLeft, ref this.marginTop, ref this.marginRight, ref this.marginBottom);
			this.AssignRect(style.padding, ref this.paddingLeft, ref this.paddingTop, ref this.paddingRight, ref this.paddingBottom);
			this.AssignRect(style.border, ref this.sliceLeft, ref this.sliceTop, ref this.sliceRight, ref this.sliceBottom);
			this.AssignState(style.normal);
			this.AssignState(style.focused);
			this.AssignState(style.hover);
			this.AssignState(style.active);
			this.AssignState(style.onNormal);
			this.AssignState(style.onFocused);
			this.AssignState(style.onHover);
			this.AssignState(style.onActive);
		}

		private void AssignState(GUIStyleState state)
		{
			state.textColor = this.textColor.GetSpecifiedValueOrDefault(state.textColor);
			if (this.backgroundImage.value != null)
			{
				state.background = this.backgroundImage.value;
				if (state.scaledBackgrounds == null || state.scaledBackgrounds.Length < 1 || state.scaledBackgrounds[0] != this.backgroundImage.value)
				{
					state.scaledBackgrounds = new Texture2D[]
					{
						this.backgroundImage.value
					};
				}
			}
		}

		private void AssignRect(RectOffset rect, ref Style<int> left, ref Style<int> top, ref Style<int> right, ref Style<int> bottom)
		{
			rect.left = left.GetSpecifiedValueOrDefault(rect.left);
			rect.top = top.GetSpecifiedValueOrDefault(rect.top);
			rect.right = right.GetSpecifiedValueOrDefault(rect.right);
			rect.bottom = bottom.GetSpecifiedValueOrDefault(rect.bottom);
		}

		private void AssignRect(RectOffset rect, ref Style<float> left, ref Style<float> top, ref Style<float> right, ref Style<float> bottom)
		{
			rect.left = (int)left.GetSpecifiedValueOrDefault((float)rect.left);
			rect.top = (int)top.GetSpecifiedValueOrDefault((float)rect.top);
			rect.right = (int)right.GetSpecifiedValueOrDefault((float)rect.right);
			rect.bottom = (int)bottom.GetSpecifiedValueOrDefault((float)rect.bottom);
		}

		internal void ApplyRule(StyleSheet registry, int specificity, StyleRule rule, StylePropertyID[] propertyIDs, LoadResourceFunction loadResourceFunc)
		{
			for (int i = 0; i < rule.properties.Length; i++)
			{
				StyleProperty styleProperty = rule.properties[i];
				StylePropertyID stylePropertyID = propertyIDs[i];
				StyleValueHandle handle = styleProperty.values[0];
				switch (stylePropertyID)
				{
				case StylePropertyID.MarginLeft:
					registry.Apply(handle, specificity, ref this.marginLeft);
					break;
				case StylePropertyID.MarginTop:
					registry.Apply(handle, specificity, ref this.marginTop);
					break;
				case StylePropertyID.MarginRight:
					registry.Apply(handle, specificity, ref this.marginRight);
					break;
				case StylePropertyID.MarginBottom:
					registry.Apply(handle, specificity, ref this.marginBottom);
					break;
				case StylePropertyID.PaddingLeft:
					registry.Apply(handle, specificity, ref this.paddingLeft);
					break;
				case StylePropertyID.PaddingTop:
					registry.Apply(handle, specificity, ref this.paddingTop);
					break;
				case StylePropertyID.PaddingRight:
					registry.Apply(handle, specificity, ref this.paddingRight);
					break;
				case StylePropertyID.PaddingBottom:
					registry.Apply(handle, specificity, ref this.paddingBottom);
					break;
				case StylePropertyID.BorderLeft:
					registry.Apply(handle, specificity, ref this.borderLeft);
					break;
				case StylePropertyID.BorderTop:
					registry.Apply(handle, specificity, ref this.borderTop);
					break;
				case StylePropertyID.BorderRight:
					registry.Apply(handle, specificity, ref this.borderRight);
					break;
				case StylePropertyID.BorderBottom:
					registry.Apply(handle, specificity, ref this.borderBottom);
					break;
				case StylePropertyID.PositionType:
					registry.Apply(handle, specificity, ref this.positionType);
					break;
				case StylePropertyID.PositionLeft:
					registry.Apply(handle, specificity, ref this.positionLeft);
					break;
				case StylePropertyID.PositionTop:
					registry.Apply(handle, specificity, ref this.positionTop);
					break;
				case StylePropertyID.PositionRight:
					registry.Apply(handle, specificity, ref this.positionRight);
					break;
				case StylePropertyID.PositionBottom:
					registry.Apply(handle, specificity, ref this.positionBottom);
					break;
				case StylePropertyID.Width:
					registry.Apply(handle, specificity, ref this.width);
					break;
				case StylePropertyID.Height:
					registry.Apply(handle, specificity, ref this.height);
					break;
				case StylePropertyID.MinWidth:
					registry.Apply(handle, specificity, ref this.minWidth);
					break;
				case StylePropertyID.MinHeight:
					registry.Apply(handle, specificity, ref this.minHeight);
					break;
				case StylePropertyID.MaxWidth:
					registry.Apply(handle, specificity, ref this.maxWidth);
					break;
				case StylePropertyID.MaxHeight:
					registry.Apply(handle, specificity, ref this.maxHeight);
					break;
				case StylePropertyID.Flex:
					registry.Apply(handle, specificity, ref this.flex);
					break;
				case StylePropertyID.BorderWidth:
					registry.Apply(handle, specificity, ref this.borderWidth);
					break;
				case StylePropertyID.BorderRadius:
					registry.Apply(handle, specificity, ref this.borderRadius);
					break;
				case StylePropertyID.FlexDirection:
					registry.Apply(handle, specificity, ref this.flexDirection);
					break;
				case StylePropertyID.FlexWrap:
					registry.Apply(handle, specificity, ref this.flexWrap);
					break;
				case StylePropertyID.JustifyContent:
					registry.Apply(handle, specificity, ref this.justifyContent);
					break;
				case StylePropertyID.AlignContent:
					registry.Apply(handle, specificity, ref this.alignContent);
					break;
				case StylePropertyID.AlignSelf:
					registry.Apply(handle, specificity, ref this.alignSelf);
					break;
				case StylePropertyID.AlignItems:
					registry.Apply(handle, specificity, ref this.alignItems);
					break;
				case StylePropertyID.TextAlignment:
					registry.Apply(handle, specificity, ref this.textAlignment);
					break;
				case StylePropertyID.TextClipping:
					registry.Apply(handle, specificity, ref this.textClipping);
					break;
				case StylePropertyID.Font:
					registry.Apply(handle, specificity, loadResourceFunc, ref this.font);
					break;
				case StylePropertyID.FontSize:
					registry.Apply(handle, specificity, ref this.fontSize);
					break;
				case StylePropertyID.FontStyle:
					registry.Apply(handle, specificity, ref this.fontStyle);
					break;
				case StylePropertyID.BackgroundSize:
					registry.Apply(handle, specificity, ref this.backgroundSize);
					break;
				case StylePropertyID.WordWrap:
					registry.Apply(handle, specificity, ref this.wordWrap);
					break;
				case StylePropertyID.BackgroundImage:
					registry.Apply(handle, specificity, loadResourceFunc, ref this.backgroundImage);
					break;
				case StylePropertyID.TextColor:
					registry.Apply(handle, specificity, ref this.textColor);
					break;
				case StylePropertyID.BackgroundColor:
					registry.Apply(handle, specificity, ref this.backgroundColor);
					break;
				case StylePropertyID.BorderColor:
					registry.Apply(handle, specificity, ref this.borderColor);
					break;
				case StylePropertyID.Overflow:
					registry.Apply(handle, specificity, ref this.overflow);
					break;
				case StylePropertyID.SliceLeft:
					registry.Apply(handle, specificity, ref this.sliceLeft);
					break;
				case StylePropertyID.SliceTop:
					registry.Apply(handle, specificity, ref this.sliceTop);
					break;
				case StylePropertyID.SliceRight:
					registry.Apply(handle, specificity, ref this.sliceRight);
					break;
				case StylePropertyID.SliceBottom:
					registry.Apply(handle, specificity, ref this.sliceBottom);
					break;
				case StylePropertyID.Opacity:
					registry.Apply(handle, specificity, ref this.opacity);
					break;
				case StylePropertyID.Custom:
				{
					if (this.m_CustomProperties == null)
					{
						this.m_CustomProperties = new Dictionary<string, CustomProperty>();
					}
					CustomProperty value = default(CustomProperty);
					if (!this.m_CustomProperties.TryGetValue(styleProperty.name, out value) || specificity >= value.specificity)
					{
						value.handle = handle;
						value.data = registry;
						value.specificity = specificity;
						this.m_CustomProperties[styleProperty.name] = value;
					}
					break;
				}
				default:
					throw new ArgumentException(string.Format("Non exhaustive switch statement (value={0})", stylePropertyID));
				}
			}
		}

		public void ApplyCustomProperty(string propertyName, ref Style<float> target)
		{
			Style<float> other = new Style<float>(0f);
			CustomProperty customProperty;
			if (this.m_CustomProperties != null && this.m_CustomProperties.TryGetValue(propertyName, out customProperty))
			{
				if (customProperty.handle.valueType == StyleValueType.Float)
				{
					customProperty.data.Apply(customProperty.handle, customProperty.specificity, ref other);
				}
				else
				{
					Debug.LogWarning(string.Format("Trying to read value as float while parsed type is {0}", customProperty.handle.valueType));
				}
			}
			target.Apply(other, StylePropertyApplyMode.CopyIfNotInline);
		}

		public void ApplyCustomProperty(string propertyName, ref Style<int> target)
		{
			Style<int> other = new Style<int>(0);
			CustomProperty customProperty;
			if (this.m_CustomProperties != null && this.m_CustomProperties.TryGetValue(propertyName, out customProperty))
			{
				if (customProperty.handle.valueType == StyleValueType.Float)
				{
					customProperty.data.Apply(customProperty.handle, customProperty.specificity, ref other);
				}
				else
				{
					Debug.LogWarning(string.Format("Trying to read value as int while parsed type is {0}", customProperty.handle.valueType));
				}
			}
			target.Apply(other, StylePropertyApplyMode.CopyIfNotInline);
		}

		public void ApplyCustomProperty(string propertyName, ref Style<bool> target)
		{
			Style<bool> other = new Style<bool>(false);
			CustomProperty customProperty;
			if (this.m_CustomProperties != null && this.m_CustomProperties.TryGetValue(propertyName, out customProperty))
			{
				if (customProperty.handle.valueType == StyleValueType.Keyword)
				{
					customProperty.data.Apply(customProperty.handle, customProperty.specificity, ref other);
				}
				else
				{
					Debug.LogWarning(string.Format("Trying to read value as bool while parsed type is {0}", customProperty.handle.valueType));
				}
			}
			target.Apply(other, StylePropertyApplyMode.CopyIfNotInline);
		}

		public void ApplyCustomProperty(string propertyName, ref Style<Color> target)
		{
			Style<Color> other = new Style<Color>(Color.clear);
			CustomProperty customProperty;
			if (this.m_CustomProperties != null && this.m_CustomProperties.TryGetValue(propertyName, out customProperty))
			{
				if (customProperty.handle.valueType == StyleValueType.Color)
				{
					customProperty.data.Apply(customProperty.handle, customProperty.specificity, ref other);
				}
				else
				{
					Debug.LogWarning(string.Format("Trying to read value as Color while parsed type is {0}", customProperty.handle.valueType));
				}
			}
			target.Apply(other, StylePropertyApplyMode.CopyIfNotInline);
		}

		public void ApplyCustomProperty<T>(string propertyName, ref Style<T> target) where T : UnityEngine.Object
		{
			if (VisualElementStyles.<>f__mg$cache0 == null)
			{
				VisualElementStyles.<>f__mg$cache0 = new LoadResourceFunction(Resources.Load);
			}
			this.ApplyCustomProperty<T>(propertyName, VisualElementStyles.<>f__mg$cache0, ref target);
		}

		public void ApplyCustomProperty<T>(string propertyName, LoadResourceFunction function, ref Style<T> target) where T : UnityEngine.Object
		{
			Style<T> other = new Style<T>((T)((object)null));
			CustomProperty customProperty;
			if (this.m_CustomProperties != null && this.m_CustomProperties.TryGetValue(propertyName, out customProperty))
			{
				if (customProperty.handle.valueType == StyleValueType.ResourcePath)
				{
					customProperty.data.Apply(customProperty.handle, customProperty.specificity, function, ref other);
				}
				else
				{
					Debug.LogWarning(string.Format("Trying to read value as Object while parsed type is {0}", customProperty.handle.valueType));
				}
			}
			target.Apply(other, StylePropertyApplyMode.CopyIfNotInline);
		}

		public void ApplyCustomProperty(string propertyName, ref Style<string> target)
		{
			Style<string> other = new Style<string>(string.Empty);
			CustomProperty customProperty;
			if (this.m_CustomProperties != null && this.m_CustomProperties.TryGetValue(propertyName, out customProperty))
			{
				other.value = customProperty.data.ReadAsString(customProperty.handle);
				other.specificity = customProperty.specificity;
			}
			target.Apply(other, StylePropertyApplyMode.CopyIfNotInline);
		}
	}
}
