using System;
using System.Collections.Generic;
using System.Text;

namespace OutOfOrderCpuSimulator
{
    public class OpCodes
    {
        public enum Op : byte
        {
            // Load instructions
            LW = 0,
            LB = 2,
            LBU = 3,
            LH = 4,
            LHU = 5,

            // Store instructions
            SW = 6,
            SB = 7,
            SH = 8,

            // Load immediate instructions
            LDI = 9,
            LDHI = 0xA,

            // Comparison instructions
            SLT = 0xB,
            SLTU = 0xC,
            SLTI = 0xD,
            SLTIU = 0xE,
            BEQ = 0xf,
            BNE = 0x10,
            BLT = 0x11,
            BGE = 0x12,
            BLTU = 0x13,
            BGEU = 0x14,
            JAL = 0x15,
            JALR = 0x16,
            J = 0x17,
            JR = 0x18,

            // Arithmetic Instructions
            ADD = 0x19,
            ADDI = 0x1A,
            SUB = 0x1B,
            SUBI = 0x1C,
            MUL = 0x1D,
            MULH = 0x1E,
            MULHSU = 0x1F,
            MULHU = 0x20,
            DIV = 0x21,
            DIVU = 0x22,
            REM = 0x23,
            REMU = 0x24,

            // Logic instructions
            AND = 0x25,
            OR = 0x26,
            XOR = 0x27,
            ANDI = 0x28,
            ORI = 0x29,
            XORI = 0x2A,
            SRL = 0x2B,
            SRLI = 0x2C,
            SLL = 0x2D,
            SLLI = 0x2E,
            SRAI = 0x2F,
            SRA = 0x30,
            // Experimental Instructions
            // ... NYI
        }

        public enum InstrFormat {
            A = 0,
            B = 1,
            C = 2,
        }

        public enum DstSource {
            Src1,
            Src2,
            Dst,
        }

        public struct OpInfo
        {
            public InstrFormat Format;
            public bool Src1Dep;
            public bool Src2Dep;
            public bool HasOutput;
            public DstSource OutputSource;
        }

