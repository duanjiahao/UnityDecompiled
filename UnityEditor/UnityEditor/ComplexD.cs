using System;

namespace UnityEditor
{
	internal class ComplexD
	{
		public double real;

		public double imag;

		public ComplexD(double real, double imag)
		{
			this.real = real;
			this.imag = imag;
		}

		public static ComplexD Add(ComplexD a, ComplexD b)
		{
			return new ComplexD(a.real + b.real, a.imag + b.imag);
		}

		public static ComplexD Add(ComplexD a, double b)
		{
			return new ComplexD(a.real + b, a.imag);
		}

		public static ComplexD Add(double a, ComplexD b)
		{
			return new ComplexD(a + b.real, b.imag);
		}

		public static ComplexD Sub(ComplexD a, ComplexD b)
		{
			return new ComplexD(a.real - b.real, a.imag - b.imag);
		}

		public static ComplexD Sub(ComplexD a, double b)
		{
			return new ComplexD(a.real - b, a.imag);
		}

		public static ComplexD Sub(double a, ComplexD b)
		{
			return new ComplexD(a - b.real, -b.imag);
		}

		public static ComplexD Mul(ComplexD a, ComplexD b)
		{
			return new ComplexD(a.real * b.real - a.imag * b.imag, a.real * b.imag + a.imag * b.real);
		}

		public static ComplexD Mul(ComplexD a, double b)
		{
			return new ComplexD(a.real * b, a.imag * b);
		}

		public static ComplexD Mul(double a, ComplexD b)
		{
			return new ComplexD(a * b.real, a * b.imag);
		}

		public static ComplexD Div(ComplexD a, ComplexD b)
		{
			double num = b.real * b.real + b.imag * b.imag;
			double num2 = 1.0 / num;
			return new ComplexD((a.real * b.real + a.imag * b.imag) * num2, (a.imag * b.real - a.real * b.imag) * num2);
		}

		public static ComplexD Div(double a, ComplexD b)
		{
			double num = b.real * b.real + b.imag * b.imag;
			double num2 = a / num;
			return new ComplexD(num2 * b.real, -num2 * b.imag);
		}

		public static ComplexD Div(ComplexD a, double b)
		{
			double num = 1.0 / b;
			return new ComplexD(a.real * num, a.imag * num);
		}

		public static ComplexD Exp(double omega)
		{
			return new ComplexD(Math.Cos(omega), Math.Sin(omega));
		}

		public static ComplexD Pow(ComplexD a, double b)
		{
			double num = Math.Atan2(a.imag, a.real);
			double num2 = Math.Pow(a.Mag2(), b * 0.5);
			return new ComplexD(num2 * Math.Cos(num * b), num2 * Math.Sin(num * b));
		}

		public double Mag2()
		{
			return this.real * this.real + this.imag * this.imag;
		}

		public double Mag()
		{
			return Math.Sqrt(this.Mag2());
		}

		public static ComplexD operator +(ComplexD a, ComplexD b)
		{
			return ComplexD.Add(a, b);
		}

		public static ComplexD operator -(ComplexD a, ComplexD b)
		{
			return ComplexD.Sub(a, b);
		}

		public static ComplexD operator *(ComplexD a, ComplexD b)
		{
			return ComplexD.Mul(a, b);
		}

		public static ComplexD operator /(ComplexD a, ComplexD b)
		{
			return ComplexD.Div(a, b);
		}

		public static ComplexD operator +(ComplexD a, double b)
		{
			return ComplexD.Add(a, b);
		}

		public static ComplexD operator -(ComplexD a, double b)
		{
			return ComplexD.Sub(a, b);
		}

		public static ComplexD operator *(ComplexD a, double b)
		{
			return ComplexD.Mul(a, b);
		}

		public static ComplexD operator /(ComplexD a, double b)
		{
			return ComplexD.Div(a, b);
		}

		public static ComplexD operator +(double a, ComplexD b)
		{
			return ComplexD.Add(a, b);
		}

		public static ComplexD operator -(double a, ComplexD b)
		{
			return ComplexD.Sub(a, b);
		}

		public static ComplexD operator *(double a, ComplexD b)
		{
			return ComplexD.Mul(a, b);
		}

		public static ComplexD operator /(double a, ComplexD b)
		{
			return ComplexD.Div(a, b);
		}
	}
}
