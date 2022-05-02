using System;
using System.Collections.Generic;
using System.Text;

namespace MO
{
    public static class Lab2
    {
        public static Vector Dichotomy(func_n f, Vector x0, Vector x1, double eps = 1e-3f, int max_iter = 1000)
        {
            Vector xc, dir;

            dir = Vector.Direction(x0, x1);

            int i = 0;

            for (; i++ != max_iter;)
            {
                if ((x1 - x0).Magnitude < eps) break;
                xc = (x1 + x0) * 0.5f;
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

        public static Vector GoldenRatio(func_n f, Vector x0, Vector x1, double eps = 1e-3f, int max_iters = 1000)
        {
            Vector a = new Vector(x0);
            Vector b = new Vector(x1);
            Vector dx;

            int i = 0;

            double oneDimPhi = 1.0f / Lab1.Phi;

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
        public static Vector Fibonacci(func_n f, Vector x0, Vector x1, double eps = 1e-3f)
        {
            int max_iters = Lab1.ClosestFibonacci((x1 - x0).Magnitude / eps);

            Vector a = new Vector(x0);
            Vector b = new Vector(x1);
            Vector dx;

            double f1 = 1.0f, f2 = 2.0f, f3 = 3.0f;

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
        public static Vector PerCoordDescend(func_n f, Vector x_start, double eps = 1e-5, int max_iters = 1000)
        {
            Vector x0 = new Vector(x_start);

            Vector x1 = new Vector(x_start);

            double step = 1.0;

            double xi, y1, y0;

            int optСoordN = 0, coordId;

            int i = 0;

            for (; i < max_iters; ++i)
            {
                coordId = i % x0.Count;

                x1[coordId] -= eps;

                y0 = f(x1);

                x1[coordId] += 2 * eps;

                y1 = f(x1);

                x1[coordId] -= eps;

                x1[coordId] = y0 > y1 ? x1[coordId] += step : x1[coordId] -= step;

                xi = x0[coordId];

                x1 = Dichotomy(f, x0, x1, eps, max_iters);

                x0 = new Vector(x1);

                if (Math.Abs(x1[coordId] - xi) < eps)
                {
                    ++optСoordN;

                    if (optСoordN == x1.Count)
                    {
#if DEBUG
                        Console.WriteLine($"per coord descend iterations number : {i}");
#endif
                        return x0;
                    }
                    continue;
                }
                optСoordN = 0;
            }
#if DEBUG
            Console.WriteLine($"per coord descend iterations number : {max_iters}");
#endif
            return x0;
        }
        public static Vector GradientDescend(func_n f, Vector x_start, double eps = 1e-5, int max_iters = 1000)
        {
            Vector xi = new Vector(x_start);

            Vector xi1 = new Vector(x_start);

            int cntr = 0;
            /*
             * Итерационная формула:
             * x[i+1] = x[k] - lambda[i]*gradient(f(x[i])), lambda[i] находится методом наискорейшего спуска
             * lambda[i] = argmin_lambda(f(x[k] - lambda*gradient(f(x[i]))))
             */
            while (true)
            {
                ++cntr;

                if (cntr == max_iters)
                {
                    break;
                }

                xi1 = xi - Vector.Gradient(f, xi, eps);

                xi1 = Dichotomy(f, xi, xi1, eps, max_iters);

                if ((xi1 - xi).Magnitude < eps)
                {
                    break;
                }

                xi = xi1;
            }
#if DEBUG
            Console.WriteLine($"gradient descend iterations number : {cntr}");
#endif
            return (xi1 + xi) * 0.5;
        }
        public static Vector СonjGradientDescend(func_n f, Vector x_start, double eps = 1e-5, int max_iters = 1000)
        {
            Vector xi = new Vector(x_start);

            Vector xi1 = new Vector(x_start);

            Vector si = Vector.Gradient(f, x_start, eps) * (-1.0), si1;

            double omega;

            int cntr = 0;

            while (true)
            {
                ++cntr;

                if (cntr == max_iters)
                {
                    break;
                }

                xi1 = xi + si;

                xi1 = Dichotomy(f, xi, xi1, eps, max_iters);

                if ((xi1 - xi).Magnitude < eps)
                {
                    break;
                }

                si1 = Vector.Gradient(f, xi1, eps);

                omega = Math.Pow((si1).Magnitude, 2) / Math.Pow((si).Magnitude, 2);

                si = si * omega - si1;

                xi = xi1;
            }
#if DEBUG
            Console.WriteLine($"Conj gradient descend iterations number : {cntr}");
#endif
            return (xi1 + xi) * 0.5;
        }
        ////////////////////
        /// Lab. work #4 ///
        ////////////////////
        public static Vector NewtoneRaphson(func_n f, Vector x_start, double eps = 1e-6, int max_iters = 1000)
        {
            Vector xi = new Vector(x_start);

            Vector xi1 = new Vector(x_start);

            int cntr = 0;

            while (true)
            {
                ++cntr;

                if (cntr == max_iters)
                {
                    break;
                }

                xi1 = xi - Matrix.Invert(Matrix.Hessian(f, xi, eps)) * Vector.Gradient(f, xi, eps);

                if ((xi1 - xi).Magnitude < eps)
                {
                    break;
                }

                xi = xi1;
            }
#if DEBUG
            Console.WriteLine($"Newtone - Raphson iterations number : {cntr}");
#endif
            return (xi1 + xi) * 0.5;
        }

        public static void ErrorFuncTest(func_n noError, func_n error, double eps = 1E-06, int maxIters = 1000)
        {
            Vector xStart = new Vector(1.0, 5.0);
            Vector vError = NewtoneRaphson(error, xStart, eps, maxIters);
            Console.WriteLine($"NewtoneRaphson: {vError}");
            Console.WriteLine($"Error: {error(vError) - noError(vError)}");
        }

        public static void Lab2Test(func_n f)
        {
            Vector x1 = new double[] { 0, 0 };
            Vector x0 = new double[] { 5, 3 };
            Console.WriteLine($"x0 = {x0}, x1 = {x1}");
            Console.WriteLine($"Dihotomia              : {Dichotomy(f, x1, x0)}");
            Console.WriteLine($"GoldenRatio            : {GoldenRatio(f, x1, x0)}");
            Console.WriteLine($"Fibonacci              : {Fibonacci(f, x1, x0)}");
            Console.WriteLine($"PerCoordDescend        : {PerCoordDescend(f, x1)}");
        }

        public static void Lab3Test(func_n f)
        {
            Vector x1 = new double[] { 0, 0 };
            Vector x0 = new double[] { 5, 5 };
            Vector x__ = new double[] { -5, -3 };
            Console.WriteLine($"GradientDescend        : {GradientDescend(f, x1)}");
            Console.WriteLine($"СonjGradientDescend    : {СonjGradientDescend(f, x1)}");
            Console.WriteLine($"NewtoneRaphson         : {NewtoneRaphson(f, x1)}\n");
        }
    }
}
