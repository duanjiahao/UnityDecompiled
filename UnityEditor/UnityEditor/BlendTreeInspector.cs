using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using UnityEditor.AnimatedValues;
using UnityEditor.Animations;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Events;

namespace UnityEditor
{
	[CustomEditor(typeof(UnityEditor.Animations.BlendTree))]
	internal class BlendTreeInspector : Editor
	{
		private class Styles
		{
			public readonly GUIStyle background = "MeBlendBackground";

			public readonly GUIStyle triangleLeft = "MeBlendTriangleLeft";

			public readonly GUIStyle triangleRight = "MeBlendTriangleRight";

			public readonly GUIStyle blendPosition = "MeBlendPosition";

			public GUIStyle clickDragFloatFieldLeft = new GUIStyle(EditorStyles.miniTextField);

			public GUIStyle clickDragFloatFieldRight = new GUIStyle(EditorStyles.miniTextField);

			public GUIStyle clickDragFloatLabelLeft = new GUIStyle(EditorStyles.miniLabel);

			public GUIStyle clickDragFloatLabelRight = new GUIStyle(EditorStyles.miniLabel);

			public GUIStyle headerIcon = new GUIStyle();

			public GUIStyle errorStyle = new GUIStyle(EditorStyles.wordWrappedLabel);

			public GUIContent speedIcon = new GUIContent(EditorGUIUtility.IconContent("SpeedScale"));

			public GUIContent mirrorIcon = new GUIContent(EditorGUIUtility.IconContent("Mirror"));

			public Texture2D pointIcon = EditorGUIUtility.LoadIcon("blendKey");

			public Texture2D pointIconSelected = EditorGUIUtility.LoadIcon("blendKeySelected");

			public Texture2D pointIconOverlay = EditorGUIUtility.LoadIcon("blendKeyOverlay");

			public Texture2D samplerIcon = EditorGUIUtility.LoadIcon("blendSampler");

			public Color visBgColor;

			public Color visWeightColor;

			public Color visWeightShapeColor;

			public Color visWeightLineColor;

			public Color visPointColor;

			public Color visPointEmptyColor;

			public Color visPointOverlayColor;

			public Color visSamplerColor;

			public Styles()
			{
				this.errorStyle.alignment = TextAnchor.MiddleCenter;
				this.speedIcon.tooltip = "Changes animation speed.";
				this.mirrorIcon.tooltip = "Mirror animation.";
				this.headerIcon.alignment = TextAnchor.MiddleCenter;
				this.clickDragFloatFieldLeft.alignment = TextAnchor.MiddleLeft;
				this.clickDragFloatFieldRight.alignment = TextAnchor.MiddleRight;
				this.clickDragFloatLabelLeft.alignment = TextAnchor.MiddleLeft;
				this.clickDragFloatLabelRight.alignment = TextAnchor.MiddleRight;
				this.visBgColor = (EditorGUIUtility.isProSkin ? new Color(0.2f, 0.2f, 0.2f) : new Color(0.95f, 0.95f, 1f));
				this.visWeightColor = (EditorGUIUtility.isProSkin ? new Color(0.65f, 0.75f, 1f, 0.65f) : new Color(0.5f, 0.6f, 0.9f, 0.8f));
				this.visWeightShapeColor = (EditorGUIUtility.isProSkin ? new Color(0.4f, 0.65f, 1f, 0.12f) : new Color(0.4f, 0.65f, 1f, 0.15f));
				this.visWeightLineColor = (EditorGUIUtility.isProSkin ? new Color(1f, 1f, 1f, 0.6f) : new Color(0f, 0f, 0f, 0.3f));
				this.visPointColor = new Color(0.5f, 0.7f, 1f);
				this.visPointEmptyColor = (EditorGUIUtility.isProSkin ? new Color(0.6f, 0.6f, 0.6f) : new Color(0.8f, 0.8f, 0.8f));
				this.visPointOverlayColor = (EditorGUIUtility.isProSkin ? new Color(1f, 1f, 1f, 0.4f) : new Color(0f, 0f, 0f, 0.2f));
				this.visSamplerColor = new Color(1f, 0.4f, 0.4f);
			}
		}

		private enum ChildPropertyToCompute
		{
			Threshold,
			PositionX,
			PositionY
		}

		private delegate float GetFloatFromMotion(Motion motion, float mirrorMultiplier);

		private static BlendTreeInspector.Styles styles;

		internal static UnityEditor.Animations.AnimatorController currentController = null;

		internal static Animator currentAnimator = null;

		internal static UnityEditor.Animations.BlendTree parentBlendTree = null;

		internal static Action<UnityEditor.Animations.BlendTree> blendParameterInputChanged = null;

		private readonly int m_BlendAnimationID = "BlendAnimationIDHash".GetHashCode();

		private readonly int m_ClickDragFloatID = "ClickDragFloatIDHash".GetHashCode();

		private float m_DragAndDropDelta;

		private float m_OriginMin;

		private float m_OriginMax;

		private ReorderableList m_ReorderableList;

		private SerializedProperty m_Childs;

		private SerializedProperty m_BlendParameter;

		private SerializedProperty m_BlendParameterY;

		private UnityEditor.Animations.BlendTree m_BlendTree;

		private SerializedProperty m_UseAutomaticThresholds;

		private SerializedProperty m_NormalizedBlendValues;

		private SerializedProperty m_MinThreshold;

		private SerializedProperty m_MaxThreshold;

		private SerializedProperty m_Name;

		private SerializedProperty m_BlendType;

		private AnimBool m_ShowGraph = new AnimBool();

		private AnimBool m_ShowCompute = new AnimBool();

		private AnimBool m_ShowAdjust = new AnimBool();

		private bool m_ShowGraphValue = false;

		private float[] m_Weights;

		private const int kVisResolution = 64;

		private Texture2D m_BlendTex = null;

		private List<Texture2D> m_WeightTexs = new List<Texture2D>();

		private string m_WarningMessage = null;

		private PreviewBlendTree m_PreviewBlendTree;

		private VisualizationBlendTree m_VisBlendTree;

		private GameObject m_VisInstance = null;

		private static bool s_ClickDragFloatDragged;

		private static float s_ClickDragFloatDistance;

		private Rect m_BlendRect;

		private int m_SelectedPoint = -1;

		private bool s_DraggingPoint = false;

		private int kNumCirclePoints = 20;

		private int ParameterCount
		{
			get
			{
				return (this.m_BlendType.intValue <= 0) ? 1 : ((this.m_BlendType.intValue >= 4) ? 0 : 2);
			}
		}

		internal static void SetParameterValue(Animator animator, UnityEditor.Animations.BlendTree blendTree, UnityEditor.Animations.BlendTree parentBlendTree, string parameterName, float parameterValue)
		{
			bool flag = EditorApplication.isPlaying && animator != null && animator.enabled && animator.gameObject.activeInHierarchy;
			if (flag)
			{
				animator.SetFloat(parameterName, parameterValue);
			}
			blendTree.SetInputBlendValue(parameterName, parameterValue);
			if (BlendTreeInspector.blendParameterInputChanged != null)
			{
				BlendTreeInspector.blendParameterInputChanged(blendTree);
			}
			if (parentBlendTree != null)
			{
				parentBlendTree.SetInputBlendValue(parameterName, parameterValue);
				if (BlendTreeInspector.blendParameterInputChanged != null)
				{
					BlendTreeInspector.blendParameterInputChanged(parentBlendTree);
				}
			}
		}

		internal static float GetParameterValue(Animator animator, UnityEditor.Animations.BlendTree blendTree, string parameterName)
		{
			bool flag = EditorApplication.isPlaying && animator != null && animator.enabled && animator.gameObject.activeInHierarchy;
			float result;
			if (flag)
			{
				result = animator.GetFloat(parameterName);
			}
			else
			{
				result = blendTree.GetInputBlendValue(parameterName);
			}
			return result;
		}

		public void OnEnable()
		{
			this.m_Name = base.serializedObject.FindProperty("m_Name");
			this.m_BlendParameter = base.serializedObject.FindProperty("m_BlendParameter");
			this.m_BlendParameterY = base.serializedObject.FindProperty("m_BlendParameterY");
			this.m_UseAutomaticThresholds = base.serializedObject.FindProperty("m_UseAutomaticThresholds");
			this.m_NormalizedBlendValues = base.serializedObject.FindProperty("m_NormalizedBlendValues");
			this.m_MinThreshold = base.serializedObject.FindProperty("m_MinThreshold");
			this.m_MaxThreshold = base.serializedObject.FindProperty("m_MaxThreshold");
			this.m_BlendType = base.serializedObject.FindProperty("m_BlendType");
		}

