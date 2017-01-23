using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.AnimatedValues;
using UnityEditor.Modules;
using UnityEngine;
using UnityEngine.Events;

namespace UnityEditor
{
	[CanEditMultipleObjects, CustomEditor(typeof(TextureImporter))]
	internal class TextureImporterInspector : AssetImporterInspector
	{
		[Flags]
		private enum TextureInspectorGUIElement
		{
			None = 0,
			PowerOfTwo = 1,
			Readable = 2,
			AlphaHandling = 4,
			ColorSpace = 8,
			MipMaps = 16,
			NormalMap = 32,
			Sprite = 64,
			Cookie = 128,
			CubeMapConvolution = 256,
			CubeMapping = 512
		}

		private struct TextureInspectorTypeGUIProperties
		{
			public TextureImporterInspector.TextureInspectorGUIElement commonElements;

			public TextureImporterInspector.TextureInspectorGUIElement advancedElements;

			public TextureImporterShape shapeCaps;

			public TextureInspectorTypeGUIProperties(TextureImporterInspector.TextureInspectorGUIElement _commonElements, TextureImporterInspector.TextureInspectorGUIElement _advancedElements, TextureImporterShape _shapeCaps)
			{
				this.commonElements = _commonElements;
				this.advancedElements = _advancedElements;
				this.shapeCaps = _shapeCaps;
			}
		}

		private delegate void GUIMethod(TextureImporterInspector.TextureInspectorGUIElement guiElements);

		private enum CookieMode
		{
			Spot,
			Directional,
			Point
		}

		internal class Styles
		{
			public readonly GUIContent textureTypeTitle = EditorGUIUtility.TextContent("Texture Type|What will this texture be used for?");

			public readonly GUIContent[] textureTypeOptions = new GUIContent[]
			{
				EditorGUIUtility.TextContent("Default|Texture is a normal image such as a diffuse texture or other."),
				EditorGUIUtility.TextContent("Normal map|Texture is a bump or normal map."),
				EditorGUIUtility.TextContent("Editor GUI and Legacy GUI|Texture is used for a GUI element."),
				EditorGUIUtility.TextContent("Sprite (2D and UI)|Texture is used for a sprite."),
				EditorGUIUtility.TextContent("Cursor|Texture is used for a cursor."),
				EditorGUIUtility.TextContent("Cookie|Texture is a cookie you put on a light."),
				EditorGUIUtility.TextContent("Lightmap|Texture is a lightmap."),
				EditorGUIUtility.TextContent("Single Channel|Texture is a one component texture.")
			};

			public readonly int[] textureTypeValues = new int[]
			{
				0,
				1,
				2,
				8,
				7,
				4,
				6,
				10
			};

			public readonly GUIContent textureShape = EditorGUIUtility.TextContent("Texture Shape|What shape is this texture?");

			private readonly GUIContent textureShape2D = EditorGUIUtility.TextContent("2D|Texture is 2D.");

			private readonly GUIContent textureShapeCube = EditorGUIUtility.TextContent("Cube|Texture is a Cubemap.");

			public readonly Dictionary<TextureImporterShape, GUIContent[]> textureShapeOptionsDictionnary = new Dictionary<TextureImporterShape, GUIContent[]>();

			public readonly Dictionary<TextureImporterShape, int[]> textureShapeValuesDictionnary = new Dictionary<TextureImporterShape, int[]>();

			public readonly GUIContent filterMode = EditorGUIUtility.TextContent("Filter Mode");

			public readonly GUIContent[] filterModeOptions = new GUIContent[]
			{
				EditorGUIUtility.TextContent("Point (no filter)"),
				EditorGUIUtility.TextContent("Bilinear"),
				EditorGUIUtility.TextContent("Trilinear")
			};

			public readonly GUIContent cookieType = EditorGUIUtility.TextContent("Light Type");

			public readonly GUIContent[] cookieOptions = new GUIContent[]
			{
				EditorGUIUtility.TextContent("Spotlight"),
				EditorGUIUtility.TextContent("Directional"),
				EditorGUIUtility.TextContent("Point")
			};

			public readonly GUIContent generateFromBump = EditorGUIUtility.TextContent("Create from Grayscale|The grayscale of the image is used as a heightmap for generating the normal map.");

			public readonly GUIContent bumpiness = EditorGUIUtility.TextContent("Bumpiness");

			public readonly GUIContent bumpFiltering = EditorGUIUtility.TextContent("Filtering");

			public readonly GUIContent[] bumpFilteringOptions = new GUIContent[]
			{
				EditorGUIUtility.TextContent("Sharp"),
				EditorGUIUtility.TextContent("Smooth")
			};

			public readonly GUIContent cubemap = EditorGUIUtility.TextContent("Mapping");

			public readonly GUIContent[] cubemapOptions = new GUIContent[]
			{
				EditorGUIUtility.TextContent("Auto"),
				EditorGUIUtility.TextContent("6 Frames Layout (Cubic Environment)|Texture contains 6 images arranged in one of the standard cubemap layouts - cross or sequence (+x,-x, +y, -y, +z, -z). Texture can be in vertical or horizontal orientation."),
				EditorGUIUtility.TextContent("Latitude-Longitude Layout (Cylindrical)|Texture contains an image of a ball unwrapped such that latitude and longitude are mapped to horizontal and vertical dimensions (as on a globe)."),
				EditorGUIUtility.TextContent("Mirrored Ball (Spheremap)|Texture contains an image of a mirrored ball.")
			};

			public readonly int[] cubemapValues2 = new int[]
			{
				6,
				5,
				2,
				1
			};

			public readonly GUIContent cubemapConvolution = EditorGUIUtility.TextContent("Convolution Type");

			public readonly GUIContent[] cubemapConvolutionOptions = new GUIContent[]
			{
				EditorGUIUtility.TextContent("None"),
				EditorGUIUtility.TextContent("Specular (Glossy Reflection)|Convolve cubemap for specular reflections with varying smoothness (Glossy Reflections)."),
				EditorGUIUtility.TextContent("Diffuse (Irradiance)|Convolve cubemap for diffuse-only reflection (Irradiance Cubemap).")
			};

			public readonly int[] cubemapConvolutionValues = new int[]
			{
				0,
				1,
				2
			};

			public readonly GUIContent seamlessCubemap = EditorGUIUtility.TextContent("Fixup Edge Seams|Enable if this texture is used for glossy reflections.");

			public readonly GUIContent textureFormat = EditorGUIUtility.TextContent("Format");

			public readonly GUIContent defaultPlatform = EditorGUIUtility.TextContent("Default");

			public readonly GUIContent mipmapFadeOutToggle = EditorGUIUtility.TextContent("Fadeout Mip Maps");

			public readonly GUIContent mipmapFadeOut = EditorGUIUtility.TextContent("Fade Range");

			public readonly GUIContent readWrite = EditorGUIUtility.TextContent("Read/Write Enabled|Enable to be able to access the raw pixel data from code.");

			public readonly GUIContent alphaSource = EditorGUIUtility.TextContent("Alpha Source|How is the alpha generated for the imported texture.");

			public readonly GUIContent[] alphaSourceOptions = new GUIContent[]
			{
				EditorGUIUtility.TextContent("None|No Alpha will be used."),
				EditorGUIUtility.TextContent("Input Texture Alpha|Use Alpha from the input texture if one is provided."),
				EditorGUIUtility.TextContent("From Gray Scale|Generate Alpha from image gray scale.")
			};

			public readonly int[] alphaSourceValues = new int[]
			{
				0,
				1,
				2
			};

			public readonly GUIContent generateMipMaps = EditorGUIUtility.TextContent("Generate Mip Maps");

			public readonly GUIContent sRGBTexture = EditorGUIUtility.TextContent("sRGB (Color Texture)|Texture content is stored in gamma space. Non-HDR color textures should enable this flag (except if used for IMGUI).");

			public readonly GUIContent borderMipMaps = EditorGUIUtility.TextContent("Border Mip Maps");

			public readonly GUIContent mipMapFilter = EditorGUIUtility.TextContent("Mip Map Filtering");

			public readonly GUIContent[] mipMapFilterOptions = new GUIContent[]
			{
				EditorGUIUtility.TextContent("Box"),
				EditorGUIUtility.TextContent("Kaiser")
			};

			public readonly GUIContent npot = EditorGUIUtility.TextContent("Non Power of 2|How non-power-of-two textures are scaled on import.");

			public readonly GUIContent generateCubemap = EditorGUIUtility.TextContent("Generate Cubemap");

			public readonly GUIContent compressionQuality = EditorGUIUtility.TextContent("Compressor Quality");

			public readonly GUIContent compressionQualitySlider = EditorGUIUtility.TextContent("Compressor Quality|Use the slider to adjust compression quality from 0 (Fastest) to 100 (Best)");

			public readonly GUIContent[] mobileCompressionQualityOptions = new GUIContent[]
			{
				EditorGUIUtility.TextContent("Fast"),
				EditorGUIUtility.TextContent("Normal"),
				EditorGUIUtility.TextContent("Best")
			};

			public readonly GUIContent spriteMode = EditorGUIUtility.TextContent("Sprite Mode");

			public readonly GUIContent[] spriteModeOptions = new GUIContent[]
			{
				EditorGUIUtility.TextContent("Single"),
				EditorGUIUtility.TextContent("Multiple"),
				EditorGUIUtility.TextContent("Polygon")
			};

