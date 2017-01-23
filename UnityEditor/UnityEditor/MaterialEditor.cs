using System;
using System.Collections;
using System.Reflection;
using UnityEditor.Utils;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Rendering;

namespace UnityEditor
{
	[CanEditMultipleObjects, CustomEditor(typeof(Material))]
	public class MaterialEditor : Editor
	{
		private static class Styles
		{
			public static readonly GUIStyle kReflectionProbePickerStyle = "PaneOptions";

			public static string[] lightmapEmissiveStrings = new string[]
			{
				"None",
				"Realtime",
				"Baked"
			};

			public static int[] lightmapEmissiveValues = new int[]
			{
				0,
				1,
				2
			};

			public static string lightmapEmissiveLabel = "Global Illumination";

			public static string propBlockWarning = EditorGUIUtility.TextContent("MaterialPropertyBlock is used to modify these values").text;

			public static int kCustomQueueValue = -2;

			public static readonly GUIContent queueLabel = EditorGUIUtility.TextContent("Render Queue");

			public static readonly GUIContent[] queueNames = new GUIContent[]
			{
				EditorGUIUtility.TextContent("From Shader"),
				EditorGUIUtility.TextContent("Geometry|Queue 2000"),
				EditorGUIUtility.TextContent("AlphaTest|Queue 2450"),
				EditorGUIUtility.TextContent("Transparent|Queue 3000"),
				EditorGUIUtility.TextContent("Custom")
			};

			public static readonly int[] queueValues = new int[]
			{
				-1,
				2000,
				2450,
				3000,
				0
			};
		}

		private enum PreviewType
		{
			Mesh,
			Plane,
			Skybox
		}

		internal class ReflectionProbePicker : PopupWindowContent
		{
			private ReflectionProbe m_SelectedReflectionProbe;

			public Transform Target
			{
				get
				{
					return (!(this.m_SelectedReflectionProbe != null)) ? null : this.m_SelectedReflectionProbe.transform;
				}
			}

			public override Vector2 GetWindowSize()
			{
				return new Vector2(170f, 48f);
			}

			public void OnEnable()
			{
				this.m_SelectedReflectionProbe = (EditorUtility.InstanceIDToObject(SessionState.GetInt("PreviewReflectionProbe", 0)) as ReflectionProbe);
			}

			public void OnDisable()
			{
				SessionState.SetInt("PreviewReflectionProbe", (!this.m_SelectedReflectionProbe) ? 0 : this.m_SelectedReflectionProbe.GetInstanceID());
			}

			public override void OnGUI(Rect rc)
			{
				EditorGUILayout.LabelField("Select Reflection Probe", EditorStyles.boldLabel, new GUILayoutOption[0]);
				EditorGUILayout.Space();
				this.m_SelectedReflectionProbe = (EditorGUILayout.ObjectField("", this.m_SelectedReflectionProbe, typeof(ReflectionProbe), true, new GUILayoutOption[0]) as ReflectionProbe);
			}
		}

		private class ForwardApplyMaterialModification
		{
			private readonly Renderer renderer;

			private bool isMaterialEditable;

			public ForwardApplyMaterialModification(Renderer r, bool inIsMaterialEditable)
			{
				this.renderer = r;
				this.isMaterialEditable = inIsMaterialEditable;
			}

			public bool DidModifyAnimationModeMaterialProperty(MaterialProperty property, int changedMask, object previousValue)
			{
				bool flag = MaterialAnimationUtility.ApplyMaterialModificationToAnimationRecording(property, changedMask, this.renderer, previousValue);
				return flag || !this.isMaterialEditable;
			}
		}

		private bool m_IsVisible;

		private bool m_CheckSetup;

		private static int s_ControlHash = "EditorTextField".GetHashCode();

		private const float kSpacingUnderTexture = 6f;

		private const float kWarningMessageHeight = 33f;

		private const float kMiniWarningMessageHeight = 27f;

		private MaterialPropertyBlock m_PropertyBlock;

		private Shader m_Shader;

		private string m_InfoMessage;

		private Vector2 m_PreviewDir = new Vector2(0f, -20f);

		private int m_SelectedMesh;

		private int m_TimeUpdate;

		private int m_LightMode = 1;

		private static readonly GUIContent s_TilingText = new GUIContent("Tiling");

		private static readonly GUIContent s_OffsetText = new GUIContent("Offset");

		private ShaderGUI m_CustomShaderGUI;

		[NonSerialized]
		private bool m_TriedCreatingCustomGUI;

		private bool m_InsidePropertiesGUI;

		private Renderer m_RendererForAnimationMode;

		private Color m_PreviousGUIColor;

		private MaterialEditor.ReflectionProbePicker m_ReflectionProbePicker = new MaterialEditor.ReflectionProbePicker();

		private TextureDimension m_DesiredTexdim;

		private PreviewRenderUtility m_PreviewUtility;

		private static readonly Mesh[] s_Meshes = new Mesh[5];

		private static Mesh s_PlaneMesh;

		private static readonly GUIContent[] s_MeshIcons = new GUIContent[5];

		private static readonly GUIContent[] s_LightIcons = new GUIContent[2];

		private static readonly GUIContent[] s_TimeIcons = new GUIContent[2];

		public const int kMiniTextureFieldLabelIndentLevel = 2;

		private const float kSpaceBetweenFlexibleAreaAndField = 5f;

		private const float kQueuePopupWidth = 100f;

		internal bool forceVisible
		{
			get;
			set;
		}

		public bool isVisible
		{
			get
			{
				return this.forceVisible || this.m_IsVisible;
			}
		}

		private static MaterialEditor.PreviewType GetPreviewType(Material mat)
		{
			MaterialEditor.PreviewType result;
			if (mat == null)
			{
				result = MaterialEditor.PreviewType.Mesh;
			}
			else
			{
				string a = mat.GetTag("PreviewType", false, string.Empty).ToLower();
				if (a == "plane")
				{
					result = MaterialEditor.PreviewType.Plane;
				}
				else if (a == "skybox")
				{
					result = MaterialEditor.PreviewType.Skybox;
				}
				else if (mat.shader != null && mat.shader.name.Contains("Skybox"))
				{
					result = MaterialEditor.PreviewType.Skybox;
				}
				else
				{
					result = MaterialEditor.PreviewType.Mesh;
				}
			}
			return result;
		}

		private static bool DoesPreviewAllowRotation(MaterialEditor.PreviewType type)
		{
			return type != MaterialEditor.PreviewType.Plane;
		}

		public void SetShader(Shader shader)
		{
			this.SetShader(shader, true);
		}

		public void SetShader(Shader newShader, bool registerUndo)
		{
			bool flag = false;
			ShaderGUI customShaderGUI = this.m_CustomShaderGUI;
			string oldEditorName = (!(this.m_Shader != null)) ? string.Empty : this.m_Shader.customEditor;
			this.CreateCustomShaderGUI(newShader, oldEditorName);
			this.m_Shader = newShader;
			if (customShaderGUI != this.m_CustomShaderGUI)
			{
				flag = true;
			}
			UnityEngine.Object[] targets = base.targets;
			for (int i = 0; i < targets.Length; i++)
			{
				Material material = (Material)targets[i];
				Shader shader = material.shader;
				Undo.RecordObject(material, "Assign shader");
				if (this.m_CustomShaderGUI != null)
				{
					this.m_CustomShaderGUI.AssignNewShaderToMaterial(material, shader, newShader);
				}
				else
				{
					material.shader = newShader;
				}
				EditorMaterialUtility.ResetDefaultTextures(material, false);
				MaterialEditor.ApplyMaterialPropertyDrawers(material);
			}
			if (flag && ActiveEditorTracker.sharedTracker != null)
			{
				InspectorWindow[] allInspectorWindows = InspectorWindow.GetAllInspectorWindows();
				for (int j = 0; j < allInspectorWindows.Length; j++)
				{
					InspectorWindow inspectorWindow = allInspectorWindows[j];
					inspectorWindow.GetTracker().ForceRebuild();
				}
			}
		}

		internal void OnSelectedShaderPopup(string command, Shader shader)
		{
			base.serializedObject.Update();
			if (shader != null)
			{
				this.SetShader(shader);
			}
			this.PropertiesChanged();
		}

		private bool HasMultipleMixedShaderValues()
		{
			bool result = false;
			Shader shader = (base.targets[0] as Material).shader;
			for (int i = 1; i < base.targets.Length; i++)
			{
				if (shader != (base.targets[i] as Material).shader)
				{
					result = true;
					break;
				}
			}
			return result;
		}

