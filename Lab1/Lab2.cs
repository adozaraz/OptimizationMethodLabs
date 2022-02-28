using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;

namespace Lab1
{
    class Lab2
    {
        public Vector2 per_coordinate_descend(Func<float, float, float> f, Vector2 x_0, float eps, float lambda)
        {
            Lab1 check = new Lab1();
            int cntr = 0;
            Vector2 result = new Vector2(x_0.X, x_0.Y);
            Vector2 oldResult = result;
            Vector2 parametrizedVector1, parametrizedVector2;

            while (true)
            {
                oldResult = result;
                if (cntr % 2 != 0)
                {
                    parametrizedVector1 = result;
                    parametrizedVector1.X -= lambda;
                    parametrizedVector2 = result;
                    parametrizedVector2.X += lambda;
                    result.X = Lab1.diochotomyModified(f, parametrizedVector1, parametrizedVector2, eps);
                    ++cntr;
                }
                else
                {
                    parametrizedVector1 = result;
                    parametrizedVector1.Y -= lambda;
                    parametrizedVector2 = result;
                    parametrizedVector2.Y += lambda;
                    result.Y = Lab1.diochotomyModified(f, parametrizedVector1, parametrizedVector2, eps);
                    ++cntr;
                }
                if (result.X*oldResult.X+result.Y*oldResult.Y < eps || cntr > 999)
                {
                    return result;
                }
            }
        }
    }
}
