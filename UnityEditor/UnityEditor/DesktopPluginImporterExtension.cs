using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor.Modules;
using UnityEngine;

namespace UnityEditor
{
	internal class DesktopPluginImporterExtension : DefaultPluginImporterExtension
	{
		internal enum DesktopPluginCPUArchitecture
		{
			None,
			AnyCPU,
			x86,
			x86_64
		}

		internal class DesktopSingleCPUProperty : DefaultPluginImporterExtension.Property
		{
			public DesktopSingleCPUProperty(GUIContent name, string platformName) : this(name, platformName, DesktopPluginImporterExtension.DesktopPluginCPUArchitecture.AnyCPU)
			{
			}

			public DesktopSingleCPUProperty(GUIContent name, string platformName, DesktopPluginImporterExtension.DesktopPluginCPUArchitecture architecture) : base(name, "CPU", architecture, platformName)
			{
			}

			internal bool IsTargetEnabled(PluginImporterInspector inspector)
			{
				return inspector.GetCompatibleWithPlatform(base.platformName);
			}

			internal override void OnGUI(PluginImporterInspector inspector)
			{
				EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
				GUILayout.Space(10f);
				EditorGUI.BeginChangeCheck();
				bool flag = EditorGUILayout.Toggle(base.name, this.IsTargetEnabled(inspector), new GUILayoutOption[0]);
				if (EditorGUI.EndChangeCheck())
				{
					base.value = ((!flag) ? DesktopPluginImporterExtension.DesktopPluginCPUArchitecture.None : base.defaultValue);
					inspector.SetCompatibleWithPlatform(base.platformName, flag);
				}
				EditorGUILayout.EndHorizontal();
			}
		}

		private DesktopPluginImporterExtension.DesktopSingleCPUProperty m_WindowsX86;

		private DesktopPluginImporterExtension.DesktopSingleCPUProperty m_WindowsX86_X64;

		private DesktopPluginImporterExtension.DesktopSingleCPUProperty m_LinuxX86;

		private DesktopPluginImporterExtension.DesktopSingleCPUProperty m_LinuxX86_X64;

		private DesktopPluginImporterExtension.DesktopSingleCPUProperty m_OSXX86;

		private DesktopPluginImporterExtension.DesktopSingleCPUProperty m_OSXX86_X64;

		public DesktopPluginImporterExtension() : base(null)
		{
			this.properties = this.GetProperties();
		}

		private DefaultPluginImporterExtension.Property[] GetProperties()
		{
			List<DefaultPluginImporterExtension.Property> list = new List<DefaultPluginImporterExtension.Property>();
			this.m_WindowsX86 = new DesktopPluginImporterExtension.DesktopSingleCPUProperty(EditorGUIUtility.TextContent("x86"), BuildPipeline.GetBuildTargetName(BuildTarget.StandaloneWindows));
			this.m_WindowsX86_X64 = new DesktopPluginImporterExtension.DesktopSingleCPUProperty(EditorGUIUtility.TextContent("x86_x64"), BuildPipeline.GetBuildTargetName(BuildTarget.StandaloneWindows64));
			this.m_LinuxX86 = new DesktopPluginImporterExtension.DesktopSingleCPUProperty(EditorGUIUtility.TextContent("x86"), BuildPipeline.GetBuildTargetName(BuildTarget.StandaloneLinux), DesktopPluginImporterExtension.DesktopPluginCPUArchitecture.x86);
			this.m_LinuxX86_X64 = new DesktopPluginImporterExtension.DesktopSingleCPUProperty(EditorGUIUtility.TextContent("x86_x64"), BuildPipeline.GetBuildTargetName(BuildTarget.StandaloneLinux64), DesktopPluginImporterExtension.DesktopPluginCPUArchitecture.x86_64);
			this.m_OSXX86 = new DesktopPluginImporterExtension.DesktopSingleCPUProperty(EditorGUIUtility.TextContent("x86"), BuildPipeline.GetBuildTargetName(BuildTarget.StandaloneOSXIntel));
			this.m_OSXX86_X64 = new DesktopPluginImporterExtension.DesktopSingleCPUProperty(EditorGUIUtility.TextContent("x86_x64"), BuildPipeline.GetBuildTargetName(BuildTarget.StandaloneOSXIntel64));
			list.Add(this.m_WindowsX86);
			list.Add(this.m_WindowsX86_X64);
			list.Add(this.m_LinuxX86);
			list.Add(this.m_LinuxX86_X64);
			list.Add(this.m_OSXX86);
			list.Add(this.m_OSXX86_X64);
			return list.ToArray();
		}

