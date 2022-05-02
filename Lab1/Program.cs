using System;
using System.Numerics;

namespace MO
{
    class Program
    {
        static double parabole(double x)
        {
            return Math.Pow(x, 2.0f); //min x = 0
        }
        static double test_f(double x)
        {
            return (x - 5) * x; //min x=2.5
        }
        static double test_n(Vector vec)
        {
            return Math.Pow(vec[0] - 2, 2.0f) + Math.Pow(vec[1] - 2, 2);
        }
        static double TestFunc2D(Vector x)
        {
            return (x[0] - 5) * x[0] + (x[1] - 3) * x[1];//{2.5,1.5}
        }
        static double TestFunc2DWithError(Vector x)
        {
            return (x[0] - 5) * x[0] + (x[1] - 3) * x[1] + 10000 * Math.Pow(x[1] + x[0] - 4.5, 2);//{2.5,1.5}
        }
        static double TestFuncNoError(Vector v)
        {
            return Math.Pow(v[0] - 4.0, 2.0) + Math.Pow(v[1] - 4.0, 2.0);
        }
        static double TestFuncWithError(Vector v)
        {
            return Math.Pow(v[0] - 4.0, 2.0) + Math.Pow(v[1] - 4.0, 2.0) + 10000*Math.Pow(v[0] + v[1] - 5.0, 2);
        }
        static double TestFuncInsideError(Vector v)
        {
            double lambd = 1.0, radius = 10.0;
            return Math.Pow(v[0] - 4.0, 2.0) + Math.Pow(v[1] - 4.0, 2.0) + lambd * Math.Pow((Math.Pow(v[0] - 4.0, 2.0) + Math.Pow(v[1] - 4.0, 2.0) - Math.Pow(radius, 2.0)), -1.0);
        }
        static void Main(string[] args)
        {
            ///  Lab1.Lab1Tester(parabole);
            /*Lab1.Lab1Tester(test_f);
            Lab2.Lab2Test(TestFunc2D);*/
            //Lab2.Lab3Test(TestFunc2D);
            //Lab2.Lab3Test(TestFuncNoError);
            //Lab2.ErrorFuncTest(TestFunc2D, TestFunc2DWithError);
            Lab2.ErrorFuncTest(TestFuncNoError, TestFuncInsideError);
            //Symplex.SympexTest();
        }
    }
}
