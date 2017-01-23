using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace UnityEditorInternal
{
	internal class ProfilerInstrumentationPopup : PopupWindowContent
	{
		private class InputData : PopupList.InputData
		{
			public override IEnumerable<PopupList.ListElement> BuildQuery(string prefix)
			{
				IEnumerable<PopupList.ListElement> result;
				if (prefix == "")
				{
					result = this.m_ListElements;
				}
				else
				{
					result = from element in this.m_ListElements
					where element.m_Content.text.Contains(prefix)
					select element;
				}
				return result;
			}
		}

		private const string kAutoInstrumentSettingKey = "ProfilerAutoInstrumentedAssemblyTypes";

		private const int kAutoInstrumentButtonHeight = 20;

		private const int kAutoInstrumentButtonsHeight = 20;

		private static GUIContent s_AutoInstrumentScriptsContent = new GUIContent("Auto instrument " + InstrumentedAssemblyTypes.Script.ToString() + " assemblies");

		private static Dictionary<string, int> s_InstrumentableFunctions;

		private static ProfilerInstrumentationPopup s_PendingPopup;

		private PopupList m_FunctionsList;

		private ProfilerInstrumentationPopup.InputData m_FunctionsListInputData;

		private bool m_ShowAllCheckbox;

		private bool m_ShowAutoInstrumemtationParams;

		private InstrumentedAssemblyTypes m_AutoInstrumentedAssemblyTypes;

		private PopupList.ListElement m_AllCheckbox;

		public static bool InstrumentationEnabled
		{
			get
			{
				return false;
			}
		}

		public ProfilerInstrumentationPopup(Dictionary<string, int> functions, bool showAllCheckbox, bool showAutoInstrumemtationParams)
		{
			this.m_ShowAutoInstrumemtationParams = showAutoInstrumemtationParams;
			this.m_ShowAllCheckbox = showAllCheckbox;
			this.m_AutoInstrumentedAssemblyTypes = (InstrumentedAssemblyTypes)SessionState.GetInt("ProfilerAutoInstrumentedAssemblyTypes", 0);
			this.m_FunctionsListInputData = new ProfilerInstrumentationPopup.InputData();
			this.m_FunctionsListInputData.m_CloseOnSelection = false;
			this.m_FunctionsListInputData.m_AllowCustom = true;
			this.m_FunctionsListInputData.m_MaxCount = 0;
			this.m_FunctionsListInputData.m_EnableAutoCompletion = false;
			this.m_FunctionsListInputData.m_SortAlphabetically = true;
			this.m_FunctionsListInputData.m_OnSelectCallback = new PopupList.OnSelectCallback(this.ProfilerInstrumentationPopupCallback);
			this.SetFunctions(functions);
			this.m_FunctionsList = new PopupList(this.m_FunctionsListInputData);
		}

		private void SetFunctions(Dictionary<string, int> functions)
		{
			this.m_FunctionsListInputData.m_ListElements.Clear();
			if (functions == null)
			{
				PopupList.ListElement listElement = this.m_FunctionsListInputData.NewOrMatchingElement("Querying instrumentable functions...");
				listElement.enabled = false;
			}
			else if (functions.Count == 0)
			{
				PopupList.ListElement listElement2 = this.m_FunctionsListInputData.NewOrMatchingElement("No instrumentable child functions found");
				listElement2.enabled = false;
			}
			else
			{
				this.m_FunctionsListInputData.m_MaxCount = Mathf.Clamp(functions.Count + 1, 0, 30);
				if (this.m_ShowAllCheckbox)
				{
					this.m_AllCheckbox = new PopupList.ListElement(" All", false, 3.40282347E+38f);
					this.m_FunctionsListInputData.m_ListElements.Add(this.m_AllCheckbox);
				}
				foreach (KeyValuePair<string, int> current in functions)
				{
					PopupList.ListElement listElement3 = new PopupList.ListElement(current.Key, current.Value != 0);
					listElement3.ResetScore();
					this.m_FunctionsListInputData.m_ListElements.Add(listElement3);
				}
				if (this.m_ShowAllCheckbox)
				{
					this.UpdateAllCheckbox();
				}
			}
		}

		public override void OnGUI(Rect rect)
		{
			Rect rect2 = new Rect(rect);
			if (this.m_ShowAutoInstrumemtationParams)
			{
				Rect position = new Rect(rect2);
				position.height = 20f;
				InstrumentedAssemblyTypes instrumentedAssemblyTypes = InstrumentedAssemblyTypes.None;
				if (GUI.Toggle(position, (this.m_AutoInstrumentedAssemblyTypes & InstrumentedAssemblyTypes.Script) != InstrumentedAssemblyTypes.None, ProfilerInstrumentationPopup.s_AutoInstrumentScriptsContent))
				{
					instrumentedAssemblyTypes |= InstrumentedAssemblyTypes.Script;
				}
				if (instrumentedAssemblyTypes != this.m_AutoInstrumentedAssemblyTypes)
				{
					this.m_AutoInstrumentedAssemblyTypes = instrumentedAssemblyTypes;
					ProfilerDriver.SetAutoInstrumentedAssemblies(this.m_AutoInstrumentedAssemblyTypes);
					SessionState.SetInt("ProfilerAutoInstrumentedAssemblyTypes", (int)this.m_AutoInstrumentedAssemblyTypes);
				}
				rect2.y += 20f;
				rect2.height -= 20f;
			}
			this.m_FunctionsList.OnGUI(rect2);
		}

		public override void OnClose()
		{
			this.m_FunctionsList.OnClose();
		}

		public override Vector2 GetWindowSize()
		{
			Vector2 windowSize = this.m_FunctionsList.GetWindowSize();
			windowSize.x = 450f;
			if (this.m_ShowAutoInstrumemtationParams)
			{
				windowSize.y += 20f;
			}
			return windowSize;
		}

		public void UpdateAllCheckbox()
		{
			if (this.m_AllCheckbox != null)
			{
				bool flag = false;
				bool flag2 = true;
				foreach (PopupList.ListElement current in this.m_FunctionsListInputData.m_ListElements)
				{
					if (current != this.m_AllCheckbox)
					{
						if (current.selected)
						{
							flag = true;
						}
						else
						{
							flag2 = false;
						}
					}
				}
				this.m_AllCheckbox.selected = flag2;
				this.m_AllCheckbox.partiallySelected = (flag && !flag2);
			}
		}

		private static void SetFunctionNamesFromUnity(bool allFunction, string[] functionNames, int[] isInstrumentedFlags)
		{
			Dictionary<string, int> dictionary = new Dictionary<string, int>(functionNames.Length);
			for (int i = 0; i < functionNames.Length; i++)
			{
				dictionary.Add(functionNames[i], isInstrumentedFlags[i]);
			}
			if (allFunction)
			{
				ProfilerInstrumentationPopup.s_InstrumentableFunctions = dictionary;
			}
			if (ProfilerInstrumentationPopup.s_PendingPopup != null)
			{
				ProfilerInstrumentationPopup.s_PendingPopup.SetFunctions(dictionary);
				ProfilerInstrumentationPopup.s_PendingPopup = null;
			}
		}

		public static void UpdateInstrumentableFunctions()
		{
			ProfilerDriver.QueryInstrumentableFunctions();
		}

		public static void Show(Rect r)
		{
			ProfilerInstrumentationPopup windowContent = new ProfilerInstrumentationPopup(ProfilerInstrumentationPopup.s_InstrumentableFunctions, false, true);
			if (ProfilerInstrumentationPopup.s_InstrumentableFunctions == null)
			{
				ProfilerInstrumentationPopup.s_PendingPopup = windowContent;
				ProfilerDriver.QueryInstrumentableFunctions();
			}
			else
			{
				ProfilerInstrumentationPopup.s_PendingPopup = null;
			}
			PopupWindow.Show(r, windowContent);
		}

		public static void Show(Rect r, string funcName)
		{
			ProfilerInstrumentationPopup windowContent = new ProfilerInstrumentationPopup(null, true, false);
			ProfilerInstrumentationPopup.s_PendingPopup = windowContent;
			ProfilerDriver.QueryFunctionCallees(funcName);
			PopupWindow.Show(r, windowContent);
		}

		public static bool FunctionHasInstrumentationPopup(string funcName)
		{
			return ProfilerInstrumentationPopup.s_InstrumentableFunctions != null && ProfilerInstrumentationPopup.s_InstrumentableFunctions.ContainsKey(funcName);
		}

		private void ProfilerInstrumentationPopupCallback(PopupList.ListElement element)
		{
			if (element == this.m_AllCheckbox)
			{
				element.selected = !element.selected;
				foreach (PopupList.ListElement current in this.m_FunctionsListInputData.m_ListElements)
				{
					if (element.selected)
					{
						ProfilerDriver.BeginInstrumentFunction(current.text);
					}
					else
					{
						ProfilerDriver.EndInstrumentFunction(current.text);
					}
					current.selected = element.selected;
				}
			}
			else
			{
				element.selected = !element.selected;
				if (element.selected)
				{
					ProfilerDriver.BeginInstrumentFunction(element.text);
				}
				else
				{
					ProfilerDriver.EndInstrumentFunction(element.text);
				}
			}
			this.UpdateAllCheckbox();
		}
	}
}
