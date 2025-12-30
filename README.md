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
| [01](code/day01.cs) | [Secret Entrance](https://adventofcode.com/2025/day/1) | Math | 0 | 13 |
| [02](code/day02.cs) | [Gift Shop](https://adventofcode.com/2025/day/2) | Parsing | 168 | 344 |
| [03](code/day03.cs) | [Lobby](https://adventofcode.com/2025/day/3) | Grid/Map | 3 | 2 |
| [04](code/day04.cs) | [Printing Department](https://adventofcode.com/2025/day/4) | Grid/Map | 36 | 364 |
| [05](code/day05.cs) | [Cafeteria](https://adventofcode.com/2025/day/5) | Grid/Map | 3 | 8 |
| [06](code/day06.cs) | [Trash Compactor](https://adventofcode.com/2025/day/6) | Grid/Map | 2 | 11 |
| [07](code/day07.cs) | [Laboratories](https://adventofcode.com/2025/day/7) | Graph | 8 | 19 |
| [08](code/day08.cs) | [Playground](https://adventofcode.com/2025/day/8) | Graph | 141 | 648 |
| [09](code/day09.cs) | [Movie Theater](https://adventofcode.com/2025/day/9) | Grid/Map | 4 | 228323 |
| [10](code/day10.cs) | [Factory](https://adventofcode.com/2025/day/10) | Parsing | 18 | 2028 |
| [11](code/day11.cs) | [Reactor](https://adventofcode.com/2025/day/11) | Dynamic Programming | 1 | 4 |
| [12](code/day12.cs) | [Christmas Tree Farm](https://adventofcode.com/2025/day/12) | Parsing | 0 | 0 |

<!-- TIMINGS END -->










