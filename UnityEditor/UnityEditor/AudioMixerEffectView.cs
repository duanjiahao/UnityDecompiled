using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEditor.Audio;
using UnityEngine;

namespace UnityEditor
{
	internal class AudioMixerEffectView
	{
		private static class Texts
		{
			public static GUIContent editInPlaymode = new GUIContent("Edit in Playmode");

			public static GUIContent pitch = new GUIContent("Pitch");

			public static GUIContent addEffect = new GUIContent("Add Effect");

			public static GUIContent volume = new GUIContent("Volume");

			public static GUIContent sendLevel = new GUIContent("Send level");

			public static GUIContent bus = new GUIContent("Receive");

			public static GUIContent none = new GUIContent("None");

			public static GUIContent wet = new GUIContent("Wet", "Enables/disables wet/dry ratio on this effect. Note that this makes the DSP graph more complex and requires additional CPU and memory, so use it only when necessary.");

			public static string dB = "dB";

			public static string percentage = "%";

			public static string cpuFormatString = " - CPU: {0:#0.00}%";
		}

		private class EffectDragging
		{
			private readonly Color kMoveColorBorderAllowed = new Color(1f, 1f, 1f, 1f);

			private readonly Color kMoveColorHiAllowed = new Color(1f, 1f, 1f, 0.3f);

			private readonly Color kMoveColorLoAllowed = new Color(1f, 1f, 1f, 0f);

			private readonly Color kMoveColorBorderDisallowed = new Color(0.8f, 0f, 0f, 1f);

			private readonly Color kMoveColorHiDisallowed = new Color(1f, 0f, 0f, 0.3f);

			private readonly Color kMoveColorLoDisallowed = new Color(1f, 0f, 0f, 0f);

			private readonly int m_DragControlID = 0;

			private int m_MovingSrcIndex = -1;

			private int m_MovingDstIndex = -1;

			private bool m_MovingEffectAllowed = false;

			private float m_MovingPos = 0f;

			private Rect m_MovingRect = new Rect(0f, 0f, 0f, 0f);

			private float m_DragHighlightPos = -1f;

			private float m_DragHighlightHeight = 2f;

			public int dragControlID
			{
				get
				{
					return this.m_DragControlID;
				}
			}

			private bool isDragging
			{
				get
				{
					return this.m_MovingSrcIndex != -1 && GUIUtility.hotControl == this.m_DragControlID;
				}
			}

			public EffectDragging()
			{
				this.m_DragControlID = GUIUtility.GetPermanentControlID();
			}

			public bool IsDraggingIndex(int effectIndex)
			{
				return this.m_MovingSrcIndex == effectIndex && GUIUtility.hotControl == this.m_DragControlID;
			}

			public void HandleDragElement(int effectIndex, Rect effectRect, Rect dragRect, AudioMixerGroupController group, List<AudioMixerGroupController> allGroups)
			{
				Event current = Event.current;
				EventType typeForControl = current.GetTypeForControl(this.m_DragControlID);
				if (typeForControl != EventType.MouseDown)
				{
					if (typeForControl == EventType.Repaint)
					{
						if (effectIndex == this.m_MovingSrcIndex)
						{
							using (new EditorGUI.DisabledScope(true))
							{
								AudioMixerDrawUtils.styles.channelStripAreaBackground.Draw(effectRect, false, false, false, false);
							}
						}
					}
				}
				else if (current.button == 0 && dragRect.Contains(current.mousePosition) && GUIUtility.hotControl == 0)
				{
					this.m_MovingSrcIndex = effectIndex;
					this.m_MovingPos = current.mousePosition.y;
					this.m_MovingRect = new Rect(effectRect.x, effectRect.y - this.m_MovingPos, effectRect.width, effectRect.height);
					GUIUtility.hotControl = this.m_DragControlID;
					EditorGUIUtility.SetWantsMouseJumping(1);
					current.Use();
				}
				if (this.isDragging)
				{
					float num = effectRect.height * 0.5f;
					float num2 = current.mousePosition.y - effectRect.y - num;
					if (Mathf.Abs(num2) <= num)
					{
						int num3 = (num2 >= 0f) ? (effectIndex + 1) : effectIndex;
						if (num3 != this.m_MovingDstIndex)
						{
							this.m_DragHighlightPos = ((num2 >= 0f) ? (effectRect.y + effectRect.height) : effectRect.y);
							this.m_MovingDstIndex = num3;
							this.m_MovingEffectAllowed = !AudioMixerController.WillMovingEffectCauseFeedback(allGroups, group, this.m_MovingSrcIndex, group, num3, null);
						}
					}
					if (this.m_MovingDstIndex == this.m_MovingSrcIndex || this.m_MovingDstIndex == this.m_MovingSrcIndex + 1)
					{
						this.m_DragHighlightPos = 0f;
					}
				}
			}

