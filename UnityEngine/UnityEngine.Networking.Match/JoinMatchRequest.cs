using System;
using UnityEngine.Networking.Types;

namespace UnityEngine.Networking.Match
{
	internal class JoinMatchRequest : Request
	{
		public NetworkID networkId
		{
			get;
			set;
		}

		public string publicAddress
		{
			get;
			set;
		}

		public string privateAddress
		{
			get;
			set;
		}

		public int eloScore
		{
			get;
			set;
		}

		public string password
		{
			get;
			set;
		}

		public override string ToString()
		{
			return UnityString.Format("[{0}]-networkId:0x{1},publicAddress:{2},privateAddress:{3},eloScore:{4},HasPassword:{5}", new object[]
			{
				base.ToString(),
				this.networkId.ToString("X"),
				this.publicAddress,
				this.privateAddress,
				this.eloScore,
				(!string.IsNullOrEmpty(this.password)) ? "YES" : "NO"
			});
		}

		public override bool IsValid()
		{
			return base.IsValid() && this.networkId != NetworkID.Invalid;
		}
	}
}
