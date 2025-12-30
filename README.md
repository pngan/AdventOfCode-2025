# Advent Of Code 2025

C# solutions to the [Advent Of Code 2025](https://adventofcode.com/) Puzzles.

## Get input files
- The convenience script `Get-AdventOfCodeInput.ps1` can be used to download puzzle inputs, using the Advent of Code API.
- Because the script is not digitally signed, you must bypass Windows security policy using
```powershell
Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass
```
An example to get the input files for Year 2025 and Day 1 is:
```powershell
 .\Get-AdventOfCodeInput.ps1 -Password <password> -Cookie <cookie> -Year 2025 -Day 1
```
 - The cookie required by this script can be obtained [as follows](https://github.com/GreenLightning/advent-of-code-downloader?tab=readme-ov-file#how-do-i-get-my-session-cookie)
<!-- TIMINGS START -->

## Timings

| Day | Name | Type | Part 1 (ms) | Part 2 (ms) |
|---:|---|---|---:|---:|
| [01](code/day01.cs) | [Secret Entrance](https://adventofcode.com/2025/day/1) | Parsing | 0 | 12 |
| [02](code/day02.cs) | [Gift Shop](https://adventofcode.com/2025/day/2) | Parsing | 192 | 847 |
| [03](code/day03.cs) | [Lobby](https://adventofcode.com/2025/day/3) | Grid/Map | 6 | 3 |
| [04](code/day04.cs) | [Printing Department](https://adventofcode.com/2025/day/4) | Grid/Map | 64 | 873 |
| [05](code/day05.cs) | [Cafeteria](https://adventofcode.com/2025/day/5) | Combinatorics | 4 | 10 |
| [06](code/day06.cs) | [Trash Compactor](https://adventofcode.com/2025/day/6) | Grid/Map | 3 | 18 |
| [07](code/day07.cs) | [Laboratories](https://adventofcode.com/2025/day/7) | Grid/Map | 12 | 27 |
| [08](code/day08.cs) | [Playground](https://adventofcode.com/2025/day/8) | Queue | 287 | 907 |
| [11](code/day11.cs) | [Reactor](https://adventofcode.com/2025/day/11) | Queue | 1 | 4 |
| [12](code/day12.cs) | [Christmas Tree Farm](https://adventofcode.com/2025/day/12) | Graph | 0 | 0 |

<!-- TIMINGS END -->











