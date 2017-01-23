using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using UnityEngine.Assertions.Comparers;

namespace UnityEngine.Assertions
{
	[DebuggerStepThrough]
	public static class Assert
	{
		internal const string UNITY_ASSERTIONS = "UNITY_ASSERTIONS";

		public static bool raiseExceptions;

		private static void Fail(string message, string userMessage)
		{
			if (Debugger.IsAttached)
			{
				throw new AssertionException(message, userMessage);
			}
			if (Assert.raiseExceptions)
			{
				throw new AssertionException(message, userMessage);
			}
			if (message == null)
			{
				message = "Assertion has failed\n";
			}
			if (userMessage != null)
			{
				message = userMessage + '\n' + message;
			}
			UnityEngine.Debug.LogAssertion(message);
		}

		[EditorBrowsable(EditorBrowsableState.Never), Obsolete("Assert.Equals should not be used for Assertions", true)]
		public new static bool Equals(object obj1, object obj2)
		{
			throw new InvalidOperationException("Assert.Equals should not be used for Assertions");
		}

		[EditorBrowsable(EditorBrowsableState.Never), Obsolete("Assert.ReferenceEquals should not be used for Assertions", true)]
		public new static bool ReferenceEquals(object obj1, object obj2)
		{
			throw new InvalidOperationException("Assert.ReferenceEquals should not be used for Assertions");
		}

		[Conditional("UNITY_ASSERTIONS")]
		public static void IsTrue(bool condition)
		{
			Assert.IsTrue(condition, null);
		}

		[Conditional("UNITY_ASSERTIONS")]
		public static void IsTrue(bool condition, string message)
		{
			if (!condition)
			{
				Assert.Fail(AssertionMessageUtil.BooleanFailureMessage(true), message);
			}
		}

		[Conditional("UNITY_ASSERTIONS")]
		public static void IsFalse(bool condition)
		{
			Assert.IsFalse(condition, null);
		}

		[Conditional("UNITY_ASSERTIONS")]
		public static void IsFalse(bool condition, string message)
		{
			if (condition)
			{
				Assert.Fail(AssertionMessageUtil.BooleanFailureMessage(false), message);
			}
		}

		[Conditional("UNITY_ASSERTIONS")]
		public static void AreApproximatelyEqual(float expected, float actual)
		{
			Assert.AreEqual<float>(expected, actual, null, FloatComparer.s_ComparerWithDefaultTolerance);
		}

		[Conditional("UNITY_ASSERTIONS")]
		public static void AreApproximatelyEqual(float expected, float actual, string message)
		{
			Assert.AreEqual<float>(expected, actual, message, FloatComparer.s_ComparerWithDefaultTolerance);
		}

		[Conditional("UNITY_ASSERTIONS")]
		public static void AreApproximatelyEqual(float expected, float actual, float tolerance)
		{
			Assert.AreApproximatelyEqual(expected, actual, tolerance, null);
		}

		[Conditional("UNITY_ASSERTIONS")]
		public static void AreApproximatelyEqual(float expected, float actual, float tolerance, string message)
		{
			Assert.AreEqual<float>(expected, actual, message, new FloatComparer(tolerance));
		}

		[Conditional("UNITY_ASSERTIONS")]
		public static void AreNotApproximatelyEqual(float expected, float actual)
		{
			Assert.AreNotEqual<float>(expected, actual, null, FloatComparer.s_ComparerWithDefaultTolerance);
		}

		[Conditional("UNITY_ASSERTIONS")]
		public static void AreNotApproximatelyEqual(float expected, float actual, string message)
		{
			Assert.AreNotEqual<float>(expected, actual, message, FloatComparer.s_ComparerWithDefaultTolerance);
		}

		[Conditional("UNITY_ASSERTIONS")]
		public static void AreNotApproximatelyEqual(float expected, float actual, float tolerance)
		{
			Assert.AreNotApproximatelyEqual(expected, actual, tolerance, null);
		}

		[Conditional("UNITY_ASSERTIONS")]
		public static void AreNotApproximatelyEqual(float expected, float actual, float tolerance, string message)
		{
			Assert.AreNotEqual<float>(expected, actual, message, new FloatComparer(tolerance));
		}

