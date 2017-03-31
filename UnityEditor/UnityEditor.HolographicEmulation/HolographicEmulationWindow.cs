using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR;

namespace UnityEditor.HolographicEmulation
{
	internal class HolographicEmulationWindow : EditorWindow
	{
		private bool m_InPlayMode = false;

		private bool m_OperatingSystemChecked = false;

		private bool m_OperatingSystemValid = false;

		private HolographicStreamerConnectionState m_ConnectionState = HolographicStreamerConnectionState.Disconnected;

		[SerializeField]
		private EmulationMode m_Mode = EmulationMode.None;

		[SerializeField]
		private int m_RoomIndex = 0;

		[SerializeField]
		private GestureHand m_Hand = GestureHand.Right;

		[SerializeField]
		private string m_RemoteMachineAddress = "";

		[SerializeField]
		private bool m_EnableVideo = true;

		[SerializeField]
		private bool m_EnableAudio = true;

		[SerializeField]
		private int m_MaxBitrateKbps = 99999;

		private string[] m_RemoteMachineHistory;

		private static int s_MaxHistoryLength = 4;

		private static GUIContent s_ConnectionStatusText = new GUIContent("Connection Status");

		private static GUIContent s_EmulationModeText = new GUIContent("Emulation Mode");

		private static GUIContent s_RoomText = new GUIContent("Room");

		private static GUIContent s_HandText = new GUIContent("Gesture Hand");

		private static GUIContent s_RemoteMachineText = new GUIContent("Remote Machine");

		private static GUIContent s_EnableVideoText = new GUIContent("Enable Video");

		private static GUIContent s_EnableAudioText = new GUIContent("Enable Audio");

		private static GUIContent s_MaxBitrateText = new GUIContent("Max Bitrate (kbps)");

		private static GUIContent s_ConnectionButtonConnectText = new GUIContent("Connect");

		private static GUIContent s_ConnectionButtonDisconnectText = new GUIContent("Disconnect");

		private static GUIContent s_ConnectionStateDisconnectedText = new GUIContent("Disconnected");

		private static GUIContent s_ConnectionStateConnectingText = new GUIContent("Connecting");

		private static GUIContent s_ConnectionStateConnectedText = new GUIContent("Connected");

		private static Texture2D s_ConnectedTexture = null;

		private static Texture2D s_ConnectingTexture = null;

		private static Texture2D s_DisconnectedTexture = null;

		private static GUIContent[] s_ModeStrings = new GUIContent[]
		{
			new GUIContent("None"),
			new GUIContent("Remote to Device"),
			new GUIContent("Simulate in Editor")
		};

		private static GUIContent[] s_RoomStrings = new GUIContent[]
		{
			new GUIContent("DefaultRoom"),
			new GUIContent("Bedroom1"),
			new GUIContent("Bedroom2"),
			new GUIContent("GreatRoom"),
			new GUIContent("LivingRoom")
		};

		private static GUIContent[] s_HandStrings = new GUIContent[]
		{
			new GUIContent("Left Hand"),
			new GUIContent("Right Hand")
		};

		private bool RemoteMachineNameSpecified
		{
			get
			{
				return !string.IsNullOrEmpty(this.m_RemoteMachineAddress);
			}
		}

		public static void Init()
		{
			HolographicEmulationWindow window = EditorWindow.GetWindow<HolographicEmulationWindow>(false);
			window.titleContent = new GUIContent("Holographic");
		}

		private void OnEnable()
		{
			EditorApplication.playmodeStateChanged = (EditorApplication.CallbackFunction)Delegate.Combine(EditorApplication.playmodeStateChanged, new EditorApplication.CallbackFunction(this.OnPlayModeChanged));
			this.m_InPlayMode = EditorApplication.isPlayingOrWillChangePlaymode;
			this.m_RemoteMachineHistory = EditorPrefs.GetString("HolographicRemoting.RemoteMachineHistory").Split(new char[]
			{
				','
			});
		}

