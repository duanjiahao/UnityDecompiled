using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnityEditor
{
	internal class AssetBundleNameGUI
	{
		private class Styles
		{
			private static GUISkin s_DarkSkin = EditorGUIUtility.GetBuiltinSkin(EditorSkin.Scene);

			public static GUIStyle label = AssetBundleNameGUI.Styles.GetStyle("ControlLabel");

			public static GUIStyle popup = AssetBundleNameGUI.Styles.GetStyle("MiniPopup");

			public static GUIStyle textField = AssetBundleNameGUI.Styles.GetStyle("textField");

			public static Color cursorColor = AssetBundleNameGUI.Styles.s_DarkSkin.settings.cursorColor;

			private static GUIStyle GetStyle(string name)
			{
				return new GUIStyle(AssetBundleNameGUI.Styles.s_DarkSkin.GetStyle(name));
			}
		}

		private static readonly GUIContent kAssetBundleName = new GUIContent("AssetBundle");

		private static readonly int kAssetBundleNameFieldIdHash = "AssetBundleNameFieldHash".GetHashCode();

		private static readonly int kAssetBundleVariantFieldIdHash = "AssetBundleVariantFieldHash".GetHashCode();

		private bool m_ShowAssetBundleNameTextField;

		private bool m_ShowAssetBundleVariantTextField;

		public void OnAssetBundleNameGUI(IEnumerable<UnityEngine.Object> assets)
		{
			EditorGUIUtility.labelWidth = 90f;
			Rect rect = EditorGUILayout.GetControlRect(true, 16f, new GUILayoutOption[0]);
			Rect rect2 = rect;
			rect.width *= 0.8f;
			rect2.xMin += rect.width + 5f;
			int controlID = GUIUtility.GetControlID(AssetBundleNameGUI.kAssetBundleNameFieldIdHash, FocusType.Native, rect);
			rect = EditorGUI.PrefixLabel(rect, controlID, AssetBundleNameGUI.kAssetBundleName, AssetBundleNameGUI.Styles.label);
			if (this.m_ShowAssetBundleNameTextField)
			{
				this.AssetBundleTextField(rect, controlID, assets, false);
			}
			else
			{
				this.AssetBundlePopup(rect, controlID, assets, false);
			}
			controlID = GUIUtility.GetControlID(AssetBundleNameGUI.kAssetBundleVariantFieldIdHash, FocusType.Native, rect2);
			if (this.m_ShowAssetBundleVariantTextField)
			{
				this.AssetBundleTextField(rect2, controlID, assets, true);
			}
			else
			{
				this.AssetBundlePopup(rect2, controlID, assets, true);
			}
		}

		private void ShowNewAssetBundleField(bool isVariant)
		{
			this.m_ShowAssetBundleNameTextField = !isVariant;
			this.m_ShowAssetBundleVariantTextField = isVariant;
			EditorGUIUtility.editingTextField = true;
		}

		private void AssetBundleTextField(Rect rect, int id, IEnumerable<UnityEngine.Object> assets, bool isVariant)
		{
			Color cursorColor = GUI.skin.settings.cursorColor;
			GUI.skin.settings.cursorColor = AssetBundleNameGUI.Styles.cursorColor;
			EditorGUI.BeginChangeCheck();
			string name = EditorGUI.DelayedTextFieldInternal(rect, id, GUIContent.none, string.Empty, null, AssetBundleNameGUI.Styles.textField);
			if (EditorGUI.EndChangeCheck())
			{
				this.SetAssetBundleForAssets(assets, name, isVariant);
				this.ShowAssetBundlePopup();
			}
			GUI.skin.settings.cursorColor = cursorColor;
			if (!EditorGUI.IsEditingTextField() && Event.current.type != EventType.Layout)
			{
				this.ShowAssetBundlePopup();
			}
		}

		private void ShowAssetBundlePopup()
		{
			this.m_ShowAssetBundleNameTextField = false;
			this.m_ShowAssetBundleVariantTextField = false;
		}

		private void AssetBundlePopup(Rect rect, int id, IEnumerable<UnityEngine.Object> assets, bool isVariant)
		{
			List<string> list = new List<string>();
			list.Add("None");
			list.Add(string.Empty);
			bool flag;
			IEnumerable<string> assetBundlesFromAssets = this.GetAssetBundlesFromAssets(assets, isVariant, out flag);
			string[] collection = (!isVariant) ? AssetDatabase.GetAllAssetBundleNamesWithoutVariant() : AssetDatabase.GetAllAssetBundleVariants();
			list.AddRange(collection);
			list.Add(string.Empty);
			int count = list.Count;
			list.Add("New...");
			int num = -1;
			int num2 = -1;
			if (!isVariant)
			{
				num = list.Count;
				list.Add("Remove Unused Names");
				num2 = list.Count;
				if (assetBundlesFromAssets.Count<string>() != 0)
				{
					list.Add("Filter Selected Name" + ((!flag) ? string.Empty : "s"));
				}
			}
			int num3 = 0;
			string text = assetBundlesFromAssets.FirstOrDefault<string>();
			if (!string.IsNullOrEmpty(text))
			{
				num3 = list.IndexOf(text);
			}
			EditorGUI.BeginChangeCheck();
			EditorGUI.showMixedValue = flag;
			num3 = EditorGUI.DoPopup(rect, id, num3, EditorGUIUtility.TempContent(list.ToArray()), AssetBundleNameGUI.Styles.popup);
			EditorGUI.showMixedValue = false;
			if (EditorGUI.EndChangeCheck())
			{
				if (num3 == 0)
				{
					this.SetAssetBundleForAssets(assets, null, isVariant);
				}
				else if (num3 == count)
				{
					this.ShowNewAssetBundleField(isVariant);
				}
				else if (num3 == num)
				{
					AssetDatabase.RemoveUnusedAssetBundleNames();
				}
				else if (num3 == num2)
				{
					this.FilterSelected(assetBundlesFromAssets);
				}
				else
				{
					this.SetAssetBundleForAssets(assets, list[num3], isVariant);
				}
			}
		}

		private void FilterSelected(IEnumerable<string> assetBundleNames)
		{
			SearchFilter searchFilter = new SearchFilter();
			searchFilter.assetBundleNames = (from name in assetBundleNames
			where !string.IsNullOrEmpty(name)
			select name).ToArray<string>();
			if (ProjectBrowser.s_LastInteractedProjectBrowser != null)
			{
				ProjectBrowser.s_LastInteractedProjectBrowser.SetSearch(searchFilter);
			}
			else
			{
				Debug.LogWarning("No Project Browser found to apply AssetBundle filter.");
			}
		}

		private IEnumerable<string> GetAssetBundlesFromAssets(IEnumerable<UnityEngine.Object> assets, bool isVariant, out bool isMixed)
		{
			HashSet<string> hashSet = new HashSet<string>();
			string text = null;
			isMixed = false;
			foreach (UnityEngine.Object current in assets)
			{
				if (!(current is MonoScript))
				{
					AssetImporter atPath = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(current));
					if (!(atPath == null))
					{
						string text2 = (!isVariant) ? atPath.assetBundleName : atPath.assetBundleVariant;
						if (text != null && text != text2)
						{
							isMixed = true;
						}
						text = text2;
						if (!string.IsNullOrEmpty(text2))
						{
							hashSet.Add(text2);
						}
					}
				}
			}
			return hashSet;
		}

		private void SetAssetBundleForAssets(IEnumerable<UnityEngine.Object> assets, string name, bool isVariant)
		{
			bool flag = false;
			foreach (UnityEngine.Object current in assets)
			{
				if (!(current is MonoScript))
				{
					AssetImporter atPath = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(current));
					if (!(atPath == null))
					{
						if (isVariant)
						{
							atPath.assetBundleVariant = name;
						}
						else
						{
							atPath.assetBundleName = name;
						}
						flag = true;
					}
				}
			}
			if (flag)
			{
				EditorApplication.Internal_CallAssetBundleNameChanged();
			}
		}
	}
}
