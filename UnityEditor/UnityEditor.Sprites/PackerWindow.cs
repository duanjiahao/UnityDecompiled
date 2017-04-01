using System;
using System.Linq;
using UnityEditorInternal;
using UnityEngine;

namespace UnityEditor.Sprites
{
	internal class PackerWindow : SpriteUtilityWindow
	{
		private class PackerWindowStyle
		{
			public static readonly GUIContent packLabel = EditorGUIUtility.TextContent("Pack");

			public static readonly GUIContent repackLabel = EditorGUIUtility.TextContent("Repack");

			public static readonly GUIContent viewAtlasLabel = EditorGUIUtility.TextContent("View Atlas:");

			public static readonly GUIContent windowTitle = EditorGUIUtility.TextContent("Sprite Packer");

			public static readonly GUIContent pageContentLabel = EditorGUIUtility.TextContent("Page {0}");

			public static readonly GUIContent packingDisabledLabel = EditorGUIUtility.TextContent("Sprite packing is disabled. Enable it in Edit > Project Settings > Editor.");

			public static readonly GUIContent openProjectSettingButton = EditorGUIUtility.TextContent("Open Project Editor Settings");
		}

		private struct Edge
		{
			public ushort v0;

			public ushort v1;

			public Edge(ushort a, ushort b)
			{
				this.v0 = a;
				this.v1 = b;
			}

			public override bool Equals(object obj)
			{
				PackerWindow.Edge edge = (PackerWindow.Edge)obj;
				return (this.v0 == edge.v0 && this.v1 == edge.v1) || (this.v0 == edge.v1 && this.v1 == edge.v0);
			}

			public override int GetHashCode()
			{
				return ((int)this.v0 << 16 | (int)this.v1) ^ ((int)this.v1 << 16 | (int)this.v0).GetHashCode();
			}
		}

		private static string[] s_AtlasNamesEmpty = new string[]
		{
			"Sprite atlas cache is empty"
		};

		private string[] m_AtlasNames = PackerWindow.s_AtlasNamesEmpty;

		private int m_SelectedAtlas = 0;

		private static string[] s_PageNamesEmpty = new string[0];

		private string[] m_PageNames = PackerWindow.s_PageNamesEmpty;

		private int m_SelectedPage = 0;

		private Sprite m_SelectedSprite = null;

		private void OnEnable()
		{
			base.minSize = new Vector2(400f, 256f);
			base.titleContent = PackerWindow.PackerWindowStyle.windowTitle;
			this.Reset();
		}

		private void Reset()
		{
			this.RefreshAtlasNameList();
			this.RefreshAtlasPageList();
			this.m_SelectedAtlas = 0;
			this.m_SelectedPage = 0;
			this.m_SelectedSprite = null;
		}

		private void RefreshAtlasNameList()
		{
			this.m_AtlasNames = Packer.atlasNames;
			if (this.m_SelectedAtlas >= this.m_AtlasNames.Length)
			{
				this.m_SelectedAtlas = 0;
			}
		}

		private void RefreshAtlasPageList()
		{
			if (this.m_AtlasNames.Length > 0)
			{
				string atlasName = this.m_AtlasNames[this.m_SelectedAtlas];
				Texture2D[] texturesForAtlas = Packer.GetTexturesForAtlas(atlasName);
				this.m_PageNames = new string[texturesForAtlas.Length];
				for (int i = 0; i < texturesForAtlas.Length; i++)
				{
					this.m_PageNames[i] = string.Format(PackerWindow.PackerWindowStyle.pageContentLabel.text, i + 1);
				}
			}
			else
			{
				this.m_PageNames = PackerWindow.s_PageNamesEmpty;
			}
			if (this.m_SelectedPage >= this.m_PageNames.Length)
			{
				this.m_SelectedPage = 0;
			}
		}

		private void OnAtlasNameListChanged()
		{
			if (this.m_AtlasNames.Length > 0)
			{
				string[] atlasNames = Packer.atlasNames;
				string text = this.m_AtlasNames[this.m_SelectedAtlas];
				string value = (atlasNames.Length > this.m_SelectedAtlas) ? atlasNames[this.m_SelectedAtlas] : null;
				if (text.Equals(value))
				{
					this.RefreshAtlasNameList();
					this.RefreshAtlasPageList();
					this.m_SelectedSprite = null;
					return;
				}
			}
			this.Reset();
		}

		private bool ValidateIsPackingEnabled()
		{
			bool result;
			if (EditorSettings.spritePackerMode == SpritePackerMode.Disabled)
			{
				EditorGUILayout.BeginVertical(new GUILayoutOption[0]);
				GUILayout.Label(PackerWindow.PackerWindowStyle.packingDisabledLabel, new GUILayoutOption[0]);
				if (GUILayout.Button(PackerWindow.PackerWindowStyle.openProjectSettingButton, new GUILayoutOption[0]))
				{
					EditorApplication.ExecuteMenuItem("Edit/Project Settings/Editor");
				}
				EditorGUILayout.EndVertical();
				result = false;
			}
			else
			{
				result = true;
			}
			return result;
		}

