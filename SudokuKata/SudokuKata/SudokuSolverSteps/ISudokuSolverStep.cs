using System;

namespace SudokuKata
{
    public interface ISudokuSolverStep
    {
        ChangesMadeStates Do(Random rng, SudokuBoard puzzle);
    }
}