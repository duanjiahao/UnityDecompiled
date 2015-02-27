using System;
namespace UnityEditor
{
	public struct UndoPropertyModification
	{
		public PropertyModification propertyModification;
		private int m_KeepPrefabOverride;
		public bool keepPrefabOverride
		{
			get
			{
				return this.m_KeepPrefabOverride != 0;
			}
			set
			{
				this.m_KeepPrefabOverride = ((!value) ? 0 : 1);
			}
		}
	}
}
