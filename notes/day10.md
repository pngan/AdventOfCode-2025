# [2025 Day 10, Part 2] 

This puzzle had me stumped. I did get a second star for this day, but with help from reddit answers and AI. I learnt lots along the way.

## What I knew or worked out myself
- That the sum of the button presses having to equal the final joltage values, led to a system of equations
- That the three examples given in the description, when not using the constraint that button presses must be non-negative,  represented, respectively:
  1. an example with multiple solutions, because the equations were under-determined
  1. an example with no solution, even though the number of unknowns (buttons) was equal to the number of knowns (joltages)
  1. an example that could lead to a single solution, because the give the equations were over-constrained.
- That an under-determined system leads to multiple solutions that reside in a sub-space defined by "free variables"
- That the number of free variables was at most 3, which is potentially traversable using brute force

## What I learned from reading
- A possible approach is to traverse the space formed by the free variables: where the bounds of the traversal are defined by the constraints and joltages values, and that the solution can be found by calculating the objective (minimum sum of button presses) at each candidate combination of unknowns.
- That the free variables can be found by finding the non-pivot points of the Reduced Row Echelon Form (RREF)

## What I tried
- For the under-determined scenarios, use the `Accord.Math` numeric package to calculate the RREF and find the free variables, with view to traversing them to find the minimum solution
- Gave up when realising that I was simply using a package for the sake of getting an answer

## What I ended up doing
- If I was going to use a package, then use the `Z3` library like many others were doing.
- Go the whole way, and use AI to help to write the core of the `Z3` code.
- The only one that was mine was the parsing and the formation of the matrix of coefficients, and vector of known values.
