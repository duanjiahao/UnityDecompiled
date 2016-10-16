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
			if (!string.IsNullOrEmpty(filePath))
			{
				UnityEngine.Object[] array = InternalEditorUtility.LoadSerializedFileAndForget(filePath);
				if (array != null && array.Length > 0)
				{
					return array[0] as T;
				}
			}
			return (T)((object)null);
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
				return;
			}
			if (string.IsNullOrEmpty(filePath))
			{
				Debug.LogError("Invalid path: '" + filePath + "'");
				return;
			}
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

		public override string ToString()
		{
			return string.Format("{0}, {1}, {2}", this.fileExtensionWithoutDot, this.saveType);
		}

		private string AppendFileExtensionIfNeeded(string path)
		{
			if (!Path.HasExtension(path) && !string.IsNullOrEmpty(this.fileExtensionWithoutDot))
			{
				return path + "." + this.fileExtensionWithoutDot;
			}
			return path;
		}
	}
}
