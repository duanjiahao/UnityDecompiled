using System;
using UnityEngine;

namespace UnityEditor
{
	internal class ColorPicker : EditorWindow
	{
		internal enum TonemappingType
		{
			Linear,
			Photographic
		}

		[Serializable]
		private class HDRValues
		{
			[NonSerialized]
			public ColorPicker.TonemappingType m_TonemappingType = ColorPicker.TonemappingType.Photographic;

			[SerializeField]
			public float m_HDRScaleFactor;

			[SerializeField]
			public float m_ExposureAdjustment = 1.5f;
		}

		private enum ColorBoxMode
		{
			SV_H,
			HV_S,
			HS_V,
			BG_R,
			BR_G,
			RG_B,
			EyeDropper
		}

		private enum SliderMode
		{
			RGB,
			HSV
		}

		private enum LabelLocation
		{
			Top,
			Bottom,
			Left,
			Right
		}

		private class Styles
		{
			public GUIStyle pickerBox = "ColorPickerBox";

			public GUIStyle thumb2D = "ColorPicker2DThumb";

			public GUIStyle thumbHoriz = "ColorPickerHorizThumb";

			public GUIStyle thumbVert = "ColorPickerVertThumb";

			public GUIStyle headerLine = "IN Title";

			public GUIStyle colorPickerBox = "ColorPickerBox";

			public GUIStyle background = new GUIStyle("ColorPickerBackground");

			public GUIStyle label = new GUIStyle(EditorStyles.miniLabel);

			public GUIStyle axisLabelNumberField = new GUIStyle(EditorStyles.miniTextField);

			public GUIStyle foldout = new GUIStyle(EditorStyles.foldout);

			public GUIStyle toggle = new GUIStyle(EditorStyles.toggle);

			public GUIContent eyeDropper = EditorGUIUtility.IconContent("EyeDropper.Large", "|Pick a color from the screen.");

			public GUIContent colorCycle = EditorGUIUtility.IconContent("ColorPicker.CycleColor");

			public GUIContent colorToggle = EditorGUIUtility.TextContent("Colors");

			public GUIContent tonemappingToggle = new GUIContent("Tonemapped Preview", "When enabled preview colors are tonemapped using Photographic Tonemapping");

			public GUIContent sliderToggle = EditorGUIUtility.TextContent("Sliders|The RGB or HSV color sliders.");

			public GUIContent presetsToggle = new GUIContent("Presets");

			public GUIContent sliderCycle = EditorGUIUtility.IconContent("ColorPicker.CycleSlider");

			public Styles()
			{
				this.axisLabelNumberField.alignment = TextAnchor.UpperRight;
				this.axisLabelNumberField.normal.background = null;
				this.label.alignment = TextAnchor.LowerCenter;
			}
		}

		private static ColorPicker s_SharedColorPicker;

		private static readonly ColorPickerHDRConfig m_DefaultHDRConfig = new ColorPickerHDRConfig(0f, 99f, 0.01010101f, 3f);

		[SerializeField]
		private bool m_HDR;

		[SerializeField]
		private ColorPickerHDRConfig m_HDRConfig;

		[SerializeField]
		private ColorPicker.HDRValues m_HDRValues = new ColorPicker.HDRValues();

		[SerializeField]
		private Color m_Color = Color.black;

		[SerializeField]
		private Color m_OriginalColor;

		[SerializeField]
		private float m_R;

		[SerializeField]
		private float m_G;

		[SerializeField]
		private float m_B;

		[SerializeField]
		private float m_H;

		[SerializeField]
		private float m_S;

		[SerializeField]
		private float m_V;

		[SerializeField]
		private float m_A = 1f;

		[SerializeField]
		private float m_ColorSliderSize = 4f;

		[SerializeField]
		private Texture2D m_ColorSlider;

		[SerializeField]
		private float m_SliderValue;

		[SerializeField]
		private Color[] m_Colors;

		private const int kHueRes = 64;

		private const int kColorBoxSize = 32;

		[SerializeField]
		private Texture2D m_ColorBox;

		private static int s_Slider2Dhash = "Slider2D".GetHashCode();

		[SerializeField]
		private bool m_ShowPresets = true;

		[SerializeField]
		private bool m_UseTonemappingPreview = false;

		[SerializeField]
		private bool m_IsOSColorPicker = false;

		[SerializeField]
		private bool m_resetKeyboardControl = false;

		[SerializeField]
		private bool m_ShowAlpha = true;

		private Texture2D m_RTexture;

		private float m_RTextureG = -1f;

		private float m_RTextureB = -1f;

		private Texture2D m_GTexture;

		private float m_GTextureR = -1f;

		private float m_GTextureB = -1f;

		private Texture2D m_BTexture;

		private float m_BTextureR = -1f;

		private float m_BTextureG = -1f;

		[SerializeField]
		private Texture2D m_HueTexture;

		private float m_HueTextureS = -1f;

		private float m_HueTextureV = -1f;

		[SerializeField]
		private Texture2D m_SatTexture;

		private float m_SatTextureH = -1f;

		private float m_SatTextureV = -1f;

		[SerializeField]
		private Texture2D m_ValTexture;

		private float m_ValTextureH = -1f;

		private float m_ValTextureS = -1f;

		[SerializeField]
		private int m_TextureColorSliderMode = -1;

		[SerializeField]
		private Vector2 m_LastConstantValues = new Vector2(-1f, -1f);

		[NonSerialized]
		private int m_TextureColorBoxMode = -1;

		[SerializeField]
		private float m_LastConstant = -1f;

		[NonSerialized]
		private bool m_ColorSpaceBoxDirty;

		[NonSerialized]
		private bool m_ColorSliderDirty;

		[NonSerialized]
		private bool m_RGBHSVSlidersDirty;

		[SerializeField]
		private ContainerWindow m_TrackingWindow;

		private string[] m_ColorBoxXAxisLabels = new string[]
		{
			"Saturation",
			"Hue",
			"Hue",
			"Blue",
			"Blue",
			"Red",
			""
		};

		private string[] m_ColorBoxYAxisLabels = new string[]
		{
			"Brightness",
			"Brightness",
			"Saturation",
			"Green",
			"Red",
			"Green",
			""
		};

		private string[] m_ColorBoxZAxisLabels = new string[]
		{
			"Hue",
			"Saturation",
			"Brightness",
			"Red",
			"Green",
			"Blue",
			""
		};

		[SerializeField]
		private ColorPicker.ColorBoxMode m_ColorBoxMode = ColorPicker.ColorBoxMode.BG_R;

		[SerializeField]
		private ColorPicker.ColorBoxMode m_OldColorBoxMode;

		[SerializeField]
		private ColorPicker.SliderMode m_SliderMode = ColorPicker.SliderMode.HSV;

		[SerializeField]
		private Texture2D m_AlphaTexture;

		private float m_OldAlpha = -1f;

		[SerializeField]
		private GUIView m_DelegateView;

		[SerializeField]
		private int m_ModalUndoGroup = -1;

		private PresetLibraryEditor<ColorPresetLibrary> m_ColorLibraryEditor;

		private PresetLibraryEditorState m_ColorLibraryEditorState;

		private const int kEyeDropperHeight = 95;

		private const int kSlidersHeight = 82;

		private const int kColorBoxHeight = 162;

		private const int kPresetsHeight = 300;

		private const float kFixedWindowWidth = 233f;

		private const float kHDRFieldWidth = 40f;

		private const float kLDRFieldWidth = 30f;

		private static ColorPicker.Styles styles;

		private static Texture2D s_LeftGradientTexture;

		private static Texture2D s_RightGradientTexture;

		public static string presetsEditorPrefID
		{
			get
			{
				return "Color";
			}
		}

		public static ColorPickerHDRConfig defaultHDRConfig
		{
			get
			{
				return ColorPicker.m_DefaultHDRConfig;
			}
		}

		private bool colorChanged
		{
			get;
			set;
		}

		private float fieldWidth
		{
			get
			{
				return (!this.m_HDR) ? 30f : 40f;
			}
		}

		public static bool visible
		{
			get
			{
				return ColorPicker.s_SharedColorPicker != null;
			}
		}

		public static Color color
		{
			get
			{
				Color result;
				if (ColorPicker.get.m_HDRValues.m_HDRScaleFactor > 1f)
				{
					result = ColorPicker.get.m_Color.RGBMultiplied(ColorPicker.get.m_HDRValues.m_HDRScaleFactor);
				}
				else
				{
					result = ColorPicker.get.m_Color;
				}
				return result;
			}
			set
			{
				ColorPicker.get.SetColor(value);
			}
		}

