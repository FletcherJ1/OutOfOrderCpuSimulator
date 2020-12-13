using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace SimulatedAnnealingExample
{
    // Reference and thanks to: http://course.ece.cmu.edu/~ee760/760docs/lec12.pdf
    class SimulatedAnnealing
    {
        private AtomGrid G;
        private Random R;
        public float T; // Temperature

        public SimulatedAnnealing(AtomGrid g)
        {
            this.G = g;
            this.R = new Random();
            this.T = 10;
            // Too large temperature just takes longer to converge due to too much 
            // randomization allowed in the metropolis part of the annealing algorithm.
        }

        public void Anneal()
        {
            Debug.WriteLine("Starting energy: {0}", G.GetSystemEnergy());
            int iters = 0;
            while (T > 0 && iters++ < 35)
            {
                // Technically we should loop until we reach a thermal equilibrium at this temperature.
                // Just assume many iterations reach this point.
                int totalReduction = 0;
                for (int i = 0; i < G.GetSize() * G.GetSize() * 10; i++)
                {
                    int dE = AnnealStep();
                    totalReduction += dE;
                }

                // If we are still reducing E then continue, else give-in
                // Currently we just do a fixed num of steps in here.
                T = T * 0.9f; // Reduce temperature
            }
            Debug.WriteLine("Finished annealing state, final energy = {0}, T={1}", G.GetSystemEnergy(), T);
        }

        public int AnnealStep()
        {
            // 1. Get current known energy
            int energy = G.GetSystemEnergy();
            // 2. Preturb the system

            // Let's pick 2 random locations and swap them.
            int x1 = R.Next(0, G.GetSize());
            int y1 = R.Next(0, G.GetSize());
            int x2 = R.Next(0, G.GetSize());
            int y2 = R.Next(0, G.GetSize());
            int oldState1 = G.GetAtom(x1, y1);
            int oldState2 = G.GetAtom(x2, y2);
            G.SetAtom(x1, y1, oldState2);
            G.SetAtom(x2, y2, oldState1);
            int deltaE = G.GetSystemEnergy() - energy; 
            // Negative delta E is good, keep the state change.
            // This part is the metropolis algorithm.
            if (deltaE > 0)
            {
                float r = (float)R.NextDouble();

                float p = (float)Math.Pow(Math.E, -deltaE / T); // Probablity we take the perturbation.
                if (r >= p)
                {
                    // Reject the perturbation. Restore the prev state
                    G.SetAtom(x1, y1, oldState1);
                    G.SetAtom(x2, y2, oldState2);
                    return 0;
                }
            }

            return deltaE;
        }
    }
}
