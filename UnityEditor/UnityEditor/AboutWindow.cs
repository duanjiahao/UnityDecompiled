using System;
using UnityEditor.Modules;
using UnityEditor.VisualStudioIntegration;
using UnityEditorInternal;
using UnityEngine;

namespace UnityEditor
{
	internal class AboutWindow : EditorWindow
	{
		private static GUIContent s_MonoLogo;

		private static GUIContent s_AgeiaLogo;

		private static GUIContent s_Header;

		private const string kSpecialThanksNames = "Thanks to Forest 'Yoggy' Johnson, Graham McAllister, David Janik-Jones, Raimund Schumacher, Alan J. Dickins and Emil 'Humus' Persson";

		private float m_TextYPos = 120f;

		private float m_TextInitialYPos = 120f;

		private float m_TotalCreditsHeight = float.PositiveInfinity;

		private double m_LastScrollUpdate = 0.0;

		private bool m_ShowDetailedVersion = false;

		private int m_InternalCodeProgress;

		private static void ShowAboutWindow()
		{
			AboutWindow windowWithRect = EditorWindow.GetWindowWithRect<AboutWindow>(new Rect(100f, 100f, 570f, 340f), true, "About Unity");
			windowWithRect.position = new Rect(100f, 100f, 570f, 340f);
			windowWithRect.m_Parent.window.m_DontSaveToLayout = true;
		}

		private static void LoadLogos()
		{
			if (AboutWindow.s_MonoLogo == null)
			{
				AboutWindow.s_MonoLogo = EditorGUIUtility.IconContent("MonoLogo");
				AboutWindow.s_AgeiaLogo = EditorGUIUtility.IconContent("AgeiaLogo");
				AboutWindow.s_Header = EditorGUIUtility.IconContent("AboutWindow.MainHeader");
			}
		}

		public void OnEnable()
		{
			EditorApplication.update = (EditorApplication.CallbackFunction)Delegate.Combine(EditorApplication.update, new EditorApplication.CallbackFunction(this.UpdateScroll));
			this.m_LastScrollUpdate = EditorApplication.timeSinceStartup;
		}

		public void OnDisable()
		{
			EditorApplication.update = (EditorApplication.CallbackFunction)Delegate.Remove(EditorApplication.update, new EditorApplication.CallbackFunction(this.UpdateScroll));
		}

		public void UpdateScroll()
		{
			double num = EditorApplication.timeSinceStartup - this.m_LastScrollUpdate;
			this.m_TextYPos -= 40f * (float)num;
			if (this.m_TextYPos < -this.m_TotalCreditsHeight)
			{
				this.m_TextYPos = this.m_TextInitialYPos;
			}
			base.Repaint();
			this.m_LastScrollUpdate = EditorApplication.timeSinceStartup;
		}

