using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Latin_Square_Solution
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("TESTY DLA LAITN SQUARE");
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
    }

    class LS_Solver
    {
        public const int FIRTS_IN_DOMAIN = 1;
        public Variable[,] board;
        public List<int> board_domain;
        public int solutions { get; set; }
        public int N { get; set; }
        public int nodes { get; set; }

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
        }

        public bool isDwo(Variable variable, List<Variable> dom_changes, int num)
        {
            int row = variable.row;
            int col = variable.col;

            for (int r = row + 1; r < N; r++)
            {
                if (board[r, col].my_domain.Contains(num))
                {
                    if (board[r, col].isMyDwo()) return true;
                    board[r, col].my_domain.Remove(num);
                    dom_changes.Add(board[r, col]);
                }
            }
            for (int c = col + 1; c < N; c++)
            {
                if (board[row, c].my_domain.Contains(num))
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
            solve_BackTracking();
            watch.Stop();
            Console.WriteLine($"Rozwiązania: {solutions}");
            Console.WriteLine($"Nodes: {nodes}");
            Console.WriteLine($"Czas: {((double)watch.ElapsedTicks / Stopwatch.Frequency) * 1000000000.0}");
            reset();
        }

        private void solve_BackTracking()
        {
            Variable variable = findFreeSpot();
            if (variable.isVariableOk())
            {
                foreach(int n in board_domain)
                {
                    nodes++;
                    int row = variable.row;
                    int col = variable.col;
                    if(isBoardOk(row, col, n))
                    {
                        board[row, col].number = n;
                        solve_BackTracking();
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
            solve_ForwardChecking();
            watch.Stop();
            Console.WriteLine($"Rozwiązania: {solutions}");
            Console.WriteLine($"Nodes: {nodes}");
            Console.WriteLine($"Czas: {((double)watch.ElapsedTicks / Stopwatch.Frequency) * 1000000000.0}");
            reset();
        }

        private void solve_ForwardChecking()
        {
            Variable variable = findFreeSpot();
            if (variable.isVariableOk())
            {
                int row = variable.row;
                int col = variable.col;
                foreach (int n in variable.my_domain)
                {
                    nodes++;
                    //if (isBoardOk(row, col, n))
                    //{
                        List<Variable> dom_changes = new List<Variable>();
                        bool dwo = isDwo(variable, dom_changes, n);
                        if (!dwo)
                        {
                            board[row, col].number = n;
                            solve_ForwardChecking();
                            board[row, col].number = 0;
                        }
                        foreach (Variable v in dom_changes)
                        {
                            if (!v.my_domain.Contains(n)) v.my_domain.Add(n);
                        }
                    //}
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