		public static ColorPicker get
		{
			get
			{
				if (!ColorPicker.s_SharedColorPicker)
				{
					UnityEngine.Object[] array = Resources.FindObjectsOfTypeAll(typeof(ColorPicker));
					if (array != null && array.Length > 0)
					{
						ColorPicker.s_SharedColorPicker = (ColorPicker)array[0];
					}
					if (!ColorPicker.s_SharedColorPicker)
					{
						ColorPicker.s_SharedColorPicker = ScriptableObject.CreateInstance<ColorPicker>();
						ColorPicker.s_SharedColorPicker.wantsMouseMove = true;
					}
				}
				return ColorPicker.s_SharedColorPicker;
			}
		}

		public string currentPresetLibrary
		{
			get
			{
				this.InitIfNeeded();
				return this.m_ColorLibraryEditor.currentLibraryWithoutExtension;
			}
			set
			{
				this.InitIfNeeded();
				this.m_ColorLibraryEditor.currentLibraryWithoutExtension = value;
			}
		}

		private void OnSelectionChange()
		{
			this.m_resetKeyboardControl = true;
			base.Repaint();
		}

		private void RGBToHSV()
		{
			Color.RGBToHSV(new Color(this.m_R, this.m_G, this.m_B, 1f), out this.m_H, out this.m_S, out this.m_V);
		}

		private void HSVToRGB()
		{
			Color color = Color.HSVToRGB(this.m_H, this.m_S, this.m_V);
			this.m_R = color.r;
			this.m_G = color.g;
			this.m_B = color.b;
		}

		private static void swap(ref float f1, ref float f2)
		{
			float num = f1;
			f1 = f2;
			f2 = num;
		}

		private Vector2 Slider2D(Rect rect, Vector2 value, Vector2 maxvalue, Vector2 minvalue, GUIStyle backStyle, GUIStyle thumbStyle)
		{
			Vector2 result;
			if (backStyle == null)
			{
				result = value;
			}
			else if (thumbStyle == null)
			{
				result = value;
			}
			else
			{
				int controlID = GUIUtility.GetControlID(ColorPicker.s_Slider2Dhash, FocusType.Passive);
				if (maxvalue.x < minvalue.x)
				{
					ColorPicker.swap(ref maxvalue.x, ref minvalue.x);
				}
				if (maxvalue.y < minvalue.y)
				{
					ColorPicker.swap(ref maxvalue.y, ref minvalue.y);
				}
				float num = (thumbStyle.fixedHeight != 0f) ? thumbStyle.fixedHeight : ((float)thumbStyle.padding.vertical);
				float num2 = (thumbStyle.fixedWidth != 0f) ? thumbStyle.fixedWidth : ((float)thumbStyle.padding.horizontal);
				Vector2 vector = new Vector2((rect.width - (float)(backStyle.padding.right + backStyle.padding.left) - num2 * 2f) / (maxvalue.x - minvalue.x), (rect.height - (float)(backStyle.padding.top + backStyle.padding.bottom) - num * 2f) / (maxvalue.y - minvalue.y));
				Rect position = new Rect(rect.x + value.x * vector.x + num2 / 2f + (float)backStyle.padding.left - minvalue.x * vector.x, rect.y + value.y * vector.y + num / 2f + (float)backStyle.padding.top - minvalue.y * vector.y, num2, num);
				Event current = Event.current;
				switch (current.GetTypeForControl(controlID))
				{
				case EventType.MouseDown:
					if (rect.Contains(current.mousePosition))
					{
						GUIUtility.hotControl = controlID;
						GUIUtility.keyboardControl = 0;
						value.x = (current.mousePosition.x - rect.x - num2 - (float)backStyle.padding.left) / vector.x + minvalue.x;
						value.y = (current.mousePosition.y - rect.y - num - (float)backStyle.padding.top) / vector.y + minvalue.y;
						GUI.changed = true;
						Event.current.Use();
					}
					break;
				case EventType.MouseUp:
					if (GUIUtility.hotControl == controlID)
					{
						GUIUtility.hotControl = 0;
						current.Use();
					}
					break;
				case EventType.MouseDrag:
					if (GUIUtility.hotControl == controlID)
					{
						value.x = (current.mousePosition.x - rect.x - num2 - (float)backStyle.padding.left) / vector.x + minvalue.x;
						value.y = (current.mousePosition.y - rect.y - num - (float)backStyle.padding.top) / vector.y + minvalue.y;
						value.x = Mathf.Clamp(value.x, minvalue.x, maxvalue.x);
						value.y = Mathf.Clamp(value.y, minvalue.y, maxvalue.y);
						GUI.changed = true;
						Event.current.Use();
					}
					break;
				case EventType.Repaint:
				{
					backStyle.Draw(rect, GUIContent.none, controlID);
					Color color = GUI.color;
					bool flag = ColorPicker.color.grayscale > 0.5f;
					if (flag)
					{
						GUI.color = Color.black;
					}
					thumbStyle.Draw(position, GUIContent.none, controlID);
					if (flag)
					{
						GUI.color = color;
					}
					break;
				}
				}
				result = value;
			}
			return result;
		}

		private void OnFloatFieldChanged(float value)
		{
			if (this.m_HDR && value > this.m_HDRValues.m_HDRScaleFactor)
			{
				this.SetHDRScaleFactor(value);
			}
		}

		private void RGBSliders()
		{
			EditorGUI.BeginChangeCheck();
			float tonemappingExposureAdjusment = this.GetTonemappingExposureAdjusment();
			float colorScale = this.GetColorScale();
			this.m_RTexture = ColorPicker.Update1DSlider(this.m_RTexture, 32, this.m_G, this.m_B, ref this.m_RTextureG, ref this.m_RTextureB, 0, false, colorScale, tonemappingExposureAdjusment, this.m_RGBHSVSlidersDirty, this.m_HDRValues.m_TonemappingType);
			this.m_GTexture = ColorPicker.Update1DSlider(this.m_GTexture, 32, this.m_R, this.m_B, ref this.m_GTextureR, ref this.m_GTextureB, 1, false, colorScale, tonemappingExposureAdjusment, this.m_RGBHSVSlidersDirty, this.m_HDRValues.m_TonemappingType);
			this.m_BTexture = ColorPicker.Update1DSlider(this.m_BTexture, 32, this.m_R, this.m_G, ref this.m_BTextureR, ref this.m_BTextureG, 2, false, colorScale, tonemappingExposureAdjusment, this.m_RGBHSVSlidersDirty, this.m_HDRValues.m_TonemappingType);
			this.m_RGBHSVSlidersDirty = false;
			float displayScale = (!this.m_HDR) ? 255f : colorScale;
			string formatString = (!this.m_HDR) ? EditorGUI.kIntFieldFormatString : EditorGUI.kFloatFieldFormatString;
			this.m_R = this.TexturedSlider(this.m_RTexture, "R", this.m_R, 0f, 1f, displayScale, formatString, new Action<float>(this.OnFloatFieldChanged));
			this.m_G = this.TexturedSlider(this.m_GTexture, "G", this.m_G, 0f, 1f, displayScale, formatString, new Action<float>(this.OnFloatFieldChanged));
			this.m_B = this.TexturedSlider(this.m_BTexture, "B", this.m_B, 0f, 1f, displayScale, formatString, new Action<float>(this.OnFloatFieldChanged));
			if (EditorGUI.EndChangeCheck())
			{
				this.RGBToHSV();
			}
		}

		private static Texture2D Update1DSlider(Texture2D tex, int xSize, float const1, float const2, ref float oldConst1, ref float oldConst2, int idx, bool hsvSpace, float scale, float exposureValue, bool forceUpdate, ColorPicker.TonemappingType tonemappingType)
		{
			if (!tex || const1 != oldConst1 || const2 != oldConst2 || forceUpdate)
			{
				if (!tex)
				{
					tex = ColorPicker.MakeTexture(xSize, 2);
				}
				Color[] array = new Color[xSize * 2];
				Color black = Color.black;
				Color black2 = Color.black;
				switch (idx)
				{
				case 0:
					black = new Color(0f, const1, const2, 1f);
					black2 = new Color(1f, 0f, 0f, 0f);
					break;
				case 1:
					black = new Color(const1, 0f, const2, 1f);
					black2 = new Color(0f, 1f, 0f, 0f);
					break;
				case 2:
					black = new Color(const1, const2, 0f, 1f);
					black2 = new Color(0f, 0f, 1f, 0f);
					break;
				case 3:
					black = new Color(0f, 0f, 0f, 1f);
					black2 = new Color(1f, 1f, 1f, 0f);
					break;
				}
				ColorPicker.FillArea(xSize, 2, array, black, black2, new Color(0f, 0f, 0f, 0f));
				if (hsvSpace)
				{
					ColorPicker.HSVToRGBArray(array, scale);
				}
				else
				{
					ColorPicker.ScaleColors(array, scale);
				}
				ColorPicker.DoTonemapping(array, exposureValue, tonemappingType);
				oldConst1 = const1;
				oldConst2 = const2;
				tex.SetPixels(array);
				tex.Apply();
			}
			return tex;
		}