		private void Init()
		{
			if (BlendTreeInspector.styles == null)
			{
				BlendTreeInspector.styles = new BlendTreeInspector.Styles();
			}
			if (this.m_BlendTree == null)
			{
				this.m_BlendTree = (base.target as UnityEditor.Animations.BlendTree);
			}
			if (BlendTreeInspector.styles == null)
			{
				BlendTreeInspector.styles = new BlendTreeInspector.Styles();
			}
			if (this.m_PreviewBlendTree == null)
			{
				this.m_PreviewBlendTree = new PreviewBlendTree();
			}
			if (this.m_VisBlendTree == null)
			{
				this.m_VisBlendTree = new VisualizationBlendTree();
			}
			if (this.m_Childs == null)
			{
				this.m_Childs = base.serializedObject.FindProperty("m_Childs");
				this.m_ReorderableList = new ReorderableList(base.serializedObject, this.m_Childs);
				this.m_ReorderableList.drawHeaderCallback = new ReorderableList.HeaderCallbackDelegate(this.DrawHeader);
				this.m_ReorderableList.drawElementCallback = new ReorderableList.ElementCallbackDelegate(this.DrawChild);
				this.m_ReorderableList.onReorderCallback = new ReorderableList.ReorderCallbackDelegate(this.EndDragChild);
				this.m_ReorderableList.onAddDropdownCallback = new ReorderableList.AddDropdownCallbackDelegate(this.AddButton);
				this.m_ReorderableList.onRemoveCallback = new ReorderableList.RemoveCallbackDelegate(this.RemoveButton);
				if (this.m_BlendType.intValue == 0)
				{
					this.SortByThreshold();
				}
				this.m_ShowGraphValue = ((this.m_BlendType.intValue != 4) ? (this.m_Childs.arraySize >= 2) : (this.m_Childs.arraySize >= 1));
				this.m_ShowGraph.value = this.m_ShowGraphValue;
				this.m_ShowAdjust.value = this.AllMotions();
				this.m_ShowCompute.value = !this.m_UseAutomaticThresholds.boolValue;
				this.m_ShowGraph.valueChanged.AddListener(new UnityAction(base.Repaint));
				this.m_ShowAdjust.valueChanged.AddListener(new UnityAction(base.Repaint));
				this.m_ShowCompute.valueChanged.AddListener(new UnityAction(base.Repaint));
			}
			this.m_PreviewBlendTree.Init(this.m_BlendTree, BlendTreeInspector.currentAnimator);
			bool flag = false;
			if (this.m_VisInstance == null)
			{
				GameObject original = (GameObject)EditorGUIUtility.Load("Avatar/DefaultAvatar.fbx");
				this.m_VisInstance = EditorUtility.InstantiateForAnimatorPreview(original);
				Renderer[] componentsInChildren = this.m_VisInstance.GetComponentsInChildren<Renderer>();
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					Renderer renderer = componentsInChildren[i];
					renderer.enabled = false;
				}
				flag = true;
			}
			this.m_VisBlendTree.Init(this.m_BlendTree, this.m_VisInstance.GetComponent<Animator>());
			if (flag && (this.m_BlendType.intValue == 1 || this.m_BlendType.intValue == 2 || this.m_BlendType.intValue == 3))
			{
				this.UpdateBlendVisualization();
				this.ValidatePositions();
			}
		}

		internal override void OnHeaderIconGUI(Rect iconRect)
		{
			Texture2D miniThumbnail = AssetPreview.GetMiniThumbnail(base.target);
			GUI.Label(iconRect, miniThumbnail);
		}

		internal override void OnHeaderTitleGUI(Rect titleRect, string header)
		{
			base.serializedObject.Update();
			Rect position = titleRect;
			position.height = 16f;
			EditorGUI.BeginChangeCheck();
			EditorGUI.showMixedValue = this.m_Name.hasMultipleDifferentValues;
			string text = EditorGUI.DelayedTextField(position, this.m_Name.stringValue, EditorStyles.textField);
			EditorGUI.showMixedValue = false;
			if (EditorGUI.EndChangeCheck() && !string.IsNullOrEmpty(text))
			{
				UnityEngine.Object[] targets = base.targets;
				for (int i = 0; i < targets.Length; i++)
				{
					UnityEngine.Object obj = targets[i];
					ObjectNames.SetNameSmart(obj, text);
				}
			}
			base.serializedObject.ApplyModifiedProperties();
		}

		internal override void OnHeaderControlsGUI()
		{
			EditorGUIUtility.labelWidth = 80f;
			base.serializedObject.Update();
			EditorGUILayout.PropertyField(this.m_BlendType, new GUILayoutOption[0]);
			base.serializedObject.ApplyModifiedProperties();
		}

		private List<string> CollectParameters(UnityEditor.Animations.AnimatorController controller)
		{
			List<string> list = new List<string>();
			if (controller != null)
			{
				UnityEngine.AnimatorControllerParameter[] parameters = controller.parameters;
				for (int i = 0; i < parameters.Length; i++)
				{
					UnityEngine.AnimatorControllerParameter animatorControllerParameter = parameters[i];
					if (animatorControllerParameter.type == UnityEngine.AnimatorControllerParameterType.Float)
					{
						list.Add(animatorControllerParameter.name);
					}
				}
			}
			return list;
		}

		private void ParameterGUI()
		{
			EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
			if (this.ParameterCount > 1)
			{
				EditorGUILayout.PrefixLabel(EditorGUIUtility.TempContent("Parameters"));
			}
			else
			{
				EditorGUILayout.PrefixLabel(EditorGUIUtility.TempContent("Parameter"));
			}
			base.serializedObject.Update();
			string text = this.m_BlendTree.blendParameter;
			string text2 = this.m_BlendTree.blendParameterY;
			List<string> list = this.CollectParameters(BlendTreeInspector.currentController);
			EditorGUI.BeginChangeCheck();
			text = EditorGUILayout.DelayedTextFieldDropDown(text, list.ToArray());
			if (EditorGUI.EndChangeCheck())
			{
				this.m_BlendParameter.stringValue = text;
			}
			if (this.ParameterCount > 1)
			{
				EditorGUI.BeginChangeCheck();
				text2 = EditorGUILayout.TextFieldDropDown(text2, list.ToArray());
				if (EditorGUI.EndChangeCheck())
				{
					this.m_BlendParameterY.stringValue = text2;
				}
			}
			base.serializedObject.ApplyModifiedProperties();
			EditorGUILayout.EndHorizontal();
		}

		public override void OnInspectorGUI()
		{
			this.Init();
			base.serializedObject.Update();
			if (this.m_BlendType.intValue != 4)
			{
				this.ParameterGUI();
			}
			this.m_ShowGraphValue = ((this.m_BlendType.intValue != 4) ? (this.m_Childs.arraySize >= 2) : (this.m_Childs.arraySize >= 1));
			this.m_ShowGraph.target = this.m_ShowGraphValue;
			this.m_UseAutomaticThresholds = base.serializedObject.FindProperty("m_UseAutomaticThresholds");
			GUI.enabled = true;
			if (EditorGUILayout.BeginFadeGroup(this.m_ShowGraph.faded))
			{
				if (this.m_BlendType.intValue == 0)
				{
					this.BlendGraph(EditorGUILayout.GetControlRect(false, 40f, BlendTreeInspector.styles.background, new GUILayoutOption[0]));
					this.ThresholdValues();
				}
				else if (this.m_BlendType.intValue == 4)
				{
					for (int i = 0; i < this.m_BlendTree.recursiveBlendParameterCount; i++)
					{
						string recursiveBlendParameter = this.m_BlendTree.GetRecursiveBlendParameter(i);
						float recursiveBlendParameterMin = this.m_BlendTree.GetRecursiveBlendParameterMin(i);
						float recursiveBlendParameterMax = this.m_BlendTree.GetRecursiveBlendParameterMax(i);
						EditorGUI.BeginChangeCheck();
						float parameterValue = EditorGUILayout.Slider(recursiveBlendParameter, BlendTreeInspector.GetParameterValue(BlendTreeInspector.currentAnimator, this.m_BlendTree, recursiveBlendParameter), recursiveBlendParameterMin, recursiveBlendParameterMax, new GUILayoutOption[0]);
						if (EditorGUI.EndChangeCheck())
						{
							BlendTreeInspector.SetParameterValue(BlendTreeInspector.currentAnimator, this.m_BlendTree, BlendTreeInspector.parentBlendTree, recursiveBlendParameter, parameterValue);
						}
					}
				}
				else
				{
					GUILayout.Space(1f);
					GUILayout.BeginHorizontal(new GUILayoutOption[0]);
					GUILayout.FlexibleSpace();
					Rect aspectRect = GUILayoutUtility.GetAspectRect(1f, new GUILayoutOption[]
					{
						GUILayout.MaxWidth(235f)
					});
					GUI.Label(new Rect(aspectRect.x - 1f, aspectRect.y - 1f, aspectRect.width + 2f, aspectRect.height + 2f), GUIContent.none, EditorStyles.textField);
					GUI.BeginGroup(aspectRect);
					aspectRect.x = 0f;
					aspectRect.y = 0f;
					this.BlendGraph2D(aspectRect);
					GUI.EndGroup();
					GUILayout.FlexibleSpace();
					GUILayout.EndHorizontal();
				}
				GUILayout.Space(5f);
			}
			EditorGUILayout.EndFadeGroup();
			if (this.m_ReorderableList != null)
			{
				this.m_ReorderableList.DoLayoutList();
			}
			if (this.m_BlendType.intValue == 4)
			{
				EditorGUILayout.PropertyField(this.m_NormalizedBlendValues, EditorGUIUtility.TempContent("Normalized Blend Values"), new GUILayoutOption[0]);
			}
			if (this.m_ShowGraphValue)
			{
				GUILayout.Space(10f);
				this.AutoCompute();
			}
			base.serializedObject.ApplyModifiedProperties();
		}

		private void SetMinMaxThresholds()
		{
			float num = float.PositiveInfinity;
			float num2 = float.NegativeInfinity;
			for (int i = 0; i < this.m_Childs.arraySize; i++)
			{
				SerializedProperty arrayElementAtIndex = this.m_Childs.GetArrayElementAtIndex(i);
				SerializedProperty serializedProperty = arrayElementAtIndex.FindPropertyRelative("m_Threshold");
				num = ((serializedProperty.floatValue >= num) ? num : serializedProperty.floatValue);
				num2 = ((serializedProperty.floatValue <= num2) ? num2 : serializedProperty.floatValue);
			}
			this.m_MinThreshold.floatValue = ((this.m_Childs.arraySize <= 0) ? 0f : num);
			this.m_MaxThreshold.floatValue = ((this.m_Childs.arraySize <= 0) ? 1f : num2);
		}

