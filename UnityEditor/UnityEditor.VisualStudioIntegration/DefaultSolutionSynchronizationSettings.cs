using System;
namespace UnityEditor.VisualStudioIntegration
{
	internal class DefaultSolutionSynchronizationSettings : ISolutionSynchronizationSettings
	{
		public virtual int VisualStudioVersion
		{
			get
			{
				return 9;
			}
		}
		public virtual string SolutionTemplate
		{
			get
			{
				return string.Join("\r\n", new string[]
				{
					"Microsoft Visual Studio Solution File, Format Version {0}",
					"# Visual Studio 2008",
					string.Empty,
					"{1}",
					"Global",
					"\tGlobalSection(SolutionConfigurationPlatforms) = preSolution",
					"\t\tDebug|Any CPU = Debug|Any CPU",
					"\t\tRelease|Any CPU = Release|Any CPU",
					"\tEndGlobalSection",
					"\tGlobalSection(ProjectConfigurationPlatforms) = postSolution",
					"{2}",
					"\tEndGlobalSection",
					"\tGlobalSection(SolutionProperties) = preSolution",
					"\t\tHideSolutionNode = FALSE",
					"\tEndGlobalSection",
					"\t{3}",
					"EndGlobal",
					string.Empty
				});
			}
		}
		public virtual string SolutionProjectEntryTemplate
		{
			get
			{
				return string.Join("\r\n", new string[]
				{
					"Project(\"{{{0}}}\") = \"{1}\", \"{2}\", \"{{{3}}}\"",
					"EndProject"
				});
			}
		}
		public virtual string SolutionProjectConfigurationTemplate
		{
			get
			{
				return string.Join("\r\n", new string[]
				{
					"\t\t{{{0}}}.Debug|Any CPU.ActiveCfg = Debug|Any CPU",
					"\t\t{{{0}}}.Debug|Any CPU.Build.0 = Debug|Any CPU",
					"\t\t{{{0}}}.Release|Any CPU.ActiveCfg = Release|Any CPU",
					"\t\t{{{0}}}.Release|Any CPU.Build.0 = Release|Any CPU"
				});
			}
		}
		public virtual string EditorAssemblyPath
		{
			get
			{
				return "/Managed/UnityEditor.dll";
			}
		}
		public virtual string EngineAssemblyPath
		{
			get
			{
				return "/Managed/UnityEngine.dll";
			}
		}
		public virtual string MonoLibFolder
		{
			get
			{
				return this.FrameworksPath() + "/Mono/lib/mono/unity/";
			}
		}
		public virtual string[] Defines
		{
			get
			{
				return new string[0];
			}
		}
		public virtual string GetProjectHeaderTemplate(ScriptingLanguage language)
		{
			return string.Join("\r\n", new string[]
			{
				"<?xml version=\"1.0\" encoding=\"utf-8\"?>",
				"<Project ToolsVersion=\"{0}\" DefaultTargets=\"Build\" xmlns=\"{6}\">",
				"  <PropertyGroup>",
				"\t<Configuration Condition=\" '$(Configuration)' == '' \">Debug</Configuration>",
				"\t<Platform Condition=\" '$(Platform)' == '' \">AnyCPU</Platform>",
				"\t<ProductVersion>{1}</ProductVersion>",
				"\t<SchemaVersion>2.0</SchemaVersion>",
				"\t<ProjectGuid>{{{2}}}</ProjectGuid>",
				"\t<OutputType>Library</OutputType>",
				"\t<AppDesignerFolder>Properties</AppDesignerFolder>",
				"\t<RootNamespace></RootNamespace>",
				"\t<AssemblyName>{7}</AssemblyName>",
				"\t<TargetFrameworkVersion>v3.5</TargetFrameworkVersion>",
				"\t<FileAlignment>512</FileAlignment>",
				"\t<BaseDirectory>Assets</BaseDirectory>",
				"  </PropertyGroup>",
				"  <PropertyGroup Condition=\" '$(Configuration)|$(Platform)' == 'Debug|AnyCPU' \">",
				"\t<DebugSymbols>true</DebugSymbols>",
				"\t<DebugType>full</DebugType>",
				"\t<Optimize>false</Optimize>",
				"\t<OutputPath>Temp\\bin\\Debug\\</OutputPath>",
				"\t<DefineConstants>DEBUG;TRACE;{5}</DefineConstants>",
				"\t<ErrorReport>prompt</ErrorReport>",
				"\t<WarningLevel>4</WarningLevel>",
				"\t<NoWarn>0169</NoWarn>",
				"  </PropertyGroup>",
				"  <PropertyGroup Condition=\" '$(Configuration)|$(Platform)' == 'Release|AnyCPU' \">",
				"\t<DebugType>pdbonly</DebugType>",
				"\t<Optimize>true</Optimize>",
				"\t<OutputPath>Temp\\bin\\Release\\</OutputPath>",
				"\t<DefineConstants>TRACE</DefineConstants>",
				"\t<ErrorReport>prompt</ErrorReport>",
				"\t<WarningLevel>4</WarningLevel>",
				"\t<NoWarn>0169</NoWarn>",
				"  </PropertyGroup>",
				"  <ItemGroup>",
				"\t<Reference Include=\"System\" />",
				"    <Reference Include=\"System.XML\" />",
				"\t<Reference Include=\"System.Core\" />",
				"\t<Reference Include=\"System.Xml.Linq\" />",
				"\t<Reference Include=\"UnityEngine\">",
				"\t  <HintPath>{3}</HintPath>",
				"\t</Reference>",
				"\t<Reference Include=\"UnityEditor\">",
				"\t  <HintPath>{4}</HintPath>",
				"\t</Reference>",
				"  </ItemGroup>",
				"  <ItemGroup>",
				string.Empty
			});
		}
		public virtual string GetProjectFooterTemplate(ScriptingLanguage language)
		{
			return string.Join("\r\n", new string[]
			{
				"  </ItemGroup>",
				"  <Import Project=\"$(MSBuildToolsPath)\\Microsoft.CSharp.targets\" />",
				"  <!-- To modify your build process, add your task inside one of the targets below and uncomment it. ",
				"\t   Other similar extension points exist, see Microsoft.Common.targets.",
				"  <Target Name=\"BeforeBuild\">",
				"  </Target>",
				"  <Target Name=\"AfterBuild\">",
				"  </Target>",
				"  -->",
				"  {0}",
				"</Project>",
				string.Empty
			});
		}
		protected virtual string FrameworksPath()
		{
			return string.Empty;
		}
	}
}
