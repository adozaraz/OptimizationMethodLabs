using System;
using System.Collections.Generic;
using System.Text;

namespace MO
{
    public enum Sign
    {
        Equal = 0,
        Less = 1,
        More = 2
    }

    public enum SymplexProblemType
    {
        Min = 0,
        Max = 1,
    }

    public class Symplex
    {
        private List<Sign> ineqs;

        private List<int> f_mod_args;

        private List<int> natural_args_ids;

        private List<int> artificial_args_ids;

        private List<int> basis_args;

        private Matrix symplex_t;

        private Matrix bounds_m;

        private Vector bounds_v;

        private Vector prices_v;

        private SymplexProblemType mode = SymplexProblemType.Max;

        public bool IsTargetFuncModified()
        {
            return f_mod_args.Count != 0;
        }
        public bool IsPlanOptimal()
        {
            Vector row = symplex_t[symplex_t.NRows - 1];

            bool opt = true;

            for (int i = 0; i < row.Count - 1; i++)
            {
                if (row[i] < 0)
                {
                    opt = false;
                    break;
                }
            }
            if (IsTargetFuncModified())
            {
                if (!opt)
                {
                    return opt;
                }
                Vector row_ = symplex_t[symplex_t.NRows - 2];

                foreach (int id in natural_args_ids)
                {
                    if (row_[id] < 0)
                    {
                        opt = false;
                        break;
                    }
                }
            }

            return opt;
        }
        private int GetMainCol()
        {

            Vector row = symplex_t[symplex_t.NRows - 1];

            double delta = 0;

            int index = -1;

            for (int i = 0; i < row.Count - 1; i++)
            {
                if (row[i] >= delta)
                {
                    continue;
                }
                delta = row[i];
                index = i;
            }

            if (IsTargetFuncModified() && index == -1)
            {
                Vector row_add = symplex_t[symplex_t.NRows - 2];

                foreach (int id in natural_args_ids)
                {
                    if (row_add[id] >= delta)
                    {
                        continue;
                    }
                    delta = row_add[id];
                    index = id;
                }
            }
            return index;
        }
        int GetMainRow(int symplex_col)
        {

            double delta = 1e12;

            int index = -1;

            double a_ik;

            int b_index = symplex_t[0].Count - 1;

            int rows_n = IsTargetFuncModified() ? symplex_t.NRows - 2 : symplex_t.NRows - 1;

            for (int i = 0; i < rows_n; i++)
            {
                a_ik = symplex_t[i][symplex_col];

                if (a_ik < 0)
                {
                    continue;
                }
                if (symplex_t[i][b_index] / a_ik > delta)
                {
                    continue;
                }
                delta = symplex_t[i][b_index] / a_ik;
                index = i;
            }

            return index;
        }
        private void BuildVirtualBasisCol(int ineq_id, Sign _ineq, ref int col_index, ref int col_index_aditional)
        {
            if (_ineq == Sign.Equal)
            {
                for (int row = 0; row < symplex_t.NRows; row++)
                {
                    if (row == ineq_id)
                    {
                        symplex_t[row].PushBack(1.0);
                        continue;
                    }
                    symplex_t[row].PushBack(0.0);
                }

                col_index = symplex_t[0].Count - 1;

                col_index_aditional = symplex_t[0].Count - 1;

                return;
            }

            if (_ineq == Sign.More)
            {
                for (int row = 0; row < symplex_t.NRows; row++)
                {
                    if (row == ineq_id)
                    {
                        symplex_t[row].PushBack(-1.0);

                        symplex_t[row].PushBack(1.0);

                        continue;
                    }

                    symplex_t[row].PushBack(0.0);

                    symplex_t[row].PushBack(0.0);
                }

                col_index = symplex_t[0].Count - 2;

                col_index_aditional = symplex_t[0].Count - 1;

                return;
            }

            for (int row = 0; row < symplex_t.NRows; row++)
            {
                if (row == ineq_id)
                {
                    symplex_t[row].PushBack(1.0);
                    continue;
                }
                symplex_t[row].PushBack(0.0);
            }

            col_index = symplex_t[0].Count - 1;

            col_index_aditional = -1;

            return;
        }

