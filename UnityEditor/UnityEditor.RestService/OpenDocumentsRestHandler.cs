using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditorInternal;

namespace UnityEditor.RestService
{
	internal class OpenDocumentsRestHandler : Handler
	{
		protected override JSONValue HandlePost(Request request, JSONValue payload)
		{
			List<string> arg_57_0;
			if (payload.ContainsKey("documents"))
			{
				arg_57_0 = (from d in payload["documents"].AsList()
				select d.AsString()).ToList<string>();
			}
			else
			{
				arg_57_0 = new List<string>();
			}
			ScriptEditorSettings.OpenDocuments = arg_57_0;
			ScriptEditorSettings.Save();
			return default(JSONValue);
		}

		protected override JSONValue HandleGet(Request request, JSONValue payload)
		{
			JSONValue result = default(JSONValue);
			result["documents"] = Handler.ToJSON(ScriptEditorSettings.OpenDocuments);
			return result;
		}

		internal static void Register()
		{
			Router.RegisterHandler("/unity/opendocuments", new OpenDocumentsRestHandler());
		}
	}
}
