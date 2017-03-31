using System;
using UnityEditorInternal;
using UnityEngine;

namespace UnityEditor
{
	internal class GradientPicker : EditorWindow
	{
		private static GradientPicker s_GradientPicker;

		private GradientEditor m_GradientEditor;

		private PresetLibraryEditor<GradientPresetLibrary> m_GradientLibraryEditor;

		[SerializeField]
		private PresetLibraryEditorState m_GradientLibraryEditorState;

		private Gradient m_Gradient;

		private const int k_DefaultNumSteps = 0;

		private GUIView m_DelegateView;

		private bool m_HDR;

		public static string presetsEditorPrefID
		{
			get
			{
				return "Gradient";
			}
		}

		private bool gradientChanged
		{
			get;
			set;
		}

		public static GradientPicker instance
		{
			get
			{
				if (!GradientPicker.s_GradientPicker)
				{
					Debug.LogError("Gradient Picker not initalized, did you call Show first?");
				}
				return GradientPicker.s_GradientPicker;
			}
		}

		public string currentPresetLibrary
		{
			get
			{
				this.InitIfNeeded();
				return this.m_GradientLibraryEditor.currentLibraryWithoutExtension;
			}
			set
			{
				this.InitIfNeeded();
				this.m_GradientLibraryEditor.currentLibraryWithoutExtension = value;
			}
		}

		public static bool visible
		{
			get
			{
				return GradientPicker.s_GradientPicker != null;
			}
		}

		public static Gradient gradient
		{
			get
			{
				Gradient result;
				if (GradientPicker.s_GradientPicker != null)
				{
					result = GradientPicker.s_GradientPicker.m_Gradient;
				}
				else
				{
					result = null;
				}
				return result;
			}
		}

		public static void Show(Gradient newGradient, bool hdr)
		{
			GUIView current = GUIView.current;
			if (GradientPicker.s_GradientPicker == null)
			{
				string title = (!hdr) ? "Gradient Editor" : "HDR Gradient Editor";
				GradientPicker.s_GradientPicker = (GradientPicker)EditorWindow.GetWindow(typeof(GradientPicker), true, title, false);
				Vector2 minSize = new Vector2(360f, 224f);
				Vector2 maxSize = new Vector2(1900f, 3000f);
				GradientPicker.s_GradientPicker.minSize = minSize;
				GradientPicker.s_GradientPicker.maxSize = maxSize;
				GradientPicker.s_GradientPicker.wantsMouseMove = true;
				GradientPicker.s_GradientPicker.ShowAuxWindow();
			}
			else
			{
				GradientPicker.s_GradientPicker.Repaint();
			}
			GradientPicker.s_GradientPicker.m_DelegateView = current;
			GradientPicker.s_GradientPicker.Init(newGradient, hdr);
			GradientPreviewCache.ClearCache();
		}

		private void Init(Gradient newGradient, bool hdr)
		{
			this.m_Gradient = newGradient;
			this.m_HDR = hdr;
			if (this.m_GradientEditor != null)
			{
				this.m_GradientEditor.Init(newGradient, 0, this.m_HDR);
			}
			base.Repaint();
		}

		private void SetGradientData(Gradient gradient)
		{
			this.m_Gradient.colorKeys = gradient.colorKeys;
			this.m_Gradient.alphaKeys = gradient.alphaKeys;
			this.m_Gradient.mode = gradient.mode;
			this.Init(this.m_Gradient, this.m_HDR);
		}

		public void OnEnable()
		{
			base.hideFlags = HideFlags.DontSave;
		}

		public void OnDisable()
		{
			if (this.m_GradientLibraryEditorState != null)
			{
				this.m_GradientLibraryEditorState.TransferEditorPrefsState(false);
			}
			GradientPicker.s_GradientPicker = null;
		}

		public void OnDestroy()
		{
			this.m_GradientLibraryEditor.UnloadUsedLibraries();
		}

		private void OnPlayModeStateChanged()
		{
			base.Close();
		}

		private void InitIfNeeded()
		{
			if (this.m_GradientEditor == null)
			{
				this.m_GradientEditor = new GradientEditor();
				this.m_GradientEditor.Init(this.m_Gradient, 0, this.m_HDR);
			}
			if (this.m_GradientLibraryEditorState == null)
			{
				this.m_GradientLibraryEditorState = new PresetLibraryEditorState(GradientPicker.presetsEditorPrefID);
				this.m_GradientLibraryEditorState.TransferEditorPrefsState(true);
			}
			if (this.m_GradientLibraryEditor == null)
			{
				ScriptableObjectSaveLoadHelper<GradientPresetLibrary> helper = new ScriptableObjectSaveLoadHelper<GradientPresetLibrary>("gradients", SaveType.Text);
				this.m_GradientLibraryEditor = new PresetLibraryEditor<GradientPresetLibrary>(helper, this.m_GradientLibraryEditorState, new Action<int, object>(this.PresetClickedCallback));
				this.m_GradientLibraryEditor.showHeader = true;
				this.m_GradientLibraryEditor.minMaxPreviewHeight = new Vector2(14f, 14f);
			}
		}

		private void PresetClickedCallback(int clickCount, object presetObject)
		{
			Gradient gradient = presetObject as Gradient;
			if (gradient == null)
			{
				Debug.LogError("Incorrect object passed " + presetObject);
			}
			GradientPicker.SetCurrentGradient(gradient);
			this.gradientChanged = true;
		}

		public void OnGUI()
		{
			if (this.m_Gradient != null)
			{
				this.InitIfNeeded();
				float num = Mathf.Min(base.position.height, 146f);
				float num2 = 10f;
				float height = base.position.height - num - num2;
				Rect position = new Rect(10f, 10f, base.position.width - 20f, num - 20f);
				Rect rect = new Rect(0f, num + num2, base.position.width, height);
				EditorGUI.DrawRect(new Rect(rect.x, rect.y - 1f, rect.width, 1f), new Color(0f, 0f, 0f, 0.3f));
				EditorGUI.DrawRect(new Rect(rect.x, rect.y, rect.width, 1f), new Color(1f, 1f, 1f, 0.1f));
				EditorGUI.BeginChangeCheck();
				this.m_GradientEditor.OnGUI(position);
				if (EditorGUI.EndChangeCheck())
				{
					this.gradientChanged = true;
				}
				this.m_GradientLibraryEditor.OnGUI(rect, this.m_Gradient);
				if (this.gradientChanged)
				{
					this.gradientChanged = false;
					this.SendEvent(true);
				}
			}
		}

		private void SendEvent(bool exitGUI)
		{
			if (this.m_DelegateView)
			{
				Event e = EditorGUIUtility.CommandEvent("GradientPickerChanged");
				base.Repaint();
				this.m_DelegateView.SendEvent(e);
				if (exitGUI)
				{
					GUIUtility.ExitGUI();
				}
			}
		}

		public static void SetCurrentGradient(Gradient gradient)
		{
			if (!(GradientPicker.s_GradientPicker == null))
			{
				GradientPicker.s_GradientPicker.SetGradientData(gradient);
				GUI.changed = true;
			}
		}

		public static void CloseWindow()
		{
			if (!(GradientPicker.s_GradientPicker == null))
			{
				GradientPicker.s_GradientPicker.Close();
				GUIUtility.ExitGUI();
			}
		}

		public static void RepaintWindow()
		{
			if (!(GradientPicker.s_GradientPicker == null))
			{
				GradientPicker.s_GradientPicker.Repaint();
			}
		}
	}
}
