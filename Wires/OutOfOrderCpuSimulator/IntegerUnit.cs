using System;
using System.Collections.Generic;
using System.Text;

namespace OutOfOrderCpuSimulator
{
    class IntegerUnit : FunctionalUnit
    {
        private Dictionary<byte, int> OpCycleCost = new Dictionary<byte, int>()
        {
            // Technically cycle count is the value in here + 1 due to the result needing
            // to be captured by the write-back stage in the next cycle. The result could be buffered
            { (byte)0, 1 }, // ADD
            { (byte)1, 1 }, // SUB
            { (byte)2, 6 }, // MUL
            { (byte)3, 15 }, // DIV
            { (byte)4, 15 }, // MOD
        };

        public override bool AcceptsOp(byte op)
        {
            return OpCycleCost.ContainsKey(op);
        }

        protected override uint GetResult()
        {
            switch (this.Op)
            {
                case (byte)0:
                    return Src1 + Src2;
                case (byte)1:
                    return Src1 - Src2;
                case (byte)2:
                    return Src1 * Src2;
                case (byte)3:
                    return Src1 / Src2;
                case (byte)4:
                    return Src1 % Src2;
                default:
                    throw new Exception("Invalid instruction for IntegerUnit op=" + (int)this.Op);
            }
        }

        protected override int GetCycleCount(byte op, uint src1, uint src2, byte dst)
        {
            // Could depend on operands for cycle count
            return OpCycleCost[op];
        }
    }
}
