using System;
using UnityEngine;

namespace UnityEditor
{
	internal class MetroCertificatePasswordWindow : EditorWindow
	{
		private static readonly GUILayoutOption kLabelWidth = GUILayout.Width(110f);

		private static readonly GUILayoutOption kButtonWidth = GUILayout.Width(110f);

		private const float kSpace = 5f;

		private const char kPasswordChar = '●';

		private const string kPasswordId = "password";

		private string path;

		private string password;

		private GUIContent message;

		private GUIStyle messageStyle;

		private string focus;

		public static void Show(string path)
		{
			MetroCertificatePasswordWindow[] array = (MetroCertificatePasswordWindow[])Resources.FindObjectsOfTypeAll(typeof(MetroCertificatePasswordWindow));
			MetroCertificatePasswordWindow metroCertificatePasswordWindow = (array.Length <= 0) ? ScriptableObject.CreateInstance<MetroCertificatePasswordWindow>() : array[0];
			metroCertificatePasswordWindow.path = path;
			metroCertificatePasswordWindow.password = string.Empty;
			metroCertificatePasswordWindow.message = GUIContent.none;
			metroCertificatePasswordWindow.messageStyle = new GUIStyle(GUI.skin.label);
			metroCertificatePasswordWindow.messageStyle.fontStyle = FontStyle.Italic;
			metroCertificatePasswordWindow.focus = "password";
			if (array.Length > 0)
			{
				metroCertificatePasswordWindow.Focus();
			}
			else
			{
				metroCertificatePasswordWindow.titleContent = EditorGUIUtility.TextContent("Enter Windows Store Certificate Password");
				metroCertificatePasswordWindow.position = new Rect(100f, 100f, 350f, 90f);
				metroCertificatePasswordWindow.minSize = new Vector2(metroCertificatePasswordWindow.position.width, metroCertificatePasswordWindow.position.height);
				metroCertificatePasswordWindow.maxSize = metroCertificatePasswordWindow.minSize;
				metroCertificatePasswordWindow.ShowUtility();
			}
		}

		public void OnGUI()
		{
			Event current = Event.current;
			bool flag = false;
			bool flag2 = false;
			if (current.type == EventType.KeyDown)
			{
				flag = (current.keyCode == KeyCode.Escape);
				flag2 = (current.keyCode == KeyCode.Return || current.keyCode == KeyCode.KeypadEnter);
			}
			using (HorizontalLayout.DoLayout())
			{
				GUILayout.Space(10f);
				using (VerticalLayout.DoLayout())
				{
					GUILayout.FlexibleSpace();
					using (HorizontalLayout.DoLayout())
					{
						GUILayout.Label(EditorGUIUtility.TextContent("Password|Certificate password."), new GUILayoutOption[]
						{
							MetroCertificatePasswordWindow.kLabelWidth
						});
						GUI.SetNextControlName("password");
						this.password = GUILayout.PasswordField(this.password, '●', new GUILayoutOption[0]);
					}
					GUILayout.Space(10f);
					using (HorizontalLayout.DoLayout())
					{
						GUILayout.Label(this.message, this.messageStyle, new GUILayoutOption[0]);
						GUILayout.FlexibleSpace();
						if (GUILayout.Button(EditorGUIUtility.TextContent("Ok"), new GUILayoutOption[]
						{
							MetroCertificatePasswordWindow.kButtonWidth
						}) || flag2)
						{
							this.message = GUIContent.none;
							try
							{
								if (PlayerSettings.WSA.SetCertificate(this.path, this.password))
								{
									flag = true;
								}
								else
								{
									this.message = EditorGUIUtility.TextContent("Invalid password.");
								}
							}
							catch (UnityException ex)
							{
								Debug.LogError(ex.Message);
							}
						}
					}
					GUILayout.FlexibleSpace();
				}
				GUILayout.Space(10f);
			}
			if (flag)
			{
				base.Close();
			}
			else if (this.focus != null)
			{
				EditorGUI.FocusTextInControl(this.focus);
				this.focus = null;
			}
		}
	}
}
