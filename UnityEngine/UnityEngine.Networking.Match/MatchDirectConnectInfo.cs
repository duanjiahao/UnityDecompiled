using System;
using System.Collections.Generic;
using UnityEngine.Networking.Types;

namespace UnityEngine.Networking.Match
{
	internal class MatchDirectConnectInfo : ResponseBase
	{
		public NodeID nodeId
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

		public HostPriority hostPriority
		{
			get;
			set;
		}

		public override string ToString()
		{
			return UnityString.Format("[{0}]-nodeId:{1},publicAddress:{2},privateAddress:{3},hostPriority:{4}", new object[]
			{
				base.ToString(),
				this.nodeId,
				this.publicAddress,
				this.privateAddress,
				this.hostPriority
			});
		}

		public override void Parse(object obj)
		{
			IDictionary<string, object> dictionary = obj as IDictionary<string, object>;
			if (dictionary != null)
			{
				this.nodeId = (NodeID)base.ParseJSONUInt16("nodeId", obj, dictionary);
				this.publicAddress = base.ParseJSONString("publicAddress", obj, dictionary);
				this.privateAddress = base.ParseJSONString("privateAddress", obj, dictionary);
				this.hostPriority = (HostPriority)base.ParseJSONInt32("hostPriority", obj, dictionary);
				return;
			}
			throw new FormatException("While parsing JSON response, found obj is not of type IDictionary<string,object>:" + obj.ToString());
		}
	}
}
