using System;
using System.Numerics;

namespace MO
{
    class Program
    {
        static float parabole(float x)
        {
            return MathF.Pow(x, 2.0f); //min x = 0
        }
        static float test_f(float x)
        {
            return (x - 5) * x; //min x=2.5
        }
        static float test_n(Vector vec)
        {
            return MathF.Pow(vec[0] - 2, 2.0f) + MathF.Pow(vec[1] - 2, 2);
        }
        static void Main(string[] args)
        {
            Lab1.Lab1Tester(parabole);
            Lab1.Lab1Tester(test_f);
            Lab2.Lab2Test(test_n);
        }
    }
}