		[Conditional("UNITY_ASSERTIONS")]
		public static void AreEqual<T>(T expected, T actual)
		{
			Assert.AreEqual<T>(expected, actual, null);
		}

		[Conditional("UNITY_ASSERTIONS")]
		public static void AreEqual<T>(T expected, T actual, string message)
		{
			Assert.AreEqual<T>(expected, actual, message, EqualityComparer<T>.Default);
		}

		[Conditional("UNITY_ASSERTIONS")]
		public static void AreEqual<T>(T expected, T actual, string message, IEqualityComparer<T> comparer)
		{
			if (typeof(UnityEngine.Object).IsAssignableFrom(typeof(T)))
			{
				Assert.AreEqual(expected as UnityEngine.Object, actual as UnityEngine.Object, message);
			}
			else if (!comparer.Equals(actual, expected))
			{
				Assert.Fail(AssertionMessageUtil.GetEqualityMessage(actual, expected, true), message);
			}
		}

		[Conditional("UNITY_ASSERTIONS")]
		public static void AreEqual(UnityEngine.Object expected, UnityEngine.Object actual, string message)
		{
			if (actual != expected)
			{
				Assert.Fail(AssertionMessageUtil.GetEqualityMessage(actual, expected, true), message);
			}
		}

		[Conditional("UNITY_ASSERTIONS")]
		public static void AreNotEqual<T>(T expected, T actual)
		{
			Assert.AreNotEqual<T>(expected, actual, null);
		}

		[Conditional("UNITY_ASSERTIONS")]
		public static void AreNotEqual<T>(T expected, T actual, string message)
		{
			Assert.AreNotEqual<T>(expected, actual, message, EqualityComparer<T>.Default);
		}

		[Conditional("UNITY_ASSERTIONS")]
		public static void AreNotEqual<T>(T expected, T actual, string message, IEqualityComparer<T> comparer)
		{
			if (typeof(UnityEngine.Object).IsAssignableFrom(typeof(T)))
			{
				Assert.AreNotEqual(expected as UnityEngine.Object, actual as UnityEngine.Object, message);
			}
			else if (comparer.Equals(actual, expected))
			{
				Assert.Fail(AssertionMessageUtil.GetEqualityMessage(actual, expected, false), message);
			}
		}

		[Conditional("UNITY_ASSERTIONS")]
		public static void AreNotEqual(UnityEngine.Object expected, UnityEngine.Object actual, string message)
		{
			if (actual == expected)
			{
				Assert.Fail(AssertionMessageUtil.GetEqualityMessage(actual, expected, false), message);
			}
		}

		[Conditional("UNITY_ASSERTIONS")]
		public static void IsNull<T>(T value) where T : class
		{
			Assert.IsNull<T>(value, null);
		}

		[Conditional("UNITY_ASSERTIONS")]
		public static void IsNull<T>(T value, string message) where T : class
		{
			if (typeof(UnityEngine.Object).IsAssignableFrom(typeof(T)))
			{
				Assert.IsNull(value as UnityEngine.Object, message);
			}
			else if (value != null)
			{
				Assert.Fail(AssertionMessageUtil.NullFailureMessage(value, true), message);
			}
		}

		[Conditional("UNITY_ASSERTIONS")]
		public static void IsNull(UnityEngine.Object value, string message)
		{
			if (value != null)
			{
				Assert.Fail(AssertionMessageUtil.NullFailureMessage(value, true), message);
			}
		}

		[Conditional("UNITY_ASSERTIONS")]
		public static void IsNotNull<T>(T value) where T : class
		{
			Assert.IsNotNull<T>(value, null);
		}

		[Conditional("UNITY_ASSERTIONS")]
		public static void IsNotNull<T>(T value, string message) where T : class
		{
			if (typeof(UnityEngine.Object).IsAssignableFrom(typeof(T)))
			{
				Assert.IsNotNull(value as UnityEngine.Object, message);
			}
			else if (value == null)
			{
				Assert.Fail(AssertionMessageUtil.NullFailureMessage(value, false), message);
			}
		}

		[Conditional("UNITY_ASSERTIONS")]
		public static void IsNotNull(UnityEngine.Object value, string message)
		{
			if (value == null)
			{
				Assert.Fail(AssertionMessageUtil.NullFailureMessage(value, false), message);
			}
		}
	}
}
