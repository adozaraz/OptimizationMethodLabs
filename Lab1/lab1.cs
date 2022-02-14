﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Lab1
{
    class Lab1
    {
        public float dichotomy(Func<float, float> f, float x0, float x1, float eps)
        {
            float y1, y2;
            while (true)
            {
                y1 = f((x0 + x1) * 0.5f - eps);
                y2 = f((x0 + x1) * 0.5f + eps);
                if (y1 < y2) x1 = (x0 + x1) * 0.5f - eps;
                else x0 = (x0 + x1) * 0.5f + eps;
                if (Math.Abs(x1 - x0) < 2 * eps) return (x1 + x0) * 0.5f;
            }
        }

        public float golden(Func<float, float> f, float x0, float x1, float eps)
        {
            float y1, y2;
            float fi1 = (float)((1 + Math.Sqrt(5)) * 0.5f);
            while (true)
            {
                y1 = f(x0 + (x1 - x0) / fi1);
                y2 = f(x1 - (x1 - x0) / fi1);
                if (y1 > y2) x1 = x0 + (x1 - x0) / fi1;
                else x0 = x1 - (x1 - x0) / fi1;
                if (Math.Abs(x1 - x0) < 2 * eps) return (x1 + x0) * 0.5f;
            }
        }

        public float fibonacciMethod(Func<float, float> f, float x0, float x1, float delta, float eps)
        {
            float y1, y2;
            float fib1 = 2, fib2 = 1;
            static int findN(float a, float b, float delta)
            {
                if ((a + b) / delta <= 1) return 2;
                else if ((a + b) / delta <= 2) return 3;
                else
                {
                    int fib1 = 2, fib2 = 1;
                    int n = 3;
                    while (true)
                    {
                        fib1 += fib2;
                        fib2 = fib1 - fib2;
                        ++n;
                        if (fib1 >= (a + b) / delta) return n;
                    }
                }
            }
            int n = findN(x0, x1, delta);
            for (int i = 0; i < n; ++i)
            {
                y1 = f((x0 + x1) * (fib2) / fib1 + eps);
                y2 = f((x0 + x1) * (fib1 - fib2) / fib1 + eps);
                if (y1 > y2) x1 = (x0 + x1) * (fib1 - fib2) / fib1 + eps;
                else x0 = (x0 + x1) * (fib1 - fib2) / fib1 + eps;
            }
            return (x1 - x0) * 0.5f;
        }

        public float parabole(float x)
        {
            return (float)Math.Pow(x, 2.0);
        }
    }
}