		public void OnGUI()
		{
			AboutWindow.LoadLogos();
			GUILayout.Space(10f);
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.Space(5f);
			GUILayout.BeginVertical(new GUILayoutOption[0]);
			GUILayout.FlexibleSpace();
			GUILayout.Label(AboutWindow.s_Header, GUIStyle.none, new GUILayoutOption[0]);
			this.ListenForSecretCodes();
			string text = "";
			if (InternalEditorUtility.HasFreeLicense())
			{
				text = " Personal";
			}
			if (InternalEditorUtility.HasEduLicense())
			{
				text = " Edu";
			}
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.Space(52f);
			string text2 = this.FormatExtensionVersionString();
			this.m_ShowDetailedVersion |= Event.current.alt;
			if (this.m_ShowDetailedVersion)
			{
				int unityVersionDate = InternalEditorUtility.GetUnityVersionDate();
				DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
				string unityBuildBranch = InternalEditorUtility.GetUnityBuildBranch();
				string text3 = "";
				if (unityBuildBranch.Length > 0)
				{
					text3 = "Branch: " + unityBuildBranch;
				}
				EditorGUILayout.SelectableLabel(string.Format("Version {0}{1}{2}\n{3:r}\n{4}", new object[]
				{
					InternalEditorUtility.GetFullUnityVersion(),
					text,
					text2,
					dateTime.AddSeconds((double)unityVersionDate),
					text3
				}), new GUILayoutOption[]
				{
					GUILayout.Width(550f),
					GUILayout.Height(42f)
				});
				this.m_TextInitialYPos = 108f;
			}
			else
			{
				GUILayout.Label(string.Format("Version {0}{1}{2}", Application.unityVersion, text, text2), new GUILayoutOption[0]);
			}
			if (Event.current.type != EventType.ValidateCommand)
			{
				GUILayout.EndHorizontal();
				GUILayout.Space(4f);
				GUILayout.EndVertical();
				GUILayout.EndHorizontal();
				GUILayout.FlexibleSpace();
				float creditsWidth = base.position.width - 10f;
				float num = this.m_TextYPos;
				GUI.BeginGroup(GUILayoutUtility.GetRect(10f, this.m_TextInitialYPos));
				string[] nameChunks = AboutWindowNames.nameChunks;
				for (int i = 0; i < nameChunks.Length; i++)
				{
					string nameChunk = nameChunks[i];
					num = AboutWindow.DoCreditsNameChunk(nameChunk, creditsWidth, num);
				}
				num = AboutWindow.DoCreditsNameChunk("Thanks to Forest 'Yoggy' Johnson, Graham McAllister, David Janik-Jones, Raimund Schumacher, Alan J. Dickins and Emil 'Humus' Persson", creditsWidth, num);
				this.m_TotalCreditsHeight = num - this.m_TextYPos;
				GUI.EndGroup();
				GUILayout.FlexibleSpace();
				GUILayout.BeginHorizontal(new GUILayoutOption[0]);
				GUILayout.Label(AboutWindow.s_MonoLogo, new GUILayoutOption[0]);
				GUILayout.Label("Scripting powered by The Mono Project.\n\n(c) 2011 Novell, Inc.", "MiniLabel", new GUILayoutOption[]
				{
					GUILayout.Width(200f)
				});
				GUILayout.Label(AboutWindow.s_AgeiaLogo, new GUILayoutOption[0]);
				GUILayout.Label("Physics powered by PhysX.\n\n(c) 2011 NVIDIA Corporation.", "MiniLabel", new GUILayoutOption[]
				{
					GUILayout.Width(200f)
				});
				GUILayout.EndHorizontal();
				GUILayout.FlexibleSpace();
				GUILayout.BeginHorizontal(new GUILayoutOption[0]);
				GUILayout.Space(5f);
				GUILayout.BeginVertical(new GUILayoutOption[0]);
				GUILayout.FlexibleSpace();
				string aboutWindowLabel = UnityVSSupport.GetAboutWindowLabel();
				if (aboutWindowLabel.Length > 0)
				{
					GUILayout.Label(aboutWindowLabel, "MiniLabel", new GUILayoutOption[0]);
				}
				GUILayout.Label(InternalEditorUtility.GetUnityCopyright(), "MiniLabel", new GUILayoutOption[0]);
				GUILayout.EndVertical();
				GUILayout.Space(10f);
				GUILayout.FlexibleSpace();
				GUILayout.BeginVertical(new GUILayoutOption[0]);
				GUILayout.FlexibleSpace();
				GUILayout.Label(InternalEditorUtility.GetLicenseInfo(), "AboutWindowLicenseLabel", new GUILayoutOption[0]);
				GUILayout.EndVertical();
				GUILayout.Space(5f);
				GUILayout.EndHorizontal();
				GUILayout.Space(5f);
			}
		}

		private static float DoCreditsNameChunk(string nameChunk, float creditsWidth, float creditsChunkYOffset)
		{
			float height = EditorStyles.wordWrappedLabel.CalcHeight(GUIContent.Temp(nameChunk), creditsWidth);
			Rect position = new Rect(5f, creditsChunkYOffset, creditsWidth, height);
			GUI.Label(position, nameChunk, EditorStyles.wordWrappedLabel);
			return position.yMax;
		}

		private void ListenForSecretCodes()
		{
			if (Event.current.type == EventType.KeyDown && Event.current.character != '\0')
			{
				if (this.SecretCodeHasBeenTyped("internal", ref this.m_InternalCodeProgress))
				{
					bool flag = !EditorPrefs.GetBool("InternalMode", false);
					EditorPrefs.SetBool("InternalMode", flag);
					base.ShowNotification(new GUIContent("Internal Mode " + ((!flag) ? "Off" : "On")));
					InternalEditorUtility.RequestScriptReload();
				}
			}
		}

		private bool SecretCodeHasBeenTyped(string code, ref int characterProgress)
		{
			if (characterProgress < 0 || characterProgress >= code.Length || code[characterProgress] != Event.current.character)
			{
				characterProgress = 0;
			}
			bool result;
			if (code[characterProgress] == Event.current.character)
			{
				characterProgress++;
				if (characterProgress >= code.Length)
				{
					characterProgress = 0;
					result = true;
					return result;
				}
			}
			result = false;
			return result;
		}

		private string FormatExtensionVersionString()
		{
			string text = EditorUserBuildSettings.selectedBuildTargetGroup.ToString();
			string extensionVersion = ModuleManager.GetExtensionVersion(text);
			string result;
			if (!string.IsNullOrEmpty(extensionVersion))
			{
				result = string.Concat(new string[]
				{
					" [",
					text,
					": ",
					extensionVersion,
					"]"
				});
			}
			else
			{
				result = "";
			}
			return result;
		}
	}
}