		private void OnDisable()
		{
			EditorApplication.playmodeStateChanged = (EditorApplication.CallbackFunction)Delegate.Remove(EditorApplication.playmodeStateChanged, new EditorApplication.CallbackFunction(this.OnPlayModeChanged));
		}

		private void LoadCurrentRoom()
		{
			string str = EditorApplication.applicationContentsPath + "/UnityExtensions/Unity/VR/HolographicSimulation/Rooms/";
			HolographicEmulation.LoadRoom(str + HolographicEmulationWindow.s_RoomStrings[this.m_RoomIndex].text + ".xef");
		}

		private void InitializeSimulation()
		{
			if (this.m_ConnectionState != HolographicStreamerConnectionState.Disconnected)
			{
				this.Disconnect();
			}
			HolographicEmulation.Initialize();
			this.LoadCurrentRoom();
		}

		private void OnPlayModeChanged()
		{
			bool inPlayMode = this.m_InPlayMode;
			this.m_InPlayMode = EditorApplication.isPlayingOrWillChangePlaymode;
			if (this.m_InPlayMode && !inPlayMode)
			{
				HolographicEmulation.SetEmulationMode(this.m_Mode);
				EmulationMode mode = this.m_Mode;
				if (mode != EmulationMode.Simulated)
				{
					if (mode != EmulationMode.RemoteDevice)
					{
					}
				}
				else
				{
					this.InitializeSimulation();
				}
			}
			else if (!this.m_InPlayMode && inPlayMode)
			{
				EmulationMode mode2 = this.m_Mode;
				if (mode2 != EmulationMode.Simulated)
				{
					if (mode2 != EmulationMode.RemoteDevice)
					{
					}
				}
				else
				{
					HolographicEmulation.Shutdown();
				}
			}
		}

		private void Connect()
		{
			PerceptionRemotingPlugin.SetVideoEncodingParameters(this.m_MaxBitrateKbps);
			PerceptionRemotingPlugin.SetEnableVideo(this.m_EnableVideo);
			PerceptionRemotingPlugin.SetEnableAudio(this.m_EnableAudio);
			PerceptionRemotingPlugin.Connect(this.m_RemoteMachineAddress);
		}

		private void Disconnect()
		{
			PerceptionRemotingPlugin.Disconnect();
		}

		private void HandleButtonPress()
		{
			if (EditorApplication.isPlayingOrWillChangePlaymode)
			{
				Debug.LogError("Unable to connect / disconnect remoting while playing.");
			}
			else if (this.m_ConnectionState == HolographicStreamerConnectionState.Connecting || this.m_ConnectionState == HolographicStreamerConnectionState.Connected)
			{
				this.Disconnect();
			}
			else if (this.RemoteMachineNameSpecified)
			{
				this.Connect();
			}
			else
			{
				Debug.LogError("Cannot connect without a remote machine address specified");
			}
		}

		private void UpdateRemoteMachineHistory()
		{
			List<string> list = new List<string>(this.m_RemoteMachineHistory);
			int i = 0;
			while (i < this.m_RemoteMachineHistory.Length)
			{
				if (this.m_RemoteMachineHistory[i].Equals(this.m_RemoteMachineAddress, StringComparison.CurrentCultureIgnoreCase))
				{
					if (i == 0)
					{
						return;
					}
					list.RemoveAt(i);
					break;
				}
				else
				{
					i++;
				}
			}
			list.Insert(0, this.m_RemoteMachineAddress);
			if (list.Count > HolographicEmulationWindow.s_MaxHistoryLength)
			{
				list.RemoveRange(HolographicEmulationWindow.s_MaxHistoryLength, list.Count - HolographicEmulationWindow.s_MaxHistoryLength);
			}
			this.m_RemoteMachineHistory = list.ToArray();
			EditorPrefs.SetString("HolographicRemoting.RemoteMachineHistory", string.Join(",", this.m_RemoteMachineHistory));
		}