        private void BuildSymplexTable()
        {
            symplex_t = new Matrix(bounds_m);
            natural_args_ids.Clear();
            basis_args.Clear();
            f_mod_args.Clear();
            artificial_args_ids.Clear();
            for (int row = 0; row < symplex_t.NRows; row++)
            {
                if (bounds_v[row] >= 0)
                {
                    continue;
                }

                ineqs[row] = ineqs[row] == Sign.Less ? Sign.More : Sign.Less;

                bounds_v[row] *= -1;

                symplex_t[row] = symplex_t[row] * (-1.0);
            }


            for (int i = 0; i < prices_v.Count; i++)
            {
                natural_args_ids.Add(i);
            }
            int basis_arg_id = -1;
            int basis_arg_id_add = -1;
            for (int ineq_id = 0; ineq_id < ineqs.Count; ineq_id++)
            {
                BuildVirtualBasisCol(ineq_id, ineqs[ineq_id], ref basis_arg_id, ref basis_arg_id_add);

                natural_args_ids.Add(basis_arg_id);

                if (basis_arg_id_add != -1)
                {
                    basis_args.Add(basis_arg_id_add);
                    f_mod_args.Add(basis_arg_id_add);
                    artificial_args_ids.Add(basis_arg_id_add);
                    continue;
                }

                basis_args.Add(basis_arg_id);
            }

            for (int row = 0; row < symplex_t.NRows; row++)
            {
                symplex_t[row].PushBack(bounds_v[row]);
            }

            Vector s_deltas = new Vector(symplex_t.NCols);

            if (mode == SymplexProblemType.Max)
            {
                for (int j = 0; j < s_deltas.Count; j++)
                {
                    s_deltas[j] = j < prices_v.Count ? -prices_v[j] : 0.0;
                }
            }
            else
            {
                for (int j = 0; j < s_deltas.Count; j++)
                {
                    s_deltas[j] = j < prices_v.Count ? prices_v[j] : 0.0;
                }
            }

            symplex_t.AddRow(s_deltas);

            if (!IsTargetFuncModified())
            {
                return;
            }

            Vector s_deltas_add = new Vector(symplex_t.NCols);

            for (int j = 0; j < f_mod_args.Count; j++)
            {
                s_deltas_add[f_mod_args[j]] = 1.0;
            }

            symplex_t.AddRow(s_deltas_add);
        }

        private bool ExcludeModArgs()
        {
            if (!IsTargetFuncModified())
            {
                return false;
            }

            int last_row_id = symplex_t.NRows - 1;

            for (int i = 0; i < f_mod_args.Count; i++)
            {
                for (int row = 0; row < symplex_t.NRows; row++)
                {
                    if (symplex_t[row][f_mod_args[i]] != 0)
                    {
                        double arg = symplex_t[last_row_id][f_mod_args[i]] / symplex_t[row][f_mod_args[i]];

                        symplex_t[last_row_id] = symplex_t[last_row_id] - arg * symplex_t[row];

                        break;
                    }
                }
            }

            return true;
        }

        private bool ValidateSolution()
        {

            double val = 0;

            int n_rows = IsTargetFuncModified() ? symplex_t.NRows - 2 : symplex_t.NRows - 1;

            int n_cols = symplex_t.NCols - 1;

            for (int i = 0; i < basis_args.Count; i++)
            {
                if (basis_args[i] < NaturalArgsN())
                {
                    val += symplex_t[i][n_cols] * prices_v[basis_args[i]];
                }
            }
            if (mode == SymplexProblemType.Max)
            {
                if (Math.Abs(val - symplex_t[n_rows][n_cols]) < 1e-5)
                {
                    if (IsTargetFuncModified())
                    {
                        return true & (Math.Abs(symplex_t[symplex_t.NRows - 1][symplex_t.NCols - 1]) < 1e-5);
                    }

                    return true;
                }
            }
            if (Math.Abs(val + symplex_t[n_rows][n_cols]) < 1e-5)
            {
                if (IsTargetFuncModified())
                {
                    return true & (Math.Abs(symplex_t[symplex_t.NRows - 1][symplex_t.NCols - 1]) < 1e-5);
                }

                return true;
            }
            return false;
        }

        public int NaturalArgsN()
        {
            return prices_v.Count;
        }

        public Matrix BoundsMatrix()
        {
            return bounds_m;
        }

        public Vector BoundsCoeffs()
        {
            return bounds_v;
        }

        public Vector PricesCoeffs()
        {
            return prices_v;
        }

        public List<Sign> Inequations()
        {
            return ineqs;
        }

