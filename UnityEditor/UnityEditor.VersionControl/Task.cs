using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace UnityEditor.VersionControl
{
	public sealed class Task
	{
		private IntPtr m_thisDummy;

		public extern int userIdentifier
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern string text
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern string description
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern bool success
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern int secondsSpent
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern int progressPct
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern string progressMessage
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern int resultCode
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern Message[] messages
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public AssetList assetList
		{
			get
			{
				AssetList assetList = new AssetList();
				Asset[] array = this.Internal_GetAssetList();
				Asset[] array2 = array;
				for (int i = 0; i < array2.Length; i++)
				{
					Asset item = array2[i];
					assetList.Add(item);
				}
				return assetList;
			}
		}

		public ChangeSets changeSets
		{
			get
			{
				ChangeSets changeSets = new ChangeSets();
				ChangeSet[] array = this.Internal_GetChangeSets();
				ChangeSet[] array2 = array;
				for (int i = 0; i < array2.Length; i++)
				{
					ChangeSet item = array2[i];
					changeSets.Add(item);
				}
				return changeSets;
			}
		}

		internal Task()
		{
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Wait();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetCompletionAction(CompletionAction action);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern Asset[] Internal_GetAssetList();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern ChangeSet[] Internal_GetChangeSets();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern Message[] Internal_GetMessages();

		[ThreadAndSerializationSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Dispose();

		~Task()
		{
			this.Dispose();
		}
	}
}
