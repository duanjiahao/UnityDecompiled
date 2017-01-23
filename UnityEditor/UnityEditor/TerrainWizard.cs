using System;
using UnityEngine;

namespace UnityEditor
{
	internal class TerrainWizard : ScriptableWizard
	{
		internal const int kMaxResolution = 4097;

		protected Terrain m_Terrain;

		protected TerrainData terrainData
		{
			get
			{
				TerrainData result;
				if (this.m_Terrain != null)
				{
					result = this.m_Terrain.terrainData;
				}
				else
				{
					result = null;
				}
				return result;
			}
		}

		internal virtual void OnWizardUpdate()
		{
			base.isValid = true;
			base.errorString = "";
			if (this.m_Terrain == null || this.m_Terrain.terrainData == null)
			{
				base.isValid = false;
				base.errorString = "Terrain does not exist";
			}
		}

		internal void InitializeDefaults(Terrain terrain)
		{
			this.m_Terrain = terrain;
			this.OnWizardUpdate();
		}

		internal void FlushHeightmapModification()
		{
			this.m_Terrain.Flush();
		}

		internal static T DisplayTerrainWizard<T>(string title, string button) where T : TerrainWizard
		{
			T[] array = Resources.FindObjectsOfTypeAll<T>();
			T result;
			if (array.Length > 0)
			{
				T t = array[0];
				t.titleContent = EditorGUIUtility.TextContent(title);
				t.createButtonName = button;
				t.otherButtonName = "";
				t.Focus();
				result = t;
			}
			else
			{
				result = ScriptableWizard.DisplayWizard<T>(title, button);
			}
			return result;
		}
	}
}