        public List<int> BasisArgsuments()
        {
            return basis_args;
        }

        public Matrix SymplexTable()
        {
            return symplex_t;
        }

        public Vector CurrentSymplexSolution(bool only_natural_args = false)
        {
            Vector solution = new Vector(only_natural_args ? NaturalArgsN() : symplex_t.NCols - 1);

            for (int i = 0; i < basis_args.Count; i++)
            {
                if (basis_args[i] >= solution.Count)
                {
                    continue;
                }

                solution[basis_args[i]] = symplex_t[i][symplex_t.NCols - 1];
            }
            return solution;
        }
        public string SymplexToString()//Matrix table, List<int> basis)
        {
            if (symplex_t.NRows == 0)
            {
                return "";
            }

            StringBuilder sb = new StringBuilder();

            int i = 0;

            sb.AppendFormat("{0,-6}", " ");

            for (; i < symplex_t.NCols - 1; i++)
            {
                sb.AppendFormat("|{0,-12}", " x " + (i + 1).ToString());
            }
            sb.AppendFormat("|{0,-12}", " b");

            sb.Append("\n");

            int n_row = -1;

            foreach (Vector row in symplex_t.Rows)
            {
                n_row++;

                if (IsTargetFuncModified())
                {
                    if (n_row == symplex_t.NRows - 2)
                    {
                        sb.AppendFormat("{0,-6}", " d0");
                    }
                    else if (n_row == symplex_t.NRows - 1)
                    {
                        sb.AppendFormat("{0,-6}", " d1");
                    }
                    else
                    {
                        sb.AppendFormat("{0,-6}", " x " + (basis_args[n_row] + 1).ToString());
                    }
                }
                else
                {
                    if (n_row == symplex_t.NRows - 1)
                    {
                        sb.AppendFormat("{0,-6}", " d");
                    }
                    else
                    {
                        sb.AppendFormat("{0,-6}", " x " + (basis_args[n_row] + 1).ToString());
                    }
                }

                for (int col = 0; col < row.Count; col++)
                {
                    if (row[col] >= 0)
                    {
                        sb.AppendFormat("|{0,-12}", " " + NumericUtils.ToRationalStr(row[col]));
                        continue;
                    }
                    sb.AppendFormat("|{0,-12}", NumericUtils.ToRationalStr(row[col]));

                }
                sb.Append("\n");
            }
            sb.Append("\n");

            return sb.ToString();
        }

