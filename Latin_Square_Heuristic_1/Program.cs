using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//SPIRAL OD ŚRODKA
namespace Latin_Square_Heuristic_1
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("TESTY DLA LAITN SQUARE - heurystyka 1");
            Console.Write("Dla jakiego N chcesz przeprowadzić testy?\nN = ");
            string N = Console.ReadLine();

            LS_Solver ls = new LS_Solver(Int32.Parse(N));
            ls.solve_BT();
            ls.solve_FC();
        }
    }

    class Variable
    {
        public const int EMPTY = 0;
        public const int NO_OK = -1;
        public int row { get; set; }
        public int col { get; set; }
        public int number { get; set; }
        public List<int> my_domain;

        public Variable()
        {
            row = NO_OK;
            col = NO_OK;
        }

        public Variable(int new_row, int new_col, List<int> new_domain)
        {
            row = new_row;
            col = new_col;
            my_domain = new_domain;
            my_domain = new_domain.Select(d => d).ToList();
            number = EMPTY;
        }

        public bool isMyDwo()
        {
            return my_domain.Count == 0;
        }

        public bool isVariableOk()
        {
            return row != NO_OK && col != NO_OK;
        }

        public override string ToString()
        {
            return "( ROW = " + row.ToString() + "; COL = " + col.ToString() + " )";
        }
    }

    class LS_Solver
    {
        public const int FIRTS_IN_DOMAIN = 1;
        public Variable[,] board;
        public List<int> board_domain;
        public int solutions { get; set; }
        public int N { get; set; }
        public int nodes { get; set; }
        public List<Variable> all_variables;

        public LS_Solver(int new_N)
        {
            N = new_N;
            solutions = 0;
            nodes = 0;
            initDomain();
        }

        public void reset()
        {
            solutions = 0;
            nodes = 0;
        }

        public void initDomain()
        {
            board_domain = new List<int>();

            for (int d = FIRTS_IN_DOMAIN; d <= N; d++)
            {
                board_domain.Add(d);
            }
        }

        public void initBoard()
        {
            board = new Variable[N, N];
            for (int row = 0; row < N; row++)
            {
                for (int col = 0; col < N; col++)
                {
                    board[row, col] = new Variable(row, col, board_domain);
                }
            }
            all_variables = new List<Variable>();
            spiralVariables();
        }

        public void spiralVariables()
        {
            int r = 0, c = 0;
            int row_end = N, col_end = N;

            while (r < row_end && c < col_end)
            {
                for (int i = c; i < col_end; i++)
                {
                    all_variables.Add(board[r, i]);
                }
                r++;

                for (int i = r; i < row_end; i++)
                {
                    all_variables.Add(board[i, col_end - 1]);
                }
                col_end--;

                for (int i = col_end - 1; i >= c; i--)
                {
                    all_variables.Add(board[row_end - 1, i]);
                }
                row_end--;

               for (int i = row_end - 1; i >= r; i--)
               {
                    all_variables.Add(board[i, c]);
               }
               c++;
            }
            all_variables.Reverse();
        }

        public bool isBoardOk(int row, int col, int number)
        {
            for (int c = 0; c < N; c++)
            {
                if (board[row, c].number == number) return false;
            }
            for (int r = 0; r < N; r++)
            {
                if (board[r, col].number == number) return false;
            }
            return true;
        }

        public Variable findFreeSpot()
        {
            for (int r = 0; r < N; r++)
            {
                for (int c = 0; c < N; c++)
                {
                    if (board[r, c].number == Variable.EMPTY) return board[r, c];
                }
            }
            return new Variable();
        }

        public void printBoard()
        {
            for (int r = 0; r < N; r++)
            {
                for (int c = 0; c < N; c++)
                {
                    Console.Write($"{board[r, c].number} ");
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        public bool isDwo(Variable variable, List<Variable> dom_changes, int num)
        {
            int row = variable.row;
            int col = variable.col;

            for (int r = 0; r < N; r++)
            {
                if (r == row) r++;
                if (r >= N) break;
                if (board[r, col].my_domain.Contains(num) && r != row && board[r, col].number == Variable.EMPTY)
                {
                    if (board[r, col].isMyDwo()) return true;
                    board[r, col].my_domain.Remove(num);
                    dom_changes.Add(board[r, col]);

                }
            }
            for (int c = 0; c < N; c++)
            {
                if (c == col) c++;
                if (c >= N) break;
                if (board[row, c].my_domain.Contains(num) && board[row, c].number == Variable.EMPTY)
                {
                    if (board[row, c].isMyDwo()) return true;
                    board[row, c].my_domain.Remove(num);
                    dom_changes.Add(board[row, c]);
                }
            }

            return false;
        }

        public void solve_BT()
        {
            Console.WriteLine($"\nBT N = {N}");
            initBoard();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            solve_BackTracking(0);
            watch.Stop();
            Console.WriteLine($"Rozwiązania: {solutions}");
            Console.WriteLine($"Nodes: {nodes}");
            Console.WriteLine($"Czas: {watch.ElapsedTicks}");
            reset();
        }

        private void solve_BackTracking(int count)
        {
            if (count < all_variables.Count)
            {
                Variable variable = all_variables[count];
                foreach (int n in board_domain)
                {
                    nodes++;
                    int row = variable.row;
                    int col = variable.col;
                    if (isBoardOk(row, col, n))
                    {
                        board[row, col].number = n;
                        solve_BackTracking(count +1);
                        board[row, col].number = 0;
                    }
                }
            }
            else
            {
                solutions++;
                //printBoard();
                return;
            }
        }

        public void solve_FC()
        {
            Console.WriteLine($"\nFC N = {N}");
            initBoard();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            solve_ForwardChecking(0);
            watch.Stop();
            Console.WriteLine($"Rozwiązania: {solutions}");
            Console.WriteLine($"Nodes: {nodes}");
            Console.WriteLine($"Czas: {watch.ElapsedTicks}");
            reset();
        }

        private void solve_ForwardChecking(int count)
        {
            if (count < all_variables.Count)
            {
                Variable variable = all_variables[count];
                int row = variable.row;
                int col = variable.col;
                for(int i = 0; i < variable.my_domain.Count; i++)
                {
                    int n = variable.my_domain[i];
                    nodes++;
                    List<Variable> dom_changes = new List<Variable>();
                    bool dwo = isDwo(variable, dom_changes, n);
                    if (!dwo)
                    {
                        board[row, col].number = n;
                        solve_ForwardChecking(count + 1);
                        board[row, col].number = 0;
                    }
                    foreach (Variable v in dom_changes)
                    {
                        if (!v.my_domain.Contains(n)) v.my_domain.Add(n);
                    }
                }
            }
            else
            {
                solutions++;
                //printBoard();
                return;
            }
        }
    }
}
