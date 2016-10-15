using System;
using System.Collections.Generic;
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
				string text2 = text;
				if (text2 != null)
				{
					if (AssetRestHandler.AssetHandler.<>f__switch$map9 == null)
					{
						AssetRestHandler.AssetHandler.<>f__switch$map9 = new Dictionary<string, int>(2)
						{
							{
								"move",
								0
							},
							{
								"create",
								1
							}
						};
					}
					int num;
					if (AssetRestHandler.AssetHandler.<>f__switch$map9.TryGetValue(text2, out num))
					{
						if (num != 0)
						{
							if (num != 1)
							{
								goto IL_106;
							}
							string assetPath = request.Url.Substring("/unity/".Length);
							string text3 = payload.Get("contents").AsString();
							byte[] bytes = Convert.FromBase64String(text3);
							text3 = Encoding.UTF8.GetString(bytes);
							this.CreateAsset(assetPath, text3);
						}
						else
						{
							string from = request.Url.Substring("/unity/".Length);
							string to = payload.Get("newpath").AsString();
							this.MoveAsset(from, to);
						}
						return default(JSONValue);
					}
				}
				IL_106:
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
				result["assets"] = Handler.ToJSON(AssetDatabase.FindAssets(string.Empty, new string[]
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
