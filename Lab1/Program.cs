using System;

namespace Lab1
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }

        static float dichotomy(Func<float, float> f, float x0, float x1, float eps)
        {
            float y1, y2;
            while (true)
            {
                y1 = f((x0 + x1) / 2 - eps);
                y2 = f((x0 + x1) / 2 + eps);
                if (y1 < y2) x0 = (x0 + x1) / 2 - eps;
                else x0 = (x0 + x1) / 2 + eps;
                if (Math.Abs(x1 - x0) < 2 * eps) return (x1 + x0) / 2;
            }
        }

        static float golden(Func<float, float> f, float x0, float x1, float eps)
        {
            float y1, y2;
            float fi1 = (float)(2 / (1 + Math.Sqrt(5)));
            float fi2 = (float)((-1 + Math.Sqrt(5)) / (1 + Math.Sqrt(5));
            while (true)
            {
                y1 = f((x0 + x1) / fi1 - eps);
                y2 = f((x0 + x1) / fi2 + eps);
                if (y1 < y2) x0 = (x0 + x1) / 2 - eps;
                else x0 = (x0 + x1) / 2 + eps;
                if (Math.Abs(x1 - x0) < 2 * eps) return (x1 + x0) / 2;
            }
        }
    }
}