		private float TexturedSlider(Texture2D background, string text, float val, float min, float max, float displayScale, string formatString, Action<float> onFloatFieldChanged)
		{
			Rect rect = GUILayoutUtility.GetRect(16f, 16f, GUI.skin.label);
			GUI.Label(new Rect(rect.x, rect.y, 20f, 16f), text);
			rect.x += 14f;
			rect.width -= 20f + this.fieldWidth;
			if (Event.current.type == EventType.Repaint)
			{
				Rect screenRect = new Rect(rect.x + 1f, rect.y + 2f, rect.width - 2f, rect.height - 4f);
				Graphics.DrawTexture(screenRect, background, new Rect(0.5f / (float)background.width, 0.5f / (float)background.height, 1f - 1f / (float)background.width, 1f - 1f / (float)background.height), 0, 0, 0, 0, Color.grey);
			}
			int controlID = GUIUtility.GetControlID(869045, FocusType.Keyboard, base.position);
			EditorGUI.BeginChangeCheck();
			val = GUI.HorizontalSlider(new Rect(rect.x, rect.y + 1f, rect.width, rect.height - 2f), val, min, max, ColorPicker.styles.pickerBox, ColorPicker.styles.thumbHoriz);
			if (EditorGUI.EndChangeCheck())
			{
				if (EditorGUI.s_RecycledEditor.IsEditingControl(controlID))
				{
					EditorGUI.s_RecycledEditor.EndEditing();
				}
				val = (float)Math.Round((double)val, 3);
				GUIUtility.keyboardControl = 0;
			}
			Rect position = new Rect(rect.xMax + 6f, rect.y, this.fieldWidth, 16f);
			EditorGUI.BeginChangeCheck();
			val = EditorGUI.DoFloatField(EditorGUI.s_RecycledEditor, position, new Rect(0f, 0f, 0f, 0f), controlID, val * displayScale, formatString, EditorStyles.numberField, false);
			if (EditorGUI.EndChangeCheck() && onFloatFieldChanged != null)
			{
				onFloatFieldChanged(val);
			}
			val = Mathf.Clamp(val / displayScale, min, max);
			GUILayout.Space(3f);
			return val;
		}

		private void HSVSliders()
		{
			EditorGUI.BeginChangeCheck();
			float tonemappingExposureAdjusment = this.GetTonemappingExposureAdjusment();
			float colorScale = this.GetColorScale();
			this.m_HueTexture = ColorPicker.Update1DSlider(this.m_HueTexture, 64, 1f, 1f, ref this.m_HueTextureS, ref this.m_HueTextureV, 0, true, 1f, -1f, this.m_RGBHSVSlidersDirty, this.m_HDRValues.m_TonemappingType);
			this.m_SatTexture = ColorPicker.Update1DSlider(this.m_SatTexture, 32, this.m_H, Mathf.Max(this.m_V, 0.2f), ref this.m_SatTextureH, ref this.m_SatTextureV, 1, true, colorScale, tonemappingExposureAdjusment, this.m_RGBHSVSlidersDirty, this.m_HDRValues.m_TonemappingType);
			this.m_ValTexture = ColorPicker.Update1DSlider(this.m_ValTexture, 32, this.m_H, this.m_S, ref this.m_ValTextureH, ref this.m_ValTextureS, 2, true, colorScale, tonemappingExposureAdjusment, this.m_RGBHSVSlidersDirty, this.m_HDRValues.m_TonemappingType);
			this.m_RGBHSVSlidersDirty = false;
			float displayScale = (!this.m_HDR) ? 255f : colorScale;
			string formatString = (!this.m_HDR) ? EditorGUI.kIntFieldFormatString : EditorGUI.kFloatFieldFormatString;
			this.m_H = this.TexturedSlider(this.m_HueTexture, "H", this.m_H, 0f, 1f, 359f, EditorGUI.kIntFieldFormatString, null);
			this.m_S = this.TexturedSlider(this.m_SatTexture, "S", this.m_S, 0f, 1f, (!this.m_HDR) ? 255f : 1f, formatString, null);
			this.m_V = this.TexturedSlider(this.m_ValTexture, "V", this.m_V, 0f, 1f, displayScale, formatString, null);
			if (EditorGUI.EndChangeCheck())
			{
				this.HSVToRGB();
			}
		}

		private static void FillArea(int xSize, int ySize, Color[] retval, Color topLeftColor, Color rightGradient, Color downGradient)
		{
			Color b = new Color(0f, 0f, 0f, 0f);
			Color b2 = new Color(0f, 0f, 0f, 0f);
			if (xSize > 1)
			{
				b = rightGradient / (float)(xSize - 1);
			}
			if (ySize > 1)
			{
				b2 = downGradient / (float)(ySize - 1);
			}
			Color color = topLeftColor;
			int num = 0;
			for (int i = 0; i < ySize; i++)
			{
				Color color2 = color;
				for (int j = 0; j < xSize; j++)
				{
					retval[num++] = color2;
					color2 += b;
				}
				color += b2;
			}
		}

		private static void ScaleColors(Color[] colors, float scale)
		{
			int num = colors.Length;
			for (int i = 0; i < num; i++)
			{
				colors[i] = colors[i].RGBMultiplied(scale);
			}
		}

		private static void HSVToRGBArray(Color[] colors, float scale)
		{
			int num = colors.Length;
			for (int i = 0; i < num; i++)
			{
				Color color = colors[i];
				Color color2 = Color.HSVToRGB(color.r, color.g, color.b).RGBMultiplied(scale);
				color2.a = color.a;
				colors[i] = color2;
			}
		}

		private static void LinearToGammaArray(Color[] colors)
		{
			int num = colors.Length;
			for (int i = 0; i < num; i++)
			{
				Color color = colors[i];
				Color gamma = color.gamma;
				gamma.a = color.a;
				colors[i] = gamma;
			}
		}

		private float GetTonemappingExposureAdjusment()
		{
			return (!this.m_HDR || !this.m_UseTonemappingPreview) ? -1f : this.m_HDRValues.m_ExposureAdjustment;
		}

		private float GetColorScale()
		{
			float result;
			if (this.m_HDR)
			{
				result = Mathf.Max(1f, this.m_HDRValues.m_HDRScaleFactor);
			}
			else
			{
				result = 1f;
			}
			return result;
		}

