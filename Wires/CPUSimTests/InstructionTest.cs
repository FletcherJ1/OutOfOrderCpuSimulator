using Microsoft.VisualStudio.TestTools.UnitTesting;
using OutOfOrderCpuSimulator;
using System;
using System.Linq;

namespace CPUSimTests
{
    [TestClass]
    public class InstructionTest
    {
        [TestMethod]
        public void InstructionInputDependenciesEntryExists()
        {
            // Check all instructions have a dependency entry in the operand dependancy hashmap
            var values = Enum.GetValues(typeof(OpCodes.Op)).Cast<OpCodes.Op>();
            foreach (OpCodes.Op o in values) 
            {
                OpCodes.GetSrc1Dependency((byte)o);
            }
        }
    }
}
