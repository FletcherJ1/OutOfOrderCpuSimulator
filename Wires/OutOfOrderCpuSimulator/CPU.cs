using System;
using System.Collections.Generic;
using System.Text;

namespace OutOfOrderCpuSimulator
{
    class CPU
    {
        private const int PHYSICAL_REGISTER_COUNT = 96;
        private const int ARCHITECTURE_REGISTER_COUNT = 8; // Don't change this.
        private const int ISSUE_QUEUE_SIZE = 16;
        private const int REORDER_BUFFER_SIZE = 16;

        private MappingTable ArchToPhysTable;
        private MappingTable CommitRAT; // Same as ArchToPhysTable but only updates at commit stage.
        private FreeList PhysFreeList;
        private RegisterFile PhysRegisters;
        private RegisterFile ArchRegisters;
        private ReOrderBuffer ROB;
        private IssueQueue IQueue;

        private int Cycles = 0;
        private int RetiredInstructionCount = 0;
        private UInt32 PC = 0; // Program Counter, not surprisingly.

        private UInt16[] InstructionList; // No memory at the moment, just have a list of instructions.

        // Statistics
        private int StalledDecodeCycles = 0;
        private int StalledRenameCycles = 0;
        private int StalledFetchCycles = 0;
        private int StalledIssueCycles = 0;
        private int StalledWriteBackCycles = 0;
        private int StalledCommitCycles = 0;

        // These variables hold data between different stages in the pipeline.
        private FE_DE_Stage FE_DE; // Data passed from fetch stage to decode stage.
        private DE_RN_Stage DE_RN; // Data passed from decode stage to rename stage.
        
        // Need a reservation station per functional unit.
        private List<FunctionalUnit> FUs;

        public CPU(UInt16[] instr)
        {
            FE_DE.Instructions = new Queue<IData>();
            DE_RN.Ops = new Queue<DecodedOp>();
            this.PhysFreeList = new FreeList(PHYSICAL_REGISTER_COUNT);
            this.PhysRegisters = new RegisterFile(PHYSICAL_REGISTER_COUNT);
            this.ArchRegisters = new RegisterFile(ARCHITECTURE_REGISTER_COUNT);
            this.ArchToPhysTable = new MappingTable(ARCHITECTURE_REGISTER_COUNT, this.PhysFreeList);
            this.CommitRAT = new MappingTable(ARCHITECTURE_REGISTER_COUNT);
            this.CommitRAT.CopyFrom(this.ArchToPhysTable);
            this.InstructionList = instr;
            this.FUs = new List<FunctionalUnit>();
            this.IQueue = new IssueQueue(ISSUE_QUEUE_SIZE);
            this.ROB = new ReOrderBuffer(REORDER_BUFFER_SIZE);
            this.FUs.Add(new ArithmeticLogicUnit(this));
            this.FUs.Add(new ArithmeticLogicUnit(this));
            this.FUs.Add(new ArithmeticLogicUnit(this));
            this.FUs.Add(new ArithmeticLogicUnit(this));
            this.FUs.Add(new BranchUnit(this));
            this.FUs.Add(new LoadStoreUnit(this));
        }

        public void SetReg(int r, UInt32 v)
        {
            this.PhysRegisters.Set(r, v);
        }

        public bool FinishedExec()
        {
            // Wait until ROB buffer is empty
            return this.ROB.HasEntries() == false && (this.PC >> 1) >= this.InstructionList.Length && IQueue.Empty();
        }

        private float ToPercent(int top, int bottom)
        {
            return 100.0f * ((float)top / (float)bottom);
        }

        public void PrintStats()
        {
            Console.WriteLine("PC: {0}", this.PC);
            Console.WriteLine("Cycle count: {0}", this.Cycles);
            Console.WriteLine("Retired instruction count: {0}", this.RetiredInstructionCount);
            Console.WriteLine("Functional Unit Count: {0}", this.FUs.Count);
            Console.WriteLine("Pipeline Utilization: ");
            Console.WriteLine("\tFetch Usage: {0}%", 400.0f - this.ToPercent(this.StalledFetchCycles, this.Cycles));
            Console.WriteLine("\tDecode Usage: {0}%", 400.0f - this.ToPercent(this.StalledDecodeCycles, this.Cycles));
            Console.WriteLine("\tRename Usage: {0}%", 400.0f - this.ToPercent(this.StalledRenameCycles, this.Cycles));
            Console.WriteLine("\tIssue Usage: {0}%", 400.0f - this.ToPercent(this.StalledIssueCycles, this.Cycles));

            Console.WriteLine("\tFunctional Units Usage:");
            for (int i = 0;i < FUs.Count;i++)
            {
                Console.WriteLine("\t\t Unit {0}: {1}%", i, this.ToPercent(this.FUs[i].GetWorkCycles(), this.Cycles));
            }

            // TODO: This statistic is kinda useless. Could do per-port utilization statistics instead. (Cycles the ALU is doing something)
            //Console.WriteLine("\tExecute Usage: {0}", this.ToPercent(this.StalledExecuteCycles, this.Cycles));
            Console.WriteLine("\tWriteBack Usage: {0}", 100.0f - this.ToPercent(this.StalledWriteBackCycles, this.Cycles));
            Console.WriteLine("\tCommit Usage: {0}", 100.0f - this.ToPercent(this.StalledCommitCycles, this.Cycles));
            Console.WriteLine("IPC: {0}", (float)this.RetiredInstructionCount / (float)this.Cycles);

            for (int i = 0;i < ARCHITECTURE_REGISTER_COUNT;i++)
            {
                Console.WriteLine("A{0}: {1}", i, ArchRegisters.Get(i));
            }
        }

