
using Microsoft.Xna.Framework;

namespace Wires
{
    class ConstGate : Gate
    {
        private bool Value;

        public ConstGate(bool v) : base(Color.Red)
        {
            this.Value = v;
        }

        public bool GetValue()
        {
            return this.Value;
        }

        public override bool Evaluate()
        {
            return Value;
        }
    }
}
