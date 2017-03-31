using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor.U2D.Interface;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.U2D.Interface;

namespace UnityEditor
{
	internal class SpriteFrameModule : SpriteFrameModuleBase
	{
		public enum AutoSlicingMethod
		{
			DeleteAll,
			Smart,
			Safe
		}

		private static class SpriteFrameModuleStyles
		{
			public static readonly GUIContent sliceButtonLabel = EditorGUIUtility.TextContent("Slice");

			public static readonly GUIContent trimButtonLabel = EditorGUIUtility.TextContent("Trim|Trims selected rectangle (T)");

			public static readonly GUIContent okButtonLabel = EditorGUIUtility.TextContent("Ok");

			public static readonly GUIContent cancelButtonLabel = EditorGUIUtility.TextContent("Cancel");
		}

		private bool[] m_AlphaPixelCache;

		private const int kDefaultColliderAlphaCutoff = 254;

		private const float kDefaultColliderDetail = 0.25f;

		internal static PrefKey k_SpriteEditorTrim = new PrefKey("Sprite Editor/Trim", "#t");

		public SpriteFrameModule(ISpriteEditor sw, IEventSystem es, IUndoSystem us, IAssetDatabase ad) : base("Sprite Editor", sw, es, us, ad)
		{
		}

		public override void OnModuleActivate()
		{
			base.OnModuleActivate();
			this.m_RectsCache = base.spriteEditor.spriteRects;
			base.spriteEditor.enableMouseMoveEvent = true;
		}

		public override void OnModuleDeactivate()
		{
			this.m_RectsCache = null;
		}

		public override bool CanBeActivated()
		{
			return SpriteUtility.GetSpriteImportMode(base.assetDatabase, base.spriteEditor.selectedTexture) != SpriteImportMode.Polygon;
		}

		private string GetUniqueName(string prefix, int startIndex)
		{
			string text;
			bool flag;
			do
			{
				text = prefix + "_" + startIndex++;
				flag = false;
				for (int i = 0; i < this.m_RectsCache.Count; i++)
				{
					if (this.m_RectsCache.RectAt(i).name == text)
					{
						flag = true;
						break;
					}
				}
			}
			while (flag);
			return text;
		}

		private List<Rect> SortRects(List<Rect> rects)
		{
			List<Rect> list = new List<Rect>();
			while (rects.Count > 0)
			{
				Rect rect = rects[rects.Count - 1];
				Rect sweepRect = new Rect(0f, rect.yMin, (float)base.previewTexture.width, rect.height);
				List<Rect> list2 = this.RectSweep(rects, sweepRect);
				if (list2.Count <= 0)
				{
					list.AddRange(rects);
					break;
				}
				list.AddRange(list2);
			}
			return list;
		}

		private List<Rect> RectSweep(List<Rect> rects, Rect sweepRect)
		{
			List<Rect> result;
			if (rects == null || rects.Count == 0)
			{
				result = new List<Rect>();
			}
			else
			{
				List<Rect> list = new List<Rect>();
				foreach (Rect current in rects)
				{
					if (current.Overlaps(sweepRect))
					{
						list.Add(current);
					}
				}
				foreach (Rect current2 in list)
				{
					rects.Remove(current2);
				}
				list.Sort((Rect a, Rect b) => a.x.CompareTo(b.x));
				result = list;
			}
			return result;
		}

		private void AddSprite(Rect frame, int alignment, Vector2 pivot, SpriteFrameModule.AutoSlicingMethod slicingMethod, ref int index)
		{
			if (slicingMethod != SpriteFrameModule.AutoSlicingMethod.DeleteAll)
			{
				SpriteRect existingOverlappingSprite = this.GetExistingOverlappingSprite(frame);
				if (existingOverlappingSprite != null)
				{
					if (slicingMethod == SpriteFrameModule.AutoSlicingMethod.Smart)
					{
						existingOverlappingSprite.rect = frame;
						existingOverlappingSprite.alignment = (SpriteAlignment)alignment;
						existingOverlappingSprite.pivot = pivot;
					}
				}
				else
				{
					this.AddSpriteWithUniqueName(frame, alignment, pivot, 254, 0.25f, index++);
				}
			}
			else
			{
				this.AddSprite(frame, alignment, pivot, 254, 0.25f, this.GetSpriteNamePrefix() + "_" + index++);
			}
		}

		private SpriteRect GetExistingOverlappingSprite(Rect rect)
		{
			SpriteRect result;
			for (int i = 0; i < this.m_RectsCache.Count; i++)
			{
				if (this.m_RectsCache.RectAt(i).rect.Overlaps(rect))
				{
					result = this.m_RectsCache.RectAt(i);
					return result;
				}
			}
			result = null;
			return result;
		}

