using System;
using System.Collections.Generic;
using System.Text;

namespace OutOfOrderCpuSimulator
{
    class RegisterFile
    {
        struct Reg
        {
            public UInt32 Value;
            public bool Ready;
        }

        Reg[] Regs;

        public RegisterFile(int archRegCount)
        {
            Regs = new Reg[archRegCount];
            for (int i = 0;i < Regs.Length;i++)
            {
                Regs[i].Value = 0;
                // All registers are ready to begin with.
                Regs[i].Ready = true;
            }
        }

        public UInt32 Get(int rid)
        {
            return this.Regs[rid].Value;
        }

        public bool IsReady(int rid)
        {
            return this.Regs[rid].Ready;
        }

        public void SetReady(int rid, bool v)
        {
            this.Regs[rid].Ready = v;
        }

        public void Set(int rid, UInt32 v)
        {
            this.Regs[rid].Value = v;
        }
    }
}
