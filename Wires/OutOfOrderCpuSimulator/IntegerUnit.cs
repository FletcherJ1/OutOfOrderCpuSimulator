using System;
using System.Collections.Generic;
using System.Text;

namespace OutOfOrderCpuSimulator
{
    class IntegerUnit : FunctionalUnit
    {
        private Dictionary<char, int> OpCycleCost = new Dictionary<char, int>()
        {
            // Technically cycle count is the value in here + 1 due to the result needing
            // to be captured by the write-back stage in the next cycle. The result could be buffered
            { (char)0, 1 }, // ADD
            { (char)1, 1 }, // SUB
            { (char)2, 6 }, // MUL
            { (char)3, 15 }, // DIV
            { (char)4, 15 }, // MOD
        };

        public override bool AcceptsOp(char op)
        {
            return OpCycleCost.ContainsKey(op);
        }

        protected override uint GetResult()
        {
            switch (this.Op)
            {
                case (char)0:
                    return Src1 + Src2;
                case (char)1:
                    return Src1 - Src2;
                case (char)2:
                    return Src1 * Src2;
                case (char)3:
                    return Src1 / Src2;
                case (char)4:
                    return Src1 % Src2;
                default:
                    throw new Exception("Invalid instruction for IntegerUnit op=" + (int)this.Op);
            }
        }

        protected override int GetCycleCount(char op, uint src1, uint src2, char dst)
        {
            // Could depend on operands for cycle count
            return OpCycleCost[op];
        }
    }
}
