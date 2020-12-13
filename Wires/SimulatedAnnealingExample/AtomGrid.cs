using System;
using System.Collections.Generic;
using System.Text;

namespace SimulatedAnnealingExample
{
    class AtomGrid
    {
        int[] G;
        int Size;

        public AtomGrid(int size, int colors)
        {
            Random r = new Random();
            G = new int[size * size]; // 2D grid.
            this.Size = size;
            for (int i = 0;i < G.Length;i++)
            {
                G[i] = r.Next(0, colors);
            }
        }

        public int GetSize()
        {
            return this.Size;
        }

        public void SetAtom(int x, int y, int v)
        {
            if (ValidPos(x, y))
                this.G[x + y * Size] = v;
        }

        public int GetAtom(int x, int y)
        {
            return this.G[x + y * Size];
        }

        bool ValidPos(int x, int y)
        {
            if (x >= Size || y >= Size || x < 0 || y < 0) return false;
            return true;
        }

        int GetDiff(int x, int y, int v)
        {
            if (ValidPos(x, y) == false) return 0;
            return (G[x + y * Size] == v) ? 0 : 1;
        }

        public int GetSystemEnergy()
        {
            int e = 0;
            for (int y = 0;y < Size;y++)
            {
                for (int x = 0;x < Size;x++)
                {
                    int mv = G[x + y * Size];
                    // Get difference to surrounding cells.
                    e += GetDiff(x + 1, y, mv);
                    e += GetDiff(x - 1, y, mv);
                    e += GetDiff(x, y + 1, mv);
                    e += GetDiff(x, y - 1, mv);
                    // Diagonals
                    //e += GetDiff(x + 1, y + 1, mv);
                    //e += GetDiff(x + 1, y - 1, mv);
                    //e += GetDiff(x - 1, y + 1, mv);
                    //e += GetDiff(x - 1, y - 1, mv);
                }
            }
            return e;
        }
    }
}