			public void HandleDragging(Rect totalRect, AudioMixerGroupController group, AudioMixerController controller)
			{
				if (this.isDragging)
				{
					Event current = Event.current;
					EventType typeForControl = current.GetTypeForControl(this.m_DragControlID);
					if (typeForControl != EventType.MouseDrag)
					{
						if (typeForControl != EventType.MouseUp)
						{
							if (typeForControl == EventType.Repaint)
							{
								if (this.m_DragHighlightPos > 0f)
								{
									float width = totalRect.width;
									Color color = (!this.m_MovingEffectAllowed) ? this.kMoveColorLoDisallowed : this.kMoveColorLoAllowed;
									Color color2 = (!this.m_MovingEffectAllowed) ? this.kMoveColorHiDisallowed : this.kMoveColorHiAllowed;
									Color color3 = (!this.m_MovingEffectAllowed) ? this.kMoveColorBorderDisallowed : this.kMoveColorBorderAllowed;
									AudioMixerDrawUtils.DrawGradientRect(new Rect(this.m_MovingRect.x, this.m_DragHighlightPos - 15f, width, 15f), color, color2);
									AudioMixerDrawUtils.DrawGradientRect(new Rect(this.m_MovingRect.x, this.m_DragHighlightPos, width, 15f), color2, color);
									AudioMixerDrawUtils.DrawGradientRect(new Rect(this.m_MovingRect.x, this.m_DragHighlightPos - this.m_DragHighlightHeight / 2f, width, this.m_DragHighlightHeight), color3, color3);
								}
							}
						}
						else
						{
							current.Use();
							if (this.m_MovingSrcIndex != -1)
							{
								if (this.m_MovingDstIndex != -1 && this.m_MovingEffectAllowed)
								{
									List<AudioMixerEffectController> list = group.effects.ToList<AudioMixerEffectController>();
									if (AudioMixerController.MoveEffect(ref list, this.m_MovingSrcIndex, ref list, this.m_MovingDstIndex))
									{
										group.effects = list.ToArray();
									}
								}
								this.m_MovingSrcIndex = -1;
								this.m_MovingDstIndex = -1;
								controller.m_HighlightEffectIndex = -1;
								if (GUIUtility.hotControl == this.m_DragControlID)
								{
									GUIUtility.hotControl = 0;
								}
								EditorGUIUtility.SetWantsMouseJumping(0);
								AudioMixerUtility.RepaintAudioMixerAndInspectors();
								GUIUtility.ExitGUI();
							}
						}
					}
					else
					{
						this.m_MovingPos = current.mousePosition.y;
						current.Use();
					}
				}
			}
		}

		private const float kMinPitch = 0.01f;

		private const float kMaxPitch = 10f;

		private const int kLabelWidth = 170;

		private const int kTextboxWidth = 70;

		private AudioMixerGroupController m_PrevGroup = null;

		private readonly AudioMixerEffectView.EffectDragging m_EffectDragging;

		private int m_LastNumChannels = 0;

		private AudioMixerEffectPlugin m_SharedPlugin = new AudioMixerEffectPlugin();

		private Dictionary<string, IAudioEffectPluginGUI> m_CustomEffectGUIs = new Dictionary<string, IAudioEffectPluginGUI>();

		[CompilerGenerated]
		private static GenericMenu.MenuFunction2 <>f__mg$cache0;