		private void DrawColorSlider(Rect colorSliderRect, Vector2 constantValues)
		{
			if (Event.current.type == EventType.Repaint)
			{
				if (this.m_ColorBoxMode != (ColorPicker.ColorBoxMode)this.m_TextureColorSliderMode)
				{
					int num = (int)this.m_ColorSliderSize;
					int num2;
					if (this.m_ColorBoxMode == ColorPicker.ColorBoxMode.SV_H)
					{
						num2 = 64;
					}
					else
					{
						num2 = (int)this.m_ColorSliderSize;
					}
					if (this.m_ColorSlider == null)
					{
						this.m_ColorSlider = ColorPicker.MakeTexture(num, num2);
					}
					if (this.m_ColorSlider.width != num || this.m_ColorSlider.height != num2)
					{
						this.m_ColorSlider.Resize(num, num2);
					}
				}
				if (this.m_ColorBoxMode != (ColorPicker.ColorBoxMode)this.m_TextureColorSliderMode || constantValues != this.m_LastConstantValues || this.m_ColorSliderDirty)
				{
					float tonemappingExposureAdjusment = this.GetTonemappingExposureAdjusment();
					float colorScale = this.GetColorScale();
					Color[] pixels = this.m_ColorSlider.GetPixels(0);
					int width = this.m_ColorSlider.width;
					int height = this.m_ColorSlider.height;
					switch (this.m_ColorBoxMode)
					{
					case ColorPicker.ColorBoxMode.SV_H:
						ColorPicker.FillArea(width, height, pixels, new Color(0f, 1f, 1f, 1f), new Color(0f, 0f, 0f, 0f), new Color(1f, 0f, 0f, 0f));
						ColorPicker.HSVToRGBArray(pixels, 1f);
						break;
					case ColorPicker.ColorBoxMode.HV_S:
						ColorPicker.FillArea(width, height, pixels, new Color(this.m_H, 0f, Mathf.Max(this.m_V, 0.3f), 1f), new Color(0f, 0f, 0f, 0f), new Color(0f, 1f, 0f, 0f));
						ColorPicker.HSVToRGBArray(pixels, colorScale);
						break;
					case ColorPicker.ColorBoxMode.HS_V:
						ColorPicker.FillArea(width, height, pixels, new Color(this.m_H, this.m_S, 0f, 1f), new Color(0f, 0f, 0f, 0f), new Color(0f, 0f, 1f, 0f));
						ColorPicker.HSVToRGBArray(pixels, colorScale);
						break;
					case ColorPicker.ColorBoxMode.BG_R:
						ColorPicker.FillArea(width, height, pixels, new Color(0f, this.m_G * colorScale, this.m_B * colorScale, 1f), new Color(0f, 0f, 0f, 0f), new Color(colorScale, 0f, 0f, 0f));
						break;
					case ColorPicker.ColorBoxMode.BR_G:
						ColorPicker.FillArea(width, height, pixels, new Color(this.m_R * colorScale, 0f, this.m_B * colorScale, 1f), new Color(0f, 0f, 0f, 0f), new Color(0f, colorScale, 0f, 0f));
						break;
					case ColorPicker.ColorBoxMode.RG_B:
						ColorPicker.FillArea(width, height, pixels, new Color(this.m_R * colorScale, this.m_G * colorScale, 0f, 1f), new Color(0f, 0f, 0f, 0f), new Color(0f, 0f, colorScale, 0f));
						break;
					}
					if (QualitySettings.activeColorSpace == ColorSpace.Linear)
					{
						ColorPicker.LinearToGammaArray(pixels);
					}
					if (this.m_ColorBoxMode != ColorPicker.ColorBoxMode.SV_H)
					{
						ColorPicker.DoTonemapping(pixels, tonemappingExposureAdjusment, this.m_HDRValues.m_TonemappingType);
					}
					this.m_ColorSlider.SetPixels(pixels, 0);
					this.m_ColorSlider.Apply(true);
				}
				Graphics.DrawTexture(colorSliderRect, this.m_ColorSlider, new Rect(0.5f / (float)this.m_ColorSlider.width, 0.5f / (float)this.m_ColorSlider.height, 1f - 1f / (float)this.m_ColorSlider.width, 1f - 1f / (float)this.m_ColorSlider.height), 0, 0, 0, 0, Color.grey);
			}
		}

		public static Texture2D MakeTexture(int width, int height)
		{
			return new Texture2D(width, height, TextureFormat.RGBA32, false)
			{
				hideFlags = HideFlags.HideAndDontSave,
				wrapMode = TextureWrapMode.Clamp,
				hideFlags = HideFlags.HideAndDontSave
			};
		}

		private void DrawColorSpaceBox(Rect colorBoxRect, float constantValue)
		{
			if (Event.current.type == EventType.Repaint)
			{
				if (this.m_ColorBoxMode != (ColorPicker.ColorBoxMode)this.m_TextureColorBoxMode)
				{
					int num = 32;
					int num2;
					if (this.m_ColorBoxMode == ColorPicker.ColorBoxMode.HV_S || this.m_ColorBoxMode == ColorPicker.ColorBoxMode.HS_V)
					{
						num2 = 64;
					}
					else
					{
						num2 = 32;
					}
					if (this.m_ColorBox == null)
					{
						this.m_ColorBox = ColorPicker.MakeTexture(num2, num);
					}
					if (this.m_ColorBox.width != num2 || this.m_ColorBox.height != num)
					{
						this.m_ColorBox.Resize(num2, num);
					}
				}
				if (this.m_ColorBoxMode != (ColorPicker.ColorBoxMode)this.m_TextureColorBoxMode || this.m_LastConstant != constantValue || this.m_ColorSpaceBoxDirty)
				{
					float tonemappingExposureAdjusment = this.GetTonemappingExposureAdjusment();
					float colorScale = this.GetColorScale();
					this.m_Colors = this.m_ColorBox.GetPixels(0);
					int width = this.m_ColorBox.width;
					int height = this.m_ColorBox.height;
					switch (this.m_ColorBoxMode)
					{
					case ColorPicker.ColorBoxMode.SV_H:
						ColorPicker.FillArea(width, height, this.m_Colors, new Color(this.m_H, 0f, 0f, 1f), new Color(0f, 1f, 0f, 0f), new Color(0f, 0f, 1f, 0f));
						ColorPicker.HSVToRGBArray(this.m_Colors, colorScale);
						break;
					case ColorPicker.ColorBoxMode.HV_S:
						ColorPicker.FillArea(width, height, this.m_Colors, new Color(0f, this.m_S, 0f, 1f), new Color(1f, 0f, 0f, 0f), new Color(0f, 0f, 1f, 0f));
						ColorPicker.HSVToRGBArray(this.m_Colors, colorScale);
						break;
					case ColorPicker.ColorBoxMode.HS_V:
						ColorPicker.FillArea(width, height, this.m_Colors, new Color(0f, 0f, this.m_V * colorScale, 1f), new Color(1f, 0f, 0f, 0f), new Color(0f, 1f, 0f, 0f));
						ColorPicker.HSVToRGBArray(this.m_Colors, 1f);
						break;
					case ColorPicker.ColorBoxMode.BG_R:
						ColorPicker.FillArea(width, height, this.m_Colors, new Color(this.m_R * colorScale, 0f, 0f, 1f), new Color(0f, 0f, colorScale, 0f), new Color(0f, colorScale, 0f, 0f));
						break;
					case ColorPicker.ColorBoxMode.BR_G:
						ColorPicker.FillArea(width, height, this.m_Colors, new Color(0f, this.m_G * colorScale, 0f, 1f), new Color(0f, 0f, colorScale, 0f), new Color(colorScale, 0f, 0f, 0f));
						break;
					case ColorPicker.ColorBoxMode.RG_B:
						ColorPicker.FillArea(width, height, this.m_Colors, new Color(0f, 0f, this.m_B * colorScale, 1f), new Color(colorScale, 0f, 0f, 0f), new Color(0f, colorScale, 0f, 0f));
						break;
					}
					if (QualitySettings.activeColorSpace == ColorSpace.Linear)
					{
						ColorPicker.LinearToGammaArray(this.m_Colors);
					}
					ColorPicker.DoTonemapping(this.m_Colors, tonemappingExposureAdjusment, this.m_HDRValues.m_TonemappingType);
					this.m_ColorBox.SetPixels(this.m_Colors, 0);
					this.m_ColorBox.Apply(true);
					this.m_LastConstant = constantValue;
					this.m_TextureColorBoxMode = (int)this.m_ColorBoxMode;
				}
				Graphics.DrawTexture(colorBoxRect, this.m_ColorBox, new Rect(0.5f / (float)this.m_ColorBox.width, 0.5f / (float)this.m_ColorBox.height, 1f - 1f / (float)this.m_ColorBox.width, 1f - 1f / (float)this.m_ColorBox.height), 0, 0, 0, 0, Color.grey);
				ColorPicker.DrawLabelOutsideRect(colorBoxRect, this.GetXAxisLabel(this.m_ColorBoxMode), ColorPicker.LabelLocation.Bottom);
				ColorPicker.DrawLabelOutsideRect(colorBoxRect, this.GetYAxisLabel(this.m_ColorBoxMode), ColorPicker.LabelLocation.Left);
			}
		}

		private string GetXAxisLabel(ColorPicker.ColorBoxMode colorBoxMode)
		{
			return this.m_ColorBoxXAxisLabels[(int)colorBoxMode];
		}

		private string GetYAxisLabel(ColorPicker.ColorBoxMode colorBoxMode)
		{
			return this.m_ColorBoxYAxisLabels[(int)colorBoxMode];
		}

		private string GetZAxisLabel(ColorPicker.ColorBoxMode colorBoxMode)
		{
			return this.m_ColorBoxZAxisLabels[(int)colorBoxMode];
		}

		private static void DrawLabelOutsideRect(Rect position, string label, ColorPicker.LabelLocation labelLocation)
		{
			Matrix4x4 matrix = GUI.matrix;
			Rect position2 = new Rect(position.x, position.y - 18f, position.width, 16f);
			switch (labelLocation)
			{
			case ColorPicker.LabelLocation.Bottom:
				position2 = new Rect(position.x, position.yMax, position.width, 16f);
				break;
			case ColorPicker.LabelLocation.Left:
				GUIUtility.RotateAroundPivot(-90f, position.center);
				break;
			case ColorPicker.LabelLocation.Right:
				GUIUtility.RotateAroundPivot(90f, position.center);
				break;
			}
			using (new EditorGUI.DisabledScope(true))
			{
				GUI.Label(position2, label, ColorPicker.styles.label);
			}
			GUI.matrix = matrix;
		}

