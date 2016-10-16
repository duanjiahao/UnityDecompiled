using System;

namespace UnityEditor.VersionControl
{
	public enum CompletionAction
	{
		UpdatePendingWindow = 1,
		OnChangeContentsPendingWindow,
		OnIncomingPendingWindow,
		OnChangeSetsPendingWindow,
		OnGotLatestPendingWindow,
		OnSubmittedChangeWindow,
		OnAddedChangeWindow,
		OnCheckoutCompleted
	}
}
