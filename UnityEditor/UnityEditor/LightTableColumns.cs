using System;
using UnityEngine;

namespace UnityEditor
{
	internal class LightTableColumns
	{
		private static class Styles
		{
			public static readonly GUIContent[] ProjectionStrings = new GUIContent[]
			{
				EditorGUIUtility.TextContent("Infinite"),
				EditorGUIUtility.TextContent("Box")
			};

			public static readonly GUIContent[] LightmapEmissiveStrings = new GUIContent[]
			{
				EditorGUIUtility.TextContent("Realtime"),
				EditorGUIUtility.TextContent("Baked")
			};

			public static readonly GUIContent Name = EditorGUIUtility.TextContent("Name");

			public static readonly GUIContent On = EditorGUIUtility.TextContent("On");

			public static readonly GUIContent Type = EditorGUIUtility.TextContent("Type");

			public static readonly GUIContent Mode = EditorGUIUtility.TextContent("Mode");

			public static readonly GUIContent Color = EditorGUIUtility.TextContent("Color");

			public static readonly GUIContent Intensity = EditorGUIUtility.TextContent("Intensity");

			public static readonly GUIContent IndirectMultiplier = EditorGUIUtility.TextContent("Indirect Multiplier");

			public static readonly GUIContent ShadowType = EditorGUIUtility.TextContent("Shadow Type");

			public static readonly GUIContent Projection = EditorGUIUtility.TextContent("Projection");

			public static readonly GUIContent HDR = EditorGUIUtility.TextContent("HDR");

			public static readonly GUIContent ShadowDistance = EditorGUIUtility.TextContent("Shadow Distance");

			public static readonly GUIContent NearPlane = EditorGUIUtility.TextContent("Near Plane");

			public static readonly GUIContent FarPlane = EditorGUIUtility.TextContent("Far Plane");

			public static readonly GUIContent GlobalIllumination = EditorGUIUtility.TextContent("Global Illumination");

			public static readonly GUIContent SelectObjects = EditorGUIUtility.TextContent("");

			public static readonly GUIContent SelectObjectsButton = EditorGUIUtility.TextContentWithIcon("|Find References in Scene", "UnityEditor.FindDependencies");
		}

		private static ColorPickerHDRConfig s_ColorPickerHDRConfig = new ColorPickerHDRConfig(0f, 99f, 0.01010101f, 3f);

		private static SerializedPropertyTreeView.Column[] FinalizeColumns(SerializedPropertyTreeView.Column[] columns, out string[] propNames)
		{
			propNames = new string[columns.Length];
			for (int i = 0; i < columns.Length; i++)
			{
				propNames[i] = columns[i].propertyName;
			}
			return columns;
		}

		private static bool IsEditable(UnityEngine.Object target)
		{
			return (target.hideFlags & HideFlags.NotEditable) == HideFlags.None;
		}