		private void InitIfNeeded()
		{
			if (ColorPicker.styles == null)
			{
				ColorPicker.styles = new ColorPicker.Styles();
			}
			if (this.m_ColorLibraryEditorState == null)
			{
				this.m_ColorLibraryEditorState = new PresetLibraryEditorState(ColorPicker.presetsEditorPrefID);
				this.m_ColorLibraryEditorState.TransferEditorPrefsState(true);
			}
			if (this.m_ColorLibraryEditor == null)
			{
				ScriptableObjectSaveLoadHelper<ColorPresetLibrary> helper = new ScriptableObjectSaveLoadHelper<ColorPresetLibrary>("colors", SaveType.Text);
				this.m_ColorLibraryEditor = new PresetLibraryEditor<ColorPresetLibrary>(helper, this.m_ColorLibraryEditorState, new Action<int, object>(this.PresetClickedCallback));
				this.m_ColorLibraryEditor.previewAspect = 1f;
				this.m_ColorLibraryEditor.minMaxPreviewHeight = new Vector2(14f, 14f);
				this.m_ColorLibraryEditor.settingsMenuRightMargin = 2f;
				this.m_ColorLibraryEditor.useOnePixelOverlappedGrid = true;
				this.m_ColorLibraryEditor.alwaysShowScrollAreaHorizontalLines = false;
				this.m_ColorLibraryEditor.marginsForGrid = new RectOffset(0, 0, 0, 0);
				this.m_ColorLibraryEditor.marginsForList = new RectOffset(0, 5, 2, 2);
				this.m_ColorLibraryEditor.InitializeGrid(233f - (float)(ColorPicker.styles.background.padding.left + ColorPicker.styles.background.padding.right));
			}
		}

		private void PresetClickedCallback(int clickCount, object presetObject)
		{
			Color color = (Color)presetObject;
			if (!this.m_HDR && color.maxColorComponent > 1f)
			{
				color = color.RGBMultiplied(1f / color.maxColorComponent);
			}
			this.SetColor(color);
			this.colorChanged = true;
		}

		private void DoColorSwatchAndEyedropper()
		{
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			if (GUILayout.Button(ColorPicker.styles.eyeDropper, GUIStyle.none, new GUILayoutOption[]
			{
				GUILayout.Width(40f),
				GUILayout.ExpandWidth(false)
			}))
			{
				EyeDropper.Start(this.m_Parent);
				this.m_ColorBoxMode = ColorPicker.ColorBoxMode.EyeDropper;
				GUIUtility.ExitGUI();
			}
			Color color = new Color(this.m_R, this.m_G, this.m_B, this.m_A);
			if (this.m_HDR)
			{
				color = ColorPicker.color;
			}
			Rect rect = GUILayoutUtility.GetRect(20f, 20f, 20f, 20f, ColorPicker.styles.colorPickerBox, new GUILayoutOption[]
			{
				GUILayout.ExpandWidth(true)
			});
			EditorGUIUtility.DrawColorSwatch(rect, color, this.m_ShowAlpha, this.m_HDR);
			if (Event.current.type == EventType.Repaint)
			{
				ColorPicker.styles.pickerBox.Draw(rect, GUIContent.none, false, false, false, false);
			}
			GUILayout.EndHorizontal();
		}

		private void DoColorSpaceGUI()
		{
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.FlexibleSpace();
			if (GUILayout.Button(ColorPicker.styles.colorCycle, GUIStyle.none, new GUILayoutOption[]
			{
				GUILayout.ExpandWidth(false)
			}))
			{
				this.m_OldColorBoxMode = (this.m_ColorBoxMode = (this.m_ColorBoxMode + 1) % ColorPicker.ColorBoxMode.EyeDropper);
			}
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.Space(20f);
			GUILayout.BeginVertical(new GUILayoutOption[0]);
			GUILayout.Space(7f);
			bool changed = GUI.changed;
			GUILayout.BeginHorizontal(new GUILayoutOption[]
			{
				GUILayout.ExpandHeight(false)
			});
			Rect aspectRect = GUILayoutUtility.GetAspectRect(1f, ColorPicker.styles.pickerBox, new GUILayoutOption[]
			{
				GUILayout.MinWidth(64f),
				GUILayout.MinHeight(64f),
				GUILayout.MaxWidth(256f),
				GUILayout.MaxHeight(256f)
			});
			EditorGUILayout.Space();
			Rect rect = GUILayoutUtility.GetRect(8f, 32f, 64f, 128f, ColorPicker.styles.pickerBox);
			rect.height = aspectRect.height;
			GUILayout.EndHorizontal();
			GUI.changed = false;
			switch (this.m_ColorBoxMode)
			{
			case ColorPicker.ColorBoxMode.SV_H:
				this.Slider3D(aspectRect, rect, ref this.m_S, ref this.m_V, ref this.m_H, ColorPicker.styles.pickerBox, ColorPicker.styles.thumb2D, ColorPicker.styles.thumbVert);
				if (GUI.changed)
				{
					this.HSVToRGB();
				}
				break;
			case ColorPicker.ColorBoxMode.HV_S:
				this.Slider3D(aspectRect, rect, ref this.m_H, ref this.m_V, ref this.m_S, ColorPicker.styles.pickerBox, ColorPicker.styles.thumb2D, ColorPicker.styles.thumbVert);
				if (GUI.changed)
				{
					this.HSVToRGB();
				}
				break;
			case ColorPicker.ColorBoxMode.HS_V:
				this.Slider3D(aspectRect, rect, ref this.m_H, ref this.m_S, ref this.m_V, ColorPicker.styles.pickerBox, ColorPicker.styles.thumb2D, ColorPicker.styles.thumbVert);
				if (GUI.changed)
				{
					this.HSVToRGB();
				}
				break;
			case ColorPicker.ColorBoxMode.BG_R:
				this.Slider3D(aspectRect, rect, ref this.m_B, ref this.m_G, ref this.m_R, ColorPicker.styles.pickerBox, ColorPicker.styles.thumb2D, ColorPicker.styles.thumbVert);
				if (GUI.changed)
				{
					this.RGBToHSV();
				}
				break;
			case ColorPicker.ColorBoxMode.BR_G:
				this.Slider3D(aspectRect, rect, ref this.m_B, ref this.m_R, ref this.m_G, ColorPicker.styles.pickerBox, ColorPicker.styles.thumb2D, ColorPicker.styles.thumbVert);
				if (GUI.changed)
				{
					this.RGBToHSV();
				}
				break;
			case ColorPicker.ColorBoxMode.RG_B:
				this.Slider3D(aspectRect, rect, ref this.m_R, ref this.m_G, ref this.m_B, ColorPicker.styles.pickerBox, ColorPicker.styles.thumb2D, ColorPicker.styles.thumbVert);
				if (GUI.changed)
				{
					this.RGBToHSV();
				}
				break;
			case ColorPicker.ColorBoxMode.EyeDropper:
				EyeDropper.DrawPreview(Rect.MinMaxRect(aspectRect.x, aspectRect.y, rect.xMax, aspectRect.yMax));
				break;
			}
			GUI.changed |= changed;
			GUILayout.Space(5f);
			GUILayout.EndVertical();
			GUILayout.Space(20f);
			GUILayout.EndHorizontal();
		}

		private void SetHDRScaleFactor(float value)
		{
			if (!this.m_HDR)
			{
				Debug.LogError("HDR scale is being set in LDR mode!");
			}
			if (value < 1f)
			{
				Debug.LogError("SetHDRScaleFactor is below 1, should be >= 1!");
			}
			this.m_HDRValues.m_HDRScaleFactor = Mathf.Clamp(value, 0f, this.m_HDRConfig.maxBrightness);
			this.m_ColorSliderDirty = true;
			this.m_ColorSpaceBoxDirty = true;
			this.m_RGBHSVSlidersDirty = true;
		}

		private void BrightnessField(float availableWidth)
		{
			if (this.m_HDR)
			{
				float labelWidth = EditorGUIUtility.labelWidth;
				EditorGUI.BeginChangeCheck();
				EditorGUI.indentLevel++;
				EditorGUIUtility.labelWidth = availableWidth - 40f - EditorGUI.indent;
				Color color = EditorGUILayout.ColorBrightnessField(GUIContent.Temp("Current Brightness"), ColorPicker.color, this.m_HDRConfig.minBrightness, this.m_HDRConfig.maxBrightness, new GUILayoutOption[0]);
				EditorGUI.indentLevel--;
				if (EditorGUI.EndChangeCheck())
				{
					float maxColorComponent = color.maxColorComponent;
					if (maxColorComponent > this.m_HDRValues.m_HDRScaleFactor)
					{
						this.SetHDRScaleFactor(maxColorComponent);
					}
					this.SetNormalizedColor(color.RGBMultiplied(1f / this.m_HDRValues.m_HDRScaleFactor));
				}
				EditorGUIUtility.labelWidth = labelWidth;
			}
		}

