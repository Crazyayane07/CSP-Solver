using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hetman_Solution
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("TESTY DLA HETMANOW");
            Console.Write("Dla jakiego N chcesz przeprowadzić testy?\nN = ");
            string N = Console.ReadLine();

            H_Solver hs = new H_Solver(Int32.Parse(N));
            hs.solve_BT();
            hs.solve_FC();
        }
    }

    class H_Spot
    {
        public const int NOTHING_ASSIGNED = -1;
        public int row { get; set; }
        public int col { get; set; }
        public List<int> my_domain;

        public H_Spot(int new_col, List<int> new_domain)
        {
            col = new_col;
            row = NOTHING_ASSIGNED;
            my_domain = new_domain.Select(d => d).ToList();
        }

        public bool isMyDwo()
        {
            return my_domain.Count == 0;
        }
    }

    class H_Solver
    {
        public const int FIRST_IN_DOMAIN = 0;
        public List<H_Spot> columns;
        public List<int> board_domain;
        public int N { get; set; }
        public int solutions { get; set; }
        public int nodes { get; set; }

        public H_Solver(int new_N)
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

            for (int d = FIRST_IN_DOMAIN; d < N; d++)
            {
                board_domain.Add(d);
            }
        }

        public void initColumns()
        {
            columns = new List<H_Spot>();
            for (int col = 0; col < N; col++)
            {
                columns.Add(new H_Spot(col, board_domain));
            }
        }

        public bool isHetmanPlacementOk(int row_, int col_)
        {
            for (int c = 0; c < N; c++)
            {
                if (columns[c].row == row_) return false;
            }

            for (int c = col_ - 1, r = row_ - 1; c > -1 && r > -1; c--, r--)
            {
                if (columns[c].row == r) return false;
            }

            for (int c = col_ - 1, r = row_ + 1; c > -1 && r < N; c--, r++)
            {
                if (columns[c].row == r) return false;
            }

            return true;

        }

        public void printColumns()
        {
            for (int c = 0; c < N; c++)
            {
                Console.Write($"{columns[c].row} ");
            }
            Console.WriteLine();
        }

        public bool isDwo(H_Spot hs, List<List<H_Spot>> dom_changes, int row)
        {
            int col = hs.col;

            for (int c = col + 1; c < N; c++)
            {
                if (columns[col].my_domain.Contains(row))
                {
                    if (columns[c].isMyDwo()) return true;
                    if (columns[c].my_domain.Contains(row))
                    {
                        columns[c].my_domain.Remove(row);
                        dom_changes[row].Add(columns[c]);
                    }
                }
            }
            for (int c = col + 1, r = row - 1; c < N && r > -1; c++, r--)
            {
                if (columns[c].isMyDwo()) return true;
                if (columns[c].my_domain.Contains(r))
                {
                    columns[c].my_domain.Remove(r);
                    dom_changes[r].Add(columns[c]);
                }                 
            }
            for (int c = col + 1, r = row + 1; c < N && r < N; c++, r++)
            {
                if (columns[c].isMyDwo()) return true;
                if (columns[c].my_domain.Contains(r))
                {
                    columns[c].my_domain.Remove(r);
                    dom_changes[r].Add(columns[c]);
                }
            }
            return false;
        }

        public List<List<H_Spot>> createDomainChangesSave()
        {
            List<List<H_Spot>> list = new List<List<H_Spot>>();
            foreach (int row in board_domain)
            {
                list.Add(new List<H_Spot>());
            }
            return list;
        }

        public void solve_BT()
        {
            Console.WriteLine($"\nBT dla N = {N}");
            initColumns();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            solve_BackTracking(0);
            watch.Stop();
            Console.WriteLine($"Rozwiązania: {solutions}");
            Console.WriteLine($"Nodes: {nodes}");
            Console.WriteLine($"Czas: {((double)watch.ElapsedTicks / Stopwatch.Frequency) * 1000000000.0}");
            reset();
        }

        private void solve_BackTracking(int col)
        {
            if (col != N)
            {
                foreach (int r in board_domain)
                {
                    nodes++;
                    if (isHetmanPlacementOk(r, col))
                    {
                        columns[col].row = r;
                        solve_BackTracking(col + 1);
                        columns[col].row = -1;
                    }
                }
            }
            else
            {
                solutions++;
                //printColumns();
                return;
            }
        }

        public void solve_FC()
        {
            Console.WriteLine($"\nFC dla N = { N}");
            initColumns();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            solve_ForwardChecking(0);
            watch.Stop();
            Console.WriteLine($"Rozwiązania: {solutions}");
            Console.WriteLine($"Nodes: {nodes}");
            Console.WriteLine($"Czas: {((double)watch.ElapsedTicks / Stopwatch.Frequency) * 1000000000.0}");
            reset();
        }

        private void solve_ForwardChecking(int col)
        {
            if (col != N)
            {
                H_Spot hs = columns[col];
                for (int i = 0; i < hs.my_domain.Count; i++)
                {
                    nodes++;
                    int r = hs.my_domain[i];
                    List<List<H_Spot>> dom_change = createDomainChangesSave();
                        bool dwo = isDwo(hs, dom_change, r);
                        if (!dwo)
                        {
                            columns[col].row = r;
                            solve_ForwardChecking(col + 1);
                            columns[col].row = -1;
                        }
                        foreach (int row in board_domain)
                        {
                            foreach (H_Spot h in dom_change[row])
                            {
                                if (!h.my_domain.Contains(row)) h.my_domain.Add(row);
                            }
                        }
                }
            }
            else
            {
                solutions++;
                //printColumns();
                return;
            }
        }
    }
}