		private void ShaderPopup(GUIStyle style)
		{
			bool enabled = GUI.enabled;
			Rect rect = EditorGUILayout.GetControlRect(new GUILayoutOption[0]);
			rect = EditorGUI.PrefixLabel(rect, 47385, EditorGUIUtility.TempContent("Shader"));
			EditorGUI.showMixedValue = this.HasMultipleMixedShaderValues();
			GUIContent content = EditorGUIUtility.TempContent((!(this.m_Shader != null)) ? "No Shader Selected" : this.m_Shader.name);
			if (EditorGUI.ButtonMouseDown(rect, content, FocusType.Keyboard, style))
			{
				EditorGUI.showMixedValue = false;
				Vector2 vector = GUIUtility.GUIToScreenPoint(new Vector2(rect.x, rect.y));
				InternalEditorUtility.SetupShaderMenu(base.target as Material);
				EditorUtility.Internal_DisplayPopupMenu(new Rect(vector.x, vector.y, rect.width, rect.height), "CONTEXT/ShaderPopup", this, 0);
				Event.current.Use();
			}
			EditorGUI.showMixedValue = false;
			GUI.enabled = enabled;
		}

		public virtual void Awake()
		{
			this.m_IsVisible = InternalEditorUtility.GetIsInspectorExpanded(base.target);
			if (MaterialEditor.GetPreviewType(base.target as Material) == MaterialEditor.PreviewType.Skybox)
			{
				this.m_PreviewDir = new Vector2(0f, 50f);
			}
		}

		private void DetectShaderChanged()
		{
			Material material = base.target as Material;
			if (material.shader != this.m_Shader)
			{
				string oldEditorName = (!(this.m_Shader != null)) ? string.Empty : this.m_Shader.customEditor;
				this.CreateCustomShaderGUI(material.shader, oldEditorName);
				this.m_Shader = material.shader;
				InspectorWindow.RepaintAllInspectors();
			}
		}

		public override void OnInspectorGUI()
		{
			base.serializedObject.Update();
			this.CheckSetup();
			this.DetectShaderChanged();
			if (this.isVisible && this.m_Shader != null && !this.HasMultipleMixedShaderValues())
			{
				if (this.PropertiesGUI())
				{
					this.PropertiesChanged();
				}
			}
		}

		private void CheckSetup()
		{
			if (this.m_CheckSetup && !(this.m_Shader == null))
			{
				this.m_CheckSetup = false;
				if (this.m_CustomShaderGUI == null && !this.IsMaterialEditor(this.m_Shader.customEditor))
				{
					Debug.LogWarningFormat("Could not create a custom UI for the shader '{0}'. The shader has the following: 'CustomEditor = {1}'. Does the custom editor specified include its namespace? And does the class either derive from ShaderGUI or MaterialEditor?", new object[]
					{
						this.m_Shader.name,
						this.m_Shader.customEditor
					});
				}
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
				this.m_InfoMessage = PerformanceChecks.CheckMaterial(base.target as Material, EditorUserBuildSettings.activeBuildTarget);
			}
		}

		protected override void OnHeaderGUI()
		{
			Rect rect = Editor.DrawHeaderGUI(this, this.targetTitle, (!this.forceVisible) ? 10f : 0f);
			int controlID = GUIUtility.GetControlID(45678, FocusType.Passive);
			if (!this.forceVisible)
			{
				Rect inspectorTitleBarObjectFoldoutRenderRect = EditorGUI.GetInspectorTitleBarObjectFoldoutRenderRect(rect);
				inspectorTitleBarObjectFoldoutRenderRect.y = rect.yMax - 17f;
				bool flag = EditorGUI.DoObjectFoldout(this.m_IsVisible, rect, inspectorTitleBarObjectFoldoutRenderRect, base.targets, controlID);
				if (flag != this.m_IsVisible)
				{
					this.m_IsVisible = flag;
					InternalEditorUtility.SetIsInspectorExpanded(base.target, flag);
				}
			}
		}

		internal override void OnHeaderControlsGUI()
		{
			base.serializedObject.Update();
			using (new EditorGUI.DisabledScope(!this.IsEnabled()))
			{
				EditorGUIUtility.labelWidth = 50f;
				this.ShaderPopup("MiniPulldown");
				if (this.m_Shader != null && this.HasMultipleMixedShaderValues() && (this.m_Shader.hideFlags & HideFlags.DontSave) == HideFlags.None)
				{
					if (GUILayout.Button("Edit...", EditorStyles.miniButton, new GUILayoutOption[]
					{
						GUILayout.ExpandWidth(false)
					}))
					{
						AssetDatabase.OpenAsset(this.m_Shader);
					}
				}
			}
		}