		public AudioMixerEffectView()
		{
			this.m_EffectDragging = new AudioMixerEffectView.EffectDragging();
			Type pluginType = typeof(IAudioEffectPluginGUI);
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			for (int i = 0; i < assemblies.Length; i++)
			{
				Assembly assembly = assemblies[i];
				try
				{
					Type[] types = assembly.GetTypes();
					foreach (Type current in from t in types
					where !t.IsAbstract && pluginType.IsAssignableFrom(t)
					select t)
					{
						this.RegisterCustomGUI(Activator.CreateInstance(current) as IAudioEffectPluginGUI);
					}
				}
				catch (Exception)
				{
				}
			}
		}

		public bool RegisterCustomGUI(IAudioEffectPluginGUI gui)
		{
			string name = gui.Name;
			bool result;
			if (this.m_CustomEffectGUIs.ContainsKey(name))
			{
				IAudioEffectPluginGUI audioEffectPluginGUI = this.m_CustomEffectGUIs[name];
				Debug.LogError("Attempt to register custom GUI for plugin " + name + " failed as another plugin is already registered under this name.");
				Debug.LogError(string.Concat(new string[]
				{
					"Plugin trying to register itself: ",
					gui.Description,
					" (Vendor: ",
					gui.Vendor,
					")"
				}));
				Debug.LogError(string.Concat(new string[]
				{
					"Plugin already registered: ",
					audioEffectPluginGUI.Description,
					" (Vendor: ",
					audioEffectPluginGUI.Vendor,
					")"
				}));
				result = false;
			}
			else
			{
				this.m_CustomEffectGUIs[name] = gui;
				result = true;
			}
			return result;
		}

		public void OnGUI(AudioMixerGroupController group)
		{
			if (!(group == null))
			{
				AudioMixerController controller = group.controller;
				List<AudioMixerGroupController> allAudioGroupsSlow = controller.GetAllAudioGroupsSlow();
				Dictionary<AudioMixerEffectController, AudioMixerGroupController> dictionary = new Dictionary<AudioMixerEffectController, AudioMixerGroupController>();
				foreach (AudioMixerGroupController current in allAudioGroupsSlow)
				{
					AudioMixerEffectController[] effects = current.effects;
					for (int i = 0; i < effects.Length; i++)
					{
						AudioMixerEffectController key = effects[i];
						dictionary[key] = current;
					}
				}
				Rect totalRect = EditorGUILayout.BeginVertical(new GUILayoutOption[0]);
				if (EditorApplication.isPlaying)
				{
					GUILayout.BeginHorizontal(new GUILayoutOption[0]);
					GUILayout.FlexibleSpace();
					EditorGUI.BeginChangeCheck();
					GUILayout.Toggle(AudioSettings.editingInPlaymode, AudioMixerEffectView.Texts.editInPlaymode, EditorStyles.miniButton, new GUILayoutOption[]
					{
						GUILayout.Width(120f)
					});
					if (EditorGUI.EndChangeCheck())
					{
						AudioSettings.editingInPlaymode = !AudioSettings.editingInPlaymode;
					}
					GUILayout.FlexibleSpace();
					GUILayout.EndHorizontal();
				}
				using (new EditorGUI.DisabledScope(!AudioMixerController.EditingTargetSnapshot()))
				{
					if (group != this.m_PrevGroup)
					{
						this.m_PrevGroup = group;
						controller.m_HighlightEffectIndex = -1;
						AudioMixerUtility.RepaintAudioMixerAndInspectors();
					}
					AudioMixerEffectView.DoInitialModule(group, controller, allAudioGroupsSlow);
					for (int j = 0; j < group.effects.Length; j++)
					{
						this.DoEffectGUI(j, group, allAudioGroupsSlow, dictionary, ref controller.m_HighlightEffectIndex);
					}
					this.m_EffectDragging.HandleDragging(totalRect, group, controller);
					GUILayout.Space(10f);
					EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
					GUILayout.FlexibleSpace();
					if (EditorGUILayout.DropdownButton(AudioMixerEffectView.Texts.addEffect, FocusType.Passive, GUISkin.current.button, new GUILayoutOption[0]))
					{
						GenericMenu genericMenu = new GenericMenu();
						Rect last = GUILayoutUtility.topLevel.GetLast();
						AudioMixerGroupController[] groups = new AudioMixerGroupController[]
						{
							group
						};
						AudioMixerChannelStripView.AddEffectItemsToMenu(controller, groups, group.effects.Length, string.Empty, genericMenu);
						genericMenu.DropDown(last);
					}
					EditorGUILayout.EndHorizontal();
				}
				EditorGUILayout.EndVertical();
			}
		}

