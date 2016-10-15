using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnityEditor
{
	internal class LabelGUI
	{
		private HashSet<UnityEngine.Object> m_CurrentAssetsSet;

		private PopupList.InputData m_AssetLabels;

		private string m_ChangedLabel;

		private bool m_CurrentChanged;

		private bool m_ChangeWasAdd;

		private bool m_IgnoreNextAssetLabelsChangedCall;

		private static Action<UnityEngine.Object> s_AssetLabelsForObjectChangedDelegates;

		private static int s_MaxShownLabels = 10;

		public void OnEnable()
		{
			LabelGUI.s_AssetLabelsForObjectChangedDelegates = (Action<UnityEngine.Object>)Delegate.Combine(LabelGUI.s_AssetLabelsForObjectChangedDelegates, new Action<UnityEngine.Object>(this.AssetLabelsChangedForObject));
		}

		public void OnDisable()
		{
			LabelGUI.s_AssetLabelsForObjectChangedDelegates = (Action<UnityEngine.Object>)Delegate.Remove(LabelGUI.s_AssetLabelsForObjectChangedDelegates, new Action<UnityEngine.Object>(this.AssetLabelsChangedForObject));
			this.SaveLabels();
		}

		public void OnLostFocus()
		{
			this.SaveLabels();
		}

		public void AssetLabelsChangedForObject(UnityEngine.Object asset)
		{
			if (!this.m_IgnoreNextAssetLabelsChangedCall && this.m_CurrentAssetsSet != null && this.m_CurrentAssetsSet.Contains(asset))
			{
				this.m_AssetLabels = null;
			}
			this.m_IgnoreNextAssetLabelsChangedCall = false;
		}

		public void SaveLabels()
		{
			if (this.m_CurrentChanged && this.m_AssetLabels != null && this.m_CurrentAssetsSet != null)
			{
				bool flag = false;
				foreach (UnityEngine.Object current in this.m_CurrentAssetsSet)
				{
					bool flag2 = false;
					string[] labels = AssetDatabase.GetLabels(current);
					List<string> list = labels.ToList<string>();
					if (this.m_ChangeWasAdd)
					{
						if (!list.Contains(this.m_ChangedLabel))
						{
							list.Add(this.m_ChangedLabel);
							flag2 = true;
						}
					}
					else if (list.Contains(this.m_ChangedLabel))
					{
						list.Remove(this.m_ChangedLabel);
						flag2 = true;
					}
					if (flag2)
					{
						AssetDatabase.SetLabels(current, list.ToArray());
						if (LabelGUI.s_AssetLabelsForObjectChangedDelegates != null)
						{
							this.m_IgnoreNextAssetLabelsChangedCall = true;
							LabelGUI.s_AssetLabelsForObjectChangedDelegates(current);
						}
						flag = true;
					}
				}
				if (flag)
				{
					EditorApplication.Internal_CallAssetLabelsHaveChanged();
				}
				this.m_CurrentChanged = false;
			}
		}

		public void AssetLabelListCallback(PopupList.ListElement element)
		{
			this.m_ChangedLabel = element.text;
			element.selected = !element.selected;
			this.m_ChangeWasAdd = element.selected;
			element.partiallySelected = false;
			this.m_CurrentChanged = true;
			this.SaveLabels();
			InspectorWindow.RepaintAllInspectors();
		}

		public void InitLabelCache(UnityEngine.Object[] assets)
		{
			HashSet<UnityEngine.Object> hashSet = new HashSet<UnityEngine.Object>(assets);
			if (this.m_CurrentAssetsSet == null || !this.m_CurrentAssetsSet.SetEquals(hashSet))
			{
				List<string> source;
				List<string> source2;
				this.GetLabelsForAssets(assets, out source, out source2);
				this.m_AssetLabels = new PopupList.InputData
				{
					m_CloseOnSelection = false,
					m_AllowCustom = true,
					m_OnSelectCallback = new PopupList.OnSelectCallback(this.AssetLabelListCallback),
					m_MaxCount = 15,
					m_SortAlphabetically = true
				};
				Dictionary<string, float> allLabels = AssetDatabase.GetAllLabels();
				foreach (KeyValuePair<string, float> pair in allLabels)
				{
					PopupList.ListElement listElement = this.m_AssetLabels.NewOrMatchingElement(pair.Key);
					if (listElement.filterScore < pair.Value)
					{
						listElement.filterScore = pair.Value;
					}
					listElement.selected = source.Any((string label) => string.Equals(label, pair.Key, StringComparison.OrdinalIgnoreCase));
					listElement.partiallySelected = source2.Any((string label) => string.Equals(label, pair.Key, StringComparison.OrdinalIgnoreCase));
				}
			}
			this.m_CurrentAssetsSet = hashSet;
			this.m_CurrentChanged = false;
		}

		public void OnLabelGUI(UnityEngine.Object[] assets)
		{
			this.InitLabelCache(assets);
			float num = 1f;
			float num2 = 2f;
			float pixels = 3f;
			float num3 = 5f;
			GUIStyle assetLabelIcon = EditorStyles.assetLabelIcon;
			float num4 = (float)assetLabelIcon.margin.left + assetLabelIcon.fixedWidth + num2;
			GUILayout.Space(pixels);
			Rect rect = GUILayoutUtility.GetRect(0f, 10240f, 0f, 0f);
			rect.width -= num4;
			EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayoutUtility.GetRect(num, num, 0f, 0f);
			this.DrawLabelList(false, rect.xMax);
			this.DrawLabelList(true, rect.xMax);
			GUILayout.FlexibleSpace();
			Rect rect2 = GUILayoutUtility.GetRect(assetLabelIcon.fixedWidth, assetLabelIcon.fixedWidth, assetLabelIcon.fixedHeight + num3, assetLabelIcon.fixedHeight + num3);
			rect2.x = rect.xMax + (float)assetLabelIcon.margin.left;
			if (EditorGUI.ButtonMouseDown(rect2, GUIContent.none, FocusType.Passive, assetLabelIcon))
			{
				PopupWindow.Show(rect2, new PopupList(this.m_AssetLabels));
			}
			EditorGUILayout.EndHorizontal();
		}

		private void DrawLabelList(bool partiallySelected, float xMax)
		{
			GUIStyle style = (!partiallySelected) ? EditorStyles.assetLabel : EditorStyles.assetLabelPartial;
			Event current = Event.current;
			foreach (GUIContent current2 in (from i in this.m_AssetLabels.m_ListElements
			where (!partiallySelected) ? i.selected : i.partiallySelected
			orderby i.text.ToLower()
			select i.m_Content).Take(LabelGUI.s_MaxShownLabels))
			{
				Rect rect = GUILayoutUtility.GetRect(current2, style);
				if (Event.current.type == EventType.Repaint && rect.xMax >= xMax)
				{
					break;
				}
				GUI.Label(rect, current2, style);
				if (rect.xMax <= xMax && current.type == EventType.MouseDown && rect.Contains(current.mousePosition) && current.button == 0 && GUI.enabled)
				{
					current.Use();
					rect.x = xMax;
					PopupWindow.Show(rect, new PopupList(this.m_AssetLabels, current2.text));
				}
			}
		}

		private void GetLabelsForAssets(UnityEngine.Object[] assets, out List<string> all, out List<string> partial)
		{
			all = new List<string>();
			partial = new List<string>();
			Dictionary<string, int> dictionary = new Dictionary<string, int>();
			for (int i = 0; i < assets.Length; i++)
			{
				UnityEngine.Object obj = assets[i];
				string[] labels = AssetDatabase.GetLabels(obj);
				string[] array = labels;
				for (int j = 0; j < array.Length; j++)
				{
					string key = array[j];
					dictionary[key] = ((!dictionary.ContainsKey(key)) ? 1 : (dictionary[key] + 1));
				}
			}
			foreach (KeyValuePair<string, int> current in dictionary)
			{
				List<string> list = (current.Value != assets.Length) ? partial : all;
				list.Add(current.Key);
			}
		}
	}
}
