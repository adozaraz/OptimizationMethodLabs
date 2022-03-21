using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;


namespace MO
{
    public delegate float func_n(Vector x);
    public class Vector : IEquatable<Vector>
    {
        private List<float> data;
        public int Size
        {
            get { return data.Count; }
        }
        public static Vector operator +(Vector a, Vector b)
        {
            if (a.Size != b.Size)
            {
                throw new Exception("error:: operator+:: vectors of different dimensions");
            }

            Vector res = new Vector(a);
            for (int i = 0; i < a.Size; i++)
            {
                res[i] = a[i] + b[i];
            }
            return res;
        }
        public static Vector operator +(Vector a, float b)
        {
            Vector res = new Vector(a);
            for (int i = 0; i < a.Size; i++)
            {
                res[i] = a[i] + b;
            }
            return res;
        }
        public static Vector operator +(float b, Vector a)
        {
            return a + b;
        }
        public static Vector operator -(Vector a, Vector b)
        {
            if (a.Size != b.Size)
            {
                throw new Exception("error:: operator-:: vectors of different dimensions");
            }

            Vector res = new Vector(a);
            for (int i = 0; i < a.Size; i++)
            {
                res[i] = a[i] - b[i];
            }
            return res;
        }
        public static Vector operator -(Vector a, float b)
        {
            Vector res = new Vector(a);
            for (int i = 0; i < a.Size; i++)
            {
                res[i] = a[i] - b;
            }
            return res;
        }
        public static Vector operator -(float b, Vector a)
        {
            Vector res = new Vector(a);
            for (int i = 0; i < a.Size; i++)
            {
                res[i] = b - a[i];
            }
            return res;
        }
        public static Vector operator *(Vector a, float val)
        {
            Vector res = new Vector(a);
            for (int i = 0; i < a.Size; i++)
            {
                res[i] = a[i] * val;
            }
            return res;
        }
        public static Vector operator *(float val, Vector a)
        {
            return a * val;
        }
        public static implicit operator Vector(float[] value)
        {
            return new Vector(value);
        }
        public static Vector Direction(Vector a, Vector b)
        {
            if (a.Size != b.Size)
            {
                return a;
            }
            return (b - a).Normalized;
        }
        public static Vector Gradient(func_n func, Vector x, float eps = 1e-6f)
        {
            Vector x_l = new Vector(x);
            Vector x_r = new Vector(x);
            Vector df = new Vector(x.Size);
            for (int i = 0; i < x.Size; i++)
            {
                x_l[i] -= eps;
                x_r[i] += eps;

                df[i] = (func(x_r) - func(x_l)) * (0.5f / eps);

                x_l[i] += eps;
                x_r[i] -= eps;
            }
            return df;
        }
        public static float Partial(func_n func, Vector x, int coord_index, float eps = 1e-6f)
        {
            if (x.Size <= coord_index)
            {
                throw new Exception("Partial derivative index out of bounds!");
            }
            x[coord_index] += eps;
            float f_r = func(x);
            x[coord_index] -= (2.0f * eps);
            float f_l = func(x);
            x[coord_index] += eps;
            return (f_r - f_l) / eps * 0.5f;
        }

        public static float Partial2(func_n func, Vector x, int coord_index_1, int coord_index_2, float eps = 1e-6f)
        {
            if (x.Size <= coord_index_2)
            {
                throw new Exception("Partial derivative index out of bounds!");
            }
            x[coord_index_2] -= eps;
            float f_l = Partial(func, x, coord_index_1, eps);
            x[coord_index_2] += (2 * eps);
            float f_r = Partial(func, x, coord_index_1, eps);
            x[coord_index_2] -= eps;
            return (f_r - f_l) / eps * 0.5f;
        }
        public float Magnitude
        {
            get
            {
                float mag = 0.0f;
                foreach (float element in data)
                {
                    mag += (element * element);
                }
                return MathF.Sqrt(mag);
            }
        }
        public Vector Normalized
        {
            get
            {
                Vector v = new Vector(this);
                float inv_mag = 1.0f / v.Magnitude;
                for (int i = 0; i < v.Size; i++)
                {
                    v[i] *= inv_mag;
                }
                return v;
            }
        }
        public Vector Normalize()
        {
            float inv_mag = 1.0f / Magnitude;
            for (int i = 0; i < Size; i++)
            {
                this[i] *= inv_mag;
            }
            return this;
        }
        public float Dot(Vector other)
        {
            if (Size != other.Size)
            {
                throw new Exception("Unable vector dot multiply");
            }

            float dot = 0.0f;

            for (int i = 0; i < other.Size; i++)
            {
                dot += this[i] * other[i];
            }
            return dot;
        }
        public static float Dot(Vector a, Vector b)
        {
            return a.Dot(b);
        }
        public void Resize(int size)
        {
            if (size == Size)
            {
                return;
            }

            if (size > Size)
            {
                for (int i = 0; i < size - Size; i++)
                {
                    data.Add(0.0f);
                }
                return;
            }

            data.RemoveRange(size, Size);
        }

        public void PushBack(float val)
        {
            data.Add(val);
        }
        public override string ToString()
        {
            string s = "{ ";
            for (int i = 0; i < data.Count - 1; i++)
            {
                s += string.Format("{0,0}, ", String.Format("{0:0.000}", data[i]));// .ToString();
            }
            s += string.Format("{0,0}", String.Format("{0:0.000}", data[data.Count - 1]));// data[data.Length - 1].ToString();
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
            if (other.Size != Size)
            {
                return false;
            }
            for (int i = 0; i < other.Size; i++)
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
        public float this[int id]
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
        public Vector(float[] _data)
        {
            data = new List<float>(_data);
        }
        public Vector(int size, float defaultValue = 0.0f)
        {
            data = new List<float>(size);
            for (int i = 0; i < size; i++)
            {
                data.Add(defaultValue);
            }
        }
        public Vector(Vector vect)
        {
            data = new List<float>(vect.data);
        }
    }
}