using System;
using System.Collections.Generic;
using System.Text;

namespace OutOfOrderCpuSimulator
{
    struct IData
    {
        public UInt32 PC;
        public UInt32 Data; // Undecoded Instruction Data
    }

    struct FE_DE_Stage {
        public Queue<IData> Instructions;
        public bool Stalled;
    }

    struct DecodedOp
    {
        public UInt32 PC;
        public byte Op; // 7 bits[31:25]
        public byte Dst; // 4 bits[8:11]
        public byte Src1; // 4 bits[4:7]
        public byte Src2; // 4 bits[0:3]
        public UInt32 Var; // 13 bits[8:24]
        public UInt32 Const; // 21 bits[4:24]
        public bool DependS1; // Instruction dependant on src1
        public bool DependS2; // Instruction dependant on src2
    }

    struct DE_RN_Stage
    {
        public Queue<DecodedOp> Ops;
        public bool Stalled;
    }
}
