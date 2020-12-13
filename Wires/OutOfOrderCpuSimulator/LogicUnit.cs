using System;
using System.Collections.Generic;
using System.Text;

namespace OutOfOrderCpuSimulator
{
    class LogicUnit : FunctionalUnit
    {
        private Dictionary<char, int> OpCycleCost = new Dictionary<char, int>()
        {
            { (char)63, 1 }, // AND
            { (char)64, 1 }, // OR
            { (char)65, 1 }, // XOR
            { (char)66, -1 }, // LSHL (Variable cycle count)
            { (char)67, -1 }, // LSHR (Variable cycle count)
        };

        public override bool AcceptsOp(char op)
        {
            return OpCycleCost.ContainsKey(op);
        }

        protected override uint GetResult()
        {
            switch (Op)
            {
                case (char)63:
                    return Src1 & Src2;
                case (char)64:
                    return Src1 | Src2;
                case (char)65:
                    return Src1 ^ Src2;
                case (char)66:
                    return Src1 << (int)Src2;
                case (char)67:
                    return Src1 >> (int)Src2;
                default:
                    throw new Exception("Logical Unit doesn't support opcode: " + (int)Op);
            }
        }

        protected override int GetCycleCount(char op, uint src1, uint src2, char dst)
        {
            if (op == 66 || op == 67) // LSHL, LSHR (variable cycle operation)
            {
                // dst = src1 >> src2
                if (src2 == 0 || src2 >= 32) // Zero result fast path
                    return 1;
                // NOTE: Dangerous cast but saved by bounds check above.
                return (int)(src2 + 1);
            }
            else
            {
                return OpCycleCost[op];
            }
        }
    }
}
