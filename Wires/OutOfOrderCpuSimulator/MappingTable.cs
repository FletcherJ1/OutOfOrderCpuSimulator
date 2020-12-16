using System;
using System.Collections.Generic;
using System.Text;

namespace OutOfOrderCpuSimulator
{
    class MappingTable
    {
        int[] Map;

        /// <summary>
        /// Creates a table to map from architectural register to a physical register.
        /// Initialized to have a mapping from every architectural register to a physical register.
        /// </summary>
        /// <param name="archRegisterCount">Number of architectural registers</param>
        /// <param name="freeList">Free table reference to calculate initial mapping</param>
        public MappingTable(int archRegisterCount, FreeList freeList)
        {
            Map = new int[archRegisterCount];
            for (int i = 0;i < archRegisterCount;i++)
            {
                Map[i] = freeList.GetUnusedRegister();
            }
        }

        public MappingTable(int archRegisterCount)
        {
            Map = new int[archRegisterCount];
        }

        public void CopyFrom(MappingTable tbl)
        {
            if (this.Map.Length != tbl.Map.Length)
            {
                throw new ArgumentOutOfRangeException("Mapping tables are different size.");
            }

            for (int i = 0;i < Map.Length;i++)
            {
                this.Map[i] = tbl.Map[i];
            }
        }

        /// <summary>
        /// Gets the id of the physical register that the architectural register maps onto.
        /// </summary>
        /// <param name="archRegId">Architectural register id</param>
        /// <returns>Mapped physical register id</returns>
        public int GetPhysicalRegisterId(int archRegId)
        {
            return Map[archRegId];
        }

        /// <summary>
        /// Changes the mapping between an architectural register and its associated physical register
        /// </summary>
        /// <param name="archRegId">Architectural register id</param>
        /// <param name="physRegId">Physical register id</param>
        public void MapArchToPhys(int archRegId, int physRegId)
        {
            Map[archRegId] = physRegId;
        }
    }
}