		public static SerializedPropertyTreeView.Column[] CreateLightColumns(out string[] propNames)
		{
			SerializedPropertyTreeView.Column[] expr_07 = new SerializedPropertyTreeView.Column[8];
			expr_07[0] = new SerializedPropertyTreeView.Column
			{
				headerContent = LightTableColumns.Styles.Name,
				headerTextAlignment = TextAlignment.Left,
				sortedAscending = true,
				sortingArrowAlignment = TextAlignment.Center,
				width = 200f,
				minWidth = 100f,
				autoResize = false,
				allowToggleVisibility = true,
				propertyName = null,
				dependencyIndices = null,
				compareDelegate = SerializedPropertyTreeView.DefaultDelegates.s_CompareName,
				drawDelegate = SerializedPropertyTreeView.DefaultDelegates.s_DrawName,
				filter = new SerializedPropertyFilters.Name()
			};
			expr_07[1] = new SerializedPropertyTreeView.Column
			{
				headerContent = LightTableColumns.Styles.On,
				headerTextAlignment = TextAlignment.Center,
				sortedAscending = true,
				sortingArrowAlignment = TextAlignment.Center,
				width = 25f,
				minWidth = 25f,
				maxWidth = 25f,
				autoResize = false,
				allowToggleVisibility = true,
				propertyName = "m_Enabled",
				dependencyIndices = null,
				compareDelegate = SerializedPropertyTreeView.DefaultDelegates.s_CompareCheckbox,
				drawDelegate = SerializedPropertyTreeView.DefaultDelegates.s_DrawCheckbox
			};
			expr_07[2] = new SerializedPropertyTreeView.Column
			{
				headerContent = LightTableColumns.Styles.Type,
				headerTextAlignment = TextAlignment.Left,
				sortedAscending = true,
				sortingArrowAlignment = TextAlignment.Center,
				width = 120f,
				minWidth = 60f,
				autoResize = false,
				allowToggleVisibility = true,
				propertyName = "m_Type",
				dependencyIndices = null,
				compareDelegate = SerializedPropertyTreeView.DefaultDelegates.s_CompareEnum,
				drawDelegate = SerializedPropertyTreeView.DefaultDelegates.s_DrawDefault
			};
			int arg_21C_1 = 3;
			SerializedPropertyTreeView.Column column = new SerializedPropertyTreeView.Column();
			column.headerContent = LightTableColumns.Styles.Mode;
			column.headerTextAlignment = TextAlignment.Left;
			column.sortedAscending = true;
			column.sortingArrowAlignment = TextAlignment.Center;
			column.width = 70f;
			column.minWidth = 40f;
			column.maxWidth = 70f;
			column.autoResize = false;
			column.allowToggleVisibility = true;
			column.propertyName = "m_Lightmapping";
			column.dependencyIndices = new int[]
			{
				2
			};
			column.compareDelegate = SerializedPropertyTreeView.DefaultDelegates.s_CompareEnum;
			column.drawDelegate = delegate(Rect r, SerializedProperty prop, SerializedProperty[] dep)
			{
				LightModeUtil.Get().DrawElement(r, prop, dep[0]);
			};
			expr_07[arg_21C_1] = column;
			expr_07[4] = new SerializedPropertyTreeView.Column
			{
				headerContent = LightTableColumns.Styles.Color,
				headerTextAlignment = TextAlignment.Left,
				sortedAscending = true,
				sortingArrowAlignment = TextAlignment.Center,
				width = 70f,
				minWidth = 40f,
				autoResize = false,
				allowToggleVisibility = true,
				propertyName = "m_Color",
				dependencyIndices = null,
				compareDelegate = SerializedPropertyTreeView.DefaultDelegates.s_CompareColor,
				drawDelegate = SerializedPropertyTreeView.DefaultDelegates.s_DrawDefault
			};
			expr_07[5] = new SerializedPropertyTreeView.Column
			{
				headerContent = LightTableColumns.Styles.Intensity,
				headerTextAlignment = TextAlignment.Left,
				sortedAscending = true,
				sortingArrowAlignment = TextAlignment.Center,
				width = 60f,
				minWidth = 30f,
				autoResize = false,
				allowToggleVisibility = true,
				propertyName = "m_Intensity",
				dependencyIndices = null,
				compareDelegate = SerializedPropertyTreeView.DefaultDelegates.s_CompareFloat,
				drawDelegate = SerializedPropertyTreeView.DefaultDelegates.s_DrawDefault
			};
			expr_07[6] = new SerializedPropertyTreeView.Column
			{
				headerContent = LightTableColumns.Styles.IndirectMultiplier,
				headerTextAlignment = TextAlignment.Left,
				sortedAscending = true,
				sortingArrowAlignment = TextAlignment.Center,
				width = 110f,
				minWidth = 60f,
				autoResize = false,
				allowToggleVisibility = true,
				propertyName = "m_BounceIntensity",
				dependencyIndices = null,
				compareDelegate = SerializedPropertyTreeView.DefaultDelegates.s_CompareFloat,
				drawDelegate = SerializedPropertyTreeView.DefaultDelegates.s_DrawDefault
			};
			expr_07[7] = new SerializedPropertyTreeView.Column
			{
				headerContent = LightTableColumns.Styles.ShadowType,
				headerTextAlignment = TextAlignment.Left,
				sortedAscending = true,
				sortingArrowAlignment = TextAlignment.Center,
				width = 100f,
				minWidth = 60f,
				autoResize = false,
				allowToggleVisibility = true,
				propertyName = "m_Shadows.m_Type",
				dependencyIndices = null,
				compareDelegate = SerializedPropertyTreeView.DefaultDelegates.s_CompareEnum,
				drawDelegate = SerializedPropertyTreeView.DefaultDelegates.s_DrawDefault
			};
			SerializedPropertyTreeView.Column[] columns = expr_07;
			return LightTableColumns.FinalizeColumns(columns, out propNames);
		}

