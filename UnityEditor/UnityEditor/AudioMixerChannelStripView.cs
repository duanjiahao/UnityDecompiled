using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Audio;
using UnityEditorInternal;
using UnityEngine;

namespace UnityEditor
{
	internal class AudioMixerChannelStripView
	{
		[Serializable]
		public class State
		{
			public int m_LastClickedInstanceID;

			public Vector2 m_ScrollPos = new Vector2(0f, 0f);
		}

		public class EffectContext
		{
			public AudioMixerController controller;

			public AudioMixerGroupController[] groups;

			public int index;

			public string name;

			public EffectContext(AudioMixerController controller, AudioMixerGroupController[] groups, int index, string name)
			{
				this.controller = controller;
				this.groups = groups;
				this.index = index;
				this.name = name;
			}
		}

		public class ConnectSendContext
		{
			public AudioMixerEffectController sendEffect;

			public AudioMixerEffectController targetEffect;

			public ConnectSendContext(AudioMixerEffectController sendEffect, AudioMixerEffectController targetEffect)
			{
				this.sendEffect = sendEffect;
				this.targetEffect = targetEffect;
			}
		}

		private class PatchSlot
		{
			public AudioMixerGroupController group;

			public float x;

			public float y;
		}

		private class BusConnection
		{
			public AudioMixerEffectController targetEffect;

			public float srcX;

			public float srcY;

			public float mixLevel;

			public Color color;

			public bool isSend;

			public bool isSelected;

			public BusConnection(float srcX, float srcY, AudioMixerEffectController targetEffect, float mixLevel, Color col, bool isSend, bool isSelected)
			{
				this.srcX = srcX;
				this.srcY = srcY;
				this.targetEffect = targetEffect;
				this.mixLevel = mixLevel;
				this.color = col;
				this.isSend = isSend;
				this.isSelected = isSelected;
			}
		}

		private class ChannelStripParams
		{
			private const float kAddEffectButtonHeight = 16f;

			public int index;

			public Rect stripRect;

			public Rect visibleRect;

			public bool visible;

			public AudioMixerGroupController group;

			public int maxEffects;

			public bool drawingBuses;

			public bool anySoloActive;

			public List<AudioMixerChannelStripView.BusConnection> busConnections = new List<AudioMixerChannelStripView.BusConnection>();

			public List<AudioMixerGroupController> rectSelectionGroups = new List<AudioMixerGroupController>();

			public List<AudioMixerGroupController> allGroups;

			public List<AudioMixerGroupController> shownGroups;

			public int numChannels;

			public float[] vuinfo_level = new float[9];

			public float[] vuinfo_peak = new float[9];

			public Dictionary<AudioMixerEffectController, AudioMixerGroupController> effectMap;

			public List<Rect> bgRects;

			public readonly int kHeaderIndex;

			public readonly int kVUMeterFaderIndex = 1;

			public readonly int kTotalVULevelIndex = 2;

			public readonly int kSoloMuteBypassIndex = 3;

			public readonly int kEffectStartIndex = 4;

			public void Init(AudioMixerController controller, Rect channelStripRect, int maxNumEffects)
			{
				this.numChannels = controller.GetGroupVUInfo(this.group.groupID, false, ref this.vuinfo_level, ref this.vuinfo_peak);
				this.maxEffects = maxNumEffects;
				this.bgRects = this.GetBackgroundRects(channelStripRect, this.group, this.maxEffects);
				this.stripRect = channelStripRect;
				this.stripRect.yMax = this.bgRects[this.bgRects.Count - 1].yMax;
			}

			private List<Rect> GetBackgroundRects(Rect r, AudioMixerGroupController group, int maxNumGroups)
			{
				List<float> list = new List<float>();
				list.AddRange(Enumerable.Repeat<float>(0f, this.kEffectStartIndex));
				list[this.kHeaderIndex] = 22f;
				list[this.kVUMeterFaderIndex] = 170f;
				list[this.kTotalVULevelIndex] = 17f;
				list[this.kSoloMuteBypassIndex] = 30f;
				for (int i = 0; i < maxNumGroups; i++)
				{
					list.Add(16f);
				}
				list.Add(10f);
				List<Rect> list2 = new List<Rect>();
				float num = r.y;
				using (List<float>.Enumerator enumerator = list.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						int num2 = (int)enumerator.Current;
						if (list2.Count > 0)
						{
							num = num;
						}
						list2.Add(new Rect(r.x, num, r.width, (float)num2));
						num += (float)num2;
					}
				}
				num += 10f;
				list2.Add(new Rect(r.x, num, r.width, 16f));
				return list2;
			}
		}

		private const float k_MinVULevel = -80f;

		private const float headerHeight = 22f;

		private const float vuHeight = 170f;

		private const float dbHeight = 17f;

		private const float soloMuteBypassHeight = 30f;

		private const float effectHeight = 16f;

		private const float spaceBetween = 0f;

		private const int channelStripSpacing = 4;

		private const float channelStripBaseWidth = 90f;

		private const float spaceBetweenMainGroupsAndReferenced = 50f;

		private const float kGridTileWidth = 12f;

		public static float kVolumeScaleMouseDrag = 1f;

		public static float kEffectScaleMouseDrag = 0.3f;

		private static Color kMoveColorHighlight = new Color(0.3f, 0.6f, 1f, 0.4f);

		private static Color kMoveSlotColHiAllowed = new Color(1f, 1f, 1f, 0.7f);

		private static Color kMoveSlotColLoAllowed = new Color(1f, 1f, 1f, 0f);

		private static Color kMoveSlotColBorderAllowed = new Color(1f, 1f, 1f, 1f);

		private static Color kMoveSlotColHiDisallowed = new Color(1f, 0f, 0f, 0.7f);

		private static Color kMoveSlotColLoDisallowed = new Color(0.8f, 0f, 0f, 0f);

		private static Color kMoveSlotColBorderDisallowed = new Color(1f, 0f, 0f, 1f);

		private static int kRectSelectionHashCode = "RectSelection".GetHashCode();

		private static int kEffectDraggingHashCode = "EffectDragging".GetHashCode();

		private static int kVerticalFaderHash = "VerticalFader".GetHashCode();

		public int m_FocusIndex = -1;

		public int m_IndexCounter;

		public int m_EffectInteractionControlID;

		public int m_RectSelectionControlID;

		public float m_MouseDragStartX;

		public float m_MouseDragStartY;

		public float m_MouseDragStartValue;

		public Vector2 m_RectSelectionStartPos = new Vector2(0f, 0f);

		public Rect m_RectSelectionRect = new Rect(0f, 0f, 0f, 0f);

		private AudioMixerChannelStripView.State m_State;

		private AudioMixerController m_Controller;

		private MixerGroupControllerCompareByName m_GroupComparer = new MixerGroupControllerCompareByName();

		private bool m_WaitingForDragEvent;

		private int m_ChangingWetMixIndex = -1;

		private int m_MovingEffectSrcIndex = -1;

		private int m_MovingEffectDstIndex = -1;

		private Rect m_MovingSrcRect = new Rect(-1f, -1f, 0f, 0f);

		private Rect m_MovingDstRect = new Rect(-1f, -1f, 0f, 0f);

		private bool m_MovingEffectAllowed;

		private AudioMixerGroupController m_MovingSrcGroup;

		private AudioMixerGroupController m_MovingDstGroup;

		private List<int> m_LastNumChannels = new List<int>();

		private bool m_RequiresRepaint;

		private readonly Vector2 channelStripsOffset = new Vector2(15f, 10f);

		private static Texture2D m_GridTexture;

		private static readonly Color kGridColorDark = new Color(0f, 0f, 0f, 0.18f);

		private static readonly Color kGridColorLight = new Color(0f, 0f, 0f, 0.1f);

		private static Color hfaderCol1 = new Color(0.2f, 0.2f, 0.2f, 1f);

		private static Color hfaderCol2 = new Color(0.4f, 0.4f, 0.4f, 1f);

		public GUIStyle sharedGuiStyle = new GUIStyle();

		private GUIContent bypassButtonContent = new GUIContent(string.Empty, "Toggle bypass on this effect");

		private GUIContent headerGUIContent = new GUIContent();

		private GUIContent addText = new GUIContent("Add..");

		[NonSerialized]
		private int FrameCounter;

		[NonSerialized]
		private GUIStyle developerInfoStyle = AudioMixerDrawUtils.BuildGUIStyleForLabel(new Color(1f, 0f, 0f, 0.5f), 20, false, FontStyle.Bold, TextAnchor.MiddleLeft);

		[NonSerialized]
		private Vector3[] cablepoints = new Vector3[20];

		public bool requiresRepaint
		{
			get
			{
				if (this.m_RequiresRepaint)
				{
					this.m_RequiresRepaint = false;
					return true;
				}
				return false;
			}
		}

		private static Color gridColor
		{
			get
			{
				if (EditorGUIUtility.isProSkin)
				{
					return AudioMixerChannelStripView.kGridColorDark;
				}
				return AudioMixerChannelStripView.kGridColorLight;
			}
		}

		private AudioMixerDrawUtils.Styles styles
		{
			get
			{
				return AudioMixerDrawUtils.styles;
			}
		}

		private Texture2D gridTextureTilable
		{
			get
			{
				if (AudioMixerChannelStripView.m_GridTexture == null)
				{
					AudioMixerChannelStripView.m_GridTexture = AudioMixerChannelStripView.CreateTilableGridTexture(12, 12, new Color(0f, 0f, 0f, 0f), AudioMixerChannelStripView.gridColor);
				}
				return AudioMixerChannelStripView.m_GridTexture;
			}
		}