		private void SetMaxDisplayBrightness(float brightness)
		{
			brightness = Mathf.Clamp(brightness, 1f, this.m_HDRConfig.maxBrightness);
			if (brightness != this.m_HDRValues.m_HDRScaleFactor)
			{
				Color normalizedColor = ColorPicker.color.RGBMultiplied(1f / brightness);
				float maxColorComponent = normalizedColor.maxColorComponent;
				if (maxColorComponent <= 1f)
				{
					this.SetNormalizedColor(normalizedColor);
					this.SetHDRScaleFactor(brightness);
					base.Repaint();
				}
			}
		}

		private void DoColorSliders()
		{
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.FlexibleSpace();
			if (GUILayout.Button(ColorPicker.styles.sliderCycle, GUIStyle.none, new GUILayoutOption[]
			{
				GUILayout.ExpandWidth(false)
			}))
			{
				this.m_SliderMode = (this.m_SliderMode + 1) % (ColorPicker.SliderMode)2;
			}
			GUILayout.EndHorizontal();
			GUILayout.Space(7f);
			ColorPicker.SliderMode sliderMode = this.m_SliderMode;
			if (sliderMode != ColorPicker.SliderMode.HSV)
			{
				if (sliderMode == ColorPicker.SliderMode.RGB)
				{
					this.RGBSliders();
				}
			}
			else
			{
				this.HSVSliders();
			}
			if (this.m_ShowAlpha)
			{
				this.m_AlphaTexture = ColorPicker.Update1DSlider(this.m_AlphaTexture, 32, 0f, 0f, ref this.m_OldAlpha, ref this.m_OldAlpha, 3, false, 1f, -1f, false, this.m_HDRValues.m_TonemappingType);
				float displayScale = (!this.m_HDR) ? 255f : 1f;
				string formatString = (!this.m_HDR) ? EditorGUI.kIntFieldFormatString : EditorGUI.kFloatFieldFormatString;
				this.m_A = this.TexturedSlider(this.m_AlphaTexture, "A", this.m_A, 0f, 1f, displayScale, formatString, null);
			}
		}

		private void DoHexField(float availableWidth)
		{
			float labelWidth = EditorGUIUtility.labelWidth;
			float fieldWidth = EditorGUIUtility.fieldWidth;
			EditorGUIUtility.labelWidth = availableWidth - 85f;
			EditorGUIUtility.fieldWidth = 85f;
			EditorGUI.indentLevel++;
			EditorGUI.BeginChangeCheck();
			Color normalizedColor = EditorGUILayout.HexColorTextField(GUIContent.Temp("Hex Color"), ColorPicker.color, this.m_ShowAlpha, new GUILayoutOption[0]);
			if (EditorGUI.EndChangeCheck())
			{
				this.SetNormalizedColor(normalizedColor);
				if (this.m_HDR)
				{
					this.SetHDRScaleFactor(1f);
				}
			}
			EditorGUI.indentLevel--;
			EditorGUIUtility.labelWidth = labelWidth;
			EditorGUIUtility.fieldWidth = fieldWidth;
		}

		private void DoPresetsGUI()
		{
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			this.m_ShowPresets = GUILayout.Toggle(this.m_ShowPresets, ColorPicker.styles.presetsToggle, ColorPicker.styles.foldout, new GUILayoutOption[0]);
			GUILayout.Space(17f);
			GUILayout.EndHorizontal();
			if (this.m_ShowPresets)
			{
				GUILayout.Space(-18f);
				Rect rect = GUILayoutUtility.GetRect(0f, Mathf.Clamp(this.m_ColorLibraryEditor.contentHeight, 20f, 250f));
				this.m_ColorLibraryEditor.OnGUI(rect, ColorPicker.color);
			}
		}

		private void OnGUI()
		{
			this.InitIfNeeded();
			if (this.m_resetKeyboardControl)
			{
				GUIUtility.keyboardControl = 0;
				this.m_resetKeyboardControl = false;
			}
			EventType type = Event.current.type;
			if (type == EventType.ExecuteCommand)
			{
				string commandName = Event.current.commandName;
				if (commandName != null)
				{
					if (!(commandName == "EyeDropperUpdate"))
					{
						if (!(commandName == "EyeDropperClicked"))
						{
							if (commandName == "EyeDropperCancelled")
							{
								base.Repaint();
								this.m_ColorBoxMode = this.m_OldColorBoxMode;
							}
						}
						else
						{
							Color lastPickedColor = EyeDropper.GetLastPickedColor();
							this.m_R = lastPickedColor.r;
							this.m_G = lastPickedColor.g;
							this.m_B = lastPickedColor.b;
							this.RGBToHSV();
							this.m_ColorBoxMode = this.m_OldColorBoxMode;
							this.m_Color = new Color(this.m_R, this.m_G, this.m_B, this.m_A);
							this.SendEvent(true);
						}
					}
					else
					{
						base.Repaint();
					}
				}
			}
			Rect rect = EditorGUILayout.BeginVertical(ColorPicker.styles.background, new GUILayoutOption[0]);
			float width = EditorGUILayout.GetControlRect(false, 1f, EditorStyles.numberField, new GUILayoutOption[0]).width;
			EditorGUIUtility.labelWidth = width - this.fieldWidth;
			EditorGUIUtility.fieldWidth = this.fieldWidth;
			EditorGUI.BeginChangeCheck();
			GUILayout.Space(10f);
			this.DoColorSwatchAndEyedropper();
			GUILayout.Space(10f);
			if (this.m_HDR)
			{
				this.TonemappingControls();
				GUILayout.Space(10f);
			}
			this.DoColorSpaceGUI();
			GUILayout.Space(10f);
			if (this.m_HDR)
			{
				GUILayout.Space(5f);
				this.BrightnessField(width);
				GUILayout.Space(10f);
			}
			this.DoColorSliders();
			GUILayout.Space(5f);
			this.DoHexField(width);
			GUILayout.Space(10f);
			if (EditorGUI.EndChangeCheck())
			{
				this.colorChanged = true;
			}
			this.DoPresetsGUI();
			this.HandleCopyPasteEvents();
			if (this.colorChanged)
			{
				this.colorChanged = false;
				this.m_Color = new Color(this.m_R, this.m_G, this.m_B, this.m_A);
				this.SendEvent(true);
			}
			EditorGUILayout.EndVertical();
			if (rect.height > 0f && Event.current.type == EventType.Repaint)
			{
				this.SetHeight(rect.height);
			}
			if (Event.current.type == EventType.KeyDown)
			{
				KeyCode keyCode = Event.current.keyCode;
				if (keyCode != KeyCode.Escape)
				{
					if (keyCode == KeyCode.Return || keyCode == KeyCode.KeypadEnter)
					{
						base.Close();
					}
				}
				else
				{
					Undo.RevertAllDownToGroup(this.m_ModalUndoGroup);
					this.m_Color = this.m_OriginalColor;
					this.SendEvent(false);
					base.Close();
					GUIUtility.ExitGUI();
				}
			}
			if ((Event.current.type == EventType.MouseDown && Event.current.button != 1) || Event.current.type == EventType.ContextClick)
			{
				GUIUtility.keyboardControl = 0;
				base.Repaint();
			}
		}

		private void SetHeight(float newHeight)
		{
			if (newHeight != base.position.height)
			{
				base.minSize = new Vector2(233f, newHeight);
				base.maxSize = new Vector2(233f, newHeight);
			}
		}

		private void HandleCopyPasteEvents()
		{
			Event current = Event.current;
			EventType type = current.type;
			if (type != EventType.ValidateCommand)
			{
				if (type == EventType.ExecuteCommand)
				{
					string commandName = current.commandName;
					if (commandName != null)
					{
						if (!(commandName == "Copy"))
						{
							if (commandName == "Paste")
							{
								Color color;
								if (ColorClipboard.TryGetColor(this.m_HDR, out color))
								{
									if (!this.m_ShowAlpha)
									{
										color.a = this.m_A;
									}
									this.SetColor(color);
									this.colorChanged = true;
									GUI.changed = true;
									current.Use();
								}
							}
						}
						else
						{
							ColorClipboard.SetColor(ColorPicker.color);
							current.Use();
						}
					}
				}
			}
			else
			{
				string commandName2 = current.commandName;
				if (commandName2 != null)
				{
					if (commandName2 == "Copy" || commandName2 == "Paste")
					{
						current.Use();
					}
				}
			}
		}

