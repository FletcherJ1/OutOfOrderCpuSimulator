using System;
using System.Collections.Generic;

namespace OutOfOrderCpuSimulator
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            // 32-bit instructions (4 bits per operand, 7 bit operand)
            // RISC Load Store Architecture

            // Requirements: Simulate multi-cycle operations and have USEFUL statistics.

            // Pipeline:
            // Fetch -> Decode -> Rename -> Issue -> Execute -> Write-back -> Commit

            // http://cseweb.ucsd.edu/~j2lau/cs141/week8.html#:~:text=to%20use%20a%20return%20address%20stack%2C%20we%20push,return%20instruction%20will%20return%20to%20the%20popped%20address.
            // Future Ideas: Branch Prediction, Branch Target Buffer, Return Address Stack

            // Simple benchmark ideas:
            //  - Integer Mean
            //  - Fibonacci
            //  - qsort
            //  - popcnt
            //  - data dedup algorithm (need to choose one)
            //  - hash function
            //  - sieve function
            //  - reed-solomon encoder / decoder
            //  - consistent-overhead byte stuffing encoder / decoder
            //  - low-density parity check
            //  - look at luajit benchmarks maybe?

            List<UInt16> instr = new List<UInt16>();
            for (int i = 0; i < 37; i++)
            {
                // Fibonacci iterative
                instr.Add((0 << 9) | (2 << 6) | (0 << 3) | (1 << 0)); // ADD r2, r0, r1 (R2 = R0 + R1)
                instr.Add((0 << 9) | (0 << 6) | (1 << 3) | (3 << 0)); // ADD r0, r1, r3 (R0 = R1)
                instr.Add((0 << 9) | (1 << 6) | (2 << 3) | (3 << 0)); // ADD r1, r2, r3 (R1 = R2)
                
                instr.Add((65 << 9) | (4 << 6) | (4 << 3) | (5 << 0)); // XOR r4, r4, r5 (r4 = r4 xor r5)
            }

            CPU cpu = new CPU(instr.ToArray());
            // r0 = 0, r1 = 1, r2 = 0
            // 1 1 2 3 5 8 13
            cpu.SetReg(0, 0);
            cpu.SetReg(1, 1);
            cpu.SetReg(2, 0);
            cpu.SetReg(3, 0);
            cpu.SetReg(4, 0);
            cpu.SetReg(5, 1);

            while (!cpu.FinishedExec())
            {
                cpu.Cycle();
            }

            cpu.PrintStats();
        }
    }
}
