using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Scripting;

namespace UnityEditor
{
	internal sealed class LicenseManagementWindow : EditorWindow
	{
		private static int width = 600;

		private static int height = 350;

		private static int left = 0;

		private static int top = 0;

		private static int offsetX = 50;

		private static int offsetY = 25;

		private static int buttonWidth = 140;

		private static Rect windowArea;

		private static Rect rectArea = new Rect((float)LicenseManagementWindow.offsetX, (float)LicenseManagementWindow.offsetY, (float)(LicenseManagementWindow.width - LicenseManagementWindow.offsetX * 2), (float)(LicenseManagementWindow.height - LicenseManagementWindow.offsetY * 2));

		private static LicenseManagementWindow win = null;

		private static LicenseManagementWindow Window
		{
			get
			{
				if (LicenseManagementWindow.win == null)
				{
					LicenseManagementWindow.win = EditorWindow.GetWindowWithRect<LicenseManagementWindow>(LicenseManagementWindow.windowArea, true, "License Management");
				}
				return LicenseManagementWindow.win;
			}
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void CheckForUpdates();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void ActivateNewLicense();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void ManualActivation();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void ReturnLicense();

		private static void ShowWindow()
		{
			Resolution currentResolution = Screen.currentResolution;
			LicenseManagementWindow.left = (currentResolution.width - LicenseManagementWindow.width) / 2;
			LicenseManagementWindow.top = (currentResolution.height - LicenseManagementWindow.height) / 2;
			LicenseManagementWindow.windowArea = new Rect((float)LicenseManagementWindow.left, (float)LicenseManagementWindow.top, (float)LicenseManagementWindow.width, (float)LicenseManagementWindow.height);
			LicenseManagementWindow.win = LicenseManagementWindow.Window;
			LicenseManagementWindow.win.position = LicenseManagementWindow.windowArea;
			LicenseManagementWindow.win.Show();
		}

		private void OnGUI()
		{
			GUILayout.BeginArea(LicenseManagementWindow.rectArea);
			GUILayout.FlexibleSpace();
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			if (GUILayout.Button("Check for updates", new GUILayoutOption[]
			{
				GUILayout.ExpandHeight(true),
				GUILayout.Width((float)LicenseManagementWindow.buttonWidth)
			}))
			{
				LicenseManagementWindow.CheckForUpdates();
			}
			GUI.skin.label.wordWrap = true;
			GUILayout.Label("Checks for updates to the currently installed license. If you have purchased addons you can install them by pressing this button (Internet access required)", new GUILayoutOption[0]);
			GUILayout.EndHorizontal();
			GUILayout.Space(15f);
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			if (GUILayout.Button("Activate new license", new GUILayoutOption[]
			{
				GUILayout.ExpandHeight(true),
				GUILayout.Width((float)LicenseManagementWindow.buttonWidth)
			}))
			{
				LicenseManagementWindow.ActivateNewLicense();
				LicenseManagementWindow.Window.Close();
			}
			GUILayout.Label("Activate Unity with a different serial number, switch to a free serial or start a trial period if you are eligible for it (Internet access required).", new GUILayoutOption[0]);
			GUILayout.EndHorizontal();
			GUILayout.Space(15f);
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			if (GUILayout.Button("Return license", new GUILayoutOption[]
			{
				GUILayout.ExpandHeight(true),
				GUILayout.Width((float)LicenseManagementWindow.buttonWidth)
			}))
			{
				LicenseManagementWindow.ReturnLicense();
			}
			GUILayout.Label("Return this license and free an activation for the serial it is using. You can then reuse the activation on another machine (Internet access required).", new GUILayoutOption[0]);
			GUILayout.EndHorizontal();
			GUILayout.Space(15f);
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			if (GUILayout.Button("Manual activation", new GUILayoutOption[]
			{
				GUILayout.ExpandHeight(true),
				GUILayout.Width((float)LicenseManagementWindow.buttonWidth)
			}))
			{
				LicenseManagementWindow.ManualActivation();
			}
			GUILayout.Label("Start the manual activation process, you can save this machines license activation request file or deploy a license file you have already activated manually.", new GUILayoutOption[0]);
			GUILayout.EndHorizontal();
			GUILayout.Space(15f);
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			if (GUILayout.Button("Unity FAQ", new GUILayoutOption[]
			{
				GUILayout.ExpandHeight(true),
				GUILayout.Width((float)LicenseManagementWindow.buttonWidth)
			}))
			{
				Application.OpenURL("http://unity3d.com/unity/faq");
			}
			GUILayout.Label("Open the Unity FAQ web page, where you can find information about Unity's license system.", new GUILayoutOption[0]);
			GUILayout.EndHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.EndArea();
		}
	}
}
