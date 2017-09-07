using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Scripting;

namespace UnityEditor.Experimental.AssetImporters
{
	public abstract class ScriptedImporter : AssetImporter
	{
		private struct ImportRequest
		{
			public readonly string m_AssetSourcePath;

			public readonly BuildTarget m_SelectedBuildTarget;
		}

		private struct ImportResult
		{
			public UnityEngine.Object[] m_Assets;

			public string[] m_AssetNames;

			public Texture2D[] m_Thumbnails;
		}

		[RequiredByNativeCode]
		private ScriptedImporter.ImportResult GenerateAssetData(ScriptedImporter.ImportRequest request)
		{
			AssetImportContext assetImportContext = new AssetImportContext
			{
				assetPath = request.m_AssetSourcePath,
				selectedBuildTarget = request.m_SelectedBuildTarget
			};
			this.OnImportAsset(assetImportContext);
			if (!assetImportContext.subAssets.Any((ImportedObject x) => x.mainAsset))
			{
				throw new Exception("Import failed/rejected as none of the sub assets was set as the 'main asset':" + assetImportContext.assetPath);
			}
			ScriptedImporter.ImportResult result = default(ScriptedImporter.ImportResult);
			result.m_Assets = (from x in assetImportContext.subAssets
			select x.asset).ToArray<UnityEngine.Object>();
			result.m_AssetNames = (from x in assetImportContext.subAssets
			select x.identifier).ToArray<string>();
			result.m_Thumbnails = (from x in assetImportContext.subAssets
			select x.thumbnail).ToArray<Texture2D>();
			return result;
		}

		public abstract void OnImportAsset(AssetImportContext ctx);

		[RequiredByNativeCode]
		internal static void RegisterScriptedImporters()
		{
			ArrayList arrayList = AttributeHelper.FindEditorClassesWithAttribute(typeof(ScriptedImporterAttribute));
			IEnumerator enumerator = arrayList.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					object current = enumerator.Current;
					Type type = current as Type;
					ScriptedImporterAttribute scriptedImporterAttribute = Attribute.GetCustomAttribute(type, typeof(ScriptedImporterAttribute)) as ScriptedImporterAttribute;
					SortedDictionary<string, bool> handledExtensionsByImporter = ScriptedImporter.GetHandledExtensionsByImporter(scriptedImporterAttribute);
					IEnumerator enumerator2 = arrayList.GetEnumerator();
					try
					{
						while (enumerator2.MoveNext())
						{
							object current2 = enumerator2.Current;
							if (current2 != current)
							{
								ScriptedImporterAttribute attribute = Attribute.GetCustomAttribute(current2 as Type, typeof(ScriptedImporterAttribute)) as ScriptedImporterAttribute;
								SortedDictionary<string, bool> handledExtensionsByImporter2 = ScriptedImporter.GetHandledExtensionsByImporter(attribute);
								foreach (KeyValuePair<string, bool> current3 in handledExtensionsByImporter2)
								{
									if (handledExtensionsByImporter.ContainsKey(current3.Key))
									{
										Debug.LogError(string.Format("Scripted importers {0} and {1} are targeting the {2} extension, rejecting both.", type.FullName, (current2 as Type).FullName, current3.Key));
										handledExtensionsByImporter.Remove(current3.Key);
									}
								}
							}
						}
					}
					finally
					{
						IDisposable disposable;
						if ((disposable = (enumerator2 as IDisposable)) != null)
						{
							disposable.Dispose();
						}
					}
					foreach (KeyValuePair<string, bool> current4 in handledExtensionsByImporter)
					{
						AssetImporter.RegisterImporter(type, scriptedImporterAttribute.version, scriptedImporterAttribute.importQueuePriority, current4.Key);
					}
				}
			}
			finally
			{
				IDisposable disposable2;
				if ((disposable2 = (enumerator as IDisposable)) != null)
				{
					disposable2.Dispose();
				}
			}
		}

		private static SortedDictionary<string, bool> GetHandledExtensionsByImporter(ScriptedImporterAttribute attribute)
		{
			SortedDictionary<string, bool> sortedDictionary = new SortedDictionary<string, bool>();
			if (attribute.fileExtensions != null)
			{
				string[] fileExtensions = attribute.fileExtensions;
				for (int i = 0; i < fileExtensions.Length; i++)
				{
					string text = fileExtensions[i];
					string text2 = text;
					if (text2.StartsWith("."))
					{
						text2 = text2.Substring(1);
					}
					sortedDictionary.Add(text2, true);
				}
			}
			return sortedDictionary;
		}
	}
}
