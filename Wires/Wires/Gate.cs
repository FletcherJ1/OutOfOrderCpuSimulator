using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Wires
{
    abstract class Gate
    {
        // Input array
        protected List<Gate> In;
        public Color C
        {
            private set;
            get;
        }

        public bool Moveable
        {
            set;
            get;
        } = true;

        public int X
        {
            get;
            set;
        }

        public int Y
        {
            get;
            set;
        }
        public Gate(Color c)
        {
            this.C = c;
            In = new List<Gate>();
        }

        public List<Gate> GetInputs()
        {
            return this.In;
        }

        public void AddInputs(params Gate[] w)
        {
            In.AddRange(w);
        }

        public abstract bool Evaluate();
    }
}