		private void ThresholdValues()
		{
			Rect controlRect = EditorGUILayout.GetControlRect(new GUILayoutOption[0]);
			Rect position = controlRect;
			Rect position2 = controlRect;
			position.width /= 4f;
			position2.width /= 4f;
			position2.x = controlRect.x + controlRect.width - position2.width;
			float num = this.m_MinThreshold.floatValue;
			float num2 = this.m_MaxThreshold.floatValue;
			EditorGUI.BeginChangeCheck();
			num = this.ClickDragFloat(position, num);
			num2 = this.ClickDragFloat(position2, num2, true);
			if (EditorGUI.EndChangeCheck())
			{
				float num3 = Mathf.Min(num, num2);
				float num4 = Mathf.Max(num, num2);
				if (this.m_Childs.arraySize >= 2)
				{
					SerializedProperty arrayElementAtIndex = this.m_Childs.GetArrayElementAtIndex(0);
					SerializedProperty arrayElementAtIndex2 = this.m_Childs.GetArrayElementAtIndex(this.m_Childs.arraySize - 1);
					SerializedProperty serializedProperty = arrayElementAtIndex.FindPropertyRelative("m_Threshold");
					SerializedProperty serializedProperty2 = arrayElementAtIndex2.FindPropertyRelative("m_Threshold");
					float floatValue = serializedProperty.floatValue;
					float floatValue2 = serializedProperty2.floatValue;
					serializedProperty.floatValue = num3;
					serializedProperty2.floatValue = num4;
					if (!this.m_UseAutomaticThresholds.boolValue)
					{
						int arraySize = this.m_Childs.arraySize;
						for (int i = 1; i < arraySize - 1; i++)
						{
							SerializedProperty arrayElementAtIndex3 = this.m_Childs.GetArrayElementAtIndex(i);
							SerializedProperty serializedProperty3 = arrayElementAtIndex3.FindPropertyRelative("m_Threshold");
							float t = Mathf.InverseLerp(floatValue, floatValue2, serializedProperty3.floatValue);
							serializedProperty3.floatValue = Mathf.Lerp(num3, num4, t);
						}
					}
					float num5 = BlendTreeInspector.GetParameterValue(BlendTreeInspector.currentAnimator, this.m_BlendTree, this.m_BlendTree.blendParameter);
					num5 = Mathf.Clamp(num5, num3, num4);
					BlendTreeInspector.SetParameterValue(BlendTreeInspector.currentAnimator, this.m_BlendTree, BlendTreeInspector.parentBlendTree, this.m_BlendTree.blendParameter, num5);
				}
				this.m_MinThreshold.floatValue = num3;
				this.m_MaxThreshold.floatValue = num4;
			}
		}

		public float ClickDragFloat(Rect position, float value)
		{
			return this.ClickDragFloat(position, value, false);
		}

