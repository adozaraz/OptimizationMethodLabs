using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;


namespace MO
{
    public delegate double func_n(Vector x);
    public class Vector : IEquatable<Vector>
    {
        private int fillness = 0;
        private double[] data;
        public int Count
        {
            get { return fillness; }
        }
        public double Magnitude
        {
            get
            {
                double mag = 0.0;
                foreach (double element in data)
                {
                    mag += (element * element);
                }
                return Math.Sqrt(mag);
            }
        }
        public Vector Normalized
        {
            get
            {
                Vector v = new Vector(this);
                double inv_mag = 1.0 / v.Magnitude;
                for (int i = 0; i < v.Count; i++)
                {
                    v[i] *= inv_mag;
                }
                return v;
            }
        }
        public Vector Normalize()
        {
            double inv_mag = 1.0 / Magnitude;
            for (int i = 0; i < Count; i++)
            {
                this[i] *= inv_mag;
            }
            return this;
        }
        public double Dot(Vector other)
        {
            if (Count != other.Count)
            {
                throw new Exception("Unable vector dot multiply");
            }

            double dot = 0.0;

            for (int i = 0; i < other.Count; i++)
            {
                dot += data[i] * other[i];
            }
            return dot;
        }

        public void PushBack(double val)
        {
            if (fillness != data.Length)
            {
                data[fillness] = val;
                fillness++;
                return;
            }

            double[] new_data = new double[(int)(data.Length * 1.5)];

            for (int i = 0; i < Count; i++)
            {
                new_data[i] = data[i];
            }
            new_data[fillness] = val;
            fillness++;
            data = new_data;
        }
        public override string ToString()
        {
            string s = "{ ";
            for (int i = 0; i < Count - 1; i++)
            {
                s += string.Format("{0,0}, ", String.Format("{0:0.000}", data[i]));
            }
            s += string.Format("{0,0}", String.Format("{0:0.000}", data[Count - 1]));
            s += " }";
            return s;
        }
        public override bool Equals(object obj)
        {
            if (!(obj is Vector))
            {
                return false;
            }
            return Equals(obj as Vector);
        }
        public bool Equals([AllowNull] Vector other)
        {
            if (other.Count != Count)
            {
                return false;
            }
            for (int i = 0; i < other.Count; i++)
            {
                if (other[i] != this[i])
                {
                    return false;
                }
            }
            return true;
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(data);
        }
        public double this[int id]
        {
            get
            {
                return data[id];
            }
            set
            {
                data[id] = value;
            }
        }
        public Vector(params double[] _data)
        {
            fillness = _data.Length;

            data = new double[(int)(fillness * 1.5)];

            for (int i = 0; i < fillness; i++)
            {
                data[i] = _data[i];
            }
        }
        public Vector(int size)// double defaultValue = 0.0)
        {
            fillness = size;

            data = new double[(int)(size * 1.5)];

            for (int i = 0; i < size; i++)
            {
                data[i] = (0.0);
            }
        }
        public Vector(Vector vect)
        {
            fillness = vect.fillness;

            data = new double[vect.data.Length];

            for (int i = 0; i < vect.fillness; i++)
            {
                data[i] = vect.data[i];
            }
        }
        public static Vector operator +(Vector a, Vector b)
        {
            if (a.Count != b.Count)
            {
                throw new Exception("error:: operator+:: vectors of different dimensions");
            }

            Vector res = new Vector(a);
            for (int i = 0; i < a.Count; i++)
            {
                res[i] = a[i] + b[i];
            }
            return res;
        }
        public static Vector operator +(Vector a, double b)
        {
            Vector res = new Vector(a);
            for (int i = 0; i < a.Count; i++)
            {
                res[i] = a[i] + b;
            }
            return res;
        }
        public static Vector operator +(double b, Vector a)
        {
            return a + b;
        }
        public static Vector operator -(Vector a, Vector b)
        {
            if (a.Count != b.Count)
            {
                throw new Exception("error:: operator-:: vectors of different dimensions");
            }

            Vector res = new Vector(a);
            for (int i = 0; i < a.Count; i++)
            {
                res[i] = a[i] - b[i];
            }
            return res;
        }
        public static Vector operator -(Vector a, double b)
        {
            Vector res = new Vector(a);
            for (int i = 0; i < a.Count; i++)
            {
                res[i] = a[i] - b;
            }
            return res;
        }
        public static Vector operator -(double b, Vector a)
        {
            Vector res = new Vector(a);
            for (int i = 0; i < a.Count; i++)
            {
                res[i] = b - a[i];
            }
            return res;
        }
        public static Vector operator *(Vector a, double val)
        {
            Vector res = new Vector(a);
            for (int i = 0; i < a.Count; i++)
            {
                res[i] = a[i] * val;
            }
            return res;
        }
        public static Vector operator *(double val, Vector a)
        {
            return a * val;
        }
        public static implicit operator Vector(double[] value)
        {
            return new Vector(value);
        }
        public static Vector Direction(Vector a, Vector b)
        {
            if (a.Count != b.Count)
            {
                return a;
            }
            return (b - a).Normalized;
        }
        public static Vector Gradient(func_n func, Vector x, double eps = 1e-6)
        {
            Vector x_l = new Vector(x);
            Vector x_r = new Vector(x);
            Vector df = new Vector(x.Count);
            for (int i = 0; i < x.Count; i++)
            {
                x_l[i] -= eps;
                x_r[i] += eps;

                df[i] = (func(x_r) - func(x_l)) * (0.5 / eps);

                x_l[i] += eps;
                x_r[i] -= eps;
            }
            return df;
        }
        public static double Partial(func_n func, Vector x, int coord_index, double eps = 1e-6)
        {
            if (x.Count <= coord_index)
            {
                throw new Exception("Partial derivative index out of bounds!");
            }
            x[coord_index] += eps;
            double f_r = func(x);
            x[coord_index] -= (2.0 * eps);
            double f_l = func(x);
            x[coord_index] += eps;
            return (f_r - f_l) / eps * 0.5;
        }

        public static double Partial2(func_n func, Vector x, int coord_index_1, int coord_index_2, double eps = 1e-6)
        {
            if (x.Count <= coord_index_2)
            {
                throw new Exception("Partial derivative index out of bounds!");
            }
            x[coord_index_2] -= eps;
            double f_l = Partial(func, x, coord_index_1, eps);
            x[coord_index_2] += (2 * eps);
            double f_r = Partial(func, x, coord_index_1, eps);
            x[coord_index_2] -= eps;
            return (f_r - f_l) / eps * 0.5;
        }
        public static double Dot(Vector a, Vector b)
        {
            return a.Dot(b);
        }
    }
}