		public static float DoInitialModule(AudioMixerGroupController group, AudioMixerController controller, List<AudioMixerGroupController> allGroups)
		{
			Rect rect = EditorGUILayout.BeginVertical(EditorStyles.inspectorDefaultMargins, new GUILayoutOption[0]);
			float valueForPitch = group.GetValueForPitch(controller, controller.TargetSnapshot);
			if (AudioMixerEffectGUI.Slider(AudioMixerEffectView.Texts.pitch, ref valueForPitch, 100f, 1f, AudioMixerEffectView.Texts.percentage, 0.01f, 10f, controller, new AudioGroupParameterPath(group, group.GetGUIDForPitch()), new GUILayoutOption[0]))
			{
				Undo.RecordObject(controller.TargetSnapshot, "Change Pitch");
				group.SetValueForPitch(controller, controller.TargetSnapshot, valueForPitch);
			}
			GUILayout.Space(5f);
			EditorGUILayout.EndVertical();
			AudioMixerDrawUtils.DrawSplitter();
			return rect.height;
		}

		public void DoEffectGUI(int effectIndex, AudioMixerGroupController group, List<AudioMixerGroupController> allGroups, Dictionary<AudioMixerEffectController, AudioMixerGroupController> effectMap, ref int highlightEffectIndex)
		{
			Event current = Event.current;
			AudioMixerController controller = group.controller;
			AudioMixerEffectController audioMixerEffectController = group.effects[effectIndex];
			MixerParameterDefinition[] effectParameters = MixerEffectDefinitions.GetEffectParameters(audioMixerEffectController.effectName);
			Rect effectRect = EditorGUILayout.BeginVertical(new GUILayoutOption[0]);
			bool flag = effectRect.Contains(current.mousePosition);
			EventType typeForControl = current.GetTypeForControl(this.m_EffectDragging.dragControlID);
			if (typeForControl == EventType.MouseMove && flag && highlightEffectIndex != effectIndex)
			{
				highlightEffectIndex = effectIndex;
				AudioMixerUtility.RepaintAudioMixerAndInspectors();
			}
			Vector2 vector = EditorStyles.iconButton.CalcSize(EditorGUI.GUIContents.titleSettingsIcon);
			Rect rect = GUILayoutUtility.GetRect(1f, 17f);
			Rect rect2 = new Rect(rect.x + 6f, rect.y + 5f, 6f, 6f);
			Rect position = new Rect(rect.x + 8f + 6f, rect.y, rect.width - 8f - 6f - vector.x - 5f, rect.height);
			Rect rect3 = new Rect(position.xMax, rect.y, vector.x, vector.y);
			Rect rect4 = new Rect(rect.x, rect.y, rect.width - vector.x - 5f, rect.height);
			bool flag2 = EditorPrefs.GetBool(AudioMixerGroupEditor.kPrefKeyForShowCpuUsage, false) && EditorUtility.audioProfilingEnabled;
			float num = (!EditorGUIUtility.isProSkin) ? 1f : 0.1f;
			Color color = new Color(num, num, num, 0.2f);
			Color color2 = GUI.color;
			GUI.color = color;
			GUI.DrawTexture(rect, EditorGUIUtility.whiteTexture);
			GUI.color = color2;
			Color effectColor = AudioMixerDrawUtils.GetEffectColor(audioMixerEffectController);
			EditorGUI.DrawRect(rect2, effectColor);
			GUI.Label(position, (!flag2) ? audioMixerEffectController.effectName : (audioMixerEffectController.effectName + string.Format(AudioMixerEffectView.Texts.cpuFormatString, audioMixerEffectController.GetCPUUsage(controller))), EditorStyles.boldLabel);
			if (EditorGUI.DropdownButton(rect3, EditorGUI.GUIContents.titleSettingsIcon, FocusType.Passive, EditorStyles.iconButton))
			{
				AudioMixerEffectView.ShowEffectContextMenu(group, audioMixerEffectController, effectIndex, controller, rect3);
			}
			if (current.type == EventType.ContextClick && rect.Contains(current.mousePosition))
			{
				AudioMixerEffectView.ShowEffectContextMenu(group, audioMixerEffectController, effectIndex, controller, new Rect(current.mousePosition.x, rect.y, 1f, rect.height));
				current.Use();
			}
			if (typeForControl == EventType.Repaint)
			{
				EditorGUIUtility.AddCursorRect(rect4, MouseCursor.ResizeVertical, this.m_EffectDragging.dragControlID);
			}
			using (new EditorGUI.DisabledScope(audioMixerEffectController.bypass || group.bypassEffects))
			{
				EditorGUILayout.BeginVertical(EditorStyles.inspectorDefaultMargins, new GUILayoutOption[0]);
				if (audioMixerEffectController.IsAttenuation())
				{
					EditorGUILayout.BeginVertical(new GUILayoutOption[0]);
					float valueForVolume = group.GetValueForVolume(controller, controller.TargetSnapshot);
					if (AudioMixerEffectGUI.Slider(AudioMixerEffectView.Texts.volume, ref valueForVolume, 1f, 1f, AudioMixerEffectView.Texts.dB, AudioMixerController.kMinVolume, AudioMixerController.GetMaxVolume(), controller, new AudioGroupParameterPath(group, group.GetGUIDForVolume()), new GUILayoutOption[0]))
					{
						Undo.RecordObject(controller.TargetSnapshot, "Change Volume Fader");
						group.SetValueForVolume(controller, controller.TargetSnapshot, valueForVolume);
						AudioMixerUtility.RepaintAudioMixerAndInspectors();
					}
					float[] array = new float[9];
					float[] array2 = new float[9];
					int num2 = group.controller.GetGroupVUInfo(group.groupID, true, ref array, ref array2);
					if (current.type == EventType.Layout)
					{
						this.m_LastNumChannels = num2;
					}
					else
					{
						if (num2 != this.m_LastNumChannels)
						{
							HandleUtility.Repaint();
						}
						num2 = this.m_LastNumChannels;
					}
					GUILayout.Space(4f);
					for (int i = 0; i < num2; i++)
					{
						float value = 1f - AudioMixerController.VolumeToScreenMapping(Mathf.Clamp(array[i], AudioMixerController.kMinVolume, AudioMixerController.GetMaxVolume()), 1f, true);
						float num3 = 1f - AudioMixerController.VolumeToScreenMapping(Mathf.Clamp(array2[i], AudioMixerController.kMinVolume, AudioMixerController.GetMaxVolume()), 1f, true);
						EditorGUILayout.VUMeterHorizontal(value, num3, new GUILayoutOption[]
						{
							GUILayout.Height(10f)
						});
						if (!EditorApplication.isPlaying && num3 > 0f)
						{
							AudioMixerUtility.RepaintAudioMixerAndInspectors();
						}
					}
					GUILayout.Space(4f);
					EditorGUILayout.EndVertical();
				}
				if (audioMixerEffectController.IsSend())
				{
					GUIContent buttonContent = (!(audioMixerEffectController.sendTarget == null)) ? GUIContent.Temp(audioMixerEffectController.GetSendTargetDisplayString(effectMap)) : AudioMixerEffectView.Texts.none;
					Rect buttonRect;
					if (AudioMixerEffectGUI.PopupButton(AudioMixerEffectView.Texts.bus, buttonContent, EditorStyles.popup, out buttonRect, new GUILayoutOption[0]))
					{
						AudioMixerEffectView.ShowBusPopupMenu(effectIndex, group, allGroups, effectMap, audioMixerEffectController, buttonRect);
					}
					if (audioMixerEffectController.sendTarget != null)
					{
						float valueForMixLevel = audioMixerEffectController.GetValueForMixLevel(controller, controller.TargetSnapshot);
						if (AudioMixerEffectGUI.Slider(AudioMixerEffectView.Texts.sendLevel, ref valueForMixLevel, 1f, 1f, AudioMixerEffectView.Texts.dB, AudioMixerController.kMinVolume, AudioMixerController.kMaxEffect, controller, new AudioEffectParameterPath(group, audioMixerEffectController, audioMixerEffectController.GetGUIDForMixLevel()), new GUILayoutOption[0]))
						{
							Undo.RecordObject(controller.TargetSnapshot, "Change Send Level");
							audioMixerEffectController.SetValueForMixLevel(controller, controller.TargetSnapshot, valueForMixLevel);
							AudioMixerUtility.RepaintAudioMixerAndInspectors();
						}
					}
				}
				if (MixerEffectDefinitions.EffectCanBeSidechainTarget(audioMixerEffectController))
				{
					bool flag3 = false;
					foreach (AudioMixerGroupController current2 in allGroups)
					{
						AudioMixerEffectController[] effects = current2.effects;
						for (int j = 0; j < effects.Length; j++)
						{
							AudioMixerEffectController audioMixerEffectController2 = effects[j];
							if (audioMixerEffectController2.IsSend() && audioMixerEffectController2.sendTarget == audioMixerEffectController)
							{
								flag3 = true;
								break;
							}
						}
						if (flag3)
						{
							break;
						}
					}
					if (!flag3)
					{
						GUILayout.Label(new GUIContent("No Send sources connected.", EditorGUIUtility.warningIcon), new GUILayoutOption[0]);
					}
				}
				if (audioMixerEffectController.enableWetMix && !audioMixerEffectController.IsReceive() && !audioMixerEffectController.IsDuckVolume() && !audioMixerEffectController.IsAttenuation() && !audioMixerEffectController.IsSend())
				{
					float valueForMixLevel2 = audioMixerEffectController.GetValueForMixLevel(controller, controller.TargetSnapshot);
					if (AudioMixerEffectGUI.Slider(AudioMixerEffectView.Texts.wet, ref valueForMixLevel2, 1f, 1f, AudioMixerEffectView.Texts.dB, AudioMixerController.kMinVolume, AudioMixerController.kMaxEffect, controller, new AudioEffectParameterPath(group, audioMixerEffectController, audioMixerEffectController.GetGUIDForMixLevel()), new GUILayoutOption[0]))
					{
						Undo.RecordObject(controller.TargetSnapshot, "Change Mix Level");
						audioMixerEffectController.SetValueForMixLevel(controller, controller.TargetSnapshot, valueForMixLevel2);
						AudioMixerUtility.RepaintAudioMixerAndInspectors();
					}
				}
				bool flag4 = true;
				if (this.m_CustomEffectGUIs.ContainsKey(audioMixerEffectController.effectName))
				{
					IAudioEffectPluginGUI audioEffectPluginGUI = this.m_CustomEffectGUIs[audioMixerEffectController.effectName];
					this.m_SharedPlugin.m_Controller = controller;
					this.m_SharedPlugin.m_Effect = audioMixerEffectController;
					this.m_SharedPlugin.m_ParamDefs = effectParameters;
					flag4 = audioEffectPluginGUI.OnGUI(this.m_SharedPlugin);
				}
				if (flag4)
				{
					MixerParameterDefinition[] array3 = effectParameters;
					for (int k = 0; k < array3.Length; k++)
					{
						MixerParameterDefinition mixerParameterDefinition = array3[k];
						float valueForParameter = audioMixerEffectController.GetValueForParameter(controller, controller.TargetSnapshot, mixerParameterDefinition.name);
						if (AudioMixerEffectGUI.Slider(GUIContent.Temp(mixerParameterDefinition.name, mixerParameterDefinition.description), ref valueForParameter, mixerParameterDefinition.displayScale, mixerParameterDefinition.displayExponent, mixerParameterDefinition.units, mixerParameterDefinition.minRange, mixerParameterDefinition.maxRange, controller, new AudioEffectParameterPath(group, audioMixerEffectController, audioMixerEffectController.GetGUIDForParameter(mixerParameterDefinition.name)), new GUILayoutOption[0]))
						{
							Undo.RecordObject(controller.TargetSnapshot, "Change " + mixerParameterDefinition.name);
							audioMixerEffectController.SetValueForParameter(controller, controller.TargetSnapshot, mixerParameterDefinition.name, valueForParameter);
						}
					}
					if (effectParameters.Length > 0)
					{
						GUILayout.Space(6f);
					}
				}
			}
			this.m_EffectDragging.HandleDragElement(effectIndex, effectRect, rect4, group, allGroups);
			EditorGUILayout.EndVertical();
			EditorGUILayout.EndVertical();
			AudioMixerDrawUtils.DrawSplitter();
		}

