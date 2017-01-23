using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.AnimatedValues;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Events;

namespace UnityEditor
{
	[CanEditMultipleObjects, CustomEditor(typeof(RectTransform))]
	internal class RectTransformEditor : Editor
	{
		private class Styles
		{
			public GUIStyle lockStyle = EditorStyles.miniButton;

			public GUIStyle measuringLabelStyle = new GUIStyle("PreOverlayLabel");

			public GUIContent anchorsContent = new GUIContent("Anchors");

			public GUIContent anchorMinContent = new GUIContent("Min", "The normalized position in the parent rectangle that the lower left corner is anchored to.");

			public GUIContent anchorMaxContent = new GUIContent("Max", "The normalized position in the parent rectangle that the upper right corner is anchored to.");

			public GUIContent positionContent = new GUIContent("Position", "The local position of the rectangle. The position specifies this rectangle's pivot relative to the anchor reference point.");

			public GUIContent sizeContent = new GUIContent("Size", "The size of the rectangle.");

			public GUIContent pivotContent = new GUIContent("Pivot", "The pivot point specified in normalized values between 0 and 1. The pivot point is the origin of this rectangle. Rotation and scaling is around this point.");

			public GUIContent transformScaleContent = new GUIContent("Scale", "The local scaling of this Game Object relative to the parent. This scales everything including image borders and text.");

			public GUIContent transformPositionZContent = new GUIContent("Pos Z", "Distance to offset the rectangle along the Z axis of the parent. The effect is visible if the Canvas uses a perspective camera, or if a parent RectTransform is rotated along the X or Y axis.");

			public GUIContent rawEditContent;

			public GUIContent blueprintContent;

			public Styles()
			{
				this.rawEditContent = EditorGUIUtility.IconContent("RectTransformRaw", "|Raw edit mode. When enabled, editing pivot and anchor values will not counter-adjust the position and size of the rectangle in order to make it stay in place.");
				this.blueprintContent = EditorGUIUtility.IconContent("RectTransformBlueprint", "|Blueprint mode. Edit RectTransforms as if they were not rotated and scaled. This enables snapping too.");
			}
		}

		private enum AnchorFusedState
		{
			None,
			All,
			Horizontal,
			Vertical
		}

		private delegate float FloatGetter(RectTransform rect);

		private delegate void FloatSetter(RectTransform rect, float f);

		private const string kShowAnchorPropsPrefName = "RectTransformEditor.showAnchorProperties";

		private const string kLockRectPrefName = "RectTransformEditor.lockRect";

		private static Vector2 kShadowOffset = new Vector2(1f, -1f);

		private static Color kShadowColor = new Color(0f, 0f, 0f, 0.5f);

		private const float kDottedLineSize = 5f;

		private static float kDropdownSize = 49f;

		private static Color kRectInParentSpaceColor = new Color(1f, 1f, 1f, 0.4f);

		private static Color kParentColor = new Color(1f, 1f, 1f, 0.6f);

		private static Color kSiblingColor = new Color(1f, 1f, 1f, 0.2f);

		private static Color kAnchorColor = new Color(1f, 1f, 1f, 1f);

		private static Color kAnchorLineColor = new Color(1f, 1f, 1f, 0.6f);

		private static Vector3[] s_Corners = new Vector3[4];

		private static RectTransformEditor.Styles s_Styles;

		private static int s_FoldoutHash = "Foldout".GetHashCode();

		private static int s_FloatFieldHash = "EditorTextField".GetHashCode();

		private static int s_ParentRectPreviewHandlesHash = "ParentRectPreviewDragHandles".GetHashCode();

		private static GUIContent[] s_XYLabels = new GUIContent[]
		{
			new GUIContent("X"),
			new GUIContent("Y")
		};

		private static GUIContent[] s_XYZLabels = new GUIContent[]
		{
			new GUIContent("X"),
			new GUIContent("Y"),
			new GUIContent("Z")
		};

		private static bool[] s_ScaleDisabledMask = new bool[3];

		private static Vector3 s_StartMouseWorldPos;

		private static Vector3 s_StartPosition;

		private static Vector2 s_StartMousePos;

		private static bool s_DragAnchorsTogether;

		private static Vector2 s_StartDragAnchorMin;

		private static Vector2 s_StartDragAnchorMax;

		private static RectTransformEditor.AnchorFusedState s_AnchorFusedState = RectTransformEditor.AnchorFusedState.None;

		private SerializedProperty m_AnchorMin;

		private SerializedProperty m_AnchorMax;

		private SerializedProperty m_AnchoredPosition;

		private SerializedProperty m_SizeDelta;

		private SerializedProperty m_Pivot;

		private SerializedProperty m_LocalPositionZ;

		private SerializedProperty m_LocalScale;

		private TransformRotationGUI m_RotationGUI;

		private bool m_ShowLayoutOptions = false;

		private bool m_RawEditMode = false;

		private int m_TargetCount = 0;

		private Dictionary<int, AnimBool> m_KeyboardControlIDs = new Dictionary<int, AnimBool>();

		private AnimBool m_ChangingAnchors = new AnimBool();

		private AnimBool m_ChangingPivot = new AnimBool();

		private AnimBool m_ChangingWidth = new AnimBool();

		private AnimBool m_ChangingHeight = new AnimBool();

		private AnimBool m_ChangingPosX = new AnimBool();

		private AnimBool m_ChangingPosY = new AnimBool();

		private AnimBool m_ChangingLeft = new AnimBool();

		private AnimBool m_ChangingRight = new AnimBool();

		private AnimBool m_ChangingTop = new AnimBool();

		private AnimBool m_ChangingBottom = new AnimBool();

		private LayoutDropdownWindow m_DropdownWindow;

		private static float s_ParentDragTime = 0f;

		private static float s_ParentDragId = 0f;

		private static Rect s_ParentDragOrigRect = default(Rect);

		private static Rect s_ParentDragPreviewRect = default(Rect);

		private static RectTransform s_ParentDragRectTransform = null;

		private static RectTransformEditor.Styles styles
		{
			get
			{
				if (RectTransformEditor.s_Styles == null)
				{
					RectTransformEditor.s_Styles = new RectTransformEditor.Styles();
				}
				return RectTransformEditor.s_Styles;
			}
		}

		private void OnEnable()
		{
			this.m_AnchorMin = base.serializedObject.FindProperty("m_AnchorMin");
			this.m_AnchorMax = base.serializedObject.FindProperty("m_AnchorMax");
			this.m_AnchoredPosition = base.serializedObject.FindProperty("m_AnchoredPosition");
			this.m_SizeDelta = base.serializedObject.FindProperty("m_SizeDelta");
			this.m_Pivot = base.serializedObject.FindProperty("m_Pivot");
			this.m_TargetCount = base.targets.Length;
			this.m_LocalPositionZ = base.serializedObject.FindProperty("m_LocalPosition.z");
			this.m_LocalScale = base.serializedObject.FindProperty("m_LocalScale");
			if (this.m_RotationGUI == null)
			{
				this.m_RotationGUI = new TransformRotationGUI();
			}
			this.m_RotationGUI.OnEnable(base.serializedObject.FindProperty("m_LocalRotation"), new GUIContent("Rotation"));
			this.m_ShowLayoutOptions = EditorPrefs.GetBool("RectTransformEditor.showAnchorProperties", false);
			this.m_RawEditMode = EditorPrefs.GetBool("RectTransformEditor.lockRect", false);
			this.m_ChangingAnchors.valueChanged.AddListener(new UnityAction(this.RepaintScene));
			this.m_ChangingPivot.valueChanged.AddListener(new UnityAction(this.RepaintScene));
			this.m_ChangingWidth.valueChanged.AddListener(new UnityAction(this.RepaintScene));
			this.m_ChangingHeight.valueChanged.AddListener(new UnityAction(this.RepaintScene));
			this.m_ChangingPosX.valueChanged.AddListener(new UnityAction(this.RepaintScene));
			this.m_ChangingPosY.valueChanged.AddListener(new UnityAction(this.RepaintScene));
			this.m_ChangingLeft.valueChanged.AddListener(new UnityAction(this.RepaintScene));
			this.m_ChangingRight.valueChanged.AddListener(new UnityAction(this.RepaintScene));
			this.m_ChangingTop.valueChanged.AddListener(new UnityAction(this.RepaintScene));
			this.m_ChangingBottom.valueChanged.AddListener(new UnityAction(this.RepaintScene));
			ManipulationToolUtility.handleDragChange = (ManipulationToolUtility.HandleDragChange)Delegate.Combine(ManipulationToolUtility.handleDragChange, new ManipulationToolUtility.HandleDragChange(this.HandleDragChange));
		}

