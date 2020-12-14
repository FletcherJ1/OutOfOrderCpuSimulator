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

        private static Dictionary<Op, Tuple<bool, bool>> SrcDependencies = new Dictionary<Op, Tuple<bool, bool>>()
        {
            // Op, Src1, Src2
            { Op.LW, Tuple.Create(false, true) },
            { Op.LB, Tuple.Create(false, true) },
            { Op.LBU, Tuple.Create(false, true) },
            { Op.LH, Tuple.Create(false, true) },
            { Op.LHU, Tuple.Create(false, true) },

            { Op.SW, Tuple.Create(false, true) },
            { Op.SB, Tuple.Create(false, true) },
            { Op.SH, Tuple.Create(false, true) },

            { Op.LDI, Tuple.Create(false, false) },
            { Op.LDHI, Tuple.Create(false, false) },

            { Op.SLT, Tuple.Create(true, true) },
            { Op.SLTU, Tuple.Create(true, true) },
            { Op.SLTI, Tuple.Create(true, true) },
            { Op.SLTIU, Tuple.Create(false, true) },
            { Op.BEQ, Tuple.Create(true, true) },
            { Op.BNE, Tuple.Create(true, true) },
            { Op.BLT, Tuple.Create(true, true) },
            { Op.BGE, Tuple.Create(true, true) },
            { Op.BLTU, Tuple.Create(true, true) },
            { Op.BGEU, Tuple.Create(true, true) },
            { Op.JAL, Tuple.Create(false, false) },
            { Op.JALR, Tuple.Create(false, true) },
            { Op.J, Tuple.Create(false, false) },
            { Op.JR, Tuple.Create(false, true) },

            { Op.ADD, Tuple.Create(true, true) },
            { Op.ADDI, Tuple.Create(true, false) },
            { Op.SUB, Tuple.Create(true, true) },
            { Op.SUBI, Tuple.Create(true, false) },
            { Op.MUL, Tuple.Create(true, true) },
            { Op.MULH, Tuple.Create(true, true) },
            { Op.MULHSU, Tuple.Create(true, true) },
            { Op.MULHU, Tuple.Create(true, true) },
            { Op.DIV, Tuple.Create(true, true) },
            { Op.DIVU, Tuple.Create(true, true) },
            { Op.REM, Tuple.Create(true, true) },
            { Op.REMU, Tuple.Create(true, true) },

            { Op.AND, Tuple.Create(true, true) },
            { Op.OR, Tuple.Create(true, true) },
            { Op.XOR, Tuple.Create(true, true) },
            { Op.ANDI, Tuple.Create(true, false) },
            { Op.ORI, Tuple.Create(true, false) },
            { Op.XORI, Tuple.Create(true, false) },
            { Op.SRL, Tuple.Create(true, true) },
            { Op.SRLI, Tuple.Create(true, false) },
            { Op.SLL, Tuple.Create(true, true) },
            { Op.SLLI, Tuple.Create(true, false) },
            { Op.SRAI, Tuple.Create(true, false) },
            { Op.SRA, Tuple.Create(true, true) },
        };

        public static bool GetSrc2Dependency(byte op)
        {
            return SrcDependencies[(Op)op].Item1;
        }

        public static bool GetSrc1Dependency(byte op)
        {
            return SrcDependencies[(Op)op].Item2;           
        }
    }
}
