using System;
using UnityEditorInternal;
using UnityEngine;
namespace UnityEditor
{
	internal class AboutWindow : EditorWindow
	{
		private const string kSpecialThanksNames = "Thanks to Forest 'Yoggy' Johnson, Graham McAllister, David Janik-Jones, Raimund Schumacher, Alan J. Dickins and Emil 'Humus' Persson";
		private static GUIContent s_MonoLogo;
		private static GUIContent s_AgeiaLogo;
		private static GUIContent s_Header;
		private readonly string kCreditsNames = string.Join(", ", AboutWindowNames.names);
		private float m_TextYPos = 120f;
		private float m_TextInitialYPos = 120f;
		private float m_TotalCreditsHeight = float.PositiveInfinity;
		private double m_LastScrollUpdate;
		private bool m_ShowDetailedVersion;
		private int m_InternalCodeProgress;
		private static void ShowAboutWindow()
		{
			AboutWindow windowWithRect = EditorWindow.GetWindowWithRect<AboutWindow>(new Rect(100f, 100f, 570f, 340f), true, "About Unity");
			windowWithRect.position = new Rect(100f, 100f, 570f, 340f);
			windowWithRect.m_Parent.window.m_DontSaveToLayout = true;
		}
		private static void LoadLogos()
		{
			if (AboutWindow.s_MonoLogo != null)
			{
				return;
			}
			AboutWindow.s_MonoLogo = EditorGUIUtility.IconContent("MonoLogo");
			AboutWindow.s_AgeiaLogo = EditorGUIUtility.IconContent("AgeiaLogo");
			AboutWindow.s_Header = EditorGUIUtility.IconContent("AboutWindow.MainHeader");
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
			string text = string.Empty;
			if (InternalEditorUtility.HasPro())
			{
				text = " Pro";
			}
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.Space(52f);
			this.m_ShowDetailedVersion |= Event.current.alt;
			if (this.m_ShowDetailedVersion)
			{
				int unityVersionDate = InternalEditorUtility.GetUnityVersionDate();
				DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
				string unityBuildBranch = InternalEditorUtility.GetUnityBuildBranch();
				string text2 = string.Empty;
				if (unityBuildBranch.Length > 0)
				{
					text2 = "Branch: " + unityBuildBranch;
				}
				EditorGUILayout.SelectableLabel(string.Format("Version {0}{1}\n{2:r}\n{3}", new object[]
				{
					InternalEditorUtility.GetFullUnityVersion(),
					text,
					dateTime.AddSeconds((double)unityVersionDate),
					text2
				}), new GUILayoutOption[]
				{
					GUILayout.Width(400f),
					GUILayout.Height(42f)
				});
				this.m_TextInitialYPos = 108f;
			}
			else
			{
				GUILayout.Label(string.Format("Version {0}{1}", Application.unityVersion, text), new GUILayoutOption[0]);
			}
			if (Event.current.type == EventType.ValidateCommand)
			{
				return;
			}
			GUILayout.EndHorizontal();
			GUILayout.Space(4f);
			GUILayout.EndVertical();
			GUILayout.EndHorizontal();
			GUILayout.FlexibleSpace();
			GUI.BeginGroup(GUILayoutUtility.GetRect(10f, this.m_TextInitialYPos));
			float width = base.position.width - 10f;
			float num = EditorStyles.wordWrappedLabel.CalcHeight(GUIContent.Temp(this.kCreditsNames), width);
			Rect position = new Rect(5f, this.m_TextYPos, width, num);
			GUI.Label(position, this.kCreditsNames, EditorStyles.wordWrappedLabel);
			float num2 = EditorStyles.wordWrappedMiniLabel.CalcHeight(GUIContent.Temp("Thanks to Forest 'Yoggy' Johnson, Graham McAllister, David Janik-Jones, Raimund Schumacher, Alan J. Dickins and Emil 'Humus' Persson"), width);
			Rect position2 = new Rect(5f, this.m_TextYPos + num, width, num2);
			GUI.Label(position2, "Thanks to Forest 'Yoggy' Johnson, Graham McAllister, David Janik-Jones, Raimund Schumacher, Alan J. Dickins and Emil 'Humus' Persson", EditorStyles.wordWrappedMiniLabel);
			GUI.EndGroup();
			this.m_TotalCreditsHeight = num + num2;
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
			GUILayout.Label("\n" + InternalEditorUtility.GetUnityCopyright(), "MiniLabel", new GUILayoutOption[0]);
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
		private void ListenForSecretCodes()
		{
			if (Event.current.type != EventType.KeyDown || Event.current.character == '\0')
			{
				return;
			}
			if (this.SecretCodeHasBeenTyped("internal", ref this.m_InternalCodeProgress))
			{
				bool flag = !EditorPrefs.GetBool("InternalMode", false);
				EditorPrefs.SetBool("InternalMode", flag);
				base.ShowNotification(new GUIContent("Internal Mode " + ((!flag) ? "Off" : "On")));
				InternalEditorUtility.RequestScriptReload();
			}
		}
		private bool SecretCodeHasBeenTyped(string code, ref int characterProgress)
		{
			if (characterProgress < 0 || characterProgress >= code.Length || code[characterProgress] != Event.current.character)
			{
				characterProgress = 0;
			}
			if (code[characterProgress] == Event.current.character)
			{
				characterProgress++;
				if (characterProgress >= code.Length)
				{
					characterProgress = 0;
					return true;
				}
			}
			return false;
		}
	}
}
