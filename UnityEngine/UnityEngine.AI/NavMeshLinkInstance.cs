using System;

namespace UnityEngine.AI
{
	public struct NavMeshLinkInstance
	{
		private int m_Handle;

		public bool valid
		{
			get
			{
				return this.m_Handle != 0 && NavMesh.IsValidLinkHandle(this.m_Handle);
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
				return NavMesh.InternalGetLinkOwner(this.id);
			}
			set
			{
				int ownerID = (!(value != null)) ? 0 : value.GetInstanceID();
				if (!NavMesh.InternalSetLinkOwner(this.id, ownerID))
				{
					Debug.LogError("Cannot set 'owner' on an invalid NavMeshLinkInstance");
				}
			}
		}

		public void Remove()
		{
			NavMesh.RemoveLinkInternal(this.id);
		}
	}
}