		private float GetScrollWheelDeltaInRect(Rect rect)
		{
			Event current = Event.current;
			float result;
			if (current.type == EventType.ScrollWheel)
			{
				if (rect.Contains(current.mousePosition))
				{
					result = current.delta.y;
					return result;
				}
			}
			result = 0f;
			return result;
		}

		private void Slider3D(Rect boxPos, Rect sliderPos, ref float x, ref float y, ref float z, GUIStyle box, GUIStyle thumb2D, GUIStyle thumbHoriz)
		{
			Rect colorBoxRect = boxPos;
			colorBoxRect.x += 1f;
			colorBoxRect.y += 1f;
			colorBoxRect.width -= 2f;
			colorBoxRect.height -= 2f;
			this.DrawColorSpaceBox(colorBoxRect, z);
			Vector2 value = new Vector2(x, 1f - y);
			value = this.Slider2D(boxPos, value, new Vector2(0f, 0f), new Vector2(1f, 1f), box, thumb2D);
			x = value.x;
			y = 1f - value.y;
			if (this.m_HDR)
			{
				this.SpecialHDRBrightnessHandling(boxPos, sliderPos);
			}
			Rect colorSliderRect = new Rect(sliderPos.x + 1f, sliderPos.y + 1f, sliderPos.width - 2f, sliderPos.height - 2f);
			this.DrawColorSlider(colorSliderRect, new Vector2(x, y));
			if (Event.current.type == EventType.MouseDown && sliderPos.Contains(Event.current.mousePosition))
			{
				this.RemoveFocusFromActiveTextField();
			}
			z = GUI.VerticalSlider(sliderPos, z, 1f, 0f, box, thumbHoriz);
			ColorPicker.DrawLabelOutsideRect(new Rect(sliderPos.xMax - sliderPos.height, sliderPos.y, sliderPos.height + 1f, sliderPos.height + 1f), this.GetZAxisLabel(this.m_ColorBoxMode), ColorPicker.LabelLocation.Right);
		}

		private void RemoveFocusFromActiveTextField()
		{
			EditorGUI.EndEditingActiveTextField();
			GUIUtility.keyboardControl = 0;
		}

		public static Texture2D GetGradientTextureWithAlpha1To0()
		{
			Texture2D arg_51_0;
			if ((arg_51_0 = ColorPicker.s_LeftGradientTexture) == null)
			{
				arg_51_0 = (ColorPicker.s_LeftGradientTexture = ColorPicker.CreateGradientTexture("ColorPicker_1To0_Gradient", 4, 4, new Color(1f, 1f, 1f, 1f), new Color(1f, 1f, 1f, 0f)));
			}
			return arg_51_0;
		}

		public static Texture2D GetGradientTextureWithAlpha0To1()
		{
			Texture2D arg_51_0;
			if ((arg_51_0 = ColorPicker.s_RightGradientTexture) == null)
			{
				arg_51_0 = (ColorPicker.s_RightGradientTexture = ColorPicker.CreateGradientTexture("ColorPicker_0To1_Gradient", 4, 4, new Color(1f, 1f, 1f, 0f), new Color(1f, 1f, 1f, 1f)));
			}
			return arg_51_0;
		}

		private static Texture2D CreateGradientTexture(string name, int width, int height, Color leftColor, Color rightColor)
		{
			Texture2D texture2D = new Texture2D(width, height, TextureFormat.RGBA32, false);
			texture2D.name = name;
			texture2D.hideFlags = HideFlags.HideAndDontSave;
			Color[] array = new Color[width * height];
			for (int i = 0; i < width; i++)
			{
				Color color = Color.Lerp(leftColor, rightColor, (float)i / (float)(width - 1));
				for (int j = 0; j < height; j++)
				{
					array[j * width + i] = color;
				}
			}
			texture2D.SetPixels(array);
			texture2D.wrapMode = TextureWrapMode.Clamp;
			texture2D.Apply();
			return texture2D;
		}

		private void TonemappingControls()
		{
			bool flag = false;
			EditorGUI.BeginChangeCheck();
			this.m_UseTonemappingPreview = GUILayout.Toggle(this.m_UseTonemappingPreview, ColorPicker.styles.tonemappingToggle, ColorPicker.styles.toggle, new GUILayoutOption[0]);
			if (EditorGUI.EndChangeCheck())
			{
				flag = true;
			}
			if (this.m_UseTonemappingPreview)
			{
				EditorGUI.indentLevel++;
				EditorGUI.BeginChangeCheck();
				float power = (QualitySettings.activeColorSpace != ColorSpace.Linear) ? 2f : 1f;
				this.m_HDRValues.m_ExposureAdjustment = EditorGUILayout.PowerSlider("", this.m_HDRValues.m_ExposureAdjustment, this.m_HDRConfig.minExposureValue, this.m_HDRConfig.maxExposureValue, power, new GUILayoutOption[0]);
				if (Event.current.type == EventType.Repaint)
				{
					GUI.Label(EditorGUILayout.s_LastRect, GUIContent.Temp("", "Exposure value"));
				}
				if (EditorGUI.EndChangeCheck())
				{
					flag = true;
				}
				Rect controlRect = EditorGUILayout.GetControlRect(true, 16f, EditorStyles.numberField, new GUILayoutOption[0]);
				EditorGUI.LabelField(controlRect, GUIContent.Temp("Tonemapped Color"));
				Rect position = new Rect(controlRect.xMax - this.fieldWidth, controlRect.y, this.fieldWidth, controlRect.height);
				EditorGUIUtility.DrawColorSwatch(position, ColorPicker.DoTonemapping(ColorPicker.color, this.m_HDRValues.m_ExposureAdjustment), false, false);
				GUI.Label(position, GUIContent.none, ColorPicker.styles.colorPickerBox);
				EditorGUI.indentLevel--;
			}
			if (flag)
			{
				this.m_RGBHSVSlidersDirty = true;
				this.m_ColorSpaceBoxDirty = true;
				this.m_ColorSliderDirty = true;
			}
		}

		private static float PhotographicTonemapping(float value, float exposureAdjustment)
		{
			return 1f - Mathf.Pow(2f, -exposureAdjustment * value);
		}

		private static Color DoTonemapping(Color col, float exposureAdjustment)
		{
			col.r = ColorPicker.PhotographicTonemapping(col.r, exposureAdjustment);
			col.g = ColorPicker.PhotographicTonemapping(col.g, exposureAdjustment);
			col.b = ColorPicker.PhotographicTonemapping(col.b, exposureAdjustment);
			return col;
		}

		private static void DoTonemapping(Color[] colors, float exposureAdjustment, ColorPicker.TonemappingType tonemappingType)
		{
			if (exposureAdjustment >= 0f)
			{
				if (tonemappingType == ColorPicker.TonemappingType.Linear)
				{
					for (int i = 0; i < colors.Length; i++)
					{
						colors[i] = colors[i].RGBMultiplied(exposureAdjustment);
					}
				}
				else
				{
					for (int j = 0; j < colors.Length; j++)
					{
						colors[j] = ColorPicker.DoTonemapping(colors[j], exposureAdjustment);
					}
				}
			}
		}

		private void SpecialHDRBrightnessHandling(Rect boxPos, Rect sliderPos)
		{
			if (this.m_ColorBoxMode == ColorPicker.ColorBoxMode.SV_H || this.m_ColorBoxMode == ColorPicker.ColorBoxMode.HV_S)
			{
				float scrollWheelDeltaInRect = this.GetScrollWheelDeltaInRect(boxPos);
				if (scrollWheelDeltaInRect != 0f)
				{
					this.SetMaxDisplayBrightness(this.m_HDRValues.m_HDRScaleFactor - scrollWheelDeltaInRect * 0.05f);
				}
				Rect rect = new Rect(0f, boxPos.y - 7f, boxPos.x - 2f, 14f);
				Rect dragRect = rect;
				dragRect.y += rect.height;
				EditorGUI.BeginChangeCheck();
				float maxDisplayBrightness = ColorPicker.EditableAxisLabel(rect, dragRect, this.m_HDRValues.m_HDRScaleFactor, 1f, this.m_HDRConfig.maxBrightness, ColorPicker.styles.axisLabelNumberField);
				if (EditorGUI.EndChangeCheck())
				{
					this.SetMaxDisplayBrightness(maxDisplayBrightness);
				}
			}
			if (this.m_ColorBoxMode == ColorPicker.ColorBoxMode.HS_V)
			{
				Rect rect2 = new Rect(sliderPos.xMax + 2f, sliderPos.y - 7f, base.position.width - sliderPos.xMax - 2f, 14f);
				Rect dragRect2 = rect2;
				dragRect2.y += rect2.height;
				EditorGUI.BeginChangeCheck();
				float maxDisplayBrightness2 = ColorPicker.EditableAxisLabel(rect2, dragRect2, this.m_HDRValues.m_HDRScaleFactor, 1f, this.m_HDRConfig.maxBrightness, ColorPicker.styles.axisLabelNumberField);
				if (EditorGUI.EndChangeCheck())
				{
					this.SetMaxDisplayBrightness(maxDisplayBrightness2);
				}
			}
		}

