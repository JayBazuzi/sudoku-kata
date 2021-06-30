using System;
using SudokuKata.Board;

namespace SudokuKata
{
    public interface ISudokuSolverStep
    {
        ChangesMadeStates Do(Random rng, SudokuBoard puzzle);
    }
}