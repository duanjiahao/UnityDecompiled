using System;
using UnityEditor.Modules;
using UnityEngine;

namespace UnityEditor
{
	internal class EditorPluginImporterExtension : DefaultPluginImporterExtension
	{
		internal enum EditorPluginCPUArchitecture
		{
			AnyCPU,
			x86,
			x86_64
		}

		internal enum EditorPluginOSArchitecture
		{
			AnyOS,
			OSX,
			Windows,
			Linux
		}

		internal class EditorProperty : DefaultPluginImporterExtension.Property
		{
			public EditorProperty(GUIContent name, string key, object defaultValue) : base(name, key, defaultValue, BuildPipeline.GetEditorTargetName())
			{
			}

			internal override void Reset(PluginImporterInspector inspector)
			{
				string editorData = inspector.importer.GetEditorData(base.key);
				base.ParseStringValue(editorData);
			}

			internal override void Apply(PluginImporterInspector inspector)
			{
				inspector.importer.SetEditorData(base.key, base.value.ToString());
			}
		}

		private EditorPluginImporterExtension.EditorPluginCPUArchitecture cpu;

		private EditorPluginImporterExtension.EditorPluginOSArchitecture os;

		public EditorPluginImporterExtension() : base(EditorPluginImporterExtension.GetProperties())
		{
		}

		private static DefaultPluginImporterExtension.Property[] GetProperties()
		{
			return new EditorPluginImporterExtension.EditorProperty[]
			{
				new EditorPluginImporterExtension.EditorProperty(EditorGUIUtility.TextContent("CPU|Is plugin compatible with 32bit or 64bit Editor?"), "CPU", EditorPluginImporterExtension.EditorPluginCPUArchitecture.AnyCPU),
				new EditorPluginImporterExtension.EditorProperty(EditorGUIUtility.TextContent("OS|Is plugin compatible with Windows, OS X or Linux Editor?"), "OS", EditorPluginImporterExtension.EditorPluginOSArchitecture.AnyOS)
			};
		}
	}
}
