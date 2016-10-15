using System;
using System.IO;
using UnityEditorInternal;
using UnityEngine;

namespace UnityEditor
{
	public class ScriptableSingleton<T> : ScriptableObject where T : ScriptableObject
	{
		private static T s_Instance;

		public static T instance
		{
			get
			{
				if (ScriptableSingleton<T>.s_Instance == null)
				{
					ScriptableSingleton<T>.CreateAndLoad();
				}
				return ScriptableSingleton<T>.s_Instance;
			}
		}

		protected ScriptableSingleton()
		{
			if (ScriptableSingleton<T>.s_Instance != null)
			{
				Debug.LogError("ScriptableSingleton already exists. Did you query the singleton in a constructor?");
			}
			else
			{
				ScriptableSingleton<T>.s_Instance = (this as T);
			}
		}

		private static void CreateAndLoad()
		{
			string filePath = ScriptableSingleton<T>.GetFilePath();
			if (!string.IsNullOrEmpty(filePath))
			{
				InternalEditorUtility.LoadSerializedFileAndForget(filePath);
			}
			if (ScriptableSingleton<T>.s_Instance == null)
			{
				T t = ScriptableObject.CreateInstance<T>();
				t.hideFlags = HideFlags.HideAndDontSave;
			}
		}

		protected virtual void Save(bool saveAsText)
		{
			if (ScriptableSingleton<T>.s_Instance == null)
			{
				Debug.Log("Cannot save ScriptableSingleton: no instance!");
				return;
			}
			string filePath = ScriptableSingleton<T>.GetFilePath();
			if (!string.IsNullOrEmpty(filePath))
			{
				string directoryName = Path.GetDirectoryName(filePath);
				if (!Directory.Exists(directoryName))
				{
					Directory.CreateDirectory(directoryName);
				}
				InternalEditorUtility.SaveToSerializedFileAndForget(new T[]
				{
					ScriptableSingleton<T>.s_Instance
				}, filePath, saveAsText);
			}
		}

		private static string GetFilePath()
		{
			Type typeFromHandle = typeof(T);
			object[] customAttributes = typeFromHandle.GetCustomAttributes(true);
			object[] array = customAttributes;
			for (int i = 0; i < array.Length; i++)
			{
				object obj = array[i];
				if (obj is FilePathAttribute)
				{
					FilePathAttribute filePathAttribute = obj as FilePathAttribute;
					return filePathAttribute.filepath;
				}
			}
			return null;
		}
	}
}