		[Obsolete("Use GetMaterialProperty instead.")]
		public float GetFloat(string propertyName, out bool hasMixedValue)
		{
			hasMixedValue = false;
			float @float = ((Material)base.targets[0]).GetFloat(propertyName);
			for (int i = 1; i < base.targets.Length; i++)
			{
				if (((Material)base.targets[i]).GetFloat(propertyName) != @float)
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
			Color color = ((Material)base.targets[0]).GetColor(propertyName);
			for (int i = 1; i < base.targets.Length; i++)
			{
				if (((Material)base.targets[i]).GetColor(propertyName) != color)
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
			Vector4 vector = ((Material)base.targets[0]).GetVector(propertyName);
			for (int i = 1; i < base.targets.Length; i++)
			{
				if (((Material)base.targets[i]).GetVector(propertyName) != vector)
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
			Texture texture = ((Material)base.targets[0]).GetTexture(propertyName);
			for (int i = 1; i < base.targets.Length; i++)
			{
				if (((Material)base.targets[i]).GetTexture(propertyName) != texture)
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
			Vector2 textureScale = ((Material)base.targets[0]).GetTextureScale(propertyName);
			for (int i = 1; i < base.targets.Length; i++)
			{
				Vector2 textureScale2 = ((Material)base.targets[i]).GetTextureScale(propertyName);
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
			Vector2 textureOffset = ((Material)base.targets[0]).GetTextureOffset(propertyName);
			for (int i = 1; i < base.targets.Length; i++)
			{
				Vector2 textureOffset2 = ((Material)base.targets[i]).GetTextureOffset(propertyName);
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
			return this.RangePropertyInternal(prop, new GUIContent(label));
		}

		internal float RangePropertyInternal(MaterialProperty prop, GUIContent label)
		{
			Rect propertyRect = this.GetPropertyRect(prop, label, true);
			return this.RangePropertyInternal(propertyRect, prop, label);
		}

		public float RangeProperty(Rect position, MaterialProperty prop, string label)
		{
			return this.RangePropertyInternal(position, prop, new GUIContent(label));
		}

		internal float RangePropertyInternal(Rect position, MaterialProperty prop, GUIContent label)
		{
			float power = (!(prop.name == "_Shininess")) ? 1f : 5f;
			return MaterialEditor.DoPowerRangeProperty(position, prop, label, power);
		}

		internal static float DoPowerRangeProperty(Rect position, MaterialProperty prop, GUIContent label, float power)
		{
			EditorGUI.BeginChangeCheck();
			EditorGUI.showMixedValue = prop.hasMixedValue;
			float labelWidth = EditorGUIUtility.labelWidth;
			EditorGUIUtility.labelWidth = 0f;
			float floatValue = EditorGUI.PowerSlider(position, label, prop.floatValue, prop.rangeLimits.x, prop.rangeLimits.y, power);
			EditorGUI.showMixedValue = false;
			EditorGUIUtility.labelWidth = labelWidth;
			if (EditorGUI.EndChangeCheck())
			{
				prop.floatValue = floatValue;
			}
			return prop.floatValue;
		}

		internal static int DoIntRangeProperty(Rect position, MaterialProperty prop, GUIContent label)
		{
			EditorGUI.BeginChangeCheck();
			EditorGUI.showMixedValue = prop.hasMixedValue;
			float labelWidth = EditorGUIUtility.labelWidth;
			EditorGUIUtility.labelWidth = 0f;
			int num = EditorGUI.IntSlider(position, label, (int)prop.floatValue, (int)prop.rangeLimits.x, (int)prop.rangeLimits.y);
			EditorGUI.showMixedValue = false;
			EditorGUIUtility.labelWidth = labelWidth;
			if (EditorGUI.EndChangeCheck())
			{
				prop.floatValue = (float)num;
			}
			return (int)prop.floatValue;
		}

		public float FloatProperty(MaterialProperty prop, string label)
		{
			return this.FloatPropertyInternal(prop, new GUIContent(label));
		}

		internal float FloatPropertyInternal(MaterialProperty prop, GUIContent label)
		{
			Rect propertyRect = this.GetPropertyRect(prop, label, true);
			return this.FloatPropertyInternal(propertyRect, prop, label);
		}

		public float FloatProperty(Rect position, MaterialProperty prop, string label)
		{
			return this.FloatPropertyInternal(position, prop, new GUIContent(label));
		}

		internal float FloatPropertyInternal(Rect position, MaterialProperty prop, GUIContent label)
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
			return this.ColorPropertyInternal(prop, new GUIContent(label));
		}

		internal Color ColorPropertyInternal(MaterialProperty prop, GUIContent label)
		{
			Rect propertyRect = this.GetPropertyRect(prop, label, true);
			return this.ColorPropertyInternal(propertyRect, prop, label);
		}

		public Color ColorProperty(Rect position, MaterialProperty prop, string label)
		{
			return this.ColorPropertyInternal(position, prop, new GUIContent(label));
		}

		internal Color ColorPropertyInternal(Rect position, MaterialProperty prop, GUIContent label)
		{
			EditorGUI.BeginChangeCheck();
			EditorGUI.showMixedValue = prop.hasMixedValue;
			bool hdr = (prop.flags & MaterialProperty.PropFlags.HDR) != MaterialProperty.PropFlags.None;
			bool showAlpha = true;
			Color colorValue = EditorGUI.ColorField(position, label, prop.colorValue, true, showAlpha, hdr, null);
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
			float labelWidth = EditorGUIUtility.labelWidth;
			EditorGUIUtility.labelWidth = 0f;
			Vector4 vectorValue = EditorGUI.Vector4Field(position, label, prop.vectorValue);
			EditorGUIUtility.labelWidth = labelWidth;
			EditorGUI.showMixedValue = false;
			if (EditorGUI.EndChangeCheck())
			{
				prop.vectorValue = vectorValue;
			}
			return prop.vectorValue;
		}

		public void TextureScaleOffsetProperty(MaterialProperty property)
		{
			Rect controlRect = EditorGUILayout.GetControlRect(true, 32f, EditorStyles.layerMaskField, new GUILayoutOption[0]);
			this.TextureScaleOffsetProperty(controlRect, property, false);
		}

		public float TextureScaleOffsetProperty(Rect position, MaterialProperty property)
		{
			return this.TextureScaleOffsetProperty(position, property, true);
		}

		public float TextureScaleOffsetProperty(Rect position, MaterialProperty property, bool partOfTexturePropertyControl)
		{
			this.BeginAnimatedCheck(property);
			EditorGUI.BeginChangeCheck();
			int mixedValueMask = property.mixedValueMask >> 1;
			Vector4 textureScaleAndOffset = MaterialEditor.TextureScaleOffsetProperty(position, property.textureScaleAndOffset, mixedValueMask, partOfTexturePropertyControl);
			if (EditorGUI.EndChangeCheck())
			{
				property.textureScaleAndOffset = textureScaleAndOffset;
			}
			this.EndAnimatedCheck();
			return 32f;
		}

		private Texture TexturePropertyBody(Rect position, MaterialProperty prop)
		{
			if (prop.type != MaterialProperty.PropType.Texture)
			{
				throw new ArgumentException(string.Format("The MaterialProperty '{0}' should be of type 'Texture' (its type is '{1})'", prop.name, prop.type));
			}
			this.m_DesiredTexdim = prop.textureDimension;
			Type textureTypeFromDimension = MaterialEditor.GetTextureTypeFromDimension(this.m_DesiredTexdim);
			bool enabled = GUI.enabled;
			EditorGUI.BeginChangeCheck();
			if ((prop.flags & MaterialProperty.PropFlags.PerRendererData) != MaterialProperty.PropFlags.None)
			{
				GUI.enabled = false;
			}
			EditorGUI.showMixedValue = prop.hasMixedValue;
			int controlID = GUIUtility.GetControlID(12354, FocusType.Keyboard, position);
			Texture textureValue = EditorGUI.DoObjectField(position, position, controlID, prop.textureValue, textureTypeFromDimension, null, new EditorGUI.ObjectFieldValidator(this.TextureValidator), false) as Texture;
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
			bool scaleOffset = (prop.flags & MaterialProperty.PropFlags.NoScaleOffset) == MaterialProperty.PropFlags.None;
			return this.TextureProperty(prop, label, scaleOffset);
		}

		public Texture TextureProperty(MaterialProperty prop, string label, bool scaleOffset)
		{
			Rect propertyRect = this.GetPropertyRect(prop, label, true);
			return this.TextureProperty(propertyRect, prop, label, scaleOffset);
		}

		public bool HelpBoxWithButton(GUIContent messageContent, GUIContent buttonContent)
		{
			Rect rect = GUILayoutUtility.GetRect(messageContent, EditorStyles.helpBox);
			GUILayoutUtility.GetRect(1f, 25f);
			rect.height += 25f;
			GUI.Label(rect, messageContent, EditorStyles.helpBox);
			Rect position = new Rect(rect.xMax - 60f - 4f, rect.yMax - 20f - 4f, 60f, 20f);
			return GUI.Button(position, buttonContent);
		}

		public void TextureCompatibilityWarning(MaterialProperty prop)
		{
			if (InternalEditorUtility.BumpMapTextureNeedsFixing(prop))
			{
				if (this.HelpBoxWithButton(EditorGUIUtility.TextContent("This texture is not marked as a normal map"), EditorGUIUtility.TextContent("Fix Now")))
				{
					InternalEditorUtility.FixNormalmapTexture(prop);
				}
			}
		}

		public Texture TexturePropertyMiniThumbnail(Rect position, MaterialProperty prop, string label, string tooltip)
		{
			this.BeginAnimatedCheck(prop);
			Rect position2;
			Rect labelPosition;
			EditorGUI.GetRectsForMiniThumbnailField(position, out position2, out labelPosition);
			EditorGUI.HandlePrefixLabel(position, labelPosition, new GUIContent(label, tooltip), 0, EditorStyles.label);
			this.EndAnimatedCheck();
			Texture result = this.TexturePropertyBody(position2, prop);
			Rect rect = position;
			rect.y += position.height;
			rect.height = 27f;
			this.TextureCompatibilityWarning(prop);
			return result;
		}

		public Rect GetTexturePropertyCustomArea(Rect position)
		{
			EditorGUI.indentLevel++;
			position.height = MaterialEditor.GetTextureFieldHeight();
			Rect rect = position;
			rect.yMin += 16f;
			rect.xMax -= EditorGUIUtility.fieldWidth + 2f;
			rect = EditorGUI.IndentedRect(rect);
			EditorGUI.indentLevel--;
			return rect;
		}

		public Texture TextureProperty(Rect position, MaterialProperty prop, string label)
		{
			bool scaleOffset = (prop.flags & MaterialProperty.PropFlags.NoScaleOffset) == MaterialProperty.PropFlags.None;
			return this.TextureProperty(position, prop, label, scaleOffset);
		}

		public Texture TextureProperty(Rect position, MaterialProperty prop, string label, bool scaleOffset)
		{
			return this.TextureProperty(position, prop, label, string.Empty, scaleOffset);
		}

		public Texture TextureProperty(Rect position, MaterialProperty prop, string label, string tooltip, bool scaleOffset)
		{
			EditorGUI.PrefixLabel(position, new GUIContent(label, tooltip));
			position.height = MaterialEditor.GetTextureFieldHeight();
			Rect position2 = position;
			position2.xMin = position2.xMax - EditorGUIUtility.fieldWidth;
			Texture result = this.TexturePropertyBody(position2, prop);
			if (scaleOffset)
			{
				this.TextureScaleOffsetProperty(this.GetTexturePropertyCustomArea(position), prop);
			}
			GUILayout.Space(-6f);
			this.TextureCompatibilityWarning(prop);
			GUILayout.Space(6f);
			return result;
		}

		public static Vector4 TextureScaleOffsetProperty(Rect position, Vector4 scaleOffset)
		{
			return MaterialEditor.TextureScaleOffsetProperty(position, scaleOffset, 0, false);
		}

		public static Vector4 TextureScaleOffsetProperty(Rect position, Vector4 scaleOffset, bool partOfTexturePropertyControl)
		{
			return MaterialEditor.TextureScaleOffsetProperty(position, scaleOffset, 0, partOfTexturePropertyControl);
		}

		internal static Vector4 TextureScaleOffsetProperty(Rect position, Vector4 scaleOffset, int mixedValueMask, bool partOfTexturePropertyControl)
		{
			Vector2 value = new Vector2(scaleOffset.x, scaleOffset.y);
			Vector2 value2 = new Vector2(scaleOffset.z, scaleOffset.w);
			float num = EditorGUIUtility.labelWidth;
			float x = position.x + num;
			float x2 = position.x + EditorGUI.indent;
			if (partOfTexturePropertyControl)
			{
				num = 65f;
				x = position.x + num;
				x2 = position.x;
				position.y = position.yMax - 32f;
			}
			Rect totalPosition = new Rect(x2, position.y, num, 16f);
			Rect position2 = new Rect(x, position.y, position.width - num, 16f);
			EditorGUI.PrefixLabel(totalPosition, MaterialEditor.s_TilingText);
			value = EditorGUI.Vector2Field(position2, GUIContent.none, value);
			totalPosition.y += 16f;
			position2.y += 16f;
			EditorGUI.PrefixLabel(totalPosition, MaterialEditor.s_OffsetText);
			value2 = EditorGUI.Vector2Field(position2, GUIContent.none, value2);
			return new Vector4(value.x, value.y, value2.x, value2.y);
		}

		public float GetPropertyHeight(MaterialProperty prop)
		{
			return this.GetPropertyHeight(prop, prop.displayName);
		}

		public float GetPropertyHeight(MaterialProperty prop, string label)
		{
			float num = 0f;
			MaterialPropertyHandler handler = MaterialPropertyHandler.GetHandler(((Material)base.target).shader, prop.name);
			float result;
			if (handler != null)
			{
				num = handler.GetPropertyHeight(prop, label ?? prop.displayName, this);
				if (handler.propertyDrawer != null)
				{
					result = num;
					return result;
				}
			}
			result = num + MaterialEditor.GetDefaultPropertyHeight(prop);
			return result;
		}

		private static float GetTextureFieldHeight()
		{
			return 64f;
		}

		public static float GetDefaultPropertyHeight(MaterialProperty prop)
		{
			float result;
			if (prop.type == MaterialProperty.PropType.Vector)
			{
				result = 32f;
			}
			else if (prop.type == MaterialProperty.PropType.Texture)
			{
				result = MaterialEditor.GetTextureFieldHeight() + 6f;
			}
			else
			{
				result = 16f;
			}
			return result;
		}

		private Rect GetPropertyRect(MaterialProperty prop, GUIContent label, bool ignoreDrawer)
		{
			return this.GetPropertyRect(prop, label.text, ignoreDrawer);
		}

		private Rect GetPropertyRect(MaterialProperty prop, string label, bool ignoreDrawer)
		{
			float num = 0f;
			Rect controlRect;
			if (!ignoreDrawer)
			{
				MaterialPropertyHandler handler = MaterialPropertyHandler.GetHandler(((Material)base.target).shader, prop.name);
				if (handler != null)
				{
					num = handler.GetPropertyHeight(prop, label ?? prop.displayName, this);
					if (handler.propertyDrawer != null)
					{
						controlRect = EditorGUILayout.GetControlRect(true, num, EditorStyles.layerMaskField, new GUILayoutOption[0]);
						return controlRect;
					}
				}
			}
			controlRect = EditorGUILayout.GetControlRect(true, num + MaterialEditor.GetDefaultPropertyHeight(prop), EditorStyles.layerMaskField, new GUILayoutOption[0]);
			return controlRect;
		}

		public void BeginAnimatedCheck(MaterialProperty prop)
		{
			if (!(this.m_RendererForAnimationMode == null))
			{
				this.m_PreviousGUIColor = GUI.color;
				if (MaterialAnimationUtility.IsAnimated(prop, this.m_RendererForAnimationMode))
				{
					GUI.color = AnimationMode.animatedPropertyColor;
				}
			}
		}

		public void EndAnimatedCheck()
		{
			if (!(this.m_RendererForAnimationMode == null))
			{
				GUI.color = this.m_PreviousGUIColor;
			}
		}

		public void ShaderProperty(MaterialProperty prop, string label)
		{
			this.ShaderProperty(prop, new GUIContent(label));
		}

		public void ShaderProperty(MaterialProperty prop, GUIContent label)
		{
			this.ShaderProperty(prop, label, 0);
		}

		public void ShaderProperty(MaterialProperty prop, string label, int labelIndent)
		{
			this.ShaderProperty(prop, new GUIContent(label), labelIndent);
		}

		public void ShaderProperty(MaterialProperty prop, GUIContent label, int labelIndent)
		{
			Rect propertyRect = this.GetPropertyRect(prop, label, false);
			this.ShaderProperty(propertyRect, prop, label, labelIndent);
		}

		public void ShaderProperty(Rect position, MaterialProperty prop, string label)
		{
			this.ShaderProperty(position, prop, new GUIContent(label));
		}

		public void ShaderProperty(Rect position, MaterialProperty prop, GUIContent label)
		{
			this.ShaderProperty(position, prop, label, 0);
		}

		public void ShaderProperty(Rect position, MaterialProperty prop, string label, int labelIndent)
		{
			this.ShaderProperty(position, prop, new GUIContent(label), labelIndent);
		}

		public void ShaderProperty(Rect position, MaterialProperty prop, GUIContent label, int labelIndent)
		{
			this.BeginAnimatedCheck(prop);
			EditorGUI.indentLevel += labelIndent;
			this.ShaderPropertyInternal(position, prop, label);
			EditorGUI.indentLevel -= labelIndent;
			this.EndAnimatedCheck();
		}

		public void LightmapEmissionProperty()
		{
			this.LightmapEmissionProperty(0);
		}

		public void LightmapEmissionProperty(int labelIndent)
		{
			Rect controlRect = EditorGUILayout.GetControlRect(true, 16f, EditorStyles.layerMaskField, new GUILayoutOption[0]);
			this.LightmapEmissionProperty(controlRect, labelIndent);
		}

		private static MaterialGlobalIlluminationFlags GetGlobalIlluminationInt(MaterialGlobalIlluminationFlags flags)
		{
			MaterialGlobalIlluminationFlags result = MaterialGlobalIlluminationFlags.None;
			if ((flags & MaterialGlobalIlluminationFlags.RealtimeEmissive) != MaterialGlobalIlluminationFlags.None)
			{
				result = MaterialGlobalIlluminationFlags.RealtimeEmissive;
			}
			else if ((flags & MaterialGlobalIlluminationFlags.BakedEmissive) != MaterialGlobalIlluminationFlags.None)
			{
				result = MaterialGlobalIlluminationFlags.BakedEmissive;
			}
			return result;
		}

		public void LightmapEmissionProperty(Rect position, int labelIndent)
		{
			EditorGUI.indentLevel += labelIndent;
			UnityEngine.Object[] targets = base.targets;
			Material material = (Material)base.target;
			MaterialGlobalIlluminationFlags materialGlobalIlluminationFlags = MaterialEditor.GetGlobalIlluminationInt(material.globalIlluminationFlags);
			bool showMixedValue = false;
			for (int i = 1; i < targets.Length; i++)
			{
				Material material2 = (Material)targets[i];
				if (MaterialEditor.GetGlobalIlluminationInt(material2.globalIlluminationFlags) != materialGlobalIlluminationFlags)
				{
					showMixedValue = true;
				}
			}
			EditorGUI.BeginChangeCheck();
			EditorGUI.showMixedValue = showMixedValue;
			materialGlobalIlluminationFlags = (MaterialGlobalIlluminationFlags)EditorGUI.IntPopup(position, MaterialEditor.Styles.lightmapEmissiveLabel, (int)materialGlobalIlluminationFlags, MaterialEditor.Styles.lightmapEmissiveStrings, MaterialEditor.Styles.lightmapEmissiveValues);
			EditorGUI.showMixedValue = false;
			if (EditorGUI.EndChangeCheck())
			{
				UnityEngine.Object[] array = targets;
				for (int j = 0; j < array.Length; j++)
				{
					Material material3 = (Material)array[j];
					MaterialGlobalIlluminationFlags materialGlobalIlluminationFlags2 = material3.globalIlluminationFlags;
					materialGlobalIlluminationFlags2 &= ~(MaterialGlobalIlluminationFlags.RealtimeEmissive | MaterialGlobalIlluminationFlags.BakedEmissive);
					materialGlobalIlluminationFlags2 |= materialGlobalIlluminationFlags;
					material3.globalIlluminationFlags = materialGlobalIlluminationFlags2;
				}
			}
			EditorGUI.indentLevel -= labelIndent;
		}

		private void ShaderPropertyInternal(Rect position, MaterialProperty prop, GUIContent label)
		{
			MaterialPropertyHandler handler = MaterialPropertyHandler.GetHandler(((Material)base.target).shader, prop.name);
			if (handler != null)
			{
				handler.OnGUI(ref position, prop, (label.text == null) ? new GUIContent(prop.displayName) : label, this);
				if (handler.propertyDrawer != null)
				{
					return;
				}
			}
			this.DefaultShaderPropertyInternal(position, prop, label);
		}

		public void DefaultShaderProperty(MaterialProperty prop, string label)
		{
			this.DefaultShaderPropertyInternal(prop, new GUIContent(label));
		}

		internal void DefaultShaderPropertyInternal(MaterialProperty prop, GUIContent label)
		{
			Rect propertyRect = this.GetPropertyRect(prop, label, true);
			this.DefaultShaderPropertyInternal(propertyRect, prop, label);
		}

		public void DefaultShaderProperty(Rect position, MaterialProperty prop, string label)
		{
			this.DefaultShaderPropertyInternal(position, prop, new GUIContent(label));
		}

		internal void DefaultShaderPropertyInternal(Rect position, MaterialProperty prop, GUIContent label)
		{
			switch (prop.type)
			{
			case MaterialProperty.PropType.Color:
				this.ColorPropertyInternal(position, prop, label);
				break;
			case MaterialProperty.PropType.Vector:
				this.VectorProperty(position, prop, label.text);
				break;
			case MaterialProperty.PropType.Float:
				this.FloatPropertyInternal(position, prop, label);
				break;
			case MaterialProperty.PropType.Range:
				this.RangePropertyInternal(position, prop, label);
				break;
			case MaterialProperty.PropType.Texture:
				this.TextureProperty(position, prop, label.text);
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
			return this.TextureProperty(materialProperty, label);
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
			if (mats == null)
			{
				throw new ArgumentNullException("mats");
			}
			if (Array.IndexOf<UnityEngine.Object>(mats, null) >= 0)
			{
				throw new ArgumentException("List of materials contains null");
			}
			return ShaderUtil.GetMaterialProperties(mats);
		}

		public static MaterialProperty GetMaterialProperty(UnityEngine.Object[] mats, string name)
		{
			if (mats == null)
			{
				throw new ArgumentNullException("mats");
			}
			if (Array.IndexOf<UnityEngine.Object>(mats, null) >= 0)
			{
				throw new ArgumentException("List of materials contains null");
			}
			return ShaderUtil.GetMaterialProperty(mats, name);
		}

		public static MaterialProperty GetMaterialProperty(UnityEngine.Object[] mats, int propertyIndex)
		{
			if (mats == null)
			{
				throw new ArgumentNullException("mats");
			}
			if (Array.IndexOf<UnityEngine.Object>(mats, null) >= 0)
			{
				throw new ArgumentException("List of materials contains null");
			}
			return ShaderUtil.GetMaterialProperty_Index(mats, propertyIndex);
		}

		private static Renderer GetAssociatedRenderFromInspector()
		{
			Renderer result;
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
						result = renderer;
						return result;
					}
				}
			}
			result = null;
			return result;
		}

		public static Renderer PrepareMaterialPropertiesForAnimationMode(MaterialProperty[] properties, bool isMaterialEditable)
		{
			bool flag = AnimationMode.InAnimationMode();
			Renderer associatedRenderFromInspector = MaterialEditor.GetAssociatedRenderFromInspector();
			if (associatedRenderFromInspector != null)
			{
				MaterialEditor.ForwardApplyMaterialModification @object = new MaterialEditor.ForwardApplyMaterialModification(associatedRenderFromInspector, isMaterialEditable);
				MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
				associatedRenderFromInspector.GetPropertyBlock(materialPropertyBlock);
				for (int i = 0; i < properties.Length; i++)
				{
					MaterialProperty materialProperty = properties[i];
					materialProperty.ReadFromMaterialPropertyBlock(materialPropertyBlock);
					if (flag)
					{
						materialProperty.applyPropertyCallback = new MaterialProperty.ApplyPropertyCallback(@object.DidModifyAnimationModeMaterialProperty);
					}
				}
			}
			Renderer result;
			if (flag)
			{
				result = associatedRenderFromInspector;
			}
			else
			{
				result = null;
			}
			return result;
		}

		public void SetDefaultGUIWidths()
		{
			EditorGUIUtility.fieldWidth = 64f;
			EditorGUIUtility.labelWidth = GUIClip.visibleRect.width - EditorGUIUtility.fieldWidth - 17f;
		}

		private bool IsMaterialEditor(string customEditorName)
		{
			string value = "UnityEditor." + customEditorName;
			Assembly[] loadedAssemblies = EditorAssemblies.loadedAssemblies;
			bool result;
			for (int i = 0; i < loadedAssemblies.Length; i++)
			{
				Assembly assembly = loadedAssemblies[i];
				Type[] typesFromAssembly = AssemblyHelper.GetTypesFromAssembly(assembly);
				Type[] array = typesFromAssembly;
				for (int j = 0; j < array.Length; j++)
				{
					Type type = array[j];
					if (type.FullName.Equals(customEditorName, StringComparison.Ordinal) || type.FullName.Equals(value, StringComparison.Ordinal))
					{
						if (typeof(MaterialEditor).IsAssignableFrom(type))
						{
							result = true;
							return result;
						}
					}
				}
			}
			result = false;
			return result;
		}

		private void CreateCustomShaderGUI(Shader shader, string oldEditorName)
		{
			if (shader == null || string.IsNullOrEmpty(shader.customEditor))
			{
				this.m_CustomShaderGUI = null;
			}
			else if (!(oldEditorName == shader.customEditor))
			{
				this.m_CustomShaderGUI = ShaderGUIUtility.CreateShaderGUI(shader.customEditor);
				this.m_CheckSetup = true;
			}
		}

		public bool PropertiesGUI()
		{
			bool result;
			if (this.m_InsidePropertiesGUI)
			{
				Debug.LogWarning("PropertiesGUI() is being called recursively. If you want to render the default gui for shader properties then call PropertiesDefaultGUI() instead");
				result = false;
			}
			else
			{
				EditorGUI.BeginChangeCheck();
				MaterialProperty[] materialProperties = MaterialEditor.GetMaterialProperties(base.targets);
				this.m_RendererForAnimationMode = MaterialEditor.PrepareMaterialPropertiesForAnimationMode(materialProperties, GUI.enabled);
				bool enabled = GUI.enabled;
				if (this.m_RendererForAnimationMode != null)
				{
					GUI.enabled = true;
				}
				this.m_InsidePropertiesGUI = true;
				try
				{
					if (this.m_CustomShaderGUI != null)
					{
						this.m_CustomShaderGUI.OnGUI(this, materialProperties);
					}
					else
					{
						this.PropertiesDefaultGUI(materialProperties);
					}
					Renderer associatedRenderFromInspector = MaterialEditor.GetAssociatedRenderFromInspector();
					if (associatedRenderFromInspector != null)
					{
						if (Event.current.type == EventType.Layout)
						{
							associatedRenderFromInspector.GetPropertyBlock(this.m_PropertyBlock);
						}
						if (this.m_PropertyBlock != null && !this.m_PropertyBlock.isEmpty)
						{
							EditorGUILayout.HelpBox(MaterialEditor.Styles.propBlockWarning, MessageType.Warning);
						}
					}
				}
				catch (Exception)
				{
					GUI.enabled = enabled;
					this.m_InsidePropertiesGUI = false;
					this.m_RendererForAnimationMode = null;
					throw;
				}
				GUI.enabled = enabled;
				this.m_InsidePropertiesGUI = false;
				this.m_RendererForAnimationMode = null;
				result = EditorGUI.EndChangeCheck();
			}
			return result;
		}

		public void PropertiesDefaultGUI(MaterialProperty[] props)
		{
			this.SetDefaultGUIWidths();
			if (this.m_InfoMessage != null)
			{
				EditorGUILayout.HelpBox(this.m_InfoMessage, MessageType.Info);
			}
			else
			{
				GUIUtility.GetControlID(MaterialEditor.s_ControlHash, FocusType.Passive, new Rect(0f, 0f, 0f, 0f));
			}
			for (int i = 0; i < props.Length; i++)
			{
				if ((props[i].flags & (MaterialProperty.PropFlags.HideInInspector | MaterialProperty.PropFlags.PerRendererData)) == MaterialProperty.PropFlags.None)
				{
					float propertyHeight = this.GetPropertyHeight(props[i], props[i].displayName);
					Rect controlRect = EditorGUILayout.GetControlRect(true, propertyHeight, EditorStyles.layerMaskField, new GUILayoutOption[0]);
					this.ShaderProperty(controlRect, props[i], props[i].displayName);
				}
			}
			this.RenderQueueField();
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
			if (targets != null && targets.Length != 0)
			{
				Material material = targets[0] as Material;
				if (!(material == null))
				{
					Shader shader = material.shader;
					MaterialProperty[] materialProperties = MaterialEditor.GetMaterialProperties(targets);
					for (int i = 0; i < materialProperties.Length; i++)
					{
						MaterialPropertyHandler handler = MaterialPropertyHandler.GetHandler(shader, materialProperties[i].name);
						if (handler != null && handler.propertyDrawer != null)
						{
							handler.propertyDrawer.Apply(materialProperties[i]);
						}
					}
				}
			}
		}

		public void RegisterPropertyChangeUndo(string label)
		{
			Undo.RecordObjects(base.targets, "Modify " + label + " of " + this.targetTitle);
		}

		private UnityEngine.Object TextureValidator(UnityEngine.Object[] references, Type objType, SerializedProperty property)
		{
			UnityEngine.Object result;
			for (int i = 0; i < references.Length; i++)
			{
				UnityEngine.Object @object = references[i];
				Texture texture = @object as Texture;
				if (texture)
				{
					if (texture.dimension == this.m_DesiredTexdim || this.m_DesiredTexdim == TextureDimension.Any)
					{
						result = texture;
						return result;
					}
				}
			}
			result = null;
			return result;
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
				IEnumerator enumerator = gameObject.transform.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						Transform transform = (Transform)enumerator.Current;
						MeshFilter component = transform.GetComponent<MeshFilter>();
						string name = transform.name;
						if (name == null)
						{
							goto IL_11A;
						}
						if (!(name == "sphere"))
						{
							if (!(name == "cube"))
							{
								if (!(name == "cylinder"))
								{
									if (!(name == "torus"))
									{
										goto IL_11A;
									}
									MaterialEditor.s_Meshes[3] = component.sharedMesh;
								}
								else
								{
									MaterialEditor.s_Meshes[2] = component.sharedMesh;
								}
							}
							else
							{
								MaterialEditor.s_Meshes[1] = component.sharedMesh;
							}
						}
						else
						{
							MaterialEditor.s_Meshes[0] = component.sharedMesh;
						}
						continue;
						IL_11A:
						Debug.Log("Something is wrong, weird object found: " + transform.name);
					}
				}
				finally
				{
					IDisposable disposable;
					if ((disposable = (enumerator as IDisposable)) != null)
					{
						disposable.Dispose();
					}
				}
				MaterialEditor.s_MeshIcons[0] = EditorGUIUtility.IconContent("PreMatSphere");
				MaterialEditor.s_MeshIcons[1] = EditorGUIUtility.IconContent("PreMatCube");
				MaterialEditor.s_MeshIcons[2] = EditorGUIUtility.IconContent("PreMatCylinder");
				MaterialEditor.s_MeshIcons[3] = EditorGUIUtility.IconContent("PreMatTorus");
				MaterialEditor.s_MeshIcons[4] = EditorGUIUtility.IconContent("PreMatQuad");
				MaterialEditor.s_LightIcons[0] = EditorGUIUtility.IconContent("PreMatLight0");
				MaterialEditor.s_LightIcons[1] = EditorGUIUtility.IconContent("PreMatLight1");
				MaterialEditor.s_TimeIcons[0] = EditorGUIUtility.IconContent("PlayButton");
				MaterialEditor.s_TimeIcons[1] = EditorGUIUtility.IconContent("PauseButton");
				Mesh mesh = Resources.GetBuiltinResource(typeof(Mesh), "Quad.fbx") as Mesh;
				MaterialEditor.s_Meshes[4] = mesh;
				MaterialEditor.s_PlaneMesh = mesh;
			}
		}