		public AudioMixerChannelStripView(AudioMixerChannelStripView.State state)
		{
			this.m_State = state;
		}

		private static Texture2D CreateTilableGridTexture(int width, int height, Color backgroundColor, Color lineColor)
		{
			Color[] array = new Color[width * height];
			for (int i = 0; i < height * width; i++)
			{
				array[i] = backgroundColor;
			}
			for (int j = 0; j < height; j++)
			{
				array[j * width + (width - 1)] = lineColor;
			}
			for (int k = 0; k < width; k++)
			{
				array[(height - 1) * width + k] = lineColor;
			}
			Texture2D texture2D = new Texture2D(width, height, TextureFormat.ARGB32, false);
			texture2D.hideFlags = HideFlags.HideAndDontSave;
			texture2D.SetPixels(array);
			texture2D.Apply();
			return texture2D;
		}

		private void DrawAreaBackground(Rect rect)
		{
			if (Event.current.type == EventType.Repaint)
			{
				Color color = GUI.color;
				GUI.color = new Color(1f, 1f, 1f, (!EditorGUIUtility.isProSkin) ? 0.2f : 0.6f);
				AudioMixerDrawUtils.styles.channelStripAreaBackground.Draw(rect, false, false, false, false);
				GUI.color = color;
			}
		}

		private void SetFocus()
		{
			this.m_FocusIndex = this.m_IndexCounter;
		}

		private void ClearFocus()
		{
			this.m_FocusIndex = -1;
		}

		private bool HasFocus()
		{
			return this.m_FocusIndex == this.m_IndexCounter;
		}

		private bool IsFocusActive()
		{
			return this.m_FocusIndex != -1;
		}

		public static void InsertEffectPopupCallback(object obj)
		{
			AudioMixerChannelStripView.EffectContext effectContext = (AudioMixerChannelStripView.EffectContext)obj;
			AudioMixerGroupController[] groups = effectContext.groups;
			for (int i = 0; i < groups.Length; i++)
			{
				AudioMixerGroupController audioMixerGroupController = groups[i];
				Undo.RecordObject(audioMixerGroupController, "Add effect");
				AudioMixerEffectController audioMixerEffectController = new AudioMixerEffectController(effectContext.name);
				int index = (effectContext.index != -1 && effectContext.index <= audioMixerGroupController.effects.Length) ? effectContext.index : audioMixerGroupController.effects.Length;
				audioMixerGroupController.InsertEffect(audioMixerEffectController, index);
				AssetDatabase.AddObjectToAsset(audioMixerEffectController, effectContext.controller);
				audioMixerEffectController.PreallocateGUIDs();
			}
			AudioMixerUtility.RepaintAudioMixerAndInspectors();
		}

		public void RemoveEffectPopupCallback(object obj)
		{
			AudioMixerChannelStripView.EffectContext effectContext = (AudioMixerChannelStripView.EffectContext)obj;
			AudioMixerGroupController[] groups = effectContext.groups;
			for (int i = 0; i < groups.Length; i++)
			{
				AudioMixerGroupController audioMixerGroupController = groups[i];
				if (effectContext.index < audioMixerGroupController.effects.Length)
				{
					AudioMixerEffectController audioMixerEffectController = audioMixerGroupController.effects[effectContext.index];
					effectContext.controller.ClearSendConnectionsTo(audioMixerEffectController);
					effectContext.controller.RemoveEffect(audioMixerEffectController, audioMixerGroupController);
				}
			}
			AudioMixerUtility.RepaintAudioMixerAndInspectors();
		}

		public static void ConnectSendPopupCallback(object obj)
		{
			AudioMixerChannelStripView.ConnectSendContext connectSendContext = (AudioMixerChannelStripView.ConnectSendContext)obj;
			Undo.RecordObject(connectSendContext.sendEffect, "Change Send Target");
			connectSendContext.sendEffect.sendTarget = connectSendContext.targetEffect;
		}

		private bool ClipRect(Rect r, Rect clipRect, ref Rect overlap)
		{
			overlap.x = Mathf.Max(r.x, clipRect.x);
			overlap.y = Mathf.Max(r.y, clipRect.y);
			overlap.width = Mathf.Min(r.x + r.width, clipRect.x + clipRect.width) - overlap.x;
			overlap.height = Mathf.Min(r.y + r.height, clipRect.y + clipRect.height) - overlap.y;
			return overlap.width > 0f && overlap.height > 0f;
		}

		public float VerticalFader(Rect r, float value, int direction, float dragScale, bool drawScaleValues, bool drawMarkerValue, string tooltip, float maxValue, GUIStyle style)
		{
			Event current = Event.current;
			int num = (int)style.fixedHeight;
			int num2 = (int)r.height - num;
			float num3 = AudioMixerController.VolumeToScreenMapping(Mathf.Clamp(value, AudioMixerController.kMinVolume, maxValue), (float)num2, true);
			Rect rect = new Rect(r.x, r.y + (float)((int)num3), r.width, (float)num);
			int controlID = GUIUtility.GetControlID(AudioMixerChannelStripView.kVerticalFaderHash, FocusType.Passive);
			switch (current.GetTypeForControl(controlID))
			{
			case EventType.MouseDown:
				if (r.Contains(current.mousePosition) && GUIUtility.hotControl == 0)
				{
					this.m_MouseDragStartY = current.mousePosition.y;
					this.m_MouseDragStartValue = num3;
					GUIUtility.hotControl = controlID;
					current.Use();
				}
				break;
			case EventType.MouseUp:
				if (GUIUtility.hotControl == controlID)
				{
					GUIUtility.hotControl = 0;
					current.Use();
				}
				break;
			case EventType.MouseDrag:
				if (GUIUtility.hotControl == controlID)
				{
					num3 = Mathf.Clamp(this.m_MouseDragStartValue + dragScale * (current.mousePosition.y - this.m_MouseDragStartY), 0f, (float)num2);
					value = Mathf.Clamp(AudioMixerController.VolumeToScreenMapping(num3, (float)num2, false), AudioMixerController.kMinVolume, maxValue);
					current.Use();
				}
				break;
			case EventType.Repaint:
				if (drawScaleValues)
				{
					float num4 = r.y + (float)num / 2f;
					float num5 = maxValue;
					using (new EditorGUI.DisabledScope(true))
					{
						while (num5 >= AudioMixerController.kMinVolume)
						{
							float num6 = AudioMixerController.VolumeToScreenMapping(num5, (float)num2, true);
							if (num5 / 10f % 2f == 0f)
							{
								GUI.Label(new Rect(r.x, num4 + num6 - 7f, r.width, 14f), GUIContent.Temp(Mathf.RoundToInt(num5).ToString()), this.styles.vuValue);
							}
							EditorGUI.DrawRect(new Rect(r.x, num4 + num6 - 1f, 5f, 1f), new Color(0f, 0f, 0f, 0.5f));
							num5 -= 10f;
						}
					}
				}
				if (drawMarkerValue)
				{
					style.Draw(rect, GUIContent.Temp(Mathf.RoundToInt(value).ToString()), false, false, false, false);
				}
				else
				{
					style.Draw(rect, false, false, false, false);
				}
				AudioMixerDrawUtils.AddTooltipOverlay(rect, tooltip);
				break;
			}
			return value;
		}

		public float HorizontalFader(Rect r, float value, float minValue, float maxValue, int direction, float dragScale)
		{
			this.m_IndexCounter++;
			Rect r2 = new Rect(r);
			float num = r.width * 0.2f;
			float num2 = r2.width - num;
			AudioMixerDrawUtils.DrawGradientRect(r2, AudioMixerChannelStripView.hfaderCol1, AudioMixerChannelStripView.hfaderCol2);
			Event current = Event.current;
			if (current.type == EventType.MouseDown && r2.Contains(current.mousePosition))
			{
				this.m_MouseDragStartX = current.mousePosition.x;
				this.m_MouseDragStartValue = value;
				this.SetFocus();
			}
			if (this.HasFocus())
			{
				if (current.type == EventType.MouseDrag)
				{
					value = this.m_MouseDragStartValue + dragScale * (maxValue - minValue) * (current.mousePosition.x - this.m_MouseDragStartX) / num2;
					Event.current.Use();
				}
				else if (current.type == EventType.MouseUp)
				{
					this.ClearFocus();
					Event.current.Use();
				}
			}
			value = Mathf.Clamp(value, minValue, maxValue);
			r2.x = r.x;
			r2.width = r.width;
			r2.x = r.x + num2 * ((value - minValue) / (maxValue - minValue));
			r2.width = num;
			AudioMixerDrawUtils.DrawGradientRect(r2, AudioMixerChannelStripView.hfaderCol2, AudioMixerChannelStripView.hfaderCol1);
			return value;
		}

		public GUIStyle GetEffectBarStyle(AudioMixerEffectController effect)
		{
			if (effect.IsSend() || effect.IsReceive() || effect.IsDuckVolume())
			{
				return this.styles.sendReturnBar;
			}
			if (effect.IsAttenuation())
			{
				return this.styles.attenuationBar;
			}
			return this.styles.effectBar;
		}