		private Rect DoToolbarGUI()
		{
			Rect rect = new Rect(0f, 0f, base.position.width, 17f);
			if (Event.current.type == EventType.Repaint)
			{
				EditorStyles.toolbar.Draw(rect, false, false, false, false);
			}
			bool enabled = GUI.enabled;
			GUI.enabled = (this.m_AtlasNames.Length > 0);
			rect = base.DoAlphaZoomToolbarGUI(rect);
			GUI.enabled = enabled;
			Rect rect2 = new Rect(5f, 0f, 0f, 17f);
			rect.width -= rect2.x;
			using (new EditorGUI.DisabledScope(Application.isPlaying))
			{
				rect2.width = EditorStyles.toolbarButton.CalcSize(PackerWindow.PackerWindowStyle.packLabel).x;
				SpriteUtilityWindow.DrawToolBarWidget(ref rect2, ref rect, delegate(Rect adjustedDrawRect)
				{
					if (GUI.Button(adjustedDrawRect, PackerWindow.PackerWindowStyle.packLabel, EditorStyles.toolbarButton))
					{
						Packer.RebuildAtlasCacheIfNeeded(EditorUserBuildSettings.activeBuildTarget, true);
						this.m_SelectedSprite = null;
						this.RefreshAtlasPageList();
						this.RefreshState();
					}
				});
				using (new EditorGUI.DisabledScope(Packer.SelectedPolicy == Packer.kDefaultPolicy))
				{
					rect2.x += rect2.width;
					rect2.width = EditorStyles.toolbarButton.CalcSize(PackerWindow.PackerWindowStyle.repackLabel).x;
					SpriteUtilityWindow.DrawToolBarWidget(ref rect2, ref rect, delegate(Rect adjustedDrawRect)
					{
						if (GUI.Button(adjustedDrawRect, PackerWindow.PackerWindowStyle.repackLabel, EditorStyles.toolbarButton))
						{
							Packer.RebuildAtlasCacheIfNeeded(EditorUserBuildSettings.activeBuildTarget, true, Packer.Execution.ForceRegroup);
							this.m_SelectedSprite = null;
							this.RefreshAtlasPageList();
							this.RefreshState();
						}
					});
				}
			}
			float x = GUI.skin.label.CalcSize(PackerWindow.PackerWindowStyle.viewAtlasLabel).x;
			float num = x + 100f + 70f + 100f;
			rect2.x += 5f;
			rect.width -= 5f;
			float width = rect.width;
			using (new EditorGUI.DisabledScope(this.m_AtlasNames.Length == 0))
			{
				rect2.x += rect2.width;
				rect2.width = x / num * width;
				SpriteUtilityWindow.DrawToolBarWidget(ref rect2, ref rect, delegate(Rect adjustedDrawArea)
				{
					GUI.Label(adjustedDrawArea, PackerWindow.PackerWindowStyle.viewAtlasLabel);
				});
				EditorGUI.BeginChangeCheck();
				rect2.x += rect2.width;
				rect2.width = 100f / num * width;
				SpriteUtilityWindow.DrawToolBarWidget(ref rect2, ref rect, delegate(Rect adjustedDrawArea)
				{
					this.m_SelectedAtlas = EditorGUI.Popup(adjustedDrawArea, this.m_SelectedAtlas, this.m_AtlasNames, EditorStyles.toolbarPopup);
				});
				if (EditorGUI.EndChangeCheck())
				{
					this.RefreshAtlasPageList();
					this.m_SelectedSprite = null;
				}
				EditorGUI.BeginChangeCheck();
				rect2.x += rect2.width;
				rect2.width = 70f / num * width;
				SpriteUtilityWindow.DrawToolBarWidget(ref rect2, ref rect, delegate(Rect adjustedDrawArea)
				{
					this.m_SelectedPage = EditorGUI.Popup(adjustedDrawArea, this.m_SelectedPage, this.m_PageNames, EditorStyles.toolbarPopup);
				});
				if (EditorGUI.EndChangeCheck())
				{
					this.m_SelectedSprite = null;
				}
			}
			EditorGUI.BeginChangeCheck();
			string[] policies = Packer.Policies;
			int selectedPolicy = Array.IndexOf<string>(policies, Packer.SelectedPolicy);
			rect2.x += rect2.width;
			rect2.width = 100f / num * width;
			SpriteUtilityWindow.DrawToolBarWidget(ref rect2, ref rect, delegate(Rect adjustedDrawArea)
			{
				selectedPolicy = EditorGUI.Popup(adjustedDrawArea, selectedPolicy, policies, EditorStyles.toolbarPopup);
			});
			if (EditorGUI.EndChangeCheck())
			{
				Packer.SelectedPolicy = policies[selectedPolicy];
			}
			return rect;
		}