		public override void OnPreviewSettings()
		{
			if (this.m_CustomShaderGUI != null)
			{
				this.m_CustomShaderGUI.OnMaterialPreviewSettingsGUI(this);
			}
			else
			{
				this.DefaultPreviewSettingsGUI();
			}
		}

		private bool PreviewSettingsMenuButton(out Rect buttonRect)
		{
			buttonRect = GUILayoutUtility.GetRect(14f, 24f, 14f, 20f);
			Rect position = new Rect(buttonRect.x + (buttonRect.width - 16f) / 2f, buttonRect.y + (buttonRect.height - 6f) / 2f, 16f, 6f);
			if (Event.current.type == EventType.Repaint)
			{
				MaterialEditor.Styles.kReflectionProbePickerStyle.Draw(position, false, false, false, false);
			}
			return EditorGUI.ButtonMouseDown(buttonRect, GUIContent.none, FocusType.Passive, GUIStyle.none);
		}

		public void DefaultPreviewSettingsGUI()
		{
			if (ShaderUtil.hardwareSupportsRectRenderTexture)
			{
				this.Init();
				Material mat = base.target as Material;
				MaterialEditor.PreviewType previewType = MaterialEditor.GetPreviewType(mat);
				if (base.targets.Length > 1 || previewType == MaterialEditor.PreviewType.Mesh)
				{
					this.m_TimeUpdate = PreviewGUI.CycleButton(this.m_TimeUpdate, MaterialEditor.s_TimeIcons);
					this.m_SelectedMesh = PreviewGUI.CycleButton(this.m_SelectedMesh, MaterialEditor.s_MeshIcons);
					this.m_LightMode = PreviewGUI.CycleButton(this.m_LightMode, MaterialEditor.s_LightIcons);
					Rect activatorRect;
					if (this.PreviewSettingsMenuButton(out activatorRect))
					{
						PopupWindow.Show(activatorRect, this.m_ReflectionProbePicker);
					}
				}
			}
		}

