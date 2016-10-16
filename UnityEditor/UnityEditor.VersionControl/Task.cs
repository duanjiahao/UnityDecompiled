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
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern string text
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern string description
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern bool success
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern int secondsSpent
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern int progressPct
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern string progressMessage
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern int resultCode
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern Message[] messages
		{
			[WrapperlessIcall]
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

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Wait();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetCompletionAction(CompletionAction action);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern Asset[] Internal_GetAssetList();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern ChangeSet[] Internal_GetChangeSets();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern Message[] Internal_GetMessages();

		[ThreadAndSerializationSafe, WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Dispose();

		~Task()
		{
			this.Dispose();
		}
	}
}