		public float ClickDragFloat(Rect position, float value, bool alignRight)
		{
			string allowedletters = "inftynaeINFTYNAE0123456789.,-";
			int controlID = GUIUtility.GetControlID(this.m_ClickDragFloatID, FocusType.Keyboard, position);
			Event current = Event.current;
			EventType type = current.type;
			if (type != EventType.MouseUp)
			{
				if (type != EventType.MouseDown)
				{
					if (type == EventType.MouseDrag)
					{
						if (GUIUtility.hotControl == controlID && !EditorGUIUtility.editingTextField)
						{
							BlendTreeInspector.s_ClickDragFloatDistance += Mathf.Abs(HandleUtility.niceMouseDelta);
							if (BlendTreeInspector.s_ClickDragFloatDistance >= 5f)
							{
								BlendTreeInspector.s_ClickDragFloatDragged = true;
								value += HandleUtility.niceMouseDelta * 0.03f;
								value = MathUtils.RoundBasedOnMinimumDifference(value, 0.03f);
								GUI.changed = true;
							}
							current.Use();
						}
					}
				}
				else if (GUIUtility.keyboardControl != controlID || !EditorGUIUtility.editingTextField)
				{
					if (position.Contains(current.mousePosition))
					{
						current.Use();
						BlendTreeInspector.s_ClickDragFloatDragged = false;
						BlendTreeInspector.s_ClickDragFloatDistance = 0f;
						GUIUtility.hotControl = controlID;
						GUIUtility.keyboardControl = controlID;
						EditorGUIUtility.editingTextField = false;
					}
				}
			}
			else if (GUIUtility.hotControl == controlID)
			{
				current.Use();
				if (position.Contains(current.mousePosition) && !BlendTreeInspector.s_ClickDragFloatDragged)
				{
					EditorGUIUtility.editingTextField = true;
				}
				else
				{
					GUIUtility.keyboardControl = 0;
					GUIUtility.hotControl = 0;
					BlendTreeInspector.s_ClickDragFloatDragged = false;
				}
			}
			GUIStyle style = (GUIUtility.keyboardControl != controlID || !EditorGUIUtility.editingTextField) ? ((!alignRight) ? BlendTreeInspector.styles.clickDragFloatLabelLeft : BlendTreeInspector.styles.clickDragFloatLabelRight) : ((!alignRight) ? BlendTreeInspector.styles.clickDragFloatFieldLeft : BlendTreeInspector.styles.clickDragFloatFieldRight);
			float result;
			if (GUIUtility.keyboardControl == controlID)
			{
				string text;
				if (!EditorGUI.s_RecycledEditor.IsEditingControl(controlID))
				{
					text = (EditorGUI.s_RecycledCurrentEditingString = value.ToString("g7"));
				}
				else
				{
					text = EditorGUI.s_RecycledCurrentEditingString;
					if (current.type == EventType.ValidateCommand && current.commandName == "UndoRedoPerformed")
					{
						text = value.ToString("g7");
					}
				}
				bool flag;
				text = EditorGUI.DoTextField(EditorGUI.s_RecycledEditor, controlID, position, text, style, allowedletters, out flag, false, false, false);
				if (flag)
				{
					GUI.changed = true;
					EditorGUI.s_RecycledCurrentEditingString = text;
					string a = text.ToLower();
					if (a == "inf" || a == "infinity")
					{
						value = float.PositiveInfinity;
					}
					else if (a == "-inf" || a == "-infinity")
					{
						value = float.NegativeInfinity;
					}
					else
					{
						text = text.Replace(',', '.');
						if (!float.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture.NumberFormat, out value))
						{
							EditorGUI.s_RecycledCurrentEditingFloat = 0.0;
							value = 0f;
							result = value;
							return result;
						}
						if (float.IsNaN(value))
						{
							value = 0f;
						}
						EditorGUI.s_RecycledCurrentEditingFloat = (double)value;
					}
				}
			}
			else
			{
				string text = value.ToString("g7");
				bool flag;
				text = EditorGUI.DoTextField(EditorGUI.s_RecycledEditor, controlID, position, text, style, allowedletters, out flag, false, false, false);
			}
			result = value;
			return result;
		}

		private void BlendGraph(Rect area)
		{
			area.xMin += 1f;
			area.xMax -= 1f;
			int controlID = GUIUtility.GetControlID(this.m_BlendAnimationID, FocusType.Passive);
			int arraySize = this.m_Childs.arraySize;
			float[] array = new float[arraySize];
			for (int i = 0; i < arraySize; i++)
			{
				SerializedProperty arrayElementAtIndex = this.m_Childs.GetArrayElementAtIndex(i);
				SerializedProperty serializedProperty = arrayElementAtIndex.FindPropertyRelative("m_Threshold");
				array[i] = serializedProperty.floatValue;
			}
			float num = Mathf.Min(array);
			float num2 = Mathf.Max(array);
			for (int j = 0; j < array.Length; j++)
			{
				array[j] = area.x + Mathf.InverseLerp(num, num2, array[j]) * area.width;
			}
			string blendParameter = this.m_BlendTree.blendParameter;
			float num3 = area.x + Mathf.InverseLerp(num, num2, BlendTreeInspector.GetParameterValue(BlendTreeInspector.currentAnimator, this.m_BlendTree, blendParameter)) * area.width;
			Rect position = new Rect(num3 - 4f, area.y, 9f, 42f);
			Event current = Event.current;
			switch (current.GetTypeForControl(controlID))
			{
			case EventType.MouseDown:
			{
				float num4 = 0f;
				if (position.Contains(current.mousePosition))
				{
					current.Use();
					GUIUtility.hotControl = controlID;
					this.m_ReorderableList.index = -1;
					num4 = BlendTreeInspector.GetParameterValue(BlendTreeInspector.currentAnimator, this.m_BlendTree, blendParameter);
				}
				else if (area.Contains(current.mousePosition))
				{
					current.Use();
					GUIUtility.hotControl = controlID;
					GUIUtility.keyboardControl = controlID;
					float x = current.mousePosition.x;
					float num5 = float.PositiveInfinity;
					for (int k = 0; k < array.Length; k++)
					{
						float num6 = (k != 0) ? array[k - 1] : array[k];
						float num7 = (k != array.Length - 1) ? array[k + 1] : array[k];
						if (Mathf.Abs(x - array[k]) < num5)
						{
							if (x < num7 && x > num6)
							{
								num5 = Mathf.Abs(x - array[k]);
								this.m_ReorderableList.index = k;
							}
						}
					}
					this.m_UseAutomaticThresholds.boolValue = false;
					SerializedProperty arrayElementAtIndex2 = this.m_Childs.GetArrayElementAtIndex(this.m_ReorderableList.index);
					SerializedProperty serializedProperty2 = arrayElementAtIndex2.FindPropertyRelative("m_Threshold");
					num4 = serializedProperty2.floatValue;
				}
				float num8 = (current.mousePosition.x - area.x) / area.width;
				num8 = Mathf.LerpUnclamped(num, num2, num8);
				this.m_DragAndDropDelta = num8 - num4;
				this.m_OriginMin = num;
				this.m_OriginMax = num2;
				break;
			}
			case EventType.MouseUp:
				if (GUIUtility.hotControl == controlID)
				{
					current.Use();
					GUIUtility.hotControl = 0;
					this.m_ReorderableList.index = -1;
				}
				break;
			case EventType.MouseDrag:
				if (GUIUtility.hotControl == controlID)
				{
					current.Use();
					float num9 = (current.mousePosition.x - area.x) / area.width;
					num9 = Mathf.LerpUnclamped(this.m_OriginMin, this.m_OriginMax, num9);
					float num10 = num9 - this.m_DragAndDropDelta;
					if (this.m_ReorderableList.index == -1)
					{
						num10 = Mathf.Clamp(num10, num, num2);
						BlendTreeInspector.SetParameterValue(BlendTreeInspector.currentAnimator, this.m_BlendTree, BlendTreeInspector.parentBlendTree, blendParameter, num10);
					}
					else
					{
						SerializedProperty arrayElementAtIndex3 = this.m_Childs.GetArrayElementAtIndex(this.m_ReorderableList.index);
						SerializedProperty serializedProperty3 = arrayElementAtIndex3.FindPropertyRelative("m_Threshold");
						SerializedProperty serializedProperty4 = (this.m_ReorderableList.index > 0) ? this.m_Childs.GetArrayElementAtIndex(this.m_ReorderableList.index - 1) : arrayElementAtIndex3;
						SerializedProperty serializedProperty5 = (this.m_ReorderableList.index != this.m_Childs.arraySize - 1) ? this.m_Childs.GetArrayElementAtIndex(this.m_ReorderableList.index + 1) : arrayElementAtIndex3;
						SerializedProperty serializedProperty6 = serializedProperty4.FindPropertyRelative("m_Threshold");
						SerializedProperty serializedProperty7 = serializedProperty5.FindPropertyRelative("m_Threshold");
						serializedProperty3.floatValue = num10;
						if (serializedProperty3.floatValue < serializedProperty6.floatValue && this.m_ReorderableList.index != 0)
						{
							this.m_Childs.MoveArrayElement(this.m_ReorderableList.index, this.m_ReorderableList.index - 1);
							this.m_ReorderableList.index--;
						}
						if (serializedProperty3.floatValue > serializedProperty7.floatValue && this.m_ReorderableList.index < this.m_Childs.arraySize - 1)
						{
							this.m_Childs.MoveArrayElement(this.m_ReorderableList.index, this.m_ReorderableList.index + 1);
							this.m_ReorderableList.index++;
						}
						float num11 = 3f * ((num2 - num) / area.width);
						if (serializedProperty3.floatValue - serializedProperty6.floatValue <= num11)
						{
							serializedProperty3.floatValue = serializedProperty6.floatValue;
						}
						else if (serializedProperty7.floatValue - serializedProperty3.floatValue <= num11)
						{
							serializedProperty3.floatValue = serializedProperty7.floatValue;
						}
						this.SetMinMaxThresholds();
					}
				}
				break;
			case EventType.Repaint:
				BlendTreeInspector.styles.background.Draw(area, GUIContent.none, false, false, false, false);
				if (this.m_Childs.arraySize >= 2)
				{
					for (int l = 0; l < array.Length; l++)
					{
						float min = (l != 0) ? array[l - 1] : array[l];
						float max = (l != array.Length - 1) ? array[l + 1] : array[l];
						bool selected = this.m_ReorderableList.index == l;
						this.DrawAnimation(array[l], min, max, selected, area);
					}
					Color color = Handles.color;
					Handles.color = new Color(0f, 0f, 0f, 0.25f);
					Handles.DrawLine(new Vector3(area.x, area.y + area.height, 0f), new Vector3(area.x + area.width, area.y + area.height, 0f));
					Handles.color = color;
					BlendTreeInspector.styles.blendPosition.Draw(position, GUIContent.none, false, false, false, false);
				}
				else
				{
					GUI.Label(area, EditorGUIUtility.TempContent("Please Add Motion Fields or Blend Trees"), BlendTreeInspector.styles.errorStyle);
				}
				break;
			}
		}

		private void UpdateBlendVisualization()
		{
			Vector2[] activeMotionPositions = this.GetActiveMotionPositions();
			if (this.m_BlendTex == null)
			{
				this.m_BlendTex = new Texture2D(64, 64, TextureFormat.RGBA32, false);
				this.m_BlendTex.hideFlags = HideFlags.HideAndDontSave;
				this.m_BlendTex.wrapMode = TextureWrapMode.Clamp;
			}
			while (this.m_WeightTexs.Count < activeMotionPositions.Length)
			{
				Texture2D texture2D = new Texture2D(64, 64, TextureFormat.RGBA32, false);
				texture2D.wrapMode = TextureWrapMode.Clamp;
				texture2D.hideFlags = HideFlags.HideAndDontSave;
				this.m_WeightTexs.Add(texture2D);
			}
			while (this.m_WeightTexs.Count > activeMotionPositions.Length)
			{
				UnityEngine.Object.DestroyImmediate(this.m_WeightTexs[this.m_WeightTexs.Count - 1]);
				this.m_WeightTexs.RemoveAt(this.m_WeightTexs.Count - 1);
			}
			if (GUIUtility.hotControl == 0)
			{
				this.m_BlendRect = this.Get2DBlendRect(this.GetMotionPositions());
			}
			this.m_VisBlendTree.Reset();
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			Texture2D[] array = this.m_WeightTexs.ToArray();
			if (GUIUtility.hotControl != 0 && this.m_ReorderableList.index >= 0)
			{
				int[] motionToActiveMotionIndices = this.GetMotionToActiveMotionIndices();
				for (int i = 0; i < array.Length; i++)
				{
					if (motionToActiveMotionIndices[this.m_ReorderableList.index] != i)
					{
						array[i] = null;
					}
				}
			}
			BlendTreePreviewUtility.CalculateBlendTexture(this.m_VisBlendTree.animator, 0, this.m_VisBlendTree.animator.GetCurrentAnimatorStateInfo(0).fullPathHash, this.m_BlendTex, array, this.m_BlendRect);
			stopwatch.Stop();
		}

		private Vector2[] GetMotionPositions()
		{
			int arraySize = this.m_Childs.arraySize;
			Vector2[] array = new Vector2[arraySize];
			for (int i = 0; i < arraySize; i++)
			{
				SerializedProperty arrayElementAtIndex = this.m_Childs.GetArrayElementAtIndex(i);
				SerializedProperty serializedProperty = arrayElementAtIndex.FindPropertyRelative("m_Position");
				array[i] = serializedProperty.vector2Value;
			}
			return array;
		}

		private Vector2[] GetActiveMotionPositions()
		{
			List<Vector2> list = new List<Vector2>();
			int arraySize = this.m_Childs.arraySize;
			for (int i = 0; i < arraySize; i++)
			{
				SerializedProperty arrayElementAtIndex = this.m_Childs.GetArrayElementAtIndex(i);
				SerializedProperty serializedProperty = arrayElementAtIndex.FindPropertyRelative("m_Motion");
				if (serializedProperty.objectReferenceValue != null)
				{
					SerializedProperty serializedProperty2 = arrayElementAtIndex.FindPropertyRelative("m_Position");
					list.Add(serializedProperty2.vector2Value);
				}
			}
			return list.ToArray();
		}

		private int[] GetMotionToActiveMotionIndices()
		{
			int arraySize = this.m_Childs.arraySize;
			int[] array = new int[arraySize];
			int num = 0;
			for (int i = 0; i < arraySize; i++)
			{
				SerializedProperty arrayElementAtIndex = this.m_Childs.GetArrayElementAtIndex(i);
				SerializedProperty serializedProperty = arrayElementAtIndex.FindPropertyRelative("m_Motion");
				if (serializedProperty.objectReferenceValue == null)
				{
					array[i] = -1;
				}
				else
				{
					array[i] = num;
					num++;
				}
			}
			return array;
		}

		private Rect Get2DBlendRect(Vector2[] points)
		{
			Vector2 vector = Vector2.zero;
			float num = 0f;
			Rect result;
			if (points.Length == 0)
			{
				result = default(Rect);
			}
			else
			{
				if (this.m_BlendType.intValue == 3)
				{
					Vector2 a = points[0];
					Vector2 b = points[0];
					for (int i = 1; i < points.Length; i++)
					{
						b.x = Mathf.Max(b.x, points[i].x);
						b.y = Mathf.Max(b.y, points[i].y);
						a.x = Mathf.Min(a.x, points[i].x);
						a.y = Mathf.Min(a.y, points[i].y);
					}
					vector = (a + b) * 0.5f;
					num = Mathf.Max(b.x - a.x, b.y - a.y) * 0.5f;
				}
				else
				{
					for (int j = 0; j < points.Length; j++)
					{
						num = Mathf.Max(num, points[j].x);
						num = Mathf.Max(num, -points[j].x);
						num = Mathf.Max(num, points[j].y);
						num = Mathf.Max(num, -points[j].y);
					}
				}
				if (num == 0f)
				{
					num = 1f;
				}
				num *= 1.35f;
				result = new Rect(vector.x - num, vector.y - num, num * 2f, num * 2f);
			}
			return result;
		}

		private float ConvertFloat(float input, float fromMin, float fromMax, float toMin, float toMax)
		{
			float num = (input - fromMin) / (fromMax - fromMin);
			return toMin * (1f - num) + toMax * num;
		}

		private void BlendGraph2D(Rect area)
		{
			if (this.m_VisBlendTree.controllerDirty)
			{
				this.UpdateBlendVisualization();
				this.ValidatePositions();
			}
			Vector2[] motionPositions = this.GetMotionPositions();
			int[] motionToActiveMotionIndices = this.GetMotionToActiveMotionIndices();
			Vector2 vector = new Vector2(this.m_BlendRect.xMin, this.m_BlendRect.yMin);
			Vector2 vector2 = new Vector2(this.m_BlendRect.xMax, this.m_BlendRect.yMax);
			for (int i = 0; i < motionPositions.Length; i++)
			{
				motionPositions[i].x = this.ConvertFloat(motionPositions[i].x, vector.x, vector2.x, area.xMin, area.xMax);
				motionPositions[i].y = this.ConvertFloat(motionPositions[i].y, vector.y, vector2.y, area.yMax, area.yMin);
			}
			string blendParameter = this.m_BlendTree.blendParameter;
			string blendParameterY = this.m_BlendTree.blendParameterY;
			float num = BlendTreeInspector.GetParameterValue(BlendTreeInspector.currentAnimator, this.m_BlendTree, blendParameter);
			float num2 = BlendTreeInspector.GetParameterValue(BlendTreeInspector.currentAnimator, this.m_BlendTree, blendParameterY);
			int num3 = this.GetActiveMotionPositions().Length;
			if (this.m_Weights == null || num3 != this.m_Weights.Length)
			{
				this.m_Weights = new float[num3];
			}
			BlendTreePreviewUtility.CalculateRootBlendTreeChildWeights(this.m_VisBlendTree.animator, 0, this.m_VisBlendTree.animator.GetCurrentAnimatorStateInfo(0).fullPathHash, this.m_Weights, num, num2);
			num = area.x + Mathf.InverseLerp(vector.x, vector2.x, num) * area.width;
			num2 = area.y + (1f - Mathf.InverseLerp(vector.y, vector2.y, num2)) * area.height;
			Rect position = new Rect(num - 5f, num2 - 5f, 11f, 11f);
			int controlID = GUIUtility.GetControlID(this.m_BlendAnimationID, FocusType.Passive);
			Event current = Event.current;
			switch (current.GetTypeForControl(controlID))
			{
			case EventType.MouseDown:
				if (position.Contains(current.mousePosition))
				{
					current.Use();
					GUIUtility.hotControl = controlID;
					this.m_SelectedPoint = -1;
				}
				else if (area.Contains(current.mousePosition))
				{
					this.m_ReorderableList.index = -1;
					for (int j = 0; j < motionPositions.Length; j++)
					{
						Rect rect = new Rect(motionPositions[j].x - 4f, motionPositions[j].y - 4f, 9f, 9f);
						if (rect.Contains(current.mousePosition))
						{
							current.Use();
							GUIUtility.hotControl = controlID;
							this.m_SelectedPoint = j;
							this.m_ReorderableList.index = j;
						}
					}
					current.Use();
				}
				break;
			case EventType.MouseUp:
				if (GUIUtility.hotControl == controlID)
				{
					current.Use();
					GUIUtility.hotControl = 0;
					this.s_DraggingPoint = false;
				}
				break;
			case EventType.MouseDrag:
				if (GUIUtility.hotControl == controlID)
				{
					if (this.m_SelectedPoint == -1)
					{
						Vector2 vector3;
						vector3.x = this.ConvertFloat(current.mousePosition.x, area.xMin, area.xMax, vector.x, vector2.x);
						vector3.y = this.ConvertFloat(current.mousePosition.y, area.yMax, area.yMin, vector.y, vector2.y);
						BlendTreeInspector.SetParameterValue(BlendTreeInspector.currentAnimator, this.m_BlendTree, BlendTreeInspector.parentBlendTree, blendParameter, vector3.x);
						BlendTreeInspector.SetParameterValue(BlendTreeInspector.currentAnimator, this.m_BlendTree, BlendTreeInspector.parentBlendTree, blendParameterY, vector3.y);
						current.Use();
					}
					else
					{
						for (int k = 0; k < motionPositions.Length; k++)
						{
							if (this.m_SelectedPoint == k)
							{
								Vector2 vector2Value;
								vector2Value.x = this.ConvertFloat(current.mousePosition.x, area.xMin, area.xMax, vector.x, vector2.x);
								vector2Value.y = this.ConvertFloat(current.mousePosition.y, area.yMax, area.yMin, vector.y, vector2.y);
								float minDifference = (vector2.x - vector.x) / area.width;
								vector2Value.x = MathUtils.RoundBasedOnMinimumDifference(vector2Value.x, minDifference);
								vector2Value.y = MathUtils.RoundBasedOnMinimumDifference(vector2Value.y, minDifference);
								vector2Value.x = Mathf.Clamp(vector2Value.x, -10000f, 10000f);
								vector2Value.y = Mathf.Clamp(vector2Value.y, -10000f, 10000f);
								SerializedProperty arrayElementAtIndex = this.m_Childs.GetArrayElementAtIndex(k);
								SerializedProperty serializedProperty = arrayElementAtIndex.FindPropertyRelative("m_Position");
								serializedProperty.vector2Value = vector2Value;
								current.Use();
								this.s_DraggingPoint = true;
							}
						}
					}
				}
				break;
			case EventType.Repaint:
				GUI.color = BlendTreeInspector.styles.visBgColor;
				GUI.DrawTexture(area, EditorGUIUtility.whiteTexture);
				if (this.m_ReorderableList.index < 0)
				{
					Color visWeightColor = BlendTreeInspector.styles.visWeightColor;
					visWeightColor.a *= 0.75f;
					GUI.color = visWeightColor;
					GUI.DrawTexture(area, this.m_BlendTex);
				}
				else if (motionToActiveMotionIndices[this.m_ReorderableList.index] >= 0)
				{
					GUI.color = BlendTreeInspector.styles.visWeightColor;
					GUI.DrawTexture(area, this.m_WeightTexs[motionToActiveMotionIndices[this.m_ReorderableList.index]]);
				}
				GUI.color = Color.white;
				if (!this.s_DraggingPoint)
				{
					for (int l = 0; l < motionPositions.Length; l++)
					{
						if (motionToActiveMotionIndices[l] >= 0)
						{
							this.DrawWeightShape(motionPositions[l], this.m_Weights[motionToActiveMotionIndices[l]], 0);
						}
					}
					for (int m = 0; m < motionPositions.Length; m++)
					{
						if (motionToActiveMotionIndices[m] >= 0)
						{
							this.DrawWeightShape(motionPositions[m], this.m_Weights[motionToActiveMotionIndices[m]], 1);
						}
					}
				}
				for (int n = 0; n < motionPositions.Length; n++)
				{
					Rect position2 = new Rect(motionPositions[n].x - 6f, motionPositions[n].y - 6f, 13f, 13f);
					bool flag = this.m_ReorderableList.index == n;
					if (motionToActiveMotionIndices[n] < 0)
					{
						GUI.color = BlendTreeInspector.styles.visPointEmptyColor;
					}
					else
					{
						GUI.color = BlendTreeInspector.styles.visPointColor;
					}
					GUI.DrawTexture(position2, (!flag) ? BlendTreeInspector.styles.pointIcon : BlendTreeInspector.styles.pointIconSelected);
					if (flag)
					{
						GUI.color = BlendTreeInspector.styles.visPointOverlayColor;
						GUI.DrawTexture(position2, BlendTreeInspector.styles.pointIconOverlay);
					}
				}
				if (!this.s_DraggingPoint)
				{
					GUI.color = BlendTreeInspector.styles.visSamplerColor;
					GUI.DrawTexture(position, BlendTreeInspector.styles.samplerIcon);
				}
				GUI.color = Color.white;
				break;
			}
			if (this.m_ReorderableList.index >= 0 && motionToActiveMotionIndices[this.m_ReorderableList.index] < 0)
			{
				this.ShowHelp(area, EditorGUIUtility.TempContent("The selected child has no Motion assigned."));
			}
			else if (this.m_WarningMessage != null)
			{
				this.ShowHelp(area, EditorGUIUtility.TempContent(this.m_WarningMessage));
			}
		}

		private void ShowHelp(Rect area, GUIContent content)
		{
			float height = EditorStyles.helpBox.CalcHeight(content, area.width);
			GUI.Label(new Rect(area.x, area.y, area.width, height), content, EditorStyles.helpBox);
		}

		private void ValidatePositions()
		{
			this.m_WarningMessage = null;
			Vector2[] motionPositions = this.GetMotionPositions();
			bool flag = this.m_BlendRect.width == 0f || this.m_BlendRect.height == 0f;
			for (int i = 0; i < motionPositions.Length; i++)
			{
				int num = 0;
				while (num < i && !flag)
				{
					if (((motionPositions[i] - motionPositions[num]) / this.m_BlendRect.height).sqrMagnitude < 0.0001f)
					{
						flag = true;
						break;
					}
					num++;
				}
			}
			if (flag)
			{
				this.m_WarningMessage = "Two or more of the positions are too close to each other.";
			}
			else if (this.m_BlendType.intValue == 1)
			{
				List<float> list = (from e in motionPositions
				where e != Vector2.zero
				select Mathf.Atan2(e.y, e.x) into e
				orderby e
				select e).ToList<float>();
				float num2 = 0f;
				float num3 = 180f;
				for (int j = 0; j < list.Count; j++)
				{
					float num4 = list[(j + 1) % list.Count] - list[j];
					if (j == list.Count - 1)
					{
						num4 += 6.28318548f;
					}
					if (num4 > num2)
					{
						num2 = num4;
					}
					if (num4 < num3)
					{
						num3 = num4;
					}
				}
				if (num2 * 57.29578f >= 180f)
				{
					this.m_WarningMessage = "Simple Directional blend should have motions with directions less than 180 degrees apart.";
				}
				else if (num3 * 57.29578f < 2f)
				{
					this.m_WarningMessage = "Simple Directional blend should not have multiple motions in almost the same direction.";
				}
			}
			else if (this.m_BlendType.intValue == 2)
			{
				bool flag2 = false;
				for (int k = 0; k < motionPositions.Length; k++)
				{
					if (motionPositions[k] == Vector2.zero)
					{
						flag2 = true;
						break;
					}
				}
				if (!flag2)
				{
					this.m_WarningMessage = "Freeform Directional blend should have one motion at position (0,0) to avoid discontinuities.";
				}
			}
		}

		private void DrawWeightShape(Vector2 point, float weight, int pass)
		{
			if (weight > 0f)
			{
				point.x = Mathf.Round(point.x);
				point.y = Mathf.Round(point.y);
				float num = 20f * Mathf.Sqrt(weight);
				Vector3[] array = new Vector3[this.kNumCirclePoints + 2];
				for (int i = 0; i < this.kNumCirclePoints; i++)
				{
					float num2 = (float)i / (float)this.kNumCirclePoints;
					array[i + 1] = new Vector3(point.x + 0.5f, point.y + 0.5f, 0f) + new Vector3(Mathf.Sin(num2 * 2f * 3.14159274f), Mathf.Cos(num2 * 2f * 3.14159274f), 0f) * num;
				}
				array[0] = (array[this.kNumCirclePoints + 1] = (array[1] + array[this.kNumCirclePoints]) * 0.5f);
				if (pass == 0)
				{
					Handles.color = BlendTreeInspector.styles.visWeightShapeColor;
					Handles.DrawSolidDisc(point + new Vector2(0.5f, 0.5f), -Vector3.forward, num);
				}
				else
				{
					Handles.color = BlendTreeInspector.styles.visWeightLineColor;
					Handles.DrawAAPolyLine(array);
				}
			}
		}

		private void DrawAnimation(float val, float min, float max, bool selected, Rect area)
		{
			float y = area.y;
			Rect position = new Rect(min, y, val - min, area.height);
			Rect position2 = new Rect(val, y, max - val, area.height);
			BlendTreeInspector.styles.triangleLeft.Draw(position, selected, selected, false, false);
			BlendTreeInspector.styles.triangleRight.Draw(position2, selected, selected, false, false);
			area.height -= 1f;
			Color color = Handles.color;
			Color color2 = (!selected) ? new Color(1f, 1f, 1f, 0.4f) : new Color(1f, 1f, 1f, 0.6f);
			Handles.color = color2;
			if (selected)
			{
				Handles.DrawLine(new Vector3(val, y, 0f), new Vector3(val, y + area.height, 0f));
			}
			Vector3[] points = new Vector3[]
			{
				new Vector3(min, y + area.height, 0f),
				new Vector3(val, y, 0f)
			};
			Handles.DrawAAPolyLine(points);
			points = new Vector3[]
			{
				new Vector3(val, y, 0f),
				new Vector3(max, y + area.height, 0f)
			};
			Handles.DrawAAPolyLine(points);
			Handles.color = color;
		}

		public void EndDragChild(ReorderableList list)
		{
			List<float> list2 = new List<float>();
			for (int i = 0; i < this.m_Childs.arraySize; i++)
			{
				SerializedProperty arrayElementAtIndex = this.m_Childs.GetArrayElementAtIndex(i);
				SerializedProperty serializedProperty = arrayElementAtIndex.FindPropertyRelative("m_Threshold");
				list2.Add(serializedProperty.floatValue);
			}
			list2.Sort();
			for (int j = 0; j < this.m_Childs.arraySize; j++)
			{
				SerializedProperty arrayElementAtIndex2 = this.m_Childs.GetArrayElementAtIndex(j);
				SerializedProperty serializedProperty2 = arrayElementAtIndex2.FindPropertyRelative("m_Threshold");
				serializedProperty2.floatValue = list2[j];
			}
			base.serializedObject.ApplyModifiedProperties();
		}

		private void DrawHeader(Rect headerRect)
		{
			headerRect.xMin += 14f;
			headerRect.y += 1f;
			headerRect.height = 16f;
			Rect[] rowRects = this.GetRowRects(headerRect, this.m_BlendType.intValue);
			int num = 0;
			rowRects[num].xMin = rowRects[num].xMin - 14f;
			GUI.Label(rowRects[num], EditorGUIUtility.TempContent("Motion"), EditorStyles.label);
			num++;
			if (this.m_Childs.arraySize >= 1)
			{
				if (this.m_BlendType.intValue == 0)
				{
					GUI.Label(rowRects[num], EditorGUIUtility.TempContent("Threshold"), EditorStyles.label);
					num++;
				}
				else if (this.m_BlendType.intValue == 4)
				{
					GUI.Label(rowRects[num], EditorGUIUtility.TempContent("Parameter"), EditorStyles.label);
					num++;
				}
				else
				{
					GUI.Label(rowRects[num], EditorGUIUtility.TempContent("Pos X"), EditorStyles.label);
					num++;
					GUI.Label(rowRects[num], EditorGUIUtility.TempContent("Pos Y"), EditorStyles.label);
					num++;
				}
				GUI.Label(rowRects[num], BlendTreeInspector.styles.speedIcon, BlendTreeInspector.styles.headerIcon);
				num++;
				GUI.Label(rowRects[num], BlendTreeInspector.styles.mirrorIcon, BlendTreeInspector.styles.headerIcon);
			}
		}

		public void AddButton(Rect rect, ReorderableList list)
		{
			GenericMenu genericMenu = new GenericMenu();
			genericMenu.AddItem(new GUIContent("Add Motion Field"), false, new GenericMenu.MenuFunction(this.AddChildAnimation));
			genericMenu.AddItem(EditorGUIUtility.TempContent("New Blend Tree"), false, new GenericMenu.MenuFunction(this.AddBlendTreeCallback));
			genericMenu.Popup(rect, 0);
		}

		public static bool DeleteBlendTreeDialog(string toDelete)
		{
			string title = "Delete selected Blend Tree asset?";
			return EditorUtility.DisplayDialog(title, toDelete, "Delete", "Cancel");
		}

		public void RemoveButton(ReorderableList list)
		{
			SerializedProperty arrayElementAtIndex = this.m_Childs.GetArrayElementAtIndex(list.index);
			SerializedProperty serializedProperty = arrayElementAtIndex.FindPropertyRelative("m_Motion");
			Motion motion = serializedProperty.objectReferenceValue as Motion;
			if (motion == null || BlendTreeInspector.DeleteBlendTreeDialog(motion.name))
			{
				this.m_Childs.DeleteArrayElementAtIndex(list.index);
				if (list.index >= this.m_Childs.arraySize)
				{
					list.index = this.m_Childs.arraySize - 1;
				}
				this.SetMinMaxThresholds();
			}
		}

		private Rect[] GetRowRects(Rect r, int blendType)
		{
			int num = (blendType <= 0 || blendType >= 4) ? 1 : 2;
			Rect[] array = new Rect[3 + num];
			float num2 = r.width;
			float num3 = 16f;
			num2 -= num3;
			num2 -= (float)(24 + 4 * (num - 1));
			float num4 = (float)Mathf.FloorToInt(num2 * 0.2f);
			float num5 = num2 - num4 * (float)(num + 1);
			float num6 = r.x;
			int num7 = 0;
			array[num7] = new Rect(num6, r.y, num5, r.height);
			num6 += num5 + 8f;
			num7++;
			for (int i = 0; i < num; i++)
			{
				array[num7] = new Rect(num6, r.y, num4, r.height);
				num6 += num4 + 4f;
				num7++;
			}
			num6 += 4f;
			array[num7] = new Rect(num6, r.y, num4, r.height);
			num6 += num4 + 8f;
			num7++;
			array[num7] = new Rect(num6, r.y, num3, r.height);
			return array;
		}

		public void DrawChild(Rect r, int index, bool isActive, bool isFocused)
		{
			SerializedProperty arrayElementAtIndex = this.m_Childs.GetArrayElementAtIndex(index);
			SerializedProperty serializedProperty = arrayElementAtIndex.FindPropertyRelative("m_Motion");
			r.y += 1f;
			r.height = 16f;
			Rect[] rowRects = this.GetRowRects(r, this.m_BlendType.intValue);
			int num = 0;
			EditorGUI.BeginChangeCheck();
			Motion motion = this.m_BlendTree.children[index].motion;
			EditorGUI.PropertyField(rowRects[num], serializedProperty, GUIContent.none);
			num++;
			if (EditorGUI.EndChangeCheck())
			{
				if (motion is UnityEditor.Animations.BlendTree && motion != serializedProperty.objectReferenceValue as Motion)
				{
					if (EditorUtility.DisplayDialog("Changing BlendTree will delete previous BlendTree", "You cannot undo this action.", "Delete", "Cancel"))
					{
						MecanimUtilities.DestroyBlendTreeRecursive(motion as UnityEditor.Animations.BlendTree);
					}
					else
					{
						serializedProperty.objectReferenceValue = motion;
					}
				}
			}
			if (this.m_BlendType.intValue == 0)
			{
				SerializedProperty serializedProperty2 = arrayElementAtIndex.FindPropertyRelative("m_Threshold");
				using (new EditorGUI.DisabledScope(this.m_UseAutomaticThresholds.boolValue))
				{
					float floatValue = serializedProperty2.floatValue;
					EditorGUI.BeginChangeCheck();
					string s = EditorGUI.DelayedTextFieldInternal(rowRects[num], floatValue.ToString(), "inftynaeINFTYNAE0123456789.,-", EditorStyles.textField);
					num++;
					if (EditorGUI.EndChangeCheck())
					{
						if (float.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture.NumberFormat, out floatValue))
						{
							serializedProperty2.floatValue = floatValue;
							base.serializedObject.ApplyModifiedProperties();
							this.m_BlendTree.SortChildren();
							this.SetMinMaxThresholds();
							GUI.changed = true;
						}
					}
				}
			}
			else if (this.m_BlendType.intValue == 4)
			{
				List<string> list = this.CollectParameters(BlendTreeInspector.currentController);
				ChildMotion[] children = this.m_BlendTree.children;
				string text = children[index].directBlendParameter;
				EditorGUI.BeginChangeCheck();
				text = EditorGUI.TextFieldDropDown(rowRects[num], text, list.ToArray());
				num++;
				if (EditorGUI.EndChangeCheck())
				{
					children[index].directBlendParameter = text;
					this.m_BlendTree.children = children;
				}
			}
			else
			{
				SerializedProperty serializedProperty3 = arrayElementAtIndex.FindPropertyRelative("m_Position");
				Vector2 vector2Value = serializedProperty3.vector2Value;
				for (int i = 0; i < 2; i++)
				{
					EditorGUI.BeginChangeCheck();
					string s2 = EditorGUI.DelayedTextFieldInternal(rowRects[num], vector2Value[i].ToString(), "inftynaeINFTYNAE0123456789.,-", EditorStyles.textField);
					num++;
					if (EditorGUI.EndChangeCheck())
					{
						float value;
						if (float.TryParse(s2, NumberStyles.Float, CultureInfo.InvariantCulture.NumberFormat, out value))
						{
							vector2Value[i] = Mathf.Clamp(value, -10000f, 10000f);
							serializedProperty3.vector2Value = vector2Value;
							base.serializedObject.ApplyModifiedProperties();
							GUI.changed = true;
						}
					}
				}
			}
			if (serializedProperty.objectReferenceValue is AnimationClip)
			{
				SerializedProperty property = arrayElementAtIndex.FindPropertyRelative("m_TimeScale");
				EditorGUI.PropertyField(rowRects[num], property, GUIContent.none);
			}
			else
			{
				using (new EditorGUI.DisabledScope(true))
				{
					EditorGUI.IntField(rowRects[num], 1);
				}
			}
			num++;
			if (serializedProperty.objectReferenceValue is AnimationClip && (serializedProperty.objectReferenceValue as AnimationClip).isHumanMotion)
			{
				SerializedProperty serializedProperty4 = arrayElementAtIndex.FindPropertyRelative("m_Mirror");
				EditorGUI.PropertyField(rowRects[num], serializedProperty4, GUIContent.none);
				SerializedProperty serializedProperty5 = arrayElementAtIndex.FindPropertyRelative("m_CycleOffset");
				serializedProperty5.floatValue = ((!serializedProperty4.boolValue) ? 0f : 0.5f);
			}
			else
			{
				using (new EditorGUI.DisabledScope(true))
				{
					EditorGUI.Toggle(rowRects[num], false);
				}
			}
		}

		private bool AllMotions()
		{
			bool flag = true;
			int num = 0;
			while (num < this.m_Childs.arraySize && flag)
			{
				SerializedProperty serializedProperty = this.m_Childs.GetArrayElementAtIndex(num).FindPropertyRelative("m_Motion");
				flag = (serializedProperty.objectReferenceValue is AnimationClip);
				num++;
			}
			return flag;
		}

		private void AutoCompute()
		{
			if (this.m_BlendType.intValue == 0)
			{
				EditorGUILayout.PropertyField(this.m_UseAutomaticThresholds, EditorGUIUtility.TempContent("Automate Thresholds"), new GUILayoutOption[0]);
				this.m_ShowCompute.target = !this.m_UseAutomaticThresholds.boolValue;
			}
			else if (this.m_BlendType.intValue == 4)
			{
				this.m_ShowCompute.target = false;
			}
			else
			{
				this.m_ShowCompute.target = true;
			}
			this.m_ShowAdjust.target = this.AllMotions();
			if (EditorGUILayout.BeginFadeGroup(this.m_ShowCompute.faded))
			{
				Rect rect = EditorGUILayout.GetControlRect(new GUILayoutOption[0]);
				GUIContent label = (this.ParameterCount != 1) ? EditorGUIUtility.TempContent("Compute Positions") : EditorGUIUtility.TempContent("Compute Thresholds");
				rect = EditorGUI.PrefixLabel(rect, 0, label);
				if (EditorGUI.DropdownButton(rect, EditorGUIUtility.TempContent("Select"), FocusType.Passive, EditorStyles.popup))
				{
					GenericMenu genericMenu = new GenericMenu();
					if (this.ParameterCount == 1)
					{
						this.AddComputeMenuItems(genericMenu, string.Empty, BlendTreeInspector.ChildPropertyToCompute.Threshold);
					}
					else
					{
						genericMenu.AddItem(new GUIContent("Velocity XZ"), false, new GenericMenu.MenuFunction(this.ComputePositionsFromVelocity));
						genericMenu.AddItem(new GUIContent("Speed And Angular Speed"), false, new GenericMenu.MenuFunction(this.ComputePositionsFromSpeedAndAngularSpeed));
						this.AddComputeMenuItems(genericMenu, "X Position From/", BlendTreeInspector.ChildPropertyToCompute.PositionX);
						this.AddComputeMenuItems(genericMenu, "Y Position From/", BlendTreeInspector.ChildPropertyToCompute.PositionY);
					}
					genericMenu.DropDown(rect);
				}
			}
			EditorGUILayout.EndFadeGroup();
			if (EditorGUILayout.BeginFadeGroup(this.m_ShowAdjust.faded))
			{
				Rect rect2 = EditorGUILayout.GetControlRect(new GUILayoutOption[0]);
				rect2 = EditorGUI.PrefixLabel(rect2, 0, EditorGUIUtility.TempContent("Adjust Time Scale"));
				if (EditorGUI.DropdownButton(rect2, EditorGUIUtility.TempContent("Select"), FocusType.Passive, EditorStyles.popup))
				{
					GenericMenu genericMenu2 = new GenericMenu();
					genericMenu2.AddItem(new GUIContent("Homogeneous Speed"), false, new GenericMenu.MenuFunction(this.ComputeTimeScaleFromSpeed));
					genericMenu2.AddItem(new GUIContent("Reset Time Scale"), false, new GenericMenu.MenuFunction(this.ResetTimeScale));
					genericMenu2.DropDown(rect2);
				}
			}
			EditorGUILayout.EndFadeGroup();
		}

		private void AddComputeMenuItems(GenericMenu menu, string menuItemPrefix, BlendTreeInspector.ChildPropertyToCompute prop)
		{
			menu.AddItem(new GUIContent(menuItemPrefix + "Speed"), false, new GenericMenu.MenuFunction2(this.ComputeFromSpeed), prop);
			menu.AddItem(new GUIContent(menuItemPrefix + "Velocity X"), false, new GenericMenu.MenuFunction2(this.ComputeFromVelocityX), prop);
			menu.AddItem(new GUIContent(menuItemPrefix + "Velocity Y"), false, new GenericMenu.MenuFunction2(this.ComputeFromVelocityY), prop);
			menu.AddItem(new GUIContent(menuItemPrefix + "Velocity Z"), false, new GenericMenu.MenuFunction2(this.ComputeFromVelocityZ), prop);
			menu.AddItem(new GUIContent(menuItemPrefix + "Angular Speed (Rad)"), false, new GenericMenu.MenuFunction2(this.ComputeFromAngularSpeedRadians), prop);
			menu.AddItem(new GUIContent(menuItemPrefix + "Angular Speed (Deg)"), false, new GenericMenu.MenuFunction2(this.ComputeFromAngularSpeedDegrees), prop);
		}

		private void ComputeFromSpeed(object obj)
		{
			BlendTreeInspector.ChildPropertyToCompute prop = (BlendTreeInspector.ChildPropertyToCompute)obj;
			this.ComputeProperty((Motion m, float mirrorMultiplier) => m.apparentSpeed, prop);
		}

		private void ComputeFromVelocityX(object obj)
		{
			BlendTreeInspector.ChildPropertyToCompute prop = (BlendTreeInspector.ChildPropertyToCompute)obj;
			this.ComputeProperty((Motion m, float mirrorMultiplier) => m.averageSpeed.x * mirrorMultiplier, prop);
		}

		private void ComputeFromVelocityY(object obj)
		{
			BlendTreeInspector.ChildPropertyToCompute prop = (BlendTreeInspector.ChildPropertyToCompute)obj;
			this.ComputeProperty((Motion m, float mirrorMultiplier) => m.averageSpeed.y, prop);
		}

		private void ComputeFromVelocityZ(object obj)
		{
			BlendTreeInspector.ChildPropertyToCompute prop = (BlendTreeInspector.ChildPropertyToCompute)obj;
			this.ComputeProperty((Motion m, float mirrorMultiplier) => m.averageSpeed.z, prop);
		}

		private void ComputeFromAngularSpeedDegrees(object obj)
		{
			BlendTreeInspector.ChildPropertyToCompute prop = (BlendTreeInspector.ChildPropertyToCompute)obj;
			this.ComputeProperty((Motion m, float mirrorMultiplier) => m.averageAngularSpeed * 180f / 3.14159274f * mirrorMultiplier, prop);
		}

		private void ComputeFromAngularSpeedRadians(object obj)
		{
			BlendTreeInspector.ChildPropertyToCompute prop = (BlendTreeInspector.ChildPropertyToCompute)obj;
			this.ComputeProperty((Motion m, float mirrorMultiplier) => m.averageAngularSpeed * mirrorMultiplier, prop);
		}

		private void ComputeProperty(BlendTreeInspector.GetFloatFromMotion func, BlendTreeInspector.ChildPropertyToCompute prop)
		{
			float num = 0f;
			float[] array = new float[this.m_Childs.arraySize];
			this.m_UseAutomaticThresholds.boolValue = false;
			for (int i = 0; i < this.m_Childs.arraySize; i++)
			{
				SerializedProperty serializedProperty = this.m_Childs.GetArrayElementAtIndex(i).FindPropertyRelative("m_Motion");
				SerializedProperty serializedProperty2 = this.m_Childs.GetArrayElementAtIndex(i).FindPropertyRelative("m_Mirror");
				Motion motion = serializedProperty.objectReferenceValue as Motion;
				if (motion != null)
				{
					float num2 = func(motion, (float)((!serializedProperty2.boolValue) ? 1 : -1));
					array[i] = num2;
					num += num2;
					if (prop == BlendTreeInspector.ChildPropertyToCompute.Threshold)
					{
						SerializedProperty serializedProperty3 = this.m_Childs.GetArrayElementAtIndex(i).FindPropertyRelative("m_Threshold");
						serializedProperty3.floatValue = num2;
					}
					else
					{
						SerializedProperty serializedProperty4 = this.m_Childs.GetArrayElementAtIndex(i).FindPropertyRelative("m_Position");
						Vector2 vector2Value = serializedProperty4.vector2Value;
						if (prop == BlendTreeInspector.ChildPropertyToCompute.PositionX)
						{
							vector2Value.x = num2;
						}
						else
						{
							vector2Value.y = num2;
						}
						serializedProperty4.vector2Value = vector2Value;
					}
				}
			}
			num /= (float)this.m_Childs.arraySize;
			float num3 = 0f;
			for (int j = 0; j < array.Length; j++)
			{
				num3 += Mathf.Pow(array[j] - num, 2f);
			}
			num3 /= (float)array.Length;
			if (num3 < Mathf.Epsilon)
			{
				UnityEngine.Debug.LogWarning("Could not compute threshold for '" + this.m_BlendTree.name + "' there is not enough data");
				this.m_SerializedObject.Update();
			}
			else
			{
				this.m_SerializedObject.ApplyModifiedProperties();
				if (prop == BlendTreeInspector.ChildPropertyToCompute.Threshold)
				{
					this.SortByThreshold();
					this.SetMinMaxThreshold();
				}
			}
		}

		private void ComputePositionsFromVelocity()
		{
			this.ComputeFromVelocityX(BlendTreeInspector.ChildPropertyToCompute.PositionX);
			this.ComputeFromVelocityZ(BlendTreeInspector.ChildPropertyToCompute.PositionY);
		}

		private void ComputePositionsFromSpeedAndAngularSpeed()
		{
			this.ComputeFromAngularSpeedRadians(BlendTreeInspector.ChildPropertyToCompute.PositionX);
			this.ComputeFromSpeed(BlendTreeInspector.ChildPropertyToCompute.PositionY);
		}

		private void ComputeTimeScaleFromSpeed()
		{
			float apparentSpeed = this.m_BlendTree.apparentSpeed;
			for (int i = 0; i < this.m_Childs.arraySize; i++)
			{
				SerializedProperty serializedProperty = this.m_Childs.GetArrayElementAtIndex(i).FindPropertyRelative("m_Motion");
				AnimationClip animationClip = serializedProperty.objectReferenceValue as AnimationClip;
				if (animationClip != null)
				{
					if (!animationClip.legacy)
					{
						if (animationClip.apparentSpeed < Mathf.Epsilon)
						{
							UnityEngine.Debug.LogWarning("Could not adjust time scale for " + animationClip.name + " because it has no speed");
						}
						else
						{
							SerializedProperty serializedProperty2 = this.m_Childs.GetArrayElementAtIndex(i).FindPropertyRelative("m_TimeScale");
							serializedProperty2.floatValue = apparentSpeed / animationClip.apparentSpeed;
						}
					}
					else
					{
						UnityEngine.Debug.LogWarning("Could not adjust time scale for " + animationClip.name + " because it is not a muscle clip");
					}
				}
			}
			this.m_SerializedObject.ApplyModifiedProperties();
		}

		private void ResetTimeScale()
		{
			for (int i = 0; i < this.m_Childs.arraySize; i++)
			{
				SerializedProperty serializedProperty = this.m_Childs.GetArrayElementAtIndex(i).FindPropertyRelative("m_Motion");
				AnimationClip animationClip = serializedProperty.objectReferenceValue as AnimationClip;
				if (animationClip != null && !animationClip.legacy)
				{
					SerializedProperty serializedProperty2 = this.m_Childs.GetArrayElementAtIndex(i).FindPropertyRelative("m_TimeScale");
					serializedProperty2.floatValue = 1f;
				}
			}
			this.m_SerializedObject.ApplyModifiedProperties();
		}

		private void SortByThreshold()
		{
			this.m_SerializedObject.Update();
			for (int i = 0; i < this.m_Childs.arraySize; i++)
			{
				float num = float.PositiveInfinity;
				int num2 = -1;
				for (int j = i; j < this.m_Childs.arraySize; j++)
				{
					SerializedProperty arrayElementAtIndex = this.m_Childs.GetArrayElementAtIndex(j);
					float floatValue = arrayElementAtIndex.FindPropertyRelative("m_Threshold").floatValue;
					if (floatValue < num)
					{
						num = floatValue;
						num2 = j;
					}
				}
				if (num2 != i)
				{
					this.m_Childs.MoveArrayElement(num2, i);
				}
			}
			this.m_SerializedObject.ApplyModifiedProperties();
		}

		private void SetMinMaxThreshold()
		{
			this.m_SerializedObject.Update();
			SerializedProperty serializedProperty = this.m_Childs.GetArrayElementAtIndex(0).FindPropertyRelative("m_Threshold");
			SerializedProperty serializedProperty2 = this.m_Childs.GetArrayElementAtIndex(this.m_Childs.arraySize - 1).FindPropertyRelative("m_Threshold");
			this.m_MinThreshold.floatValue = Mathf.Min(serializedProperty.floatValue, serializedProperty2.floatValue);
			this.m_MaxThreshold.floatValue = Mathf.Max(serializedProperty.floatValue, serializedProperty2.floatValue);
			this.m_SerializedObject.ApplyModifiedProperties();
		}

		private void AddChildAnimation()
		{
			this.m_BlendTree.AddChild(null);
			int num = this.m_BlendTree.children.Length;
			this.m_BlendTree.SetDirectBlendTreeParameter(num - 1, BlendTreeInspector.currentController.GetDefaultBlendTreeParameter());
			this.SetNewThresholdAndPosition(num - 1);
			this.m_ReorderableList.index = num - 1;
		}

		private void AddBlendTreeCallback()
		{
			UnityEditor.Animations.BlendTree blendTree = this.m_BlendTree.CreateBlendTreeChild(0f);
			ChildMotion[] children = this.m_BlendTree.children;
			int num = children.Length;
			if (BlendTreeInspector.currentController != null)
			{
				blendTree.blendParameter = this.m_BlendTree.blendParameter;
				this.m_BlendTree.SetDirectBlendTreeParameter(num - 1, BlendTreeInspector.currentController.GetDefaultBlendTreeParameter());
			}
			this.SetNewThresholdAndPosition(num - 1);
			this.m_ReorderableList.index = this.m_Childs.arraySize - 1;
		}

		private void SetNewThresholdAndPosition(int index)
		{
			base.serializedObject.Update();
			if (!this.m_UseAutomaticThresholds.boolValue)
			{
				float floatValue3;
				if (this.m_Childs.arraySize >= 3 && index == this.m_Childs.arraySize - 1)
				{
					float floatValue = this.m_Childs.GetArrayElementAtIndex(index - 2).FindPropertyRelative("m_Threshold").floatValue;
					float floatValue2 = this.m_Childs.GetArrayElementAtIndex(index - 1).FindPropertyRelative("m_Threshold").floatValue;
					floatValue3 = floatValue2 + (floatValue2 - floatValue);
				}
				else if (this.m_Childs.arraySize == 1)
				{
					floatValue3 = 0f;
				}
				else
				{
					floatValue3 = this.m_Childs.GetArrayElementAtIndex(this.m_Childs.arraySize - 1).FindPropertyRelative("m_Threshold").floatValue + 1f;
				}
				SerializedProperty serializedProperty = this.m_Childs.GetArrayElementAtIndex(index).FindPropertyRelative("m_Threshold");
				serializedProperty.floatValue = floatValue3;
				this.SetMinMaxThresholds();
			}
			Vector2 vector = Vector2.zero;
			if (this.m_Childs.arraySize >= 1)
			{
				Vector2 center = this.m_BlendRect.center;
				Vector2[] motionPositions = this.GetMotionPositions();
				float num = this.m_BlendRect.width * 0.07f;
				for (int i = 0; i < 24; i++)
				{
					bool flag = true;
					int num2 = 0;
					while (num2 < motionPositions.Length && flag)
					{
						if (num2 != index && Vector2.Distance(motionPositions[num2], vector) < num)
						{
							flag = false;
						}
						num2++;
					}
					if (flag)
					{
						break;
					}
					float f = (float)(i * 15) * 0.0174532924f;
					vector = center + new Vector2(-Mathf.Cos(f), Mathf.Sin(f)) * 0.37f * this.m_BlendRect.width;
					vector.x = MathUtils.RoundBasedOnMinimumDifference(vector.x, this.m_BlendRect.width * 0.005f);
					vector.y = MathUtils.RoundBasedOnMinimumDifference(vector.y, this.m_BlendRect.width * 0.005f);
				}
			}
			SerializedProperty serializedProperty2 = this.m_Childs.GetArrayElementAtIndex(index).FindPropertyRelative("m_Position");
			serializedProperty2.vector2Value = vector;
			base.serializedObject.ApplyModifiedProperties();
		}

		public override bool HasPreviewGUI()
		{
			return this.m_PreviewBlendTree != null && this.m_PreviewBlendTree.HasPreviewGUI();
		}

		public override void OnPreviewSettings()
		{
			if (this.m_PreviewBlendTree != null)
			{
				this.m_PreviewBlendTree.OnPreviewSettings();
			}
		}

		public override void OnInteractivePreviewGUI(Rect r, GUIStyle background)
		{
			if (this.m_PreviewBlendTree != null)
			{
				this.m_PreviewBlendTree.OnInteractivePreviewGUI(r, background);
			}
		}

		public void OnDisable()
		{
			if (this.m_PreviewBlendTree != null)
			{
				this.m_PreviewBlendTree.OnDisable();
			}
			if (this.m_VisBlendTree != null)
			{
				this.m_VisBlendTree.Reset();
			}
		}

		public void OnDestroy()
		{
			if (this.m_PreviewBlendTree != null)
			{
				this.m_PreviewBlendTree.OnDestroy();
			}
			if (this.m_VisBlendTree != null)
			{
				this.m_VisBlendTree.Destroy();
			}
			if (this.m_VisInstance != null)
			{
				UnityEngine.Object.DestroyImmediate(this.m_VisInstance);
			}
			for (int i = 0; i < this.m_WeightTexs.Count; i++)
			{
				UnityEngine.Object.DestroyImmediate(this.m_WeightTexs[i]);
			}
			if (this.m_BlendTex != null)
			{
				UnityEngine.Object.DestroyImmediate(this.m_BlendTex);
			}
		}
	}
}
