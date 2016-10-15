using System;
using System.Runtime.InteropServices;

namespace UnityEditor.Collaboration
{
	[StructLayout(LayoutKind.Sequential)]
	internal class Change
	{
		[Flags]
		public enum RevertableStates : ulong
		{
			Revertable = 1uL,
			NotRevertable = 2uL,
			Revertable_File = 4uL,
			Revertable_Folder = 8uL,
			Revertable_EmptyFolder = 16uL,
			NotRevertable_File = 32uL,
			NotRevertable_Folder = 64uL,
			NotRevertable_FileAdded = 128uL,
			NotRevertable_FolderAdded = 256uL,
			NotRevertable_FolderContainsAdd = 512uL,
			InvalidRevertableState = 2147483648uL
		}

		private string m_Path;

		private Collab.CollabStates m_State;

		private Change.RevertableStates m_RevertableState;

		private string m_RelatedTo;

		private string m_LocalStatus;

		private string m_RemoteStatus;

		private string m_ResolveStatus;

		public string path
		{
			get
			{
				return this.m_Path;
			}
		}

		public ulong state
		{
			get
			{
				return (ulong)this.m_State;
			}
		}

		public bool isRevertable
		{
			get
			{
				return (this.m_RevertableState & Change.RevertableStates.Revertable) == Change.RevertableStates.Revertable;
			}
		}

		public ulong revertableState
		{
			get
			{
				return (ulong)this.m_RevertableState;
			}
		}

		public string relatedTo
		{
			get
			{
				return this.m_RelatedTo;
			}
		}

		public bool isMeta
		{
			get
			{
				return (this.m_State & Collab.CollabStates.kCollabMetaFile) == Collab.CollabStates.kCollabMetaFile;
			}
		}

		public bool isConflict
		{
			get
			{
				return (this.m_State & Collab.CollabStates.kCollabConflicted) == Collab.CollabStates.kCollabConflicted || (this.m_State & Collab.CollabStates.kCollabPendingMerge) == Collab.CollabStates.kCollabPendingMerge;
			}
		}

		public bool isFolderMeta
		{
			get
			{
				return (this.m_State & Collab.CollabStates.kCollabFolderMetaFile) == Collab.CollabStates.kCollabFolderMetaFile;
			}
		}

		public bool isResolved
		{
			get
			{
				return (this.m_State & Collab.CollabStates.kCollabUseMine) == Collab.CollabStates.kCollabUseMine || (this.m_State & Collab.CollabStates.kCollabUseTheir) == Collab.CollabStates.kCollabUseTheir || (this.m_State & Collab.CollabStates.kCollabMerged) == Collab.CollabStates.kCollabMerged;
			}
		}

		public string localStatus
		{
			get
			{
				return this.m_LocalStatus;
			}
		}

		public string remoteStatus
		{
			get
			{
				return this.m_RemoteStatus;
			}
		}

		public string resolveStatus
		{
			get
			{
				return this.m_ResolveStatus;
			}
		}

		private Change()
		{
		}
	}
}
