using System;
using System.Collections.Generic;
using UnityEngine.Networking.Types;

namespace UnityEngine.Networking.Match
{
	internal class DropConnectionResponse : Response
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
			return UnityString.Format("[{0}]-networkId:{1}", new object[]
			{
				base.ToString(),
				this.networkId.ToString("X")
			});
		}

		public override void Parse(object obj)
		{
			base.Parse(obj);
			IDictionary<string, object> dictionary = obj as IDictionary<string, object>;
			if (dictionary != null)
			{
				this.networkId = (NetworkID)base.ParseJSONUInt64("networkId", obj, dictionary);
				this.nodeId = (NodeID)base.ParseJSONUInt16("nodeId", obj, dictionary);
				return;
			}
			throw new FormatException("While parsing JSON response, found obj is not of type IDictionary<string,object>:" + obj.ToString());
		}
	}
}
