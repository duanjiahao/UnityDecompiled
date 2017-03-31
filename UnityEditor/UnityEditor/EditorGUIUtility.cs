using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Internal;
using UnityEngine.Rendering;
using UnityEngine.Scripting;
using UnityEngineInternal;

namespace UnityEditor
{
	public sealed class EditorGUIUtility : GUIUtility
	{
		public class IconSizeScope : GUI.Scope
		{
			private Vector2 m_OriginalIconSize;

			public IconSizeScope(Vector2 iconSizeWithinScope)
			{
				this.m_OriginalIconSize = EditorGUIUtility.GetIconSize();
				EditorGUIUtility.SetIconSize(iconSizeWithinScope);
			}

			protected override void CloseScope()
			{
				EditorGUIUtility.SetIconSize(this.m_OriginalIconSize);
			}
		}

		internal class SkinnedColor
		{
			private Color normalColor;

			private Color proColor;

			public Color color
			{
				get
				{
					Color result;
					if (EditorGUIUtility.isProSkin)
					{
						result = this.proColor;
					}
					else
					{
						result = this.normalColor;
					}
					return result;
				}
				set
				{
					if (EditorGUIUtility.isProSkin)
					{
						this.proColor = value;
					}
					else
					{
						this.normalColor = value;
					}
				}
			}

			public SkinnedColor(Color color, Color proColor)
			{
				this.normalColor = color;
				this.proColor = proColor;
			}

			public SkinnedColor(Color color)
			{
				this.normalColor = color;
				this.proColor = color;
			}

			public static implicit operator Color(EditorGUIUtility.SkinnedColor colorSkin)
			{
				return colorSkin.color;
			}
		}

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

		internal static int s_LastControlID;

		private static bool s_HierarchyMode;

		internal static bool s_WideMode;

		private static float s_ContextWidth;

		private static float s_LabelWidth;

		private static float s_FieldWidth;

		[Obsolete("This field is no longer used by any builtin controls. If passing this field to GetControlID, explicitly use the FocusType enum instead.", false)]
		public static FocusType native;

		internal static Material s_GUITextureBlitColorspaceMaterial;

		[CompilerGenerated]
		private static GUISkin.SkinChangedDelegate <>f__mg$cache0;

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
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern Texture2D whiteTexture
		{
			[GeneratedByOldBindingsGenerator]
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
				float result;
				if (EditorGUIUtility.s_ContextWidth > 0f)
				{
					result = EditorGUIUtility.s_ContextWidth;
				}
				else
				{
					result = EditorGUIUtility.CalcContextWidth();
				}
				return result;
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
				float result;
				if (EditorGUIUtility.s_LabelWidth > 0f)
				{
					result = EditorGUIUtility.s_LabelWidth;
				}
				else if (EditorGUIUtility.s_HierarchyMode)
				{
					result = Mathf.Max(EditorGUIUtility.contextWidth * 0.45f - 40f, 120f);
				}
				else
				{
					result = 150f;
				}
				return result;
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
				float result;
				if (EditorGUIUtility.s_FieldWidth > 0f)
				{
					result = EditorGUIUtility.s_FieldWidth;
				}
				else
				{
					result = 50f;
				}
				return result;
			}
			set
			{
				EditorGUIUtility.s_FieldWidth = value;
			}
		}

		public new static extern string systemCopyBuffer
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
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

		internal static Material GUITextureBlitColorspaceMaterial
		{
			get
			{
				if (!EditorGUIUtility.s_GUITextureBlitColorspaceMaterial)
				{
					Shader shader = EditorGUIUtility.LoadRequired("SceneView/GUITextureBlitColorspace.shader") as Shader;
					EditorGUIUtility.s_GUITextureBlitColorspaceMaterial = new Material(shader);
					EditorGUIUtility.SetGUITextureBlitColorspaceSettings(EditorGUIUtility.s_GUITextureBlitColorspaceMaterial);
				}
				return EditorGUIUtility.s_GUITextureBlitColorspaceMaterial;
			}
		}

