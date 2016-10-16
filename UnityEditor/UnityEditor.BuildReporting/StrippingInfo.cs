using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEditor.BuildReporting
{
	internal class StrippingInfo : ScriptableObject, ISerializationCallbackReceiver
	{
		[Serializable]
		public struct SerializedDependency
		{
			[SerializeField]
			public string key;

			[SerializeField]
			public List<string> value;

			[SerializeField]
			public string icon;
		}

		public const string RequiredByScripts = "Required by Scripts";

		public const string RequiredByScenes = "Used in Scenes";

		public const string RequiredByModule = "Required by Module";

		public List<StrippingInfo.SerializedDependency> serializedDependencies;

		public List<string> modules = new List<string>();

		public List<int> serializedSizes = new List<int>();

		public Dictionary<string, HashSet<string>> dependencies = new Dictionary<string, HashSet<string>>();

		public Dictionary<string, int> sizes = new Dictionary<string, int>();

		public Dictionary<string, string> icons = new Dictionary<string, string>();

		public int totalSize;

		private void OnEnable()
		{
			this.SetIcon("Required by Scripts", "class/MonoScript");
			this.SetIcon("Used in Scenes", "class/SceneAsset");
			this.SetIcon("AI", "class/NavMeshAgent");
			this.SetIcon("Animation", "class/Animation");
			this.SetIcon("Audio", "class/AudioSource");
			this.SetIcon("Core", "class/GameManager");
			this.SetIcon("IMGUI", "class/GUILayer");
			this.SetIcon("ParticleSystem", "class/ParticleSystem");
			this.SetIcon("ParticlesLegacy", "class/EllipsoidParticleEmitter");
			this.SetIcon("Physics", "class/PhysicMaterial");
			this.SetIcon("Physics2D", "class/PhysicsMaterial2D");
			this.SetIcon("TextRendering", "class/Font");
			this.SetIcon("UI", "class/CanvasGroup");
			this.SetIcon("Umbra", "class/SceneSettings");
			this.SetIcon("UNET", "class/NetworkTransform");
		}

		public void OnBeforeSerialize()
		{
			this.serializedDependencies = new List<StrippingInfo.SerializedDependency>();
			foreach (KeyValuePair<string, HashSet<string>> current in this.dependencies)
			{
				List<string> list = new List<string>();
				foreach (string current2 in current.Value)
				{
					list.Add(current2);
				}
				StrippingInfo.SerializedDependency item;
				item.key = current.Key;
				item.value = list;
				item.icon = ((!this.icons.ContainsKey(current.Key)) ? "class/DefaultAsset" : this.icons[current.Key]);
				this.serializedDependencies.Add(item);
			}
			this.serializedSizes = new List<int>();
			foreach (string current3 in this.modules)
			{
				this.serializedSizes.Add((!this.sizes.ContainsKey(current3)) ? 0 : this.sizes[current3]);
			}
		}

		public void OnAfterDeserialize()
		{
			this.dependencies = new Dictionary<string, HashSet<string>>();
			this.icons = new Dictionary<string, string>();
			for (int i = 0; i < this.serializedDependencies.Count; i++)
			{
				HashSet<string> hashSet = new HashSet<string>();
				foreach (string current in this.serializedDependencies[i].value)
				{
					hashSet.Add(current);
				}
				this.dependencies.Add(this.serializedDependencies[i].key, hashSet);
				this.icons[this.serializedDependencies[i].key] = this.serializedDependencies[i].icon;
			}
			this.sizes = new Dictionary<string, int>();
			for (int j = 0; j < this.serializedSizes.Count; j++)
			{
				this.sizes[this.modules[j]] = this.serializedSizes[j];
			}
		}

		public void RegisterDependency(string obj, string depends)
		{
			if (!this.dependencies.ContainsKey(obj))
			{
				this.dependencies[obj] = new HashSet<string>();
			}
			this.dependencies[obj].Add(depends);
			if (!this.icons.ContainsKey(depends))
			{
				this.SetIcon(depends, "class/" + depends);
			}
		}

		public void AddModule(string module)
		{
			if (!this.modules.Contains(module))
			{
				this.modules.Add(module);
			}
			if (!this.sizes.ContainsKey(module))
			{
				this.sizes[module] = 0;
			}
		}

		private void SetIcon(string dependency, string icon)
		{
			this.icons[dependency] = icon;
			if (!this.dependencies.ContainsKey(dependency))
			{
				this.dependencies[dependency] = new HashSet<string>();
			}
		}

		public void AddModuleSize(string module, int size)
		{
			if (this.modules.Contains(module))
			{
				this.sizes[module] = size;
			}
		}

		public static StrippingInfo GetBuildReportData(BuildReport report)
		{
			if (report == null)
			{
				return null;
			}
			StrippingInfo[] array = (StrippingInfo[])report.GetAppendices(typeof(StrippingInfo));
			if (array.Length > 0)
			{
				return array[0];
			}
			StrippingInfo strippingInfo = ScriptableObject.CreateInstance<StrippingInfo>();
			report.AddAppendix(strippingInfo);
			return strippingInfo;
		}
	}
}
