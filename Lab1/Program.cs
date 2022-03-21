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
        static void Main(string[] args)
        {
            Lab1.Lab1Tester(parabole);
            Lab1.Lab1Tester(test_f);
        }
    }
}
