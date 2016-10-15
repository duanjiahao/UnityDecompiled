using System;
using System.Collections.Generic;
using UnityEditorInternal;

namespace UnityEditor
{
	internal class PurchaseResult : AssetStoreResultBase<PurchaseResult>
	{
		public enum Status
		{
			BasketNotEmpty,
			ServiceDisabled,
			AnonymousUser,
			PasswordMissing,
			PasswordWrong,
			PurchaseDeclined,
			Ok
		}

		public PurchaseResult.Status status;

		public int packageID;

		public string message;

		public PurchaseResult(AssetStoreResultBase<PurchaseResult>.Callback c) : base(c)
		{
		}

		protected override void Parse(Dictionary<string, JSONValue> dict)
		{
			this.packageID = int.Parse(dict["package_id"].AsString());
			this.message = ((!dict.ContainsKey("message")) ? null : dict["message"].AsString(true));
			string a = dict["status"].AsString(true);
			if (a == "basket-not-empty")
			{
				this.status = PurchaseResult.Status.BasketNotEmpty;
			}
			else if (a == "service-disabled")
			{
				this.status = PurchaseResult.Status.ServiceDisabled;
			}
			else if (a == "user-anonymous")
			{
				this.status = PurchaseResult.Status.AnonymousUser;
			}
			else if (a == "password-missing")
			{
				this.status = PurchaseResult.Status.PasswordMissing;
			}
			else if (a == "password-wrong")
			{
				this.status = PurchaseResult.Status.PasswordWrong;
			}
			else if (a == "purchase-declined")
			{
				this.status = PurchaseResult.Status.PurchaseDeclined;
			}
			else if (a == "ok")
			{
				this.status = PurchaseResult.Status.Ok;
			}
		}
	}
}