		private void OnDisable()
		{
			this.m_ChangingAnchors.valueChanged.RemoveListener(new UnityAction(this.RepaintScene));
			this.m_ChangingPivot.valueChanged.RemoveListener(new UnityAction(this.RepaintScene));
			this.m_ChangingWidth.valueChanged.RemoveListener(new UnityAction(this.RepaintScene));
			this.m_ChangingHeight.valueChanged.RemoveListener(new UnityAction(this.RepaintScene));
			this.m_ChangingPosX.valueChanged.RemoveListener(new UnityAction(this.RepaintScene));
			this.m_ChangingPosY.valueChanged.RemoveListener(new UnityAction(this.RepaintScene));
			this.m_ChangingLeft.valueChanged.RemoveListener(new UnityAction(this.RepaintScene));
			this.m_ChangingRight.valueChanged.RemoveListener(new UnityAction(this.RepaintScene));
			this.m_ChangingTop.valueChanged.RemoveListener(new UnityAction(this.RepaintScene));
			this.m_ChangingBottom.valueChanged.RemoveListener(new UnityAction(this.RepaintScene));
			ManipulationToolUtility.handleDragChange = (ManipulationToolUtility.HandleDragChange)Delegate.Remove(ManipulationToolUtility.handleDragChange, new ManipulationToolUtility.HandleDragChange(this.HandleDragChange));
			if (this.m_DropdownWindow != null && this.m_DropdownWindow.editorWindow != null)
			{
				this.m_DropdownWindow.editorWindow.Close();
			}
		}

		private void HandleDragChange(string handleName, bool dragging)
		{
			AnimBool animBool;
			switch (handleName)
			{
			case "ChangingLeft":
				animBool = this.m_ChangingLeft;
				goto IL_143;
			case "ChangingRight":
				animBool = this.m_ChangingRight;
				goto IL_143;
			case "ChangingPosY":
				animBool = this.m_ChangingPosY;
				goto IL_143;
			case "ChangingWidth":
				animBool = this.m_ChangingWidth;
				goto IL_143;
			case "ChangingBottom":
				animBool = this.m_ChangingBottom;
				goto IL_143;
			case "ChangingTop":
				animBool = this.m_ChangingTop;
				goto IL_143;
			case "ChangingPosX":
				animBool = this.m_ChangingPosX;
				goto IL_143;
			case "ChangingHeight":
				animBool = this.m_ChangingHeight;
				goto IL_143;
			case "ChangingPivot":
				animBool = this.m_ChangingPivot;
				goto IL_143;
			}
			animBool = null;
			IL_143:
			if (animBool != null)
			{
				animBool.target = dragging;
			}
		}

		private void SetFadingBasedOnMouseDownUp(ref AnimBool animBool, Event eventBefore)
		{
			if (eventBefore.type == EventType.MouseDrag && Event.current.type != EventType.MouseDrag)
			{
				animBool.value = true;
			}
			else if (eventBefore.type == EventType.MouseUp && Event.current.type != EventType.MouseUp)
			{
				animBool.target = false;
			}
		}

		private void SetFadingBasedOnControlID(ref AnimBool animBool, int id)
		{
			GUIView y = (!(EditorWindow.focusedWindow == null)) ? EditorWindow.focusedWindow.m_Parent : null;
			if (GUIUtility.keyboardControl == id && GUIView.current == y)
			{
				animBool.value = true;
				this.m_KeyboardControlIDs[id] = animBool;
			}
			else if ((GUIUtility.keyboardControl != id || GUIView.current != y) && this.m_KeyboardControlIDs.ContainsKey(id))
			{
				this.m_KeyboardControlIDs.Remove(id);
				if (!this.m_KeyboardControlIDs.ContainsValue(animBool))
				{
					animBool.target = false;
				}
			}
		}

		private void RepaintScene()
		{
			SceneView.RepaintAll();
		}

		private static bool ShouldDoIntSnapping(RectTransform rect)
		{
			Canvas componentInParent = rect.gameObject.GetComponentInParent<Canvas>();
			return componentInParent != null && componentInParent.renderMode != RenderMode.WorldSpace;
		}

		public override void OnInspectorGUI()
		{
			if (!EditorGUIUtility.wideMode)
			{
				EditorGUIUtility.wideMode = true;
				EditorGUIUtility.labelWidth = EditorGUIUtility.currentViewWidth - 212f;
			}
			bool flag = false;
			bool anyDrivenX = false;
			bool anyDrivenY = false;
			bool anyWithoutParent = false;
			UnityEngine.Object[] targets = base.targets;
			for (int i = 0; i < targets.Length; i++)
			{
				RectTransform rectTransform = (RectTransform)targets[i];
				if (rectTransform.drivenByObject != null)
				{
					flag = true;
					if ((rectTransform.drivenProperties & (DrivenTransformProperties.AnchoredPositionX | DrivenTransformProperties.SizeDeltaX)) != DrivenTransformProperties.None)
					{
						anyDrivenX = true;
					}
					if ((rectTransform.drivenProperties & (DrivenTransformProperties.AnchoredPositionY | DrivenTransformProperties.SizeDeltaY)) != DrivenTransformProperties.None)
					{
						anyDrivenY = true;
					}
				}
				PrefabType prefabType = PrefabUtility.GetPrefabType(rectTransform.gameObject);
				if ((rectTransform.transform.parent == null || rectTransform.transform.parent.GetComponent<RectTransform>() == null) && prefabType != PrefabType.Prefab && prefabType != PrefabType.ModelPrefab)
				{
					anyWithoutParent = true;
				}
			}
			if (flag)
			{
				if (base.targets.Length == 1)
				{
					EditorGUILayout.HelpBox("Some values driven by " + (base.target as RectTransform).drivenByObject.GetType().Name + ".", MessageType.None);
				}
				else
				{
					EditorGUILayout.HelpBox("Some values in some or all objects are driven.", MessageType.None);
				}
			}
			base.serializedObject.Update();
			this.LayoutDropdownButton(anyWithoutParent);
			this.SmartPositionAndSizeFields(anyWithoutParent, anyDrivenX, anyDrivenY);
			this.SmartAnchorFields();
			this.SmartPivotField();
			EditorGUILayout.Space();
			this.m_RotationGUI.RotationField(base.targets.Any((UnityEngine.Object x) => ((x as RectTransform).drivenProperties & DrivenTransformProperties.Rotation) != DrivenTransformProperties.None));
			RectTransformEditor.s_ScaleDisabledMask[0] = base.targets.Any((UnityEngine.Object x) => ((x as RectTransform).drivenProperties & DrivenTransformProperties.ScaleX) != DrivenTransformProperties.None);
			RectTransformEditor.s_ScaleDisabledMask[1] = base.targets.Any((UnityEngine.Object x) => ((x as RectTransform).drivenProperties & DrivenTransformProperties.ScaleY) != DrivenTransformProperties.None);
			RectTransformEditor.s_ScaleDisabledMask[2] = base.targets.Any((UnityEngine.Object x) => ((x as RectTransform).drivenProperties & DrivenTransformProperties.ScaleZ) != DrivenTransformProperties.None);
			RectTransformEditor.Vector3FieldWithDisabledMash(EditorGUILayout.GetControlRect(new GUILayoutOption[0]), this.m_LocalScale, RectTransformEditor.styles.transformScaleContent, RectTransformEditor.s_ScaleDisabledMask);
			base.serializedObject.ApplyModifiedProperties();
		}

		private static void Vector3FieldWithDisabledMash(Rect position, SerializedProperty property, GUIContent label, bool[] disabledMask)
		{
			int controlID = GUIUtility.GetControlID(RectTransformEditor.s_FoldoutHash, FocusType.Keyboard, position);
			position = EditorGUI.MultiFieldPrefixLabel(position, controlID, label, 3);
			position.height = EditorGUIUtility.singleLineHeight;
			SerializedProperty serializedProperty = property.Copy();
			serializedProperty.NextVisible(true);
			EditorGUI.MultiPropertyField(position, RectTransformEditor.s_XYZLabels, serializedProperty, 13f, disabledMask);
		}

