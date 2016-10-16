using System;
using UnityEngine.Networking.Types;

namespace UnityEngine.Networking.Match
{
	internal class DropConnectionRequest : Request
	{
		public NetworkID networkId
		{
			get;
			set;
		}

		public NodeID nodeId
		{
			get;
			set;
		}

		public override string ToString()
		{
			return UnityString.Format("[{0}]-networkId:0x{1},nodeId:0x{2}", new object[]
			{
				base.ToString(),
				this.networkId.ToString("X"),
				this.nodeId.ToString("X")
			});
		}

		public override bool IsValid()
		{
			return base.IsValid() && this.networkId != NetworkID.Invalid && this.nodeId != NodeID.Invalid;
		}
	}
}
