using System;
using System.Collections.Generic;
using System.Text;

namespace OutOfOrderCpuSimulator
{
    class BranchUnit : FunctionalUnit
    {
        private Dictionary<byte, int> OpCycleCost = new Dictionary<byte, int>()
        {
            { (byte)0xF, 3 },
            { (byte)0x10, 3 },
            { (byte)0x11, 3 },
            { (byte)0x12, 3 },
            { (byte)0x13, 3 },
            { (byte)0x14, 3 },
            { (byte)0x15, 2 },
            { (byte)0x16, 3 },
            { (byte)0x17, 1 },
            { (byte)0x18, 2 },
        };

        public override bool AcceptsOp(byte op)
        {
            return OpCycleCost.ContainsKey(op);
        }

        protected override uint GetResult()
        {
            switch (this.Op)
            {
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
