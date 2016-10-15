using System;
using UnityEngine.Networking.Types;

namespace UnityEngine.Networking.Match
{
	internal abstract class Request
	{
		public static readonly int currentVersion = 3;

		public int version
		{
			get;
			set;
		}

		public SourceID sourceId
		{
			get;
			set;
		}

		public string projectId
		{
			get;
			set;
		}

		public AppID appId
		{
			get;
			set;
		}

		public string accessTokenString
		{
			get;
			set;
		}

		public int domain
		{
			get;
			set;
		}

		public virtual bool IsValid()
		{
			return this.sourceId != SourceID.Invalid;
		}

		public override string ToString()
		{
			return UnityString.Format("[{0}]-SourceID:0x{1},projectId:{2},accessTokenString.IsEmpty:{3},domain:{4}", new object[]
			{
				base.ToString(),
				this.sourceId.ToString("X"),
				this.projectId,
				string.IsNullOrEmpty(this.accessTokenString),
				this.domain
			});
		}
	}
}
