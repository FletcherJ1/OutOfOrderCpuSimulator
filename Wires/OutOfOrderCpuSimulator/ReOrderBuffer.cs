﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace OutOfOrderCpuSimulator
{
    class ReOrderBuffer
    {
        public class Entry
        {
            public int PC;
            public int ArchDst;
            public char PhysDst;
            public UInt32 Result;
            public bool Completed;
            public bool Branch;
        }

        private Queue<Entry> Buffer;
        private int MaxSize;
        public ReOrderBuffer(int size)
        {
            this.MaxSize = size;
            this.Buffer = new Queue<Entry>(size);
        }

        public bool Full()
        {
            return this.Buffer.Count >= MaxSize;
        }

        public void AddToQueue(int pc, int archDst, char physDst)
        {
            Debug.Assert(!Full());
            // When instruction is added to queue, it is not completed.
            Buffer.Enqueue(new Entry() { PC = pc, ArchDst = archDst, Completed = false, PhysDst = physDst });
        }

        public bool HeadCompleted()
        {
            return Buffer.Peek().Completed;
        }

        public bool HasEntries()
        {
            return this.Buffer.Count > 0;
        }

        public Entry RemoveHead()
        {
            return Buffer.Dequeue();
        }

        public void MarkCompleted(int pc, UInt32 res)
        {
            foreach (var e in Buffer)
            {
                if (e.PC == pc)
                {
                    e.Completed = true;
                    e.Result = res;
                }
            }
        }
    }
}