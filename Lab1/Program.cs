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
            Lab1 f = new Lab1();
            float ans1 = f.dichotomy(f.parabole, x0, x1, eps);
            float ans2 = f.golden(f.parabole, x0, x1, eps);
            Console.WriteLine($"Dichotomy: {ans1} \nGolden: {ans2}");
        }

        
    }
}
