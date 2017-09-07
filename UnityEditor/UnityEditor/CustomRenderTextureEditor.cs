using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor.AnimatedValues;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

namespace UnityEditor
{
	[CanEditMultipleObjects, CustomEditor(typeof(CustomRenderTexture))]
	internal class CustomRenderTextureEditor : RenderTextureEditor
	{
		private class Styles
		{
			public readonly GUIStyle separator = "sv_iconselector_sep";

			public readonly GUIContent materials = EditorGUIUtility.TextContent("Materials");

			public readonly GUIContent shaderPass = EditorGUIUtility.TextContent("Shader Pass|Shader Pass used to update the Custom Texture.");

			public readonly GUIContent needSwap = EditorGUIUtility.TextContent("Swap (Double Buffer)|If ticked, and if the texture is double buffered, a request is made to swap the buffers before the next update. If this is not ticked, the buffers will not be swapped.");

			public readonly GUIContent updateMode = EditorGUIUtility.TextContent("Update Mode|Specify how the texture should be updated.");

			public readonly GUIContent updatePeriod = EditorGUIUtility.TextContent("Period|Period in seconds at which real-time textures are updated (0.0 will update every frame).");

			public readonly GUIContent doubleBuffered = EditorGUIUtility.TextContent("Double Buffered|If ticked, the custom Texture is double buffered so that you can access it during its own update. If unticked, the custom Texture will be not be double buffered.");

			public readonly GUIContent initializationMode = EditorGUIUtility.TextContent("Initialization Mode|Specify how the texture should be initialized.");

			public readonly GUIContent initSource = EditorGUIUtility.TextContent("Source|Specify if the texture is initialized by a Material or by a Texture and a Color.");

			public readonly GUIContent initColor = EditorGUIUtility.TextContent("Color|Color with which the custom texture is initialized.");

			public readonly GUIContent initTexture = EditorGUIUtility.TextContent("Texture|Texture with which the custom texture is initialized (multiplied by the initialization color).");

			public readonly GUIContent initMaterial = EditorGUIUtility.TextContent("Material|Material with which the custom texture is initialized.");

			public readonly GUIContent updateZoneSpace = EditorGUIUtility.TextContent("Update Zone Space|Space in which the update zones are expressed (Normalized or Pixel space).");

			public readonly GUIContent updateZoneList = EditorGUIUtility.TextContent("Update Zones|List of partial update zones.");

			public readonly GUIContent cubemapFacesLabel = EditorGUIUtility.TextContent("Cubemap Faces|Enable or disable rendering on each face of the cubemap.");

			public readonly GUIContent updateZoneCenter = EditorGUIUtility.TextContent("Center|Center of the partial update zone.");

			public readonly GUIContent updateZoneSize = EditorGUIUtility.TextContent("Size|Size of the partial update zone.");

			public readonly GUIContent updateZoneRotation = EditorGUIUtility.TextContent("Rotation|Rotation of the update zone.");

			public readonly GUIContent wrapUpdateZones = EditorGUIUtility.TextContent("Wrap Update Zones|If ticked, Update zones will wrap around the border of the Custom Texture. If unticked, Update zones will be clamped at the border of the Custom Texture.");

			public readonly GUIContent saveButton = EditorGUIUtility.TextContent("Save Texture|Save the content of the custom texture to an EXR or PNG file.");

			public readonly GUIContent[] updateModeStrings = new GUIContent[]
			{
				EditorGUIUtility.TextContent("OnLoad"),
				EditorGUIUtility.TextContent("Realtime"),
				EditorGUIUtility.TextContent("OnDemand")
			};

			public readonly int[] updateModeValues = new int[]
			{
				0,
				1,
				2
			};

			public readonly GUIContent[] initSourceStrings = new GUIContent[]
			{
				EditorGUIUtility.TextContent("Texture and Color"),
				EditorGUIUtility.TextContent("Material")
			};

			public readonly int[] initSourceValues = new int[]
			{
				0,
				1
			};

			public readonly GUIContent[] updateZoneSpaceStrings = new GUIContent[]
			{
				EditorGUIUtility.TextContent("Normalized"),
				EditorGUIUtility.TextContent("Pixel")
			};

			public readonly int[] updateZoneSpaceValues = new int[]
			{
				0,
				1
			};

