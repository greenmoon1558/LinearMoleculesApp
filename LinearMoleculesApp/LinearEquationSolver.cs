using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LinearMoleculesApp
{
    class LinearEquationSolver
    {
        /// <summary>Computes the solution of a linear equation system.</summary>
        /// <param name="M">
        /// The system of linear equations as an augmented matrix[row, col] where (rows + 1 == cols).
        /// It will contain the solution in "row canonical form" if the function returns "true".
        /// </param>
        /// <returns>Returns whether the matrix has a unique solution or not.</returns>
        /// 
        private static int[,] Shuffle(int n)
        {
            int min = 0;
            int size = n;
            bool[][] resultBool =
                Enumerable.Range(0, 1 << size)
                .Select(i => new BitArray(new int[] { i }).Cast<bool>().Take(size).ToArray())
                .ToArray();
            //return resultBool;
            int[,] result = new int[resultBool.Length, resultBool[0].Length];
            for (int i = 0; i < resultBool.Length; i++)
            {
                for (int j = 0; j < resultBool[0].Length; j++)
                {
                  result[i, j] = resultBool[i][j]?1:0;
                }
                }
            return result;
        }

        public static double[] Solvev(double[,] M)
        {
            int count = 0;
            int num_rows = M.GetLength(0), num_cols = M.GetLength(0);
            
            //MessageBox.Show(num_rows+" "+ num_cols);
            for (int i = 1; i< num_rows; i++)
            {
                for (int j = 2; j < num_cols; j++)
                {
                    if (M[i,j] == 1)
                    {
                        count++;
                    }
                    j++;
                }
            }
           // MessageBox.Show(count.ToString());
            return new double[] { };
        }
        private static void PrintArr(double[,] arr)
        {
            string s = "";
            int num_rows = arr.GetLength(0), num_cols = arr.GetLength(1);
            for (int i = 0; i < num_rows; i++)
            {
                for (int j = 0; j < num_cols; j++)
                {
                    s += arr[i, j].ToString() + " ";
                }
                s += "\r\n";
            }
            MessageBox.Show(s);
        }
            public static double[,] Solve(double[,] M)
        {
                const double tiny = 0.00001;
                
                // Build the augmented matrix.
                // The values num_rows and num_cols are the number of rows
                // and columns in the matrix, not the augmented matrix.
                int num_rows = M.GetLength(0), num_cols = M.GetLength(0);
            
                double[,] arr = M;
                double[,] orig_arr = M;
               
            // Display the initial arrays.
           

            // Start solving.
            for (int r = 0; r < num_rows - 1; r++)
                {
                    // Zero out all entries in column r after this row.
                    // See if this row has a non-zero entry in column r.
                    if (Math.Abs(arr[r, r]) < tiny)
                    {
                        // Too close to zero. Try to swap with a later row.
                        for (int r2 = r + 1; r2 < num_rows; r2++)
                        {
                            if (Math.Abs(arr[r2, r]) > tiny)
                            {
                                // This row will work. Swap them.
                                for (int c = 0; c <= num_cols; c++)
                                {
                                    double tmp = arr[r, c];
                                    arr[r, c] = arr[r2, c];
                                    arr[r2, c] = tmp;
                                }
                                break;
                            }
                        }
                    }
       
                // If this row has a non-zero entry in column r, use it.
                if (Math.Abs(arr[r, r]) > tiny)
                    {
                        // Zero out this column in later rows.
                        for (int r2 = r + 1; r2 < num_rows; r2++)
                        {
                            double factor = -arr[r2, r] / arr[r, r];
                            for (int c = r; c <= num_cols; c++)
                            {
                                arr[r2, c] = arr[r2, c] + factor * arr[r, c];
                            }
                        }
                    }
                }

            // Display the upper-triangular array.

            // See if we have a solution.
            if (arr[num_rows - 1, num_cols - 1] == 0)
            {
                // We have no solution.
                // See if all of the entries in this row are 0.
                bool all_zeros = true;
                for (int c = 0; c <= num_cols + 1; c++)
                {
                    if (arr[num_rows - 1, c] != 0)
                    {
                        all_zeros = false;
                        break;
                    }
                }
                if (all_zeros)
                {
                    throw new Exception("The solution is not unique");
                }
                else
                {
                    throw new Exception("There is no solution");
                }
            }
            else
            {
                // Backsolve.

                //double[] tmpArr = new double[num_rows - 1];
                // PrintArr(arr);
                // MessageBox.Show(num_rows + " " + num_cols+" "+arr.GetLength(1));
                int countRows = 0;
                for (int i = 1; i < num_rows; i++)
                {
                    int count = 0;
                    for (int j = 0; j < num_rows + 2; j++)
                    {
                        if (arr[i, j] != 0)
                            count++;
                    }

                    if (count <= 2)
                    {
                        // MessageBox.Show(i.ToString());
                        countRows++;
                    }
                }

                int[,] cof = Shuffle(countRows);
                //MessageBox.Show(sb.ToString());
                double[,] resultArr = new double[num_rows, countRows];
                for (int i = 0; i < countRows; i++)
                {
                    for (int r = num_rows - 1, ind = 0; ind < countRows; ind++, r--)
                    {
                        arr[r, num_cols + 1] = cof[i, ind];
                        resultArr[r, i] = cof[i, ind];
                    }
                    for (int r = num_rows - 1 - countRows, ind = 0; r >= 1; ind++, r--)
                    {
                        double tmp = arr[r, num_cols];
                        for (int r2 = r + 1; r2 < num_rows; r2++)
                        {
                            tmp -= arr[r, r2] * arr[r2, num_cols + 1];
                        }

                        arr[r, num_cols + 1] = tmp / arr[r, r];
                        resultArr[r, i] = tmp / arr[r, r];

                    }

                }

                return resultArr;
            }
        }
    }
}
