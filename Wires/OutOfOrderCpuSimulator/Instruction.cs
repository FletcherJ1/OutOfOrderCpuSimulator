using System;
using System.Collections.Generic;
using System.Text;

namespace OutOfOrderCpuSimulator
{
    class Instruction
    {
        public char OpCode;
        public char Src1;
        public char Src2;
        public char Dst1;

        public Instruction(char op, char src1, char src2, char dst1)
        {
            this.OpCode = op;
            this.Src1 = src1;
            this.Src2 = src2;
            this.Dst1 = dst1;
        }
    }
}