		private void OnSelectionChange()
		{
			if (!(Selection.activeObject == null))
			{
				Sprite sprite = Selection.activeObject as Sprite;
				if (sprite != this.m_SelectedSprite)
				{
					if (sprite != null)
					{
						string selAtlasName;
						Texture2D selAtlasTexture;
						Packer.GetAtlasDataForSprite(sprite, out selAtlasName, out selAtlasTexture);
						int num = this.m_AtlasNames.ToList<string>().FindIndex((string s) => selAtlasName == s);
						if (num == -1)
						{
							return;
						}
						int num2 = Packer.GetTexturesForAtlas(selAtlasName).ToList<Texture2D>().FindIndex((Texture2D t) => selAtlasTexture == t);
						if (num2 == -1)
						{
							return;
						}
						this.m_SelectedAtlas = num;
						this.m_SelectedPage = num2;
						this.RefreshAtlasPageList();
					}
					this.m_SelectedSprite = sprite;
					base.Repaint();
				}
			}
		}

		private void RefreshState()
		{
			string[] atlasNames = Packer.atlasNames;
			if (!atlasNames.SequenceEqual(this.m_AtlasNames))
			{
				if (atlasNames.Length == 0)
				{
					this.Reset();
					return;
				}
				this.OnAtlasNameListChanged();
			}
			if (this.m_AtlasNames.Length == 0)
			{
				base.SetNewTexture(null);
			}
			else
			{
				if (this.m_SelectedAtlas >= this.m_AtlasNames.Length)
				{
					this.m_SelectedAtlas = 0;
				}
				string atlasName = this.m_AtlasNames[this.m_SelectedAtlas];
				Texture2D[] texturesForAtlas = Packer.GetTexturesForAtlas(atlasName);
				if (this.m_SelectedPage >= texturesForAtlas.Length)
				{
					this.m_SelectedPage = 0;
				}
				base.SetNewTexture(texturesForAtlas[this.m_SelectedPage]);
				Texture2D[] alphaTexturesForAtlas = Packer.GetAlphaTexturesForAtlas(atlasName);
				Texture2D alphaTextureOverride = (this.m_SelectedPage >= alphaTexturesForAtlas.Length) ? null : alphaTexturesForAtlas[this.m_SelectedPage];
				base.SetAlphaTextureOverride(alphaTextureOverride);
			}
		}

		public void OnGUI()
		{
			if (this.ValidateIsPackingEnabled())
			{
				Matrix4x4 matrix = Handles.matrix;
				base.InitStyles();
				this.RefreshState();
				Rect rect = this.DoToolbarGUI();
				if (!(this.m_Texture == null))
				{
					EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
					this.m_TextureViewRect = new Rect(0f, rect.yMax, base.position.width - 16f, base.position.height - 16f - rect.height);
					GUILayout.FlexibleSpace();
					base.DoTextureGUI();
					string text = string.Format("{1}x{2}, {0}", TextureUtil.GetTextureFormatString(this.m_Texture.format), this.m_Texture.width, this.m_Texture.height);
					EditorGUI.DropShadowLabel(new Rect(this.m_TextureViewRect.x, this.m_TextureViewRect.y + 10f, this.m_TextureViewRect.width, 20f), text);
					EditorGUILayout.EndHorizontal();
					Handles.matrix = matrix;
				}
			}
		}

		private void DrawLineUtility(Vector2 from, Vector2 to)
		{
			SpriteEditorUtility.DrawLine(new Vector3(from.x * (float)this.m_Texture.width + 1f / this.m_Zoom, from.y * (float)this.m_Texture.height + 1f / this.m_Zoom, 0f), new Vector3(to.x * (float)this.m_Texture.width + 1f / this.m_Zoom, to.y * (float)this.m_Texture.height + 1f / this.m_Zoom, 0f));
		}

		private PackerWindow.Edge[] FindUniqueEdges(ushort[] indices)
		{
			PackerWindow.Edge[] array = new PackerWindow.Edge[indices.Length];
			int num = indices.Length / 3;
			for (int i = 0; i < num; i++)
			{
				array[i * 3] = new PackerWindow.Edge(indices[i * 3], indices[i * 3 + 1]);
				array[i * 3 + 1] = new PackerWindow.Edge(indices[i * 3 + 1], indices[i * 3 + 2]);
				array[i * 3 + 2] = new PackerWindow.Edge(indices[i * 3 + 2], indices[i * 3]);
			}
			return (from x in array
			group x by x into x
			where x.Count<PackerWindow.Edge>() == 1
			select x.First<PackerWindow.Edge>()).ToArray<PackerWindow.Edge>();
		}

		protected override void DrawGizmos()
		{
			if (this.m_SelectedSprite != null && this.m_Texture != null)
			{
				Vector2[] spriteUVs = SpriteUtility.GetSpriteUVs(this.m_SelectedSprite, true);
				ushort[] triangles = this.m_SelectedSprite.triangles;
				PackerWindow.Edge[] array = this.FindUniqueEdges(triangles);
				SpriteEditorUtility.BeginLines(new Color(0.3921f, 0.5843f, 0.9294f, 0.75f));
				PackerWindow.Edge[] array2 = array;
				for (int i = 0; i < array2.Length; i++)
				{
					PackerWindow.Edge edge = array2[i];
					this.DrawLineUtility(spriteUVs[(int)edge.v0], spriteUVs[(int)edge.v1]);
				}
				SpriteEditorUtility.EndLines();
			}
		}
	}
}