			public readonly GUIContent[] spriteMeshTypeOptions = new GUIContent[]
			{
				EditorGUIUtility.TextContent("Full Rect"),
				EditorGUIUtility.TextContent("Tight")
			};

			public readonly GUIContent spritePackingTag = EditorGUIUtility.TextContent("Packing Tag|Tag for the Sprite Packing system.");

			public readonly GUIContent spritePixelsPerUnit = EditorGUIUtility.TextContent("Pixels Per Unit|How many pixels in the sprite correspond to one unit in the world.");

			public readonly GUIContent spriteExtrude = EditorGUIUtility.TextContent("Extrude Edges|How much empty area to leave around the sprite in the generated mesh.");

			public readonly GUIContent spriteMeshType = EditorGUIUtility.TextContent("Mesh Type|Type of sprite mesh to generate.");

			public readonly GUIContent spriteAlignment = EditorGUIUtility.TextContent("Pivot|Sprite pivot point in its localspace. May be used for syncing animation frames of different sizes.");

			public readonly GUIContent[] spriteAlignmentOptions = new GUIContent[]
			{
				EditorGUIUtility.TextContent("Center"),
				EditorGUIUtility.TextContent("Top Left"),
				EditorGUIUtility.TextContent("Top"),
				EditorGUIUtility.TextContent("Top Right"),
				EditorGUIUtility.TextContent("Left"),
				EditorGUIUtility.TextContent("Right"),
				EditorGUIUtility.TextContent("Bottom Left"),
				EditorGUIUtility.TextContent("Bottom"),
				EditorGUIUtility.TextContent("Bottom Right"),
				EditorGUIUtility.TextContent("Custom")
			};

			public readonly GUIContent alphaIsTransparency = EditorGUIUtility.TextContent("Alpha Is Transparency|If the provided alpha channel is transparency, enable this to pre-filter the color to avoid texture filtering artifacts. This is not supported for HDR textures.");

			public readonly GUIContent etc1Compression = EditorGUIUtility.TextContent("Compress using ETC1 (split alpha channel)|Alpha for this texture will be preserved by splitting the alpha channel to another texture, and both resulting textures will be compressed using ETC1.");

			public readonly GUIContent crunchedCompression = EditorGUIUtility.TextContent("Use Crunch Compression|Texture is crunch-compressed to save space on disk when applicable.");

			public readonly GUIContent showAdvanced = EditorGUIUtility.TextContent("Advanced|Show advanced settings.");

			public Styles()
			{
				GUIContent[] value = new GUIContent[]
				{
					this.textureShape2D
				};
				GUIContent[] value2 = new GUIContent[]
				{
					this.textureShapeCube
				};
				GUIContent[] value3 = new GUIContent[]
				{
					this.textureShape2D,
					this.textureShapeCube
				};
				this.textureShapeOptionsDictionnary.Add(TextureImporterShape.Texture2D, value);
				this.textureShapeOptionsDictionnary.Add(TextureImporterShape.TextureCube, value2);
				this.textureShapeOptionsDictionnary.Add(TextureImporterShape.Texture2D | TextureImporterShape.TextureCube, value3);
				int[] value4 = new int[]
				{
					1
				};
				int[] value5 = new int[]
				{
					2
				};
				int[] value6 = new int[]
				{
					1,
					2
				};
				this.textureShapeValuesDictionnary.Add(TextureImporterShape.Texture2D, value4);
				this.textureShapeValuesDictionnary.Add(TextureImporterShape.TextureCube, value5);
				this.textureShapeValuesDictionnary.Add(TextureImporterShape.Texture2D | TextureImporterShape.TextureCube, value6);
			}
		}

		public static string s_DefaultPlatformName = "DefaultTexturePlatform";

		private SerializedProperty m_TextureType;

		private Dictionary<TextureImporterInspector.TextureInspectorGUIElement, TextureImporterInspector.GUIMethod> m_GUIElementMethods = new Dictionary<TextureImporterInspector.TextureInspectorGUIElement, TextureImporterInspector.GUIMethod>();

		[SerializeField]
		internal List<TextureImportPlatformSettings> m_PlatformSettings;

		internal static int[] s_TextureFormatsValueAll;

		internal static int[] s_NormalFormatsValueAll;

		internal static readonly TextureImporterFormat[] kFormatsWithCompressionSettings = new TextureImporterFormat[]
		{
			TextureImporterFormat.DXT1Crunched,
			TextureImporterFormat.DXT5Crunched,
			TextureImporterFormat.PVRTC_RGB2,
			TextureImporterFormat.PVRTC_RGB4,
			TextureImporterFormat.PVRTC_RGBA2,
			TextureImporterFormat.PVRTC_RGBA4,
			TextureImporterFormat.ATC_RGB4,
			TextureImporterFormat.ATC_RGBA8,
			TextureImporterFormat.ETC_RGB4,
			TextureImporterFormat.ETC2_RGB4,
			TextureImporterFormat.ETC2_RGB4_PUNCHTHROUGH_ALPHA,
			TextureImporterFormat.ETC2_RGBA8,
			TextureImporterFormat.ASTC_RGB_4x4,
			TextureImporterFormat.ASTC_RGB_5x5,
			TextureImporterFormat.ASTC_RGB_6x6,
			TextureImporterFormat.ASTC_RGB_8x8,
			TextureImporterFormat.ASTC_RGB_10x10,
			TextureImporterFormat.ASTC_RGB_12x12,
			TextureImporterFormat.ASTC_RGBA_4x4,
			TextureImporterFormat.ASTC_RGBA_5x5,
			TextureImporterFormat.ASTC_RGBA_6x6,
			TextureImporterFormat.ASTC_RGBA_8x8,
			TextureImporterFormat.ASTC_RGBA_10x10,
			TextureImporterFormat.ASTC_RGBA_12x12
		};

		internal static string[] s_TextureFormatStringsAll;

		internal static string[] s_TextureFormatStringsWiiU;

		internal static string[] s_TextureFormatStringsWebGL;

		internal static string[] s_TextureFormatStringsApplePVR;

		internal static string[] s_TextureFormatStringsAndroid;

		internal static string[] s_TextureFormatStringsTizen;

		internal static string[] s_TextureFormatStringsSTV;

		internal static string[] s_TextureFormatStringsSingleChannel;

		internal static string[] s_TextureFormatStringsDefault;

		internal static string[] s_NormalFormatStringsDefault;

		private readonly AnimBool m_ShowBumpGenerationSettings = new AnimBool();

		private readonly AnimBool m_ShowCubeMapSettings = new AnimBool();

		private readonly AnimBool m_ShowGenericSpriteSettings = new AnimBool();

		private readonly AnimBool m_ShowMipMapSettings = new AnimBool();

		private readonly GUIContent m_EmptyContent = new GUIContent(" ");

		private readonly int[] m_FilterModeOptions = (int[])Enum.GetValues(typeof(FilterMode));

		private string m_ImportWarning = null;

		internal static TextureImporterInspector.Styles s_Styles;

		private TextureImporterInspector.TextureInspectorTypeGUIProperties[] m_TextureTypeGUIElements = new TextureImporterInspector.TextureInspectorTypeGUIProperties[Enum.GetValues(typeof(TextureImporterType)).Length];

		private List<TextureImporterInspector.TextureInspectorGUIElement> m_GUIElementsDisplayOrder = new List<TextureImporterInspector.TextureInspectorGUIElement>();

		private SerializedProperty m_AlphaSource;

		private SerializedProperty m_ConvertToNormalMap;

		private SerializedProperty m_HeightScale;

		private SerializedProperty m_NormalMapFilter;

		private SerializedProperty m_GenerateCubemap;

		private SerializedProperty m_CubemapConvolution;

		private SerializedProperty m_SeamlessCubemap;

		private SerializedProperty m_BorderMipMap;

		private SerializedProperty m_NPOTScale;

		private SerializedProperty m_IsReadable;

		private SerializedProperty m_sRGBTexture;

		private SerializedProperty m_EnableMipMap;

		private SerializedProperty m_MipMapMode;

		private SerializedProperty m_FadeOut;

		private SerializedProperty m_MipMapFadeDistanceStart;

		private SerializedProperty m_MipMapFadeDistanceEnd;

		private SerializedProperty m_Aniso;

		private SerializedProperty m_FilterMode;

		private SerializedProperty m_WrapMode;

		private SerializedProperty m_SpritePackingTag;

		private SerializedProperty m_SpritePixelsToUnits;

		private SerializedProperty m_SpriteExtrude;

		private SerializedProperty m_SpriteMeshType;

		private SerializedProperty m_Alignment;

		private SerializedProperty m_SpritePivot;

		private SerializedProperty m_AlphaIsTransparency;

		private SerializedProperty m_TextureShape;

		private SerializedProperty m_SpriteMode;

		private bool m_ShowAdvanced = false;

		private int m_TextureWidth = 0;

		private int m_TextureHeight = 0;

		private bool m_IsPOT = false;

		internal TextureImporterType textureType
		{
			get
			{
				TextureImporterType result;
				if (this.m_TextureType.hasMultipleDifferentValues)
				{
					result = TextureImporterType.Default;
				}
				else
				{
					result = (TextureImporterType)this.m_TextureType.intValue;
				}
				return result;
			}
		}