			public readonly GUIContent[] cubemapFaces = new GUIContent[]
			{
				EditorGUIUtility.TextContent("+X"),
				EditorGUIUtility.TextContent("-X"),
				EditorGUIUtility.TextContent("+Y"),
				EditorGUIUtility.TextContent("-Y"),
				EditorGUIUtility.TextContent("+Z"),
				EditorGUIUtility.TextContent("-Z")
			};
		}

		private static CustomRenderTextureEditor.Styles s_Styles = null;

		private SerializedProperty m_Material;

		private SerializedProperty m_ShaderPass;

		private SerializedProperty m_InitializationMode;

		private SerializedProperty m_InitSource;

		private SerializedProperty m_InitColor;

		private SerializedProperty m_InitTexture;

		private SerializedProperty m_InitMaterial;

		private SerializedProperty m_UpdateMode;

		private SerializedProperty m_UpdatePeriod;

		private SerializedProperty m_UpdateZoneSpace;

		private SerializedProperty m_UpdateZones;

		private SerializedProperty m_WrapUpdateZones;

		private SerializedProperty m_DoubleBuffered;

		private SerializedProperty m_CubeFaceMask;

		private ReorderableList m_RectList;

		private const float kCubefaceToggleWidth = 70f;

		private const float kRListAddButtonOffset = 16f;

		private const float kIndentSize = 15f;

		private const float kToggleWidth = 100f;

		private readonly AnimBool m_ShowInitSourceAsMaterial = new AnimBool();

		private static CustomRenderTextureEditor.Styles styles
		{
			get
			{
				if (CustomRenderTextureEditor.s_Styles == null)
				{
					CustomRenderTextureEditor.s_Styles = new CustomRenderTextureEditor.Styles();
				}
				return CustomRenderTextureEditor.s_Styles;
			}
		}

		private bool multipleEditing
		{
			get
			{
				return base.targets.Length > 1;
			}
		}

		private void UpdateZoneVec3PropertyField(Rect rect, SerializedProperty prop, GUIContent label, bool as2D)
		{
			EditorGUI.BeginProperty(rect, label, prop);
			if (!as2D)
			{
				prop.vector3Value = EditorGUI.Vector3Field(rect, label, prop.vector3Value);
			}
			else
			{
				Vector2 vector = EditorGUI.Vector2Field(rect, label, new Vector2(prop.vector3Value.x, prop.vector3Value.y));
				prop.vector3Value = new Vector3(vector.x, vector.y, prop.vector3Value.z);
			}
			EditorGUI.EndProperty();
		}

		private void OnDrawElement(Rect rect, int index, bool isActive, bool isFocused)
		{
			CustomRenderTexture customRenderTexture = base.target as CustomRenderTexture;
			bool flag = customRenderTexture.dimension == TextureDimension.Tex3D;
			bool doubleBuffered = customRenderTexture.doubleBuffered;
			SerializedProperty arrayElementAtIndex = this.m_RectList.serializedProperty.GetArrayElementAtIndex(index);
			float singleLineHeight = EditorGUIUtility.singleLineHeight;
			rect.y += EditorGUIUtility.standardVerticalSpacing;
			rect.height = singleLineHeight;
			EditorGUI.LabelField(rect, string.Format("Update Zone {0}", index));
			rect.y += singleLineHeight;
			SerializedProperty prop = arrayElementAtIndex.FindPropertyRelative("updateZoneCenter");
			this.UpdateZoneVec3PropertyField(rect, prop, CustomRenderTextureEditor.styles.updateZoneCenter, !flag);
			rect.y += singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
			SerializedProperty prop2 = arrayElementAtIndex.FindPropertyRelative("updateZoneSize");
			this.UpdateZoneVec3PropertyField(rect, prop2, CustomRenderTextureEditor.styles.updateZoneSize, !flag);
			if (!flag)
			{
				rect.y += EditorGUIUtility.standardVerticalSpacing + singleLineHeight;
				EditorGUI.PropertyField(rect, arrayElementAtIndex.FindPropertyRelative("rotation"), CustomRenderTextureEditor.styles.updateZoneRotation);
			}
			List<GUIContent> list = new List<GUIContent>();
			List<int> list2 = new List<int>();
			Material material = this.m_Material.objectReferenceValue as Material;
			if (material != null)
			{
				this.BuildShaderPassPopup(material, list, list2, true);
			}
			using (new EditorGUI.DisabledScope(list.Count == 0))
			{
				SerializedProperty property = arrayElementAtIndex.FindPropertyRelative("passIndex");
				rect.y += EditorGUIUtility.standardVerticalSpacing + singleLineHeight;
				EditorGUI.IntPopup(rect, property, list.ToArray(), list2.ToArray(), CustomRenderTextureEditor.styles.shaderPass);
			}
			if (doubleBuffered)
			{
				rect.y += EditorGUIUtility.standardVerticalSpacing + singleLineHeight;
				EditorGUI.PropertyField(rect, arrayElementAtIndex.FindPropertyRelative("needSwap"), CustomRenderTextureEditor.styles.updateZoneRotation);
			}
		}