        public Vector Solve(SymplexProblemType mode = SymplexProblemType.Max)
        {
            this.mode = mode;

            Console.WriteLine($"SymplexProblemType : {SymplexProblemType.Max}\n");

            Vector solution = new Vector(NaturalArgsN());

            BuildSymplexTable();

            double a_ik;

            int main_row;

            int main_col;

            Console.WriteLine("Start symplex table:");
            Console.WriteLine(SymplexToString());

            if (ExcludeModArgs())
            {
                Console.WriteLine("Symplex table after args exclude:");
                Console.WriteLine(SymplexToString());
            }

            while (!IsPlanOptimal())
            {
                main_col = GetMainCol();

                if (main_col == -1)
                {
                    break;
                }

                main_row = GetMainRow(main_col);

                if (main_row == -1)
                {
                    Console.WriteLine("Unable to get main row. Symplex is probably boundless...");
                    return null;
                }

                basis_args[main_row] = main_col;

                a_ik = symplex_t[main_row][main_col];

                symplex_t[main_row] = symplex_t[main_row] * (1.0 / a_ik);

                for (int i = 0; i < symplex_t.NRows; i++)
                {
                    if (i == main_row)
                    {
                        continue;
                    }
                    symplex_t[i] = symplex_t[i] - symplex_t[i][main_col] * symplex_t[main_row];
                }
                solution = CurrentSymplexSolution();

#if DEBUG
                Console.WriteLine($"a_main {{{ main_row + 1}, {main_col + 1}}} = {NumericUtils.ToRationalStr(a_ik)}\n");
                Console.WriteLine(SymplexToString());
                Console.WriteLine($"current solution: {NumericUtils.ToRationalStr(solution)}\n");
#endif
            }
            if (ValidateSolution())
            {
                solution = CurrentSymplexSolution(true);
                Console.WriteLine($"solution: {NumericUtils.ToRationalStr(solution)}\n");
                return solution;
            }
            Console.WriteLine("Symplex is unresolvable");
            return null;
        }
        public Symplex(Matrix a, Vector c, List<Sign> _ineq, Vector b)
        {
            if (b.Count != _ineq.Count)
            {
                throw new Exception("Error symplex creation :: b.size() != inequation.size()");
            }

            if (a.NRows != _ineq.Count)
            {
                throw new Exception("Error symplex creation :: A.rows_number() != inequation.size()");
            }

            if (a.NCols != c.Count)
            {
                throw new Exception("Error symplex creation :: A.cols_number() != price_coeffs.size()");
            }

            natural_args_ids = new List<int>();
            basis_args = new List<int>();
            f_mod_args = new List<int>();
            artificial_args_ids = new List<int>();

            bounds_v = b;

            bounds_m = a;

            prices_v = c;

            ineqs = _ineq;
        }
        public Symplex(Matrix a, Vector c, Vector b)
        {
            if (b.Count != b.Count)
            {
                throw new Exception("Error symplex creation :: b.size() != bouns_coeffs.size()");
            }

            if (a.NCols != c.Count)
            {
                throw new Exception("Error symplex creation :: A.cols_number() != price_coeffs.size()");
            }

            ineqs = new List<Sign>();

            for (int i = 0; i < b.Count; i++)
            {
                ineqs.Add(Sign.Less);
            }

            natural_args_ids = new List<int>();
            basis_args = new List<int>();
            f_mod_args = new List<int>();
            artificial_args_ids = new List<int>();

            bounds_v = b;

            bounds_m = a;

            prices_v = c;
        }
        public static void SympexTest()
        {
            Console.WriteLine("\n/////////////////////////////");
            Console.WriteLine("//////// SymplexTest ////////");
            Console.WriteLine("/////////////////////////////\n");

            Vector b = new Vector(40, 28, 14);
            Vector c = new Vector(2.0, 3.0);

            Matrix A = new Matrix
            (
                          new Vector(-2.0, 6.0),
                          new Vector(3.0, 2.0),
                          new Vector(2.0, -1.0)
            );


            Console.WriteLine(" f(x,c) =  2x1 + 3x2;\n arg_max = {4, 8}, f(arg_max) = 32");
            Console.WriteLine(" |-2x1 + 6x2 <= 40");
            Console.WriteLine(" | 3x1 + 2x2 <= 28");
            Console.WriteLine(" | 2x1 -  x2 <= 14\n");

            Symplex sym_0 = new Symplex(A, c, new List<Sign>() { Sign.Less, Sign.Less, Sign.Less }, b);
            sym_0.Solve(SymplexProblemType.Max);

            Console.WriteLine("\n f(x,c) = -2x1 + 3x2;\n arg_min = {7, 0}, f(arg_min) =-14\n");

            Symplex sym_1 = new Symplex(A, new Vector(-2.0, 3.0), new List<Sign>() { Sign.Less, Sign.Less, Sign.Less }, b);
            sym_1.Solve(SymplexProblemType.Min);


            Console.WriteLine("/////////////////////////////");
            Console.WriteLine(" f(x,c) =  2x1 + 3x2;\n arg_min = {62/5, 54/5}, f(arg_max) = 57 1/5");
            Console.WriteLine(" |-2x1 + 6x2 >= 40");
            Console.WriteLine(" | 3x1 + 2x2 >= 28");
            Console.WriteLine(" | 2x1 -  x2 >= 14\n");
            Symplex sym_2 = new Symplex(A, new Vector(2, 1), new List<Sign>() { Sign.More, Sign.More, Sign.More }, b);
            sym_2.Solve(SymplexProblemType.Min);
            Console.WriteLine(" f(x,c) =  -2x1 - x2;\n arg_min = {62/5, 54/5}, f(arg_max) = -35 3/5");

            Symplex sym_3 = new Symplex(A, new Vector(-2, -1), new List<Sign>() { Sign.More, Sign.More, Sign.More }, b);
            sym_3.Solve(SymplexProblemType.Max);
            Console.WriteLine(" f(x,c) =  2x1 + 3x2;\n arg_min = {none, none}, f(arg_max) = none");
            Symplex sym_4 = new Symplex(A, c, new List<Sign>() { Sign.Equal, Sign.Equal, Sign.Equal }, b);
            sym_4.Solve(SymplexProblemType.Max);

        }

    }
}