		internal bool textureTypeHasMultipleDifferentValues
		{
			get
			{
				return this.m_TextureType.hasMultipleDifferentValues;
			}
		}

		internal override bool showImportedObject
		{
			get
			{
				return false;
			}
		}

		internal static int[] TextureFormatsValueAll
		{
			get
			{
				int[] result;
				if (TextureImporterInspector.s_TextureFormatsValueAll != null)
				{
					result = TextureImporterInspector.s_TextureFormatsValueAll;
				}
				else
				{
					bool flag = false;
					bool flag2 = false;
					bool flag3 = false;
					bool flag4 = false;
					bool flag5 = false;
					BuildPlayerWindow.BuildPlatform[] buildPlayerValidPlatforms = TextureImporterInspector.GetBuildPlayerValidPlatforms();
					BuildPlayerWindow.BuildPlatform[] array = buildPlayerValidPlatforms;
					for (int i = 0; i < array.Length; i++)
					{
						BuildPlayerWindow.BuildPlatform buildPlatform = array[i];
						BuildTarget defaultTarget = buildPlatform.DefaultTarget;
						switch (defaultTarget)
						{
						case BuildTarget.SamsungTV:
							flag = true;
							goto IL_B6;
						case BuildTarget.N3DS:
						case BuildTarget.WiiU:
							IL_61:
							if (defaultTarget == BuildTarget.iOS)
							{
								flag2 = true;
								goto IL_B6;
							}
							if (defaultTarget == BuildTarget.Android)
							{
								flag2 = true;
								flag = true;
								flag3 = true;
								flag4 = true;
								flag5 = true;
								goto IL_B6;
							}
							if (defaultTarget != BuildTarget.Tizen)
							{
								goto IL_B6;
							}
							flag = true;
							goto IL_B6;
						case BuildTarget.tvOS:
							flag2 = true;
							flag5 = true;
							goto IL_B6;
						}
						goto IL_61;
						IL_B6:;
					}
					List<int> list = new List<int>();
					list.AddRange(new int[]
					{
						10,
						12
					});
					if (flag)
					{
						list.Add(34);
					}
					if (flag2)
					{
						list.AddRange(new int[]
						{
							30,
							31,
							32,
							33
						});
					}
					if (flag3)
					{
						list.AddRange(new int[]
						{
							35,
							36
						});
					}
					if (flag4)
					{
						list.AddRange(new int[]
						{
							45,
							46,
							47
						});
					}
					if (flag5)
					{
						list.AddRange(new int[]
						{
							48,
							49,
							50,
							51,
							52,
							53,
							54,
							55,
							56,
							57,
							58,
							59
						});
					}
					list.AddRange(new int[]
					{
						7,
						2,
						13,
						3,
						1,
						5,
						4,
						17,
						24,
						25,
						28,
						29
					});
					TextureImporterInspector.s_TextureFormatsValueAll = list.ToArray();
					result = TextureImporterInspector.s_TextureFormatsValueAll;
				}
				return result;
			}
		}

		internal static int[] NormalFormatsValueAll
		{
			get
			{
				bool flag = false;
				bool flag2 = false;
				bool flag3 = false;
				bool flag4 = false;
				bool flag5 = false;
				BuildPlayerWindow.BuildPlatform[] buildPlayerValidPlatforms = TextureImporterInspector.GetBuildPlayerValidPlatforms();
				BuildPlayerWindow.BuildPlatform[] array = buildPlayerValidPlatforms;
				for (int i = 0; i < array.Length; i++)
				{
					BuildPlayerWindow.BuildPlatform buildPlatform = array[i];
					BuildTarget defaultTarget = buildPlatform.DefaultTarget;
					if (defaultTarget != BuildTarget.iOS)
					{
						if (defaultTarget != BuildTarget.Android)
						{
							if (defaultTarget != BuildTarget.Tizen)
							{
								if (defaultTarget == BuildTarget.tvOS)
								{
									flag2 = true;
									flag = true;
								}
							}
							else
							{
								flag = true;
							}
						}
						else
						{
							flag2 = true;
							flag3 = true;
							flag = true;
							flag4 = true;
							flag5 = true;
						}
					}
					else
					{
						flag2 = true;
						flag = true;
					}
				}
				List<int> list = new List<int>();
				list.AddRange(new int[]
				{
					12
				});
				if (flag2)
				{
					list.AddRange(new int[]
					{
						30,
						31,
						32,
						33
					});
				}
				if (flag3)
				{
					list.AddRange(new int[]
					{
						35,
						36
					});
				}
				if (flag)
				{
					list.AddRange(new int[]
					{
						34
					});
				}
				if (flag4)
				{
					list.AddRange(new int[]
					{
						45,
						46,
						47
					});
				}
				if (flag5)
				{
					list.AddRange(new int[]
					{
						48,
						49,
						50,
						51,
						52,
						53,
						54,
						55,
						56,
						57,
						58,
						59
					});
				}
				list.AddRange(new int[]
				{
					2,
					13,
					4,
					29
				});
				TextureImporterInspector.s_NormalFormatsValueAll = list.ToArray();
				return TextureImporterInspector.s_NormalFormatsValueAll;
			}
		}

		internal SpriteImportMode spriteImportMode
		{
			get
			{
				return (SpriteImportMode)this.m_SpriteMode.intValue;
			}
		}

		public new void OnDisable()
		{
			base.OnDisable();
			EditorPrefs.SetBool("TextureImporterShowAdvanced", this.m_ShowAdvanced);
		}

		public static bool IsCompressedDXTTextureFormat(TextureImporterFormat format)
		{
			return format == TextureImporterFormat.DXT1 || format == TextureImporterFormat.DXT5;
		}

		internal static bool IsGLESMobileTargetPlatform(BuildTarget target)
		{
			return target == BuildTarget.iOS || target == BuildTarget.tvOS || target == BuildTarget.Android || target == BuildTarget.Tizen || target == BuildTarget.SamsungTV;
		}

		private void UpdateImportWarning()
		{
			TextureImporter textureImporter = base.target as TextureImporter;
			this.m_ImportWarning = ((!textureImporter) ? null : textureImporter.GetImportWarnings());
		}

		private void ToggleFromInt(SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginChangeCheck();
			EditorGUI.showMixedValue = property.hasMultipleDifferentValues;
			int intValue = (!EditorGUILayout.Toggle(label, property.intValue > 0, new GUILayoutOption[0])) ? 0 : 1;
			EditorGUI.showMixedValue = false;
			if (EditorGUI.EndChangeCheck())
			{
				property.intValue = intValue;
			}
		}

		private void EnumPopup(SerializedProperty property, Type type, GUIContent label)
		{
			EditorGUILayout.IntPopup(property, EditorGUIUtility.TempContent(Enum.GetNames(type)), Enum.GetValues(type) as int[], label, new GUILayoutOption[0]);
		}

		private void CacheSerializedProperties()
		{
			this.m_AlphaSource = base.serializedObject.FindProperty("m_AlphaUsage");
			this.m_ConvertToNormalMap = base.serializedObject.FindProperty("m_ConvertToNormalMap");
			this.m_HeightScale = base.serializedObject.FindProperty("m_HeightScale");
			this.m_NormalMapFilter = base.serializedObject.FindProperty("m_NormalMapFilter");
			this.m_GenerateCubemap = base.serializedObject.FindProperty("m_GenerateCubemap");
			this.m_SeamlessCubemap = base.serializedObject.FindProperty("m_SeamlessCubemap");
			this.m_BorderMipMap = base.serializedObject.FindProperty("m_BorderMipMap");
			this.m_NPOTScale = base.serializedObject.FindProperty("m_NPOTScale");
			this.m_IsReadable = base.serializedObject.FindProperty("m_IsReadable");
			this.m_sRGBTexture = base.serializedObject.FindProperty("m_sRGBTexture");
			this.m_EnableMipMap = base.serializedObject.FindProperty("m_EnableMipMap");
			this.m_MipMapMode = base.serializedObject.FindProperty("m_MipMapMode");
			this.m_FadeOut = base.serializedObject.FindProperty("m_FadeOut");
			this.m_MipMapFadeDistanceStart = base.serializedObject.FindProperty("m_MipMapFadeDistanceStart");
			this.m_MipMapFadeDistanceEnd = base.serializedObject.FindProperty("m_MipMapFadeDistanceEnd");
			this.m_Aniso = base.serializedObject.FindProperty("m_TextureSettings.m_Aniso");
			this.m_FilterMode = base.serializedObject.FindProperty("m_TextureSettings.m_FilterMode");
			this.m_WrapMode = base.serializedObject.FindProperty("m_TextureSettings.m_WrapMode");
			this.m_CubemapConvolution = base.serializedObject.FindProperty("m_CubemapConvolution");
			this.m_SpriteMode = base.serializedObject.FindProperty("m_SpriteMode");
			this.m_SpritePackingTag = base.serializedObject.FindProperty("m_SpritePackingTag");
			this.m_SpritePixelsToUnits = base.serializedObject.FindProperty("m_SpritePixelsToUnits");
			this.m_SpriteExtrude = base.serializedObject.FindProperty("m_SpriteExtrude");
			this.m_SpriteMeshType = base.serializedObject.FindProperty("m_SpriteMeshType");
			this.m_Alignment = base.serializedObject.FindProperty("m_Alignment");
			this.m_SpritePivot = base.serializedObject.FindProperty("m_SpritePivot");
			this.m_AlphaIsTransparency = base.serializedObject.FindProperty("m_AlphaIsTransparency");
			this.m_TextureType = base.serializedObject.FindProperty("m_TextureType");
			this.m_TextureShape = base.serializedObject.FindProperty("m_TextureShape");
		}