		private void OnDrawHeader(Rect rect)
		{
			GUI.Label(rect, CustomRenderTextureEditor.styles.updateZoneList);
		}

		private void OnAdd(ReorderableList l)
		{
			CustomRenderTexture customRenderTexture = base.target as CustomRenderTexture;
			int arraySize = l.serializedProperty.arraySize;
			l.serializedProperty.arraySize++;
			l.index = arraySize;
			SerializedProperty arrayElementAtIndex = l.serializedProperty.GetArrayElementAtIndex(arraySize);
			Vector3 vector3Value = new Vector3(0.5f, 0.5f, 0.5f);
			Vector3 vector3Value2 = new Vector3(1f, 1f, 1f);
			if (customRenderTexture.updateZoneSpace == CustomRenderTextureUpdateZoneSpace.Pixel)
			{
				Vector3 scale = new Vector3((float)customRenderTexture.width, (float)customRenderTexture.height, (float)customRenderTexture.volumeDepth);
				vector3Value.Scale(scale);
				vector3Value2.Scale(scale);
			}
			arrayElementAtIndex.FindPropertyRelative("updateZoneCenter").vector3Value = vector3Value;
			arrayElementAtIndex.FindPropertyRelative("updateZoneSize").vector3Value = vector3Value2;
			arrayElementAtIndex.FindPropertyRelative("rotation").floatValue = 0f;
			arrayElementAtIndex.FindPropertyRelative("passIndex").intValue = -1;
			arrayElementAtIndex.FindPropertyRelative("needSwap").boolValue = false;
		}

		private void OnRemove(ReorderableList l)
		{
			l.serializedProperty.arraySize--;
			if (l.index == l.serializedProperty.arraySize)
			{
				l.index--;
			}
		}

		private float OnElementHeight(int index)
		{
			CustomRenderTexture customRenderTexture = base.target as CustomRenderTexture;
			bool flag = customRenderTexture.dimension == TextureDimension.Tex3D;
			bool doubleBuffered = customRenderTexture.doubleBuffered;
			int num = 4;
			if (!flag)
			{
				num++;
			}
			if (doubleBuffered)
			{
				num++;
			}
			return (EditorGUIUtility.singleLineHeight + 2f) * (float)num;
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			this.m_Material = base.serializedObject.FindProperty("m_Material");
			this.m_ShaderPass = base.serializedObject.FindProperty("m_ShaderPass");
			this.m_InitializationMode = base.serializedObject.FindProperty("m_InitializationMode");
			this.m_InitSource = base.serializedObject.FindProperty("m_InitSource");
			this.m_InitColor = base.serializedObject.FindProperty("m_InitColor");
			this.m_InitTexture = base.serializedObject.FindProperty("m_InitTexture");
			this.m_InitMaterial = base.serializedObject.FindProperty("m_InitMaterial");
			this.m_UpdateMode = base.serializedObject.FindProperty("m_UpdateMode");
			this.m_UpdatePeriod = base.serializedObject.FindProperty("m_UpdatePeriod");
			this.m_UpdateZoneSpace = base.serializedObject.FindProperty("m_UpdateZoneSpace");
			this.m_UpdateZones = base.serializedObject.FindProperty("m_UpdateZones");
			this.m_WrapUpdateZones = base.serializedObject.FindProperty("m_WrapUpdateZones");
			this.m_DoubleBuffered = base.serializedObject.FindProperty("m_DoubleBuffered");
			this.m_CubeFaceMask = base.serializedObject.FindProperty("m_CubemapFaceMask");
			this.m_RectList = new ReorderableList(base.serializedObject, this.m_UpdateZones);
			this.m_RectList.drawElementCallback = new ReorderableList.ElementCallbackDelegate(this.OnDrawElement);
			this.m_RectList.drawHeaderCallback = new ReorderableList.HeaderCallbackDelegate(this.OnDrawHeader);
			this.m_RectList.onAddCallback = new ReorderableList.AddCallbackDelegate(this.OnAdd);
			this.m_RectList.onRemoveCallback = new ReorderableList.RemoveCallbackDelegate(this.OnRemove);
			this.m_RectList.elementHeightCallback = new ReorderableList.ElementHeightCallbackDelegate(this.OnElementHeight);
			this.m_RectList.footerHeight = 0f;
			this.m_ShowInitSourceAsMaterial.value = (!this.m_InitSource.hasMultipleDifferentValues && this.m_InitSource.intValue == 1);
			this.m_ShowInitSourceAsMaterial.valueChanged.AddListener(new UnityAction(base.Repaint));
		}

