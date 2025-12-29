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