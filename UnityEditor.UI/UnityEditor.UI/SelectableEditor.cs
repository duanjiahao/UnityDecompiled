using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEditor.AnimatedValues;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UnityEditor.UI
{
	[CustomEditor(typeof(Selectable), true)]
	public class SelectableEditor : Editor
	{
		private SerializedProperty m_Script;

		private SerializedProperty m_InteractableProperty;

		private SerializedProperty m_TargetGraphicProperty;

		private SerializedProperty m_TransitionProperty;

		private SerializedProperty m_ColorBlockProperty;

		private SerializedProperty m_SpriteStateProperty;

		private SerializedProperty m_AnimTriggerProperty;

		private SerializedProperty m_NavigationProperty;

		private GUIContent m_VisualizeNavigation = new GUIContent("Visualize", "Show navigation flows between selectable UI elements.");

		private AnimBool m_ShowColorTint = new AnimBool();

		private AnimBool m_ShowSpriteTrasition = new AnimBool();

		private AnimBool m_ShowAnimTransition = new AnimBool();

		private static List<SelectableEditor> s_Editors = new List<SelectableEditor>();

		private static bool s_ShowNavigation = false;

		private static string s_ShowNavigationKey = "SelectableEditor.ShowNavigation";

		private string[] m_PropertyPathToExcludeForChildClasses;

		private const float kArrowThickness = 2.5f;

		private const float kArrowHeadSize = 1.2f;

		[CompilerGenerated]
		private static SceneView.OnSceneFunc <>f__mg$cache0;

		[CompilerGenerated]
		private static SceneView.OnSceneFunc <>f__mg$cache1;

		protected virtual void OnEnable()
		{
			this.m_Script = base.serializedObject.FindProperty("m_Script");
			this.m_InteractableProperty = base.serializedObject.FindProperty("m_Interactable");
			this.m_TargetGraphicProperty = base.serializedObject.FindProperty("m_TargetGraphic");
			this.m_TransitionProperty = base.serializedObject.FindProperty("m_Transition");
			this.m_ColorBlockProperty = base.serializedObject.FindProperty("m_Colors");
			this.m_SpriteStateProperty = base.serializedObject.FindProperty("m_SpriteState");
			this.m_AnimTriggerProperty = base.serializedObject.FindProperty("m_AnimationTriggers");
			this.m_NavigationProperty = base.serializedObject.FindProperty("m_Navigation");
			this.m_PropertyPathToExcludeForChildClasses = new string[]
			{
				this.m_Script.propertyPath,
				this.m_NavigationProperty.propertyPath,
				this.m_TransitionProperty.propertyPath,
				this.m_ColorBlockProperty.propertyPath,
				this.m_SpriteStateProperty.propertyPath,
				this.m_AnimTriggerProperty.propertyPath,
				this.m_InteractableProperty.propertyPath,
				this.m_TargetGraphicProperty.propertyPath
			};
			Selectable.Transition transition = SelectableEditor.GetTransition(this.m_TransitionProperty);
			this.m_ShowColorTint.value = (transition == Selectable.Transition.ColorTint);
			this.m_ShowSpriteTrasition.value = (transition == Selectable.Transition.SpriteSwap);
			this.m_ShowAnimTransition.value = (transition == Selectable.Transition.Animation);
			this.m_ShowColorTint.valueChanged.AddListener(new UnityAction(base.Repaint));
			this.m_ShowSpriteTrasition.valueChanged.AddListener(new UnityAction(base.Repaint));
			SelectableEditor.s_Editors.Add(this);
			this.RegisterStaticOnSceneGUI();
			SelectableEditor.s_ShowNavigation = EditorPrefs.GetBool(SelectableEditor.s_ShowNavigationKey);
		}

		protected virtual void OnDisable()
		{
			this.m_ShowColorTint.valueChanged.RemoveListener(new UnityAction(base.Repaint));
			this.m_ShowSpriteTrasition.valueChanged.RemoveListener(new UnityAction(base.Repaint));
			SelectableEditor.s_Editors.Remove(this);
			this.RegisterStaticOnSceneGUI();
		}

		private void RegisterStaticOnSceneGUI()
		{
			Delegate arg_23_0 = SceneView.onSceneGUIDelegate;
			if (SelectableEditor.<>f__mg$cache0 == null)
			{
				SelectableEditor.<>f__mg$cache0 = new SceneView.OnSceneFunc(SelectableEditor.StaticOnSceneGUI);
			}
			SceneView.onSceneGUIDelegate = (SceneView.OnSceneFunc)Delegate.Remove(arg_23_0, SelectableEditor.<>f__mg$cache0);
			if (SelectableEditor.s_Editors.Count > 0)
			{
				Delegate arg_64_0 = SceneView.onSceneGUIDelegate;
				if (SelectableEditor.<>f__mg$cache1 == null)
				{
					SelectableEditor.<>f__mg$cache1 = new SceneView.OnSceneFunc(SelectableEditor.StaticOnSceneGUI);
				}
				SceneView.onSceneGUIDelegate = (SceneView.OnSceneFunc)Delegate.Combine(arg_64_0, SelectableEditor.<>f__mg$cache1);
			}
		}

		private static Selectable.Transition GetTransition(SerializedProperty transition)
		{
			return (Selectable.Transition)transition.enumValueIndex;
		}

		public override void OnInspectorGUI()
		{
			base.serializedObject.Update();
			EditorGUILayout.PropertyField(this.m_InteractableProperty, new GUILayoutOption[0]);
			Selectable.Transition transition = SelectableEditor.GetTransition(this.m_TransitionProperty);
			Graphic graphic = this.m_TargetGraphicProperty.objectReferenceValue as Graphic;
			if (graphic == null)
			{
				graphic = (base.target as Selectable).GetComponent<Graphic>();
			}
			Animator animator = (base.target as Selectable).GetComponent<Animator>();
			this.m_ShowColorTint.target = (!this.m_TransitionProperty.hasMultipleDifferentValues && transition == Selectable.Transition.ColorTint);
			this.m_ShowSpriteTrasition.target = (!this.m_TransitionProperty.hasMultipleDifferentValues && transition == Selectable.Transition.SpriteSwap);
			this.m_ShowAnimTransition.target = (!this.m_TransitionProperty.hasMultipleDifferentValues && transition == Selectable.Transition.Animation);
			EditorGUILayout.PropertyField(this.m_TransitionProperty, new GUILayoutOption[0]);
			EditorGUI.indentLevel++;
			if (transition == Selectable.Transition.ColorTint || transition == Selectable.Transition.SpriteSwap)
			{
				EditorGUILayout.PropertyField(this.m_TargetGraphicProperty, new GUILayoutOption[0]);
			}
			if (transition != Selectable.Transition.ColorTint)
			{
				if (transition == Selectable.Transition.SpriteSwap)
				{
					if (graphic as Image == null)
					{
						EditorGUILayout.HelpBox("You must have a Image target in order to use a sprite swap transition.", MessageType.Warning);
					}
				}
			}
			else if (graphic == null)
			{
				EditorGUILayout.HelpBox("You must have a Graphic target in order to use a color transition.", MessageType.Warning);
			}
			if (EditorGUILayout.BeginFadeGroup(this.m_ShowColorTint.faded))
			{
				EditorGUILayout.PropertyField(this.m_ColorBlockProperty, new GUILayoutOption[0]);
			}
			EditorGUILayout.EndFadeGroup();
			if (EditorGUILayout.BeginFadeGroup(this.m_ShowSpriteTrasition.faded))
			{
				EditorGUILayout.PropertyField(this.m_SpriteStateProperty, new GUILayoutOption[0]);
			}
			EditorGUILayout.EndFadeGroup();
			if (EditorGUILayout.BeginFadeGroup(this.m_ShowAnimTransition.faded))
			{
				EditorGUILayout.PropertyField(this.m_AnimTriggerProperty, new GUILayoutOption[0]);
				if (animator == null || animator.runtimeAnimatorController == null)
				{
					Rect controlRect = EditorGUILayout.GetControlRect(new GUILayoutOption[0]);
					controlRect.xMin += EditorGUIUtility.labelWidth;
					if (GUI.Button(controlRect, "Auto Generate Animation", EditorStyles.miniButton))
					{
						AnimatorController animatorController = SelectableEditor.GenerateSelectableAnimatorContoller((base.target as Selectable).animationTriggers, base.target as Selectable);
						if (animatorController != null)
						{
							if (animator == null)
							{
								animator = (base.target as Selectable).gameObject.AddComponent<Animator>();
							}
							AnimatorController.SetAnimatorController(animator, animatorController);
						}
					}
				}
			}
			EditorGUILayout.EndFadeGroup();
			EditorGUI.indentLevel--;
			EditorGUILayout.Space();
			EditorGUILayout.PropertyField(this.m_NavigationProperty, new GUILayoutOption[0]);
			EditorGUI.BeginChangeCheck();
			Rect controlRect2 = EditorGUILayout.GetControlRect(new GUILayoutOption[0]);
			controlRect2.xMin += EditorGUIUtility.labelWidth;
			SelectableEditor.s_ShowNavigation = GUI.Toggle(controlRect2, SelectableEditor.s_ShowNavigation, this.m_VisualizeNavigation, EditorStyles.miniButton);
			if (EditorGUI.EndChangeCheck())
			{
				EditorPrefs.SetBool(SelectableEditor.s_ShowNavigationKey, SelectableEditor.s_ShowNavigation);
				SceneView.RepaintAll();
			}
			this.ChildClassPropertiesGUI();
			base.serializedObject.ApplyModifiedProperties();
		}

		private void ChildClassPropertiesGUI()
		{
			if (!this.IsDerivedSelectableEditor())
			{
				Editor.DrawPropertiesExcluding(base.serializedObject, this.m_PropertyPathToExcludeForChildClasses);
			}
		}

		private bool IsDerivedSelectableEditor()
		{
			return base.GetType() != typeof(SelectableEditor);
		}

		private static AnimatorController GenerateSelectableAnimatorContoller(AnimationTriggers animationTriggers, Selectable target)
		{
			AnimatorController result;
			if (target == null)
			{
				result = null;
			}
			else
			{
				string saveControllerPath = SelectableEditor.GetSaveControllerPath(target);
				if (string.IsNullOrEmpty(saveControllerPath))
				{
					result = null;
				}
				else
				{
					string name = (!string.IsNullOrEmpty(animationTriggers.normalTrigger)) ? animationTriggers.normalTrigger : "Normal";
					string name2 = (!string.IsNullOrEmpty(animationTriggers.highlightedTrigger)) ? animationTriggers.highlightedTrigger : "Highlighted";
					string name3 = (!string.IsNullOrEmpty(animationTriggers.pressedTrigger)) ? animationTriggers.pressedTrigger : "Pressed";
					string name4 = (!string.IsNullOrEmpty(animationTriggers.disabledTrigger)) ? animationTriggers.disabledTrigger : "Disabled";
					AnimatorController animatorController = AnimatorController.CreateAnimatorControllerAtPath(saveControllerPath);
					SelectableEditor.GenerateTriggerableTransition(name, animatorController);
					SelectableEditor.GenerateTriggerableTransition(name2, animatorController);
					SelectableEditor.GenerateTriggerableTransition(name3, animatorController);
					SelectableEditor.GenerateTriggerableTransition(name4, animatorController);
					AssetDatabase.ImportAsset(saveControllerPath);
					result = animatorController;
				}
			}
			return result;
		}

		private static string GetSaveControllerPath(Selectable target)
		{
			string name = target.gameObject.name;
			string message = string.Format("Create a new animator for the game object '{0}':", name);
			return EditorUtility.SaveFilePanelInProject("New Animation Contoller", name, "controller", message);
		}

		private static void SetUpCurves(AnimationClip highlightedClip, AnimationClip pressedClip, string animationPath)
		{
			string[] array = new string[]
			{
				"m_LocalScale.x",
				"m_LocalScale.y",
				"m_LocalScale.z"
			};
			Keyframe[] keys = new Keyframe[]
			{
				new Keyframe(0f, 1f),
				new Keyframe(0.5f, 1.1f),
				new Keyframe(1f, 1f)
			};
			AnimationCurve curve = new AnimationCurve(keys);
			string[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				string inPropertyName = array2[i];
				AnimationUtility.SetEditorCurve(highlightedClip, EditorCurveBinding.FloatCurve(animationPath, typeof(Transform), inPropertyName), curve);
			}
			Keyframe[] keys2 = new Keyframe[]
			{
				new Keyframe(0f, 1.15f)
			};
			AnimationCurve curve2 = new AnimationCurve(keys2);
			string[] array3 = array;
			for (int j = 0; j < array3.Length; j++)
			{
				string inPropertyName2 = array3[j];
				AnimationUtility.SetEditorCurve(pressedClip, EditorCurveBinding.FloatCurve(animationPath, typeof(Transform), inPropertyName2), curve2);
			}
		}

		private static string BuildAnimationPath(Selectable target)
		{
			Graphic targetGraphic = target.targetGraphic;
			string result;
			if (targetGraphic == null)
			{
				result = string.Empty;
			}
			else
			{
				GameObject gameObject = targetGraphic.gameObject;
				GameObject gameObject2 = target.gameObject;
				Stack<string> stack = new Stack<string>();
				while (gameObject2 != gameObject)
				{
					stack.Push(gameObject.name);
					if (gameObject.transform.parent == null)
					{
						result = string.Empty;
						return result;
					}
					gameObject = gameObject.transform.parent.gameObject;
				}
				StringBuilder stringBuilder = new StringBuilder();
				if (stack.Count > 0)
				{
					stringBuilder.Append(stack.Pop());
				}
				while (stack.Count > 0)
				{
					stringBuilder.Append("/").Append(stack.Pop());
				}
				result = stringBuilder.ToString();
			}
			return result;
		}

		private static AnimationClip GenerateTriggerableTransition(string name, AnimatorController controller)
		{
			AnimationClip animationClip = AnimatorController.AllocateAnimatorClip(name);
			AssetDatabase.AddObjectToAsset(animationClip, controller);
			AnimatorState destinationState = controller.AddMotion(animationClip);
			controller.AddParameter(name, AnimatorControllerParameterType.Trigger);
			AnimatorStateMachine stateMachine = controller.layers[0].stateMachine;
			AnimatorStateTransition animatorStateTransition = stateMachine.AddAnyStateTransition(destinationState);
			animatorStateTransition.AddCondition(AnimatorConditionMode.If, 0f, name);
			return animationClip;
		}

		private static void StaticOnSceneGUI(SceneView view)
		{
			if (SelectableEditor.s_ShowNavigation)
			{
				for (int i = 0; i < Selectable.allSelectables.Count; i++)
				{
					SelectableEditor.DrawNavigationForSelectable(Selectable.allSelectables[i]);
				}
			}
		}

		private static void DrawNavigationForSelectable(Selectable sel)
		{
			if (!(sel == null))
			{
				Transform transform = sel.transform;
				bool flag = Selection.transforms.Any((Transform e) => e == transform);
				Handles.color = new Color(1f, 0.9f, 0.1f, (!flag) ? 0.4f : 1f);
				SelectableEditor.DrawNavigationArrow(-Vector2.right, sel, sel.FindSelectableOnLeft());
				SelectableEditor.DrawNavigationArrow(Vector2.right, sel, sel.FindSelectableOnRight());
				SelectableEditor.DrawNavigationArrow(Vector2.up, sel, sel.FindSelectableOnUp());
				SelectableEditor.DrawNavigationArrow(-Vector2.up, sel, sel.FindSelectableOnDown());
			}
		}

		private static void DrawNavigationArrow(Vector2 direction, Selectable fromObj, Selectable toObj)
		{
			if (!(fromObj == null) && !(toObj == null))
			{
				Transform transform = fromObj.transform;
				Transform transform2 = toObj.transform;
				Vector2 vector = new Vector2(direction.y, -direction.x);
				Vector3 vector2 = transform.TransformPoint(SelectableEditor.GetPointOnRectEdge(transform as RectTransform, direction));
				Vector3 vector3 = transform2.TransformPoint(SelectableEditor.GetPointOnRectEdge(transform2 as RectTransform, -direction));
				float d = HandleUtility.GetHandleSize(vector2) * 0.05f;
				float d2 = HandleUtility.GetHandleSize(vector3) * 0.05f;
				vector2 += transform.TransformDirection(vector) * d;
				vector3 += transform2.TransformDirection(vector) * d2;
				float d3 = Vector3.Distance(vector2, vector3);
				Vector3 b = transform.rotation * direction * d3 * 0.3f;
				Vector3 b2 = transform2.rotation * -direction * d3 * 0.3f;
				Handles.DrawBezier(vector2, vector3, vector2 + b, vector3 + b2, Handles.color, null, 2.5f);
				Handles.DrawAAPolyLine(2.5f, new Vector3[]
				{
					vector3,
					vector3 + transform2.rotation * (-direction - vector) * d2 * 1.2f
				});
				Handles.DrawAAPolyLine(2.5f, new Vector3[]
				{
					vector3,
					vector3 + transform2.rotation * (-direction + vector) * d2 * 1.2f
				});
			}
		}

		private static Vector3 GetPointOnRectEdge(RectTransform rect, Vector2 dir)
		{
			Vector3 result;
			if (rect == null)
			{
				result = Vector3.zero;
			}
			else
			{
				if (dir != Vector2.zero)
				{
					dir /= Mathf.Max(Mathf.Abs(dir.x), Mathf.Abs(dir.y));
				}
				dir = rect.rect.center + Vector2.Scale(rect.rect.size, dir * 0.5f);
				result = dir;
			}
			return result;
		}
	}
}