        private void Fetch()
        {
            if (FE_DE.Stalled)
            {
                this.StalledFetchCycles++;
                return;
            }

            int maxFetched = 0;
            while ( (PC >> 1) < InstructionList.Length && maxFetched < 4)
            {
                UInt16 i = InstructionList[PC >> 1];
                IData d;
                d.PC = PC;
                d.Data = i;
                FE_DE.Instructions.Enqueue(d);
                // Optimization: Use branch target buffer to check if this PC is a branch and modify PC here.
                // This saves a partial pipeline flush and some wasted cycles.
                PC += 2; // 2 bytes per instruction
                maxFetched++;
            }

            this.StalledFetchCycles += (4 - maxFetched);
        }

        private void Decode()
        {
            int maxDecode = 0;
            while (!DE_RN.Stalled && FE_DE.Instructions.Count > 0 && maxDecode < 4)
            {
                IData d = FE_DE.Instructions.Dequeue();
                UInt32 i = d.Data;
                DecodedOp op;
                op.PC = d.PC;
                op.Op = (byte)(i >> 25);
                op.Const = (UInt32)(i >> 4) & 0x1FFFFFU;
                op.Var = (UInt32)(i >> 8) & 0x1FFFF;
                op.Dst = (byte)((i >> 8) & 0xf);
                op.Src1 = (byte)((i >> 4) & 0xf);
                op.Src2 = (byte)((i >> 0) & 0xf);
                OpCodes.OpInfo opd = OpCodes.GetInfo((OpCodes.Op)op.Op);
                op.Meta = opd;

                DE_RN.Ops.Enqueue(op);
                // Optimization: Check op is a branch and if it is use branch prediction
                // to load in new instructions and do a partial pipeline flush.
                maxDecode++;
            }

            // Renamer is stalled for some reason, we will have to stall and propogate stall to fetch stage.
            FE_DE.Stalled = DE_RN.Stalled && FE_DE.Instructions.Count >= 4;

            this.StalledDecodeCycles += (4 - maxDecode);
        }

        private void Rename()
        {
            int maxRename = 0;
            while (PhysFreeList.PhysicalRegisterAvailable() && !this.IQueue.IsFull() && !ROB.Full() && DE_RN.Ops.Count > 0 && maxRename < 4)
            {
                DecodedOp o = DE_RN.Ops.Dequeue();
                byte op = o.Op;
                OpCodes.OpInfo opi = o.Meta;
                byte dst = 0;
                if (opi.HasOutput)
                {
                    // TODO: Don't cast this. Do it properly.
                    dst = (byte)PhysFreeList.GetUnusedRegister();
                    PhysRegisters.SetReady(dst, false);
                }

                byte src1 = (byte)ArchToPhysTable.GetPhysicalRegisterId(opi.Src1Dep ? o.Src1 : 0);
                byte src2 = (byte)ArchToPhysTable.GetPhysicalRegisterId(opi.Src2Dep ? o.Src2 : 0);
                bool rd1 = opi.Src1Dep ? PhysRegisters.IsReady(src1) : true;
                bool rd2 = opi.Src2Dep ? PhysRegisters.IsReady(src2) : true;

                if (opi.HasOutput)
                {
                    byte archReg = 0;
                    switch (opi.OutputSource)
                    {
                        case OpCodes.DstSource.Dst:
                            archReg = o.Dst;
                            break;
                        case OpCodes.DstSource.Src1:
                            archReg = o.Src1;
                            break;
                        case OpCodes.DstSource.Src2:
                            archReg = o.Src2;
                            break;
                    }
                    ArchToPhysTable.MapArchToPhys(archReg, dst);
                    // Redirect dst register to the correct destination.
                    o.Dst = archReg;
                }
                // Resulting register of this operation should be declared not ready until it has executed.
                Console.WriteLine("Rename: rd1={0}, rd2={1}, src1={2}, src2={3}, dst={4}", rd1, rd2, (int)src1, (int)src2, (int)dst);
                // Add entry to issue queue and re-order buffer
                this.IQueue.AddToQueue(o.PC, op, src1, src2, dst, rd1, rd2, o.Const, o.Meta);
                this.ROB.AddToQueue(o.PC, o.Dst, dst, o.Meta.HasOutput);
                maxRename++;
            }

            // Stallable by multiple resource hazards: re-order buffer full, issue queue full or lack of physical registers.
            // Therefore a stall may occur, resulting in wasted cycles. 
            this.DE_RN.Stalled = !PhysFreeList.PhysicalRegisterAvailable() || this.IQueue.IsFull() || ROB.Full() || DE_RN.Ops.Count >= 4;

            this.StalledRenameCycles += (4 - maxRename);
        }