		private void InitializeGUI()
		{
			TextureImporterShape shapeCaps = TextureImporterShape.Texture2D | TextureImporterShape.TextureCube;
			this.m_TextureTypeGUIElements[0] = new TextureImporterInspector.TextureInspectorTypeGUIProperties(TextureImporterInspector.TextureInspectorGUIElement.AlphaHandling | TextureImporterInspector.TextureInspectorGUIElement.ColorSpace | TextureImporterInspector.TextureInspectorGUIElement.CubeMapConvolution | TextureImporterInspector.TextureInspectorGUIElement.CubeMapping, TextureImporterInspector.TextureInspectorGUIElement.PowerOfTwo | TextureImporterInspector.TextureInspectorGUIElement.Readable | TextureImporterInspector.TextureInspectorGUIElement.MipMaps, shapeCaps);
			this.m_TextureTypeGUIElements[1] = new TextureImporterInspector.TextureInspectorTypeGUIProperties(TextureImporterInspector.TextureInspectorGUIElement.NormalMap | TextureImporterInspector.TextureInspectorGUIElement.CubeMapping, TextureImporterInspector.TextureInspectorGUIElement.PowerOfTwo | TextureImporterInspector.TextureInspectorGUIElement.Readable | TextureImporterInspector.TextureInspectorGUIElement.MipMaps, shapeCaps);
			this.m_TextureTypeGUIElements[8] = new TextureImporterInspector.TextureInspectorTypeGUIProperties(TextureImporterInspector.TextureInspectorGUIElement.Sprite, TextureImporterInspector.TextureInspectorGUIElement.Readable | TextureImporterInspector.TextureInspectorGUIElement.AlphaHandling | TextureImporterInspector.TextureInspectorGUIElement.ColorSpace | TextureImporterInspector.TextureInspectorGUIElement.MipMaps, TextureImporterShape.Texture2D);
			this.m_TextureTypeGUIElements[4] = new TextureImporterInspector.TextureInspectorTypeGUIProperties(TextureImporterInspector.TextureInspectorGUIElement.AlphaHandling | TextureImporterInspector.TextureInspectorGUIElement.Cookie | TextureImporterInspector.TextureInspectorGUIElement.CubeMapping, TextureImporterInspector.TextureInspectorGUIElement.PowerOfTwo | TextureImporterInspector.TextureInspectorGUIElement.Readable | TextureImporterInspector.TextureInspectorGUIElement.MipMaps, TextureImporterShape.Texture2D | TextureImporterShape.TextureCube);
			this.m_TextureTypeGUIElements[10] = new TextureImporterInspector.TextureInspectorTypeGUIProperties(TextureImporterInspector.TextureInspectorGUIElement.AlphaHandling | TextureImporterInspector.TextureInspectorGUIElement.CubeMapping, TextureImporterInspector.TextureInspectorGUIElement.PowerOfTwo | TextureImporterInspector.TextureInspectorGUIElement.Readable | TextureImporterInspector.TextureInspectorGUIElement.MipMaps, shapeCaps);
			this.m_TextureTypeGUIElements[2] = new TextureImporterInspector.TextureInspectorTypeGUIProperties(TextureImporterInspector.TextureInspectorGUIElement.None, TextureImporterInspector.TextureInspectorGUIElement.PowerOfTwo | TextureImporterInspector.TextureInspectorGUIElement.Readable | TextureImporterInspector.TextureInspectorGUIElement.AlphaHandling | TextureImporterInspector.TextureInspectorGUIElement.MipMaps, TextureImporterShape.Texture2D);
			this.m_TextureTypeGUIElements[7] = new TextureImporterInspector.TextureInspectorTypeGUIProperties(TextureImporterInspector.TextureInspectorGUIElement.None, TextureImporterInspector.TextureInspectorGUIElement.PowerOfTwo | TextureImporterInspector.TextureInspectorGUIElement.Readable | TextureImporterInspector.TextureInspectorGUIElement.AlphaHandling | TextureImporterInspector.TextureInspectorGUIElement.MipMaps, TextureImporterShape.Texture2D);
			this.m_TextureTypeGUIElements[6] = new TextureImporterInspector.TextureInspectorTypeGUIProperties(TextureImporterInspector.TextureInspectorGUIElement.None, TextureImporterInspector.TextureInspectorGUIElement.PowerOfTwo | TextureImporterInspector.TextureInspectorGUIElement.Readable | TextureImporterInspector.TextureInspectorGUIElement.MipMaps, TextureImporterShape.Texture2D);
			this.m_GUIElementMethods.Clear();
			this.m_GUIElementMethods.Add(TextureImporterInspector.TextureInspectorGUIElement.PowerOfTwo, new TextureImporterInspector.GUIMethod(this.POTScaleGUI));
			this.m_GUIElementMethods.Add(TextureImporterInspector.TextureInspectorGUIElement.Readable, new TextureImporterInspector.GUIMethod(this.ReadableGUI));
			this.m_GUIElementMethods.Add(TextureImporterInspector.TextureInspectorGUIElement.ColorSpace, new TextureImporterInspector.GUIMethod(this.ColorSpaceGUI));
			this.m_GUIElementMethods.Add(TextureImporterInspector.TextureInspectorGUIElement.AlphaHandling, new TextureImporterInspector.GUIMethod(this.AlphaHandlingGUI));
			this.m_GUIElementMethods.Add(TextureImporterInspector.TextureInspectorGUIElement.MipMaps, new TextureImporterInspector.GUIMethod(this.MipMapGUI));
			this.m_GUIElementMethods.Add(TextureImporterInspector.TextureInspectorGUIElement.NormalMap, new TextureImporterInspector.GUIMethod(this.BumpGUI));
			this.m_GUIElementMethods.Add(TextureImporterInspector.TextureInspectorGUIElement.Sprite, new TextureImporterInspector.GUIMethod(this.SpriteGUI));
			this.m_GUIElementMethods.Add(TextureImporterInspector.TextureInspectorGUIElement.Cookie, new TextureImporterInspector.GUIMethod(this.CookieGUI));
			this.m_GUIElementMethods.Add(TextureImporterInspector.TextureInspectorGUIElement.CubeMapping, new TextureImporterInspector.GUIMethod(this.CubemapMappingGUI));
			this.m_GUIElementsDisplayOrder.Clear();
			this.m_GUIElementsDisplayOrder.Add(TextureImporterInspector.TextureInspectorGUIElement.CubeMapping);
			this.m_GUIElementsDisplayOrder.Add(TextureImporterInspector.TextureInspectorGUIElement.CubeMapConvolution);
			this.m_GUIElementsDisplayOrder.Add(TextureImporterInspector.TextureInspectorGUIElement.Cookie);
			this.m_GUIElementsDisplayOrder.Add(TextureImporterInspector.TextureInspectorGUIElement.ColorSpace);
			this.m_GUIElementsDisplayOrder.Add(TextureImporterInspector.TextureInspectorGUIElement.AlphaHandling);
			this.m_GUIElementsDisplayOrder.Add(TextureImporterInspector.TextureInspectorGUIElement.NormalMap);
			this.m_GUIElementsDisplayOrder.Add(TextureImporterInspector.TextureInspectorGUIElement.Sprite);
			this.m_GUIElementsDisplayOrder.Add(TextureImporterInspector.TextureInspectorGUIElement.PowerOfTwo);
			this.m_GUIElementsDisplayOrder.Add(TextureImporterInspector.TextureInspectorGUIElement.Readable);
			this.m_GUIElementsDisplayOrder.Add(TextureImporterInspector.TextureInspectorGUIElement.MipMaps);
		}

