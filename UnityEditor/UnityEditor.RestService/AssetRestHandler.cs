using System;
using System.IO;
using System.Text;
using UnityEditorInternal;
using UnityEngine;

namespace UnityEditor.RestService
{
	internal class AssetRestHandler
	{
		internal class AssetHandler : Handler
		{
			protected override JSONValue HandleDelete(Request request, JSONValue payload)
			{
				string path = request.Url.Substring("/unity/".Length);
				if (!AssetDatabase.DeleteAsset(path))
				{
					throw new RestRequestException
					{
						HttpStatusCode = HttpStatusCode.InternalServerError,
						RestErrorString = "FailedDeletingAsset",
						RestErrorDescription = "DeleteAsset() returned false"
					};
				}
				return default(JSONValue);
			}

			protected override JSONValue HandlePost(Request request, JSONValue payload)
			{
				string text = payload.Get("action").AsString();
				if (text != null)
				{
					if (!(text == "move"))
					{
						if (!(text == "create"))
						{
							goto IL_CF;
						}
						string assetPath = request.Url.Substring("/unity/".Length);
						string text2 = payload.Get("contents").AsString();
						byte[] bytes = Convert.FromBase64String(text2);
						text2 = Encoding.UTF8.GetString(bytes);
						this.CreateAsset(assetPath, text2);
					}
					else
					{
						string from = request.Url.Substring("/unity/".Length);
						string to = payload.Get("newpath").AsString();
						this.MoveAsset(from, to);
					}
					return default(JSONValue);
				}
				IL_CF:
				throw new RestRequestException
				{
					HttpStatusCode = HttpStatusCode.BadRequest,
					RestErrorString = "Uknown action: " + text
				};
			}

			internal bool MoveAsset(string from, string to)
			{
				string text = AssetDatabase.MoveAsset(from, to);
				if (text.Length > 0)
				{
					throw new RestRequestException(HttpStatusCode.BadRequest, "MoveAsset failed with error: " + text);
				}
				return text.Length == 0;
			}

			internal void CreateAsset(string assetPath, string contents)
			{
				string fullPath = Path.GetFullPath(assetPath);
				try
				{
					using (StreamWriter streamWriter = new StreamWriter(File.OpenWrite(fullPath)))
					{
						streamWriter.Write(contents);
						streamWriter.Close();
					}
				}
				catch (Exception arg)
				{
					throw new RestRequestException(HttpStatusCode.BadRequest, "FailedCreatingAsset", "Caught exception: " + arg);
				}
			}

			protected override JSONValue HandleGet(Request request, JSONValue payload)
			{
				int num = request.Url.ToLowerInvariant().IndexOf("/assets/");
				string assetPath = request.Url.ToLowerInvariant().Substring(num + 1);
				return this.GetAssetText(assetPath);
			}

			internal JSONValue GetAssetText(string assetPath)
			{
				UnityEngine.Object @object = AssetDatabase.LoadAssetAtPath(assetPath, typeof(UnityEngine.Object));
				if (@object == null)
				{
					throw new RestRequestException(HttpStatusCode.BadRequest, "AssetNotFound");
				}
				JSONValue result = default(JSONValue);
				result["file"] = assetPath;
				result["contents"] = Convert.ToBase64String(Encoding.UTF8.GetBytes(@object.ToString()));
				return result;
			}
		}

		internal class LibraryHandler : Handler
		{
			protected override JSONValue HandleGet(Request request, JSONValue payload)
			{
				JSONValue result = default(JSONValue);
				result["assets"] = Handler.ToJSON(AssetDatabase.FindAssets("", new string[]
				{
					"Assets"
				}));
				return result;
			}
		}

		internal static void Register()
		{
			Router.RegisterHandler("/unity/assets", new AssetRestHandler.LibraryHandler());
			Router.RegisterHandler("/unity/assets/*", new AssetRestHandler.AssetHandler());
		}
	}
}