		private void LayoutDropdownButton(bool anyWithoutParent)
		{
			Rect rect = GUILayoutUtility.GetRect(0f, 0f);
			rect.x += 2f;
			rect.y += 17f;
			rect.height = RectTransformEditor.kDropdownSize;
			rect.width = RectTransformEditor.kDropdownSize;
			using (new EditorGUI.DisabledScope(anyWithoutParent))
			{
				Color color = GUI.color;
				GUI.color = new Color(1f, 1f, 1f, 0.6f) * color;
				if (EditorGUI.ButtonMouseDown(rect, GUIContent.none, FocusType.Passive, "box"))
				{
					GUIUtility.keyboardControl = 0;
					this.m_DropdownWindow = new LayoutDropdownWindow(base.serializedObject);
					PopupWindow.Show(rect, this.m_DropdownWindow);
				}
				GUI.color = color;
			}
			if (!anyWithoutParent)
			{
				LayoutDropdownWindow.DrawLayoutMode(new RectOffset(7, 7, 7, 7).Remove(rect), this.m_AnchorMin, this.m_AnchorMax, this.m_AnchoredPosition, this.m_SizeDelta);
				LayoutDropdownWindow.DrawLayoutModeHeadersOutsideRect(rect, this.m_AnchorMin, this.m_AnchorMax, this.m_AnchoredPosition, this.m_SizeDelta);
			}
		}

