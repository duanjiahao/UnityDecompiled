using System;
using UnityEditor.AnimatedValues;
using UnityEngine;
using UnityEngine.Events;

namespace UnityEditor
{
	[CanEditMultipleObjects, CustomEditor(typeof(Canvas))]
	internal class CanvasEditor : Editor
	{
		private static class Styles
		{
			public static GUIContent eventCamera = new GUIContent("Event Camera", "The Camera which the events are triggered through. This is used to determine clicking and hover positions if the Canvas is in World Space render mode.");

			public static GUIContent renderCamera = new GUIContent("Render Camera", "The Camera which will render the canvas. This is also the camera used to send events.");

			public static GUIContent sortingOrder = new GUIContent("Sort Order", "The order in which Screen Space - Overlay canvas will render");

			public static string s_RootAndNestedMessage = "Cannot multi-edit root Canvas together with nested Canvas.";

			public static GUIContent m_SortingLayerStyle = EditorGUIUtility.TextContent("Sorting Layer");

			public static GUIContent targetDisplay = new GUIContent("Target Display", "Display on which to render the canvas when in overlay mode");

			public static GUIContent m_SortingOrderStyle = EditorGUIUtility.TextContent("Order in Layer");
		}

		private enum PixelPerfect
		{
			Inherit,
			On,
			Off
		}

		private SerializedProperty m_RenderMode;

		private SerializedProperty m_Camera;

		private SerializedProperty m_PixelPerfect;

		private SerializedProperty m_PixelPerfectOverride;

		private SerializedProperty m_PlaneDistance;

		private SerializedProperty m_SortingLayerID;

		private SerializedProperty m_SortingOrder;

		private SerializedProperty m_TargetDisplay;

		private SerializedProperty m_OverrideSorting;

		private AnimBool m_OverlayMode;

		private AnimBool m_CameraMode;

		private AnimBool m_WorldMode;

		private AnimBool m_SortingOverride;

		private bool m_AllNested = false;

		private bool m_AllRoot = false;

		private bool m_AllOverlay = false;

		private bool m_NoneOverlay = false;

		private CanvasEditor.PixelPerfect pixelPerfect = CanvasEditor.PixelPerfect.Inherit;

		private void OnEnable()
		{
			this.m_RenderMode = base.serializedObject.FindProperty("m_RenderMode");
			this.m_Camera = base.serializedObject.FindProperty("m_Camera");
			this.m_PixelPerfect = base.serializedObject.FindProperty("m_PixelPerfect");
			this.m_PlaneDistance = base.serializedObject.FindProperty("m_PlaneDistance");
			this.m_SortingLayerID = base.serializedObject.FindProperty("m_SortingLayerID");
			this.m_SortingOrder = base.serializedObject.FindProperty("m_SortingOrder");
			this.m_TargetDisplay = base.serializedObject.FindProperty("m_TargetDisplay");
			this.m_OverrideSorting = base.serializedObject.FindProperty("m_OverrideSorting");
			this.m_PixelPerfectOverride = base.serializedObject.FindProperty("m_OverridePixelPerfect");
			this.m_OverlayMode = new AnimBool(this.m_RenderMode.intValue == 0);
			this.m_OverlayMode.valueChanged.AddListener(new UnityAction(base.Repaint));
			this.m_CameraMode = new AnimBool(this.m_RenderMode.intValue == 1);
			this.m_CameraMode.valueChanged.AddListener(new UnityAction(base.Repaint));
			this.m_WorldMode = new AnimBool(this.m_RenderMode.intValue == 2);
			this.m_WorldMode.valueChanged.AddListener(new UnityAction(base.Repaint));
			this.m_SortingOverride = new AnimBool(this.m_OverrideSorting.boolValue);
			this.m_SortingOverride.valueChanged.AddListener(new UnityAction(base.Repaint));
			if (this.m_PixelPerfectOverride.boolValue)
			{
				this.pixelPerfect = ((!this.m_PixelPerfect.boolValue) ? CanvasEditor.PixelPerfect.Off : CanvasEditor.PixelPerfect.On);
			}
			else
			{
				this.pixelPerfect = CanvasEditor.PixelPerfect.Inherit;
			}
			this.m_AllNested = true;
			this.m_AllRoot = true;
			this.m_AllOverlay = true;
			this.m_NoneOverlay = true;
			for (int i = 0; i < base.targets.Length; i++)
			{
				Canvas canvas = base.targets[i] as Canvas;
				if (canvas.transform.parent == null)
				{
					this.m_AllNested = false;
				}
				else if (canvas.transform.parent.GetComponentInParent<Canvas>() == null)
				{
					this.m_AllNested = false;
				}
				else
				{
					this.m_AllRoot = false;
				}
				if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
				{
					this.m_NoneOverlay = false;
				}
				else
				{
					this.m_AllOverlay = false;
				}
			}
		}

		private void OnDisable()
		{
			this.m_OverlayMode.valueChanged.RemoveListener(new UnityAction(base.Repaint));
			this.m_CameraMode.valueChanged.RemoveListener(new UnityAction(base.Repaint));
			this.m_WorldMode.valueChanged.RemoveListener(new UnityAction(base.Repaint));
			this.m_SortingOverride.valueChanged.RemoveListener(new UnityAction(base.Repaint));
		}

