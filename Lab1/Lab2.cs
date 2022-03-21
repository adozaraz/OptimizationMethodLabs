using System;
using System.Collections.Generic;
using System.Text;

namespace MO
{
    public static class Lab2
    {
        public static Vector Dichotomy(func_n f, Vector x0, Vector x1, float eps = 1e-3f, int max_iter = 1000)
        {
            Vector xc, dir;

            dir = Vector.Direction(x0, x1);

            int i = 0;

            for (; i++ != max_iter;)
            {
                if ((x1 - x0).Magnitude < eps) break;
                xc = (x1 - x0) * 0.5f;
                if (f(xc + dir * eps) > f(xc - dir * eps))
                {
                    x1 = xc;
                    continue;
                }
                x0 = xc;
            }
#if DEBUG
            Console.WriteLine($"N-Dichotomy iterations number: {i}");
#endif
            return (x1 + x0) * 0.5f;
        }

        public static Vector GoldenRatio(func_n f, Vector x0, Vector x1, float eps = 1e-3f, int max_iters = 1000)
        {
            Vector a = new Vector(x0);
            Vector b = new Vector(x1);
            Vector dx;

            int i = 0;

            float oneDimPhi = 1.0f / (float)Lab1.Phi;

            for (; i++ != max_iters;)
            {
                dx = (b - a) * oneDimPhi;
                x1 = b - dx;
                x0 = a + dx;

                if ((x1 - x0).Magnitude < eps)
                {
                    break;
                }

                if (f(x0) > f(x1))
                {
                    b = x0;
                    continue;
                }
                a = x1;
            }
#if DEBUG
            Console.WriteLine($"N-Golden iterations number : {i}");
#endif
            return (a + b) * 0.5f;
        }
        public static Vector Fibonacci(func_n f, Vector x0, Vector x1, float eps = 1e-3f)
        {
            int max_iters = Lab1.findN((x1 - x0).Magnitude / eps);

            Vector a = new Vector(x0);
            Vector b = new Vector(x1);
            Vector dx;

            float f1 = 1.0f, f2 = 2.0f, f3 = 3.0f;

            int i = 0;

            for (; i++ != max_iters;)
            {
                dx = b - a;
                x0 = b - dx * (f1 / f3);
                x1 = b + dx * (f2 / f3);

                f1 = f2;
                f2 = f3;
                f3 = f1 + f2;

                if ((x1 - x0).Magnitude < eps)
                {
                    break;
                }

                if (f(x0) < f(x1))
                {
                    b = x0;
                    continue;
                }
                a = x1;
            }
#if DEBUG
            Console.WriteLine($"Fibonacci iterations number : {i}");
#endif
            return (a + b) * 0.5f;
        }
        public static Vector PerCoordDescend(func_n f, Vector xStart, float eps = 1e-3f, int max_iters = 1000)
        {
            int cntr = 0;
            Vector x0 = new Vector(xStart);
            Vector x1 = new Vector(xStart);
            float step = 1.0f;
            float y1, y0;
            while (true)
            {
                for (int i = 0; i < x0.Size; i++)
                {
                    cntr++;
                    if (cntr == max_iters)
                    {
#if DEBUG
                        Console.WriteLine("per coord descend iterations number : " + cntr);
#endif
                        return x0;
                    }
                    x1[i] -= eps;
                    y0 = f(x1);
                    x1[i] += 2 * eps;
                    y1 = f(x1);
                    x1[i] -= eps;

                    if (y0 > y1)
                    {
                        x1[i] += step;
                    }

                    else
                    {
                        x1[i] -= step;
                    }
                    x1 = Dichotomy(f, x0, x1, eps, max_iters);
                    if ((x1 - x0).Magnitude < eps)
                    {
#if DEBUG
                        Console.WriteLine("per coord descend iterations number : " + cntr);
#endif
                        return x0;
                    }
                    x0 = new Vector(x1);
                }
            }
        }
        public static void Lab2Test(func_n f)
        {
            Vector x1 = new float[] { 0, 0 };
            Vector x0 = new float[] { 5, 5 };
            Console.WriteLine("\n{ x, y } = agrmin((x - 2) * (x - 2) + (y - 2) * (y - 2))\n");
            Console.WriteLine($"x0 = {x0}, x1 = {x1}");
            Console.WriteLine($"Dihotomia              : {Dichotomy(f, x1, x0)}");
            Console.WriteLine($"GoldenRatio            : {GoldenRatio(f, x1, x0)}");
            Console.WriteLine($"Fibonacci              : {Fibonacci(f, x1, x0)}");
            Console.WriteLine($"PerCoordDescend        : {PerCoordDescend(f, x1)}");
        }
    }
}
