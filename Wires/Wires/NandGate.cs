using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Wires
{
    class NandGate : Gate
    {
        public NandGate() : base(Color.Green)
        {
        }

        public override bool Evaluate()
        {
            bool o = false;
            foreach (Gate g in In) {
                if (g.Evaluate() == false)
                {
                    o = true;
                }
            }
            return o;
        }
    }
}
