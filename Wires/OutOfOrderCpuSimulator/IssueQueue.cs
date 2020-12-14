using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace OutOfOrderCpuSimulator
{
    class IssueQueue
    {
        public class Entry
        {
            public UInt32 PC;
            public byte Op;
            public byte Dst;
            public byte Src1;
            public byte Src2;
            public bool Rd1;
            public bool Rd2;
        }

        public List<Entry> IQueue;
        private int MaxQueueSize;

        public IssueQueue(int queueSize)
        {
            IQueue = new List<Entry>(queueSize);
            this.MaxQueueSize = queueSize;
        }

        public bool IsFull()
        {
            return this.IQueue.Count >= this.MaxQueueSize;
        }

        public bool Empty()
        {
            return this.IQueue.Count == 0;
        }

        public void AddToQueue(UInt32 pc, byte op, byte src1, byte src2, byte dst, bool rd1, bool rd2)
        {
            Debug.Assert(!IsFull());
            IQueue.Add(new Entry() { PC = pc, Op = op, Src1 = src1, Src2 = src2, Rd1 = rd1, Rd2 = rd2, Dst = dst });
        }

        public (Entry e, int i) InstructionIsReady()
        {
            // Any instruction where both source operands are ready and available.
            for (int i = 0;i < IQueue.Count;i++)
            {
                Entry e = IQueue[i];
                if (e.Rd1 && e.Rd2) return (e, i);
            }
            return (e: null, i: -1);
        }

        public void RemoveEntry(int idx)
        {
            this.IQueue.RemoveAt(idx);
        }

        public void BroadcastTag(byte tag)
        {
            // Check any instruction in issue queue to see if the source operands match the tag.
            // If so, it is ready to be used.
            foreach (Entry e in IQueue)
            {
                if (e.Src1 == tag) e.Rd1 = true;
                if (e.Src2 == tag) e.Rd2 = true;
            }
        }
    }
}
