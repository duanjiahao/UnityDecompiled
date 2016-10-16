using System;
using UnityEditorInternal;
using UnityEngine;

namespace UnityEditor
{
	[AttributeUsage(AttributeTargets.Class)]
	internal class FilePathAttribute : Attribute
	{
		public enum Location
		{
			PreferencesFolder,
			ProjectFolder
		}

		public string filepath
		{
			get;
			set;
		}

		public FilePathAttribute(string relativePath, FilePathAttribute.Location location)
		{
			if (string.IsNullOrEmpty(relativePath))
			{
				Debug.LogError("Invalid relative path! (its null or empty)");
				return;
			}
			if (relativePath[0] == '/')
			{
				relativePath = relativePath.Substring(1);
			}
			if (location == FilePathAttribute.Location.PreferencesFolder)
			{
				this.filepath = InternalEditorUtility.unityPreferencesFolder + "/" + relativePath;
			}
			else
			{
				this.filepath = relativePath;
			}
		}
	}
}
