using System;
using System.Collections.Generic;
using System.Text;

namespace OutOfOrderCpuSimulator
{
    struct IData
    {
        public int PC;
        public UInt16 Data; // Undecoded Instruction Data
    }

    struct FE_DE_Stage {
        public Queue<IData> Instructions;
        public bool Stalled;
    }

    struct DecodedOp
    {
        public int PC;
        public char Op;
        public char Dst;
        public char Src1;
        public char Src2;
    }

    struct DE_RN_Stage
    {
        public Queue<DecodedOp> Ops;
        public bool Stalled;
    }
}
