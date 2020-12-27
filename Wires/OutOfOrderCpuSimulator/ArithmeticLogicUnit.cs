using System;
using System.Collections.Generic;
using System.Text;

namespace OutOfOrderCpuSimulator
{
    class ArithmeticLogicUnit : FunctionalUnit
    {
        private Dictionary<byte, int> OpCycleCost = new Dictionary<byte, int>()
        {
            { (byte)0x9, 1 }, // LDI
            { (byte)0xA, 1 }, // LDHI
            { (byte)0xB, 2 }, // SLT
            { (byte)0xC, 2 }, // SLTU
            { (byte)0xD, 2 }, // SLTI
            { (byte)0xE, 2 }, // SLTIU
            { (byte)0x19, 1 }, // ADD
            { (byte)0x1A, 1 }, // ADDI
            { (byte)0x1B, 1 }, // SUB
            { (byte)0x1C, 1 }, // SUBI
            { (byte)0x1D, 7 }, // MUL
            { (byte)0x1E, 7 }, // MULH
            { (byte)0x1F, 7 }, // MULHSU
            { (byte)0x20, 7 }, // MULHU
            { (byte)0x21, 12 }, // DIV
            { (byte)0x22, 12 }, // DIVU
            { (byte)0x23, 12 }, // REM
            { (byte)0x24, 12 }, // REMU
            { (byte)0x25, 1 }, // AND
            { (byte)0x26, 1 }, // OR
            { (byte)0x27, 1 }, // XOR
            { (byte)0x28, 1 }, // ANDI
            { (byte)0x29, 1 }, // ORI
            { (byte)0x2A, 1 }, // XORI
            { (byte)0x2B, 1 }, // SRL
            { (byte)0x2C, 1 }, // SRLI
            { (byte)0x2D, 1 }, // SLL
            { (byte)0x2E, 1 }, // SLLI
            { (byte)0x2F, 1 }, // SRAI
            { (byte)0x30, 1 }, // SRA
        };

        public ArithmeticLogicUnit(CPU c) : base(c)
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
                case OpCodes.Op.SLT:
                    return ((Int32)Src1 < (Int32)Src2) ? 1U : 0U;
                case OpCodes.Op.SLTU:
                    return (Src1 < Src2) ? 1U : 0U;
                case OpCodes.Op.SLTI:
                    return ((Int32)Src1 < (Int32)(Const >> 4)) ? 1U : 0U;
                case OpCodes.Op.SLTIU:
                    return (Src1 < (Const >> 4)) ? 1U : 0U;
                case OpCodes.Op.ADD:
                    return Src1 + Src2;
                case OpCodes.Op.ADDI:
                    return Src2 + (Const >> 4);
                case OpCodes.Op.SUB:
                    return Src1 - Src2;
                case OpCodes.Op.SUBI:
                    return Src2 - (Const >> 4);
                case OpCodes.Op.MUL:
                    return (uint)((Int32)Src1 * (Int32)Src2);
                case OpCodes.Op.MULH:
                    return (uint)(((Int64)Src1 * (Int64)Src2) >> 32);
                case OpCodes.Op.MULHU:
                    return (uint)(((UInt64)Src1 * (UInt64)Src2) >> 32);
                case OpCodes.Op.MULHSU:
                    return (uint)(((Int64)Src1 * (Int64)(UInt64)Src2) >> 32);
                case OpCodes.Op.DIV:
                    return (uint)((Int32)Src1 / (Int32)Src2);
                case OpCodes.Op.DIVU:
                    return Src1 / Src2;
                case OpCodes.Op.REM:
                    return (uint)((Int32)Src1 % (Int32)Src2);
                case OpCodes.Op.REMU:
                    return Src1 % Src2;
                case OpCodes.Op.AND:
                    return Src1 & Src2;
                case OpCodes.Op.ANDI:
                    return Src2 & (Const >> 4);
                case OpCodes.Op.OR:
                    return Src1 | Src2;
                case OpCodes.Op.ORI:
                    return Src2 | (Const >> 4);
                case OpCodes.Op.XOR:
                    return Src1 ^ Src2;
                case OpCodes.Op.XORI:
                    return Src2 ^ (Const >> 4);
                case OpCodes.Op.SRL:
                    return Src1 >> (Int32)Src2;
                case OpCodes.Op.SRLI:
                    return Src2 >> (Int32)(Const >> 4);
                case OpCodes.Op.SLL:
                    return Src1 << (Int32)Src2;
                case OpCodes.Op.SLLI:
                    return Src2 << (Int32)(Const >> 4);
                case OpCodes.Op.SRA:
                    return (uint)((Int32)Src1 >> (Int32)Src2);
                case OpCodes.Op.SRAI:
                    return (uint)((Int32)Src2 >> (Int32)(Const >> 4));
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