		public override void OnInspectorGUI()
		{
			base.serializedObject.Update();
			if (this.m_AllRoot)
			{
				EditorGUILayout.PropertyField(this.m_RenderMode, new GUILayoutOption[0]);
				this.m_OverlayMode.target = (this.m_RenderMode.intValue == 0);
				this.m_CameraMode.target = (this.m_RenderMode.intValue == 1);
				this.m_WorldMode.target = (this.m_RenderMode.intValue == 2);
				EditorGUI.indentLevel++;
				if (EditorGUILayout.BeginFadeGroup(this.m_OverlayMode.faded))
				{
					EditorGUILayout.PropertyField(this.m_PixelPerfect, new GUILayoutOption[0]);
					EditorGUILayout.PropertyField(this.m_SortingOrder, CanvasEditor.Styles.sortingOrder, new GUILayoutOption[0]);
					GUIContent[] displayNames = DisplayUtility.GetDisplayNames();
					EditorGUILayout.IntPopup(this.m_TargetDisplay, displayNames, DisplayUtility.GetDisplayIndices(), CanvasEditor.Styles.targetDisplay, new GUILayoutOption[0]);
				}
				EditorGUILayout.EndFadeGroup();
				if (EditorGUILayout.BeginFadeGroup(this.m_CameraMode.faded))
				{
					EditorGUILayout.PropertyField(this.m_PixelPerfect, new GUILayoutOption[0]);
					EditorGUILayout.PropertyField(this.m_Camera, CanvasEditor.Styles.renderCamera, new GUILayoutOption[0]);
					if (this.m_Camera.objectReferenceValue != null)
					{
						EditorGUILayout.PropertyField(this.m_PlaneDistance, new GUILayoutOption[0]);
					}
					EditorGUILayout.Space();
					if (this.m_Camera.objectReferenceValue != null)
					{
						EditorGUILayout.SortingLayerField(CanvasEditor.Styles.m_SortingLayerStyle, this.m_SortingLayerID, EditorStyles.popup, EditorStyles.label);
					}
					EditorGUILayout.PropertyField(this.m_SortingOrder, CanvasEditor.Styles.m_SortingOrderStyle, new GUILayoutOption[0]);
					if (this.m_Camera.objectReferenceValue == null)
					{
						EditorGUILayout.HelpBox("Screen Space - A canvas with no specified camera acts like a Overlay Canvas. Please assign a camera to it in the 'Render Camera' field.", MessageType.Warning);
					}
				}
				EditorGUILayout.EndFadeGroup();
				if (EditorGUILayout.BeginFadeGroup(this.m_WorldMode.faded))
				{
					EditorGUILayout.PropertyField(this.m_Camera, CanvasEditor.Styles.eventCamera, new GUILayoutOption[0]);
					EditorGUILayout.Space();
					EditorGUILayout.SortingLayerField(CanvasEditor.Styles.m_SortingLayerStyle, this.m_SortingLayerID, EditorStyles.popup);
					EditorGUILayout.PropertyField(this.m_SortingOrder, CanvasEditor.Styles.m_SortingOrderStyle, new GUILayoutOption[0]);
				}
				EditorGUILayout.EndFadeGroup();
				EditorGUI.indentLevel--;
			}
			else if (this.m_AllNested)
			{
				EditorGUI.BeginChangeCheck();
				this.pixelPerfect = (CanvasEditor.PixelPerfect)EditorGUILayout.EnumPopup("Pixel Perfect", this.pixelPerfect, new GUILayoutOption[0]);
				if (EditorGUI.EndChangeCheck())
				{
					if (this.pixelPerfect == CanvasEditor.PixelPerfect.Inherit)
					{
						this.m_PixelPerfectOverride.boolValue = false;
					}
					else if (this.pixelPerfect == CanvasEditor.PixelPerfect.Off)
					{
						this.m_PixelPerfectOverride.boolValue = true;
						this.m_PixelPerfect.boolValue = false;
					}
					else
					{
						this.m_PixelPerfectOverride.boolValue = true;
						this.m_PixelPerfect.boolValue = true;
					}
				}
				EditorGUI.BeginChangeCheck();
				EditorGUILayout.PropertyField(this.m_OverrideSorting, new GUILayoutOption[0]);
				if (EditorGUI.EndChangeCheck())
				{
					((Canvas)base.serializedObject.targetObject).overrideSorting = this.m_OverrideSorting.boolValue;
					this.m_SortingOverride.target = this.m_OverrideSorting.boolValue;
				}
				if (EditorGUILayout.BeginFadeGroup(this.m_SortingOverride.faded))
				{
					GUIContent gUIContent = null;
					if (this.m_AllOverlay)
					{
						gUIContent = CanvasEditor.Styles.sortingOrder;
					}
					else if (this.m_NoneOverlay)
					{
						gUIContent = CanvasEditor.Styles.m_SortingOrderStyle;
						EditorGUILayout.SortingLayerField(CanvasEditor.Styles.m_SortingLayerStyle, this.m_SortingLayerID, EditorStyles.popup);
					}
					if (gUIContent != null)
					{
						EditorGUI.BeginChangeCheck();
						EditorGUILayout.PropertyField(this.m_SortingOrder, gUIContent, new GUILayoutOption[0]);
						if (EditorGUI.EndChangeCheck())
						{
							((Canvas)base.serializedObject.targetObject).sortingOrder = this.m_SortingOrder.intValue;
						}
					}
				}
				EditorGUILayout.EndFadeGroup();
			}
			else
			{
				GUILayout.Label(CanvasEditor.Styles.s_RootAndNestedMessage, EditorStyles.helpBox, new GUILayoutOption[0]);
			}
			base.serializedObject.ApplyModifiedProperties();
		}
	}
}
