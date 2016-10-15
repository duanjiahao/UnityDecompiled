using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;

namespace UnityEditor
{
	internal class ExpressionEvaluator
	{
		private enum Associativity
		{
			Left,
			Right
		}

		private struct Operator
		{
			public char character;

			public int presedence;

			public ExpressionEvaluator.Associativity associativity;

			public int inputs;

			public Operator(char character, int presedence, int inputs, ExpressionEvaluator.Associativity associativity)
			{
				this.character = character;
				this.presedence = presedence;
				this.inputs = inputs;
				this.associativity = associativity;
			}
		}

		private static readonly ExpressionEvaluator.Operator[] s_Operators = new ExpressionEvaluator.Operator[]
		{
			new ExpressionEvaluator.Operator('-', 2, 2, ExpressionEvaluator.Associativity.Left),
			new ExpressionEvaluator.Operator('+', 2, 2, ExpressionEvaluator.Associativity.Left),
			new ExpressionEvaluator.Operator('/', 3, 2, ExpressionEvaluator.Associativity.Left),
			new ExpressionEvaluator.Operator('*', 3, 2, ExpressionEvaluator.Associativity.Left),
			new ExpressionEvaluator.Operator('%', 3, 2, ExpressionEvaluator.Associativity.Left),
			new ExpressionEvaluator.Operator('^', 4, 2, ExpressionEvaluator.Associativity.Right),
			new ExpressionEvaluator.Operator('u', 4, 1, ExpressionEvaluator.Associativity.Left)
		};

		public static T Evaluate<T>(string expression)
		{
			T result = default(T);
			if (!ExpressionEvaluator.TryParse<T>(expression, out result))
			{
				expression = ExpressionEvaluator.PreFormatExpression(expression);
				string[] tokens = ExpressionEvaluator.ExpressionToTokens(expression);
				tokens = ExpressionEvaluator.FixUnaryOperators(tokens);
				string[] tokens2 = ExpressionEvaluator.InfixToRPN(tokens);
				result = ExpressionEvaluator.Evaluate<T>(tokens2);
			}
			return result;
		}

		private static T Evaluate<T>(string[] tokens)
		{
			Stack<string> stack = new Stack<string>();
			for (int i = 0; i < tokens.Length; i++)
			{
				string text = tokens[i];
				if (ExpressionEvaluator.IsOperator(text))
				{
					ExpressionEvaluator.Operator @operator = ExpressionEvaluator.CharToOperator(text[0]);
					List<T> list = new List<T>();
					bool flag = true;
					while (stack.LongCount<string>() > 0L && !ExpressionEvaluator.IsCommand(stack.Peek()) && list.Count < @operator.inputs)
					{
						T item;
						flag &= ExpressionEvaluator.TryParse<T>(stack.Pop(), out item);
						list.Add(item);
					}
					list.Reverse();
					if (!flag || list.Count != @operator.inputs)
					{
						return default(T);
					}
					Stack<string> arg_CA_0 = stack;
					T t = ExpressionEvaluator.Evaluate<T>(list.ToArray(), text[0]);
					arg_CA_0.Push(t.ToString());
				}
				else
				{
					stack.Push(text);
				}
			}
			T result;
			if (stack.LongCount<string>() == 1L && ExpressionEvaluator.TryParse<T>(stack.Pop(), out result))
			{
				return result;
			}
			return default(T);
		}

		private static string[] InfixToRPN(string[] tokens)
		{
			Stack<char> stack = new Stack<char>();
			Stack<string> stack2 = new Stack<string>();
			for (int i = 0; i < tokens.Length; i++)
			{
				string text = tokens[i];
				if (ExpressionEvaluator.IsCommand(text))
				{
					char c = text[0];
					if (c == '(')
					{
						stack.Push(c);
					}
					else if (c == ')')
					{
						while (stack.LongCount<char>() > 0L && stack.Peek() != '(')
						{
							stack2.Push(stack.Pop().ToString());
						}
						if (stack.LongCount<char>() > 0L)
						{
							stack.Pop();
						}
					}
					else
					{
						ExpressionEvaluator.Operator newOperator = ExpressionEvaluator.CharToOperator(c);
						while (ExpressionEvaluator.NeedToPop(stack, newOperator))
						{
							stack2.Push(stack.Pop().ToString());
						}
						stack.Push(c);
					}
				}
				else
				{
					stack2.Push(text);
				}
			}
			while (stack.LongCount<char>() > 0L)
			{
				stack2.Push(stack.Pop().ToString());
			}
			return stack2.Reverse<string>().ToArray<string>();
		}

