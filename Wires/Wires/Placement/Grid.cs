using System;
using System.Collections.Generic;
using System.Text;

namespace Wires.Placement
{

    /*
     * A few options for ASIC design:
     *  - Standard cell design (row for routing, row for cells) a cell is some manufacturer IP (Nand, Nor, Xor, Latch, etc)
     *      Channel routing
     *  - Gate array (spaced 2D grid of one type of gate that gets routed between)
     *  - Full custom (aka sea-of-gates) allows for placement of gates and wires anywhere. Maximum density and performance.
     *      - Requires sea-of-gates routing and placement algorithm, dense but difficult.
     */

    class Grid
    {
        private Gate[] G;
        private int W;
        private int H;
        private Random R;
        
        public Grid(int width, int height)
        {
            this.G = new Gate[width * height];
            this.W = width;
            this.H = height;
            this.R = new Random();
        }

        public int FindGate(Gate g)
        {
            return g.X + g.Y * W;
        }

        public int GetWidth()
        {
            return this.W;
        }

        public int GetHeight()
        {
            return this.H;
        }

        public Gate GetCell(int x, int y)
        {
            if (x >= W || y >= H || x < 0 || y < 0) 
                return null;
            return this.G[x + y * W];
        }

        public bool PlaceableCell(int x, int y)
        {
            if (x < 0 || y < 0 || x >= W || y >= H) 
                return false;
            bool c = GetCell(x, y) == null;
            return c;
        }

        public Gate GetCell(int i)
        {
            return this.G[i];
        }

        public void SetCell(int i, Gate g)
        {
            this.G[i] = g;
            if (g != null)
            {
                g.X = i % W;
                g.Y = i / W;
            }
        }

        public void PlaceGateAt(Gate g, int x, int y)
        {
            if (PlaceableCell(x, y))
            {
                SetCell(x + y * W, g);
            } else
            {
                throw new Exception("Placing gate at unplaceable location");
            }
        }

        public void PlaceGates(params Gate[] gates)
        {
            for (int i = 0;i < gates.Length;i++)
            {
                // Random, unpopulated location
                // Constraint: adjacent cells cannot contain another cell
                while (true)
                {
                    int x = R.Next(0, W);
                    int y = R.Next(0, H);
                    if (G[x + y * W] != null) continue;

                    if (PlaceableCell(x,y))
                    {
                        this.SetCell(x + y * W, gates[i]);
                        break;
                    }
                }
            }
        }
    }
}