		public virtual void OnEnable()
		{
			TextureImporterInspector.s_DefaultPlatformName = TextureImporter.defaultPlatformName;
			this.m_ShowAdvanced = EditorPrefs.GetBool("TextureImporterShowAdvanced", this.m_ShowAdvanced);
			this.CacheSerializedProperties();
			this.m_ShowBumpGenerationSettings.valueChanged.AddListener(new UnityAction(base.Repaint));
			this.m_ShowCubeMapSettings.valueChanged.AddListener(new UnityAction(base.Repaint));
			this.m_ShowCubeMapSettings.value = (this.m_TextureShape.intValue == 2);
			this.m_ShowGenericSpriteSettings.valueChanged.AddListener(new UnityAction(base.Repaint));
			this.m_ShowGenericSpriteSettings.value = (this.m_SpriteMode.intValue != 0);
			this.m_ShowMipMapSettings.valueChanged.AddListener(new UnityAction(base.Repaint));
			this.m_ShowMipMapSettings.value = this.m_EnableMipMap.boolValue;
			this.InitializeGUI();
			TextureImporter textureImporter = base.target as TextureImporter;
			if (!(textureImporter == null))
			{
				textureImporter.GetWidthAndHeight(ref this.m_TextureWidth, ref this.m_TextureHeight);
				this.m_IsPOT = (TextureImporterInspector.IsPowerOfTwo(this.m_TextureWidth) && TextureImporterInspector.IsPowerOfTwo(this.m_TextureHeight));
				if (TextureImporterInspector.s_TextureFormatStringsApplePVR == null)
				{
					TextureImporterInspector.s_TextureFormatStringsApplePVR = TextureImporterInspector.BuildTextureStrings(TextureImportPlatformSettings.kTextureFormatsValueApplePVR);
				}
				if (TextureImporterInspector.s_TextureFormatStringsAndroid == null)
				{
					TextureImporterInspector.s_TextureFormatStringsAndroid = TextureImporterInspector.BuildTextureStrings(TextureImportPlatformSettings.kTextureFormatsValueAndroid);
				}
				if (TextureImporterInspector.s_TextureFormatStringsTizen == null)
				{
					TextureImporterInspector.s_TextureFormatStringsTizen = TextureImporterInspector.BuildTextureStrings(TextureImportPlatformSettings.kTextureFormatsValueTizen);
				}
				if (TextureImporterInspector.s_TextureFormatStringsSTV == null)
				{
					TextureImporterInspector.s_TextureFormatStringsSTV = TextureImporterInspector.BuildTextureStrings(TextureImportPlatformSettings.kTextureFormatsValueSTV);
				}
				if (TextureImporterInspector.s_TextureFormatStringsWebGL == null)
				{
					TextureImporterInspector.s_TextureFormatStringsWebGL = TextureImporterInspector.BuildTextureStrings(TextureImportPlatformSettings.kTextureFormatsValueWebGL);
				}
				if (TextureImporterInspector.s_TextureFormatStringsWiiU == null)
				{
					TextureImporterInspector.s_TextureFormatStringsWiiU = TextureImporterInspector.BuildTextureStrings(TextureImportPlatformSettings.kTextureFormatsValueWiiU);
				}
				if (TextureImporterInspector.s_TextureFormatStringsDefault == null)
				{
					TextureImporterInspector.s_TextureFormatStringsDefault = TextureImporterInspector.BuildTextureStrings(TextureImportPlatformSettings.kTextureFormatsValueDefault);
				}
				if (TextureImporterInspector.s_NormalFormatStringsDefault == null)
				{
					TextureImporterInspector.s_NormalFormatStringsDefault = TextureImporterInspector.BuildTextureStrings(TextureImportPlatformSettings.kNormalFormatsValueDefault);
				}
				if (TextureImporterInspector.s_TextureFormatStringsSingleChannel == null)
				{
					TextureImporterInspector.s_TextureFormatStringsSingleChannel = TextureImporterInspector.BuildTextureStrings(TextureImportPlatformSettings.kTextureFormatsValueSingleChannel);
				}
			}
		}

		private void SetSerializedPropertySettings(TextureImporterSettings settings)
		{
			this.m_AlphaSource.intValue = (int)settings.alphaSource;
			this.m_ConvertToNormalMap.intValue = ((!settings.convertToNormalMap) ? 0 : 1);
			this.m_HeightScale.floatValue = settings.heightmapScale;
			this.m_NormalMapFilter.intValue = (int)settings.normalMapFilter;
			this.m_GenerateCubemap.intValue = (int)settings.generateCubemap;
			this.m_CubemapConvolution.intValue = (int)settings.cubemapConvolution;
			this.m_SeamlessCubemap.intValue = ((!settings.seamlessCubemap) ? 0 : 1);
			this.m_BorderMipMap.intValue = ((!settings.borderMipmap) ? 0 : 1);
			this.m_NPOTScale.intValue = (int)settings.npotScale;
			this.m_IsReadable.intValue = ((!settings.readable) ? 0 : 1);
			this.m_EnableMipMap.intValue = ((!settings.mipmapEnabled) ? 0 : 1);
			this.m_sRGBTexture.intValue = ((!settings.sRGBTexture) ? 0 : 1);
			this.m_MipMapMode.intValue = (int)settings.mipmapFilter;
			this.m_FadeOut.intValue = ((!settings.fadeOut) ? 0 : 1);
			this.m_MipMapFadeDistanceStart.intValue = settings.mipmapFadeDistanceStart;
			this.m_MipMapFadeDistanceEnd.intValue = settings.mipmapFadeDistanceEnd;
			this.m_SpriteMode.intValue = settings.spriteMode;
			this.m_SpritePixelsToUnits.floatValue = settings.spritePixelsPerUnit;
			this.m_SpriteExtrude.intValue = (int)settings.spriteExtrude;
			this.m_SpriteMeshType.intValue = (int)settings.spriteMeshType;
			this.m_Alignment.intValue = settings.spriteAlignment;
			this.m_WrapMode.intValue = (int)settings.wrapMode;
			this.m_FilterMode.intValue = (int)settings.filterMode;
			this.m_Aniso.intValue = settings.aniso;
			this.m_AlphaIsTransparency.intValue = ((!settings.alphaIsTransparency) ? 0 : 1);
			this.m_TextureType.intValue = (int)settings.textureType;
			this.m_TextureShape.intValue = (int)settings.textureShape;
		}

		internal TextureImporterSettings GetSerializedPropertySettings()
		{
			return this.GetSerializedPropertySettings(new TextureImporterSettings());
		}

		internal TextureImporterSettings GetSerializedPropertySettings(TextureImporterSettings settings)
		{
			if (!this.m_AlphaSource.hasMultipleDifferentValues)
			{
				settings.alphaSource = (TextureImporterAlphaSource)this.m_AlphaSource.intValue;
			}
			if (!this.m_ConvertToNormalMap.hasMultipleDifferentValues)
			{
				settings.convertToNormalMap = (this.m_ConvertToNormalMap.intValue > 0);
			}
			if (!this.m_HeightScale.hasMultipleDifferentValues)
			{
				settings.heightmapScale = this.m_HeightScale.floatValue;
			}
			if (!this.m_NormalMapFilter.hasMultipleDifferentValues)
			{
				settings.normalMapFilter = (TextureImporterNormalFilter)this.m_NormalMapFilter.intValue;
			}
			if (!this.m_GenerateCubemap.hasMultipleDifferentValues)
			{
				settings.generateCubemap = (TextureImporterGenerateCubemap)this.m_GenerateCubemap.intValue;
			}
			if (!this.m_CubemapConvolution.hasMultipleDifferentValues)
			{
				settings.cubemapConvolution = (TextureImporterCubemapConvolution)this.m_CubemapConvolution.intValue;
			}
			if (!this.m_SeamlessCubemap.hasMultipleDifferentValues)
			{
				settings.seamlessCubemap = (this.m_SeamlessCubemap.intValue > 0);
			}
			if (!this.m_BorderMipMap.hasMultipleDifferentValues)
			{
				settings.borderMipmap = (this.m_BorderMipMap.intValue > 0);
			}
			if (!this.m_NPOTScale.hasMultipleDifferentValues)
			{
				settings.npotScale = (TextureImporterNPOTScale)this.m_NPOTScale.intValue;
			}
			if (!this.m_IsReadable.hasMultipleDifferentValues)
			{
				settings.readable = (this.m_IsReadable.intValue > 0);
			}
			if (!this.m_sRGBTexture.hasMultipleDifferentValues)
			{
				settings.sRGBTexture = (this.m_sRGBTexture.intValue > 0);
			}
			if (!this.m_EnableMipMap.hasMultipleDifferentValues)
			{
				settings.mipmapEnabled = (this.m_EnableMipMap.intValue > 0);
			}
			if (!this.m_MipMapMode.hasMultipleDifferentValues)
			{
				settings.mipmapFilter = (TextureImporterMipFilter)this.m_MipMapMode.intValue;
			}
			if (!this.m_FadeOut.hasMultipleDifferentValues)
			{
				settings.fadeOut = (this.m_FadeOut.intValue > 0);
			}
			if (!this.m_MipMapFadeDistanceStart.hasMultipleDifferentValues)
			{
				settings.mipmapFadeDistanceStart = this.m_MipMapFadeDistanceStart.intValue;
			}
			if (!this.m_MipMapFadeDistanceEnd.hasMultipleDifferentValues)
			{
				settings.mipmapFadeDistanceEnd = this.m_MipMapFadeDistanceEnd.intValue;
			}
			if (!this.m_SpriteMode.hasMultipleDifferentValues)
			{
				settings.spriteMode = this.m_SpriteMode.intValue;
			}
			if (!this.m_SpritePixelsToUnits.hasMultipleDifferentValues)
			{
				settings.spritePixelsPerUnit = this.m_SpritePixelsToUnits.floatValue;
			}
			if (!this.m_SpriteExtrude.hasMultipleDifferentValues)
			{
				settings.spriteExtrude = (uint)this.m_SpriteExtrude.intValue;
			}
			if (!this.m_SpriteMeshType.hasMultipleDifferentValues)
			{
				settings.spriteMeshType = (SpriteMeshType)this.m_SpriteMeshType.intValue;
			}
			if (!this.m_Alignment.hasMultipleDifferentValues)
			{
				settings.spriteAlignment = this.m_Alignment.intValue;
			}
			if (!this.m_SpritePivot.hasMultipleDifferentValues)
			{
				settings.spritePivot = this.m_SpritePivot.vector2Value;
			}
			if (!this.m_WrapMode.hasMultipleDifferentValues)
			{
				settings.wrapMode = (TextureWrapMode)this.m_WrapMode.intValue;
			}
			if (!this.m_FilterMode.hasMultipleDifferentValues)
			{
				settings.filterMode = (FilterMode)this.m_FilterMode.intValue;
			}
			if (!this.m_Aniso.hasMultipleDifferentValues)
			{
				settings.aniso = this.m_Aniso.intValue;
			}
			if (!this.m_AlphaIsTransparency.hasMultipleDifferentValues)
			{
				settings.alphaIsTransparency = (this.m_AlphaIsTransparency.intValue > 0);
			}
			if (!this.m_TextureType.hasMultipleDifferentValues)
			{
				settings.textureType = (TextureImporterType)this.m_TextureType.intValue;
			}
			if (!this.m_TextureShape.hasMultipleDifferentValues)
			{
				settings.textureShape = (TextureImporterShape)this.m_TextureShape.intValue;
			}
			return settings;
		}

