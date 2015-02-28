using System;
using System.Collections;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Internal;
using UnityEngineInternal;
namespace UnityEditor
{
	public sealed class EditorGUIUtility : GUIUtility
	{
		internal static int s_FontIsBold;
		private static Texture2D s_InfoIcon;
		private static Texture2D s_WarningIcon;
		private static Texture2D s_ErrorIcon;
		internal static SliderLabels sliderLabels;
		internal static Color kDarkViewBackground;
		private static GUIContent s_ObjectContent;
		private static GUIContent s_Text;
		private static GUIContent s_Image;
		private static GUIContent s_TextImage;
		private static GUIContent s_BlankContent;
		private static GUIStyle s_WhiteTextureStyle;
		private static GUIStyle s_BasicTextureStyle;
		private static Hashtable s_TextGUIContents;
		private static Hashtable s_IconGUIContents;
		private static Hashtable s_ScriptInfos;
		internal static int s_LastControlID;
		private static bool s_HierarchyMode;
		internal static bool s_WideMode;
		private static float s_ContextWidth;
		private static float s_LabelWidth;
		private static float s_FieldWidth;
		public static FocusType native;
		public static float singleLineHeight
		{
			get
			{
				return 16f;
			}
		}
		public static float standardVerticalSpacing
		{
			get
			{
				return 2f;
			}
		}
		public static bool isProSkin
		{
			get
			{
				return EditorGUIUtility.skinIndex == 1;
			}
		}
		internal static extern int skinIndex
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
		public static extern Texture2D whiteTexture
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		internal static Texture2D infoIcon
		{
			get
			{
				if (EditorGUIUtility.s_InfoIcon == null)
				{
					EditorGUIUtility.s_InfoIcon = EditorGUIUtility.LoadIcon("console.infoicon");
				}
				return EditorGUIUtility.s_InfoIcon;
			}
		}
		internal static Texture2D warningIcon
		{
			get
			{
				if (EditorGUIUtility.s_WarningIcon == null)
				{
					EditorGUIUtility.s_WarningIcon = EditorGUIUtility.LoadIcon("console.warnicon");
				}
				return EditorGUIUtility.s_WarningIcon;
			}
		}
		internal static Texture2D errorIcon
		{
			get
			{
				if (EditorGUIUtility.s_ErrorIcon == null)
				{
					EditorGUIUtility.s_ErrorIcon = EditorGUIUtility.LoadIcon("console.erroricon");
				}
				return EditorGUIUtility.s_ErrorIcon;
			}
		}
		internal static GUIContent blankContent
		{
			get
			{
				return EditorGUIUtility.s_BlankContent;
			}
		}
		internal static GUIStyle whiteTextureStyle
		{
			get
			{
				if (EditorGUIUtility.s_WhiteTextureStyle == null)
				{
					EditorGUIUtility.s_WhiteTextureStyle = new GUIStyle();
					EditorGUIUtility.s_WhiteTextureStyle.normal.background = EditorGUIUtility.whiteTexture;
				}
				return EditorGUIUtility.s_WhiteTextureStyle;
			}
		}
		public static bool editingTextField
		{
			get
			{
				return EditorGUI.RecycledTextEditor.s_ActuallyEditing;
			}
			set
			{
				EditorGUI.RecycledTextEditor.s_ActuallyEditing = value;
			}
		}
		public static bool hierarchyMode
		{
			get
			{
				return EditorGUIUtility.s_HierarchyMode;
			}
			set
			{
				EditorGUIUtility.s_HierarchyMode = value;
			}
		}
		public static bool wideMode
		{
			get
			{
				return EditorGUIUtility.s_WideMode;
			}
			set
			{
				EditorGUIUtility.s_WideMode = value;
			}
		}
		internal static float contextWidth
		{
			get
			{
				if (EditorGUIUtility.s_ContextWidth > 0f)
				{
					return EditorGUIUtility.s_ContextWidth;
				}
				return EditorGUIUtility.CalcContextWidth();
			}
		}
		public static float currentViewWidth
		{
			get
			{
				return GUIView.current.position.width;
			}
		}
		public static float labelWidth
		{
			get
			{
				if (EditorGUIUtility.s_LabelWidth > 0f)
				{
					return EditorGUIUtility.s_LabelWidth;
				}
				if (EditorGUIUtility.s_HierarchyMode)
				{
					return Mathf.Max(EditorGUIUtility.contextWidth * 0.45f - 40f, 120f);
				}
				return 150f;
			}
			set
			{
				EditorGUIUtility.s_LabelWidth = value;
			}
		}
		public static float fieldWidth
		{
			get
			{
				if (EditorGUIUtility.s_FieldWidth > 0f)
				{
					return EditorGUIUtility.s_FieldWidth;
				}
				return 50f;
			}
			set
			{
				EditorGUIUtility.s_FieldWidth = value;
			}
		}
		public static extern string systemCopyBuffer
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
		internal static EventType magnifyGestureEventType
		{
			get
			{
				return (EventType)1000;
			}
		}
		internal static EventType swipeGestureEventType
		{
			get
			{
				return (EventType)1001;
			}
		}
		internal static EventType rotateGestureEventType
		{
			get
			{
				return (EventType)1002;
			}
		}
		static EditorGUIUtility()
		{
			EditorGUIUtility.s_FontIsBold = -1;
			EditorGUIUtility.sliderLabels = default(SliderLabels);
			EditorGUIUtility.kDarkViewBackground = new Color(0.22f, 0.22f, 0.22f, 0f);
			EditorGUIUtility.s_ObjectContent = new GUIContent();
			EditorGUIUtility.s_Text = new GUIContent();
			EditorGUIUtility.s_Image = new GUIContent();
			EditorGUIUtility.s_TextImage = new GUIContent();
			EditorGUIUtility.s_BlankContent = new GUIContent(" ");
			EditorGUIUtility.s_TextGUIContents = new Hashtable();
			EditorGUIUtility.s_IconGUIContents = new Hashtable();
			EditorGUIUtility.s_ScriptInfos = null;
			EditorGUIUtility.s_LastControlID = 0;
			EditorGUIUtility.s_HierarchyMode = false;
			EditorGUIUtility.s_WideMode = false;
			EditorGUIUtility.s_ContextWidth = 0f;
			EditorGUIUtility.s_LabelWidth = 0f;
			EditorGUIUtility.s_FieldWidth = 0f;
			EditorGUIUtility.native = FocusType.Keyboard;
			GUISkin.m_SkinChanged = (GUISkin.SkinChangedDelegate)Delegate.Combine(GUISkin.m_SkinChanged, new GUISkin.SkinChangedDelegate(EditorGUIUtility.SkinChanged));
		}
		internal static GUIContent TextContent(string name)
		{
			if (name == null)
			{
				name = string.Empty;
			}
			GUIContent gUIContent = (GUIContent)EditorGUIUtility.s_TextGUIContents[name];
			if (gUIContent == null)
			{
				if (EditorGUIUtility.s_ScriptInfos == null)
				{
					EditorGUIUtility.LoadScriptInfos();
				}
				gUIContent = (GUIContent)EditorGUIUtility.s_ScriptInfos[name];
				if (gUIContent == null)
				{
					gUIContent = new GUIContent(name);
				}
				gUIContent.image = EditorGUIUtility.LoadIconForSkin(name, EditorGUIUtility.skinIndex);
				EditorGUIUtility.s_TextGUIContents[name] = gUIContent;
			}
			return gUIContent;
		}
		internal static GUIContent[] GetTextContentsForEnum(Type type)
		{
			Array values = Enum.GetValues(type);
			string[] names = Enum.GetNames(type);
			int num = 0;
			foreach (int num2 in values)
			{
				if (num2 < 0)
				{
					Debug.LogError("Enum may not be smaller than zero");
					GUIContent[] result = null;
					return result;
				}
				if (num2 > 512)
				{
					Debug.LogError("Largest value in enum may not be larger than 512");
					GUIContent[] result = null;
					return result;
				}
				num = ((num2 <= num) ? num : num2);
			}
			GUIContent[] array = new GUIContent[num + 1];
			for (int i = 0; i < names.Length; i++)
			{
				int num3 = (int)values.GetValue(i);
				string text = type.Name + "." + names[i];
				array[num3] = EditorGUIUtility.TextContent(text);
				if (text == array[num3].text)
				{
					Debug.LogError("enum name is not found in localization file: " + text);
				}
			}
			return array;
		}
		public static GUIContent IconContent(string name)
		{
			GUIContent gUIContent = (GUIContent)EditorGUIUtility.s_IconGUIContents[name];
			if (gUIContent == null)
			{
				if (EditorGUIUtility.s_ScriptInfos == null)
				{
					EditorGUIUtility.LoadScriptInfos();
				}
				GUIContent gUIContent2 = (GUIContent)EditorGUIUtility.s_ScriptInfos[name];
				gUIContent = new GUIContent();
				if (gUIContent2 != null)
				{
					gUIContent.tooltip = gUIContent2.tooltip;
				}
				gUIContent.image = EditorGUIUtility.LoadIconRequired(name);
				EditorGUIUtility.s_IconGUIContents[name] = gUIContent;
			}
			return gUIContent;
		}
		internal static Texture2D LoadIconRequired(string name)
		{
			Texture2D texture2D = EditorGUIUtility.LoadIcon(name);
			if (!texture2D)
			{
				Debug.LogError(string.Concat(new string[]
				{
					"Unable to load '",
					EditorResourcesUtility.iconsPath,
					name,
					"' nor '",
					EditorResourcesUtility.generatedIconsPath,
					name,
					"'"
				}));
			}
			return texture2D;
		}
		internal static Texture2D LoadIcon(string name)
		{
			return EditorGUIUtility.LoadIconForSkin(name, EditorGUIUtility.skinIndex);
		}
		private static Texture2D LoadGeneratedIconOrNormalIcon(string name)
		{
			Texture2D texture2D = EditorGUIUtility.Load(EditorResourcesUtility.generatedIconsPath + name + ".asset") as Texture2D;
			if (!texture2D)
			{
				texture2D = (EditorGUIUtility.Load(EditorResourcesUtility.iconsPath + name + ".png") as Texture2D);
			}
			return texture2D;
		}
		internal static Texture2D LoadIconForSkin(string name, int skinIndex)
		{
			if (string.IsNullOrEmpty(name))
			{
				return null;
			}
			if (skinIndex == 0)
			{
				return EditorGUIUtility.LoadGeneratedIconOrNormalIcon(name);
			}
			string text = "d_" + Path.GetFileName(name);
			string directoryName = Path.GetDirectoryName(name);
			if (!string.IsNullOrEmpty(directoryName))
			{
				text = string.Format("{0}/{1}", directoryName, text);
			}
			Texture2D texture2D = EditorGUIUtility.LoadGeneratedIconOrNormalIcon(text);
			if (!texture2D)
			{
				texture2D = EditorGUIUtility.LoadGeneratedIconOrNormalIcon(name);
			}
			return texture2D;
		}
		public static GUIContent IconContent(string name, string tooltip)
		{
			GUIContent gUIContent = EditorGUIUtility.IconContent(name);
			gUIContent.tooltip = tooltip;
			return gUIContent;
		}
		internal static void Internal_SwitchSkin()
		{
			EditorGUIUtility.skinIndex = 1 - EditorGUIUtility.skinIndex;
		}
		public static GUIContent ObjectContent(UnityEngine.Object obj, Type type)
		{
			if (obj)
			{
				if (obj is AudioMixerGroup)
				{
					EditorGUIUtility.s_ObjectContent.text = obj.name + " (" + ((AudioMixerGroup)obj).audioMixer.name + ")";
				}
				else
				{
					if (obj is AudioMixerSnapshot)
					{
						EditorGUIUtility.s_ObjectContent.text = obj.name + " (" + ((AudioMixerSnapshot)obj).audioMixer.name + ")";
					}
					else
					{
						EditorGUIUtility.s_ObjectContent.text = obj.name;
					}
				}
				EditorGUIUtility.s_ObjectContent.image = AssetPreview.GetMiniThumbnail(obj);
			}
			else
			{
				string arg;
				if (type == null)
				{
					arg = "<no type>";
				}
				else
				{
					if (type.Namespace != null)
					{
						arg = type.ToString().Substring(type.Namespace.ToString().Length + 1);
					}
					else
					{
						arg = type.ToString();
					}
				}
				EditorGUIUtility.s_ObjectContent.text = string.Format("None ({0})", arg);
				EditorGUIUtility.s_ObjectContent.image = AssetPreview.GetMiniTypeThumbnail(type);
			}
			return EditorGUIUtility.s_ObjectContent;
		}
		internal static GUIContent TempContent(string t)
		{
			EditorGUIUtility.s_Text.text = t;
			return EditorGUIUtility.s_Text;
		}
		internal static GUIContent TempContent(Texture i)
		{
			EditorGUIUtility.s_Image.image = i;
			return EditorGUIUtility.s_Image;
		}
		internal static GUIContent TempContent(string t, Texture i)
		{
			EditorGUIUtility.s_TextImage.image = i;
			EditorGUIUtility.s_TextImage.text = t;
			return EditorGUIUtility.s_TextImage;
		}
		internal static GUIContent[] TempContent(string[] texts)
		{
			GUIContent[] array = new GUIContent[texts.Length];
			for (int i = 0; i < texts.Length; i++)
			{
				array[i] = new GUIContent(texts[i]);
			}
			return array;
		}
		internal static bool HasHolddownKeyModifiers(Event evt)
		{
			return evt.shift | evt.control | evt.alt | evt.command;
		}
		public static bool HasObjectThumbnail(Type objType)
		{
			return objType != null && (objType.IsSubclassOf(typeof(Texture)) || objType == typeof(Texture));
		}
		public static void SetIconSize(Vector2 size)
		{
			EditorGUIUtility.INTERNAL_CALL_SetIconSize(ref size);
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_SetIconSize(ref Vector2 size);
		public static Vector2 GetIconSize()
		{
			Vector2 result;
			EditorGUIUtility.Internal_GetIconSize(out result);
			return result;
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_GetIconSize(out Vector2 size);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern UnityEngine.Object GetScript(string scriptClass);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetIconForObject(UnityEngine.Object obj, Texture2D icon);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern Texture2D GetIconForObject(UnityEngine.Object obj);
		internal static Texture2D GetHelpIcon(MessageType type)
		{
			switch (type)
			{
			case MessageType.Info:
				return EditorGUIUtility.infoIcon;
			case MessageType.Warning:
				return EditorGUIUtility.warningIcon;
			case MessageType.Error:
				return EditorGUIUtility.errorIcon;
			default:
				return null;
			}
		}
		internal static GUIStyle GetBasicTextureStyle(Texture2D tex)
		{
			if (EditorGUIUtility.s_BasicTextureStyle == null)
			{
				EditorGUIUtility.s_BasicTextureStyle = new GUIStyle();
			}
			EditorGUIUtility.s_BasicTextureStyle.normal.background = tex;
			return EditorGUIUtility.s_BasicTextureStyle;
		}
		private static void LoadScriptInfos()
		{
			string text = File.ReadAllText(EditorApplication.applicationContentsPath + "/Resources/UI_Strings_EN.txt");
			EditorGUIUtility.s_ScriptInfos = new Hashtable();
			char[] separator = new char[]
			{
				':',
				'|'
			};
			string[] array = text.Split(new char[]
			{
				'\n'
			});
			for (int i = 0; i < array.Length; i++)
			{
				string text2 = array[i];
				if (!text2.StartsWith("//"))
				{
					string[] array2 = text2.Split(separator);
					switch (array2.Length)
					{
					case 0:
					case 1:
						break;
					case 2:
						EditorGUIUtility.s_ScriptInfos[array2[0]] = new GUIContent(array2[1].Trim().Replace("\\n", "\n"));
						break;
					case 3:
						EditorGUIUtility.s_ScriptInfos[array2[0]] = new GUIContent(array2[1].Trim().Replace("\\n", "\n"), array2[2].Trim().Replace("\\n", "\n"));
						break;
					default:
						Debug.LogError("Error in Tooltips: Too many strings in line beginning with '" + array2[0] + "'");
						break;
					}
				}
			}
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern Texture2D FindTexture(string name);
		public static GUISkin GetBuiltinSkin(EditorSkin skin)
		{
			return GUIUtility.GetBuiltinSkin((int)skin);
		}
		public static UnityEngine.Object LoadRequired(string path)
		{
			UnityEngine.Object @object = EditorGUIUtility.Load(path, typeof(UnityEngine.Object));
			if (!@object)
			{
				Debug.LogError("Unable to find required resource at 'Editor Default Resources/" + path + "'");
			}
			return @object;
		}
		public static UnityEngine.Object Load(string path)
		{
			return EditorGUIUtility.Load(path, typeof(UnityEngine.Object));
		}
		[TypeInferenceRule(TypeInferenceRules.TypeReferencedBySecondArgument)]
		private static UnityEngine.Object Load(string filename, Type type)
		{
			UnityEngine.Object @object = AssetDatabase.LoadAssetAtPath("Assets/Editor Default Resources/" + filename, type);
			if (@object != null)
			{
				return @object;
			}
			return EditorGUIUtility.GetEditorAssetBundle().LoadAsset(filename, type);
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern UnityEngine.Object GetBuiltinExtraResource(Type type, string path);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern BuiltinResource[] GetBuiltinResourceList(int classID);
		public static void PingObject(UnityEngine.Object obj)
		{
			if (obj != null)
			{
				EditorGUIUtility.PingObject(obj.GetInstanceID());
			}
		}
		public static void PingObject(int targetInstanceID)
		{
			foreach (SceneHierarchyWindow current in SceneHierarchyWindow.GetAllSceneHierarchyWindows())
			{
				bool ping = true;
				current.FrameObject(targetInstanceID, ping);
			}
			foreach (ProjectBrowser current2 in ProjectBrowser.GetAllProjectBrowsers())
			{
				bool ping2 = true;
				current2.FrameObject(targetInstanceID, ping2);
			}
		}
		internal static void MoveFocusAndScroll(bool forward)
		{
			int keyboardControl = GUIUtility.keyboardControl;
			EditorGUIUtility.Internal_MoveKeyboardFocus(forward);
			if (keyboardControl != GUIUtility.keyboardControl)
			{
				EditorGUIUtility.RefreshScrollPosition();
			}
		}
		internal static void RefreshScrollPosition()
		{
			GUI.ScrollViewState topScrollView = GUI.GetTopScrollView();
			Rect position;
			if (topScrollView != null && EditorGUIUtility.Internal_GetKeyboardRect(GUIUtility.keyboardControl, out position))
			{
				topScrollView.ScrollTo(position);
			}
		}
		internal static void ScrollForTabbing(bool forward)
		{
			GUI.ScrollViewState topScrollView = GUI.GetTopScrollView();
			Rect position;
			if (topScrollView != null && EditorGUIUtility.Internal_GetKeyboardRect(EditorGUIUtility.Internal_GetNextKeyboardControlID(forward), out position))
			{
				topScrollView.ScrollTo(position);
			}
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool Internal_GetKeyboardRect(int id, out Rect rect);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_MoveKeyboardFocus(bool forward);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int Internal_GetNextKeyboardControlID(bool forward);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern AssetBundle GetEditorAssetBundle();
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetRenderTextureNoViewport(RenderTexture rt);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetVisibleLayers(int layers);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetLockedLayers(int layers);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool IsGizmosAllowedForObject(UnityEngine.Object obj);
		internal static void ResetGUIState()
		{
			GUI.skin = null;
			Color white = Color.white;
			GUI.contentColor = white;
			GUI.backgroundColor = white;
			GUI.color = ((!EditorApplication.isPlayingOrWillChangePlaymode) ? Color.white : HostView.kPlayModeDarken);
			GUI.enabled = true;
			GUI.changed = false;
			EditorGUI.indentLevel = 0;
			EditorGUI.ClearStacks();
			EditorGUIUtility.fieldWidth = 0f;
			EditorGUIUtility.labelWidth = 0f;
			EditorGUIUtility.SetBoldDefaultFont(false);
			EditorGUIUtility.UnlockContextWidth();
			EditorGUIUtility.hierarchyMode = false;
			EditorGUIUtility.wideMode = false;
			ScriptAttributeUtility.propertyHandlerCache = null;
		}
		public static void RenderGameViewCameras(Rect cameraRect, int targetDisplay, bool gizmos, bool gui)
		{
			EditorGUIUtility.INTERNAL_CALL_RenderGameViewCameras(ref cameraRect, targetDisplay, gizmos, gui);
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_RenderGameViewCameras(ref Rect cameraRect, int targetDisplay, bool gizmos, bool gui);
		public static void RenderGameViewCameras(Rect cameraRect, bool gizmos, bool gui)
		{
			EditorGUIUtility.RenderGameViewCameras(cameraRect, 0, gizmos, gui);
		}
		[Obsolete("Use version without the statsRect (it is not used anymore)")]
		public static void RenderGameViewCameras(Rect cameraRect, Rect statsRect, bool gizmos, bool gui)
		{
			EditorGUIUtility.RenderGameViewCameras(cameraRect, 0, gizmos, gui);
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void QueueGameViewInputEvent(Event evt);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetDefaultFont(Font font);
		private static GUIStyle GetStyle(string styleName)
		{
			GUIStyle gUIStyle = GUI.skin.FindStyle(styleName);
			if (gUIStyle == null)
			{
				gUIStyle = EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector).FindStyle(styleName);
			}
			if (gUIStyle == null)
			{
				Debug.Log("Missing built-in guistyle " + styleName);
				gUIStyle = GUISkin.error;
			}
			return gUIStyle;
		}
		internal static void HandleControlID(int id)
		{
			EditorGUIUtility.s_LastControlID = id;
			if (EditorGUI.s_PrefixLabel.text != null)
			{
				EditorGUI.HandlePrefixLabel(EditorGUI.s_PrefixTotalRect, EditorGUI.s_PrefixRect, EditorGUI.s_PrefixLabel, EditorGUIUtility.s_LastControlID, EditorGUI.s_PrefixStyle);
			}
		}
		private static float CalcContextWidth()
		{
			float num = GUIClip.GetTopRect().width;
			if (num < 1f || num >= 40000f)
			{
				num = EditorGUIUtility.currentViewWidth;
			}
			return num;
		}
		internal static void LockContextWidth()
		{
			EditorGUIUtility.s_ContextWidth = EditorGUIUtility.CalcContextWidth();
		}
		internal static void UnlockContextWidth()
		{
			EditorGUIUtility.s_ContextWidth = 0f;
		}
		[Obsolete("LookLikeControls and LookLikeInspector modes are deprecated. Use EditorGUIUtility.labelWidth and EditorGUIUtility.fieldWidth to control label and field widths."), ExcludeFromDocs]
		public static void LookLikeControls(float labelWidth)
		{
			float fieldWidth = 0f;
			EditorGUIUtility.LookLikeControls(labelWidth, fieldWidth);
		}
		[ExcludeFromDocs]
		public static void LookLikeControls()
		{
			float fieldWidth = 0f;
			float labelWidth = 0f;
			EditorGUIUtility.LookLikeControls(labelWidth, fieldWidth);
		}
		public static void LookLikeControls([DefaultValue("0")] float labelWidth, [DefaultValue("0")] float fieldWidth)
		{
			EditorGUIUtility.fieldWidth = fieldWidth;
			EditorGUIUtility.labelWidth = labelWidth;
		}
		[Obsolete("LookLikeControls and LookLikeInspector modes are deprecated.")]
		public static void LookLikeInspector()
		{
			EditorGUIUtility.fieldWidth = 0f;
			EditorGUIUtility.labelWidth = 0f;
		}
		internal static void SkinChanged()
		{
			EditorStyles.UpdateSkinCache();
		}
		internal static Rect DragZoneRect(Rect position)
		{
			return new Rect(position.x, position.y, EditorGUIUtility.labelWidth, position.height);
		}
		internal static void SetBoldDefaultFont(bool isBold)
		{
			int num = (!isBold) ? 0 : 1;
			if (num != EditorGUIUtility.s_FontIsBold)
			{
				EditorGUIUtility.SetDefaultFont((!isBold) ? EditorStyles.standardFont : EditorStyles.boldFont);
				EditorGUIUtility.s_FontIsBold = num;
			}
		}
		internal static bool GetBoldDefaultFont()
		{
			return EditorGUIUtility.s_FontIsBold == 1;
		}
		public static Event CommandEvent(string commandName)
		{
			Event @event = new Event();
			EditorGUIUtility.Internal_SetupEventValues(@event);
			@event.type = EventType.ExecuteCommand;
			@event.commandName = commandName;
			return @event;
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_SetupEventValues(object evt);
		public static void DrawColorSwatch(Rect position, Color color)
		{
			EditorGUIUtility.DrawColorSwatch(position, color, true);
		}
		internal static void DrawColorSwatch(Rect position, Color color, bool showAlpha)
		{
			if (Event.current.type != EventType.Repaint)
			{
				return;
			}
			Color color2 = GUI.color;
			float a = (float)((!GUI.enabled) ? 2 : 1);
			GUI.color = ((!EditorGUI.showMixedValue) ? new Color(color.r, color.g, color.b, a) : (new Color(0.82f, 0.82f, 0.82f, a) * color2));
			GUIStyle whiteTextureStyle = EditorGUIUtility.whiteTextureStyle;
			whiteTextureStyle.Draw(position, false, false, false, false);
			if (!EditorGUI.showMixedValue)
			{
				if (showAlpha)
				{
					GUI.color = new Color(0f, 0f, 0f, a);
					float num = Mathf.Clamp(position.height * 0.2f, 2f, 20f);
					position.yMin = position.yMax - num;
					whiteTextureStyle.Draw(position, false, false, false, false);
					GUI.color = new Color(1f, 1f, 1f, a);
					position.width *= Mathf.Clamp01(color.a);
					whiteTextureStyle.Draw(position, false, false, false, false);
				}
			}
			else
			{
				EditorGUI.BeginHandleMixedValueContentColor();
				whiteTextureStyle.Draw(position, EditorGUI.mixedValueContent, false, false, false, false);
				EditorGUI.EndHandleMixedValueContentColor();
			}
			GUI.color = color2;
		}
		public static void DrawCurveSwatch(Rect position, AnimationCurve curve, SerializedProperty property, Color color, Color bgColor)
		{
			EditorGUIUtility.DrawCurveSwatchInternal(position, curve, null, property, null, color, bgColor, false, default(Rect));
		}
		public static void DrawCurveSwatch(Rect position, AnimationCurve curve, SerializedProperty property, Color color, Color bgColor, Rect curveRanges)
		{
			EditorGUIUtility.DrawCurveSwatchInternal(position, curve, null, property, null, color, bgColor, true, curveRanges);
		}
		public static void DrawRegionSwatch(Rect position, SerializedProperty property, SerializedProperty property2, Color color, Color bgColor, Rect curveRanges)
		{
			EditorGUIUtility.DrawCurveSwatchInternal(position, null, null, property, property2, color, bgColor, true, curveRanges);
		}
		public static void DrawRegionSwatch(Rect position, AnimationCurve curve, AnimationCurve curve2, Color color, Color bgColor, Rect curveRanges)
		{
			EditorGUIUtility.DrawCurveSwatchInternal(position, curve, curve2, null, null, color, bgColor, true, curveRanges);
		}
		private static void DrawCurveSwatchInternal(Rect position, AnimationCurve curve, AnimationCurve curve2, SerializedProperty property, SerializedProperty property2, Color color, Color bgColor, bool useCurveRanges, Rect curveRanges)
		{
			if (Event.current.type != EventType.Repaint)
			{
				return;
			}
			int previewWidth = (int)position.width;
			int previewHeight = (int)position.height;
			Color color2 = GUI.color;
			GUI.color = bgColor;
			GUIStyle gUIStyle = EditorGUIUtility.whiteTextureStyle;
			gUIStyle.Draw(position, false, false, false, false);
			GUI.color = color2;
			if (property != null && property.hasMultipleDifferentValues)
			{
				EditorGUI.BeginHandleMixedValueContentColor();
				GUI.Label(position, EditorGUI.mixedValueContent, "PreOverlayLabel");
				EditorGUI.EndHandleMixedValueContentColor();
			}
			else
			{
				Texture2D texture2D = null;
				if (property != null)
				{
					if (property2 == null)
					{
						texture2D = ((!useCurveRanges) ? AnimationCurvePreviewCache.GetPreview(previewWidth, previewHeight, property, color) : AnimationCurvePreviewCache.GetPreview(previewWidth, previewHeight, property, color, curveRanges));
					}
					else
					{
						texture2D = ((!useCurveRanges) ? AnimationCurvePreviewCache.GetPreview(previewWidth, previewHeight, property, property2, color) : AnimationCurvePreviewCache.GetPreview(previewWidth, previewHeight, property, property2, color, curveRanges));
					}
				}
				else
				{
					if (curve != null)
					{
						if (curve2 == null)
						{
							texture2D = ((!useCurveRanges) ? AnimationCurvePreviewCache.GetPreview(previewWidth, previewHeight, curve, color) : AnimationCurvePreviewCache.GetPreview(previewWidth, previewHeight, curve, color, curveRanges));
						}
						else
						{
							texture2D = ((!useCurveRanges) ? AnimationCurvePreviewCache.GetPreview(previewWidth, previewHeight, curve, curve2, color) : AnimationCurvePreviewCache.GetPreview(previewWidth, previewHeight, curve, curve2, color, curveRanges));
						}
					}
				}
				gUIStyle = EditorGUIUtility.GetBasicTextureStyle(texture2D);
				position.width = (float)texture2D.width;
				position.height = (float)texture2D.height;
				gUIStyle.Draw(position, false, false, false, false);
			}
		}
		public static void RGBToHSV(Color rgbColor, out float H, out float S, out float V)
		{
			if (rgbColor.b > rgbColor.g && rgbColor.b > rgbColor.r)
			{
				EditorGUIUtility.RGBToHSVHelper(4f, rgbColor.b, rgbColor.r, rgbColor.g, out H, out S, out V);
			}
			else
			{
				if (rgbColor.g > rgbColor.r)
				{
					EditorGUIUtility.RGBToHSVHelper(2f, rgbColor.g, rgbColor.b, rgbColor.r, out H, out S, out V);
				}
				else
				{
					EditorGUIUtility.RGBToHSVHelper(0f, rgbColor.r, rgbColor.g, rgbColor.b, out H, out S, out V);
				}
			}
		}
		private static void RGBToHSVHelper(float offset, float dominantcolor, float colorone, float colortwo, out float H, out float S, out float V)
		{
			V = dominantcolor;
			if (V != 0f)
			{
				float num;
				if (colorone > colortwo)
				{
					num = colortwo;
				}
				else
				{
					num = colorone;
				}
				float num2 = V - num;
				if (num2 != 0f)
				{
					S = num2 / V;
					H = offset + (colorone - colortwo) / num2;
				}
				else
				{
					S = 0f;
					H = offset + (colorone - colortwo);
				}
				H /= 6f;
				if (H < 0f)
				{
					H += 1f;
				}
			}
			else
			{
				S = 0f;
				H = 0f;
			}
		}
		public static Color HSVToRGB(float H, float S, float V)
		{
			Color white = Color.white;
			if (S == 0f)
			{
				white.r = V;
				white.g = V;
				white.b = V;
			}
			else
			{
				if (V == 0f)
				{
					white.r = 0f;
					white.g = 0f;
					white.b = 0f;
				}
				else
				{
					white.r = 0f;
					white.g = 0f;
					white.b = 0f;
					float num = H * 6f;
					int num2 = (int)Mathf.Floor(num);
					float num3 = num - (float)num2;
					float num4 = V * (1f - S);
					float num5 = V * (1f - S * num3);
					float num6 = V * (1f - S * (1f - num3));
					int num7 = num2;
					switch (num7 + 1)
					{
					case 0:
						white.r = V;
						white.g = num4;
						white.b = num5;
						break;
					case 1:
						white.r = V;
						white.g = num6;
						white.b = num4;
						break;
					case 2:
						white.r = num5;
						white.g = V;
						white.b = num4;
						break;
					case 3:
						white.r = num4;
						white.g = V;
						white.b = num6;
						break;
					case 4:
						white.r = num4;
						white.g = num5;
						white.b = V;
						break;
					case 5:
						white.r = num6;
						white.g = num4;
						white.b = V;
						break;
					case 6:
						white.r = V;
						white.g = num4;
						white.b = num5;
						break;
					case 7:
						white.r = V;
						white.g = num6;
						white.b = num4;
						break;
					}
					white.r = Mathf.Clamp(white.r, 0f, 1f);
					white.g = Mathf.Clamp(white.g, 0f, 1f);
					white.b = Mathf.Clamp(white.b, 0f, 1f);
				}
			}
			return white;
		}
		public static void AddCursorRect(Rect position, MouseCursor mouse)
		{
			EditorGUIUtility.AddCursorRect(position, mouse, 0);
		}
		public static void AddCursorRect(Rect position, MouseCursor mouse, int controlID)
		{
			if (Event.current.type == EventType.Repaint)
			{
				Rect rect = GUIClip.Unclip(position);
				Rect topmostRect = GUIClip.topmostRect;
				Rect r = Rect.MinMaxRect(Mathf.Max(rect.x, topmostRect.x), Mathf.Max(rect.y, topmostRect.y), Mathf.Min(rect.xMax, topmostRect.xMax), Mathf.Min(rect.yMax, topmostRect.yMax));
				if (r.width <= 0f || r.height <= 0f)
				{
					return;
				}
				EditorGUIUtility.Internal_AddCursorRect(r, mouse, controlID);
			}
		}
		private static void Internal_AddCursorRect(Rect r, MouseCursor m, int controlID)
		{
			EditorGUIUtility.INTERNAL_CALL_Internal_AddCursorRect(ref r, m, controlID);
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_Internal_AddCursorRect(ref Rect r, MouseCursor m, int controlID);
		internal static Rect HandleHorizontalSplitter(Rect dragRect, float width, float minLeftSide, float minRightSide)
		{
			if (Event.current.type == EventType.Repaint)
			{
				EditorGUIUtility.AddCursorRect(dragRect, MouseCursor.SplitResizeLeftRight);
			}
			float num = 0f;
			float x = EditorGUI.MouseDeltaReader(dragRect, true).x;
			if (x != 0f)
			{
				dragRect.x += x;
				num = Mathf.Clamp(dragRect.x, minLeftSide, width - minRightSide);
			}
			if (dragRect.x > width - minRightSide)
			{
				num = width - minRightSide;
			}
			if (num > 0f)
			{
				dragRect.x = num;
			}
			return dragRect;
		}
		internal static void DrawHorizontalSplitter(Rect dragRect)
		{
			if (Event.current.type != EventType.Repaint)
			{
				return;
			}
			Color color = GUI.color;
			Color b = (!EditorGUIUtility.isProSkin) ? new Color(0.6f, 0.6f, 0.6f, 1.333f) : new Color(0.12f, 0.12f, 0.12f, 1.333f);
			GUI.color *= b;
			Rect position = new Rect(dragRect.x - 1f, dragRect.y, 1f, dragRect.height);
			GUI.DrawTexture(position, EditorGUIUtility.whiteTexture);
			GUI.color = color;
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void CleanCache(string text);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetSearchIndexOfControlIDList(int index);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int GetSearchIndexOfControlIDList();
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool CanHaveKeyboardFocus(int id);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetWantsMouseJumping(int wantz);
		public static void ShowObjectPicker<T>(UnityEngine.Object obj, bool allowSceneObjects, string searchFilter, int controlID) where T : UnityEngine.Object
		{
			Type typeFromHandle = typeof(T);
			ObjectSelector.get.Show(obj, typeFromHandle, null, allowSceneObjects);
			ObjectSelector.get.objectSelectorID = controlID;
			ObjectSelector.get.searchFilter = searchFilter;
		}
		public static UnityEngine.Object GetObjectPickerObject()
		{
			return ObjectSelector.GetCurrentObject();
		}
		public static int GetObjectPickerControlID()
		{
			return ObjectSelector.get.objectSelectorID;
		}
	}
}
