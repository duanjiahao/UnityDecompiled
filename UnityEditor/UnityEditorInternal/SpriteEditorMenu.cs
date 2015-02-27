using System;
using UnityEditor;
using UnityEngine;
namespace UnityEditorInternal
{
	internal class SpriteEditorMenu : EditorWindow
	{
		private enum SlicingType
		{
			Automatic,
			Grid
		}
		private class Styles
		{
			public GUIStyle background = "grey_border";
			public GUIStyle notice;
			public Styles()
			{
				this.notice = new GUIStyle(GUI.skin.label);
				this.notice.alignment = TextAnchor.MiddleCenter;
				this.notice.wordWrap = true;
			}
		}
		public readonly string[] spriteAlignmentOptions = new string[]
		{
			EditorGUIUtility.TextContent("SpriteInspector.Pivot.Center").text,
			EditorGUIUtility.TextContent("SpriteInspector.Pivot.TopLeft").text,
			EditorGUIUtility.TextContent("SpriteInspector.Pivot.Top").text,
			EditorGUIUtility.TextContent("SpriteInspector.Pivot.TopRight").text,
			EditorGUIUtility.TextContent("SpriteInspector.Pivot.Left").text,
			EditorGUIUtility.TextContent("SpriteInspector.Pivot.Right").text,
			EditorGUIUtility.TextContent("SpriteInspector.Pivot.BottomLeft").text,
			EditorGUIUtility.TextContent("SpriteInspector.Pivot.Bottom").text,
			EditorGUIUtility.TextContent("SpriteInspector.Pivot.BottomRight").text
		};
		public readonly string[] slicingMethodOptions = new string[]
		{
			EditorGUIUtility.TextContent("SpriteEditor.Slicing.DeleteAll").text,
			EditorGUIUtility.TextContent("SpriteEditor.Slicing.Smart").text,
			EditorGUIUtility.TextContent("SpriteEditor.Slicing.Safe").text
		};
		public static SpriteEditorMenu s_SpriteEditorMenu;
		private static SpriteEditorMenu.Styles s_Styles;
		private static long s_LastClosedTime;
		private static int s_Selected;
		private static Vector2 s_GridSpriteSize = new Vector2(64f, 64f);
		private static Vector2 s_GridSpriteOffset = new Vector2(0f, 0f);
		private static Vector2 s_GridSpritePadding = new Vector2(0f, 0f);
		private static int s_AutoSlicingMethod = 0;
		private static int s_SpriteAlignment;
		private static SpriteEditorMenu.SlicingType s_SlicingType;
		public static SpriteEditorWindow s_SpriteEditor;
		private void Init(Rect buttonRect)
		{
			buttonRect = GUIUtility.GUIToScreenRect(buttonRect);
			float y = 120f;
			Vector2 windowSize = new Vector2(300f, y);
			base.ShowAsDropDown(buttonRect, windowSize);
		}
		private void OnDisable()
		{
			SpriteEditorMenu.s_LastClosedTime = DateTime.Now.Ticks / 10000L;
			SpriteEditorMenu.s_SpriteEditorMenu = null;
		}
		internal static bool ShowAtPosition(Rect buttonRect)
		{
			long num = DateTime.Now.Ticks / 10000L;
			if (num >= SpriteEditorMenu.s_LastClosedTime + 50L)
			{
				Event.current.Use();
				if (SpriteEditorMenu.s_SpriteEditorMenu == null)
				{
					SpriteEditorMenu.s_SpriteEditorMenu = ScriptableObject.CreateInstance<SpriteEditorMenu>();
				}
				SpriteEditorMenu.s_SpriteEditorMenu.Init(buttonRect);
				return true;
			}
			return false;
		}
		private void OnGUI()
		{
			if (SpriteEditorMenu.s_Styles == null)
			{
				SpriteEditorMenu.s_Styles = new SpriteEditorMenu.Styles();
			}
			GUILayout.Space(4f);
			EditorGUIUtility.labelWidth = 124f;
			EditorGUIUtility.wideMode = true;
			GUI.Label(new Rect(0f, 0f, base.position.width, base.position.height), GUIContent.none, SpriteEditorMenu.s_Styles.background);
			SpriteEditorMenu.s_SlicingType = (SpriteEditorMenu.SlicingType)EditorGUILayout.EnumPopup("Type", SpriteEditorMenu.s_SlicingType, new GUILayoutOption[0]);
			SpriteEditorMenu.SlicingType slicingType = SpriteEditorMenu.s_SlicingType;
			if (slicingType != SpriteEditorMenu.SlicingType.Automatic)
			{
				if (slicingType == SpriteEditorMenu.SlicingType.Grid)
				{
					this.OnGridGUI();
				}
			}
			else
			{
				this.OnAutomaticGUI();
			}
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.Space(EditorGUIUtility.labelWidth + 4f);
			if (GUILayout.Button("Slice", new GUILayoutOption[0]))
			{
				this.DoSlicing();
			}
			GUILayout.EndHorizontal();
		}
		private void DoSlicing()
		{
			this.DoAnalytics();
			SpriteEditorMenu.SlicingType slicingType = SpriteEditorMenu.s_SlicingType;
			if (slicingType != SpriteEditorMenu.SlicingType.Automatic)
			{
				if (slicingType == SpriteEditorMenu.SlicingType.Grid)
				{
					this.DoGridSlicing();
				}
			}
			else
			{
				this.DoAutomaticSlicing();
			}
		}
		private void DoAnalytics()
		{
			Analytics.Event("Sprite Editor", "Slice", "Type", (int)SpriteEditorMenu.s_SlicingType);
			if (SpriteEditorMenu.s_SpriteEditor.originalTexture != null)
			{
				Analytics.Event("Sprite Editor", "Slice", "Texture Width", SpriteEditorMenu.s_SpriteEditor.originalTexture.width);
				Analytics.Event("Sprite Editor", "Slice", "Texture Height", SpriteEditorMenu.s_SpriteEditor.originalTexture.height);
			}
			if (SpriteEditorMenu.s_SlicingType == SpriteEditorMenu.SlicingType.Automatic)
			{
				Analytics.Event("Sprite Editor", "Slice", "Auto Slicing Method", SpriteEditorMenu.s_AutoSlicingMethod);
			}
			else
			{
				Analytics.Event("Sprite Editor", "Slice", "Grid Slicing Size X", (int)SpriteEditorMenu.s_GridSpriteSize.x);
				Analytics.Event("Sprite Editor", "Slice", "Grid Slicing Size Y", (int)SpriteEditorMenu.s_GridSpriteSize.y);
				Analytics.Event("Sprite Editor", "Slice", "Grid Slicing Offset X", (int)SpriteEditorMenu.s_GridSpriteOffset.x);
				Analytics.Event("Sprite Editor", "Slice", "Grid Slicing Offset Y", (int)SpriteEditorMenu.s_GridSpriteOffset.y);
				Analytics.Event("Sprite Editor", "Slice", "Grid Slicing Padding X", (int)SpriteEditorMenu.s_GridSpritePadding.x);
				Analytics.Event("Sprite Editor", "Slice", "Grid Slicing Padding Y", (int)SpriteEditorMenu.s_GridSpritePadding.y);
			}
		}
		private void OnGridGUI()
		{
			SpriteEditorMenu.s_GridSpriteSize = EditorGUILayout.Vector2Field("Pixel Size", SpriteEditorMenu.s_GridSpriteSize, new GUILayoutOption[0]);
			SpriteEditorMenu.s_GridSpriteSize.x = (float)Mathf.Clamp((int)SpriteEditorMenu.s_GridSpriteSize.x, 1, 4096);
			SpriteEditorMenu.s_GridSpriteSize.y = (float)Mathf.Clamp((int)SpriteEditorMenu.s_GridSpriteSize.y, 1, 4096);
			SpriteEditorMenu.s_GridSpriteOffset = EditorGUILayout.Vector2Field("Offset", SpriteEditorMenu.s_GridSpriteOffset, new GUILayoutOption[0]);
			SpriteEditorMenu.s_GridSpriteOffset.x = (float)Mathf.Clamp((int)SpriteEditorMenu.s_GridSpriteOffset.x, 0, 4096);
			SpriteEditorMenu.s_GridSpriteOffset.y = (float)Mathf.Clamp((int)SpriteEditorMenu.s_GridSpriteOffset.y, 0, 4096);
			SpriteEditorMenu.s_GridSpritePadding = EditorGUILayout.Vector2Field("Padding", SpriteEditorMenu.s_GridSpritePadding, new GUILayoutOption[0]);
			SpriteEditorMenu.s_GridSpritePadding.x = (float)Mathf.Clamp((int)SpriteEditorMenu.s_GridSpritePadding.x, 0, 4096);
			SpriteEditorMenu.s_GridSpritePadding.y = (float)Mathf.Clamp((int)SpriteEditorMenu.s_GridSpritePadding.y, 0, 4096);
			SpriteEditorMenu.s_SpriteAlignment = EditorGUILayout.Popup("Pivot", SpriteEditorMenu.s_SpriteAlignment, this.spriteAlignmentOptions, new GUILayoutOption[0]);
			GUILayout.Space(2f);
		}
		private void OnAutomaticGUI()
		{
			float num = 38f;
			if (SpriteEditorMenu.s_SpriteEditor.originalTexture != null && TextureUtil.IsCompressedTextureFormat(SpriteEditorMenu.s_SpriteEditor.originalTexture.format))
			{
				EditorGUILayout.LabelField("To obtain more accurate slicing results, manual slicing is recommended!", SpriteEditorMenu.s_Styles.notice, new GUILayoutOption[0]);
				num -= 31f;
			}
			SpriteEditorMenu.s_SpriteAlignment = EditorGUILayout.Popup("Pivot", SpriteEditorMenu.s_SpriteAlignment, this.spriteAlignmentOptions, new GUILayoutOption[0]);
			SpriteEditorMenu.s_AutoSlicingMethod = EditorGUILayout.Popup("Method", SpriteEditorMenu.s_AutoSlicingMethod, this.slicingMethodOptions, new GUILayoutOption[0]);
			GUILayout.Space(num);
		}
		private void DoAutomaticSlicing()
		{
			SpriteEditorMenu.s_SpriteEditor.DoAutomaticSlicing(4, SpriteEditorMenu.s_SpriteAlignment, (SpriteEditorWindow.AutoSlicingMethod)SpriteEditorMenu.s_AutoSlicingMethod);
		}
		private void DoGridSlicing()
		{
			SpriteEditorMenu.s_SpriteEditor.DoGridSlicing(SpriteEditorMenu.s_GridSpriteSize, SpriteEditorMenu.s_GridSpriteOffset, SpriteEditorMenu.s_GridSpritePadding, SpriteEditorMenu.s_SpriteAlignment);
		}
	}
}