		public new static float pixelsPerPoint
		{
			get
			{
				return GUIUtility.pixelsPerPoint;
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
			EditorGUIUtility.s_LastControlID = 0;
			EditorGUIUtility.s_HierarchyMode = false;
			EditorGUIUtility.s_WideMode = false;
			EditorGUIUtility.s_ContextWidth = 0f;
			EditorGUIUtility.s_LabelWidth = 0f;
			EditorGUIUtility.s_FieldWidth = 0f;
			EditorGUIUtility.native = FocusType.Keyboard;
			Delegate arg_D6_0 = GUISkin.m_SkinChanged;
			if (EditorGUIUtility.<>f__mg$cache0 == null)
			{
				EditorGUIUtility.<>f__mg$cache0 = new GUISkin.SkinChangedDelegate(EditorGUIUtility.SkinChanged);
			}
			GUISkin.m_SkinChanged = (GUISkin.SkinChangedDelegate)Delegate.Combine(arg_D6_0, EditorGUIUtility.<>f__mg$cache0);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string SerializeMainMenuToString();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetMenuLocalizationTestMode(bool onoff);

		internal static GUIContent TextContent(string textAndTooltip)
		{
			if (textAndTooltip == null)
			{
				textAndTooltip = "";
			}
			string key = textAndTooltip;
			GUIContent gUIContent = (GUIContent)EditorGUIUtility.s_TextGUIContents[key];
			if (gUIContent == null)
			{
				string[] nameAndTooltipString = EditorGUIUtility.GetNameAndTooltipString(textAndTooltip);
				gUIContent = new GUIContent(nameAndTooltipString[1]);
				if (nameAndTooltipString[2] != null)
				{
					gUIContent.tooltip = nameAndTooltipString[2];
				}
				EditorGUIUtility.s_TextGUIContents[key] = gUIContent;
			}
			return gUIContent;
		}

		internal static GUIContent TextContentWithIcon(string textAndTooltip, string icon)
		{
			if (textAndTooltip == null)
			{
				textAndTooltip = "";
			}
			if (icon == null)
			{
				icon = "";
			}
			string key = string.Format("{0}|{1}", textAndTooltip, icon);
			GUIContent gUIContent = (GUIContent)EditorGUIUtility.s_TextGUIContents[key];
			if (gUIContent == null)
			{
				string[] nameAndTooltipString = EditorGUIUtility.GetNameAndTooltipString(textAndTooltip);
				gUIContent = new GUIContent(nameAndTooltipString[1]);
				gUIContent.image = EditorGUIUtility.LoadIconRequired(icon);
				if (nameAndTooltipString[2] != null)
				{
					gUIContent.tooltip = nameAndTooltipString[2];
				}
				EditorGUIUtility.s_TextGUIContents[key] = gUIContent;
			}
			return gUIContent;
		}

		internal static string[] GetNameAndTooltipString(string nameAndTooltip)
		{
			nameAndTooltip = LocalizationDatabase.GetLocalizedString(nameAndTooltip);
			string[] array = new string[3];
			string[] array2 = nameAndTooltip.Split(new char[]
			{
				'|'
			});
			switch (array2.Length)
			{
			case 0:
				array[0] = "";
				array[1] = "";
				break;
			case 1:
				array[0] = array2[0].Trim();
				array[1] = array[0];
				break;
			case 2:
				array[0] = array2[0].Trim();
				array[1] = array[0];
				array[2] = array2[1].Trim();
				break;
			default:
				Debug.LogError("Error in Tooltips: Too many strings in line beginning with '" + array2[0] + "'");
				break;
			}
			return array;
		}

		internal static Texture2D LoadIconRequired(string name)
		{
			Texture2D texture2D = EditorGUIUtility.LoadIcon(name);
			if (!texture2D)
			{
				Debug.LogErrorFormat("Unable to load the icon: '{0}'.\nNote that either full project path should be used (with extension) or just the icon name if the icon is located in the following location: '{1}' (without extension, since png is assumed)", new object[]
				{
					name,
					"Assets/Editor Default Resources/" + EditorResourcesUtility.iconsPath
				});
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
			if (!texture2D)
			{
				texture2D = (EditorGUIUtility.Load(name) as Texture2D);
			}
			return texture2D;
		}

		internal static Texture2D LoadIconForSkin(string name, int skinIndex)
		{
			Texture2D result;
			if (string.IsNullOrEmpty(name))
			{
				result = null;
			}
			else if (skinIndex == 0)
			{
				result = EditorGUIUtility.LoadGeneratedIconOrNormalIcon(name);
			}
			else
			{
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
				result = texture2D;
			}
			return result;
		}

		[ExcludeFromDocs]
		public static GUIContent IconContent(string name)
		{
			string text = null;
			return EditorGUIUtility.IconContent(name, text);
		}

		public static GUIContent IconContent(string name, [DefaultValue("null")] string text)
		{
			GUIContent gUIContent = (GUIContent)EditorGUIUtility.s_IconGUIContents[name];
			GUIContent result;
			if (gUIContent != null)
			{
				result = gUIContent;
			}
			else
			{
				gUIContent = new GUIContent();
				if (text != null)
				{
					string[] nameAndTooltipString = EditorGUIUtility.GetNameAndTooltipString(text);
					if (nameAndTooltipString[2] != null)
					{
						gUIContent.tooltip = nameAndTooltipString[2];
					}
				}
				gUIContent.image = EditorGUIUtility.LoadIconRequired(name);
				EditorGUIUtility.s_IconGUIContents[name] = gUIContent;
				result = gUIContent;
			}
			return result;
		}

		internal static void Internal_SwitchSkin()
		{
			EditorGUIUtility.skinIndex = 1 - EditorGUIUtility.skinIndex;
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern string GetObjectNameWithInfo(UnityEngine.Object obj);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern string GetTypeNameWithInfo(string typeName);

		public static GUIContent ObjectContent(UnityEngine.Object obj, Type type)
		{
			if (obj)
			{
				EditorGUIUtility.s_ObjectContent.text = EditorGUIUtility.GetObjectNameWithInfo(obj);
				EditorGUIUtility.s_ObjectContent.image = AssetPreview.GetMiniThumbnail(obj);
			}
			else if (type != null)
			{
				EditorGUIUtility.s_ObjectContent.text = EditorGUIUtility.GetTypeNameWithInfo(type.Name);
				EditorGUIUtility.s_ObjectContent.image = AssetPreview.GetMiniTypeThumbnail(type);
			}
			else
			{
				EditorGUIUtility.s_ObjectContent.text = "<no type>";
				EditorGUIUtility.s_ObjectContent.image = null;
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
			return objType != null && (objType.IsSubclassOf(typeof(Texture)) || objType == typeof(Texture) || objType == typeof(Sprite));
		}

		public static void SetIconSize(Vector2 size)
		{
			EditorGUIUtility.INTERNAL_CALL_SetIconSize(ref size);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_SetIconSize(ref Vector2 size);

		public static Vector2 GetIconSize()
		{
			Vector2 result;
			EditorGUIUtility.Internal_GetIconSize(out result);
			return result;
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_GetIconSize(out Vector2 size);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern UnityEngine.Object GetScript(string scriptClass);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetIconForObject(UnityEngine.Object obj, Texture2D icon);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern Texture2D GetIconForObject(UnityEngine.Object obj);

		internal static Texture2D GetHelpIcon(MessageType type)
		{
			Texture2D result;
			if (type != MessageType.Info)
			{
				if (type != MessageType.Warning)
				{
					if (type != MessageType.Error)
					{
						result = null;
					}
					else
					{
						result = EditorGUIUtility.errorIcon;
					}
				}
				else
				{
					result = EditorGUIUtility.warningIcon;
				}
			}
			else
			{
				result = EditorGUIUtility.infoIcon;
			}
			return result;
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

		internal static void NotifyLanguageChanged(SystemLanguage newLanguage)
		{
			EditorGUIUtility.s_TextGUIContents = new Hashtable();
			EditorUtility.Internal_UpdateMenuTitleForLanguage(newLanguage);
			LocalizationDatabase.SetCurrentEditorLanguage(newLanguage);
			EditorApplication.RequestRepaintAllViews();
		}

		[GeneratedByOldBindingsGenerator]
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
			UnityEngine.Object result;
			if (@object != null)
			{
				result = @object;
			}
			else
			{
				AssetBundle editorAssetBundle = EditorGUIUtility.GetEditorAssetBundle();
				if (editorAssetBundle == null)
				{
					if (!Application.isBatchmode)
					{
						throw new NullReferenceException("Failure to load editor resource asset bundle.");
					}
					result = null;
				}
				else
				{
					@object = editorAssetBundle.LoadAsset(filename, type);
					if (@object != null)
					{
						result = @object;
					}
					else
					{
						result = AssetDatabase.LoadAssetAtPath(filename, type);
					}
				}
			}
			return result;
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern UnityEngine.Object GetBuiltinExtraResource(Type type, string path);

		[GeneratedByOldBindingsGenerator]
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
			Rect position;
			if (EditorGUIUtility.Internal_GetKeyboardRect(GUIUtility.keyboardControl, out position))
			{
				GUI.ScrollTo(position);
			}
		}

		internal static void ScrollForTabbing(bool forward)
		{
			Rect position;
			if (EditorGUIUtility.Internal_GetKeyboardRect(EditorGUIUtility.Internal_GetNextKeyboardControlID(forward), out position))
			{
				GUI.ScrollTo(position);
			}
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool Internal_GetKeyboardRect(int id, out Rect rect);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_MoveKeyboardFocus(bool forward);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int Internal_GetNextKeyboardControlID(bool forward);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern AssetBundle GetEditorAssetBundle();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetRenderTextureNoViewport(RenderTexture rt);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetVisibleLayers(int layers);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetLockedLayers(int layers);

		[GeneratedByOldBindingsGenerator]
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

		internal static void RenderGameViewCamerasInternal(RenderTexture target, int targetDisplay, Rect screenRect, Vector2 mousePosition, bool gizmos)
		{
			EditorGUIUtility.INTERNAL_CALL_RenderGameViewCamerasInternal(target, targetDisplay, ref screenRect, ref mousePosition, gizmos);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_RenderGameViewCamerasInternal(RenderTexture target, int targetDisplay, ref Rect screenRect, ref Vector2 mousePosition, bool gizmos);

		[Obsolete("RenderGameViewCameras is no longer supported. Consider rendering cameras manually.", true)]
		public static void RenderGameViewCameras(RenderTexture target, int targetDisplay, Rect screenRect, Vector2 mousePosition, bool gizmos)
		{
			EditorGUIUtility.INTERNAL_CALL_RenderGameViewCameras(target, targetDisplay, ref screenRect, ref mousePosition, gizmos);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_RenderGameViewCameras(RenderTexture target, int targetDisplay, ref Rect screenRect, ref Vector2 mousePosition, bool gizmos);

		[Obsolete("RenderGameViewCameras is no longer supported. Consider rendering cameras manually.", true)]
		public static void RenderGameViewCameras(Rect cameraRect, bool gizmos, bool gui)
		{
		}

		[Obsolete("RenderGameViewCameras is no longer supported. Consider rendering cameras manually.", true)]
		public static void RenderGameViewCameras(Rect cameraRect, Rect statsRect, bool gizmos, bool gui)
		{
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool IsDisplayReferencedByCameras(int displayIndex);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void QueueGameViewInputEvent(Event evt);

		[GeneratedByOldBindingsGenerator]
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

		[RequiredByNativeCode]
		internal static void HandleControlID(int id)
		{
			EditorGUIUtility.s_LastControlID = id;
			EditorGUI.PrepareCurrentPrefixLabel(EditorGUIUtility.s_LastControlID);
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

		[Obsolete("LookLikeControls and LookLikeInspector modes are deprecated. Use EditorGUIUtility.labelWidth and EditorGUIUtility.fieldWidth to control label and field widths."), ExcludeFromDocs]
		public static void LookLikeControls()
		{
			float fieldWidth = 0f;
			float labelWidth = 0f;
			EditorGUIUtility.LookLikeControls(labelWidth, fieldWidth);
		}

		[Obsolete("LookLikeControls and LookLikeInspector modes are deprecated. Use EditorGUIUtility.labelWidth and EditorGUIUtility.fieldWidth to control label and field widths.")]
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

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_SetupEventValues(object evt);

		public static void DrawColorSwatch(Rect position, Color color)
		{
			EditorGUIUtility.DrawColorSwatch(position, color, true);
		}

		internal static void DrawColorSwatch(Rect position, Color color, bool showAlpha)
		{
			EditorGUIUtility.DrawColorSwatch(position, color, showAlpha, false);
		}

		internal static void DrawColorSwatch(Rect position, Color color, bool showAlpha, bool hdr)
		{
			if (Event.current.type == EventType.Repaint)
			{
				Color color2 = GUI.color;
				float a = (float)((!GUI.enabled) ? 2 : 1);
				GUI.color = ((!EditorGUI.showMixedValue) ? new Color(color.r, color.g, color.b, a) : (new Color(0.82f, 0.82f, 0.82f, a) * color2));
				GUIStyle whiteTextureStyle = EditorGUIUtility.whiteTextureStyle;
				whiteTextureStyle.Draw(position, false, false, false, false);
				float maxColorComponent = GUI.color.maxColorComponent;
				if (hdr && maxColorComponent > 1f)
				{
					float num = position.width / 3f;
					Rect position2 = new Rect(position.x, position.y, num, position.height);
					Rect position3 = new Rect(position.xMax - num, position.y, num, position.height);
					Color color3 = GUI.color.RGBMultiplied(1f / maxColorComponent);
					Color color4 = GUI.color;
					GUI.color = color3;
					GUIStyle basicTextureStyle = EditorGUIUtility.GetBasicTextureStyle(EditorGUIUtility.whiteTexture);
					basicTextureStyle.Draw(position2, false, false, false, false);
					basicTextureStyle.Draw(position3, false, false, false, false);
					GUI.color = color4;
					basicTextureStyle = EditorGUIUtility.GetBasicTextureStyle(ColorPicker.GetGradientTextureWithAlpha0To1());
					basicTextureStyle.Draw(position2, false, false, false, false);
					basicTextureStyle = EditorGUIUtility.GetBasicTextureStyle(ColorPicker.GetGradientTextureWithAlpha1To0());
					basicTextureStyle.Draw(position3, false, false, false, false);
				}
				if (!EditorGUI.showMixedValue)
				{
					if (showAlpha)
					{
						GUI.color = new Color(0f, 0f, 0f, a);
						float num2 = Mathf.Clamp(position.height * 0.2f, 2f, 20f);
						Rect position4 = new Rect(position.x, position.yMax - num2, position.width, num2);
						whiteTextureStyle.Draw(position4, false, false, false, false);
						GUI.color = new Color(1f, 1f, 1f, a);
						position4.width *= Mathf.Clamp01(color.a);
						whiteTextureStyle.Draw(position4, false, false, false, false);
					}
				}
				else
				{
					EditorGUI.BeginHandleMixedValueContentColor();
					whiteTextureStyle.Draw(position, EditorGUI.mixedValueContent, false, false, false, false);
					EditorGUI.EndHandleMixedValueContentColor();
				}
				GUI.color = color2;
				if (hdr && maxColorComponent > 1f)
				{
					GUI.Label(new Rect(position.x, position.y, position.width - 3f, position.height), "HDR", EditorStyles.centeredGreyMiniLabel);
				}
			}
		}

		public static void DrawCurveSwatch(Rect position, AnimationCurve curve, SerializedProperty property, Color color, Color bgColor)
		{
			EditorGUIUtility.DrawCurveSwatchInternal(position, curve, null, property, null, color, bgColor, false, default(Rect), Color.clear, Color.clear);
		}

		public static void DrawCurveSwatch(Rect position, AnimationCurve curve, SerializedProperty property, Color color, Color bgColor, Color topFillColor, Color bottomFillColor)
		{
			EditorGUIUtility.DrawCurveSwatchInternal(position, curve, null, property, null, color, bgColor, false, default(Rect), topFillColor, bottomFillColor);
		}

		public static void DrawCurveSwatch(Rect position, AnimationCurve curve, SerializedProperty property, Color color, Color bgColor, Color topFillColor, Color bottomFillColor, Rect curveRanges)
		{
			EditorGUIUtility.DrawCurveSwatchInternal(position, curve, null, property, null, color, bgColor, true, curveRanges, topFillColor, bottomFillColor);
		}

		public static void DrawCurveSwatch(Rect position, AnimationCurve curve, SerializedProperty property, Color color, Color bgColor, Rect curveRanges)
		{
			EditorGUIUtility.DrawCurveSwatchInternal(position, curve, null, property, null, color, bgColor, true, curveRanges, Color.clear, Color.clear);
		}

		public static void DrawRegionSwatch(Rect position, SerializedProperty property, SerializedProperty property2, Color color, Color bgColor, Rect curveRanges)
		{
			EditorGUIUtility.DrawCurveSwatchInternal(position, null, null, property, property2, color, bgColor, true, curveRanges, Color.clear, Color.clear);
		}

		public static void DrawRegionSwatch(Rect position, AnimationCurve curve, AnimationCurve curve2, Color color, Color bgColor, Rect curveRanges)
		{
			EditorGUIUtility.DrawCurveSwatchInternal(position, curve, curve2, null, null, color, bgColor, true, curveRanges, Color.clear, Color.clear);
		}

		private static void DrawCurveSwatchInternal(Rect position, AnimationCurve curve, AnimationCurve curve2, SerializedProperty property, SerializedProperty property2, Color color, Color bgColor, bool useCurveRanges, Rect curveRanges, Color topFillColor, Color bottomFillColor)
		{
			if (Event.current.type == EventType.Repaint)
			{
				int num = (int)position.width;
				int num2 = (int)position.height;
				int maxTextureSize = SystemInfo.maxTextureSize;
				bool flag = num > maxTextureSize;
				bool flag2 = num2 > maxTextureSize;
				if (flag)
				{
					num = Mathf.Min(num, maxTextureSize);
				}
				if (flag2)
				{
					num2 = Mathf.Min(num2, maxTextureSize);
				}
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
							texture2D = ((!useCurveRanges) ? AnimationCurvePreviewCache.GetPreview(num, num2, property, color, topFillColor, bottomFillColor) : AnimationCurvePreviewCache.GetPreview(num, num2, property, color, topFillColor, bottomFillColor, curveRanges));
						}
						else
						{
							texture2D = ((!useCurveRanges) ? AnimationCurvePreviewCache.GetPreview(num, num2, property, property2, color, topFillColor, bottomFillColor) : AnimationCurvePreviewCache.GetPreview(num, num2, property, property2, color, topFillColor, bottomFillColor, curveRanges));
						}
					}
					else if (curve != null)
					{
						if (curve2 == null)
						{
							texture2D = ((!useCurveRanges) ? AnimationCurvePreviewCache.GetPreview(num, num2, curve, color, topFillColor, bottomFillColor) : AnimationCurvePreviewCache.GetPreview(num, num2, curve, color, topFillColor, bottomFillColor, curveRanges));
						}
						else
						{
							texture2D = ((!useCurveRanges) ? AnimationCurvePreviewCache.GetPreview(num, num2, curve, curve2, color, topFillColor, bottomFillColor) : AnimationCurvePreviewCache.GetPreview(num, num2, curve, curve2, color, topFillColor, bottomFillColor, curveRanges));
						}
					}
					gUIStyle = EditorGUIUtility.GetBasicTextureStyle(texture2D);
					if (!flag)
					{
						position.width = (float)texture2D.width;
					}
					if (!flag2)
					{
						position.height = (float)texture2D.height;
					}
					gUIStyle.Draw(position, false, false, false, false);
				}
			}
		}

		[Obsolete("EditorGUIUtility.RGBToHSV is obsolete. Use Color.RGBToHSV instead (UnityUpgradable) -> [UnityEngine] UnityEngine.Color.RGBToHSV(*)", true)]
		public static void RGBToHSV(Color rgbColor, out float H, out float S, out float V)
		{
			Color.RGBToHSV(rgbColor, out H, out S, out V);
		}

		[Obsolete("EditorGUIUtility.HSVToRGB is obsolete. Use Color.HSVToRGB instead (UnityUpgradable) -> [UnityEngine] UnityEngine.Color.HSVToRGB(*)", true)]
		public static Color HSVToRGB(float H, float S, float V)
		{
			return Color.HSVToRGB(H, S, V);
		}

		[Obsolete("EditorGUIUtility.HSVToRGB is obsolete. Use Color.HSVToRGB instead (UnityUpgradable) -> [UnityEngine] UnityEngine.Color.HSVToRGB(*)", true)]
		public static Color HSVToRGB(float H, float S, float V, bool hdr)
		{
			return Color.HSVToRGB(H, S, V, hdr);
		}

		internal static void SetPasteboardColor(Color color)
		{
			EditorGUIUtility.INTERNAL_CALL_SetPasteboardColor(ref color);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_SetPasteboardColor(ref Color color);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool HasPasteboardColor();

		internal static Color GetPasteboardColor()
		{
			Color result;
			EditorGUIUtility.INTERNAL_CALL_GetPasteboardColor(out result);
			return result;
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GetPasteboardColor(out Color value);

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
				if (r.width > 0f && r.height > 0f)
				{
					EditorGUIUtility.Internal_AddCursorRect(r, mouse, controlID);
				}
			}
		}

		private static void Internal_AddCursorRect(Rect r, MouseCursor m, int controlID)
		{
			EditorGUIUtility.INTERNAL_CALL_Internal_AddCursorRect(ref r, m, controlID);
		}

		[GeneratedByOldBindingsGenerator]
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
			if (Event.current.type == EventType.Repaint)
			{
				Color color = GUI.color;
				Color b = (!EditorGUIUtility.isProSkin) ? new Color(0.6f, 0.6f, 0.6f, 1.333f) : new Color(0.12f, 0.12f, 0.12f, 1.333f);
				GUI.color *= b;
				Rect position = new Rect(dragRect.x - 1f, dragRect.y, 1f, dragRect.height);
				GUI.DrawTexture(position, EditorGUIUtility.whiteTexture);
				GUI.color = color;
			}
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void CleanCache(string text);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetSearchIndexOfControlIDList(int index);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int GetSearchIndexOfControlIDList();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool CanHaveKeyboardFocus(int id);

		[GeneratedByOldBindingsGenerator]
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

		internal static void RepaintCurrentWindow()
		{
			GUIUtility.CheckOnGUI();
			GUIView.current.Repaint();
		}

		internal static bool HasCurrentWindowKeyFocus()
		{
			GUIUtility.CheckOnGUI();
			return GUIView.current.hasFocus;
		}

		internal static void SetGUITextureBlitColorspaceSettings(Material mat)
		{
			bool flag = SystemInfo.graphicsDeviceType == GraphicsDeviceType.Metal;
			if (flag && QualitySettings.activeColorSpace == ColorSpace.Linear)
			{
				mat.SetFloat("_ConvertToGamma", 1f);
			}
			else
			{
				mat.SetFloat("_ConvertToGamma", 0f);
			}
		}

		public static Rect PointsToPixels(Rect rect)
		{
			float pixelsPerPoint = EditorGUIUtility.pixelsPerPoint;
			rect.x *= pixelsPerPoint;
			rect.y *= pixelsPerPoint;
			rect.width *= pixelsPerPoint;
			rect.height *= pixelsPerPoint;
			return rect;
		}

		public static Rect PixelsToPoints(Rect rect)
		{
			float num = 1f / EditorGUIUtility.pixelsPerPoint;
			rect.x *= num;
			rect.y *= num;
			rect.width *= num;
			rect.height *= num;
			return rect;
		}

		public static Vector2 PointsToPixels(Vector2 position)
		{
			float pixelsPerPoint = EditorGUIUtility.pixelsPerPoint;
			position.x *= pixelsPerPoint;
			position.y *= pixelsPerPoint;
			return position;
		}

		public static Vector2 PixelsToPoints(Vector2 position)
		{
			float num = 1f / EditorGUIUtility.pixelsPerPoint;
			position.x *= num;
			position.y *= num;
			return position;
		}

		public static List<Rect> GetFlowLayoutedRects(Rect rect, GUIStyle style, float horizontalSpacing, float verticalSpacing, List<string> items)
		{
			List<Rect> list = new List<Rect>(items.Count);
			Vector2 position = rect.position;
			for (int i = 0; i < items.Count; i++)
			{
				GUIContent content = EditorGUIUtility.TempContent(items[i]);
				Vector2 size = style.CalcSize(content);
				Rect item = new Rect(position, size);
				if (position.x + size.x + horizontalSpacing >= rect.xMax)
				{
					position.x = rect.x;
					position.y += size.y + verticalSpacing;
					item.position = position;
				}
				list.Add(item);
				position.x += size.x + horizontalSpacing;
			}
			return list;
		}
	}
}
