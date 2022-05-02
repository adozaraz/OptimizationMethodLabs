using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace MO
{
    public enum SolutionType
    {
        Single = 0,
        Infinite = 1,
        None = 2
    }
    public class Matrix : IEquatable<Matrix>
    {
        private List<Vector> rows;
        public Matrix AddCol(Vector col)
        {
            if (col.Count != NRows)
            {
                throw new Exception("Error::AddCol::col.Size != NRows");
            }
            for (int i = 0; i < rows.Count; i++)
            {
                rows[i].PushBack(col[i]);
            }
            return this;
        }

        public Matrix AddRow(Vector row)
        {
            if (row.Count != NCols)
            {
                throw new Exception("Error::AddRow::row.Size != NCols");
            }
            rows.Add(row);
            return this;
        }
        public bool Equals([AllowNull] Matrix other)
        {
            if (other.rows.Count != rows.Count)
            {
                return false;
            }
            for (int i = 0; i < rows.Count; i++)
            {
                if (!other[i].Equals(this[i]))
                {
                    return false;
                }
            }
            return true;
        }
        public override bool Equals(object other)
        {
            if (!(other is Matrix))
            {
                return false;
            }
            return Equals(other as Matrix);
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(rows);
        }
        public override string ToString()
        {
            string s = "{\n";
            for (int i = 0; i < rows.Count - 1; i++)
            {
                s += " " + rows[i].ToString();
                s += ",\n";
            }
            s += " " + rows[rows.Count - 1].ToString();
            s += "\n}";
            return s;
        }
        public Vector this[int id]
        {
            get
            {
                return rows[id];
            }
            set
            {
                rows[id] = value;
            }
        }
        public int NRows
        {
            get
            {
                return rows.Count;
            }
        }
        public int NCols
        {
            get
            {
                if (NRows == 0)
                {
                    return 0;
                }
                return rows[0].Count;
            }
        }

        public List<Vector> Rows
        {
            get
            {
                return rows;
            }
        }
        public int[] Size
        {
            get
            {
                return new int[] { NRows, NCols };
            }
        }
        public Matrix(params Vector[] rows)
        {
            if (rows == null)
            {
                throw new Exception("Data is null...");
            }

            if (rows.Length == 0)
            {
                throw new Exception("Data is empty...");
            }

            int row_size = rows[0].Count;

            for (int i = 0; i < rows.Length; i++)
            {
                if (rows[i].Count != row_size)
                {
                    throw new Exception("Incorrect matrix data");
                }
            }
            this.rows = new List<Vector>(rows);
        }
        public Matrix(int n_rows, int n_cols, double defualtVal = 0.0)
        {
            rows = new List<Vector>(n_rows);

            for (int i = 0; i < n_rows; i++)
            {
                rows.Add(new Vector(n_cols, defualtVal));
            }
        }
        public Matrix(Matrix original)
        {
            rows = new List<Vector>(original.rows.Count);

            for (int i = 0; i < original.rows.Count; i++)
            {
                rows.Add(new Vector(original.rows[i]));
            }
        }
        public static implicit operator Matrix(Vector[] rows)
        {
            return new Matrix(rows);
        }
        public static Matrix Hessian(func_n f, Vector x, double eps = 1e-6)
        {
            Matrix res = new Matrix(x.Count, x.Count);
            int row, col;
            for (row = 0; row < res.NRows; row++)
            {
                for (col = 0; col <= row; col++)
                {
                    res[row][col] = Vector.Partial2(f, x, row, col, eps);
                    res[col][row] = res[row][col];
                }
            }
            return res;
        }
        public static int Rank(Matrix A)
        {
            int n = A.NRows;

            int m = A.NCols;

            int rank = 0;

            bool[] row_selected = new bool[n];

            for (int i = 0; i < row_selected.Length; i++)
            {
                row_selected[i] = false;
            }

            for (int i = 0; i < m; i++)
            {
                int j;
                for (j = 0; j < n; j++)
                {
                    if (!row_selected[j] && Math.Abs(A[j][i]) > 1e-12)
                    {
                        break;
                    }
                }

                if (j != n)
                {
                    ++rank;

                    row_selected[j] = true;

                    for (int p = i + 1; p < m; p++)
                    {
                        A[j][p] /= A[j][i];
                    }

                    for (int k = 0; k < n; k++)
                    {
                        if (k != j && Math.Abs(A[k][i]) > 1e-12)
                        {
                            for (int p = i + 1; p < m; p++)
                            {
                                A[k][p] -= A[j][p] * A[k][i];
                            }
                        }
                    }
                }
            }
            return rank;
        }
        public static Matrix Zeros(int n_rows, int n_cols)
        {
            return new Matrix(n_rows, n_cols);
        }
        public static Matrix Zeros(int size)
        {
            return Zeros(size, size);
        }
        public static Matrix Identity(int n_rows, int n_cols)
        {
            Matrix I = new Matrix(n_rows, n_cols);

            for (int i = 0; i < Math.Min(n_rows, n_cols); i++)
            {
                I[i][i] = 1.0;
            }
            return I;
        }
        public static Matrix Identity(int size)
        {
            return Identity(size, size);
        }
        public static void LU(ref Matrix src, out Matrix low, out Matrix up)
        {
            if (src.NCols != src.NRows)
            {
                throw new Exception("LU decomposition error::non square matrix");
            }

            low = new Matrix(src.NCols, src.NRows);

            up = new Matrix(src.NCols, src.NRows);

            int i, j, k;

            for (i = 0; i < src.NRows; i++)
            {
                for (j = 0; j < src.NRows; j++)
                {
                    if (j >= i)
                    {
                        low[j][i] = src[j][i];

                        for (k = 0; k < i; k++)
                        {
                            low[j][i] = low[j][i] - low[j][k] * up[k][i];
                        }
                    }
                }

                for (j = 0; j < src.NRows; j++)
                {
                    if (j < i)
                    {
                        continue;
                    }
                    if (j == i)
                    {
                        up[i][j] = 1.0;
                        continue;
                    }
                    up[i][j] = src[i][j] / low[i][i];

                    for (k = 0; k < i; k++)
                    {
                        up[i][j] = up[i][j] - ((low[i][k] * up[k][j]) / low[i][i]);
                    }
                }
            }
        }
        private static Vector Linsolve(ref Matrix low, ref Matrix up, ref Vector b)
        {
            double det = 1.0;

            Vector x, z;

            for (int i = 0; i < up.NRows; i++)
            {
                det *= (up[i][i] * up[i][i]);
            }

            if (Math.Abs(det) < 1e-12)
            {
                return null;
            }

            z = new Vector(up.NRows);

            double tmp;

            for (int i = 0; i < z.Count; i++)
            {
                tmp = 0.0;

                for (int j = 0; j < i; j++)
                {
                    tmp += z[j] * low[i][j];
                }
                z[i] = (b[i] - tmp) / low[i][i];
            }

            x = new Vector(up.NRows);

            for (int i = z.Count - 1; i >= 0; i--)
            {
                tmp = 0.0;
                for (int j = i + 1; j < z.Count; j++)
                {
                    tmp += x[j] * up[i][j];
                }
                x[i] = (z[i] - tmp);
            }

            return x;
        }
        public static Vector Linsolve(Matrix mat, Vector b)
        {
            if (mat.NRows != mat.NCols)
            {
                throw new Exception("non square matrix");
            }

            Matrix low, up;

            LU(ref mat, out low, out up);

            return Linsolve(ref low, ref up, ref b);
        }
        public static Matrix Invert(Matrix mat)
        {
            if (mat.NRows != mat.NCols)
            {
                throw new Exception("non square matrix");
            }

            Matrix low, up, inv;

            LU(ref mat, out low, out up);

            Vector b, col;

            b = new Vector(mat.NRows);

            for (int i = 0; i < b.Count; i++)
            {
                b[i] = 0.0;
            }

            inv = Zeros(mat.NRows);

            for (int i = 0; i < mat.NCols; i++)
            {
                b[i] = 1.0;
                col = Linsolve(ref low, ref up, ref b);
                if (col == null)
                {
                    throw new Exception("unable to find matrix inversion");
                }
                if (col.Count == 0)
                {
                    throw new Exception("unable to find matrix inversion");
                }
                b[i] = 0.0;
                for (int j = 0; j < mat.NRows; j++)
                {
                    inv[j][i] = col[j];
                }
            }
            return inv;
        }
        public static Matrix Transpose(Matrix mat)
        {
            Matrix trans = new Matrix(mat.NCols, mat.NRows);
            for (int i = 0; i < mat.NRows; i++)
            {
                for (int j = 0; j < mat.NCols; j++)
                {
                    trans[j][i] = mat[i][j];
                }
            }
            return trans;
        }
        public static SolutionType CheckSystem(Matrix A, Vector b)
        {
            Matrix a = new Matrix(A);

            int rank_a = Matrix.Rank(a);

            Matrix ab = new Matrix(A);

            int rank_a_b = Matrix.Rank(ab.AddCol(b));

#if DEBUG
            Console.WriteLine($"rank ( A ) {rank_a}\n");
            Console.WriteLine($"rank (A|b) {rank_a_b}\n");
            if (rank_a == rank_a_b)
            {
                Console.WriteLine("one solution\n");
            }
            if (rank_a < rank_a_b)
            {
                Console.WriteLine("infinite amount of solutions\n");
            }
            if (rank_a > rank_a_b)
            {
                Console.WriteLine("no solutions\n");
            }
#endif

            if (rank_a == rank_a_b)
            {
                return SolutionType.Single;
            }
            if (rank_a < rank_a_b)
            {
                return SolutionType.Infinite;
            }
            if (rank_a > rank_a_b)
            {
                return SolutionType.None;
            }
            throw new Exception("error :: check_system");
        }
        public static Matrix operator *(Matrix a, Matrix b)
        {
            if (a.NCols != b.NRows)
            {
                throw new Exception("Error matrix multiplication::a.NCols != b.NRows");
            }

            Matrix b_t = Transpose(b);

            Matrix res = new Matrix(a.NRows, b.NCols);

            Parallel.For(0, a.NRows, (i) =>
            {
                for (int j = 0; j < b.NCols; j++)
                {
                    res[i][j] = Vector.Dot(a[i], b_t[j]);
                }
            });

            return res;
        }
        public static Vector operator *(Matrix mat, Vector vec)
        {
            if (mat.NCols != vec.Count)
            {
                throw new Exception("unable to matrix and vector myltiply");
            }
            Vector result = new Vector(mat.NRows);
            int cntr = 0;
            foreach (Vector row in mat.rows)
            {
                result[cntr++] = Vector.Dot(row, vec);
            }
            return result;
        }
        public static Vector operator *(Vector vec, Matrix mat)
        {
            if (mat.NRows != vec.Count)
            {
                throw new Exception("unable to matrix and vector myltiply");
            }
            Vector result = new Vector(mat.NCols);

            for (int i = 0; i < mat.NCols; i++)
            {
                for (int j = 0; j < mat.NRows; j++)
                {
                    result[i] += mat[j][i] * vec[i];
                }
            }

            return result;
        }
        public static Matrix operator *(Matrix mat, double a)
        {
            Matrix result = new Matrix(mat);
            int cntr = 0;
            foreach (Vector row in mat.rows)
            {
                result[cntr++] *= a;
            }
            return result;
        }
        public static Matrix operator *(double a, Matrix mat)
        {
            return mat * a;
        }
        public static Matrix operator +(Matrix a, Matrix b)
        {
            if (a.NCols != b.NCols)
            {
                throw new Exception("unable to add matrix a to matrix b");
            }
            if (a.NRows != b.NRows)
            {
                throw new Exception("unable to add matrix a to matrix b");
            }

            Matrix result = new Matrix(a.NRows, a.NCols);

            for (int i = 0; i < a.NRows; i++)
            {
                result[i] = a[i] + b[i];
            }
            return result;
        }
        public static Matrix operator +(Matrix a, double b)
        {
            Matrix result = new Matrix(a.NRows, a.NCols);

            for (int i = 0; i < a.NRows; i++)
            {
                result[i] = a[i] + b;
            }
            return result;
        }
        public static Matrix operator +(double b, Matrix a)
        {
            return a + b;
        }
        public static Matrix operator -(Matrix a, Matrix b)
        {
            if (a.NCols != b.NCols)
            {
                throw new Exception("unable to add matrix a to matrix b");
            }
            if (a.NRows != b.NRows)
            {
                throw new Exception("unable to add matrix a to matrix b");
            }

            Matrix result = new Matrix(a.NRows, a.NCols);

            for (int i = 0; i < a.NRows; i++)
            {
                result[i] = a[i] - b[i];
            }
            return result;
        }
        public static Matrix operator -(Matrix a, double b)
        {
            Matrix result = new Matrix(a.NRows, a.NCols);

            for (int i = 0; i < a.NRows; i++)
            {
                result[i] = a[i] - b;
            }
            return result;
        }
        public static Matrix operator -(double b, Matrix a)
        {
            Matrix result = new Matrix(a.NRows, a.NCols);

            for (int i = 0; i < a.NRows; i++)
            {
                result[i] = b - a[i];
            }
            return result;
        }
    }
}