		private void RemotingPreferencesOnGUI()
		{
			EditorGUI.BeginChangeCheck();
			this.m_RemoteMachineAddress = EditorGUILayout.DelayedTextFieldDropDown(HolographicEmulationWindow.s_RemoteMachineText, this.m_RemoteMachineAddress, this.m_RemoteMachineHistory);
			if (EditorGUI.EndChangeCheck())
			{
				this.UpdateRemoteMachineHistory();
			}
			this.m_EnableVideo = EditorGUILayout.Toggle(HolographicEmulationWindow.s_EnableVideoText, this.m_EnableVideo, new GUILayoutOption[0]);
			this.m_EnableAudio = EditorGUILayout.Toggle(HolographicEmulationWindow.s_EnableAudioText, this.m_EnableAudio, new GUILayoutOption[0]);
			this.m_MaxBitrateKbps = EditorGUILayout.IntSlider(HolographicEmulationWindow.s_MaxBitrateText, this.m_MaxBitrateKbps, 1024, 99999, new GUILayoutOption[0]);
		}

		private void ConnectionStateGUI()
		{
			if (HolographicEmulationWindow.s_ConnectedTexture == null)
			{
				HolographicEmulationWindow.s_ConnectedTexture = EditorGUIUtility.LoadIconRequired("sv_icon_dot3_sml");
				HolographicEmulationWindow.s_ConnectingTexture = EditorGUIUtility.LoadIconRequired("sv_icon_dot4_sml");
				HolographicEmulationWindow.s_DisconnectedTexture = EditorGUIUtility.LoadIconRequired("sv_icon_dot6_sml");
			}
			Texture2D image;
			GUIContent label;
			GUIContent content;
			switch (this.m_ConnectionState)
			{
			case HolographicStreamerConnectionState.Disconnected:
				IL_5E:
				image = HolographicEmulationWindow.s_DisconnectedTexture;
				label = HolographicEmulationWindow.s_ConnectionStateDisconnectedText;
				content = HolographicEmulationWindow.s_ConnectionButtonConnectText;
				goto IL_A3;
			case HolographicStreamerConnectionState.Connecting:
				image = HolographicEmulationWindow.s_ConnectingTexture;
				label = HolographicEmulationWindow.s_ConnectionStateConnectingText;
				content = HolographicEmulationWindow.s_ConnectionButtonDisconnectText;
				goto IL_A3;
			case HolographicStreamerConnectionState.Connected:
				image = HolographicEmulationWindow.s_ConnectedTexture;
				label = HolographicEmulationWindow.s_ConnectionStateConnectedText;
				content = HolographicEmulationWindow.s_ConnectionButtonDisconnectText;
				goto IL_A3;
			}
			goto IL_5E;
			IL_A3:
			EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
			EditorGUILayout.PrefixLabel(HolographicEmulationWindow.s_ConnectionStatusText, "Button");
			float singleLineHeight = EditorGUIUtility.singleLineHeight;
			Rect rect = GUILayoutUtility.GetRect(singleLineHeight, singleLineHeight, new GUILayoutOption[]
			{
				GUILayout.ExpandWidth(false)
			});
			GUI.DrawTexture(rect, image);
			EditorGUILayout.LabelField(label, new GUILayoutOption[0]);
			EditorGUILayout.EndHorizontal();
			EditorGUI.BeginDisabledGroup(this.m_InPlayMode);
			bool flag = EditorGUILayout.DropdownButton(content, FocusType.Passive, EditorStyles.miniButton, new GUILayoutOption[0]);
			EditorGUI.EndDisabledGroup();
			if (flag)
			{
				this.HandleButtonPress();
			}
		}

		private bool IsHoloLensCurrentTarget()
		{
			return PlayerSettings.virtualRealitySupported && Array.IndexOf<string>(VRSettings.supportedDevices, "HoloLens") >= 0;
		}

