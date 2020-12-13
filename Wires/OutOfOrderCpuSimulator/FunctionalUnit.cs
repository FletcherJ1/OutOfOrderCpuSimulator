using System;
using System.Collections.Generic;
using System.Text;

namespace OutOfOrderCpuSimulator
{
    abstract class FunctionalUnit
    {
        // Required cycles for the operation
        private int CycleCount = 0;
        private bool OutputReserved = false;
        private int PC;
        private char Dst;
        protected char Op;
        protected UInt32 Src1;
        protected UInt32 Src2;

        private int WorkCycles;

        private UInt32 Result;

        protected void SetCycles(int cycles)
        {
            this.CycleCount = cycles;
        }

        public bool Busy()
        {
            return CycleCount != 0 || OutputReserved;
        }

        public abstract bool AcceptsOp(char op);

        public void GiveWork(int pc, char op, UInt32 src1, UInt32 src2, char dst)
        {
            this.CycleCount = GetCycleCount(op, src1, src2, dst);
            this.OutputReserved = false;
            this.PC = pc;
            this.Src1 = src1;
            this.Src2 = src2;
            this.Op = op;
            this.Dst = dst;
        }

        protected abstract int GetCycleCount(char op, UInt32 src1, UInt32 src2, char dst);

        protected abstract UInt32 GetResult();

        public UInt32 GetOutput()
        {
            this.OutputReserved = false;
            return this.Result;
        }

        public int GetPC()
        {
            return this.PC;
        }

        public int GetWorkCycles()
        {
            return this.WorkCycles;
        }

        public char GetDst()
        {
            return this.Dst;
        }

        public bool Finished()
        {
            return this.OutputReserved;
        }

        public void Execute()
        {
            WorkCycles++;

            if (CycleCount > 0)
            {
                CycleCount--;
                if (CycleCount == 0)
                {
                    // Store the result into the functional unit output to be grabbed by the next stage.
                    this.Result = this.GetResult();
                    // Instruction finished, unit is free but waiting for output
                    OutputReserved = true;
                }
            }
        }
    }
}