		protected override void OnDisable()
		{
			base.OnDisable();
			this.m_ShowInitSourceAsMaterial.valueChanged.RemoveListener(new UnityAction(base.Repaint));
		}

		private void DisplayRenderTextureGUI()
		{
			base.OnRenderTextureGUI(RenderTextureEditor.GUIElements.RenderTargetNoneGUI);
			GUILayout.Space(10f);
		}

		private void BuildShaderPassPopup(Material material, List<GUIContent> names, List<int> values, bool addDefaultPass)
		{
			names.Clear();
			values.Clear();
			int passCount = material.passCount;
			for (int i = 0; i < passCount; i++)
			{
				string text = material.GetPassName(i);
				if (text.Length == 0)
				{
					text = string.Format("Unnamed Pass {0}", i);
				}
				names.Add(EditorGUIUtility.TextContent(text));
				values.Add(i);
			}
			if (addDefaultPass)
			{
				CustomRenderTexture customRenderTexture = base.target as CustomRenderTexture;
				GUIContent item = EditorGUIUtility.TextContent(string.Format("Default ({0})", names[customRenderTexture.shaderPass].text));
				names.Insert(0, item);
				values.Insert(0, -1);
			}
		}

		private void DisplayMaterialGUI()
		{
			EditorGUILayout.PropertyField(this.m_Material, true, new GUILayoutOption[0]);
			EditorGUI.indentLevel++;
			List<GUIContent> list = new List<GUIContent>();
			List<int> list2 = new List<int>();
			Material material = this.m_Material.objectReferenceValue as Material;
			if (material != null)
			{
				this.BuildShaderPassPopup(material, list, list2, false);
			}
			using (new EditorGUI.DisabledScope(list.Count == 0 || this.m_Material.hasMultipleDifferentValues))
			{
				if (material != null)
				{
					EditorGUILayout.IntPopup(this.m_ShaderPass, list.ToArray(), list2.ToArray(), CustomRenderTextureEditor.styles.shaderPass, new GUILayoutOption[0]);
				}
			}
			EditorGUI.indentLevel--;
		}

		private void DisplayInitializationGUI()
		{
			this.m_ShowInitSourceAsMaterial.target = (!this.m_InitSource.hasMultipleDifferentValues && this.m_InitSource.intValue == 1);
			EditorGUILayout.IntPopup(this.m_InitializationMode, CustomRenderTextureEditor.styles.updateModeStrings, CustomRenderTextureEditor.styles.updateModeValues, CustomRenderTextureEditor.styles.initializationMode, new GUILayoutOption[0]);
			EditorGUI.indentLevel++;
			EditorGUILayout.IntPopup(this.m_InitSource, CustomRenderTextureEditor.styles.initSourceStrings, CustomRenderTextureEditor.styles.initSourceValues, CustomRenderTextureEditor.styles.initSource, new GUILayoutOption[0]);
			if (!this.m_InitSource.hasMultipleDifferentValues)
			{
				EditorGUI.indentLevel++;
				if (EditorGUILayout.BeginFadeGroup(this.m_ShowInitSourceAsMaterial.faded))
				{
					EditorGUILayout.PropertyField(this.m_InitMaterial, CustomRenderTextureEditor.styles.initMaterial, new GUILayoutOption[0]);
				}
				EditorGUILayout.EndFadeGroup();
				if (EditorGUILayout.BeginFadeGroup(1f - this.m_ShowInitSourceAsMaterial.faded))
				{
					EditorGUILayout.PropertyField(this.m_InitColor, CustomRenderTextureEditor.styles.initColor, new GUILayoutOption[0]);
					EditorGUILayout.PropertyField(this.m_InitTexture, CustomRenderTextureEditor.styles.initTexture, new GUILayoutOption[0]);
				}
				EditorGUILayout.EndFadeGroup();
				EditorGUI.indentLevel--;
			}
			EditorGUI.indentLevel--;
		}

