using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace UnityEditor.Modules
{
	internal class DefaultPluginImporterExtension : IPluginImporterExtension
	{
		internal class Property
		{
			internal GUIContent name
			{
				get;
				set;
			}

			internal string key
			{
				get;
				set;
			}

			internal object defaultValue
			{
				get;
				set;
			}

			internal Type type
			{
				get;
				set;
			}

			internal string platformName
			{
				get;
				set;
			}

			internal object value
			{
				get;
				set;
			}

			internal Property(string name, string key, object defaultValue, string platformName) : this(new GUIContent(name), key, defaultValue, platformName)
			{
			}

			internal Property(GUIContent name, string key, object defaultValue, string platformName)
			{
				this.name = name;
				this.key = key;
				this.defaultValue = defaultValue;
				this.type = defaultValue.GetType();
				this.platformName = platformName;
			}

			internal virtual void Reset(PluginImporterInspector inspector)
			{
				string platformData = inspector.importer.GetPlatformData(this.platformName, this.key);
				try
				{
					this.value = TypeDescriptor.GetConverter(this.type).ConvertFromString(platformData);
				}
				catch
				{
					this.value = this.defaultValue;
					if (!string.IsNullOrEmpty(platformData))
					{
						Debug.LogWarning(string.Concat(new object[]
						{
							"Failed to parse value ('",
							platformData,
							"') for ",
							this.key,
							", platform: ",
							this.platformName,
							", type: ",
							this.type,
							". Default value will be set '",
							this.defaultValue,
							"'"
						}));
					}
				}
			}

			internal virtual void Apply(PluginImporterInspector inspector)
			{
				inspector.importer.SetPlatformData(this.platformName, this.key, this.value.ToString());
			}

			internal virtual void OnGUI(PluginImporterInspector inspector)
			{
				if (this.type == typeof(bool))
				{
					this.value = EditorGUILayout.Toggle(this.name, (bool)this.value, new GUILayoutOption[0]);
				}
				else if (this.type.IsEnum)
				{
					this.value = EditorGUILayout.EnumPopup(this.name, (Enum)this.value, new GUILayoutOption[0]);
				}
				else
				{
					if (this.type != typeof(string))
					{
						throw new NotImplementedException("Don't know how to display value.");
					}
					this.value = EditorGUILayout.TextField(this.name, (string)this.value, new GUILayoutOption[0]);
				}
			}
		}

		protected bool hasModified = false;

		protected DefaultPluginImporterExtension.Property[] properties = null;

		internal bool propertiesRefreshed = false;

		public DefaultPluginImporterExtension(DefaultPluginImporterExtension.Property[] properties)
		{
			this.properties = properties;
		}

		public virtual void ResetValues(PluginImporterInspector inspector)
		{
			this.hasModified = false;
			this.RefreshProperties(inspector);
		}

		public virtual bool HasModified(PluginImporterInspector inspector)
		{
			return this.hasModified;
		}

		public virtual void Apply(PluginImporterInspector inspector)
		{
			if (this.propertiesRefreshed)
			{
				DefaultPluginImporterExtension.Property[] array = this.properties;
				for (int i = 0; i < array.Length; i++)
				{
					DefaultPluginImporterExtension.Property property = array[i];
					property.Apply(inspector);
				}
			}
		}

		public virtual void OnEnable(PluginImporterInspector inspector)
		{
			this.RefreshProperties(inspector);
		}

		public virtual void OnDisable(PluginImporterInspector inspector)
		{
		}

		public virtual void OnPlatformSettingsGUI(PluginImporterInspector inspector)
		{
			if (!this.propertiesRefreshed)
			{
				this.RefreshProperties(inspector);
			}
			EditorGUI.BeginChangeCheck();
			DefaultPluginImporterExtension.Property[] array = this.properties;
			for (int i = 0; i < array.Length; i++)
			{
				DefaultPluginImporterExtension.Property property = array[i];
				property.OnGUI(inspector);
			}
			if (EditorGUI.EndChangeCheck())
			{
				this.hasModified = true;
			}
		}

		protected virtual void RefreshProperties(PluginImporterInspector inspector)
		{
			DefaultPluginImporterExtension.Property[] array = this.properties;
			for (int i = 0; i < array.Length; i++)
			{
				DefaultPluginImporterExtension.Property property = array[i];
				property.Reset(inspector);
			}
			this.propertiesRefreshed = true;
		}

		public virtual string CalculateFinalPluginPath(string platformName, PluginImporter imp)
		{
			return Path.GetFileName(imp.assetPath);
		}

		protected Dictionary<string, List<PluginImporter>> GetCompatiblePlugins(string buildTargetName)
		{
			PluginImporter[] array = (from imp in PluginImporter.GetAllImporters()
			where (imp.GetCompatibleWithPlatform(buildTargetName) || imp.GetCompatibleWithAnyPlatform()) && !string.IsNullOrEmpty(imp.assetPath)
			select imp).ToArray<PluginImporter>();
			Dictionary<string, List<PluginImporter>> dictionary = new Dictionary<string, List<PluginImporter>>();
			PluginImporter[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				PluginImporter pluginImporter = array2[i];
				if (!string.IsNullOrEmpty(pluginImporter.assetPath))
				{
					string text = this.CalculateFinalPluginPath(buildTargetName, pluginImporter);
					if (!string.IsNullOrEmpty(text))
					{
						List<PluginImporter> list = null;
						if (!dictionary.TryGetValue(text, out list))
						{
							list = new List<PluginImporter>();
							dictionary[text] = list;
						}
						list.Add(pluginImporter);
					}
				}
			}
			return dictionary;
		}

		public virtual bool CheckFileCollisions(string buildTargetName)
		{
			Dictionary<string, List<PluginImporter>> compatiblePlugins = this.GetCompatiblePlugins(buildTargetName);
			bool flag = false;
			StringBuilder stringBuilder = new StringBuilder();
			foreach (KeyValuePair<string, List<PluginImporter>> current in compatiblePlugins)
			{
				List<PluginImporter> value = current.Value;
				if (value.Count != 1)
				{
					int num = 0;
					foreach (PluginImporter current2 in value)
					{
						if (!current2.GetIsOverridable())
						{
							num++;
						}
					}
					if (num != 1)
					{
						flag = true;
						stringBuilder.AppendLine(string.Format("Plugin '{0}' is used from several locations:", Path.GetFileName(current.Key)));
						foreach (PluginImporter current3 in value)
						{
							stringBuilder.AppendLine(" " + current3.assetPath + " would be copied to <PluginPath>/" + current.Key.Replace("\\", "/"));
						}
					}
				}
			}
			if (flag)
			{
				stringBuilder.AppendLine("Please fix plugin settings and try again.");
				Debug.LogError(stringBuilder.ToString());
			}
			return flag;
		}
	}
}
