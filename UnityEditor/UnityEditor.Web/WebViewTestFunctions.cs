using System;
using System.Linq;
using UnityEngine;

namespace UnityEditor.Web
{
	internal class WebViewTestFunctions
	{
		public int ReturnInt()
		{
			return 5;
		}

		public string ReturnString()
		{
			return "Five";
		}

		public bool ReturnBool()
		{
			return true;
		}

		public int[] ReturnNumberArray()
		{
			return new int[]
			{
				1,
				2,
				3
			};
		}

		public string[] ReturnStringArray()
		{
			return new string[]
			{
				"One",
				"Two",
				"Three"
			};
		}

		public bool[] ReturnBoolArray()
		{
			return new bool[]
			{
				true,
				false,
				true
			};
		}

		public TestObject ReturnObject()
		{
			return new TestObject
			{
				NumberProperty = 5,
				StringProperty = "Five",
				BoolProperty = true
			};
		}

		public void AcceptInt(int passedInt)
		{
			Debug.Log("A value was passed from JS: " + passedInt);
		}

		public void AcceptString(string passedString)
		{
			Debug.Log("A value was passed from JS: " + passedString);
		}

		public void AcceptBool(bool passedBool)
		{
			Debug.Log("A value was passed from JS: " + passedBool);
		}

		public void AcceptIntArray(int[] passedArray)
		{
			Debug.Log("An array was passed from the JS. Array elements were:");
			for (int i = 0; i <= passedArray.Length; i++)
			{
				Debug.Log(string.Concat(new object[]
				{
					"Element at index ",
					i,
					": ",
					passedArray[i]
				}));
			}
		}

		public void AcceptStringArray(string[] passedArray)
		{
			Debug.Log("An array was passed from the JS. Array elements were:");
			for (int i = 0; i <= passedArray.Length; i++)
			{
				Debug.Log(string.Concat(new object[]
				{
					"Element at index ",
					i,
					": ",
					passedArray[i]
				}));
			}
		}

		public void AcceptBoolArray(bool[] passedArray)
		{
			Debug.Log("An array was passed from the JS. Array elements were:");
			for (int i = 1; i <= passedArray.Length; i++)
			{
				Debug.Log(string.Concat(new object[]
				{
					"Element at index ",
					i,
					": ",
					passedArray[i]
				}));
			}
		}

		public void AcceptTestObject(TestObject passedObject)
		{
			Debug.Log("An object was passed from the JS. Properties were:");
			Debug.Log("StringProperty: " + passedObject.StringProperty);
			Debug.Log("NumberProperty: " + passedObject.NumberProperty);
			Debug.Log("BoolProperty: " + passedObject.BoolProperty);
		}

		public void VoidMethod(string logMessage)
		{
			Debug.Log("A method was called from the CEF: " + logMessage);
		}

		private string APrivateMethod(string input)
		{
			return "This method is private and not for CEF";
		}

		public string[] ArrayReverse(string[] input)
		{
			return (string[])input.Reverse<string>();
		}

		public void LogMessage(string message)
		{
			Debug.Log(message);
		}

		public static void RunTestScript(string path)
		{
			string sourcesPath = "file:///" + path;
			JSProxyMgr.GetInstance().AddGlobalObject("WebViewTestFunctions", new WebViewTestFunctions());
			WebViewEditorWindowTabs webViewEditorWindowTabs = WebViewEditorWindow.Create<WebViewEditorWindowTabs>("Test Window", sourcesPath, 0, 0, 0, 0);
			webViewEditorWindowTabs.OnBatchMode();
		}
	}
}