		private bool PixelHasAlpha(int x, int y, ITexture2D texture)
		{
			if (this.m_AlphaPixelCache == null)
			{
				this.m_AlphaPixelCache = new bool[texture.width * texture.height];
				Color32[] pixels = texture.GetPixels32();
				for (int i = 0; i < pixels.Length; i++)
				{
					this.m_AlphaPixelCache[i] = (pixels[i].a != 0);
				}
			}
			int num = y * texture.width + x;
			return this.m_AlphaPixelCache[num];
		}

		private SpriteRect AddSprite(Rect rect, int alignment, Vector2 pivot, int colliderAlphaCutoff, float colliderDetail, string name)
		{
			SpriteRect spriteRect = new SpriteRect();
			spriteRect.rect = rect;
			spriteRect.alignment = (SpriteAlignment)alignment;
			spriteRect.pivot = pivot;
			spriteRect.name = name;
			spriteRect.originalName = spriteRect.name;
			spriteRect.border = Vector4.zero;
			base.spriteEditor.SetDataModified();
			this.m_RectsCache.AddRect(spriteRect);
			base.spriteEditor.SetDataModified();
			return spriteRect;
		}

		public SpriteRect AddSpriteWithUniqueName(Rect rect, int alignment, Vector2 pivot, int colliderAlphaCutoff, float colliderDetail, int nameIndexingHint)
		{
			string uniqueName = this.GetUniqueName(this.GetSpriteNamePrefix(), nameIndexingHint);
			return this.AddSprite(rect, alignment, pivot, colliderAlphaCutoff, colliderDetail, uniqueName);
		}

		private string GetSpriteNamePrefix()
		{
			return Path.GetFileNameWithoutExtension(base.spriteAssetPath);
		}

		public void DoAutomaticSlicing(int minimumSpriteSize, int alignment, Vector2 pivot, SpriteFrameModule.AutoSlicingMethod slicingMethod)
		{
			base.undoSystem.RegisterCompleteObjectUndo(this.m_RectsCache, "Automatic Slicing");
			if (slicingMethod == SpriteFrameModule.AutoSlicingMethod.DeleteAll)
			{
				this.m_RectsCache.ClearAll();
			}
			ITexture2D readableTexture2D = base.spriteEditor.GetReadableTexture2D();
			List<Rect> list = new List<Rect>(InternalSpriteUtility.GenerateAutomaticSpriteRectangles(readableTexture2D, minimumSpriteSize, 0));
			list = this.SortRects(list);
			int num = 0;
			foreach (Rect current in list)
			{
				this.AddSprite(current, alignment, pivot, slicingMethod, ref num);
			}
			base.selected = null;
			base.spriteEditor.SetDataModified();
			base.Repaint();
		}

		public void DoGridSlicing(Vector2 size, Vector2 offset, Vector2 padding, int alignment, Vector2 pivot)
		{
			ITexture2D readableTexture2D = base.spriteEditor.GetReadableTexture2D();
			Rect[] array = InternalSpriteUtility.GenerateGridSpriteRectangles(readableTexture2D, offset, size, padding);
			int num = 0;
			base.undoSystem.RegisterCompleteObjectUndo(this.m_RectsCache, "Grid Slicing");
			this.m_RectsCache.ClearAll();
			Rect[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				Rect rect = array2[i];
				this.AddSprite(rect, alignment, pivot, 254, 0.25f, this.GetSpriteNamePrefix() + "_" + num++);
			}
			base.selected = null;
			base.spriteEditor.SetDataModified();
			base.Repaint();
		}

		public void ScaleSpriteRect(Rect r)
		{
			if (base.selected != null)
			{
				base.undoSystem.RegisterCompleteObjectUndo(this.m_RectsCache, "Scale sprite");
				base.selected.rect = SpriteFrameModuleBase.ClampSpriteRect(r, (float)base.previewTexture.width, (float)base.previewTexture.height);
				base.selected.border = SpriteFrameModuleBase.ClampSpriteBorderToRect(base.selected.border, base.selected.rect);
				base.spriteEditor.SetDataModified();
			}
		}