		public sealed override Texture2D RenderStaticPreview(string assetPath, UnityEngine.Object[] subAssets, int width, int height)
		{
			Texture2D result;
			if (!ShaderUtil.hardwareSupportsRectRenderTexture)
			{
				result = null;
			}
			else
			{
				this.Init();
				this.m_PreviewUtility.BeginStaticPreview(new Rect(0f, 0f, (float)width, (float)height));
				this.DoRenderPreview();
				result = this.m_PreviewUtility.EndStaticPreview();
			}
			return result;
		}

		private void DoRenderPreview()
		{
			if (this.m_PreviewUtility.m_RenderTexture.width > 0 && this.m_PreviewUtility.m_RenderTexture.height > 0)
			{
				Material mat = base.target as Material;
				MaterialEditor.PreviewType previewType = MaterialEditor.GetPreviewType(mat);
				this.m_PreviewUtility.m_Camera.transform.position = -Vector3.forward * 5f;
				this.m_PreviewUtility.m_Camera.transform.rotation = Quaternion.identity;
				Color ambient;
				if (this.m_LightMode == 0)
				{
					this.m_PreviewUtility.m_Light[0].intensity = 1f;
					this.m_PreviewUtility.m_Light[0].transform.rotation = Quaternion.Euler(30f, 30f, 0f);
					this.m_PreviewUtility.m_Light[1].intensity = 0f;
					ambient = new Color(0.2f, 0.2f, 0.2f, 0f);
				}
				else
				{
					this.m_PreviewUtility.m_Light[0].intensity = 1f;
					this.m_PreviewUtility.m_Light[0].transform.rotation = Quaternion.Euler(50f, 50f, 0f);
					this.m_PreviewUtility.m_Light[1].intensity = 1f;
					ambient = new Color(0.2f, 0.2f, 0.2f, 0f);
				}
				InternalEditorUtility.SetCustomLighting(this.m_PreviewUtility.m_Light, ambient);
				Quaternion quaternion = Quaternion.identity;
				if (MaterialEditor.DoesPreviewAllowRotation(previewType))
				{
					quaternion = Quaternion.Euler(this.m_PreviewDir.y, 0f, 0f) * Quaternion.Euler(0f, this.m_PreviewDir.x, 0f);
				}
				Mesh mesh = MaterialEditor.s_Meshes[this.m_SelectedMesh];
				if (previewType != MaterialEditor.PreviewType.Plane)
				{
					if (previewType != MaterialEditor.PreviewType.Mesh)
					{
						if (previewType == MaterialEditor.PreviewType.Skybox)
						{
							mesh = null;
							this.m_PreviewUtility.m_Camera.transform.rotation = Quaternion.Inverse(quaternion);
							this.m_PreviewUtility.m_Camera.fieldOfView = 120f;
						}
					}
					else
					{
						this.m_PreviewUtility.m_Camera.transform.position = Quaternion.Inverse(quaternion) * this.m_PreviewUtility.m_Camera.transform.position;
						this.m_PreviewUtility.m_Camera.transform.LookAt(Vector3.zero);
						quaternion = Quaternion.identity;
					}
				}
				else
				{
					mesh = MaterialEditor.s_PlaneMesh;
				}
				if (mesh != null)
				{
					this.m_PreviewUtility.DrawMesh(mesh, Vector3.zero, quaternion, mat, 0, null, this.m_ReflectionProbePicker.Target);
				}
				bool fog = RenderSettings.fog;
				Unsupported.SetRenderSettingsUseFogNoDirty(false);
				this.m_PreviewUtility.m_Camera.Render();
				if (previewType == MaterialEditor.PreviewType.Skybox)
				{
					GL.sRGBWrite = (QualitySettings.activeColorSpace == ColorSpace.Linear);
					InternalEditorUtility.DrawSkyboxMaterial(mat, this.m_PreviewUtility.m_Camera);
					GL.sRGBWrite = false;
				}
				Unsupported.SetRenderSettingsUseFogNoDirty(fog);
				InternalEditorUtility.RemoveCustomLighting();
			}
		}

