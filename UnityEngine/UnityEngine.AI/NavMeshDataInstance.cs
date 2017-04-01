using System;

namespace UnityEngine.AI
{
	public struct NavMeshDataInstance
	{
		private int m_Handle;

		public bool valid
		{
			get
			{
				return this.m_Handle != 0 && NavMesh.IsValidNavMeshDataHandle(this.m_Handle);
			}
		}

		internal int id
		{
			get
			{
				return this.m_Handle;
			}
			set
			{
				this.m_Handle = value;
			}
		}

		public UnityEngine.Object owner
		{
			get
			{
				return NavMesh.InternalGetOwner(this.id);
			}
			set
			{
				int ownerID = (!(value != null)) ? 0 : value.GetInstanceID();
				if (!NavMesh.InternalSetOwner(this.id, ownerID))
				{
					Debug.LogError("Cannot set 'owner' on an invalid NavMeshDataInstance");
				}
			}
		}

		public void Remove()
		{
			NavMesh.RemoveNavMeshDataInternal(this.id);
		}
	}
}