        public static Dictionary<Op, OpInfo> OpData = new Dictionary<Op, OpInfo>()
        {
            // Op, Src1, Src2
            { Op.LW, new OpInfo{ Format=InstrFormat.B, HasOutput=true, OutputSource=DstSource.Src1, Src1Dep=false, Src2Dep=true } },
            { Op.LB, new OpInfo{ Format=InstrFormat.B, HasOutput=true, OutputSource=DstSource.Src1, Src1Dep=false, Src2Dep=true } },
            { Op.LBU, new OpInfo{ Format=InstrFormat.B, HasOutput=true, OutputSource=DstSource.Src1, Src1Dep=false, Src2Dep=true } },
            { Op.LH, new OpInfo{ Format=InstrFormat.B, HasOutput=true, OutputSource=DstSource.Src1, Src1Dep=false, Src2Dep=true } },
            { Op.LHU, new OpInfo{ Format=InstrFormat.B, HasOutput=true, OutputSource=DstSource.Src1, Src1Dep=false, Src2Dep=true } },

            { Op.SW, new OpInfo{ Format=InstrFormat.B, HasOutput=false, Src1Dep=true, Src2Dep=true } },
            { Op.SB, new OpInfo{ Format=InstrFormat.B, HasOutput=false, Src1Dep=true, Src2Dep=true } },
            { Op.SH, new OpInfo{ Format=InstrFormat.B, HasOutput=false, Src1Dep=true, Src2Dep=true } },

            { Op.LDI, new OpInfo{ Format=InstrFormat.C, HasOutput=true, OutputSource=DstSource.Src2, Src1Dep=false, Src2Dep=false } },
            { Op.LDHI, new OpInfo{ Format=InstrFormat.C, HasOutput=true, OutputSource=DstSource.Src2, Src1Dep=false, Src2Dep=false } },

            { Op.SLT, new OpInfo{ Format=InstrFormat.A, HasOutput=true, OutputSource=DstSource.Dst, Src1Dep=true, Src2Dep=true } },
            { Op.SLTU, new OpInfo{ Format=InstrFormat.A, HasOutput=true, OutputSource=DstSource.Dst, Src1Dep=true, Src2Dep=true } },
            { Op.SLTI, new OpInfo{ Format=InstrFormat.B, HasOutput=true, OutputSource=DstSource.Src1, Src1Dep=false, Src2Dep=true } },
            { Op.SLTIU, new OpInfo{ Format=InstrFormat.B, HasOutput=true, OutputSource=DstSource.Src1, Src1Dep=false, Src2Dep=true } },
            { Op.BEQ, new OpInfo{ Format=InstrFormat.B, HasOutput=false, Src1Dep=true, Src2Dep=true }  },
            { Op.BNE, new OpInfo{ Format=InstrFormat.B, HasOutput=false, Src1Dep=true, Src2Dep=true }  },
            { Op.BLT, new OpInfo{ Format=InstrFormat.B, HasOutput=false, Src1Dep=true, Src2Dep=true }  },
            { Op.BGE, new OpInfo{ Format=InstrFormat.B, HasOutput=false, Src1Dep=true, Src2Dep=true }  },
            { Op.BLTU,new OpInfo{ Format=InstrFormat.B, HasOutput=false, Src1Dep=true, Src2Dep=true }  },
            { Op.BGEU, new OpInfo{ Format=InstrFormat.B, HasOutput=false, Src1Dep=true, Src2Dep=true }  },
            { Op.JAL, new OpInfo{ Format=InstrFormat.C, HasOutput=true, OutputSource=DstSource.Src2, Src1Dep=false, Src2Dep=false } },
            { Op.JALR, new OpInfo{ Format=InstrFormat.B, HasOutput=true, OutputSource=DstSource.Src1, Src1Dep=false, Src2Dep=true } },
            { Op.J, new OpInfo{ Format=InstrFormat.C, HasOutput=false, Src1Dep=false, Src2Dep=false } },
            { Op.JR, new OpInfo{ Format=InstrFormat.C, HasOutput=false, Src1Dep=false, Src2Dep=true } },

            { Op.ADD, new OpInfo{ Format=InstrFormat.A, HasOutput=true, OutputSource=DstSource.Dst, Src1Dep=true, Src2Dep=true } },
            { Op.ADDI, new OpInfo{ Format=InstrFormat.B, HasOutput=true, OutputSource=DstSource.Src1, Src1Dep=false, Src2Dep=true } },
            { Op.SUB, new OpInfo{ Format=InstrFormat.A, HasOutput=true, OutputSource=DstSource.Dst, Src1Dep=true, Src2Dep=true } },
            { Op.SUBI, new OpInfo{ Format=InstrFormat.B, HasOutput=true, OutputSource=DstSource.Src1, Src1Dep=false, Src2Dep=true } },
            { Op.MUL, new OpInfo{ Format=InstrFormat.A, HasOutput=true, OutputSource=DstSource.Dst, Src1Dep=true, Src2Dep=true } },
            { Op.MULH, new OpInfo{ Format=InstrFormat.A, HasOutput=true, OutputSource=DstSource.Dst, Src1Dep=true, Src2Dep=true } },
            { Op.MULHSU, new OpInfo{ Format=InstrFormat.A, HasOutput=true, OutputSource=DstSource.Dst, Src1Dep=true, Src2Dep=true } },
            { Op.MULHU, new OpInfo{ Format=InstrFormat.A, HasOutput=true, OutputSource=DstSource.Dst, Src1Dep=true, Src2Dep=true } },
            { Op.DIV, new OpInfo{ Format=InstrFormat.A, HasOutput=true, OutputSource=DstSource.Dst, Src1Dep=true, Src2Dep=true } },
            { Op.DIVU, new OpInfo{ Format=InstrFormat.A, HasOutput=true, OutputSource=DstSource.Dst, Src1Dep=true, Src2Dep=true } },
            { Op.REM, new OpInfo{ Format=InstrFormat.A, HasOutput=true, OutputSource=DstSource.Dst, Src1Dep=true, Src2Dep=true } },
            { Op.REMU, new OpInfo{ Format=InstrFormat.A, HasOutput=true, OutputSource=DstSource.Dst, Src1Dep=true, Src2Dep=true } },

            { Op.AND, new OpInfo{ Format=InstrFormat.A, HasOutput=true, OutputSource=DstSource.Dst, Src1Dep=true, Src2Dep=true } },
            { Op.OR, new OpInfo{ Format=InstrFormat.A, HasOutput=true, OutputSource=DstSource.Dst, Src1Dep=true, Src2Dep=true } },
            { Op.XOR, new OpInfo{ Format=InstrFormat.A, HasOutput=true, OutputSource=DstSource.Dst, Src1Dep=true, Src2Dep=true } },
            { Op.ANDI, new OpInfo{ Format=InstrFormat.B, HasOutput=true, OutputSource=DstSource.Src1, Src1Dep=false, Src2Dep=true } },
            { Op.ORI, new OpInfo{ Format=InstrFormat.B, HasOutput=true, OutputSource=DstSource.Src1, Src1Dep=false, Src2Dep=true } },
            { Op.XORI, new OpInfo{ Format=InstrFormat.B, HasOutput=true, OutputSource=DstSource.Src1, Src1Dep=false, Src2Dep=true } },
            { Op.SRL, new OpInfo{ Format=InstrFormat.A, HasOutput=true, OutputSource=DstSource.Dst, Src1Dep=true, Src2Dep=true } },
            { Op.SRLI, new OpInfo{ Format=InstrFormat.B, HasOutput=true, OutputSource=DstSource.Src1, Src1Dep=false, Src2Dep=true } },
            { Op.SLL, new OpInfo{ Format=InstrFormat.A, HasOutput=true, OutputSource=DstSource.Dst, Src1Dep=true, Src2Dep=true } },
            { Op.SLLI, new OpInfo{ Format=InstrFormat.B, HasOutput=true, OutputSource=DstSource.Src1, Src1Dep=false, Src2Dep=true } },
            { Op.SRAI, new OpInfo{ Format=InstrFormat.B, HasOutput=true, OutputSource=DstSource.Src1, Src1Dep=false, Src2Dep=true } },
            { Op.SRA, new OpInfo{ Format=InstrFormat.A, HasOutput=true, OutputSource=DstSource.Dst, Src1Dep=true, Src2Dep=true } },
        };

        public static OpInfo GetInfo(Op op)
        {
            return OpData[op];
        }
    }
}
