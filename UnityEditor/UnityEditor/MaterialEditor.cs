using System;
using UnityEditor.Utils;
using UnityEditorInternal;
using UnityEngine;
namespace UnityEditor
{
	[CanEditMultipleObjects, CustomEditor(typeof(Material))]
	public class MaterialEditor : Editor
	{
		private enum PreviewType
		{
			Mesh,
			Plane
		}
		private class ForwardApplyMaterialModification
		{
			private Renderer renderer;
			public ForwardApplyMaterialModification(Renderer r)
			{
				this.renderer = r;
			}
			public bool DidModifyAnimationModeMaterialProperty(MaterialProperty property, int changedMask, object previousValue)
			{
				return MaterialAnimationUtility.ApplyMaterialModificationToAnimationRecording(property, changedMask, this.renderer, previousValue);
			}
		}
		private const int kSpacingUnderTexture = 6;
		private bool m_IsVisible;
		private SerializedProperty m_Shader;
		private string m_InfoMessage;
		private Vector2 m_PreviewDir = new Vector2(0f, -20f);
		private int m_SelectedMesh;
		private int m_TimeUpdate;
		private int m_LightMode = 1;
		private MaterialProperty.TexDim m_DesiredTexdim;
		private PreviewRenderUtility m_PreviewUtility;
		private static Mesh[] s_Meshes = new Mesh[4];
		private static Mesh s_PlaneMesh = null;
		private static GUIContent[] s_MeshIcons = new GUIContent[4];
		private static GUIContent[] s_LightIcons = new GUIContent[2];
		private static GUIContent[] s_TimeIcons = new GUIContent[2];
		public bool isVisible
		{
			get
			{
				return this.m_IsVisible;
			}
		}
		private MaterialEditor.PreviewType GetPreviewType(Material mat)
		{
			if (mat == null)
			{
				return MaterialEditor.PreviewType.Mesh;
			}
			string a = mat.GetTag("PreviewType", false, string.Empty).ToLower();
			if (a == "plane")
			{
				return MaterialEditor.PreviewType.Plane;
			}
			return MaterialEditor.PreviewType.Mesh;
		}
		private bool DoesPreviewAllowRotation(MaterialEditor.PreviewType type)
		{
			return type != MaterialEditor.PreviewType.Plane;
		}
		public void SetShader(Shader shader)
		{
			this.SetShader(shader, true);
		}
		public void SetShader(Shader shader, bool registerUndo)
		{
			bool flag = false;
			Shader shader2 = this.m_Shader.objectReferenceValue as Shader;
			if (shader2 != null && shader2.customEditor != shader.customEditor)
			{
				flag = true;
			}
			this.m_Shader.objectReferenceValue = shader;
			base.serializedObject.ApplyModifiedProperties();
			UnityEngine.Object[] targets = base.targets;
			for (int i = 0; i < targets.Length; i++)
			{
				Material material = (Material)targets[i];
				if (material.shader.customEditor != shader.customEditor)
				{
					flag = true;
				}
				Undo.RecordObject(material, "Assign shader");
				material.shader = shader;
				EditorMaterialUtility.ResetDefaultTextures(material, false);
				MaterialEditor.ApplyMaterialPropertyDrawers(material);
			}
			if (flag && ActiveEditorTracker.sharedTracker != null)
			{
				ActiveEditorTracker.sharedTracker.ForceRebuild();
			}
		}
		private void OnSelectedShaderPopup(string command, Shader shader)
		{
			base.serializedObject.Update();
			if (shader != null)
			{
				this.SetShader(shader);
			}
			this.PropertiesChanged();
		}
		private void ShaderPopup(GUIStyle style)
		{
			bool enabled = GUI.enabled;
			Shader shader = this.m_Shader.objectReferenceValue as Shader;
			Rect rect = EditorGUILayout.GetControlRect(new GUILayoutOption[0]);
			rect = EditorGUI.PrefixLabel(rect, 47385, EditorGUIUtility.TempContent("Shader"));
			EditorGUI.showMixedValue = this.m_Shader.hasMultipleDifferentValues;
			GUIContent content = EditorGUIUtility.TempContent((!(shader != null)) ? "No Shader Selected" : shader.name);
			if (EditorGUI.ButtonMouseDown(rect, content, EditorGUIUtility.native, style))
			{
				EditorGUI.showMixedValue = false;
				Vector2 vector = GUIUtility.GUIToScreenPoint(new Vector2(rect.x, rect.y));
				InternalEditorUtility.SetupShaderMenu(this.target as Material);
				EditorUtility.Internal_DisplayPopupMenu(new Rect(vector.x, vector.y, rect.width, rect.height), "CONTEXT/ShaderPopup", this, 0);
				Event.current.Use();
			}
			EditorGUI.showMixedValue = false;
			GUI.enabled = enabled;
		}
		public virtual void Awake()
		{
			this.m_IsVisible = InternalEditorUtility.GetIsInspectorExpanded(this.target);
		}
		public override void OnInspectorGUI()
		{
			base.serializedObject.Update();
			if (this.m_IsVisible && !this.m_Shader.hasMultipleDifferentValues && this.m_Shader.objectReferenceValue != null && this.PropertiesGUI())
			{
				this.PropertiesChanged();
			}
		}
		internal override void OnAssetStoreInspectorGUI()
		{
			this.OnInspectorGUI();
		}
		public void PropertiesChanged()
		{
			this.m_InfoMessage = null;
			if (base.targets.Length == 1)
			{
				this.m_InfoMessage = PerformanceChecks.CheckMaterial(this.target as Material, EditorUserBuildSettings.activeBuildTarget);
			}
		}
		protected override void OnHeaderGUI()
		{
			Rect position = Editor.DrawHeaderGUI(this, this.targetTitle);
			int controlID = GUIUtility.GetControlID(45678, FocusType.Passive);
			bool flag = EditorGUI.DoObjectFoldout(controlID, position, base.targets, this.m_IsVisible);
			if (flag != this.m_IsVisible)
			{
				this.m_IsVisible = flag;
				InternalEditorUtility.SetIsInspectorExpanded(this.target, flag);
			}
		}
		internal override void OnHeaderControlsGUI()
		{
			base.serializedObject.Update();
			EditorGUI.BeginDisabledGroup(!this.IsEnabled());
			EditorGUIUtility.labelWidth = 50f;
			this.ShaderPopup("MiniPulldown");
			if (!this.m_Shader.hasMultipleDifferentValues && this.m_Shader.objectReferenceValue != null && (this.m_Shader.objectReferenceValue.hideFlags & HideFlags.DontSave) == HideFlags.None && GUILayout.Button("Edit...", EditorStyles.miniButton, new GUILayoutOption[]
			{
				GUILayout.ExpandWidth(false)
			}))
			{
				AssetDatabase.OpenAsset(this.m_Shader.objectReferenceValue);
			}
			EditorGUI.EndDisabledGroup();
		}
		[Obsolete("Use GetMaterialProperty instead.")]
		public float GetFloat(string propertyName, out bool hasMixedValue)
		{
			hasMixedValue = false;
			float @float = (base.targets[0] as Material).GetFloat(propertyName);
			for (int i = 1; i < base.targets.Length; i++)
			{
				if ((base.targets[i] as Material).GetFloat(propertyName) != @float)
				{
					hasMixedValue = true;
					break;
				}
			}
			return @float;
		}
		[Obsolete("Use MaterialProperty instead.")]
		public void SetFloat(string propertyName, float value)
		{
			UnityEngine.Object[] targets = base.targets;
			for (int i = 0; i < targets.Length; i++)
			{
				Material material = (Material)targets[i];
				material.SetFloat(propertyName, value);
			}
		}
		[Obsolete("Use GetMaterialProperty instead.")]
		public Color GetColor(string propertyName, out bool hasMixedValue)
		{
			hasMixedValue = false;
			Color color = (base.targets[0] as Material).GetColor(propertyName);
			for (int i = 1; i < base.targets.Length; i++)
			{
				if ((base.targets[i] as Material).GetColor(propertyName) != color)
				{
					hasMixedValue = true;
					break;
				}
			}
			return color;
		}
		[Obsolete("Use MaterialProperty instead.")]
		public void SetColor(string propertyName, Color value)
		{
			UnityEngine.Object[] targets = base.targets;
			for (int i = 0; i < targets.Length; i++)
			{
				Material material = (Material)targets[i];
				material.SetColor(propertyName, value);
			}
		}
		[Obsolete("Use GetMaterialProperty instead.")]
		public Vector4 GetVector(string propertyName, out bool hasMixedValue)
		{
			hasMixedValue = false;
			Vector4 vector = (base.targets[0] as Material).GetVector(propertyName);
			for (int i = 1; i < base.targets.Length; i++)
			{
				if ((base.targets[i] as Material).GetVector(propertyName) != vector)
				{
					hasMixedValue = true;
					break;
				}
			}
			return vector;
		}
		[Obsolete("Use MaterialProperty instead.")]
		public void SetVector(string propertyName, Vector4 value)
		{
			UnityEngine.Object[] targets = base.targets;
			for (int i = 0; i < targets.Length; i++)
			{
				Material material = (Material)targets[i];
				material.SetVector(propertyName, value);
			}
		}
		[Obsolete("Use GetMaterialProperty instead.")]
		public Texture GetTexture(string propertyName, out bool hasMixedValue)
		{
			hasMixedValue = false;
			Texture texture = (base.targets[0] as Material).GetTexture(propertyName);
			for (int i = 1; i < base.targets.Length; i++)
			{
				if ((base.targets[i] as Material).GetTexture(propertyName) != texture)
				{
					hasMixedValue = true;
					break;
				}
			}
			return texture;
		}
		[Obsolete("Use MaterialProperty instead.")]
		public void SetTexture(string propertyName, Texture value)
		{
			UnityEngine.Object[] targets = base.targets;
			for (int i = 0; i < targets.Length; i++)
			{
				Material material = (Material)targets[i];
				material.SetTexture(propertyName, value);
			}
		}
		[Obsolete("Use MaterialProperty instead.")]
		public Vector2 GetTextureScale(string propertyName, out bool hasMixedValueX, out bool hasMixedValueY)
		{
			hasMixedValueX = false;
			hasMixedValueY = false;
			Vector2 textureScale = (base.targets[0] as Material).GetTextureScale(propertyName);
			for (int i = 1; i < base.targets.Length; i++)
			{
				Vector2 textureScale2 = (base.targets[i] as Material).GetTextureScale(propertyName);
				if (textureScale2.x != textureScale.x)
				{
					hasMixedValueX = true;
				}
				if (textureScale2.y != textureScale.y)
				{
					hasMixedValueY = true;
				}
				if (hasMixedValueX && hasMixedValueY)
				{
					break;
				}
			}
			return textureScale;
		}
		[Obsolete("Use MaterialProperty instead.")]
		public Vector2 GetTextureOffset(string propertyName, out bool hasMixedValueX, out bool hasMixedValueY)
		{
			hasMixedValueX = false;
			hasMixedValueY = false;
			Vector2 textureOffset = (base.targets[0] as Material).GetTextureOffset(propertyName);
			for (int i = 1; i < base.targets.Length; i++)
			{
				Vector2 textureOffset2 = (base.targets[i] as Material).GetTextureOffset(propertyName);
				if (textureOffset2.x != textureOffset.x)
				{
					hasMixedValueX = true;
				}
				if (textureOffset2.y != textureOffset.y)
				{
					hasMixedValueY = true;
				}
				if (hasMixedValueX && hasMixedValueY)
				{
					break;
				}
			}
			return textureOffset;
		}
		[Obsolete("Use MaterialProperty instead.")]
		public void SetTextureScale(string propertyName, Vector2 value, int coord)
		{
			UnityEngine.Object[] targets = base.targets;
			for (int i = 0; i < targets.Length; i++)
			{
				Material material = (Material)targets[i];
				Vector2 textureScale = material.GetTextureScale(propertyName);
				textureScale[coord] = value[coord];
				material.SetTextureScale(propertyName, textureScale);
			}
		}
		[Obsolete("Use MaterialProperty instead.")]
		public void SetTextureOffset(string propertyName, Vector2 value, int coord)
		{
			UnityEngine.Object[] targets = base.targets;
			for (int i = 0; i < targets.Length; i++)
			{
				Material material = (Material)targets[i];
				Vector2 textureOffset = material.GetTextureOffset(propertyName);
				textureOffset[coord] = value[coord];
				material.SetTextureOffset(propertyName, textureOffset);
			}
		}
		public float RangeProperty(MaterialProperty prop, string label)
		{
			Rect propertyRect = this.GetPropertyRect(prop, label, true);
			return this.RangeProperty(propertyRect, prop, label);
		}
		public float RangeProperty(Rect position, MaterialProperty prop, string label)
		{
			EditorGUI.BeginChangeCheck();
			EditorGUI.showMixedValue = prop.hasMixedValue;
			float power = (!(prop.name == "_Shininess")) ? 1f : 5f;
			float floatValue = EditorGUI.PowerSlider(position, label, prop.floatValue, prop.rangeLimits.x, prop.rangeLimits.y, power);
			EditorGUI.showMixedValue = false;
			if (EditorGUI.EndChangeCheck())
			{
				prop.floatValue = floatValue;
			}
			return prop.floatValue;
		}
		public float FloatProperty(MaterialProperty prop, string label)
		{
			Rect propertyRect = this.GetPropertyRect(prop, label, true);
			return this.FloatProperty(propertyRect, prop, label);
		}
		public float FloatProperty(Rect position, MaterialProperty prop, string label)
		{
			EditorGUI.BeginChangeCheck();
			EditorGUI.showMixedValue = prop.hasMixedValue;
			float floatValue = EditorGUI.FloatField(position, label, prop.floatValue);
			EditorGUI.showMixedValue = false;
			if (EditorGUI.EndChangeCheck())
			{
				prop.floatValue = floatValue;
			}
			return prop.floatValue;
		}
		public Color ColorProperty(MaterialProperty prop, string label)
		{
			Rect propertyRect = this.GetPropertyRect(prop, label, true);
			return this.ColorProperty(propertyRect, prop, label);
		}
		public Color ColorProperty(Rect position, MaterialProperty prop, string label)
		{
			EditorGUI.BeginChangeCheck();
			EditorGUI.showMixedValue = prop.hasMixedValue;
			Color colorValue = EditorGUI.ColorField(position, label, prop.colorValue);
			EditorGUI.showMixedValue = false;
			if (EditorGUI.EndChangeCheck())
			{
				prop.colorValue = colorValue;
			}
			return prop.colorValue;
		}
		public Vector4 VectorProperty(MaterialProperty prop, string label)
		{
			Rect propertyRect = this.GetPropertyRect(prop, label, true);
			return this.VectorProperty(propertyRect, prop, label);
		}
		public Vector4 VectorProperty(Rect position, MaterialProperty prop, string label)
		{
			EditorGUI.BeginChangeCheck();
			EditorGUI.showMixedValue = prop.hasMixedValue;
			Vector4 vectorValue = EditorGUI.Vector4Field(position, label, prop.vectorValue);
			EditorGUI.showMixedValue = false;
			if (EditorGUI.EndChangeCheck())
			{
				prop.vectorValue = vectorValue;
			}
			return prop.vectorValue;
		}
		internal static void TextureScaleOffsetProperty(Rect position, MaterialProperty property)
		{
			EditorGUI.BeginChangeCheck();
			int mixedValueMask = property.mixedValueMask >> 1;
			Vector4 textureScaleAndOffset = MaterialEditor.TextureScaleOffsetProperty(position, property.textureScaleAndOffset, mixedValueMask);
			if (EditorGUI.EndChangeCheck())
			{
				property.textureScaleAndOffset = textureScaleAndOffset;
			}
		}
		private Texture TexturePropertyBody(Rect position, MaterialProperty prop, string label)
		{
			position.xMin = position.xMax - 64f;
			this.m_DesiredTexdim = prop.textureDimension;
			Type objType;
			switch (this.m_DesiredTexdim)
			{
			case MaterialProperty.TexDim.Tex2D:
				objType = typeof(Texture);
				goto IL_8F;
			case MaterialProperty.TexDim.Tex3D:
				objType = typeof(Texture3D);
				goto IL_8F;
			case MaterialProperty.TexDim.Cube:
				objType = typeof(Cubemap);
				goto IL_8F;
			case MaterialProperty.TexDim.Any:
				objType = typeof(Texture);
				goto IL_8F;
			}
			objType = null;
			IL_8F:
			bool enabled = GUI.enabled;
			EditorGUI.BeginChangeCheck();
			if ((prop.flags & MaterialProperty.PropFlags.PerRendererData) != MaterialProperty.PropFlags.None)
			{
				GUI.enabled = false;
			}
			EditorGUI.showMixedValue = prop.hasMixedValue;
			Texture textureValue = EditorGUI.DoObjectField(position, position, GUIUtility.GetControlID(12354, EditorGUIUtility.native, position), prop.textureValue, objType, null, new EditorGUI.ObjectFieldValidator(this.TextureValidator), false) as Texture;
			EditorGUI.showMixedValue = false;
			if (EditorGUI.EndChangeCheck())
			{
				prop.textureValue = textureValue;
			}
			GUI.enabled = enabled;
			return prop.textureValue;
		}
		public Texture TextureProperty(MaterialProperty prop, string label)
		{
			return this.TextureProperty(prop, label, true);
		}
		public Texture TextureProperty(MaterialProperty prop, string label, bool scaleOffset)
		{
			Rect propertyRect = this.GetPropertyRect(prop, label, true);
			return this.TextureProperty(propertyRect, prop, label, scaleOffset);
		}
		private static bool DoesNeedNormalMapFix(MaterialProperty prop)
		{
			if (prop.name != "_BumpMap")
			{
				return false;
			}
			UnityEngine.Object[] targets = prop.targets;
			for (int i = 0; i < targets.Length; i++)
			{
				Material material = (Material)targets[i];
				if (InternalEditorUtility.BumpMapTextureNeedsFixing(material))
				{
					return true;
				}
			}
			return false;
		}
		public Texture TextureProperty(Rect position, MaterialProperty prop, string label, bool scaleOffset)
		{
			if (this.target is ProceduralMaterial)
			{
				UnityEngine.Object[] targets = base.targets;
				for (int i = 0; i < targets.Length; i++)
				{
					Material material = (Material)targets[i];
					if (SubstanceImporter.IsProceduralTextureSlot(material, material.GetTexture(prop.name), prop.name))
					{
						break;
					}
				}
			}
			EditorGUI.PrefixLabel(position, EditorGUIUtility.TempContent(label));
			float height = position.height - 64f - 6f;
			position.height = 64f;
			Rect position2 = position;
			position2.xMin = position2.xMax - EditorGUIUtility.fieldWidth;
			Texture result = this.TexturePropertyBody(position2, prop, label);
			if (scaleOffset)
			{
				EditorGUI.indentLevel++;
				Rect rect = position;
				rect.yMin += 16f;
				rect.xMax -= EditorGUIUtility.fieldWidth + 2f;
				rect = EditorGUI.IndentedRect(rect);
				EditorGUI.indentLevel--;
				MaterialEditor.TextureScaleOffsetProperty(rect, prop);
			}
			if (MaterialEditor.DoesNeedNormalMapFix(prop))
			{
				Rect rect2 = position;
				rect2.y += rect2.height;
				rect2.height = height;
				rect2.yMin += 4f;
				EditorGUI.indentLevel++;
				rect2 = EditorGUI.IndentedRect(rect2);
				EditorGUI.indentLevel--;
				GUI.Box(rect2, GUIContent.none, EditorStyles.helpBox);
				rect2 = EditorStyles.helpBox.padding.Remove(rect2);
				Rect position3 = rect2;
				position3.width -= 60f;
				GUI.Label(position3, EditorGUIUtility.TextContent("MaterialInspector.BumpMapFixingWarning"), EditorStyles.wordWrappedMiniLabel);
				position3 = rect2;
				position3.xMin += position3.width - 60f;
				position3.y += 2f;
				position3.height -= 4f;
				if (GUI.Button(position3, EditorGUIUtility.TextContent("MaterialInspector.BumpMapFixingButton")))
				{
					UnityEngine.Object[] targets2 = base.targets;
					for (int j = 0; j < targets2.Length; j++)
					{
						Material material2 = (Material)targets2[j];
						if (InternalEditorUtility.BumpMapTextureNeedsFixing(material2))
						{
							InternalEditorUtility.FixNormalmapTexture(material2);
						}
					}
				}
			}
			return result;
		}
		internal static Vector4 TextureScaleOffsetProperty(Rect position, Vector4 scaleOffset, int mixedValueMask)
		{
			Rect rect = default(Rect);
			Rect rect6;
			Rect rect5;
			Rect rect4;
			Rect rect3;
			Rect rect2 = rect3 = (rect4 = (rect5 = (rect6 = rect)));
			position.y = position.yMax - 48f + 3f;
			rect5.x = position.x;
			rect5.width = 10f;
			rect6.x = rect5.xMax + 2f;
			rect6.width = (position.xMax - rect6.x - 2f) / 2f;
			rect.x = rect6.xMax + 2f;
			rect.xMax = position.xMax;
			rect3.y = position.y;
			rect3.height = 16f;
			rect2 = rect3;
			rect2.y += 16f;
			rect4 = rect2;
			rect4.y += 16f;
			GUI.Label(new Rect(rect5.x, rect2.y, rect5.width, rect2.height), "x", EditorStyles.miniLabel);
			GUI.Label(new Rect(rect5.x, rect4.y, rect5.width, rect4.height), "y", EditorStyles.miniLabel);
			GUI.Label(new Rect(rect6.x, rect3.y, rect6.width, rect3.height), "Tiling", EditorStyles.miniLabel);
			GUI.Label(new Rect(rect.x, rect3.y, rect.width, rect3.height), "Offset", EditorStyles.miniLabel);
			for (int i = 0; i < 2; i++)
			{
				int num = i;
				EditorGUI.showMixedValue = ((mixedValueMask & 1 << num) != 0);
				Rect rect7 = (i != 0) ? rect4 : rect2;
				scaleOffset[num] = EditorGUI.FloatField(new Rect(rect6.x, rect7.y, rect6.width, rect7.height), scaleOffset[num], EditorStyles.miniTextField);
			}
			for (int j = 0; j < 2; j++)
			{
				int num2 = j + 2;
				EditorGUI.showMixedValue = ((mixedValueMask & 1 << num2) != 0);
				Rect rect8 = (j != 0) ? rect4 : rect2;
				scaleOffset[num2] = EditorGUI.FloatField(new Rect(rect.x, rect8.y, rect.width, rect8.height), scaleOffset[num2], EditorStyles.miniTextField);
			}
			return scaleOffset;
		}
		public float GetPropertyHeight(MaterialProperty prop)
		{
			return this.GetPropertyHeight(prop, prop.displayName);
		}
		public float GetPropertyHeight(MaterialProperty prop, string label)
		{
			MaterialPropertyDrawer drawer = MaterialPropertyDrawer.GetDrawer((this.target as Material).shader, prop.name);
			if (drawer != null)
			{
				return drawer.GetPropertyHeight(prop, label ?? prop.displayName, this);
			}
			return MaterialEditor.GetDefaultPropertyHeight(prop);
		}
		public static float GetDefaultPropertyHeight(MaterialProperty prop)
		{
			if (prop.type == MaterialProperty.PropType.Vector)
			{
				return 32f;
			}
			if (prop.type == MaterialProperty.PropType.Texture)
			{
				float num = 70f;
				if (MaterialEditor.DoesNeedNormalMapFix(prop))
				{
					num += 36f;
				}
				return num;
			}
			return 16f;
		}
		private Rect GetPropertyRect(MaterialProperty prop, string label, bool ignoreDrawer)
		{
			if (!ignoreDrawer)
			{
				MaterialPropertyDrawer drawer = MaterialPropertyDrawer.GetDrawer((this.target as Material).shader, prop.name);
				if (drawer != null)
				{
					return EditorGUILayout.GetControlRect(true, drawer.GetPropertyHeight(prop, label ?? prop.displayName, this), EditorStyles.layerMaskField, new GUILayoutOption[0]);
				}
			}
			return EditorGUILayout.GetControlRect(true, MaterialEditor.GetDefaultPropertyHeight(prop), EditorStyles.layerMaskField, new GUILayoutOption[0]);
		}
		public void ShaderProperty(MaterialProperty prop, string label)
		{
			Rect propertyRect = this.GetPropertyRect(prop, label, false);
			this.ShaderProperty(propertyRect, prop, label);
		}
		public void ShaderProperty(Rect position, MaterialProperty prop, string label)
		{
			MaterialPropertyDrawer drawer = MaterialPropertyDrawer.GetDrawer((this.target as Material).shader, prop.name);
			if (drawer != null)
			{
				float labelWidth = EditorGUIUtility.labelWidth;
				float fieldWidth = EditorGUIUtility.fieldWidth;
				drawer.OnGUI(position, prop, label ?? prop.displayName, this);
				EditorGUIUtility.labelWidth = labelWidth;
				EditorGUIUtility.fieldWidth = fieldWidth;
				return;
			}
			this.DefaultShaderProperty(position, prop, label);
		}
		public void DefaultShaderProperty(MaterialProperty prop, string label)
		{
			Rect propertyRect = this.GetPropertyRect(prop, label, true);
			this.DefaultShaderProperty(propertyRect, prop, label);
		}
		public void DefaultShaderProperty(Rect position, MaterialProperty prop, string label)
		{
			switch (prop.type)
			{
			case MaterialProperty.PropType.Color:
				this.ColorProperty(position, prop, label);
				break;
			case MaterialProperty.PropType.Vector:
				this.VectorProperty(position, prop, label);
				break;
			case MaterialProperty.PropType.Float:
				this.FloatProperty(position, prop, label);
				break;
			case MaterialProperty.PropType.Range:
				this.RangeProperty(position, prop, label);
				break;
			case MaterialProperty.PropType.Texture:
				this.TextureProperty(position, prop, label, true);
				break;
			default:
				GUI.Label(position, string.Concat(new object[]
				{
					"Unknown property type: ",
					prop.name,
					": ",
					(int)prop.type
				}));
				break;
			}
		}
		[Obsolete("Use RangeProperty with MaterialProperty instead.")]
		public float RangeProperty(string propertyName, string label, float v2, float v3)
		{
			MaterialProperty materialProperty = MaterialEditor.GetMaterialProperty(base.targets, propertyName);
			return this.RangeProperty(materialProperty, label);
		}
		[Obsolete("Use FloatProperty with MaterialProperty instead.")]
		public float FloatProperty(string propertyName, string label)
		{
			MaterialProperty materialProperty = MaterialEditor.GetMaterialProperty(base.targets, propertyName);
			return this.FloatProperty(materialProperty, label);
		}
		[Obsolete("Use ColorProperty with MaterialProperty instead.")]
		public Color ColorProperty(string propertyName, string label)
		{
			MaterialProperty materialProperty = MaterialEditor.GetMaterialProperty(base.targets, propertyName);
			return this.ColorProperty(materialProperty, label);
		}
		[Obsolete("Use VectorProperty with MaterialProperty instead.")]
		public Vector4 VectorProperty(string propertyName, string label)
		{
			MaterialProperty materialProperty = MaterialEditor.GetMaterialProperty(base.targets, propertyName);
			return this.VectorProperty(materialProperty, label);
		}
		[Obsolete("Use TextureProperty with MaterialProperty instead.")]
		public Texture TextureProperty(string propertyName, string label, ShaderUtil.ShaderPropertyTexDim texDim)
		{
			MaterialProperty materialProperty = MaterialEditor.GetMaterialProperty(base.targets, propertyName);
			return this.TextureProperty(materialProperty, label, true);
		}
		[Obsolete("Use TextureProperty with MaterialProperty instead.")]
		public Texture TextureProperty(string propertyName, string label, ShaderUtil.ShaderPropertyTexDim texDim, bool scaleOffset)
		{
			MaterialProperty materialProperty = MaterialEditor.GetMaterialProperty(base.targets, propertyName);
			return this.TextureProperty(materialProperty, label, scaleOffset);
		}
		[Obsolete("Use ShaderProperty that takes MaterialProperty parameter instead.")]
		public void ShaderProperty(Shader shader, int propertyIndex)
		{
			MaterialProperty materialProperty = MaterialEditor.GetMaterialProperty(base.targets, propertyIndex);
			this.ShaderProperty(materialProperty, materialProperty.displayName);
		}
		public static MaterialProperty[] GetMaterialProperties(UnityEngine.Object[] mats)
		{
			return ShaderUtil.GetMaterialProperties(mats);
		}
		public static MaterialProperty GetMaterialProperty(UnityEngine.Object[] mats, string name)
		{
			return ShaderUtil.GetMaterialProperty(mats, name);
		}
		public static MaterialProperty GetMaterialProperty(UnityEngine.Object[] mats, int propertyIndex)
		{
			return ShaderUtil.GetMaterialProperty_Index(mats, propertyIndex);
		}
		private static Renderer GetAssociatedRenderFromInspector()
		{
			if (InspectorWindow.s_CurrentInspectorWindow)
			{
				Editor[] activeEditors = InspectorWindow.s_CurrentInspectorWindow.GetTracker().activeEditors;
				Editor[] array = activeEditors;
				for (int i = 0; i < array.Length; i++)
				{
					Editor editor = array[i];
					Renderer renderer = editor.target as Renderer;
					if (renderer)
					{
						return renderer;
					}
				}
			}
			return null;
		}
		private Renderer PrepareMaterialPropertiesForAnimationMode(MaterialProperty[] properties)
		{
			if (!AnimationMode.InAnimationMode())
			{
				return null;
			}
			Renderer associatedRenderFromInspector = MaterialEditor.GetAssociatedRenderFromInspector();
			if (associatedRenderFromInspector != null)
			{
				MaterialEditor.ForwardApplyMaterialModification @object = new MaterialEditor.ForwardApplyMaterialModification(associatedRenderFromInspector);
				MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
				associatedRenderFromInspector.GetPropertyBlock(materialPropertyBlock);
				for (int i = 0; i < properties.Length; i++)
				{
					MaterialProperty materialProperty = properties[i];
					materialProperty.ReadFromMaterialPropertyBlock(materialPropertyBlock);
					materialProperty.applyPropertyCallback = new MaterialProperty.ApplyPropertyCallback(@object.DidModifyAnimationModeMaterialProperty);
				}
			}
			return associatedRenderFromInspector;
		}
		public bool PropertiesGUI()
		{
			EditorGUIUtility.fieldWidth = 64f;
			EditorGUIUtility.labelWidth = GUIClip.visibleRect.width - EditorGUIUtility.fieldWidth - 17f;
			EditorGUI.BeginChangeCheck();
			if (this.m_InfoMessage != null)
			{
				EditorGUILayout.HelpBox(this.m_InfoMessage, MessageType.Info);
			}
			MaterialProperty[] materialProperties = MaterialEditor.GetMaterialProperties(base.targets);
			Renderer renderer = this.PrepareMaterialPropertiesForAnimationMode(materialProperties);
			float num = 0f;
			for (int i = 0; i < materialProperties.Length; i++)
			{
				if ((materialProperties[i].flags & MaterialProperty.PropFlags.HideInInspector) == MaterialProperty.PropFlags.None)
				{
					num += this.GetPropertyHeight(materialProperties[i], materialProperties[i].displayName) + 2f;
				}
			}
			Rect controlRect = EditorGUILayout.GetControlRect(true, num, EditorStyles.layerMaskField, new GUILayoutOption[0]);
			for (int j = 0; j < materialProperties.Length; j++)
			{
				if ((materialProperties[j].flags & MaterialProperty.PropFlags.HideInInspector) == MaterialProperty.PropFlags.None)
				{
					float propertyHeight = this.GetPropertyHeight(materialProperties[j], materialProperties[j].displayName);
					controlRect.height = propertyHeight;
					Color color = GUI.color;
					if (renderer != null && MaterialAnimationUtility.IsAnimated(materialProperties[j], renderer))
					{
						GUI.color = AnimationMode.animatedPropertyColor;
					}
					this.ShaderProperty(controlRect, materialProperties[j], materialProperties[j].displayName);
					GUI.color = color;
					controlRect.y += propertyHeight + 2f;
				}
			}
			return EditorGUI.EndChangeCheck();
		}
		public static void ApplyMaterialPropertyDrawers(Material material)
		{
			UnityEngine.Object[] targets = new UnityEngine.Object[]
			{
				material
			};
			MaterialEditor.ApplyMaterialPropertyDrawers(targets);
		}
		public static void ApplyMaterialPropertyDrawers(UnityEngine.Object[] targets)
		{
			if (targets == null || targets.Length == 0)
			{
				return;
			}
			Material material = targets[0] as Material;
			if (material == null)
			{
				return;
			}
			Shader shader = material.shader;
			MaterialProperty[] materialProperties = MaterialEditor.GetMaterialProperties(targets);
			for (int i = 0; i < materialProperties.Length; i++)
			{
				MaterialPropertyDrawer drawer = MaterialPropertyDrawer.GetDrawer(shader, materialProperties[i].name);
				if (drawer != null)
				{
					drawer.Apply(materialProperties[i]);
				}
			}
		}
		public void RegisterPropertyChangeUndo(string label)
		{
			Undo.RecordObjects(base.targets, "Modify " + label + " of " + this.targetTitle);
		}
		private UnityEngine.Object TextureValidator(UnityEngine.Object[] references, Type objType, SerializedProperty property)
		{
			for (int i = 0; i < references.Length; i++)
			{
				UnityEngine.Object @object = references[i];
				Texture texture = @object as Texture;
				if (texture && (ShaderUtil.GetTextureDimension(texture) == (int)this.m_DesiredTexdim || this.m_DesiredTexdim == MaterialProperty.TexDim.Any))
				{
					return texture;
				}
			}
			return null;
		}
		private void Init()
		{
			if (this.m_PreviewUtility == null)
			{
				this.m_PreviewUtility = new PreviewRenderUtility();
				EditorUtility.SetCameraAnimateMaterials(this.m_PreviewUtility.m_Camera, true);
			}
			if (MaterialEditor.s_Meshes[0] == null)
			{
				GameObject gameObject = (GameObject)EditorGUIUtility.LoadRequired("Previews/PreviewMaterials.fbx");
				gameObject.SetActive(false);
				foreach (Transform transform in gameObject.transform)
				{
					string name = transform.name;
					switch (name)
					{
					case "sphere":
						MaterialEditor.s_Meshes[0] = ((MeshFilter)transform.GetComponent("MeshFilter")).sharedMesh;
						continue;
					case "cube":
						MaterialEditor.s_Meshes[1] = ((MeshFilter)transform.GetComponent("MeshFilter")).sharedMesh;
						continue;
					case "cylinder":
						MaterialEditor.s_Meshes[2] = ((MeshFilter)transform.GetComponent("MeshFilter")).sharedMesh;
						continue;
					case "torus":
						MaterialEditor.s_Meshes[3] = ((MeshFilter)transform.GetComponent("MeshFilter")).sharedMesh;
						continue;
					}
					Debug.Log("Something is wrong, weird object found: " + transform.name);
				}
				MaterialEditor.s_MeshIcons[0] = EditorGUIUtility.IconContent("PreMatSphere");
				MaterialEditor.s_MeshIcons[1] = EditorGUIUtility.IconContent("PreMatCube");
				MaterialEditor.s_MeshIcons[2] = EditorGUIUtility.IconContent("PreMatCylinder");
				MaterialEditor.s_MeshIcons[3] = EditorGUIUtility.IconContent("PreMatTorus");
				MaterialEditor.s_LightIcons[0] = EditorGUIUtility.IconContent("PreMatLight0");
				MaterialEditor.s_LightIcons[1] = EditorGUIUtility.IconContent("PreMatLight1");
				MaterialEditor.s_TimeIcons[0] = EditorGUIUtility.IconContent("PlayButton");
				MaterialEditor.s_TimeIcons[1] = EditorGUIUtility.IconContent("PauseButton");
				MaterialEditor.s_PlaneMesh = (Resources.GetBuiltinResource(typeof(Mesh), "Quad.fbx") as Mesh);
			}
		}
		public sealed override void OnPreviewSettings()
		{
			if (!ShaderUtil.hardwareSupportsRectRenderTexture)
			{
				return;
			}
			this.Init();
			Material mat = this.target as Material;
			MaterialEditor.PreviewType previewType = this.GetPreviewType(mat);
			if (base.targets.Length > 1 || previewType == MaterialEditor.PreviewType.Mesh)
			{
				this.m_TimeUpdate = PreviewGUI.CycleButton(this.m_TimeUpdate, MaterialEditor.s_TimeIcons);
				this.m_SelectedMesh = PreviewGUI.CycleButton(this.m_SelectedMesh, MaterialEditor.s_MeshIcons);
				this.m_LightMode = PreviewGUI.CycleButton(this.m_LightMode, MaterialEditor.s_LightIcons);
			}
		}
		public sealed override Texture2D RenderStaticPreview(string assetPath, UnityEngine.Object[] subAssets, int width, int height)
		{
			if (!ShaderUtil.hardwareSupportsRectRenderTexture)
			{
				return null;
			}
			this.Init();
			this.m_PreviewUtility.BeginStaticPreview(new Rect(0f, 0f, (float)width, (float)height));
			this.DoRenderPreview();
			return this.m_PreviewUtility.EndStaticPreview();
		}
		private void DoRenderPreview()
		{
			if (this.m_PreviewUtility.m_RenderTexture.width <= 0 || this.m_PreviewUtility.m_RenderTexture.height <= 0)
			{
				return;
			}
			Material mat = this.target as Material;
			MaterialEditor.PreviewType previewType = this.GetPreviewType(mat);
			this.m_PreviewUtility.m_Camera.transform.position = -Vector3.forward * 5f;
			this.m_PreviewUtility.m_Camera.transform.rotation = Quaternion.identity;
			Color ambient;
			if (this.m_LightMode == 0)
			{
				this.m_PreviewUtility.m_Light[0].intensity = 0.5f;
				this.m_PreviewUtility.m_Light[0].transform.rotation = Quaternion.Euler(30f, 30f, 0f);
				this.m_PreviewUtility.m_Light[1].intensity = 0f;
				ambient = new Color(0.2f, 0.2f, 0.2f, 0f);
			}
			else
			{
				this.m_PreviewUtility.m_Light[0].intensity = 0.5f;
				this.m_PreviewUtility.m_Light[0].transform.rotation = Quaternion.Euler(50f, 50f, 0f);
				this.m_PreviewUtility.m_Light[1].intensity = 0.5f;
				ambient = new Color(0.2f, 0.2f, 0.2f, 0f);
			}
			InternalEditorUtility.SetCustomLighting(this.m_PreviewUtility.m_Light, ambient);
			Quaternion rot = Quaternion.identity;
			if (this.DoesPreviewAllowRotation(previewType))
			{
				rot = Quaternion.Euler(this.m_PreviewDir.y, 0f, 0f) * Quaternion.Euler(0f, this.m_PreviewDir.x, 0f);
			}
			Mesh mesh = MaterialEditor.s_Meshes[this.m_SelectedMesh];
			if (previewType == MaterialEditor.PreviewType.Plane)
			{
				mesh = MaterialEditor.s_PlaneMesh;
			}
			this.m_PreviewUtility.DrawMesh(mesh, Vector3.zero, rot, mat, 0);
			bool fog = RenderSettings.fog;
			Unsupported.SetRenderSettingsUseFogNoDirty(false);
			this.m_PreviewUtility.m_Camera.Render();
			Unsupported.SetRenderSettingsUseFogNoDirty(fog);
			InternalEditorUtility.RemoveCustomLighting();
		}
		public sealed override bool HasPreviewGUI()
		{
			return true;
		}
		public override bool RequiresConstantRepaint()
		{
			return this.m_TimeUpdate == 1;
		}
		public override void OnPreviewGUI(Rect r, GUIStyle background)
		{
			if (!ShaderUtil.hardwareSupportsRectRenderTexture)
			{
				if (Event.current.type == EventType.Repaint)
				{
					EditorGUI.DropShadowLabel(new Rect(r.x, r.y, r.width, 40f), "Material preview \nnot available");
				}
				return;
			}
			this.Init();
			Material mat = this.target as Material;
			MaterialEditor.PreviewType previewType = this.GetPreviewType(mat);
			if (this.DoesPreviewAllowRotation(previewType))
			{
				this.m_PreviewDir = PreviewGUI.Drag2D(this.m_PreviewDir, r);
			}
			if (Event.current.type != EventType.Repaint)
			{
				return;
			}
			this.m_PreviewUtility.BeginPreview(r, background);
			this.DoRenderPreview();
			Texture image = this.m_PreviewUtility.EndPreview();
			GUI.DrawTexture(r, image, ScaleMode.StretchToFill, false);
		}
		public virtual void OnEnable()
		{
			this.m_Shader = base.serializedObject.FindProperty("m_Shader");
			Undo.undoRedoPerformed = (Undo.UndoRedoCallback)Delegate.Combine(Undo.undoRedoPerformed, new Undo.UndoRedoCallback(this.UndoRedoPerformed));
			this.PropertiesChanged();
		}
		public virtual void UndoRedoPerformed()
		{
			if (ActiveEditorTracker.sharedTracker != null)
			{
				ActiveEditorTracker.sharedTracker.ForceRebuild();
			}
			this.PropertiesChanged();
		}
		public virtual void OnDisable()
		{
			if (this.m_PreviewUtility != null)
			{
				this.m_PreviewUtility.Cleanup();
				this.m_PreviewUtility = null;
			}
			Undo.undoRedoPerformed = (Undo.UndoRedoCallback)Delegate.Remove(Undo.undoRedoPerformed, new Undo.UndoRedoCallback(this.UndoRedoPerformed));
		}
		internal void OnSceneDrag(SceneView sceneView)
		{
			Event current = Event.current;
			if (current.type == EventType.Repaint)
			{
				return;
			}
			GameObject gameObject = HandleUtility.PickGameObject(current.mousePosition, false);
			if (!gameObject || !gameObject.renderer)
			{
				return;
			}
			EventType type = current.type;
			if (type != EventType.DragUpdated)
			{
				if (type == EventType.DragPerform)
				{
					DragAndDrop.AcceptDrag();
					Undo.RecordObject(gameObject.renderer, "Set Material");
					gameObject.renderer.sharedMaterial = (this.target as Material);
					current.Use();
				}
			}
			else
			{
				DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
				Undo.RecordObject(gameObject.renderer, "Set Material");
				gameObject.renderer.sharedMaterial = (this.target as Material);
				current.Use();
			}
		}
	}
}
