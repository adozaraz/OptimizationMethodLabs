using System;
using System.Numerics;

namespace Lab1
{
    class Program
    {
        static void Main(string[] args)
        {
            float x0 = -3f;
            float x1 = 3f;
            float eps = 0.2f;
            float delta = 0.1f;
            float lambda = 2;
            Vector2 vec2 = new Vector2(3, 3);
            /*
            float ans1 = f.dichotomy(f.parabole, x0, x1, eps);
            float ans2 = f.golden(f.parabole, x0, x1, eps);
            float ans3 = f.fibonacciMethod(f.parabole, x0, x1, delta, eps);
            Console.WriteLine($"Dichotomy: {ans1} \nGolden: {ans2}\nFibonacci: {ans3}");*/
            Lab2 lab2 = new Lab2();
            Vector2 ans = lab2.per_coordinate_descend(Lab1.stoneParabole, vec2, eps, lambda);
            Console.WriteLine($"Descent: ({ans.X}, {ans.Y})");
        }
    }
}