        private void Issue()
        {
            bool done = false;
            bool worked = false;
            while (!done)
            {
                // TODO: Optimization: Multiple instructions may be ready and should all be checked.
                (var e, int i) = this.IQueue.InstructionIsReady();
                if (e != null)
                {
                    // Check if a functional unit is available for use
                    foreach (FunctionalUnit u in FUs)
                    {
                        if (u.AcceptsOp(e.Op) && !u.Busy())
                        {
                            // Found a free FU that supports the operation and isn't currently in use.
                            // Remove instruction entry from issue queue
                            this.IQueue.RemoveEntry(i);
                            Console.WriteLine("Issue: PC={0}, Op={1}, l{2}={3}, l{4}={5}, Dst=l{6}", e.PC, (int)e.Op, (int)e.Src1, PhysRegisters.Get(e.Src1), (int)e.Src2, PhysRegisters.Get(e.Src2), (int)e.Dst);
                            // Lookup register values from physical register file and place operation into functional unit.

                            int s1 = e.Meta.Src1Dep ? e.Src1 : 0;
                            int s2 = e.Meta.Src2Dep ? e.Src2 : 0;

                            u.GiveWork(e.PC, e.Op, PhysRegisters.Get(s1), PhysRegisters.Get(s2), e.Dst, e.Const);
                            worked = true;
                            goto more;
                        }
                    }

                    // No unit found, wasted cycle but don't stall.
                    StalledIssueCycles++;
                    done = true;
                }
                else
                {
                    // Don't stall rename stage because the issue queue can keep filling up.
                    if (!worked)
                        StalledIssueCycles++;
                    done = true;
                }

            more: continue;
            }
        }

        private void Execute()
        {
            // Execute single cycle for all functional units.
            for (int i = 0;i < FUs.Count;i++)
            {
                var u = FUs[i];
                if (u.Busy())
                {
                    u.Execute();
                }
            }
        }

        private void WriteBack()
        {
            bool worked = false;
            for (int i = 0;i < FUs.Count;i++)
            {
                var u = FUs[i];
                if (u.Finished())
                {
                    UInt32 res = u.GetOutput();
                    byte dst = u.GetDst();
                    UInt32 pc = u.GetPC();
                    // Update ROB with result, mark instruction as completed in ROB and broadcast result to issue queue
                    // Update phys register with result
                    ROB.MarkCompleted(pc, res);
                    PhysRegisters.Set(dst, res);
                    // Say the mapping is ready for use. The result is now in the physical register file.
                    PhysRegisters.SetReady(dst, true);
                    IQueue.BroadcastTag(dst);
                    Console.WriteLine("Instruction finished: r{0}={1}", (int)dst, res);
                    // return; // This only allows single instruction to be captured from FUs at a time
                    worked = true;
                }
            }

            if (!worked)
                StalledWriteBackCycles++;
        }

        private void Commit()
        {
            // This checks if the head of the Re-order buffer has finished executing
            // If so, remove it and write the result to the associated architecture register.
            // This stage ensures that the CPU execution looks like it is in-order.

            // While versus if -> Multiple-instruction commit versus single instruction commit
            bool used = false;
            while (this.ROB.HasEntries() && this.ROB.HeadCompleted())
            {
                var head = this.ROB.RemoveHead();
                if (head.DstOutput)
                {
                    // Update architectural register and free the physical register
                    this.ArchRegisters.Set(head.ArchDst, head.Result);
                    this.PhysFreeList.Add(head.PhysDst);
                    // Commit RAT is used for re-winding state when a branch miss occurs.
                    this.CommitRAT.MapArchToPhys(head.ArchDst, head.PhysDst);
                }

                if (head.Branch)
                {
                    BranchReplay();
                }
                this.RetiredInstructionCount++;
                used = true;
            }
            
            if (!used)
            {
                StalledCommitCycles++;
            }
        }

        public void BranchReplay()
        {
            // Move retire RAT into active RAT
            // Flush issue queue and pipeline.
            // Change PC?
        }

        public void Cycle()
        {
            Commit();
            WriteBack();
            Execute();
            Issue();
            Rename();
            Decode();
            Fetch();
            // Fetch -> Decode -> Rename -> Issue -> Execute -> Write-back -> Commit
            Cycles++;
        }
    }
}