		public sealed override bool HasPreviewGUI()
		{
			return true;
		}

		public override bool RequiresConstantRepaint()
		{
			return this.m_TimeUpdate == 1;
		}

		public override void OnInteractivePreviewGUI(Rect r, GUIStyle background)
		{
			if (this.m_CustomShaderGUI != null)
			{
				this.m_CustomShaderGUI.OnMaterialInteractivePreviewGUI(this, r, background);
			}
			else
			{
				base.OnInteractivePreviewGUI(r, background);
			}
		}

		public override void OnPreviewGUI(Rect r, GUIStyle background)
		{
			if (this.m_CustomShaderGUI != null)
			{
				this.m_CustomShaderGUI.OnMaterialPreviewGUI(this, r, background);
			}
			else
			{
				this.DefaultPreviewGUI(r, background);
			}
		}

		public void DefaultPreviewGUI(Rect r, GUIStyle background)
		{
			if (!ShaderUtil.hardwareSupportsRectRenderTexture)
			{
				if (Event.current.type == EventType.Repaint)
				{
					EditorGUI.DropShadowLabel(new Rect(r.x, r.y, r.width, 40f), "Material preview \nnot available");
				}
			}
			else
			{
				this.Init();
				Material mat = base.target as Material;
				MaterialEditor.PreviewType previewType = MaterialEditor.GetPreviewType(mat);
				if (MaterialEditor.DoesPreviewAllowRotation(previewType))
				{
					this.m_PreviewDir = PreviewGUI.Drag2D(this.m_PreviewDir, r);
				}
				if (Event.current.type == EventType.Repaint)
				{
					this.m_PreviewUtility.BeginPreview(r, background);
					this.DoRenderPreview();
					this.m_PreviewUtility.EndAndDrawPreview(r);
				}
			}
		}

