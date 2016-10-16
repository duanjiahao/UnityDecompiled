using System;
using UnityEngine.Networking.Types;

namespace UnityEngine.Networking.Match
{
	internal class SetMatchAttributesRequest : Request
	{
		public NetworkID networkId
		{
			get;
			set;
		}

		public bool isListed
		{
			get;
			set;
		}

		public override string ToString()
		{
			return UnityString.Format("[{0}]-networkId:{1},isListed:{2}", new object[]
			{
				base.ToString(),
				this.networkId.ToString("X"),
				this.isListed
			});
		}

		public override bool IsValid()
		{
			return base.IsValid() && this.networkId != NetworkID.Invalid;
		}
	}
}