		private DesktopPluginImporterExtension.DesktopPluginCPUArchitecture CalculateMultiCPUArchitecture(bool x86, bool x64)
		{
			if (x86 && x64)
			{
				return DesktopPluginImporterExtension.DesktopPluginCPUArchitecture.AnyCPU;
			}
			if (x86 && !x64)
			{
				return DesktopPluginImporterExtension.DesktopPluginCPUArchitecture.x86;
			}
			if (!x86 && x64)
			{
				return DesktopPluginImporterExtension.DesktopPluginCPUArchitecture.x86_64;
			}
			return DesktopPluginImporterExtension.DesktopPluginCPUArchitecture.None;
		}

		private bool IsUsableOnWindows(PluginImporter imp)
		{
			if (!imp.isNativePlugin)
			{
				return true;
			}
			string a = Path.GetExtension(imp.assetPath).ToLower();
			return a == ".dll";
		}

		private bool IsUsableOnOSX(PluginImporter imp)
		{
			if (!imp.isNativePlugin)
			{
				return true;
			}
			string a = Path.GetExtension(imp.assetPath).ToLower();
			return a == ".so" || a == ".bundle";
		}

		private bool IsUsableOnLinux(PluginImporter imp)
		{
			if (!imp.isNativePlugin)
			{
				return true;
			}
			string a = Path.GetExtension(imp.assetPath).ToLower();
			return a == ".so";
		}

		public override void OnPlatformSettingsGUI(PluginImporterInspector inspector)
		{
			PluginImporter importer = inspector.importer;
			EditorGUI.BeginChangeCheck();
			if (this.IsUsableOnWindows(importer))
			{
				EditorGUILayout.LabelField(EditorGUIUtility.TextContent("Windows"), EditorStyles.boldLabel, new GUILayoutOption[0]);
				this.m_WindowsX86.OnGUI(inspector);
				this.m_WindowsX86_X64.OnGUI(inspector);
				EditorGUILayout.Space();
			}
			if (this.IsUsableOnLinux(importer))
			{
				EditorGUILayout.LabelField(EditorGUIUtility.TextContent("Linux"), EditorStyles.boldLabel, new GUILayoutOption[0]);
				this.m_LinuxX86.OnGUI(inspector);
				this.m_LinuxX86_X64.OnGUI(inspector);
				EditorGUILayout.Space();
			}
			if (this.IsUsableOnOSX(importer))
			{
				EditorGUILayout.LabelField(EditorGUIUtility.TextContent("Mac OS X"), EditorStyles.boldLabel, new GUILayoutOption[0]);
				this.m_OSXX86.OnGUI(inspector);
				this.m_OSXX86_X64.OnGUI(inspector);
			}
			if (EditorGUI.EndChangeCheck())
			{
				this.ValidateUniversalTargets(inspector);
				this.hasModified = true;
			}
		}

