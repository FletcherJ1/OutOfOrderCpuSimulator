using System;
using System.Collections.Generic;
using System.Text;

namespace OutOfOrderCpuSimulator
{
    class FreeList
    {
        private Queue<int> UnusedRegisters;

        /// <summary>
        /// Creates a list to maintain all the freely available physical registers
        /// </summary>
        /// <param name="physicalRegisterCount">Number of physical registers available</param>
        public FreeList(int physicalRegisterCount)
        {
            UnusedRegisters = new Queue<int>(physicalRegisterCount);
            // All logical registers start out un-used.
            for (int i = 0;i < physicalRegisterCount; i++)
            {
                UnusedRegisters.Enqueue(i); // i = logical register id.
            }
        }

        /// <summary>
        /// Checks if some physical register is freely available for use.
        /// </summary>
        /// <returns>Some physical registers are free to use</returns>
        public bool PhysicalRegisterAvailable()
        {
            return this.UnusedRegisters.Count > 0;
        }

        /// <summary>
        /// Add an unused register to the free list.
        /// </summary>
        /// <param name="dst">Physical register id</param>
        public void Add(char dst)
        {
            this.UnusedRegisters.Enqueue(dst);
        }

        /// <summary>
        /// Retreives the identifier for an available logical register.
        /// </summary>
        /// <returns>Logical register id</returns>
        public int GetUnusedRegister()
        {
            return this.UnusedRegisters.Dequeue();
        }
    }
}
