using System;
using System.IO;
using UnityEditorInternal;
using UnityEngine;

namespace UnityEditor
{
	internal class ScriptableObjectSaveLoadHelper<T> where T : ScriptableObject
	{
		public string fileExtensionWithoutDot
		{
			get;
			private set;
		}

		private SaveType saveType
		{
			get;
			set;
		}

		public ScriptableObjectSaveLoadHelper(string fileExtensionWithoutDot, SaveType saveType)
		{
			this.saveType = saveType;
			this.fileExtensionWithoutDot = fileExtensionWithoutDot.TrimStart(new char[]
			{
				'.'
			});
		}

		public T Load(string filePath)
		{
			filePath = this.AppendFileExtensionIfNeeded(filePath);
			T result;
			if (!string.IsNullOrEmpty(filePath))
			{
				UnityEngine.Object[] array = InternalEditorUtility.LoadSerializedFileAndForget(filePath);
				if (array != null && array.Length > 0)
				{
					result = (array[0] as T);
					return result;
				}
			}
			result = (T)((object)null);
			return result;
		}

		public T Create()
		{
			return ScriptableObject.CreateInstance<T>();
		}

		public void Save(T t, string filePath)
		{
			if (t == null)
			{
				Debug.LogError("Cannot save scriptableObject: its null!");
			}
			else if (string.IsNullOrEmpty(filePath))
			{
				Debug.LogError("Invalid path: '" + filePath + "'");
			}
			else
			{
				string directoryName = Path.GetDirectoryName(filePath);
				if (!Directory.Exists(directoryName))
				{
					Directory.CreateDirectory(directoryName);
				}
				filePath = this.AppendFileExtensionIfNeeded(filePath);
				InternalEditorUtility.SaveToSerializedFileAndForget(new T[]
				{
					t
				}, filePath, this.saveType == SaveType.Text);
			}
		}

		public override string ToString()
		{
			return string.Format("{0}, {1}, {2}", this.fileExtensionWithoutDot, this.saveType);
		}

		private string AppendFileExtensionIfNeeded(string path)
		{
			string result;
			if (!Path.HasExtension(path) && !string.IsNullOrEmpty(this.fileExtensionWithoutDot))
			{
				result = path + "." + this.fileExtensionWithoutDot;
			}
			else
			{
				result = path;
			}
			return result;
		}
	}
}
