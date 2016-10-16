using System;
using System.Collections.Generic;

namespace UnityEditorInternal
{
	internal struct JSONValue
	{
		private object data;

		public JSONValue this[string index]
		{
			get
			{
				Dictionary<string, JSONValue> dictionary = this.AsDict();
				return dictionary[index];
			}
			set
			{
				if (this.data == null)
				{
					this.data = new Dictionary<string, JSONValue>();
				}
				Dictionary<string, JSONValue> dictionary = this.AsDict();
				dictionary[index] = value;
			}
		}

		public JSONValue(object o)
		{
			this.data = o;
		}

		public bool IsString()
		{
			return this.data is string;
		}

		public bool IsFloat()
		{
			return this.data is float;
		}

		public bool IsList()
		{
			return this.data is List<JSONValue>;
		}

		public bool IsDict()
		{
			return this.data is Dictionary<string, JSONValue>;
		}

		public bool IsBool()
		{
			return this.data is bool;
		}

		public bool IsNull()
		{
			return this.data == null;
		}

		public object AsObject()
		{
			return this.data;
		}

		public string AsString(bool nothrow)
		{
			if (this.data is string)
			{
				return (string)this.data;
			}
			if (!nothrow)
			{
				throw new JSONTypeException("Tried to read non-string json value as string");
			}
			return string.Empty;
		}

		public string AsString()
		{
			return this.AsString(false);
		}

		public float AsFloat(bool nothrow)
		{
			if (this.data is float)
			{
				return (float)this.data;
			}
			if (!nothrow)
			{
				throw new JSONTypeException("Tried to read non-float json value as float");
			}
			return 0f;
		}

		public float AsFloat()
		{
			return this.AsFloat(false);
		}

		public bool AsBool(bool nothrow)
		{
			if (this.data is bool)
			{
				return (bool)this.data;
			}
			if (!nothrow)
			{
				throw new JSONTypeException("Tried to read non-bool json value as bool");
			}
			return false;
		}

		public bool AsBool()
		{
			return this.AsBool(false);
		}

		public List<JSONValue> AsList(bool nothrow)
		{
			if (this.data is List<JSONValue>)
			{
				return (List<JSONValue>)this.data;
			}
			if (!nothrow)
			{
				throw new JSONTypeException("Tried to read " + this.data.GetType().Name + " json value as list");
			}
			return null;
		}

		public List<JSONValue> AsList()
		{
			return this.AsList(false);
		}

		public Dictionary<string, JSONValue> AsDict(bool nothrow)
		{
			if (this.data is Dictionary<string, JSONValue>)
			{
				return (Dictionary<string, JSONValue>)this.data;
			}
			if (!nothrow)
			{
				throw new JSONTypeException("Tried to read non-dictionary json value as dictionary");
			}
			return null;
		}

		public Dictionary<string, JSONValue> AsDict()
		{
			return this.AsDict(false);
		}

		public static JSONValue NewString(string val)
		{
			return new JSONValue(val);
		}

		public static JSONValue NewFloat(float val)
		{
			return new JSONValue(val);
		}

		public static JSONValue NewDict()
		{
			return new JSONValue(new Dictionary<string, JSONValue>());
		}

		public static JSONValue NewList()
		{
			return new JSONValue(new List<JSONValue>());
		}

		public static JSONValue NewBool(bool val)
		{
			return new JSONValue(val);
		}

		public static JSONValue NewNull()
		{
			return new JSONValue(null);
		}

		public bool ContainsKey(string index)
		{
			return this.IsDict() && this.AsDict().ContainsKey(index);
		}

		public JSONValue Get(string key)
		{
			if (!this.IsDict())
			{
				return new JSONValue(null);
			}
			JSONValue result = this;
			string[] array = key.Split(new char[]
			{
				'.'
			});
			for (int i = 0; i < array.Length; i++)
			{
				string index = array[i];
				if (!result.ContainsKey(index))
				{
					return new JSONValue(null);
				}
				result = result[index];
			}
			return result;
		}

		public void Set(string key, string value)
		{
			if (value == null)
			{
				this[key] = JSONValue.NewNull();
				return;
			}
			this[key] = JSONValue.NewString(value);
		}

		public void Set(string key, float value)
		{
			this[key] = JSONValue.NewFloat(value);
		}

		public void Set(string key, bool value)
		{
			this[key] = JSONValue.NewBool(value);
		}

		public void Add(string value)
		{
			List<JSONValue> list = this.AsList();
			if (value == null)
			{
				list.Add(JSONValue.NewNull());
				return;
			}
			list.Add(JSONValue.NewString(value));
		}

		public void Add(float value)
		{
			List<JSONValue> list = this.AsList();
			list.Add(JSONValue.NewFloat(value));
		}

		public void Add(bool value)
		{
			List<JSONValue> list = this.AsList();
			list.Add(JSONValue.NewBool(value));
		}

		public override string ToString()
		{
			if (this.IsString())
			{
				return "\"" + JSONValue.EncodeString(this.AsString()) + "\"";
			}
			if (this.IsFloat())
			{
				return this.AsFloat().ToString();
			}
			if (this.IsList())
			{
				string str = "[";
				string str2 = string.Empty;
				foreach (JSONValue current in this.AsList())
				{
					str = str + str2 + current.ToString();
					str2 = ", ";
				}
				return str + "]";
			}
			if (this.IsDict())
			{
				string text = "{";
				string text2 = string.Empty;
				foreach (KeyValuePair<string, JSONValue> current2 in this.AsDict())
				{
					string text3 = text;
					text = string.Concat(new object[]
					{
						text3,
						text2,
						'"',
						JSONValue.EncodeString(current2.Key),
						"\" : ",
						current2.Value.ToString()
					});
					text2 = ", ";
				}
				return text + "}";
			}
			if (this.IsBool())
			{
				return (!this.AsBool()) ? "false" : "true";
			}
			if (this.IsNull())
			{
				return "null";
			}
			throw new JSONTypeException("Cannot serialize json value of unknown type");
		}

		private static string EncodeString(string str)
		{
			str = str.Replace("\"", "\\\"");
			str = str.Replace("\\", "\\\\");
			str = str.Replace("\b", "\\b");
			str = str.Replace("\f", "\\f");
			str = str.Replace("\n", "\\n");
			str = str.Replace("\r", "\\r");
			str = str.Replace("\t", "\\t");
			return str;
		}

		public static implicit operator JSONValue(string s)
		{
			return new JSONValue(s);
		}

		public static implicit operator JSONValue(float s)
		{
			return new JSONValue(s);
		}

		public static implicit operator JSONValue(bool s)
		{
			return new JSONValue(s);
		}

		public static implicit operator JSONValue(int s)
		{
			return new JSONValue((float)s);
		}
	}
}