		public void ValidateSingleCPUTargets(PluginImporterInspector inspector)
		{
			DesktopPluginImporterExtension.DesktopSingleCPUProperty[] array = new DesktopPluginImporterExtension.DesktopSingleCPUProperty[]
			{
				this.m_WindowsX86,
				this.m_WindowsX86_X64,
				this.m_LinuxX86,
				this.m_LinuxX86_X64,
				this.m_OSXX86,
				this.m_OSXX86_X64
			};
			DesktopPluginImporterExtension.DesktopSingleCPUProperty[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				DesktopPluginImporterExtension.DesktopSingleCPUProperty desktopSingleCPUProperty = array2[i];
				string value = (!desktopSingleCPUProperty.IsTargetEnabled(inspector)) ? DesktopPluginImporterExtension.DesktopPluginCPUArchitecture.None.ToString() : desktopSingleCPUProperty.defaultValue.ToString();
				inspector.importer.SetPlatformData(desktopSingleCPUProperty.platformName, "CPU", value);
			}
			this.ValidateUniversalTargets(inspector);
		}

		private void ValidateUniversalTargets(PluginImporterInspector inspector)
		{
			bool flag = this.m_LinuxX86.IsTargetEnabled(inspector);
			bool flag2 = this.m_LinuxX86_X64.IsTargetEnabled(inspector);
			DesktopPluginImporterExtension.DesktopPluginCPUArchitecture desktopPluginCPUArchitecture = this.CalculateMultiCPUArchitecture(flag, flag2);
			inspector.importer.SetPlatformData(BuildTarget.StandaloneLinuxUniversal, "CPU", desktopPluginCPUArchitecture.ToString());
			inspector.SetCompatibleWithPlatform(BuildPipeline.GetBuildTargetName(BuildTarget.StandaloneLinuxUniversal), flag || flag2);
			bool flag3 = this.m_OSXX86.IsTargetEnabled(inspector);
			bool flag4 = this.m_OSXX86_X64.IsTargetEnabled(inspector);
			DesktopPluginImporterExtension.DesktopPluginCPUArchitecture desktopPluginCPUArchitecture2 = this.CalculateMultiCPUArchitecture(flag3, flag4);
			inspector.importer.SetPlatformData(BuildTarget.StandaloneOSXUniversal, "CPU", desktopPluginCPUArchitecture2.ToString());
			inspector.SetCompatibleWithPlatform(BuildPipeline.GetBuildTargetName(BuildTarget.StandaloneOSXUniversal), flag3 || flag4);
		}

		public override string CalculateFinalPluginPath(string platformName, PluginImporter imp)
		{
			BuildTarget buildTargetByName = BuildPipeline.GetBuildTargetByName(platformName);
			bool flag = buildTargetByName == BuildTarget.StandaloneWindows || buildTargetByName == BuildTarget.StandaloneWindows64;
			bool flag2 = buildTargetByName == BuildTarget.StandaloneOSXIntel || buildTargetByName == BuildTarget.StandaloneOSXIntel64 || buildTargetByName == BuildTarget.StandaloneOSXUniversal;
			bool flag3 = buildTargetByName == BuildTarget.StandaloneLinux || buildTargetByName == BuildTarget.StandaloneLinux64 || buildTargetByName == BuildTarget.StandaloneLinuxUniversal;
			if (!flag3 && !flag2 && !flag)
			{
				throw new Exception(string.Format("Failed to resolve standalone platform, platform string '{0}', resolved target '{1}'", platformName, buildTargetByName.ToString()));
			}
			if (flag && !this.IsUsableOnWindows(imp))
			{
				return string.Empty;
			}
			if (flag2 && !this.IsUsableOnOSX(imp))
			{
				return string.Empty;
			}
			if (flag3 && !this.IsUsableOnLinux(imp))
			{
				return string.Empty;
			}
			string platformData = imp.GetPlatformData(platformName, "CPU");
			if (string.Compare(platformData, "None", true) == 0)
			{
				return string.Empty;
			}
			if (!string.IsNullOrEmpty(platformData) && string.Compare(platformData, "AnyCPU", true) != 0)
			{
				return Path.Combine(platformData, Path.GetFileName(imp.assetPath));
			}
			return Path.GetFileName(imp.assetPath);
		}
	}
}