		public virtual void OnEnable()
		{
			this.m_Shader = (base.serializedObject.FindProperty("m_Shader").objectReferenceValue as Shader);
			this.CreateCustomShaderGUI(this.m_Shader, "");
			Undo.undoRedoPerformed = (Undo.UndoRedoCallback)Delegate.Combine(Undo.undoRedoPerformed, new Undo.UndoRedoCallback(this.UndoRedoPerformed));
			this.PropertiesChanged();
			this.m_PropertyBlock = new MaterialPropertyBlock();
			this.m_ReflectionProbePicker.OnEnable();
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
			this.m_ReflectionProbePicker.OnDisable();
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
			if (current.type != EventType.Repaint)
			{
				int materialIndex = -1;
				GameObject gameObject = HandleUtility.PickGameObject(current.mousePosition, out materialIndex);
				if (EditorMaterialUtility.IsBackgroundMaterial(base.target as Material))
				{
					this.HandleSkybox(gameObject, current);
				}
				else if (gameObject && gameObject.GetComponent<Renderer>())
				{
					this.HandleRenderer(gameObject.GetComponent<Renderer>(), materialIndex, current);
				}
			}
		}

		internal void HandleSkybox(GameObject go, Event evt)
		{
			bool flag = !go;
			bool flag2 = false;
			if (!flag || evt.type == EventType.DragExited)
			{
				evt.Use();
			}
			else
			{
				EventType type = evt.type;
				if (type != EventType.DragUpdated)
				{
					if (type == EventType.DragPerform)
					{
						DragAndDrop.AcceptDrag();
						flag2 = true;
					}
				}
				else
				{
					DragAndDrop.visualMode = DragAndDropVisualMode.Link;
					flag2 = true;
				}
			}
			if (flag2)
			{
				Undo.RecordObject(UnityEngine.Object.FindObjectOfType<RenderSettings>(), "Assign Skybox Material");
				RenderSettings.skybox = (base.target as Material);
				evt.Use();
			}
		}

		internal void HandleRenderer(Renderer r, int materialIndex, Event evt)
		{
			bool flag = false;
			EventType type = evt.type;
			if (type != EventType.DragUpdated)
			{
				if (type == EventType.DragPerform)
				{
					DragAndDrop.AcceptDrag();
					flag = true;
				}
			}
			else
			{
				DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
				flag = true;
			}
			if (flag)
			{
				Undo.RecordObject(r, "Assign Material");
				Material[] sharedMaterials = r.sharedMaterials;
				bool alt = evt.alt;
				bool flag2 = materialIndex >= 0 && materialIndex < r.sharedMaterials.Length;
				if (!alt && flag2)
				{
					sharedMaterials[materialIndex] = (base.target as Material);
				}
				else
				{
					for (int i = 0; i < sharedMaterials.Length; i++)
					{
						sharedMaterials[i] = (base.target as Material);
					}
				}
				r.sharedMaterials = sharedMaterials;
				evt.Use();
			}
		}

		private bool HasMultipleMixedQueueValues()
		{
			int materialRawRenderQueue = ShaderUtil.GetMaterialRawRenderQueue(base.targets[0] as Material);
			bool result;
			for (int i = 1; i < base.targets.Length; i++)
			{
				if (materialRawRenderQueue != ShaderUtil.GetMaterialRawRenderQueue(base.targets[i] as Material))
				{
					result = true;
					return result;
				}
			}
			result = false;
			return result;
		}

		public void RenderQueueField()
		{
			EditorGUILayout.Space();
			EditorGUILayout.Space();
			Rect controlRectForSingleLine = this.GetControlRectForSingleLine();
			this.RenderQueueField(controlRectForSingleLine);
		}

		public void RenderQueueField(Rect r)
		{
			bool showMixedValue = this.HasMultipleMixedQueueValues();
			EditorGUI.showMixedValue = showMixedValue;
			float labelWidth = EditorGUIUtility.labelWidth;
			float fieldWidth = EditorGUIUtility.fieldWidth;
			this.SetDefaultGUIWidths();
			EditorGUIUtility.labelWidth -= 100f;
			Rect position = r;
			position.width -= EditorGUIUtility.fieldWidth + 2f;
			Rect position2 = r;
			position2.xMin = position2.xMax - EditorGUIUtility.fieldWidth;
			Material material = base.targets[0] as Material;
			int materialRawRenderQueue = ShaderUtil.GetMaterialRawRenderQueue(material);
			int renderQueue = material.renderQueue;
			bool flag = Array.IndexOf<int>(MaterialEditor.Styles.queueValues, materialRawRenderQueue) < 0;
			int num = (!flag) ? materialRawRenderQueue : MaterialEditor.Styles.kCustomQueueValue;
			int num2 = EditorGUI.IntPopup(position, MaterialEditor.Styles.queueLabel, num, MaterialEditor.Styles.queueNames, MaterialEditor.Styles.queueValues);
			int num3 = EditorGUI.DelayedIntField(position2, renderQueue);
			if (num != num2 || renderQueue != num3)
			{
				this.RegisterPropertyChangeUndo("Render Queue");
				int num4 = num3;
				if (num2 != num && num2 != MaterialEditor.Styles.kCustomQueueValue)
				{
					num4 = num2;
				}
				num4 = Mathf.Clamp(num4, -1, 5000);
				UnityEngine.Object[] targets = base.targets;
				for (int i = 0; i < targets.Length; i++)
				{
					UnityEngine.Object @object = targets[i];
					((Material)@object).renderQueue = num4;
				}
			}
			EditorGUIUtility.labelWidth = labelWidth;
			EditorGUIUtility.fieldWidth = fieldWidth;
			EditorGUI.showMixedValue = false;
		}

		public Rect TexturePropertySingleLine(GUIContent label, MaterialProperty textureProp)
		{
			return this.TexturePropertySingleLine(label, textureProp, null, null);
		}

