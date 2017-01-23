using System;
using System.IO;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;

namespace UnityEditor
{
	[Serializable]
	internal class CreateAssetUtility
	{
		[SerializeField]
		private EndNameEditAction m_EndAction;

		[SerializeField]
		private int m_InstanceID;

		[SerializeField]
		private string m_Path = "";

		[SerializeField]
		private Texture2D m_Icon;

		[SerializeField]
		private string m_ResourceFile;

		public int instanceID
		{
			get
			{
				return this.m_InstanceID;
			}
		}

		public Texture2D icon
		{
			get
			{
				return this.m_Icon;
			}
		}

		public string folder
		{
			get
			{
				return Path.GetDirectoryName(this.m_Path);
			}
		}

		public string extension
		{
			get
			{
				return Path.GetExtension(this.m_Path);
			}
		}

		public string originalName
		{
			get
			{
				return Path.GetFileNameWithoutExtension(this.m_Path);
			}
		}

		public EndNameEditAction endAction
		{
			get
			{
				return this.m_EndAction;
			}
		}

		public void Clear()
		{
			this.m_EndAction = null;
			this.m_InstanceID = 0;
			this.m_Path = "";
			this.m_Icon = null;
			this.m_ResourceFile = "";
		}

		private static bool IsPathDataValid(string filePath)
		{
			bool result;
			if (string.IsNullOrEmpty(filePath))
			{
				result = false;
			}
			else
			{
				string directoryName = Path.GetDirectoryName(filePath);
				int mainAssetInstanceID = AssetDatabase.GetMainAssetInstanceID(directoryName);
				result = (mainAssetInstanceID != 0);
			}
			return result;
		}

		public bool BeginNewAssetCreation(int instanceID, EndNameEditAction newAssetEndAction, string filePath, Texture2D icon, string newAssetResourceFile)
		{
			string text;
			if (!filePath.StartsWith("assets/", StringComparison.CurrentCultureIgnoreCase))
			{
				text = AssetDatabase.GetUniquePathNameAtSelectedPath(filePath);
			}
			else
			{
				text = AssetDatabase.GenerateUniqueAssetPath(filePath);
			}
			bool result;
			if (!CreateAssetUtility.IsPathDataValid(text))
			{
				Debug.LogErrorFormat("Invalid generated unique path '{0}' (input path '{1}')", new object[]
				{
					text,
					filePath
				});
				this.Clear();
				result = false;
			}
			else
			{
				this.m_InstanceID = instanceID;
				this.m_Path = text;
				this.m_Icon = icon;
				this.m_EndAction = newAssetEndAction;
				this.m_ResourceFile = newAssetResourceFile;
				Selection.activeObject = EditorUtility.InstanceIDToObject(instanceID);
				result = true;
			}
			return result;
		}

		public void EndNewAssetCreation(string name)
		{
			string pathName = this.folder + "/" + name + this.extension;
			EndNameEditAction endAction = this.m_EndAction;
			int instanceID = this.m_InstanceID;
			string resourceFile = this.m_ResourceFile;
			this.Clear();
			ProjectWindowUtil.EndNameEditAction(endAction, instanceID, pathName, resourceFile);
		}

		public bool IsCreatingNewAsset()
		{
			return !string.IsNullOrEmpty(this.m_Path);
		}
	}
}
