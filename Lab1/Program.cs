using System;

namespace Lab1
{
    class Program
    {
        static void Main(string[] args)
        {
            float x0 = -3f;
            float x1 = 3f;
            float eps = 0.2f;
            float ans1 = dichotomy(parabole, x0, x1, eps);
            float ans2 = golden(parabole, x0, x1, eps);
            Console.WriteLine($"Dichotomy: {ans1} \nGolden: {ans2}");
        }

        static float dichotomy(Func<float, float> f, float x0, float x1, float eps)
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

        static float golden(Func<float, float> f, float x0, float x1, float eps)
        {
            float y1, y2;
            float fi1 = (float)((1 + Math.Sqrt(5)) * 0.5f);
            while (true)
            {
                y1 = f(x0 + (x1 - x0) / fi1);
                y2 = f(x1 - (x1 - x0) / fi1);
                if (y1 < y2) x0 = x1 - (x1 - x0) / fi1;
                else x1 = x0 + (x1 - x0) / fi1;
                if (Math.Abs(x1 - x0) < 2 * eps) return (x1 + x0) * 0.5f;
            }
        }

        static float parabole(float x)
        {
            return (float)Math.Pow(x, 2.0);
        }
    }
}
