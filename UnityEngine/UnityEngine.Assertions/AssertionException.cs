using System;

namespace UnityEngine.Assertions
{
	public class AssertionException : Exception
	{
		private string m_UserMessage;

		public override string Message
		{
			get
			{
				string text = base.Message;
				if (this.m_UserMessage != null)
				{
					text = text + '\n' + this.m_UserMessage;
				}
				return text;
			}
		}

		public AssertionException(string message, string userMessage) : base(message)
		{
			this.m_UserMessage = userMessage;
		}
	}
}
