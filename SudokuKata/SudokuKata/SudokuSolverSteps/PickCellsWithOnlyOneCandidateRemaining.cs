using System;
using System.Linq;

namespace SudokuKata
{
    internal class PickCellsWithOnlyOneCandidateRemaining : ISudokuSolverStep
    {
        public ChangesMadeStates Do(Random rng, SudokuBoard puzzle)
        {
            var singleCandidateIndices = puzzle.GetCandidates().GetCellsWithOnlyOneCandidateRemaining();

            var skip = singleCandidateIndices.Length == 0 ? 0 : rng.Next(singleCandidateIndices.Length);
            var cell = singleCandidateIndices.Skip(skip).FirstOrDefault();
            if (cell == null)
            {
                return ChangesMadeStates.None;
            }

            puzzle.SetValue(cell.Row, cell.Col, cell.Value);

            Console.WriteLine("({0}, {1}) can only contain {2}.", cell.Row + 1, cell.Col + 1, cell.Value);

            return new ChangesMadeStates {CellChanged = true};
        }
    }
}