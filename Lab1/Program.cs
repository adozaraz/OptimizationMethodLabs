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
            float delta = 0.1f;
            Lab1 f = new Lab1();
            float ans1 = f.dichotomy(f.parabole, x0, x1, eps);
            float ans2 = f.golden(f.parabole, x0, x1, eps);
            float ans3 = f.fibonacciMethod(f.parabole, x0, x1, delta, eps);
            Console.WriteLine($"Dichotomy: {ans1} \nGolden: {ans2}\nFibonacci: {ans3}");
        }

        
    }
}
