using System;
using UnityEngine;

namespace UnityEditor
{
	[CustomEditor(typeof(Cubemap))]
	internal class CubemapInspector : TextureInspector
	{
		private static readonly string[] kSizes = new string[]
		{
			"16",
			"32",
			"64",
			"128",
			"256",
			"512",
			"1024",
			"2048"
		};

		private static readonly int[] kSizesValues = new int[]
		{
			16,
			32,
			64,
			128,
			256,
			512,
			1024,
			2048
		};

		private const int kTextureSize = 64;

		private Texture2D[] m_Images;

		protected override void OnEnable()
		{
			base.OnEnable();
			this.InitTexturesFromCubemap();
		}

		protected override void OnDisable()
		{
			base.OnDisable();
			if (this.m_Images != null)
			{
				for (int i = 0; i < this.m_Images.Length; i++)
				{
					if (this.m_Images[i] && !EditorUtility.IsPersistent(this.m_Images[i]))
					{
						UnityEngine.Object.DestroyImmediate(this.m_Images[i]);
					}
				}
			}
			this.m_Images = null;
		}

		private void InitTexturesFromCubemap()
		{
			Cubemap cubemap = base.target as Cubemap;
			if (cubemap != null)
			{
				if (this.m_Images == null)
				{
					this.m_Images = new Texture2D[6];
				}
				for (int i = 0; i < this.m_Images.Length; i++)
				{
					if (this.m_Images[i] && !EditorUtility.IsPersistent(this.m_Images[i]))
					{
						UnityEngine.Object.DestroyImmediate(this.m_Images[i]);
					}
					if (TextureUtil.GetSourceTexture(cubemap, (CubemapFace)i))
					{
						this.m_Images[i] = TextureUtil.GetSourceTexture(cubemap, (CubemapFace)i);
					}
					else
					{
						this.m_Images[i] = new Texture2D(64, 64, TextureFormat.RGBA32, false);
						this.m_Images[i].hideFlags = HideFlags.HideAndDontSave;
						TextureUtil.CopyCubemapFaceIntoTexture(cubemap, (CubemapFace)i, this.m_Images[i]);
					}
				}
			}
		}

		public override void OnInspectorGUI()
		{
			if (this.m_Images == null)
			{
				this.InitTexturesFromCubemap();
			}
			EditorGUIUtility.labelWidth = 50f;
			Cubemap cubemap = base.target as Cubemap;
			if (!(cubemap == null))
			{
				GUILayout.BeginVertical(new GUILayoutOption[0]);
				GUILayout.BeginHorizontal(new GUILayoutOption[0]);
				this.ShowFace("Right\n(+X)", CubemapFace.PositiveX);
				this.ShowFace("Left\n(-X)", CubemapFace.NegativeX);
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal(new GUILayoutOption[0]);
				this.ShowFace("Top\n(+Y)", CubemapFace.PositiveY);
				this.ShowFace("Bottom\n(-Y)", CubemapFace.NegativeY);
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal(new GUILayoutOption[0]);
				this.ShowFace("Front\n(+Z)", CubemapFace.PositiveZ);
				this.ShowFace("Back\n(-Z)", CubemapFace.NegativeZ);
				GUILayout.EndHorizontal();
				GUILayout.EndVertical();
				EditorGUIUtility.labelWidth = 0f;
				EditorGUILayout.Space();
				EditorGUI.BeginChangeCheck();
				EditorGUILayout.HelpBox("Lowering face size is a destructive operation, you might need to re-assign the textures later to fix resolution issues. It's preferable to use Cubemap texture import type instead of Legacy Cubemap assets.", MessageType.Warning);
				int num = TextureUtil.GetGPUWidth(cubemap);
				num = EditorGUILayout.IntPopup("Face size", num, CubemapInspector.kSizes, CubemapInspector.kSizesValues, new GUILayoutOption[0]);
				int mipmapCount = TextureUtil.GetMipmapCount(cubemap);
				bool useMipmap = EditorGUILayout.Toggle("MipMaps", mipmapCount > 1, new GUILayoutOption[0]);
				bool flag = TextureUtil.GetLinearSampled(cubemap);
				flag = EditorGUILayout.Toggle("Linear", flag, new GUILayoutOption[0]);
				bool flag2 = TextureUtil.IsCubemapReadable(cubemap);
				flag2 = EditorGUILayout.Toggle("Readable", flag2, new GUILayoutOption[0]);
				if (EditorGUI.EndChangeCheck())
				{
					if (TextureUtil.ReformatCubemap(ref cubemap, num, num, cubemap.format, useMipmap, flag))
					{
						this.InitTexturesFromCubemap();
					}
					TextureUtil.MarkCubemapReadable(cubemap, flag2);
					cubemap.Apply();
				}
			}
		}

		internal override void OnAssetStoreInspectorGUI()
		{
			this.OnInspectorGUI();
		}

		private void ShowFace(string label, CubemapFace face)
		{
			Cubemap cubemapRef = base.target as Cubemap;
			GUI.changed = false;
			Texture2D texture2D = (Texture2D)CubemapInspector.ObjectField(label, this.m_Images[(int)face], typeof(Texture2D), false, new GUILayoutOption[0]);
			if (GUI.changed)
			{
				TextureUtil.CopyTextureIntoCubemapFace(texture2D, cubemapRef, face);
				this.m_Images[(int)face] = texture2D;
			}
		}

		public static UnityEngine.Object ObjectField(string label, UnityEngine.Object obj, Type objType, bool allowSceneObjects, params GUILayoutOption[] options)
		{
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			Rect position = GUILayoutUtility.GetRect(EditorGUIUtility.labelWidth, 32f, EditorStyles.label, new GUILayoutOption[]
			{
				GUILayout.ExpandWidth(false)
			});
			GUI.Label(position, label, EditorStyles.label);
			position = GUILayoutUtility.GetAspectRect(1f, EditorStyles.objectField, new GUILayoutOption[]
			{
				GUILayout.Width(64f)
			});
			UnityEngine.Object result = EditorGUI.ObjectField(position, obj, objType, allowSceneObjects);
			GUILayout.EndHorizontal();
			return result;
		}
	}
}
