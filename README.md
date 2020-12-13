# Out-Of-Order CPU Simulator
I was intrigued how "modern" CPU's work and decided the best way was through making one! This is a 32-bit RISC CPU with a load-store architecture. On top of that, I wanted to learn about how the transistors are placed in silicon and how they are connected together given that some CPU's have 20+ billion transistors. Therefore, I started a very WIP placement system for a 2D grid of standard cells using MonoGame for visualization. Currently my efforts are aimed more towards the CPU simulator.

## Directory Structure
 * /CpuSpec - PDF for CPU specification
 * /Wires
 * /Wires/Wires - Cell placement and routing with visualization
 * /Wires/OutOfOrderCpuSimulator - CPU simulator
 * /Wires/SimulatedAnnealingExample - An example showing how simulated annealing works which is used in the placement algorithm for cells.

## CPU Simulator Features:
* 32-bit RISC load-store architecture
* Out of order execution (using [Tomasulo's](https://en.wikipedia.org/wiki/Tomasulo_algorithm) algorithm)
* 4-wide superscalar
* 7-stage pipeline (Fetch, Decode, Rename, Issue, Execute, Write-back, Commit)
* Single-core
* General CPU performance statistics (pipeline stalls, IPC, etc)

### CPU configurable variables with defaults
- PHYSICAL_REGISTER_COUNT = 96
- ISSUE_QUEUE_SIZE = 16
- REORDER_BUFFER_SIZE = 16
- FETCH_WIDTH = 4
- RENAME_WIDTH = 4
- ISSUE_WIDTH = 4
- WRITE_BACK_WIDTH = 4
- COMMIT_WIDTH = 4
- Execution Units (ALU=3, AGU=2, BR=1)

### TODO List:
- Build full instruction set (a small subset is currently implemented)
- Unit test each instruction
- Implement memory (including latency characteristcs)
- Benchmark some basic programs showing statistics