		private static bool NeedToPop(Stack<char> operatorStack, ExpressionEvaluator.Operator newOperator)
		{
			if (operatorStack.LongCount<char>() > 0L)
			{
				ExpressionEvaluator.Operator @operator = ExpressionEvaluator.CharToOperator(operatorStack.Peek());
				if (ExpressionEvaluator.IsOperator(@operator.character) && ((newOperator.associativity == ExpressionEvaluator.Associativity.Left && newOperator.presedence <= @operator.presedence) || (newOperator.associativity == ExpressionEvaluator.Associativity.Right && newOperator.presedence < @operator.presedence)))
				{
					return true;
				}
			}
			return false;
		}

		private static string[] ExpressionToTokens(string expression)
		{
			List<string> list = new List<string>();
			string text = string.Empty;
			for (int i = 0; i < expression.Length; i++)
			{
				char c = expression[i];
				if (ExpressionEvaluator.IsCommand(c))
				{
					if (text.Length > 0)
					{
						list.Add(text);
					}
					list.Add(c.ToString());
					text = string.Empty;
				}
				else if (c != ' ')
				{
					text += c;
				}
			}
			if (text.Length > 0)
			{
				list.Add(text);
			}
			return list.ToArray();
		}

		private static bool IsCommand(string token)
		{
			return token.Length == 1 && ExpressionEvaluator.IsCommand(token[0]);
		}

		private static bool IsCommand(char character)
		{
			return character == '(' || character == ')' || ExpressionEvaluator.IsOperator(character);
		}

		private static bool IsOperator(string token)
		{
			return token.Length == 1 && ExpressionEvaluator.IsOperator(token[0]);
		}

		private static bool IsOperator(char character)
		{
			ExpressionEvaluator.Operator[] array = ExpressionEvaluator.s_Operators;
			for (int i = 0; i < array.Length; i++)
			{
				ExpressionEvaluator.Operator @operator = array[i];
				if (@operator.character == character)
				{
					return true;
				}
			}
			return false;
		}

		private static ExpressionEvaluator.Operator CharToOperator(char character)
		{
			ExpressionEvaluator.Operator[] array = ExpressionEvaluator.s_Operators;
			for (int i = 0; i < array.Length; i++)
			{
				ExpressionEvaluator.Operator result = array[i];
				if (result.character == character)
				{
					return result;
				}
			}
			return default(ExpressionEvaluator.Operator);
		}

		private static string PreFormatExpression(string expression)
		{
			string text = expression.Trim();
			if (text.Length == 0)
			{
				return text;
			}
			char c = text[text.Length - 1];
			if (ExpressionEvaluator.IsOperator(c))
			{
				text = text.TrimEnd(new char[]
				{
					c
				});
			}
			return text;
		}

		private static string[] FixUnaryOperators(string[] tokens)
		{
			if (tokens.Length == 0)
			{
				return tokens;
			}
			if (tokens[0] == "-")
			{
				tokens[0] = "u";
			}
			for (int i = 1; i < tokens.Length - 1; i++)
			{
				string a = tokens[i];
				string token = tokens[i - 1];
				string a2 = tokens[i - 1];
				if (a == "-" && (ExpressionEvaluator.IsCommand(token) || a2 == "(" || a2 == ")"))
				{
					tokens[i] = "u";
				}
			}
			return tokens;
		}