		private static void ShowEffectContextMenu(AudioMixerGroupController group, AudioMixerEffectController effect, int effectIndex, AudioMixerController controller, Rect buttonRect)
		{
			GenericMenu genericMenu = new GenericMenu();
			if (!effect.IsReceive())
			{
				if (!effect.IsAttenuation() && !effect.IsSend() && !effect.IsDuckVolume())
				{
					genericMenu.AddItem(new GUIContent("Allow Wet Mixing (causes higher memory usage)"), effect.enableWetMix, delegate
					{
						effect.enableWetMix = !effect.enableWetMix;
					});
					genericMenu.AddItem(new GUIContent("Bypass"), effect.bypass, delegate
					{
						effect.bypass = !effect.bypass;
						controller.UpdateBypass();
						AudioMixerUtility.RepaintAudioMixerAndInspectors();
					});
					genericMenu.AddSeparator("");
				}
				genericMenu.AddItem(new GUIContent("Copy effect settings to all snapshots"), false, delegate
				{
					Undo.RecordObject(controller, "Copy effect settings to all snapshots");
					if (effect.IsAttenuation())
					{
						controller.CopyAttenuationToAllSnapshots(group, controller.TargetSnapshot);
					}
					else
					{
						controller.CopyEffectSettingsToAllSnapshots(group, effectIndex, controller.TargetSnapshot, effect.IsSend());
					}
					AudioMixerUtility.RepaintAudioMixerAndInspectors();
				});
				if (!effect.IsAttenuation() && !effect.IsSend() && !effect.IsDuckVolume() && effect.enableWetMix)
				{
					genericMenu.AddItem(new GUIContent("Copy effect settings to all snapshots, including wet level"), false, delegate
					{
						Undo.RecordObject(controller, "Copy effect settings to all snapshots, including wet level");
						controller.CopyEffectSettingsToAllSnapshots(group, effectIndex, controller.TargetSnapshot, true);
						AudioMixerUtility.RepaintAudioMixerAndInspectors();
					});
				}
				genericMenu.AddSeparator("");
			}
			AudioMixerGroupController[] groups = new AudioMixerGroupController[]
			{
				group
			};
			AudioMixerChannelStripView.AddEffectItemsToMenu(controller, groups, effectIndex, "Add effect before/", genericMenu);
			AudioMixerChannelStripView.AddEffectItemsToMenu(controller, groups, effectIndex + 1, "Add effect after/", genericMenu);
			if (!effect.IsAttenuation())
			{
				genericMenu.AddSeparator("");
				genericMenu.AddItem(new GUIContent("Remove this effect"), false, delegate
				{
					controller.ClearSendConnectionsTo(effect);
					controller.RemoveEffect(effect, group);
					AudioMixerUtility.RepaintAudioMixerAndInspectors();
				});
			}
			genericMenu.DropDown(buttonRect);
		}

