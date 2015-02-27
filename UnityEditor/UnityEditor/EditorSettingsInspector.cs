using System;
using UnityEditor.VersionControl;
using UnityEditorInternal;
using UnityEngine;
namespace UnityEditor
{
	[CustomEditor(typeof(EditorSettings))]
	internal class EditorSettingsInspector : Editor
	{
		private struct PopupElement
		{
			public readonly bool requiresTeamLicense;
			public readonly bool requiresProLicense;
			public readonly GUIContent content;
			public bool Enabled
			{
				get
				{
					return (!this.requiresTeamLicense || InternalEditorUtility.HasMaint()) && (!this.requiresProLicense || InternalEditorUtility.HasPro());
				}
			}
			public PopupElement(string content, bool requiresTeamLicense, bool requiresProLicense)
			{
				this.content = new GUIContent(content);
				this.requiresTeamLicense = requiresTeamLicense;
				this.requiresProLicense = requiresProLicense;
			}
		}
		private EditorSettingsInspector.PopupElement[] vcDefaultPopupList = new EditorSettingsInspector.PopupElement[]
		{
			new EditorSettingsInspector.PopupElement(ExternalVersionControl.Disabled, false, false),
			new EditorSettingsInspector.PopupElement(ExternalVersionControl.Generic, false, false),
			new EditorSettingsInspector.PopupElement(ExternalVersionControl.AssetServer, true, false)
		};
		private EditorSettingsInspector.PopupElement[] vcPopupList;
		private EditorSettingsInspector.PopupElement[] serializationPopupList = new EditorSettingsInspector.PopupElement[]
		{
			new EditorSettingsInspector.PopupElement("Mixed", false, false),
			new EditorSettingsInspector.PopupElement("Force Binary", false, false),
			new EditorSettingsInspector.PopupElement("Force Text", false, false)
		};
		private EditorSettingsInspector.PopupElement[] remoteDeviceList = new EditorSettingsInspector.PopupElement[]
		{
			new EditorSettingsInspector.PopupElement("None", false, false),
			new EditorSettingsInspector.PopupElement("Any Android Device", false, false),
			new EditorSettingsInspector.PopupElement("Any iOS Device", false, false)
		};
		private EditorSettingsInspector.PopupElement[] behaviorPopupList = new EditorSettingsInspector.PopupElement[]
		{
			new EditorSettingsInspector.PopupElement("3D", false, false),
			new EditorSettingsInspector.PopupElement("2D", false, false)
		};
		private EditorSettingsInspector.PopupElement[] spritePackerPopupList = new EditorSettingsInspector.PopupElement[]
		{
			new EditorSettingsInspector.PopupElement("Disabled", false, false),
			new EditorSettingsInspector.PopupElement("Enabled For Builds", false, false),
			new EditorSettingsInspector.PopupElement("Always Enabled", false, false)
		};
		private string[] logLevelPopupList = new string[]
		{
			"Verbose",
			"Info",
			"Notice",
			"Fatal"
		};
		public void OnEnable()
		{
			Plugin[] availablePlugins = Plugin.availablePlugins;
			this.vcPopupList = new EditorSettingsInspector.PopupElement[availablePlugins.Length + this.vcDefaultPopupList.Length];
			Array.Copy(this.vcDefaultPopupList, this.vcPopupList, this.vcDefaultPopupList.Length);
			int num = 0;
			int i = this.vcDefaultPopupList.Length;
			while (i < this.vcPopupList.Length)
			{
				this.vcPopupList[i] = new EditorSettingsInspector.PopupElement(availablePlugins[num].name, true, false);
				i++;
				num++;
			}
		}
		public override void OnInspectorGUI()
		{
			bool enabled = GUI.enabled;
			this.ShowUnityRemoteGUI(enabled);
			GUILayout.Space(10f);
			GUI.enabled = true;
			GUILayout.Label("Version Control", EditorStyles.boldLabel, new GUILayoutOption[0]);
			GUI.enabled = enabled;
			ExternalVersionControl selvc = EditorSettings.externalVersionControl;
			int num = Array.FindIndex<EditorSettingsInspector.PopupElement>(this.vcPopupList, (EditorSettingsInspector.PopupElement cand) => cand.content.text == selvc);
			if (num < 0)
			{
				num = 0;
			}
			GUIContent content = new GUIContent(this.vcPopupList[num].content);
			Rect rect = GUILayoutUtility.GetRect(content, EditorStyles.popup);
			rect = EditorGUI.PrefixLabel(rect, 0, new GUIContent("Mode"));
			if (EditorGUI.ButtonMouseDown(rect, content, FocusType.Passive, EditorStyles.popup))
			{
				this.DoPopup(rect, this.vcPopupList, num, new GenericMenu.MenuFunction2(this.SetVersionControlSystem));
			}
			if (this.VersionControlSystemHasGUI())
			{
				GUI.enabled = true;
				bool enabled2 = false;
				if (EditorSettings.externalVersionControl == ExternalVersionControl.AssetServer)
				{
					EditorUserSettings.SetConfigValue("vcUsername", EditorGUILayout.TextField("User", EditorUserSettings.GetConfigValue("vcUsername"), new GUILayoutOption[0]));
					EditorUserSettings.SetConfigValue("vcPassword", EditorGUILayout.PasswordField("Password", EditorUserSettings.GetConfigValue("vcPassword"), new GUILayoutOption[0]));
				}
				else
				{
					if (!(EditorSettings.externalVersionControl == ExternalVersionControl.Generic))
					{
						if (!(EditorSettings.externalVersionControl == ExternalVersionControl.Disabled))
						{
							ConfigField[] activeConfigFields = Provider.GetActiveConfigFields();
							enabled2 = true;
							ConfigField[] array = activeConfigFields;
							for (int i = 0; i < array.Length; i++)
							{
								ConfigField configField = array[i];
								string configValue = EditorUserSettings.GetConfigValue(configField.name);
								string text;
								if (configField.isPassword)
								{
									text = EditorGUILayout.PasswordField(configField.label, configValue, new GUILayoutOption[0]);
								}
								else
								{
									text = EditorGUILayout.TextField(configField.label, configValue, new GUILayoutOption[0]);
								}
								if (text != configValue)
								{
									EditorUserSettings.SetConfigValue(configField.name, text);
								}
								if (configField.isRequired && string.IsNullOrEmpty(text))
								{
									enabled2 = false;
								}
							}
						}
					}
				}
				string logLevel = EditorUserSettings.GetConfigValue("vcSharedLogLevel");
				int num2 = Array.FindIndex<string>(this.logLevelPopupList, (string item) => item.ToLower() == logLevel);
				if (num2 == -1)
				{
					logLevel = "info";
				}
				int num3 = EditorGUILayout.Popup("Log Level", Math.Abs(num2), this.logLevelPopupList, new GUILayoutOption[0]);
				if (num3 != num2)
				{
					EditorUserSettings.SetConfigValue("vcSharedLogLevel", this.logLevelPopupList[num3].ToLower());
				}
				GUI.enabled = enabled;
				string label = "Connected";
				if (Provider.onlineState == OnlineState.Updating)
				{
					label = "Connecting...";
				}
				else
				{
					if (Provider.onlineState == OnlineState.Offline)
					{
						label = "Disconnected";
					}
				}
				EditorGUILayout.LabelField("Status", label, new GUILayoutOption[0]);
				if (Provider.onlineState != OnlineState.Online && !string.IsNullOrEmpty(Provider.offlineReason))
				{
					GUI.enabled = false;
					GUILayout.TextArea(Provider.offlineReason, new GUILayoutOption[0]);
					GUI.enabled = enabled;
				}
				GUILayout.BeginHorizontal(new GUILayoutOption[0]);
				GUILayout.FlexibleSpace();
				GUI.enabled = enabled2;
				if (GUILayout.Button("Connect", EditorStyles.miniButton, new GUILayoutOption[0]))
				{
					Provider.UpdateSettings();
				}
				GUILayout.EndHorizontal();
				EditorUserSettings.AutomaticAdd = EditorGUILayout.Toggle("Automatic add", EditorUserSettings.AutomaticAdd, new GUILayoutOption[0]);
				if (Provider.requiresNetwork)
				{
					bool flag = EditorGUILayout.Toggle("Work Offline", EditorUserSettings.WorkOffline, new GUILayoutOption[0]);
					if (flag != EditorUserSettings.WorkOffline)
					{
						if (flag && !EditorUtility.DisplayDialog("Confirm working offline", "Working offline and making changes to your assets means that you will have to manually integrate changes back into version control using your standard version control client before you stop working offline in Unity. Make sure you know what you are doing.", "Work offline", "Cancel"))
						{
							flag = false;
						}
						EditorUserSettings.WorkOffline = flag;
						EditorApplication.RequestRepaintAllViews();
					}
				}
				GUI.enabled = enabled;
				this.DrawOverlayDescriptions();
			}
			GUILayout.Space(10f);
			GUILayout.Label("WWW Security Emulation", EditorStyles.boldLabel, new GUILayoutOption[0]);
			string text2 = EditorGUILayout.TextField("Host URL", EditorSettings.webSecurityEmulationHostUrl, new GUILayoutOption[0]);
			if (text2 != EditorSettings.webSecurityEmulationHostUrl)
			{
				EditorSettings.webSecurityEmulationHostUrl = text2;
			}
			GUILayout.Space(10f);
			GUI.enabled = true;
			GUILayout.Label("Asset Serialization", EditorStyles.boldLabel, new GUILayoutOption[0]);
			GUI.enabled = enabled;
			content = new GUIContent(this.serializationPopupList[(int)EditorSettings.serializationMode].content);
			rect = GUILayoutUtility.GetRect(content, EditorStyles.popup);
			rect = EditorGUI.PrefixLabel(rect, 0, new GUIContent("Mode"));
			if (EditorGUI.ButtonMouseDown(rect, content, FocusType.Passive, EditorStyles.popup))
			{
				this.DoPopup(rect, this.serializationPopupList, (int)EditorSettings.serializationMode, new GenericMenu.MenuFunction2(this.SetAssetSerializationMode));
			}
			GUILayout.Space(10f);
			GUI.enabled = true;
			GUILayout.Label("Default Behavior Mode", EditorStyles.boldLabel, new GUILayoutOption[0]);
			GUI.enabled = enabled;
			int num4 = Mathf.Clamp((int)EditorSettings.defaultBehaviorMode, 0, this.behaviorPopupList.Length - 1);
			content = new GUIContent(this.behaviorPopupList[num4].content);
			rect = GUILayoutUtility.GetRect(content, EditorStyles.popup);
			rect = EditorGUI.PrefixLabel(rect, 0, new GUIContent("Mode"));
			if (EditorGUI.ButtonMouseDown(rect, content, FocusType.Passive, EditorStyles.popup))
			{
				this.DoPopup(rect, this.behaviorPopupList, num4, new GenericMenu.MenuFunction2(this.SetDefaultBehaviorMode));
			}
			GUILayout.Space(10f);
			GUI.enabled = true;
			GUILayout.Label("Sprite Packer", EditorStyles.boldLabel, new GUILayoutOption[0]);
			GUI.enabled = enabled;
			num4 = Mathf.Clamp((int)EditorSettings.spritePackerMode, 0, this.spritePackerPopupList.Length - 1);
			content = new GUIContent(this.spritePackerPopupList[num4].content);
			rect = GUILayoutUtility.GetRect(content, EditorStyles.popup);
			rect = EditorGUI.PrefixLabel(rect, 0, new GUIContent("Mode"));
			if (EditorGUI.ButtonMouseDown(rect, content, FocusType.Passive, EditorStyles.popup))
			{
				this.DoPopup(rect, this.spritePackerPopupList, num4, new GenericMenu.MenuFunction2(this.SetSpritePackerMode));
			}
		}
		private void ShowUnityRemoteGUI(bool editorEnabled)
		{
			GUI.enabled = true;
			GUILayout.Label("Unity Remote", EditorStyles.boldLabel, new GUILayoutOption[0]);
			GUI.enabled = editorEnabled;
			string unityRemoteDevice = EditorSettings.unityRemoteDevice;
			int num = 0;
			for (int i = 0; i < this.remoteDeviceList.Length; i++)
			{
				if (this.remoteDeviceList[i].content.text == unityRemoteDevice)
				{
					num = i;
				}
			}
			GUIContent content = new GUIContent(this.remoteDeviceList[num].content);
			Rect rect = GUILayoutUtility.GetRect(content, EditorStyles.popup);
			rect = EditorGUI.PrefixLabel(rect, 0, new GUIContent("Device"));
			if (EditorGUI.ButtonMouseDown(rect, content, FocusType.Passive, EditorStyles.popup))
			{
				this.DoPopup(rect, this.remoteDeviceList, num, new GenericMenu.MenuFunction2(this.SetUnityRemoteDevice));
			}
		}
		private void DrawOverlayDescriptions()
		{
			Texture2D overlayAtlas = Provider.overlayAtlas;
			if (overlayAtlas == null)
			{
				return;
			}
			GUILayout.Space(10f);
			GUILayout.Label("Overlay legends", EditorStyles.boldLabel, new GUILayoutOption[0]);
			this.DrawOverlayDescription(Asset.States.Local);
			this.DrawOverlayDescription(Asset.States.OutOfSync);
			this.DrawOverlayDescription(Asset.States.CheckedOutLocal);
			this.DrawOverlayDescription(Asset.States.CheckedOutRemote);
			this.DrawOverlayDescription(Asset.States.DeletedLocal);
			this.DrawOverlayDescription(Asset.States.DeletedRemote);
			this.DrawOverlayDescription(Asset.States.AddedLocal);
			this.DrawOverlayDescription(Asset.States.AddedRemote);
			this.DrawOverlayDescription(Asset.States.Conflicted);
			this.DrawOverlayDescription(Asset.States.LockedLocal);
			this.DrawOverlayDescription(Asset.States.LockedRemote);
		}
		private void DrawOverlayDescription(Asset.States state)
		{
			Rect atlasRectForState = Provider.GetAtlasRectForState((int)state);
			if (atlasRectForState.width == 0f)
			{
				return;
			}
			Texture2D overlayAtlas = Provider.overlayAtlas;
			if (overlayAtlas == null)
			{
				return;
			}
			GUILayout.Label("    " + Asset.StateToString(state), EditorStyles.miniLabel, new GUILayoutOption[0]);
			Rect lastRect = GUILayoutUtility.GetLastRect();
			lastRect.width = 16f;
			GUI.DrawTextureWithTexCoords(lastRect, overlayAtlas, atlasRectForState);
		}
		private void DoPopup(Rect popupRect, EditorSettingsInspector.PopupElement[] elements, int selectedIndex, GenericMenu.MenuFunction2 func)
		{
			GenericMenu genericMenu = new GenericMenu();
			for (int i = 0; i < elements.Length; i++)
			{
				EditorSettingsInspector.PopupElement popupElement = elements[i];
				if (popupElement.Enabled)
				{
					genericMenu.AddItem(popupElement.content, i == selectedIndex, func, i);
				}
				else
				{
					genericMenu.AddDisabledItem(popupElement.content);
				}
			}
			genericMenu.DropDown(popupRect);
		}
		private bool VersionControlSystemHasGUI()
		{
			ExternalVersionControl d = EditorSettings.externalVersionControl;
			return d != ExternalVersionControl.Disabled && d != ExternalVersionControl.AutoDetect && d != ExternalVersionControl.AssetServer && d != ExternalVersionControl.Generic;
		}
		private void SetVersionControlSystem(object data)
		{
			int num = (int)data;
			if (num < 0 && num >= this.vcPopupList.Length)
			{
				return;
			}
			EditorSettingsInspector.PopupElement popupElement = this.vcPopupList[num];
			string externalVersionControl = EditorSettings.externalVersionControl;
			EditorSettings.externalVersionControl = popupElement.content.text;
			Provider.UpdateSettings();
			AssetDatabase.Refresh();
			if (externalVersionControl != popupElement.content.text)
			{
				if (popupElement.content.text == ExternalVersionControl.AssetServer || popupElement.content.text == ExternalVersionControl.Disabled || popupElement.content.text == ExternalVersionControl.Generic)
				{
					WindowPending.CloseAllWindows();
				}
				else
				{
					ASMainWindow[] array = Resources.FindObjectsOfTypeAll(typeof(ASMainWindow)) as ASMainWindow[];
					ASMainWindow aSMainWindow = (array.Length <= 0) ? null : array[0];
					if (aSMainWindow != null)
					{
						aSMainWindow.Close();
					}
				}
			}
		}
		private void SetAssetSerializationMode(object data)
		{
			int serializationMode = (int)data;
			EditorSettings.serializationMode = (SerializationMode)serializationMode;
		}
		private void SetUnityRemoteDevice(object data)
		{
			EditorSettings.unityRemoteDevice = this.remoteDeviceList[(int)data].content.text;
		}
		private void SetDefaultBehaviorMode(object data)
		{
			int defaultBehaviorMode = (int)data;
			EditorSettings.defaultBehaviorMode = (EditorBehaviorMode)defaultBehaviorMode;
		}
		private void SetSpritePackerMode(object data)
		{
			int spritePackerMode = (int)data;
			EditorSettings.spritePackerMode = (SpritePackerMode)spritePackerMode;
		}
	}
}
