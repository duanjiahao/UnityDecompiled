using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace UnityEditor
{
	internal static class Json
	{
		private sealed class Parser : IDisposable
		{
			private enum TOKEN
			{
				NONE,
				CURLY_OPEN,
				CURLY_CLOSE,
				SQUARED_OPEN,
				SQUARED_CLOSE,
				COLON,
				COMMA,
				STRING,
				NUMBER,
				TRUE,
				FALSE,
				NULL
			}

			private const string WORD_BREAK = "{}[],:\"";

			private StringReader json;

			private char PeekChar
			{
				get
				{
					return Convert.ToChar(this.json.Peek());
				}
			}

			private char NextChar
			{
				get
				{
					return Convert.ToChar(this.json.Read());
				}
			}

			private string NextWord
			{
				get
				{
					StringBuilder stringBuilder = new StringBuilder();
					while (!Json.Parser.IsWordBreak(this.PeekChar))
					{
						stringBuilder.Append(this.NextChar);
						if (this.json.Peek() == -1)
						{
							break;
						}
					}
					return stringBuilder.ToString();
				}
			}

			private Json.Parser.TOKEN NextToken
			{
				get
				{
					this.EatWhitespace();
					Json.Parser.TOKEN result;
					if (this.json.Peek() != -1)
					{
						char peekChar = this.PeekChar;
						switch (peekChar)
						{
						case ',':
							this.json.Read();
							result = Json.Parser.TOKEN.COMMA;
							return result;
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
							result = Json.Parser.TOKEN.NUMBER;
							return result;
						case '.':
						case '/':
							IL_6C:
							switch (peekChar)
							{
							case '[':
								result = Json.Parser.TOKEN.SQUARED_OPEN;
								return result;
							case '\\':
								IL_81:
								switch (peekChar)
								{
								case '{':
									result = Json.Parser.TOKEN.CURLY_OPEN;
									return result;
								case '|':
									IL_96:
									if (peekChar != '"')
									{
										string nextWord = this.NextWord;
										if (nextWord != null)
										{
											if (nextWord == "false")
											{
												result = Json.Parser.TOKEN.FALSE;
												return result;
											}
											if (nextWord == "true")
											{
												result = Json.Parser.TOKEN.TRUE;
												return result;
											}
											if (nextWord == "null")
											{
												result = Json.Parser.TOKEN.NULL;
												return result;
											}
										}
										result = Json.Parser.TOKEN.NONE;
										return result;
									}
									result = Json.Parser.TOKEN.STRING;
									return result;
								case '}':
									this.json.Read();
									result = Json.Parser.TOKEN.CURLY_CLOSE;
									return result;
								}
								goto IL_96;
							case ']':
								this.json.Read();
								result = Json.Parser.TOKEN.SQUARED_CLOSE;
								return result;
							}
							goto IL_81;
						case ':':
							result = Json.Parser.TOKEN.COLON;
							return result;
						}
						goto IL_6C;
					}
					result = Json.Parser.TOKEN.NONE;
					return result;
				}
			}

			private Parser(string jsonString)
			{
				this.json = new StringReader(jsonString);
			}

			public static bool IsWordBreak(char c)
			{
				return char.IsWhiteSpace(c) || "{}[],:\"".IndexOf(c) != -1;
			}

			public static object Parse(string jsonString)
			{
				object result;
				using (Json.Parser parser = new Json.Parser(jsonString))
				{
					result = parser.ParseValue();
				}
				return result;
			}

			public void Dispose()
			{
				this.json.Dispose();
				this.json = null;
			}

			private Dictionary<string, object> ParseObject()
			{
				Dictionary<string, object> dictionary = new Dictionary<string, object>();
				this.json.Read();
				while (true)
				{
					Json.Parser.TOKEN nextToken = this.NextToken;
					switch (nextToken)
					{
					case Json.Parser.TOKEN.NONE:
						goto IL_3A;
					case Json.Parser.TOKEN.CURLY_OPEN:
					{
						IL_2E:
						if (nextToken == Json.Parser.TOKEN.COMMA)
						{
							continue;
						}
						string text = this.ParseString();
						if (text == null)
						{
							goto Block_2;
						}
						if (this.NextToken != Json.Parser.TOKEN.COLON)
						{
							goto Block_3;
						}
						this.json.Read();
						dictionary[text] = this.ParseValue();
						continue;
					}
					case Json.Parser.TOKEN.CURLY_CLOSE:
						goto IL_46;
					}
					goto IL_2E;
				}
				IL_3A:
				Dictionary<string, object> result = null;
				return result;
				IL_46:
				result = dictionary;
				return result;
				Block_2:
				result = null;
				return result;
				Block_3:
				result = null;
				return result;
			}

			private List<object> ParseArray()
			{
				List<object> list = new List<object>();
				this.json.Read();
				bool flag = true;
				List<object> result;
				while (flag)
				{
					Json.Parser.TOKEN nextToken = this.NextToken;
					switch (nextToken)
					{
					case Json.Parser.TOKEN.SQUARED_CLOSE:
						flag = false;
						continue;
					case Json.Parser.TOKEN.COLON:
						IL_36:
						if (nextToken != Json.Parser.TOKEN.NONE)
						{
							object item = this.ParseByToken(nextToken);
							list.Add(item);
							continue;
						}
						result = null;
						return result;
					case Json.Parser.TOKEN.COMMA:
						continue;
					}
					goto IL_36;
				}
				result = list;
				return result;
			}

			private object ParseValue()
			{
				Json.Parser.TOKEN nextToken = this.NextToken;
				return this.ParseByToken(nextToken);
			}

			private object ParseByToken(Json.Parser.TOKEN token)
			{
				object result;
				switch (token)
				{
				case Json.Parser.TOKEN.STRING:
					result = this.ParseString();
					break;
				case Json.Parser.TOKEN.NUMBER:
					result = this.ParseNumber();
					break;
				case Json.Parser.TOKEN.TRUE:
					result = true;
					break;
				case Json.Parser.TOKEN.FALSE:
					result = false;
					break;
				case Json.Parser.TOKEN.NULL:
					result = null;
					break;
				default:
					switch (token)
					{
					case Json.Parser.TOKEN.CURLY_OPEN:
						result = this.ParseObject();
						return result;
					case Json.Parser.TOKEN.SQUARED_OPEN:
						result = this.ParseArray();
						return result;
					}
					result = null;
					break;
				}
				return result;
			}

			private string ParseString()
			{
				StringBuilder stringBuilder = new StringBuilder();
				this.json.Read();
				bool flag = true;
				while (flag)
				{
					if (this.json.Peek() == -1)
					{
						break;
					}
					char nextChar = this.NextChar;
					if (nextChar != '"')
					{
						if (nextChar != '\\')
						{
							stringBuilder.Append(nextChar);
						}
						else
						{
							if (this.json.Peek() != -1)
							{
								nextChar = this.NextChar;
								switch (nextChar)
								{
								case 'r':
									stringBuilder.Append('\r');
									continue;
								case 's':
									IL_90:
									if (nextChar == '"' || nextChar == '/' || nextChar == '\\')
									{
										stringBuilder.Append(nextChar);
										continue;
									}
									if (nextChar == 'b')
									{
										stringBuilder.Append('\b');
										continue;
									}
									if (nextChar == 'f')
									{
										stringBuilder.Append('\f');
										continue;
									}
									if (nextChar != 'n')
									{
										continue;
									}
									stringBuilder.Append('\n');
									continue;
								case 't':
									stringBuilder.Append('\t');
									continue;
								case 'u':
								{
									char[] array = new char[4];
									for (int i = 0; i < 4; i++)
									{
										array[i] = this.NextChar;
									}
									stringBuilder.Append((char)Convert.ToInt32(new string(array), 16));
									continue;
								}
								}
								goto IL_90;
							}
							flag = false;
						}
					}
					else
					{
						flag = false;
					}
				}
				return stringBuilder.ToString();
			}

			private object ParseNumber()
			{
				string nextWord = this.NextWord;
				object result;
				if (nextWord.IndexOf('.') == -1)
				{
					long num;
					long.TryParse(nextWord, NumberStyles.Any, CultureInfo.InvariantCulture, out num);
					result = num;
				}
				else
				{
					double num2;
					double.TryParse(nextWord, NumberStyles.Any, CultureInfo.InvariantCulture, out num2);
					result = num2;
				}
				return result;
			}

			private void EatWhitespace()
			{
				while (char.IsWhiteSpace(this.PeekChar))
				{
					this.json.Read();
					if (this.json.Peek() == -1)
					{
						break;
					}
				}
			}
		}

		private sealed class Serializer
		{
			private StringBuilder builder;

			private Serializer()
			{
				this.builder = new StringBuilder();
			}

			public static string Serialize(object obj)
			{
				Json.Serializer serializer = new Json.Serializer();
				serializer.SerializeValue(obj);
				return serializer.builder.ToString();
			}

			private void SerializeValue(object value)
			{
				string str;
				IList anArray;
				IDictionary obj;
				if (value == null)
				{
					this.builder.Append("null");
				}
				else if ((str = (value as string)) != null)
				{
					this.SerializeString(str);
				}
				else if (value is bool)
				{
					this.builder.Append((!(bool)value) ? "false" : "true");
				}
				else if ((anArray = (value as IList)) != null)
				{
					this.SerializeArray(anArray);
				}
				else if ((obj = (value as IDictionary)) != null)
				{
					this.SerializeObject(obj);
				}
				else if (value is char)
				{
					this.SerializeString(new string((char)value, 1));
				}
				else
				{
					this.SerializeOther(value);
				}
			}

			private void SerializeObject(IDictionary obj)
			{
				bool flag = true;
				this.builder.Append('{');
				IEnumerator enumerator = obj.Keys.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						object current = enumerator.Current;
						if (!flag)
						{
							this.builder.Append(',');
						}
						this.SerializeString(current.ToString());
						this.builder.Append(':');
						this.SerializeValue(obj[current]);
						flag = false;
					}
				}
				finally
				{
					IDisposable disposable;
					if ((disposable = (enumerator as IDisposable)) != null)
					{
						disposable.Dispose();
					}
				}
				this.builder.Append('}');
			}

			private void SerializeArray(IList anArray)
			{
				this.builder.Append('[');
				bool flag = true;
				IEnumerator enumerator = anArray.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						object current = enumerator.Current;
						if (!flag)
						{
							this.builder.Append(',');
						}
						this.SerializeValue(current);
						flag = false;
					}
				}
				finally
				{
					IDisposable disposable;
					if ((disposable = (enumerator as IDisposable)) != null)
					{
						disposable.Dispose();
					}
				}
				this.builder.Append(']');
			}

			private void SerializeString(string str)
			{
				this.builder.Append('"');
				char[] array = str.ToCharArray();
				char[] array2 = array;
				for (int i = 0; i < array2.Length; i++)
				{
					char c = array2[i];
					switch (c)
					{
					case '\b':
						this.builder.Append("\\b");
						goto IL_152;
					case '\t':
						this.builder.Append("\\t");
						goto IL_152;
					case '\n':
						this.builder.Append("\\n");
						goto IL_152;
					case '\v':
						IL_45:
						if (c == '"')
						{
							this.builder.Append("\\\"");
							goto IL_152;
						}
						if (c != '\\')
						{
							int num = Convert.ToInt32(c);
							if (num >= 32 && num <= 126)
							{
								this.builder.Append(c);
							}
							else
							{
								this.builder.Append("\\u");
								this.builder.Append(num.ToString("x4"));
							}
							goto IL_152;
						}
						this.builder.Append("\\\\");
						goto IL_152;
					case '\f':
						this.builder.Append("\\f");
						goto IL_152;
					case '\r':
						this.builder.Append("\\r");
						goto IL_152;
					}
					goto IL_45;
					IL_152:;
				}
				this.builder.Append('"');
			}

			private void SerializeOther(object value)
			{
				if (value is float)
				{
					this.builder.Append(((float)value).ToString("R", CultureInfo.InvariantCulture));
				}
				else if (value is int || value is uint || value is long || value is sbyte || value is byte || value is short || value is ushort || value is ulong)
				{
					this.builder.Append(value);
				}
				else if (value is double || value is decimal)
				{
					this.builder.Append(Convert.ToDouble(value).ToString("R", CultureInfo.InvariantCulture));
				}
				else
				{
					Dictionary<string, object> dictionary = new Dictionary<string, object>();
					List<FieldInfo> list = value.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public).ToList<FieldInfo>();
					foreach (FieldInfo current in list)
					{
						dictionary.Add(current.Name, current.GetValue(value));
					}
					List<PropertyInfo> list2 = value.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).ToList<PropertyInfo>();
					foreach (PropertyInfo current2 in list2)
					{
						dictionary.Add(current2.Name, current2.GetValue(value, null));
					}
					this.SerializeObject(dictionary);
				}
			}
		}

		public static object Deserialize(string json)
		{
			object result;
			if (json == null)
			{
				result = null;
			}
			else
			{
				result = Json.Parser.Parse(json);
			}
			return result;
		}

		public static string Serialize(object obj)
		{
			return Json.Serializer.Serialize(obj);
		}
	}
}