		public static SerializedPropertyTreeView.Column[] CreateReflectionColumns(out string[] propNames)
		{
			SerializedPropertyTreeView.Column[] expr_07 = new SerializedPropertyTreeView.Column[8];
			expr_07[0] = new SerializedPropertyTreeView.Column
			{
				headerContent = LightTableColumns.Styles.Name,
				headerTextAlignment = TextAlignment.Left,
				sortedAscending = true,
				sortingArrowAlignment = TextAlignment.Center,
				width = 200f,
				minWidth = 100f,
				autoResize = false,
				allowToggleVisibility = true,
				propertyName = null,
				dependencyIndices = null,
				compareDelegate = SerializedPropertyTreeView.DefaultDelegates.s_CompareName,
				drawDelegate = SerializedPropertyTreeView.DefaultDelegates.s_DrawName,
				filter = new SerializedPropertyFilters.Name()
			};
			expr_07[1] = new SerializedPropertyTreeView.Column
			{
				headerContent = LightTableColumns.Styles.On,
				headerTextAlignment = TextAlignment.Center,
				sortedAscending = true,
				sortingArrowAlignment = TextAlignment.Center,
				width = 25f,
				minWidth = 25f,
				maxWidth = 25f,
				autoResize = false,
				allowToggleVisibility = true,
				propertyName = "m_Enabled",
				dependencyIndices = null,
				compareDelegate = SerializedPropertyTreeView.DefaultDelegates.s_CompareCheckbox,
				drawDelegate = SerializedPropertyTreeView.DefaultDelegates.s_DrawCheckbox
			};
			int arg_192_1 = 2;
			SerializedPropertyTreeView.Column column = new SerializedPropertyTreeView.Column();
			column.headerContent = LightTableColumns.Styles.Mode;
			column.headerTextAlignment = TextAlignment.Left;
			column.sortedAscending = true;
			column.sortingArrowAlignment = TextAlignment.Center;
			column.width = 70f;
			column.minWidth = 40f;
			column.autoResize = false;
			column.allowToggleVisibility = true;
			column.propertyName = "m_Mode";
			column.dependencyIndices = null;
			column.compareDelegate = SerializedPropertyTreeView.DefaultDelegates.s_CompareInt;
			column.drawDelegate = delegate(Rect r, SerializedProperty prop, SerializedProperty[] dep)
			{
				EditorGUI.IntPopup(r, prop, ReflectionProbeEditor.Styles.reflectionProbeMode, ReflectionProbeEditor.Styles.reflectionProbeModeValues, GUIContent.none);
			};
			expr_07[arg_192_1] = column;
			int arg_220_1 = 3;
			column = new SerializedPropertyTreeView.Column();
			column.headerContent = LightTableColumns.Styles.Projection;
			column.headerTextAlignment = TextAlignment.Left;
			column.sortedAscending = true;
			column.sortingArrowAlignment = TextAlignment.Center;
			column.width = 80f;
			column.minWidth = 40f;
			column.autoResize = false;
			column.allowToggleVisibility = true;
			column.propertyName = "m_BoxProjection";
			column.dependencyIndices = null;
			column.compareDelegate = SerializedPropertyTreeView.DefaultDelegates.s_CompareCheckbox;
			column.drawDelegate = delegate(Rect r, SerializedProperty prop, SerializedProperty[] dep)
			{
				int[] optionValues = new int[]
				{
					0,
					1
				};
				prop.boolValue = (EditorGUI.IntPopup(r, (!prop.boolValue) ? 0 : 1, LightTableColumns.Styles.ProjectionStrings, optionValues) == 1);
			};
			expr_07[arg_220_1] = column;
			expr_07[4] = new SerializedPropertyTreeView.Column
			{
				headerContent = LightTableColumns.Styles.HDR,
				headerTextAlignment = TextAlignment.Center,
				sortedAscending = true,
				sortingArrowAlignment = TextAlignment.Center,
				width = 35f,
				minWidth = 35f,
				maxWidth = 35f,
				autoResize = false,
				allowToggleVisibility = true,
				propertyName = "m_HDR",
				dependencyIndices = null,
				compareDelegate = SerializedPropertyTreeView.DefaultDelegates.s_CompareCheckbox,
				drawDelegate = SerializedPropertyTreeView.DefaultDelegates.s_DrawCheckbox
			};
			expr_07[5] = new SerializedPropertyTreeView.Column
			{
				headerContent = LightTableColumns.Styles.ShadowDistance,
				headerTextAlignment = TextAlignment.Left,
				sortedAscending = true,
				sortingArrowAlignment = TextAlignment.Center,
				width = 110f,
				minWidth = 50f,
				autoResize = false,
				allowToggleVisibility = true,
				propertyName = "m_ShadowDistance",
				dependencyIndices = null,
				compareDelegate = SerializedPropertyTreeView.DefaultDelegates.s_CompareFloat,
				drawDelegate = SerializedPropertyTreeView.DefaultDelegates.s_DrawDefault
			};
			expr_07[6] = new SerializedPropertyTreeView.Column
			{
				headerContent = LightTableColumns.Styles.NearPlane,
				headerTextAlignment = TextAlignment.Left,
				sortedAscending = true,
				sortingArrowAlignment = TextAlignment.Center,
				width = 70f,
				minWidth = 30f,
				autoResize = false,
				allowToggleVisibility = true,
				propertyName = "m_NearClip",
				dependencyIndices = null,
				compareDelegate = SerializedPropertyTreeView.DefaultDelegates.s_CompareFloat,
				drawDelegate = SerializedPropertyTreeView.DefaultDelegates.s_DrawDefault
			};
			expr_07[7] = new SerializedPropertyTreeView.Column
			{
				headerContent = LightTableColumns.Styles.FarPlane,
				headerTextAlignment = TextAlignment.Left,
				sortedAscending = true,
				sortingArrowAlignment = TextAlignment.Center,
				width = 70f,
				minWidth = 30f,
				autoResize = false,
				allowToggleVisibility = true,
				propertyName = "m_FarClip",
				dependencyIndices = null,
				compareDelegate = SerializedPropertyTreeView.DefaultDelegates.s_CompareFloat,
				drawDelegate = SerializedPropertyTreeView.DefaultDelegates.s_DrawDefault
			};
			SerializedPropertyTreeView.Column[] columns = expr_07;
			return LightTableColumns.FinalizeColumns(columns, out propNames);
		}

