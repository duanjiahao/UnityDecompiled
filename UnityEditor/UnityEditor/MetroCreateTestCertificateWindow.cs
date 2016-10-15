using System;
using System.IO;
using UnityEngine;

namespace UnityEditor
{
	internal class MetroCreateTestCertificateWindow : EditorWindow
	{
		private const float kSpace = 5f;

		private const char kPasswordChar = '●';

		private const string kPublisherId = "publisher";

		private const string kPasswordId = "password";

		private const string kConfirmId = "confirm";

		private static readonly GUILayoutOption kLabelWidth = GUILayout.Width(110f);

		private static readonly GUILayoutOption kButtonWidth = GUILayout.Width(110f);

		private string path;

		private string publisher;

		private string password;

		private string confirm;

		private GUIContent message;

		private GUIStyle messageStyle;

		private string focus;

		public static void Show(string publisher)
		{
			MetroCreateTestCertificateWindow[] array = (MetroCreateTestCertificateWindow[])Resources.FindObjectsOfTypeAll(typeof(MetroCreateTestCertificateWindow));
			MetroCreateTestCertificateWindow metroCreateTestCertificateWindow = (array.Length <= 0) ? ScriptableObject.CreateInstance<MetroCreateTestCertificateWindow>() : array[0];
			metroCreateTestCertificateWindow.path = Path.Combine(Application.dataPath, "WSATestCertificate.pfx").Replace('\\', '/');
			metroCreateTestCertificateWindow.publisher = publisher;
			metroCreateTestCertificateWindow.password = string.Empty;
			metroCreateTestCertificateWindow.confirm = metroCreateTestCertificateWindow.password;
			metroCreateTestCertificateWindow.message = ((!File.Exists(metroCreateTestCertificateWindow.path)) ? GUIContent.none : EditorGUIUtility.TextContent("Current file will be overwritten."));
			metroCreateTestCertificateWindow.messageStyle = new GUIStyle(GUI.skin.label);
			metroCreateTestCertificateWindow.messageStyle.fontStyle = FontStyle.Italic;
			metroCreateTestCertificateWindow.focus = "publisher";
			if (array.Length > 0)
			{
				metroCreateTestCertificateWindow.Focus();
			}
			else
			{
				metroCreateTestCertificateWindow.titleContent = EditorGUIUtility.TextContent("Create Test Certificate for Windows Store");
				metroCreateTestCertificateWindow.position = new Rect(100f, 100f, 350f, 140f);
				metroCreateTestCertificateWindow.minSize = new Vector2(metroCreateTestCertificateWindow.position.width, metroCreateTestCertificateWindow.position.height);
				metroCreateTestCertificateWindow.maxSize = metroCreateTestCertificateWindow.minSize;
				metroCreateTestCertificateWindow.ShowUtility();
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
						GUILayout.Label(EditorGUIUtility.TextContent("Publisher|Publisher of the package."), new GUILayoutOption[]
						{
							MetroCreateTestCertificateWindow.kLabelWidth
						});
						GUI.SetNextControlName("publisher");
						this.publisher = GUILayout.TextField(this.publisher, new GUILayoutOption[0]);
					}
					GUILayout.Space(5f);
					using (HorizontalLayout.DoLayout())
					{
						GUILayout.Label(EditorGUIUtility.TextContent("Password|Certificate password."), new GUILayoutOption[]
						{
							MetroCreateTestCertificateWindow.kLabelWidth
						});
						GUI.SetNextControlName("password");
						this.password = GUILayout.PasswordField(this.password, '●', new GUILayoutOption[0]);
					}
					GUILayout.Space(5f);
					using (HorizontalLayout.DoLayout())
					{
						GUILayout.Label(EditorGUIUtility.TextContent("Confirm password|Re-enter certificate password."), new GUILayoutOption[]
						{
							MetroCreateTestCertificateWindow.kLabelWidth
						});
						GUI.SetNextControlName("confirm");
						this.confirm = GUILayout.PasswordField(this.confirm, '●', new GUILayoutOption[0]);
					}
					GUILayout.Space(10f);
					using (HorizontalLayout.DoLayout())
					{
						GUILayout.Label(this.message, this.messageStyle, new GUILayoutOption[0]);
						GUILayout.FlexibleSpace();
						if (GUILayout.Button(EditorGUIUtility.TextContent("Create"), new GUILayoutOption[]
						{
							MetroCreateTestCertificateWindow.kButtonWidth
						}) || flag2)
						{
							this.message = GUIContent.none;
							if (string.IsNullOrEmpty(this.publisher))
							{
								this.message = EditorGUIUtility.TextContent("Publisher must be specified.");
								this.focus = "publisher";
							}
							else if (this.password != this.confirm)
							{
								if (string.IsNullOrEmpty(this.confirm))
								{
									this.message = EditorGUIUtility.TextContent("Confirm the password.");
									this.focus = "confirm";
								}
								else
								{
									this.message = EditorGUIUtility.TextContent("Passwords do not match.");
									this.password = string.Empty;
									this.confirm = this.password;
									this.focus = "password";
								}
							}
							else
							{
								try
								{
									EditorUtility.WSACreateTestCertificate(this.path, this.publisher, this.password, true);
									AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
									if (!PlayerSettings.WSA.SetCertificate(FileUtil.GetProjectRelativePath(this.path), this.password))
									{
										this.message = EditorGUIUtility.TextContent("Invalid password.");
									}
									flag = true;
								}
								catch (UnityException ex)
								{
									Debug.LogError(ex.Message);
								}
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
