using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.Internal;

namespace UnityEditor
{
	public class ScriptableWizard : EditorWindow
	{
		private class Styles
		{
			public static string errorText = "Wizard Error";

			public static string box = "Wizard Box";
		}

		private GenericInspector m_Inspector;

		private string m_HelpString = "";

		private string m_ErrorString = "";

		private bool m_IsValid = true;

		private Vector2 m_ScrollPosition;

		private string m_CreateButton = "Create";

		private string m_OtherButton = "";

		public string helpString
		{
			get
			{
				return this.m_HelpString;
			}
			set
			{
				string text = value ?? string.Empty;
				if (this.m_HelpString != text)
				{
					this.m_HelpString = text;
					base.Repaint();
				}
			}
		}

		public string errorString
		{
			get
			{
				return this.m_ErrorString;
			}
			set
			{
				string text = value ?? string.Empty;
				if (this.m_ErrorString != text)
				{
					this.m_ErrorString = text;
					base.Repaint();
				}
			}
		}

		public string createButtonName
		{
			get
			{
				return this.m_CreateButton;
			}
			set
			{
				string text = value ?? string.Empty;
				if (this.m_CreateButton != text)
				{
					this.m_CreateButton = text;
					base.Repaint();
				}
			}
		}

		public string otherButtonName
		{
			get
			{
				return this.m_OtherButton;
			}
			set
			{
				string text = value ?? string.Empty;
				if (this.m_OtherButton != text)
				{
					this.m_OtherButton = text;
					base.Repaint();
				}
			}
		}

		public bool isValid
		{
			get
			{
				return this.m_IsValid;
			}
			set
			{
				this.m_IsValid = value;
			}
		}

		private void OnDestroy()
		{
			UnityEngine.Object.DestroyImmediate(this.m_Inspector);
		}

		private void InvokeWizardUpdate()
		{
			MethodInfo method = base.GetType().GetMethod("OnWizardUpdate", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
			if (method != null)
			{
				method.Invoke(this, null);
			}
		}

		private void OnGUI()
		{
			EditorGUIUtility.labelWidth = 150f;
			GUILayout.Label(this.m_HelpString, EditorStyles.wordWrappedLabel, new GUILayoutOption[]
			{
				GUILayout.ExpandHeight(true)
			});
			this.m_ScrollPosition = EditorGUILayout.BeginVerticalScrollView(this.m_ScrollPosition, false, GUI.skin.verticalScrollbar, "OL Box", new GUILayoutOption[0]);
			GUIUtility.GetControlID(645789, FocusType.Passive);
			bool flag = this.DrawWizardGUI();
			EditorGUILayout.EndScrollView();
			GUILayout.BeginVertical(new GUILayoutOption[0]);
			if (this.m_ErrorString != string.Empty)
			{
				GUILayout.Label(this.m_ErrorString, ScriptableWizard.Styles.errorText, new GUILayoutOption[]
				{
					GUILayout.MinHeight(32f)
				});
			}
			else
			{
				GUILayout.Label(string.Empty, new GUILayoutOption[]
				{
					GUILayout.MinHeight(32f)
				});
			}
			GUILayout.FlexibleSpace();
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.FlexibleSpace();
			GUI.enabled = this.m_IsValid;
			if (this.m_OtherButton != "" && GUILayout.Button(this.m_OtherButton, new GUILayoutOption[]
			{
				GUILayout.MinWidth(100f)
			}))
			{
				MethodInfo method = base.GetType().GetMethod("OnWizardOtherButton", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
				if (method != null)
				{
					method.Invoke(this, null);
					GUIUtility.ExitGUI();
				}
				else
				{
					Debug.LogError("OnWizardOtherButton has not been implemented in script");
				}
			}
			if (this.m_CreateButton != "" && GUILayout.Button(this.m_CreateButton, new GUILayoutOption[]
			{
				GUILayout.MinWidth(100f)
			}))
			{
				MethodInfo method2 = base.GetType().GetMethod("OnWizardCreate", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
				if (method2 != null)
				{
					method2.Invoke(this, null);
				}
				else
				{
					Debug.LogError("OnWizardCreate has not been implemented in script");
				}
				base.Close();
				GUIUtility.ExitGUI();
			}
			GUI.enabled = true;
			GUILayout.EndHorizontal();
			GUILayout.EndVertical();
			if (flag)
			{
				this.InvokeWizardUpdate();
			}
		}

		protected virtual bool DrawWizardGUI()
		{
			if (this.m_Inspector == null)
			{
				this.m_Inspector = ScriptableObject.CreateInstance<GenericInspector>();
				this.m_Inspector.hideFlags = HideFlags.HideAndDontSave;
				this.m_Inspector.InternalSetTargets(new UnityEngine.Object[]
				{
					this
				});
			}
			return this.m_Inspector.DrawDefaultInspector();
		}

		public static T DisplayWizard<T>(string title) where T : ScriptableWizard
		{
			return ScriptableWizard.DisplayWizard<T>(title, "Create", "");
		}

		public static T DisplayWizard<T>(string title, string createButtonName) where T : ScriptableWizard
		{
			return ScriptableWizard.DisplayWizard<T>(title, createButtonName, "");
		}

		public static T DisplayWizard<T>(string title, string createButtonName, string otherButtonName) where T : ScriptableWizard
		{
			return (T)((object)ScriptableWizard.DisplayWizard(title, typeof(T), createButtonName, otherButtonName));
		}

		[ExcludeFromDocs]
		public static ScriptableWizard DisplayWizard(string title, Type klass, string createButtonName)
		{
			string otherButtonName = "";
			return ScriptableWizard.DisplayWizard(title, klass, createButtonName, otherButtonName);
		}

		[ExcludeFromDocs]
		public static ScriptableWizard DisplayWizard(string title, Type klass)
		{
			string otherButtonName = "";
			string createButtonName = "Create";
			return ScriptableWizard.DisplayWizard(title, klass, createButtonName, otherButtonName);
		}

		public static ScriptableWizard DisplayWizard(string title, Type klass, [DefaultValue("\"Create\"")] string createButtonName, [DefaultValue("\"\"")] string otherButtonName)
		{
			ScriptableWizard scriptableWizard = ScriptableObject.CreateInstance(klass) as ScriptableWizard;
			scriptableWizard.m_CreateButton = createButtonName;
			scriptableWizard.m_OtherButton = otherButtonName;
			scriptableWizard.titleContent = new GUIContent(title);
			if (scriptableWizard != null)
			{
				scriptableWizard.InvokeWizardUpdate();
				scriptableWizard.ShowUtility();
			}
			return scriptableWizard;
		}
	}
}