		public void TrimAlpha()
		{
			ITexture2D readableTexture2D = base.spriteEditor.GetReadableTexture2D();
			if (!(readableTexture2D == null))
			{
				Rect rect = base.selected.rect;
				int num = (int)rect.xMax;
				int num2 = (int)rect.xMin;
				int num3 = (int)rect.yMax;
				int num4 = (int)rect.yMin;
				for (int i = (int)rect.yMin; i < (int)rect.yMax; i++)
				{
					for (int j = (int)rect.xMin; j < (int)rect.xMax; j++)
					{
						if (this.PixelHasAlpha(j, i, readableTexture2D))
						{
							num = Mathf.Min(num, j);
							num2 = Mathf.Max(num2, j);
							num3 = Mathf.Min(num3, i);
							num4 = Mathf.Max(num4, i);
						}
					}
				}
				if (num > num2 || num3 > num4)
				{
					rect = new Rect(0f, 0f, 0f, 0f);
				}
				else
				{
					rect = new Rect((float)num, (float)num3, (float)(num2 - num + 1), (float)(num4 - num3 + 1));
				}
				if (rect.width <= 0f && rect.height <= 0f)
				{
					this.m_RectsCache.RemoveRect(base.selected);
					base.spriteEditor.SetDataModified();
					base.selected = null;
				}
				else
				{
					rect = SpriteFrameModuleBase.ClampSpriteRect(rect, (float)readableTexture2D.width, (float)readableTexture2D.height);
					if (base.selected.rect != rect)
					{
						base.spriteEditor.SetDataModified();
					}
					base.selected.rect = rect;
				}
			}
		}

		public void DuplicateSprite()
		{
			if (base.selected != null)
			{
				base.undoSystem.RegisterCompleteObjectUndo(this.m_RectsCache, "Duplicate sprite");
				base.selected = this.AddSpriteWithUniqueName(base.selected.rect, (int)base.selected.alignment, base.selected.pivot, 254, 0.25f, 0);
			}
		}

		public void CreateSprite(Rect rect)
		{
			rect = SpriteFrameModuleBase.ClampSpriteRect(rect, (float)base.previewTexture.width, (float)base.previewTexture.height);
			base.undoSystem.RegisterCompleteObjectUndo(this.m_RectsCache, "Create sprite");
			base.selected = this.AddSpriteWithUniqueName(rect, 0, Vector2.zero, 254, 0.25f, 0);
		}

		public void DeleteSprite()
		{
			if (base.selected != null)
			{
				base.undoSystem.RegisterCompleteObjectUndo(this.m_RectsCache, "Delete sprite");
				this.m_RectsCache.RemoveRect(base.selected);
				base.selected = null;
				base.spriteEditor.SetDataModified();
			}
		}

		public override void DoTextureGUI()
		{
			base.DoTextureGUI();
			base.DrawSpriteRectGizmos();
			base.HandleGizmoMode();
			if (base.containsMultipleSprites)
			{
				this.HandleRectCornerScalingHandles();
			}
			base.HandleBorderCornerScalingHandles();
			base.HandleBorderSidePointScalingSliders();
			if (base.containsMultipleSprites)
			{
				this.HandleRectSideScalingHandles();
			}
			base.HandleBorderSideScalingHandles();
			base.HandlePivotHandle();
			if (base.containsMultipleSprites)
			{
				this.HandleDragging();
			}
			if (!base.MouseOnTopOfInspector())
			{
				base.spriteEditor.HandleSpriteSelection();
			}
			if (base.containsMultipleSprites)
			{
				this.HandleCreate();
				this.HandleDelete();
				this.HandleDuplicate();
			}
		}

		public override void DrawToolbarGUI(Rect toolbarRect)
		{
			using (new EditorGUI.DisabledScope(!base.containsMultipleSprites || base.spriteEditor.editingDisabled))
			{
				GUIStyle skin = EditorStyles.toolbarPopup;
				Rect rect = toolbarRect;
				rect.width = skin.CalcSize(SpriteFrameModule.SpriteFrameModuleStyles.sliceButtonLabel).x;
				SpriteUtilityWindow.DrawToolBarWidget(ref rect, ref toolbarRect, delegate(Rect adjustedDrawArea)
				{
					if (GUI.Button(adjustedDrawArea, SpriteFrameModule.SpriteFrameModuleStyles.sliceButtonLabel, skin))
					{
						if (SpriteEditorMenu.ShowAtPosition(adjustedDrawArea, this, this.spriteEditor.previewTexture, this.spriteEditor.selectedTexture))
						{
							GUIUtility.ExitGUI();
						}
					}
				});
				using (new EditorGUI.DisabledScope(!base.hasSelected))
				{
					rect.x += rect.width;
					rect.width = skin.CalcSize(SpriteFrameModule.SpriteFrameModuleStyles.trimButtonLabel).x;
					SpriteUtilityWindow.DrawToolBarWidget(ref rect, ref toolbarRect, delegate(Rect adjustedDrawArea)
					{
						if (GUI.Button(adjustedDrawArea, SpriteFrameModule.SpriteFrameModuleStyles.trimButtonLabel, EditorStyles.toolbarButton) || (string.IsNullOrEmpty(GUI.GetNameOfFocusedControl()) && SpriteFrameModule.k_SpriteEditorTrim.activated))
						{
							this.TrimAlpha();
							this.Repaint();
						}
					});
				}
			}
		}