		private void DrawRemotingMode()
		{
			this.m_Mode = (EmulationMode)EditorGUILayout.Popup(HolographicEmulationWindow.s_EmulationModeText, (int)this.m_Mode, HolographicEmulationWindow.s_ModeStrings, new GUILayoutOption[0]);
		}

		private bool CheckOperatingSystem()
		{
			if (!this.m_OperatingSystemChecked)
			{
				this.m_OperatingSystemValid = (Environment.OSVersion.Version.Build >= 14318);
				this.m_OperatingSystemChecked = true;
			}
			return this.m_OperatingSystemValid;
		}

		private void OnGUI()
		{
			if (!this.CheckOperatingSystem())
			{
				EditorGUILayout.HelpBox("You must be running Windows build 14318 or later to use Holographic Simulation or Remoting.", MessageType.Warning);
			}
			else if (!this.IsHoloLensCurrentTarget())
			{
				EditorGUILayout.HelpBox("You must enable Virtual Reality support in settings and add Windows Holographic to the devices to use Holographic Emulation.", MessageType.Warning);
			}
			else
			{
				EditorGUILayout.Space();
				EditorGUI.BeginDisabledGroup(this.m_InPlayMode);
				this.DrawRemotingMode();
				EditorGUI.EndDisabledGroup();
				EmulationMode mode = this.m_Mode;
				if (mode != EmulationMode.RemoteDevice)
				{
					if (mode == EmulationMode.Simulated)
					{
						EditorGUI.BeginChangeCheck();
						this.m_RoomIndex = EditorGUILayout.Popup(HolographicEmulationWindow.s_RoomText, this.m_RoomIndex, HolographicEmulationWindow.s_RoomStrings, new GUILayoutOption[0]);
						if (EditorGUI.EndChangeCheck() && this.m_InPlayMode)
						{
							this.LoadCurrentRoom();
						}
						EditorGUI.BeginChangeCheck();
						this.m_Hand = (GestureHand)EditorGUILayout.Popup(HolographicEmulationWindow.s_HandText, (int)this.m_Hand, HolographicEmulationWindow.s_HandStrings, new GUILayoutOption[0]);
						if (EditorGUI.EndChangeCheck())
						{
							HolographicEmulation.SetGestureHand(this.m_Hand);
						}
					}
				}
				else
				{
					EditorGUI.BeginDisabledGroup(this.m_ConnectionState != HolographicStreamerConnectionState.Disconnected);
					this.RemotingPreferencesOnGUI();
					EditorGUI.EndDisabledGroup();
					this.ConnectionStateGUI();
				}
			}
		}

		private void Update()
		{
			EmulationMode mode = this.m_Mode;
			if (mode != EmulationMode.Simulated)
			{
				if (mode == EmulationMode.RemoteDevice)
				{
					HolographicStreamerConnectionState connectionState = this.m_ConnectionState;
					this.m_ConnectionState = PerceptionRemotingPlugin.GetConnectionState();
					if (connectionState != this.m_ConnectionState)
					{
						base.Repaint();
					}
					HolographicStreamerConnectionFailureReason holographicStreamerConnectionFailureReason = PerceptionRemotingPlugin.CheckForDisconnect();
					if (holographicStreamerConnectionFailureReason == HolographicStreamerConnectionFailureReason.Unreachable || holographicStreamerConnectionFailureReason == HolographicStreamerConnectionFailureReason.ConnectionLost)
					{
						Debug.LogWarning("Disconnected with failure reason " + holographicStreamerConnectionFailureReason + ", attempting to reconnect.");
						this.Connect();
					}
					else if (holographicStreamerConnectionFailureReason != HolographicStreamerConnectionFailureReason.None)
					{
						Debug.LogError("Disconnected with error " + holographicStreamerConnectionFailureReason);
					}
				}
			}
			else
			{
				HolographicEmulation.SetGestureHand(this.m_Hand);
			}
		}
	}
}
