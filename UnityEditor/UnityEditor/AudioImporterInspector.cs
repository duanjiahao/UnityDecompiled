using System;
using UnityEngine;
namespace UnityEditor
{
	[CustomEditor(typeof(AudioImporter))]
	internal class AudioImporterInspector : AssetImporterInspector
	{
		private AudioImporterFormat m_format;
		private int m_compressionBitrate;
		private AudioImporterLoadType m_loadType;
		private bool m_3D;
		private bool m_Hardware;
		private bool m_Loopable;
		private bool m_ForceToMono;
		private int m_durationMS;
		private int m_origchannels;
		private bool m_iscompressible;
		private AudioType m_type;
		private int m_maxbitrate;
		private int m_minbitrate;
		private AudioType m_compressedType;
		private static string[] formatLabels = new string[2];
		internal override bool HasModified()
		{
			AudioImporter audioImporter = this.target as AudioImporter;
			return audioImporter.format != this.m_format || audioImporter.compressionBitrate != this.m_compressionBitrate || audioImporter.loadType != this.m_loadType || audioImporter.threeD != this.m_3D || audioImporter.forceToMono != this.m_ForceToMono || audioImporter.hardware != this.m_Hardware || audioImporter.loopable != this.m_Loopable;
		}
		internal override void ResetValues()
		{
			AudioImporter audioImporter = this.target as AudioImporter;
			this.m_format = audioImporter.format;
			this.m_loadType = audioImporter.loadType;
			this.m_compressionBitrate = audioImporter.compressionBitrate;
			this.m_3D = audioImporter.threeD;
			this.m_ForceToMono = audioImporter.forceToMono;
			this.m_Hardware = audioImporter.hardware;
			this.m_Loopable = audioImporter.loopable;
			audioImporter.updateOrigData();
			this.m_durationMS = audioImporter.durationMS;
			this.m_origchannels = audioImporter.origChannelCount;
			this.m_iscompressible = audioImporter.origIsCompressible;
			this.m_type = audioImporter.origType;
			if (this.m_durationMS != 0)
			{
				this.m_compressedType = AudioUtil.GetPlatformConversionType(this.m_type, BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget), AudioImporterFormat.Compressed);
			}
			this.m_minbitrate = audioImporter.minBitrate(this.m_compressedType) / 1000;
			this.m_maxbitrate = audioImporter.maxBitrate(this.m_compressedType) / 1000;
		}
		internal override void Apply()
		{
			AudioImporter audioImporter = this.target as AudioImporter;
			audioImporter.format = this.m_format;
			audioImporter.loadType = this.m_loadType;
			audioImporter.compressionBitrate = this.m_compressionBitrate;
			audioImporter.threeD = this.m_3D;
			audioImporter.forceToMono = this.m_ForceToMono;
			audioImporter.hardware = this.m_Hardware;
			audioImporter.loopable = this.m_Loopable;
		}
		public override void OnInspectorGUI()
		{
			BuildTargetGroup buildTargetGroup = BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget);
			AudioImporter audioImporter = this.target as AudioImporter;
			if (audioImporter != null)
			{
				BuildTargetGroup buildTargetGroup2 = buildTargetGroup;
				if (buildTargetGroup2 != BuildTargetGroup.FlashPlayer)
				{
					if (buildTargetGroup2 != BuildTargetGroup.PSP2)
					{
						EditorGUI.BeginDisabledGroup(this.m_type == AudioType.MPEG || this.m_type == AudioType.OGGVORBIS || !this.m_iscompressible);
						AudioImporterInspector.formatLabels[0] = "Native (" + this.m_type + ")";
						AudioImporterInspector.formatLabels[1] = "Compressed (" + this.m_compressedType + ")";
						int num = EditorGUILayout.Popup("Audio Format", (int)(this.m_format + 1), AudioImporterInspector.formatLabels, new GUILayoutOption[0]);
						this.m_format = (AudioImporterFormat)(num - 1);
						EditorGUI.EndDisabledGroup();
						this.m_3D = EditorGUILayout.Toggle("3D Sound", this.m_3D, new GUILayoutOption[0]);
						bool flag = this.m_origchannels > 1 && (audioImporter.origIsMonoForcable || (this.m_format == AudioImporterFormat.Compressed && this.m_iscompressible));
						EditorGUI.BeginDisabledGroup(!flag);
						this.m_ForceToMono = EditorGUILayout.Toggle("Force to mono", this.m_ForceToMono, new GUILayoutOption[0]);
						EditorGUI.EndDisabledGroup();
						if (this.m_format == AudioImporterFormat.Compressed)
						{
							string[] displayedOptions = new string[]
							{
								"Decompress on load",
								"Compressed in memory",
								"Stream from disc"
							};
							this.m_loadType = (AudioImporterLoadType)EditorGUILayout.Popup("Load type", (int)this.m_loadType, displayedOptions, new GUILayoutOption[0]);
						}
						else
						{
							string[] displayedOptions = new string[]
							{
								"Load into memory",
								"Stream from disc"
							};
							int num2 = Mathf.Clamp(this.m_loadType - AudioImporterLoadType.CompressedInMemory, 0, 1);
							EditorGUI.BeginChangeCheck();
							num2 = EditorGUILayout.Popup("Load type", num2, displayedOptions, new GUILayoutOption[0]);
							if (EditorGUI.EndChangeCheck())
							{
								this.m_loadType = num2 + AudioImporterLoadType.CompressedInMemory;
							}
						}
						EditorGUI.BeginDisabledGroup(buildTargetGroup != BuildTargetGroup.iPhone || this.m_format != AudioImporterFormat.Compressed);
						this.m_Hardware = EditorGUILayout.Toggle("Hardware decoding", this.m_Hardware, new GUILayoutOption[0]);
						EditorGUI.EndDisabledGroup();
						EditorGUI.BeginDisabledGroup(!this.m_iscompressible || (this.m_compressedType != AudioType.MPEG && this.m_compressedType != AudioType.XMA) || this.m_format != AudioImporterFormat.Compressed);
						this.m_Loopable = EditorGUILayout.Toggle(new GUIContent("Gapless looping", "Perform special MPEG encoding and stretch to loop the sound perfectly"), this.m_Loopable, new GUILayoutOption[0]);
						EditorGUI.EndDisabledGroup();
						if (this.m_durationMS != 0)
						{
							EditorGUI.BeginDisabledGroup(this.m_format != AudioImporterFormat.Compressed);
							int compressionBitrate;
							if (buildTargetGroup == BuildTargetGroup.XBOX360)
							{
								compressionBitrate = (56 + 2 * this.GetQualityGUI()) * 1000;
							}
							else
							{
								compressionBitrate = this.GetBitRateGUI(audioImporter) * 1000;
							}
							if (GUI.changed)
							{
								this.m_compressionBitrate = compressionBitrate;
							}
							EditorGUI.EndDisabledGroup();
						}
						else
						{
							GUILayout.BeginHorizontal(new GUILayoutOption[0]);
							GUILayout.Label("Unity failed to import this audio file. Try to reimport.", new GUILayoutOption[0]);
							GUILayout.EndHorizontal();
						}
					}
					else
					{
						GUI.enabled = (this.m_type != AudioType.MPEG && this.m_type != AudioType.OGGVORBIS && this.m_iscompressible);
						AudioImporterInspector.formatLabels[0] = "Native (" + this.m_type + ")";
						AudioImporterInspector.formatLabels[1] = "Compressed (" + this.m_compressedType + ")";
						int num3 = EditorGUILayout.Popup("Audio Format", (int)(this.m_format + 1), AudioImporterInspector.formatLabels, new GUILayoutOption[0]);
						this.m_format = (AudioImporterFormat)(num3 - 1);
						GUI.enabled = true;
						this.m_3D = EditorGUILayout.Toggle("3D Sound", this.m_3D, new GUILayoutOption[0]);
						if (this.m_format == AudioImporterFormat.Compressed)
						{
							string[] displayedOptions2 = new string[]
							{
								"Decompress on load",
								"Compressed in memory",
								"Stream from disc"
							};
							this.m_loadType = (AudioImporterLoadType)EditorGUILayout.Popup("Load type", (int)this.m_loadType, displayedOptions2, new GUILayoutOption[0]);
						}
						else
						{
							string[] displayedOptions2 = new string[]
							{
								"Load into memory",
								"Stream from disc"
							};
							int num4 = Mathf.Clamp(this.m_loadType - AudioImporterLoadType.CompressedInMemory, 0, 1);
							EditorGUI.BeginChangeCheck();
							num4 = EditorGUILayout.Popup("Load type", num4, displayedOptions2, new GUILayoutOption[0]);
							if (EditorGUI.EndChangeCheck())
							{
								this.m_loadType = num4 + AudioImporterLoadType.CompressedInMemory;
							}
						}
						GUI.enabled = (this.m_iscompressible && (this.m_compressedType == AudioType.MPEG || this.m_compressedType == AudioType.XMA || this.m_compressedType == AudioType.VAG) && this.m_format == AudioImporterFormat.Compressed);
						this.m_Loopable = EditorGUILayout.Toggle(new GUIContent("Looping", "Encode file for looping"), this.m_Loopable, new GUILayoutOption[0]);
						GUI.enabled = true;
						if (this.m_durationMS == 0)
						{
							GUILayout.BeginHorizontal(new GUILayoutOption[0]);
							GUILayout.Label("Unity failed to import this audio file. Try to reimport.", new GUILayoutOption[0]);
							GUILayout.EndHorizontal();
						}
					}
				}
				else
				{
					this.m_3D = EditorGUILayout.Toggle("3D Sound", this.m_3D, new GUILayoutOption[0]);
				}
			}
			GUI.enabled = true;
			GUI.changed = false;
			base.ApplyRevertGUI();
			if (GUI.changed)
			{
				GUIUtility.ExitGUI();
			}
		}
		private int GetBitRateGUI(AudioImporter importer)
		{
			int num = (this.m_compressionBitrate >= 0) ? (this.m_compressionBitrate / 1000) : (importer.defaultBitrate / 1000);
			int result = EditorGUILayout.IntSlider("Compression (kbps)", num, this.m_minbitrate, this.m_maxbitrate, new GUILayoutOption[0]);
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.FlexibleSpace();
			if (this.m_format == AudioImporterFormat.Compressed)
			{
				float num2 = (float)this.m_durationMS / 1000f;
				int bytes = (int)(num2 * ((float)num * 1000f / 4f));
				GUILayout.Label(string.Format("New file size : {0:000}", EditorUtility.FormatBytes(bytes)), new GUILayoutOption[0]);
			}
			GUILayout.EndHorizontal();
			return result;
		}
		private int GetQualityGUI()
		{
			float num = (this.m_compressionBitrate >= 0) ? (((float)this.m_compressionBitrate / 1000f - 56f) / 2f) : 50f;
			int value = (int)Math.Round((double)num);
			return EditorGUILayout.IntSlider("XMA Quality (1-100)", value, 1, 100, new GUILayoutOption[0]);
		}
	}
}