		private void CookieGUI(TextureImporterInspector.TextureInspectorGUIElement guiElements)
		{
			EditorGUI.BeginChangeCheck();
			TextureImporterInspector.CookieMode cookieMode;
			if (this.m_BorderMipMap.intValue > 0)
			{
				cookieMode = TextureImporterInspector.CookieMode.Spot;
			}
			else if (this.m_TextureShape.intValue == 2)
			{
				cookieMode = TextureImporterInspector.CookieMode.Point;
			}
			else
			{
				cookieMode = TextureImporterInspector.CookieMode.Directional;
			}
			cookieMode = (TextureImporterInspector.CookieMode)EditorGUILayout.Popup(TextureImporterInspector.s_Styles.cookieType, (int)cookieMode, TextureImporterInspector.s_Styles.cookieOptions, new GUILayoutOption[0]);
			if (EditorGUI.EndChangeCheck())
			{
				this.SetCookieMode(cookieMode);
			}
			if (cookieMode == TextureImporterInspector.CookieMode.Point)
			{
				this.m_TextureShape.intValue = 2;
			}
			else
			{
				this.m_TextureShape.intValue = 1;
			}
		}

		private void CubemapMappingGUI(TextureImporterInspector.TextureInspectorGUIElement guiElements)
		{
			this.m_ShowCubeMapSettings.target = (this.m_TextureShape.intValue == 2);
			if (EditorGUILayout.BeginFadeGroup(this.m_ShowCubeMapSettings.faded))
			{
				if (this.m_TextureShape.intValue == 2)
				{
					using (new EditorGUI.DisabledScope(!this.m_IsPOT && this.m_NPOTScale.intValue == 0))
					{
						EditorGUI.showMixedValue = (this.m_GenerateCubemap.hasMultipleDifferentValues || this.m_SeamlessCubemap.hasMultipleDifferentValues);
						EditorGUI.BeginChangeCheck();
						int intValue = EditorGUILayout.IntPopup(TextureImporterInspector.s_Styles.cubemap, this.m_GenerateCubemap.intValue, TextureImporterInspector.s_Styles.cubemapOptions, TextureImporterInspector.s_Styles.cubemapValues2, new GUILayoutOption[0]);
						if (EditorGUI.EndChangeCheck())
						{
							this.m_GenerateCubemap.intValue = intValue;
						}
						EditorGUI.indentLevel++;
						if (this.ShouldDisplayGUIElement(guiElements, TextureImporterInspector.TextureInspectorGUIElement.CubeMapConvolution))
						{
							EditorGUILayout.IntPopup(this.m_CubemapConvolution, TextureImporterInspector.s_Styles.cubemapConvolutionOptions, TextureImporterInspector.s_Styles.cubemapConvolutionValues, TextureImporterInspector.s_Styles.cubemapConvolution, new GUILayoutOption[0]);
						}
						this.ToggleFromInt(this.m_SeamlessCubemap, TextureImporterInspector.s_Styles.seamlessCubemap);
						EditorGUI.indentLevel--;
						EditorGUI.showMixedValue = false;
						EditorGUILayout.Space();
					}
				}
			}
			EditorGUILayout.EndFadeGroup();
		}

		private void ColorSpaceGUI(TextureImporterInspector.TextureInspectorGUIElement guiElements)
		{
			this.ToggleFromInt(this.m_sRGBTexture, TextureImporterInspector.s_Styles.sRGBTexture);
		}

		private void POTScaleGUI(TextureImporterInspector.TextureInspectorGUIElement guiElements)
		{
			using (new EditorGUI.DisabledScope(this.m_IsPOT))
			{
				this.EnumPopup(this.m_NPOTScale, typeof(TextureImporterNPOTScale), TextureImporterInspector.s_Styles.npot);
			}
		}

		private void ReadableGUI(TextureImporterInspector.TextureInspectorGUIElement guiElements)
		{
			this.ToggleFromInt(this.m_IsReadable, TextureImporterInspector.s_Styles.readWrite);
		}

		private void AlphaHandlingGUI(TextureImporterInspector.TextureInspectorGUIElement guiElements)
		{
			int num = 0;
			int num2 = 0;
			bool flag = TextureImporterInspector.CountImportersWithAlpha(base.targets, out num);
			flag = (flag && TextureImporterInspector.CountImportersWithHDR(base.targets, out num2));
			EditorGUI.showMixedValue = this.m_AlphaSource.hasMultipleDifferentValues;
			EditorGUI.BeginChangeCheck();
			int intValue = EditorGUILayout.IntPopup(TextureImporterInspector.s_Styles.alphaSource, this.m_AlphaSource.intValue, TextureImporterInspector.s_Styles.alphaSourceOptions, TextureImporterInspector.s_Styles.alphaSourceValues, new GUILayoutOption[0]);
			EditorGUI.showMixedValue = false;
			if (EditorGUI.EndChangeCheck())
			{
				this.m_AlphaSource.intValue = intValue;
			}
			bool flag2 = flag && this.m_AlphaSource.intValue != 0 && num2 == 0;
			using (new EditorGUI.DisabledScope(!flag2))
			{
				this.ToggleFromInt(this.m_AlphaIsTransparency, TextureImporterInspector.s_Styles.alphaIsTransparency);
			}
		}

		private void SpriteGUI(TextureImporterInspector.TextureInspectorGUIElement guiElements)
		{
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.IntPopup(this.m_SpriteMode, TextureImporterInspector.s_Styles.spriteModeOptions, new int[]
			{
				1,
				2,
				3
			}, TextureImporterInspector.s_Styles.spriteMode, new GUILayoutOption[0]);
			if (EditorGUI.EndChangeCheck())
			{
				GUIUtility.keyboardControl = 0;
			}
			EditorGUI.indentLevel++;
			this.m_ShowGenericSpriteSettings.target = (this.m_SpriteMode.intValue != 0);
			if (EditorGUILayout.BeginFadeGroup(this.m_ShowGenericSpriteSettings.faded))
			{
				EditorGUILayout.PropertyField(this.m_SpritePackingTag, TextureImporterInspector.s_Styles.spritePackingTag, new GUILayoutOption[0]);
				EditorGUILayout.PropertyField(this.m_SpritePixelsToUnits, TextureImporterInspector.s_Styles.spritePixelsPerUnit, new GUILayoutOption[0]);
				EditorGUILayout.IntPopup(this.m_SpriteMeshType, TextureImporterInspector.s_Styles.spriteMeshTypeOptions, new int[]
				{
					0,
					1
				}, TextureImporterInspector.s_Styles.spriteMeshType, new GUILayoutOption[0]);
				EditorGUILayout.IntSlider(this.m_SpriteExtrude, 0, 32, TextureImporterInspector.s_Styles.spriteExtrude, new GUILayoutOption[0]);
				if (this.m_SpriteMode.intValue == 1)
				{
					EditorGUILayout.Popup(this.m_Alignment, TextureImporterInspector.s_Styles.spriteAlignmentOptions, TextureImporterInspector.s_Styles.spriteAlignment, new GUILayoutOption[0]);
					if (this.m_Alignment.intValue == 9)
					{
						GUILayout.BeginHorizontal(new GUILayoutOption[0]);
						EditorGUILayout.PropertyField(this.m_SpritePivot, this.m_EmptyContent, new GUILayoutOption[0]);
						GUILayout.EndHorizontal();
					}
				}
				using (new EditorGUI.DisabledScope(base.targets.Length != 1))
				{
					GUILayout.BeginHorizontal(new GUILayoutOption[0]);
					GUILayout.FlexibleSpace();
					if (GUILayout.Button("Sprite Editor", new GUILayoutOption[0]))
					{
						if (this.HasModified())
						{
							string text = "Unapplied import settings for '" + ((TextureImporter)base.target).assetPath + "'.\n";
							text += "Apply and continue to sprite editor or cancel.";
							if (EditorUtility.DisplayDialog("Unapplied import settings", text, "Apply", "Cancel"))
							{
								base.ApplyAndImport();
								SpriteEditorWindow.GetWindow();
								GUIUtility.ExitGUI();
							}
						}
						else
						{
							SpriteEditorWindow.GetWindow();
						}
					}
					GUILayout.EndHorizontal();
				}
			}
			EditorGUILayout.EndFadeGroup();
			EditorGUI.indentLevel--;
		}

