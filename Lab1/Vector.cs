using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace MO
{
    public delegate float func_n(Vector x);
    public struct Vector : IEquatable<Vector>
    {
        private float[] data;
        public int Size
        {
            get { return data.Length; }
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
        public static Vector Direvative(func_n func, Vector x, float eps = 1e-6f)
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
            this = Normalized;
            return this;
        }
        public void Resize(int size)
        {
            if (size == Size)
            {
                return;
            }
            float[] data_ = new float[size];
            Array.Copy(data, data_, Math.Min(size, Size));
            data = data_;
        }
        public override string ToString()
        {
            string s = "{ ";
            for (int i = 0; i < data.Length - 1; i++)
            {
                s += data[i].ToString();
                s += ", ";
            }
            s += data[data.Length - 1].ToString();
            s += " }";
            return s;
        }
        public override bool Equals(object obj)
        {
            if (!(obj is Vector))
            {
                return false;
            }
            return Equals((Vector)obj);
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
            data = _data;
        }
        public Vector(int size)
        {
            data = new float[size];
        }
        public Vector(Vector vect)
        {
            data = new float[vect.Size];
            Array.Copy(vect.data, data, vect.Size);
        }
    }
}
