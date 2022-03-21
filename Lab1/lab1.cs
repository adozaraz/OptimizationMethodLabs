using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;

namespace MO
{
    public delegate float func_1(float x);
    public static class Lab1
    {
        public static readonly float Phi = (float)((1 + Math.Sqrt(5)) * 0.5f);
        static void Swap<T>(ref T lhs, ref T rhs)
        {
            T temp;
            temp = lhs;
            lhs = rhs;
            rhs = temp;
        }
        public static float Dichotomy(func_1 f, float x0, float x1, float eps=1e-6f, int max_iter = 1000)
        {
            float ans = 0.0f;
            if (x0 > x1) Swap(ref x0, ref x1);
            int i = 0;
            for (; i++ != max_iter; )
            {
                if (MathF.Abs(x1 - x0) < eps) break;
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

        public static float Golden(func_1 f, float x0, float x1, float eps=1e-6f, int max_iter = 1000)
        {
            if (x0 > x1) Swap(ref x0, ref x1);
            float a = x0, b = x1;
            int i = 0;
            for ( ; i++ < max_iter; )
            {
                if (MathF.Abs(x1 - x0) < eps) break;
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

        public static float FibonacciMethod(func_1 f, float x0, float x1, float eps = 1e-6f)
        {
            if (x0 > x1) Swap(ref x0, ref x1);
            float a = x0, b = x1;
            float f1 = 1.0f, f2 = 2.0f, f3 = 3.0f;
            int i = 0, max_iters = findN((b - a) / eps);

            for (; i++ < max_iters;)
            {
                if (MathF.Abs(x1 - x0) < eps) break;

                x0 = b - (b - a) * f1 / f3;
                x1 = b + (b - a) * f2 / f3;

                f1 = f2;
                f2 = f3;
                f3 = f1 + f2;

                if (f(x0) < f(x1))
                {
                    b = x0;
                    continue;
                }
                a = x1;
            }
#if DEBUG
            Console.WriteLine($"Fibonacci iterations: {i}");
#endif
            return (x1 + x0) * 0.5f;
        }
        public static int findN(float value)
        {
            int f1 = 2, f2 = 1;
            if (value <= f2) return 1;
            if (value <= f1) return 2;
            int i = 2;
            while (true)
            {
                ++i;
                f1 += f2;
                f2 = f1 - f2;
                if (f1 > value) return i;
            }
        }
        public static float parabole(float x)
        {
            return MathF.Pow(x, 2.0f);
        }
        public static void Lab1Tester(func_1 f)
        {
            const float x_0 = 5;
            const float x_1 = -3;
            Console.WriteLine("\n");
            Console.WriteLine("x_0 = " + x_0 + ", x_1 = " + x_1);
            Console.WriteLine("Dihotomia   : " + Dichotomy(f, x_0, x_1, 1e-3f));
            Console.WriteLine("GoldenRatio : " + Golden(f, x_0, x_1, 1e-3f));
            Console.WriteLine("Fibonacchi  : " + FibonacciMethod(f, x_0, x_1, 1e-3f));
        }
    }
}
