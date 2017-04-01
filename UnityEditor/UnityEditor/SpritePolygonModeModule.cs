using System;
using System.Collections.Generic;
using UnityEditor.Sprites;
using UnityEditor.U2D.Interface;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.U2D.Interface;

namespace UnityEditor
{
	internal class SpritePolygonModeModule : SpriteFrameModuleBase, ISpriteEditorModule
	{
		private static class SpritePolygonModeStyles
		{
			public static readonly GUIContent changeShapeLabel = EditorGUIUtility.TextContent("Change Shape");

			public static readonly GUIContent sidesLabel = EditorGUIUtility.TextContent("Sides");

			public static readonly GUIContent polygonChangeShapeHelpBoxContent = EditorGUIUtility.TextContent("Sides can only be either 0 or anything between 3 and 128");

			public static readonly GUIContent changeButtonLabel = EditorGUIUtility.TextContent("Change|Change to the new number of sides");
		}

		private const int k_PolygonChangeShapeWindowMargin = 17;

		private const int k_PolygonChangeShapeWindowWidth = 150;

		private const int k_PolygonChangeShapeWindowHeight = 45;

		private const int k_PolygonChangeShapeWindowWarningHeight = 65;

		private Rect m_PolygonChangeShapeWindowRect = new Rect(0f, 17f, 150f, 45f);

		private bool polygonSprite
		{
			get
			{
				return base.spriteImportMode == SpriteImportMode.Polygon;
			}
		}

		public int polygonSides
		{
			get;
			set;
		}

		private bool isSidesValid
		{
			get
			{
				return this.polygonSides == 0 || (this.polygonSides >= 3 && this.polygonSides <= 128);
			}
		}

		public bool showChangeShapeWindow
		{
			get;
			set;
		}

		public SpritePolygonModeModule(ISpriteEditor sw, IEventSystem es, IUndoSystem us, IAssetDatabase ad) : base("Sprite Polygon Mode Editor", sw, es, us, ad)
		{
		}

		public override void OnModuleActivate()
		{
			base.OnModuleActivate();
			this.m_RectsCache = base.spriteEditor.spriteRects;
			this.showChangeShapeWindow = this.polygonSprite;
			if (this.polygonSprite)
			{
				this.DeterminePolygonSides();
			}
		}

		public override void OnModuleDeactivate()
		{
			this.m_RectsCache = null;
		}

		public override bool CanBeActivated()
		{
			return SpriteUtility.GetSpriteImportMode(base.assetDatabase, base.spriteEditor.selectedTexture) == SpriteImportMode.Polygon;
		}

		private void DeterminePolygonSides()
		{
			if (this.polygonSprite && this.m_RectsCache.Count == 1)
			{
				SpriteRect spriteRect = this.m_RectsCache.RectAt(0);
				if (spriteRect.outline.Count == 1)
				{
					this.polygonSides = spriteRect.outline[0].Count;
				}
			}
			else
			{
				this.polygonSides = 0;
			}
		}

		public int GetPolygonSideCount()
		{
			this.DeterminePolygonSides();
			return this.polygonSides;
		}

		public void GeneratePolygonOutline()
		{
			for (int i = 0; i < this.m_RectsCache.Count; i++)
			{
				SpriteRect spriteRect = this.m_RectsCache.RectAt(i);
				SpriteOutline spriteOutline = new SpriteOutline();
				spriteOutline.AddRange(UnityEditor.Sprites.SpriteUtility.GeneratePolygonOutlineVerticesOfSize(this.polygonSides, (int)spriteRect.rect.width, (int)spriteRect.rect.height));
				spriteRect.outline.Clear();
				spriteRect.outline.Add(spriteOutline);
				base.spriteEditor.SetDataModified();
			}
			base.Repaint();
		}

		public override void OnPostGUI()
		{
			this.DoPolygonChangeShapeWindow();
			base.OnPostGUI();
		}

		public override void DoTextureGUI()
		{
			base.DoTextureGUI();
			this.DrawGizmos();
			base.HandleGizmoMode();
			base.HandleBorderCornerScalingHandles();
			base.HandleBorderSidePointScalingSliders();
			base.HandleBorderSideScalingHandles();
			base.HandlePivotHandle();
			if (!base.MouseOnTopOfInspector())
			{
				base.spriteEditor.HandleSpriteSelection();
			}
		}