		private static T Evaluate<T>(T[] values, char oper)
		{
			if (typeof(T) == typeof(float))
			{
				if (values.Length == 1)
				{
					if (oper == 'u')
					{
						return (T)((object)((float)((object)values[0]) * -1f));
					}
				}
				else if (values.Length == 2)
				{
					switch (oper)
					{
					case '*':
						return (T)((object)((float)((object)values[0]) * (float)((object)values[1])));
					case '+':
						return (T)((object)((float)((object)values[0]) + (float)((object)values[1])));
					case ',':
					case '.':
						IL_84:
						if (oper == '%')
						{
							return (T)((object)((float)((object)values[0]) % (float)((object)values[1])));
						}
						if (oper != '^')
						{
							goto IL_1B1;
						}
						return (T)((object)Mathf.Pow((float)((object)values[0]), (float)((object)values[1])));
					case '-':
						return (T)((object)((float)((object)values[0]) - (float)((object)values[1])));
					case '/':
						return (T)((object)((float)((object)values[0]) / (float)((object)values[1])));
					}
					goto IL_84;
				}
				IL_1B1:;
			}
			else if (typeof(T) == typeof(int))
			{
				if (values.Length == 1)
				{
					if (oper == 'u')
					{
						return (T)((object)((int)((object)values[0]) * -1));
					}
				}
				else if (values.Length == 2)
				{
					switch (oper)
					{
					case '*':
						return (T)((object)((int)((object)values[0]) * (int)((object)values[1])));
					case '+':
						return (T)((object)((int)((object)values[0]) + (int)((object)values[1])));
					case ',':
					case '.':
						IL_236:
						if (oper == '%')
						{
							return (T)((object)((int)((object)values[0]) % (int)((object)values[1])));
						}
						if (oper != '^')
						{
							goto IL_366;
						}
						return (T)((object)((int)Math.Pow((double)((int)((object)values[0])), (double)((int)((object)values[1])))));
					case '-':
						return (T)((object)((int)((object)values[0]) - (int)((object)values[1])));
					case '/':
						return (T)((object)((int)((object)values[0]) / (int)((object)values[1])));
					}
					goto IL_236;
				}
			}
			IL_366:
			if (typeof(T) == typeof(double))
			{
				if (values.Length == 1)
				{
					if (oper == 'u')
					{
						return (T)((object)((double)((object)values[0]) * -1.0));
					}
				}
				else if (values.Length == 2)
				{
					switch (oper)
					{
					case '*':
						return (T)((object)((double)((object)values[0]) * (double)((object)values[1])));
					case '+':
						return (T)((object)((double)((object)values[0]) + (double)((object)values[1])));
					case ',':
					case '.':
						IL_3EE:
						if (oper == '%')
						{
							return (T)((object)((double)((object)values[0]) % (double)((object)values[1])));
						}
						if (oper != '^')
						{
							goto IL_51B;
						}
						return (T)((object)Math.Pow((double)((object)values[0]), (double)((object)values[1])));
					case '-':
						return (T)((object)((double)((object)values[0]) - (double)((object)values[1])));
					case '/':
						return (T)((object)((double)((object)values[0]) / (double)((object)values[1])));
					}
					goto IL_3EE;
				}
				IL_51B:;
			}
			else if (typeof(T) == typeof(long))
			{
				if (values.Length == 1)
				{
					if (oper == 'u')
					{
						return (T)((object)((long)((object)values[0]) * -1L));
					}
				}
				else if (values.Length == 2)
				{
					switch (oper)
					{
					case '*':
						return (T)((object)((long)((object)values[0]) * (long)((object)values[1])));
					case '+':
						return (T)((object)((long)((object)values[0]) + (long)((object)values[1])));
					case ',':
					case '.':
						IL_5A1:
						if (oper == '%')
						{
							return (T)((object)((long)((object)values[0]) % (long)((object)values[1])));
						}
						if (oper != '^')
						{
							goto IL_6D1;
						}
						return (T)((object)((long)Math.Pow((double)((long)((object)values[0])), (double)((long)((object)values[1])))));
					case '-':
						return (T)((object)((long)((object)values[0]) - (long)((object)values[1])));
					case '/':
						return (T)((object)((long)((object)values[0]) / (long)((object)values[1])));
					}
					goto IL_5A1;
				}
			}
			IL_6D1:
			return default(T);
		}

		private static bool TryParse<T>(string expression, out T result)
		{
			expression = expression.Replace(',', '.');
			bool result2 = false;
			result = default(T);
			if (typeof(T) == typeof(float))
			{
				float num = 0f;
				result2 = float.TryParse(expression, NumberStyles.Float, CultureInfo.InvariantCulture.NumberFormat, out num);
				result = (T)((object)num);
			}
			else if (typeof(T) == typeof(int))
			{
				int num2 = 0;
				result2 = int.TryParse(expression, NumberStyles.Integer, CultureInfo.InvariantCulture.NumberFormat, out num2);
				result = (T)((object)num2);
			}
			else if (typeof(T) == typeof(double))
			{
				double num3 = 0.0;
				result2 = double.TryParse(expression, NumberStyles.Float, CultureInfo.InvariantCulture.NumberFormat, out num3);
				result = (T)((object)num3);
			}
			else if (typeof(T) == typeof(long))
			{
				long num4 = 0L;
				result2 = long.TryParse(expression, NumberStyles.Integer, CultureInfo.InvariantCulture.NumberFormat, out num4);
				result = (T)((object)num4);
			}
			return result2;
		}
	}
}