		private static void ShowBusPopupMenu(int effectIndex, AudioMixerGroupController group, List<AudioMixerGroupController> allGroups, Dictionary<AudioMixerEffectController, AudioMixerGroupController> effectMap, AudioMixerEffectController effect, Rect buttonRect)
		{
			GenericMenu genericMenu = new GenericMenu();
			GenericMenu arg_38_0 = genericMenu;
			GUIContent arg_38_1 = new GUIContent("None");
			bool arg_38_2 = false;
			if (AudioMixerEffectView.<>f__mg$cache0 == null)
			{
				AudioMixerEffectView.<>f__mg$cache0 = new GenericMenu.MenuFunction2(AudioMixerChannelStripView.ConnectSendPopupCallback);
			}
			arg_38_0.AddItem(arg_38_1, arg_38_2, AudioMixerEffectView.<>f__mg$cache0, new AudioMixerChannelStripView.ConnectSendContext(effect, null));
			genericMenu.AddSeparator("");
			AudioMixerChannelStripView.AddMenuItemsForReturns(genericMenu, string.Empty, effectIndex, group, allGroups, effectMap, effect, true);
			if (genericMenu.GetItemCount() == 2)
			{
				genericMenu.AddDisabledItem(new GUIContent("No valid Receive targets found"));
			}
			genericMenu.DropDown(buttonRect);
		}
	}
}
