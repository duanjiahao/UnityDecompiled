using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace UnityEditorInternal.VR
{
	internal class PlayerSettingsEditorVR
	{
		private static class Styles
		{
			public static readonly GUIContent singlepassAndroidWarning = EditorGUIUtility.TextContent("Single-pass stereo rendering requires OpenGL ES 3. Please make sure that it's the first one listed under Graphics APIs.");

			public static readonly GUIContent singlepassAndroidWarning2 = EditorGUIUtility.TextContent("Multi-pass stereo rendering will be used on Android devices that don't support single-pass stereo rendering.");

			public static readonly GUIContent singlePassStereoRendering = EditorGUIUtility.TextContent("Single-Pass Stereo Rendering");

			public static readonly GUIContent[] kDefaultStereoRenderingPaths = new GUIContent[]
			{
				new GUIContent("Multi Pass"),
				new GUIContent("Single Pass"),
				new GUIContent("Single Pass Instanced")
			};

			public static readonly GUIContent[] kAndroidStereoRenderingPaths = new GUIContent[]
			{
				new GUIContent("Multi Pass"),
				new GUIContent("Single Pass (Preview)")
			};
		}

		private SerializedObject m_Settings;

		private Dictionary<BuildTargetGroup, VRDeviceInfoEditor[]> m_AllVRDevicesForBuildTarget = new Dictionary<BuildTargetGroup, VRDeviceInfoEditor[]>();

		private Dictionary<BuildTargetGroup, ReorderableList> m_VRDeviceActiveUI = new Dictionary<BuildTargetGroup, ReorderableList>();

		private Dictionary<string, string> m_MapVRDeviceKeyToUIString = new Dictionary<string, string>();

		private Dictionary<string, string> m_MapVRUIStringToDeviceKey = new Dictionary<string, string>();

		private Dictionary<string, VRCustomOptions> m_CustomOptions = new Dictionary<string, VRCustomOptions>();

		public PlayerSettingsEditorVR(SerializedObject settingsEditor)
		{
			this.m_Settings = settingsEditor;
		}

		private void RefreshVRDeviceList(BuildTargetGroup targetGroup)
		{
			VRDeviceInfoEditor[] array = VREditor.GetAllVRDeviceInfo(targetGroup);
			array = (from d in array
			orderby d.deviceNameUI
			select d).ToArray<VRDeviceInfoEditor>();
			this.m_AllVRDevicesForBuildTarget[targetGroup] = array;
			for (int i = 0; i < array.Length; i++)
			{
				VRDeviceInfoEditor vRDeviceInfoEditor = array[i];
				this.m_MapVRDeviceKeyToUIString[vRDeviceInfoEditor.deviceNameKey] = vRDeviceInfoEditor.deviceNameUI;
				this.m_MapVRUIStringToDeviceKey[vRDeviceInfoEditor.deviceNameUI] = vRDeviceInfoEditor.deviceNameKey;
				VRCustomOptions vRCustomOptions;
				if (!this.m_CustomOptions.TryGetValue(vRDeviceInfoEditor.deviceNameKey, out vRCustomOptions))
				{
					Type type = Type.GetType("UnityEditorInternal.VR.VRCustomOptions" + vRDeviceInfoEditor.deviceNameKey, false, true);
					if (type != null)
					{
						vRCustomOptions = (VRCustomOptions)Activator.CreateInstance(type);
					}
					else
					{
						vRCustomOptions = new VRCustomOptionsNone();
					}
					vRCustomOptions.Initialize(this.m_Settings);
					this.m_CustomOptions.Add(vRDeviceInfoEditor.deviceNameKey, vRCustomOptions);
				}
			}
		}

		internal bool TargetGroupSupportsVirtualReality(BuildTargetGroup targetGroup)
		{
			if (!this.m_AllVRDevicesForBuildTarget.ContainsKey(targetGroup))
			{
				this.RefreshVRDeviceList(targetGroup);
			}
			VRDeviceInfoEditor[] array = this.m_AllVRDevicesForBuildTarget[targetGroup];
			return array.Length > 0;
		}

		internal void DevicesGUI(BuildTargetGroup targetGroup)
		{
			if (this.TargetGroupSupportsVirtualReality(targetGroup))
			{
				bool flag = VREditor.GetVREnabledOnTargetGroup(targetGroup);
				EditorGUI.BeginChangeCheck();
				flag = EditorGUILayout.Toggle(EditorGUIUtility.TextContent("Virtual Reality Supported"), flag, new GUILayoutOption[0]);
				if (EditorGUI.EndChangeCheck())
				{
					VREditor.SetVREnabledOnTargetGroup(targetGroup, flag);
				}
				if (flag)
				{
					this.VRDevicesGUIOneBuildTarget(targetGroup);
				}
			}
		}

		private static bool TargetSupportsSinglePassStereoRendering(BuildTargetGroup targetGroup)
		{
			return targetGroup == BuildTargetGroup.Standalone || targetGroup == BuildTargetGroup.Android || targetGroup == BuildTargetGroup.PS4;
		}

		private static bool TargetSupportsStereoInstancingRendering(BuildTargetGroup targetGroup)
		{
			return targetGroup == BuildTargetGroup.WSA;
		}

		private static GUIContent[] GetStereoRenderingPaths(BuildTargetGroup targetGroup)
		{
			return (targetGroup != BuildTargetGroup.Android) ? PlayerSettingsEditorVR.Styles.kDefaultStereoRenderingPaths : PlayerSettingsEditorVR.Styles.kAndroidStereoRenderingPaths;
		}

		internal void SinglePassStereoGUI(BuildTargetGroup targetGroup, SerializedProperty stereoRenderingPath)
		{
			if (PlayerSettings.virtualRealitySupported)
			{
				bool flag = PlayerSettingsEditorVR.TargetSupportsSinglePassStereoRendering(targetGroup);
				bool flag2 = PlayerSettingsEditorVR.TargetSupportsStereoInstancingRendering(targetGroup);
				int num = 1 + ((!flag) ? 0 : 1) + ((!flag2) ? 0 : 1);
				GUIContent[] array = new GUIContent[num];
				int[] array2 = new int[num];
				int[] array3 = new int[]
				{
					0,
					1,
					2
				};
				GUIContent[] stereoRenderingPaths = PlayerSettingsEditorVR.GetStereoRenderingPaths(targetGroup);
				int num2 = 0;
				array[num2] = stereoRenderingPaths[0];
				array2[num2++] = array3[0];
				if (flag)
				{
					array[num2] = stereoRenderingPaths[1];
					array2[num2++] = array3[1];
				}
				if (flag2 && stereoRenderingPaths.Length > 2)
				{
					array[num2] = stereoRenderingPaths[2];
					array2[num2++] = array3[2];
				}
				if (!flag2 && stereoRenderingPath.intValue == 2)
				{
					stereoRenderingPath.intValue = 1;
				}
				if (!flag && stereoRenderingPath.intValue == 1)
				{
					stereoRenderingPath.intValue = 0;
				}
				EditorGUILayout.IntPopup(stereoRenderingPath, array, array2, EditorGUIUtility.TextContent("Stereo Rendering Method*"), new GUILayoutOption[0]);
				if (stereoRenderingPath.intValue == 1 && targetGroup == BuildTargetGroup.Android)
				{
					GraphicsDeviceType[] graphicsAPIs = PlayerSettings.GetGraphicsAPIs(BuildTarget.Android);
					if (graphicsAPIs.Length > 0 && graphicsAPIs[0] == GraphicsDeviceType.OpenGLES3)
					{
						EditorGUILayout.HelpBox(PlayerSettingsEditorVR.Styles.singlepassAndroidWarning2.text, MessageType.Warning);
					}
					else
					{
						EditorGUILayout.HelpBox(PlayerSettingsEditorVR.Styles.singlepassAndroidWarning.text, MessageType.Error);
					}
				}
			}
		}

		private void AddVRDeviceMenuSelected(object userData, string[] options, int selected)
		{
			BuildTargetGroup buildTargetGroup = (BuildTargetGroup)userData;
			List<string> list = VREditor.GetVREnabledDevicesOnTargetGroup(buildTargetGroup).ToList<string>();
			string item;
			if (!this.m_MapVRUIStringToDeviceKey.TryGetValue(options[selected], out item))
			{
				item = options[selected];
			}
			list.Add(item);
			this.ApplyChangedVRDeviceList(buildTargetGroup, list.ToArray());
		}

		private void AddVRDeviceElement(BuildTargetGroup target, Rect rect, ReorderableList list)
		{
			VRDeviceInfoEditor[] source = this.m_AllVRDevicesForBuildTarget[target];
			List<string> enabledDevices = VREditor.GetVREnabledDevicesOnTargetGroup(target).ToList<string>();
			string[] options = (from d in source
			select d.deviceNameUI).ToArray<string>();
			bool[] enabled = (from d in source
			select !enabledDevices.Any((string enabledDeviceName) => d.deviceNameKey == enabledDeviceName)).ToArray<bool>();
			EditorUtility.DisplayCustomMenu(rect, options, enabled, null, new EditorUtility.SelectMenuItemFunction(this.AddVRDeviceMenuSelected), target);
		}

		private void RemoveVRDeviceElement(BuildTargetGroup target, ReorderableList list)
		{
			List<string> list2 = VREditor.GetVREnabledDevicesOnTargetGroup(target).ToList<string>();
			list2.RemoveAt(list.index);
			this.ApplyChangedVRDeviceList(target, list2.ToArray());
		}

		private void ReorderVRDeviceElement(BuildTargetGroup target, ReorderableList list)
		{
			string[] devices = list.list.Cast<string>().ToArray<string>();
			this.ApplyChangedVRDeviceList(target, devices);
		}

		private void ApplyChangedVRDeviceList(BuildTargetGroup target, string[] devices)
		{
			if (this.m_VRDeviceActiveUI.ContainsKey(target))
			{
				if (target == BuildTargetGroup.iPhone)
				{
					if (devices.Contains("cardboard") && PlayerSettings.iOS.cameraUsageDescription == "")
					{
						PlayerSettings.iOS.cameraUsageDescription = "Used to scan QR codes";
					}
				}
				VREditor.SetVREnabledDevicesOnTargetGroup(target, devices);
				this.m_VRDeviceActiveUI[target].list = devices;
			}
		}

		private void DrawVRDeviceElement(BuildTargetGroup target, Rect rect, int index, bool selected, bool focused)
		{
			string text = (string)this.m_VRDeviceActiveUI[target].list[index];
			string text2;
			if (!this.m_MapVRDeviceKeyToUIString.TryGetValue(text, out text2))
			{
				text2 = text + " (missing from build)";
			}
			VRCustomOptions vRCustomOptions;
			if (this.m_CustomOptions.TryGetValue(text, out vRCustomOptions))
			{
				if (!(vRCustomOptions is VRCustomOptionsNone))
				{
					Rect position = new Rect(rect);
					position.width = (float)EditorStyles.foldout.border.left;
					position.height = (float)EditorStyles.foldout.border.top;
					bool hierarchyMode = EditorGUIUtility.hierarchyMode;
					EditorGUIUtility.hierarchyMode = false;
					vRCustomOptions.IsExpanded = EditorGUI.Foldout(position, vRCustomOptions.IsExpanded, "", false, EditorStyles.foldout);
					EditorGUIUtility.hierarchyMode = hierarchyMode;
				}
			}
			rect.xMin += (float)EditorStyles.foldout.border.left;
			GUI.Label(rect, text2, EditorStyles.label);
			rect.y += EditorGUIUtility.singleLineHeight + 2f;
			if (vRCustomOptions != null && vRCustomOptions.IsExpanded)
			{
				vRCustomOptions.Draw(rect);
			}
		}

		private float GetVRDeviceElementHeight(BuildTargetGroup target, int index)
		{
			ReorderableList reorderableList = this.m_VRDeviceActiveUI[target];
			string key = (string)reorderableList.list[index];
			float num = 0f;
			VRCustomOptions vRCustomOptions;
			if (this.m_CustomOptions.TryGetValue(key, out vRCustomOptions))
			{
				num = ((!vRCustomOptions.IsExpanded) ? 0f : (vRCustomOptions.GetHeight() + 2f));
			}
			return reorderableList.elementHeight + num;
		}

		private void SelectVRDeviceElement(BuildTargetGroup target, ReorderableList list)
		{
			string key = (string)this.m_VRDeviceActiveUI[target].list[list.index];
			VRCustomOptions vRCustomOptions;
			if (this.m_CustomOptions.TryGetValue(key, out vRCustomOptions))
			{
				vRCustomOptions.IsExpanded = false;
			}
		}

		private void VRDevicesGUIOneBuildTarget(BuildTargetGroup targetGroup)
		{
			if (!this.m_VRDeviceActiveUI.ContainsKey(targetGroup))
			{
				ReorderableList reorderableList = new ReorderableList(VREditor.GetVREnabledDevicesOnTargetGroup(targetGroup), typeof(VRDeviceInfoEditor), true, true, true, true);
				reorderableList.onAddDropdownCallback = delegate(Rect rect, ReorderableList list)
				{
					this.AddVRDeviceElement(targetGroup, rect, list);
				};
				reorderableList.onRemoveCallback = delegate(ReorderableList list)
				{
					this.RemoveVRDeviceElement(targetGroup, list);
				};
				reorderableList.onReorderCallback = delegate(ReorderableList list)
				{
					this.ReorderVRDeviceElement(targetGroup, list);
				};
				reorderableList.drawElementCallback = delegate(Rect rect, int index, bool isActive, bool isFocused)
				{
					this.DrawVRDeviceElement(targetGroup, rect, index, isActive, isFocused);
				};
				reorderableList.drawHeaderCallback = delegate(Rect rect)
				{
					GUI.Label(rect, "Virtual Reality SDKs", EditorStyles.label);
				};
				reorderableList.elementHeightCallback = ((int index) => this.GetVRDeviceElementHeight(targetGroup, index));
				reorderableList.onSelectCallback = delegate(ReorderableList list)
				{
					this.SelectVRDeviceElement(targetGroup, list);
				};
				this.m_VRDeviceActiveUI.Add(targetGroup, reorderableList);
			}
			this.m_VRDeviceActiveUI[targetGroup].DoLayoutList();
			if (this.m_VRDeviceActiveUI[targetGroup].list.Count == 0)
			{
				EditorGUILayout.HelpBox("Must add at least one Virtual Reality SDK.", MessageType.Warning);
			}
		}
	}
}
