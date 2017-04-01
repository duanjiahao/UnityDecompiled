using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace UnityEditor
{
	[StructLayout(LayoutKind.Sequential)]
	public sealed class EditorBuildSettingsScene : IComparable
	{
		private int m_Enabled;

		private string m_Path;

		private GUID m_GUID;

		public bool enabled
		{
			get
			{
				return this.m_Enabled != 0;
			}
			set
			{
				this.m_Enabled = ((!value) ? 0 : 1);
			}
		}

		public string path
		{
			get
			{
				return this.m_Path;
			}
			set
			{
				this.m_Path = value.Replace("\\", "/");
			}
		}

		public GUID guid
		{
			get
			{
				return this.m_GUID;
			}
			set
			{
				this.m_GUID = value;
			}
		}

		public EditorBuildSettingsScene()
		{
		}

		public EditorBuildSettingsScene(string path, bool enable)
		{
			this.m_Path = path.Replace("\\", "/");
			this.enabled = enable;
			GUID.TryParse(AssetDatabase.AssetPathToGUID(path), out this.m_GUID);
		}

		public EditorBuildSettingsScene(GUID guid, bool enable)
		{
			this.m_GUID = guid;
			this.enabled = enable;
			this.m_Path = AssetDatabase.GUIDToAssetPath(guid.ToString());
		}

		public int CompareTo(object obj)
		{
			if (obj is EditorBuildSettingsScene)
			{
				EditorBuildSettingsScene editorBuildSettingsScene = (EditorBuildSettingsScene)obj;
				return editorBuildSettingsScene.m_Path.CompareTo(this.m_Path);
			}
			throw new ArgumentException("object is not a EditorBuildSettingsScene");
		}

		public static string[] GetActiveSceneList(EditorBuildSettingsScene[] scenes)
		{
			return (from scene in scenes
			where scene.enabled
			select scene.path).ToArray<string>();
		}
	}
}
