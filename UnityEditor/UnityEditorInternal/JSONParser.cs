using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace UnityEditorInternal
{
	internal class JSONParser
	{
		private string json;

		private int line;

		private int linechar;

		private int len;

		private int idx;

		private int pctParsed;

		private char cur;

		private static char[] endcodes = new char[]
		{
			'\\',
			'"'
		};

		public JSONParser(string jsondata)
		{
			this.json = jsondata + "    ";
			this.line = 1;
			this.linechar = 1;
			this.len = this.json.Length;
			this.idx = 0;
			this.pctParsed = 0;
		}

		public static JSONValue SimpleParse(string jsondata)
		{
			JSONParser jSONParser = new JSONParser(jsondata);
			JSONValue result;
			try
			{
				result = jSONParser.Parse();
				return result;
			}
			catch (JSONParseException ex)
			{
				Debug.LogError(ex.Message);
			}
			result = new JSONValue(null);
			return result;
		}

		public JSONValue Parse()
		{
			this.cur = this.json[this.idx];
			return this.ParseValue();
		}

		private char Next()
		{
			if (this.cur == '\n')
			{
				this.line++;
				this.linechar = 0;
			}
			this.idx++;
			if (this.idx >= this.len)
			{
				throw new JSONParseException("End of json while parsing at " + this.PosMsg());
			}
			this.linechar++;
			int num = (int)((float)this.idx * 100f / (float)this.len);
			if (num != this.pctParsed)
			{
				this.pctParsed = num;
			}
			this.cur = this.json[this.idx];
			return this.cur;
		}

		private void SkipWs()
		{
			string text = " \n\t\r";
			while (text.IndexOf(this.cur) != -1)
			{
				this.Next();
			}
		}

		private string PosMsg()
		{
			return "line " + this.line.ToString() + ", column " + this.linechar.ToString();
		}

		private JSONValue ParseValue()
		{
			this.SkipWs();
			char c = this.cur;
			switch (c)
			{
			case '-':
			case '0':
			case '1':
			case '2':
			case '3':
			case '4':
			case '5':
			case '6':
			case '7':
			case '8':
			case '9':
			{
				JSONValue result = this.ParseNumber();
				return result;
			}
			case '.':
			case '/':
			{
				IL_4B:
				JSONValue result;
				if (c == '"')
				{
					result = this.ParseString();
					return result;
				}
				if (c == '[')
				{
					result = this.ParseArray();
					return result;
				}
				if (c == 'f' || c == 'n' || c == 't')
				{
					result = this.ParseConstant();
					return result;
				}
				if (c != '{')
				{
					throw new JSONParseException("Cannot parse json value starting with '" + this.json.Substring(this.idx, 5) + "' at " + this.PosMsg());
				}
				result = this.ParseDict();
				return result;
			}
			}
			goto IL_4B;
		}

		private JSONValue ParseArray()
		{
			this.Next();
			this.SkipWs();
			List<JSONValue> list = new List<JSONValue>();
			while (this.cur != ']')
			{
				list.Add(this.ParseValue());
				this.SkipWs();
				if (this.cur == ',')
				{
					this.Next();
					this.SkipWs();
				}
			}
			this.Next();
			return new JSONValue(list);
		}

		private JSONValue ParseDict()
		{
			this.Next();
			this.SkipWs();
			Dictionary<string, JSONValue> dictionary = new Dictionary<string, JSONValue>();
			while (this.cur != '}')
			{
				JSONValue jSONValue = this.ParseValue();
				if (!jSONValue.IsString())
				{
					throw new JSONParseException("Key not string type at " + this.PosMsg());
				}
				this.SkipWs();
				if (this.cur != ':')
				{
					throw new JSONParseException("Missing dict entry delimiter ':' at " + this.PosMsg());
				}
				this.Next();
				dictionary.Add(jSONValue.AsString(), this.ParseValue());
				this.SkipWs();
				if (this.cur == ',')
				{
					this.Next();
					this.SkipWs();
				}
			}
			this.Next();
			return new JSONValue(dictionary);
		}

		private JSONValue ParseString()
		{
			string text = "";
			this.Next();
			while (this.idx < this.len)
			{
				int num = this.json.IndexOfAny(JSONParser.endcodes, this.idx);
				if (num < 0)
				{
					throw new JSONParseException("missing '\"' to end string at " + this.PosMsg());
				}
				text += this.json.Substring(this.idx, num - this.idx);
				if (this.json[num] == '"')
				{
					this.cur = this.json[num];
					this.idx = num;
					break;
				}
				num++;
				if (num >= this.len)
				{
					throw new JSONParseException("End of json while parsing while parsing string at " + this.PosMsg());
				}
				char c = this.json[num];
				switch (c)
				{
				case 'r':
					text += '\r';
					goto IL_29E;
				case 's':
					IL_E6:
					if (c != '"')
					{
						if (c != '/')
						{
							if (c != '\\')
							{
								if (c == 'b')
								{
									text += '\b';
									goto IL_29E;
								}
								if (c == 'f')
								{
									text += '\f';
									goto IL_29E;
								}
								if (c != 'n')
								{
									throw new JSONParseException(string.Concat(new object[]
									{
										"Invalid escape char '",
										c,
										"' near ",
										this.PosMsg()
									}));
								}
								text += '\n';
								goto IL_29E;
							}
						}
					}
					text += c;
					goto IL_29E;
				case 't':
					text += '\t';
					goto IL_29E;
				case 'u':
				{
					string text2 = "";
					if (num + 4 >= this.len)
					{
						throw new JSONParseException("End of json while parsing while parsing unicode char near " + this.PosMsg());
					}
					text2 += this.json[num + 1];
					text2 += this.json[num + 2];
					text2 += this.json[num + 3];
					text2 += this.json[num + 4];
					try
					{
						int num2 = int.Parse(text2, NumberStyles.AllowHexSpecifier);
						text += (char)num2;
					}
					catch (FormatException)
					{
						throw new JSONParseException("Invalid unicode escape char near " + this.PosMsg());
					}
					num += 4;
					goto IL_29E;
				}
				}
				goto IL_E6;
				IL_29E:
				this.idx = num + 1;
			}
			if (this.idx >= this.len)
			{
				throw new JSONParseException("End of json while parsing while parsing string near " + this.PosMsg());
			}
			this.cur = this.json[this.idx];
			this.Next();
			return new JSONValue(text);
		}

		private JSONValue ParseNumber()
		{
			string text = "";
			if (this.cur == '-')
			{
				text = "-";
				this.Next();
			}
			while (this.cur >= '0' && this.cur <= '9')
			{
				text += this.cur;
				this.Next();
			}
			if (this.cur == '.')
			{
				this.Next();
				text += '.';
				while (this.cur >= '0' && this.cur <= '9')
				{
					text += this.cur;
					this.Next();
				}
			}
			if (this.cur == 'e' || this.cur == 'E')
			{
				text += "e";
				this.Next();
				if (this.cur != '-' && this.cur != '+')
				{
					text += this.cur;
					this.Next();
				}
				while (this.cur >= '0' && this.cur <= '9')
				{
					text += this.cur;
					this.Next();
				}
			}
			JSONValue result;
			try
			{
				float num = Convert.ToSingle(text);
				result = new JSONValue(num);
			}
			catch (Exception)
			{
				throw new JSONParseException("Cannot convert string to float : '" + text + "' at " + this.PosMsg());
			}
			return result;
		}

		private JSONValue ParseConstant()
		{
			string a = string.Concat(new object[]
			{
				"",
				this.cur,
				this.Next(),
				this.Next(),
				this.Next()
			});
			this.Next();
			JSONValue result;
			if (!(a == "true"))
			{
				if (a == "fals")
				{
					if (this.cur == 'e')
					{
						this.Next();
						result = new JSONValue(false);
						return result;
					}
				}
				else if (a == "null")
				{
					result = new JSONValue(null);
					return result;
				}
				throw new JSONParseException("Invalid token at " + this.PosMsg());
			}
			result = new JSONValue(true);
			return result;
		}
	}
}