		private static float EditableAxisLabel(Rect rect, Rect dragRect, float value, float minValue, float maxValue, GUIStyle style)
		{
			int controlID = GUIUtility.GetControlID(162594855, FocusType.Keyboard, rect);
			string kFloatFieldFormatString = EditorGUI.kFloatFieldFormatString;
			EditorGUI.kFloatFieldFormatString = ((value >= 10f) ? "n0" : "n1");
			float value2 = EditorGUI.DoFloatField(EditorGUI.s_RecycledEditor, rect, dragRect, controlID, value, EditorGUI.kFloatFieldFormatString, style, true);
			EditorGUI.kFloatFieldFormatString = kFloatFieldFormatString;
			return Mathf.Clamp(value2, minValue, maxValue);
		}

		private void SendEvent(bool exitGUI)
		{
			if (this.m_DelegateView)
			{
				Event e = EditorGUIUtility.CommandEvent("ColorPickerChanged");
				if (!this.m_IsOSColorPicker)
				{
					base.Repaint();
				}
				this.m_DelegateView.SendEvent(e);
				if (!this.m_IsOSColorPicker && exitGUI)
				{
					GUIUtility.ExitGUI();
				}
			}
		}

		private void SetNormalizedColor(Color c)
		{
			if (c.maxColorComponent > 1f)
			{
				Debug.LogError("Setting normalized color with a non-normalized color: " + c);
			}
			this.m_Color = c;
			this.m_R = c.r;
			this.m_G = c.g;
			this.m_B = c.b;
			this.RGBToHSV();
			this.m_A = c.a;
		}

		private void SetColor(Color c)
		{
			if (this.m_IsOSColorPicker)
			{
				OSColorPicker.color = c;
			}
			else
			{
				float hDRScaleFactor = this.m_HDRValues.m_HDRScaleFactor;
				if (this.m_HDR)
				{
					float maxColorComponent = c.maxColorComponent;
					if (maxColorComponent > 1f)
					{
						c = c.RGBMultiplied(1f / maxColorComponent);
					}
					this.SetHDRScaleFactor(Mathf.Max(1f, maxColorComponent));
				}
				if (this.m_Color.r != c.r || this.m_Color.g != c.g || this.m_Color.b != c.b || this.m_Color.a != c.a || hDRScaleFactor != this.m_HDRValues.m_HDRScaleFactor)
				{
					if (c.r > 1f || c.g > 1f || c.b > 1f)
					{
						Debug.LogError(string.Format("Invalid normalized color: {0}, normalize value: {1}", c, this.m_HDRValues.m_HDRScaleFactor));
					}
					this.m_resetKeyboardControl = true;
					this.SetNormalizedColor(c);
					base.Repaint();
				}
			}
		}

		public static void Show(GUIView viewToUpdate, Color col)
		{
			ColorPicker.Show(viewToUpdate, col, true, false, null);
		}

		public static void Show(GUIView viewToUpdate, Color col, bool showAlpha, bool hdr, ColorPickerHDRConfig hdrConfig)
		{
			ColorPicker get = ColorPicker.get;
			get.m_HDR = hdr;
			get.m_HDRConfig = new ColorPickerHDRConfig(hdrConfig ?? ColorPicker.defaultHDRConfig);
			get.m_DelegateView = viewToUpdate;
			get.SetColor(col);
			get.m_OriginalColor = ColorPicker.get.m_Color;
			get.m_ShowAlpha = showAlpha;
			get.m_ModalUndoGroup = Undo.GetCurrentGroup();
			if (get.m_HDR)
			{
				get.m_IsOSColorPicker = false;
			}
			if (get.m_IsOSColorPicker)
			{
				OSColorPicker.Show(showAlpha);
			}
			else
			{
				get.titleContent = ((!hdr) ? EditorGUIUtility.TextContent("Color") : EditorGUIUtility.TextContent("HDR Color"));
				float y = (float)EditorPrefs.GetInt("CPickerHeight", (int)get.position.height);
				get.minSize = new Vector2(233f, y);
				get.maxSize = new Vector2(233f, y);
				get.InitIfNeeded();
				get.ShowAuxWindow();
			}
		}

		private void PollOSColorPicker()
		{
			if (this.m_IsOSColorPicker)
			{
				if (!OSColorPicker.visible || Application.platform != RuntimePlatform.OSXEditor)
				{
					UnityEngine.Object.DestroyImmediate(this);
				}
				else
				{
					Color color = OSColorPicker.color;
					if (this.m_Color != color)
					{
						this.m_Color = color;
						this.SendEvent(true);
					}
				}
			}
		}

		private void OnEnable()
		{
			base.hideFlags = HideFlags.DontSave;
			this.m_IsOSColorPicker = EditorPrefs.GetBool("UseOSColorPicker");
			base.hideFlags = HideFlags.DontSave;
			EditorApplication.update = (EditorApplication.CallbackFunction)Delegate.Combine(EditorApplication.update, new EditorApplication.CallbackFunction(this.PollOSColorPicker));
			EditorGUIUtility.editingTextField = true;
			this.m_HDRValues.m_ExposureAdjustment = EditorPrefs.GetFloat("CPickerExposure", 1f);
			this.m_UseTonemappingPreview = (EditorPrefs.GetInt("CPTonePreview", 0) != 0);
			this.m_SliderMode = (ColorPicker.SliderMode)EditorPrefs.GetInt("CPSliderMode", 0);
			this.m_ColorBoxMode = (ColorPicker.ColorBoxMode)EditorPrefs.GetInt("CPColorMode", 0);
			this.m_ShowPresets = (EditorPrefs.GetInt("CPPresetsShow", 1) != 0);
		}

		private void OnDisable()
		{
			EditorPrefs.SetFloat("CPickerExposure", this.m_HDRValues.m_ExposureAdjustment);
			EditorPrefs.SetInt("CPTonePreview", (!this.m_UseTonemappingPreview) ? 0 : 1);
			EditorPrefs.SetInt("CPSliderMode", (int)this.m_SliderMode);
			EditorPrefs.SetInt("CPColorMode", (int)this.m_ColorBoxMode);
			EditorPrefs.SetInt("CPPresetsShow", (!this.m_ShowPresets) ? 0 : 1);
			EditorPrefs.SetInt("CPickerHeight", (int)base.position.height);
		}

		public void OnDestroy()
		{
			Undo.CollapseUndoOperations(this.m_ModalUndoGroup);
			if (this.m_ColorSlider)
			{
				UnityEngine.Object.DestroyImmediate(this.m_ColorSlider);
			}
			if (this.m_ColorBox)
			{
				UnityEngine.Object.DestroyImmediate(this.m_ColorBox);
			}
			if (this.m_RTexture)
			{
				UnityEngine.Object.DestroyImmediate(this.m_RTexture);
			}
			if (this.m_GTexture)
			{
				UnityEngine.Object.DestroyImmediate(this.m_GTexture);
			}
			if (this.m_BTexture)
			{
				UnityEngine.Object.DestroyImmediate(this.m_BTexture);
			}
			if (this.m_HueTexture)
			{
				UnityEngine.Object.DestroyImmediate(this.m_HueTexture);
			}
			if (this.m_SatTexture)
			{
				UnityEngine.Object.DestroyImmediate(this.m_SatTexture);
			}
			if (this.m_ValTexture)
			{
				UnityEngine.Object.DestroyImmediate(this.m_ValTexture);
			}
			if (this.m_AlphaTexture)
			{
				UnityEngine.Object.DestroyImmediate(this.m_AlphaTexture);
			}
			ColorPicker.s_SharedColorPicker = null;
			if (this.m_IsOSColorPicker)
			{
				OSColorPicker.Close();
			}
			EditorApplication.update = (EditorApplication.CallbackFunction)Delegate.Remove(EditorApplication.update, new EditorApplication.CallbackFunction(this.PollOSColorPicker));
			if (this.m_ColorLibraryEditorState != null)
			{
				this.m_ColorLibraryEditorState.TransferEditorPrefsState(false);
			}
			if (this.m_ColorLibraryEditor != null)
			{
				this.m_ColorLibraryEditor.UnloadUsedLibraries();
			}
			if (this.m_ColorBoxMode == ColorPicker.ColorBoxMode.EyeDropper)
			{
				EditorPrefs.SetInt("CPColorMode", (int)this.m_OldColorBoxMode);
			}
		}
	}
}