		public static SerializedPropertyTreeView.Column[] CreateLightProbeColumns(out string[] propNames)
		{
			SerializedPropertyTreeView.Column[] columns = new SerializedPropertyTreeView.Column[]
			{
				new SerializedPropertyTreeView.Column
				{
					headerContent = LightTableColumns.Styles.Name,
					headerTextAlignment = TextAlignment.Left,
					sortedAscending = true,
					sortingArrowAlignment = TextAlignment.Center,
					width = 200f,
					minWidth = 100f,
					autoResize = false,
					allowToggleVisibility = true,
					propertyName = null,
					dependencyIndices = null,
					compareDelegate = SerializedPropertyTreeView.DefaultDelegates.s_CompareName,
					drawDelegate = SerializedPropertyTreeView.DefaultDelegates.s_DrawName,
					filter = new SerializedPropertyFilters.Name()
				},
				new SerializedPropertyTreeView.Column
				{
					headerContent = LightTableColumns.Styles.On,
					headerTextAlignment = TextAlignment.Center,
					sortedAscending = true,
					sortingArrowAlignment = TextAlignment.Center,
					width = 25f,
					minWidth = 25f,
					maxWidth = 25f,
					autoResize = false,
					allowToggleVisibility = true,
					propertyName = "m_Enabled",
					dependencyIndices = null,
					compareDelegate = SerializedPropertyTreeView.DefaultDelegates.s_CompareCheckbox,
					drawDelegate = SerializedPropertyTreeView.DefaultDelegates.s_DrawCheckbox
				}
			};
			return LightTableColumns.FinalizeColumns(columns, out propNames);
		}

