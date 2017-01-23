using System;
using UnityEngine;

namespace UnityEditor
{
	[CustomEditor(typeof(MovieImporter))]
	internal class MovieImporterInspector : AssetImporterInspector
	{
		private float m_quality;

		private float m_duration;

		private bool m_linearTexture;

		private static GUIContent linearTextureContent = EditorGUIUtility.TextContent("Bypass sRGB Sampling|Texture will not be converted from gamma space to linear when sampled. Enable for IMGUI textures and non-color textures.");

		internal override bool showImportedObject
		{
			get
			{
				return false;
			}
		}

		internal override bool HasModified()
		{
			MovieImporter movieImporter = base.target as MovieImporter;
			return movieImporter.quality != this.m_quality || movieImporter.linearTexture != this.m_linearTexture;
		}

		internal override void ResetValues()
		{
			MovieImporter movieImporter = base.target as MovieImporter;
			this.m_quality = movieImporter.quality;
			this.m_linearTexture = movieImporter.linearTexture;
			this.m_duration = movieImporter.duration;
		}

		internal override void Apply()
		{
			MovieImporter movieImporter = base.target as MovieImporter;
			movieImporter.quality = this.m_quality;
			movieImporter.linearTexture = this.m_linearTexture;
		}

		public override void OnInspectorGUI()
		{
			MovieImporter x = base.target as MovieImporter;
			if (x != null)
			{
				GUILayout.BeginVertical(new GUILayoutOption[0]);
				this.m_linearTexture = EditorGUILayout.Toggle(MovieImporterInspector.linearTextureContent, this.m_linearTexture, new GUILayoutOption[0]);
				int num = (int)(this.GetVideoBitrateForQuality((double)this.m_quality) + this.GetAudioBitrateForQuality((double)this.m_quality));
				float num2 = (float)(num / 8) * this.m_duration;
				float num3 = 1048576f;
				this.m_quality = EditorGUILayout.Slider("Quality", this.m_quality, 0f, 1f, new GUILayoutOption[0]);
				GUILayout.Label(string.Format("Approx. {0:0.00} " + ((num2 >= num3) ? "MB" : "kB") + ", {1} kbps", num2 / ((num2 >= num3) ? num3 : 1024f), num / 1000), EditorStyles.helpBox, new GUILayoutOption[0]);
				GUILayout.EndVertical();
			}
			base.ApplyRevertGUI();
			MovieTexture movieTexture = this.assetEditor.target as MovieTexture;
			if (movieTexture && movieTexture.loop)
			{
				EditorGUILayout.Space();
				movieTexture.loop = EditorGUILayout.Toggle("Loop", movieTexture.loop, new GUILayoutOption[0]);
				GUILayout.Label("The Loop setting in the Inspector is obsolete. Use the Scripting API to control looping instead.\n\nThe loop setting will be disabled on next re-import or by disabling it above.", EditorStyles.helpBox, new GUILayoutOption[0]);
			}
		}

		private double GetAudioBitrateForQuality(double f)
		{
			return 56000.0 + 200000.0 * f;
		}

		private double GetVideoBitrateForQuality(double f)
		{
			return 100000.0 + 8000000.0 * f;
		}

		private double GetAudioQualityForBitrate(double f)
		{
			return (f - 56000.0) / 200000.0;
		}

		private double GetVideoQualityForBitrate(double f)
		{
			return (f - 100000.0) / 8000000.0;
		}
	}
}
