using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace UnityEditorInternal.VR
{
	internal class PlayerSettingsEditorVR
	{
		private SerializedProperty m_VREditorSettings;

		private Dictionary<BuildTargetGroup, VRDeviceInfoEditor[]> m_AllVRDevicesForBuildTarget = new Dictionary<BuildTargetGroup, VRDeviceInfoEditor[]>();

		private Dictionary<BuildTargetGroup, ReorderableList> m_VRDeviceActiveUI = new Dictionary<BuildTargetGroup, ReorderableList>();

		private Dictionary<string, string> m_MapVRDeviceKeyToUIString = new Dictionary<string, string>();

		private Dictionary<string, string> m_MapVRUIStringToDeviceKey = new Dictionary<string, string>();

		private Dictionary<string, VRCustomOptions> m_CustomOptions = new Dictionary<string, VRCustomOptions>();

		public PlayerSettingsEditorVR(SerializedProperty settingsEditor)
		{
			this.m_VREditorSettings = settingsEditor;
		}

		private void RefreshVRDeviceList(BuildTargetGroup targetGroup)
		{
			VRDeviceInfoEditor[] allVRDeviceInfo = VREditor.GetAllVRDeviceInfo(targetGroup);
			this.m_AllVRDevicesForBuildTarget[targetGroup] = allVRDeviceInfo;
			for (int i = 0; i < allVRDeviceInfo.Length; i++)
			{
				VRDeviceInfoEditor vRDeviceInfoEditor = allVRDeviceInfo[i];
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
					vRCustomOptions.Initialize(this.m_VREditorSettings);
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