		private void EffectSlot(Rect effectRect, AudioMixerSnapshotController snapshot, AudioMixerEffectController effect, int effectIndex, ref int highlightEffectIndex, AudioMixerChannelStripView.ChannelStripParams p, ref Dictionary<AudioMixerEffectController, AudioMixerChannelStripView.PatchSlot> patchslots)
		{
			if (effect == null)
			{
				return;
			}
			Rect rect = effectRect;
			Event current = Event.current;
			if (current.type == EventType.Repaint && patchslots != null && (effect.IsSend() || MixerEffectDefinitions.EffectCanBeSidechainTarget(effect)))
			{
				AudioMixerChannelStripView.PatchSlot patchSlot = new AudioMixerChannelStripView.PatchSlot();
				patchSlot.group = p.group;
				patchSlot.x = rect.xMax - (rect.yMax - rect.yMin) * 0.5f;
				patchSlot.y = (rect.yMin + rect.yMax) * 0.5f;
				patchslots[effect] = patchSlot;
			}
			bool flag = !effect.DisallowsBypass();
			Rect position = rect;
			position.width = 10f;
			rect.xMin += 10f;
			if (flag && GUI.Button(position, this.bypassButtonContent, GUIStyle.none))
			{
				effect.bypass = !effect.bypass;
				this.m_Controller.UpdateBypass();
				InspectorWindow.RepaintAllInspectors();
			}
			this.m_IndexCounter++;
			float num = (!(effect != null)) ? AudioMixerController.kMinVolume : Mathf.Clamp(effect.GetValueForMixLevel(this.m_Controller, snapshot), AudioMixerController.kMinVolume, AudioMixerController.kMaxEffect);
			bool flag2 = effect != null && ((effect.IsSend() && effect.sendTarget != null) || effect.enableWetMix);
			if (current.type == EventType.Repaint)
			{
				GUIStyle effectBarStyle = this.GetEffectBarStyle(effect);
				float num2 = (!flag2) ? 1f : ((num - AudioMixerController.kMinVolume) / (AudioMixerController.kMaxEffect - AudioMixerController.kMinVolume));
				bool flag3 = (!p.group.bypassEffects && (effect == null || !effect.bypass)) || (effect != null && effect.DisallowsBypass());
				Color color = (!(effect != null)) ? new Color(0f, 0f, 0f, 0.5f) : AudioMixerDrawUtils.GetEffectColor(effect);
				if (!flag3)
				{
					color = new Color(color.r * 0.5f, color.g * 0.5f, color.b * 0.5f);
				}
				if (flag3)
				{
					if (num2 < 1f)
					{
						float num3 = rect.width * num2;
						if (num3 < 4f)
						{
							num3 = Mathf.Max(num3, 2f);
							float num4 = 1f - num3 / 4f;
							Color color2 = GUI.color;
							if (!GUI.enabled)
							{
								GUI.color = new Color(1f, 1f, 1f, 0.5f);
							}
							GUI.DrawTextureWithTexCoords(new Rect(rect.x, rect.y, num3, rect.height), effectBarStyle.focused.background, new Rect(num4, 0f, 1f - num4, 1f));
							GUI.color = color2;
						}
						else
						{
							effectBarStyle.Draw(new Rect(rect.x, rect.y, num3, rect.height), false, false, false, true);
						}
						GUI.DrawTexture(new Rect(rect.x + num3, rect.y, rect.width - num3, rect.height), effectBarStyle.onFocused.background, ScaleMode.StretchToFill);
					}
					else
					{
						effectBarStyle.Draw(rect, !flag2, false, false, flag2);
					}
				}
				else
				{
					effectBarStyle.Draw(rect, false, false, false, false);
				}
				if (flag)
				{
					this.styles.circularToggle.Draw(new Rect(position.x + 2f, position.y + 5f, position.width - 2f, position.width - 2f), false, false, !effect.bypass, false);
				}
				if (effect.IsSend() && effect.sendTarget != null)
				{
					position.y -= 1f;
					GUI.Label(position, this.styles.sendString, EditorStyles.miniLabel);
				}
				using (new EditorGUI.DisabledScope(!flag3))
				{
					string effectSlotName = this.GetEffectSlotName(effect, flag2, snapshot, p);
					string effectSlotTooltip = this.GetEffectSlotTooltip(effect, rect, p);
					GUI.Label(new Rect(rect.x, rect.y, rect.width - 10f, rect.height), GUIContent.Temp(effectSlotName, effectSlotTooltip), this.styles.effectName);
				}
			}
			else
			{
				this.EffectSlotDragging(effectRect, snapshot, effect, flag2, num, effectIndex, ref highlightEffectIndex, p);
			}
		}

		private string GetEffectSlotName(AudioMixerEffectController effect, bool showLevel, AudioMixerSnapshotController snapshot, AudioMixerChannelStripView.ChannelStripParams p)
		{
			if (this.m_ChangingWetMixIndex == this.m_IndexCounter && showLevel)
			{
				return string.Format("{0:F1} dB", effect.GetValueForMixLevel(this.m_Controller, snapshot));
			}
			if (effect.IsSend() && effect.sendTarget != null)
			{
				return effect.GetSendTargetDisplayString(p.effectMap);
			}
			return effect.effectName;
		}

		private string GetEffectSlotTooltip(AudioMixerEffectController effect, Rect effectRect, AudioMixerChannelStripView.ChannelStripParams p)
		{
			if (!effectRect.Contains(Event.current.mousePosition))
			{
				return string.Empty;
			}
			if (effect.IsSend())
			{
				if (effect.sendTarget != null)
				{
					string sendTargetDisplayString = effect.GetSendTargetDisplayString(p.effectMap);
					return "Send to: " + sendTargetDisplayString;
				}
				return this.styles.emptySendSlotGUIContent.tooltip;
			}
			else
			{
				if (effect.IsReceive())
				{
					return this.styles.returnSlotGUIContent.tooltip;
				}
				if (effect.IsDuckVolume())
				{
					return this.styles.duckVolumeSlotGUIContent.tooltip;
				}
				if (effect.IsAttenuation())
				{
					return this.styles.attenuationSlotGUIContent.tooltip;
				}
				return this.styles.effectSlotGUIContent.tooltip;
			}
		}