		private void DisplayUpdateGUI()
		{
			EditorGUILayout.IntPopup(this.m_UpdateMode, CustomRenderTextureEditor.styles.updateModeStrings, CustomRenderTextureEditor.styles.updateModeValues, CustomRenderTextureEditor.styles.updateMode, new GUILayoutOption[0]);
			EditorGUI.indentLevel++;
			if (this.m_UpdateMode.intValue == 1)
			{
				EditorGUILayout.PropertyField(this.m_UpdatePeriod, CustomRenderTextureEditor.styles.updatePeriod, new GUILayoutOption[0]);
			}
			EditorGUILayout.PropertyField(this.m_DoubleBuffered, CustomRenderTextureEditor.styles.doubleBuffered, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_WrapUpdateZones, CustomRenderTextureEditor.styles.wrapUpdateZones, new GUILayoutOption[0]);
			bool flag = true;
			UnityEngine.Object[] targets = base.targets;
			for (int i = 0; i < targets.Length; i++)
			{
				UnityEngine.Object @object = targets[i];
				CustomRenderTexture customRenderTexture = @object as CustomRenderTexture;
				if (customRenderTexture != null && customRenderTexture.dimension != TextureDimension.Cube)
				{
					flag = false;
				}
			}
			if (flag)
			{
				int num = 0;
				int intValue = this.m_CubeFaceMask.intValue;
				Rect rect = GUILayoutUtility.GetRect(0f, EditorGUIUtility.singleLineHeight * 3f + EditorGUIUtility.standardVerticalSpacing * 2f);
				EditorGUI.BeginProperty(rect, GUIContent.none, this.m_CubeFaceMask);
				Rect position = rect;
				position.width = 100f;
				position.height = EditorGUIUtility.singleLineHeight;
				int num2 = 0;
				Rect position2 = rect;
				EditorGUI.LabelField(position2, CustomRenderTextureEditor.styles.cubemapFacesLabel);
				EditorGUI.BeginChangeCheck();
				for (int j = 0; j < 3; j++)
				{
					position.x = rect.x + EditorGUIUtility.labelWidth - 15f;
					for (int k = 0; k < 2; k++)
					{
						bool flag2 = EditorGUI.ToggleLeft(position, CustomRenderTextureEditor.styles.cubemapFaces[num2], (intValue & 1 << num2) != 0);
						if (flag2)
						{
							num |= 1 << num2;
						}
						num2++;
						position.x += 100f;
					}
					position.y += EditorGUIUtility.singleLineHeight;
				}
				if (EditorGUI.EndChangeCheck())
				{
					this.m_CubeFaceMask.intValue = num;
				}
				EditorGUI.EndProperty();
			}
			EditorGUILayout.IntPopup(this.m_UpdateZoneSpace, CustomRenderTextureEditor.styles.updateZoneSpaceStrings, CustomRenderTextureEditor.styles.updateZoneSpaceValues, CustomRenderTextureEditor.styles.updateZoneSpace, new GUILayoutOption[0]);
			if (!this.multipleEditing)
			{
				Rect rect2 = GUILayoutUtility.GetRect(0f, this.m_RectList.GetHeight() + 16f, new GUILayoutOption[]
				{
					GUILayout.ExpandWidth(true)
				});
				float num3 = 15f;
				rect2.x += num3;
				rect2.width -= num3;
				this.m_RectList.DoList(rect2);
			}
			else
			{
				EditorGUILayout.HelpBox("Update Zones cannot be changed while editing multiple Custom Textures.", MessageType.Info);
			}
			EditorGUI.indentLevel--;
		}