		private void HandleRectCornerScalingHandles()
		{
			if (base.hasSelected)
			{
				GUIStyle dragdot = SpriteFrameModuleBase.styles.dragdot;
				GUIStyle dragdotactive = SpriteFrameModuleBase.styles.dragdotactive;
				Color white = Color.white;
				Rect r = new Rect(base.selectedSpriteRect);
				float xMin = r.xMin;
				float xMax = r.xMax;
				float yMax = r.yMax;
				float yMin = r.yMin;
				EditorGUI.BeginChangeCheck();
				base.HandleBorderPointSlider(ref xMin, ref yMax, MouseCursor.ResizeUpLeft, false, dragdot, dragdotactive, white);
				base.HandleBorderPointSlider(ref xMax, ref yMax, MouseCursor.ResizeUpRight, false, dragdot, dragdotactive, white);
				base.HandleBorderPointSlider(ref xMin, ref yMin, MouseCursor.ResizeUpRight, false, dragdot, dragdotactive, white);
				base.HandleBorderPointSlider(ref xMax, ref yMin, MouseCursor.ResizeUpLeft, false, dragdot, dragdotactive, white);
				if (EditorGUI.EndChangeCheck())
				{
					r.xMin = xMin;
					r.xMax = xMax;
					r.yMax = yMax;
					r.yMin = yMin;
					this.ScaleSpriteRect(r);
				}
			}
		}

		private void HandleRectSideScalingHandles()
		{
			if (base.hasSelected)
			{
				Rect r = new Rect(base.selectedSpriteRect);
				float num = r.xMin;
				float num2 = r.xMax;
				float num3 = r.yMax;
				float num4 = r.yMin;
				Vector2 vector = Handles.matrix.MultiplyPoint(new Vector3(r.xMin, r.yMin));
				Vector2 vector2 = Handles.matrix.MultiplyPoint(new Vector3(r.xMax, r.yMax));
				float width = Mathf.Abs(vector2.x - vector.x);
				float height = Mathf.Abs(vector2.y - vector.y);
				EditorGUI.BeginChangeCheck();
				num = base.HandleBorderScaleSlider(num, r.yMax, width, height, true);
				num2 = base.HandleBorderScaleSlider(num2, r.yMax, width, height, true);
				num3 = base.HandleBorderScaleSlider(r.xMin, num3, width, height, false);
				num4 = base.HandleBorderScaleSlider(r.xMin, num4, width, height, false);
				if (EditorGUI.EndChangeCheck())
				{
					r.xMin = num;
					r.xMax = num2;
					r.yMax = num3;
					r.yMin = num4;
					this.ScaleSpriteRect(r);
				}
			}
		}

		private void HandleDragging()
		{
			if (base.hasSelected && !base.MouseOnTopOfInspector())
			{
				Rect clamp = new Rect(0f, 0f, (float)base.previewTexture.width, (float)base.previewTexture.height);
				EditorGUI.BeginChangeCheck();
				Rect selectedSpriteRect = base.selectedSpriteRect;
				Rect selectedSpriteRect2 = SpriteEditorUtility.ClampedRect(SpriteEditorUtility.RoundedRect(SpriteEditorHandles.SliderRect(selectedSpriteRect)), clamp, true);
				if (EditorGUI.EndChangeCheck())
				{
					base.selectedSpriteRect = selectedSpriteRect2;
				}
			}
		}

		private void HandleCreate()
		{
			if (!base.MouseOnTopOfInspector() && !base.eventSystem.current.alt)
			{
				EditorGUI.BeginChangeCheck();
				Rect rect = SpriteEditorHandles.RectCreator((float)base.previewTexture.width, (float)base.previewTexture.height, SpriteFrameModuleBase.styles.createRect);
				if (EditorGUI.EndChangeCheck() && rect.width > 0f && rect.height > 0f)
				{
					this.CreateSprite(rect);
					GUIUtility.keyboardControl = 0;
				}
			}
		}

		private void HandleDuplicate()
		{
			IEvent current = base.eventSystem.current;
			if ((current.type == EventType.ValidateCommand || current.type == EventType.ExecuteCommand) && current.commandName == "Duplicate")
			{
				if (current.type == EventType.ExecuteCommand)
				{
					this.DuplicateSprite();
				}
				current.Use();
			}
		}

		private void HandleDelete()
		{
			IEvent current = base.eventSystem.current;
			if ((current.type == EventType.ValidateCommand || current.type == EventType.ExecuteCommand) && (current.commandName == "SoftDelete" || current.commandName == "Delete"))
			{
				if (current.type == EventType.ExecuteCommand && base.hasSelected)
				{
					this.DeleteSprite();
				}
				current.Use();
			}
		}
	}
}
