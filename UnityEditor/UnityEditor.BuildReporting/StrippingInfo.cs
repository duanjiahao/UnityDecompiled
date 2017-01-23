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

		public List<StrippingInfo.SerializedDependency> serializedDependencies;

		public List<string> modules = new List<string>();

		public List<int> serializedSizes = new List<int>();

		public Dictionary<string, HashSet<string>> dependencies = new Dictionary<string, HashSet<string>>();

		public Dictionary<string, int> sizes = new Dictionary<string, int>();

		public Dictionary<string, string> icons = new Dictionary<string, string>();

		public int totalSize = 0;

		private void OnEnable()
		{
			this.SetIcon("Required by Scripts", "class/MonoScript");
			this.SetIcon(StrippingInfo.ModuleName("AI"), "class/NavMeshAgent");
			this.SetIcon(StrippingInfo.ModuleName("Animation"), "class/Animation");
			this.SetIcon(StrippingInfo.ModuleName("Audio"), "class/AudioSource");
			this.SetIcon(StrippingInfo.ModuleName("Core"), "class/GameManager");
			this.SetIcon(StrippingInfo.ModuleName("IMGUI"), "class/GUILayer");
			this.SetIcon(StrippingInfo.ModuleName("ParticleSystem"), "class/ParticleSystem");
			this.SetIcon(StrippingInfo.ModuleName("ParticlesLegacy"), "class/EllipsoidParticleEmitter");
			this.SetIcon(StrippingInfo.ModuleName("Physics"), "class/PhysicMaterial");
			this.SetIcon(StrippingInfo.ModuleName("Physics2D"), "class/PhysicsMaterial2D");
			this.SetIcon(StrippingInfo.ModuleName("TextRendering"), "class/Font");
			this.SetIcon(StrippingInfo.ModuleName("UI"), "class/CanvasGroup");
			this.SetIcon(StrippingInfo.ModuleName("Umbra"), "class/OcclusionCullingSettings");
			this.SetIcon(StrippingInfo.ModuleName("UNET"), "class/NetworkTransform");
			this.SetIcon(StrippingInfo.ModuleName("Vehicles"), "class/WheelCollider");
			this.SetIcon(StrippingInfo.ModuleName("Cloth"), "class/Cloth");
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
			if (!this.icons.ContainsKey(module))
			{
				this.SetIcon(module, "class/DefaultAsset");
			}
		}

		public void SetIcon(string dependency, string icon)
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
			StrippingInfo result;
			if (report == null)
			{
				result = null;
			}
			else
			{
				StrippingInfo[] array = (StrippingInfo[])report.GetAppendices(typeof(StrippingInfo));
				if (array.Length > 0)
				{
					result = array[0];
				}
				else
				{
					StrippingInfo strippingInfo = ScriptableObject.CreateInstance<StrippingInfo>();
					report.AddAppendix(strippingInfo);
					result = strippingInfo;
				}
			}
			return result;
		}

		public static string ModuleName(string module)
		{
			return module + " Module";
		}
	}
}
