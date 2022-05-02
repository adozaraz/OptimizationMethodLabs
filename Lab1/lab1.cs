using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;

namespace MO
{
    public delegate double func_1(double x);
    public static class Lab1
    {
        public static readonly double Phi = (1 + Math.Sqrt(5)) * 0.5;
        static void Swap<T>(ref T lhs, ref T rhs)
        {
            T temp;
            temp = lhs;
            lhs = rhs;
            rhs = temp;
        }
        public static double Dichotomy(func_1 f, double x0, double x1, double eps=1e-6f, int max_iter = 1000)
        {
            double ans = 0.0f;
            if (x0 > x1) Swap(ref x0, ref x1);
            int i = 0;
            for (; i++ != max_iter; )
            {
                if (Math.Abs(x1 - x0) < eps) break;
                ans = (x1 + x0) * 0.5f;

                if (f(ans + eps) > f(ans - eps))
                {
                    x1 = ans;
                    continue;
                }
                x0 = ans;
            }
#if DEBUG
            Console.WriteLine($"Dichotomy iterations: {i}");
#endif
            return ans;
        }

        public static double Golden(func_1 f, double x0, double x1, double eps=1e-6f, int max_iter = 1000)
        {
            if (x0 > x1) Swap(ref x0, ref x1);
            double a = x0, b = x1;
            int i = 0;
            for ( ; i++ != max_iter; )
            {
                if (Math.Abs(x1 - x0) < eps) break;
                x0 = a + (b - a) / Phi;
                x1 = b - (b - a) / Phi;

                if (f(x0) > f(x1))
                {
                    b = x0;
                    continue;
                }
                a = x1;
            }
#if DEBUG
            Console.WriteLine($"Golden iterations: {i}");
#endif
            return (x1 + x0) * 0.5f;
        }

        public static double FibonacciMethod(func_1 f, double x_0, double x_1, double eps = 1e-6f)
        {
            if (x_0 > x_1)
            {
                Swap(ref x_0, ref x_1);
            }
            double a = x_0, b = x_1, dx;
            double f_1 = 1.0f, f_2 = 2.0f, f_3 = 3.0f;
            int cntr = 0;

            int max_iters = ClosestFibonacci((b - a) / eps);

            for (; cntr != max_iters; cntr++)
            {
                if (Math.Abs(x_1 - x_0) < eps)
                {
                    break;
                }

                dx = b - a;
                x_0 = b - dx * f_1 / f_3;
                x_1 = b + dx * f_2 / f_3;

                f_1 = f_2;
                f_2 = f_3;
                f_3 = f_1 + f_2;

                if (f(x_0) < f(x_1))
                {
                    b = x_0;
                    continue;
                }
                a = x_1;
            }
#if DEBUG
            Console.WriteLine("fibonacchi iterations number : " + cntr + "\n");
#endif
            return (x_1 + x_0) * 0.5f;
        }
        public static int ClosestFibonacci(double value)
        {
            int f_1 = 1;
            if (value <= 1)
            {
                return f_1;
            }
            int f_2 = 2;
            if (value <= 2)
            {
                return f_2;
            }
            int f_3 = 3;
            if (value <= 3)
            {
                return f_3;
            }
            int cntr = 3;
            while (true)
            {
                f_1 = f_2;
                f_2 = f_3;
                f_3 = f_1 + f_2;
                if (f_3 > value)
                {
                    return cntr;
                }
                cntr++;
            }
        }
        public static void Lab1Tester(func_1 f)
        {
            const double x_0 = 5;
            const double x_1 = -3;
            Console.WriteLine("\n");
            Console.WriteLine("x_0 = " + x_0 + ", x_1 = " + x_1);
            Console.WriteLine("Dihotomia   : " + Dichotomy(f, x_0, x_1, 1e-3f));
            Console.WriteLine("GoldenRatio : " + Golden(f, x_0, x_1, 1e-3f));
            Console.WriteLine("Fibonacchi  : " + FibonacciMethod(f, x_0, x_1, 1e-3f));
        }
    }
}