		private void DisplayCustomRenderTextureGUI()
		{
			CustomRenderTexture customRenderTexture = base.target as CustomRenderTexture;
			this.DisplayMaterialGUI();
			EditorGUILayout.Space();
			this.DisplayInitializationGUI();
			EditorGUILayout.Space();
			this.DisplayUpdateGUI();
			EditorGUILayout.Space();
			if (customRenderTexture.updateMode != CustomRenderTextureUpdateMode.Realtime && customRenderTexture.initializationMode == CustomRenderTextureUpdateMode.Realtime)
			{
				EditorGUILayout.HelpBox("Initialization Mode is set to Realtime but Update Mode is not. This will result in update never being visible.", MessageType.Warning);
			}
		}

		public override void OnInspectorGUI()
		{
			base.serializedObject.Update();
			this.DisplayRenderTextureGUI();
			this.DisplayCustomRenderTextureGUI();
			base.serializedObject.ApplyModifiedProperties();
		}

		[MenuItem("CONTEXT/CustomRenderTexture/Export", false)]
		private static void SaveToDisk(MenuCommand command)
		{
			CustomRenderTexture customRenderTexture = command.context as CustomRenderTexture;
			int width = customRenderTexture.width;
			int height = customRenderTexture.height;
			int volumeDepth = customRenderTexture.volumeDepth;
			bool flag = RenderTextureEditor.IsHDRFormat(customRenderTexture.format);
			bool flag2 = customRenderTexture.format == RenderTextureFormat.ARGBFloat || customRenderTexture.format == RenderTextureFormat.RFloat;
			TextureFormat format = (!flag) ? TextureFormat.RGBA32 : TextureFormat.RGBAFloat;
			int width2 = width;
			if (customRenderTexture.dimension == TextureDimension.Tex3D)
			{
				width2 = width * volumeDepth;
			}
			else if (customRenderTexture.dimension == TextureDimension.Cube)
			{
				width2 = width * 6;
			}
			Texture2D texture2D = new Texture2D(width2, height, format, false);
			if (customRenderTexture.dimension == TextureDimension.Tex2D)
			{
				Graphics.SetRenderTarget(customRenderTexture);
				texture2D.ReadPixels(new Rect(0f, 0f, (float)width, (float)height), 0, 0);
				texture2D.Apply();
			}
			else if (customRenderTexture.dimension == TextureDimension.Tex3D)
			{
				int num = 0;
				for (int i = 0; i < volumeDepth; i++)
				{
					Graphics.SetRenderTarget(customRenderTexture, 0, CubemapFace.Unknown, i);
					texture2D.ReadPixels(new Rect(0f, 0f, (float)width, (float)height), num, 0);
					texture2D.Apply();
					num += width;
				}
			}
			else
			{
				int num2 = 0;
				for (int j = 0; j < 6; j++)
				{
					Graphics.SetRenderTarget(customRenderTexture, 0, (CubemapFace)j);
					texture2D.ReadPixels(new Rect(0f, 0f, (float)width, (float)height), num2, 0);
					texture2D.Apply();
					num2 += width;
				}
			}
			byte[] bytes;
			if (flag)
			{
				bytes = texture2D.EncodeToEXR(Texture2D.EXRFlags.CompressZIP | ((!flag2) ? Texture2D.EXRFlags.None : Texture2D.EXRFlags.OutputAsFloat));
			}
			else
			{
				bytes = texture2D.EncodeToPNG();
			}
			UnityEngine.Object.DestroyImmediate(texture2D);
			string extension = (!flag) ? "png" : "exr";
			string directoryName = Path.GetDirectoryName(AssetDatabase.GetAssetPath(customRenderTexture.GetInstanceID()));
			string text = EditorUtility.SaveFilePanel("Save Custom Texture", directoryName, customRenderTexture.name, extension);
			if (!string.IsNullOrEmpty(text))
			{
				File.WriteAllBytes(text, bytes);
				AssetDatabase.Refresh();
			}
		}

		public override string GetInfoString()
		{
			return base.GetInfoString();
		}
	}
}