		public static SerializedPropertyTreeView.Column[] CreateEmissivesColumns(out string[] propNames)
		{
			SerializedPropertyTreeView.Column[] expr_07 = new SerializedPropertyTreeView.Column[4];
			int arg_9B_1 = 0;
			SerializedPropertyTreeView.Column column = new SerializedPropertyTreeView.Column();
			column.headerContent = LightTableColumns.Styles.SelectObjects;
			column.headerTextAlignment = TextAlignment.Left;
			column.sortedAscending = true;
			column.sortingArrowAlignment = TextAlignment.Center;
			column.width = 20f;
			column.minWidth = 20f;
			column.maxWidth = 20f;
			column.autoResize = false;
			column.allowToggleVisibility = true;
			column.propertyName = "m_LightmapFlags";
			column.dependencyIndices = null;
			column.compareDelegate = null;
			column.drawDelegate = delegate(Rect r, SerializedProperty prop, SerializedProperty[] dep)
			{
				if (GUI.Button(r, LightTableColumns.Styles.SelectObjectsButton, "label"))
				{
					SearchableEditorWindow.SearchForReferencesToInstanceID(prop.serializedObject.targetObject.GetInstanceID());
				}
			};
			expr_07[arg_9B_1] = column;
			expr_07[1] = new SerializedPropertyTreeView.Column
			{
				headerContent = LightTableColumns.Styles.Name,
				headerTextAlignment = TextAlignment.Left,
				sortedAscending = true,
				sortingArrowAlignment = TextAlignment.Center,
				width = 200f,
				minWidth = 100f,
				autoResize = false,
				allowToggleVisibility = true,
				propertyName = null,
				dependencyIndices = null,
				compareDelegate = SerializedPropertyTreeView.DefaultDelegates.s_CompareName,
				drawDelegate = SerializedPropertyTreeView.DefaultDelegates.s_DrawName,
				filter = new SerializedPropertyFilters.Name()
			};
			int arg_1A6_1 = 2;
			column = new SerializedPropertyTreeView.Column();
			column.headerContent = LightTableColumns.Styles.GlobalIllumination;
			column.headerTextAlignment = TextAlignment.Left;
			column.sortedAscending = true;
			column.sortingArrowAlignment = TextAlignment.Center;
			column.width = 120f;
			column.minWidth = 70f;
			column.autoResize = false;
			column.allowToggleVisibility = true;
			column.propertyName = "m_LightmapFlags";
			column.dependencyIndices = null;
			column.compareDelegate = SerializedPropertyTreeView.DefaultDelegates.s_CompareInt;
			column.drawDelegate = delegate(Rect r, SerializedProperty prop, SerializedProperty[] dep)
			{
				if (prop.serializedObject.targetObject.GetType().Equals(typeof(Material)))
				{
					using (new EditorGUI.DisabledScope(!LightTableColumns.IsEditable(prop.serializedObject.targetObject)))
					{
						MaterialGlobalIlluminationFlags materialGlobalIlluminationFlags = ((prop.intValue & 2) == 0) ? MaterialGlobalIlluminationFlags.RealtimeEmissive : MaterialGlobalIlluminationFlags.BakedEmissive;
						int[] optionValues = new int[]
						{
							1,
							2
						};
						EditorGUI.BeginChangeCheck();
						materialGlobalIlluminationFlags = (MaterialGlobalIlluminationFlags)EditorGUI.IntPopup(r, (int)materialGlobalIlluminationFlags, LightTableColumns.Styles.LightmapEmissiveStrings, optionValues);
						if (EditorGUI.EndChangeCheck())
						{
							Material material = (Material)prop.serializedObject.targetObject;
							Undo.RecordObjects(new Material[]
							{
								material
							}, "Modify GI Settings of " + material.name);
							material.globalIlluminationFlags = materialGlobalIlluminationFlags;
							prop.serializedObject.Update();
						}
					}
				}
			};
			expr_07[arg_1A6_1] = column;
			int arg_26F_1 = 3;
			column = new SerializedPropertyTreeView.Column();
			column.headerContent = LightTableColumns.Styles.Intensity;
			column.headerTextAlignment = TextAlignment.Left;
			column.sortedAscending = true;
			column.sortingArrowAlignment = TextAlignment.Center;
			column.width = 70f;
			column.minWidth = 40f;
			column.autoResize = false;
			column.allowToggleVisibility = true;
			column.propertyName = "m_Shader";
			column.dependencyIndices = null;
			column.compareDelegate = delegate(SerializedProperty lhs, SerializedProperty rhs)
			{
				float num;
				float num2;
				float num3;
				Color.RGBToHSV(((Material)lhs.serializedObject.targetObject).GetColor("_EmissionColor"), out num, out num2, out num3);
				float num4;
				float num5;
				float value;
				Color.RGBToHSV(((Material)rhs.serializedObject.targetObject).GetColor("_EmissionColor"), out num4, out num5, out value);
				return num3.CompareTo(value);
			};
			column.drawDelegate = delegate(Rect r, SerializedProperty prop, SerializedProperty[] dep)
			{
				if (prop.serializedObject.targetObject.GetType().Equals(typeof(Material)))
				{
					using (new EditorGUI.DisabledScope(!LightTableColumns.IsEditable(prop.serializedObject.targetObject)))
					{
						Material material = (Material)prop.serializedObject.targetObject;
						Color color = material.GetColor("_EmissionColor");
						ColorPickerHDRConfig colorPickerHDRConfig = LightTableColumns.s_ColorPickerHDRConfig ?? ColorPicker.defaultHDRConfig;
						EditorGUI.BeginChangeCheck();
						Color value = EditorGUI.ColorBrightnessField(r, GUIContent.Temp(""), color, colorPickerHDRConfig.minBrightness, colorPickerHDRConfig.maxBrightness);
						if (EditorGUI.EndChangeCheck())
						{
							Undo.RecordObjects(new Material[]
							{
								material
							}, "Modify Color of " + material.name);
							material.SetColor("_EmissionColor", value);
						}
					}
				}
			};
			column.copyDelegate = delegate(SerializedProperty target, SerializedProperty source)
			{
				Material material = (Material)source.serializedObject.targetObject;
				Color color = material.GetColor("_EmissionColor");
				Material material2 = (Material)target.serializedObject.targetObject;
				material2.SetColor("_EmissionColor", color);
			};
			expr_07[arg_26F_1] = column;
			SerializedPropertyTreeView.Column[] columns = expr_07;
			return LightTableColumns.FinalizeColumns(columns, out propNames);
		}
	}
}