		private void EffectSlotDragging(Rect r, AudioMixerSnapshotController snapshot, AudioMixerEffectController effect, bool showLevel, float level, int effectIndex, ref int highlightEffectIndex, AudioMixerChannelStripView.ChannelStripParams p)
		{
			Event current = Event.current;
			switch (current.GetTypeForControl(this.m_EffectInteractionControlID))
			{
			case EventType.MouseDown:
				if (r.Contains(current.mousePosition) && current.button == 0 && GUIUtility.hotControl == 0)
				{
					GUIUtility.hotControl = this.m_EffectInteractionControlID;
					this.m_MouseDragStartX = current.mousePosition.x;
					this.m_MouseDragStartValue = level;
					highlightEffectIndex = effectIndex;
					this.m_MovingEffectSrcIndex = -1;
					this.m_MovingEffectDstIndex = -1;
					this.m_WaitingForDragEvent = true;
					this.m_MovingSrcRect = r;
					this.m_MovingDstRect = r;
					this.m_MovingSrcGroup = p.group;
					this.m_MovingDstGroup = p.group;
					this.m_MovingEffectAllowed = true;
					this.SetFocus();
					Event.current.Use();
					EditorGUIUtility.SetWantsMouseJumping(1);
					InspectorWindow.RepaintAllInspectors();
				}
				break;
			case EventType.MouseUp:
				if (GUIUtility.hotControl == this.m_EffectInteractionControlID && current.button == 0 && p.stripRect.Contains(current.mousePosition))
				{
					if (this.m_MovingEffectDstIndex != -1 && this.m_MovingEffectAllowed)
					{
						if (this.IsDuplicateKeyPressed() && this.CanDuplicateDraggedEffect())
						{
							AudioMixerEffectController sourceEffect = this.m_MovingSrcGroup.effects[this.m_MovingEffectSrcIndex];
							AudioMixerEffectController effect2 = this.m_MovingSrcGroup.controller.CopyEffect(sourceEffect);
							List<AudioMixerEffectController> list = this.m_MovingDstGroup.effects.ToList<AudioMixerEffectController>();
							if (AudioMixerController.InsertEffect(effect2, ref list, this.m_MovingEffectDstIndex))
							{
								this.m_MovingDstGroup.effects = list.ToArray();
							}
						}
						else if (this.m_MovingSrcGroup == this.m_MovingDstGroup)
						{
							List<AudioMixerEffectController> list2 = this.m_MovingSrcGroup.effects.ToList<AudioMixerEffectController>();
							if (AudioMixerController.MoveEffect(ref list2, this.m_MovingEffectSrcIndex, ref list2, this.m_MovingEffectDstIndex))
							{
								this.m_MovingSrcGroup.effects = list2.ToArray();
							}
						}
						else if (!this.m_MovingSrcGroup.effects[this.m_MovingEffectSrcIndex].IsAttenuation())
						{
							List<AudioMixerEffectController> list3 = this.m_MovingSrcGroup.effects.ToList<AudioMixerEffectController>();
							List<AudioMixerEffectController> list4 = this.m_MovingDstGroup.effects.ToList<AudioMixerEffectController>();
							if (AudioMixerController.MoveEffect(ref list3, this.m_MovingEffectSrcIndex, ref list4, this.m_MovingEffectDstIndex))
							{
								this.m_MovingSrcGroup.effects = list3.ToArray();
								this.m_MovingDstGroup.effects = list4.ToArray();
							}
						}
					}
					this.ClearEffectDragging(ref highlightEffectIndex);
					current.Use();
					EditorGUIUtility.SetWantsMouseJumping(0);
					GUIUtility.ExitGUI();
				}
				break;
			case EventType.MouseDrag:
				if (GUIUtility.hotControl == this.m_EffectInteractionControlID)
				{
					if (this.HasFocus() && this.m_WaitingForDragEvent)
					{
						this.m_ChangingWetMixIndex = -1;
						if (effectIndex < p.group.effects.Length)
						{
							if (Mathf.Abs(current.delta.y) > Mathf.Abs(current.delta.x))
							{
								this.m_MovingEffectSrcIndex = effectIndex;
								this.ClearFocus();
							}
							else
							{
								this.m_ChangingWetMixIndex = this.m_IndexCounter;
							}
						}
						this.m_WaitingForDragEvent = false;
					}
					if (this.IsMovingEffect() && p.stripRect.Contains(current.mousePosition))
					{
						float num = r.height * 0.5f;
						float num2 = (effectIndex != 0) ? 0f : (-num);
						float num3 = (effectIndex != p.group.effects.Length - 1) ? r.height : (r.height + num);
						float num4 = current.mousePosition.y - r.y;
						if (num4 >= num2 && num4 <= num3 && effectIndex < p.group.effects.Length)
						{
							int num5 = (num4 >= num) ? (effectIndex + 1) : effectIndex;
							if (num5 != this.m_MovingEffectDstIndex || this.m_MovingDstGroup != p.group)
							{
								this.m_MovingDstRect.x = r.x;
								this.m_MovingDstRect.width = r.width;
								this.m_MovingDstRect.y = ((num4 >= num) ? (r.y + r.height) : r.y) - 1f;
								this.m_MovingEffectDstIndex = num5;
								this.m_MovingDstGroup = p.group;
								this.m_MovingEffectAllowed = ((!this.m_MovingSrcGroup.effects[this.m_MovingEffectSrcIndex].IsAttenuation() || !(this.m_MovingSrcGroup != this.m_MovingDstGroup)) && !AudioMixerController.WillMovingEffectCauseFeedback(p.allGroups, this.m_MovingSrcGroup, this.m_MovingEffectSrcIndex, this.m_MovingDstGroup, num5, null) && (!this.IsDuplicateKeyPressed() || this.CanDuplicateDraggedEffect()));
							}
							current.Use();
						}
					}
					if (this.IsAdjustingWetMix() && this.HasFocus() && showLevel)
					{
						this.m_WaitingForDragEvent = false;
						float value = AudioMixerChannelStripView.kEffectScaleMouseDrag * HandleUtility.niceMouseDelta + level;
						float num6 = Mathf.Clamp(value, AudioMixerController.kMinVolume, AudioMixerController.kMaxEffect) - level;
						if (num6 != 0f)
						{
							Undo.RecordObject(this.m_Controller.TargetSnapshot, "Change effect level");
							if (effect.IsSend() && this.m_Controller.CachedSelection.Count > 1 && this.m_Controller.CachedSelection.Contains(p.group))
							{
								List<AudioMixerEffectController> list5 = new List<AudioMixerEffectController>();
								foreach (AudioMixerGroupController current2 in this.m_Controller.CachedSelection)
								{
									AudioMixerEffectController[] effects = current2.effects;
									for (int i = 0; i < effects.Length; i++)
									{
										AudioMixerEffectController audioMixerEffectController = effects[i];
										if (audioMixerEffectController.effectName == effect.effectName && audioMixerEffectController.sendTarget == effect.sendTarget)
										{
											list5.Add(audioMixerEffectController);
										}
									}
								}
								foreach (AudioMixerEffectController current3 in list5)
								{
									if (!current3.IsSend() || current3.sendTarget != null)
									{
										current3.SetValueForMixLevel(this.m_Controller, snapshot, Mathf.Clamp(current3.GetValueForMixLevel(this.m_Controller, snapshot) + num6, AudioMixerController.kMinVolume, AudioMixerController.kMaxEffect));
									}
								}
							}
							else if (!effect.IsSend() || effect.sendTarget != null)
							{
								effect.SetValueForMixLevel(this.m_Controller, snapshot, Mathf.Clamp(level + num6, AudioMixerController.kMinVolume, AudioMixerController.kMaxEffect));
							}
							InspectorWindow.RepaintAllInspectors();
						}
						current.Use();
					}
				}
				break;
			}
		}

		private void ClearEffectDragging(ref int highlightEffectIndex)
		{
			if (GUIUtility.hotControl == this.m_EffectInteractionControlID)
			{
				GUIUtility.hotControl = 0;
			}
			this.m_MovingEffectSrcIndex = -1;
			this.m_MovingEffectDstIndex = -1;
			this.m_MovingSrcRect = new Rect(-1f, -1f, 0f, 0f);
			this.m_MovingDstRect = new Rect(-1f, -1f, 0f, 0f);
			this.m_MovingSrcGroup = null;
			this.m_MovingDstGroup = null;
			this.m_ChangingWetMixIndex = -1;
			highlightEffectIndex = -1;
			this.ClearFocus();
			InspectorWindow.RepaintAllInspectors();
		}

		private void UnhandledEffectDraggingEvents(ref int highlightEffectIndex)
		{
			Event current = Event.current;
			EventType typeForControl = current.GetTypeForControl(this.m_EffectInteractionControlID);
			switch (typeForControl)
			{
			case EventType.MouseUp:
				if (GUIUtility.hotControl == this.m_EffectInteractionControlID && current.button == 0)
				{
					this.ClearEffectDragging(ref highlightEffectIndex);
					current.Use();
				}
				return;
			case EventType.MouseMove:
				IL_29:
				if (typeForControl != EventType.Repaint)
				{
					return;
				}
				if (this.IsMovingEffect() && current.type == EventType.Repaint)
				{
					EditorGUI.DrawRect(this.m_MovingSrcRect, AudioMixerChannelStripView.kMoveColorHighlight);
					MouseCursor mouse = (!this.IsDuplicateKeyPressed() || !this.m_MovingEffectAllowed) ? MouseCursor.ResizeVertical : MouseCursor.ArrowPlus;
					EditorGUIUtility.AddCursorRect(new Rect(current.mousePosition.x - 10f, current.mousePosition.y - 10f, 20f, 20f), mouse, this.m_EffectInteractionControlID);
				}
				if (this.m_MovingEffectDstIndex != -1 && this.m_MovingDstRect.y >= 0f)
				{
					float num = 2f;
					Color color = (!this.m_MovingEffectAllowed) ? AudioMixerChannelStripView.kMoveSlotColLoDisallowed : AudioMixerChannelStripView.kMoveSlotColLoAllowed;
					Color color2 = (!this.m_MovingEffectAllowed) ? AudioMixerChannelStripView.kMoveSlotColHiDisallowed : AudioMixerChannelStripView.kMoveSlotColHiAllowed;
					Color color3 = (!this.m_MovingEffectAllowed) ? AudioMixerChannelStripView.kMoveSlotColBorderDisallowed : AudioMixerChannelStripView.kMoveSlotColBorderAllowed;
					AudioMixerDrawUtils.DrawGradientRect(new Rect(this.m_MovingDstRect.x, this.m_MovingDstRect.y - num, this.m_MovingDstRect.width, num), color, color2);
					AudioMixerDrawUtils.DrawGradientRect(new Rect(this.m_MovingDstRect.x, this.m_MovingDstRect.y, this.m_MovingDstRect.width, num), color2, color);
					AudioMixerDrawUtils.DrawGradientRect(new Rect(this.m_MovingDstRect.x, this.m_MovingDstRect.y - 1f, this.m_MovingDstRect.width, 1f), color3, color3);
				}
				return;
			case EventType.MouseDrag:
				if (GUIUtility.hotControl == this.m_EffectInteractionControlID)
				{
					this.m_MovingEffectDstIndex = -1;
					this.m_MovingDstRect = new Rect(-1f, -1f, 0f, 0f);
					this.m_MovingDstGroup = null;
					current.Use();
				}
				return;
			}
			goto IL_29;
		}

		private bool IsDuplicateKeyPressed()
		{
			return Event.current.alt;
		}

		private bool CanDuplicateDraggedEffect()
		{
			return this.IsMovingEffect() && this.m_MovingSrcGroup != null && !this.m_MovingSrcGroup.effects[this.m_MovingEffectSrcIndex].IsAttenuation();
		}

		private bool DoSoloButton(Rect r, AudioMixerGroupController group, List<AudioMixerGroupController> allGroups, List<AudioMixerGroupController> selection)
		{
			Event current = Event.current;
			if (current.type == EventType.MouseUp && current.button == 1 && r.Contains(current.mousePosition))
			{
				if (allGroups.Any((AudioMixerGroupController g) => g.solo))
				{
					Undo.RecordObject(group, "Change solo state");
					foreach (AudioMixerGroupController current2 in allGroups)
					{
						current2.solo = false;
					}
					current.Use();
					return true;
				}
			}
			bool flag = GUI.Toggle(r, group.solo, this.styles.soloGUIContent, AudioMixerDrawUtils.styles.soloToggle);
			if (flag != group.solo)
			{
				Undo.RecordObject(group, "Change solo state");
				group.solo = !group.solo;
				foreach (AudioMixerGroupController current3 in selection)
				{
					current3.solo = group.solo;
				}
				return true;
			}
			return false;
		}

