using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Wires.Placement
{
    class PlaceAnnealer
    {
        private List<Gate> Gates;
        private Grid G;
        private float T; // Temperature
        private Random R;

        private int StepCount = 0;
        private int MinE = int.MaxValue;

        public PlaceAnnealer(Grid g)
        {
            this.T = 20.0f;
            this.G = g;
            this.Gates = new List<Gate>();
            this.R = new Random();
            GetAllGates();
        }

        private void GetAllGates()
        {
            for (int y = 0;y < G.GetHeight();y++)
            {
                for (int x = 0;x < G.GetWidth();x++)
                {
                    if (G.GetCell(x, y) != null)
                    {
                        this.Gates.Add(G.GetCell(x, y));
                    }
                }
            }
        }

        private int GetApproxNetSize(Gate g)
        {
            // Apply Half-perimeter metric to get a guarenteed
            // lower bound for the wire length of this net.
            // For a 2-point net this is the same as the manhatten
            // distance.

            int pos = G.FindGate(g);

            int minX = pos % G.GetWidth();
            int minY = pos / G.GetWidth();
            int maxX = minX;
            int maxY = minY;

            foreach (Gate gg in g.GetInputs())
            {
                int p = G.FindGate(gg);
                int zx = p % G.GetWidth();
                int zy = p / G.GetWidth();

                minX = Math.Min(minX, zx);
                minY = Math.Min(minY, zy);

                maxX = Math.Max(maxX, zx);
                maxY = Math.Max(maxY, zy);
            }

            int wi = maxX - minX;
            int hi = maxY - minY;

            return wi + hi;
        }

        private int GetSystemEnergy()
        {
            int e = 0;
            foreach (Gate g in Gates)
            {
                e += GetApproxNetSize(g);
            }
            return e;
        }

        private bool Metropolis(int delta)
        {
            if (delta >= 0)
            {
                // Apply metropolis           
                float r = (float)R.NextDouble();
                float p = (float)Math.Pow(Math.E, -delta / T); // Probablity we take the perturbation.
                if (r >= p)
                {
                    // Revert current state to prev state
                    return false;
                }
            }
            // Keep current state.
            return true;
        }

        public void AnnealStep()
        {
            int e = GetSystemEnergy();
            int delta = 0;
            double moveType = R.NextDouble();

            if (moveType < 0.5)
            {
                // Swap nodes.
                int i1 = 0;
                int i2 = 0;

                // Don't want the same value in g1 and g2
                do
                {
                    i1 = R.Next(0, Gates.Count);
                    i2 = R.Next(0, Gates.Count);
                } while (i1 == i2 || Gates[i1].Moveable == false || Gates[i2].Moveable == false);
                Gate g1 = Gates[i1];
                Gate g2 = Gates[i2];
                int p1 = G.FindGate(Gates[i1]);
                int p2 = G.FindGate(Gates[i2]);
                // Swap the gates around
                G.SetCell(p1, g2);
                G.SetCell(p2, g1);

                delta = GetSystemEnergy() - e;

                if (!Metropolis(delta))
                {
                    // Reject the perturbation. Restore the prev state
                    G.SetCell(p1, g1);
                    G.SetCell(p2, g2);
                }

            } else
            {
                // Let's move a random node in a direction.
                int i1 = 0;

                do
                {
                    i1 = R.Next(0, Gates.Count);
                } while (Gates[i1].Moveable == false);

                Gate g1 = Gates[i1];
                int p1 = G.FindGate(g1);
                int x1 = p1 % G.GetWidth();
                int y1 = p1 / G.GetWidth();
                int dir = R.Next(0, 4); // N,E,S,W

                // Get the gates inputs and try to move towards them
                int sx = x1;
                int sy = y1;
                int c = 1;
                foreach (Gate g in g1.GetInputs())
                {
                    int gi = G.FindGate(g);
                    int xx = gi % G.GetWidth();
                    int yy = gi / G.GetWidth();
                    sx += xx;
                    sy += yy;
                    c++;
                }
                // Centroid
                sx /= c;
                sy /= c;

                // Test we can move in that direction without invalidating placement constraints (adjacency to other gates)
                //int nx = dir == 1 ? x1 + 1 : (dir == 3 ? x1 - 1 : x1);
                //int ny = dir == 0 ? y1 - 1 : (dir == 2 ? y1 + 1 : y1);
                G.SetCell(p1, null); // Remove old gate else placeable will always return false
                if (G.PlaceableCell(sx, sy))
                {
                    G.SetCell(sx + sy * G.GetWidth(), g1); // Place in new position
                    delta = GetSystemEnergy() - e;

                    if (!Metropolis(delta))
                    {
                        // Restore state
                        G.SetCell(p1, g1);
                        G.SetCell(sx + sy * G.GetWidth(), null);
                    }
                } else
                {
                    // Try to swap instead
                    Gate g2 = G.GetCell(sx + sy * G.GetWidth());
                    G.SetCell(sx + sy * G.GetWidth(), g1);
                    G.SetCell(p1, g2);

                    delta = GetSystemEnergy() - e;

                    if (!Metropolis(delta))
                    {
                        // Swap back
                        G.SetCell(sx + sy * G.GetWidth(), g2);
                        G.SetCell(p1, g1);
                    }
                }
            }

            this.StepCount++;

            /*if (GetSystemEnergy() < MinE)
            {
                MinE = GetSystemEnergy();
                this.StepCount = 0;
            }*/

            if (this.StepCount > this.Gates.Count * 100)
            {
                // T,E
                Console.WriteLine("{0},{1}", T, e + delta);
                this.StepCount = 0;
                if (T > 0.05)
                    T *= 0.95f;
            }
        }
    }
}