		public override void DrawToolbarGUI(Rect toolbarRect)
		{
			using (new EditorGUI.DisabledScope(base.spriteEditor.editingDisabled))
			{
				GUIStyle toolbarPopup = EditorStyles.toolbarPopup;
				Rect rect = toolbarRect;
				rect.width = toolbarPopup.CalcSize(SpritePolygonModeModule.SpritePolygonModeStyles.changeShapeLabel).x;
				SpriteUtilityWindow.DrawToolBarWidget(ref rect, ref toolbarRect, delegate(Rect adjustedDrawArea)
				{
					this.showChangeShapeWindow = GUI.Toggle(adjustedDrawArea, this.showChangeShapeWindow, SpritePolygonModeModule.SpritePolygonModeStyles.changeShapeLabel, EditorStyles.toolbarButton);
				});
			}
		}

		private void DrawGizmos()
		{
			if (base.eventSystem.current.type == EventType.Repaint)
			{
				for (int i = 0; i < base.spriteCount; i++)
				{
					List<SpriteOutline> spriteOutlineAt = base.GetSpriteOutlineAt(i);
					Vector2 b = base.GetSpriteRectAt(i).size * 0.5f;
					if (spriteOutlineAt.Count > 0)
					{
						SpriteEditorUtility.BeginLines(new Color(0.75f, 0.75f, 0.75f, 0.75f));
						for (int j = 0; j < spriteOutlineAt.Count; j++)
						{
							int k = 0;
							int index = spriteOutlineAt[j].Count - 1;
							while (k < spriteOutlineAt[j].Count)
							{
								SpriteEditorUtility.DrawLine(spriteOutlineAt[j][index] + b, spriteOutlineAt[j][k] + b);
								index = k;
								k++;
							}
						}
						SpriteEditorUtility.EndLines();
					}
				}
				base.DrawSpriteRectGizmos();
			}
		}

		private void DoPolygonChangeShapeWindow()
		{
			if (this.showChangeShapeWindow && !base.spriteEditor.editingDisabled)
			{
				bool flag = false;
				float labelWidth = EditorGUIUtility.labelWidth;
				EditorGUIUtility.labelWidth = 45f;
				GUILayout.BeginArea(this.m_PolygonChangeShapeWindowRect);
				GUILayout.BeginVertical(GUI.skin.box, new GUILayoutOption[0]);
				IEvent current = base.eventSystem.current;
				if (this.isSidesValid && current.type == EventType.KeyDown && current.keyCode == KeyCode.Return)
				{
					flag = true;
					current.Use();
				}
				EditorGUI.BeginChangeCheck();
				this.polygonSides = EditorGUILayout.IntField(SpritePolygonModeModule.SpritePolygonModeStyles.sidesLabel, this.polygonSides, new GUILayoutOption[0]);
				if (EditorGUI.EndChangeCheck())
				{
					this.m_PolygonChangeShapeWindowRect.height = (float)((!this.isSidesValid) ? 65 : 45);
				}
				GUILayout.FlexibleSpace();
				if (!this.isSidesValid)
				{
					EditorGUILayout.HelpBox(SpritePolygonModeModule.SpritePolygonModeStyles.polygonChangeShapeHelpBoxContent.text, MessageType.Warning, true);
				}
				else
				{
					GUILayout.BeginHorizontal(new GUILayoutOption[0]);
					GUILayout.FlexibleSpace();
					EditorGUI.BeginDisabledGroup(!this.isSidesValid);
					if (GUILayout.Button(SpritePolygonModeModule.SpritePolygonModeStyles.changeButtonLabel, new GUILayoutOption[0]))
					{
						flag = true;
					}
					EditorGUI.EndDisabledGroup();
					GUILayout.EndHorizontal();
				}
				GUILayout.EndVertical();
				if (flag)
				{
					if (this.isSidesValid)
					{
						this.GeneratePolygonOutline();
					}
					this.showChangeShapeWindow = false;
					GUIUtility.hotControl = 0;
					GUIUtility.keyboardControl = 0;
				}
				EditorGUIUtility.labelWidth = labelWidth;
				GUILayout.EndArea();
			}
		}
	}
}