		private void SmartPositionAndSizeFields(bool anyWithoutParent, bool anyDrivenX, bool anyDrivenY)
		{
			Rect controlRect = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight * 4f, new GUILayoutOption[0]);
			controlRect.height = EditorGUIUtility.singleLineHeight * 2f;
			bool flag = base.targets.Any((UnityEngine.Object x) => (x as RectTransform).anchorMin.x != (x as RectTransform).anchorMax.x);
			bool flag2 = base.targets.Any((UnityEngine.Object x) => (x as RectTransform).anchorMin.y != (x as RectTransform).anchorMax.y);
			bool flag3 = base.targets.Any((UnityEngine.Object x) => (x as RectTransform).anchorMin.x == (x as RectTransform).anchorMax.x);
			bool flag4 = base.targets.Any((UnityEngine.Object x) => (x as RectTransform).anchorMin.y == (x as RectTransform).anchorMax.y);
			Rect rect = this.GetColumnRect(controlRect, 0);
			if (flag3 || anyWithoutParent || anyDrivenX)
			{
				EditorGUI.BeginProperty(rect, null, this.m_AnchoredPosition.FindPropertyRelative("x"));
				this.FloatFieldLabelAbove(rect, (RectTransform rectTransform) => rectTransform.anchoredPosition.x, delegate(RectTransform rectTransform, float val)
				{
					rectTransform.anchoredPosition = new Vector2(val, rectTransform.anchoredPosition.y);
				}, DrivenTransformProperties.AnchoredPositionX, new GUIContent("Pos X"));
				this.SetFadingBasedOnControlID(ref this.m_ChangingPosX, EditorGUIUtility.s_LastControlID);
				EditorGUI.EndProperty();
			}
			else
			{
				EditorGUI.BeginProperty(rect, null, this.m_AnchoredPosition.FindPropertyRelative("x"));
				EditorGUI.BeginProperty(rect, null, this.m_SizeDelta.FindPropertyRelative("x"));
				this.FloatFieldLabelAbove(rect, (RectTransform rectTransform) => rectTransform.offsetMin.x, delegate(RectTransform rectTransform, float val)
				{
					rectTransform.offsetMin = new Vector2(val, rectTransform.offsetMin.y);
				}, DrivenTransformProperties.None, new GUIContent("Left"));
				this.SetFadingBasedOnControlID(ref this.m_ChangingLeft, EditorGUIUtility.s_LastControlID);
				EditorGUI.EndProperty();
				EditorGUI.EndProperty();
			}
			rect = this.GetColumnRect(controlRect, 1);
			if (flag4 || anyWithoutParent || anyDrivenY)
			{
				EditorGUI.BeginProperty(rect, null, this.m_AnchoredPosition.FindPropertyRelative("y"));
				this.FloatFieldLabelAbove(rect, (RectTransform rectTransform) => rectTransform.anchoredPosition.y, delegate(RectTransform rectTransform, float val)
				{
					rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, val);
				}, DrivenTransformProperties.AnchoredPositionY, new GUIContent("Pos Y"));
				this.SetFadingBasedOnControlID(ref this.m_ChangingPosY, EditorGUIUtility.s_LastControlID);
				EditorGUI.EndProperty();
			}
			else
			{
				EditorGUI.BeginProperty(rect, null, this.m_AnchoredPosition.FindPropertyRelative("y"));
				EditorGUI.BeginProperty(rect, null, this.m_SizeDelta.FindPropertyRelative("y"));
				this.FloatFieldLabelAbove(rect, (RectTransform rectTransform) => -rectTransform.offsetMax.y, delegate(RectTransform rectTransform, float val)
				{
					rectTransform.offsetMax = new Vector2(rectTransform.offsetMax.x, -val);
				}, DrivenTransformProperties.None, new GUIContent("Top"));
				this.SetFadingBasedOnControlID(ref this.m_ChangingTop, EditorGUIUtility.s_LastControlID);
				EditorGUI.EndProperty();
				EditorGUI.EndProperty();
			}
			rect = this.GetColumnRect(controlRect, 2);
			EditorGUI.BeginProperty(rect, null, this.m_LocalPositionZ);
			this.FloatFieldLabelAbove(rect, (RectTransform rectTransform) => rectTransform.transform.localPosition.z, delegate(RectTransform rectTransform, float val)
			{
				rectTransform.transform.localPosition = new Vector3(rectTransform.transform.localPosition.x, rectTransform.transform.localPosition.y, val);
			}, DrivenTransformProperties.AnchoredPositionZ, new GUIContent("Pos Z"));
			EditorGUI.EndProperty();
			controlRect.y += EditorGUIUtility.singleLineHeight * 2f;
			rect = this.GetColumnRect(controlRect, 0);
			if (flag3 || anyWithoutParent || anyDrivenX)
			{
				EditorGUI.BeginProperty(rect, null, this.m_SizeDelta.FindPropertyRelative("x"));
				this.FloatFieldLabelAbove(rect, (RectTransform rectTransform) => rectTransform.sizeDelta.x, delegate(RectTransform rectTransform, float val)
				{
					rectTransform.sizeDelta = new Vector2(val, rectTransform.sizeDelta.y);
				}, DrivenTransformProperties.SizeDeltaX, (!flag) ? new GUIContent("Width") : new GUIContent("W Delta"));
				this.SetFadingBasedOnControlID(ref this.m_ChangingWidth, EditorGUIUtility.s_LastControlID);
				EditorGUI.EndProperty();
			}
			else
			{
				EditorGUI.BeginProperty(rect, null, this.m_AnchoredPosition.FindPropertyRelative("x"));
				EditorGUI.BeginProperty(rect, null, this.m_SizeDelta.FindPropertyRelative("x"));
				this.FloatFieldLabelAbove(rect, (RectTransform rectTransform) => -rectTransform.offsetMax.x, delegate(RectTransform rectTransform, float val)
				{
					rectTransform.offsetMax = new Vector2(-val, rectTransform.offsetMax.y);
				}, DrivenTransformProperties.None, new GUIContent("Right"));
				this.SetFadingBasedOnControlID(ref this.m_ChangingRight, EditorGUIUtility.s_LastControlID);
				EditorGUI.EndProperty();
				EditorGUI.EndProperty();
			}
			rect = this.GetColumnRect(controlRect, 1);
			if (flag4 || anyWithoutParent || anyDrivenY)
			{
				EditorGUI.BeginProperty(rect, null, this.m_SizeDelta.FindPropertyRelative("y"));
				this.FloatFieldLabelAbove(rect, (RectTransform rectTransform) => rectTransform.sizeDelta.y, delegate(RectTransform rectTransform, float val)
				{
					rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, val);
				}, DrivenTransformProperties.SizeDeltaY, (!flag2) ? new GUIContent("Height") : new GUIContent("H Delta"));
				this.SetFadingBasedOnControlID(ref this.m_ChangingHeight, EditorGUIUtility.s_LastControlID);
				EditorGUI.EndProperty();
			}
			else
			{
				EditorGUI.BeginProperty(rect, null, this.m_AnchoredPosition.FindPropertyRelative("y"));
				EditorGUI.BeginProperty(rect, null, this.m_SizeDelta.FindPropertyRelative("y"));
				this.FloatFieldLabelAbove(rect, (RectTransform rectTransform) => rectTransform.offsetMin.y, delegate(RectTransform rectTransform, float val)
				{
					rectTransform.offsetMin = new Vector2(rectTransform.offsetMin.x, val);
				}, DrivenTransformProperties.None, new GUIContent("Bottom"));
				this.SetFadingBasedOnControlID(ref this.m_ChangingBottom, EditorGUIUtility.s_LastControlID);
				EditorGUI.EndProperty();
				EditorGUI.EndProperty();
			}
			rect = controlRect;
			rect.height = EditorGUIUtility.singleLineHeight;
			rect.y += EditorGUIUtility.singleLineHeight;
			rect.yMin -= 2f;
			rect.xMin = rect.xMax - 26f;
			rect.x -= rect.width;
			this.BlueprintButton(rect);
			rect.x += rect.width;
			this.RawButton(rect);
		}

		private void SmartAnchorFields()
		{
			Rect controlRect = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight * (float)((!this.m_ShowLayoutOptions) ? 1 : 3), new GUILayoutOption[0]);
			controlRect.height = EditorGUIUtility.singleLineHeight;
			EditorGUI.BeginChangeCheck();
			this.m_ShowLayoutOptions = EditorGUI.Foldout(controlRect, this.m_ShowLayoutOptions, RectTransformEditor.styles.anchorsContent);
			if (EditorGUI.EndChangeCheck())
			{
				EditorPrefs.SetBool("RectTransformEditor.showAnchorProperties", this.m_ShowLayoutOptions);
			}
			if (this.m_ShowLayoutOptions)
			{
				EditorGUI.indentLevel++;
				controlRect.y += EditorGUIUtility.singleLineHeight;
				this.Vector2Field(controlRect, (RectTransform rectTransform) => rectTransform.anchorMin.x, delegate(RectTransform rectTransform, float val)
				{
					RectTransformEditor.SetAnchorSmart(rectTransform, val, 0, false, !this.m_RawEditMode, true);
				}, (RectTransform rectTransform) => rectTransform.anchorMin.y, delegate(RectTransform rectTransform, float val)
				{
					RectTransformEditor.SetAnchorSmart(rectTransform, val, 1, false, !this.m_RawEditMode, true);
				}, DrivenTransformProperties.AnchorMinX, DrivenTransformProperties.AnchorMinY, this.m_AnchorMin.FindPropertyRelative("x"), this.m_AnchorMin.FindPropertyRelative("y"), RectTransformEditor.styles.anchorMinContent);
				controlRect.y += EditorGUIUtility.singleLineHeight;
				this.Vector2Field(controlRect, (RectTransform rectTransform) => rectTransform.anchorMax.x, delegate(RectTransform rectTransform, float val)
				{
					RectTransformEditor.SetAnchorSmart(rectTransform, val, 0, true, !this.m_RawEditMode, true);
				}, (RectTransform rectTransform) => rectTransform.anchorMax.y, delegate(RectTransform rectTransform, float val)
				{
					RectTransformEditor.SetAnchorSmart(rectTransform, val, 1, true, !this.m_RawEditMode, true);
				}, DrivenTransformProperties.AnchorMaxX, DrivenTransformProperties.AnchorMaxY, this.m_AnchorMax.FindPropertyRelative("x"), this.m_AnchorMax.FindPropertyRelative("y"), RectTransformEditor.styles.anchorMaxContent);
				EditorGUI.indentLevel--;
			}
		}

		private void SmartPivotField()
		{
			this.Vector2Field(EditorGUILayout.GetControlRect(new GUILayoutOption[0]), (RectTransform rectTransform) => rectTransform.pivot.x, delegate(RectTransform rectTransform, float val)
			{
				RectTransformEditor.SetPivotSmart(rectTransform, val, 0, !this.m_RawEditMode, false);
			}, (RectTransform rectTransform) => rectTransform.pivot.y, delegate(RectTransform rectTransform, float val)
			{
				RectTransformEditor.SetPivotSmart(rectTransform, val, 1, !this.m_RawEditMode, false);
			}, DrivenTransformProperties.PivotX, DrivenTransformProperties.PivotY, this.m_Pivot.FindPropertyRelative("x"), this.m_Pivot.FindPropertyRelative("y"), RectTransformEditor.styles.pivotContent);
		}

		private void RawButton(Rect position)
		{
			EditorGUI.BeginChangeCheck();
			this.m_RawEditMode = GUI.Toggle(position, this.m_RawEditMode, RectTransformEditor.styles.rawEditContent, "ButtonRight");
			if (EditorGUI.EndChangeCheck())
			{
				EditorPrefs.SetBool("RectTransformEditor.lockRect", this.m_RawEditMode);
			}
		}

		private void BlueprintButton(Rect position)
		{
			EditorGUI.BeginChangeCheck();
			bool rectBlueprintMode = GUI.Toggle(position, Tools.rectBlueprintMode, RectTransformEditor.styles.blueprintContent, "ButtonLeft");
			if (EditorGUI.EndChangeCheck())
			{
				Tools.rectBlueprintMode = rectBlueprintMode;
				Tools.RepaintAllToolViews();
			}
		}

		private void FloatFieldLabelAbove(Rect position, RectTransformEditor.FloatGetter getter, RectTransformEditor.FloatSetter setter, DrivenTransformProperties driven, GUIContent label)
		{
			using (new EditorGUI.DisabledScope(base.targets.Any((UnityEngine.Object x) => ((x as RectTransform).drivenProperties & driven) != DrivenTransformProperties.None)))
			{
				float value = getter(base.target as RectTransform);
				EditorGUI.showMixedValue = ((from x in base.targets
				select getter(x as RectTransform)).Distinct<float>().Count<float>() >= 2);
				EditorGUI.BeginChangeCheck();
				int controlID = GUIUtility.GetControlID(RectTransformEditor.s_FloatFieldHash, FocusType.Keyboard, position);
				Rect rect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
				Rect position2 = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight, position.width, EditorGUIUtility.singleLineHeight);
				EditorGUI.HandlePrefixLabel(position, rect, label, controlID);
				float f = EditorGUI.DoFloatField(EditorGUI.s_RecycledEditor, position2, rect, controlID, value, EditorGUI.kFloatFieldFormatString, EditorStyles.textField, true);
				if (EditorGUI.EndChangeCheck())
				{
					Undo.RecordObjects(base.targets, "Inspector");
					UnityEngine.Object[] targets = base.targets;
					for (int i = 0; i < targets.Length; i++)
					{
						RectTransform rect2 = (RectTransform)targets[i];
						setter(rect2, f);
					}
				}
			}
		}

		private void Vector2Field(Rect position, RectTransformEditor.FloatGetter xGetter, RectTransformEditor.FloatSetter xSetter, RectTransformEditor.FloatGetter yGetter, RectTransformEditor.FloatSetter ySetter, DrivenTransformProperties xDriven, DrivenTransformProperties yDriven, SerializedProperty xProperty, SerializedProperty yProperty, GUIContent label)
		{
			EditorGUI.PrefixLabel(position, -1, label);
			float labelWidth = EditorGUIUtility.labelWidth;
			int indentLevel = EditorGUI.indentLevel;
			Rect columnRect = this.GetColumnRect(position, 0);
			Rect columnRect2 = this.GetColumnRect(position, 1);
			EditorGUIUtility.labelWidth = 13f;
			EditorGUI.indentLevel = 0;
			EditorGUI.BeginProperty(columnRect, RectTransformEditor.s_XYLabels[0], xProperty);
			this.FloatField(columnRect, xGetter, xSetter, xDriven, RectTransformEditor.s_XYLabels[0]);
			EditorGUI.EndProperty();
			EditorGUI.BeginProperty(columnRect, RectTransformEditor.s_XYLabels[1], yProperty);
			this.FloatField(columnRect2, yGetter, ySetter, yDriven, RectTransformEditor.s_XYLabels[1]);
			EditorGUI.EndProperty();
			EditorGUIUtility.labelWidth = labelWidth;
			EditorGUI.indentLevel = indentLevel;
		}

		private void FloatField(Rect position, RectTransformEditor.FloatGetter getter, RectTransformEditor.FloatSetter setter, DrivenTransformProperties driven, GUIContent label)
		{
			using (new EditorGUI.DisabledScope(base.targets.Any((UnityEngine.Object x) => ((x as RectTransform).drivenProperties & driven) != DrivenTransformProperties.None)))
			{
				float value = getter(base.target as RectTransform);
				EditorGUI.showMixedValue = ((from x in base.targets
				select getter(x as RectTransform)).Distinct<float>().Count<float>() >= 2);
				EditorGUI.BeginChangeCheck();
				float f = EditorGUI.FloatField(position, label, value);
				if (EditorGUI.EndChangeCheck())
				{
					Undo.RecordObjects(base.targets, "Inspector");
					UnityEngine.Object[] targets = base.targets;
					for (int i = 0; i < targets.Length; i++)
					{
						RectTransform rect = (RectTransform)targets[i];
						setter(rect, f);
					}
				}
			}
		}

		private Rect GetColumnRect(Rect totalRect, int column)
		{
			totalRect.xMin += EditorGUIUtility.labelWidth - 1f;
			Rect result = totalRect;
			result.xMin += (totalRect.width - 4f) * ((float)column / 3f) + (float)(column * 2);
			result.width = (totalRect.width - 4f) / 3f;
			return result;
		}

		private int AnchorPopup(Rect position, string label, int selected, string[] displayedOptions)
		{
			EditorGUIUtility.labelWidth = 12f;
			int result = EditorGUI.Popup(position, label, selected, displayedOptions);
			EditorGUIUtility.labelWidth = 0f;
			return result;
		}

		private void DrawRect(Rect rect, Transform space, bool dotted)
		{
			Vector3 vector = space.TransformPoint(new Vector2(rect.x, rect.y));
			Vector3 vector2 = space.TransformPoint(new Vector2(rect.x, rect.yMax));
			Vector3 vector3 = space.TransformPoint(new Vector2(rect.xMax, rect.yMax));
			Vector3 vector4 = space.TransformPoint(new Vector2(rect.xMax, rect.y));
			if (!dotted)
			{
				Handles.DrawLine(vector, vector2);
				Handles.DrawLine(vector2, vector3);
				Handles.DrawLine(vector3, vector4);
				Handles.DrawLine(vector4, vector);
			}
			else
			{
				RectHandles.DrawDottedLineWithShadow(RectTransformEditor.kShadowColor, RectTransformEditor.kShadowOffset, vector, vector2, 5f);
				RectHandles.DrawDottedLineWithShadow(RectTransformEditor.kShadowColor, RectTransformEditor.kShadowOffset, vector2, vector3, 5f);
				RectHandles.DrawDottedLineWithShadow(RectTransformEditor.kShadowColor, RectTransformEditor.kShadowOffset, vector3, vector4, 5f);
				RectHandles.DrawDottedLineWithShadow(RectTransformEditor.kShadowColor, RectTransformEditor.kShadowOffset, vector4, vector, 5f);
			}
		}

		private void OnSceneGUI()
		{
			RectTransform rectTransform = base.target as RectTransform;
			Rect rect = rectTransform.rect;
			Rect rectInUserSpace = rect;
			Rect rect2 = rect;
			Transform transform = rectTransform.transform;
			Transform userSpace = transform;
			Transform transform2 = transform;
			RectTransform rectTransform2 = null;
			if (transform.parent != null)
			{
				transform2 = transform.parent;
				rect2.x += transform.localPosition.x;
				rect2.y += transform.localPosition.y;
				rectTransform2 = transform2.GetComponent<RectTransform>();
			}
			if (Tools.rectBlueprintMode)
			{
				userSpace = transform2;
				rectInUserSpace = rect2;
			}
			float num = Mathf.Max(this.m_ChangingAnchors.faded, this.m_ChangingPivot.faded);
			if (rectTransform.anchorMin != rectTransform.anchorMax)
			{
				num = Mathf.Max(new float[]
				{
					num,
					this.m_ChangingPosX.faded,
					this.m_ChangingPosY.faded,
					this.m_ChangingLeft.faded,
					this.m_ChangingRight.faded,
					this.m_ChangingTop.faded,
					this.m_ChangingBottom.faded
				});
			}
			Color color = RectTransformEditor.kRectInParentSpaceColor;
			color.a *= num;
			Handles.color = color;
			this.DrawRect(rect2, transform2, true);
			if (this.m_TargetCount == 1)
			{
				RectTransformSnapping.OnGUI();
				if (rectTransform2 != null)
				{
					this.AllAnchorsSceneGUI(rectTransform, rectTransform2, transform2, transform);
				}
				this.DrawSizes(rectInUserSpace, userSpace, rect2, transform2, rectTransform, rectTransform2);
				RectTransformSnapping.DrawGuides();
				if (Tools.current == Tool.Rect)
				{
					this.ParentRectPreviewDragHandles(rectTransform2, transform2);
				}
			}
		}

		private void ParentRectPreviewDragHandles(RectTransform gui, Transform space)
		{
			if (!(gui == null))
			{
				float size = 0.05f * HandleUtility.GetHandleSize(space.position);
				Rect rect = gui.rect;
				for (int i = 0; i <= 2; i++)
				{
					for (int j = 0; j <= 2; j++)
					{
						if (i == 1 != (j == 1))
						{
							Vector3 position = Vector2.zero;
							for (int k = 0; k < 2; k++)
							{
								position[k] = Mathf.Lerp(rect.min[k], rect.max[k], (float)((k != 0) ? j : i) * 0.5f);
							}
							position = space.TransformPoint(position);
							int controlID = GUIUtility.GetControlID(RectTransformEditor.s_ParentRectPreviewHandlesHash, FocusType.Passive);
							Vector3 sideVector = (i != 1) ? (space.up * rect.height) : (space.right * rect.width);
							Vector3 direction = (i != 1) ? space.right : space.up;
							EditorGUI.BeginChangeCheck();
							Vector3 position2 = RectHandles.SideSlider(controlID, position, sideVector, direction, size, null, 0f, -3f);
							if (EditorGUI.EndChangeCheck())
							{
								Vector2 b = space.InverseTransformPoint(position);
								Vector2 a = space.InverseTransformPoint(position2);
								Rect rect2 = rect;
								Vector2 vector = a - b;
								if (i == 0)
								{
									rect2.min = new Vector2(rect2.min.x + vector.x, rect2.min.y);
								}
								if (i == 2)
								{
									rect2.max = new Vector2(rect2.max.x + vector.x, rect2.max.y);
								}
								if (j == 0)
								{
									rect2.min = new Vector2(rect2.min.x, rect2.min.y + vector.y);
								}
								if (j == 2)
								{
									rect2.max = new Vector2(rect2.max.x, rect2.max.y + vector.y);
								}
								this.SetTemporaryRect(gui, rect2, controlID);
							}
							if (GUIUtility.hotControl == controlID)
							{
								Handles.BeginGUI();
								EditorGUI.DropShadowLabel(new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y, 60f, 16f), "Preview");
								Handles.EndGUI();
							}
						}
					}
				}
			}
		}

		private void SetTemporaryRect(RectTransform gui, Rect rect, int id)
		{
			if (RectTransformEditor.s_ParentDragRectTransform == null)
			{
				RectTransformEditor.s_ParentDragRectTransform = gui;
				RectTransformEditor.s_ParentDragOrigRect = gui.rect;
				RectTransformEditor.s_ParentDragId = (float)id;
			}
			else if (RectTransformEditor.s_ParentDragRectTransform != gui)
			{
				return;
			}
			RectTransformEditor.s_ParentDragPreviewRect = rect;
			RectTransformEditor.s_ParentDragTime = Time.realtimeSinceStartup;
			InternalEditorUtility.SetRectTransformTemporaryRect(gui, rect);
			EditorApplication.update = (EditorApplication.CallbackFunction)Delegate.Remove(EditorApplication.update, new EditorApplication.CallbackFunction(this.UpdateTemporaryRect));
			EditorApplication.update = (EditorApplication.CallbackFunction)Delegate.Combine(EditorApplication.update, new EditorApplication.CallbackFunction(this.UpdateTemporaryRect));
		}

		private void UpdateTemporaryRect()
		{
			if (!(RectTransformEditor.s_ParentDragRectTransform == null))
			{
				if ((float)GUIUtility.hotControl == RectTransformEditor.s_ParentDragId)
				{
					RectTransformEditor.s_ParentDragTime = Time.realtimeSinceStartup;
					Canvas.ForceUpdateCanvases();
					GameView.RepaintAll();
				}
				else
				{
					float num = Time.realtimeSinceStartup - RectTransformEditor.s_ParentDragTime;
					float num2 = Mathf.Clamp01(1f - num * 8f);
					if (num2 > 0f)
					{
						Rect rect = default(Rect);
						rect.position = Vector2.Lerp(RectTransformEditor.s_ParentDragOrigRect.position, RectTransformEditor.s_ParentDragPreviewRect.position, num2);
						rect.size = Vector2.Lerp(RectTransformEditor.s_ParentDragOrigRect.size, RectTransformEditor.s_ParentDragPreviewRect.size, num2);
						InternalEditorUtility.SetRectTransformTemporaryRect(RectTransformEditor.s_ParentDragRectTransform, rect);
					}
					else
					{
						InternalEditorUtility.SetRectTransformTemporaryRect(RectTransformEditor.s_ParentDragRectTransform, default(Rect));
						EditorApplication.update = (EditorApplication.CallbackFunction)Delegate.Remove(EditorApplication.update, new EditorApplication.CallbackFunction(this.UpdateTemporaryRect));
						RectTransformEditor.s_ParentDragRectTransform = null;
					}
					Canvas.ForceUpdateCanvases();
					SceneView.RepaintAll();
					GameView.RepaintAll();
				}
			}
		}

		private void AllAnchorsSceneGUI(RectTransform gui, RectTransform guiParent, Transform parentSpace, Transform transform)
		{
			Handles.color = RectTransformEditor.kParentColor;
			this.DrawRect(guiParent.rect, parentSpace, false);
			Handles.color = RectTransformEditor.kSiblingColor;
			IEnumerator enumerator = parentSpace.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					Transform transform2 = (Transform)enumerator.Current;
					if (transform2.gameObject.activeInHierarchy)
					{
						RectTransform component = transform2.GetComponent<RectTransform>();
						if (component)
						{
							Rect rect = component.rect;
							rect.x += component.transform.localPosition.x;
							rect.y += component.transform.localPosition.y;
							this.DrawRect(component.rect, component, false);
							if (component != transform)
							{
								this.AnchorsSceneGUI(component, guiParent, parentSpace, false);
							}
						}
					}
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
			Handles.color = RectTransformEditor.kAnchorColor;
			this.AnchorsSceneGUI(gui, guiParent, parentSpace, true);
		}

		private Vector3 GetAnchorLocal(RectTransform guiParent, Vector2 anchor)
		{
			return RectTransformEditor.NormalizedToPointUnclamped(guiParent.rect, anchor);
		}

		private static Vector2 NormalizedToPointUnclamped(Rect rectangle, Vector2 normalizedRectCoordinates)
		{
			return new Vector2(Mathf.LerpUnclamped(rectangle.x, rectangle.xMax, normalizedRectCoordinates.x), Mathf.LerpUnclamped(rectangle.y, rectangle.yMax, normalizedRectCoordinates.y));
		}

		private static bool AnchorAllowedOutsideParent(int axis, int minmax)
		{
			bool result;
			if (EditorGUI.actionKey || GUIUtility.hotControl == 0)
			{
				result = true;
			}
			else
			{
				float num = (minmax != 0) ? RectTransformEditor.s_StartDragAnchorMax[axis] : RectTransformEditor.s_StartDragAnchorMin[axis];
				result = (num < -0.001f || num > 1.001f);
			}
			return result;
		}

		private void AnchorsSceneGUI(RectTransform gui, RectTransform guiParent, Transform parentSpace, bool interactive)
		{
			if (Event.current.type == EventType.MouseDown)
			{
				RectTransformEditor.s_AnchorFusedState = RectTransformEditor.AnchorFusedState.None;
				if (gui.anchorMin == gui.anchorMax)
				{
					RectTransformEditor.s_AnchorFusedState = RectTransformEditor.AnchorFusedState.All;
				}
				else if (gui.anchorMin.x == gui.anchorMax.x)
				{
					RectTransformEditor.s_AnchorFusedState = RectTransformEditor.AnchorFusedState.Horizontal;
				}
				else if (gui.anchorMin.y == gui.anchorMax.y)
				{
					RectTransformEditor.s_AnchorFusedState = RectTransformEditor.AnchorFusedState.Vertical;
				}
			}
			this.AnchorSceneGUI(gui, guiParent, parentSpace, interactive, 0, 0, GUIUtility.GetControlID(FocusType.Passive));
			this.AnchorSceneGUI(gui, guiParent, parentSpace, interactive, 0, 1, GUIUtility.GetControlID(FocusType.Passive));
			this.AnchorSceneGUI(gui, guiParent, parentSpace, interactive, 1, 0, GUIUtility.GetControlID(FocusType.Passive));
			this.AnchorSceneGUI(gui, guiParent, parentSpace, interactive, 1, 1, GUIUtility.GetControlID(FocusType.Passive));
			if (interactive)
			{
				int controlID = GUIUtility.GetControlID(FocusType.Passive);
				int controlID2 = GUIUtility.GetControlID(FocusType.Passive);
				int controlID3 = GUIUtility.GetControlID(FocusType.Passive);
				int controlID4 = GUIUtility.GetControlID(FocusType.Passive);
				int controlID5 = GUIUtility.GetControlID(FocusType.Passive);
				if (RectTransformEditor.s_AnchorFusedState == RectTransformEditor.AnchorFusedState.All)
				{
					this.AnchorSceneGUI(gui, guiParent, parentSpace, interactive, 2, 2, controlID);
				}
				if (RectTransformEditor.s_AnchorFusedState == RectTransformEditor.AnchorFusedState.Horizontal)
				{
					this.AnchorSceneGUI(gui, guiParent, parentSpace, interactive, 2, 0, controlID2);
					this.AnchorSceneGUI(gui, guiParent, parentSpace, interactive, 2, 1, controlID3);
				}
				if (RectTransformEditor.s_AnchorFusedState == RectTransformEditor.AnchorFusedState.Vertical)
				{
					this.AnchorSceneGUI(gui, guiParent, parentSpace, interactive, 0, 2, controlID4);
					this.AnchorSceneGUI(gui, guiParent, parentSpace, interactive, 1, 2, controlID5);
				}
			}
		}

		private void AnchorSceneGUI(RectTransform gui, RectTransform guiParent, Transform parentSpace, bool interactive, int minmaxX, int minmaxY, int id)
		{
			Vector3 vector = default(Vector2);
			vector.x = ((minmaxX != 0) ? gui.anchorMax.x : gui.anchorMin.x);
			vector.y = ((minmaxY != 0) ? gui.anchorMax.y : gui.anchorMin.y);
			vector = this.GetAnchorLocal(guiParent, vector);
			vector = parentSpace.TransformPoint(vector);
			float num = 0.05f * HandleUtility.GetHandleSize(vector);
			if (minmaxX < 2)
			{
				vector += parentSpace.right * num * (float)(minmaxX * 2 - 1);
			}
			if (minmaxY < 2)
			{
				vector += parentSpace.up * num * (float)(minmaxY * 2 - 1);
			}
			if (minmaxX < 2 && minmaxY < 2)
			{
				this.DrawAnchor(vector, parentSpace.right * num * 2f * (float)(minmaxX * 2 - 1), parentSpace.up * num * 2f * (float)(minmaxY * 2 - 1));
			}
			if (interactive)
			{
				Event @event = new Event(Event.current);
				EditorGUI.BeginChangeCheck();
				Vector3 vector2 = Handles.Slider2D(id, vector, parentSpace.forward, parentSpace.right, parentSpace.up, num, null, Vector2.zero);
				if (@event.type == EventType.MouseDown && GUIUtility.hotControl == id)
				{
					RectTransformEditor.s_DragAnchorsTogether = EditorGUI.actionKey;
					RectTransformEditor.s_StartDragAnchorMin = gui.anchorMin;
					RectTransformEditor.s_StartDragAnchorMax = gui.anchorMax;
					RectTransformSnapping.CalculateAnchorSnapValues(parentSpace, gui.transform, gui, minmaxX, minmaxY);
				}
				if (EditorGUI.EndChangeCheck())
				{
					Undo.RecordObject(gui, "Move Rectangle Anchors");
					Vector2 vector3 = parentSpace.InverseTransformVector(vector2 - vector);
					for (int i = 0; i <= 1; i++)
					{
						int index;
						vector3[index = i] = vector3[index] / guiParent.rect.size[i];
						int num2 = (i != 0) ? minmaxY : minmaxX;
						bool flag = num2 == 1;
						float num3 = (!flag) ? gui.anchorMin[i] : gui.anchorMax[i];
						float num4 = num3 + vector3[i];
						float num5 = num4;
						if (!RectTransformEditor.AnchorAllowedOutsideParent(i, num2))
						{
							num5 = Mathf.Clamp01(num5);
						}
						if (num2 == 0)
						{
							num5 = Mathf.Min(num5, gui.anchorMax[i]);
						}
						if (num2 == 1)
						{
							num5 = Mathf.Max(num5, gui.anchorMin[i]);
						}
						float num6 = HandleUtility.GetHandleSize(vector2) * 0.05f / guiParent.rect.size[i];
						num6 *= parentSpace.InverseTransformVector((i != 0) ? Vector3.up : Vector3.right)[i];
						num5 = RectTransformSnapping.SnapToGuides(num5, num6, i);
						bool enforceExactValue = num5 != num4;
						num4 = num5;
						if (num2 == 2)
						{
							RectTransformEditor.SetAnchorSmart(gui, num4, i, false, !@event.shift, enforceExactValue, false, RectTransformEditor.s_DragAnchorsTogether);
							RectTransformEditor.SetAnchorSmart(gui, num4, i, true, !@event.shift, enforceExactValue, false, RectTransformEditor.s_DragAnchorsTogether);
						}
						else
						{
							RectTransformEditor.SetAnchorSmart(gui, num4, i, flag, !@event.shift, enforceExactValue, true, RectTransformEditor.s_DragAnchorsTogether);
						}
						EditorUtility.SetDirty(gui);
						if (gui.drivenByObject != null)
						{
							RectTransform.SendReapplyDrivenProperties(gui);
						}
					}
				}
				this.SetFadingBasedOnMouseDownUp(ref this.m_ChangingAnchors, @event);
			}
		}

		private static float Round(float value)
		{
			return Mathf.Floor(0.5f + value);
		}

		private static int RoundToInt(float value)
		{
			return Mathf.FloorToInt(0.5f + value);
		}

		private void DrawSizes(Rect rectInUserSpace, Transform userSpace, Rect rectInParentSpace, Transform parentSpace, RectTransform gui, RectTransform guiParent)
		{
			float size = 0.05f * HandleUtility.GetHandleSize(parentSpace.position);
			bool flag = gui.anchorMin.x != gui.anchorMax.x;
			bool flag2 = gui.anchorMin.y != gui.anchorMax.y;
			float alpha = Mathf.Max(new float[]
			{
				this.m_ChangingPosX.faded,
				this.m_ChangingLeft.faded,
				this.m_ChangingRight.faded,
				this.m_ChangingAnchors.faded
			});
			this.DrawAnchorRect(parentSpace, gui, guiParent, 0, alpha);
			alpha = Mathf.Max(new float[]
			{
				this.m_ChangingPosY.faded,
				this.m_ChangingTop.faded,
				this.m_ChangingBottom.faded,
				this.m_ChangingAnchors.faded
			});
			this.DrawAnchorRect(parentSpace, gui, guiParent, 1, alpha);
			this.DrawAnchorDistances(parentSpace, gui, guiParent, size, this.m_ChangingAnchors.faded);
			if (flag)
			{
				this.DrawPositionDistances(userSpace, rectInParentSpace, parentSpace, gui, guiParent, size, 0, 1, this.m_ChangingLeft.faded);
				this.DrawPositionDistances(userSpace, rectInParentSpace, parentSpace, gui, guiParent, size, 0, 2, this.m_ChangingRight.faded);
			}
			else
			{
				this.DrawPositionDistances(userSpace, rectInParentSpace, parentSpace, gui, guiParent, size, 0, 0, this.m_ChangingPosX.faded);
				this.DrawSizeDistances(userSpace, rectInParentSpace, parentSpace, gui, guiParent, size, 0, this.m_ChangingWidth.faded);
			}
			if (flag2)
			{
				this.DrawPositionDistances(userSpace, rectInParentSpace, parentSpace, gui, guiParent, size, 1, 1, this.m_ChangingBottom.faded);
				this.DrawPositionDistances(userSpace, rectInParentSpace, parentSpace, gui, guiParent, size, 1, 2, this.m_ChangingTop.faded);
			}
			else
			{
				this.DrawPositionDistances(userSpace, rectInParentSpace, parentSpace, gui, guiParent, size, 1, 0, this.m_ChangingPosY.faded);
				this.DrawSizeDistances(userSpace, rectInParentSpace, parentSpace, gui, guiParent, size, 1, this.m_ChangingHeight.faded);
			}
		}

		private void DrawSizeDistances(Transform userSpace, Rect rectInParentSpace, Transform parentSpace, RectTransform gui, RectTransform guiParent, float size, int axis, float alpha)
		{
			if (alpha > 0f)
			{
				Color color = RectTransformEditor.kAnchorColor;
				color.a *= alpha;
				GUI.color = color;
				if (userSpace == gui.transform)
				{
					gui.GetWorldCorners(RectTransformEditor.s_Corners);
				}
				else
				{
					gui.GetLocalCorners(RectTransformEditor.s_Corners);
					for (int i = 0; i < 4; i++)
					{
						RectTransformEditor.s_Corners[i] += gui.transform.localPosition;
						RectTransformEditor.s_Corners[i] = userSpace.TransformPoint(RectTransformEditor.s_Corners[i]);
					}
				}
				string text = gui.sizeDelta[axis].ToString();
				GUIContent label = new GUIContent(text);
				Vector3 b = ((axis != 0) ? userSpace.right : userSpace.up) * size * 2f;
				this.DrawLabelBetweenPoints(RectTransformEditor.s_Corners[0] + b, RectTransformEditor.s_Corners[(axis != 0) ? 1 : 3] + b, label);
			}
		}

		private void DrawPositionDistances(Transform userSpace, Rect rectInParentSpace, Transform parentSpace, RectTransform gui, RectTransform guiParent, float size, int axis, int side, float alpha)
		{
			if (!(guiParent == null) && alpha > 0f)
			{
				Color color = RectTransformEditor.kAnchorLineColor;
				color.a *= alpha;
				Handles.color = color;
				color = RectTransformEditor.kAnchorColor;
				color.a *= alpha;
				GUI.color = color;
				Vector3 vector;
				Vector3 vector2;
				float num;
				if (side == 0)
				{
					Vector2 v = Rect.NormalizedToPoint(rectInParentSpace, gui.pivot);
					vector = v;
					vector2 = v;
					vector[axis] = Mathf.LerpUnclamped(guiParent.rect.min[axis], guiParent.rect.max[axis], gui.anchorMin[axis]);
					num = gui.anchoredPosition[axis];
				}
				else
				{
					Vector2 center = rectInParentSpace.center;
					vector = center;
					vector2 = center;
					if (side == 1)
					{
						vector[axis] = Mathf.LerpUnclamped(guiParent.rect.min[axis], guiParent.rect.max[axis], gui.anchorMin[axis]);
						vector2[axis] = rectInParentSpace.min[axis];
						num = gui.offsetMin[axis];
					}
					else
					{
						vector[axis] = Mathf.LerpUnclamped(guiParent.rect.min[axis], guiParent.rect.max[axis], gui.anchorMax[axis]);
						vector2[axis] = rectInParentSpace.max[axis];
						num = -gui.offsetMax[axis];
					}
				}
				vector = parentSpace.TransformPoint(vector);
				vector2 = parentSpace.TransformPoint(vector2);
				RectHandles.DrawDottedLineWithShadow(RectTransformEditor.kShadowColor, RectTransformEditor.kShadowOffset, vector, vector2, 5f);
				GUIContent label = new GUIContent(num.ToString());
				this.DrawLabelBetweenPoints(vector, vector2, label);
			}
		}

		private float LerpUnclamped(float a, float b, float t)
		{
			return a * (1f - t) + b * t;
		}

		private void DrawAnchorDistances(Transform parentSpace, RectTransform gui, RectTransform guiParent, float size, float alpha)
		{
			if (!(guiParent == null) && alpha > 0f)
			{
				Color color = RectTransformEditor.kAnchorColor;
				color.a *= alpha;
				GUI.color = color;
				Vector3[,] array = new Vector3[2, 4];
				for (int i = 0; i < 2; i++)
				{
					for (int j = 0; j < 4; j++)
					{
						Vector3 vector = Vector3.zero;
						switch (j)
						{
						case 0:
							vector = Vector3.zero;
							break;
						case 1:
							vector = gui.anchorMin;
							break;
						case 2:
							vector = gui.anchorMax;
							break;
						case 3:
							vector = Vector3.one;
							break;
						}
						vector[i] = gui.anchorMin[i];
						vector = parentSpace.TransformPoint(this.GetAnchorLocal(guiParent, vector));
						array[i, j] = vector;
					}
				}
				for (int k = 0; k < 2; k++)
				{
					Vector3 b = ((k != 0) ? parentSpace.up : parentSpace.right) * size * 2f;
					int num = RectTransformEditor.RoundToInt(gui.anchorMin[1 - k] * 100f);
					int num2 = RectTransformEditor.RoundToInt((gui.anchorMax[1 - k] - gui.anchorMin[1 - k]) * 100f);
					int num3 = RectTransformEditor.RoundToInt((1f - gui.anchorMax[1 - k]) * 100f);
					if (num > 0)
					{
						this.DrawLabelBetweenPoints(array[k, 0] - b, array[k, 1] - b, GUIContent.Temp(num.ToString() + "%"));
					}
					if (num2 > 0)
					{
						this.DrawLabelBetweenPoints(array[k, 1] - b, array[k, 2] - b, GUIContent.Temp(num2.ToString() + "%"));
					}
					if (num3 > 0)
					{
						this.DrawLabelBetweenPoints(array[k, 2] - b, array[k, 3] - b, GUIContent.Temp(num3.ToString() + "%"));
					}
				}
			}
		}

		private void DrawAnchorRect(Transform parentSpace, RectTransform gui, RectTransform guiParent, int axis, float alpha)
		{
			if (!(guiParent == null) && alpha > 0f)
			{
				Color color = RectTransformEditor.kAnchorLineColor;
				color.a *= alpha;
				Handles.color = color;
				Vector3[,] array = new Vector3[2, 2];
				for (int i = 0; i < 2; i++)
				{
					if (i != 1 || gui.anchorMin[axis] != gui.anchorMax[axis])
					{
						array[i, 0][1 - axis] = Mathf.Min(0f, gui.anchorMin[1 - axis]);
						array[i, 1][1 - axis] = Mathf.Max(1f, gui.anchorMax[1 - axis]);
						for (int j = 0; j < 2; j++)
						{
							array[i, j][axis] = ((i != 0) ? gui.anchorMax[axis] : gui.anchorMin[axis]);
							array[i, j] = parentSpace.TransformPoint(this.GetAnchorLocal(guiParent, array[i, j]));
						}
						RectHandles.DrawDottedLineWithShadow(RectTransformEditor.kShadowColor, RectTransformEditor.kShadowOffset, array[i, 0], array[i, 1], 5f);
					}
				}
			}
		}

		private void DrawLabelBetweenPoints(Vector3 pA, Vector3 pB, GUIContent label)
		{
			if (!(pA == pB))
			{
				Vector2 a = HandleUtility.WorldToGUIPoint(pA);
				Vector2 b = HandleUtility.WorldToGUIPoint(pB);
				Vector2 pivotPoint = (a + b) * 0.5f;
				pivotPoint.x = RectTransformEditor.Round(pivotPoint.x);
				pivotPoint.y = RectTransformEditor.Round(pivotPoint.y);
				float num = Mathf.Atan2(b.y - a.y, b.x - a.x) * 57.29578f;
				num = Mathf.Repeat(num + 89f, 180f) - 89f;
				Handles.BeginGUI();
				Matrix4x4 matrix = GUI.matrix;
				GUIStyle measuringLabelStyle = RectTransformEditor.styles.measuringLabelStyle;
				measuringLabelStyle.alignment = TextAnchor.MiddleCenter;
				GUIUtility.RotateAroundPivot(num, pivotPoint);
				EditorGUI.DropShadowLabel(new Rect(pivotPoint.x - 50f, pivotPoint.y - 9f, 100f, 16f), label, measuringLabelStyle);
				GUI.matrix = matrix;
				Handles.EndGUI();
			}
		}

		private static Vector3 GetRectReferenceCorner(RectTransform gui, bool worldSpace)
		{
			Vector3 result;
			if (worldSpace)
			{
				Transform transform = gui.transform;
				gui.GetWorldCorners(RectTransformEditor.s_Corners);
				if (transform.parent)
				{
					result = transform.parent.InverseTransformPoint(RectTransformEditor.s_Corners[0]);
				}
				else
				{
					result = RectTransformEditor.s_Corners[0];
				}
			}
			else
			{
				result = gui.rect.min + gui.transform.localPosition;
			}
			return result;
		}

		private void DrawAnchor(Vector3 pos, Vector3 right, Vector3 up)
		{
			pos -= up * 0.5f;
			pos -= right * 0.5f;
			up *= 1.4f;
			right *= 1.4f;
			RectHandles.DrawPolyLineWithShadow(RectTransformEditor.kShadowColor, RectTransformEditor.kShadowOffset, new Vector3[]
			{
				pos,
				pos + up + right * 0.5f,
				pos + right + up * 0.5f,
				pos
			});
		}

		public static void SetPivotSmart(RectTransform rect, float value, int axis, bool smart, bool parentSpace)
		{
			Vector3 rectReferenceCorner = RectTransformEditor.GetRectReferenceCorner(rect, !parentSpace);
			Vector2 pivot = rect.pivot;
			pivot[axis] = value;
			rect.pivot = pivot;
			if (smart)
			{
				Vector3 rectReferenceCorner2 = RectTransformEditor.GetRectReferenceCorner(rect, !parentSpace);
				Vector3 v = rectReferenceCorner2 - rectReferenceCorner;
				rect.anchoredPosition -= v;
				Vector3 position = rect.transform.position;
				position.z -= v.z;
				rect.transform.position = position;
			}
		}

		public static void SetAnchorSmart(RectTransform rect, float value, int axis, bool isMax, bool smart)
		{
			RectTransformEditor.SetAnchorSmart(rect, value, axis, isMax, smart, false, false, false);
		}

		public static void SetAnchorSmart(RectTransform rect, float value, int axis, bool isMax, bool smart, bool enforceExactValue)
		{
			RectTransformEditor.SetAnchorSmart(rect, value, axis, isMax, smart, enforceExactValue, false, false);
		}

		public static void SetAnchorSmart(RectTransform rect, float value, int axis, bool isMax, bool smart, bool enforceExactValue, bool enforceMinNoLargerThanMax, bool moveTogether)
		{
			RectTransform rectTransform = null;
			if (rect.transform.parent == null)
			{
				smart = false;
			}
			else
			{
				rectTransform = rect.transform.parent.GetComponent<RectTransform>();
				if (rectTransform == null)
				{
					smart = false;
				}
			}
			bool flag = !RectTransformEditor.AnchorAllowedOutsideParent(axis, (!isMax) ? 0 : 1);
			if (flag)
			{
				value = Mathf.Clamp01(value);
			}
			if (enforceMinNoLargerThanMax)
			{
				if (isMax)
				{
					value = Mathf.Max(value, rect.anchorMin[axis]);
				}
				else
				{
					value = Mathf.Min(value, rect.anchorMax[axis]);
				}
			}
			float num = 0f;
			float num2 = 0f;
			if (smart)
			{
				float num3 = (!isMax) ? rect.anchorMin[axis] : rect.anchorMax[axis];
				num = (value - num3) * rectTransform.rect.size[axis];
				float num4 = 0f;
				if (RectTransformEditor.ShouldDoIntSnapping(rect))
				{
					num4 = Mathf.Round(num) - num;
				}
				num += num4;
				if (!enforceExactValue)
				{
					value += num4 / rectTransform.rect.size[axis];
					if (Mathf.Abs(RectTransformEditor.Round(value * 1000f) - value * 1000f) < 0.1f)
					{
						value = RectTransformEditor.Round(value * 1000f) * 0.001f;
					}
					if (flag)
					{
						value = Mathf.Clamp01(value);
					}
					if (enforceMinNoLargerThanMax)
					{
						if (isMax)
						{
							value = Mathf.Max(value, rect.anchorMin[axis]);
						}
						else
						{
							value = Mathf.Min(value, rect.anchorMax[axis]);
						}
					}
				}
				if (moveTogether)
				{
					num2 = num;
				}
				else
				{
					num2 = ((!isMax) ? (num * (1f - rect.pivot[axis])) : (num * rect.pivot[axis]));
				}
			}
			if (isMax)
			{
				Vector2 anchorMax = rect.anchorMax;
				anchorMax[axis] = value;
				rect.anchorMax = anchorMax;
				Vector2 anchorMin = rect.anchorMin;
				if (moveTogether)
				{
					anchorMin[axis] = RectTransformEditor.s_StartDragAnchorMin[axis] + anchorMax[axis] - RectTransformEditor.s_StartDragAnchorMax[axis];
				}
				rect.anchorMin = anchorMin;
			}
			else
			{
				Vector2 anchorMin2 = rect.anchorMin;
				anchorMin2[axis] = value;
				rect.anchorMin = anchorMin2;
				Vector2 anchorMax2 = rect.anchorMax;
				if (moveTogether)
				{
					anchorMax2[axis] = RectTransformEditor.s_StartDragAnchorMax[axis] + anchorMin2[axis] - RectTransformEditor.s_StartDragAnchorMin[axis];
				}
				rect.anchorMax = anchorMax2;
			}
			if (smart)
			{
				Vector2 anchoredPosition = rect.anchoredPosition;
				anchoredPosition[axis] -= num2;
				rect.anchoredPosition = anchoredPosition;
				if (!moveTogether)
				{
					Vector2 sizeDelta = rect.sizeDelta;
					sizeDelta[axis] += num * (float)((!isMax) ? 1 : -1);
					rect.sizeDelta = sizeDelta;
				}
			}
		}
	}
}
