using System;
using System.Collections.Generic;
using System.Text;

namespace OutOfOrderCpuSimulator
{
    class LoadStoreUnit : FunctionalUnit
    {
        private Dictionary<byte, int> OpCycleCost = new Dictionary<byte, int>()
        {
            { (byte)0x0, 1 }, // LW
            { (byte)0x2, 1 }, // LB
            { (byte)0x3, 1 }, // LBU
            { (byte)0x4, 1 }, // LH
            { (byte)0x5, 1 }, // LHU
            { (byte)0x6, 1 }, // SW
            { (byte)0x7, 1 }, // SB
            { (byte)0x8, 1 }, // SH
            { (byte)0x9, 1 }, // LDI
            { (byte)0xA, 1 }, // LDHI
        };

        public LoadStoreUnit(CPU c) : base(c)
        {

        }

        public override bool AcceptsOp(byte op)
        {
            return OpCycleCost.ContainsKey(op);
        }

        protected override uint GetResult()
        {
            switch ((OpCodes.Op)this.Op)
            {
                case OpCodes.Op.LDI:
                    return Const;
                case OpCodes.Op.LDHI:
                    return (Const & 0x7ff) << 21;
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
