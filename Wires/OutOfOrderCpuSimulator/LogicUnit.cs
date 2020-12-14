using System;
using System.Collections.Generic;
using System.Text;

namespace OutOfOrderCpuSimulator
{
    class LogicUnit : FunctionalUnit
    {
        private Dictionary<byte, int> OpCycleCost = new Dictionary<byte, int>()
        {
            { (byte)63, 1 }, // AND
            { (byte)64, 1 }, // OR
            { (byte)65, 1 }, // XOR
            { (byte)66, -1 }, // LSHL (Variable cycle count)
            { (byte)67, -1 }, // LSHR (Variable cycle count)
        };

        public override bool AcceptsOp(byte op)
        {
            return OpCycleCost.ContainsKey(op);
        }

        protected override uint GetResult()
        {
            switch (Op)
            {
                case (byte)63:
                    return Src1 & Src2;
                case (byte)64:
                    return Src1 | Src2;
                case (byte)65:
                    return Src1 ^ Src2;
                case (byte)66:
                    return Src1 << (int)Src2;
                case (byte)67:
                    return Src1 >> (int)Src2;
                default:
                    throw new Exception("Logical Unit doesn't support opcode: " + (int)Op);
            }
        }

        protected override int GetCycleCount(byte op, uint src1, uint src2, byte dst)
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