		private void MipMapGUI(TextureImporterInspector.TextureInspectorGUIElement guiElements)
		{
			this.ToggleFromInt(this.m_EnableMipMap, TextureImporterInspector.s_Styles.generateMipMaps);
			this.m_ShowMipMapSettings.target = (this.m_EnableMipMap.boolValue && !this.m_EnableMipMap.hasMultipleDifferentValues);
			if (EditorGUILayout.BeginFadeGroup(this.m_ShowMipMapSettings.faded))
			{
				EditorGUI.indentLevel++;
				this.ToggleFromInt(this.m_BorderMipMap, TextureImporterInspector.s_Styles.borderMipMaps);
				EditorGUILayout.Popup(this.m_MipMapMode, TextureImporterInspector.s_Styles.mipMapFilterOptions, TextureImporterInspector.s_Styles.mipMapFilter, new GUILayoutOption[0]);
				this.ToggleFromInt(this.m_FadeOut, TextureImporterInspector.s_Styles.mipmapFadeOutToggle);
				if (this.m_FadeOut.intValue > 0)
				{
					EditorGUI.indentLevel++;
					EditorGUI.BeginChangeCheck();
					float f = (float)this.m_MipMapFadeDistanceStart.intValue;
					float f2 = (float)this.m_MipMapFadeDistanceEnd.intValue;
					EditorGUILayout.MinMaxSlider(TextureImporterInspector.s_Styles.mipmapFadeOut, ref f, ref f2, 0f, 10f, new GUILayoutOption[0]);
					if (EditorGUI.EndChangeCheck())
					{
						this.m_MipMapFadeDistanceStart.intValue = Mathf.RoundToInt(f);
						this.m_MipMapFadeDistanceEnd.intValue = Mathf.RoundToInt(f2);
					}
					EditorGUI.indentLevel--;
				}
				EditorGUI.indentLevel--;
			}
			EditorGUILayout.EndFadeGroup();
		}

		private void BumpGUI(TextureImporterInspector.TextureInspectorGUIElement guiElements)
		{
			EditorGUI.BeginChangeCheck();
			this.ToggleFromInt(this.m_ConvertToNormalMap, TextureImporterInspector.s_Styles.generateFromBump);
			this.m_ShowBumpGenerationSettings.target = (this.m_ConvertToNormalMap.intValue > 0);
			if (EditorGUILayout.BeginFadeGroup(this.m_ShowBumpGenerationSettings.faded))
			{
				EditorGUI.indentLevel++;
				EditorGUILayout.Slider(this.m_HeightScale, 0f, 0.3f, TextureImporterInspector.s_Styles.bumpiness, new GUILayoutOption[0]);
				EditorGUILayout.Popup(this.m_NormalMapFilter, TextureImporterInspector.s_Styles.bumpFilteringOptions, TextureImporterInspector.s_Styles.bumpFiltering, new GUILayoutOption[0]);
				EditorGUI.indentLevel--;
			}
			EditorGUILayout.EndFadeGroup();
			if (EditorGUI.EndChangeCheck())
			{
				this.SyncPlatformSettings();
			}
		}

		private void TextureSettingsGUI()
		{
			EditorGUI.BeginChangeCheck();
			EditorGUI.BeginChangeCheck();
			EditorGUI.showMixedValue = this.m_WrapMode.hasMultipleDifferentValues;
			TextureWrapMode textureWrapMode = (TextureWrapMode)this.m_WrapMode.intValue;
			if (textureWrapMode == (TextureWrapMode)(-1))
			{
				textureWrapMode = TextureWrapMode.Repeat;
			}
			textureWrapMode = (TextureWrapMode)EditorGUILayout.EnumPopup(EditorGUIUtility.TempContent("Wrap Mode"), textureWrapMode, new GUILayoutOption[0]);
			EditorGUI.showMixedValue = false;
			if (EditorGUI.EndChangeCheck())
			{
				this.m_WrapMode.intValue = (int)textureWrapMode;
			}
			if (this.m_NPOTScale.intValue == 0 && textureWrapMode == TextureWrapMode.Repeat && !ShaderUtil.hardwareSupportsFullNPOT)
			{
				bool flag = false;
				UnityEngine.Object[] targets = base.targets;
				for (int i = 0; i < targets.Length; i++)
				{
					UnityEngine.Object @object = targets[i];
					int value = -1;
					int value2 = -1;
					TextureImporter textureImporter = (TextureImporter)@object;
					textureImporter.GetWidthAndHeight(ref value, ref value2);
					if (!Mathf.IsPowerOfTwo(value) || !Mathf.IsPowerOfTwo(value2))
					{
						flag = true;
						break;
					}
				}
				if (flag)
				{
					GUIContent gUIContent = EditorGUIUtility.TextContent("Graphics device doesn't support Repeat wrap mode on NPOT textures. Falling back to Clamp.");
					EditorGUILayout.HelpBox(gUIContent.text, MessageType.Warning, true);
				}
			}
			EditorGUI.BeginChangeCheck();
			EditorGUI.showMixedValue = this.m_FilterMode.hasMultipleDifferentValues;
			FilterMode filterMode = (FilterMode)this.m_FilterMode.intValue;
			if (filterMode == (FilterMode)(-1))
			{
				if (this.m_FadeOut.intValue > 0 || this.m_ConvertToNormalMap.intValue > 0)
				{
					filterMode = FilterMode.Trilinear;
				}
				else
				{
					filterMode = FilterMode.Bilinear;
				}
			}
			filterMode = (FilterMode)EditorGUILayout.IntPopup(TextureImporterInspector.s_Styles.filterMode, (int)filterMode, TextureImporterInspector.s_Styles.filterModeOptions, this.m_FilterModeOptions, new GUILayoutOption[0]);
			EditorGUI.showMixedValue = false;
			if (EditorGUI.EndChangeCheck())
			{
				this.m_FilterMode.intValue = (int)filterMode;
			}
			bool flag2 = this.m_FilterMode.intValue != 0 && this.m_EnableMipMap.intValue > 0 && this.m_TextureShape.intValue != 2;
			using (new EditorGUI.DisabledScope(!flag2))
			{
				EditorGUI.BeginChangeCheck();
				EditorGUI.showMixedValue = this.m_Aniso.hasMultipleDifferentValues;
				int num = this.m_Aniso.intValue;
				if (num == -1)
				{
					num = 1;
				}
				num = EditorGUILayout.IntSlider("Aniso Level", num, 0, 16, new GUILayoutOption[0]);
				EditorGUI.showMixedValue = false;
				if (EditorGUI.EndChangeCheck())
				{
					this.m_Aniso.intValue = num;
				}
				TextureInspector.DoAnisoGlobalSettingNote(num);
			}
			if (EditorGUI.EndChangeCheck())
			{
				this.ApplySettingsToTexture();
			}
		}

		public override void OnInspectorGUI()
		{
			if (TextureImporterInspector.s_Styles == null)
			{
				TextureImporterInspector.s_Styles = new TextureImporterInspector.Styles();
			}
			bool enabled = GUI.enabled;
			EditorGUILayout.Space();
			EditorGUI.BeginChangeCheck();
			EditorGUI.showMixedValue = this.m_TextureType.hasMultipleDifferentValues;
			int num = EditorGUILayout.IntPopup(TextureImporterInspector.s_Styles.textureTypeTitle, this.m_TextureType.intValue, TextureImporterInspector.s_Styles.textureTypeOptions, TextureImporterInspector.s_Styles.textureTypeValues, new GUILayoutOption[0]);
			EditorGUI.showMixedValue = false;
			if (EditorGUI.EndChangeCheck())
			{
				this.m_TextureType.intValue = num;
				TextureImporterSettings serializedPropertySettings = this.GetSerializedPropertySettings();
				serializedPropertySettings.ApplyTextureType((TextureImporterType)this.m_TextureType.intValue);
				this.SetSerializedPropertySettings(serializedPropertySettings);
				this.SyncPlatformSettings();
				this.ApplySettingsToTexture();
			}
			int[] array = TextureImporterInspector.s_Styles.textureShapeValuesDictionnary[this.m_TextureTypeGUIElements[num].shapeCaps];
			using (new EditorGUI.DisabledScope(array.Length == 1 || this.m_TextureType.intValue == 4))
			{
				EditorGUI.BeginChangeCheck();
				EditorGUI.showMixedValue = this.m_TextureShape.hasMultipleDifferentValues;
				int intValue = EditorGUILayout.IntPopup(TextureImporterInspector.s_Styles.textureShape, this.m_TextureShape.intValue, TextureImporterInspector.s_Styles.textureShapeOptionsDictionnary[this.m_TextureTypeGUIElements[num].shapeCaps], TextureImporterInspector.s_Styles.textureShapeValuesDictionnary[this.m_TextureTypeGUIElements[num].shapeCaps], new GUILayoutOption[0]);
				EditorGUI.showMixedValue = false;
				if (EditorGUI.EndChangeCheck())
				{
					this.m_TextureShape.intValue = intValue;
				}
			}
			if (Array.IndexOf<int>(array, this.m_TextureShape.intValue) == -1)
			{
				this.m_TextureShape.intValue = array[0];
			}
			EditorGUILayout.Space();
			if (!this.m_TextureType.hasMultipleDifferentValues)
			{
				this.DoGUIElements(this.m_TextureTypeGUIElements[num].commonElements, this.m_GUIElementsDisplayOrder);
				if (this.m_TextureTypeGUIElements[num].advancedElements != TextureImporterInspector.TextureInspectorGUIElement.None)
				{
					EditorGUILayout.Space();
					this.m_ShowAdvanced = EditorGUILayout.Foldout(this.m_ShowAdvanced, TextureImporterInspector.s_Styles.showAdvanced, true);
					if (this.m_ShowAdvanced)
					{
						EditorGUI.indentLevel++;
						this.DoGUIElements(this.m_TextureTypeGUIElements[num].advancedElements, this.m_GUIElementsDisplayOrder);
						EditorGUI.indentLevel--;
					}
				}
			}
			EditorGUILayout.Space();
			this.TextureSettingsGUI();
			this.ShowPlatformSpecificSettings();
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.FlexibleSpace();
			base.ApplyRevertGUI();
			GUILayout.EndHorizontal();
			this.UpdateImportWarning();
			if (this.m_ImportWarning != null)
			{
				EditorGUILayout.HelpBox(this.m_ImportWarning, MessageType.Warning);
			}
			GUI.enabled = enabled;
		}