		public Rect TexturePropertySingleLine(GUIContent label, MaterialProperty textureProp, MaterialProperty extraProperty1)
		{
			return this.TexturePropertySingleLine(label, textureProp, extraProperty1, null);
		}

		public Rect TexturePropertySingleLine(GUIContent label, MaterialProperty textureProp, MaterialProperty extraProperty1, MaterialProperty extraProperty2)
		{
			Rect controlRectForSingleLine = this.GetControlRectForSingleLine();
			this.TexturePropertyMiniThumbnail(controlRectForSingleLine, textureProp, label.text, label.tooltip);
			Rect result;
			if (extraProperty1 == null && extraProperty2 == null)
			{
				result = controlRectForSingleLine;
			}
			else
			{
				if (extraProperty1 == null || extraProperty2 == null)
				{
					MaterialProperty materialProperty = extraProperty1 ?? extraProperty2;
					if (materialProperty.type == MaterialProperty.PropType.Color)
					{
						this.ExtraPropertyAfterTexture(MaterialEditor.GetLeftAlignedFieldRect(controlRectForSingleLine), materialProperty);
					}
					else
					{
						this.ExtraPropertyAfterTexture(MaterialEditor.GetRectAfterLabelWidth(controlRectForSingleLine), materialProperty);
					}
				}
				else if (extraProperty1.type == MaterialProperty.PropType.Color)
				{
					this.ExtraPropertyAfterTexture(MaterialEditor.GetFlexibleRectBetweenFieldAndRightEdge(controlRectForSingleLine), extraProperty2);
					this.ExtraPropertyAfterTexture(MaterialEditor.GetLeftAlignedFieldRect(controlRectForSingleLine), extraProperty1);
				}
				else
				{
					this.ExtraPropertyAfterTexture(MaterialEditor.GetRightAlignedFieldRect(controlRectForSingleLine), extraProperty2);
					this.ExtraPropertyAfterTexture(MaterialEditor.GetFlexibleRectBetweenLabelAndField(controlRectForSingleLine), extraProperty1);
				}
				result = controlRectForSingleLine;
			}
			return result;
		}

		public Rect TexturePropertyWithHDRColor(GUIContent label, MaterialProperty textureProp, MaterialProperty colorProperty, ColorPickerHDRConfig hdrConfig, bool showAlpha)
		{
			Rect controlRectForSingleLine = this.GetControlRectForSingleLine();
			this.TexturePropertyMiniThumbnail(controlRectForSingleLine, textureProp, label.text, label.tooltip);
			Rect result;
			if (colorProperty.type != MaterialProperty.PropType.Color)
			{
				Debug.LogError("Assuming MaterialProperty.PropType.Color (was " + colorProperty.type + ")");
				result = controlRectForSingleLine;
			}
			else
			{
				this.BeginAnimatedCheck(colorProperty);
				ColorPickerHDRConfig colorPickerHDRConfig = hdrConfig ?? ColorPicker.defaultHDRConfig;
				Rect leftAlignedFieldRect = MaterialEditor.GetLeftAlignedFieldRect(controlRectForSingleLine);
				EditorGUI.BeginChangeCheck();
				EditorGUI.showMixedValue = colorProperty.hasMixedValue;
				Color colorValue = EditorGUI.ColorField(leftAlignedFieldRect, GUIContent.none, colorProperty.colorValue, true, showAlpha, true, colorPickerHDRConfig);
				EditorGUI.showMixedValue = false;
				if (EditorGUI.EndChangeCheck())
				{
					colorProperty.colorValue = colorValue;
				}
				Rect flexibleRectBetweenFieldAndRightEdge = MaterialEditor.GetFlexibleRectBetweenFieldAndRightEdge(controlRectForSingleLine);
				float labelWidth = EditorGUIUtility.labelWidth;
				EditorGUIUtility.labelWidth = flexibleRectBetweenFieldAndRightEdge.width - EditorGUIUtility.fieldWidth;
				EditorGUI.BeginChangeCheck();
				colorValue = EditorGUI.ColorBrightnessField(flexibleRectBetweenFieldAndRightEdge, GUIContent.Temp(" "), colorProperty.colorValue, colorPickerHDRConfig.minBrightness, colorPickerHDRConfig.maxBrightness);
				if (EditorGUI.EndChangeCheck())
				{
					colorProperty.colorValue = colorValue;
				}
				EditorGUIUtility.labelWidth = labelWidth;
				this.EndAnimatedCheck();
				result = controlRectForSingleLine;
			}
			return result;
		}

		public Rect TexturePropertyTwoLines(GUIContent label, MaterialProperty textureProp, MaterialProperty extraProperty1, GUIContent label2, MaterialProperty extraProperty2)
		{
			Rect result;
			if (extraProperty2 == null)
			{
				result = this.TexturePropertySingleLine(label, textureProp, extraProperty1);
			}
			else
			{
				Rect controlRectForSingleLine = this.GetControlRectForSingleLine();
				this.TexturePropertyMiniThumbnail(controlRectForSingleLine, textureProp, label.text, label.tooltip);
				Rect r = MaterialEditor.GetRectAfterLabelWidth(controlRectForSingleLine);
				if (extraProperty1.type == MaterialProperty.PropType.Color)
				{
					r = MaterialEditor.GetLeftAlignedFieldRect(controlRectForSingleLine);
				}
				this.ExtraPropertyAfterTexture(r, extraProperty1);
				Rect controlRectForSingleLine2 = this.GetControlRectForSingleLine();
				this.ShaderProperty(controlRectForSingleLine2, extraProperty2, label2.text, 3);
				controlRectForSingleLine.height += controlRectForSingleLine2.height;
				result = controlRectForSingleLine;
			}
			return result;
		}

		private Rect GetControlRectForSingleLine()
		{
			return EditorGUILayout.GetControlRect(true, 18f, EditorStyles.layerMaskField, new GUILayoutOption[0]);
		}

		private void ExtraPropertyAfterTexture(Rect r, MaterialProperty property)
		{
			if ((property.type == MaterialProperty.PropType.Float || property.type == MaterialProperty.PropType.Color) && r.width > EditorGUIUtility.fieldWidth)
			{
				float labelWidth = EditorGUIUtility.labelWidth;
				EditorGUIUtility.labelWidth = r.width - EditorGUIUtility.fieldWidth;
				this.ShaderProperty(r, property, " ");
				EditorGUIUtility.labelWidth = labelWidth;
			}
			else
			{
				this.ShaderProperty(r, property, string.Empty);
			}
		}

		public static Rect GetRightAlignedFieldRect(Rect r)
		{
			return new Rect(r.xMax - EditorGUIUtility.fieldWidth, r.y, EditorGUIUtility.fieldWidth, EditorGUIUtility.singleLineHeight);
		}

		public static Rect GetLeftAlignedFieldRect(Rect r)
		{
			return new Rect(r.x + EditorGUIUtility.labelWidth, r.y, EditorGUIUtility.fieldWidth, EditorGUIUtility.singleLineHeight);
		}

		public static Rect GetFlexibleRectBetweenLabelAndField(Rect r)
		{
			return new Rect(r.x + EditorGUIUtility.labelWidth, r.y, r.width - EditorGUIUtility.labelWidth - EditorGUIUtility.fieldWidth - 5f, EditorGUIUtility.singleLineHeight);
		}

		public static Rect GetFlexibleRectBetweenFieldAndRightEdge(Rect r)
		{
			Rect rectAfterLabelWidth = MaterialEditor.GetRectAfterLabelWidth(r);
			rectAfterLabelWidth.xMin += EditorGUIUtility.fieldWidth + 5f;
			return rectAfterLabelWidth;
		}

		public static Rect GetRectAfterLabelWidth(Rect r)
		{
			return new Rect(r.x + EditorGUIUtility.labelWidth, r.y, r.width - EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight);
		}

		internal static Type GetTextureTypeFromDimension(TextureDimension dim)
		{
			Type result;
			switch (dim)
			{
			case TextureDimension.Any:
				result = typeof(Texture);
				break;
			case TextureDimension.Tex2D:
				result = typeof(Texture);
				break;
			case TextureDimension.Tex3D:
				result = typeof(Texture3D);
				break;
			case TextureDimension.Cube:
				result = typeof(Cubemap);
				break;
			case TextureDimension.Tex2DArray:
				result = typeof(Texture2DArray);
				break;
			case TextureDimension.CubeArray:
				result = typeof(CubemapArray);
				break;
			default:
				result = null;
				break;
			}
			return result;
		}
	}
}