		private bool DoMuteButton(Rect r, AudioMixerGroupController group, List<AudioMixerGroupController> allGroups, bool anySoloActive, List<AudioMixerGroupController> selection)
		{
			Event current = Event.current;
			if (current.type == EventType.MouseUp && current.button == 1 && r.Contains(current.mousePosition))
			{
				if (allGroups.Any((AudioMixerGroupController g) => g.mute))
				{
					Undo.RecordObject(group, "Change mute state");
					if (allGroups.Any((AudioMixerGroupController g) => g.solo))
					{
						return false;
					}
					foreach (AudioMixerGroupController current2 in allGroups)
					{
						current2.mute = false;
					}
					current.Use();
					return true;
				}
			}
			Color color = GUI.color;
			bool flag = anySoloActive && group.mute;
			if (flag)
			{
				GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, 0.5f);
			}
			bool flag2 = GUI.Toggle(r, group.mute, this.styles.muteGUIContent, AudioMixerDrawUtils.styles.muteToggle);
			if (flag)
			{
				GUI.color = color;
			}
			if (flag2 != group.mute)
			{
				Undo.RecordObject(group, "Change mute state");
				group.mute = !group.mute;
				foreach (AudioMixerGroupController current3 in selection)
				{
					current3.mute = group.mute;
				}
				return true;
			}
			return false;
		}

		private bool DoBypassEffectsButton(Rect r, AudioMixerGroupController group, List<AudioMixerGroupController> allGroups, List<AudioMixerGroupController> selection)
		{
			Event current = Event.current;
			if (current.type == EventType.MouseUp && current.button == 1 && r.Contains(current.mousePosition))
			{
				if (allGroups.Any((AudioMixerGroupController g) => g.bypassEffects))
				{
					Undo.RecordObject(group, "Change bypass effects state");
					foreach (AudioMixerGroupController current2 in allGroups)
					{
						current2.bypassEffects = false;
					}
					current.Use();
					return true;
				}
			}
			bool flag = GUI.Toggle(r, group.bypassEffects, this.styles.bypassGUIContent, AudioMixerDrawUtils.styles.bypassToggle);
			if (flag != group.bypassEffects)
			{
				Undo.RecordObject(group, "Change bypass effects state");
				group.bypassEffects = !group.bypassEffects;
				foreach (AudioMixerGroupController current3 in selection)
				{
					current3.bypassEffects = group.bypassEffects;
				}
				return true;
			}
			return false;
		}

		private static bool RectOverlaps(Rect r1, Rect r2)
		{
			Rect rect = default(Rect);
			rect.x = Mathf.Max(r1.x, r2.x);
			rect.y = Mathf.Max(r1.y, r2.y);
			rect.width = Mathf.Min(r1.x + r1.width, r2.x + r2.width) - rect.x;
			rect.height = Mathf.Min(r1.y + r1.height, r2.y + r2.height) - rect.y;
			return rect.width > 0f && rect.height > 0f;
		}

		private bool IsRectSelectionActive()
		{
			return GUIUtility.hotControl == this.m_RectSelectionControlID;
		}

		private void GroupClicked(AudioMixerGroupController clickedGroup, AudioMixerChannelStripView.ChannelStripParams p, bool clickedControlInGroup)
		{
			List<int> list = new List<int>();
			foreach (AudioMixerGroupController current in p.shownGroups)
			{
				list.Add(current.GetInstanceID());
			}
			List<int> list2 = new List<int>();
			foreach (AudioMixerGroupController current2 in this.m_Controller.CachedSelection)
			{
				list2.Add(current2.GetInstanceID());
			}
			int lastClickedInstanceID = this.m_State.m_LastClickedInstanceID;
			bool allowMultiSelection = true;
			bool keepMultiSelection = Event.current.shift || clickedControlInGroup;
			bool useShiftAsActionKey = false;
			List<int> newSelection = InternalEditorUtility.GetNewSelection(clickedGroup.GetInstanceID(), list, list2, lastClickedInstanceID, keepMultiSelection, useShiftAsActionKey, allowMultiSelection);
			List<AudioMixerGroupController> list3 = (from x in p.allGroups
			where newSelection.Contains(x.GetInstanceID())
			select x).ToList<AudioMixerGroupController>();
			Selection.objects = list3.ToArray();
			this.m_Controller.OnUnitySelectionChanged();
			InspectorWindow.RepaintAllInspectors();
		}

		private void DoAttenuationFader(Rect r, AudioMixerGroupController group, List<AudioMixerGroupController> selection, GUIStyle style)
		{
			float num = Mathf.Clamp(group.GetValueForVolume(this.m_Controller, this.m_Controller.TargetSnapshot), AudioMixerController.kMinVolume, AudioMixerController.GetMaxVolume());
			float num2 = this.VerticalFader(r, num, 1, AudioMixerChannelStripView.kVolumeScaleMouseDrag, true, true, this.styles.attenuationFader.tooltip, AudioMixerController.GetMaxVolume(), style);
			if (num != num2)
			{
				float num3 = num2 - num;
				Undo.RecordObject(this.m_Controller.TargetSnapshot, "Change volume fader");
				foreach (AudioMixerGroupController current in selection)
				{
					float num4 = Mathf.Clamp(current.GetValueForVolume(this.m_Controller, this.m_Controller.TargetSnapshot), AudioMixerController.kMinVolume, AudioMixerController.GetMaxVolume());
					current.SetValueForVolume(this.m_Controller, this.m_Controller.TargetSnapshot, Mathf.Clamp(num4 + num3, AudioMixerController.kMinVolume, AudioMixerController.GetMaxVolume()));
				}
				InspectorWindow.RepaintAllInspectors();
			}
		}

		internal static void AddEffectItemsToMenu(AudioMixerController controller, AudioMixerGroupController[] groups, int insertIndex, string prefix, GenericMenu pm)
		{
			string[] effectList = MixerEffectDefinitions.GetEffectList();
			for (int i = 0; i < effectList.Length; i++)
			{
				if (effectList[i] != "Attenuation")
				{
					pm.AddItem(new GUIContent(prefix + AudioMixerController.FixNameForPopupMenu(effectList[i])), false, new GenericMenu.MenuFunction2(AudioMixerChannelStripView.InsertEffectPopupCallback), new AudioMixerChannelStripView.EffectContext(controller, groups, insertIndex, effectList[i]));
				}
			}
		}

		private void DoEffectSlotInsertEffectPopup(Rect buttonRect, AudioMixerGroupController group, List<AudioMixerGroupController> allGroups, int effectSlotIndex, ref Dictionary<AudioMixerEffectController, AudioMixerGroupController> effectMap)
		{
			GenericMenu genericMenu = new GenericMenu();
			AudioMixerGroupController[] groups = new AudioMixerGroupController[]
			{
				group
			};
			if (effectSlotIndex < group.effects.Length)
			{
				AudioMixerEffectController effect = group.effects[effectSlotIndex];
				if (!effect.IsAttenuation() && !effect.IsSend() && !effect.IsReceive() && !effect.IsDuckVolume())
				{
					genericMenu.AddItem(new GUIContent("Allow Wet Mixing (causes higher memory usage)"), effect.enableWetMix, delegate
					{
						effect.enableWetMix = !effect.enableWetMix;
					});
					genericMenu.AddItem(new GUIContent("Bypass"), effect.bypass, delegate
					{
						effect.bypass = !effect.bypass;
						this.m_Controller.UpdateBypass();
						InspectorWindow.RepaintAllInspectors();
					});
					genericMenu.AddSeparator(string.Empty);
				}
				AudioMixerChannelStripView.AddEffectItemsToMenu(group.controller, groups, effectSlotIndex, "Add effect before/", genericMenu);
				AudioMixerChannelStripView.AddEffectItemsToMenu(group.controller, groups, effectSlotIndex + 1, "Add effect after/", genericMenu);
			}
			else
			{
				AudioMixerChannelStripView.AddEffectItemsToMenu(group.controller, groups, effectSlotIndex, string.Empty, genericMenu);
			}
			if (effectSlotIndex < group.effects.Length)
			{
				AudioMixerEffectController audioMixerEffectController = group.effects[effectSlotIndex];
				if (!audioMixerEffectController.IsAttenuation())
				{
					genericMenu.AddSeparator(string.Empty);
					genericMenu.AddItem(new GUIContent("Remove"), false, new GenericMenu.MenuFunction2(this.RemoveEffectPopupCallback), new AudioMixerChannelStripView.EffectContext(this.m_Controller, groups, effectSlotIndex, string.Empty));
					bool flag = false;
					if (audioMixerEffectController.IsSend())
					{
						if (audioMixerEffectController.sendTarget != null)
						{
							if (!flag)
							{
								flag = true;
								genericMenu.AddSeparator(string.Empty);
							}
							genericMenu.AddItem(new GUIContent("Disconnect from '" + audioMixerEffectController.GetSendTargetDisplayString(effectMap) + "'"), false, new GenericMenu.MenuFunction2(AudioMixerChannelStripView.ConnectSendPopupCallback), new AudioMixerChannelStripView.ConnectSendContext(audioMixerEffectController, null));
						}
						if (!flag)
						{
							this.AddSeperatorIfAnyReturns(genericMenu, allGroups);
						}
						AudioMixerChannelStripView.AddMenuItemsForReturns(genericMenu, "Connect to ", effectSlotIndex, group, allGroups, effectMap, audioMixerEffectController, false);
					}
				}
			}
			genericMenu.DropDown(buttonRect);
			Event.current.Use();
		}

		private void AddSeperatorIfAnyReturns(GenericMenu pm, List<AudioMixerGroupController> allGroups)
		{
			foreach (AudioMixerGroupController current in allGroups)
			{
				AudioMixerEffectController[] effects = current.effects;
				for (int i = 0; i < effects.Length; i++)
				{
					AudioMixerEffectController audioMixerEffectController = effects[i];
					if (audioMixerEffectController.IsReceive() || audioMixerEffectController.IsDuckVolume())
					{
						pm.AddSeparator(string.Empty);
						return;
					}
				}
			}
		}

		public static void AddMenuItemsForReturns(GenericMenu pm, string prefix, int effectIndex, AudioMixerGroupController group, List<AudioMixerGroupController> allGroups, Dictionary<AudioMixerEffectController, AudioMixerGroupController> effectMap, AudioMixerEffectController effect, bool showCurrent)
		{
			foreach (AudioMixerGroupController current in allGroups)
			{
				AudioMixerEffectController[] effects = current.effects;
				for (int i = 0; i < effects.Length; i++)
				{
					AudioMixerEffectController audioMixerEffectController = effects[i];
					if (MixerEffectDefinitions.EffectCanBeSidechainTarget(audioMixerEffectController))
					{
						List<AudioMixerController.ConnectionNode> list = new List<AudioMixerController.ConnectionNode>();
						if (!AudioMixerController.WillChangeOfEffectTargetCauseFeedback(allGroups, group, effectIndex, audioMixerEffectController, list))
						{
							if (showCurrent || effect.sendTarget != audioMixerEffectController)
							{
								pm.AddItem(new GUIContent(prefix + "'" + audioMixerEffectController.GetDisplayString(effectMap) + "'"), effect.sendTarget == audioMixerEffectController, new GenericMenu.MenuFunction2(AudioMixerChannelStripView.ConnectSendPopupCallback), new AudioMixerChannelStripView.ConnectSendContext(effect, audioMixerEffectController));
							}
						}
						else
						{
							string text = "A connection to '" + AudioMixerController.FixNameForPopupMenu(audioMixerEffectController.GetDisplayString(effectMap)) + "' would result in a feedback loop/";
							pm.AddDisabledItem(new GUIContent(text + "Loop: "));
							int num = 1;
							foreach (AudioMixerController.ConnectionNode current2 in list)
							{
								pm.AddDisabledItem(new GUIContent(string.Concat(new object[]
								{
									text,
									num,
									": ",
									current2.GetDisplayString(),
									"->"
								})));
								num++;
							}
							pm.AddDisabledItem(new GUIContent(text + num + ": ..."));
						}
					}
				}
			}
		}

		public void VUMeter(AudioMixerGroupController group, Rect r, float level, float peak)
		{
			EditorGUI.VUMeter.VerticalMeter(r, level, peak, EditorGUI.VUMeter.verticalVUTexture, Color.grey);
		}

		private void DrawBackgrounds(AudioMixerChannelStripView.ChannelStripParams p, bool selected)
		{
			if (Event.current.type == EventType.Repaint)
			{
				this.styles.channelStripBg.Draw(p.stripRect, false, false, selected, false);
				float num = 0.466666669f;
				float num2 = 0.227450982f;
				Color color = (!EditorGUIUtility.isProSkin) ? new Color(num, num, num) : new Color(num2, num2, num2);
				Rect rect = p.bgRects[p.kEffectStartIndex];
				rect.y -= 1f;
				rect.height = 1f;
				EditorGUI.DrawRect(rect, color);
			}
			Rect rect2 = p.bgRects[p.kVUMeterFaderIndex];
			rect2.height = (float)((!EditorGUIUtility.isProSkin) ? 2 : 1);
			rect2.y -= rect2.height;
			int userColorIndex = p.group.userColorIndex;
			if (userColorIndex != 0)
			{
				EditorGUI.DrawRect(rect2, AudioMixerColorCodes.GetColor(userColorIndex));
			}
		}

		private void OpenGroupContextMenu(AudioMixerGroupController[] groups)
		{
			GenericMenu genericMenu = new GenericMenu();
			AudioMixerChannelStripView.AddEffectItemsToMenu(groups[0].controller, groups, 0, "Add effect at top/", genericMenu);
			AudioMixerChannelStripView.AddEffectItemsToMenu(groups[0].controller, groups, -1, "Add effect at bottom/", genericMenu);
			genericMenu.AddSeparator(string.Empty);
			AudioMixerColorCodes.AddColorItemsToGenericMenu(genericMenu, groups);
			genericMenu.AddSeparator(string.Empty);
			genericMenu.ShowAsContext();
		}

		private void DrawChannelStrip(AudioMixerChannelStripView.ChannelStripParams p, ref int highlightEffectIndex, ref Dictionary<AudioMixerEffectController, AudioMixerChannelStripView.PatchSlot> patchslots, bool showBusConnectionsOfSelection)
		{
			Event current = Event.current;
			bool flag = current.type == EventType.MouseDown && p.stripRect.Contains(current.mousePosition);
			bool flag2 = this.m_Controller.CachedSelection.Contains(p.group);
			if (this.IsRectSelectionActive() && AudioMixerChannelStripView.RectOverlaps(p.stripRect, this.m_RectSelectionRect))
			{
				p.rectSelectionGroups.Add(p.group);
				flag2 = true;
			}
			this.DrawBackgrounds(p, flag2);
			GUIContent arg_9B_0 = this.headerGUIContent;
			string displayString = p.group.GetDisplayString();
			this.headerGUIContent.tooltip = displayString;
			arg_9B_0.text = displayString;
			GUI.Label(p.bgRects[p.kHeaderIndex], this.headerGUIContent, AudioMixerDrawUtils.styles.channelStripHeaderStyle);
			Rect rect = new RectOffset(-6, 0, 0, -4).Add(p.bgRects[p.kVUMeterFaderIndex]);
			float num = 1f;
			float num2 = 54f;
			float width = rect.width - num2 - num;
			Rect vuRect = new Rect(rect.x, rect.y, num2, rect.height);
			Rect rect2 = new Rect(vuRect.xMax + num, rect.y, width, rect.height);
			float width2 = 29f;
			Rect r = new Rect(rect2.x, rect2.y, width2, rect2.height);
			Rect rect3 = p.bgRects[p.kSoloMuteBypassIndex];
			GUIStyle channelStripAttenuationMarkerSquare = AudioMixerDrawUtils.styles.channelStripAttenuationMarkerSquare;
			using (new EditorGUI.DisabledScope(!AudioMixerController.EditingTargetSnapshot()))
			{
				this.DoVUMeters(vuRect, channelStripAttenuationMarkerSquare.fixedHeight, p);
				this.DoAttenuationFader(r, p.group, this.m_Controller.CachedSelection, channelStripAttenuationMarkerSquare);
				this.DoTotaldB(p);
				this.DoEffectList(p, flag2, ref highlightEffectIndex, ref patchslots, showBusConnectionsOfSelection);
			}
			this.DoSoloMuteBypassButtons(rect3, p.group, p.allGroups, this.m_Controller.CachedSelection, p.anySoloActive);
			if (flag && current.button == 0)
			{
				this.GroupClicked(p.group, p, current.type == EventType.Used);
			}
			if (current.type == EventType.ContextClick && p.stripRect.Contains(current.mousePosition))
			{
				current.Use();
				if (flag2)
				{
					this.OpenGroupContextMenu(this.m_Controller.CachedSelection.ToArray());
				}
				else
				{
					this.OpenGroupContextMenu(new AudioMixerGroupController[]
					{
						p.group
					});
				}
			}
		}

		private void DoTotaldB(AudioMixerChannelStripView.ChannelStripParams p)
		{
			float num = 50f;
			this.styles.totalVULevel.padding.right = (int)((p.stripRect.width - num) * 0.5f);
			float num2 = Mathf.Max(p.vuinfo_level[8], -80f);
			Rect position = p.bgRects[p.kTotalVULevelIndex];
			GUI.Label(position, string.Format("{0:F1} dB", num2), this.styles.totalVULevel);
		}

		private void DoEffectList(AudioMixerChannelStripView.ChannelStripParams p, bool selected, ref int highlightEffectIndex, ref Dictionary<AudioMixerEffectController, AudioMixerChannelStripView.PatchSlot> patchslots, bool showBusConnectionsOfSelection)
		{
			Event current = Event.current;
			for (int i = 0; i < p.maxEffects; i++)
			{
				Rect rect = p.bgRects[p.kEffectStartIndex + i];
				if (i < p.group.effects.Length)
				{
					AudioMixerEffectController effect = p.group.effects[i];
					if (p.visible)
					{
						if (current.type == EventType.ContextClick && rect.Contains(Event.current.mousePosition))
						{
							this.ClearFocus();
							this.DoEffectSlotInsertEffectPopup(rect, p.group, p.allGroups, i, ref p.effectMap);
							current.Use();
						}
						this.EffectSlot(rect, this.m_Controller.TargetSnapshot, effect, i, ref highlightEffectIndex, p, ref patchslots);
					}
				}
			}
			if (p.visible)
			{
				Rect rect2 = p.bgRects[p.bgRects.Count - 1];
				if (current.type == EventType.Repaint)
				{
					GUI.DrawTextureWithTexCoords(new Rect(rect2.x, rect2.y, rect2.width, rect2.height - 1f), this.styles.effectBar.hover.background, new Rect(0f, 0.5f, 0.1f, 0.1f));
					GUI.Label(rect2, this.addText, this.styles.effectName);
				}
				if (current.type == EventType.MouseDown && rect2.Contains(Event.current.mousePosition))
				{
					this.ClearFocus();
					int effectSlotIndex = p.group.effects.Length;
					this.DoEffectSlotInsertEffectPopup(rect2, p.group, p.allGroups, effectSlotIndex, ref p.effectMap);
					current.Use();
				}
			}
		}

		private float DoVUMeters(Rect vuRect, float attenuationMarkerHeight, AudioMixerChannelStripView.ChannelStripParams p)
		{
			float num = 1f;
			int num2 = p.numChannels;
			if (num2 == 0)
			{
				if (p.index >= 0 && p.index < this.m_LastNumChannels.Count)
				{
					num2 = this.m_LastNumChannels[p.index];
				}
			}
			else
			{
				while (p.index >= this.m_LastNumChannels.Count)
				{
					this.m_LastNumChannels.Add(0);
				}
				this.m_LastNumChannels[p.index] = num2;
			}
			if (num2 <= 2)
			{
				vuRect.x = vuRect.xMax - 25f;
				vuRect.width = 25f;
			}
			if (num2 == 0)
			{
				return vuRect.x;
			}
			float num3 = Mathf.Floor(attenuationMarkerHeight / 2f);
			vuRect.y += num3;
			vuRect.height -= 2f * num3;
			float num4 = Mathf.Round((vuRect.width - (float)num2 * num) / (float)num2);
			Rect r = new Rect(vuRect.xMax - num4, vuRect.y, num4, vuRect.height);
			for (int i = num2 - 1; i >= 0; i--)
			{
				if (i != num2 - 1)
				{
					r.x -= r.width + num;
				}
				float level = 1f - AudioMixerController.VolumeToScreenMapping(Mathf.Clamp(p.vuinfo_level[i], AudioMixerController.kMinVolume, AudioMixerController.GetMaxVolume()), 1f, true);
				float peak = 1f - AudioMixerController.VolumeToScreenMapping(Mathf.Clamp(p.vuinfo_peak[i], AudioMixerController.kMinVolume, AudioMixerController.GetMaxVolume()), 1f, true);
				this.VUMeter(p.group, r, level, peak);
			}
			AudioMixerDrawUtils.AddTooltipOverlay(vuRect, this.styles.vuMeterGUIContent.tooltip);
			return r.x;
		}

		private void DoSoloMuteBypassButtons(Rect rect, AudioMixerGroupController group, List<AudioMixerGroupController> allGroups, List<AudioMixerGroupController> selection, bool anySoloActive)
		{
			float num = 21f;
			float num2 = 2f;
			float x = rect.x + (rect.width - (num * 3f + num2 * 2f)) * 0.5f;
			Rect r = new Rect(x, rect.y, num, num);
			bool flag = false;
			flag |= this.DoSoloButton(r, group, allGroups, selection);
			r.x += num + num2;
			flag |= this.DoMuteButton(r, group, allGroups, anySoloActive, selection);
			if (flag)
			{
				this.m_Controller.UpdateMuteSolo();
			}
			r.x += num + num2;
			if (this.DoBypassEffectsButton(r, group, allGroups, selection))
			{
				this.m_Controller.UpdateBypass();
			}
		}

		public void OnMixerControllerChanged(AudioMixerController controller)
		{
			this.m_Controller = controller;
		}

		public void ShowDeveloperOverlays(Rect rect, Event evt, bool show)
		{
			if (show && Unsupported.IsDeveloperBuild() && evt.type == EventType.Repaint)
			{
				AudioMixerDrawUtils.ReadOnlyLabel(new Rect(rect.x + 5f, rect.y + 5f, rect.width - 10f, 20f), "Current snapshot: " + this.m_Controller.TargetSnapshot.name, this.developerInfoStyle);
				AudioMixerDrawUtils.ReadOnlyLabel(new Rect(rect.x + 5f, rect.y + 25f, rect.width - 10f, 20f), "Frame count: " + this.FrameCounter++, this.developerInfoStyle);
			}
		}

		public static float Lerp(float x1, float x2, float t)
		{
			return x1 + (x2 - x1) * t;
		}

		public static void GetCableVertex(float x1, float y1, float x2, float y2, float x3, float y3, float t, out float x, out float y)
		{
			x = AudioMixerChannelStripView.Lerp(AudioMixerChannelStripView.Lerp(x1, x2, t), AudioMixerChannelStripView.Lerp(x2, x3, t), t);
			y = AudioMixerChannelStripView.Lerp(AudioMixerChannelStripView.Lerp(y1, y2, t), AudioMixerChannelStripView.Lerp(y2, y3, t), t);
		}

		public void OnGUI(Rect rect, bool showReferencedBuses, bool showBusConnections, bool showBusConnectionsOfSelection, List<AudioMixerGroupController> allGroups, Dictionary<AudioMixerEffectController, AudioMixerGroupController> effectMap, bool sortGroupsAlphabetically, bool showDeveloperOverlays, AudioMixerGroupController scrollToItem)
		{
			if (this.m_Controller == null)
			{
				this.DrawAreaBackground(rect);
				return;
			}
			if (Event.current.type == EventType.Layout)
			{
				return;
			}
			this.m_RectSelectionControlID = GUIUtility.GetControlID(AudioMixerChannelStripView.kRectSelectionHashCode, FocusType.Passive);
			this.m_EffectInteractionControlID = GUIUtility.GetControlID(AudioMixerChannelStripView.kEffectDraggingHashCode, FocusType.Passive);
			this.m_IndexCounter = 0;
			Event current = Event.current;
			List<AudioMixerGroupController> list = this.m_Controller.GetCurrentViewGroupList().ToList<AudioMixerGroupController>();
			if (sortGroupsAlphabetically)
			{
				list.Sort(this.m_GroupComparer);
			}
			Rect rect2 = new Rect(this.channelStripsOffset.x, this.channelStripsOffset.y, 90f, 300f);
			if (scrollToItem != null)
			{
				int num = list.IndexOf(scrollToItem);
				if (num >= 0)
				{
					float num2 = (rect2.width + 4f) * (float)num - this.m_State.m_ScrollPos.x;
					if (num2 < -20f || num2 > rect.width)
					{
						AudioMixerChannelStripView.State expr_111_cp_0 = this.m_State;
						expr_111_cp_0.m_ScrollPos.x = expr_111_cp_0.m_ScrollPos.x + num2;
					}
				}
			}
			List<AudioMixerGroupController> list2 = new List<AudioMixerGroupController>();
			foreach (AudioMixerGroupController current2 in list)
			{
				AudioMixerEffectController[] effects = current2.effects;
				for (int i = 0; i < effects.Length; i++)
				{
					AudioMixerEffectController audioMixerEffectController = effects[i];
					if (audioMixerEffectController.sendTarget != null)
					{
						AudioMixerGroupController item = effectMap[audioMixerEffectController.sendTarget];
						if (!list2.Contains(item) && !list.Contains(item))
						{
							list2.Add(item);
						}
					}
				}
			}
			List<AudioMixerGroupController> list3 = list2.ToList<AudioMixerGroupController>();
			list3.Sort(this.m_GroupComparer);
			int count = list.Count;
			if (showReferencedBuses && list3.Count > 0)
			{
				list.AddRange(list3);
			}
			int num3 = 1;
			foreach (AudioMixerGroupController current3 in list)
			{
				num3 = Mathf.Max(num3, current3.effects.Length);
			}
			bool isShowingReferencedGroups = list.Count != count;
			Rect contentRect = this.GetContentRect(list, isShowingReferencedGroups, num3);
			this.m_State.m_ScrollPos = GUI.BeginScrollView(rect, this.m_State.m_ScrollPos, contentRect);
			this.DrawAreaBackground(new Rect(0f, 0f, 10000f, 10000f));
			AudioMixerChannelStripView.ChannelStripParams channelStripParams = new AudioMixerChannelStripView.ChannelStripParams();
			channelStripParams.effectMap = effectMap;
			channelStripParams.allGroups = allGroups;
			channelStripParams.shownGroups = list;
			channelStripParams.anySoloActive = allGroups.Any((AudioMixerGroupController g) => g.solo);
			channelStripParams.visibleRect = new Rect(this.m_State.m_ScrollPos.x, this.m_State.m_ScrollPos.y, rect.width, rect.height);
			AudioMixerChannelStripView.ChannelStripParams channelStripParams2 = channelStripParams;
			Dictionary<AudioMixerEffectController, AudioMixerChannelStripView.PatchSlot> dictionary = (!showBusConnections) ? null : new Dictionary<AudioMixerEffectController, AudioMixerChannelStripView.PatchSlot>();
			for (int j = 0; j < list.Count; j++)
			{
				AudioMixerGroupController group = list[j];
				channelStripParams2.index = j;
				channelStripParams2.group = group;
				channelStripParams2.drawingBuses = false;
				channelStripParams2.visible = AudioMixerChannelStripView.RectOverlaps(channelStripParams2.visibleRect, rect2);
				channelStripParams2.Init(this.m_Controller, rect2, num3);
				this.DrawChannelStrip(channelStripParams2, ref this.m_Controller.m_HighlightEffectIndex, ref dictionary, showBusConnectionsOfSelection);
				if (current.type == EventType.MouseDown && current.button == 0 && channelStripParams2.stripRect.Contains(current.mousePosition))
				{
					current.Use();
				}
				if (this.IsMovingEffect() && current.type == EventType.MouseDrag && channelStripParams2.stripRect.Contains(current.mousePosition) && GUIUtility.hotControl == this.m_EffectInteractionControlID)
				{
					this.m_MovingEffectDstIndex = -1;
					current.Use();
				}
				rect2.x += channelStripParams2.stripRect.width + 4f;
				if (showReferencedBuses && j == count - 1 && list.Count > count)
				{
					rect2.x += 50f;
					using (new EditorGUI.DisabledScope(true))
					{
						GUI.Label(new Rect(rect2.x, channelStripParams2.stripRect.yMax, 130f, 22f), this.styles.referencedGroups, this.styles.channelStripHeaderStyle);
					}
				}
			}
			this.UnhandledEffectDraggingEvents(ref this.m_Controller.m_HighlightEffectIndex);
			if (current.type == EventType.Repaint && dictionary != null)
			{
				foreach (KeyValuePair<AudioMixerEffectController, AudioMixerChannelStripView.PatchSlot> current4 in dictionary)
				{
					AudioMixerChannelStripView.PatchSlot value = current4.Value;
					bool flag = !showBusConnectionsOfSelection || this.m_Controller.CachedSelection.Contains(value.group);
					if (flag)
					{
						this.styles.circularToggle.Draw(new Rect(value.x - 3f, value.y - 3f, 6f, 6f), false, false, flag, false);
					}
				}
				float num4 = Mathf.Exp(-0.03f * Time.time * Time.time) * 0.1f;
				Color color = new Color(0f, 0f, 0f, (!AudioMixerController.EditingTargetSnapshot()) ? 0.05f : 0.1f);
				Color color2 = new Color(0f, 0f, 0f, (!AudioMixerController.EditingTargetSnapshot()) ? 0.5f : 1f);
				foreach (KeyValuePair<AudioMixerEffectController, AudioMixerChannelStripView.PatchSlot> current5 in dictionary)
				{
					AudioMixerEffectController key = current5.Key;
					AudioMixerEffectController sendTarget = key.sendTarget;
					if (!(sendTarget == null))
					{
						AudioMixerChannelStripView.PatchSlot value2 = current5.Value;
						if (dictionary.ContainsKey(sendTarget))
						{
							AudioMixerChannelStripView.PatchSlot patchSlot = dictionary[sendTarget];
							int num5 = key.GetInstanceID() ^ sendTarget.GetInstanceID();
							float num6 = (float)(num5 & 63) * 0.1f;
							float x = Mathf.Abs(patchSlot.x - value2.x) * Mathf.Sin(Time.time * 5f + num6) * num4 + (value2.x + patchSlot.x) * 0.5f;
							float y = Mathf.Abs(patchSlot.y - value2.y) * Mathf.Cos(Time.time * 5f + num6) * num4 + Math.Max(value2.y, patchSlot.y) + Mathf.Max(Mathf.Min(0.5f * Math.Abs(patchSlot.y - value2.y), 50f), 50f);
							for (int k = 0; k < this.cablepoints.Length; k++)
							{
								AudioMixerChannelStripView.GetCableVertex(value2.x, value2.y, x, y, patchSlot.x, patchSlot.y, (float)k / (float)(this.cablepoints.Length - 1), out this.cablepoints[k].x, out this.cablepoints[k].y);
							}
							bool flag2 = showBusConnectionsOfSelection && !this.m_Controller.CachedSelection.Contains(value2.group) && !this.m_Controller.CachedSelection.Contains(patchSlot.group);
							Handles.color = ((!flag2) ? color2 : color);
							Handles.DrawAAPolyLine(7f, this.cablepoints.Length, this.cablepoints);
							if (!flag2)
							{
								num5 ^= (num5 >> 6 ^ num5 >> 12 ^ num5 >> 18);
								Handles.color = new Color((float)(num5 & 3) * 0.15f + 0.55f, (float)(num5 >> 2 & 3) * 0.15f + 0.55f, (float)(num5 >> 4 & 3) * 0.15f + 0.55f, (!AudioMixerController.EditingTargetSnapshot()) ? 0.5f : 1f);
								Handles.DrawAAPolyLine(4f, this.cablepoints.Length, this.cablepoints);
								Handles.color = new Color(1f, 1f, 1f, (!AudioMixerController.EditingTargetSnapshot()) ? 0.25f : 0.5f);
								Handles.DrawAAPolyLine(3f, this.cablepoints.Length, this.cablepoints);
							}
						}
					}
				}
			}
			this.RectSelection(channelStripParams2);
			GUI.EndScrollView(true);
			AudioMixerDrawUtils.DrawScrollDropShadow(rect, this.m_State.m_ScrollPos.y, contentRect.height);
			this.WarningOverlay(allGroups, rect, contentRect);
			this.ShowDeveloperOverlays(rect, current, showDeveloperOverlays);
			if (!EditorApplication.isPlaying && !this.m_Controller.isSuspended)
			{
				this.m_RequiresRepaint = true;
			}
		}

		private bool IsMovingEffect()
		{
			return this.m_MovingEffectSrcIndex != -1;
		}

		private bool IsAdjustingWetMix()
		{
			return this.m_ChangingWetMixIndex != -1;
		}

		private void RectSelection(AudioMixerChannelStripView.ChannelStripParams channelStripParams)
		{
			Event current = Event.current;
			if (current.type == EventType.MouseDown && current.button == 0 && GUIUtility.hotControl == 0)
			{
				if (!current.shift)
				{
					Selection.objects = new UnityEngine.Object[0];
					this.m_Controller.OnUnitySelectionChanged();
				}
				GUIUtility.hotControl = this.m_RectSelectionControlID;
				this.m_RectSelectionStartPos = current.mousePosition;
				this.m_RectSelectionRect = new Rect(this.m_RectSelectionStartPos.x, this.m_RectSelectionStartPos.y, 0f, 0f);
				Event.current.Use();
				InspectorWindow.RepaintAllInspectors();
			}
			if (current.type == EventType.MouseDrag)
			{
				if (this.IsRectSelectionActive())
				{
					this.m_RectSelectionRect.x = Mathf.Min(this.m_RectSelectionStartPos.x, current.mousePosition.x);
					this.m_RectSelectionRect.y = Mathf.Min(this.m_RectSelectionStartPos.y, current.mousePosition.y);
					this.m_RectSelectionRect.width = Mathf.Max(this.m_RectSelectionStartPos.x, current.mousePosition.x) - this.m_RectSelectionRect.x;
					this.m_RectSelectionRect.height = Mathf.Max(this.m_RectSelectionStartPos.y, current.mousePosition.y) - this.m_RectSelectionRect.y;
					Event.current.Use();
				}
				if (this.m_MovingSrcRect.y >= 0f)
				{
					Event.current.Use();
				}
			}
			if (this.IsRectSelectionActive() && current.GetTypeForControl(this.m_RectSelectionControlID) == EventType.MouseUp)
			{
				List<AudioMixerGroupController> list = (!current.shift) ? new List<AudioMixerGroupController>() : this.m_Controller.CachedSelection;
				foreach (AudioMixerGroupController current2 in channelStripParams.rectSelectionGroups)
				{
					if (!list.Contains(current2))
					{
						list.Add(current2);
					}
				}
				Selection.objects = list.ToArray();
				this.m_Controller.OnUnitySelectionChanged();
				GUIUtility.hotControl = 0;
				Event.current.Use();
			}
			if (current.type == EventType.Repaint && this.IsRectSelectionActive())
			{
				Color color = new Color(1f, 1f, 1f, 0.3f);
				AudioMixerDrawUtils.DrawGradientRectHorizontal(this.m_RectSelectionRect, color, color);
			}
		}

		private void WarningOverlay(List<AudioMixerGroupController> allGroups, Rect rect, Rect contentRect)
		{
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			foreach (AudioMixerGroupController current in allGroups)
			{
				if (current.solo)
				{
					num++;
				}
				if (current.mute)
				{
					num2++;
				}
				if (current.bypassEffects)
				{
					num3 += current.effects.Length - 1;
				}
				else
				{
					num3 += current.effects.Count((AudioMixerEffectController e) => e.bypass);
				}
			}
			Event current2 = Event.current;
			if ((current2.type == EventType.Repaint && num > 0) || num2 > 0 || num3 > 0)
			{
				string t = "Warning: " + ((num <= 0) ? ((num2 <= 0) ? (num3 + ((num3 <= 1) ? " effect" : " effects") + " currently bypassed") : (num2 + ((num2 <= 1) ? " group" : " groups") + " currently muted")) : (num + ((num <= 1) ? " group" : " groups") + " currently soloed"));
				bool flag = contentRect.width > rect.width;
				float x = this.styles.warningOverlay.CalcSize(GUIContent.Temp(t)).x;
				Rect position = new Rect(rect.x + 5f + Mathf.Max((rect.width - 10f - x) * 0.5f, 0f), rect.yMax - this.styles.warningOverlay.fixedHeight - 5f - ((!flag) ? 0f : 17f), x, this.styles.warningOverlay.fixedHeight);
				GUI.Label(position, GUIContent.Temp(t), this.styles.warningOverlay);
			}
		}

		private Rect GetContentRect(List<AudioMixerGroupController> sortedGroups, bool isShowingReferencedGroups, int maxEffects)
		{
			float num = 239f;
			float height = this.channelStripsOffset.y + num + (float)maxEffects * 16f + 10f + 16f + 10f;
			float width = this.channelStripsOffset.x * 2f + 94f * (float)sortedGroups.Count + ((!isShowingReferencedGroups) ? 0f : 50f);
			return new Rect(0f, 0f, width, height);
		}
	}
}
