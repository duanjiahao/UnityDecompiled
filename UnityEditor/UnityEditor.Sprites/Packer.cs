using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Internal;

namespace UnityEditor.Sprites
{
	public sealed class Packer
	{
		public enum Execution
		{
			Normal,
			ForceRegroup
		}

		public static string kDefaultPolicy = typeof(DefaultPackerPolicy).Name;

		private static string[] m_policies = null;

		private static string m_selectedPolicy = null;

		private static Dictionary<string, Type> m_policyTypeCache = null;

		public static extern string[] atlasNames
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static string[] Policies
		{
			get
			{
				Packer.RegenerateList();
				return Packer.m_policies;
			}
		}

		public static string SelectedPolicy
		{
			get
			{
				Packer.RegenerateList();
				return Packer.m_selectedPolicy;
			}
			set
			{
				Packer.RegenerateList();
				if (value == null)
				{
					throw new ArgumentNullException();
				}
				if (!Packer.m_policies.Contains(value))
				{
					throw new ArgumentException("Specified policy {0} is not in the policy list.", value);
				}
				Packer.SetSelectedPolicy(value);
			}
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern Texture2D[] GetTexturesForAtlas(string atlasName);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern Texture2D[] GetAlphaTexturesForAtlas(string atlasName);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void RebuildAtlasCacheIfNeeded(BuildTarget target, [DefaultValue("false")] bool displayProgressBar, [DefaultValue("Execution.Normal")] Packer.Execution execution);

		[ExcludeFromDocs]
		public static void RebuildAtlasCacheIfNeeded(BuildTarget target, bool displayProgressBar)
		{
			Packer.Execution execution = Packer.Execution.Normal;
			Packer.RebuildAtlasCacheIfNeeded(target, displayProgressBar, execution);
		}

		[ExcludeFromDocs]
		public static void RebuildAtlasCacheIfNeeded(BuildTarget target)
		{
			Packer.Execution execution = Packer.Execution.Normal;
			bool displayProgressBar = false;
			Packer.RebuildAtlasCacheIfNeeded(target, displayProgressBar, execution);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void GetAtlasDataForSprite(Sprite sprite, out string atlasName, [Writable] out Texture2D atlasTexture);

		private static void SetSelectedPolicy(string value)
		{
			Packer.m_selectedPolicy = value;
			PlayerSettings.spritePackerPolicy = Packer.m_selectedPolicy;
		}

		private static void RegenerateList()
		{
			if (Packer.m_policies != null)
			{
				return;
			}
			List<Type> list = new List<Type>();
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			Assembly[] array = assemblies;
			for (int i = 0; i < array.Length; i++)
			{
				Assembly assembly = array[i];
				try
				{
					Type[] types = assembly.GetTypes();
					Type[] array2 = types;
					for (int j = 0; j < array2.Length; j++)
					{
						Type type = array2[j];
						if (typeof(IPackerPolicy).IsAssignableFrom(type) && type != typeof(IPackerPolicy))
						{
							list.Add(type);
						}
					}
				}
				catch (Exception ex)
				{
					Debug.Log(string.Format("SpritePacker failed to get types from {0}. Error: {1}", assembly.FullName, ex.Message));
				}
			}
			Packer.m_policies = (from t in list
			select t.Name).ToArray<string>();
			Packer.m_policyTypeCache = new Dictionary<string, Type>();
			foreach (Type current in list)
			{
				if (Packer.m_policyTypeCache.ContainsKey(current.Name))
				{
					Type type2 = Packer.m_policyTypeCache[current.Name];
					Debug.LogError(string.Format("Duplicate Sprite Packer policies found: {0} and {1}. Please rename one.", current.FullName, type2.FullName));
				}
				else
				{
					Packer.m_policyTypeCache[current.Name] = current;
				}
			}
			Packer.m_selectedPolicy = ((!string.IsNullOrEmpty(PlayerSettings.spritePackerPolicy)) ? PlayerSettings.spritePackerPolicy : Packer.kDefaultPolicy);
			if (!Packer.m_policies.Contains(Packer.m_selectedPolicy))
			{
				Packer.SetSelectedPolicy(Packer.kDefaultPolicy);
			}
		}

		internal static string GetSelectedPolicyId()
		{
			Packer.RegenerateList();
			Type type = Packer.m_policyTypeCache[Packer.m_selectedPolicy];
			IPackerPolicy packerPolicy = Activator.CreateInstance(type) as IPackerPolicy;
			return string.Format("{0}::{1}", type.AssemblyQualifiedName, packerPolicy.GetVersion());
		}

		internal static void ExecuteSelectedPolicy(BuildTarget target, int[] textureImporterInstanceIDs)
		{
			Packer.RegenerateList();
			Type type = Packer.m_policyTypeCache[Packer.m_selectedPolicy];
			IPackerPolicy packerPolicy = Activator.CreateInstance(type) as IPackerPolicy;
			packerPolicy.OnGroupAtlases(target, new PackerJob(), textureImporterInstanceIDs);
		}

		internal static void SaveUnappliedTextureImporterSettings()
		{
			InspectorWindow[] allInspectorWindows = InspectorWindow.GetAllInspectorWindows();
			for (int i = 0; i < allInspectorWindows.Length; i++)
			{
				InspectorWindow inspectorWindow = allInspectorWindows[i];
				ActiveEditorTracker tracker = inspectorWindow.GetTracker();
				Editor[] activeEditors = tracker.activeEditors;
				for (int j = 0; j < activeEditors.Length; j++)
				{
					Editor editor = activeEditors[j];
					TextureImporterInspector textureImporterInspector = editor as TextureImporterInspector;
					if (!(textureImporterInspector == null))
					{
						if (textureImporterInspector.HasModified())
						{
							TextureImporter textureImporter = textureImporterInspector.target as TextureImporter;
							if (EditorUtility.DisplayDialog("Unapplied import settings", "Unapplied import settings for '" + textureImporter.assetPath + "'", "Apply", "Revert"))
							{
								textureImporterInspector.ApplyAndImport();
							}
						}
					}
				}
			}
		}
	}
}