		private bool ShouldDisplayGUIElement(TextureImporterInspector.TextureInspectorGUIElement guiElements, TextureImporterInspector.TextureInspectorGUIElement guiElement)
		{
			return (guiElements & guiElement) == guiElement;
		}

		private void DoGUIElements(TextureImporterInspector.TextureInspectorGUIElement guiElements, List<TextureImporterInspector.TextureInspectorGUIElement> guiElementsDisplayOrder)
		{
			foreach (TextureImporterInspector.TextureInspectorGUIElement current in guiElementsDisplayOrder)
			{
				if (this.ShouldDisplayGUIElement(guiElements, current) && this.m_GUIElementMethods.ContainsKey(current))
				{
					this.m_GUIElementMethods[current](guiElements);
				}
			}
		}

		private void ApplySettingsToTexture()
		{
			UnityEngine.Object[] targets = base.targets;
			for (int i = 0; i < targets.Length; i++)
			{
				AssetImporter assetImporter = (AssetImporter)targets[i];
				Texture tex = AssetDatabase.LoadMainAssetAtPath(assetImporter.assetPath) as Texture;
				if (this.m_Aniso.intValue != -1)
				{
					TextureUtil.SetAnisoLevelNoDirty(tex, this.m_Aniso.intValue);
				}
				if (this.m_FilterMode.intValue != -1)
				{
					TextureUtil.SetFilterModeNoDirty(tex, (FilterMode)this.m_FilterMode.intValue);
				}
				if (this.m_WrapMode.intValue != -1)
				{
					TextureUtil.SetWrapModeNoDirty(tex, (TextureWrapMode)this.m_WrapMode.intValue);
				}
			}
			SceneView.RepaintAll();
		}

		private static bool CountImportersWithAlpha(UnityEngine.Object[] importers, out int count)
		{
			bool result;
			try
			{
				count = 0;
				for (int i = 0; i < importers.Length; i++)
				{
					UnityEngine.Object @object = importers[i];
					if ((@object as TextureImporter).DoesSourceTextureHaveAlpha())
					{
						count++;
					}
				}
				result = true;
			}
			catch
			{
				count = importers.Length;
				result = false;
			}
			return result;
		}

		private static bool CountImportersWithHDR(UnityEngine.Object[] importers, out int count)
		{
			bool result;
			try
			{
				count = 0;
				for (int i = 0; i < importers.Length; i++)
				{
					UnityEngine.Object @object = importers[i];
					if ((@object as TextureImporter).IsSourceTextureHDR())
					{
						count++;
					}
				}
				result = true;
			}
			catch
			{
				count = importers.Length;
				result = false;
			}
			return result;
		}

		private void SetCookieMode(TextureImporterInspector.CookieMode cm)
		{
			if (cm != TextureImporterInspector.CookieMode.Spot)
			{
				if (cm != TextureImporterInspector.CookieMode.Point)
				{
					if (cm == TextureImporterInspector.CookieMode.Directional)
					{
						this.m_BorderMipMap.intValue = 0;
						this.m_WrapMode.intValue = 0;
						this.m_GenerateCubemap.intValue = 6;
						this.m_TextureShape.intValue = 1;
					}
				}
				else
				{
					this.m_BorderMipMap.intValue = 0;
					this.m_WrapMode.intValue = 1;
					this.m_GenerateCubemap.intValue = 1;
					this.m_TextureShape.intValue = 2;
				}
			}
			else
			{
				this.m_BorderMipMap.intValue = 1;
				this.m_WrapMode.intValue = 1;
				this.m_GenerateCubemap.intValue = 6;
				this.m_TextureShape.intValue = 1;
			}
		}

		private void SyncPlatformSettings()
		{
			foreach (TextureImportPlatformSettings current in this.m_PlatformSettings)
			{
				current.Sync();
			}
		}

		internal static string[] BuildTextureStrings(int[] texFormatValues)
		{
			string[] array = new string[texFormatValues.Length];
			for (int i = 0; i < texFormatValues.Length; i++)
			{
				int format = texFormatValues[i];
				array[i] = " " + TextureUtil.GetTextureFormatString((TextureFormat)format);
			}
			return array;
		}

		protected void ShowPlatformSpecificSettings()
		{
			BuildPlayerWindow.BuildPlatform[] array = TextureImporterInspector.GetBuildPlayerValidPlatforms().ToArray<BuildPlayerWindow.BuildPlatform>();
			GUILayout.Space(10f);
			int num = EditorGUILayout.BeginPlatformGrouping(array, TextureImporterInspector.s_Styles.defaultPlatform);
			TextureImportPlatformSettings textureImportPlatformSettings = this.m_PlatformSettings[num + 1];
			if (!textureImportPlatformSettings.isDefault)
			{
				EditorGUI.BeginChangeCheck();
				EditorGUI.showMixedValue = textureImportPlatformSettings.overriddenIsDifferent;
				string label = "Override for " + array[num].title.text;
				bool overriddenForAll = EditorGUILayout.ToggleLeft(label, textureImportPlatformSettings.overridden, new GUILayoutOption[0]);
				EditorGUI.showMixedValue = false;
				if (EditorGUI.EndChangeCheck())
				{
					textureImportPlatformSettings.SetOverriddenForAll(overriddenForAll);
					this.SyncPlatformSettings();
				}
			}
			bool disabled = !textureImportPlatformSettings.isDefault && !textureImportPlatformSettings.allAreOverridden;
			using (new EditorGUI.DisabledScope(disabled))
			{
				ITextureImportSettingsExtension textureImportSettingsExtension = ModuleManager.GetTextureImportSettingsExtension(textureImportPlatformSettings.m_Target);
				textureImportSettingsExtension.ShowImportSettings(this, textureImportPlatformSettings);
				this.SyncPlatformSettings();
			}
			EditorGUILayout.EndPlatformGrouping();
		}

		private static bool IsPowerOfTwo(int f)
		{
			return (f & f - 1) == 0;
		}

		public static BuildPlayerWindow.BuildPlatform[] GetBuildPlayerValidPlatforms()
		{
			List<BuildPlayerWindow.BuildPlatform> validPlatforms = BuildPlayerWindow.GetValidPlatforms();
			return validPlatforms.ToArray();
		}

		public virtual void BuildTargetList()
		{
			BuildPlayerWindow.BuildPlatform[] buildPlayerValidPlatforms = TextureImporterInspector.GetBuildPlayerValidPlatforms();
			this.m_PlatformSettings = new List<TextureImportPlatformSettings>();
			this.m_PlatformSettings.Add(new TextureImportPlatformSettings(TextureImporterInspector.s_DefaultPlatformName, BuildTarget.StandaloneWindows, this));
			BuildPlayerWindow.BuildPlatform[] array = buildPlayerValidPlatforms;
			for (int i = 0; i < array.Length; i++)
			{
				BuildPlayerWindow.BuildPlatform buildPlatform = array[i];
				this.m_PlatformSettings.Add(new TextureImportPlatformSettings(buildPlatform.name, buildPlatform.DefaultTarget, this));
			}
		}

		internal override bool HasModified()
		{
			bool result;
			if (base.HasModified())
			{
				result = true;
			}
			else
			{
				foreach (TextureImportPlatformSettings current in this.m_PlatformSettings)
				{
					if (current.HasChanged())
					{
						result = true;
						return result;
					}
				}
				result = false;
			}
			return result;
		}

		public static void SelectMainAssets(UnityEngine.Object[] targets)
		{
			ArrayList arrayList = new ArrayList();
			for (int i = 0; i < targets.Length; i++)
			{
				AssetImporter assetImporter = (AssetImporter)targets[i];
				Texture texture = AssetDatabase.LoadMainAssetAtPath(assetImporter.assetPath) as Texture;
				if (texture)
				{
					arrayList.Add(texture);
				}
			}
			Selection.objects = (arrayList.ToArray(typeof(UnityEngine.Object)) as UnityEngine.Object[]);
		}

		internal override void ResetValues()
		{
			base.ResetValues();
			this.CacheSerializedProperties();
			this.BuildTargetList();
			this.ApplySettingsToTexture();
			TextureImporterInspector.SelectMainAssets(base.targets);
		}

		internal override void Apply()
		{
			SpriteEditorWindow.TextureImporterApply(base.serializedObject);
			base.Apply();
			this.SyncPlatformSettings();
			foreach (TextureImportPlatformSettings current in this.m_PlatformSettings)
			{
				current.Apply();
			}
		}
	}
}
