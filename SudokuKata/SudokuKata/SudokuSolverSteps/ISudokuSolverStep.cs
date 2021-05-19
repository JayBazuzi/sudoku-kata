using System;

namespace SudokuKata
{
    internal interface ISudokuSolverStep
    {
        ChangesMadeStates Do(Random rng, SudokuBoard puzzle);
    }
}